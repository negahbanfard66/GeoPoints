using GP.Lib.Base.DataLayer;
using GP.Lib.Base.Interfaces.Repositories;
using GP.Lib.Base.Interfaces.Services;
using GP.Lib.Base.ViewModel.GeoPoint;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GP.Lib.Services.Mapping;

namespace GP.Lib.Services
{
    public class ServiceGeoPoints : IServiceGeoPoints
    {
        private readonly IRepositoryGeoPoints _geoPointsRepo;

        public ServiceGeoPoints(IRepositoryGeoPoints geoPointsRepo) {
            _geoPointsRepo = geoPointsRepo;
        }
        public async Task<VmGeoPointResult> AddAsync(VmGeoPointAdd geoPoints)
        {
            var data = new DbGeoPoints()
            {
                OriginLat = geoPoints.OriginLat,
                OriginLon = geoPoints.OriginLon,
                DestinationLat = geoPoints.DestinationLat,
                DestinationLon = geoPoints.DestinationLon,
                UserId = geoPoints.UserId
            };

            await _geoPointsRepo.CreateAsync(data);
            _geoPointsRepo.Save();
            return data.ToGeoPointsResultViewModel();
        }

        public async Task<List<VmGeoPointResult>> FindGeoPointsByUserIdAsync(int userId)
        {
           var result =  await _geoPointsRepo.FindWithUserIdAsync(userId);
            var response = new List<VmGeoPointResult>();
            foreach (var item in result)
            {
                response.Add(item.ToGeoPointsResultViewModel());
            }
            return response;
        }

        public async Task<List<VmGeoPointResult>> FindGeoPointsByUserNameAsync(string userName)
        {
            var result = await _geoPointsRepo.FindWithUserNameAsync(userName);
            var response = new List<VmGeoPointResult>();
            foreach (var item in result)
            {
                response.Add(item.ToGeoPointsResultViewModel());
            }
            return response;
        }

        public Task<List<VmGeoPointResult>> GetGeoPointsAsync(Expression<Func<DbGeoPoints, bool>> conditon)
        {
            throw new NotImplementedException();
        }
    }
}
