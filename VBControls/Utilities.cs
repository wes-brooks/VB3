using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data;
using Microsoft.VisualBasic;
using System.Drawing;
using System.Windows.Forms;

namespace VBControls
{
    /// <summary>
    /// class contains a number of useful methods applicable to datatable and datagridview
    /// controls.  code was collected here to help readablilty of datasheet code and avoid 
    /// duplicating these functional requirements.
    /// </summary>
    public class Utilities
    {
        public bool testValueAttribute(DataColumn dc, string attr)
        {
            if (dc.ExtendedProperties.ContainsKey(attr))
            {
                bool boolValue = (bool)dc.ExtendedProperties[attr];
                return boolValue;
            }
            return false;
        }


        public void setAttributeValue(DataColumn dc, string attr, bool value)
        {
            dc.ExtendedProperties[attr] = value;
        }


        /// <summary>
        /// datatable methods for filtering and adding columns based on table extended properties
        /// </summary>
        public class TableUtils
        {
            private dtRowInformation _dtRI = null;
            private dtColumnInformation _dtCI = null;
            private DataTable _dt = null;

            public TableUtils(DataTable dt)
            {
                _dt = dt;
                _dtCI = dtColumnInformation.getdtCI(dt, false);
                _dtRI = dtRowInformation.getdtRI(dt, false);

            }


            public void registerNewCols(DataTable dt)
            {
                _dtCI = dtColumnInformation.getdtCI(dt, false);
                foreach (DataColumn c in dt.Columns)
                {
                    if (!_dtCI.getColStatus(c.ColumnName))
                    {
                        _dtCI.addColumnNameToDic(c.ColumnName);
                    }
                }
            }


            public DataTable filterDisabledCols(DataTable dt)
            {
                //filter out disabled columns
                DataTable dtCopy = dt.Copy();

                dtColumnInformation dtCI = dtColumnInformation.getdtCI(dt, false);
                foreach (KeyValuePair<string, bool> kv in dtCI.DTColInfo)
                {
                    if (kv.Value) continue; 
                    if (dtCopy.Columns.Contains(kv.Key))
                        dtCopy.Columns.Remove(kv.Key);
                }
                dtCopy.AcceptChanges();
                return dtCopy;
            }


            public DataTable filterDataTableRows(DataTable dt)
            {
                //filter out disabled rows
                DataTable dtCopy = dt.Copy();
                Dictionary<string, bool> dictRstatus = _dtRI.DTRowInfo;
                for (int i = 0; i < dtCopy.Rows.Count; i++)
                {
                    if (!dictRstatus[dtCopy.Rows[i][0].ToString()])
                        dtCopy.Rows[i].Delete();
                }
                dtCopy.AcceptChanges();
                return dtCopy;
            }


            public DataTable filterTcols(DataTable dt)
            {
                //filter transformed columns
                DataTable dtCopy = dt.Copy();
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    bool boolTransformed = dt.Columns[c].ExtendedProperties.ContainsKey(VBCommon.Globals.TRANSFORM);
                    if (boolTransformed == true) 
                        if (dtCopy.Columns.Contains(dt.Columns[c].Caption.ToString()))
                            dtCopy.Columns.Remove(dt.Columns[c].Caption.ToString());
                }
                dtCopy.AcceptChanges();
                return dtCopy;
            }


            public DataTable filterRVHcols(DataTable dt)
            {
                //filter hidden response variable columns
                DataTable dtCopy = dt.Copy();

                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    bool boolIsrv = dt.Columns[c].ExtendedProperties.ContainsKey(VBCommon.Globals.DEPENDENTVAR);
                    bool boolIstrv = dt.Columns[c].ExtendedProperties.ContainsKey(VBCommon.Globals.DEPENDENTVARIBLETRANSFORM);
                    if (boolIsrv == true)
                    {
                        bool boolIsH = dt.Columns[c].ExtendedProperties.ContainsKey(VBCommon.Globals.HIDDEN);
                        if (boolIsH == true)
                        {
                            bool boolIsVis = (bool)dt.Columns[c].ExtendedProperties[VBCommon.Globals.HIDDEN];
                            if (boolIsVis == true)
                            {
                                if (dtCopy.Columns.Contains(dt.Columns[c].Caption))
                                    dtCopy.Columns.Remove(dt.Columns[c].Caption);
                            }
                        }
                    }
                }
                dtCopy.AcceptChanges();
                return dtCopy;
            }


