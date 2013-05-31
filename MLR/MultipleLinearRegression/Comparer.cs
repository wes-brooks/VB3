using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GALib;

namespace MultipleLinearRegression
{

    public class AscendSort : IComparer<IIndividual>
    {

        #region IComparer<IIndividual> Members
        /// <summary>
        /// Return 1 if first IIndividual is greater (better) than second
        /// Return -1 if first IIndividual is smaller (worse) than second
        /// Return 0 if they are equal
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(IIndividual x, IIndividual y)
        {
            int retVal = 0;

            if (x.Fitness > y.Fitness)
                retVal = 1;
            else if (x.Fitness < y.Fitness)
                retVal = -1;

            return retVal;
        }

        #endregion
    }

    public class DescendSort : IComparer<IIndividual>
    {

        #region IComparer<IIndividual> Members
        /// Return 1 if first IIndividual is smaller (better) than second
        /// Return -1 if first IIndividual is greater (worse) than second
        /// Return 0 if they are equal
        public int Compare(IIndividual x, IIndividual y)
        {
            int retVal = 0;

            if (x.Fitness > y.Fitness)
                retVal = -1;
            else if (x.Fitness < y.Fitness)
                retVal = 1;

            return retVal;
        }

        #endregion
    }

    public class CompareChromosomes : EqualityComparer<IIndividual>
    {
        public override bool Equals(IIndividual x, IIndividual y)
        {
            string xChrom = x.Genotype;
            string yChrom = y.Genotype;
            if (String.Compare(xChrom, yChrom, true) == 0)
                return true;
            else
                return false;
        }

        public override int GetHashCode(IIndividual obj)
        {
            return obj.Genotype.GetHashCode();
        }
    }

    public class CompareChromosomes2 : IEqualityComparer<IIndividual>
    {
        
        #region IEqualityComparer<IIndividual> Members

        public bool Equals(IIndividual x, IIndividual y)
        {
            string xChrom = x.Genotype;
            string yChrom = y.Genotype;
            if (xChrom == yChrom)
                return true;
            else
                return false;

            //List<short> union = x.Chromosome.Union(y.Chromosome).ToList();
            //if (union.Count == x.Chromosome.Count)
            //    return true;
            //else
            //    return false;
        }

        public int GetHashCode(IIndividual obj)
        {
            List<string> list = obj.Chromosome.ToList();
            list.Sort();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != "")
                    sb.Append(list[i] + "_");                
            }
            return sb.ToString().GetHashCode();
        }

        #endregion
    }
}
