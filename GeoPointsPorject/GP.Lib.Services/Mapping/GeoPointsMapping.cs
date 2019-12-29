using GP.Lib.Base.DataLayer;
using GP.Lib.Base.ViewModel.GeoPoint;
using System;
using System.Collections.Generic;
using System.Text;

namespace GP.Lib.Services.Mapping
{
    public static class GeoPointsMapping
    {
        public static VmGeoPointResult ToGeoPointsResultViewModel(this DbGeoPoints geoPoints)
        {
            return new VmGeoPointResult
            {
                OriginLat = geoPoints.OriginLat,
                OriginLon = geoPoints.OriginLon,
                DestinationLat = geoPoints.DestinationLat,
                DestinationLon = geoPoints.DestinationLon,
                UserId = geoPoints.UserId
            };
        }
    }
}
