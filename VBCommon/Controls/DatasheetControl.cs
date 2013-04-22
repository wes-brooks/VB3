using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using VBCommon;
using VBCommon.Metadata;
using System.IO;


namespace VBCommon.Controls
{
    public partial class DatasheetControl : UserControl
    {
         //context menus
        private ContextMenu cmforResponseVar = new ContextMenu();  
        private ContextMenu cmforIVs = new ContextMenu();           
        private ContextMenu cmforRows = new ContextMenu();
        
        //hold incoming datatable (where does this get set for each time?)
        private DataTable dt = null;
        private dtRowInformation dtRI = null;
        private dtColumnInformation dtCI = null;

        //This event alerts the containing control to the fact that a notifiable change has been made to the datasheet.
        public event EventHandler NotifiableChangeEvent;

        private int intSelectedColIndex = -1;
        private int intSelectedRowIndex = -1;
        private int intResponseVarColIndex = 1;
        private string strResponseVarColName = string.Empty;
        private string strResponseVarColNameAsImported = string.Empty;
        private string strSelectedColName = string.Empty;
        private dynamic initialValue;

        private enum AddReplace { Add, Replace };
        private AddReplace addreplace;

        //data state relative to data used in modeling/residuals/prediction
        public enum dtState { clean, dirty };
        private dtState state = dtState.dirty;

        //dealing with transform
        private VBCommon.Transforms.DependentVariableTransforms depVarTransform;
        private double dblPowerTransformExp = double.NaN;
        private string fn = string.Empty;
        private int intCheckedMenu;
        private int intCheckedItem;

        private Type ListVal = typeof(VBCommon.Globals.listvals);
        private double dblOrientation;

        // getter/setter for transform type
        [JsonProperty]
        public VBCommon.Transforms.DependentVariableTransforms DependentVariableTransform  
        {
            get { return depVarTransform; }
            set { depVarTransform = value; }
        }


        // getter/setter for power tranform
        [JsonProperty]
        public double PowerTransformExponent   
        {
            get { return dblPowerTransformExp; }
            set { dblPowerTransformExp = value; }
        }


        //get filename for listview
        [JsonProperty]
        public string FileName
        {
            get { return this.fn; }
            set { fn = value; }
        }


        // getter/setter for datatable
        [JsonProperty]
        public DataTable DT     
        {
            get { return this.dt; }
            set 
            { 
                dt = value;
                SetTransformCheckmarks(Menu: 3, Item: 0);
            }
        }

        
        // getter/setter for DTRowInformation
        public dtRowInformation DTRI
        {
            get { return this.dtRI; }
            set { dtRI = value; }
        }


        // getter/setter for DTColumnInformation
        public dtColumnInformation DTCI
        {
            get { return this.dtCI; }
            set { dtCI = value; }
        }


        //returns current selected column index
        [JsonProperty]
        public int SelectedColIndex    
        {
            get { return this.intSelectedColIndex; }
            set { intSelectedColIndex = value; }
        }

        //returns current selected column index
        [JsonProperty]
        public int CurrentSelectedRowIndex
        {
            get { return this.intSelectedRowIndex; }
            set { intSelectedRowIndex = value; }
        }


        //return Response variable column index
        [JsonProperty]
        public int ResponseVarColIndex
        {
            get { return this.intResponseVarColIndex; }
            set { intResponseVarColIndex = value; }
        }

        //returns dependent variable column name
        [JsonProperty]
        public string ResponseVarColName  
        {
            get { return this.strResponseVarColName; }
            set { strResponseVarColName = value; }
        }


        //returns dependent variable column name
        [JsonProperty]
        public string ResponseVarColNameAsImported
        {
            get { return this.strResponseVarColNameAsImported; }
            set { strResponseVarColNameAsImported = value; }
        }


        //return selected column name
        [JsonProperty]
        public string SelectColName
        {
            get { return this.strSelectedColName; }
            set { strSelectedColName = value; }
        }

       
        //return state of datasheet
        [JsonProperty]
        public dtState State
        {
            get { return this.state; }
            set { state = value; }
        }


        public double Orientation
        {
            get { return this.dblOrientation; }
            set { this.dblOrientation = value; }
        }


        //constructor
        public DatasheetControl()      
        {
            InitializeComponent();
            InitializeContextMenus();
        }


        public void Clear()
        {
            listView1.Clear();            

            dt = null;
            dtRI = null;
            dtCI = null;

            dgv.DataSource = null;
            dgv.Update();
        }


        //Notify the containing control that there's been a change to the datasheet
        private void NotifyContainer()
        {
            if (NotifiableChangeEvent != null)
            {
                EventArgs args = new EventArgs();
                NotifiableChangeEvent(this, args);
            }
        }


        // load the datasheet form, initialize then gridview's menu items/eventhandlers
        /*public void datasheet_load(object sender, EventArgs e)      
        {
            //menu items for response variable column
            cmforResponseVar.MenuItems.Add("Transform");
            cmforResponseVar.MenuItems[0].MenuItems.Add("Log10", new EventHandler(log10T));
            cmforResponseVar.MenuItems[0].MenuItems.Add("Ln", new EventHandler(lnT));
            cmforResponseVar.MenuItems[0].MenuItems.Add("Power", new EventHandler(powerT));
            cmforResponseVar.MenuItems.Add("View Plots", new EventHandler(seePlot));
            cmforResponseVar.MenuItems.Add("UnTransform", new EventHandler(UnTransform));
            cmforResponseVar.MenuItems.Add("Define Transform");
            cmforResponseVar.MenuItems[3].MenuItems.Add("none", new EventHandler(defineTransformForRV));                
            cmforResponseVar.MenuItems[3].MenuItems.Add("Log10", new EventHandler(defineTransformForRV));            
            cmforResponseVar.MenuItems[3].MenuItems.Add("Ln", new EventHandler(defineTransformForRV));            
            cmforResponseVar.MenuItems[3].MenuItems.Add("Power", new EventHandler(defineTransformForRV));

            //menu items for iv columns
            cmforIVs.MenuItems.Add("Disable Column", new EventHandler(DisableCol));
            cmforIVs.MenuItems.Add("Enable Column", new EventHandler(EnableCol));
            cmforIVs.MenuItems.Add("Set Response Variable", new EventHandler(SetResponse));
            cmforIVs.MenuItems.Add("View Plots", new EventHandler(seePlot));
            cmforIVs.MenuItems.Add("Delete Column", new EventHandler(DeleteCol));
            cmforIVs.MenuItems.Add("Enable All Columns", new EventHandler(EnableAllCols));

            //menu items for rows 
            cmforRows.MenuItems.Add("Disable Row", new EventHandler(DisableRow));
            cmforRows.MenuItems.Add("Enable Row", new EventHandler(EnableRow));
            cmforRows.MenuItems.Add("Enable All Rows", new EventHandler(EnableAllRows));

            SetTransformCheckmarks(Menu:3, Item:0);
        }*/


