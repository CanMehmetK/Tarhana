using AutoMapper;
using Blueshift.EntityFrameworkCore.MongoDB.Infrastructure;
using Blueshift.Identity.MongoDB;
using Ideative.Infra.CrossCutting.Identity.Data;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CustomIdentity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MongoCustomIdentity.Controllers;
using Multitenancy;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using UserManage.EF.Data;

namespace MongoCustomIdentity
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var myIdentityOptions = new IdentityOptions();
            Configuration.GetSection("IdentityOptions").Bind(myIdentityOptions);
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseMongoDb(
                    connectionString: "mongodb://localhost:27017",
                    mongoDbOptionsAction: optionsBuilder => optionsBuilder.UseDatabase("__test_identities")
                    )
                .EnableSensitiveDataLogging(true);
            })
                .AddCustomIdentity<MongoDbIdentityUser, MongoDbIdentityRole>(config =>
                {
                    //In Multitenant unique email may couse some proglems.
                    config.User.RequireUniqueEmail = myIdentityOptions.User.RequireUniqueEmail;
                    config.Lockout.MaxFailedAccessAttempts = myIdentityOptions.Lockout.MaxFailedAccessAttempts;

                    //SignIn
                    config.SignIn.RequireConfirmedEmail = myIdentityOptions.SignIn.RequireConfirmedEmail;
                    config.SignIn.RequireConfirmedPhoneNumber = myIdentityOptions.SignIn.RequireConfirmedPhoneNumber;

                    //Password
                    config.Password.RequireDigit = myIdentityOptions.Password.RequireDigit;
                    config.Password.RequiredLength = myIdentityOptions.Password.RequiredLength;
                    config.Password.RequireNonAlphanumeric = myIdentityOptions.Password.RequireNonAlphanumeric;
                    config.Password.RequireUppercase = myIdentityOptions.Password.RequireUppercase;
                    config.Password.RequireLowercase = myIdentityOptions.Password.RequireLowercase;
                    config.Password.RequiredUniqueChars = myIdentityOptions.Password.RequiredUniqueChars;
                })
                .AddEntityFrameworkMongoDbStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(o =>
                {
                    o.LoginPath = new PathString("/giris");
                    o.AccessDeniedPath = new PathString("/access-denied");
                })
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = true;
                    o.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = Configuration["Tokens:Issuer"],
                        ValidAudience = Configuration["Tokens:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"]))
                    };
                });
            services.AddMultitenancy<AppTenant, CachingMongoDbAppTenantResolver>();

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new TenantViewLocationExpander());
            });
            services.Configure<MultitenancyOptions>(Configuration.GetSection("Multitenancy"));

            services.AddResponseCaching(options =>
            {
                options.SizeLimit = 200 * 1024 * 1024;
                options.UseCaseSensitivePaths = false;
                //options.MaximumBodySize = 1024;
            });

            services.AddMvc(options =>
            {
                options.CacheProfiles.Add("Hourly", new CacheProfile()
                {
                    Duration = 60 * 60 // 1 hour
                });
                options.CacheProfiles.Add("Daily", new CacheProfile()
                {
                    Duration = 24 * 60 * 60 // 1 Day
                });
                options.CacheProfiles.Add("Weekly", new CacheProfile()
                {
                    Duration = 60 * 60 * 24 * 7 // 7 days
                });
                options.CacheProfiles.Add("Monthly", new CacheProfile()
                {
                    Duration = 60 * 60 * 24 * 7 * 30 // 30 days
                });
            });
            services.AddCors(o => o.AddPolicy("all", builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }));
            services.AddSession();
            services.Configure<AuthMessageSenderOptions>(Configuration);

            services.AddAutoMapper();

            services.AddAuthorization(options =>
            {
                //options.AddPolicy("CanWriteCustomerData", policy => policy.Requirements.Add(new ClaimRequirement("Customers", "Write")));
                //options.AddPolicy("CanRemoveCustomerData", policy => policy.Requirements.Add(new ClaimRequirement("Customers", "Remove")));
            });

            // Adding MediatR for Domain Events and Notifications
            services.AddMediatR(typeof(Startup));

            //services.BuildServiceProvider();
            // .NET Native DI Abstraction
            RegisterServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            // Add dependencies
            
        }
    }





    public static class UrlHelperExtensions
    {
        public static string currentUrl(this Microsoft.AspNetCore.Http.HttpContext Context)
        {
            return Context.Request.Scheme + "://" + Context.Request.Host;
        }
        public static string EmailConfirmationLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.ConfirmEmail),
                controller: "Account",
                values: new { userId, code },
                protocol: scheme);
        }

        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.ResetPassword),
                controller: "Account",
                values: new { userId, code },
                protocol: scheme);
        }

        public static string fromEmptyToNull(this string source)
        {
            return (string.IsNullOrEmpty(source) ? null : source);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
       



        /// <summary>
        /// Generates a fully qualified URL to an action method by using the specified action name, controller name and
        /// route values.
        /// </summary>
        /// <param name="url">The URL helper.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">The route values.</param>
        /// <returns>The absolute URL.</returns>
        public static string AbsoluteAction(
            this IUrlHelper url,
            string actionName,
            string controllerName,
            object routeValues = null) =>
            url.Action(actionName, controllerName, routeValues, url.ActionContext.HttpContext.Request.Scheme);

        /// <summary>
        /// Generates a fully qualified URL to the specified content by using the specified content path. Converts a
        /// virtual (relative) path to an application absolute path.
        /// </summary>
        /// <param name="url">The URL helper.</param>
        /// <param name="contentPath">The content path.</param>
        /// <returns>The absolute URL.</returns>
        public static string AbsoluteContent(
            this IUrlHelper url,
            string contentPath)
        {
            var request = url.ActionContext.HttpContext.Request;
            return new Uri(new Uri(request.Scheme + "://" + request.Host.Value), url.Content(contentPath)).ToString();
        }

        /// <summary>
        /// Generates a fully qualified URL to the specified route by using the route name and route values.
        /// </summary>
        /// <param name="url">The URL helper.</param>
        /// <param name="routeName">Name of the route.</param>
        /// <param name="routeValues">The route values.</param>
        /// <returns>The absolute URL.</returns>
        public static string AbsoluteRouteUrl(
            this IUrlHelper url,
            string routeName,
            object routeValues = null) =>
            url.RouteUrl(routeName, routeValues, url.ActionContext.HttpContext.Request.Scheme);
    }
    public class appsetting
    {
        public int activedays { get; set; }
    }

    public class AuthMessageSenderOptions
    {
        public string User { get; set; }
        public string Pass { get; set; }

    }
}
