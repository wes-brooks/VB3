using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VBCommon.Spatial;

namespace VBCommon.Interfaces
{
    public interface IBeachSite
    {

        PointGeo Location { get; set; }

        double Orientation { get; set; }

        string BeachName { get; set; }

        PointGeo LeftMarker { get; set; }

        PointGeo RightMarker { get; set; }

        PointGeo WaterMarker { get; set; }

        IBeachSite Clone();
        

    }
}
