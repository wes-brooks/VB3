using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VBCommon.Interfaces
{
    public interface IModel
    {
        List<double> Predict(System.Data.DataSet data, double RegulatoryThreshold, double DecisionThreshold, VBCommon.Transforms.DependentVariableTransforms ThresholdTransform, double ThresholdPowerExponent);
        List<double> PredictExceedanceProbability(System.Data.DataSet data, double RegulatoryThreshold, double DecisionThreshold, VBCommon.Transforms.DependentVariableTransforms ThresholdTransform, double ThresholdPowerExponent);
        string ModelString();
        IDictionary<string, object> GetPackedState();
    }
}
