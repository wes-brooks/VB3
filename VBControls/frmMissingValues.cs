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

namespace VBControls
{
    /// <summary>
    /// Class provides a mechanism to validate imported datasets.
    /// Validation consists of idenifying and changing "missing"
    /// data values.  Several options exist for replacing "missing"
    /// data values
    /// </summary>
    public partial class frmMissingValues : Form
    {
        private DataTable _dt = null;
        private string strWithString = string.Empty;
        private string strTargetCols = string.Empty;
        private DataGridView _dgv = null;
        private bool boolStatus = false;
        private string strFrAction = "ReplaceValue";
        private double dblReplacevalue;
        private string strReplacestring = string.Empty;
        private string strFindstring = string.Empty;

        private Utilities.TableUtils _tu = null;
        private dtColumnInformation _dtCI = null;
        private dtRowInformation _dtRI = null;

        private frmBadCellsRpt _rpt = null;
        private List<int[]> lstBadCells = null;

        //target dropdownlists content depends on which radio button action seleced
        private string[] strArrDdlReplaceWith = { "Only This Cell", "Entire Row", "Entire Column", "Entire Sheet" };
        private string[] strArrDdlDeleteRow = { "Only This Row", "Entire Column", "Entire Sheet" };
        private string[] strArrDdlDeletColumn = { "Only This Column", "Entire Row", "Entire Sheet" };

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
        public frmMissingValues(DataGridView dgv, DataTable dt)
        {
            InitializeComponent();

            _tu = new Utilities.TableUtils(dt);
            _dtRI = dtRowInformation.getdtRI(_dt, false);
            _dtCI = dtColumnInformation.getdtCI(dt, false);

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
            lstBadCells = getBadCellsByRow();
            if (lstBadCells.Count > 0)
                groupBox4.Enabled = true;
        }


        /// <summary>
        /// method gathers bad cells (blanks, nulls, text, search criterion) by datatable row
        /// </summary>
        /// <returns>list of int arrays containg column index and row index of bad cells</returns>
        private List<int[]> getBadCellsByRow()
        {
            int[] intArrCellNdxs = new int[2];
            List<int[]> lstBadCells = new List<int[]>();

            foreach (DataRow dr in _dt.Rows)
            {
                int intRndx = _dt.Rows.IndexOf(dr);
                foreach (DataColumn dc in _dt.Columns)
                {
                    int intCndx = _dt.Columns.IndexOf(dc);
                    //gather blanks...
                    if (string.IsNullOrEmpty(dr[dc].ToString()))
                    {
                        intArrCellNdxs[0] = intCndx;
                        intArrCellNdxs[1] = intRndx;
                        lstBadCells.Add(intArrCellNdxs);
                        intArrCellNdxs = new int[2];
                    }
                    // ...AND alpha cells only (blanks captured above)...
                    else if (!Information.IsNumeric(dr[dc].ToString()) && _dt.Columns.IndexOf(dc) != 0)
                    {
                        intArrCellNdxs[0] = intCndx;
                        intArrCellNdxs[1] = intRndx;
                        lstBadCells.Add(intArrCellNdxs);
                        intArrCellNdxs = new int[2];
                    }
                    //...AND user input
                    if (dr[dc].ToString() == strFindstring && !string.IsNullOrWhiteSpace(strFindstring))
                    {
                        intArrCellNdxs[0] = intCndx;
                        intArrCellNdxs[1] = intRndx;
                        lstBadCells.Add(intArrCellNdxs);
                        intArrCellNdxs = new int[2];
                    }
                }
            }

            if (lstBadCells.Count > 0)
            {
                intArrCellNdxs = lstBadCells[0];
                _dgv.Rows[intArrCellNdxs[1]].Cells[intArrCellNdxs[0]].Selected = true;
                _dgv.CurrentCell = _dgv.SelectedCells[0];
                btnReturn.Enabled = false; //can't return until clean
                boolStatus = false;
                return lstBadCells;
            }
            else
            {
                lblStatus.Text = "No anomalous data values found.";
                btnReturn.Enabled = true;  //can return if user selects
                boolStatus = true;
                groupBox4.Enabled = false;
                return lstBadCells;
            }
        }


