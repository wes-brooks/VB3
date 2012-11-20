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
        

        //constructor
        public frmPredictionScatterPlot(DataTable dtObs, DataTable dtStats, string ObservationColumn="Observation", string PredictionColumn="Model_Prediction")
        {
            InitializeComponent();
            this.dtObs = dtObs;
            this.dtStats = dtStats;
            this.strObservationColumn = ObservationColumn;
            this.strPredictionColumn = PredictionColumn;

            scatterPlot.UpdateResults(GetObsPredData());
        }

        
        //configure display for plot
        public void ConfigureDisplay(double decisionThreshold, double regulatoryThreshold,string transform, double exponent)
        {
            scatterPlot.SetThresholds(decisionThreshold, regulatoryThreshold);
            scatterPlot.PowerExponent = exponent;
            scatterPlot.UpdateResults(GetObsPredData());
            scatterPlot.Transform = transform;
        }


        //return a list of pred data
        private List<double[]> GetObsPredData()
        {
            if ((dtObs == null) || (dtStats == null))
                return null;

            int intMinRecs = Math.Min(dtObs.Rows.Count, dtStats.Rows.Count);
            if (intMinRecs < 1)
                return null;

            List<double[]> lstData = new List<double[]>();
            double[] dblArrRecord = null;
            for (int i = 0; i < intMinRecs; i++)
            {
                dblArrRecord = new double[2];
                dblArrRecord[0] = Convert.ToDouble(dtObs.Rows[i][strObservationColumn]);
                dblArrRecord[1] = Convert.ToDouble(dtStats.Rows[i][strPredictionColumn]);
                lstData.Add(dblArrRecord);
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
