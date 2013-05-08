using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Collections;
using VBCommon;
using VBCommon.Metadata;

namespace VBCommon.Controls
{
    /// <summary>
    /// Class provides a mechanism to validate imported datasets.
    /// Validation consists of idenifying and changing "missing"
    /// data values.  Several options exist for replacing "missing"
    /// data values.  
    /// 
    /// Very different from the first two implementations of this class.
    /// Implementation of this beast is hugely inefficient if users
    /// target cell replacement one at a time as the table is re-scanned
    /// with each iteration.
    /// </summary>
    public partial class frmMissingPredValues : Form
    {
        private DataTable _dt = null;
        private string strWithString = string.Empty;
        private string strTargetCols = string.Empty;
        private DataGridView _dgv = null;
        private bool boolStatus = false;

        private double dblReplacevalue;
        private string strReplacestring = string.Empty;
        private string strFindstring = string.Empty;

        private Metadata.Utilities.TableUtils _tu = null;
        private dtColumnInformation _dtCI = null;
        private dtRowInformation _dtRI = null;

        private List<int[]> lstBadCells = null;

        //target dropdownlists content depends on which radio button action seleced
        private string[] strArrDdlReplaceWith = { "Only This Cell", "Entire Row", "Entire Column", "Entire Sheet" };
        private string[] strArrDdlDeleteRow = { "Only This Row", "Entire Column", "Entire Sheet" };

        private bool boolUpdatetargetrow = true;
        private bool boolUpdatetargetcol = true;

        private static int intTargetcol = 0;
        private static int intTargetrow = 0;


        /// <summary>
        /// accessor to return the working copy of the datatable to the caller
        /// </summary>
        public DataTable ValidatedDT
        {
            get { return _dt; }
        }


        /// <summary>
        /// accessor to return the validation tool's status to the caller
        /// note status is only true after successful scan and btnReturn_Click()
        /// i.e., if they return via cancel, the grid is still disabled
        /// </summary>
        public bool Status
        {
            get { return boolStatus; }
        }


        /// <summary>
        /// Constructor takes the datagridview to show changes implemented in this class
        /// </summary>
        /// <param name="dgv">datasheet grid reference</param>
        /// <param name="dt">datasource for the grid</param>
        public frmMissingPredValues(DataGridView dgv, DataTable dt)
        {
            InitializeComponent();

            _tu = new Metadata.Utilities.TableUtils(dt);
            _dtRI = new dtRowInformation(dt);
            _dtCI = new dtColumnInformation(dt);

            //get a working copy of the dataset
            _dt = dt.Copy();
            _dgv = dgv;

            cboCols.DataSource = strArrDdlReplaceWith;
            btnReturn.Enabled = false;
        }


        /// <summary>
        /// Constructor takes the datagridview to show changes implemented in this class
        /// </summary>
        /// <param name="dgv">datasheet grid reference</param>
        /// <param name="dt">datasource for the grid</param>
        public frmMissingPredValues(DataGridView dgv, DataTable dt, dtRowInformation dtri, dtColumnInformation dtci)
        {
            InitializeComponent();

            _tu = new Metadata.Utilities.TableUtils(dt);
            _dtRI = dtri;
            _dtCI = dtci;

            //get a working copy of the dataset
            _dt = dt.Copy();
            _dgv = dgv;

            cboCols.DataSource = strArrDdlReplaceWith;
            btnReturn.Enabled = false;
        }


        /// <summary>
        /// Scan the table of anomolous data values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnScan_Click(object sender, EventArgs e)
        {
            bool bColumnsOnly = false;
            lstBadCells = VBCommon.IO.ImportExport.GetBadCellsByRow(_dt, strFindstring);
            int[] cellNdxs = new int[2];

            if (lstBadCells.Count > 0 && !bColumnsOnly)
            {
                cellNdxs = lstBadCells[0];
                _dgv.Rows[cellNdxs[1]].Cells[cellNdxs[0]].Selected = true;
                _dgv.CurrentCell = _dgv.SelectedCells[0];
                btnReturn.Enabled = false; //can't return until clean
                boolStatus = false;
                //return badCells;
            }
            else
            {
                //Make sure that the imported data are doubles (and not strings)
                DataTable dtCloned = _dt.Clone();
                for (int i = 1; i < _dt.Columns.Count; i++)
                {
                    dtCloned.Columns[i].DataType = typeof(Double);
                }                                
                
                foreach (DataRow row in _dt.Rows) 
                {
                    dtCloned.ImportRow(row);
                }
                _dt = dtCloned;

                lblStatus.Text = "No anomalous data values found.";
                btnReturn.Enabled = true;  //can return if user selects
                boolStatus = true;
                groupBox4.Enabled = false;
                //return badCells;
            }

            if (lstBadCells.Count > 0)
                groupBox4.Enabled = true;
        }




