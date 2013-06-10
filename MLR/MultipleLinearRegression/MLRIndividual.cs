using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using GALib;
using VBCommon.Statistics;
using MLRCore;

namespace MultipleLinearRegression
{
    public enum FitnessCriteria
    {
        Akaike,
        AdjustedR2,
        R2,
        BIC,
        AICC,
        Press,
        RMSE,
        Specificity,
        Sensitivity,
        Accuracy
    }


    public class MLRIndividual : IIndividual
    {
        private double _fitness = 0.0;
        private int _numGenes;
        private int _maxGeneValue = 0;
        private List<string> _chromosome = null;
        private FitnessCriteria _fitnessCriteria;
        private DataTable _parameters = null;
        private double[] _predictedValues = null;
        private double[] _observedValues = null;
        private double[] _dffits = null;

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
        private double _maxVIF = Double.NaN;
        private double _decisionThreshold = Double.NaN;
        private double _mandateThreshold = Double.NaN;
        private double _eigenvalueRatio = Double.NaN;
        private double _eigenvalueRatioLimit = Double.NaN;

        private bool _cantCompute = false;


        #region RequiredForSerializaiton

        //Added these accessors in for use by the Serialization routines
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

        public FitnessCriteria FitnessCriteria
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

        #endregion

        public MLRIndividual(int numGenes, int maxGeneValue, FitnessCriteria fitnessCriteria, int maxVIF, double decisionThreshold, double mandateThreshold)
        {
            _numGenes = numGenes;
            _maxGeneValue = maxGeneValue;
            _fitnessCriteria = fitnessCriteria;
            _maxVIF = maxVIF;
            _decisionThreshold = decisionThreshold;
            _mandateThreshold = mandateThreshold;
            Init();
        }


        public MLRIndividual(int numGenes, int maxGeneValue, FitnessCriteria fitnessCriteria, int maxVIF, double threshold)
        {
            _numGenes = numGenes;
            _maxGeneValue = maxGeneValue;
            _fitnessCriteria = fitnessCriteria;
            _maxVIF = maxVIF;
            _decisionThreshold = threshold;
            _mandateThreshold = threshold;
            Init();
        }


        public MLRIndividual(int numGenes, int maxGeneValue, FitnessCriteria fitnessCriteria, int maxVIF)
        {
            _numGenes = numGenes;
            _maxGeneValue = maxGeneValue;
            _fitnessCriteria = fitnessCriteria;
            _maxVIF = maxVIF;
            Init();
        }


        public MLRIndividual(int numGenes, int maxGeneValue, FitnessCriteria fitnessCriteria)
        {
            _numGenes = numGenes;
            _maxGeneValue = maxGeneValue;
            _fitnessCriteria = fitnessCriteria;
            Init();
        }


        public MLRIndividual(MLRIndividual individual)
        {
            _numGenes = individual.Chromosome.Count;
            _chromosome = individual.Chromosome.ToList();
            _fitnessCriteria = individual.FitnessCriteria;
            _fitness = individual.Fitness;
            _maxVIF = individual.MaxVIF;
            _adjustedR2 = individual.AdjustedR2;
            _AIC = individual.AIC;
            _AICC = individual.AICC;
            _BIC = individual.BIC;
            _Press = individual.Press;
            _R2 = individual.R2;
            _RMSE = individual.RMSE;
            _VIF = individual.VIF;
            _decisionThreshold = individual.DecisionThreshold;
            _mandateThreshold = individual.MandatedThreshold;
            _sensitivity = individual.Sensitivity;
            _specificity = individual.Specificity;
            _accuracy = individual.Accuracy;

            _maxGeneValue = individual.MaxGeneValue;

            if (individual.Parameters != null)
                _parameters = individual.Parameters.Copy();

            if (individual.PredictedValues != null)
            {
                _predictedValues = new double[individual.PredictedValues.Length];
                individual.PredictedValues.CopyTo(_predictedValues, 0);
            }

            if (individual.ObservedValues != null)
            {
                _observedValues = new double[individual.ObservedValues.Length];
                individual.ObservedValues.CopyTo(_observedValues, 0);
            }

            if (individual.DFFITS != null)
            {
                _dffits = new double[individual.DFFITS.Length];
                individual.DFFITS.CopyTo(_dffits, 0);
            }
        }

        
        private void Init()
        {
            if (_numGenes < 1)
                throw new Exception("MLR must have at least one independent variable.");

            string[] tmp = new string[_numGenes];
            _chromosome = new List<string>(tmp);
           
            if (_fitnessCriteria == FitnessCriteria.Akaike)
                _fitness = Double.PositiveInfinity;

            if (Double.IsNaN(_maxVIF)) 
                _maxVIF = 5;
        }


