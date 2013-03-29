using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Ciloci.Flee;


namespace MLRPrediction
{
    public class ExpressionEvaluator
    {
        private static DataTable MyTable;
        private static DataRow MyCurrentRow;


        //public List<double> Evaluate(string expression, DataTable dt)
        public DataTable Evaluate(string expression, DataTable dt)//, bool row0Only)
        {
            List<double> calcValues = new List<double>();
            DataTable dtCalcValues = new DataTable();
            dtCalcValues.Columns.Add("ID", typeof(string));
            dtCalcValues.Columns.Add("CalcValue", typeof(double));

            MyTable = dt;

            ExpressionContext context = new ExpressionContext();
            // Use string.format
            context.Imports.AddType(typeof(string));            
            context.Imports.AddType(typeof(CustomFunctions));

            // Use on demand variables to provide the values for the columns
            context.Variables.ResolveVariableType += new EventHandler<ResolveVariableTypeEventArgs>(Variables_ResolveVariableType);
            context.Variables.ResolveVariableValue += new EventHandler<ResolveVariableValueEventArgs>(Variables_ResolveVariableValue);

            // Create the expression; Flee will now query for the types of ItemName, Price, and Tax
            IDynamicExpression e = context.CompileDynamic(expression);
            
            Console.WriteLine("Computing value of '{0}' for all rows", e.Text);

            DataRow dr = null;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                MyCurrentRow = dt.Rows[i];
                // Evaluate the expression; Flee will query for the values of the columns
                double result = (double) e.Evaluate();
                calcValues.Add(result);

                dr = dtCalcValues.NewRow();
                dr[0] = MyCurrentRow[0] as string;
                dr[1] = result;
                dtCalcValues.Rows.Add(dr);
                Console.WriteLine("Row {0} = {1}", i, result);
                //if (row0Only) break;
            }

            //return calcValues;
            return dtCalcValues;
        }


        /// <summary>
        /// Used to evaluate the mlr model variable that could be decomposed, transformed and/or interacted
        /// with other variables in the input grid.  Purpose of method is to get vector of dependent variable
        /// values for use in probability of exceedance calculation for the prediction.  If the mlr variable 
        /// in "expression" is simply a main effect, the variable value for that is returned.
        /// </summary>
        /// <param name="expression">a string representation of the mlr variable</param>
        /// <param name="dr">a table row of main effect values from the input grid</param>
        /// <returns>a double value for the mlr variable</returns>
        public double Evaluate(string expression, DataRow dr)
        {
            double result = double.NaN;
 
            ExpressionContext context = new ExpressionContext();
            // Use string.format
            context.Imports.AddType(typeof(string));
            context.Imports.AddType(typeof(CustomFunctions));

            // Use on demand variables to provide the values for the columns
            context.Variables.ResolveVariableType += new EventHandler<ResolveVariableTypeEventArgs>(Variables_ResolveVariableType);
            context.Variables.ResolveVariableValue += new EventHandler<ResolveVariableValueEventArgs>(Variables_ResolveVariableValue);

            // Create the expression; Flee will now query for the types of ItemName, Price, and Tax
            IDynamicExpression e = context.CompileDynamic(expression);

            MyCurrentRow = dr;
            result = (double)e.Evaluate();
            //return calcValues;
            return result;
        }

        static void Variables_ResolveVariableType(object sender, ResolveVariableTypeEventArgs e)
        {
            // Simply supply the type of the column with the given name
            e.VariableType = MyTable.Columns[e.VariableName].DataType;
        }

        static void Variables_ResolveVariableValue(object sender, ResolveVariableValueEventArgs e)
        {
            // Supply the value of the column in the current row
            e.VariableValue = MyCurrentRow[e.VariableName];
        }

        static public string[] GetReferencedVariables(string expression, string[] variables)
        {
            if (String.IsNullOrWhiteSpace(expression))
                return null;

            if ((variables == null) ||(variables.Length < 1))
                return null;

            ExpressionContext context = new ExpressionContext();
            // Use string.format
            context.Imports.AddType(typeof(string));
            context.Imports.AddType(typeof(CustomFunctions));

            for (int i=0;i<variables.Length;i++)
                context.Variables[variables[i]] = 1.0;

            IDynamicExpression e = context.CompileDynamic(expression);
            return e.Info.GetReferencedVariables();

        }
    }
}
