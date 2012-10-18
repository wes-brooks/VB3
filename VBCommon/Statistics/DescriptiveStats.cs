using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Accord;
using Accord.Math;
using Accord.Statistics;

namespace VBCommon.Statistics
{
    public class DescriptiveStats
    {
        //private NumericalVariable _stats = null;
        private double _max;
        private double _min;
        private double _mean;
        private double _stddev;
        private double _kurtosis;
        private double _skewness;
        private double _range;
        private double _variance;
        private double _sum;
        private int _count;
        private double _median;

        public void getStats(double[] data) 
        {
            _max = data.Max();
            _min = data.Min();
            _mean = data.Mean();
            _stddev = data.StandardDeviation();
            _kurtosis = data.Kurtosis();
            _skewness = data.Skewness();
            _range = data.Range().Length;
            _variance = data.Variance();
            _sum = data.Sum();
            _count = data.Count();
            _median = data.Median();

        }


        public double Max
        {
            get { return _max; }
        }
        public double Min
        {
            get { return _min; }
        }
        public double Mean
        {
            get { return _mean; }
        }
        public double StdDev
        {
            get { return _stddev; }
        }
        public double Kurtosis
        {
            get { return _kurtosis; }
        }
        public double Range
        {
            get { return _range; }
        }
        public double Skewness
        {
            get { return _skewness; }
        }
        public double Variance
        {
            get { return _variance; }
        }
        public double Sum
        {
            get { return _sum; }
        }
        public int Count
        {
            get { return _count; }
        }
        public double Median
        {
            get { return _median; }
        }
    }
}
