using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using VBTools;
using VBCommon.IO;

namespace GALibForm
{
    public partial class frmDataTable : Form
    {
        //private VBProjectManager _proj = null;
        private List<int> _recsRemoved = null;
        private List<double> _dffitsRemoved = null;
        private List<string> _residType = null;
        private DataTable _dt = null;

        public frmDataTable(List<int> deletedRecs, List<double> dffits, List<string> type, DataTable dt)
        {
            InitializeComponent();
            //_proj = VBProjectManager.GetProjectManager();
            _recsRemoved = deletedRecs;
            _dffitsRemoved = dffits;
            _residType = type;
            _dt = dt;
        }

        private void frmDataTable_Load(object sender, EventArgs e)
        {
            //get structure of model dataset
            DataTable newDT = _dt.Clone();
            //add some extra info
            newDT.Columns.Add("Model", typeof(System.String));
            newDT.Columns.Add("Residual Value", typeof(System.Double));
            newDT.Columns.Add("Residual Type", typeof(System.String));
            newDT.Columns["Model"].SetOrdinal(0);
            newDT.Columns["Residual Value"].SetOrdinal(1);
            newDT.Columns["Residual Type"].SetOrdinal(2);

            //display original model dataset
            dgv1.DataSource = _dt;

            int ndx = 0;
            for (int r = 0; r < _recsRemoved.Count; r++)
            {
                //mark records eliminated
                //if (r == 0) ndx = _recsRemoved[r];
                //else ndx = _recsRemoved[r] + 1;
                ndx = _recsRemoved[r];
                dgv1.Rows[ndx].DefaultCellStyle.BackColor = Color.Pink;

                //collect records eliminated to show in other grid
                DataRow row = _dt.Rows[ndx];
                newDT.ImportRow(row);
                string modelname = "Rebuild" + (r + 1).ToString("n0");
                newDT.Rows[r]["Model"] = modelname;
                newDT.Rows[r]["Residual Value"] = _dffitsRemoved[r];
                newDT.Rows[r]["Residual Type"] = _residType[r];
            }
            //show removed records
            dgv2.DataSource = newDT;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            DataTable dt = getExportTable();
            ImportExport export = new ImportExport(dt);
            if (export.Output) return;
        }

        private DataTable getExportTable()
        {
            DataTable dt = _dt.Copy();

            for (int i = 0; i < _recsRemoved.Count; i++)
            {
                int rowndx = _recsRemoved[i];
                dt.Rows[rowndx].Delete();
            }


            return dt;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        //private void frmDataTable_HelpRequested(object sender, HelpEventArgs hlpevent)
        //{
        //    string apppath = Application.StartupPath.ToString();
        //    VBCSHelp help = new VBCSHelp(apppath, sender);
        //    if (!help.Status)
        //    {
        //        MessageBox.Show(
        //        "User documentation is found in the Documentation folder where you installed Virtual Beach"
        //        + "\nIf your web browser is PDF-enabled, open the User Guide with your browser.",
        //        "Neither Adobe Acrobat nor Adobe Reader found.",
        //        MessageBoxButtons.OK);
        //    }
        //}
    }
}
