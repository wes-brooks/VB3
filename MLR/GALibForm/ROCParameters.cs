using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GALibForm
{
    /// <summary>
    /// class for storage/retrieval of roc table information
    /// </summary>
    public class ROCParameters
    {
        private double _decisionThreshold;
        private double _sensitivity;
        private double _specificity;
        private double _accuracy;
        private int _falsePosCount;
        private int _falseNegCount;
        private object[] _rocParameters;

        /// <summary>
        /// default constructor
        /// </summary>
        public ROCParameters()
        {
        }

        /// <summary>
        ///constructor for easy data retrieval
        /// </summary>
        /// <param name="parameters">object array containing roc table information</param>
        public ROCParameters(object[] parameters)
        {
            _decisionThreshold = (double)parameters[0];
            _sensitivity = (double)parameters[1];
            _specificity = (double)parameters[2];
            _falsePosCount = (int)parameters[3];
            _falseNegCount = (int)parameters[4];
            _accuracy = (double)parameters[5];
        }

        /// <summary>
        /// constructor for saving roc table information
        /// </summary>
        /// <param name="dct">decision threshold, double</param>
        /// <param name="sens">sensitivity of model at threshold, double</param>
        /// <param name="spec">specificity of model at threshold</param>
        /// <param name="fpc">false positive count for model at threshold, int</param>
        /// <param name="fnc">false negative count for model at threshold</param>
        public ROCParameters(double dct, double sens, double spec, int fpc, int fnc, double accuracy)
        {
            _decisionThreshold = dct;
            _sensitivity = sens;
            _specificity = spec;
            _falsePosCount = fpc;
            _falseNegCount = fnc;
            _accuracy = accuracy;
            _rocParameters = new object[6];
            _rocParameters[0] = _decisionThreshold;
            _rocParameters[1] = _sensitivity;
            _rocParameters[2] = _specificity;
            _rocParameters[3] = _falsePosCount;
            _rocParameters[4] = _falseNegCount;
            _rocParameters[5] = _accuracy;
        }

        #region ROC curve information properties
        /// <summary>
        /// threshold accessor (double)
        /// </summary>
        public double DecisionThreshold
        {
            set { _decisionThreshold = value; }
            get { return _decisionThreshold; }
        }
        /// <summary>
        /// sensitivity accessor (double)
        /// </summary>
        public double Sensitivity
        {
            set { _sensitivity = value; }
            get { return _sensitivity; }
        }
        /// <summary>
        /// specificity accessor (double)
        /// </summary>
        public double Specificity
        {
            set { _specificity = value; }
            get { return _specificity; }
        }
        /// <summary>
        /// false positive accessor (int)
        /// </summary>
        public int FalsePosCount
        {
            set { _falsePosCount = value; }
            get { return _falsePosCount; }
        }
        /// <summary>
        /// false negative accessor (int)
        /// </summary>
        public int FalseNegCount
        {
            set { _falseNegCount = value; }
            get { return _falseNegCount; }
        }
        public double Accuracy
        {
            set { _accuracy = value; }
            get { return _accuracy; }
        }
        /// <summary>
        /// returns an object array for all roc parameters
        /// in threshold, sensitivity, specificity, fp count and fn count order
        /// </summary>
        public object ROCPars
        {
            get { return _rocParameters; }
        }
        #endregion


    }


}
