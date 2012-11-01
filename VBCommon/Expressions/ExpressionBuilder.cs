using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using VBCommon;

namespace VBCommon.Expressions
{   
    public class Expression
    {
        private List<string> lstVariables;
        private Globals.Operations _operation;
        private string strExpressionString;

        public string ExpressionString
        {
            get { return strExpressionString; }
        }


        public Expression(Globals.Operations operation)
        {
            _operation = operation;
            lstVariables = new List<string>();
        }


        public Expression(Expression exp)
        {
            _operation = exp.Operation;
            lstVariables = exp.Variables.ToList();
            strExpressionString = exp.ExpressionString;
        }


        public List<string> Variables
        {
            get { return lstVariables; }
        }
        

        public Globals.Operations Operation
        {
            get { return _operation; }
        }
        

        public string SetOperation(Globals.Operations operation)
        {
            _operation = operation;
            UpdateString();
            return strExpressionString;
        }


        public string AddVariable(string variable)
        {
            lstVariables.Add(variable);
            UpdateString();
            return strExpressionString;
        }


        public string RemoveVariable(string variable)
        {
            lstVariables.Remove(variable);
            UpdateString();
            return strExpressionString;
        }


        //Update the expression string with current operation and variables.
        private void UpdateString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetEnumString(_operation));
            sb.Append("[");
            for (int i = 0; i < lstVariables.Count; i++)
            {
                if (i != 0)
                    sb.Append(",");
                sb.Append(lstVariables[i]);                
            }
            sb.Append("]");
            strExpressionString = sb.ToString();
        }


        private string GetEnumString(Enum enumVal)
        {
            return Convert.ToString(enumVal);
        }


        public override string ToString()
        {
            return strExpressionString;
        }
    }
}