        private void InitializeContextMenus()
        {
            //menu items for response variable column
            cmforResponseVar.MenuItems.Clear();
            cmforResponseVar.MenuItems.Add("Transform");
            cmforResponseVar.MenuItems[0].MenuItems.Add("Log10", new EventHandler(log10T));
            cmforResponseVar.MenuItems[0].MenuItems.Add("Ln", new EventHandler(lnT));
            cmforResponseVar.MenuItems[0].MenuItems.Add("Power", new EventHandler(powerT));
            cmforResponseVar.MenuItems.Add("View Plots", new EventHandler(seePlot));
            cmforResponseVar.MenuItems.Add("UnTransform", new EventHandler(UnTransform));
            cmforResponseVar.MenuItems.Add("Define Transform");
            cmforResponseVar.MenuItems[3].MenuItems.Add("none", new EventHandler(defineTransformForRV));                
            cmforResponseVar.MenuItems[3].MenuItems.Add("Log10", new EventHandler(defineTransformForRV));            
            cmforResponseVar.MenuItems[3].MenuItems.Add("Ln", new EventHandler(defineTransformForRV));            
            cmforResponseVar.MenuItems[3].MenuItems.Add("Power", new EventHandler(defineTransformForRV));

            //menu items for iv columns
            cmforIVs.MenuItems.Clear();
            cmforIVs.MenuItems.Add("Disable Column", new EventHandler(DisableCol));
            cmforIVs.MenuItems.Add("Enable Column", new EventHandler(EnableCol));
            cmforIVs.MenuItems.Add("Set Response Variable", new EventHandler(SetResponse));
            cmforIVs.MenuItems.Add("View Plots", new EventHandler(seePlot));
            cmforIVs.MenuItems.Add("Delete Column", new EventHandler(DeleteCol));
            cmforIVs.MenuItems.Add("Enable All Columns", new EventHandler(EnableAllCols));

            //menu items for rows 
            cmforRows.MenuItems.Clear();
            cmforRows.MenuItems.Add("Disable Row", new EventHandler(DisableRow));
            cmforRows.MenuItems.Add("Enable Row", new EventHandler(EnableRow));
            cmforRows.MenuItems.Add("Enable All Rows", new EventHandler(EnableAllRows));

            SetTransformCheckmarks(Menu:3, Item:0);
        }


        // populate the UI list with some file/data information
        public void showListInfo(string fn, DataTable dt)
        {
            this.dt = dt;
            /*int intNcols = dt.Columns.Count;
            int intNrows = dt.Rows.Count;
            string strDtname = dt.Columns[0].ColumnName.ToString();
            string strDepvarname = dt.Columns[1].ColumnName.ToString();
            intNivs = dt.Columns.Count - 2;*/
            //clear listView
            listView1.Clear();
            listView1.View = View.Details;

            ListViewItem lvi;
            //add column counts
            lvi = new ListViewItem("Column Count");
            lvi.SubItems.Add("");
            //lvi.SubItems.Add(intNcols.ToString());
            listView1.Items.Add(lvi);
            //add row counts
            lvi = new ListViewItem("Row Count");
            lvi.SubItems.Add("");
            //lvi.SubItems.Add(intNrows.ToString());
            listView1.Items.Add(lvi);
            //add datetime
            lvi = new ListViewItem("Date-Time Index");
            lvi.SubItems.Add("");
            //lvi.SubItems.Add(strDtname);
            listView1.Items.Add(lvi);
            //add resp var
            lvi = new ListViewItem("Response Variable");
            lvi.SubItems.Add("");
            //lvi.SubItems.Add(strDepvarname);
            listView1.Items.Add(lvi);
            //add space
            lvi = new ListViewItem("");
            lvi.SubItems.Add("");
            listView1.Items.Add(lvi);
            //add disabled row counts
            lvi = new ListViewItem("Disabled Row Count");
            lvi.SubItems.Add("");
            //lvi.SubItems.Add(intNdisabledrows.ToString());
            listView1.Items.Add(lvi);
            //add disabled column counts
            lvi = new ListViewItem("Disabled Column Count");
            lvi.SubItems.Add("");
            //lvi.SubItems.Add(intNdisabledcols.ToString());
            listView1.Items.Add(lvi);
            //add hidden column counts
            lvi = new ListViewItem("Hidden Column Count");
            lvi.SubItems.Add("");
            //lvi.SubItems.Add(intNhiddencols.ToString());
            listView1.Items.Add(lvi);
            //add indep var counts
            lvi = new ListViewItem("Independent Variable Count");
            lvi.SubItems.Add("");
            //lvi.SubItems.Add(intNivs.ToString());
            listView1.Items.Add(lvi);

            ////magic numbers for widths: -1 set to max characters in subitems, -2 == autosize
            listView1.Columns.Add("File", -1, HorizontalAlignment.Right);
            listView1.Columns.Add(fn, -2, HorizontalAlignment.Left);

            UpdateListView();
        }

        
        // as user manipulates the dataset, track changes and update the UI listview
        public void UpdateListView()
        {
            int ncols = dt.Columns.Count;
            int nrows = dt.Rows.Count;
            int ndisabledcols = 0;
            int ndisabledrows = 0;
            int nhiddencols = 0;
            int nivs = 0;
            string IDcolname = dt.Columns[0].ColumnName;
            string RVcolname = dt.Columns[intResponseVarColIndex].ColumnName;

            foreach (DataColumn col in dt.Columns)
            {
                bool visible=true, enabled=true, predictor=true;
                if (col.ExtendedProperties.ContainsKey(VBCommon.Globals.HIDDEN))
                {
                    if (col.ExtendedProperties[VBCommon.Globals.HIDDEN].ToString() == "True")
                    {
                        nhiddencols++;
                        visible = false;
                    }
                }
                if (col.ExtendedProperties.ContainsKey(VBCommon.Globals.ENABLED))
                {
                    if (col.ExtendedProperties[VBCommon.Globals.ENABLED].ToString() == "False")
                    {
                        ndisabledcols++;
                        enabled = false;
                    }
                }
                if (col.ExtendedProperties.ContainsKey(VBCommon.Globals.DEPENDENTVAR))
                {
                    if (col.ExtendedProperties[VBCommon.Globals.DEPENDENTVAR].ToString() == "True")
                        predictor = false;
                }
                if (col.ExtendedProperties.ContainsKey(VBCommon.Globals.DATETIMESTAMP))
                {
                    if (col.ExtendedProperties[VBCommon.Globals.DATETIMESTAMP].ToString() == "True")
                        predictor = false;
                }
                if (visible && enabled && predictor)
                    nivs++;
            }

            foreach (DataRow row in dt.Rows)
            {
                if (dtRI.GetRowStatus(row.ItemArray[0].ToString()) == false)
                    ndisabledrows++;
            }

            listView1.Items[0].SubItems[1].Text = ncols.ToString();
            listView1.Items[1].SubItems[1].Text = nrows.ToString();
            listView1.Items[2].SubItems[1].Text = IDcolname;
            listView1.Items[3].SubItems[1].Text = RVcolname;
            listView1.Items[4].SubItems[1].Text = "";
            listView1.Items[5].SubItems[1].Text = ndisabledrows.ToString();
            listView1.Items[6].SubItems[1].Text = ndisabledcols.ToString();
            listView1.Items[7].SubItems[1].Text = nhiddencols.ToString();
            listView1.Items[8].SubItems[1].Text = nivs.ToString();
        }
        
        
        // response variable transform log10
        public void log10T(object o, EventArgs e)
        {
            VBCommon.Transforms.Transformer t = new VBCommon.Transforms.Transformer(dt, intSelectedColIndex);
            double[] newvals = t.LOG10;
            string strMessage = t.Message;

            if (strMessage != "")
            {
                MessageBox.Show("Cannot Log-transform variable. " + strMessage, "VB Transform Rule", MessageBoxButtons.OK);
                return;
            }

            string newcolname = "LOG10[" + dt.Columns[intSelectedColIndex].Caption + "]";
            performTOperation(dt, newcolname, intSelectedColIndex, newvals);
            dt.Columns[newcolname].ExtendedProperties[VBCommon.Globals.DEPENDENTVARIBLEDEFINEDTRANSFORM] = VBCommon.Transforms.DependentVariableTransforms.Log10.ToString();

            intResponseVarColIndex = dt.Columns.IndexOf(newcolname);
            strResponseVarColName = dt.Columns[intResponseVarColIndex].Caption;
            depVarTransform = VBCommon.Transforms.DependentVariableTransforms.Log10;
            SetTransformCheckmarks(Menu: 0, Item: 0);

            state = dtState.dirty;
            NotifyContainer();
        }


