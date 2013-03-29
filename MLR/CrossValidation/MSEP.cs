using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GALib;

namespace CrossValidation
{
    //Class to associate an MSEP with a model
    public class MSEP
    {
        private IIndividual _model;
        private double _msep;
        private string[] _indepVars = null;

        public MSEP()
        {
            _model = null;
            _msep = Double.NaN;
            _indepVars = null;
        }

        public MSEP(IIndividual model, double msep, string[] independentVariables)
        {
            _model = model;            
            _msep = msep;
            _indepVars = independentVariables;
        }

        public IIndividual Model
        {
            get { return _model; }
            set { _model = value; }
        }

        public double msep
        {
            get { return _msep; }
            set { _msep = value; }
        }

        public string[] IndependentVariables
        {
            get { return _indepVars; }
            set { _indepVars = value; }
        }
    }
}
