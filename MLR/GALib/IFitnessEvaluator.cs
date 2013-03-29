using System;
using System.Collections.Generic;
using System.Text;

namespace GALib
{
    public interface IFitnessEvaluator
    {
        /// <summary>
        /// Evaluate all fitness measures for this individual
        /// </summary>
        /// <param name="individual"></param>
        /// <returns></returns>
        double evaluate(IIndividual individual);

        /// <summary>
        /// Evaluate only the specified fitness measure
        /// </summary>
        /// <param name="individual"></param>
        /// <param name="fitnessIndex"></param>
        /// <returns></returns>
        double evaluate(IIndividual individual, int fitnessIndex);
    }
}