        // a transform of the response variable has been requested, add it to the table
        public void performTOperation(DataTable dt, string newcolname, int selectedColIndex, double[] newvals)
        {
            switch (addreplace)
            {
                case AddReplace.Add:
                    transformRVValues(dt, newcolname, selectedColIndex, newvals);
                    break;
            }
        }

                
        // set table/grid/listview properties for the user-selected column menu item Transform (the individual 
        //transform event handlers all call this method to add the transformed values to the table as a new column, 
        //hide the untransformed column.  if the previous variable was a transformed variable,
        public void transformRVValues(DataTable dt, string newcolname, int selectedColIndex, double[] newvals)
        {
            //we're only here on transform of dependent variable
            DialogResult dlgr = DialogResult.Yes;
            if (hasTCols())
            {
                dlgr = MessageBox.Show("Datasheet contains independent variable transforms; transforming the dependent variable\n results in removal of created columns; Proceed (Y?N)",
                    "Are you sure you want to continue?", MessageBoxButtons.YesNo);
            }
            if (dlgr == DialogResult.Yes)
            {
                try
                {
                    DataTable dtCopy = dt.Copy();
                    dtCopy.Columns[selectedColIndex].ExtendedProperties[VBCommon.Globals.DEPENDENTVAR] = false;
                    string colname = dtCopy.Columns[selectedColIndex].Caption;

                    if (hasTCols())
                    {
                        dtCopy = filterTcols(dtCopy);
                    }

                    dtCopy = setHiddenIVstoUnhidden(dtCopy);
                    int ordinal = dtCopy.Columns.IndexOf(colname);
                    dtCopy.Columns.Add(newcolname, typeof(double));

                    for (int r = 0; r < dtCopy.Rows.Count; r++)
                        dtCopy.Rows[r][newcolname] = newvals[r];
                    dtCopy.Columns[newcolname].SetOrdinal(ordinal + 1);
                    dtCopy.AcceptChanges();
                    dtCI.AddColumnNameToDict(newcolname);

                    //set properties of new rv
                    strResponseVarColName = newcolname;
                    intResponseVarColIndex = dtCopy.Columns.IndexOf(strResponseVarColName);
                    dtCopy.Columns[newcolname].ExtendedProperties[VBCommon.Globals.DEPENDENTVARIBLETRANSFORM] = false;
                    dtCopy.Columns[newcolname].ExtendedProperties[VBCommon.Globals.DEPENDENTVAR] = true;

                    //set properties of old one
                    dtCopy.Columns[selectedColIndex].ExtendedProperties[VBCommon.Globals.HIDDEN] = true;
                    dtCopy.Columns[selectedColIndex].ExtendedProperties[VBCommon.Globals.ENABLED] = false;

                    this.dt =  dt = dtCopy;
                    maintainGrid(dgv, dt, selectedColIndex, strResponseVarColName);
                    UpdateListView();

                    //Disable "Define Transform" menu
                    cmforResponseVar.MenuItems[3].Enabled = false;

                    state = dtState.dirty;
                    NotifyContainer();
                }
                catch (DuplicateNameException e)
                {
                    MessageBox.Show("Table already contains column: " + newcolname, "Cannot Add Column to Table", MessageBoxButtons.OK);
                }
            }
        }


        // determine if the table has transformed IV columns
        public bool hasTCols()
        {
            //determine is the table has transformed data
            foreach (DataColumn c in dt.Columns)
            {
                bool colisT = c.ExtendedProperties.ContainsKey(VBCommon.Globals.TRANSFORM);
                if (colisT == true) return true;
            }
            return false;
        }


        // determine if the table has manipulated (interacted) data columns
        //true if table has manipulated columns, false otherwise</returns>
        public bool hasOCols()
        {
            foreach (DataColumn c in dt.Columns)
            {
                bool colisI = c.ExtendedProperties.ContainsKey(VBCommon.Globals.OPERATION);
                if (colisI == true) return true;
            }
            return false;
        }


        // determine if the table has decomposed data columns
        //true if table has decomposed columns, false otherwise</returns>
        public bool hasDCols()
        {
            foreach (DataColumn c in dt.Columns)
            {
                bool colisD = c.ExtendedProperties.ContainsKey(VBCommon.Globals.DECOMPOSITION);
                if (colisD == true) return true;
            }
            return false;
        }


        // determine if the table has either transformed IV columns OR manipulated columns
        public bool hasOorTCols()
        {
            foreach (DataColumn c in dt.Columns)
            {
                bool colisT = c.ExtendedProperties.ContainsKey(VBCommon.Globals.TRANSFORM);
                bool colisI = c.ExtendedProperties.ContainsKey(VBCommon.Globals.OPERATION);
                if (colisI == true || colisT == true) return true;
            }
            return false;
        }


        // determine if the table has either transformed IV columns OR manipulated columns
        public bool AllColsAreMainEffects()
        {
            foreach (DataColumn c in dt.Columns)
            {
                bool colisT = c.ExtendedProperties.ContainsKey(VBCommon.Globals.TRANSFORM);
                bool colisI = c.ExtendedProperties.ContainsKey(VBCommon.Globals.OPERATION);
                bool colisD = c.ExtendedProperties.ContainsKey(VBCommon.Globals.DECOMPOSITION);
                if (colisI == true || colisT == true || colisD == true) return false;
            }
            return true;
        }
      

        // response variable transform natural log
        public void lnT(object o, EventArgs e)
        {
            VBCommon.Transforms.Transformer t = new VBCommon.Transforms.Transformer(dt, intSelectedColIndex);
            double[] newvals = t.LOGE;
            string strMessage = t.Message;

            if (strMessage != "")
            {
                MessageBox.Show("Cannot Log-transform variable. " + strMessage, "VB Transform Rule", MessageBoxButtons.OK);
                return;
            }

            string newcolname = "LN[" + dt.Columns[intSelectedColIndex].Caption + "]";
            performTOperation(dt, newcolname, intSelectedColIndex, newvals);
            dt.Columns[newcolname].ExtendedProperties[VBCommon.Globals.DEPENDENTVARIBLEDEFINEDTRANSFORM] = VBCommon.Transforms.DependentVariableTransforms.Ln.ToString();
            depVarTransform = VBCommon.Transforms.DependentVariableTransforms.Ln;
            SetTransformCheckmarks(Menu: 0, Item: 1);

            state = dtState.dirty;
            NotifyContainer();
        }


        // response variable transform power(exp)
        public void powerT(object o, EventArgs e)
        {
            frmPowerExponent frmExp = new frmPowerExponent(dt, intSelectedColIndex);
            DialogResult dlgr = frmExp.ShowDialog();
            if (dlgr != DialogResult.Cancel)
            {
                double[] newvals = new double[dt.Rows.Count];
                newvals = frmExp.TransformedValues;
                if (frmExp.TransformMessage != "")
                {
                    MessageBox.Show("Cannot Power transform variable. " + frmExp.TransformMessage, "VB Transform Rule", MessageBoxButtons.OK);
                    return;
                }
                string sexp = frmExp.Exponent.ToString("n2");
                string newcolname = "POWER" + "[" + sexp + "," + dt.Columns[intSelectedColIndex].Caption + "]";
                performTOperation(dt, newcolname, intSelectedColIndex, newvals);
                dt.Columns[newcolname].ExtendedProperties[VBCommon.Globals.DEPENDENTVARIBLEDEFINEDTRANSFORM] = VBCommon.Transforms.DependentVariableTransforms.Power.ToString() + "," + sexp;
                depVarTransform = VBCommon.Transforms.DependentVariableTransforms.Power;
                dblPowerTransformExp = Convert.ToDouble(sexp);
                SetTransformCheckmarks(Menu: 0, Item: 2);

                state = dtState.dirty;
                NotifyContainer();
            }
        }


