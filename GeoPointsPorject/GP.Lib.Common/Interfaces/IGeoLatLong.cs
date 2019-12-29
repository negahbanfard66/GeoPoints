using System;
using System.Collections.Generic;
using System.Text;

namespace GP.Lib.Common.Interfaces
{
    public interface IGeoLatLong
    {
        double deg2rad(double deg);


        double rad2deg(double rad);
    }
}
