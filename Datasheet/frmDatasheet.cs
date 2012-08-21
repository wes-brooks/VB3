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
using VBCommon;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Datasheet;
using VBCommon;
using VBCommon.Controls;
using VBCommon.PluginSupport;
using VBCommon.Metadata;
using VBCommon.Interfaces;

namespace Datasheet
{
    [JsonObject]
    public partial class frmDatasheet : UserControl, IFormState
    {
        //context menus
        //private ContextMenu cmforResponseVar = new ContextMenu();
        //private ContextMenu cmforIVs = new ContextMenu(); 
        //private ContextMenu cmforRows = new ContextMenu(); 
        ////table information
        //private int intSelectedColIndex = -1;            
        //private int intSelectedRowIndex = -1;     
        //private int intResponseVarColIndex = 1;  
        //private string strResponseVarColName = string.Empty;
        //private string strSelectedColName = string.Empty;   

        private enum AddReplace { Add, Replace };     
        private AddReplace addreplace;                   

        //private DataTable dt = null;                    
        //private dtRowInformation dtRI = null;            
        //private dtColumnInformation dtCI = null;       

        //private Utilities utils = null;                   
        //private Utilities.TableUtils tableutils = null;     
        //private Utilities.GridUtils gridutils = null;        
        private bool boolClean = true;

        public event EventHandler DataImported;
       
        //private enum listvals {NCOLS, NROWS, DATECOLNAME, RVCOLNAME, BLANK, NDISABLEDROWS, NDISABLEDCOLS, NHIDDENCOLS, NIVS};  //
        //private Type ListVal = typeof(listvals);    
        //private int intNdisabledcols = 0;     
        //private int intNdisabledrows = 0;   
        //private int intNhiddencols = 0;     
        //private int intNivs = 0;     

        //data state relative to data used in modeling/residuals/prediction
        //state is dirty until the project manager's version of the datatable
        //matches the filtered "gotomodeling" datatable version
        //public enum dtState { clean, dirty };        
        //private dtState state = dtState.dirty;

        private DataTable savedDT = null;
        private DataGridView savedDGV = null;
        private IDictionary<string, object> dictPackedDatasheetState = null;

        private bool boolInitialPass = true;
        private bool boolValidated = false;
        //public event EventHandler ResetModel;
        
        //dealing with transform
        //private VBCommon.DependentVariableTransforms depVarTransform; 
        //private double dblPowerTransformExp = double.NaN;                       
        private string strXmlDataTable = string.Empty;
        private DataTable dataSheetData = null;
        private DataTable correlationData = null;
        private DataTable modelData = null;
        //public string fn = string.Empty; 

        // getter/setter for transform type
        //public VBCommon.DependentVariableTransforms DependentVariableTransform 
        //{
        //    get { return depVarTransform; }
        //    set { depVarTransform = value; }
        //}


        //// getter/setter for power tranform
        //public double PowerTransformExponent  
        //{
        //    get { return dblPowerTransformExp; }
        //    set { dblPowerTransformExp = value; }
        //}

        
        // return datatable as xml string
        public string XmlDataTable
        {
            get { return strXmlDataTable; }
        }


        // getter/setter for datasheet table
        [JsonProperty]
        public IDictionary<string, object> PackedDatasheetState
        {
            set { dictPackedDatasheetState = value; }
            get { return dictPackedDatasheetState; }
        }


        // getter/setter for datasheet table
        [JsonProperty]
        public DataTable DataSheetDataTable
        {
            set { dataSheetData = value; }
            get { return dataSheetData; }
        }


        // getter/setter for correlation data table
        [JsonProperty]
        public DataTable CorrelationDataTable
        {
            get { return correlationData; }
            set { correlationData = value; }
        }

        
        // getter/setter for model datatable
        public DataTable ModelDataTable
        {
            get { return modelData; }
            set { modelData = value; }
        }

        
        //// getter/setter for datatable
        //[JsonProperty]
        //public DataTable DT 
        //{
        //    get { return this.dt; }
        //}


        ////returns datatable row info
        //[JsonProperty]
        //public Dictionary<string, bool> DTRowInfo 
        //{
        //    get { return this.dtRI.DTRowInfo; }
        //}


        //// returns datatable column info
        //[JsonProperty]
        //public Dictionary<string, bool> DTColInfo  
        //{
        //    get { return this.dtCI.DTColInfo; }
        //}


