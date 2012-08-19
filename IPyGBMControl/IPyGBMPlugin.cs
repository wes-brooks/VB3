using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPyModeling;

namespace IPyGBMPlugin
{
    class IPyGBMPlugin : IPyModelingPlugin
    {
        public IPyGBMPlugin()
        {
            innerIronPythonControl = new IPyGBMControl.IPyGBMControl();
            strPanelKey = "GBMPanel";
            strPanelCaption = "GBM";
        }
    }
}
