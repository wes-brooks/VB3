using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VBStatistics;
using VBCommon;
using VBCommon.Transforms;
using ZedGraph;

namespace VBControls
{
    public partial class MLRPlots : UserControl
    {
        private List<double[]> _XYPlotdata;
        //Threshold value used for sensitiviy, specificity, accuracy
        double _decisionThreshold;
        double _mandateThreshold;
        double _probabilityThreshold = 50;
        double _powerExp = double.NaN;
        double _rmse = double.NaN;
        DependentVariableTransforms _depVarXFrm = DependentVariableTransforms.none;
        double _pwrXFrmExponent = double.NaN;

        string _sdecisionThreshold;
        string _smandateThreshold;
        string _sprobabilityThreshold;

        private string[] _rptlist = new string[]{
                "Plot: Pred vs Obs",
                "Error Table: CFU as DC",
                "Plot: % Exc vs Obs",
                "Error Table: % Exc as DC"
                };
        double[] _obs;
        double[] _pred;
        double[] _prob;
        string[] _tags;

        string _sdc = "Decision Criterion: ";

        public enum Exceedance { model, prediction};
        private Exceedance _exceedance;
                

        public MLRPlots()
        {
            InitializeComponent();

            //cboxPlotList.DataSource = _rptlist;
            _decisionThreshold = Convert.ToDouble(tbThresholdDec.Text);
            _mandateThreshold = Convert.ToDouble(tbThresholdReg .Text);
            _sprobabilityThreshold = "50";
            _sdecisionThreshold = tbThresholdDec.Text;
            _smandateThreshold = tbThresholdReg.Text;
            cboxPlotList.DataSource = _rptlist;

            InitResultsGraph();
        }

        public double[] Exceedances
        {
            set { _prob = value; }
            get { return _prob; }
        }

        public double ThresholdHoriz
        {
            get { return _decisionThreshold; }
        }

        public double ThresholdVert
        {
            get { return _mandateThreshold; }
        }

        public ZedGraphControl ZGC
        {
            get { return this.zgc; }
        }

        public ListView LISTVIEW
        {
            get { return this.listView1; }
        }

        public DependentVariableTransforms DependentVarXFrm
        {
            set { _depVarXFrm = value; }
            get { return _depVarXFrm; }
        }

        public double PowerTransformExponent
        {
            get { return _pwrXFrmExponent; }
            set { _pwrXFrmExponent = value; }
        }

        public void SetThresholds(double decisionThreshold, double mandateThreshold)
        {
            _decisionThreshold = decisionThreshold;
            _mandateThreshold = mandateThreshold;
            tbThresholdDec.Text = _decisionThreshold.ToString();
            tbThresholdReg.Text = _mandateThreshold.ToString();
            //_sprobabilityThreshold = "50";
            //_sdecisionThreshold = tbThresholdDec.Text;
            //_smandateThreshold = tbThresholdReg.Text;    
        }

        public void SetThresholds(string decisionThreshold, string mandateThreshold)
        {
            tbThresholdDec.Text = decisionThreshold;
            tbThresholdReg.Text = mandateThreshold;
            //_sprobabilityThreshold = "50";
            //_sdecisionThreshold = tbThresholdDec.Text;
            //_smandateThreshold = tbThresholdReg.Text;
        }

        public VBCommon.Transforms.DependentVariableTransforms Transform
        {
            set 
            {
                if (value == VBCommon.Transforms.DependentVariableTransforms.none)
                {
                    rbValue.Checked = true;
                    _decisionThreshold = Convert.ToDouble(tbThresholdDec.Text);
                    _mandateThreshold = Convert.ToDouble(tbThresholdReg.Text);
                }
                else if (value == VBCommon.Transforms.DependentVariableTransforms.Ln)
                {
                    rbLogeValue.Checked = true;
                    _decisionThreshold = Math.Log(Convert.ToDouble(tbThresholdDec.Text));
                    _mandateThreshold = Math.Log(Convert.ToDouble(tbThresholdReg.Text));
                }
                else if (value == VBCommon.Transforms.DependentVariableTransforms.Log10)
                {
                    rbLog10Value.Checked = true;
                    _decisionThreshold = Math.Log10(Convert.ToDouble(tbThresholdDec.Text));
                    _mandateThreshold = Math.Log10(Convert.ToDouble(tbThresholdReg.Text));
                }
                else if (value == VBCommon.Transforms.DependentVariableTransforms.Power)
                {
                    rbPwrValue.Checked = true;
                    double pwr = Convert.ToDouble(txtPwrValue.Text);
                    _decisionThreshold = Math.Pow(Convert.ToDouble(tbThresholdDec.Text),pwr);
                    _mandateThreshold = Math.Pow(Convert.ToDouble(tbThresholdReg.Text),pwr);
                }
            }

            get
            {               
                if (rbLogeValue.Checked)
                    return VBCommon.Transforms.DependentVariableTransforms.Ln;
                else if (rbLog10Value.Checked)
                    return VBCommon.Transforms.DependentVariableTransforms.Log10;
                else if (rbPwrValue.Checked)
                    return VBCommon.Transforms.DependentVariableTransforms.Power;
                else //(rbValue.Checked)
                    return VBCommon.Transforms.DependentVariableTransforms.none;
            }                        
        }

