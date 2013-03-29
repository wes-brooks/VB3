using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GALib
{
    class Util
    {
        public static List<IIndividual> CloneRange(List<IIndividual> list, int index, int count)
        {
            List<IIndividual> newList = new List<IIndividual>(count);

            for (int i = index; i < index + count; i++)
            {
                newList.Add(list[i].Clone());
            }

            return newList;

        }

        /// <summary>
        /// Returns a unique subset of individuals.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<IIndividual> Distinct(List<IIndividual> list)
        {
            if (list == null)
                return null;
            
            List<IIndividual> distinctList = new List<IIndividual>();
            if (list.Count < 1)
                return distinctList;

            Dictionary<string, string> dct = new Dictionary<string, string>();
            
            string genType = "";
            foreach (IIndividual indiv in list)
            {
                genType = indiv.Genotype;
                if (!dct.ContainsKey(genType))
                {
                    dct.Add(genType, genType);
                    distinctList.Add(indiv);
                }
            }

            return distinctList;
        }
    }
}
