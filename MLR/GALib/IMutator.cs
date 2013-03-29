using System;
using System.Collections.Generic;
using System.Text;

namespace GALib
{
    public interface IMutator
    {
        void Mutate(IIndividual individual);

        double MutationRate
        {
            get;
            set;
        }
    }
}
