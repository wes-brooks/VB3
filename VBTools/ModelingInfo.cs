using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VBTools
{
    public enum DependentVariableTransform
    {
        None,
        Log10,
        LogE
    }

    public class ModelingInfo
    {
        List<ListItem> _availableVariables = null;
        List<ListItem> _independentVariables = null;

        private DependentVariableTransform _depVarTransform;
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


        public ModelingInfo()
        {

        }

        //statistics
        public double AIC
        {
            get { return _AIC; }
            set {_AIC = value;}
        }

        public double AICC
        {
            get { return _AICC; }
            set {_AICC = value;}
        }

        public double Press
        {
            get { return _Press; }
            set {_Press = value;}
        }

        public double BIC
        {
            get { return _BIC; }
            set {_BIC = value;}
        }

        public double R2
        {
            get { return _R2; }
            set {_R2 = value;}
        }

        public double AdjustedR2
        {
            get { return _adjustedR2; }
            set {_adjustedR2 = value;}
        }

        public double RMSE
        {
            get { return _RMSE; }
            set {_RMSE = value;}
        }

        public double Specificity
        {
            get { return _specificity; }
            set {_specificity = value;}
        }

        public double Sensitivity
        {
            get { return _sensitivity; }
            set {_sensitivity = value;}
        }

        public double Accuracy
        {
            get { return _accuracy; }
            set {_accuracy = value;}
        }

        public double VIF
        {
            get { return _VIF; }
            set { _VIF = value; }
        }


        public string DependentVariable
        {
            get { return _dependendVariable; }
            set { _dependendVariable = value; }
        }

        public int NumGenes
        {
            get { return _numGenes; }
            set { _numGenes = value; }
        }

        public int MaxGeneValue
        {
            get { return _maxGeneValue; }
            set { _maxGeneValue = value; }
        }

        public int FitnessCriteria
        {
            get { return _fitnessCriteria; }
            set { _fitnessCriteria = value; }
        }

        public int MaxVIF
        {
            get { return (int)_maxVIF; }
            set { _maxVIF = value; }
        }

        public double DecisionThreshold
        {
            get { return _decisionThreshold; }
            set { _decisionThreshold = value; }
        }

        public double MandatedThreshold
        {
            get { return _mandateThreshold; }
            set { _mandateThreshold = value; }
        }

        public List<ListItem> AvailableVariables
        {
            get { return _availableVariables; }
            set { _availableVariables = value; }
        }

        public List<ListItem> IndependentVariables
        {
            get { return _independentVariables; }
            set { _independentVariables = value; }
        }

        public DependentVariableTransform DependentVariableTransform
        {
            get { return _depVarTransform; }
            set { _depVarTransform = value; }
        }

        public List<List<short>> Chromosomes
        {
            get { return _chromosomes; }
            set { _chromosomes = value; }
        }
    }
}