        //returns current selected column index
        //[JsonProperty]
        //public int CurrentColIndex   
        //{
        //    get { return this.intSelectedColIndex; }
        //}


        ////returns dependent variable column name
        //[JsonProperty]
        //public string DepVarColName 
        //{
        //    get { return this.strResponseVarColName; }
        //}


        //returns validated flag
        [JsonProperty]
        public bool DSValidated
        {
            get { return this.boolValidated; }
        }


        //returns clean flag
        [JsonProperty]
        public bool Clean
        {
            get { return this.boolClean; }
        }
        

        //constructor
        public frmDatasheet()    
        {
            InitializeComponent();
         
        }

        
        /*//unpack event handler. unpacks packed state in dictionary to repopulate datasheet
        public void UnpackState(IDictionary<string, object> dictPluginState)
        {
            //unpack datatable
            
            dsControl1.DT = (DataTable)dictPluginState["DT"];
            dsControl1.DT.TableName = "DataSheetData";
            dsControl1.dgv.DataSource = null;
            dsControl1.dgv.DataSource = dsControl1.DT;

            //get row and column information
            dsControl1.DTRI = VBCommon.Metadata.dtRowInformation.getdtRI(dsControl1.DT, true);
            dsControl1.DTRI.DTRowInfo = (Dictionary<string, bool>)dictPluginState["DTRowInfo"];

            dsControl1.DTCI = VBCommon.Metadata.dtColumnInformation.getdtCI(dsControl1.DT, true);
            dsControl1.DTCI.DTColInfo = (Dictionary<string, bool>)dictPluginState["DTColInfo"];

            dsControl1.SelectedColIndex = (int)dictPluginState["CurrentColIndex"];
            dsControl1.ResponseVarColName = (string)dictPluginState["DepVarColName"];
            dsControl1.ResponseVarColIndex = dsControl1.DT.Columns.IndexOf(dsControl1.ResponseVarColName);
            //get validated flag
            this.boolValidated = (bool)dictPluginState["DSValidated"];

            dsControl1.Utils = new VBCommon.Metadata.Utilities();
            dsControl1.TableUtils = new VBCommon.Metadata.Utilities.TableUtils(dsControl1.DT);
            dsControl1.GridUtils = new VBCommon.Metadata.Utilities.GridUtils(dsControl1.dgv);

            dsControl1.GridUtils.maintainGrid(dsControl1.dgv, dsControl1.DT, dsControl1.SelectedColIndex, dsControl1.ResponseVarColName);

            //initial info for the list
            FileInfo fi = new FileInfo( Name);
            dsControl1.FileName = fi.Name;
            dsControl1.showListInfo(dsControl1.FileName, dsControl1.DT);

            if ((bool)dictPluginState["Clean"])
            {
                dsControl1.State = VBCommon.Controls.DatasheetControl.dtState.clean;
            }
            else
            {
                dsControl1.State = VBCommon.Controls.DatasheetControl.dtState.dirty;
            }

            //if clean, initial pass is false
            boolInitialPass = !(bool)dictPluginState["Clean"];
        } */