        /*
        /// <summary>
        /// depricated - gathers bad cells (blanks, nulls, text, search criterion) by datatable column
        /// MC wants the action to work by row.
        /// </summary>
        /// <returns>list of int arrays containg column index and row index of bad cells</returns>
        private List<int[]> getBadCells()
        {
            int[] cellNdxs = new int[2];
            List<int[]> badCells = new List<int[]>();

            foreach (DataColumn dc in _dt.Columns)
            {
                int cndx = _dt.Columns.IndexOf(dc);
                foreach (DataRow dr in _dt.Rows)
                {
                    int rndx = _dt.Rows.IndexOf(dr);
                    //gather blanks...
                    if (string.IsNullOrEmpty(dr[dc].ToString())) 
                    {
                        cellNdxs[0] = cndx;
                        cellNdxs[1] = rndx;
                        badCells.Add(cellNdxs);
                        cellNdxs = new int[2];
                    }
                    // ...AND alpha cells only (blanks captured above)...
                    else if (!Information.IsNumeric(dr[dc].ToString()) && _dt.Columns.IndexOf(dc) != 0) 
                    {
                        cellNdxs[0] = cndx;
                        cellNdxs[1] = rndx;
                        badCells.Add(cellNdxs);
                        cellNdxs = new int[2];
                    }
                    //...AND user input
                    if (dr[dc].ToString() == strFindstring && !string.IsNullOrWhiteSpace(strFindstring)) 
                    {
                        cellNdxs[0] = cndx;
                        cellNdxs[1] = rndx;
                        badCells.Add(cellNdxs);
                        cellNdxs = new int[2];
                    }
                }
            }

            if (badCells.Count > 0)
            {
                lblStatus.Text = string.Empty;
                cellNdxs = badCells[0];
                _dgv.Rows[cellNdxs[1]].Cells[cellNdxs[0]].Selected = true;
                _dgv.CurrentCell = _dgv.SelectedCells[0];
                btnReturn.Enabled = false; //can't return until clean
                boolStatus = false;
                return badCells;
            }
            else
            {
                lblStatus.Text = "No anomolous data values found.";
                btnReturn.Enabled = true;  //can return if user selects
                boolStatus = true;
                groupBox4.Enabled = false;
                return badCells;
            }
        }*/
        

        /// <summary>
        /// Event handler for btnAction
        /// controls the action for a replace value in bad cells, row delete or column delete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAction_Click(object sender, EventArgs e)
        {
            lblStatus.Text = string.Empty;
            strFindstring = txtFindVal.Text;
            strReplacestring = txtReplaceWith.Text;

            //validate the user-supplied replacement value if replacement is what we're doing
            if (rbActionReplace.Checked && string.IsNullOrWhiteSpace(strReplacestring))
            {
                CancelEventArgs ce = new CancelEventArgs();
                txtReplaceWith_Validating(txtReplaceWith, ce);
                return;
            }

            //foreach bad cell (action modified by radio button selected and dropdown target selection)
            //perform the cell update, column delete or row delete
            //possible targets are specified in the global _ddreplacewith, _dddeletecol and _dddeleterow lists
            //and the selectable targets available are controled by which UI radio button is checked
            bool stop = false;
            if (lstBadCells.Count > 0)
            {
                for (int ndx = 0; ndx < lstBadCells.Count; ndx++)
                {
                    if (stop = doAction(ndx)) break;
                }
            }

            //re-scan the table - maybe the target was limited and bad cells remain
            //if so, the next bad cell will be selected
            //if not, the scan will enable the appropriate controls allowing return
            btnScan_Click(null, null);
            if (lstBadCells.Count > 0)
            {
                if (!stop)
                {
                    boolUpdatetargetcol = true;
                    boolUpdatetargetrow = true;
                }
                else
                {
                    boolUpdatetargetcol = false;
                    boolUpdatetargetrow = false;
                }
            }
        }


