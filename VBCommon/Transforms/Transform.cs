using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace VBCommon.Transforms
{
    public enum DependentVariableTransforms
    {
        none,
        Log10,
        Ln,
        Power
    }


    public static class Apply
    {
        public static double UntransformThreshold(double Value, DependentVariableTransforms Transform, double Exponent = 1)
        {
            if (Transform == DependentVariableTransforms.none)
                return Value;
            else if (Transform == DependentVariableTransforms.Log10)
                return Math.Pow(10, Value);
            else if (Transform == DependentVariableTransforms.Ln)
                return Math.Exp(Value);
            else if (Transform == DependentVariableTransforms.Power)
                return Math.Sign(Value) * Math.Pow(Math.Abs(Value), 1 / Exponent);
            else
                return Value;
        }


        public static double TransformThreshold(double Value, DependentVariableTransforms Transform, double Exponent = 1)
        {
            if (Transform == DependentVariableTransforms.none)
                return Value;
            else if (Transform == DependentVariableTransforms.Log10)
                return Math.Log10(Value);
            else if (Transform == DependentVariableTransforms.Ln)
                return Math.Log(Value);
            else if (Transform == DependentVariableTransforms.Power)
                return Math.Pow(Math.Abs(Value), Exponent);
            else
                return Value;
        }


        public static double TransformThreshold(double Value, string Transform, double Exponent = 1)
        {
            if (Transform == DependentVariableTransforms.none.ToString())
                return Value;
            else if (Transform == DependentVariableTransforms.Log10.ToString())
                return Math.Log10(Value);
            else if (Transform == DependentVariableTransforms.Ln.ToString())
                return Math.Log(Value);
            else if (Transform == DependentVariableTransforms.Power.ToString())
                return Math.Pow(Value, Exponent);
            else
                return Value;
        }
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

        [JsonProperty]
        public DependentVariableTransforms Type
        {
            get { return _transformType; }
            set { _transformType = value; }
        }

        [JsonProperty]
        public double Exponent
        {
            get { return _dblExponent; }
            set { _dblExponent = value; }
        }
    }
}