        //unpack event handler. unpacks packed state in dictionary to repopulate datasheet
        public void UnpackState(IDictionary<string, object> dictPluginState)
        {
            //unpack datasheet control
           
            PackedDatasheetState = (IDictionary<string, object>)dictPluginState["PackedDatasheetState"];

            dsControl1.UnpackState(PackedDatasheetState);

            
            //get validated flag
            this.boolValidated = (bool)dictPluginState["DSValidated"];

            //initial info for the list
            FileInfo fi = new FileInfo(Name);


            /*if ((bool)dictPluginState["Clean"])
            {
                dsControl1.State = VBCommon.Controls.DatasheetControl.dtState.clean;
            }
            else
            {
                dsControl1.State = VBCommon.Controls.DatasheetControl.dtState.dirty;
            }*/

            //if clean, initial pass is false
            boolInitialPass = !(bool)dictPluginState["Clean"];
        }

        
        /*//event handler for packing state to save project
        public IDictionary<string, object> PackState()
        {
            //save packed state to a dictionary
            IDictionary<string, object> dictPluginState = new Dictionary<string, object>();

            //check to see if this is the first time going to modeling
            if (dsControl1.State == VBCommon.Controls.DatasheetControl.dtState.dirty && !boolInitialPass)
            {
                DialogResult dlgr = MessageBox.Show("Changes in data and/or data attributes have occurred.\nPrevious modeling results will be erased. Proceed?", "Proceed to Modeling.", MessageBoxButtons.OKCancel);
                if (dlgr == DialogResult.OK)
                {
                    correlationData = dsControl1.DT;
 //                   savedDT = dt;                 //dont see this being used anywhere
                    dataSheetData = dsControl1.DT;
                    dsControl1.State = VBCommon.Controls.DatasheetControl.dtState.clean;
                }
                else
                { return null; }
            }
            else if (boolInitialPass)
            {
                correlationData = dsControl1.DT;
                modelData = dsControl1.DT;
                dsControl1.State = VBCommon.Controls.DatasheetControl.dtState.clean;
                boolInitialPass = false;
//                savedDT = dt;                   //dont see this being used anywhere
//                savedDGV = dsControl1.dgv;     //dont see this being used anywhere
            }

            dictPluginState.Add("CorrelationDataTable", dsControl1.DT); //for Modeling to use
            dictPluginState.Add("ModelDataTable", dsControl1.DT);   //for Modeling to use
            dictPluginState.Add("DT", dsControl1.DT);
            //pack up mainEffect columns for Prediction
            dictPluginState.Add("CurrentColIndex", dsControl1.SelectedColIndex);
            dictPluginState.Add("DepVarColName", dsControl1.ResponseVarColName);
            dictPluginState.Add("DTColInfo", dsControl1.DTCI.DTColInfo);
            dictPluginState.Add("DTRowInfo", dsControl1.DTRI.DTRowInfo);
            dictPluginState.Add("DSValidated", boolValidated);

            //pack up listInfo for model datasheet
            int intNumCols = dsControl1.DT.Columns.Count;
            int intNumRows = dsControl1.DT.Rows.Count;
            string strDateName = dsControl1.DT.Columns[0].ColumnName.ToString();
            string strResponseVar = dsControl1.DT.Columns[1].ColumnName.ToString();
            dictPluginState.Add("ColCount", intNumCols);
            dictPluginState.Add("RowCount", intNumRows);
            dictPluginState.Add("DateIndex", strDateName);
            dictPluginState.Add("ResponseVar", strResponseVar);
            dictPluginState.Add("DisabledRwCt", dsControl1.DisabledRows);
            dictPluginState.Add("DisabledColCt", dsControl1.DisabledCols);
            dictPluginState.Add("HiddenColCt", dsControl1.HiddenCols);
            dictPluginState.Add("IndVarCt", dsControl1.NumberIVs);
            dictPluginState.Add("fileName", dsControl1.FileName);

            StringWriter sw = null;
            //Save Datasheet info as xml string for serialization
            sw = null;
            if (dsControl1.DT != null)
            {
                dsControl1.DT.TableName = "DataSheetData";
                sw = new StringWriter();
                dsControl1.DT.WriteXml(sw, XmlWriteMode.WriteSchema, false);
                strXmlDataTable = sw.ToString();
                sw.Close();
                sw = null;
                dictPluginState.Add("XmlDataTable", strXmlDataTable);
            }

            if (dsControl1.State == VBCommon.Controls.DatasheetControl.dtState.clean)
            {
                boolClean = true;
                dictPluginState.Add("Clean", boolClean);
            }
            else
            {
                boolClean = false;
                dictPluginState.Add("Clean", boolClean);
            }

            //model expects this change to the dt first
            DataTable tempDt = (DataTable)dictPluginState["DT"];
            tempDt.Columns[dsControl1.ResponseVarColName].SetOrdinal(1);
            //filter diabled rows and columns
            tempDt = dsControl1.filterDataTableRows(tempDt);
            Utilities.TableUtils tableutils = new Utilities.TableUtils(tempDt);
            tempDt = tableutils.filterRVHcols(tempDt);
            dictPluginState.Add("DataSheetDatatable", tempDt);  //for modeling to use
            
            return dictPluginState;
        }*/


