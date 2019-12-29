using Microsoft.Extensions.DependencyInjection;
using GP.Lib.Base.Interfaces.Services;

namespace GP.Lib.Services
{
    public static class ServiceRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IServiceGeoPoints, ServiceGeoPoints>();
        }
    }
}