using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
//using VBTools;
using GALib;

namespace MultipleLinearRegression
{
    public class MLRMutator : IMutator
    {
        double _mutationRate = 0.0;
        int _maxVal = 0;

        public MLRMutator(double mutationRate, int maxVal)
        {
            _mutationRate = mutationRate;
            _maxVal = maxVal;
        }

        #region IMutator Members

        /// <summary>
        /// If a gene mutates, generate a new value, loop over all the existing genes to
        /// make sure the value does not already exist.  If it does, move along.  If it doesn't
        /// assign the new value.
        /// </summary>
        /// <param name="individual"></param>
        public void Mutate(IIndividual individual)
        {
            Test(individual);

            return;            
        }

        private void Test(IIndividual individual)
        {
            double dval = 0.0;
            int newVal = -1;
            bool valExists = false;
            List<string> listIVsDomain = MLRCore.MLRDataManager.GetDataManager().ModelFieldList;
            List<string> list = new List<string>();
            list.Add("");            

            //Have to evaluate at this point to make sure calculated values are synched up with current chromosome.
            MLRIndividual indiv = individual as MLRIndividual;
            indiv.Evaluate();
            if (!indiv.IsViable())
                return;

            DataTable dt = indiv.Parameters;            
            
            double pval = 0.0;
            double pvalMutationRate = 0.0;

            for (int i = 0; i < individual.Chromosome.Count; i++)
            {
                pvalMutationRate = 0.0;
                //if the gene is already 0 or the pvalue for the gene is less than 0.05 then use standard mutation rate.
                //if pvalue is between 0.05 and 0.8 use pvalue as mutation rate.
                //if pvalue is greater than 0.8 use pvalue equal to 0.8.
                if (individual.Chromosome[i] == "")
                    pvalMutationRate = _mutationRate;
                else
                {
                    list[0] = individual.Chromosome[i];
                    string[] varName = list.ToArray();
                    DataRow[] dr = dt.Select("Name = '" + varName[0] + "'");
                    pval = Convert.ToDouble(dr[0]["PValue"]);

                    if (pval < 0.05)
                        pvalMutationRate = _mutationRate;
                    else if (pval > 0.8)
                        pvalMutationRate = Math.Max(_mutationRate, 0.8);
                    else                    
                        pvalMutationRate = Math.Max(_mutationRate, pval);
                    
                }

                dval = RandomNumbers.NextDouble();

                if (dval < pvalMutationRate)
                {
                    valExists = false;
                    newVal = RandomNumbers.NextInteger(_maxVal + 1);

                    dval = RandomNumbers.NextDouble();
                    if (dval < 0.33)                         
                        newVal = 0;                        
                    else 
                    {
                        if (newVal > 0)
                        {
                            for (int j = 0; j < individual.Chromosome.Count; j++)
                            {
                                if (individual.Chromosome[j] == listIVsDomain[newVal-1])
                                {
                                    valExists = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (!valExists)
                    {
                        if (newVal > 0) { individual.Chromosome[i] = listIVsDomain[newVal - 1]; }
                        else { individual.Chromosome[i] = ""; }
                    }
                }
            }
        }

        public double MutationRate
        {
            get { return _mutationRate; }
            set { _mutationRate = value; }            
        }

        #endregion
    }
}