        // show the plots for the seleected column
        public void seePlot(object o, EventArgs e)
        {
            frmPlot frmplot = new frmPlot(intResponseVarColIndex, intSelectedColIndex, filterDataTableRows(dt));

            frmplot.Show();

            //set up a delegate for each plot form and listen for disable/enable point events
            frmplot.pointDisabled += new frmPlot.PointDisableEventHandler(frmplot_pointDisabled);
            frmplot.pointEnabled += new frmPlot.PointEnableEventHandler(frmplot_pointEnabled);
        }


        // strips disabled records from the table passed to modeling
        public DataTable filterDataTableRows(DataTable dt)
        {
            //filter out disabled rows
            DataTable dtCopy = dt.Copy();
            Dictionary<string, bool> rstatus = dtRI.DTRowInfo;
            for (int i = 0; i < dtCopy.Rows.Count; i++)
            {
                if (!rstatus[dtCopy.Rows[i][0].ToString()])
                    dtCopy.Rows[i].Delete();
            }
            dtCopy.AcceptChanges();
            return dtCopy;
        }


        // disable a row in the grid for a point selected by user on the plots
        public void frmplot_pointDisabled(string tag)
        {
            bool found = false;
            string d = tag;

            //find it in the datatable
            foreach (DataRow r in dt.Rows)
            {
                if (r[0].ToString() != d) continue;
                //otherwise we found it
                intSelectedRowIndex = dt.Rows.IndexOf(r);
                DisableRow(null, null);
                found = true;

                break;
            }
            if (!found)
            {
                MessageBox.Show("Unable to locate row with date/time " + tag,
                    "Disable Record Error.",
                    MessageBoxButtons.OK);
            }
        }


        // enable a row in the grid for a point selected by user on the plots
        public void frmplot_pointEnabled(string tag)
        {
            bool found = false;
            string d = tag;

            //find it in the datatable
            foreach (DataRow r in dt.Rows)
            {
                if (r[0].ToString() != d) continue;
                //otherwise we found it
                intSelectedRowIndex = dt.Rows.IndexOf(r);
                EnableRow(null, null);
                found = true;
                break;
            }

            if (!found)
            {
                MessageBox.Show("Unable to locate row with date/time " + tag,
                    "Disable Record Error.",
                    MessageBoxButtons.OK);
            }
        }


        // set grid/listview properties for the user-selected row menu item Disable
        public void DisableRow(object sender, EventArgs e)
        {
            List<int> rowsToDisable = new List<int>();

            //Figure out which rows we're going to disable
            if (dgv.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow dgvr in dgv.SelectedRows)
                {
                    rowsToDisable.Add(dgvr.Index);
                }
            }
            else
            {
                rowsToDisable.Add(intSelectedRowIndex);
            }
                        
            foreach (int intRowToDisable in rowsToDisable)
            {
                //update the dtRowInformation dictionary of disabled/enabled rows
                dtRI.SetRowStatus(dt.Rows[intRowToDisable][0].ToString(), false);
                for (int c = 0; c < dgv.Columns.Count; c++)
                {
                    dgv[c, intRowToDisable].Style.ForeColor = Color.Red;
                }

                //check to see if this index is already in extendedProps (was set to true before)
                //set extendedproperties so dtRowInformation can maintain dictDTRI
                if (dt.ExtendedProperties.ContainsKey(intRowToDisable.ToString()))
                    dt.ExtendedProperties.Remove(intRowToDisable.ToString());
                dt.ExtendedProperties.Add(intRowToDisable.ToString(), false);
            }

            //UpdateListView(VBCommon.Globals.listvals.NDISABLEDROWS, ++intNdisabledrows);
            UpdateListView();
            state = dtState.dirty;
            NotifyContainer();
        }

       
        // set grid/listview properties for the user-selected row menu item Enable
        public void EnableRow(object sender, EventArgs e)
        {
            List<int> rowsToEnable = new List<int>();

            //Figure out which rows we're going to disable
            if (dgv.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow dgvr in dgv.SelectedRows)
                {
                    rowsToEnable.Add(dgvr.Index);
                }
            }
            else
            {
                rowsToEnable.Add(intSelectedRowIndex);
            }

            foreach (int intRowToEnable in rowsToEnable)
            {
                dtRI.SetRowStatus(dt.Rows[intRowToEnable][0].ToString(), true);

                for (int c = 0; c < dgv.Columns.Count; c++)
                {
                    if (!dtCI.GetColStatus(dgv.Columns[c].Name.ToString())) continue;
                    dgv[c, intRowToEnable].Style.ForeColor = Color.Black;

                }

                //check to see if this index is already in extendedProps (was set to false before)
                //then set extendedproperties so dtRowInformation can maintain dictDTRI
                if (dt.ExtendedProperties.ContainsKey(intRowToEnable.ToString()))
                    dt.ExtendedProperties.Remove(intRowToEnable.ToString());
                dt.ExtendedProperties.Add(intRowToEnable.ToString(), true);
            }

