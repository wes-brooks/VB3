using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data;
using Microsoft.VisualBasic;
using System.Drawing;
using System.Windows.Forms;


namespace VBCommon.Metadata

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
                if ((bool)dc.ExtendedProperties[attr] == true)
                    return true;
                else
                    return false;
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
                _dtCI = new dtColumnInformation(dt);
                _dtRI = new dtRowInformation(dt);

            }


            public void registerNewCols(DataTable dt)
            {
                _dtCI = new dtColumnInformation(dt);
                foreach (DataColumn c in dt.Columns)
                {
                    if (!_dtCI.GetColStatus(c.ColumnName))
                    {
                        _dtCI.AddColumnNameToDict(c.ColumnName);
                    }

                }
            }


            public DataTable FilterDisabledCols(DataTable dt, dtColumnInformation dtci=null)
            {
                //filter out disabled columns
                DataTable dtCopy = dt.Copy();

                if (dtci==null)
                    dtci = new dtColumnInformation(dt);

                foreach (KeyValuePair<string, bool> kv in dtci.DTColInfo)
                {
                    if (kv.Value) continue; 
                    if (dtCopy.Columns.Contains(kv.Key))
                        dtCopy.Columns.Remove(kv.Key);
                }
                dtCopy.AcceptChanges();
                return dtCopy;
            }


            public DataTable FilterDataTableRows(DataTable dt, dtRowInformation dtri)
            {
                //filter out disabled rows
                DataTable dtCopy = dt.Copy();
                Dictionary<string, bool> rstatus = dtri.DTRowInfo;
                for (int i = dt.Rows.Count-1; i >= 0; i--)
                {
                    if (!rstatus[dt.Rows[i][0].ToString()])
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
                    bool transformed = dt.Columns[c].ExtendedProperties.ContainsKey(VBCommon.Globals.TRANSFORM);
                    if (transformed == true) 
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
                    bool isrv = dt.Columns[c].ExtendedProperties.ContainsKey(VBCommon.Globals.DEPENDENTVAR);
                    bool istrv = dt.Columns[c].ExtendedProperties.ContainsKey(VBCommon.Globals.DEPENDENTVARIBLETRANSFORM);
                    if (isrv == true)
                    {
                        bool isH = dt.Columns[c].ExtendedProperties.ContainsKey(VBCommon.Globals.HIDDEN);
                        if (isH == true)
                        {
                            if (dt.Columns[c].ExtendedProperties[VBCommon.Globals.HIDDEN].ToString() == "True")
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
                    bool hascat = dt.Columns[c].ExtendedProperties.ContainsKey(VBCommon.Globals.CATEGORICAL);
                    if (hascat == true)
                    {
                        if (dt.Columns[c].ExtendedProperties[VBCommon.Globals.CATEGORICAL].ToString() == "True")
                        {

                            if (dtCopy.Columns.Contains(dt.Columns[c].Caption))
                                dtCopy.Columns.Remove(dt.Columns[c].Caption);
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
                    bool hasAttribute = dc.ExtendedProperties.ContainsKey(VBCommon.Globals.CATEGORICAL);
                    if (!hasAttribute) continue;
                    if (!dtCopy.Columns.Contains(dc.Caption))
                    {
                        int ndx = dt.Columns.IndexOf(dc);
                        var dvalues = (from row in dt.Select() select row.Field<double>(dt.Columns.IndexOf(dc))).ToArray<double>();
                        dtCopy.Columns.Add(dc.Caption, typeof(double));
                        for (int r = 0; r < dtCopy.Rows.Count; r++)
                            dtCopy.Rows[r][dc.Caption] = dvalues[r];

                        if (ndx > dtCopy.Columns.Count)
                        {
                            ndx = dtCopy.Columns.Count - 1;
                        }
                        dtCopy.Columns[dc.Caption].SetOrdinal(ndx);
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
                    bool hasAttribute = dc.ExtendedProperties.ContainsKey(VBCommon.Globals.ENABLED);
                    if (!hasAttribute) continue;

                    if ((bool)dc.ExtendedProperties[VBCommon.Globals.ENABLED] != true)
                    {
                        if (!dtCopy.Columns.Contains(dc.Caption))
                        {
                            int ndx = dt.Columns.IndexOf(dc);
                            var dvalues = (from row in dt.Select() select row.Field<double>(dt.Columns.IndexOf(dc))).ToArray<double>();
                            dtCopy.Columns.Add(dc.Caption, typeof(double));
                            for (int r = 0; r < dtCopy.Rows.Count; r++)
                                dtCopy.Rows[r][dc.Caption] = dvalues[r];

                            if (ndx > dtCopy.Columns.Count)
                            {
                                ndx = dtCopy.Columns.Count - 1;
                            }
                            dtCopy.Columns[dc.Caption].SetOrdinal(ndx);
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
                            var dvalues = (from row in dt.Select() select row.Field<double>(dt.Columns.IndexOf(dc))).ToArray<double>();
                            dtCopy.Columns.Add(dc.Caption, typeof(double));
                            for (int r = 0; r < dt.Rows.Count; r++)
                                dtCopy.Rows[r][dc.Caption] = dvalues[r];

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
                    bool hasAttribute = dc.ExtendedProperties.ContainsKey(VBCommon.Globals.DEPENDENTVAR);
                    if (!hasAttribute) continue;

                    bool hasHidden = dc.ExtendedProperties.ContainsKey(VBCommon.Globals.HIDDEN);
                    if (!hasHidden) continue;

                    if (dc.ExtendedProperties[VBCommon.Globals.HIDDEN].ToString() == "True")
                    {
                        if (!dtCopy.Columns.Contains(dc.Caption))
                        {
                            int ndx = dt.Columns.IndexOf(dc);
                            var dvalues = (from row in dt.Select() select row.Field<double>(dt.Columns.IndexOf(dc))).ToArray<double>();
                            dtCopy.Columns.Add(dc.Caption, typeof(double));
                            for (int r = 0; r < dtCopy.Rows.Count; r++)
                                dtCopy.Rows[r][dc.Caption] = dvalues[r];

                            dtCopy.Columns[dc.Caption].SetOrdinal(ndx);
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
                    bool hasAttr = dc.ExtendedProperties.ContainsKey(VBCommon.Globals.MAINEFFECT);
                    if (hasAttr)
                    {
                        hasAttr = dc.ExtendedProperties.ContainsKey(VBCommon.Globals.HIDDEN);
                        if (hasAttr)
                        {
                            if (dc.ExtendedProperties[VBCommon.Globals.HIDDEN].ToString() == "True")
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

            public GridUtils(DataGridView dgv)
            {
            }


            public void maintainGrid(DataGridView dgv, DataTable dt, int selectedColIndex, string responseVarColName)
            {
                //reset the grid
                dgv.DataSource = null;
                dgv.DataSource = dt;
                dtColumnInformation _dtCI = new dtColumnInformation(dt);
                dtRowInformation _dtRI = new dtRowInformation(dt);

                //mark all grid cols visible, not sortable
                for (int c = 0; c < dgv.Columns.Count; c++)
                {
                    dgv.Columns[c].SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                //hide any main effect cols that have been transformed (hidden attribute is set in the transform class)
                foreach (DataColumn c in dt.Columns)
                {
                    bool hashidden = c.ExtendedProperties.ContainsKey(VBCommon.Globals.HIDDEN);
                    if (hashidden == true)
                    {
                        if (c.ExtendedProperties[VBCommon.Globals.HIDDEN].ToString() == "True")
                            { dgv.Columns[c.ColumnName].Visible = false; }
                    }

                    //reset the column disabled properties
                    bool hasattribute = c.ExtendedProperties.ContainsKey(VBCommon.Globals.ENABLED);
                    if (hasattribute)
                    {
                        selectedColIndex = dt.Columns.IndexOf(c);
                        if (c.ExtendedProperties[VBCommon.Globals.ENABLED].ToString() != "True")
                        {
                            for (int r = 0; r < dgv.Rows.Count; r++)
                                dgv[selectedColIndex, r].Style.ForeColor = Color.Red;
                            _dtCI.SetColStatus(c.ColumnName.ToString(), false);
                        }
                        else
                        {
                            for (int r = 0; r < dgv.Rows.Count; r++)
                                dgv[selectedColIndex, r].Style.ForeColor = Color.Black;
                            _dtCI.SetColStatus(c.ColumnName.ToString(), true); 
                        }
                    }
                }

                //reset the UI clues for the response variable
                dgv.Columns[responseVarColName].DefaultCellStyle.BackColor = Color.LightBlue;

                //reset disable rows
                for (int r = 0; r < dt.Rows.Count; r++)
                {
                    bool enabled = _dtRI.GetRowStatus(dt.Rows[r][0].ToString());
                    if (!enabled)
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
                    bool isNum = Information.IsNumeric(testcellval); //try a little visualbasic magic

                    if (isNum)
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
                dtRowInformation dtRI = new dtRowInformation(dt);
                for (int r = 0; r < dgv.Rows.Count; r++)
                {
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
                    bool hasattribute = c.ExtendedProperties.ContainsKey(VBCommon.Globals.ENABLED);
                    if (hasattribute)
                    {
                        int selectedColIndex = dt.Columns.IndexOf(c);
                        if (c.ExtendedProperties[VBCommon.Globals.ENABLED].ToString() != "True")
                        {
                            for (int r = 0; r < dgv.Rows.Count; r++)
                                dgv[selectedColIndex, r].Style.ForeColor = Color.Red;
                        }
                    }
                }

                //reset disable rows
                dtRowInformation _dtRI = new dtRowInformation(dt);
                for (int r = 0; r < dt.Rows.Count; r++)
                {
                    bool enabled = _dtRI.GetRowStatus(dt.Rows[r][0].ToString());
                    if (!enabled)
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
                    bool hasH = dc.ExtendedProperties.ContainsKey(VBCommon.Globals.HIDDEN);
                    if (hasH)
                    {
                        bool isHidden = (bool)dc.ExtendedProperties[VBCommon.Globals.HIDDEN];
                        if (isHidden)
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