using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Data;
using MLRCore;

namespace MLRPlugin
{
    [Export(typeof(VBCommon.Interfaces.IModel))]
    [ExportMetadata("PluginKey", "MLR")]
    public class MLRPredPlugin : VBCommon.Interfaces.IModel
    {
        private ctlMLRModel _mlrPluginControl = null;
        private List<double> _lstProbEx = null;

        public MLRPredPlugin()
        {
            _mlrPluginControl = ctlMLRModel.MLRControl;
        }


        public List<double> Predict(System.Data.DataSet ds, double RegulatoryThreshold, double DecisionThreshold, VBCommon.Transforms.DependentVariableTransforms ThresholdTransform, double ThresholdPowerExponent)
        {
            if (ds == null || !ds.Tables.Contains("Raw"))
                throw new Exception("Unmodified data table does not exist.");

            DataTable dt = ds.Tables["Raw"];
            string modelString;

            //Dont pass in the dep var name.  Prediction plugin parser doesnt like it.
            modelString = Support.BuildModelExpression(_mlrPluginControl.ModelForm.ModelInfo.Model, null, "");
                    
            ExpressionEvaluator expEval = new ExpressionEvaluator();
            DataTable dtPredictions = expEval.Evaluate(modelString, dt);

            if (dtPredictions == null || dtPredictions.Rows.Count < 1)
                return null;

            List<double> lstPred = new List<double>();
            _lstProbEx = new List<double>();

            //get the threshold synced with measurements and dependent variable transform
            double udc = VBCommon.Transforms.Apply.UntransformThreshold(DecisionThreshold, ThresholdTransform, ThresholdPowerExponent);
            double tdc = VBCommon.Transforms.Apply.TransformThreshold(udc, _mlrPluginControl.ModelForm.DepVarTrans, ThresholdPowerExponent);
            
            //get a table of model variables/values (big X)
            DataTable dtModelVars = Support.getModelDatatable();

            //get the mlr rmse term
            double rmse = _mlrPluginControl.ModelForm.ModelInfo.RMSE;

            //compute P(exceedance) for each prediction
            for (int i = 0; i < dtPredictions.Rows.Count; i++)
            {
                //prediction...
                double pred = Convert.ToDouble(dtPredictions.Rows[i][1].ToString());
                lstPred.Add(pred);
            }                       
                 
            //do the matrix math... (mlrModelVar/Vals == x, dtModelVars == X...)
            double[] predictions = lstPred.ToArray<double>();
            _lstProbEx = VBCommon.Statistics.Statistics.PExceed(dtModelVars, predictions, tdc, rmse, dtPredData: ds.Tables["Prediction"]);

            return lstPred;
        }


        public List<double> PredictExceedanceProbability(System.Data.DataSet data, double RegulatoryThreshold, double DecisionThreshold, VBCommon.Transforms.DependentVariableTransforms ThresholdTransform, double ThresholdPowerExponent)
        {
            return _lstProbEx;
        }

        public string ModelString()
        {
            string modelString;
            //Dont pass in the dep var name.  Prediction plugin parser doesnt like it.
            modelString = Support.BuildModelExpression(_mlrPluginControl.ModelForm.ModelInfo.Model, null, "g4");
            return modelString;
        }

        public IDictionary<string,object> GetPackedState()
        {
            return _mlrPluginControl.PackProjectState();            
        }
    }
}
