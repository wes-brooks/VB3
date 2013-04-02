using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using MLRCore;
using MultipleLinearRegression;
using GALib;
using VBCommon;
using System.IO;
using VBCommon.Statistics;


namespace GALibForm
{
    public partial class frmModelingReport : Form
    {

        public static List<IIndividual> _mlist;
        private static string _iv = string.Empty;

        //StringReader _Reader = null;
        TextReader _Reader = null;

        private DataTable _dt = null;
        private MLRDataManager _dataMgr = null;

        //reusable report header
        private string _rptHearder = string.Empty;

        //some semi-usefull lines and format strings
        private string _line = string.Empty;
        private string _blank = "";
        private string _delimiter = "***********************************************************************************************************";
        private string _numfmt = "0.####e-00";
        private string _numfmt2 = "{0:00.####e-00}";
        private string _tab = "\t";

        //format string for model data
        private string _fs = "{0:0.000#e-00\t} {1:0.000#e-00\t} {2:0.000#e-00\t} {3:0.000#e-00}";

        //format string for model parameter statistics
        private string _fs2 = "{0:0.000#e-00\t} {1:0.000#e-00\t} {2:0.000#e-00\t} {3:0.000#e-00\t} {4:0.000#e-00\t} {5:0}\n";
        private string _fs3 = "{0:0.000#e-00\t} {1:0.000#e-00\t} {2:0.000#e-00\t\t} {3:0.000#e-00}";

        //this may be trouble - where is notepad universally?
        private string _notepad = @"c:\windows\system32\notepad.exe";

        public frmModelingReport()
        {
            InitializeComponent();
        }


        public void addList (string[] list, List<IIndividual> models)
        {
            //get a copy of the models and listbox list
            listBox1.DataSource = list;
            _mlist = models;
        }

        public void addHeader()
        {
            //generates some header info, adds it to rtb 

            _dataMgr = MLRDataManager.GetDataManager();
            //_dt = _dataMgr.CorrelationDataTable;
            _dt = _dataMgr.ModelDataTable;

            addLine("MLR Model Building Report");
            addLine(_blank);
            string site = string.Empty;
            string projectfile = string.Empty;
            //Do we need site name
            //if (_projMgr.SiteInfo != null)
            //{
            //    site = _projMgr.SiteInfo.Name;
            //    projectfile = _projMgr.Name;
            //}
            
            //addLine("VB3 Project Name: " + site);

            //addLine("VB3 Project File: " + projectfile);
            addLine(_blank);

            _line = "Independent Variable: " + MLRDataManager.GetDataManager().ModelDependentVariable;
            addLine(_line);
            _line = "Number of observations: " + _dt.Rows.Count;
            addLine(_line);
            addLine(_blank);
            addLine("Models are listed in order of best-fit based upon selected evaluation criterion.");
        }

        public void addModelEvalCriterion(string text)
        {
            //add the last header line

            addLine(text);
            //save the header for regeneration after user selects model(s)
            _rptHearder = richTextBox1.Text;
        }

        private void addLine(string text)
        {
            richTextBox1.Text = richTextBox1.Text + text + "\n";
        }

