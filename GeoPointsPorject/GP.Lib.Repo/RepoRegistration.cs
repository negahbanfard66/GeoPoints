using Microsoft.Extensions.DependencyInjection;
using GP.Lib.Base.Interfaces.Repositories;
using GP.Lib.Repo.Repositories;

namespace GP.Lib.Repo
{
    public static class RepoRegistration
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryGeoPoints, RepositoryGeoPoints>();
            services.AddUserRepositories();
        }

        public static void AddUserRepositories(this IServiceCollection services)
        {
        }
    }
}