        //event handler for packing state to save project
        public IDictionary<string, object> PackState()
        {
            //save packed state to a dictionary
            IDictionary<string, object> dictPluginState = new Dictionary<string, object>();

            //check to see if this is the first time going to modeling
            if (dsControl1.State == VBCommon.Controls.DatasheetControl.dtState.dirty && !boolInitialPass)
            {
                
                DialogResult dlgr = MessageBox.Show("Changes in data and/or data attributes have occurred.\nPrevious modeling results will be erased. Proceed?", "Proceed to Modeling.", MessageBoxButtons.OKCancel);
                if (dlgr == DialogResult.OK)
                {
                    correlationData = dsControl1.DT;
                    //  savedDT = dt;                 //dont see this being used anywhere
                    dataSheetData = dsControl1.DT;
                    dsControl1.State = VBCommon.Controls.DatasheetControl.dtState.clean;
                }
                else
                { return null; }
            }
            else if (boolInitialPass)
            {
                correlationData = dsControl1.DT;
                modelData = dsControl1.DT;
                dsControl1.State = VBCommon.Controls.DatasheetControl.dtState.clean;
                boolInitialPass = false;
                //   savedDT = dt;                   //dont see this being used anywhere
                //   savedDGV = dsControl1.dgv;     //dont see this being used anywhere
            }

            dictPackedDatasheetState = dsControl1.PackState();
            dictPluginState.Add("PackedDatasheetState", dictPackedDatasheetState);
            
            dictPluginState.Add("DSValidated", boolValidated);

            StringWriter sw = null;
            //Save Datasheet info as xml string for serialization
            sw = null;
            if (dsControl1.DT != null)
            {
                dsControl1.DT.TableName = "DataSheetData";
                sw = new StringWriter();
                dsControl1.DT.WriteXml(sw, XmlWriteMode.WriteSchema, false);
                strXmlDataTable = sw.ToString();
                sw.Close();
                sw = null;
                dictPluginState.Add("XmlDataTable", strXmlDataTable);
            }

            if (dsControl1.State == VBCommon.Controls.DatasheetControl.dtState.clean)
            {
                boolClean = true;
                dictPluginState.Add("Clean", boolClean);
            }
            else
            {
                boolClean = false;
                dictPluginState.Add("Clean", boolClean);
            }

            return dictPluginState;
        }


        /// <summary>
        /// load the datasheet form, initialize then gridview's menu items/eventhandlers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmDatasheet_Load(object sender, EventArgs e) 
        {

            //is this where I'd add the DSControl??

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
            dsControl1.DTRI = VBCommon.Metadata.dtRowInformation.getdtRI(dsControl1.DT, true);
            //initialize cols to all enabled for imported table
            //(builds dictionary of keys, <string>datetime and values <bool>enabled/disabled col)
            dsControl1.DTCI = VBCommon.Metadata.dtColumnInformation.getdtCI(dsControl1.DT, true);

            //init the utilities
            dsControl1.Utils = new VBCommon.Metadata.Utilities();
            dsControl1.TableUtils = new VBCommon.Metadata.Utilities.TableUtils(dsControl1.DT);
            dsControl1.GridUtils = new VBCommon.Metadata.Utilities.GridUtils(dsControl1.dgv);

            //default col 1 as response
            dsControl1.SelectedColIndex = 1;
            dsControl1.ResponseVarColIndex = 1;
            dsControl1.ResponseVarColName = dsControl1.DT.Columns[1].Caption;
            dsControl1.GridUtils.setResponseVarCol(dsControl1.dgv, dsControl1.SelectedColIndex,dsControl1.SelectedColIndex);
            dsControl1.GridUtils.setViewOnGrid(dsControl1.dgv);

            //initial info for the list
            FileInfo fi = new FileInfo(import.getFileImportedName);
            dsControl1.FileName = fi.Name;
            dsControl1.showListInfo(dsControl1.FileName, dsControl1.DT);

            dsControl1.dgv.Enabled = false;
            boolInitialPass = true;
            boolValidated = false;
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


        /// <summary>
        /// test all cells in the datetime column for uniqueness
        /// </summary>
        /// <param name="dt">table to search</param>
        /// <param name="where">record number of offending timestamp</param>
        /// <returns>true iff all unique, false otherwise</returns>
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
                dsControl1.DTRI = VBCommon.Metadata.dtRowInformation.getdtRI(dsControl1.DT, true);
                dsControl1.DTCI = VBCommon.Metadata.dtColumnInformation.getdtCI(dsControl1.DT, true);
                boolValidated = false;
            }
        }


        public void btnComputeAO_Click(object sender, EventArgs e)
        {

        }


        public void btnManipulate_Click(object sender, EventArgs e)
        {

        }


        public void btnTransform_Click(object sender, EventArgs e)
        {

        }

    }
}
