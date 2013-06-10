using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;

namespace VBCommon.Transforms
{
    public class Transformer
    {
        //perform VB2 data transformations on a data collection

        //VB2 transformations have specific processing for data values and are NOT (in most cases) 
        //pure mathematical transformations.

        // SQUAREROOT transforms are the signed equivalent of the mathematical function:
        //      i.e. sqr(x) == sqr(x) for x > 0 but sqr(-x) == -srq(x) for x < 0

        // LOG10 and LOGE transforms are signed equivalent (like squareroot) of the mathematical functions:
        //      i.e. log(-x) == -log(x) and
        //          log10(-x) == -log10(x)
        //      additionally, if (-1 <= x <= 1), then log(x) == 0 and log10(x) == 0

        // INVERSE transforms also have special processing:
        //      if x == 0 (or in these cases < nearzero) then 1/x == 1/next smallest closest-to-zero value
        //      in the data collection
        // change 2/2010
        //  if x == 0 then 1/x == (1/next smallest-to-zero value)/2
        //  also: iff number of xes == 1 (eval of inverse on single value as in prediction) then we need to filter
        //  the entire data set used to build the model to find "next-smallest-to-zero-value"

        // SQUARE transform is what you'd expect; sq(x) == x*x for all x
        //  uhh, no it's not - now a signed square as in sqr and log transforms
        //  sq(x) == x*x and sq(-x) == -(x*x) (1/26/2010 via MC)

        //new rule: if zero count > 10% the number of values then cannot perform INV, LOG or LN transform
        //via MC 6/2010

        //see Mike Cyterski, VB2 PI for a validation/explanation of these rules.

        public const double nearzero = 1.0e-21;
        private double[] _data = null;
        private string _message = string.Empty;
        private double _exponent = double.NaN;
        private double _minnonzero = double.NaN;
        

        public Transformer() { }


        public Transformer(double value)
        {
            _data = new double[1];
            _data[0] = value;
        }


        public Transformer(double[] data)
        {
            _data = data;
        }


        public Transformer(DataTable dt, int colndx, double exponent)
        {
            int r = -1;
            _data = new double[dt.Rows.Count];
            _exponent = exponent;
            //get the data out of the specified dt[colndx] and save in _data
            try
            {

                for (r = 0; r < dt.Rows.Count; r++)
                {
                    _data[r] = Convert.ToDouble(dt.Rows[r][colndx].ToString());
                }
            }
            catch (Exception e)
            {
                _message = "Error accessing data in datatable; attempting read at position " + r + " with data "
                    + dt.Rows[r][colndx].ToString() + " in column index " + colndx + ": " + e.Message;
            }
        }


        public Transformer(DataTable dt, int colndx)
        {
            int r = -1;
            _data = new double[dt.Rows.Count];
            //get the data out of the specified dt[colndx] and save in _data
            try
            {

                for (r = 0; r < dt.Rows.Count; r++)
                {
                    _data[r] = Convert.ToDouble(dt.Rows[r][colndx].ToString());
                }
            }
            catch (Exception e)
            {
                _message = "Error accessing data in datatable; attempting read at position " + r + " with data "
                    + dt.Rows[r][colndx].ToString() + " in column index " + colndx + ": " + e.Message;
            }
        }
        public Transformer(DataTable dt, string colname)
        {
            int r = -1;
            _data = new double[dt.Rows.Count];
            //get the data out of the specified dt[colname] and save in _data
            try
            {
                for (r = 0; r < dt.Rows.Count; r++)
                {
                    _data[r] = Convert.ToDouble(dt.Rows[r][colname].ToString());
                }
            }
            catch (Exception e)
            {
                _message = "Error accessing data in datatable; attempting read at position " + r + " with data "
                    + dt.Rows[r][colname].ToString() + " in column name " + colname + ": " + e.Message;
            }
        }


        public double MinNonZero
        {
            get { return _minnonzero; }
        }


        public double Exponent
        {
            get { return _exponent; }
        }


        public string Message
        {
            get { return _message; }
        }


        public double[] INVERSE
        {
            get { return transformInverse(); }
        }