        #region IIndividual Members

        public List<string> Chromosome
        {
            get { return _chromosome;}
            set 
            {
                List<string> temp = value;
                if (temp.Count != _numGenes)
                    throw new Exception("Invalid chromosome size.");
                else
                    _chromosome = temp;
            }
        }


        public IIndividual Clone()
        {
            MLRIndividual individual = new MLRIndividual(this);
            return individual;
        }


        public double Fitness
        {
            get { return _fitness; }
            set {_fitness = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public void Initialize()
        {
            List<string> genes = new List<string>();            
            List<string> listIVsDomain = MLRDataManager.GetDataManager().ModelFieldList;
            short geneVal = -1;

            for (int i = 0; i < _chromosome.Count; i++)
            {
                //Have to do _maxGeneValue - 2 because first 2 columns of datatable are date and response value
                geneVal = (short)RandomNumbers.NextInteger(_maxGeneValue+1);

                if (geneVal > 0)
                {
                    while (genes.Contains(listIVsDomain[geneVal-1]))
                    {
                        geneVal = (short)RandomNumbers.NextInteger(_maxGeneValue+1);
                        if (geneVal < 1)
                            break;

                        if (genes.Count > _chromosome.Count)
                            throw new Exception("Problem initializing MLRIndividual.");
                    }
                }
                else
                {
                    int j = 1;
                }

                if (geneVal > 0)
                {
                    genes.Add(listIVsDomain[geneVal - 1]);
                    _chromosome[i] = listIVsDomain[geneVal - 1];
                }
                else { _chromosome[i] = ""; }
            }
        }


        public string Genotype
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                List<string> tmp = _chromosome.ToList();
                tmp.Sort();

                for (int i = 0; i < tmp.Count; i++)
                {
                    if (tmp[i] != "")
                        sb.Append(tmp[i] + ",");
                }

                string retVal = sb.ToString();
                return retVal;
            }   
        }


        public void Evaluate()
        {
            _cantCompute = false;
            try
            {
                MLRDataManager _proj = MLRDataManager.GetDataManager();
                DataTable dt = _proj.ModelDataTable;
                string depVar = _proj.ModelDependentVariable;
                string[] indVars = _chromosome.Where(s => s != "").ToArray<string>();

                //When genes are all "" IsValid will return false:
                if (indVars.Count() == 0)
                    return;

                MultipleRegression mlr = new MultipleRegression(dt, depVar, indVars);
                mlr.Compute();

                _adjustedR2 = mlr.AdjustedR2;
                _R2 = mlr.R2;
                _AIC = mlr.AIC;
                _BIC = mlr.BIC;
                _AICC = mlr.AICC;
                _Press = mlr.Press;
                _RMSE = mlr.RMSE;
                _VIF = mlr.MaxVIF;
                
                _parameters = mlr.Parameters;
                _predictedValues = mlr.PredictedValues;
                _observedValues = mlr.ObservedValues;
                _dffits = mlr.DFFITS;

                if ((_decisionThreshold != Double.NaN) && (_mandateThreshold != Double.NaN))
                {
                    if (_predictedValues.Length != _observedValues.Length)
                        throw new Exception("The number of predicted and observed values are not equal.");

                    double pred;
                    double obs;
                    double truePos = 0;
                    double falsePos = 0;
                    double trueNeg = 0;
                    double falseNeg = 0;

                    for (int i = 0; i < _predictedValues.Length; i++)
                    {
                        pred = _predictedValues[i];
                        obs = _observedValues[i];
                        if ((pred > _decisionThreshold) && (obs > _mandateThreshold))
                            truePos++;
                        else if ((pred > _decisionThreshold) && (obs < _mandateThreshold))
                            falsePos++;
                        else if ((pred < _decisionThreshold) && (obs > _mandateThreshold))
                            falseNeg++;
                        else if ((pred < _decisionThreshold) && (obs < _mandateThreshold))
                            trueNeg++;
                    }

                    _sensitivity = truePos / (truePos + falseNeg);
                    _specificity = trueNeg / (trueNeg + falsePos);
                    _accuracy = (truePos + trueNeg) / (truePos + trueNeg + falsePos + falseNeg);
                }

                if (_fitnessCriteria == FitnessCriteria.AdjustedR2)
                    _fitness = mlr.AdjustedR2;
                else if (_fitnessCriteria == FitnessCriteria.Akaike)
                    _fitness = mlr.AIC;
                else if (_fitnessCriteria == FitnessCriteria.AICC)
                    _fitness = mlr.AICC;
                else if (_fitnessCriteria == FitnessCriteria.BIC)
                    _fitness = mlr.BIC;
                else if (_fitnessCriteria == FitnessCriteria.R2)
                    _fitness = mlr.R2;
                else if (_fitnessCriteria == FitnessCriteria.Press)
                    _fitness = mlr.Press;
                else if (_fitnessCriteria == FitnessCriteria.RMSE)
                    _fitness = mlr.RMSE;
                else if (_fitnessCriteria == FitnessCriteria.Sensitivity)
                {
                    if ((_decisionThreshold != Double.NaN) && (_mandateThreshold != Double.NaN))
                        _fitness = _sensitivity;
                }
                else if (_fitnessCriteria == FitnessCriteria.Specificity)
                {
                    if ((_decisionThreshold != Double.NaN) && (_mandateThreshold != Double.NaN))
                        _fitness = _specificity;
                }
                else if (_fitnessCriteria == FitnessCriteria.Accuracy)
                {
                    if ((_decisionThreshold != Double.NaN) && (_mandateThreshold != Double.NaN))
                        _fitness = _accuracy;
                }
                else
                {
                    throw new Exception("Invalid Fitness Criteria: " + _fitness);
                }
            }
            catch (Exception e)
            {
                _cantCompute = true;
            }
        }