        public double PowerExponent
        {
            get { return _powerExp; }
            set 
            {
                    _powerExp = value; 
                    txtPwrValue.Text = _powerExp.ToString(); 
            }
        }

        private void InitResultsGraph()
        {
            _XYPlotdata = new List<double[]>();

            GraphPane myPane = zgc.GraphPane;
            if (myPane.CurveList.Count > 0)
                myPane.CurveList.RemoveRange(0, myPane.CurveList.Count - 1);

            myPane.Title.Text = "Predicted vs Observed";
            myPane.XAxis.Title.Text = "Observed";
            myPane.YAxis.Title.Text = "Predicted";

            myPane.XAxis.MajorGrid.IsVisible = true;
            myPane.XAxis.MajorGrid.Color = Color.Gray;

            myPane.YAxis.MajorGrid.IsVisible = true;
            myPane.YAxis.MajorGrid.Color = Color.Gray;

            PointPairList list = new PointPairList();

            //LineItem curve = myPane.AddCurve("Y", list, Color.Red, SymbolType.Circle);
            LineItem curve = myPane.AddCurve("", list, Color.Red, SymbolType.Circle);
            curve.Line.IsVisible = false;
            // Hide the symbol outline
            curve.Symbol.Border.IsVisible = true;
            // Fill the symbol interior with color
            curve.Symbol.Fill = new Fill(Color.Firebrick);

            //Vertical and horizontal threshold lines
            PointPairList list2 = new PointPairList();
            LineItem curve2 = myPane.AddCurve("Decision Threshold", list2, Color.Blue, SymbolType.None);
            curve2.Line.IsVisible = false;

            PointPairList list3 = new PointPairList();
            LineItem curve3 = myPane.AddCurve("Regulatory Threshold", list3, Color.Green, SymbolType.None);
            curve3.Line.IsVisible = false;

            // Scale the axes
            zgc.AxisChange();
        }

        public void UpdateResults(List<double[]> data, double rmse, Exceedance exceedance)
        {
            ////string[] rptlist = null;
            string plotName = string.Empty;
            if (exceedance == Exceedance.prediction)
                plotName = "Pred vs Obs";
             else 
                plotName = "Fit vs Obs";
            
 
                
            //    "Plot: Pred vs Obs",
            //    "Error Table: CFU as DC",
            //    "Plot: % Exc vs Obs",
            //    "Error Table: % Exc as DC"
            //    };
            //}
            //else
            //{
            //    _rptlist = new string[]{
            //    "Plot: Fit vs Obs",
            //    "Error Table: CFU as DC",
            //    "Plot: % Exc vs Obs",
            //    "Error Table: % Exc as DC"
            //    };
            //}
            //cboxPlotList.DataSource = _rptlist;

            _XYPlotdata = data;
            _rmse = rmse;
            _exceedance = exceedance;

            if (_XYPlotdata == null || _XYPlotdata.Count < 1)
            {
                return;
            }

            _obs = new double[data.Count];
            _pred = new double[data.Count];
            //prediction will use the accessor SetPredictionProbabilities to populate _prob
            //if (exceedance == Exceedance.model) 
            //    _prob = new double[data.Count];

            double dc = ThresholdHoriz;

            for (int i = 0; i < data.Count; i++)
            {
                _obs[i] = data[i][0];
                _pred[i] = data[i][1];
                //prediction will use the accessor SetPredictionProbabilities to populate _prob
                //if (exceedance == Exceedance.model)
                    //_prob[i] = Statistics.PExceed(_pred[i],  _decisionThreshold, _rmse);
            }

            if (cboxPlotList.SelectedIndex == 0 || cboxPlotList.SelectedIndex == 1)
            {
                GraphPane myPane = zgc.GraphPane;
                myPane.CurveList.Clear();
                myPane = addPlotXY(_obs, _pred, null, null);
                myPane = addThresholdCurve(myPane, null);
                //myPane.XAxis.Cross = 0.0;
                zgc.AxisChange();
                zgc.Refresh();

                CreateMFailTable();
            }

            else if (cboxPlotList.SelectedIndex == 2 || cboxPlotList.SelectedIndex == 3)
            {
                GraphPane myPane = zgc.GraphPane;
                myPane.CurveList.Clear();
                myPane = addPlotPROB(_obs, _prob, null, null);
                myPane = addProbThresholdCurve(myPane);
                myPane.XAxis.Cross = 0.0;
                zgc.AxisChange();
                zgc.Refresh();
 
                CreatePExceedTable();
            }
        }

        /// <summary>
        /// accessor to populate probabilities; used from prediction
        /// mlr model probabilities differ from predictions - set after call to UpdateResults()
        /// </summary>
        public double[] SetPredictionProbabilities
        {
            set { _prob = value; }
        }

