using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VBCommon;
using System.Collections;

namespace MLRCore
{
    public class Support
    {
        private static List<string> _modelTokens = null;

        public static List<string> ModelTokens
        {
            set { _modelTokens = value; }
            get { return _modelTokens; }
        }
        /// <summary>
        /// create a model expression string for display
        /// </summary>
        /// <param name="model"></param>
        /// <param name="depVarname"></param>
        /// <returns>model expression as a string (depvar = i + c(j)x(j)...)</returns>
        static public string BuildModelExpression(Dictionary<string, double> model, string depVarname, string format)
        {
            string numFormat = "";
            if (String.IsNullOrWhiteSpace(format))
                numFormat = "g8";
            else
                numFormat = format;

            //generates the model expression
            string opsign = string.Empty;
            string variable = string.Empty;
            double coeff = double.NaN;
            string line = "";
            
            if (!String.IsNullOrWhiteSpace(depVarname))
                line = depVarname + " = ";

            if (model.Keys.Count < 0) return line;

            _modelTokens = new List<string>();

            foreach (KeyValuePair<string, double> mterm in model)
            {
                variable = mterm.Key;
                coeff = mterm.Value;
                if (coeff < 0) opsign = " - ";
                else opsign = " + ";
                if (variable.Contains("(Intercept)"))
                {
                    line = line + coeff.ToString(numFormat);
                    _modelTokens.Add(coeff.ToString());
                }
                else
                {
                    coeff = Math.Abs(coeff);
                    line = line + opsign + coeff.ToString(numFormat) +
                        "*[" + variable + "]";
                    _modelTokens.Add(coeff.ToString() + "*(" + variable + ")");
                }
            }

            line = line.Replace("[", "(");
            line = line.Replace("]", ")");

            return line;
        }


        static public bool IsMainEffect(string columnName, DataTable dt)
        {
            DataColumn dc = null;
            if (dt.Columns.Contains(columnName) == false)
                return false;

            dc = dt.Columns[columnName];

            if (dc.ExtendedProperties.ContainsKey(VBCommon.Globals.MAINEFFECT) == false)
                return false;

            string sval = dc.ExtendedProperties[VBCommon.Globals.MAINEFFECT].ToString();
            bool retVal = false;
            if (String.Compare(sval, "true", true) == 0)
                retVal = true;

            return retVal;
            
        }

        static public bool IsNearZero(double val)
        {
            double nearzero = 1.0e-21;
            double negnearzero = -1.0e-21;

            if ((val < nearzero) && (val > negnearzero))
                return true;
            else
                return false;

        }

         /// <summary>
            /// get a row of mlr model variable values from a row of prediction input grid table main effects
            /// </summary>
            /// <param name="drComponentXs">a table row from the input grid table</param>
            /// <returns>a vector (row) of mlr variable values</returns>
        public static DataRow getMLRxVals(DataRow drComponentXs, Dictionary<string, double> model)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(string));
            DataRow dr = dt.NewRow();
            dr["ID"] = drComponentXs[0];
           //inx = 1;
            foreach (KeyValuePair<string, double> kvp in model)
            {
                if (kvp.Key == "(Intercept)") continue;
                //the parser confuses functions with variables when brackets encountered in expressions 
                string exp = kvp.Key.Replace('[', '(');
                exp = exp.Replace(']', ')');
                ExpressionEvaluator expEval = new ExpressionEvaluator();
                dt.Columns.Add(exp, typeof(double));
                dr[exp] = expEval.Evaluate(exp, drComponentXs);
            }

            dt.Rows.Add(dr);


            return dt.Rows[0];
        }

        /// <summary>
        /// get a table of model variables and values out of the pool of protential variables/values
        /// </summary>
        /// <returns>table containing only model variable values</returns>
        public static DataTable getModelDatatable()
        {
            //get the datatable of potential model variables
            MLRDataManager dm = MLRDataManager.GetDataManager();
            DataTable dt = dm.ModelDataTable;
            DataView dv = dt.DefaultView;

            //get a list of variables in the model
            Dictionary<string, double> varsInModel = dm.Model;
            List<string> listVarsInModel = varsInModel.Keys.ToList<string>();

            //remove the intercept term
            listVarsInModel.RemoveAt(0);
                
            //create a table of model variable values only
            return dv.ToTable("ModelData", false, listVarsInModel.ToArray());

        }
    }
}
