using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VBStatistics;
using MultipleLinearRegression;
using Combinatorics;
using VBTools;
using LogUtilities;
using GALib;

namespace GALibForm
{
    public class StepwiseSearchManager
    {
        public delegate void SWProgressHandler(double generation, double max);
        public delegate void SWCompleteHandler(StepwiseSearchManager esManager);

        public SWProgressHandler SWProgress;
        public SWCompleteHandler SWComplete;

        private Cache _stepwiseCache = null;
        private FitnessCriteria _fitnessCrit;
        private int _numVars = 0;
        private int _maxVIF = Int32.MaxValue;
        private double _decisionThreshold = Double.NaN;
        private double _mandateThreshold = Double.NaN;

        public volatile bool Cancel = false;

        public List<IIndividual> Results
        {
            get
            {
                if (_stepwiseCache == null)
                    return null;

                return _stepwiseCache.CacheList;
            }
        }

        public StepwiseSearchManager(int numberOfVariables, FitnessCriteria fitnessCriteria)
        {
            _numVars = numberOfVariables;
            _fitnessCrit = fitnessCriteria;
        }

        public StepwiseSearchManager(int numberOfVariables, FitnessCriteria fitnessCriteria, int maxVIF)
        {
            _numVars = numberOfVariables;
            _fitnessCrit = fitnessCriteria;
            _maxVIF = maxVIF;
        }

        public StepwiseSearchManager(int numberOfVariables, FitnessCriteria fitnessCriteria, int maxVIF, double threshold)
        {
            _numVars = numberOfVariables;
            _fitnessCrit = fitnessCriteria;
            _maxVIF = maxVIF;
            _decisionThreshold = threshold;
            _mandateThreshold = threshold;
        }

        public StepwiseSearchManager(int numberOfVariables, FitnessCriteria fitnessCriteria, int maxVIF, double decisionThreshold, double mandateThreshold)
        {
            _numVars = numberOfVariables;
            _fitnessCrit = fitnessCriteria;
            _maxVIF = maxVIF;
            _decisionThreshold = decisionThreshold;
            _mandateThreshold = mandateThreshold;
        }

        public void Run()
        {
        }

        public void RunBackwardStepwise()
        {
            long percentComplete = 0;            
            _stepwiseCache = new Cache(10, 0.0);

            IComparer<IIndividual> comparer = null;
           

            if ((_fitnessCrit == FitnessCriteria.R2) || (_fitnessCrit == FitnessCriteria.AdjustedR2))            
                comparer = new DescendSort();            
            else if ((_fitnessCrit == FitnessCriteria.Sensitivity) || (_fitnessCrit == FitnessCriteria.Specificity) || (_fitnessCrit == FitnessCriteria.Accuracy))
                comparer = new DescendSort();            
            else            
                comparer = new AscendSort();            

            _stepwiseCache.Comparer = comparer;

            IIndividual indiv = null;

            if ((!Double.IsNaN(_decisionThreshold)) && (!Double.IsNaN(_mandateThreshold)) && (_maxVIF != Int32.MaxValue))
                indiv = new MLRIndividual(_numVars, _numVars, _fitnessCrit, _maxVIF, _decisionThreshold, _mandateThreshold);
            else if (_maxVIF != Int32.MaxValue)
                indiv = new MLRIndividual(_numVars, _numVars, _fitnessCrit, _maxVIF);
            else
                indiv = new MLRIndividual(_numVars, _numVars, _fitnessCrit);

            //Init the chromosome with a full model            
            for (int i = 0; i < _numVars; i++)
                indiv.Chromosome[i] = (short)(i+1);

            indiv.Evaluate();


            Dictionary<string, IIndividual> models = new Dictionary<string, IIndividual>();
            bool[] droppedGenes = new bool[_numVars];
            for (int i = 0; i < _numVars; i++)
                droppedGenes[i] = false;

            IIndividual bestIndiv = indiv.Clone();
            IIndividual currentIndiv = null;

            bestIndiv.Evaluate();

            _stepwiseCache.Add(bestIndiv.Clone());

            string genotype = bestIndiv.Genotype;
            models.Add(genotype,bestIndiv);

            //Start with a full chromosome

            while (true)
            {
               
                short tempVal = 0;
                int droppedGene = -1;

                currentIndiv = bestIndiv.Clone();

                for (int i = 0; i < _numVars; i++)
                {
                    
                    tempVal = currentIndiv.Chromosome[i];
                    if (tempVal > 0)
                    {
                        currentIndiv.Chromosome[i] = 0;
                        currentIndiv.Evaluate();
                        if (comparer.Compare(bestIndiv, currentIndiv) > 0)
                        {
                            droppedGene = i;
                            bestIndiv = currentIndiv.Clone();
                        }

                        currentIndiv.Chromosome[i] = tempVal;
                    }                                       
                }

                if (droppedGene >= 0)
                    droppedGenes[droppedGene] = true;


                currentIndiv = bestIndiv.Clone();
                for (int i = 0; i < droppedGenes.Length; i++)
                {
                    
                    if (droppedGenes[i])
                    {
                        currentIndiv.Chromosome[i] = (short)(i + 1);
                        currentIndiv.Evaluate();

                        if (comparer.Compare(bestIndiv, currentIndiv) > 0)
                        {
                            droppedGene = i;
                            droppedGenes[i] = false;
                            bestIndiv = currentIndiv.Clone();
                        }

                        currentIndiv.Chromosome[i] = 0;
                    }
                }
                
                genotype = bestIndiv.Genotype;
                if (models.ContainsKey(genotype))
                    break;
                else
                    models.Add(genotype, bestIndiv.Clone());

                _stepwiseCache.Add(bestIndiv.Clone());

            }
            
        }

