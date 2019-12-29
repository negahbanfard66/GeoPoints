using GP.Lib.Base.DataLayer;
using GP.Lib.Base.Interfaces.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GP.Lib.Base.Interfaces.Repositories
{
    public interface IRepositoryGeoPoints : IRepository<DbGeoPoints>
    {
        Task<List<DbGeoPoints>> FindWithUserIdAsync(int userId);

        Task<List<DbGeoPoints>> FindWithUserNameAsync(string userName);
    }
}