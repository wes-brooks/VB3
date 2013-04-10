using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Accord;
using Accord.Math;
using Accord.Statistics;

//using VBTools;

using ZedGraph;
//using Extreme.Statistics;
//using Extreme.Statistics.Tests;
//using VBStatistics;

namespace VBCommon.Controls
{
    /// <summary>
    /// class to show plots of a variable and all its possible transforms.
    /// user-selected plot types are frequency plot, xy scatter plot, and timeseries.
    /// a varying number of plots, corresponding to the number of possible data transforms 
    /// for the selected variable, are displayed in a zedgraph master control
    /// </summary>
    public partial class frmPCPlots : Form
    {
        private DataTable _dtCopy = null;
        private string _varName = string.Empty;
        private string _depVarName = string.Empty;
        private Dictionary<string, double[]> _plotData = null;
        private DataTable _corrResults = null;
        private Dictionary<string,double> _pValueDict = null;

        bool _init = true;

        /// <summary>
        /// class constructor expects the variable name and table containing the plot data.
        /// it also uses the response variable name to get at the x-axis data for the xy plot
        /// and the correlation results table for listing the pearson scores
        /// </summary>
        /// <param name="varName">variable name</param>
        /// <param name="depVarName">response variable name</param>
        /// <param name="dt">table with plot data</param>
        /// <param name="corrResults">table with pearson scores</param>
        public frmPCPlots(string varName, string depVarName, DataTable dt, DataTable corrResults)
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Maximized;

            this.Text = "Variable " + varName + " and its Transforms";

            _dtCopy = dt.Copy();
            _varName = varName;
            _depVarName = depVarName;
            _corrResults = corrResults;

            string[] plotTypes = new string[] { "Scatter Plots", "Time Series Plots", "Frequency Plots" };
            cbSelectPlot.DataSource = plotTypes;
            cbSelectPlot.SelectedIndex = 0;

            renderPlots();
        }


        /// <summary>
        /// method extracts data for plotting the timeseries plot for the named variable and its transforms
        /// and produces the plots for the zg master control to display and generates/shows the stats data
        /// in the listbox
        /// </summary>
        private void renderPlots()
        {
            //List<object> plotdata = new List<object>();
            _plotData = new Dictionary<string,double[]>();
            double[] values = new double[_dtCopy.Rows.Count];
            string varname = string.Empty;

            int colndx = _dtCopy.Columns.IndexOf(_varName);
            values = getValues<double>(colndx, _dtCopy);
            _plotData.Add(_varName, values);
            foreach (DataColumn dc in _dtCopy.Columns)
            {
                if (!dc.ColumnName.Contains(_varName)) continue;
                //otherwise we've got plotting data
                colndx = _dtCopy.Columns.IndexOf(dc);
                varname = dc.ColumnName.ToString();
                values = getValues<double>(colndx, _dtCopy);
                if (!_plotData.ContainsKey(varname))
                    _plotData.Add(varname, values);
            }

            if (_plotData.Keys.Count < 1) return; //go away if no data

            MasterPane master = zgc1.MasterPane;
            master.PaneList.Clear();

            double[] depvals = getValues<double>(_dtCopy.Columns.IndexOf(_depVarName), _dtCopy);
            string[] tags = getValues<string>(0, _dtCopy);
            _pValueDict = new Dictionary<string, double>();
            foreach (KeyValuePair<string, double[]> kv in _plotData)
            {
                GraphPane gp = addPlotXY(depvals, kv.Value, tags, kv.Key);
                master.Add(gp);
            }

            using (Graphics g = this.CreateGraphics())
            { master.SetLayout(g, PaneLayout.SquareColPreferred); }

            zgc1.IsShowPointValues = true;
            zgc1.AxisChange();
            master.AxisChange();
            zgc1.Refresh();

            _init = false;

            showListInfo();
        }