        private List<int[]> getBadCells()
        {
            int[] intArrCellNdxs = new int[2];
            List<int[]> listBadCells = new List<int[]>();

            foreach (DataColumn dc in _dt.Columns)
            {
                int intCndx = _dt.Columns.IndexOf(dc);
                foreach (DataRow dr in _dt.Rows)
                {
                    int intRndx = _dt.Rows.IndexOf(dr);
                    //gather blanks...
                    if (string.IsNullOrEmpty(dr[dc].ToString())) 
                    {
                        intArrCellNdxs[0] = intCndx;
                        intArrCellNdxs[1] = intRndx;
                        listBadCells.Add(intArrCellNdxs);
                        intArrCellNdxs = new int[2];
                    }
                    // ...AND alpha cells only (blanks captured above)...
                    else if (!Information.IsNumeric(dr[dc].ToString()) && _dt.Columns.IndexOf(dc) != 0) 
                    {
                        intArrCellNdxs[0] = intCndx;
                        intArrCellNdxs[1] = intRndx;
                        listBadCells.Add(intArrCellNdxs);
                        intArrCellNdxs = new int[2];
                    }
                    //...AND user input
                    if (dr[dc].ToString() == strFindstring && !string.IsNullOrWhiteSpace(strFindstring)) 
                    {
                        intArrCellNdxs[0] = intCndx;
                        intArrCellNdxs[1] = intRndx;
                        listBadCells.Add(intArrCellNdxs);
                        intArrCellNdxs = new int[2];
                    }
                }
            }

            if (listBadCells.Count > 0)
            {
                intArrCellNdxs = listBadCells[0];
                _dgv.Rows[intArrCellNdxs[1]].Cells[intArrCellNdxs[0]].Selected = true;
                _dgv.CurrentCell = _dgv.SelectedCells[0];
                btnReturn.Enabled = false; //can't return until clean
                boolStatus = false;
                return listBadCells;
            }
            else
            {
                lblStatus.Text = "No anomalous data values found.";
                btnReturn.Enabled = true;  //can return if user selects
                boolStatus = true;
                groupBox4.Enabled = false;
                return listBadCells;
            }
        }


        /// <summary>
        /// Event handler for btnAction
        /// performs a find/replace
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAction_Click(object sender, EventArgs e)
        {
            //find/replace
            lblStatus.Text = string.Empty;
            strFindstring = txtFindVal.Text;
            strReplacestring = txtReplaceWith.Text;

            if (rbActionReplace.Checked && string.IsNullOrWhiteSpace(strReplacestring))
            {
                CancelEventArgs ce = new CancelEventArgs();
                txtReplaceWith_Validating(txtReplaceWith, ce);
                return;
            }

            for (int ndx = 0; ndx < lstBadCells.Count; ndx++)
            {
                if (doAction(ndx)) break;
            }

            btnScan_Click(null, null);
            if (lstBadCells.Count > 0)
            {
                boolUpdatetargetcol = true;
                boolUpdatetargetrow = true;
            }
        }