        private void addModel (MLRIndividual model)
        {
            //extract and display model info

            addLine(_delimiter);
            addModelExpression(model);
            addLine("Model Evaluation Score: " + model.Fitness.ToString(_numfmt) + "\n");
            addScores(model);
            
            foreach (DataRow r in model.Parameters.Rows)
            {
                addLine("\t- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
                //model info
                addModelRow(r);
                //model parameter stats
                addParameterStats(r[0].ToString());
            }

        }

        private void addParameterStats(string varname)
        {
            // get and display some statistics for the data column

            //string varname = r[0].ToString();
            if (varname == "(Intercept)") return;
            _iv = varname;
            DataRow[] dr = new DataRow[_dt.Rows.Count];
            _dt.Rows.CopyTo(dr, 0);
            double[] data = Array.ConvertAll(dr, new Converter<DataRow, Double>(DataRowToDouble));

            int zeroCount = data.Count(n => n == 0);

            DescriptiveStats ds = new DescriptiveStats();
            ds.getStats(data);
            addLine("Descriptive Statistics for " + _iv + ": ");
            _line = "Maximum\tMinimum\tDataRange\tMeanValue\tSum\t\tZeroCount";
            addLine(_line);
            _line = string.Format(_fs2, ds.Max, ds.Min, ds.Range, ds.Mean, ds.Sum, zeroCount);
            addLine(_line);
        }

        private static double DataRowToDouble(DataRow r)
        {
            return Convert.ToDouble(r[_iv].ToString());
        }

        private void addModelExpression(MLRIndividual model)
        {
            //generates the model expression, adds it to form rtb

            string opsign = string.Empty;
            string parameter = string.Empty;
            double coeff = double.NaN;

            _line = "Model: " + _dt.Columns[1].ColumnName.ToString() + " = ";

            foreach (DataRow r in model.Parameters.Rows)
            {
                parameter = r[0].ToString();
                coeff = (double)r[1];
                if (coeff < 0) opsign = " - ";
                else opsign = " + ";
                if (parameter == "(Intercept)") 
                    _line = _line + string.Format(_numfmt2, coeff);
                else
                _line = _line + opsign + string.Format(_numfmt2, Math.Abs(coeff)) + 
                    "*" + parameter.ToString();
            }
            addLine(_line + "\n");
        }

        private void addScores(MLRIndividual model)
        {
            //generates list of model evaluation metrics, adds it to rtb

            addLine("All Evaluation Metrics: ");
            _line = _tab + "R Squared: " + "\t\t" + model.R2.ToString(_numfmt) + "\n\t";
            _line = _line + "Adjusted R Squared: " + "\t" + model.AdjustedR2.ToString(_numfmt) + "\n\t";
            _line = _line + "Akaike Info Criterion: " + "\t" + model.AIC.ToString(_numfmt) + "\n\t";
            _line = _line + "Corrected AIC: " + "\t\t" + model.AICC.ToString(_numfmt) + "\n\t";
            _line = _line + "Bayesian Info Criterion: " + "\t" + model.BIC.ToString(_numfmt) + "\n\t";
            _line = _line + "PRESS: " + "\t\t" + model.Press.ToString(_numfmt) + "\n\t";
            _line = _line + "RMSE: " + "\t\t\t" + model.RMSE.ToString(_numfmt) + "\n\t";
            if (frmModel.ThresholdChecked)
            {
                _line = _line + "Sensitivity: " + "\t\t" + model.Sensitivity.ToString(_numfmt) + "\n\t";
                _line = _line + "Specificity: " + "\t\t" + model.Specificity.ToString(_numfmt) + "\n\t";
                _line = _line + "Accuracy: " + "\t\t" + model.Accuracy.ToString(_numfmt) + "\n";
            }
            addLine(_line);
        }

        private void addModelRow(DataRow r)
        {
            //extracts statistics for each model parameter, adds it to rtb

            _line = string.Format("Parameter Name: {0}", r[0]);
            addLine(_line);
            _line = "Coefficient\tStandard Error\tt-Statistic\tP-Value";
            addLine(_line);
            _line = string.Format(_fs, r[1], r[2], r[3], r[4]);
            addLine(_line + "\n");
            //_line = string.Format(_rtbfs, r[1], r[2], r[3], r[4]);
            //addLine(_line);
            
            //if its a polynomial model term, add the regression info
            if (r[0].ToString().Contains("POLY[")) rptPolyTerms(r[0].ToString());//reportPolyInfo(r[0].ToString());
        }

        private void reportPolyInfo(string colname)
        {
            string dummy = string.Empty;
            string teststring = string.Empty;

            string poly = colname.Substring(colname.IndexOf("POLY"));
            addLine("Polynomial Regression Information for " + poly);
            
            //addLine("Intercept\tCoefficientOnX\tCoefficientOnX**2\tAdjustedR**2");
            addLine("Intercept\tCoefficientOnX\tCoefficientOnX**2");
            try
            {
                dummy = colname.Substring(0, colname.IndexOf("]"));
                string[] p = dummy.Split(',');
                _line = p[1] + _tab + p[2] + _tab + p[3];
            }
            catch
            {
                _line = "Cannot parse model term - view the model term to see the regression parameters.";
            }
            addLine(_line + "\n");

            //another poly term in model term?
            //if (teststring.Contains("POLY[")) reportPolyInfo(teststring.Substring(teststring.IndexOf("POLY")));

        }

        private void rptPolyTerms(string modelterm)
        {

            Regex polyPat = new Regex(@"(POLY\[.*\][^\]])|POLY\[.*\]");
            MatchCollection matches = polyPat.Matches(modelterm);
            foreach (Match m in matches)
            {
                reportPolyInfo(m.ToString());
            }
           
        }

        protected void btnPrint_Click(object sender, System.EventArgs e)
        {
            richTextBox1.SelectAll();  
            printDialog1.Document = printDocument1;
            
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                this.printDocument1.Print();
               
            }
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs ev)
        {
            float linesPerPage = 0;
            float yPosition = 0;
            int count = 0;
            float leftMargin = ev.MarginBounds.Left;
            float topMargin = ev.MarginBounds.Top;
            string line = null;
            Font printFont = this.richTextBox1.Font;
            SolidBrush myBrush = new SolidBrush(Color.Black);
            // Work out the number of lines per page, using the MarginBounds.
            linesPerPage = ev.MarginBounds.Height / printFont.GetHeight(ev.Graphics);
            // Iterate over the string using the StringReader, printing each line.
            while (count < linesPerPage && ((line = _Reader.ReadLine()) != null))
            {
                // calculate the next line position based on the height of the font according to the printing device
                yPosition = topMargin + (count * printFont.GetHeight(ev.Graphics));
                // draw the next line in the rich edit control
                ev.Graphics.DrawString(line, printFont, myBrush, leftMargin, yPosition, new StringFormat());
                count++;
            }
            // If there are more lines, print another page.
            if (line != null)
                ev.HasMorePages = true;
            else
                ev.HasMorePages = false;
            myBrush.Dispose();
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //save report to file and optionally open in notepad

            saveFileDialog1.DefaultExt = "*.txt";
            saveFileDialog1.Filter = "Text Files|*.txt";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.SaveFile(saveFileDialog1.FileName, RichTextBoxStreamType.PlainText);
                DialogResult yn = MessageBox.Show("Open saved file in NotePad?", "File Open",
                    MessageBoxButtons.YesNo);
                if (yn == DialogResult.Yes)
                {
                    Process.Start(_notepad, saveFileDialog1.FileName);
                }
              
            }
            
        }