        private void tbThresholdReg_TextChanged(object sender, EventArgs e)
        {

            if (Double.TryParse(tbThresholdReg.Text, out _mandateThreshold) == false)
            {
                //string msg = @"Mandate threshold must be a numeric value.";
                string msg = @"Regulatory standard must be a numeric value.";
                MessageBox.Show(msg);
                return;
            }
            _smandateThreshold = tbThresholdReg.Text;
        }

        private void tbThresholdDec_TextChanged(object sender, EventArgs e)
        {
            double threshold;
            if (Double.TryParse(tbThresholdDec.Text, out threshold) == false)
            {
                string msg = @"Decision criterion must be a numeric value.";
                MessageBox.Show(msg);
                return;
            }

            
            if (cboxPlotList.SelectedIndex == 0 || cboxPlotList.SelectedIndex == 1)
            {
                _decisionThreshold = threshold;
                _sdecisionThreshold = tbThresholdDec.Text;
            }
            else if (cboxPlotList.SelectedIndex == 2 || cboxPlotList.SelectedIndex == 3)
            {
                _probabilityThreshold = threshold;
                _sprobabilityThreshold = tbThresholdDec.Text;
            }
        }

        private void txtPwr_Leave(object sender, EventArgs e)
        {
            double power;
            TextBox txtBox = (TextBox)sender;

            if (!Double.TryParse(txtBox.Text, out power))
            {
                MessageBox.Show("Invalid number.");
                txtBox.Focus();
                //_sdc = "Decision Criterion: ";
                return;
            }
            //_powerExp = Convert.ToDouble(txtBox.Text.ToString());
            _powerExp = power;
            
        }

        private void rbPwrValue_CheckedChanged(object sender, EventArgs e)
        {
            if (rbPwrValue.Checked)
            {
                txtPwrValue.Enabled = true;
                //txtPwr_Leave(this.txtPwrValue, null);
               // _sdc += "(" + tbThresholdDec.Text + ")" + "**" + txtPwrValue.Text;
            }
            else
            {
                txtPwrValue.Enabled = false;
                //_sdc = "Decision Criterion: ";
            }
        }

        public void btnXYPlot_Click(object sender, EventArgs e)
        {

            //just validate and get the thresholds, and then transform
            tbThresholdDec_TextChanged(null, EventArgs.Empty);
            tbThresholdReg_TextChanged(null, EventArgs.Empty);
            if (rbValue.Checked)
            {
                rbValue_CheckedChanged(null, EventArgs.Empty);
            }
            else if (rbLog10Value.Checked)
            {
                rbLog10Value_CheckedChanged(null, EventArgs.Empty);
            }
            else if (rbLogeValue.Checked)
            {
                rbLogeValue_CheckedChanged(null, EventArgs.Empty);
            }
            else if (rbPwrValue.Checked)
            {
                rbPwrValue_Changed(null, EventArgs.Empty);
            }

            UpdateResults(_XYPlotdata, _rmse, _exceedance);
        }


        private void rbValue_CheckedChanged(object sender, EventArgs e)
        {
            if (rbValue.Checked)
            {
                double tv = double.NaN;
                double th = double.NaN;
                try
                {
                    tv = Convert.ToDouble(tbThresholdReg.Text.ToString());
                    th = Convert.ToDouble(tbThresholdDec.Text.ToString());

                    tv = Apply.UntransformThreshold(tv, DependentVariableTransforms.none);
                    th = Apply.UntransformThreshold(th, DependentVariableTransforms.none);

                    if (_depVarXFrm == DependentVariableTransforms.Power)
                    {
                        tv = Apply.TransformThreshold(tv, DependentVariableTransforms.Power, _pwrXFrmExponent);
                        th = Apply.TransformThreshold(th, DependentVariableTransforms.Power, _pwrXFrmExponent);
                    }
                    else
                    {
                        tv = Apply.TransformThreshold(tv, DependentVarXFrm);
                        th = Apply.TransformThreshold(th, DependentVarXFrm);
                    }
                }
                catch
                {
                    string msg = @"Cannot convert thresholds. (" + tbThresholdDec.Text + ", " + tbThresholdReg.Text + ") ";
                    MessageBox.Show(msg);
                    return;
                }                

                _mandateThreshold = tv;
                //_decisionThreshold = th;
                if (cboxPlotList.SelectedIndex == 0 || cboxPlotList.SelectedIndex == 1)
                {
                    _decisionThreshold = th;
                    _sdc = "Decision Criterion: " + tbThresholdDec.Text;
                }
                else if (cboxPlotList.SelectedIndex == 2 || cboxPlotList.SelectedIndex == 3)
                    _probabilityThreshold = th;
                //_decisionThreshold = th;

               
            }
        }

