using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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

        private Utilities utils = null;
        private Utilities.TableUtils tableutils = null;
        private Utilities.GridUtils gridutils = null;

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

        bool boolValidated = false;

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


        ////returns datatable row info
        [JsonProperty]
        public Dictionary<string, bool> DTRowInfo
        {
            get { return this.dtRI.DTRowInfo; }
            set { dtRI.DTRowInfo = value; }
        }


        //// returns datatable column info
        [JsonProperty]
        public Dictionary<string, bool> DTColInfo     
        {
            get { return this.dtCI.DTColInfo; }
            set { dtCI.DTColInfo = value; }
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


        //return utilities
        [JsonProperty]
        public Utilities Utils
        {
            get {
                if (utils == null)
                {
                    utils = new Utilities();
                }
                return this.utils;
            }
            set { utils = value; }
        }


        //return utilities
        [JsonProperty]
        public Utilities.TableUtils TableUtils
        {
            get {
                if (tableutils == null)
                {
                    tableutils = new Utilities.TableUtils(dt);
                } 
                return this.tableutils;
            }
            set { tableutils = value; }
        }


        //return utilities
        [JsonProperty]
        public Utilities.GridUtils GridUtils
        {
            get {
                if (gridutils == null)
                {
                    gridutils = new Utilities.GridUtils(dgv);
                } 
                return this.gridutils;
            }
            set { gridutils = value; }
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
                    utils.setAttributeValue(dtCopy.Columns[selectedColIndex], VBCommon.Globals.DEPENDENTVAR, false);
                    string colname = dtCopy.Columns[selectedColIndex].Caption;

                    if (hasTCols())
                    {
                        dtCopy = tableutils.filterTcols(dtCopy);
                    }

                    dtCopy = tableutils.setHiddenIVstoUnhidden(dtCopy);
                    int ordinal = dtCopy.Columns.IndexOf(colname);
                    dtCopy.Columns.Add(newcolname, typeof(double));

                    for (int r = 0; r < dtCopy.Rows.Count; r++)
                        dtCopy.Rows[r][newcolname] = newvals[r];
                    dtCopy.Columns[newcolname].SetOrdinal(ordinal + 1);
                    dtCopy.AcceptChanges();
                    dtCI.addColumnNameToDic(newcolname);

                    //set properties of new rv
                    strResponseVarColName = newcolname;
                    intResponseVarColIndex = dtCopy.Columns.IndexOf(strResponseVarColName);
                    utils.setAttributeValue(dtCopy.Columns[newcolname], VBCommon.Globals.DEPENDENTVARIBLETRANSFORM, true);

                    //set properties of old one
                    utils.setAttributeValue(dtCopy.Columns[selectedColIndex], VBCommon.Globals.HIDDEN, true);

                    dt = dtCopy;
                    gridutils.maintainGrid(dgv, dt, selectedColIndex, strResponseVarColName);

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
            dtRI.setRowStatus(dt.Rows[intSelectedRowIndex][0].ToString(), false);
            for (int c = 0; c < dgv.Columns.Count; c++)
            {
                dgv[c, intSelectedRowIndex].Style.ForeColor = Color.Red;
            }
            updateListView(listvals.NDISABLEDROWS, ++intNdisabledrows);
            state = dtState.dirty;
            NotifyContainer();
        }

       
        // set grid/listview properties for the user-selected row menu item Enable
        // note that there are no table row extended properties - row status is tracked
        // in the dtRowInformation class
        public void EnableRow(object sender, EventArgs e)
        {
            dtRI.setRowStatus(dt.Rows[intSelectedRowIndex][0].ToString(), true);
            for (int c = 0; c < dgv.Columns.Count; c++)
            {
                if (!dtCI.getColStatus(dgv.Columns[c].Name.ToString())) continue;
                dgv[c, intSelectedRowIndex].Style.ForeColor = Color.Black;
            }

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
                    dtCopy = tableutils.filterTcols(dtCopy);
                }

                foreach (DataColumn c in dtCopy.Columns)
                {
                    if (!utils.testValueAttribute(c, VBCommon.Globals.HIDDEN)) continue;
                    {
                        utils.setAttributeValue(c, VBCommon.Globals.DEPENDENTVAR, true);
                        utils.setAttributeValue(c, VBCommon.Globals.HIDDEN, false);
                        intSelectedColIndex = dtCopy.Columns.IndexOf(c);
                        strResponseVarColName = dtCopy.Columns[intSelectedColIndex].Caption;
                        intResponseVarColIndex = intSelectedColIndex;

                        updateListView(listvals.NHIDDENCOLS, --intNhiddencols);
                        break;
                    }
                }

                dt = dtCopy;
                gridutils.maintainGrid(dgv, dt, intSelectedColIndex, strResponseVarColName);
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
            dtCI.setColStatus(dt.Columns[intSelectedColIndex].ColumnName.ToString(), false);
            dt.Columns[intSelectedColIndex].ExtendedProperties[VBCommon.Globals.ENABLED] = false;
            updateListView(listvals.NDISABLEDCOLS, ++intNdisabledcols);
            updateListView(listvals.NIVS, --intNivs);
            gridutils.disableGridCol(dgv, intSelectedColIndex);

            state = dtState.dirty;
            NotifyContainer();
        }


        // set table/grid/listview properties for the user-selected column menu item Enable
        public void EnableCol(object sender, EventArgs e)
        {
            string cn = dt.Columns[intSelectedColIndex].Caption;
            dtCI.setColStatus(dt.Columns[intSelectedColIndex].ColumnName.ToString(), true);
            dt.Columns[intSelectedColIndex].ExtendedProperties[VBCommon.Globals.ENABLED] = true;
            updateListView(listvals.NDISABLEDCOLS, --intNdisabledcols);
            updateListView(listvals.NIVS, ++intNivs);
            gridutils.enableGridCol(dgv, intSelectedColIndex, dt);

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
            gridutils = new Utilities.GridUtils(dgv); //otherwise getting error at line 838 

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
                utils.setAttributeValue(dt.Columns[intSelectedColIndex], VBCommon.Globals.DEPENDENTVAR, true);
                utils.setAttributeValue(dt.Columns[intResponseVarColIndex], VBCommon.Globals.DEPENDENTVAR, false);

                //filter transformed cols
                if (hasTCols()) dt = tableutils.filterTcols(dt);

                if (utils.testValueAttribute(dt.Columns[strResponseVarColName], VBCommon.Globals.DEPENDENTVARIBLETRANSFORM))
                {
                    dt.Columns.Remove(dt.Columns[strResponseVarColName].Caption);
                    dt.AcceptChanges();
                    gridutils.unHideHiddenCols(dgv, dt);

                    updateListView(listvals.NHIDDENCOLS, --intNhiddencols);
                }

                intResponseVarColIndex = dt.Columns.IndexOf(strSelectedColName);
                strResponseVarColName = dt.Columns[intResponseVarColIndex].Caption;

                gridutils.maintainGrid(dgv, dt, intSelectedColIndex, strResponseVarColName);

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
                dtCI.removeColumnFromDic(dt.Columns[intSelectedColIndex].Caption);

                int gridpos = dgv.FirstDisplayedScrollingColumnIndex;
                dt.Columns.Remove(dt.Columns[intSelectedColIndex].Caption);

                dt.AcceptChanges();

                gridutils.maintainGrid(dgv, dt, intSelectedColIndex, strResponseVarColName);

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
                dtCI.setColStatus(dt.Columns[c].Caption, true);
                dt.Columns[c].ExtendedProperties[VBCommon.Globals.ENABLED] = true;
                gridutils.enableGridCol(dgv, c, dt);
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
                dtRI.setRowStatus(dt.Rows[r][0].ToString(), true);
            }

            for (int r = 0; r < dgv.Rows.Count; r++)
            {
                for (int c = 0; c < dgv.Columns.Count; c++)
                {
                    if (!dtCI.getColStatus(dgv.Columns[c].Name.ToString())) continue;
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
            //dtColumnInformation dtCI = new dtColumnInformation(dt);
            //Utilities utils = new Utilities();

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
                        if (utils.testValueAttribute(dt.Columns[intResponseVarColIndex], VBCommon.Globals.DEPENDENTVAR))
                        {
                            cmforResponseVar.MenuItems[0].Enabled = true; //we can transform a response variable
                        }
                        else
                        {
                            cmforResponseVar.MenuItems[0].Enabled = false; //but we cannot transform a transformed response
                        }

                        if (utils.testValueAttribute(dt.Columns[intResponseVarColIndex], VBCommon.Globals.DEPENDENTVARIBLETRANSFORM))
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
                        if (dtCI.getColStatus(dt.Columns[intSelectedColIndex].ColumnName.ToString()))
                        {
                            //here if col enabled
                            cmforIVs.MenuItems[0].Enabled = true;
                            cmforIVs.MenuItems[1].Enabled = false; //cannot enable enabled col
                            cmforIVs.MenuItems[2].Enabled = true;

                            //response variable must be a ME, T(RV) or I(IV) not created by general transform
                            //if they do this then we're to remove all general operations performed,
                            if (canSetRV(utils)) cmforIVs.MenuItems[2].Enabled = true;
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
                //row header hit, show menu
                intSelectedRowIndex = rowndx;
                if (dtRI.getRowStatus(dt.Rows[intSelectedRowIndex][0].ToString()))
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
        public bool canSetRV(Utilities utils)
        {
            bool colisT = dt.Columns[intSelectedColIndex].ExtendedProperties.ContainsKey(VBCommon.Globals.TRANSFORM);
            bool colisI = dt.Columns[intSelectedColIndex].ExtendedProperties.ContainsKey(VBCommon.Globals.OPERATION);
            bool colisCat = utils.testValueAttribute(dt.Columns[intSelectedColIndex], VBCommon.Globals.CATEGORICAL);
            if (colisT == true || colisCat == true) return false;
            else return true;
        }


        //event handler for packing state to save project
        public IDictionary<string, object> PackState()
        {
            //save packed state to a dictionary
            IDictionary<string, object> dictPackedState = new Dictionary<string, object>();

            /*//check to see if this is the first time going to modeling
            if (this.State == VBCommon.Controls.DatasheetControl.dtState.dirty)
            {
                DialogResult dlgr = MessageBox.Show("Changes in data and/or data attributes have occurred.\nPrevious modeling results will be erased. Proceed?", "Proceed to Modeling.", MessageBoxButtons.OKCancel);
                if (dlgr == DialogResult.OK)
                {
                    correlationData = this.DT;
                    dataSheetData = this.DT;
                    this.State = VBCommon.Controls.DatasheetControl.dtState.clean;
                }
                else
                { return null; }
            }
            else if (boolInitialPass)
            {
                correlationData = this.DT;
                modelData = this.DT;
                this.State = VBCommon.Controls.DatasheetControl.dtState.clean;
                boolInitialPass = false;
            }*/

            dictPackedState.Add("CorrelationDataTable", this.DT); //for Modeling to use
            dictPackedState.Add("ModelDataTable", this.DT);   //for Modeling to use
            dictPackedState.Add("DT", this.DT);
            //pack up mainEffect columns for Prediction
            dictPackedState.Add("CurrentColIndex", this.SelectedColIndex);
            dictPackedState.Add("DepVarColName", this.ResponseVarColName);
            dictPackedState.Add("DTColInfo", this.DTCI.DTColInfo);
            dictPackedState.Add("DTRowInfo", this.DTRI.DTRowInfo);
            dictPackedState.Add("DSValidated", boolValidated);

            //pack up listInfo for model datasheet
            int intNumCols = this.DT.Columns.Count;
            int intNumRows = this.DT.Rows.Count;
            string strDateName = this.DT.Columns[0].ColumnName.ToString();
            string strResponseVar = this.DT.Columns[1].ColumnName.ToString();
            dictPackedState.Add("ColCount", intNumCols);
            dictPackedState.Add("RowCount", intNumRows);
            dictPackedState.Add("DateIndex", strDateName);
            dictPackedState.Add("ResponseVar", strResponseVar);
            dictPackedState.Add("DisabledRwCt", this.DisabledRows);
            dictPackedState.Add("DisabledColCt", this.DisabledCols);
            dictPackedState.Add("HiddenColCt", this.HiddenCols);
            dictPackedState.Add("IndVarCt", this.NumberIVs);
            dictPackedState.Add("fileName", this.FileName);

            StringWriter sw = null;
            //Save Datasheet info as xml string for serialization
            sw = null;
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

            //model expects this change to the dt first
            DataTable tempDt = (DataTable)dictPackedState["DT"];
            tempDt.Columns[this.ResponseVarColName].SetOrdinal(1);
            //filter diabled rows and columns
            tempDt = this.filterDataTableRows(tempDt);
            Utilities.TableUtils tableutils = new Utilities.TableUtils(tempDt);
            tempDt = tableutils.filterRVHcols(tempDt);
            dictPackedState.Add("DataSheetDatatable", tempDt);  //for modeling to use

            return dictPackedState;
        }


        //unpack event handler. unpacks packed state in dictionary to repopulate datasheet
        public void UnpackState(IDictionary<string, object> dictPackedState)
        {
            //unpack datatable

            this.DT = (DataTable)dictPackedState["DT"];
            this.DT.TableName = "DataSheetData";
            this.dgv.DataSource = null;
            this.dgv.DataSource = this.DT;

            //get row and column information
            this.DTRI = VBCommon.Metadata.dtRowInformation.getdtRI(this.DT, true);
            this.DTRI.DTRowInfo = (Dictionary<string, bool>)dictPackedState["DTRowInfo"];

            this.DTCI = VBCommon.Metadata.dtColumnInformation.getdtCI(this.DT, true);
            this.DTCI.DTColInfo = (Dictionary<string, bool>)dictPackedState["DTColInfo"];

            this.SelectedColIndex = (int)dictPackedState["CurrentColIndex"];
            this.ResponseVarColName = (string)dictPackedState["DepVarColName"];
            this.ResponseVarColIndex = this.DT.Columns.IndexOf(this.ResponseVarColName);
            //get validated flag
            this.boolValidated = (bool)dictPackedState["DSValidated"];

            this.Utils = new VBCommon.Metadata.Utilities();
            this.TableUtils = new VBCommon.Metadata.Utilities.TableUtils(this.DT);
            this.GridUtils = new VBCommon.Metadata.Utilities.GridUtils(this.dgv);

            this.GridUtils.maintainGrid(this.dgv, this.DT, this.SelectedColIndex, this.ResponseVarColName);

            //initial info for the list
            this.FileName = (string)dictPackedState["fileName"];
            
            //FileInfo fi = new FileInfo(Name);
            //this.FileName = fi.Name;
            this.showListInfo(this.FileName, this.DT);

            if ((bool)dictPackedState["Clean"])
            {
                this.State = VBCommon.Controls.DatasheetControl.dtState.clean;
            }
            else
            {
                this.State = VBCommon.Controls.DatasheetControl.dtState.dirty;
            }

            //if clean, initial pass is false
            //boolInitialPass = !(bool)dictPluginState["Clean"];
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

