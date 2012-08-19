using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPyModeling;

namespace IPyPLSPlugin
{
    class IPyPLSPlugin : IPyModelingPlugin
    {
        public IPyPLSPlugin()
        {
            innerIronPythonControl = new IPyPLSControl.IPyPLSControl();
            strPanelKey = "PLSPanel";
            strPanelCaption = "PLS";
        }
    }
}
