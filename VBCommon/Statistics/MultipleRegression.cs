using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Accord.Statistics;
using Accord.Math;
using Accord.Math.Decompositions;
//using Extreme.Statistics;
//using Extreme.Mathematics.LinearAlgebra;
//using Extreme.Statistics.Tests;

namespace VBCommon.Statistics
{
    public class MultipleRegression
    {
        private MultipleRegression _model;

        //private VariableCollection _data = null;
        private DataTable _dataTable = null;
        private string _dependentVar = "";
        private string[] _independentVars = null;
        private double _adjR2;
        private double _R2;
        private double _AIC;
        private double _AICC;
        private double _BIC;
        private double _Press;
        private double _RMSE;

        //private Dictionary<string, double> _parameters = null;
        private double[] _studentizedResiduals = null;
        private double[] _dffits = null;
        private double[] _cooks = null;
        private DataTable _parameters = null;
        private double[] _predictedValues = null;
        private double[] _observedValues = null;

        private double[] arrOutputData = null;
        private double[] arrInputData = null;
        private string strOutputName;
        private string strInputName;

        private Dictionary<string, double> _VIF = null;
        private double _maxVIF = 0;
        private string _maxVIFParameter = "";

        private double _ADresidPvalue = double.NaN;
        private double _ADresidNormStatVal = double.NaN;

        private double _WSresidPvalue = double.NaN;
        private double _WSresidNormStatVal = double.NaN;

        public MultipleRegression(DataTable dataTable, string dependentVariable, string[] independendVariables)
        {
            _dataTable = dataTable;
            _dependentVar = dependentVariable;
            _independentVars = independendVariables;
            //_data = new VariableCollection(dataTable);
        }


        public MultipleRegression(double[] OutputData, double[] InputData, string OutputName="", string InputName="")
        {
            arrOutputData = OutputData;
            arrInputData = InputData;

            strOutputName = OutputName;
            strInputName = InputName;

            _independentVars = new string[1] { InputName };

            //_model = new MultipleRegression(
        }


        public double R2
        {
            get { return _R2; }
        }

        public double AdjustedR2
        {
            get { return _adjR2; }
        }

        public double AIC
        {
            get { return _AIC; }
        }

        public double AICC
        {
            get { return _AICC; }
        }

        public double BIC
        {
            get { return _BIC; }
        }

        public double Press
        {
            get { return _Press; }
        }

        public double RMSE
        {
            get { return _RMSE; }
        }

        public double[] DFFITS
        {
            get { return _dffits; }
        }

        public double[] Cooks
        {
            get { return _cooks; }
        }

        public double[] StudentizedResiduals
        {
            get { return _studentizedResiduals; }
        }

        public DataTable Parameters
        {
            get { return _parameters; }
        }

        public double[] PredictedValues
        {
            get { return _predictedValues; }
        }

        public double[] ObservedValues
        {
            get { return _observedValues; }
        }

        public double ADResidPvalue 
        {
            get { return _ADresidPvalue; }
        }

        public double ADResidNormStatVal
        {
            get { return _ADresidNormStatVal; }
        }

        public double WSResidPvalue
        {
            get { return _WSresidPvalue; }
        }

        public double WSResidNormStatVal
        {
            get { return _WSresidNormStatVal; }
        }



        //public Dictionary<string, double> VIFs
        //{
        //    get { return _VIF; }
        //}

        public double MaxVIF
        {
            get { return _maxVIF; }
        }

