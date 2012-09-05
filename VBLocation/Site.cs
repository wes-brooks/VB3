using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;


namespace VBLocation
{
    [Serializable] 
    public class Site
    {
        //private PointGeo _locationCenter;
        private PointGeo _markerLeft;
        private PointGeo _markerRight;
        private double _orientation = Double.NaN;
        private PointGeo _watermarker;
        private PointGeo _location;
        //private string _project;
        private string _name = "";

        public Site()
        {
            _location = new PointGeo();
            _markerLeft = new PointGeo();
            _markerRight = new PointGeo();
            _watermarker = new PointGeo();
        }

        //public string Project
        //{
        //    get { return _project; }
        //    set { _project = value; }
        //}

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

        public string Name
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

        
    }
}