            public DataTable filterCatVars(DataTable dt)
            {
                DataTable dtCopy = dt.Copy();
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    bool boolHascat = dt.Columns[c].ExtendedProperties.ContainsKey(VBCommon.Globals.CATEGORICAL);
                    if (boolHascat == true)
                    {
                        bool boolIscat = dt.Columns[c].ExtendedProperties.ContainsKey(VBCommon.Globals.CATEGORICAL);
                        if (boolIscat == true)
                        {
                            if (boolIscat == true)
                            {
                                if (dtCopy.Columns.Contains(dt.Columns[c].Caption))
                                    dtCopy.Columns.Remove(dt.Columns[c].Caption);
                            }
                        }
                    }
                }

                dtCopy.AcceptChanges();
                return dtCopy;
            }


            public DataTable addCatCols(DataTable dtnew, DataTable dt)
            {
                DataTable dtCopy = dtnew.Copy();
                foreach (DataColumn dc in dt.Columns)
                {
                    bool boolHasAttribute = dc.ExtendedProperties.ContainsKey(VBCommon.Globals.CATEGORICAL);
                    if (!boolHasAttribute) continue;
                    if (!dtCopy.Columns.Contains(dc.Caption))
                    {
                        int intNdx = dt.Columns.IndexOf(dc);
                        var varDvalues = (from row in dt.Select() select row.Field<double>(dt.Columns.IndexOf(dc))).ToArray<double>();
                        dtCopy.Columns.Add(dc.Caption, typeof(double));
                        for (int r = 0; r < dtCopy.Rows.Count; r++)
                            dtCopy.Rows[r][dc.Caption] = varDvalues[r];

                        if (intNdx > dtCopy.Columns.Count)
                        {
                            intNdx = dtCopy.Columns.Count - 1;
                        }
                        dtCopy.Columns[dc.Caption].SetOrdinal(intNdx);
                        dtCopy = copyAllColAttributes(dc, dt, dtCopy);
                    }
                }
                dtCopy.AcceptChanges();
                return dtCopy;
            }


            public DataTable addDisabledCols(DataTable dtnew, DataTable dt)
            {
                DataTable dtCopy = dtnew.Copy();
                foreach (DataColumn dc in dt.Columns)
                {
                    bool boolHasAttribute = dc.ExtendedProperties.ContainsKey(VBCommon.Globals.ENABLED);
                    if (!boolHasAttribute) continue;
                    bool boolEnabled = (bool)dc.ExtendedProperties[VBCommon.Globals.ENABLED];
                    if (!boolEnabled)
                    {
                        if (!dtCopy.Columns.Contains(dc.Caption))
                        {
                            int intNdx = dt.Columns.IndexOf(dc);
                            var varDvalues = (from row in dt.Select() select row.Field<double>(dt.Columns.IndexOf(dc))).ToArray<double>();
                            dtCopy.Columns.Add(dc.Caption, typeof(double));
                            for (int r = 0; r < dtCopy.Rows.Count; r++)
                                dtCopy.Rows[r][dc.Caption] = varDvalues[r];

                            if (intNdx > dtCopy.Columns.Count)
                            {
                                intNdx = dtCopy.Columns.Count - 1;
                            }
                            dtCopy.Columns[dc.Caption].SetOrdinal(intNdx);
                            dtCopy = copyAllColAttributes(dc, dt, dtCopy);
                        }
                    }
                }
                dtCopy.AcceptChanges();
                return dtCopy;
            }


            public DataTable addOldTCols(DataTable dtnew, DataTable dt)
            {
                DataTable dtCopy = dtnew.Copy();
                foreach (DataColumn dc in dt.Columns)
                {
                    if (dc.ExtendedProperties.ContainsKey(VBCommon.Globals.TRANSFORM))
                    {
                        if (!dtCopy.Columns.Contains(dc.Caption))
                        {
                            var varDvalues = (from row in dt.Select() select row.Field<double>(dt.Columns.IndexOf(dc))).ToArray<double>();
                            dtCopy.Columns.Add(dc.Caption, typeof(double));
                            for (int r = 0; r < dt.Rows.Count; r++)
                                dtCopy.Rows[r][dc.Caption] = varDvalues[r];

                            dtCopy = copyAllColAttributes(dc, dt, dtCopy);
                        }
                    }
                }
                dtCopy.AcceptChanges();
                return dtCopy;
            }


            public DataTable addHiddenResponseVarCols(DataTable dtnew, DataTable dt)
            {
                DataTable dtCopy = dtnew.Copy();
                foreach (DataColumn dc in dt.Columns)
                {
                    bool boolHasAttribute = dc.ExtendedProperties.ContainsKey(VBCommon.Globals.DEPENDENTVAR);
                    if (!boolHasAttribute) continue;

                    bool boolHasHidden = dc.ExtendedProperties.ContainsKey(VBCommon.Globals.HIDDEN);
                    if (!boolHasHidden) continue;

                    bool boolIsHidden = (bool)dc.ExtendedProperties[VBCommon.Globals.HIDDEN];
                    if (boolIsHidden)
                    {
                        if (!dtCopy.Columns.Contains(dc.Caption))
                        {
                            int intNdx = dt.Columns.IndexOf(dc);
                            var varDvalues = (from row in dt.Select() select row.Field<double>(dt.Columns.IndexOf(dc))).ToArray<double>();
                            dtCopy.Columns.Add(dc.Caption, typeof(double));
                            for (int r = 0; r < dtCopy.Rows.Count; r++)
                                dtCopy.Rows[r][dc.Caption] = varDvalues[r];

                            dtCopy.Columns[dc.Caption].SetOrdinal(intNdx);
                            dtCopy = copyAllColAttributes(dc, dt, dtCopy);
                        }
                    }
                }
                dtCopy.AcceptChanges();
                return dtCopy;
            }


            public DataTable setHiddenIVstoUnhidden(DataTable dt)
            {
                DataTable dtCopy = dt.Copy();
                foreach (DataColumn dc in dtCopy.Columns)
                {
                    bool boolHasAttr = dc.ExtendedProperties.ContainsKey(VBCommon.Globals.MAINEFFECT);
                    if (boolHasAttr)
                    {
                        boolHasAttr = dc.ExtendedProperties.ContainsKey(VBCommon.Globals.HIDDEN);
                        if (boolHasAttr)
                        {
                            bool boolIsHidden = (bool)dc.ExtendedProperties[VBCommon.Globals.HIDDEN];
                            if (boolIsHidden)
                            {
                                dc.ExtendedProperties[VBCommon.Globals.HIDDEN] = false;
                            }
                        }
                    }
                }
                return dtCopy;
            }


            private DataTable copyAllColAttributes(DataColumn dc, DataTable sourceDT, DataTable targetDT)
            {
                PropertyCollection sourceproperties = sourceDT.Columns[dc.Caption].ExtendedProperties;    
                foreach (DictionaryEntry kv in sourceproperties)
                {
                    targetDT.Columns[dc.Caption].ExtendedProperties[kv.Key] = kv.Value;
                }
                return targetDT;
            }
        }


        /// <summary>
        /// datagridview methods for maintaining the view of the datatable data in the grid
        /// </summary>
        public class GridUtils
        {
            private dtRowInformation _dtRI = null;
            
            public GridUtils(DataGridView dgv)
            {
            }


            public void maintainGrid(DataGridView dgv, DataTable dt, int selectedColIndex, string responseVarColName)
            {
                //reset the grid
                dgv.DataSource = null;
                dgv.DataSource = dt;

                //mark all grid cols visible, not sortable
                for (int c = 0; c < dgv.Columns.Count; c++)
                {
                    dgv.Columns[c].SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                //hide any main effect cols that have been transformed (hidden attribute is set in the transform class)
                foreach (DataColumn c in dt.Columns)
                {
                    bool boolHashidden = c.ExtendedProperties.ContainsKey(VBCommon.Globals.HIDDEN);
                    if (boolHashidden == true)
                    {
                        bool boolHide = (bool)c.ExtendedProperties[VBCommon.Globals.HIDDEN];
                        if (boolHide) { dgv.Columns[c.ColumnName].Visible = false; }
                    }
                    //reset the column disabled properties
                    bool boolHasattribute = c.ExtendedProperties.ContainsKey(VBCommon.Globals.ENABLED);
                    if (boolHasattribute)
                    {
                        selectedColIndex = dt.Columns.IndexOf(c);
                        bool boolEnabled = (bool)c.ExtendedProperties[VBCommon.Globals.ENABLED];
                        if (!boolEnabled) 
                        {
                            for (int r = 0; r < dgv.Rows.Count; r++)
                                dgv[selectedColIndex, r].Style.ForeColor = Color.Red;
                        }
                    }
                }

                //reset the UI clues for the response variable
                dgv.Columns[responseVarColName].DefaultCellStyle.BackColor = Color.LightBlue;

                //reset disable rows
                _dtRI = dtRowInformation.getdtRI(dt, false);
                for (int r = 0; r < dt.Rows.Count; r++)
                {
                    bool boolEnabled = _dtRI.getRowStatus(dt.Rows[r][0].ToString());
                    if (!boolEnabled)
                    {
                        for (int c = 0; c < dgv.Columns.Count; c++)
                            dgv[c, r].Style.ForeColor = Color.Red;
                    }
                }
              
                //set the numerical precision for display
                setViewOnGrid(dgv);
            }


            public void setViewOnGrid(DataGridView dgv)
            {
                //utility method used to set numerical precision displayed in grid

                //seems to be the only way I can figure to get a string in col 1 that may
                //(or may not) be a date and numbers in all other columns.
                //in design mode set "no format" for the dgv defaultcellstyle
                string testcellval = string.Empty;
                for (int col = 1; col < dgv.Columns.Count; col++)
                {
                    //skips col 0 (date/time/string/whatever)
                    testcellval = dgv[col, 0].Value.ToString();
                    bool boolIsNum = Information.IsNumeric(testcellval); //try a little visualbasic magic

                    if (boolIsNum)
                    {
                        dgv.Columns[col].ValueType = typeof(System.Double);
                        dgv.Columns[col].DefaultCellStyle.Format = "g4";
                    }
                    else
                    {
                        dgv.Columns[col].ValueType = typeof(System.String);
                    }
                }
            }


            public void disableGridCol(DataGridView dgv, int selectedColIndex)
            {
                for (int r = 0; r < dgv.Rows.Count; r++)
                    dgv[selectedColIndex, r].Style.ForeColor = Color.Red;
            }


            public void enableGridCol(DataGridView dgv, int selectedColIndex, DataTable dt)
            {
                dtRowInformation dtRI = dtRowInformation.getdtRI(dt, false);
                for (int r = 0; r < dgv.Rows.Count; r++)
                {
                    //set style to black unless the row is disabled
                    if (!dtRI.getRowStatus(dgv[0, r].Value.ToString())) continue;
                    dgv[selectedColIndex, r].Style.ForeColor = Color.Black;
                }
            }


            public void setResponseVarCol(DataGridView dgv, int selectedColIndex, int responseVarColIndex)
            {
                dgv.Columns[responseVarColIndex].DefaultCellStyle.BackColor = Color.White;
                dgv.Columns[selectedColIndex].DefaultCellStyle.BackColor = Color.LightBlue;
            }


            public void setDisabledColandRows(DataGridView dgv, DataTable dt)
            {
                //reset the column disabled properties
                foreach (DataColumn c in dt.Columns)
                {
                    bool boolHasattribute = c.ExtendedProperties.ContainsKey(VBCommon.Globals.ENABLED);
                    if (boolHasattribute)
                    {
                        int intSelectedColIndex = dt.Columns.IndexOf(c);
                        bool boolEnabled = (bool)c.ExtendedProperties[VBCommon.Globals.ENABLED];
                        if (!boolEnabled) //{ DisableCol(null, null); }
                        {
                            for (int r = 0; r < dgv.Rows.Count; r++)
                                dgv[intSelectedColIndex, r].Style.ForeColor = Color.Red;
                        }
                    }
                }

                //reset disable rows
                _dtRI = dtRowInformation.getdtRI(dt, false);
                for (int r = 0; r < dt.Rows.Count; r++)
                {
                    bool boolEnabled = _dtRI.getRowStatus(dt.Rows[r][0].ToString());
                    if (!boolEnabled)
                    {
                        for (int c = 0; c < dgv.Columns.Count; c++)
                            dgv[c, r].Style.ForeColor = Color.Red;
                    }
                }
            }


            public void unHideHiddenCols(DataGridView dgv, DataTable dt)
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    bool boolHasH = dc.ExtendedProperties.ContainsKey(VBCommon.Globals.HIDDEN);
                    if (boolHasH)
                    {
                        bool boolIsHidden = (bool)dc.ExtendedProperties[VBCommon.Globals.HIDDEN];
                        if (boolIsHidden)
                        {
                            dt.Columns[dc.Caption].ExtendedProperties[VBCommon.Globals.HIDDEN] = false;
                            dgv.Columns[dc.Caption].Visible = true;
                        }
                    }
                }
            }
        }
    }
}