        /// <summary>
        /// lists Anderson-Darling stats and Pearson Correlation score for each variable and its transform
        /// </summary>
        private void showListInfo()
        {
            //lv1.Items.Clear();
            lv1.Columns.Clear();
            lv1.Items.Clear();
            lv1.View = View.Details;

            lv1.Columns.Add("", -1, HorizontalAlignment.Left);

            ListViewItem lvPC;
            lvPC = new ListViewItem("Pearson Coefficient");
            //lvi.SubItems.Add(kurtosis.ToString("n3"));
            lv1.Items.Add(lvPC);

            ListViewItem lvPV;
            lvPV = new ListViewItem("Correlation P-Value");
            lv1.Items.Add(lvPV);

            ListViewItem lvAD;
            lvAD = new ListViewItem("Anderson-Darling Normality");
            //lvi.SubItems.Add(skewness.ToString("n3"));
            lv1.Items.Add(lvAD);

            string t;
            //ListViewItem lvi;
            foreach (KeyValuePair<string, double[]> kv in _plotData)
            {
                if (kv.Key.ToString().Contains(']'))
                    t = kv.Key.Substring(0, kv.Key.IndexOf('['));
                else 
                    t = kv.Key.ToString();

                lv1.Columns.Add(t, -2, HorizontalAlignment.Left);
                //get the pearson coefficient and add to list
                double pc = getCorrResults(kv.Key.ToString());
                lvPC.SubItems.Add(pc.ToString("n4"));

                string pv = getPVal(kv.Key.ToString());
                lvPV.SubItems.Add(pv);

                //get the ad stat and add to list
                double ad = getADstat(kv.Key.ToString());
                lvAD.SubItems.Add(ad.ToString("n4"));

            }
            //should probably figure some way to signify best and/or selected stats in table here...
            //List<double> lad = lvAD.SubItems.Cast<double>().ToList<double>();
            //List<double> lpc = lvPC.SubItems.Cast<double>().ToList<double>();                      
        }


        /// <summary>
        /// calculate the Anderson-Darling normality statistic for the datatable column of data
        /// </summary>
        /// <param name="t">name of the column</param>
        /// <returns>the AD statistic</returns>
        private double getADstat(string t)
        {
            double retval = 0.0d;
            VBCommon.Statistics.AndersonDarlingNormality adtest = new VBCommon.Statistics.AndersonDarlingNormality();
            double[] vals = (from DataRow r in _dtCopy.Rows select (double)r[t]).ToArray<double>();
            adtest.getADstat(vals);
            if (!adtest.ADStat.Equals(double.NaN)) retval = adtest.ADStat;
            return retval;
        }


        /// <summary>
        /// extracts the previously calculated Pearson Correlation Coefficient statistic of the datatable column
        /// (the PC coefficient score is contained in the global correlation table _corrResults)
        /// </summary>
        /// <param name="t">table column name</param>
        /// <returns>the PC score</returns>
        private double getCorrResults(string t)
        {
            double retval = 0.0d;
            for (int r = 0; r < _corrResults.Rows.Count; r++)
            {
                string varname = _corrResults.Rows[r]["Variable"].ToString();
                string tn = _corrResults.Rows[r]["Transform"].ToString();
                if (t != tn && t!= varname) continue;
                retval = Convert.ToDouble(_corrResults.Rows[r]["Pearson Coefficient"].ToString());
                break;
            }
            return retval;
        }


        private string getPVal(string key)
        {
            string retval = string.Empty;
            if (_pValueDict != null)
            {
                if (_pValueDict.ContainsKey(key))
                {
                    double pval = _pValueDict[key];
                    string fmtstring = formatNumberString(pval, 0);
                    retval = string.Format(fmtstring, pval);
                }
            }
            return retval;
        }


        /// <summary>
        /// extract data from a datatable column into a typed array of values
        /// </summary>
        /// <typeparam name="T">type of data in column (should only be of type string or double in this table)</typeparam>
        /// <param name="colndx">datatable column index of the data to extract</param>
        /// <param name="_dt">datatable to operate on</param>
        /// <returns>a typed array of values</returns>
        private T[] getValues<T>(int colndx, DataTable _dt)
        {
            //try
            //{
            //    var values = (from row in _dt.Select()
            //                  select row.Field<T>(colndx)).ToArray<T>();
            //    return values;
            //}

            //catch (InvalidCastException)
            //{
            //    //var values = (from row in _dt.Select()
            //    //              select row[colndx]).Cast<string>().ToArray();
            //    //return values.Cast<T>().ToArray();
            //    var values = (from row in _dt.Select() select row.Field<T>(colndx)).ToArray<T>;
            //    return values;
            //}

            string t = typeof(T).ToString();
            switch (t)
            {
                case "System.Double":
                    var dvalues = (from row in _dt.Select()
                                   select row.Field<T>(colndx)).ToArray<T>();
                    return dvalues;
                case "System.String":
                    //var svalues = (from row in _dt.Select()
                    //          select row[colndx]).Cast<string>().ToArray();
                    //return svalues.Cast<T>().ToArray();
                    string[] svalues = new string[_dt.Rows.Count];
                    for (int r = 0; r < _dt.Rows.Count; r++)
                    {
                        svalues[r] = _dt.Rows[r][colndx].ToString();
                    }
                    return svalues.Cast<T>().ToArray();
                default:
                    return null;
            }
        }


