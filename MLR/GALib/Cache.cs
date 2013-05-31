//********************************************************************************************************
//
// File GALib.Cache.cs
// Author: Kurt Wolfe
// Created: 05/02/2007
// Cache to hold IIndividuals outside of the GA population
//
//********************************************************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GALib
{
    public class Cache
    {
        //Dictionary<string, IIndividual> _cache = null;
        List<IIndividual> _cache = null;
        IComparer<IIndividual> _comparer = null;
        private int _cacheSize = 0;
        private double _minFitness, _maxFitness;
        private double testMax = 0.0;
        private int _sortIndex;
        IEqualityComparer<IIndividual> _chromosomeComparer = null;
        public Cache()
        {
            _cacheSize = 10;
            _sortIndex = 0;
            _cache = new List<IIndividual>();
            
        }

        public Cache(int cacheSize)
        {
            _cacheSize = cacheSize;
            _sortIndex = 0;
            _cache = new List<IIndividual>();
        }

        public Cache(int cacheSize, double minimumFitness)
        {
            _cacheSize = cacheSize;
            _sortIndex = 0;
            _cache = new List<IIndividual>();
            _minFitness = minimumFitness;
        }

        public double MinimumFitness
        {
            get
            {
                if ((_cache != null) && (_cache.Count > 0))
                    return _cache[_cache.Count - 1].Fitness;
                else
                    return _minFitness;
            }
        }

        public double MaximumFitness
        {
            get
            {
                _maxFitness = _cache[0].Fitness;
                return _maxFitness;
            }
        }


        public List<IIndividual> CacheList
        {
            get { return _cache; }
        }

        public IIndividual this[int index]
        {
            get { return _cache[index]; }
            set { _cache[index] = value; }
        }

        public IComparer<IIndividual> Comparer
        {
            get { return _comparer; }
            set { _comparer = value; }
        }

        /// <summary>
        /// Individual Fitness Comparer
        /// </summary>
        public IEqualityComparer<IIndividual> ChromosomeComparer
        {
            get { return _chromosomeComparer; }
            set { _chromosomeComparer = value; }
        }

        /// <summary>
        /// Function returns the top N # of individuals as the population
        /// is currently sorted.
        /// </summary>
        /// <param name="numBest"></param>
        /// <returns></returns>
        public List<IIndividual> GetTopUniqueIndividuals(int numIndividuals)
        {
            if ((_cache == null) || (_cache.Count < 1))
                return null;

            List<IIndividual> list = this.Distinct();

            if (list == null)
                return null;


            int count = Math.Min(numIndividuals, list.Count);

            return Util.CloneRange(list, 0, count);

        }

        public List<IIndividual> Distinct()
        {
            List<IIndividual> list = Util.Distinct(_cache);
            //List<IIndividual> list = _cache.Distinct(_chromosomeComparer).ToList();
            if (list.Count < _cache.Count)
            {
                int i = 1;
            }
            return list;
        }



        public void Sort()
        {            
            if ((_cache == null) || (_cache.Count < 1))
                return;

            _cache.Sort(_comparer);           
            
            _maxFitness = _cache[0].Fitness;
            _minFitness = _cache[_cache.Count - 1].Fitness;
        }

       
        public void ReplaceMinimum(IIndividual indiv)
        {
            if (indiv == null)
                return;

            if (_cache.Count < _cacheSize)
                _cache.Add(indiv);
            else
            {
                if (_comparer.Compare(_cache[_cache.Count - 1], indiv) > 0)
                    _cache[_cache.Count - 1] = indiv;

            }
        }


        public void Add(List<IIndividual> list)
        {
            _cache.AddRange(list);

            _cache = Distinct();

            Sort();

            int numElements = Math.Min(_cacheSize, _cache.Count);
            List<IIndividual> tmpList = Util.CloneRange(_cache, 0, numElements);
            
            _cache = tmpList;

            return;
        }


        public void Add(IIndividual individual)
        {
            List<IIndividual> tmpCache = new List<IIndividual>();
            tmpCache.Add(individual);

            Add(tmpCache);

            return;
        }


        public bool Contains(IIndividual individual)
        {
            string chromNew = individual.Genotype;

            if (chromNew.Contains("1,"))
            {
                int i = 1;
            }
            string chromCurrent = "";
            
            foreach (IIndividual indiv in _cache)
            {
                chromCurrent = indiv.Genotype;
                Console.WriteLine("Chromosome: " + chromCurrent);
                if (String.Compare(chromCurrent, chromNew, true) == 0)
                    return true;
            }

            return false;

            Dictionary<string, string> dictChromomsome = new Dictionary<string, string>();
            if (dictChromomsome.ContainsKey(individual.Genotype))
                return true;
            else
                return false;


            //Dictionary<short,short> dict = new Dictionary<short,short>();
            List<string> list = new List<string>();
            for (short i=0;i<individual.Chromosome.Count;i++)
            {
                if (individual.Chromosome[i] != "")
                    list.Add(individual.Chromosome[i]);
            }
           
            
            //If identical indiv. (same chromosome bitstring) is already in cache not need to add this one
            //string bitString = individual.GetChromosomeAsString();
            int count = 0;

            foreach (IIndividual indiv in _cache)
            {
                count = 0;
                for (int j = 0; j < indiv.Chromosome.Count; j++)
                {
                    if (indiv.Chromosome[j] != "")
                    {
                        if (list.Contains(indiv.Chromosome[j]))
                            count++;
                        else
                            break;
                    }
                }
                if (count == list.Count)
                    return true;
            }            
            return false;
        }

        public void EmptyCache()
        {
            if (_cache == null)
                return;

            _cache.Clear();
        }

    }
}
