using System;
using System.Collections.Generic;
using System.Text;

namespace GALib
{
    public interface IIndividual : IEqualityComparer<IIndividual>
    {
       
        /// <summary>
        /// The List of 1s or 0s representing the chromosome
        /// </summary>
        List<string> Chromosome
        {
            get;
            set;
        }

        /// <summary>
        /// Make another individual just like this one
        /// Including the fitness value
        /// </summary>
        /// <returns></returns>
        IIndividual Clone();

        /// <summary>
        /// The fitness of the individual after its been evaluated
        /// It is an array so multiple values can be stored - e.g. fitness and cost
        /// </summary>
        double Fitness  
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the string representation of the chromosome. 
        /// </summary>
        /// <returns></returns>
        string Genotype
        {
            get;
        }


        /// <summary>
        /// Generate random values for genes
        /// Also make sure to reset fitness to default
        /// </summary>
        void Initialize();
        

        /// <summary>
        /// Evaluate the fitness of the individual based on internal fitness
        /// </summary>
        void Evaluate();

        /// <summary>
        /// Given the current state of the individual, returns whether this individual meets viability criteria
        /// </summary>
        /// <returns></returns>
        bool IsViable();

    }
}