        private bool doAction(int ndx)
        {
            bool boolBreakloop = false;
            string strTarget = cboCols.SelectedValue.ToString();

            int[] intArrLocation = lstBadCells[ndx];
            int intRndx = intArrLocation[1];
            int intCndx = intArrLocation[0];

            #region replace cell value
            if (rbActionReplace.Checked)
            {
                if (strTarget == "Only This Cell")
                {
                    _dgv.Rows[intRndx].Cells[intCndx].Selected = true;
                    _dgv[intCndx, intRndx].Value = dblReplacevalue;
                    lstBadCells.RemoveAt(ndx);
                    _dt.Rows[intRndx][intCndx] = _dgv[intCndx, intRndx].Value;
                    _dt.AcceptChanges();
                    boolBreakloop = true;
                }
                else if (strTarget == "Entire Row")
                {
                    if (boolUpdatetargetrow)
                    {
                        boolUpdatetargetrow = false;
                        intTargetrow = intRndx;
                    }
                    if (intRndx == intTargetrow)
                    {
                        _dgv.Rows[intRndx].Cells[intCndx].Selected = true;
                        _dgv[intCndx, intRndx].Value = dblReplacevalue;
                        _dt.Rows[intRndx][intCndx] = _dgv[intCndx, intRndx].Value;
                        _dt.AcceptChanges();
                    }
                    boolBreakloop = false;
                }
                else if (strTarget == "Entire Column")
                {
                    if (boolUpdatetargetcol)
                    {
                        boolUpdatetargetcol = false;
                        intTargetcol = intCndx;
                    }
                    if (intCndx == intTargetcol)
                    {
                        _dgv.Rows[intRndx].Cells[intCndx].Selected = true;
                        _dgv[intCndx, intRndx].Value = dblReplacevalue;
                        _dt.Rows[intRndx][intCndx] = _dgv[intCndx, intRndx].Value;
                        _dt.AcceptChanges();
                    }
                    boolBreakloop = false;
                }
                else if (strTarget == "Entire Sheet")
                {
                    _dgv.Rows[intRndx].Cells[intCndx].Selected = true;
                    _dgv[intCndx, intRndx].Value = dblReplacevalue;
                    _dt.Rows[intRndx][intCndx] = _dgv[intCndx, intRndx].Value;
                    _dt.AcceptChanges();
                    boolBreakloop = false;
                }
            }
            #endregion
            #region delete row
            else if (rbActionDelRow.Checked)
            {
                if (strTarget == "Only This Row")
                {
                    _dgv.Rows[intRndx].Cells[intCndx].Selected = true;
                    _dtRI.setRowStatus(_dt.Rows[intRndx][0].ToString(), false);
                    _dt = _tu.filterDataTableRows(_dt);
                    _dt.AcceptChanges();
                    _dgv.DataSource = _dt;
                    boolBreakloop = true;
                }
                else if (strTarget == "Entire Column")
                {
                    for (int i = 0; i < lstBadCells.Count; i++)
                    {
                        int[] intArrNdxs = lstBadCells[i];
                        int c = intArrNdxs[0];
                        int r = intArrNdxs[1];
                        if (c != intCndx) continue;
                        _dtRI.setRowStatus(_dt.Rows[r][0].ToString(), false);

                    }
                    _dt = _tu.filterDataTableRows(_dt);
                    _dt.AcceptChanges();
                    _dgv.DataSource = _dt;
                    boolBreakloop = true; 
                }
                else if (strTarget == "Entire Sheet")
                {
                    for (int i = 0; i < lstBadCells.Count; i++)
                    {
                        int[] ndxs = lstBadCells[i];
                        int c = ndxs[0];
                        int r = ndxs[1];
                        _dtRI.setRowStatus(_dt.Rows[r][0].ToString(), false);

                    }
                    _dt = _tu.filterDataTableRows(_dt);
                    _dt.AcceptChanges();
                    _dgv.DataSource = _dt;
                    boolBreakloop = true;
                }
            }
            #endregion

            return boolBreakloop;
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
        /// Check the table one last time lest they're managed to get anomolous data in there somehow...
        /// and if ok, return. Return button is only enabled if they've done a Scan (btn_Look_Click()) 
        /// and it comes back clean. (The Status property is set there as well.)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReturn_Click(object sender, EventArgs e)
        {
            boolStatus = true;
            Close();
        }


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
        /// If we cancel out of the missing dialog, close, but set the status
        /// appropriately.  Users MUST return to the caller via the Return
        /// button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            boolStatus = false;
            if (_rpt != null) _rpt.Close();
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