            //UpdateListView(VBCommon.Globals.listvals.NDISABLEDROWS, --intNdisabledrows);
            UpdateListView();
            state = dtState.dirty;
            NotifyContainer();
        }


        public void UnTransform(object o, EventArgs e)
        {
            //can only untransform response variable (only one transformable)
            //unhide the original response variable
            //remove the transformed column
            DialogResult dlgr = DialogResult.Yes;
            if (hasTCols())
            {
                dlgr = MessageBox.Show("Datasheet contains independent variable transforms; un-transforming the dependent variable\n results in removal of created columns; Proceed (Y?N)",
                    "Are you sure you want to continue?", MessageBoxButtons.YesNo);
            }
            if (dlgr == DialogResult.Yes)
            {
                DataTable dtCopy = dt;
                dtCopy.Columns.Remove(dt.Columns[intSelectedColIndex].Caption);
                dtCopy.AcceptChanges();
                if (hasTCols())
                {
                    dtCopy = filterTcols(dtCopy);
                }

                foreach (DataColumn c in dtCopy.Columns)
                {
                    if (!testValueAttribute(c, VBCommon.Globals.HIDDEN)) continue;
                    {
                        c.ExtendedProperties[VBCommon.Globals.DEPENDENTVAR] = true;
                        c.ExtendedProperties[VBCommon.Globals.HIDDEN] = false;
                        c.ExtendedProperties[VBCommon.Globals.ENABLED] = true;
                        intSelectedColIndex = dtCopy.Columns.IndexOf(c);
                        strResponseVarColName = dtCopy.Columns[intSelectedColIndex].Caption;
                        intResponseVarColIndex = intSelectedColIndex;

                        break;
                    }
                }

                dt = dtCopy;
                maintainGrid(dgv, dt, intSelectedColIndex, strResponseVarColName);
                UpdateListView();

                //Re-enable "Define Transform" menu and clear all checkmarks
                SetTransformCheckmarks(Menu: -1, Item: -1);
                cmforResponseVar.MenuItems[3].Enabled = true;
                state = dtState.dirty;
                NotifyContainer();
            }
        }

        public void defineTransformForRV(object o, EventArgs e)
        {
            //menu response from right click, determine which transform was selected
            MenuItem mi = (MenuItem)o;
            string transform = mi.Text;
            if (transform == VBCommon.Transforms.DependentVariableTransforms.Power.ToString())
            {
                frmPowerExponent frmExp = new frmPowerExponent(dt, intSelectedColIndex);
                DialogResult dlgr = frmExp.ShowDialog();
                if (dlgr != DialogResult.Cancel)
                {
                    string sexp = frmExp.Exponent.ToString("n2");
                    transform += "," + sexp;
                    depVarTransform = VBCommon.Transforms.DependentVariableTransforms.Power;
                    dblPowerTransformExp = Convert.ToDouble(sexp);
                    dt.Columns[intSelectedColIndex].ExtendedProperties[VBCommon.Globals.DEPENDENTVARIBLEDEFINEDTRANSFORM] = transform;
                    SetTransformCheckmarks(Menu: 3, Item: 3);
                    state = dtState.dirty;
                    NotifyContainer();
                }
            }
            else
            {
                if (String.Compare(transform, "Log10", true) == 0)
                {
                    depVarTransform = VBCommon.Transforms.DependentVariableTransforms.Log10;
                    SetTransformCheckmarks(Menu: 3, Item: 1);
                }
                else if (String.Compare(transform, "Ln", true) == 0)
                {
                    depVarTransform = VBCommon.Transforms.DependentVariableTransforms.Ln;
                    SetTransformCheckmarks(Menu: 3, Item: 2);
                }
                else if (String.Compare(transform, "none", true) == 0)
                {
                    depVarTransform = VBCommon.Transforms.DependentVariableTransforms.none;
                    SetTransformCheckmarks(Menu: 3, Item: 0);
                }

                dt.Columns[intSelectedColIndex].ExtendedProperties[VBCommon.Globals.DEPENDENTVARIBLEDEFINEDTRANSFORM] = transform;
                state = dtState.dirty;
                NotifyContainer();
            }

            if (!(String.Compare(transform, "none", true) == 0))
            {
                //Disable "Perform Transform" menu
                cmforResponseVar.MenuItems[0].Enabled = false;
            }
            else
            {
                //Enable "Perform Transform" menu
                cmforResponseVar.MenuItems[0].Enabled = true;
            }
        }


        private void SetTransformCheckmarks(int Menu, int Item)
        {
            int i;
            intCheckedMenu = Menu;
            intCheckedItem = Item;

            //First handle the applied transforms' menu:
            for (i = 0; i < 3; i++)
            {
                if (Menu != 0)
                    cmforResponseVar.MenuItems[0].MenuItems[i].Checked = false;
                else
                    if (Item == i)
                        cmforResponseVar.MenuItems[0].MenuItems[i].Checked = true;
                    else
                        cmforResponseVar.MenuItems[0].MenuItems[i].Checked = false;
            }

            //Now handle the defined transforms' menu:
            for (i = 0; i < 4; i++)
            {
                if (Menu != 3)
                    cmforResponseVar.MenuItems[3].MenuItems[i].Checked = false;
                else
                    if (Item == i)
                        cmforResponseVar.MenuItems[3].MenuItems[i].Checked = true;
                    else
                        cmforResponseVar.MenuItems[3].MenuItems[i].Checked = false;
            }
        }


        // set table/grid/listview properties for the user-selected column menu item Disable
        public void DisableCol(object sender, EventArgs e)
        {
            string cn = dt.Columns[intSelectedColIndex].Caption;
            dtCI.SetColStatus(dt.Columns[intSelectedColIndex].ColumnName.ToString(), false);
            dt.Columns[intSelectedColIndex].ExtendedProperties[VBCommon.Globals.ENABLED] = false;

            UpdateListView();
            disableGridCol(dgv, intSelectedColIndex);

            state = dtState.dirty;
            NotifyContainer();
        }


        // set table/grid/listview properties for the user-selected column menu item Enable
        public void EnableCol(object sender, EventArgs e)
        {
            string cn = dt.Columns[intSelectedColIndex].Caption;
            dtCI.SetColStatus(dt.Columns[intSelectedColIndex].ColumnName.ToString(), true);
            dt.Columns[intSelectedColIndex].ExtendedProperties[VBCommon.Globals.ENABLED] = true;

            UpdateListView();
            enableGridCol(dgv, intSelectedColIndex, dt);

            state = dtState.dirty;
            NotifyContainer();
        }


        // set table/grid/listview properties for the user-selected column menu item SetResponse.
        // pick this variable as the response variable, and if the previous response variable was a 
        // transform, delete it from the table and unhide its original column.  Further, if
        // independent variable transforms are present in the table remove them from the table (with 
        // user permission)
        public void SetResponse(object sender, EventArgs e)
        {
            DialogResult dlgr = DialogResult.Yes;
            if (!AllColsAreMainEffects())
            {
                dlgr = MessageBox.Show("Changing response variable results in removal of created columns; Proceed (Y?N)",
                "Are you sure you want to continue?", MessageBoxButtons.YesNo);
            }

            if (dlgr == DialogResult.Yes)
            {
                //save this 'cause we're maybe screwing with indicies by removing columns
                strSelectedColName = dt.Columns[intSelectedColIndex].Caption;

                //maintain attributes
                dt.Columns[intSelectedColIndex].ExtendedProperties[VBCommon.Globals.DEPENDENTVAR] = true;
                dt.Columns[intSelectedColIndex].ExtendedProperties[VBCommon.Globals.DEPENDENTVARIBLEDEFINEDTRANSFORM] = VBCommon.Transforms.DependentVariableTransforms.none.ToString();
                dt.Columns[intResponseVarColIndex].ExtendedProperties[VBCommon.Globals.DEPENDENTVAR] = false;

                //filter transformed cols
                if (!AllColsAreMainEffects()) dt = FilterCols(dt);

                if (testValueAttribute(dt.Columns[strResponseVarColName], VBCommon.Globals.DEPENDENTVARIBLETRANSFORM))
                {
                    dt.Columns.Remove(dt.Columns[strResponseVarColName].Caption);
                    dt.AcceptChanges();
                    UnhideHiddenCols(dgv, dt);
                }

                intResponseVarColIndex = dt.Columns.IndexOf(strSelectedColName);
                strResponseVarColName = dt.Columns[intResponseVarColIndex].Caption;
                strResponseVarColNameAsImported = dt.Columns[intResponseVarColIndex].Caption;
                SetTransformCheckmarks(Menu: -1, Item: -1);
                cmforResponseVar.MenuItems[0].Enabled = true;
                cmforResponseVar.MenuItems[3].Enabled = true;
                maintainGrid(dgv, dt, intSelectedColIndex, strResponseVarColName);
                UpdateListView();

                state = dtState.dirty;
                NotifyContainer();
            }
        }

        
        // delete the select column
        public void DeleteCol(object o, EventArgs e)
        {
            if (dt.Columns[intSelectedColIndex].ExtendedProperties.ContainsKey(VBCommon.Globals.TRANSFORM) ||
                dt.Columns[intSelectedColIndex].ExtendedProperties.ContainsKey(VBCommon.Globals.OPERATION) ||
                dt.Columns[intSelectedColIndex].ExtendedProperties.ContainsKey(VBCommon.Globals.DECOMPOSITION))
            {
                dtCI.RemoveColumnFromDict(dt.Columns[intSelectedColIndex].Caption);

                int gridpos = dgv.FirstDisplayedScrollingColumnIndex;
                dt.Columns.Remove(dt.Columns[intSelectedColIndex].Caption);

                dt.AcceptChanges();

                maintainGrid(dgv, dt, intSelectedColIndex, strResponseVarColName);

                dgv.DataSource = dt;
                dgv.FirstDisplayedScrollingColumnIndex = gridpos;

                UpdateListView();

                state = dtState.dirty;
                NotifyContainer();
            }
        }


        public void EnableAllCols(object sender, EventArgs e)
        {
            for (int c = 1; c < dt.Columns.Count; c++)
            {
                dtCI.SetColStatus(dt.Columns[c].Caption, true);
                dt.Columns[c].ExtendedProperties[VBCommon.Globals.ENABLED] = true;
                enableGridCol(dgv, c, dt);
            }

            UpdateListView();

            state = dtState.dirty;
            NotifyContainer();
        }


        public void EnableAllRows(object sender, EventArgs e)
        {
            for (int r = 0; r < dt.Rows.Count; r++)
            {
                dtRI.SetRowStatus(dt.Rows[r][0].ToString(), true);

                //update extendedProps for those rows
                if (dt.ExtendedProperties.ContainsKey(r.ToString()))
                    dt.ExtendedProperties[r.ToString()] = true;
            }

            for (int r = 0; r < dgv.Rows.Count; r++)
            {
                for (int c = 0; c < dgv.Columns.Count; c++)
                {
                    if (!dtCI.GetColStatus(dgv.Columns[c].Name.ToString())) continue;
                    dgv[c, r].Style.ForeColor = Color.Black;
                }
            }

            UpdateListView();

            state = dtState.dirty;
            NotifyContainer();
        }


        public void dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {            
            dgv.EndEdit();

            double dblValue = Convert.ToDouble(dgv[e.ColumnIndex, e.RowIndex].Value);
            dt.Rows[e.RowIndex][e.ColumnIndex] = dblValue;
            dt.AcceptChanges();
            state = dtState.dirty;
            NotifyContainer();
            maintainGrid(dgv, dt, SelectedColIndex, ResponseVarColName); //ensure disabled rows/cols stay red
        }


        public void dgv_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            double result;
            string scellval = dgv[e.ColumnIndex, e.RowIndex].Value.ToString();
            if (double.TryParse(scellval, out result))
                if (result == 0.0d)
                {
                    MessageBox.Show("Grid cell values cannot be blank or non-numeric", "Cell Error", MessageBoxButtons.OK);
                    e.Cancel = true;
                    dgv[e.ColumnIndex, e.RowIndex].Selected = true;
                }
                else
                {
                    e.Cancel = false;
                }
        }


        public void dgv_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.C) && (e.Modifiers == Keys.Control))
            {
                Clipboard.SetDataObject(dgv.GetClipboardContent());
            }
        }

        // user clicked on the UI grid - decide what to do if anything
        public void dgv_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            showContextMenus((DataGridView)sender, e, dt);
        }

        
        // user click captured - decide what menu items are appropriate and show them
        public void showContextMenus(DataGridView dgv, MouseEventArgs me, DataTable dt)
        {            
            dtCI = new dtColumnInformation(dt);

            DataGridView.HitTestInfo ht = dgv.HitTest(me.X, me.Y);
            int colndx = ht.ColumnIndex;
            int rowndx = ht.RowIndex;

            if (rowndx > 0 && colndx > 0) return; //cell hit, go away

            if (rowndx < 0 && colndx >= 0)
            {
                //col header hit, show proper menu
                intSelectedColIndex = colndx;

                //do nothing if col 0 selected
                if (colndx >= 1)
                {
                    string colname = dt.Columns[colndx].Caption;
                    if (colname == strResponseVarColName)
                    {
                        if (testValueAttribute(dt.Columns[intResponseVarColIndex], VBCommon.Globals.DEPENDENTVAR))
                        {
                            cmforResponseVar.MenuItems[0].Enabled = true; //we can transform a response variable
                        }
                        else
                        {
                            cmforResponseVar.MenuItems[0].Enabled = false; //but we cannot transform a transformed response
                        }

                        if (testValueAttribute(dt.Columns[intResponseVarColIndex], VBCommon.Globals.DEPENDENTVARIBLETRANSFORM))
                        {
                            cmforResponseVar.MenuItems[2].Enabled = true; //we can untransform the transformed response variable
                        }
                        else
                        {
                            cmforResponseVar.MenuItems[2].Enabled = false; //but cannot untransform a response variable
                        }

                        cmforResponseVar.Show(dgv, new Point(me.X, me.Y));
                    }
                    else
                    {
                        //show context menu for ivs
                        if (dtCI.GetColStatus(dt.Columns[intSelectedColIndex].ColumnName.ToString()))
                        {
                            //here if col enabled
                            cmforIVs.MenuItems[0].Enabled = true;
                            cmforIVs.MenuItems[1].Enabled = false; //cannot enable enabled col
                            cmforIVs.MenuItems[2].Enabled = true;

                            //response variable must be a ME, T(RV) or I(IV) not created by general transform
                            //if they do this then we're to remove all general operations performed,
                            if (canSetRV()) cmforIVs.MenuItems[2].Enabled = true;
                            else cmforIVs.MenuItems[2].Enabled = false;

                            if (dt.Columns[intSelectedColIndex].ExtendedProperties.ContainsKey(VBCommon.Globals.MAINEFFECT))
                                cmforIVs.MenuItems[4].Enabled = false;  //cannot remove maineffect column
                            else cmforIVs.MenuItems[4].Enabled = true;
                        }
                        else
                        {
                            //here if col disabled
                            cmforIVs.MenuItems[0].Enabled = false; //cannot disable disabled col
                            cmforIVs.MenuItems[1].Enabled = true;
                            cmforIVs.MenuItems[2].Enabled = false; //cannot disabled the response variable
                        }
                        cmforIVs.Show(dgv, new Point(me.X, me.Y));
                    }
                }
            }
            else if (rowndx >= 0 && colndx < 0)
            {
                dtRowInformation dtRI = new dtRowInformation(dt);
                //row header hit, show menu
                intSelectedRowIndex = rowndx;
                if (dtRI.GetRowStatus(dt.Rows[intSelectedRowIndex][0].ToString()))
                {
                    //here if row is enabled
                    cmforRows.MenuItems[0].Enabled = true;
                    cmforRows.MenuItems[1].Enabled = false; //cannot enable enabled row
                }
                else
                {
                    //here if row is disabled
                    cmforRows.MenuItems[0].Enabled = false; //cannot disable disabled row
                    cmforRows.MenuItems[1].Enabled = true;
                }
                cmforRows.Show(dgv, new Point(me.X, me.Y));
            }
        }

        
        // determine if we can set the selected column as the response variable
        /// (cannot set a transformed independent variable as the response variable - or categorical variables....)
        public bool canSetRV()
        {
            bool colisT = dt.Columns[intSelectedColIndex].ExtendedProperties.ContainsKey(VBCommon.Globals.TRANSFORM);
            bool colisI = dt.Columns[intSelectedColIndex].ExtendedProperties.ContainsKey(VBCommon.Globals.OPERATION);
            bool colisCat = testValueAttribute(dt.Columns[intSelectedColIndex], VBCommon.Globals.CATEGORICAL);
            if (colisT == true || colisCat == true) return false;
            else return true;
        }


        //event handler for packing state to save project
        public IDictionary<string, object> PackState()
        {
            IDictionary<string, object> dictPackedState = new Dictionary<string, object>();

            if (this.DT != null)
            {
                //pack up mainEffect columns for Prediction
                dictPackedState.Add("CurrentColIndex", this.SelectedColIndex);
                dictPackedState.Add("DepVarColName", this.ResponseVarColName);
                dictPackedState.Add("DepVarColNameAsImported", this.ResponseVarColNameAsImported);
                dictPackedState.Add("DTColInfo", this.DTCI.DTColInfo);
                dictPackedState.Add("DTRowInfo", this.DTRI.DTRowInfo);
                dictPackedState.Add("DepVarTransform", this.DependentVariableTransform);
                dictPackedState.Add("DepVarExponent", this.PowerTransformExponent);
                dictPackedState.Add("CheckedTransformMenu", this.intCheckedMenu);
                dictPackedState.Add("CheckedTransformItem", this.intCheckedItem);
                
                //pack up listInfo for model datasheet
                dictPackedState.Add("ColCount", this.DT.Columns.Count);
                dictPackedState.Add("RowCount", this.DT.Rows.Count);
                dictPackedState.Add("DateIndex", this.DT.Columns[0].ColumnName.ToString());
                dictPackedState.Add("fileName", this.FileName);

                dictPackedState.Add("orientation", dblOrientation);

                //Save Datasheet info as xml string for serialization
                StringWriter sw = null;
                this.DT.TableName = "DataSheetData";
                sw = new StringWriter();
                this.DT.WriteXml(sw, XmlWriteMode.WriteSchema, false);
                string strXmlDataTable = sw.ToString();
                sw.Close();
                sw = null;
                dictPackedState.Add("XmlDataTable", strXmlDataTable);

                if (this.State == VBCommon.Controls.DatasheetControl.dtState.clean)
                {
                    bool boolClean = true;
                    dictPackedState.Add("Clean", boolClean);
                }
                else
                {
                    bool boolClean = false;
                    dictPackedState.Add("Clean", boolClean);
                }

                //model expects this change to the dt first ... I DON'T SEE THIS USED ANYWHERE ELSE
                DataTable tblFiltered = this.DT;
                //tblFiltered.Columns[this.ResponseVarColName].SetOrdinal(1);

                //filter diabled rows and columns
                tblFiltered = this.filterDataTableRows(tblFiltered);
                Metadata.Utilities.TableUtils tableutils = new Metadata.Utilities.TableUtils(tblFiltered);
                tblFiltered = tableutils.filterRVHcols(tblFiltered);
                dictPackedState.Add("DataSheetDatatable", tblFiltered);
            }

            return dictPackedState;
        }


        //unpack event handler. unpacks packed state in dictionary to repopulate datasheet
        public void UnpackState(IDictionary<string, object> dictPackedState)
        {
            //unpack datatable
            //unpack xmlDT and repopulate this.DT
            if (dictPackedState.ContainsKey("XmlDataTable"))
            {
                string xmlDT = (string)dictPackedState["XmlDataTable"];
                StringReader sr = new StringReader(xmlDT);
                DataSet ds = new DataSet();
                ds.ReadXml(sr);
                sr.Close();
                this.DT = ds.Tables[0];

                this.DT.TableName = "DataSheetData";
                this.dgv.DataSource = null;
                this.dgv.DataSource = this.DT;


                //get row information
                this.DTRI = new VBCommon.Metadata.dtRowInformation(this.DT);

                //json deserialize the dictionary first
                object jsonRowHolder = (object)dictPackedState["DTRowInfo"];
                if (jsonRowHolder.GetType().ToString() == "Newtonsoft.Json.Linq.JObject")
                {
                    string strJson = jsonRowHolder.ToString();
                    Dictionary<string, bool> dtriRowInfo = new Dictionary<string, bool>();
                    dtriRowInfo = JsonConvert.DeserializeObject<Dictionary<string, bool>>(strJson);
                    this.DTRI.DTRowInfo = dtriRowInfo;
                }
                else
                {
                    this.DTRI.DTRowInfo = (Dictionary<string, bool>)dictPackedState["DTRowInfo"];
                }

                //get column information
                this.DTCI = new VBCommon.Metadata.dtColumnInformation(this.DT);

                //json deserialize the dictionary first
                object jsonColHolder = (object)dictPackedState["DTColInfo"];
                if (jsonColHolder.GetType().ToString() == "Newtonsoft.Json.Linq.JObject")
                {
                    string strJson = jsonColHolder.ToString();
                    Dictionary<string, bool> dtriColInfo = new Dictionary<string, bool>();
                    dtriColInfo = JsonConvert.DeserializeObject<Dictionary<string, bool>>(strJson);
                    this.DTCI.DTColInfo = dtriColInfo;
                }
                else
                {
                    this.DTCI.DTColInfo = (Dictionary<string, bool>)dictPackedState["DTColInfo"];
                }

                //need to convert if its unpacked from saved project
                if (dictPackedState["CurrentColIndex"].GetType().ToString() == "System.Int64")
                    this.SelectedColIndex = Convert.ToInt16((Int64)dictPackedState["CurrentColIndex"]);
                else
                    this.SelectedColIndex = (int)dictPackedState["CurrentColIndex"];

                //need to change from 1 to the actual type
                string depVarTran = Convert.ToString(dictPackedState["DepVarTransform"]);
                this.DependentVariableTransform = (VBCommon.Transforms.DependentVariableTransforms)Enum.Parse(typeof(VBCommon.Transforms.DependentVariableTransforms), depVarTran);
                this.ResponseVarColName = (string)dictPackedState["DepVarColName"];
                this.ResponseVarColIndex = this.DT.Columns.IndexOf(this.ResponseVarColName);
                this.ResponseVarColNameAsImported = (string)dictPackedState["DepVarColNameAsImported"];

                this.intCheckedMenu = Convert.ToInt32(dictPackedState["CheckedTransformMenu"]);
                this.intCheckedItem = Convert.ToInt32(dictPackedState["CheckedTransformItem"]);
                SetTransformCheckmarks(Menu: intCheckedMenu, Item: intCheckedItem);
                if (intCheckedMenu == 0) { cmforResponseVar.MenuItems[3].Enabled = false; }
                else if (intCheckedMenu == 3 && intCheckedItem > 0) { cmforResponseVar.MenuItems[0].Enabled = false; }

                maintainGrid(this.dgv, this.DT, this.SelectedColIndex, this.ResponseVarColName);
                this.FileName = (string)dictPackedState["fileName"];
                this.showListInfo(this.FileName, this.DT);

                if (dictPackedState.ContainsKey("orientation"))
                    this.dblOrientation = (double)dictPackedState["orientation"];

                if ((bool)dictPackedState["Clean"])
                {
                    this.State = VBCommon.Controls.DatasheetControl.dtState.clean;
                }
                else
                {
                    this.State = VBCommon.Controls.DatasheetControl.dtState.dirty;
                }
                //seems like a logical place to put this - need to notify the mlr variableselection control that we've got available variables for the list
                NotifyContainer();
            }
        }


        public void registerNewCols(DataTable dt)
        {
            if (DTCI==null)
                DTCI = new dtColumnInformation(dt);

            foreach (DataColumn c in dt.Columns)
            {
                if (!DTCI.GetColStatus(c.ColumnName))
                {
                    DTCI.AddColumnNameToDict(c.ColumnName);
                }

            }
        }


        public bool testValueAttribute(DataColumn dc, string attr)
        {
            if (dc.ExtendedProperties.ContainsKey(attr))
            {
                if (dc.ExtendedProperties[attr].ToString() == "True")
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


        public DataTable filterDisabledCols(DataTable dt)
        {
            //filter out disabled columns
            DataTable dtCopy = dt.Copy();

            dtColumnInformation dtCI = new dtColumnInformation(dt);
            foreach (KeyValuePair<string, bool> kv in dtCI.DTColInfo)
            {
                if (kv.Value) continue;
                if (dtCopy.Columns.Contains(kv.Key))
                    dtCopy.Columns.Remove(kv.Key);
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


        public DataTable FilterCols(DataTable dt)
        {
            //filter transformed columns
            DataTable dtCopy = dt.Copy();

            for (int c = 0; c < dt.Columns.Count; c++)
            {
                bool transformed = dt.Columns[c].ExtendedProperties.ContainsKey(VBCommon.Globals.TRANSFORM);
                bool decomposition = dt.Columns[c].ExtendedProperties.ContainsKey(VBCommon.Globals.DECOMPOSITION);
                bool interaction = dt.Columns[c].ExtendedProperties.ContainsKey(VBCommon.Globals.OPERATION);

                if (transformed || decomposition || interaction)
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

                if (dc.ExtendedProperties[VBCommon.Globals.ENABLED].ToString() != "True")
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
                            dc.ExtendedProperties[VBCommon.Globals.HIDDEN] = "False";
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


        public void maintainGrid(DataGridView dgv, DataTable dt, int selectedColIndex, string responseVarColName)
        {
            //reset the grid
            dgv.DataSource = null;
            dgv.DataSource = dt;
            dtColumnInformation _dtCI = new dtColumnInformation(dt);

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
                        _dtCI.SetColStatus(c.ColumnName.ToString(), false);   //make sure col status is updated
                    }
                    else
                    {
                        for (int r = 0; r < dgv.Rows.Count; r++)
                            dgv[selectedColIndex, r].Style.ForeColor = Color.Black;
                        _dtCI.SetColStatus(c.ColumnName.ToString(), true);   //make sure col status is updated
                    }
                }
                else 
                {
                    if (!c.ExtendedProperties.ContainsKey(VBCommon.Globals.DATETIMESTAMP))
                    {
                        //On the first pass through (after import) enable all columns.
                        setAttributeValue(c, VBCommon.Globals.ENABLED.ToString(), true);
                    }
                }
            }

            //reset the UI clues for the response variable
            dgv.Columns[responseVarColName].DefaultCellStyle.BackColor = Color.LightBlue;

            //reset disable rows
            if (this.DTRI == null)
            {
                dtRI = new dtRowInformation(dt);
            }

            for (int r = 0; r < dt.Rows.Count; r++)
            {
                bool enabled = dtRI.GetRowStatus(dt.Rows[r][0].ToString());
                if (!enabled)
                {
                    for (int c = 0; c < dgv.Columns.Count; c++)
                    {
                        dgv[c, r].Style.ForeColor = Color.Red;
                    }
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
                bool isNum = Information.IsNumeric(testcellval); 

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
                    if ((bool)c.ExtendedProperties[VBCommon.Globals.ENABLED] != true)
                    {
                        for (int r = 0; r < dgv.Rows.Count; r++)
                            dgv[selectedColIndex, r].Style.ForeColor = Color.Red;
                    }
                }
            }

            //reset disable rows
            for (int r = 0; r < dt.Rows.Count; r++)
            {
                bool enabled = dtRI.GetRowStatus(dt.Rows[r][0].ToString());
                if (!enabled)
                {
                    for (int c = 0; c < dgv.Columns.Count; c++)
                        dgv[c, r].Style.ForeColor = Color.Red;
                }
            }
        }


        public void UnhideHiddenCols(DataGridView dgv, DataTable dt)
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
        /// invoke the independent variable transforms tool
        /// creates columns of data transforms
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnTransform_Click(object sender, EventArgs e)
        {
            if (dt != null)
            {
                //filter disabled cols
                DataTable tblFiltered = filterDisabledCols(dt);

                //filter transformed cols
                tblFiltered = filterTcols(tblFiltered);

                //filter hidden (untransformed) dependent variable
                tblFiltered = filterRVHcols(tblFiltered);

                //filter cat vars
                tblFiltered = filterCatVars(tblFiltered);

                int rvndx = tblFiltered.Columns.IndexOf(ResponseVarColName);
                //string responseVarColName = dt.Columns[rvndx].Caption;
                frmTransform frmT = new frmTransform(tblFiltered, rvndx);
                DialogResult dlgr = frmT.ShowDialog();

                if (dlgr != DialogResult.Cancel)
                {
                    DataTable dtnew = frmT.PCDT;

                    registerNewCols(dtnew);

                    //add any disabled columns back into the operational datatable
                    dtnew = addDisabledCols(dtnew, dt);

                    //add any response variables back ...
                    dtnew = addHiddenResponseVarCols(dtnew, dt);

                    //add any previously transform cols back
                    //dtnew = addOldTCols(dtnew, _dt);
                    dtnew = addOldTCols(dtnew, dt);

                    dtnew = addCatCols(dtnew, dt);

                    foreach (DataColumn dc in dtnew.Columns)
                    {
                        if (dc.ExtendedProperties.ContainsKey(VBCommon.Globals.TRANSFORM)) //||
                            //dc.ExtendedProperties.ContainsKey(VBTools.Globals.OPERATION) ||
                            //dc.ExtendedProperties.ContainsKey(VBTools.Globals.DECOMPOSITION) )
                            dtnew = arrangeTableCols(dtnew, dc);
                    }

                    //update global table
                    dt = dtnew;
                    dt.AcceptChanges();

                    //grid operations
                    SelectedColIndex = dt.Columns.IndexOf(ResponseVarColName);
                    ResponseVarColIndex = dt.Columns.IndexOf(ResponseVarColName);
                    maintainGrid(dgv, dt, SelectedColIndex, ResponseVarColName);

                    //count iv columns and update list
                    /*int nonivs = HiddenCols + 2;
                    NumberIVs = dt.Columns.Count - nonivs;
                    UpdateListView(VBCommon.Globals.listvals.NIVS, NumberIVs);
                    UpdateListView(VBCommon.Globals.listvals.NCOLS, dt.Columns.Count);
                    */
                    UpdateListView();

                    NotifyContainer();

                    //_state = _dtState.dirty;
                }
            }
            else
            {
                MessageBox.Show("Must import data first.", "Proceedural Error", MessageBoxButtons.OK);
            }
        }


        /// <summary>
        /// invoke the data manipulation tool - allows for creation of data columns of interaction data
        /// (sums, means, products, ...etc)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnManipulate_Click(object sender, EventArgs e)
        {
            if (dt != null)
            {
                DataTable tblFiltered = filterDisabledCols(dt);
                int rvndx = tblFiltered.Columns.IndexOf(ResponseVarColName);
                //string responseVarColName = dt.Columns[rvndx].Caption;
                frmManipulate frmIA = new frmManipulate(tblFiltered, rvndx);
                DialogResult dlgr = frmIA.ShowDialog();

                if (dlgr != DialogResult.Cancel)
                {
                    DataTable dtnew = frmIA.NewDataTable;

                    registerNewCols(dtnew);
                    dtnew = addDisabledCols(dtnew, dt);

                    dt = dtnew;
                    maintainGrid(dgv, dtnew, SelectedColIndex, ResponseVarColName);

                    /*int nonivs = HiddenCols + 2;
                    NumberIVs = dt.Columns.Count - nonivs;
                    UpdateListView(VBCommon.Globals.listvals.NIVS, NumberIVs);
                    UpdateListView(VBCommon.Globals.listvals.NCOLS, dt.Columns.Count);*/
                    UpdateListView();

                    NotifyContainer();
                }
            }
            else
            {
                MessageBox.Show("Must import data first.", "Proceedural Error", MessageBoxButtons.OK);
            }
        }


        // invoke the wind/current component decomposition tool
        // creates data columns of orthogonal wind/current components
        public void btnComputeAO_Click(object sender, EventArgs e)
        {
            //just adds columns for wind and/or current components to the datatable/grid view
            //this will need some sort of property setting mechanisms to reset columns to
            //hidden, enabled, etc when the form for decomposition exits.
            if (dt != null)
            {
                //DataTable dt = filterDataTableCols(_dt);
                DataTable tblFiltered = filterDisabledCols(dt);
                string rvname = ResponseVarColName;
                string dtsname = tblFiltered.Columns[0].Caption;

                VBCommon.Controls.frmUV frmWC = new VBCommon.Controls.frmUV(tblFiltered, rvname, dtsname, dblOrientation);
                frmWC.ShowDialog();

                DataTable dtnew = frmWC.WCDT;

                //this will effect to enable column context menus on new columns
                foreach (DataColumn c in dtnew.Columns)
                {
                    if (!DTCI.GetColStatus(c.ColumnName))
                        DTCI.AddColumnNameToDict(c.ColumnName);
                }

                //add disabled col back in
                dtnew = addDisabledCols(dtnew, dt);

                //mark created cols as decomposition
                List<string> newcols = frmWC.WCColsAdded;
                foreach (string colname in newcols)
                {
                    dtnew.Columns[colname].ExtendedProperties[VBCommon.Globals.DECOMPOSITION] = true;
                }

                dt = dtnew;
                dgv.DataSource = dtnew;
                maintainGrid(dgv, dt, SelectedColIndex, ResponseVarColName);

                /*int nonivs = HiddenCols + 2;
                NumberIVs = dt.Columns.Count - nonivs;
                UpdateListView(VBCommon.Globals.listvals.NIVS, NumberIVs);
                UpdateListView(VBCommon.Globals.listvals.NCOLS, dt.Columns.Count);*/
                UpdateListView();

                NotifyContainer();
            }
            else
            {
                MessageBox.Show("Must import data first.", "Proceedural Error", MessageBoxButtons.OK);
            }
        }
    }
}

