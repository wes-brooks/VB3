using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GALib
{
    public class Population
    {
        private int _maxPopSize = 0;
        List<IIndividual> _population = null;
        IFitnessEvaluator _fitnessEvaluator = null;
        ICrossover _crossoverMethod = null;
        ISelector _selector = null;
        IMutator _mutator = null;
        IComparer<IIndividual> _comparer = null;
        IEqualityComparer<IIndividual> _chromosomeComparer = null;

        public static int FitnessIndex = 0;

        public IFitnessEvaluator FitenessEvaluator
        {
            get { return _fitnessEvaluator; }
            set { _fitnessEvaluator = value; }
        }

        /// <summary>
        /// Gets and set the object to handle crossover
        /// </summary>
        public ICrossover CrossoverMethod
        {
            get { return _crossoverMethod; }
            set { _crossoverMethod = value; }
        }

        /// <summary>
        /// Gets and set the object to handle crossover
        /// </summary>
        public IMutator Mutator
        {
            get { return _mutator; }
            set { _mutator = value; }
        }

        /// <summary>
        /// Gets and sets the selector object
        /// </summary>
        public ISelector Selector
        {
            get { return _selector; }
            set { _selector = value; }
        }

        /// <summary>
        /// Individual Fitness Comparer
        /// </summary>
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
        /// Returns the current size of the population
        /// </summary>
        public int PopulationSize
        {
            get { return _population.Count; }
        }

        /// <summary>
        /// Returns the maximum size of the population
        /// </summary>
        public int MaximumPopulationSize
        {
            get { return _maxPopSize; }
        }


        /// <summary>
        /// Create a population with the default size of 100
        /// </summary>
        /// <param name="individual">Population will be composed of individuals of this type</param>
        public Population(IIndividual individual)
        {
            _maxPopSize = 100;
            _population = new List<IIndividual>();

            for (int i = 0; i < _maxPopSize; i++)                            
                _population.Add(individual.Clone());
            
        }

        /// <summary>
        /// Create a population with a specified size
        /// </summary>
        /// <param name="populationSize">Size of the population</param>
        /// <param name="individual">Population will be composed of individuals of this type</param>
        public Population(IIndividual individual, int populationSize )
        {
            _maxPopSize = populationSize;
            _population = new List<IIndividual>();

            for (int i = 0; i < _maxPopSize; i++)                            
                _population.Add(individual.Clone());
            
        }

        /// <summary>
        /// Create a population with a list of individuals
        /// </summary>
        /// <param name="population">A previously generated population</param>
        public Population(List<IIndividual> population)
        {
            _population = population;
            if (_population != null)
                _maxPopSize = _population.Count;
        }

        public void Initialize()
        {
            for (int i = 0; i < _population.Count; i++)
                _population[i].Initialize();
        }

        /// <summary>
        /// Sort the population based on the fitness
        /// </summary>
        public void Sort()
        {            
            if ((_population != null) || (_population.Count > 1 ))            
                _population.Sort(_comparer);            
        }

        
        /// <summary>
        /// Evaluate the population based on the default fitness measure
        /// </summary>
        public void Evaluate()
        {
            for (int i = 0; i < _population.Count; i++)
                _population[i].Evaluate();
        }

        /// <summary>
        /// Function returns the top N # of individuals as the population
        /// is currently sorted.
        /// </summary>
        /// <param name="numBest"></param>
        /// <returns></returns>
        public List<IIndividual> GetTopIndividuals(int numIndividuals)
        {
            if (_population == null)
                return null;

            Sort();

            int count = Math.Min(numIndividuals, _population.Count);

            return Util.CloneRange(_population, 0, count);


            if (_population.Count >= numIndividuals)
                return _population.GetRange(0, numIndividuals);
            else
                return _population;
        }

        /// <summary>
        /// Function returns the top N # of individuals as the population
        /// is currently sorted.
        /// </summary>
        /// <param name="numBest"></param>
        /// <returns></returns>
        public List<IIndividual> GetTopUniqueIndividuals(int numIndividuals)
        {
            if (_population == null)
                return null;

            List<IIndividual> list = this.Distinct();

            if (list == null)
                return null;
            
            list.Sort(_comparer);            

            int count = Math.Min(numIndividuals, list.Count);

            List<IIndividual> newList = Util.CloneRange(list, 0, count);
            return newList;
        }


        /// <summary>
        /// Generate next generation of individuals - i.e. crossover and mutate
        /// </summary>
        public void Evolve()
        {
            if (_crossoverMethod == null)
                throw new Exception("No crossover method specified.");

            if (_mutator == null)
                throw new Exception("No mutation method specified.");

            Crossover();
            Mutation();
        }


        private void Crossover()
        {
            if (_population.Count < 3)
                return;

            List<IIndividual> list = new List<IIndividual>();
            List<IIndividual> listTmp;

            IIndividual parent1, parent2;

            int iParent1 = -1;
            int iParent2 = -1;
            for (int i = 0; i < _population.Count; i = i + 2)
            {
                iParent1 = _selector.Select(_population);
                iParent2 = _selector.Select(_population);

                while (iParent1 == iParent2) //Dont want to select the same individual to crossover with itself
                {
                    iParent2 = _selector.Select(_population);
                }

                parent1 = _population[iParent1];
                parent2 = _population[iParent2];

                listTmp = _crossoverMethod.Crossover(parent1, parent2);
                
                list.AddRange(listTmp);
            }

            int removeAt = 0;
            while (list.Count > _population.Count)
            {
                removeAt = RandomNumbers.NextInteger(list.Count);
                list.RemoveAt(removeAt);
            }

            //Replace existing population with new one
            _population = list;
        }


        private void Mutation()
        {
            if (_mutator == null)
                throw new Exception("No mutator specified.");

            for (int i = 0; i < _population.Count; i++)
            {
                _mutator.Mutate(_population[i]);
            }
        }


        /// <summary>
        /// Remove non viable members of the population
        /// </summary>
        public void Cull()
        {
            for (int i = _population.Count-1; i >= 0;  i--)
            {
                if (!_population[i].IsViable())
                    _population.RemoveAt(i);
            }
        }


        /// <summary>
        /// Add an individual to the population
        /// </summary>
        /// <param name="individual"></param>
        public void Add(IIndividual individual)
        {
            if (_population.Count < _maxPopSize)
                _population.Add(individual);
        }


        public IIndividual this[int index]
        {
            get { return _population[index]; }
            set { _population[index] = value; }
        }


        public void ReplaceLeastFit(List<IIndividual> list)
        {
            if (list == null)
                return;

            int numAdd = Math.Min(_maxPopSize, list.Count);

            Sort();
            for (int i = 0; i < numAdd; i++)
            {
                _population[_population.Count - numAdd + i] = list[i];
            }
        }


        public List<IIndividual> Distinct()
        {
            List<IIndividual> list = Util.Distinct(_population);
            return list;            
        }
    }
}
