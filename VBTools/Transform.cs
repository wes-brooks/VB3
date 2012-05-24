using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VBTools
{
    public enum DependentVariableTransforms
    {
        none,
        Log10,
        Ln,
        Power
    }


    [Serializable]
    public class Transform
    {
        private DependentVariableTransforms _transformType;
        private double _dblExponent;

        public Transform() { }

        public Transform(DependentVariableTransforms type)
        {
            this._transformType = type;
            this._dblExponent = 1;
        }

        public Transform(DependentVariableTransforms type, double exponent)
        {
            this._transformType = type;
            this._dblExponent = exponent;
        }

        public DependentVariableTransforms Type
        {
            get { return _transformType; }
            set { _transformType = value; }
        }

        public double Exponent
        {
            get { return _dblExponent; }
            set { _dblExponent = value; }
        }
    }
}
