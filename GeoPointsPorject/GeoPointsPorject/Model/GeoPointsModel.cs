using GP.Lib.Base.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeographicalPointsProject.Model
{
    public class GeoPointsModel
    {
        public double OriginLat { get; set; }

        public double OriginLon { get; set; }

        public double DestinationLat { get; set; }

        public double DestinationLon { get; set; }

    }
}