        /// <summary>
        /// generate a zedgraph timeseries plot graphpane
        /// </summary>
        /// <param name="iv"></param>
        /// <param name="tags"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private GraphPane addPlotTS(double[] iv, string[] tags, string title)
        {
            DateTime date;
            bool dateAxis = false;
            if (IsDate(tags[0], out date)) dateAxis = true;

            PointPairList ppl = new PointPairList();
            string tag = string.Empty;
            //might be a smarter way to add tags to points but I can't find it.
            for (int i = 0; i < iv.Length; i++)
            {
                //tag = "(" + iv[i].ToString("n2") + " , " + tags[i] + ") ";
                tag = tags[i];
                if (dateAxis)
                {

                    DateTime d = Convert.ToDateTime(tags[i]);
                    ppl.Add((XDate)d, iv[i], tag);
                }
                else
                {
                    double n = Convert.ToDouble(tags[i]);
                    ppl.Add(n, iv[i], tag);
                }
            }
            GraphPane gp = new GraphPane();
            LineItem curve = gp.AddCurve("", ppl, Color.Blue);
            curve.Line.IsVisible = true;
            if (dateAxis) gp.XAxis.Type = AxisType.Date;
            else gp.XAxis.Type = AxisType.Linear;
            //gp.XAxis.Title.Text = _dt.Columns[0].ColumnName;
            gp.YAxis.Title.Text = title;

            gp.Tag = "TSPlot";
            //gp.Title.Text = "Time Series Plot";
            gp.Title.Text = title;

            return gp;
        }


        /// <summary>
        /// generate a zedgraph frequency plot graphpane
        /// </summary>
        /// <param name="iv"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private GraphPane addPlotFQ(double[] iv, string title)
        {
            //ncats is the number of categories
            const int ncats = 9;

            VBCommon.Statistics.Frequency counts = new VBCommon.Statistics.Frequency();

            counts.getHist(iv, ncats);
            double[] c = counts.Counts;
            double[] center = counts.Center;

            GraphPane gp = new GraphPane();
            PointPairList ppl = new PointPairList();
            ppl.Add(center, c);
            BarItem bar = gp.AddBar("", ppl, Color.Black);
            //gp.XAxis.Title.Text = _dt.Columns[_selectedcol].ColumnName;
            gp.YAxis.Title.Text = "Count";

            gp.Title.Text = title;

            return gp;
        }


        /// <summary>
        /// generate an xy-plot zedgraph graphpane
        /// </summary>
        /// <param name="response"></param>
        /// <param name="iv"></param>
        /// <param name="tags"></param>
        /// <param name="title"></param>
        /// <returns>a zedgraphpane</returns>
        private GraphPane addPlotXY(double[] response, double[] iv, string[] tags, string title)
        {
            PointPairList ppl = new PointPairList();
            string tag = string.Empty;

            GraphPane gp = new GraphPane();

            if (_dtCopy.Rows.Count > 6)
            {
                //mc wants a regression line and some new stats on the plot
                double pcoeff = response.Covariance(iv) / response.StandardDeviation() / iv.StandardDeviation(); //Statistics.Correlation(response, iv);
                double pval = VBCommon.Statistics.Statistics.Pvalue4Correlation(pcoeff, _dtCopy.Rows.Count);
                _pValueDict.Add(title,pval);

                string[] ivname = new string[] { _dtCopy.Columns[title].Caption };
                VBCommon.Statistics.MultipleRegression mlr = new VBCommon.Statistics.MultipleRegression(_dtCopy, _dtCopy.Columns[_depVarName].Caption, ivname);
                mlr.Compute();

                PointPairList ppl2 = new PointPairList(iv, mlr.PredictedValues);
                string fmtstring = formatNumberString(pval, 1);
                fmtstring = "r = {0:f4}, P-Value = " + fmtstring;
                string annot = string.Format(fmtstring, pcoeff, pval);
                //string annot = string.Format("r = {0:f4}, p-value = {1:e4}", pcoeff, pval);
                LineItem curve2 = gp.AddCurve(annot, ppl2, Color.Red);
                curve2.Symbol.IsVisible = false;
                curve2.Line.IsVisible = true;
            }

            for (int i = 0; i < response.Length; i++)
            {
                tag = "(" + iv[i].ToString("n2") + " , " + response[i].ToString("n2") + ") " + tags[i];
                ppl.Add(iv[i], response[i], tag);
            }
            //GraphPane gp = new GraphPane();
            LineItem curve = gp.AddCurve("", ppl, Color.Black);
            curve.Line.IsVisible = false;
            gp.XAxis.Title.Text = title;
            gp.YAxis.Title.Text = _depVarName;

            gp.Tag = "XYPlot";
            gp.Title.Text = title;

            return gp;
        }


