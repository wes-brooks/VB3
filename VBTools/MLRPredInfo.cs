using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPyModeling;

namespace VBTools
{
    [Serializable]
    public class PredInfo
    {
        private string _predVariables = "";
        private string _predObs = "";
        private string _predStats = "";

        private double _decCritThreshold = double.NaN;
        private double _regStdThreshold = double.NaN;

        private Globals.DependentVariableTransforms _transform = Globals.DependentVariableTransforms.none;
        private double _powerTrans = double.NaN;

        private IPyModeling.ModelState ipyModelState;

        public PredInfo()
        {

        }

        public string IVData
        {
            get { return _predVariables; }
            set { _predVariables = value; }
        }

        public string ObsData
        {
            get { return _predObs; }
            set { _predObs = value; }
        }

        public string StatData
        {
            get { return _predStats; }
            set { _predStats = value; }
        }

        public double DecisionCriteria
        {
            get { return _decCritThreshold; }
            set { _decCritThreshold = value; }
        }

        public double RegulatoryStandard
        {
            get { return _regStdThreshold; }
            set { _regStdThreshold = value; }
        }

        public Globals.DependentVariableTransforms DependentVariableTransform
        {
            get { return _transform; }
            set { _transform = value; }
        }

        public double PowerTransform
        {
            get { return _powerTrans; }
            set { _powerTrans = value; }
        }

        public IPyModeling.ModelState ModelState
        {
            get { return ipyModelState; }
            set { ipyModelState = value; }
        }
    }
}
