using System;
using System.Collections.Generic;
using System.Text;

namespace GALib
{
    public interface ICrossover
    {
        List<IIndividual> Crossover(IIndividual parent1, IIndividual parent2);

        double CrossoverRate
        {
            get;
            set;
        }
    }
}