        private void rbLog10Value_CheckedChanged(object sender, EventArgs e)
        {
            if (rbLog10Value.Checked)
            {
                double tv = double.NaN;
                double th = double.NaN;
                //ms has no fp error checking... check for all conditions.
                //log10(x) when x == 0 results in NaN and when x < 0 results in -Infinity
                bool xyPlot = cboxPlotList.SelectedIndex == 0 || cboxPlotList.SelectedIndex == 1;

                tv = Convert.ToDouble(tbThresholdReg.Text.ToString());
                th = Convert.ToDouble(tbThresholdDec.Text.ToString());

                try
                {
                    //tv = Math.Log10(Convert.ToDouble(tbThresholdReg.Text.ToString()));
                    tv = Apply.UntransformThreshold(tv, DependentVariableTransforms.Log10);
                    if (DependentVarXFrm == DependentVariableTransforms.Power)
                        tv = Apply.TransformThreshold(tv, DependentVariableTransforms.Power, _pwrXFrmExponent);
                    else
                        tv = Apply.TransformThreshold(tv, DependentVarXFrm);

                    if (xyPlot)
                    {
                        
                        th = Apply.UntransformThreshold(th, DependentVariableTransforms.Log10);
                        if (DependentVarXFrm == DependentVariableTransforms.Power)                           
                            th = Apply.TransformThreshold(th, DependentVariableTransforms.Power, _pwrXFrmExponent);                        
                        else
                            th = Apply.TransformThreshold(th, DependentVarXFrm);
                        
                       // th = Math.Log10(Convert.ToDouble(tbThresholdDec.Text.ToString()));
                        _sdc = "Decision Criterion: " + "Log(" + tbThresholdDec.Text + ")";
                    }
                    else if (cboxPlotList.SelectedIndex == 2 || cboxPlotList.SelectedIndex == 3)
                        th = Convert.ToDouble(tbThresholdDec.Text.ToString());
                }
                catch
                {
                    string msg = @"Cannot Log 10 transform thresholds. (" + tbThresholdDec.Text + ", " + tbThresholdReg.Text + ") ";
                    MessageBox.Show(msg);
                    return;
                }
                if (tv.Equals(double.NaN) || th.Equals(double.NaN))
                {
                    string msg = @"Entered values must be greater than 0. (" + tbThresholdDec.Text + ", " + tbThresholdReg.Text + ") ";
                    MessageBox.Show(msg);
                    return;
                }
                if (tv < 0 || th < 0)
                {
                    string msg = @"Entered values must be greater than 0. (" + tbThresholdDec.Text + ", " + tbThresholdReg.Text + ") ";
                    MessageBox.Show(msg);
                    return;
                }                
                _mandateThreshold = tv;

                if (cboxPlotList.SelectedIndex == 0 || cboxPlotList.SelectedIndex == 1)
                    _decisionThreshold = th;
                else if (cboxPlotList.SelectedIndex == 2 || cboxPlotList.SelectedIndex == 3)
                    _probabilityThreshold = th;
                //_decisionThreshold = th;
                
            }
        }

        private void rbLogeValue_CheckedChanged(object sender, EventArgs e)
        {
            if (rbLogeValue.Checked)
            {
                double tv = double.NaN;
                double th = double.NaN;
                //ms has no fp error checking... check for all conditions.
                //loge(x) when x == 0 results in NaN and when x < 0 results in -Infinity

                bool xyPlot = cboxPlotList.SelectedIndex == 0 || cboxPlotList.SelectedIndex == 1;

                tv = Convert.ToDouble(tbThresholdReg.Text.ToString());
                th = Convert.ToDouble(tbThresholdDec.Text.ToString());

                tv = Apply.UntransformThreshold(tv, DependentVariableTransforms.Ln);
                if (DependentVarXFrm == DependentVariableTransforms.Power)                
                    tv = Apply.TransformThreshold(tv, DependentVariableTransforms.Power, _pwrXFrmExponent);
                else
                    tv = Apply.TransformThreshold(tv, DependentVarXFrm);

             

                try
                {
                    //tv = Math.Log(Convert.ToDouble(tbThresholdReg.Text.ToString()));
                    if (xyPlot)
                    {
                        
                        th = Apply.UntransformThreshold(th, DependentVariableTransforms.Ln);
                        if (DependentVarXFrm == DependentVariableTransforms.Power)
                            th = Apply.TransformThreshold(th, DependentVariableTransforms.Power, _pwrXFrmExponent);                        
                        else
                            th = Apply.TransformThreshold(th, DependentVarXFrm);                        

                        //th = Math.Log(Convert.ToDouble(tbThresholdDec.Text.ToString()));
                        _sdc = "Decision Criterion: " + "Ln(" + tbThresholdDec.Text + ")";
                    }
                    else if (cboxPlotList.SelectedIndex == 2 || cboxPlotList.SelectedIndex == 3)
                        th = Convert.ToDouble(tbThresholdDec.Text.ToString());
                }
                catch
                {
                    string msg = @"Cannot Log e transform thresholds. (" + tbThresholdDec.Text + ", " + tbThresholdReg.Text + ") ";
                    MessageBox.Show(msg);
                    return;
                }
                if (tv.Equals(double.NaN) || th.Equals(double.NaN))
                {
                    string msg = @"Entered values must be greater than 0. (" + tbThresholdDec.Text + ", " + tbThresholdReg.Text + ") ";
                    MessageBox.Show(msg);
                    return;
                }
                if (tv < 0 || th < 0)
                {
                    string msg = @"Entered values must be greater than 0. (" + tbThresholdDec.Text + ", " + tbThresholdReg.Text + ") ";
                    MessageBox.Show(msg);
                    return;
                }

               

                _mandateThreshold = tv;
                //_decisionThreshold = th;
                if (cboxPlotList.SelectedIndex == 0 || cboxPlotList.SelectedIndex == 1)
                    _decisionThreshold = th;
                else if (cboxPlotList.SelectedIndex == 2 || cboxPlotList.SelectedIndex == 3)
                    _probabilityThreshold = th;
                //_decisionThreshold = th;

                
            }

        }

