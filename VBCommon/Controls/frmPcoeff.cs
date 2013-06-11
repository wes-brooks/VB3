using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
using Accord;
using Accord.Math;
using Accord.Statistics;
//using LogUtilities;
//using VBTools;

namespace VBCommon.Controls
{
    /// <summary>
    /// Class displays and processes interacted and transformed variables the user imported.
    /// Processing includes calculating univariate correlation coefficients (Pearson coeff)
    /// for each variable and its transforms. Three modes of selecting the winning variable
    /// or its transform are provided: 
    /// auto select: variable or it transform with the max pearson is selected, 
    /// percentage select: a transform of the variable must exceed the pearson score
    /// of the untransform variable's pearson score by a user seleted percentage to be selected
    /// manual select: users mouse click on the row header of the variable/transform they want 
    /// selected.
    /// For each variable, only the variable, or one of its transforms is included (returned) 
    /// for consideration in model building.  I.e., if you pass 11 variables, you get back 11
    /// variables but one or more returned will probably be transforms of the variables passed.
    /// Facility is also provided to allow viewing of plots of variable/transforms groups.
    /// The idea here is to rank and select the best variables (by pearson score) for MLR model
    /// building.
    /// </summary>
    public partial class frmPcoeff : Form
    {
        // The DataGridView Control which will be printed.
        private DataGridView MyDataGridView;
        // The class that will do the printing process.
        private DataGridViewPrinter MyDataGridViewPrinter;
        //local globals
        private Dictionary<List<string>, List<string>> names = new Dictionary<List<string>, List<string>>();
        private Dictionary<List<int>, List<double>> colcoeff = new Dictionary<List<int>, List<double>>();
        private List<string> varname = new List<string>();
        private List<string> colname = new List<string>();
        private List<int> colnum = new List<int>();
        private List<double> pCoeff = new List<double>();
        private DataGridView _dgv = null;
        private string _depvarname = string.Empty;

        private Dictionary<object, object> _corrResults = null;

        private ContextMenu _plotsMenu = new ContextMenu();

        private int _gridColNdx = -1;
        private int _gridRowNdx = -1;

        private DataTable _dtCopy = null;


        /// <summary>
        /// accessor that returns the table containg the best variables
        /// </summary>
        public DataTable PCDT
        {
            get { return _dtCopy; }
        }
       

        /// <summary>
        /// constructor needs the correlation coefficient scores calculated elsewhere and their
        /// associated information, column index, column name, transformname, etc...
        /// the dependent variable name and the datatable with all variables/transforms
        /// </summary>
        /// <param name="corrResults"></param>
        /// <param name="depvar"></param>
        /// <param name="dt"></param>
        public frmPcoeff(Dictionary<object, object> corrResults, string depvar, DataTable dt)
        {
            
            InitializeComponent();

            //default threshold
            numericUpDown1.Value = 20;
            _corrResults = corrResults;
            _depvarname = depvar;
            _dtCopy = dt;

            _plotsMenu.MenuItems.Add("View Plots", new EventHandler(viewPlotsEH));
        }


        /// <summary>
        /// method builds a table of variables, their transforms
        /// and their pearson scores for UI display in the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmPcoeff_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            lbDepVar.Text = _depvarname;

            //initialize a datatable
            DataTable dt = new DataTable();
            dt.Columns.Add("Variable", typeof(string));
            dt.Columns.Add("Transform", typeof(string));
            dt.Columns.Add("Pearson Coefficient", typeof(double));
            dt.Columns.Add("Correlation P-Value", typeof(double));

            //unpack the transform dictionaries into lists
            foreach (var pair in _corrResults)
            {
                names = (Dictionary<List<string>, List<string>>)pair.Key;
                foreach (var n in names)
                {
                    varname = n.Key;
                    colname = n.Value;
                }
                colcoeff = (Dictionary<List<int>, List<double>>)pair.Value;
                foreach (var l in colcoeff)
                {
                    colnum = l.Key;
                    pCoeff = l.Value;
                }
            }