        private string formatNumberString(double number, int argnum)
        {
            string fmtstring = string.Empty;
            if (number.Equals(double.NaN)) return fmtstring;
            if (argnum == 1)
            {
                if (Math.Abs(number) >= 10000 || Math.Abs(number) <= 0.0001)
                    fmtstring = "{1:e4}";
                else
                    fmtstring = "{1:f4}";
            }
            else
            {
                if (Math.Abs(number) >= 10000 || Math.Abs(number) <= 0.0001)
                    fmtstring = "{0:e4}";
                else
                    fmtstring = "{0:f4}";
            }
 
            //return string.Format(fmtstring, number);
            return fmtstring;
        }


        /// <summary>
        /// determine if the x-axis variable (of the ts-plot) is a date
        /// </summary>
        /// <param name="anyString">x-variable value</param>
        /// <param name="resultDate">a date-time data variable</param>
        /// <returns>true if x variable value parses to a date, false otherwise</returns>
        private bool IsDate(string anyString, out DateTime resultDate)
        {
            bool isDate = true;

            if (anyString == null)
            {
                anyString = "";
            }
            try
            {
                resultDate = DateTime.Parse(anyString);
            }
            catch
            {
                resultDate = DateTime.MinValue;
                isDate = false;
            }

            return isDate;
        }


        /// <summary>
        /// combobox selection of plot type maintneance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbSelectPlot_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_init) return;
            redoPlots(cbSelectPlot.SelectedItem.ToString());
        }


        /// <summary>
        /// another plot type was selected, generate the plot for the variable and its transforms
        /// and pass them to the zg master control for display
        /// </summary>
        /// <param name="p"></param>
        private void redoPlots(string p)
        {
            MasterPane master = zgc1.MasterPane;
            master.PaneList.Clear();

            string[] tags = getValues<string>(0, _dtCopy);
            double[] depvals = getValues<double>(_dtCopy.Columns.IndexOf(_depVarName), _dtCopy);

            switch (p.ToString())
            {
                case "Time Series Plots":
                    
                    foreach (KeyValuePair<string, double[]> kv in _plotData)
                    {
                        GraphPane gp = addPlotTS(kv.Value, tags, kv.Key);
                        master.Add(gp);
                    }
                    break;

                case "Frequency Plots":
                    foreach (KeyValuePair<string, double[]> kv in _plotData)
                    {
                        GraphPane gp = addPlotFQ(kv.Value, kv.Key);
                        master.Add(gp);
                    }
                    break;

                case "Scatter Plots":
                    _pValueDict = new Dictionary<string, double>();
                    foreach (KeyValuePair<string, double[]> kv in _plotData)
                    {
                        GraphPane gp = addPlotXY(depvals, kv.Value, tags, kv.Key);
                        master.Add(gp);
                    }
                    //showListInfo();
                    break;
            }

            using (Graphics g = this.CreateGraphics())
            { master.SetLayout(g, PaneLayout.SquareColPreferred); }

            zgc1.IsShowPointValues = true;
            zgc1.AxisChange();
            master.AxisChange();
            zgc1.Refresh();
        }


        private void frmPCPlots_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            /*string apppath = Application.StartupPath.ToString();
            VBCSHelp help = new VBCSHelp(apppath, sender);
            if (!help.Status)
            {
                MessageBox.Show(
                "User documentation is found in the Documentation folder where you installed Virtual Beach"
                + "\nIf your web browser is PDF-enabled, open the User Guide with your browser.",
                "Neither Adobe Acrobat nor Adobe Reader found.",
                MessageBoxButtons.OK);
            }*/
        }
    }
}
