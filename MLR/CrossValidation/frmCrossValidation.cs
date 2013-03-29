using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GALib;
using CrossValidation;
//using VBTools;


namespace CrossValidation
{
    public partial class frmCrossValidation : Form
    {
        private List<IIndividual> _models;
        private int _numObs;

        public frmCrossValidation(List<IIndividual> models, int numObs)
        {
            InitializeComponent();
            lblTotalObs.Text = numObs.ToString();
            _models = models;
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            try
            {

                progressBar1.Visible = true;
                dataGridView1.DataSource = null;

                int numTesting = Convert.ToInt32(txtNumTesting.Text);
                int numTrials = Convert.ToInt32(txtNumTrials.Text);

                CrossValidation crossVal = new CrossValidation(_models);
                crossVal.ProgressBar = progressBar1;
                crossVal.SampleSize = numTesting;
                crossVal.Iterations = numTrials;
                crossVal.Run();

                MSEP[] msepList = crossVal.MSEPList;

                int maxVars = 0;
                for (int i = 0; i < _models.Count; i++)
                {
                    if (_models[i].Chromosome.Count > maxVars)
                        maxVars = _models[i].Chromosome.Count;
                }

                DataTable dt = createDataTable(maxVars);

                for (int i = 0; i < msepList.Length; i++)
                {
                    DataRow dr = dt.NewRow();
                    dr["Fitness"] = msepList[i].Model.Fitness;
                    dr["MSEP"] = msepList[i].msep;

                    int numVars = msepList[i].IndependentVariables.Length;
                    for (int j = 0; j < numVars; j++)
                    {
                        dr["Ind Var " + (j + 1).ToString()] = msepList[i].IndependentVariables[j];
                    }

                    dt.Rows.Add(dr);

                }

                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {

            }
            finally
            {
                progressBar1.Visible = false;
            }

        }

        private DataTable createDataTable(int numVars)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Fitness", typeof(double));
            dt.Columns.Add("MSEP", typeof(double));

            for (int i = 1; i <= numVars; i++)
                dt.Columns.Add("Ind Var " + i.ToString(), typeof(string));

            return dt;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// show the help document via F1 key event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="hlpevent"></param>
        private void frmCrossValidation_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            //string apppath = Application.StartupPath.ToString();
            //VBCSHelp help = new VBCSHelp(apppath, sender);
            //if (!help.Status)
            //{
            //    MessageBox.Show(
            //    "User documentation is found in the Documentation folder where you installed Virtual Beach"
            //    + "\nIf your web browser is PDF-enabled, open the User Guide with your browser.",
            //    "Neither Adobe Acrobat nor Adobe Reader found.",
            //    MessageBoxButtons.OK);
            //}
        }



    }
}