        private void rbPwrValue_Changed(object sender, EventArgs e)
        {
            if (rbPwrValue.Checked)
            {
                double tv = double.NaN;
                double th = double.NaN;
                double power = double.NaN;
                //ms has no fp error checking... check for all conditions.
                //loge(x) when x == 0 results in NaN and when x < 0 results in -Infinity

                bool xyPlot = cboxPlotList.SelectedIndex == 0 || cboxPlotList.SelectedIndex == 1;

                tv = Convert.ToDouble(tbThresholdReg.Text.ToString());
                th = Convert.ToDouble(tbThresholdDec.Text.ToString());
                power = Convert.ToDouble(txtPwrValue.Text);

                tv = Apply.UntransformThreshold(tv, DependentVariableTransforms.Power, power);
                if (DependentVarXFrm == DependentVariableTransforms.Power)
                    tv = Apply.TransformThreshold(tv, DependentVariableTransforms.Power, _pwrXFrmExponent);
                else
                    tv = Apply.TransformThreshold(tv, DependentVarXFrm);

                try
                {                    
                    //tv = Math.Pow(Convert.ToDouble(tbThresholdReg.Text), power);
                    if (xyPlot)
                    {
                        th = Apply.UntransformThreshold(th, DependentVariableTransforms.Power, power);
                        if (DependentVarXFrm == DependentVariableTransforms.Power)
                            th = Apply.TransformThreshold(th, DependentVariableTransforms.Power, _pwrXFrmExponent);                        
                        else
                            th = Apply.TransformThreshold(th, DependentVarXFrm);
                        
                        //th = Math.Pow(Convert.ToDouble(tbThresholdDec.Text), power);
                    }
                    else if (cboxPlotList.SelectedIndex == 2 || cboxPlotList.SelectedIndex == 3)
                        th = Convert.ToDouble(tbThresholdDec.Text.ToString());

                    //tv = Math.Pow(Convert.ToDouble(tbThresholdReg.Text), power);
                    //th = Math.Pow(Convert.ToDouble(tbThresholdDec.Text), power);
                }
                catch
                {
                    string msg = @"Cannot power transform thresholds. (" + tbThresholdDec.Text + ", " + tbThresholdReg.Text + ") ";
                    MessageBox.Show(msg);
                    return;
                }
                if (tv.Equals(double.NaN) || th.Equals(double.NaN))
                {
                    string msg = @"Entered values must be greater than 0. (" + tbThresholdDec.Text + ", " + tbThresholdReg.Text + ") ";
                    MessageBox.Show(msg);
                    return;
                }
                if (tv < 0 || th < 0)
                {
                    string msg = @"Entered values must be greater than 0. (" + tbThresholdDec.Text + ", " + tbThresholdReg.Text + ") ";
                    MessageBox.Show(msg);
                    return;
                }

                


                _mandateThreshold = tv;
 
                if (cboxPlotList.SelectedIndex == 0 || cboxPlotList.SelectedIndex == 1)
                {
                    _decisionThreshold = th;
                    _sdc = "Decision Criterion: " + "(" + tbThresholdDec.Text + ")" + "**" + txtPwrValue.Text;
                }
                else if (cboxPlotList.SelectedIndex == 2 || cboxPlotList.SelectedIndex == 3)
                    _probabilityThreshold = th;
                //_decisionThreshold = th;
            }
        }

        private void cboxPlotList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboxPlotList.SelectedIndex == 0)
            {
                zgc.Visible = true;
                listView1.Visible = false;
                label11.Text = "Decision Criterion (Horizontal)";
                tbThresholdDec.Text =_sdecisionThreshold;
                btnXYPlot_Click(null, null);
                groupBox2.Visible = true;
                groupBox3.Visible = true;
            }
            else if (cboxPlotList.SelectedIndex == 1)
            {
                label11.Text = "Decision Criterion (Horizontal)";
                tbThresholdDec.Text = _sdecisionThreshold;
                zgc.Visible = false;
                listView1.Visible = true;
                groupBox2.Visible = false;
                groupBox3.Visible = false;

            }
            else if (cboxPlotList.SelectedIndex == 2)
            {
                zgc.Visible = true;
                listView1.Visible = false;
                label11.Text = "Percent Probability (0-100)";
                tbThresholdDec.Text = _sprobabilityThreshold;
                groupBox2.Visible = true;
                groupBox3.Visible = true;
            }
            else if (cboxPlotList.SelectedIndex == 3)
            {
                label11.Text = "Percent Probability (0-100)";
                tbThresholdDec.Text = _sprobabilityThreshold;
                zgc.Visible = false;
                listView1.Visible = true;
                groupBox2.Visible = false;
                groupBox3.Visible = false;
            }

