using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VBCommon;
using VBCommon.Controls;
using VBCommon.Transforms;

namespace Prediction
{
    public partial class frmPredictionScatterPlot : Form
    {
        /* Never Used
            //private double _plotThresholdHoriz = double.NaN;
            //private double _plotThresholdVert = double.NaN;
            //private double _plotProbThreshold = double.NaN;
        */
        //observations and stats datatables
        private DataTable dtObs = null;
        private DataTable dtStats = null;
        private string strObservationColumn;
        private string strPredictionColumn;

        private DependentVariableTransforms xfrmObs;
        private DependentVariableTransforms xfrmPred;
        private DependentVariableTransforms xfrmThresh;

        private double dblObsExponent;
        private double dblPredExponent;
        private double dblThreshExponent;
        

        //constructor
        public frmPredictionScatterPlot(DataTable dtObs, DataTable dtStats, string ObservationColumn="Observation", string PredictionColumn="Model_Prediction", bool RawPredictions=true)
        {
            InitializeComponent();
            this.dtObs = dtObs;
            this.dtStats = dtStats;
            this.strObservationColumn = ObservationColumn;
            this.strPredictionColumn = PredictionColumn;

            scatterPlot.RawPredictions = RawPredictions;
            scatterPlot.UpdateResults(GetObsPredData());            
        }

        
        //configure display for plot
        public void ConfigureDisplay(double DecisionThreshold, double RegulatoryThreshold, double ProbabilityThreshold, string Transform, double Exponent)        
        {
            scatterPlot.SetThresholds(DecisionThreshold, RegulatoryThreshold, ProbabilityThreshold);
            scatterPlot.PowerExponent = Exponent;
            scatterPlot.UpdateResults(GetObsPredData());            
            scatterPlot.Transform = Transform;
            scatterPlot.DisplayTransform = Transform;
            scatterPlot.DisplayTransformExponent = Exponent;
        }


        //configure display for plot
        public void ConfigureDisplay(double DecisionThreshold, double RegulatoryThreshold, double ProbabilityThreshold, DependentVariableTransforms ObservationTransform, double ObservationExponent, DependentVariableTransforms PredictionTransform, double PredictionExponent, DependentVariableTransforms ThresholdTransform, double ThresholdExponent, bool RawPredictions=false)
        {
            scatterPlot.SetThresholds(DecisionThreshold, RegulatoryThreshold, ProbabilityThreshold);
            scatterPlot.PowerExponent = ThresholdExponent;            
            scatterPlot.Transform = ThresholdTransform.ToString();
            scatterPlot.RawPredictions = RawPredictions;
            scatterPlot.DisplayTransform = PredictionTransform.ToString();
            scatterPlot.DisplayTransformExponent = PredictionExponent;

            xfrmObs = ObservationTransform;
            xfrmPred = PredictionTransform;
            xfrmThresh = ThresholdTransform;

            dblObsExponent = ObservationExponent;
            dblPredExponent = PredictionExponent;
            dblThreshExponent = ThresholdExponent;

            scatterPlot.UpdateResults(GetObsPredData());
            scatterPlot.Refresh();
            
            //scatterPlot.PowerExponent = PredictionExponent;
            //scatterPlot.Transform = PredictionTransform.ToString();            
        }


        //return a list of pred data
        private List<double[]> GetObsPredData()
        {
            if ((dtObs == null) || (dtStats == null))
                return null;

            //int intMinRecs = Math.Min(dtObs.Rows.Count, dtStats.Rows.Count);
            //if (intMinRecs < 1)
            //    return null;
                                   
            List<string> lstStatsKeys = new List<string>();
            foreach (DataRow row in dtStats.Rows) {
                try
                {
                    lstStatsKeys.Add(row[0].ToString());
                }
                catch {}
            }

            List<string> lstObsKeys = new List<string>();
            foreach (DataRow row in dtObs.Rows) {
                try
                {
                    lstObsKeys.Add(row[0].ToString());
                }
                catch {}
            }

            if (lstStatsKeys.Count < 1)
                return null;

            List<double[]> lstData = new List<double[]>();
            double[] dblArrRecord = null;
            for (int i = 0; i < lstStatsKeys.Count; i++)
            {
                if (lstObsKeys.Contains(lstStatsKeys[i]))
                {
                    int j = lstObsKeys.IndexOf(lstStatsKeys[i]);

                    dblArrRecord = new double[2];
                    dblArrRecord[0] = VBCommon.Transforms.Apply.TransformThreshold(VBCommon.Transforms.Apply.UntransformThreshold(Convert.ToDouble(dtObs.Rows[j][strObservationColumn]), xfrmObs, dblObsExponent), xfrmPred, dblPredExponent);
                    if (scatterPlot.RawPredictions) { dblArrRecord[1] = Convert.ToDouble(dtStats.Rows[i][strPredictionColumn]); }
                    else { dblArrRecord[1] = Convert.ToDouble(dtStats.Rows[i][strPredictionColumn]); }
                    lstData.Add(dblArrRecord);
                }
            }
            return lstData;
        }

        
        //close the plot form
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        //update plot
        private void frmMLRPredObs_Load(object sender, EventArgs e)
        {

            scatterPlot.UpdateResults(GetObsPredData());
        }
    }
}
