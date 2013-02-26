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
            innerIronPythonControl.ControlChangeEvent += new EventHandler(ControlChangeEventHandler);
        }


        public List<double> Predict(System.Data.DataSet dsPredictionData, double RegulatoryThreshold, double DecisionThreshold, VBCommon.Transforms.DependentVariableTransforms ThresholdTransform, double ThresholdPowerExponent) { return (innerIronPythonControl.Predict(dsPredictionData, RegulatoryThreshold, DecisionThreshold, ThresholdTransform, ThresholdPowerExponent)); }
        public List<double> PredictExceedanceProbability(System.Data.DataSet dsPredictionData, double RegulatoryThreshold, double DecisionThreshold, VBCommon.Transforms.DependentVariableTransforms ThresholdTransform, double ThresholdPowerExponent) { return (innerIronPythonControl.PredictExceedanceProbability(dsPredictionData, RegulatoryThreshold, DecisionThreshold, ThresholdTransform, ThresholdPowerExponent)); }
        public string ModelString() { return (innerIronPythonControl.ModelString()); }
    }
}
