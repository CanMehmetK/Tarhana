using System.Collections.ObjectModel;

namespace Multitenancy
{
    public class MultitenancyOptions
    {
        public Collection<AppTenant> Tenants { get; set; }
    }
}
