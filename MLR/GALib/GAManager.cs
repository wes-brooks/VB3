using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GALib
{
    public class GAManager
    {
        public delegate void GAProgressHandler(double generation, double max);
        public delegate void GACompleteHandler(GAManager gaManager);

        private int _numberGenerations = -1;
        private int maxPopSize = -1;
        private Cache _cache = null;
        private int _cacheSize = 20;

        private int _numElite = 0;
        private Population _population = null;
        public volatile bool Cancel = false;

        public List<IIndividual> Results
        {
            get
            {
                if (_cache == null)
                    return null;

                return _cache.CacheList;
            }
        }

        public int CacheSize
        {
            get { return _cacheSize; }
            set { _cacheSize = value; }
        }

        public GAManager()
        {
            //Initializing random number generator is done from interface
            //RandomNumbers.SetRandomSeed();
            _cacheSize = 20;
        }

        public GAProgressHandler GAProgress;
        public GACompleteHandler GAComplete;


        public int NumberOfGenerations
        {
            get { return _numberGenerations; }
            set { _numberGenerations = value; }
        }

        public void Init(Population population)
        {
            _population = population;
        }

        public void Run()
        {
            Population population;

            if (_population == null)
                return;

            population = _population;
            _numElite = 2;
            maxPopSize = population.PopulationSize;

            _cache = new Cache(_cacheSize);
            _cache.Comparer = population.Comparer;
            _cache.ChromosomeComparer = population.ChromosomeComparer;
                     
            population.Evaluate();

            List<IIndividual> elite = null;
            List<IIndividual> lstCache = null;
            IIndividual cloner = population.GetTopIndividuals(1)[0];
            
            for (int i = 0; i < _numberGenerations; i++)
            {      
                int iPopulationShortage = population.MaximumPopulationSize - population.PopulationSize;
                if (iPopulationShortage > 0)
                {
                    for (int j = 0; j < iPopulationShortage; j++)
                    {
                        IIndividual indiv = cloner.Clone();
                        indiv.Initialize();
                        indiv.Evaluate();
                        population.Add(indiv);
                    }
                }

                population.ReplaceLeastFit(elite);
                population.Evolve();
                population.Evaluate();
                population.Cull();
                population.Sort();

                lstCache = population.GetTopUniqueIndividuals(_cacheSize);
                int numElements = Math.Min(lstCache.Count, _numElite);
                elite = Util.CloneRange(lstCache,0, numElements);

                _cache.Add(lstCache);
                _cache.Sort();

                double percentComplete = i * 100 / _numberGenerations;
                GAProgress(percentComplete, _cache.MaximumFitness);
                Console.WriteLine("Min: " + _cache.MinimumFitness + "  Max: " + _cache.MaximumFitness);

                if (Cancel)
                    break;
            }

            _cache.Sort();
            GAComplete(this);
            return;            
        }
    }
}
