using GP.Lib.Base.DataLayer;
using GP.Lib.Base.ViewModel.GeoPoint;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GP.Lib.Base.Interfaces.Services
{
    public interface IServiceGeoPoints
    {
        Task<List<VmGeoPointResult>> FindGeoPointsByUserIdAsync(int userId);

        Task<List<VmGeoPointResult>> FindGeoPointsByUserNameAsync(string userName);
        Task<List<VmGeoPointResult>> GetGeoPointsAsync(Expression<Func<DbGeoPoints, bool>> conditon);
        Task<VmGeoPointResult> AddAsync(VmGeoPointAdd geoPoints);
    }
}