        public void RunForwardStepwise()
        {
            long percentComplete = 0;
            _stepwiseCache = new Cache(10, 0.0);

            IComparer<IIndividual> comparer = null;


            if ((_fitnessCrit == FitnessCriteria.R2) || (_fitnessCrit == FitnessCriteria.AdjustedR2))
                comparer = new DescendSort();
            else if ((_fitnessCrit == FitnessCriteria.Sensitivity) || (_fitnessCrit == FitnessCriteria.Specificity) || (_fitnessCrit == FitnessCriteria.Accuracy))
                comparer = new DescendSort();
            else
                comparer = new AscendSort();

            _stepwiseCache.Comparer = comparer;

            IIndividual indiv = null;

            if ((!Double.IsNaN(_decisionThreshold)) && (!Double.IsNaN(_mandateThreshold)) && (_maxVIF != Int32.MaxValue))
                indiv = new MLRIndividual(_numVars, _numVars, _fitnessCrit, _maxVIF, _decisionThreshold, _mandateThreshold);
            else if (_maxVIF != Int32.MaxValue)
                indiv = new MLRIndividual(_numVars, _numVars, _fitnessCrit, _maxVIF);
            else
                indiv = new MLRIndividual(_numVars, _numVars, _fitnessCrit);

            //Init the chromosome with a full model                        
            indiv.Chromosome[0] = (short)(1);
            indiv.Evaluate();


            Dictionary<string, IIndividual> models = new Dictionary<string, IIndividual>();
            bool[] droppedGenes = new bool[_numVars];
            for (int i = 1; i < _numVars; i++)
                droppedGenes[i] = false;

            IIndividual bestIndiv = indiv.Clone();
            IIndividual currentIndiv = null;

            bestIndiv.Evaluate();

            _stepwiseCache.Add(bestIndiv.Clone());

            string genotype = bestIndiv.Genotype;
            models.Add(genotype, bestIndiv);

            //Start with only one independent variable

            while (true)
            {

                short tempVal = 0;
                int addedGene = -1;

                currentIndiv = bestIndiv.Clone();

                for (int i = 0; i < _numVars; i++)
                {

                    tempVal = currentIndiv.Chromosome[i];
                    if (tempVal < 1)
                    {
                        currentIndiv.Chromosome[i] = (short)(i + 1);
                        currentIndiv.Evaluate();
                        if (comparer.Compare(bestIndiv, currentIndiv) > 0)
                        {
                            addedGene = i;
                            bestIndiv = currentIndiv.Clone();
                        }

                        currentIndiv.Chromosome[i] = tempVal;
                    }
                }


                currentIndiv = bestIndiv.Clone();
                for (int i = 0; i < _numVars; i++)
                {

                    if ((currentIndiv.Chromosome[i] > 0) && (i != addedGene))
                    {
                        tempVal = currentIndiv.Chromosome[i];

                        currentIndiv.Chromosome[i] = 0;
                        currentIndiv.Evaluate();

                        if (comparer.Compare(bestIndiv, currentIndiv) > 0)
                        {
                            bestIndiv = currentIndiv.Clone();
                        }

                        currentIndiv.Chromosome[i] = tempVal;
                    }
                }

                genotype = bestIndiv.Genotype;
                if (models.ContainsKey(genotype))
                    break;
                else
                    models.Add(genotype, bestIndiv.Clone());

                _stepwiseCache.Add(bestIndiv.Clone());

            }
        }
    }
}