            UpdateResults(_XYPlotdata, _rmse, _exceedance);
        }

        private void CreatePExceedTable()
        {
            listView1.Clear();
            listView1.View = View.Details;

            //string[] cols = { "P(Exceedance)", " FN ", " FP ", "Total", "Specificity", "Sensitivity", "Accuracy" };
            string[] cols = { "P(Exceedance)", "False Non-Exceed", "False Exceed", "Total", "Sensitivity", "Specificity", "Accuracy" };
            for (int i = 0; i < cols.Length; i++)
            {
                listView1.Columns.Add(new ColumnHeader());
                listView1.Columns[i].Text = cols[i];
                listView1.Columns[i].TextAlign = HorizontalAlignment.Left;
            }
            //listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            for (int prob = 5; prob < 101; prob = prob + 5)
            {
                string[] line = new string[7];
                line[0] = prob.ToString();

                VBStatistics.ModelErrorCounts mec = new VBStatistics.ModelErrorCounts();
                mec.getCounts(prob, _mandateThreshold, _prob, _obs);

                line[1] = mec.FNCount.ToString();
                line[2] = mec.FPCount.ToString();
                int tot = mec.FPCount + mec.FNCount;
                line[3] = tot.ToString();
                line[5] = mec.Specificity.ToString("f4");
                line[4] = mec.Sensitivity.ToString("f4");
                line[6] = mec.Accuracy.ToString("f4");

                ListViewItem lvi = new ListViewItem(line);
                listView1.Items.Add(lvi);
            }
            //listView1.Items.Add(new ListViewItem(""));
            //listView1.Items.Add(_sdc);
            string[] newline = new string[] { "", "", "", "", "", "", "" };
            listView1.Items.Add(new ListViewItem(newline));
            newline[0] = _sdc;
            listView1.Items.Add(new ListViewItem(newline));
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void CreateMFailTable()
        {

            //just validate and get the thresholds, and then transform
            tbThresholdDec_TextChanged(null, EventArgs.Empty);
            tbThresholdReg_TextChanged(null, EventArgs.Empty);
            if (rbValue.Checked)
            {
                rbValue_CheckedChanged(null, EventArgs.Empty);
            }
            else if (rbLog10Value.Checked)
            {
                rbLog10Value_CheckedChanged(null, EventArgs.Empty);
            }
            else if (rbLogeValue.Checked)
            {
                rbLogeValue_CheckedChanged(null, EventArgs.Empty);
            }
            else if (rbPwrValue.Checked)
            {
                rbPwrValue_Changed(null, EventArgs.Empty);
            }

            const int interations = 25;

            listView1.Clear();
            listView1.View = View.Details;

            //string[] cols = { "Decision Threshold", "FN", "FP", "Specificity", "Sensitivity", "Accuracy" };
            string[] cols = { "Decision Threshold", "False Non-Exceed", "False Exceed", "Total", "Sensitivity", "Specificity", "Accuracy" };
            for (int i = 0; i < cols.Length; i++)
            {
                listView1.Columns.Add(new ColumnHeader());
                listView1.Columns[i].Text = cols[i];
                listView1.Columns[i].TextAlign = HorizontalAlignment.Left;
            }
            //listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);


            double dcMax = _pred.Max();
            double dcMin = _pred.Min();
            double inc = (dcMax - dcMin) / (double)interations;
            double dc = dcMin - inc;

            ModelErrorCounts mec;
            int tot;
            ListViewItem lvi;
            bool lineInserted = false;
            while (dc < dcMax)
            {
                dc += inc;
                string[] line = new string[7];
                double tdc = 0.0d;
                if (Transform == VBCommon.Transforms.DependentVariableTransforms.Log10)
                {
                    tdc = Math.Pow(10.0d, dc);
                }
                else if (Transform == VBCommon.Transforms.DependentVariableTransforms.Ln)
                {
                    tdc = Math.Pow(Math.E, dc);
                }
                else if (Transform == VBCommon.Transforms.DependentVariableTransforms.Power)
                {
                    tdc = Math.Pow(dc, 1.0 / _powerExp);
                }
                else //(Transform == Globals.DependendVariableTransforms.none)
                {
                    tdc = dc;
                }

                line[0] = tdc.ToString("n4");

                //ModelErrorCounts mec = new ModelErrorCounts();
                mec = new ModelErrorCounts();
                mec.getCounts(dc, _mandateThreshold, _pred, _obs);
                line[1] = mec.FNCount.ToString();
                line[2] = mec.FPCount.ToString();
                tot = mec.FNCount + mec.FPCount;
                line[3] = tot.ToString();
                line[5] = mec.Specificity.ToString("f4");
                line[4] = mec.Sensitivity.ToString("f4");
                line[6] = mec.Accuracy.ToString("f4");

                lvi = new ListViewItem(line);
                listView1.Items.Add(lvi);
            }
            string[] newline = new string[] { "", "", "", "", "", "", "" };
            listView1.Items.Add(new ListViewItem(newline));
            newline[0] = _sdc;
            listView1.Items.Add(new ListViewItem(newline));
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

        }

        private GraphPane addPlotPROB(double[] obs, double[] pexceed, string[] tags, string plot)
        {
            PointPairList ppl1 = new PointPairList();

            string tag = string.Empty;

            int npts = obs.Length > pexceed.Length ? pexceed.Length : obs.Length;

            for (int i = 0; i < npts; i++)
            {
                //tag = tags[i];
                ppl1.Add(obs[i], pexceed[i]); //, tag);
            }

            GraphPane gp = zgc.GraphPane;
            LineItem curve1 = gp.AddCurve(_sdc, ppl1, Color.Red, SymbolType.Circle);
            curve1.Symbol.Border.IsVisible = true;
            curve1.Symbol.Fill = new Fill(Color.Firebrick);
            curve1.Line.IsVisible = false;

            gp.XAxis.Title.Text = "Observed";
            if (_exceedance == Exceedance.prediction)
                gp.YAxis.Title.Text = "Probability of Prediction Exceedance";
            else gp.YAxis.Title.Text = "Probability of FitValue Exceedance";

            gp.Tag = "PROBPlot";
            if (_exceedance == Exceedance.prediction)
                gp.Title.Text = "Prediction Probability Exceedance vs Observations";
            else gp.Title.Text = "FitValue Probability Exceedance vs Observations";
            gp.XAxis.Type = AxisType.Linear;

            VBStatistics.ModelErrorCounts mec = new VBStatistics.ModelErrorCounts();
            mec.getCounts(_probabilityThreshold, _mandateThreshold, pexceed, obs);

            tbFN.Text = mec.FNCount.ToString();
            tbFP.Text = mec.FPCount.ToString();
            txbSpecificity.Text = mec.Specificity.ToString();
            txbSensitivity.Text = mec.Sensitivity.ToString();
            txbAccuracy.Text = mec.Accuracy.ToString();

            gp.XAxis.Cross = 0;

            return gp;
        }

        private GraphPane addProbThresholdCurve(GraphPane myPane)
        {
            double xMin, xMax, yMin, yMax;
            double xPlotMin, xPlotMax, yPlotMin, yPlotMax;
            myPane.CurveList[0].GetRange(out xMin, out xMax, out yMin, out yMax, false, false, myPane);

            if (xMax.GetType() == typeof(ZedGraph.XDate))
            {
                xPlotMin = xMin < 0.0 ? xMin : 0;
                xPlotMax = xMax > _probabilityThreshold ? xMax : _probabilityThreshold;
            }
            else
            {
                xPlotMin = xMin;
                xPlotMax = xMax;
            }
            yPlotMin = yMin < 0.0 ? yMin : 0;

            //mikec wants max yscale to be this so display is max 1.2
            yPlotMax = 101.0;

            //probability threshold
            PointPair pp1 = new PointPair(xPlotMin, _probabilityThreshold);
            PointPair pp2 = new PointPair(xPlotMax, _probabilityThreshold);
            PointPairList ppl1 = new PointPairList();
            ppl1.Add(pp1);
            ppl1.Add(pp2);

            //regulatory threshold
            pp1 = new PointPair(_mandateThreshold, yPlotMin);
            pp2 = new PointPair(_mandateThreshold, yPlotMax);
            PointPairList ppl2 = new PointPairList();
            ppl2.Add(pp1);
            ppl2.Add(pp2);

            LineItem curve1 = myPane.AddCurve("Exceedance Probability Threshold", ppl1, Color.Blue, SymbolType.None);
            LineItem curve2 = myPane.AddCurve("Regulatory Threshold", ppl2, Color.Green, SymbolType.None);
            curve1.Line.IsVisible = true;

            return myPane;

        }

        private GraphPane addPlotXY(double[] obs, double[] pred, string[] tags, string plot /*, double[] unbiased */)
        {

            PointPairList ppl1 = new PointPairList();

            string tag = string.Empty;

            int npts = obs.Length > pred.Length ? pred.Length : obs.Length;

            for (int i = 0; i < npts; i++)
            {
                //tag = tags[i];
                ppl1.Add(obs[i], pred[i]); //, tag);
            }

            GraphPane gp = zgc.GraphPane;
            LineItem curve1 = gp.AddCurve(null, ppl1, Color.Red, SymbolType.Circle);
            curve1.Symbol.Border.IsVisible = true;
            curve1.Symbol.Fill = new Fill(Color.Firebrick);
            //LineItem curve2 = gp.AddCurve("Unbiased Estimates", ppl2, Color.Blue);
            curve1.Line.IsVisible = false;

            gp.XAxis.Title.Text = "Observed";
            if (_exceedance == Exceedance.prediction)
                gp.YAxis.Title.Text = "Predicted";
            else gp.YAxis.Title.Text = "Fitted";

            gp.Tag = "XYPlot";
            if (_exceedance == Exceedance.prediction)
                gp.Title.Text = "Predicted vs Observed Values";
            else gp.Title.Text = "Fitted vs Observed";
            gp.XAxis.Type = AxisType.Linear;

            VBStatistics.ModelErrorCounts mec = new VBStatistics.ModelErrorCounts();
            mec.getCounts(_decisionThreshold, _mandateThreshold, pred, obs);

            tbFN.Text = mec.FNCount.ToString();
            tbFP.Text = mec.FPCount.ToString();
            txbSpecificity.Text = mec.Specificity.ToString();
            txbSensitivity.Text = mec.Sensitivity.ToString();
            txbAccuracy.Text = mec.Accuracy.ToString();

            gp.XAxis.MajorGrid.IsVisible = true;
            gp.XAxis.MajorGrid.Color = Color.Gray;

            gp.YAxis.MajorGrid.IsVisible = true;
            gp.YAxis.MajorGrid.Color = Color.Gray;

            return gp;
        }

        private GraphPane addThresholdCurve(GraphPane myPane, string plot)
        {
            double xMin, xMax, yMin, yMax;
            double xPlotMin, xPlotMax, yPlotMin, yPlotMax;
            myPane.CurveList[0].GetRange(out xMin, out xMax, out yMin, out yMax, false, false, myPane);
 
            xPlotMin = xMin;
            xPlotMax = xMax > _decisionThreshold ? xMax : _decisionThreshold;
  
            yPlotMin = yMin < 0.0 ? yMin : 0;
            yPlotMax = yMax > _mandateThreshold ? yMax : _mandateThreshold;

            //decision threshold
            PointPair pp1 = new PointPair(xPlotMin - 1, _decisionThreshold);
            PointPair pp2 = new PointPair(xPlotMax + 1, _decisionThreshold);
            PointPairList ppl1 = new PointPairList();
            ppl1.Add(pp1);
            ppl1.Add(pp2);

            //regulatory threshold
            pp1 = new PointPair(_mandateThreshold, yPlotMin - 1);
            pp2 = new PointPair(_mandateThreshold, yPlotMax + 1);
            PointPairList ppl2 = new PointPairList();
            ppl2.Add(pp1);
            ppl2.Add(pp2);

            LineItem curve1 = myPane.AddCurve("Decision Threshold", ppl1, Color.Blue, SymbolType.None);
            LineItem curve2 = myPane.AddCurve("Regulatory Threshold", ppl2, Color.Green, SymbolType.None);
            curve1.Line.IsVisible = true;
            curve2.Line.IsVisible = true;

            return myPane;
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Modifiers & Keys.Control) != 0)
            {
                if (e.KeyCode == Keys.C)
                {
                    CopyListViewToClipboard(listView1);
                }
            }
        }

        public void CopyListViewToClipboard(ListView lv)
        {
            StringBuilder buffer = new StringBuilder();

            for (int i = 0; i < lv.Columns.Count; i++)
            {
                buffer.Append(lv.Columns[i].Text);
                buffer.Append("\t");
            }

            buffer.Append(Environment.NewLine);

            for (int i = 0; i < lv.Items.Count; i++)
            {
                if (lv.Items[i].Selected)
                {
                    for (int j = 0; j < lv.Columns.Count; j++)
                    {
                        buffer.Append(lv.Items[i].SubItems[j].Text);
                        buffer.Append("\t");
                    }

                    buffer.Append(Environment.NewLine);
                }
            }

            Clipboard.SetText(buffer.ToString());
        }

        private void rbValue_CheckedChanged_1(object sender, EventArgs e)
        {
            if (rbValue.Checked)
            {
                if (cboxPlotList.SelectedIndex == 0 || cboxPlotList.SelectedIndex == 1)
                {
                    _sdc = "Decision Criterion: " + tbThresholdDec.Text;
                }
            }

        }

        private void rbLog10Value_CheckedChanged_1(object sender, EventArgs e)
        {
            if (rbLog10Value.Checked)
            {
                if (cboxPlotList.SelectedIndex == 0 || cboxPlotList.SelectedIndex == 1)
                {
                    _sdc = "Decision Criterion: " + "Log(" + tbThresholdDec.Text + ")";
                }
            }

        }

        private void rbLogeValue_CheckedChanged_1(object sender, EventArgs e)
        {
            if (rbLogeValue.Checked)
            {
                if (cboxPlotList.SelectedIndex == 0 || cboxPlotList.SelectedIndex == 1)
                {
                    _sdc = "Decision Criterion: " + "Ln(" + tbThresholdDec.Text + ")";
                }
            }
  
        }
    }
}
