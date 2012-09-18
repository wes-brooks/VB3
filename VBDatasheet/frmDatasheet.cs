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

        private bool boolInitialPass = true;
        private bool boolValidated = false;

        public event EventHandler ChangeMade4Stack;

        // getter/setter for datasheet table
        [JsonProperty]
        public IDictionary<string, object> PackedDatasheetState
        {
            set { dictPackedDatasheetState = value; }
            get { return dictPackedDatasheetState; }
        }


        //returns validated flag
        [JsonProperty]
        public bool DSValidated
        {
            get { return this.boolValidated; }
        }


        //constructor
        public frmDatasheet()    
        {
            InitializeComponent();
            this.dsControl1.NotifiableChangeEvent +=new EventHandler(this.UpdateData);
         
        }


        //unpack event handler. unpacks packed state in dictionary to repopulate datasheet
        public void UnpackState(IDictionary<string, object> dictPluginState)
        {
            //unpack datasheet control
           
            PackedDatasheetState = (IDictionary<string, object>)dictPluginState["PackedDatasheetState"];

            dsControl1.UnpackState(PackedDatasheetState);

            //get validated flag
            this.boolValidated = (bool)dictPluginState["DSValidated"];

            //if changesMade is true then initialPass is false
            if ((bool)dictPluginState["ChangesMadeDS"])
                boolInitialPass = false;
            else
                boolInitialPass = true;
           
            //if clean, initial pass is false
            //if (dsControl1.State.ToString() == "clean")
            //    boolInitialPass = false;
            //else
            //    boolInitialPass = true;
        }


        //event handler for packing state to save project
        public IDictionary<string, object> PackState()
        {
            //make sure user chose transformation on response var
            if (dsControl1.DependentVariableTransform == VBCommon.DependentVariableTransforms.none)
            {
                MessageBox.Show("You must define the transformation on the response variable before continuing");
                return null;
            }
            //save packed state to a dictionary
            IDictionary<string, object> dictPluginState = new Dictionary<string, object>();

            bool boolChangesMadeDS = false; ; //holds flag to clear model if changes made to ds

            //have left and come back
            if (!boolInitialPass)
            {      
                boolChangesMadeDS = true;
                //dsControl1.State = VBCommon.Controls.DatasheetControl.dtState.clean;
                
            }
            //else if (boolInitialPass) //haven't left and came back here
            //{
            //    dsControl1.State = VBCommon.Controls.DatasheetControl.dtState.clean;
            //    boolInitialPass = false;

            //}

            dictPackedDatasheetState = dsControl1.PackState();
            dictPluginState.Add("PackedDatasheetState", dictPackedDatasheetState);
            
            dictPluginState.Add("DSValidated", boolValidated);

            //pack the state of the datasheet so model will know if it needs to clear itself
            dictPluginState.Add("ChangesMadeDS", boolChangesMadeDS);

            return dictPluginState;
        }


        //listener for notify change event
        public void UpdateData(object source, EventArgs e)
        {
            //broadcast to be added to stack
            if (ChangeMade4Stack != null)
            {
                EventArgs ev = new EventArgs();
                ChangeMade4Stack(this, ev);
            }
        }


        //import datatable
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
                    dsControl1.dgv.Columns[c].ReadOnly = true; //cannot edit this puppy..... editable makes it breakable
                }
                if (c == 1) dsControl1.DT.Columns[c].ExtendedProperties[VBCommon.Globals.DEPENDENTVAR] = true;
                dsControl1.dgv.Columns[c].SortMode = DataGridViewColumnSortMode.NotSortable;
                dsControl1.dgv.Columns[c].DefaultCellStyle.ForeColor = Color.Black;
            }

            //initialize rows to all enabled for imported table
            //(builds dictionary of keys, <string>datetime and values <bool>enabled/disabled row)
            dsControl1.DTRI = new VBCommon.Metadata.dtRowInformation(dsControl1.DT);
            //initialize cols to all enabled for imported table
            //(builds dictionary of keys, <string>datetime and values <bool>enabled/disabled col)
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
           
            boolInitialPass = true;
            boolValidated = false;
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
                dsControl1.DT = frmMissing.ValidatedDT; //keep the grid (updated in tool) data but update the global table
                dsControl1.dgv.Enabled = true;
                //update list in case they've deleted cols/rows
                dsControl1.updateListView(VBCommon.Controls.DatasheetControl.listvals.NCOLS, dsControl1.DT.Columns.Count);
                int nonivs = dsControl1.HiddenCols > 0 ? 3 : 2;
                dsControl1.NumberIVs = dsControl1.DT.Columns.Count - nonivs;
                dsControl1.updateListView(VBCommon.Controls.DatasheetControl.listvals.NIVS, dsControl1.NumberIVs);
                int recount = dsControl1.DT.Rows.Count;
                dsControl1.updateListView(VBCommon.Controls.DatasheetControl.listvals.NROWS, recount);
                //_state = _dtState.clean;
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


        //change initialPass value if leaving datasheet
        public void btnGoToModel_Click()
        {
            boolInitialPass = false;
        }


        // invoke the wind/current component decomposition tool
        // creates data columns of orthogonal wind/current components
        public void btnComputeAO_Click(object sender, EventArgs e)
        {
            //just adds columns for wind and/or current components to the datatable/grid view
            //this will need some sort of property setting mechanisms to reset columns to
            //hidden, enabled, etc when the form for decomposition exits.
            //if (dsControl1.DT != null)
            //{
            //    DataTable dt = filterDataTableCols(_dt);
            //    DataTable dt = dsControl1.TableUtils.filterDisabledCols(dsControl1.DT);
            //    string rvname = _dt.Columns[_responseVarColIndex].ColumnName.ToString();
            //    string dtsname = dt.Columns[0].Caption;

            //    frmUV frmWC = new frmUV(dt, rvname, dtsname);
            //    frmUV frmWC = new frmUV(dsControl1.DT, dsControl1.ResponseVarColName, dtsname);
            //    frmWC.ShowDialog();

            //    DataTable dtnew = frmWC.WCDT;

            //    this will effect to enable column context menus on new columns
            //    foreach (DataColumn c in dtnew.Columns)
            //    {
            //        if (!dsControl1.DTCI.getColStatus(c.ColumnName))
            //            dsControl1.DTCI.addColumnNameToDic(c.ColumnName);

            //    }
            //    add disabled col back in
            //    dtnew = dsControl1.TableUtils.addDisabledCols(dtnew, dsControl1.DT);
            //    mark created cols as decomposition
            //    List<string> newcols = frmWC.WCColsAdded;
            //    foreach (string colname in newcols)
            //    {
            //        dtnew.Columns[colname].ExtendedProperties[VBCommon.Globals.DECOMPOSITION] = true;
            //    }

            //                    _dt = dtnew;
            //    dsControl1.DT = dtnew;
            //    dsControl1.dgv.DataSource = dtnew;
            //                    dgv.DataSource = dtnew;
            //    dsControl1.GridUtils.maintainGrid(dsControl1.dgv, dsControl1.DT, dsControl1.SelectedColIndex, dsControl1.ResponseVarColName);
            //                    _gridutils.maintainGrid(dgv, _dt, _selectedColIndex, _responseVarColName);
            //    _gridutils.setViewOnGrid(dgv);

            //    count IVs and update list
            //    int nonivs = dsControl1.HiddenCols > 0 ? 3 : 2;
            //                    int nonivs = _nhiddencols > 0 ? 3 : 2;
            //    dsControl1.NumberIVs = dsControl1.DT.Columns.Count - nonivs;
            //                    _nivs = _dt.Columns.Count - nonivs;
            //    dsControl1.updateListView(DatasheetControl.listvals.NIVS, dsControl1.NumberIVs);
            //                    updateListView(_listvals.NIVS, _nivs);
            //    dsControl1.updateListView(DatasheetControl.listvals.NCOLS, dsControl1.DT.Columns.Count);
            //                    updateListView(_listvals.NCOLS, _dt.Columns.Count);
            //    dsControl1.State = DatasheetControl.dtState.dirty;
            //                    _state = _dtState.dirty;

            //}
            //else
            //{
            //    MessageBox.Show("Must import data first.", "Proceedural Error", MessageBoxButtons.OK);
            //}
        }


        public void btnManipulate_Click(object sender, EventArgs e)
        {

        }


        public void btnTransform_Click(object sender, EventArgs e)
        {

        }

    }
}
