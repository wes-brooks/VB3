using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using IPyModeling;

namespace IPyGBMPlugin
{
    [Export(typeof(VBCommon.Interfaces.IModel))]
    [ExportMetadata("PluginKey", "GBM")]
    class IPyGBMPlugin : IPyModelingPlugin, VBCommon.Interfaces.IModel
    {
        public IPyGBMPlugin()
        {
            innerIronPythonControl = new IPyGBMControl.IPyGBMControl();
            strPanelKey = "GBMPanel";
            strPanelCaption = "GBM";
        }


        public List<double> Predict(System.Data.DataSet dsPredictionData) { return (innerIronPythonControl.Predict(dsPredictionData)); }
        public List<double> PredictExceedanceProbability(System.Data.DataSet dsPredictionData) { return (innerIronPythonControl.PredictExceedanceProbability(dsPredictionData)); }
        public string ModelString() { return (innerIronPythonControl.ModelString()); }
    }
}