        private void btnCopyClip_Click(object sender, EventArgs e)
        {
            //copy report to clipboard and optionally paste into notepad

            richTextBox1.SelectAll();
            richTextBox1.Copy();

            
            DialogResult yn = MessageBox.Show("Report copied to clipboard; paste to NotePad?",
                "You can paste to your favorite text editor or word processor.",
                MessageBoxButtons.YesNo);
            if (yn == DialogResult.Yes)
            {
                ProcessStartInfo psi = new ProcessStartInfo(_notepad);
                psi.RedirectStandardInput = true;
                psi.UseShellExecute = false;
                Process np = new Process();
                np.StartInfo = psi;
                np.Start();
                np.WaitForInputIdle();

                //redirecting IO to windows app doesn't work
                //np.StandardInput.Write(richTextBox1.Text);
                //np.StandardInput.Flush();

                //found this code online; uses the dll import routines below - seems to work ok        
                //  Give focus to the Notepad window
                SetFocus(FindWindow("Notepad", "Untitled - Notepad"));
                //
                //  Send Ctl-V to paste clipboard contents
                const byte VK_CONTROL = 0x11;       // CTRL key
                const uint KEYEVENTF_EXTENDEDKEY = 1;
                const uint KEYEVENTF_KEYUP = 2;
                keybd_event(VK_CONTROL, 0, KEYEVENTF_EXTENDEDKEY, UIntPtr.Zero);
                keybd_event((byte)'V', 0, 0, UIntPtr.Zero);
                keybd_event((byte)'V', 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                keybd_event(VK_CONTROL, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, UIntPtr.Zero);
                
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //generates report for selected model in list

            richTextBox1.Clear();
            addLine(_rptHearder);
            //foreach (ListItem li in listBox1.SelectedItems)
            foreach (int ndx in listBox1.SelectedIndices)
            {
                MLRIndividual model = (MLRIndividual) _mlist[ndx];
                addModel(model);
            }
            richTextBox1.Refresh();
        }

        [DllImport("user32.dll", EntryPoint = "SetFocus")]
        public static extern int SetFocus(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        private void button2_Click(object sender, EventArgs e)
        {
            Form frmGraphs = new frmEvalPlots(_mlist, _dt);
            frmGraphs.ShowDialog();
        }

        private void frmModelingReport_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string apppath = Application.StartupPath.ToString();
            VBCSHelp help = new VBCSHelp(apppath, sender);
            if (!help.Status)
            {
                MessageBox.Show(
                "User documentation is found in the Documentation folder where you installed Virtual Beach"
                + "\nIf your web browser is PDF-enabled, open the User Guide with your browser.",
                "Neither Adobe Acrobat nor Adobe Reader found.",
                MessageBoxButtons.OK);
            }
        }

    }
}