        /// <summary>
        /// performs a replace value in bad cells, row delete or column delete limited by target selection
        /// </summary>
        /// <param name="ndx">index of the bad cell list to work on</param>
        /// <returns>bool to control the interation loop on the bad cell list</returns>
        private bool doAction(int ndx)
        {
            bool breakloop = false;
            string target = cboCols.SelectedValue.ToString();

            int[] location;
            location = lstBadCells[ndx];

            int rndx = location[1];
            int cndx = location[0];

            #region replace cell value
            if (rbActionReplace.Checked)
            {
                if (target == "Only This Cell")
                {
                    _dgv.Rows[rndx].Cells[cndx].Selected = true;
                    _dgv[cndx, rndx].Value = dblReplacevalue;
                    lstBadCells.RemoveAt(ndx);
                    _dt.Rows[rndx][cndx] = _dgv[cndx, rndx].Value;
                    _dt.AcceptChanges();
                    breakloop = true;
                }
                else if (target == "Entire Row")
                {
                    if (boolUpdatetargetrow)
                    {
                        boolUpdatetargetrow = false;
                        intTargetrow = rndx;
                    }
                    if (rndx == intTargetrow)
                    {
                        _dgv.Rows[rndx].Cells[cndx].Selected = true;
                        _dgv[cndx, rndx].Value = dblReplacevalue;
                        _dt.Rows[rndx][cndx] = _dgv[cndx, rndx].Value;
                        _dt.AcceptChanges();
                    }
                    breakloop = false;
                }
                else if (target == "Entire Column")
                {
                    if (boolUpdatetargetcol)
                    {
                        boolUpdatetargetcol = false;
                        intTargetcol = cndx;
                    }
                    if (cndx == intTargetcol)
                    {
                        _dgv.Rows[rndx].Cells[cndx].Selected = true;
                        _dgv[cndx, rndx].Value = dblReplacevalue;
                        _dt.Rows[rndx][cndx] = _dgv[cndx, rndx].Value;
                        _dt.AcceptChanges();
                    }
                    breakloop = false;
                }
                else if (target == "Entire Sheet")
                {
                    _dgv.Rows[rndx].Cells[cndx].Selected = true;
                    _dgv[cndx, rndx].Value = dblReplacevalue;
                    _dt.Rows[rndx][cndx] = _dgv[cndx, rndx].Value;
                    _dt.AcceptChanges();
                    breakloop = false;
                }
            }
            #endregion
            #region delete row
            else if (rbActionDelRow.Checked)
            {
                if (target == "Only This Row")
                {
                    _dgv.Rows[rndx].Cells[cndx].Selected = true;
                    _dtRI.SetRowStatus(_dt.Rows[rndx][0].ToString(), false);
                    _dt = _tu.FilterDataTableRows(_dt, _dtRI);
                    _dt.AcceptChanges();
                    _dgv.DataSource = _dt;
                    breakloop = true;
                }
                else if (target == "Entire Column")
                {
                    for (int i = 0; i < lstBadCells.Count; i++)
                    {
                        int[] ndxs = lstBadCells[i];
                        int c = ndxs[0];
                        int r = ndxs[1];
                        if (c != cndx) continue;
                        _dtRI.SetRowStatus(_dt.Rows[r][0].ToString(), false);

                    }
                    _dt = _tu.FilterDataTableRows(_dt, _dtRI);
                    _dt.AcceptChanges();
                    _dgv.DataSource = _dt;
                    breakloop = true; 
                }
                else if (target == "Entire Sheet")
                {
                    for (int i = 0; i < lstBadCells.Count; i++)
                    {
                        int[] ndxs = lstBadCells[i];
                        int c = ndxs[0];
                        int r = ndxs[1];
                        _dtRI.SetRowStatus(_dt.Rows[r][0].ToString(), false);

                    }
                    _dt = _tu.FilterDataTableRows(_dt, _dtRI);
                    _dt.AcceptChanges();
                    _dgv.DataSource = _dt;
                    breakloop = true;
                }
            }
            #endregion
            
            return breakloop;
        }


        # region maintain dropdowns selections - these are find/replace/target selections and/or type-ins

        private void cboCols_SelectedIndexChanged(object sender, EventArgs e)
        {
            strTargetCols = cboCols.SelectedItem.ToString();
        }


        private void rbActionReplace_CheckedChanged(object sender, EventArgs e)
        {
            if (rbActionReplace.Checked) cboCols.DataSource = strArrDdlReplaceWith;
        }


        private void rbActionDelRow_CheckedChanged(object sender, EventArgs e)
        {
            if (rbActionDelRow.Checked) cboCols.DataSource = strArrDdlDeleteRow;
        }

        #endregion


        /// <summary>
        /// Shows a simplified variable specification dialog to identify categorical variables only
        /// updates the global table with variables that the user might have identified as categorical
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCat_Click(object sender, EventArgs e)
        {
            frmVariableSpecification frmCat = new frmVariableSpecification(_dt);
            frmCat.ShowDialog();
            //table has column extended properties set for categorical variables
            DataTable dt = frmCat.Table;
            _dt = dt;
        }


        /// <summary>
        /// Users can only return if the table scans as clean (all cells numeric); if here then it's clean
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReturn_Click(object sender, EventArgs e)
        {
            boolStatus = true;
            Close();
        }


        /// <summary>
        /// If we cancel out of the missing dialog, close, but set the status
        /// appropriately.  Users MUST return to the caller via the Return
        /// button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            boolStatus = false;
            //if (_rpt != null) _rpt.Close();
            Close();
        }


        private void txtFindVal_TextChanged(object sender, EventArgs e)
        {
            strFindstring = txtFindVal.Text.ToString();
        }


        private void txtReplaceWith_Validating(object sender, CancelEventArgs e)
        {
            if (!double.TryParse(txtReplaceWith.Text, out dblReplacevalue))
            {
                e.Cancel = true;
                txtReplaceWith.Select(0, txtReplaceWith.Text.Length);
                this.errorProvider1.SetError(txtReplaceWith, "Text must convert to a number.");
            }
        }


        private void txtReplaceWith_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(txtReplaceWith, "");
        }
    }
}
