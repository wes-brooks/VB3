using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VBCommon.Statistics;
using MultipleLinearRegression;
using Combinatorics;
using VBCommon;
//using LogUtilities;
using GALib;

namespace GALibForm
{
    public class ExhaustiveSearchManager
    {
        public delegate void ESProgressHandler(double generation, double max);
        public delegate void ESCompleteHandler(ExhaustiveSearchManager esManager);

        public ESProgressHandler ESProgress;
        public ESCompleteHandler ESComplete;

        private Cache _exhuastiveCache = null;
        private FitnessCriteria _fitnessCrit;
        private int _numVars = 0;
        private int _maxVarsInModel = 0;
        private int _maxVIF = Int32.MaxValue;
        private double _decisionThreshold = Double.NaN;
        private double _mandateThreshold = Double.NaN;

        public volatile bool Cancel = false;

        public List<IIndividual> Results
        {
            get
            {
                if (_exhuastiveCache == null)
                    return null;

                return _exhuastiveCache.CacheList;
            }
        }

        public ExhaustiveSearchManager(int numberOfVariables, FitnessCriteria fitnessCriteria)
        {
            _numVars = numberOfVariables;
            _maxVarsInModel = _numVars;
            _fitnessCrit = fitnessCriteria;
        }

        public ExhaustiveSearchManager(int numberOfVariables, FitnessCriteria fitnessCriteria, int maxVIF)
        {
            _numVars = numberOfVariables;
            _maxVarsInModel = _numVars;
            _fitnessCrit = fitnessCriteria;
            _maxVIF = maxVIF;
        }

        public ExhaustiveSearchManager(int numberOfVariables, FitnessCriteria fitnessCriteria, int maxVIF, double threshold)
        {
            _numVars = numberOfVariables;
            _maxVarsInModel = _numVars;
            _fitnessCrit = fitnessCriteria;
            _maxVIF = maxVIF;
            _decisionThreshold = threshold;
            _mandateThreshold = threshold;
        }

        public ExhaustiveSearchManager(int numberOfVariables, FitnessCriteria fitnessCriteria, int maxVIF, double decisionThreshold, double mandateThreshold)
        {
            _numVars = numberOfVariables;
            _maxVarsInModel = _numVars;
            _fitnessCrit = fitnessCriteria;
            _maxVIF = maxVIF;
            _decisionThreshold = decisionThreshold;
            _mandateThreshold = mandateThreshold;
        }

        public ExhaustiveSearchManager(int numberOfVariables, FitnessCriteria fitnessCriteria, int maxVIF, double decisionThreshold, double mandateThreshold, int maxVariablesInModel)
        {
            _numVars = numberOfVariables;
            _maxVarsInModel = maxVariablesInModel;
            _fitnessCrit = fitnessCriteria;
            _maxVIF = maxVIF;
            _decisionThreshold = decisionThreshold;
            _mandateThreshold = mandateThreshold;
        }

        public void Run()
        {
            long percentComplete = 0;
            List<string> listIVsDomain = MLRCore.MLRDataManager.GetDataManager().ModelFieldList;
            _exhuastiveCache = new Cache(10, 0.0);
            
            if ((_fitnessCrit == FitnessCriteria.R2) || (_fitnessCrit == FitnessCriteria.AdjustedR2))
            {
                _exhuastiveCache.Comparer = new DescendSort();
            }
            else if ((_fitnessCrit == FitnessCriteria.Sensitivity) || (_fitnessCrit == FitnessCriteria.Specificity) || (_fitnessCrit == FitnessCriteria.Accuracy))
            {
                _exhuastiveCache.Comparer = new DescendSort();
            }
            else
            {
                _exhuastiveCache.Comparer = new AscendSort();
            }

            IIndividual indiv = null;

            List<short> combList = new List<short>();
            short tmp = 0; ;
            for (int i = 0; i < _numVars; i++)
            {
                tmp = (short)(i + 1);
                combList.Add(tmp);
            }

            long totalComb = 0;
            long totalComplete = 0;


            Combinations<short> combinations = null;
            List<Combinations<short>> listAllComb = new List<Combinations<short>>();
            for (short i = 1; i <= _maxVarsInModel; i++)
            {
                combinations = new Combinations<short>(combList.ToArray(), i, GenerateOption.WithoutRepetition);
                listAllComb.Add(combinations);
                totalComb += combinations.Count;
            }

            for (short i = 1; i <= _maxVarsInModel; i++)
            {
                if (Cancel)
                    break;

                combinations = listAllComb[i - 1];
                foreach (IList<short> comb in combinations)
                {
                    if ((!Double.IsNaN(_decisionThreshold)) && (!Double.IsNaN(_mandateThreshold)) && (_maxVIF != Int32.MaxValue))
                        indiv = new MLRIndividual(i, i, _fitnessCrit, _maxVIF, _decisionThreshold, _mandateThreshold);
                    else if (_maxVIF != Int32.MaxValue)
                        indiv = new MLRIndividual(i, i, _fitnessCrit, _maxVIF);
                    else
                        indiv = new MLRIndividual(i, i, _fitnessCrit);

                    for (int j = 0; j < comb.Count; j++)
                        indiv.Chromosome[j] = listIVsDomain[comb[j]-1];

                    if (Cancel)
                        break;

                    indiv.Evaluate();

                    if (indiv.IsViable())
                    {
                        _exhuastiveCache.Add(indiv);
                    }

                    totalComplete++;

                    if ((totalComplete % 10) == 0)
                    {
                        percentComplete = totalComplete * 100 / totalComb;
                        ESProgress(percentComplete, _exhuastiveCache.MaximumFitness);
                        //VBLogger.getLogger().logEvent(percentComplete.ToString(), VBLogger.messageIntent.UserOnly, VBLogger.targetSStrip.ProgressBar);
                        //lblProgress.Text = "Progress: " + (Convert.ToDouble(totalComplete) / Convert.ToDouble(totalComb)) * 100;
                        //Console.WriteLine("Progress: " + (Convert.ToDouble(totalComplete) / Convert.ToDouble(totalComb)) * 100);
                        //lblProgress.Refresh();
                        //Application.DoEvents();
                    }
                }
            }
            _exhuastiveCache.Sort();
            ESComplete(this);
        }
    }
}
