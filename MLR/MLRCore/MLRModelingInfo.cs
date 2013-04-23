using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VBCommon;
using Newtonsoft.Json;
using ZedGraph;

namespace MLRCore
{
    //public enum DependentVariableTransform
    //{
    //    None,
    //    Log10,
    //    LogE,
    //    Power
    //}
    [Serializable]
    public class MLRModelingInfo
    {
        private Dictionary<string, double> _selectedModel;
        
        List<VBCommon.ListItem> _availableVariables = null;
        List<VBCommon.ListItem> _independentVariables = null;

        private VBCommon.Transforms.DependentVariableTransforms _thresholdTransform;
	    private VBCommon.Transforms.DependentVariableTransforms _importedTransform;
        private double[] _listFitnessProgressX = null;
        private double[] _listFitnessProgressY = null;
        private double _thresholdPowerTransformExponent = 1;
        private double _depVarTransformExponent = 1;
        private string _dependendVariable = "";

        List<List<short>> _chromosomes = null;

        private int _numGenes;
        private int _maxGeneValue;
        private int _fitnessCriteria;
        private double _maxVIF = Double.NaN;
        private double _decisionThreshold = Double.NaN;
        private double _mandateThreshold = Double.NaN;
        private double _AIC = Double.NaN;
        private double _AICC = Double.NaN;
        private double _Press = Double.NaN;
        private double _BIC = Double.NaN;
        private double _R2 = Double.NaN;
        private double _adjustedR2 = Double.NaN;
        private double _RMSE = Double.NaN;
        private double _specificity = Double.NaN;
        private double _sensitivity = Double.NaN;
        private double _accuracy = Double.NaN;
        private double _VIF = Double.NaN;
        private int _bestFitModelIndexSelected = -1;

        //residual rebuild variables
        private Dictionary<string, double> _selectedRebuild = null;
        private int _selectedRebuildIndex = -1;

        public MLRModelingInfo() { }

        //statistics
        [JsonProperty]
        public double[] FitnessProgressListX
        {
            get { return _listFitnessProgressX; }
            set { _listFitnessProgressX = value; }
        }

        [JsonProperty]
        public double[] FitnessProgressListY
        {
            get { return _listFitnessProgressY; }
            set { _listFitnessProgressY = value; }
        }

        public IPointListEdit GetFitnessProgress()
        {
            { 
                double[] x, y;
                x = _listFitnessProgressX;
                y = _listFitnessProgressY;

                PointPairList ppl = new PointPairList(x, y);
                return ppl;
            }
        }

        public void SetFitnessProgress(IPointList points)
        {
            {
                List<double> tempListX = new List<double>();
                List<double> tempListY = new List<double>();

                if (points != null)
                {
                    for (int i = 0; i < points.Count; i++)
                    {
                        PointPair item = points[i];
                        tempListX.Add(item.X);
                        tempListY.Add(item.Y);
                    }

                    _listFitnessProgressX = tempListX.ToArray();
                    _listFitnessProgressY = tempListY.ToArray();
                }
            }
        }

        [JsonProperty]
        public double AIC
        {
            get { return _AIC; }
            set {_AIC = value;}
        }

        [JsonProperty]
        public double AICC
        {
            get { return _AICC; }
            set {_AICC = value;}
        }

        [JsonProperty]
        public double Press
        {
            get { return _Press; }
            set {_Press = value;}
        }

        [JsonProperty]
        public double BIC
        {
            get { return _BIC; }
            set {_BIC = value;}
        }

        [JsonProperty]
        public double R2
        {
            get { return _R2; }
            set {_R2 = value;}
        }

        [JsonProperty]
        public double AdjustedR2
        {
            get { return _adjustedR2; }
            set {_adjustedR2 = value;}
        }

        [JsonProperty]
        public double RMSE
        {
            get { return _RMSE; }
            set {_RMSE = value;}
        }

        [JsonProperty]
        public double Specificity
        {
            get { return _specificity; }
            set {_specificity = value;}
        }

        [JsonProperty]
        public double Sensitivity
        {
            get { return _sensitivity; }
            set {_sensitivity = value;}
        }

        [JsonProperty]
        public double Accuracy
        {
            get { return _accuracy; }
            set {_accuracy = value;}
        }

        [JsonProperty]
        public double VIF
        {
            get { return _VIF; }
            set { _VIF = value; }
        }

        [JsonProperty]
        public string DependentVariable
        {
            get { return _dependendVariable; }
            set { _dependendVariable = value; }
        }

        [JsonProperty]
        public int NumGenes
        {
            get { return _numGenes; }
            set { _numGenes = value; }
        }

        [JsonProperty]
        public int MaxGeneValue
        {
            get { return _maxGeneValue; }
            set { _maxGeneValue = value; }
        }

        [JsonProperty]
        public int FitnessCriteria
        {
            get { return _fitnessCriteria; }
            set { _fitnessCriteria = value; }
        }

        [JsonProperty]
        public int MaxVIF
        {
            get { return (int)_maxVIF; }
            set { _maxVIF = value; }
        }

        [JsonProperty]
        public double DecisionThreshold
        {
            get { return _decisionThreshold; }
            set { _decisionThreshold = value; }
        }

        [JsonProperty]
        public double MandatedThreshold
        {
            get { return _mandateThreshold; }
            set { _mandateThreshold = value; }
        }

        [JsonProperty]
        public List<ListItem> AvailableVariables
        {
            get { return _availableVariables; }
            set { _availableVariables = value; }
        }

        [JsonProperty]
        public List<ListItem> IndependentVariables
        {
            get { return _independentVariables; }
            set { _independentVariables = value; }
        }


        [JsonProperty]
        public VBCommon.Transforms.DependentVariableTransforms ThresholdTransform
        {
            get { return _thresholdTransform; }
            set { _thresholdTransform = value; }
        }


        [JsonProperty]
        public double ThresholdPowerTransformExponent
        {
            get { return _thresholdPowerTransformExponent; }
            set { _thresholdPowerTransformExponent = value; }
        }


        [JsonProperty]
        public VBCommon.Transforms.DependentVariableTransforms DependentVariableTransform
        {
            get { return _importedTransform; }
            set { _importedTransform = value; }
        }

        [JsonProperty]
        public double DependentVariablePowerTransformExponent
        {
            get { return _depVarTransformExponent; }
            set { _depVarTransformExponent = value; }
        }

        [JsonProperty]
        public List<List<short>> Chromosomes
        {
            get { return _chromosomes; }
            set { _chromosomes = value; }
        }

        [JsonProperty]
        public Dictionary<string, double> Model
        {
            get { return _selectedModel; }
            set { _selectedModel = value; }
        }

        [JsonProperty]
        public int SelectedModel
        {
            set { _bestFitModelIndexSelected = value; }
            get { return _bestFitModelIndexSelected; }
        }

        [JsonProperty]
        public Dictionary<string, double> SelectedRebuild
        {
            set { _selectedRebuild = value; }
            get { return _selectedRebuild; }
        }

        [JsonProperty]
        public int SelectedRebuildIndex
        {
            set { _selectedRebuildIndex = value; }
            get { return _selectedRebuildIndex; }
        }
    }
}
