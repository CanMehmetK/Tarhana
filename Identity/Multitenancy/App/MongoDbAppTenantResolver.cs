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
    public abstract class MongoDbAppTenantResolver<TTenant> : ITenantResolver<TTenant>
    {
        protected readonly ILogger log;

        public MongoDbAppTenantResolver(ILoggerFactory loggerFactory)
        {
            Ensure.Argument.NotNull(loggerFactory, nameof(loggerFactory));
            this.log = loggerFactory.CreateLogger<MemoryCacheTenantResolver<TTenant>>();
        }

        protected virtual void DisposeTenantContext(object cacheKey, TenantContext<TTenant> tenantContext)
        {
            if (tenantContext != null)
            {
                log.LogDebug("Disposing TenantContext:{id} instance with key \"{cacheKey}\".", tenantContext.Id, cacheKey);
                tenantContext.Dispose();
            }
        }

        protected abstract string GetContextIdentifier(HttpContext context);
        protected abstract IEnumerable<string> GetTenantIdentifiers(TenantContext<TTenant> context);
        protected abstract Task<TenantContext<TTenant>> ResolveAsync(HttpContext context);

        async Task<TenantContext<TTenant>> ITenantResolver<TTenant>.ResolveAsync(HttpContext context)
        {
            Ensure.Argument.NotNull(context, nameof(context));

            TenantContext<TTenant> tenantContext;


            tenantContext = await ResolveAsync(context);

            if (tenantContext == null)
            {
                //TODO:
            }

            return tenantContext;
        }


    }

    public class DbAppTenantResolver : MongoDbAppTenantResolver<AppTenant>
    {
        private readonly IEnumerable<AppTenant> tenants;

        public DbAppTenantResolver(            
            ILoggerFactory loggerFactory
            )
            : base( loggerFactory)
        {
            //this.tenants = options.Value.Tenants;
            var client = new MongoDB.Driver.MongoClient("mongodb://localhost"); // mongodb://admin:abc123!@localhost/database      
            MongoDB.Driver.IMongoDatabase _database = client.GetDatabase("mydb");

            var collection = _database.GetCollection<AppTenant>("tenants");
            this.tenants = collection.Find(Builders<AppTenant>.Filter.Empty).ToList();
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

            var tenant = tenants.FirstOrDefault(t => t.HostNames.Any(h => h.Equals(context.Request.Host.Value.ToLower())));

            if (tenant != null) { tenantContext = new TenantContext<AppTenant>(tenant); }

            return Task.FromResult(tenantContext);
        }
        
    }
}
