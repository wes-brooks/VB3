using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GALib;

namespace MultipleLinearRegression
{
    public class MLROnePointCrossover : ICrossover
    {
        private double _crossoverRate = 0.0;

        public MLROnePointCrossover(double crossoverRate)
        {
            _crossoverRate = crossoverRate;
        }
        
        #region ICrossover Members

        public List<IIndividual> Crossover(IIndividual parent1, IIndividual parent2)
        {
            List<IIndividual> children = new List<IIndividual>();

            if ((parent1 == null) || (parent2 == null))
                throw new Exception("Invalid parent for crossover.");

            int chromosomeLength = parent1.Chromosome.Count;
            if (chromosomeLength != parent2.Chromosome.Count)
                throw new Exception("Parents have different chromosome length.");

            if (chromosomeLength < 1)
                throw new Exception("Chromosome lenght must be > 0.");

            if (chromosomeLength == 1)
            {
                children.Add(parent1);
                children.Add(parent2);
                return children;
            }

            IIndividual child1 = parent1.Clone();
            IIndividual child2 = parent2.Clone();

            if (RandomNumbers.NextDouble() >= _crossoverRate)
            {
                children = new List<IIndividual>();
                children.Add(child1);
                children.Add(child2);
                return children;
            }
            
            //
            Dictionary<short, short> childGenes1 = new Dictionary<short, short>();
            Dictionary<short, short> childGenes2 = new Dictionary<short, short>();

            for (short i = 0; i < chromosomeLength; i++)
            {
                if (child1.Chromosome[i] > 0)
                    childGenes1.Add(child1.Chromosome[i],i);
                
                if (child2.Chromosome[i] > 0)
                    childGenes2.Add(child2.Chromosome[i],i);
            }


            int cutPoint = RandomNumbers.NextInteger(1, chromosomeLength - 1);
            //System.Console.WriteLine("Crossover point: " + cutPoint.ToString());

            List<short> crossoverGenes1 = new List<short>();
            List<short> crossoverGenes2 = new List<short>();
            //Check to see if any of the genes from cutpoint on exist in other parent
            //Keep a list of the genes found and randomly insert them into the other chromosome
            for (int i = cutPoint; i < chromosomeLength; i++)
            {

                if (childGenes2.ContainsKey(child1.Chromosome[i]))
                    child2.Chromosome[i] = 0;
                else
                    child2.Chromosome[i] = child1.Chromosome[i];

                if (childGenes1.ContainsKey(child2.Chromosome[i]))
                    child1.Chromosome[i] = 0;
                else
                    child1.Chromosome[i] = child2.Chromosome[i];
                
            }

            
            children.Add(child1);
            children.Add(child2);

            return children;                         
        }

        public double CrossoverRate
        {
            get { return _crossoverRate; }
            set { _crossoverRate = value; }
        }

        #endregion
    }
}
