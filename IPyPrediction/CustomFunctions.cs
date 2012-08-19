using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VBCommon;

namespace IPyPrediction
{
    public static class CustomFunctions
    {
        // Declare a function that takes a variable number of arguments
        public static double PROD(params double[] args)
        {
            double dblProduct = 1.0;
            for (int i = 0; i < args.Length; i++)
                dblProduct *= args[i]; ;

            return dblProduct;
        }


        // Declare a function that takes a variable number of arguments
        public static double SUM(params double[] args)
        {
            double dblSum = 0.0;
            dblSum = args.Sum();
            return dblSum;
        }


        // Declare a function that takes a variable number of arguments
        public static double MIN(params double[] args)
        {
            double dblMin = Double.MaxValue;
            dblMin = args.Min();
            return dblMin;
        }


        // Declare a function that takes a variable number of arguments
        public static double MAX(params double[] args)
        {
            double dblMax = Double.MinValue; ;
            dblMax = args.Max();
            return dblMax;
        }


        // Declare a function that takes a variable number of arguments
        public static double MEAN(params double[] args)
        {
            double dblAvg = 0.0;
            dblAvg = args.Average();
            return dblAvg;
        }


        // Declare a function that takes a variable number of arguments
        public static double LOG10(double arg)
        {
            double dblRetVal = 0.0;
            if (Math.Abs(arg) < 1.0d)
                dblRetVal = 0.0;
            else
                dblRetVal = Math.Sign(arg) * Math.Log10(Math.Abs(arg));

            return dblRetVal;
        }


        // Declare a function that takes a variable number of arguments
        public static double LN(double arg)
        {
            double dblRetVal = 0.0;
            if (Math.Abs(arg) < 1.0d)
                dblRetVal = 0.0;
            else
                dblRetVal = Math.Sign(arg) * Math.Log(Math.Abs(arg));
            
            return dblRetVal;
        }


        // Declare a function that takes a variable number of arguments
        //public static double INVERSE(double min, double arg)
        public static double INVERSE(double arg, double min)
        {
            double dblRetVal = 0.0;
            if (Support.IsNearZero(arg))
                dblRetVal = Math.Pow(min, -1);
            else
                dblRetVal = Math.Pow(arg, -1);

            return dblRetVal;
        }


        // Declare a function that takes a variable number of arguments
        public static double SQUARE(double arg)
        {
            double dblRetVal = arg * arg;
            return dblRetVal;
        }


        // Declare a function that takes a variable number of arguments
        public static double SQUAREROOT(double arg)
        {
            double dblRetVal = Math.Sign(arg) * Math.Sqrt (Math.Abs(arg));
            return dblRetVal;
        }


        // Declare a function that takes a variable number of arguments
        public static double QUADROOT(double arg)
        {
            double dblRetVal = Math.Sign(arg) * Math.Pow(Math.Abs(arg), 0.25);
            return dblRetVal;
        }


        // Declare a function that takes a variable number of arguments
        //public static double POLYNOMIAL(double intercept, double xCoeff, double x2Coeff, double arg)
        public static double POLYNOMIAL(double arg, double intercept, double xCoeff, double x2Coeff)
        {
            double dblRetVal = intercept + (xCoeff * arg) + (x2Coeff * arg * arg);
            return dblRetVal;
        }


        //dup for name change
        public static double POLY(double arg, double intercept, double xCoeff, double x2Coeff)
        {
            double dblRetVal = intercept + (xCoeff * arg) + (x2Coeff * arg * arg);
            return dblRetVal;
        }


        // Declare a function that takes a variable number of arguments
        //public static double POWER(double exp, double arg)
        public static double POWER(double arg, double exp)
        {
            double dblRetVal = Math.Sign(arg) * Math.Pow(Math.Abs(arg), exp);
            return dblRetVal;
        }


        // Declare a function that takes a variable number of arguments
        public static double WindA_comp(double windDirection, double windMagnitude, double beachOrientation)
        {
            WindComponents wndCmp = new WindComponents(windMagnitude, windDirection, beachOrientation);
            double dblRetVal = wndCmp.dblUcomp;
            return dblRetVal;
        }


        // Declare a function that takes a variable number of arguments
        public static double WindO_comp(double windDirection, double windMagnitude, double beachOrientation)
        {
            WindComponents wndCmp = new WindComponents(windMagnitude, windDirection, beachOrientation);
            double dblRetVal = wndCmp.dblVcomp;
            return dblRetVal;
        }


        // Declare a function that takes a variable number of arguments
        public static double CurrentA_comp(double currentDirection, double currentMagnitude, double beachOrientation)
        {
            WaterCurrentComponents waterCmp = new WaterCurrentComponents(currentMagnitude, currentDirection, beachOrientation);
            double dblRetVal = waterCmp.dblUcomp;
            return dblRetVal;
        }


        // Declare a function that takes a variable number of arguments
        public static double CurrentO_comp(double currentDirection, double currentMagnitude, double beachOrientation)
        {
            WaterCurrentComponents waterCmp = new WaterCurrentComponents(currentMagnitude, currentDirection, beachOrientation);
            double dblRetVal = waterCmp.dblVcomp;
            return dblRetVal;
        }
    }
}