        public double[] LOG10
        {
            get { return transformLog10(); }
        }


        public double[] LOGE
        {
            get { return transformLogE(); }
        }


        public double[] SQUAREROOT
        {
            get { return transformSquareRoot(); }
        }


        public double[] SQUARE
        {
            get { return transformSquare(); }
        }


        public double[] QUARTICROOT
        {
            get { return transformQuarticRoot(); }
        }


        public double[] NONE
        {
            get { return _data; }
        }


        public double[] REALSQUARE
        {
            //cuz sometimes you just want one
            get { return transformRealSquare(); }
        }


        public double[] POWER
        {
            get {
                if (_exponent == -1)
                {
                    _message = string.Empty;
                    return transformInverse();
                }
                else
                {
                    _message = string.Empty;
                    return transformPower();
                }
            }
        }


        private double[] transformPower()
        {
            double[] tarray = new double[_data.Length];

            for (int i = 0; i < _data.Length; i++)
            {
                try
                {
                    tarray[i] = Math.Sign(_data[i]) * Math.Pow(Math.Abs(_data[i]), _exponent);
                }
                catch (Exception e)
                {
                    _message = "Cannot transform Power( value = " + _data[i] + " ) at position " + i + ": " + e.Message;
                    return null;
                }
            }
            return tarray;
        }


        private double[] transformRealSquare()
        {
            double[] tarray = new double[_data.Length];
            for (int i = 0; i < _data.Length; i++)
            {
                try
                {
                    tarray[i] = Math.Pow(_data[i], 2.0d);
                    //change 1/26/2010
                    //tarray[i] = Math.Sign(_data[i]) * Math.Pow(_data[i], 2.0d);
                }
                catch (Exception e)
                {
                    _message = "Cannot transform RealSquare( value = " + _data[i] + " ) at position " + i + ": " + e.Message;
                    return null;
                }
            }
            return tarray;
        }


        private double[] transformInverse()
        {
            double [] tarray = new double[_data.Length];
            double minNonZero = double.NaN;
            bool skiptrans = zeroCount();
            if (! skiptrans)
            {
                minNonZero = closestToZero();
                _minnonzero = minNonZero;

                for (int i = 0; i < _data.Length; i++)
                {
                    try
                    {
                        if (Math.Abs(Convert.ToDouble(_data[i])) < nearzero)
                        {
                            tarray[i] = 1.0d / minNonZero;
                        }
                        else
                        {
                            tarray[i] = 1.0d / _data[i];
                        }
                    }

                    catch (Exception e)
                    {
                        //Console.WriteLine("Trouble transforming " + newcolname + " skipping..." + e.Message);
                        _message = "Cannot transform Inverse( value = " + _data[i] + " ) at position " + i + ": " + e.Message;
                        return null;
                    }
                }
            }
            return tarray;
        }


        private double[] transformLogE()
        {
            double[] tarray = new double[_data.Length];
            bool skiptrans = zeroCount();
            if (!skiptrans)
            {
                for (int i = 0; i < _data.Length; i++)
                {
                    try
                    {
                        if (Math.Abs(_data[i]) < 1.0d)
                        {
                            tarray[i] = 0.0d;
                        }
                        else
                        {
                            tarray[i] = Math.Sign(_data[i]) * Math.Log(Math.Abs(_data[i]));
                        }
                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine("Trouble transforming " + newcolname + " skipping..." + e.Message);
                        _message = "Cannot transform LogE( value = " + _data[i] + " ) at position " + i + ": " + e.Message;
                        return null;
                    }
                }
            }
            return tarray;
        }


        private double[] transformLog10()
        {
            double[] tarray = new double[_data.Length];
            bool skiptrans = zeroCount();
            if (!skiptrans)
            {
                for (int i = 0; i < _data.Length; i++)
                {
                    try
                    {
                        if (Math.Abs(_data[i]) < 1.0d)
                        {
                            tarray[i] = 0.0d;
                        }
                        else
                        {
                            tarray[i] = Math.Sign(_data[i]) * Math.Log10(Math.Abs(_data[i]));
                        }
                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine("Trouble transforming " + newcolname + " skipping..." + e.Message);
                        _message = "Cannot transform Log10( value  = " + _data[i] + " ) at position " + i + ": " + e.Message;
                        return null;
                    }
                }
            }
            return tarray;
        }


