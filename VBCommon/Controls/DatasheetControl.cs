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
        private string strSelectedColName = string.Empty;

        private enum AddReplace { Add, Replace };
        private AddReplace addreplace;

        //data state relative to data used in modeling/residuals/prediction
        //state is dirty until the project manager's version of the datatable
        //matches the filtered "gotomodeling" datatable version
        public enum dtState { clean, dirty };
        private dtState state = dtState.dirty;

        //dealing with transform
        private VBCommon.DependentVariableTransforms depVarTransform;
        private double dblPowerTransformExp = double.NaN;
        private string fn = string.Empty;

        public enum listvals { NCOLS, NROWS, DATECOLNAME, RVCOLNAME, BLANK, NDISABLEDROWS, NDISABLEDCOLS, NHIDDENCOLS, NIVS };
        private Type ListVal = typeof(listvals);
        private int intNdisabledcols = 0;
        private int intNdisabledrows = 0;
        private int intNhiddencols = 0;
        private int intNivs = 0;

        // getter/setter for transform type
        [JsonProperty]
        public VBCommon.DependentVariableTransforms DependentVariableTransform  
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
            set { dt = value; }
        }


        public dtRowInformation DTRI
        {
            get { return this.dtRI; }
            set { dtRI = value; }
        }


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


        //return Disabled columns
        [JsonProperty]
        public int DisabledCols
        {
            get { return this.intNdisabledcols; }
            set { intNdisabledcols = value; }
        }


        //return Disabled Rows
        [JsonProperty]
        public int DisabledRows
        {
            get { return this.intNdisabledrows; }
            set { intNdisabledrows = value; }
        }


        //return Hidden Columns
        [JsonProperty]
        public int HiddenCols
        {
            get { return this.intNhiddencols; }
            set { intNhiddencols = value; }
        }


        //return Number of IVs
        [JsonProperty]
        public int NumberIVs
        {
            get { return this.intNivs; }
            set { intNivs = value; }
        }


        //constructor
        public DatasheetControl()      
        {
            InitializeComponent();
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
        public void datasheet_load(object sender, EventArgs e)      
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
        }


        // populate the UI list with some file/data information
        public void showListInfo(string fn, DataTable dt)
        {
            this.dt = dt;
            int intNcols = dt.Columns.Count;
            int intNrows = dt.Rows.Count;
            string strDtname = dt.Columns[0].ColumnName.ToString();
            string strDepvarname = dt.Columns[1].ColumnName.ToString();
            intNivs = dt.Columns.Count - 2;
            //clear listView
            listView1.Clear();
            listView1.View = View.Details;

            ListViewItem lvi;
            //add column counts to listView
            lvi = new ListViewItem("Column Count");
            lvi.SubItems.Add(intNcols.ToString());
            listView1.Items.Add(lvi);
            //add row counts to listView
            lvi = new ListViewItem("Row Count");
            lvi.SubItems.Add(intNrows.ToString());
            listView1.Items.Add(lvi);
            //add datetime to listView
            lvi = new ListViewItem("Date-Time Index");
            lvi.SubItems.Add(strDtname);
            listView1.Items.Add(lvi);
            //add resp var to listView
            lvi = new ListViewItem("Response Variable");
            lvi.SubItems.Add(strDepvarname);
            listView1.Items.Add(lvi);
            //add space to listView
            lvi = new ListViewItem("");
            lvi.SubItems.Add("");
            listView1.Items.Add(lvi);
            //add disabled row counts to listView
            lvi = new ListViewItem("Disabled Row Count");
            lvi.SubItems.Add(intNdisabledrows.ToString());
            listView1.Items.Add(lvi);
            //add disabled column counts to listView
            lvi = new ListViewItem("Disabled Column Count");
            lvi.SubItems.Add(intNdisabledcols.ToString());
            listView1.Items.Add(lvi);
            //add hidden column counts to listView
            lvi = new ListViewItem("Hidden Column Count");
            lvi.SubItems.Add(intNhiddencols.ToString());
            listView1.Items.Add(lvi);
            //add indep var counts to listView
            lvi = new ListViewItem("Independent Variable Count");
            lvi.SubItems.Add(intNivs.ToString());
            listView1.Items.Add(lvi);

            ////magic numbers for widths: -1 set to max characters in subitems, -2 == autosize
            listView1.Columns.Add("File", -1, HorizontalAlignment.Right);
            listView1.Columns.Add(fn, -2, HorizontalAlignment.Left);
        }

        
        // as user manipulates the dataset, track changes and update the UI listview
        public void updateListView(listvals listitem, object value)
        {
            string strName = string.Empty;
            int intNumber;

            switch (listitem)
            {
                case listvals.NCOLS:
                    intNumber = (int)value;
                    listView1.Items[0].SubItems[1].Text = intNumber.ToString();
                    break;
                case listvals.NROWS:
                    intNumber = (int)value;
                    listView1.Items[1].SubItems[1].Text = intNumber.ToString();
                    break;
                case listvals.DATECOLNAME:
                    strName = (string)value;
                    listView1.Items[2].SubItems[1].Text = strName;
                    break;
                case listvals.RVCOLNAME:
                    strName = (string)value;
                    listView1.Items[3].SubItems[1].Text = strName;
                    break;
                case listvals.BLANK:
                    intNumber = (int)value;
                    listView1.Items[4].SubItems[1].Text = "";
                    break;
                case listvals.NDISABLEDROWS:
                    intNumber = (int)value;
                    listView1.Items[5].SubItems[1].Text = intNumber.ToString();
                    break;
                case listvals.NDISABLEDCOLS:
                    intNumber = (int)value;
                    listView1.Items[6].SubItems[1].Text = intNumber.ToString();
                    break;
                case listvals.NHIDDENCOLS:
                    intNumber = (int)value;
                    listView1.Items[7].SubItems[1].Text = intNumber.ToString();
                    break;
                case listvals.NIVS:
                    intNumber = (int)value;
                    listView1.Items[8].SubItems[1].Text = intNumber.ToString();
                    break;
            }
        }
        
        
        // response variable transform log10
        public void log10T(object o, EventArgs e)
        {
            //can only transform the response variable and the response variable can not be a transformed ME or and interacted ME
            double[] newvals = new double[dt.Rows.Count];

            string newcolname = "LOG10[" + dt.Columns[intSelectedColIndex].Caption + "]";
            performTOperation(dt, newcolname, intSelectedColIndex, newvals);
            dt.Columns[newcolname].ExtendedProperties[VBCommon.Globals.DEPENDENTVARIBLEDEFINEDTRANSFORM] = VBCommon.DependentVariableTransforms.Log10.ToString();

            intResponseVarColIndex = dt.Columns.IndexOf(newcolname);
            strResponseVarColName = dt.Columns[intResponseVarColIndex].Caption;

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
                    dtCopy.Columns[newcolname].ExtendedProperties[VBCommon.Globals.DEPENDENTVARIBLETRANSFORM] = true;

                    //set properties of old one
                    dtCopy.Columns[selectedColIndex].ExtendedProperties[VBCommon.Globals.HIDDEN] = true;

                    dt = dtCopy;
                    maintainGrid(dgv, dt, selectedColIndex, strResponseVarColName);

                    updateListView(listvals.NHIDDENCOLS, ++intNhiddencols);
                    //count iv columns and update list
                    int nonivs = intNhiddencols > 0 ? 3 : 2;
                    intNivs = dt.Columns.Count - nonivs;
                    updateListView(listvals.NIVS, intNivs);
                    //and rv name
                    updateListView(listvals.RVCOLNAME, strResponseVarColName);

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
                bool colisI = c.ExtendedProperties.ContainsKey(VBCommon.Globals.OPERATION);
                if (colisT == true) return true;
            }
            return false;
        }

        /// <summary>
        /// determine if the table has manipulated (interacted) data columns
        /// </summary>
        /// <returns>true if table has manipulated columns, false otherwise</returns>
        public bool hasOCols()
        {
            foreach (DataColumn c in dt.Columns)
            {
                bool colisI = c.ExtendedProperties.ContainsKey(VBCommon.Globals.OPERATION);
                if (colisI == true) return true;
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
      

        // response variable transform natural log
        public void lnT(object o, EventArgs e)
        {
            double[] newvals = new double[dt.Rows.Count];

            string newcolname = "LN[" + dt.Columns[intSelectedColIndex].Caption + "]";
            performTOperation(dt, newcolname, intSelectedColIndex, newvals);
            dt.Columns[newcolname].ExtendedProperties[VBCommon.Globals.DEPENDENTVARIBLEDEFINEDTRANSFORM] = VBCommon.DependentVariableTransforms.Ln.ToString();
            depVarTransform = VBCommon.DependentVariableTransforms.Ln;
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
                dt.Columns[newcolname].ExtendedProperties[VBCommon.Globals.DEPENDENTVARIBLEDEFINEDTRANSFORM] = VBCommon.DependentVariableTransforms.Power.ToString() + "," + sexp;
                depVarTransform = VBCommon.DependentVariableTransforms.Power;
                dblPowerTransformExp = Convert.ToDouble(sexp);
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

                //updateListView(_listvals.NDISABLEDROWS, ++_ndisabledrows);
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
        // note that there are no table row extended properties - row status is tracked
        // in the dtRowInformation class
        public void DisableRow(object sender, EventArgs e)
        {
            //update the dtRowInformation dictionary of disabled/enabled rows
            dtRI.SetRowStatus(dt.Rows[intSelectedRowIndex][0].ToString(), false);
            for (int c = 0; c < dgv.Columns.Count; c++)
            {
                dgv[c, intSelectedRowIndex].Style.ForeColor = Color.Red;
                
            }

            //check to see if this index is already in extendedProps (was set to true before)
            //set extendedproperties so dtRowInformation can maintain dictDTRI
            if (dt.ExtendedProperties.ContainsKey(intSelectedRowIndex.ToString()))
                dt.ExtendedProperties.Remove(intSelectedRowIndex.ToString());
            dt.ExtendedProperties.Add(intSelectedRowIndex.ToString(), "false");

            updateListView(listvals.NDISABLEDROWS, ++intNdisabledrows);
            state = dtState.dirty;
            NotifyContainer();
        }

       
        // set grid/listview properties for the user-selected row menu item Enable
        // note that there are no table row extended properties - row status is tracked
        // in the dtRowInformation class
        public void EnableRow(object sender, EventArgs e)
        {
            dtRI.SetRowStatus(dt.Rows[intSelectedRowIndex][0].ToString(), true);

            for (int c = 0; c < dgv.Columns.Count; c++)
            {
                if (!dtCI.GetColStatus(dgv.Columns[c].Name.ToString())) continue;
                dgv[c, intSelectedRowIndex].Style.ForeColor = Color.Black;
                
            }

            //check to see if this index is already in extendedProps (was set to false before)
            //then set extendedproperties so dtRowInformation can maintain dictDTRI
            if (dt.ExtendedProperties.ContainsKey(intSelectedRowIndex.ToString()))
                dt.ExtendedProperties.Remove(intSelectedRowIndex.ToString());
            dt.ExtendedProperties.Add(intSelectedRowIndex.ToString(), "true");
            
            updateListView(listvals.NDISABLEDROWS, --intNdisabledrows);
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
                        intSelectedColIndex = dtCopy.Columns.IndexOf(c);
                        strResponseVarColName = dtCopy.Columns[intSelectedColIndex].Caption;
                        intResponseVarColIndex = intSelectedColIndex;

                        updateListView(listvals.NHIDDENCOLS, --intNhiddencols);
                        break;
                    }
                }

                dt = dtCopy;
                maintainGrid(dgv, dt, intSelectedColIndex, strResponseVarColName);
                //count iv columns and update list
                int nonivs = intNhiddencols > 0 ? 3 : 2;
                intNivs = dt.Columns.Count - nonivs;
                updateListView(listvals.NIVS, intNivs);
                //and rv name
                updateListView(listvals.RVCOLNAME, strResponseVarColName);

                state = dtState.dirty;
                NotifyContainer();
            }
        }

        public void defineTransformForRV(object o, EventArgs e)
        {
            //menu response from right click, determine which transform was selected
            MenuItem mi = (MenuItem)o;
            string transform = mi.Text;
            if (transform == VBCommon.DependentVariableTransforms.Power.ToString())
            {
                frmPowerExponent frmExp = new frmPowerExponent(dt, intSelectedColIndex);
                DialogResult dlgr = frmExp.ShowDialog();
                if (dlgr != DialogResult.Cancel)
                {
                    string sexp = frmExp.Exponent.ToString("n2");
                    transform += "," + sexp;
                    depVarTransform = VBCommon.DependentVariableTransforms.Power;
                    dblPowerTransformExp = Convert.ToDouble(sexp);
                    dt.Columns[intSelectedColIndex].ExtendedProperties[VBCommon.Globals.DEPENDENTVARIBLEDEFINEDTRANSFORM] = transform;
                    state = dtState.dirty;
                    NotifyContainer();
                }
            }
            else
            {
                if (String.Compare(transform, "Log10", true) == 0)
                    depVarTransform = VBCommon.DependentVariableTransforms.Log10;
                else if (String.Compare(transform, "Ln", true) == 0)
                    depVarTransform = VBCommon.DependentVariableTransforms.Ln;
                else if (String.Compare(transform, "none", true) == 0)
                    depVarTransform = VBCommon.DependentVariableTransforms.none;

                dt.Columns[intSelectedColIndex].ExtendedProperties[VBCommon.Globals.DEPENDENTVARIBLEDEFINEDTRANSFORM] = transform;
                state = dtState.dirty;
                NotifyContainer();
            }
        }


        // set table/grid/listview properties for the user-selected column menu item Disable
        public void DisableCol(object sender, EventArgs e)
        {
            string cn = dt.Columns[intSelectedColIndex].Caption;
            dtCI.SetColStatus(dt.Columns[intSelectedColIndex].ColumnName.ToString(), false);
            dt.Columns[intSelectedColIndex].ExtendedProperties[VBCommon.Globals.ENABLED] = false;
            updateListView(listvals.NDISABLEDCOLS, ++intNdisabledcols);
            updateListView(listvals.NIVS, --intNivs);
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
            updateListView(listvals.NDISABLEDCOLS, --intNdisabledcols);
            updateListView(listvals.NIVS, ++intNivs);
            enableGridCol(dgv, intSelectedColIndex, dt);

            state = dtState.dirty;
            NotifyContainer();
        }


        /// <summary>
        /// set table/grid/listview properties for the user-selected column menu item SetResponse.
        /// pick this variable as the response variable, and if the previous response variable was a 
        /// transform, delete it from the table and unhide its original column.  Further, if
        /// independent variable transforms are present in the table remove them from the table (with 
        /// user permission)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SetResponse(object sender, EventArgs e)
        {
            //gridutils = new Utilities.GridUtils(dgv); //otherwise getting error at line 838 

            DialogResult dlgr = DialogResult.Yes;
            if (hasTCols())
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
                dt.Columns[intResponseVarColIndex].ExtendedProperties[VBCommon.Globals.DEPENDENTVAR] = false;

                //filter transformed cols
                if (hasTCols()) dt = filterTcols(dt);

                if (testValueAttribute(dt.Columns[strResponseVarColName], VBCommon.Globals.DEPENDENTVARIBLETRANSFORM))
                {
                    dt.Columns.Remove(dt.Columns[strResponseVarColName].Caption);
                    dt.AcceptChanges();
                    unHideHiddenCols(dgv, dt);

                    updateListView(listvals.NHIDDENCOLS, --intNhiddencols);
                }

                intResponseVarColIndex = dt.Columns.IndexOf(strSelectedColName);
                strResponseVarColName = dt.Columns[intResponseVarColIndex].Caption;

                maintainGrid(dgv, dt, intSelectedColIndex, strResponseVarColName);

                //count iv columns and update list
                int nonivs = intNhiddencols > 0 ? 3 : 2;
                intNivs = dt.Columns.Count - nonivs;
                updateListView(listvals.NIVS, intNivs);
                //and rv name
                updateListView(listvals.RVCOLNAME, strResponseVarColName);

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
                updateListView(listvals.NCOLS, dt.Columns.Count);
                updateListView(listvals.NIVS, --intNivs);

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

            intNdisabledcols = 0;
            updateListView(listvals.NDISABLEDCOLS, intNdisabledcols);

            int intNonivs = intNhiddencols > 0 ? 3 : 2;
            intNivs = dt.Columns.Count - intNonivs;
            updateListView(listvals.NIVS, intNivs);
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

            intNdisabledrows = 0;
            updateListView(listvals.NDISABLEDROWS, intNdisabledrows);
            state = dtState.dirty;
            NotifyContainer();
        }


        //done editing, accept changes
        public void dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            dgv.EndEdit();
            dt.Rows[e.RowIndex][e.ColumnIndex] = dgv[e.ColumnIndex, e.RowIndex].Value;
            dt.AcceptChanges();
            state = dtState.dirty;
            NotifyContainer();
            maintainGrid(dgv, dt, SelectedColIndex, ResponseVarColName); //ensure disabled rows/cols stay red
        }

        //response if error
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
            dtColumnInformation dtCI = new dtColumnInformation(dt);

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
            //save packed state to a dictionary
            IDictionary<string, object> dictPackedState = new Dictionary<string, object>();
            
            //pack up mainEffect columns for Prediction
            dictPackedState.Add("CurrentColIndex", this.SelectedColIndex);
            dictPackedState.Add("DepVarColName", this.ResponseVarColName);
            dictPackedState.Add("DTColInfo", this.DTCI.DTColInfo);
            dictPackedState.Add("DTRowInfo", this.DTRI.DTRowInfo);
            dictPackedState.Add("DepVarTransform", this.DependentVariableTransform);


            //pack up listInfo for model datasheet
            dictPackedState.Add("ColCount", this.DT.Columns.Count);
            dictPackedState.Add("RowCount", this.DT.Rows.Count);
            dictPackedState.Add("DateIndex", this.DT.Columns[0].ColumnName.ToString());
            dictPackedState.Add("ResponseVar", this.DT.Columns[1].ColumnName.ToString());
            dictPackedState.Add("DisabledRwCt", this.DisabledRows);
            dictPackedState.Add("DisabledColCt", this.DisabledCols);
            dictPackedState.Add("HiddenColCt", this.HiddenCols);
            dictPackedState.Add("IndVarCt", this.NumberIVs);
            dictPackedState.Add("fileName", this.FileName);

            
            //Save Datasheet info as xml string for serialization
            StringWriter sw = null;
            if (this.DT != null)
            {
                this.DT.TableName = "DataSheetData";
                sw = new StringWriter();
                this.DT.WriteXml(sw, XmlWriteMode.WriteSchema, false);
                string strXmlDataTable = sw.ToString();
                sw.Close();
                sw = null;
                dictPackedState.Add("XmlDataTable", strXmlDataTable);
            }

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
            DataTable Filtered4ModelDt = this.DT;
            Filtered4ModelDt.Columns[this.ResponseVarColName].SetOrdinal(1);
            //filter diabled rows and columns
            Filtered4ModelDt = this.filterDataTableRows(Filtered4ModelDt);
            Utilities.TableUtils tableutils = new Utilities.TableUtils(Filtered4ModelDt);
            Filtered4ModelDt = tableutils.filterRVHcols(Filtered4ModelDt);
            dictPackedState.Add("DataSheetDatatable", Filtered4ModelDt);  //for modeling to use

            return dictPackedState;
        }


        //show the modeling datasheet when no changes made to global
        public void UnhideModelDS(DataTable dt)
        {
            this.DT = dt;
            maintainGrid(this.dgv, this.DT, this.SelectedColIndex, this.ResponseVarColName);
            showListInfo(this.FileName, this.DT);
        }


        //unpack event handler. unpacks packed state in dictionary to repopulate datasheet
        public void UnpackState(IDictionary<string, object> dictPackedState)
        {
            //unpack datatable
            //unpack xmlDT and repopulate this.DT
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


            //.....need to change from 1 to the actual type.......
            string depVarTran = Convert.ToString(dictPackedState["DepVarTransform"]);
            this.DependentVariableTransform = (VBCommon.DependentVariableTransforms)Enum.Parse(typeof(VBCommon.DependentVariableTransforms), depVarTran);
            this.ResponseVarColName = (string)dictPackedState["DepVarColName"];
            this.ResponseVarColIndex = this.DT.Columns.IndexOf(this.ResponseVarColName);

            maintainGrid(this.dgv, this.DT, this.SelectedColIndex, this.ResponseVarColName);

            //initial info for the list
            this.FileName = (string)dictPackedState["fileName"];

            //unpack listInfo for model datasheet
            //need to convert if its unpacked from saved project
            if (dictPackedState["DisabledColCt"].GetType().ToString() == "System.Int64")
                this.DisabledCols = Convert.ToInt16((Int64)dictPackedState["DisabledColCt"]);
            else this.DisabledCols = (int)dictPackedState["DisabledColCt"];

            if (dictPackedState["DisabledRwCt"].GetType().ToString() == "System.Int64")
                this.DisabledRows = Convert.ToInt16((Int64)dictPackedState["DisabledRwCt"]);
            else this.DisabledRows = (int)dictPackedState["DisabledRwCt"];

            if (dictPackedState["HiddenColCt"].GetType().ToString() == "System.Int64")
                this.HiddenCols = Convert.ToInt16((Int64)dictPackedState["HiddenColCt"]);
            else this.HiddenCols = (int)dictPackedState["HiddenColCt"];
            
            if (dictPackedState["IndVarCt"].GetType().ToString() == "System.Int64")
                this.NumberIVs = Convert.ToInt16((Int64)dictPackedState["IndVarCt"]);
            else this.NumberIVs = (int)dictPackedState["IndVarCt"];

            this.showListInfo(this.FileName, this.DT);

            if ((bool)dictPackedState["Clean"])
            {
                this.State = VBCommon.Controls.DatasheetControl.dtState.clean;
            }
            else
            {
                this.State = VBCommon.Controls.DatasheetControl.dtState.dirty;
            }
        }


        /*public void registerNewCols(DataTable dt)
        {
            _dtCI = new dtColumnInformation(dt);
            //          _dtCI = dtColumnInformation.getdtCI(dt, false);
            foreach (DataColumn c in dt.Columns)
            {
                if (!_dtCI.GetColStatus(c.ColumnName))
                {
                    _dtCI.AddColumnNameToDict(c.ColumnName);
                }

            }
        }*/


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
            }

            //reset the UI clues for the response variable
            dgv.Columns[responseVarColName].DefaultCellStyle.BackColor = Color.LightBlue;

            //reset disable rows
            dtRI = new dtRowInformation(dt);
            for (int r = 0; r < dt.Rows.Count; r++)
            {
                //bool enabled = _dtRI.GetRowStatus(dt.Rows[r][0].ToString());
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
                //set style to black unless the row is disabled
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
            //_dtRI = new dtRowInformation(dt);
            for (int r = 0; r < dt.Rows.Count; r++)
            {
                //bool enabled = _dtRI.GetRowStatus(dt.Rows[r][0].ToString());
                bool enabled = dtRI.GetRowStatus(dt.Rows[r][0].ToString());
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

