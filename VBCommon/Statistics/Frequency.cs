using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Extreme.Statistics;
using Accord;
using Accord.Controls;
using Accord.Math;
using Accord.Statistics;

namespace VBCommon.Statistics
{
    public class Frequency
    {
        private double[] _freq = null;
        private double [] _lb = null;
        private double [] _ub = null;
        private double[] _center = null;

        public void getHist(double[] data, int bins)
        {
            Accord.Statistics.Visualizations.Histogram hist = new Accord.Statistics.Visualizations.Histogram(); //(data.Min(), data.Max(), bins);
            hist.Compute(data, numberOfBins:bins);

            _freq = new double[bins];
            _lb = new double[bins];
            _ub = new double[bins];
            _center = new double[bins];
            double inc = ( ( (data.Max() - data.Min()) / bins) / 2.0d);
            for (int bin = 0; bin < hist.Bins.Count; bin++)
            {
                _freq[bin] = Convert.ToInt32(hist.Bins[bin].Value);
                _lb[bin] = hist.Bins[bin].Range.Min;
                _ub[bin] = hist.Bins[bin].Range.Max;
                _center[bin] = _lb[bin] + inc;
            }
        }

        public double[] Counts
        {
            get { return _freq; }
        }
        public double[] LowerBounds
        {
            get { return _lb; }
        }
        public double[] UpperBounds
        {
            get { return _ub; }
        }
        public double [] Center
        {
         get { return _center; }
        }
    }
}
