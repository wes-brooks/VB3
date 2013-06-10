using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace VBCommon.Spatial
{
    [Serializable] 
    public class Site : VBCommon.Interfaces.IBeachSite
    {
        private PointGeo _markerLeft;
        private PointGeo _markerRight;
        private double _orientation = Double.NaN;
        private PointGeo _watermarker;
        private PointGeo _location;
        private string _name = "";

        public Site()
        {
            _location = new PointGeo();
            _markerLeft = new PointGeo();
            _markerRight = new PointGeo();
            _watermarker = new PointGeo();
        }

        public PointGeo Location
        {
            get { return _location; }
            set { _location = value; }
        }

        public PointGeo WaterMarker
        {
            get { return _watermarker; }
            set { _watermarker = value; }
        }

        public string BeachName
        {
            get { return _name; }
            set { _name = value; }
        }

        public PointGeo LeftMarker
        {
            get { return _markerLeft; }
            set { _markerLeft = value; }
        }

        public PointGeo RightMarker
        {
            get { return _markerRight; }
            set { _markerRight = value; }
        }

        public double Orientation
        {
            get { return _orientation; }
            set { _orientation = value; }
        }


        public VBCommon.Interfaces.IBeachSite Clone()
        {
            Site site = new Site();

            site.BeachName = _name;
            site.Orientation = _orientation;

            site.Location.Latitude = _location.Latitude;
            site.Location.Longitude = _location.Longitude;

            site.LeftMarker.Latitude = _markerLeft.Latitude;
            site.LeftMarker.Longitude = _markerLeft.Longitude;

            site.RightMarker.Latitude = _markerRight.Latitude;
            site.RightMarker.Longitude = _markerRight.Longitude;

            site.WaterMarker.Latitude = _watermarker.Latitude;
            site.WaterMarker.Longitude = _watermarker.Longitude;

            return site;
        }        
    }
}
