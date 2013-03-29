using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VBTools;

namespace MLRPrediction
{
    public partial class frmMLRPredObs : Form
    {
        VBProjectManager _projMgr = null;


        private DataTable _dtObs = null;
        private DataTable _dtStats = null;
        private Globals.DependendVariableTransforms _obsState;
        private Globals.DependendVariableTransforms _rvtType;
        private double _exponent;
        private double _importedExp;
        private Globals.DependendVariableTransforms _predview;
        private double _predviewExp;
        
        public frmMLRPredObs( DataTable dtObs, DataTable dtStats, 
            string decisionThreshold, string regulatoryThreshold, 
            Globals.DependendVariableTransforms rvtType, double exponent,
            Globals.DependendVariableTransforms obsstate, double importedExp, 
            Globals.DependendVariableTransforms predview, double predviewExp)
        {
            //if the prediction view is none, thresholds, observations and predictions will all mesh
            //if the prediction view is log/ln/power, we've got to convert thresholds, observations 
            //and predictions into view state from depvar state

            InitializeComponent();
            _projMgr = VBProjectManager.GetProjectManager();
            _dtObs = dtObs;
            _dtStats = dtStats;
            _obsState = obsstate;
            _rvtType = rvtType;
            _exponent = exponent;
            _importedExp = importedExp;
            _predview = predview;
            _predviewExp = predviewExp;
            double[] probabilities = null;

            mlrPlots1.SetThresholds(decisionThreshold, regulatoryThreshold);
            mlrPlots1.PowerExponent = exponent;
            mlrPlots1.Transform = rvtType;
            //mlrPlots1.UpdateResults(GetObsPredData(), _projMgr.ModelingInfo.RMSE, VBControls.MLRPlots.Exceedance.prediction);
            mlrPlots1.UpdateResults(getSyncedObsPredProb(out probabilities), _projMgr.ModelingInfo.RMSE, VBControls.MLRPlots.Exceedance.prediction);


            //p(exceedances) are calculated in the plot control for models; for predictions, send them
            //double[] probs = new double[_dtStats.Rows.Count];
            //for (int r = 0; r < _dtStats.Rows.Count; r++)
            //    probs[r] = (double)_dtStats.Rows[r]["Exceedance_Probability"];

            //send the p(exceedances) to the plot control
            //mlrPlots1.SetPredictionProbabilities = probs;
            mlrPlots1.SetPredictionProbabilities = probabilities;

        }

        private List<double[]> getSyncedObsPredProb(out double[] probs)
        {
            if ((_dtObs == null) || (_dtStats == null))
            {
                probs = null;
                return null;
                
            }

            int minRecs = Math.Min(_dtObs.Rows.Count, _dtStats.Rows.Count);
            if (minRecs < 1)
            {
                probs = null;
                return null;
            }

            List<double[]> data = new List<double[]>();
            double[] record = null;
            probs = new double[_dtStats.Rows.Count];

            int ndx = 0;
            for (int r = 0; r < _dtStats.Rows.Count; r++)
            {
                record = new double[2];
                string id = _dtStats.Rows[r]["ID"].ToString();
                DataRow[] rowsObs = _dtObs.Select("ID = '" + id + "'");
                if (rowsObs != null && rowsObs.Length > 0)
                {
                    //if we have a matching observation ID for a prediction ID ...
                    try
                    {
                        double obs;
                        if (double.TryParse(rowsObs[0][1].ToString(), out obs))
                        {
                            //...and we have a valid value for the observation...
                            double pred = (double)_dtStats.Rows[r]["Model_Prediction"];
                            record[1] = GetTransformPrediction(pred);
                            record[0] = GetTransformObservation(obs);
                            data.Add(record);

                            double prob = (double)_dtStats.Rows[r]["Exceedance_Probability"];
                            probs[ndx] = prob;
                            ndx++;
                        }
                    }
                    catch (InvalidCastException)
                    {
                        //in case we have bad prediction info somehow
                        continue;
                    }
                }
            }
            return data;

        }


        private List<double[]> GetObsPredData()
        {

            if ((_dtObs == null) || (_dtStats == null))
                return null;

            int minRecs = Math.Min(_dtObs.Rows.Count, _dtStats.Rows.Count);
            if (minRecs < 1)
                return null;

            List<double[]> data = new List<double[]>();
            double[] record = null;
            for (int i = 0; i < minRecs; i++)
            {
                record = new double[2];
                try
                {
                    record[0] = GetTransformObservation(Convert.ToDouble(_dtObs.Rows[i]["Observation"]));
                    record[1] = GetTransformPrediction(Convert.ToDouble(_dtStats.Rows[i]["Model_Prediction"]));
                    //record[2] = Convert.ToDouble(_dtStats.Rows[i]["Exceedance_Probability"]);
                }
                catch (InvalidCastException)
                {
                    //skips bad cells that exist in the obs table
                    continue;
                }
                data.Add(record);
            }

            return data;
        }