            /*VBLogger.getLogger().logEvent("Building display....", VBLogger.messageIntent.UserOnly,
                VBLogger.targetSStrip.StatusStrip3);*/
            Application.DoEvents();

            //build the table for display and make the table readable
            string varx = varname[0];
            string colnm = varx;
            string transform = "none";
            for (int i = 0; i < varname.Count; i++)
            {
                //string message = "Variable: " + varname[i];
                //VBLogger.getLogger().logEvent(message, VBLogger.messageIntent.UserOnly,
                //    VBLogger.targetSStrip.StatusStrip2);
                //Application.DoEvents();

                if (varname[i] != varx)
                {
                    //insert a blank row between variable transform groups
                    dt.Rows.Add("", "", null);
                    varx = varname[i];
                    transform = "none";
                    colnm = varx;
                }
                else if (i != 0)
                {
                    transform = colname[i];
                    //colnm = "";
                    colnm = varx;
                }

                double pval = VBCommon.Statistics.Statistics.Pvalue4Correlation(pCoeff[i], _dtCopy.Rows.Count);
                dt.Rows.Add(colnm, transform, pCoeff[i], pval);
            }

            //VBLogger.getLogger().logEvent("Binding data to grid...", VBLogger.messageIntent.UserOnly,
            //    VBLogger.targetSStrip.StatusStrip3);
            //Application.DoEvents();

            //bind the datagridview to the table
            dataGridView1.DataSource = dt;

            //VBLogger.getLogger().logEvent("Binding done. Setting sort mode...", VBLogger.messageIntent.UserOnly,
            //    VBLogger.targetSStrip.StatusStrip3);
            //Application.DoEvents();

            dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;


            /*VBLogger.getLogger().logEvent("Setting cell attributes...", VBLogger.messageIntent.UserOnly,
                VBLogger.targetSStrip.StatusStrip3);*/
            Application.DoEvents();

            //make the grid pretty - bold variable with max Pcoeff and disable all other rows
            makeVarGrid(1.0d);

            Cursor.Current = Cursors.Default;
        }


        /// <summary>
        /// method provides UI clues for the selected variable/transform
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="dgv"></param>
        private void changeRowState(int rowIndex, DataGridView dgv)
        {
            if (dgv.Rows[rowIndex].ReadOnly)
            {
                dgv.Rows[rowIndex].ReadOnly = false;
                dgv.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Black;
            }
            else
            {
                dgv.Rows[rowIndex].ReadOnly = true;
                dgv.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Red;
            }
        }


        /// <summary>
        /// we're done, return but set table  by removing readonly rows and set column 
        /// properties for transformed columns
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            _dgv = dataGridView1;
            this.Hide();
            //this.Close();
            //this.Dispose();
            
