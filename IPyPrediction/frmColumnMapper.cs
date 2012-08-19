using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IPyPrediction
{
    public partial class frmColumnMapper : Form
    {
        private string[] strArrMainEffects = null;
        private DataTable dtMapped;
        private bool boolIdColumn;

        //The captions for the two columns in the mapping grid
        private string[] strArrHeaderCaptions;

        //The columns names in the imported datatable
        private List<string> lstImpColNames = null;

        public DataTable MappedTable
        {
            get { return dtMapped; }
        }

       
        public frmColumnMapper(string[] mainEffects, DataTable dt, string[] headerCaptions, bool IDColumn)
        {
            InitializeComponent();
            strArrMainEffects = mainEffects;
            dtMapped = dt.Copy();

            strArrHeaderCaptions = headerCaptions.ToArray();
            lstImpColNames = new List<string>();
            boolIdColumn = IDColumn;

            foreach (DataColumn dc in dtMapped.Columns)
                lstImpColNames.Add(dc.ColumnName);
        }


        private void frmColumnMapper_Load(object sender, EventArgs e)
        {
            DataGridViewTextBoxColumn dgTextCol = new DataGridViewTextBoxColumn();
            dgTextCol.HeaderText = strArrHeaderCaptions[0];
            dgTextCol.Width = 220;

            DataGridViewComboBoxColumn dgComboCol = new DataGridViewComboBoxColumn();
            dgComboCol.HeaderText = strArrHeaderCaptions[1];
            dgComboCol.Width = 220;
            dgComboCol.Items.AddRange(lstImpColNames.ToArray());

            dataGridView1.Columns.Add(dgTextCol);
            dataGridView1.Columns.Add(dgComboCol);
            
            //Add the Main effect IVs to the grid.
            for(int i=0; i<strArrMainEffects.Length; i++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].Cells[0].Value = strArrMainEffects[i];

                if (dtMapped.Columns.Contains(strArrMainEffects[i]))
                    dataGridView1.Rows[i].Cells[1].Value = strArrMainEffects[i];
            }

            //default the id/obs col selections to first/second cols
            if(boolIdColumn)
            {
                dataGridView1.Rows[0].Cells[1].Value = lstImpColNames[0];
                //dataGridView1.Rows[1].Cells[1].Value = _impColNames[1];
            }
        }


        //Assign the correct main effect column names to the right imported dataset columns.
        private void btnOk_Click(object sender, EventArgs e)
        {
            int intNumRows = dataGridView1.Rows.Count;
            Dictionary<string, string> dictColMap = new Dictionary<string, string>(); 

            for (int i = 0; i < intNumRows; i++)
            {
                string strMe    = dataGridView1.Rows[i].Cells[0].Value as string;
                string strIdata = dataGridView1.Rows[i].Cells[1].Value as string;

                if ((String.IsNullOrEmpty(strIdata)) || (strIdata == ""))
                {
                    string msg = "Model variable {0} is not mapped to an imported data column.";
                    MessageBox.Show(String.Format(msg, strMe));                    
                    return;
                }
                dictColMap.Add(strMe, strIdata);                
            }

            DataTable dt = new DataTable();
            if (dictColMap.ContainsKey("ID"))
                dt.Columns.Add("ID", typeof(string));     

            foreach (string meKey in dictColMap.Keys)
            {
                if (String.Compare(meKey,"ID",true) != 0)                
                    dt.Columns.Add(meKey, typeof(double));                                                                      
            }

            //Populate the new data table with data from the old.
            for (int i = 0; i < dtMapped.Rows.Count; i++)
            {
                DataRow dr = dt.NewRow();
                foreach (string meKey in dictColMap.Keys)
                {
                    if (String.Compare(meKey, "ID",true) == 0)
                        dr[meKey] = dtMapped.Rows[i][dictColMap[meKey]].ToString();
                    else
                        dr[meKey] = dtMapped.Rows[i][dictColMap[meKey]];
                }
                dt.Rows.Add(dr);
            }

            if (dt.Columns.Contains("ID"))
                dt.Columns["ID"].SetOrdinal(0);

            dtMapped = dt;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }


        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            //Not sure why this exception is getting thrown.  Might be because of non numeric data in test set.
            string msg = e.Exception.Message;
            e.ThrowException = false;
        }
    }
}