        public bool IsViable()
        {
            if (_cantCompute)
                return false;

            //Check for Variance Inflation Factor (VIF) above the threshold
            if (!Double.IsNaN(_VIF))
            {
                if (_VIF > _maxVIF)
                    return false;
            }

            //Check for duplicate covariates
            List<string> genes = new List<string>();
            for (int i = 0; i < _chromosome.Count; i++)
            {
                
                if (_chromosome[i] != "")
                {
                    if (!genes.Contains(_chromosome[i]))
                        genes.Add(_chromosome[i]);
                    else
                        return false;
                }
            }

            //Check for an empty model
            if (genes.Count < 1)
                return false;

            return true;
        }
        #endregion


        public DataTable Parameters
        {
            get { return _parameters; }
        }

        public double[] PredictedValues
        {
            get { return _predictedValues; }
        }

        public double[] ObservedValues
        {
            get { return _observedValues; }
        }

        public double[] DFFITS
        {
            get { return _dffits; }
        }

        /*/// <summary>
        /// Returns an array of variable names.  First element is the depenedent variable
        /// The rest are the independent variable.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private string[] GetIndependentVariableNames()
        {
            //List<string> fieldList = DataStore.GetFieldList();
            MLRDataManager _proj = MLRDataManager.GetDataManager();

            string[] varNames = _proj.GetIndependentVariableList(_chromosome);
            return varNames;            
        }*/

        public double AIC
        {
            get { return _AIC; }
        }

        public double AICC
        {
            get { return _AICC; }
        }

        public double Press
        {
            get { return _Press; }
        }

        public double BIC
        {
            get { return _BIC; }
        }

        public double R2
        {
            get { return _R2; }
        }

        public double AdjustedR2
        {
            get { return _adjustedR2; }
        }

        public double RMSE
        {
            get { return _RMSE; }
        }

        public double Specificity
        {
            get { return _specificity; }
        }

        public double Sensitivity
        {
            get { return _sensitivity; }
        }

        public double Accuracy
        {
            get { return _accuracy; }
        }

        public double VIF
        {
            get { return _VIF; }
        }

        #region IEqualityComparer<IIndividual> Members

        public bool Equals(IIndividual x, IIndividual y)
        {
            List<string> union = x.Chromosome.Union(y.Chromosome).ToList();
            if (union.Count == x.Chromosome.Count)
                return true;
            else
                return false;
        }

        public int GetHashCode(IIndividual obj)
        {
            List<string> list = obj.Chromosome.ToList();
            list.Sort();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)            
                sb.Append(i);

            return sb.ToString().GetHashCode();
        }

        #endregion

        public Dictionary<string, double> Model
        {
            get
            {

                Dictionary<string, double> parameters = new Dictionary<string, double>();
                for (int i = 0; i < _parameters.Rows.Count; i++)
                {
                    parameters.Add(_parameters.Rows[i][0].ToString(), Convert.ToDouble(_parameters.Rows[i][1]));
                }

                return parameters;
            }
        }
    }
}