            //cycle over the grid rows, match variable/transform with column name and delete columns with readonly rows
            for (int gridrow = 0; gridrow < _dgv.RowCount; gridrow++)
            {
                string colname = string.Empty;
                string varname = _dgv[0, gridrow].Value.ToString();
                if (varname == string.Empty) continue;  //go on if blank
                string tname = _dgv[1, gridrow].Value.ToString();
                if (tname == "none") colname = varname;
                else colname = tname;

                if (_dgv.Rows[gridrow].ReadOnly) //test if col is removable
                {
                    if (colname != varname)
                        _dtCopy.Columns.Remove(colname); //...it is if its not a ME
                }
                else
                {
                    if (colname != varname)
                    {
                        //if (!_dtCopy.Columns[varname].ExtendedProperties.Contains(VBCommon.Globals.DECOMPOSITION.ToString()))
                        //{
                        _dtCopy.Columns[varname].ExtendedProperties[VBCommon.Globals.ENABLED] = false;                            
                        //}
                        _dtCopy.Columns[colname].ExtendedProperties[VBCommon.Globals.TRANSFORM] = true;
                    }
                    else
                    {
                        _dtCopy.Columns[colname].ExtendedProperties[VBCommon.Globals.ENABLED] = true;
                    }                

                    if (!_dtCopy.Columns[colname].ExtendedProperties.Contains(VBCommon.Globals.MAINEFFECT.ToString()) && !(_dtCopy.Columns[colname].ExtendedProperties.Contains(VBCommon.Globals.DECOMPOSITION.ToString())))
                    {
                        _dtCopy.Columns[colname].ExtendedProperties[VBCommon.Globals.TRANSFORM] = true;
                    }

                    /*if (tname != "none")
                        _dtCopy.Columns[colname].ExtendedProperties[VBCommon.Globals.TRANSFORM] = true;
                    else
                        _dtCopy.Columns[colname].ExtendedProperties[VBCommon.Globals.HIDDEN] = true;*/
                }
                
                /*//mark transformed cols as transformed
                foreach (DataColumn c in _dtCopy.Columns)
                {
                    if (!c.ExtendedProperties.Contains(VBCommon.Globals.MAINEFFECT.ToString()) &&
                        !c.ExtendedProperties.Contains(VBCommon.Globals.DECOMPOSITION.ToString()))
                    {
                        c.ExtendedProperties[VBCommon.Globals.TRANSFORM] = true;
                        _dtCopy.Columns[varname].ExtendedProperties[VBCommon.Globals.HIDDEN] = true;
                    }
                }*/

            }
            _dtCopy.AcceptChanges();
            this.DialogResult = DialogResult.OK;            
        }


        /// <summary>
        /// handler responds to grid header mouse clicks, if left click users want to select this
        /// row's variable or one of its transforms - set the variable group grid properties
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                _gridColNdx = e.ColumnIndex;
                _gridRowNdx = e.RowIndex;
                return;
            }

            //maximum number of transforms (includes "none")
            const int NTransforms = 11;

            int selectedRow = e.RowIndex;
            string varname = dataGridView1[0, selectedRow].Value.ToString();
            changeRowState(selectedRow, (DataGridView)sender);
            dataGridView1.Rows[selectedRow].Selected = false;
            //if there's another row in this group enabled, find it and disable it
            //backup a max of NTransforms and check (will need maintenance if number of transforms changes)
            int startndx = selectedRow - NTransforms < 0 ? 0 : selectedRow - NTransforms;
            int endndx = Math.Min(startndx + (2 * NTransforms), dataGridView1.Rows.Count);
            //for (int r = startndx; r < startndx + 2*NTransforms; r++)
            for (int r = startndx; r < endndx; r++)
            {
                if (r == selectedRow) continue; //leave the one we just changed
                if (dataGridView1[0, r].Value.ToString() != varname) continue; //skip if we're out of the group
                if (dataGridView1.Rows[r].ReadOnly) continue; //skip if it's disabled
                else changeRowState(r, (DataGridView)sender); //found it - change it
                break; // go do something else
            }
            //there's probably a smarter way to do this with lists of groups or indices....
        }


        /// <summary>
        /// handler responds to mouse clicks; if right click, invoke the plot menu show handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) return;
            showPlotContexMenu((DataGridView)sender, e);
        }


        /// <summary>
        /// grid context menu for plots clicked, show the plot form
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="e"></param>
        private void showPlotContexMenu(DataGridView dgv, MouseEventArgs e)
        {
            //need something here to restrict showing contextmenu only if in dgv row header....
            int pos = dataGridView1.RowHeadersWidth;
            if (e.X > pos) return;

            //otherwise show the menu
            _plotsMenu.Show(dgv, new Point(e.X, e.Y));
        }


        /// <summary>
        /// viewplots menu item clicked handler - show the plot form
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void viewPlotsEH(object o, EventArgs e)
        {
            //show plots for the selected group of transforms in the grid rowlist
            if (_gridRowNdx < 0) return;
            string variable = dataGridView1[0, _gridRowNdx].Value.ToString();

            //Get a list of the transformations that we're plotting
            List<string> plotvarnames = new List<string>();
            var rowlist = dataGridView1.Rows.Cast<DataGridViewRow>().Where(row => row.Cells[0].Value.ToString() == variable.ToString());
            foreach (var item in rowlist)
            {
                plotvarnames.Add(dataGridView1[1, item.Index].Value.ToString());
            }

            if (variable == "" || variable == null) return;
            //all info should be in dtCopy.....
            //frmPCPlots frmPlots = new frmPCPlots(variable, _depvarname, _dtCopy, (DataTable)dataGridView1.DataSource);
            frmPCPlots frmPlots = new frmPCPlots(variable, plotvarnames, _depvarname, _dtCopy, (DataTable)dataGridView1.DataSource);
            frmPlots.ShowDialog();
        }


        private void btnThresholdSelect_Click(object sender, EventArgs e)
        {
            double threshold = 1.0d + (Convert.ToDouble(numericUpDown1.Value) / 100.0d);
            makeVarGrid(threshold);
        }


        private void makeVarGrid(double threshold)
        {
            Cursor.Current = Cursors.WaitCursor;
            resetGrid();
            //make the grid pretty - bold variable with max Pcoeff and disable all other rows
            Font dgvCellBold = new Font("Tahoma", 8, FontStyle.Bold);
            double maxpc = 0;
            int rowndx = 0;
            double testval = threshold;
            string message = string.Empty;

            /*VBLogger.getLogger().logEvent("Arranging transforms....", VBLogger.messageIntent.UserOnly,
                    VBLogger.targetSStrip.StatusStrip3);*/
            Application.DoEvents();

            for (int r = 0; r < dataGridView1.Rows.Count; r++)
            {
                //string message = dataGridView1.Rows[r].Cells[1].Value.ToString();
                //VBLogger.getLogger().logEvent(message, VBLogger.messageIntent.UserOnly,
                //    VBLogger.targetSStrip.StatusStrip2);
                //Application.DoEvents();

                //default row state is disabled
                changeRowState(r, dataGridView1);
                if (dataGridView1.Rows[r].Cells[1].Value.ToString() == string.Empty)
                {
                    //if here, means we're in new transform group
                    //dataGridView1.Rows[rowndx].Cells[2].Style.ForeColor = Color.Red;
                    dataGridView1.Rows[rowndx].Cells[2].Style.Font = dgvCellBold;
                    //switch row with max Pcoeff back on
                    changeRowState(rowndx, dataGridView1);
                    maxpc = 0;
                    testval = threshold;
                    continue;
                }
                //find the row within transform group that has max Pcoeff
                if (Math.Abs(maxpc) * testval < Math.Abs(Convert.ToDouble(dataGridView1.Rows[r].Cells[2].Value)))
                {
                    //if threshold set (>1) then we've found value > max*threshold
                    //reset testval to see if any are bigger than this value
                    if (threshold > 1.0d && maxpc != 0) testval = 1.0d;
                    maxpc = Convert.ToDouble(dataGridView1.Rows[r].Cells[2].Value);
                    rowndx = dataGridView1.Rows[r].Index;
                }

                if (Math.IEEERemainder(r, 100) == 0)
                {
                    message = Math.Round((((double)r / (double)dataGridView1.Rows.Count) * 100)).ToString();
                    /*VBLogger.getLogger().logEvent(message, VBLogger.messageIntent.UserOnly,
                        VBLogger.targetSStrip.ProgressBar);*/
                }
                Application.DoEvents();
            }
            //deal with the last group
            //dataGridView1.Rows[rowndx].Cells[2].Style.ForeColor = Color.Red;
            dataGridView1.Rows[rowndx].Cells[2].Style.Font = dgvCellBold;
            changeRowState(rowndx, dataGridView1);

            //VBLogger.getLogger().logEvent("0", VBLogger.messageIntent.UserOnly,
            //VBLogger.targetSStrip.ProgressBar);

            MyDataGridView = dataGridView1;  //for printing the grid

            Cursor.Current = Cursors.Default;
        }


        private void resetGrid()
        {
            string message = string.Empty;
            //dataGridView1.DataSource = null;
            Font dgvCellRegular = new Font("Tahoma", 8, FontStyle.Regular);
            for (int r = 0; r < dataGridView1.Rows.Count; r++)
            {
                dataGridView1.Rows[r].ReadOnly = false;
                dataGridView1.Rows[r].Cells[2].Style.Font = dgvCellRegular;
                dataGridView1.Rows[r].DefaultCellStyle.ForeColor = Color.Black;

                message = "Working row: " + r.ToString() + "/" + dataGridView1.Rows.Count;
                /*VBLogger.getLogger().logEvent(message, VBLogger.messageIntent.UserOnly,
                    VBLogger.targetSStrip.StatusStrip3);*/
                if (Math.IEEERemainder(r, 100) == 0)
                {
                    message = Math.Round ((((double)r / (double)dataGridView1.Rows.Count)*100)).ToString();
                    /*VBLogger.getLogger().logEvent(message, VBLogger.messageIntent.UserOnly,
                        VBLogger.targetSStrip.ProgressBar);*/
                }
                Application.DoEvents();
            }
            /*VBLogger.getLogger().logEvent("0", VBLogger.messageIntent.UserOnly,
                        VBLogger.targetSStrip.ProgressBar);*/
        }


        private void btnAuto_Click(object sender, EventArgs e)
        {
            makeVarGrid(1.0d);
        }


        public DataGridView DGV
        {
            set { _dgv = value; }
            get { return _dgv; }
        }


        private void frmPcoeff_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            /*string apppath = Application.StartupPath.ToString();
            VBCSHelp help = new VBCSHelp(apppath, sender);
            if (!help.Status)
            {
                MessageBox.Show(
                "User documentation is found in the Documentation folder where \nyou installed Virtual Beach ",
                "Neither Adobe Acrobat nor Adobe Reader found.",
                MessageBoxButtons.OK);
            }*/
        }


        #region region - print methods
        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (SetupThePrinting())
                MyPrintDocument.Print();
        }


        // The PrintPage action for the PrintDocument control
        private void MyPrintDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            bool more = MyDataGridViewPrinter.DrawDataGridView(e.Graphics);
            if (more == true)
                e.HasMorePages = true;
        }


        // The printing setup function
        private bool SetupThePrinting()
        {
            PrintDialog MyPrintDialog = new PrintDialog();
            MyPrintDialog.AllowCurrentPage = false;
            MyPrintDialog.AllowPrintToFile = false;
            MyPrintDialog.AllowSelection = false;
            MyPrintDialog.AllowSomePages = false;
            MyPrintDialog.PrintToFile = false;
            MyPrintDialog.ShowHelp = false;
            MyPrintDialog.ShowNetwork = false;

            if (MyPrintDialog.ShowDialog() != DialogResult.OK)
                return false;

            MyPrintDocument.DocumentName = "Pearson Correlation Results";
            MyPrintDocument.PrinterSettings = MyPrintDialog.PrinterSettings;
            MyPrintDocument.DefaultPageSettings = MyPrintDialog.PrinterSettings.DefaultPageSettings;
            MyPrintDocument.DefaultPageSettings.Margins = new Margins(40, 40, 40, 40);

            if (MessageBox.Show("Do you want the report to be centered on the page", "Pearson Correlation Results - Center on Page", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                MyDataGridViewPrinter = new DataGridViewPrinter(MyDataGridView, MyPrintDocument, true, true, "Pearson Correlation Results\nDependent Variable: " + _depvarname, new Font("Tahoma", 18, FontStyle.Bold, GraphicsUnit.Point), Color.Black, true);
            else
                MyDataGridViewPrinter = new DataGridViewPrinter(MyDataGridView, MyPrintDocument, false, true, "Pearson Correlation Results\nDependent Variable: " + _depvarname, new Font("Tahoma", 18, FontStyle.Bold, GraphicsUnit.Point), Color.Black, true);

            return true;
        }


        // The Print Preview Button
        private void btnPrintPreview_Click(object sender, EventArgs e)
        {
            if (SetupThePrinting())
            {
                PrintPreviewDialog MyPrintPreviewDialog = new PrintPreviewDialog();
                MyPrintPreviewDialog.Document = MyPrintDocument;
                MyPrintPreviewDialog.ShowDialog();
            }
        }
        #endregion


        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }



}
