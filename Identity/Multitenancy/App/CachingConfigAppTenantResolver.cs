using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multitenancy
{
    /// <summary>
    /// From Config To Cache
    /// </summary>
    public class CachingConfigAppTenantResolver : MemoryCacheTenantResolver<AppTenant>
    {
        private readonly IEnumerable<AppTenant> tenants;

        public CachingConfigAppTenantResolver(
            IMemoryCache cache,
            ILoggerFactory loggerFactory,
            IOptions<MultitenancyOptions> options)
            : base(cache, loggerFactory)
        {
            this.tenants = options.Value.Tenants;
        }

        protected override string GetContextIdentifier(HttpContext context)
        {
            // TODO:@kanpinar Different senarios via options...
            return context.Request.Host.Value.ToLower();
        }

        protected override IEnumerable<string> GetTenantIdentifiers(TenantContext<AppTenant> context)
        {
            return context.Tenant.HostNames;
        }

        protected override Task<TenantContext<AppTenant>> ResolveAsync(HttpContext context)
        {
            TenantContext<AppTenant> tenantContext = null;

            var tenant = tenants.FirstOrDefault(t =>
                t.HostNames.Any(h => h.Equals(context.Request.Host.Value.ToLower())));

            if (tenant != null)
            {
                tenantContext = new TenantContext<AppTenant>(tenant);
            }

            return Task.FromResult(tenantContext);
        }

        protected override MemoryCacheEntryOptions CreateCacheEntryOptions()
        {
            return base.CreateCacheEntryOptions().SetSlidingExpiration(new TimeSpan(0, 1, 2));
        }
    }

    /// <summary>
    /// From MongoDb To Cache
    /// </summary>
    public class CachingMongoDbAppTenantResolver : MemoryCacheTenantResolver<AppTenant>
    {
        private readonly IEnumerable<AppTenant> tenants;

        public CachingMongoDbAppTenantResolver(
            IMemoryCache cache,
            ILoggerFactory loggerFactory,
            IOptions<MultitenancyOptions> options)
            : base(cache, loggerFactory)
        {
            //this.tenants = options.Value.Tenants;
            var client = new MongoDB.Driver.MongoClient("mongodb://localhost"); // mongodb://admin:abc123!@localhost      
            MongoDB.Driver.IMongoDatabase _database = client.GetDatabase("kiralikbul");

            var collection = _database.GetCollection<AppTenant>("tenants");
            this.tenants = collection.Find(Builders<AppTenant>.Filter.Empty).ToList();
        }

        protected override string GetContextIdentifier(HttpContext context)
        {
            log.LogDebug(context.Request.Host + context.Request.Path + context.Request.QueryString);
            
#if DEBUG 
return "example.com";
#endif
            return context.Request.Host.Value.ToLower();
        }

        protected override IEnumerable<string> GetTenantIdentifiers(TenantContext<AppTenant> context)
        {
            return context.Tenant.HostNames;
        }

        protected override Task<TenantContext<AppTenant>> ResolveAsync(HttpContext context)
        {
            TenantContext<AppTenant> tenantContext = null;
            AppTenant tenant;
#if DEBUG
            tenant = tenants.FirstOrDefault(t =>t.HostNames.Any(h => h.Equals("example.com")));
#else
 tenant = tenants.FirstOrDefault(t =>
                t.HostNames.Any(h => h.Equals(context.Request.Host.Value.ToLower().Replace("www.",""))));
#endif


            if (tenant != null)
            {
                tenantContext = new TenantContext<AppTenant>(tenant);
            }

            return Task.FromResult(tenantContext);
        }

        protected override MemoryCacheEntryOptions CreateCacheEntryOptions()
        {
            return base.CreateCacheEntryOptions().SetSlidingExpiration(new TimeSpan(0, 0, 2));
        }
    }
}
