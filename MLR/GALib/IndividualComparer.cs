using System;
using System.Collections.Generic;
using System.Text;

namespace GALib
{
    public class IndividualComparer : IComparer<IIndividual>
    {
        int _fitnessIndex = 0;

        public IndividualComparer()
        {
            _fitnessIndex = 0;
        }

        public IndividualComparer(int fitnessIndex)
        {
            _fitnessIndex = fitnessIndex;
        }

        #region IComparer<IIndividual> Members

        public int Compare(IIndividual x, IIndividual y)
        {            
            int retVal = 0;

            if (x.Fitness < y.Fitness)
                retVal = -1;
            else if (x.Fitness > y.Fitness)
                retVal = 1;

            return retVal;
        }

        #endregion
    }
}
