using GP.Lib.Common.Cache;
using GP.Lib.Common.GeoAlgoritm;
using GP.Lib.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GP.Lib.Common
{
    public static class CommonServiceRegisteration
    {
        public static void AddCommonServices(this IServiceCollection services)
        {
            services.AddScoped<IGeoLatLong, GeoLatLong>();
            services.AddScoped<ICacheProvider, CacheProvider>();
        }
    }
}