        public void Compute()
        {
            // Next, create a VariableCollection from the data table:
            

            // Now create the regression model. Parameters are the name 
            // of the dependent variable, a string array containing 
            // the names of the independent variables, and the VariableCollection
            // containing all variables.
            //LinearRegressionModel model = new LinearRegressionModel(_dataTable, _dependentVar, _independentVars);
            double[][] input = new double[arrInputData.Length][]; // {arrInputData};
            double[,] input2 = new double[arrInputData.Length,2]; // {arrInputData};
            for(int i=0; i<arrInputData.Length; i++)
            {
                input[i] = new double[] {arrInputData[i]};

                input2[i,0] = 1;
                input2[i,1] = arrInputData[i];
            }

            string[] inputNames = new string[] {strInputName};

            Accord.Statistics.Analysis.MultipleLinearRegressionAnalysis M2 = new Accord.Statistics.Analysis.MultipleLinearRegressionAnalysis(inputs: input, outputs: arrOutputData, inputNames: inputNames, outputName: strOutputName, intercept: true);
            M2.Compute();

            /*
            //LogisticRegressionModel model = new LogisticRegressionModel(_dataTable, _dependentVar, _independentVars);            

            // We can set model options now, such as whether to include a constant:
            _model.NoIntercept = false;
            */
            // The Compute method performs the actual regression analysis.
            //_model.Compute();            
            _adjR2 = M2.RSquareAdjusted;
            _R2 = M2.RSquared;
            _RMSE = Math.Sqrt(M2.Table[M2.Table.Count - 2].MeanSquares);
            
            //_AIC = model.GetAkaikeInformationCriterion();
            //_BIC = model.GetBayesianInformationCriterion();            

            
            //Calculate the Corrected AIC
            double sse = M2.Table[M2.Table.Count-2].SumOfSquares;            
            int n = Convert.ToInt32(M2.Results.Length);
            double p = M2.CoefficientValues.Length;
            double[,] H = input2.Multiply((input2.Transpose().Multiply(input2)).Inverse().Multiply(input2.Transpose()));

            double[] SquaredResiduals = new double[M2.Results.Length];
            double[] Residuals = new double[M2.Results.Length];
            double[] Leverage = new double[M2.Results.Length];
            for (int i=0; i<M2.Results.Length; i++)
            {
                SquaredResiduals[i] = Math.Pow(M2.Outputs[i] - M2.Results[i], 2);
                Residuals[i] = M2.Outputs[i] - M2.Results[i];
                Leverage[i] = H[i,i];
            }

            double SSR = SquaredResiduals.Sum();
            double sigma = SSR/(n-p);

            double[] ExternallyStudentizedResiduals = new double[M2.Results.Length];
            _dffits = new double[M2.Results.Length];
            _cooks = new double[M2.Results.Length];
            for (int i=0; i<M2.Results.Length; i++)
            {
                ExternallyStudentizedResiduals[i] = (SSR - SquaredResiduals[i])/(n-p-1);
                _dffits[i] = ExternallyStudentizedResiduals[i] * Math.Sqrt(H[i, i] * (1 - H[i, i]));
                _cooks[i] = SquaredResiduals[i] / (p * M2.Table[M2.Table.Count - 2].MeanSquares) * H[i, i] / (1 - H[i, i]);
            }

            _AIC = n * Math.Log(sse / n) + (2 * p) + n + 2;
            //_AICC = 1 + (Math.Log(sse / n)) + (2)*(p + 1)/ (n - p - 1);
            _AICC = _AIC + (2 * (p + 1) * (p + 2)) / (n - p - 2);
            _BIC = n * (Math.Log(sse / n)) + (p * Math.Log(n));

            
            _Press = 0.0;
            //GeneralVector vecLeverage = _model.GetLeverage();
            //GeneralVector vecResiduals = _model.Residuals;
            double leverage = 0.0;
            for (int i = 0; i < M2.Results.Length; i++)
            {
                leverage = Math.Min(Leverage[i], 0.99);
                //_Press += ((Residuals[i])* (Residuals[i])) / ((1 - Leverage[i]) * (1 - Leverage[i]));
                _Press += Math.Pow((Residuals[i]) / (1 - leverage),2);
            }

            
            _parameters = createParametersDataTable();
            DataRow dr = null;
            foreach (Accord.Statistics.Analysis.LinearRegressionCoefficient param in M2.Coefficients)
            {
                dr = _parameters.NewRow();
                dr["Name"] = param.Name;
                dr["Value"] = param.Value;
                dr["StandardError"] = param.StandardError;
                dr["TStatistic"] = param.Value / param.StandardError;
                dr["PValue"] = param.TTest.PValue;
                dr["StandardizedCoefficient"] = getStandardCoeff(param.Name, param.Value);
                _parameters.Rows.Add(dr);
            }

            
            _predictedValues = M2.Results;            
            _observedValues = M2.Outputs;            
            _studentizedResiduals = ExternallyStudentizedResiduals;

            Accord.Statistics.Distributions.Univariate.NormalDistribution distribution = new Accord.Statistics.Distributions.Univariate.NormalDistribution(0, 1);

            double[] standardizedResid = new double[M2.Results.Length];
            for (int i = 0; i < M2.Results.Length; i++)
            {
                standardizedResid[i] = (Residuals[i] - Residuals.Mean()) / Residuals.StandardDeviation();
            }
            Array.Sort(standardizedResid);


            double AD_stat = 0;
            for (int i = 0; i < M2.Results.Length; i++)
            {
                AD_stat += (2 * i + 1) * (Math.Log(distribution.DistributionFunction(standardizedResid[i])) + Math.Log(1-distribution.DistributionFunction(standardizedResid[n-1-i])));
            }

            AD_stat = -Convert.ToDouble(n) - AD_stat / Convert.ToDouble(n);

            //AndersonDarlingTest adtest = new AndersonDarlingTest((NumericalVariable)vecResiduals);
           
            
            //Extreme.Statistics.Tests.OneSampleTest ADtest = _model.GetNormalityOfResidualsTest(TestOfNormality.AndersonDarling);
            //Extreme.Statistics.Tests.OneSampleTest WStest = _model.GetNormalityOfResidualsTest(TestOfNormality.ShapiroWilk);
            //_ADresidPvalue = ADtest.PValue;
            _ADresidNormStatVal = AD_stat;
            //_WSresidPvalue = WStest.PValue;
            //_WSresidNormStatVal = WStest.Statistic;

            double[,] centered = new double[arrInputData.Length, _independentVars.Length];
            for (int i=0; i<M2.Results.Length; i++)
            {
                for(int j=0; j<_independentVars.Length; j++)
                {
                    centered[i, j] = input2[i, j + 1];
                }
            }


            double[,] matrix = input2.Transpose().Multiply(input2);

            //Extreme.Mathematics.LinearAlgebra.SymmetricMatrix matrix = _model.GetCorrelationMatrix();
            //Extreme.Mathematics.LinearAlgebra.SymmetricMatrix corrMatrix = new SymmetricMatrix(matrix.ColumnCount -1);

            //Extreme Opt returns a Correlation matrix that contains an extra row and column
            //Looks like these are related to the intercept
            //We are carving out the std correlation matrix
            //for (int row=1;row < matrix.ColumnCount; row++)
            //{
            //    for (int col=1;col < matrix.ColumnCount; col++)
            //    {
            //        corrMatrix[row-1,col-1] = matrix[row,col];
            //    }
            //}           
            
            double[,] corrMatrix = matrix;
            double[,] InvCorrMatrix = corrMatrix.Inverse();
            
            /*Extreme.Mathematics.Matrix InvCorrMatrix = corrMatrix.GetInverse();
            Extreme.Mathematics.Vector VIFVector = InvCorrMatrix.GetDiagonal();
            Extreme.Mathematics.Vector vifs = InvCorrMatrix.GetDiagonal().ToArray();

            _VIF = new Dictionary<string, double>();
            for (int i = 0; i < vifs.Count(); i++)
                _VIF.Add(_independentVars[i].ToString(), vifs.GetValue(i));

            _maxVIF = VIFVector.AbsoluteMax();
            _maxVIFParameter = _independentVars[VIFVector.AbsoluteMaxIndex()];           */      
      

        }

