using System;
using System.Collections.Generic;
using System.Text;

namespace GP.Lib.Base.ViewModel.GeoPoint
{
    public class VmGeoPointAdd
    {
        public double OriginLat { get; set; }

        public double OriginLon { get; set; }

        public double DestinationLat { get; set; }

        public double DestinationLon { get; set; }

        public  int UserId { get; set; }
    }
}