        private double GetTransformPrediction(double Value)
        {
            //double Value = Convert.ToDouble(sValue);
            double retValue = double.NaN;
            if (_rvtType == Globals.DependendVariableTransforms.none)
            {
                retValue = Value;
            }
            else if (_rvtType == Globals.DependendVariableTransforms.Log10)
            {
                retValue = Math.Sign(Value) * Math.Log10(Math.Abs( Value ));
            }
            else if (_rvtType == Globals.DependendVariableTransforms.Ln)
            {
                retValue = Math.Sign(Value) * Math.Log(Math.Abs( Value ));
            }
            else if (_rvtType == Globals.DependendVariableTransforms.Power)
            {
                retValue = Math.Pow(Value, _exponent);
            }

            //if (_predview == Globals.DependendVariableTransforms.none)
            //    retValue = retValue;
            //else if (_predview == Globals.DependendVariableTransforms.Log10)
            //    //retVal = Math.Log10(retVal);
            //    retValue = Math.Pow(10.0, retValue);
            //else if (_predview == Globals.DependendVariableTransforms.Ln)
            //    //retVal = Math.Log(retVal);
            //    retValue = Math.Pow(Math.E, retValue);
            //else if (_predview == Globals.DependendVariableTransforms.Power)
            //    retValue = Math.Pow(retValue, 1.0 / _predviewExp);

            return retValue;
        }

        private double GetTransformObservation(double value)
        {
            //get the observation value in the same transformation state as the response variable.
            //response variable state is determined from the UI RV transform label (itself set via 
            //datatable entended properties from onformenter, or from predictioninfo on modelopen 
            //(projectopen listener).  returned values are used in determining model failures (FN/FP
            //counts)
            double retVal = double.NaN;

            if (_rvtType == Globals.DependendVariableTransforms.none)
            {
                if (_obsState == Globals.DependendVariableTransforms.none)
                {
                    retVal = value;
                }
                else if (_obsState == Globals.DependendVariableTransforms.Log10)
                {
                    retVal = Math.Pow(10.0, value);
                }
                else if (_obsState == Globals.DependendVariableTransforms.Ln)
                {
                    retVal = Math.Pow(Math.E, value);
                }
                else if (_obsState == Globals.DependendVariableTransforms.Power)
                {
                    retVal = Math.Pow(value, 1.0 / _exponent);
                }
            }
            else if (_rvtType == Globals.DependendVariableTransforms.Log10)
            {
                if (_obsState == Globals.DependendVariableTransforms.none)
                {
                    retVal = Math.Log10(value);
                }
                else if (_obsState == Globals.DependendVariableTransforms.Log10)
                {
                    retVal = value;
                }
                else if (_obsState == Globals.DependendVariableTransforms.Ln)
                {
                    retVal = Math.Pow(Math.E, value);
                    retVal = Math.Log10(retVal);
                    //retVal = Math.Log(value) / Math.Log(10.0d);
                }
                else if (_obsState == Globals.DependendVariableTransforms.Power)
                {
                    retVal = Math.Pow(value, 1.0 / _exponent);
                    retVal = Math.Log10(retVal);
                }
            }
            else if (_rvtType == Globals.DependendVariableTransforms.Ln)
            {
                if (_obsState == Globals.DependendVariableTransforms.none)
                {
                    retVal = Math.Log(value);
                }
                else if (_obsState == Globals.DependendVariableTransforms.Log10)
                {
                    retVal = Math.Pow(10.0, value);
                    retVal = Math.Log(retVal);
                    //retVal = Math.Log10(value) / Math.Log10(Math.E);
                }
                else if (_obsState == Globals.DependendVariableTransforms.Ln)
                {
                    retVal = value;
                }
                else if (_obsState == Globals.DependendVariableTransforms.Power)
                {
                    retVal = Math.Pow(value, 1.0 / _exponent);
                    retVal = Math.Log(retVal);
                }
            }
            else if (_rvtType == Globals.DependendVariableTransforms.Power)
            {
                if (_obsState == Globals.DependendVariableTransforms.none)
                {
                    retVal = Math.Pow(value, _exponent);
                }
                else if (_obsState == Globals.DependendVariableTransforms.Log10)
                {
                    retVal = Math.Pow(10.0, value);
                    retVal = Math.Pow(retVal, _exponent);
                }
                else if (_obsState == Globals.DependendVariableTransforms.Ln)
                {
                    retVal = Math.Pow(Math.E, value);
                    retVal = Math.Pow(retVal, _exponent);
                }
                else if (_obsState == Globals.DependendVariableTransforms.Power)
                {
                    retVal = Math.Pow(value, 1.0 / _importedExp);
                    retVal = Math.Pow(retVal, _exponent);
                }
            }

            //if (_predview == Globals.DependendVariableTransforms.none)
            //    retVal = retVal;
            //else if (_predview == Globals.DependendVariableTransforms.Log10)
            //    //retVal = Math.Log10(retVal);
            //    retVal = Math.Pow(10.0, retVal);
            //else if (_predview == Globals.DependendVariableTransforms.Ln)
            //    //retVal = Math.Log(retVal);
            //    retVal = Math.Pow(Math.E, retVal);
            //else if (_predview == Globals.DependendVariableTransforms.Power)
            //    retVal = Math.Pow(retVal, 1.0 / _predviewExp);

            return retVal;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmMLRPredObs_Load(object sender, EventArgs e)
        {
            VBTools.VBProjectManager _projMgr = VBProjectManager.GetProjectManager();            
            //mlrPlots1.UpdateResults(GetObsPredData(), _projMgr.ModelingInfo.RMSE);

        }


        private void frmMLRPredObs_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string apppath = Application.StartupPath.ToString();
            VBCSHelp help = new VBCSHelp(apppath, sender);
            if (!help.Status)
            {
                MessageBox.Show(
                "User documentation is found in the Documentation folder where you installed Virtual Beach"
                + "\nIf your web browser is PDF-enabled, open the User Guide with your browser.",
                "Neither Adobe Acrobat nor Adobe Reader found.",
                MessageBoxButtons.OK);
            }
        }


    }
}
