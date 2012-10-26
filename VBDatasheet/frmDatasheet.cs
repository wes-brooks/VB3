using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotSpatial.Controls;
using WeifenLuo.WinFormsUI.Docking;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VBDatasheet;
using VBCommon;
using VBCommon.Controls;
using VBCommon.PluginSupport;
using VBCommon.Metadata;
using VBCommon.Interfaces;

namespace VBDatasheet
{
    [JsonObject]
    public partial class frmDatasheet : UserControl, IFormState
    {
        private IDictionary<string, object> dictPackedDatasheetState = null;
        private bool boolInitialPass;
        private bool boolValidated;

        private double dblOrientation;
        
        public event EventHandler NotifiableDataEvent;

        //constructor
        public frmDatasheet()    
        {
            InitializeComponent();
            this.dsControl1.NotifiableChangeEvent +=new EventHandler(this.UpdateData);
        }


        //unpack event handler. unpacks packed state in dictionary to repopulate datasheet
        public void UnpackState(IDictionary<string, object> dictPluginState)
        {
            IDictionary<string, object> dictDatasheetState = (IDictionary<string, object>)dictPluginState["PackedDatasheetState"];
            dsControl1.UnpackState(dictDatasheetState);
        }


        //event handler for packing state to save project
        public IDictionary<string, object> PackState()
        {
            /*//make sure user chose transformation on response var
            if (dsControl1.DependentVariableTransform == VBCommon.DependentVariableTransforms.none)
            {
                MessageBox.Show("You must define the transformation on the response variable before continuing");
                return null;
            }*/

            IDictionary<string, object> dictPluginState = new Dictionary<string, object>();
           
            dictPackedDatasheetState = dsControl1.PackState();
            dictPluginState.Add("PackedDatasheetState", dictPackedDatasheetState);

            dictPluginState.Add("InitialPass", boolInitialPass);
            dictPluginState.Add("DSValidated", boolValidated);

            return dictPluginState;
        }


        public void UpdateData(object source, EventArgs e)
        {
            //Re-raise the event to be handled at the plugin level
            if (NotifiableDataEvent != null)
            {
                NotifiableDataEvent(source, e);
            }
        }


        public void btnImportData_Click(object sender, EventArgs e)
        {
            DataTable dataDT = new DataTable("Imported Data");
            VBCommon.IO.ImportExport import = new VBCommon.IO.ImportExport();
            if ((dataDT = import.Input) == null) return;

            //check for unique records or blanks
            string errcolname = string.Empty;
            int errndx = 0;
            if (!recordIndexUnique(dataDT, out errndx))
            {
                MessageBox.Show("Unable to import datasets with non-unique record identifiers.\n" +
                                "Fix your datatable by assuring unique record identifier values\n" +
                                "in the 1st column and try importing again.\n\n" +
                                "Record Identifier values cannot be blank or duplicated;\nencountered " +
                                "error near row " + errndx.ToString(), "Import Data Error - Cannot Import This Dataset", MessageBoxButtons.OK);
                return;
            }
            //check for spaces
            string offendingCol = string.Empty;
            if (datasetHasSpacesinColNmaes(dataDT, out offendingCol))
            {
                MessageBox.Show("Cannot import datasets with spaces in column names.\nEdit your dataset and re-import.\n" +
                    "First offending column encountered = " + offendingCol,
                    "Import Data Error - Column names have spaces.", MessageBoxButtons.OK);
                return;
            }

            dsControl1.DT = dataDT.Copy();

            //enable validation_click
            //init in case they've re-imported
            dsControl1.dgv.DataSource = null;
            dsControl1.dgv.DataSource = dsControl1.DT;
                        
            //set extendedProperties on columns
            for (int c = 0; c < dsControl1.DT.Columns.Count; c++)
            {
                dsControl1.DT.Columns[c].ExtendedProperties[VBCommon.Globals.MAINEFFECT] = true;
                if (c == 0)
                {
                    dsControl1.DT.Columns[c].ExtendedProperties[VBCommon.Globals.DATETIMESTAMP] = true;
                    dsControl1.dgv.Columns[c].ReadOnly = true; //cannot edit this
                }
                if (c == 1) dsControl1.DT.Columns[c].ExtendedProperties[VBCommon.Globals.DEPENDENTVAR] = true;
                dsControl1.dgv.Columns[c].SortMode = DataGridViewColumnSortMode.NotSortable;
                dsControl1.dgv.Columns[c].DefaultCellStyle.ForeColor = Color.Black;
            }

            //RowInformation and ColumnInformation to track which rows, columns are enabled/disabled.
            dsControl1.DTRI = new VBCommon.Metadata.dtRowInformation(dsControl1.DT);
            dsControl1.DTCI = new VBCommon.Metadata.dtColumnInformation(dsControl1.DT);

            //default col 1 as response
            dsControl1.SelectedColIndex = 1;
            dsControl1.ResponseVarColIndex = 1;
            dsControl1.ResponseVarColName = dsControl1.DT.Columns[1].Caption;

            //initial info for the list
            FileInfo fi = new FileInfo(import.getFileImportedName);
            dsControl1.FileName = fi.Name;
            dsControl1.showListInfo(dsControl1.FileName, dsControl1.DT);
            dsControl1.dgv.Enabled = false;

            dsControl1.maintainGrid(dsControl1.dgv, dsControl1.DT, dsControl1.SelectedColIndex, dsControl1.ResponseVarColName);
        }


