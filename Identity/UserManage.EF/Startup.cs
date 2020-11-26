using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CustomIdentity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Multitenancy;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using UserManage.EF.Data;
using UserManage.EF.Models;
using UserManage.EF.Services;

namespace UserManage.EF
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
            // ===== Add DbContext ========
            services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));



            var myIdentityOptions = new IdentityOptions();
            Configuration.GetSection("IdentityOptions").Bind(myIdentityOptions);

            // ===== Add Identity ========
            // Default Tenant
            //var defaultTenant = new Tenant();
            //Configuration.GetSection("DefaultTenant").Bind(defaultTenant);
            //TODO:@kanpinar Vary by Tenant ??? is possible
            services.AddCustomIdentity<ApplicationUser, ApplicationRole>(config =>
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
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // ===== Add Jwt Authentication ========
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/giris");
                    options.AccessDeniedPath = new PathString("/access-denied");
                })
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = true;
                    o.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = Configuration["Tokens:Issuer"],
                        ValidAudience = Configuration["Tokens:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"])),
                        ClockSkew = TimeSpan.Zero // remove delay of token when expire
                    };
                });

            //services.AddMemoryCache();            
            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();

            services.AddMultitenancy<AppTenant, CachingConfigAppTenantResolver>();
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new TenantViewLocationExpander());
            });
            services.Configure<MultitenancyOptions>(Configuration.GetSection("Multitenancy"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseMultitenancy<AppTenant>();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                  name: "areas",
                  template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