        private double getStandardCoeff(string paramName, double coeff)
        {
            //throw new NotImplementedException();
            if (paramName == "(Intercept)") return double.NaN;
            //double[] nv = new double[] (_dataTable.Columns[_dependentVar]);
            //double stdevY = nv.StandardDeviation;
            //nv = new NumericalVariable(_dataTable.Columns[paramName]);
            //double stdevX = nv.StandardDeviation;
            //return coeff * stdevX / stdevY;
            return (0);
        }

        //public double Predict(DataRow independentValues)
        //{
        //    double[] indVals = new double[_independentVars.Length];
        //    for (int i = 0; i < _independentVars.Length; i++)
        //    {
        //        indVals[i] = Convert.ToDouble(independentValues[_independentVars[i]]);
        //    }

        //    return Predict(indVals);
        //}

        //public double Predict(double[] independentValues)
        //{
        //    return _model.Predict(independentValues);
        //}

        private DataTable createParametersDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Name",typeof(string));
            dt.Columns.Add("Value", typeof(double));
            dt.Columns.Add("StandardError", typeof(double));
            dt.Columns.Add("TStatistic", typeof(double));
            dt.Columns.Add("PValue", typeof(double));
            dt.Columns.Add("StandardizedCoefficient", typeof(double));

            return dt;
        }

        public Dictionary<string, double> Model
        {
            get
            {

                Dictionary<string, double> parameters = new Dictionary<string, double>();
                for (int i = 0; i < _parameters.Rows.Count; i++)
                {
                    parameters.Add(_parameters.Rows[i][0].ToString(), Convert.ToDouble(_parameters.Rows[i][1]));
                }

                return parameters;
            }
        }
        
    }
}