        //check for spaces
        private bool datasetHasSpacesinColNmaes(DataTable dataDT, out string name)
        {
            name = string.Empty;
            foreach (DataColumn dc in (dataDT.Columns))
            {
                name = dc.Caption;
                if (name.Contains(" ")) return true;
            }
            return false;
        }


        // test all cells in the datetime column for uniqueness
        private bool recordIndexUnique(DataTable dt, out int where)
        {
            Dictionary<string, int> temp = new Dictionary<string, int>();
            int ndx = -1;
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string tempval = dr[0].ToString();
                    temp.Add(dr[0].ToString(), ++ndx);
                    if (string.IsNullOrWhiteSpace(dr[0].ToString()))
                    {
                        where = ndx++;
                        return false;
                    }
                }
            }
            catch (ArgumentException)
            {
                where = ndx++;
                return false;
            }
            where = ndx;
            return true;
        }


        //validate data
        public void btnValidateData_Click(object sender, EventArgs e)
        {
            DataTable savedt = dsControl1.DT.Copy();
            frmMissingValues frmMissing = new frmMissingValues(dsControl1.dgv, dsControl1.DT);
            frmMissing.ShowDialog();
            //when whatever checks we're doing are good, enable the manipulation buttons
            if (frmMissing.Status)
            {
                dsControl1.DT = frmMissing.ValidatedDT;
                dsControl1.dgv.Enabled = true;
                //update list in case they've deleted cols/rows
                dsControl1.updateListView(VBCommon.Globals.listvals.NCOLS, dsControl1.DT.Columns.Count);
                int nonivs = dsControl1.HiddenCols > 0 ? 3 : 2;
                dsControl1.NumberIVs = dsControl1.DT.Columns.Count - nonivs;
                dsControl1.updateListView(VBCommon.Globals.listvals.NIVS, dsControl1.NumberIVs);
                int recount = dsControl1.DT.Rows.Count;
                dsControl1.updateListView(VBCommon.Globals.listvals.NROWS, recount);
                dsControl1.State = VBCommon.Controls.DatasheetControl.dtState.dirty;
                boolValidated = true;
            }
            else
            {
                dsControl1.DT = savedt;
                dsControl1.dgv.DataSource = dsControl1.DT;
                dsControl1.dgv.Enabled = false;
                dsControl1.DTRI = new VBCommon.Metadata.dtRowInformation(dsControl1.DT);
                dsControl1.DTCI = new VBCommon.Metadata.dtColumnInformation(dsControl1.DT);
                boolValidated = false;
            }
        }


        public void SetLocation(IDictionary<string, object> dictPackedLocation)
        {
            dblOrientation = Convert.ToDouble(dictPackedLocation["Orientation"]);
        }

        // invoke the wind/current component decomposition tool
        // creates data columns of orthogonal wind/current components
        public void btnComputeAO_Click(object sender, EventArgs e)
        {
            //just adds columns for wind and/or current components to the datatable/grid view
            //this will need some sort of property setting mechanisms to reset columns to
            //hidden, enabled, etc when the form for decomposition exits.
            if (dsControl1.DT != null)
            {
                //DataTable dt = filterDataTableCols(_dt);
                DataTable dt = dsControl1.filterDisabledCols(dsControl1.DT);
                string rvname = dsControl1.ResponseVarColName;
                string dtsname = dt.Columns[0].Caption;

                VBCommon.Controls.frmUV frmWC = new VBCommon.Controls.frmUV(dt, rvname, dtsname, dblOrientation);
                //frmUV frmWC = new frmUV(dsControl1.DT, dsControl1.ResponseVarColName, dtsname);
                frmWC.ShowDialog();
                
                DataTable dtnew = frmWC.WCDT;

                //this will effect to enable column context menus on new columns
                foreach (DataColumn c in dtnew.Columns)
                {
                    if (!dsControl1.DTCI.GetColStatus(c.ColumnName))
                        dsControl1.DTCI.AddColumnNameToDict(c.ColumnName);

                }
                //add disabled col back in
                dtnew = dsControl1.addDisabledCols(dtnew, dsControl1.DT);
                //mark created cols as decomposition
                List<string> newcols = frmWC.WCColsAdded;
                foreach (string colname in newcols)
                {
                    dtnew.Columns[colname].ExtendedProperties[VBCommon.Globals.DECOMPOSITION] = true;
                }

                //                _dt = dtnew;
                dsControl1.DT = dtnew;
                dsControl1.dgv.DataSource = dtnew;
                //                dgv.DataSource = dtnew;
                dsControl1.maintainGrid(dsControl1.dgv, dsControl1.DT, dsControl1.SelectedColIndex, dsControl1.ResponseVarColName);
                //                _gridutils.maintainGrid(dgv, _dt, _selectedColIndex, _responseVarColName);
                //_gridutils.setViewOnGrid(dgv);

                //count IVs and update list
                int nonivs = dsControl1.HiddenCols > 0 ? 3 : 2;
                //                int nonivs = _nhiddencols > 0 ? 3 : 2;
                dsControl1.NumberIVs = dsControl1.DT.Columns.Count - nonivs;
                //                _nivs = _dt.Columns.Count - nonivs;
                dsControl1.updateListView(VBCommon.Globals.listvals.NIVS, dsControl1.NumberIVs);
                //                updateListView(_listvals.NIVS, _nivs);
                dsControl1.updateListView(VBCommon.Globals.listvals.NCOLS, dsControl1.DT.Columns.Count);
                //                updateListView(_listvals.NCOLS, _dt.Columns.Count);
                dsControl1.State = DatasheetControl.dtState.dirty;
                //                _state = _dtState.dirty;

            }
            else
            {
                MessageBox.Show("Must import data first.", "Proceedural Error", MessageBoxButtons.OK);
            }
        }


        /// <summary>
        /// sets the table display order for transformed data columns
        /// finds main effect and sets the transform column next to it
        /// </summary>
        /// <param name="dtnew">table to arrange</param>
        /// <param name="dc">transformed data column</param>
        /// <returns>re-arranged table</returns>
        private DataTable arrangeTableCols(DataTable dtnew, DataColumn dc)
        {
            string me = string.Empty;
            DataTable dtcopy = dtnew.Copy();
            //added if clause for new column nameing conventions: T(x) == T[COLNAME] or T[p1, <p2, p3>, COLNAME]
            //changed to T[COLNAME, p1, <P2, p3>]
            if (dc.Caption.Contains(","))
            {
                //me = dc.Caption.Substring(dc.Caption.IndexOf("[") + 1, (dc.Caption.IndexOf(",") - dc.Caption.Length(dc.Caption.IndexOf("[") ) ) );
                int start = dc.Caption.IndexOf("[") + 1;
                int stop = dc.Caption.IndexOf(",") - start;
                me = dc.Caption.Substring(start, stop);

            }
            else
            {
                me = dc.Caption.Substring(dc.Caption.IndexOf("[") + 1, (dc.Caption.Length - dc.Caption.IndexOf("[") - 2));
            }
            int pos = dtnew.Columns.IndexOf(me);
            if (pos > 0 && pos < dtcopy.Columns.Count)
            {
                dtcopy.Columns[dc.Caption].SetOrdinal(pos + 1);
                dtcopy.AcceptChanges();
            }
            return dtcopy;
        }


        /// <summary>
        /// invoke the data manipulation tool - allows for creation of data columns of interaction data
        /// (sums, means, products, ...etc)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnManipulate_Click(object sender, EventArgs e)
        {
            if (dsControl1.DT != null)
            {
                DataTable dt = dsControl1.filterDisabledCols(dsControl1.DT);
                int rvndx = dt.Columns.IndexOf(dsControl1.ResponseVarColName);
                //string responseVarColName = dt.Columns[rvndx].Caption;
                frmManipulate frmIA = new frmManipulate(dt, rvndx);
                DialogResult dlgr = frmIA.ShowDialog();

                if (dlgr != DialogResult.Cancel)
                {
                    DataTable dtnew = frmIA.NewDataTable;

                    dsControl1.registerNewCols(dtnew);
                    dtnew = dsControl1.addDisabledCols(dtnew, dsControl1.DT);

                    dsControl1.DT = dtnew;
                    dsControl1.maintainGrid(dsControl1.dgv, dtnew, dsControl1.SelectedColIndex, dsControl1.ResponseVarColName);
                    //dgv.DataSource = dtnew;
                    //_gridutils.setViewOnGrid(dgv);

                    int nonivs = dsControl1.HiddenCols > 0 ? 3 : 2;
                    dsControl1.NumberIVs = dsControl1.DT.Columns.Count - nonivs;
                    dsControl1.updateListView(VBCommon.Globals.listvals.NIVS, dsControl1.NumberIVs);
                    dsControl1.updateListView(VBCommon.Globals.listvals.NCOLS, dsControl1.DT.Columns.Count);
                    //_state = _dtState.dirty;
                }

            }
            else
            {
                MessageBox.Show("Must import data first.", "Proceedural Error", MessageBoxButtons.OK);
            }


        }

        /// <summary>
        /// invoke the independent variable transforms tool
        /// creates columns of data transforms
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnTransform_Click(object sender, EventArgs e)
        {
            if (dsControl1.DT != null)
            {
                //filter disabled cols
                DataTable dt = dsControl1.filterDisabledCols(dsControl1.DT);

                //filter transformed cols
                dt = dsControl1.filterTcols(dt);

                //filter hidden (untransformed) dependent variable
                dt = dsControl1.filterRVHcols(dt);

                //filter cat vars
                dt = dsControl1.filterCatVars(dt);

                int rvndx = dt.Columns.IndexOf(dsControl1.ResponseVarColName);
                //string responseVarColName = dt.Columns[rvndx].Caption;
                frmTransform frmT = new frmTransform(dt, rvndx);
                DialogResult dlgr = frmT.ShowDialog();

                if (dlgr != DialogResult.Cancel)
                {
                    DataTable dtnew = frmT.PCDT;
                    
                    dsControl1.registerNewCols(dtnew);

                    //add any disabled columns back into the operational datatable
                    dtnew = dsControl1.addDisabledCols(dtnew, dsControl1.DT);

                    //add any response variables back ...
                    dtnew = dsControl1.addHiddenResponseVarCols(dtnew, dsControl1.DT);

                    //add any previously transform cols back
                    //dtnew = addOldTCols(dtnew, _dt);
                    dtnew = dsControl1.addOldTCols(dtnew, dsControl1.DT);

                    dtnew = dsControl1.addCatCols(dtnew, dsControl1.DT);

                    foreach (DataColumn dc in dtnew.Columns)
                    {
                        if (dc.ExtendedProperties.ContainsKey(VBCommon.Globals.TRANSFORM)) //||
                            //dc.ExtendedProperties.ContainsKey(VBTools.Globals.OPERATION) ||
                            //dc.ExtendedProperties.ContainsKey(VBTools.Globals.DECOMPOSITION) )
                            dtnew = arrangeTableCols(dtnew, dc);
                    }

                    //update global table
                    dsControl1.DT = dtnew;
                    dsControl1.DT.AcceptChanges();

                    //grid operations
                    dsControl1.SelectedColIndex = dsControl1.DT.Columns.IndexOf(dsControl1.ResponseVarColName);
                    dsControl1.ResponseVarColIndex = dsControl1.DT.Columns.IndexOf(dsControl1.ResponseVarColName);
                    dsControl1.maintainGrid(dsControl1.dgv, dsControl1.DT, dsControl1.SelectedColIndex, dsControl1.ResponseVarColName);

                    //count iv columns and update list
                    int nonivs = dsControl1.HiddenCols > 0 ? 3 : 2;
                    dsControl1.NumberIVs = dsControl1.DT.Columns.Count - nonivs;
                    dsControl1.updateListView(VBCommon.Globals.listvals.NIVS, dsControl1.NumberIVs);
                    dsControl1.updateListView(VBCommon.Globals.listvals.NCOLS, dsControl1.DT.Columns.Count);

                    //_state = _dtState.dirty;
                }
            }
            else
            {
                MessageBox.Show("Must import data first.", "Proceedural Error", MessageBoxButtons.OK);
            }


        }

    }
}
