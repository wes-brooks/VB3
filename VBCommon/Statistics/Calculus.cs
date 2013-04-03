using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using Extreme.Mathematics;
//using Extreme.Mathematics.Curves;
//using Extreme.Mathematics.Calculus;
//using Extreme.Mathematics.Algorithms;
using System.Data.Common;
using MathNet;

namespace VBCommon.Statistics
{
    public class Calculus
    {
        private double[] _X = null;
        private double[] _Y = null;
        private double _lowerbound = double.NaN;
        private double _upperbound = double.NaN;

        /// <summary>
        /// note that points passed here must be sorted min to max and all Xs unique...
        /// otherwise integrators will fail
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        public Calculus (double[] X, double[] Y, double lower, double upper)
        {
            _X = X;
            _Y = Y;
            _lowerbound = lower;
            _upperbound = upper;
        }

        private static double Integrand(double x)
        {
            //dummy ... fit a curve to data?.... seems wrong.
            //dont think a lookup will work either: integrator needs to interpolate
            //need to return a value for all x, not just my data points
            return x;
        }

        public double Integrate()
        {
            //double x;
            //RealFunction f = new RealFunction(Integrand);
            //AdaptiveIntegrator integrator = new AdaptiveIntegrator(f, 0, 1);
            //integrator.RelativeTolerance = 0.00001d;
            //integrator.ConvergenceCriterion = ConvergenceCriterion.WithinRelativeTolerance;
            //integrator.Integrate();
            double result = MathNet.Numerics.Integration.Integrate.OnClosedInterval(Integrand, 0, 1, 0.00001d);
            return result;
        }


        public double PieceWiseLinearCurveIntegrate()
        {
            var curve = MathNet.Numerics.Interpolation.Interpolate.LinearBetweenPoints(_X, _Y);
            return curve.Integrate(_upperbound) - curve.Integrate(_lowerbound); //(_lowerbound, _upperbound);
        }
    }
}
