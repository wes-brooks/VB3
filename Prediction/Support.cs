using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VBCommon;

namespace Prediction
{
    public class Support
    {                
        /// <summary>
        /// create a model expression string for display
        /// </summary>
        /// <param name="model"></param>
        /// <param name="depVarname"></param>
        /// <returns>model expression as a string (depvar = i + c(j)x(j)...)</returns>
        static public string BuildModelExpression(Dictionary<string, double> model, string depVarname, string format)
        {
            string strNumFormat = "";
            if (String.IsNullOrWhiteSpace(format))
                strNumFormat = "g8";
            else
                strNumFormat = format;

            //generates the model expression
            string strOpsign = string.Empty;
            string strVariable = string.Empty;
            double dblCoeff = double.NaN;
            string strLine = "";
            
            if (!String.IsNullOrWhiteSpace(depVarname))
                strLine = depVarname + " = ";

            if (model.Keys.Count < 0) return strLine;

            foreach (KeyValuePair<string, double> mterm in model)
            {
                strVariable = mterm.Key;
                dblCoeff = mterm.Value;
                if (dblCoeff < 0) strOpsign = " - ";
                else strOpsign = " + ";
                if (strVariable.Contains("(Intercept)"))
                    strLine = strLine + dblCoeff.ToString(strNumFormat);
                else
                {
                    dblCoeff = Math.Abs(dblCoeff);
                    strLine = strLine + strOpsign + dblCoeff.ToString(strNumFormat) +
                        "*[" + strVariable + "]";
                }
            }

            strLine = strLine.Replace("[", "(");
            strLine = strLine.Replace("]", ")");
            return strLine;
        }


        static public bool IsMainEffect(string columnName, DataTable dt)
        {
            DataColumn dc = null;
            if (dt.Columns.Contains(columnName) == false)
                return false;

            dc = dt.Columns[columnName];

            if (dc.ExtendedProperties.ContainsKey(VBCommon.Globals.MAINEFFECT) == false)
                return false;

            string strSval = dc.ExtendedProperties[VBCommon.Globals.MAINEFFECT].ToString();
            bool boolRetVal = false;
            if (String.Compare(strSval, "true", true) == 0)
                boolRetVal = true;

            return boolRetVal;
        }


        static public bool IsNearZero(double val)
        {
            double dblNearzero = 1.0e-21;
            double dblNegnearzero = -1.0e-21;

            if ((val < dblNearzero) && (val > dblNegnearzero))
                return true;
            else
                return false;
        }
    }
}
