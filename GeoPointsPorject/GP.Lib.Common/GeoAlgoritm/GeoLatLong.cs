using GP.Lib.Common.Interfaces;
using System;

namespace GP.Lib.Common.GeoAlgoritm
{
    public class GeoLatLong: IGeoLatLong
    {
        public double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        public double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }
    }
}
