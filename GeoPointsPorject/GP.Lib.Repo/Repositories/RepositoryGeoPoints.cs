using GP.Lib.Base.DataLayer;
using GP.Lib.Base.Interfaces.Repositories;
using GP.Lib.Repo.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Lib.Repo.Repositories
{
    public class RepositoryGeoPoints : EntityFrameworkRepository<DbGeoPoints>, IRepositoryGeoPoints
    {
        public RepositoryGeoPoints(ApplicationDbContext ctx) : base(ctx)
        {
            Context = ctx;
        }

        public async Task<List<DbGeoPoints>> FindWithUserIdAsync(int userId)
        {
            var context = Context as ApplicationDbContext;
            var result = await context.GeoPoints
                .Include(t => t.User)
                .Where(t => t.User.Id == userId).ToListAsync();

            return result;
        }

        public async Task<List<DbGeoPoints>> FindWithUserNameAsync(string userName)
        {
            var context = Context as ApplicationDbContext;
            var result = await context.GeoPoints
                .Include(t => t.User)
                .Where(t => t.User.UserName == userName).ToListAsync();

            return result;
        }
    }
}