        private double[] transformSquare()
        {
            double [] tarray = new double[_data.Length];
            for (int i = 0; i < _data.Length; i++)
            {
                try
                {
                    //tarray[i] = Math.Pow(_data[i], 2.0d);
                    //change 1/26/2010
                    tarray[i] = Math.Sign(_data[i]) * Math.Pow(_data[i], 2.0d);
                }
                catch (Exception e)
                {
                    _message = "Cannot transform Square( value = " + _data[i] + " ) at position " + i + ": " + e.Message;
                    return null;
                }
            }
            return tarray;
        }


        private double[] transformQuarticRoot()
        {
            double[] tarray = new double[_data.Length];
            for (int i = 0; i < _data.Length; i++)
            {
                try
                {
                    //tarray[i] = Math.Pow(_data[i], 2.0d);
                    //change 1/26/2010
                    tarray[i] = Math.Sign(_data[i]) * Math.Pow(Math.Abs (_data[i]), 0.25d);
                }
                catch (Exception e)
                {
                    _message = "Cannot quarticroot transform ( value = " + _data[i] + " ) at position " + i + ": " + e.Message;
                    return null;
                }
            }
            return tarray;
        }


        private double[] transformSquareRoot()
        {
            double [] tarray = new double[_data.Length];
            for (int i = 0; i < _data.Length; i++)
            {
                try
                {
                    tarray[i] = Math.Sign(_data[i]) * Math.Sqrt(Math.Abs(_data[i]));
                }
                catch (Exception e)
                {
                    _message = "Cannot transform SquareRoot( value = " + _data[i] + " ) at position " + i + ": " + e.Message;
                    return null;
                }
            }
            return tarray;
        }
        

        public double INVERSEsv(double input)
        {
            //inverse transform of single values but need dataset used in model to get min-non-zero value
            double minNonZero = double.NaN;
            double retval = double.NaN;
            minNonZero = closestToZero();
            try
            {
                if (Math.Abs(Convert.ToDouble(input)) < nearzero)
                {
                    retval = 1.0d / minNonZero;
                }
                else
                {
                    retval = 1.0d / input;
                }
            }

            catch (Exception e)
            {
                _message = "Cannot transform Inverse( value = " + input + " ) " + e.Message;
                return double.NaN;
            }
            return retval;
        }


        private double closestToZero()
        {
            //don't want minimum; want value closest to zero
            double negmax;
            double posmin;

            try
            {
                var posminval = (from x in _data
                                 where (double)x > 0.0
                                 select x).Min();
                posmin = (double)posminval;
            }
            catch (InvalidOperationException)
            {
                posmin = double.NaN;
            }

            try
            {
                var negmaxval = (from x in _data
                                 where (double)x < 0.0
                                 select x).Max();
                negmax = (double)negmaxval;
            }
            catch (InvalidOperationException)
            {
                negmax = double.NaN;
            }

            if (! posmin.Equals(double.NaN) && ! negmax.Equals(double.NaN))
            {
                return posmin < Math.Abs(negmax) ? posmin/2.0d : negmax/2.0d;
            }
            else if (posmin.Equals(double.NaN) && ! negmax.Equals(double.NaN))
            {
                return negmax/2.0d;
            }
            else if (! posmin.Equals(double.NaN) && negmax.Equals(double.NaN))
            {
                return posmin/2.0d;
            }
            else { return nearzero/2.0d; }
        }


        private bool zeroCount()
        {
            //count zeros in dataset and return true if count/observations > .1
            int count = _data.Count(n => n == 0);
            if (((double)count / (double)_data.Length) > .1)
            {
                _message = "Zero count exceeds maximum (>10% row count).";
                return true;
            }
            else
            {
                _message = "";
                return false;

            }
        }        
    }
}
