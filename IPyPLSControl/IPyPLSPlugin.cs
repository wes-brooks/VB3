using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using IPyModeling;

namespace IPyPLSPlugin
{
    [Export(typeof(VBCommon.Interfaces.IModel))]
    [ExportMetadata("PluginKey", "PLS")]
    class IPyPLSPlugin : IPyModelingPlugin, VBCommon.Interfaces.IModel
    {
        public IPyPLSPlugin()
        {
            innerIronPythonControl = new IPyPLSControl.IPyPLSControl();
            strPanelKey = "PLSPanel";
            strPanelCaption = "PLS";
        }


        public List<double> Predict(System.Data.DataTable tblPredictionFrame) { return (innerIronPythonControl.Predict(tblPredictionFrame)); }
        public List<double> PredictExceedanceProbability(System.Data.DataTable tblPredictionFrame) { return (innerIronPythonControl.Predict(tblPredictionFrame)); }
        public string ModelString() { return (innerIronPythonControl.ModelString()); }
    }
}
