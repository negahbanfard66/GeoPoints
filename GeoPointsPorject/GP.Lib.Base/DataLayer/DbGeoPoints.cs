using GP.Lib.Base.DataLayer.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GP.Lib.Base.DataLayer
{
    public class DbGeoPoints:Entity
    {
        public double OriginLat { get; set; }

        public double OriginLon { get; set; }

        public double DestinationLat { get; set; }

        public double DestinationLon { get; set; }

        public virtual int UserId { get; set; }
        public virtual DbUser User { get; set; }
    }
}
