using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VBCommon.Spatial
{
    [Serializable] 
    public class PointGeo
    {
        private double _latitude;
        private double _longitude;

        public PointGeo()
        {
            _latitude = 0d;
            _longitude = 0d;
        }

        public PointGeo(double latitude, double longitude)
        {
            _latitude = latitude;
            _longitude = longitude;
        }

        public double Latitude
        {
            get { return _latitude; }
            set { _latitude = value; }
        }

        public double Longitude
        {
            get { return _longitude; }
            set { _longitude = value; }
        }
    }
}
