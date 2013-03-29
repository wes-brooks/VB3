using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GALib;

namespace MultipleLinearRegression
{

    abstract public class LargeSelector : ISelector
    {
        protected FitnessCriteria _fitnessCriteria;
        protected IComparer<IIndividual> _comparer;

        public IComparer<IIndividual> Comparer
        {
            get { return _comparer; }
            set { _comparer = value; }
        }

        #region ISelector Members

        //This is for Adj R2 - need one for AIC
        public int Select(List<IIndividual> list)
        {
            if ((list == null) || (list.Count < 1))
                throw new Exception("Invalid list to select from.");

            double num = list.Count;

            int first = RandomNumbers.NextInteger(0, list.Count - 1);
            int second = RandomNumbers.NextInteger(0, list.Count - 1);

            int compVal = _comparer.Compare(list[first], list[second]);
            if (compVal <= 0)
                return first;
            else if (compVal > 0)
                return second;
             

            double sumVal = 0.0;
            for (int i = 0; i < num; i++)
                sumVal += list[i].Fitness;

            double randVal = RandomNumbers.NextDouble() * sumVal;

            sumVal = 0.0;
            for (int i = 0; i < num; i++)
            {
                sumVal += list[i].Fitness;
                if (sumVal >= randVal)
                {
                    //System.Console.WriteLine("Rand," + randVal + " ,Index," + i);
                    return i;
                }
            }
            return -1;
        }

        #endregion
    }

    abstract public class SmallSelector : ISelector
    {
        protected FitnessCriteria _fitnessCriteria;
        protected IComparer<IIndividual> _comparer;

        public IComparer<IIndividual> Comparer
        {
            get { return _comparer; }
            set { _comparer = value; }
        }
        
        #region ISelector Members
        //Implement a tournement selection
        //Compare two individuals - most fit wins
        public int Select(List<IIndividual> list)
        {
            if ((list == null) || (list.Count < 1))
                throw new Exception("Invalid list to select from.");

            //KW test code
            //if (_comparer != null)
            //{
            //    List<IIndividual> tmpList = new List<IIndividual>(list);
            //    tmpList.Sort(_comparer);
            //}

            int first = RandomNumbers.NextInteger(0, list.Count - 1);
            int second = RandomNumbers.NextInteger(0, list.Count - 1);

            int compVal = _comparer.Compare(list[first], list[second]);
            if (compVal <= 0)
                return first;
            else if (compVal > 0)
                return second;

            double num = list.Count;
            //double totalRank = (num + 1.0) * (num / 2);

            double sumVal = 0.0;
            for (int i = 0; i < num; i++)
                sumVal += 1 / list[i].Fitness;

            double randVal = RandomNumbers.NextDouble() * sumVal;

            sumVal = 0.0;
            for (int i = 0; i < num; i++)
            {
                sumVal += 1 / list[i].Fitness;
                if (sumVal > randVal)
                {
                    //System.Console.WriteLine("Rand," + randVal + " ,Index," + i);
                    return i;
                }
            }
            return -1;
        }
        #endregion
    }

    public class AdjR2Selector : LargeSelector
    {        
        public AdjR2Selector()
        {
            _fitnessCriteria = FitnessCriteria.AdjustedR2;
        }       
    }

    public class R2Selector : LargeSelector
    {
        public R2Selector()
        {
            _fitnessCriteria = FitnessCriteria.R2;
        }
    }

    public class AICSelector : SmallSelector
    {        
        public AICSelector()
        {
            _fitnessCriteria = FitnessCriteria.Akaike;
        }        
    }

    public class AICCSelector : SmallSelector
    {        
        public AICCSelector()
        {
            _fitnessCriteria = FitnessCriteria.AICC;
        }        
    }

    public class BICSelector : SmallSelector
    {
        public BICSelector()
        {
            _fitnessCriteria = FitnessCriteria.BIC;
        }
    }

    public class PressSelector : SmallSelector
    {
        public PressSelector()
        {
            _fitnessCriteria = FitnessCriteria.Press;
        }
    }
    public class RMSESelector : SmallSelector
    {
        public RMSESelector()
        {
            _fitnessCriteria = FitnessCriteria.RMSE;
        }
    }

    public class SensitivitySelector : LargeSelector
    {
        public SensitivitySelector()
        {
            _fitnessCriteria = FitnessCriteria.Sensitivity;
        }
    }

    public class SpecificitySelector : LargeSelector
    {
        public SpecificitySelector()
        {
            _fitnessCriteria = FitnessCriteria.Specificity;
        }
    }

    public class AccuracySelector : LargeSelector
    {
        public AccuracySelector()
        {
            _fitnessCriteria = FitnessCriteria.Accuracy;
        }
    }
}
