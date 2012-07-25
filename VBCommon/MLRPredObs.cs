using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VBCommon;
//using VBStatistics;
using ZedGraph;

namespace VBControls
{
    public partial class MLRPredObs : UserControl
    {
        private List<double[]> lstXYPlotdata;
        //Threshold value used for sensitiviy, specificity, accuracy
        double dblDecisionThreshold;
        double dblMandateThreshold;
        double dblPowerExp = double.NaN;

        //constructor
        public MLRPredObs()
        {
            InitializeComponent();

            dblDecisionThreshold = Convert.ToDouble(tbThresholdDec.Text);
            dblMandateThreshold = Convert.ToDouble(tbThresholdDec.Text);

            InitResultsGraph();
        }


        //return horizonal threshold value
        public double ThresholdHoriz
        {
            get { return dblDecisionThreshold; }
        }


        //return vertical threshold value
        public double ThresholdVert
        {
            get { return dblMandateThreshold; }
        }


        //set threshold using variables
        public void SetThresholds(double decisionThreshold, double mandateThreshold)
        {
            dblDecisionThreshold = decisionThreshold;
            dblMandateThreshold = mandateThreshold;
            tbThresholdDec.Text = dblDecisionThreshold.ToString();
            tbThresholdReg.Text = dblMandateThreshold.ToString();
        }


        //set threshold using parameters
        public void SetThresholds(string decisionThreshold, string mandateThreshold)
        {
            dblDecisionThreshold = Convert.ToDouble(decisionThreshold);
            dblMandateThreshold = Convert.ToDouble(mandateThreshold);
            tbThresholdDec.Text = decisionThreshold;
            tbThresholdReg.Text = mandateThreshold;
        }

        
        // getter/setter for transform type
        public string Transform
        {
            set 
            {
                EventArgs args = new EventArgs();
                
                if (value == "none")
                {
                    rbValue.Checked = true;
                    dblDecisionThreshold = Convert.ToDouble(tbThresholdDec.Text);
                    dblMandateThreshold = Convert.ToDouble(tbThresholdReg.Text);
                    rbValue_CheckedChanged(this, args);
                }
                else if (value == "Ln")
                {
                    rbLogeValue.Checked = true;
                    dblDecisionThreshold = Math.Log(Convert.ToDouble(tbThresholdDec.Text));
                    dblMandateThreshold = Math.Log(Convert.ToDouble(tbThresholdReg.Text));
                    rbLogeValue_CheckedChanged(this, args);
                }
                else if (value == "Log10")
                {
                    rbLog10Value.Checked = true;
                    dblDecisionThreshold = Math.Log10(Convert.ToDouble(tbThresholdDec.Text));
                    dblMandateThreshold = Math.Log10(Convert.ToDouble(tbThresholdReg.Text));
                    rbLog10Value_CheckedChanged(this, args);
                }
                else if (value == "Power")
                {
                    rbPwrValue.Checked = true;
                    double pwr = Convert.ToDouble(txtPwrValue.Text);
                    dblDecisionThreshold = Math.Pow(Convert.ToDouble(tbThresholdDec.Text),pwr);
                    dblMandateThreshold = Math.Pow(Convert.ToDouble(tbThresholdReg.Text),pwr);
                    rbPwrValue_CheckedChanged(this, args);
                }
                this.Refresh();
            }
            get
            {               
                if (rbLogeValue.Checked)
                    return "Ln";
                else if (rbLog10Value.Checked)
                    return "Log10";
                else if (rbPwrValue.Checked)
                    return "Power";
                else //(rbValue.Checked)
                    return "none";
            }                        
        }


        //getter/setter for power exponent value
        public double PowerExponent
        {
            get { return dblPowerExp; }
            set 
            {
                dblPowerExp = value; 
                txtPwrValue.Text = dblPowerExp.ToString(); 
            }
        }


        //initialize results graph
        private void InitResultsGraph()
        {
            lstXYPlotdata = new List<double[]>();
            GraphPane myPane = zedGraphControl1.GraphPane;
            //clear values
            if (myPane.CurveList.Count > 0)
                myPane.CurveList.RemoveRange(0, myPane.CurveList.Count - 1);
            //set text
            myPane.Title.Text = "Results";
            myPane.XAxis.Title.Text = "Observed";
            myPane.YAxis.Title.Text = "Predicted";
            //set values
            myPane.XAxis.MajorGrid.IsVisible = true;
            myPane.XAxis.MajorGrid.Color = Color.Gray;

            myPane.YAxis.MajorGrid.IsVisible = true;
            myPane.YAxis.MajorGrid.Color = Color.Gray;

            PointPairList list = new PointPairList();

            LineItem curve = myPane.AddCurve("Y", list, Color.Red, SymbolType.Circle);
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
            zedGraphControl1.AxisChange();
        }


        //update results based on changed data
        public void UpdateResults(List<double[]> data)
        {
            lstXYPlotdata = data;

            if ((lstXYPlotdata == null) || (lstXYPlotdata.Count < 1))
            {
                zedGraphControl1.GraphPane.CurveList[0].Clear();
                zedGraphControl1.Refresh();
                return;
            }
                        
            // Make sure that the curvelist has at least one curve
            if (zedGraphControl1.GraphPane.CurveList.Count <= 0)
                return;

            // Get the first CurveItem in the graph
            LineItem curve = zedGraphControl1.GraphPane.CurveList[0] as LineItem;
            if (curve == null)
                return;

            // Get the PointPairList
            IPointListEdit list = curve.Points as IPointListEdit;

            // If this is null, it means the reference at curve.Points does not
            // support IPointListEdit, so we won't be able to modify it
            if (list == null)
                return;

            list.Clear();

            double maxX = Double.NegativeInfinity;
            double maxY = Double.NegativeInfinity;
            double minX = 0;
            double minY = 0;
            for (int i = 0; i < data.Count; i++)
            {
                list.Add(data[i][0], data[i][1]);
                if (data[i][0] > maxX)
                    maxX = data[i][0];
                if (data[i][1] > maxY)
                    maxY = data[i][1];
                if (data[i][0] < minX)
                    minX = data[i][0];
                if (data[i][1] < minY)
                    minY = data[i][1];
            }

            //if data out of range of thresholds, make the threshold plot lines bigger
            if (dblDecisionThreshold > maxX) maxX = dblMandateThreshold; //_decisionThreshold;
            if (dblMandateThreshold > maxY) maxY = dblDecisionThreshold; //_mandateThreshold;

            //find the model error counts for the XYplot display
            ModelErrorCounts mec = new ModelErrorCounts();
            mec.getCounts(dblDecisionThreshold, dblMandateThreshold, data);
            if (mec.Status)
            {
                int intFpc = mec.FPCount;
                int intFnc = mec.FNCount;
                tbFN.Text = intFnc.ToString();
                tbFP.Text = intFpc.ToString();
                txbSpecificity.Text = mec.Specificity.ToString();
                txbSensitivity.Text = mec.Sensitivity.ToString();
                txbAccuracy.Text = mec.Accuracy.ToString();
            }
            else
            {
                string msg = "Data Error: " + mec.Message.ToString();
                MessageBox.Show(msg);
                return;
            }

            LineItem curve2 = zedGraphControl1.GraphPane.CurveList[1] as LineItem;
            LineItem curve3 = zedGraphControl1.GraphPane.CurveList[2] as LineItem;
            if ((curve2 != null) && (curve3 != null))
            {
                curve2.Line.IsVisible = false;
                curve3.Line.IsVisible = false;
                
                // Get the PointPairList
                IPointListEdit list2 = curve2.Points as IPointListEdit;
                list2.Clear();
                //mikec want the thresholds crossing, thus the "-1", "+1"
                list2.Add(minX - 1, dblDecisionThreshold);
                list2.Add(maxX + 1, dblDecisionThreshold);
                curve2.Line.IsVisible = true;

                // Get the PointPairList
                //mikec want the thresholds crossing, thus the "-1", "+1"
                IPointListEdit list3 = curve3.Points as IPointListEdit;
                list3.Clear();
                list3.Add(dblMandateThreshold, minY - 1);
                list3.Add(dblMandateThreshold, maxY + 1);
                curve3.Line.IsVisible = true;
            }

            // Keep the X scale at a rolling 30 second interval, with one
            // major step between the max X value and the end of the axis
            Scale xScale = zedGraphControl1.GraphPane.XAxis.Scale;
            if (data[data.Count - 1][0] > xScale.Max - xScale.MajorStep)
            {
            }

            GraphPane zgc1pane = zedGraphControl1.GraphPane;
            zgc1pane.XAxis.Cross = 0;
            
            // Make sure the Y axis is rescaled to accommodate actual data
            zedGraphControl1.AxisChange();
            // Force a redraw
            zedGraphControl1.Invalidate();
            zedGraphControl1.Refresh();
            Application.DoEvents();
        }


        //check for numeric values on text change
        private void tbThresholdReg_TextChanged(object sender, EventArgs e)
        {
            if (Double.TryParse(tbThresholdReg.Text, out dblMandateThreshold) == false)
            {
                //string msg = @"Mandate threshold must be a numeric value.";
                string msg = @"Regulatory standard must be a numeric value.";
                MessageBox.Show(msg);
                return;
            }
        }


        //check for numeric values on text change
        private void tbThresholdDec_TextChanged(object sender, EventArgs e)
        {
            if (Double.TryParse(tbThresholdDec.Text, out dblDecisionThreshold) == false)
            {
                string msg = @"Decision criterion must be a numeric value.";
                MessageBox.Show(msg);
                return;
            }
        }


        //check for valid number on leave
        private void txtPwr_Leave(object sender, EventArgs e)
        {
            double power;
            TextBox txtBox = (TextBox)sender;

            if (!Double.TryParse(txtBox.Text, out power))
            {
                MessageBox.Show("Invalid number.");
                txtBox.Focus();
            }
        }


        //set Power value
        private void rbPwrValue_CheckedChanged(object sender, EventArgs e)
        {
            if (rbPwrValue.Checked)
                txtPwrValue.Enabled = true;
            else
                txtPwrValue.Enabled = false;
        }


        //click event
        private void btnXYPlot_Click(object sender, EventArgs e)
        {
            this.Refresh();
        }


        //refresh after click event
        public void Refresh()
        {
            //just validate and get the thresholds, and then transform
            tbThresholdDec_TextChanged(null, EventArgs.Empty);
            tbThresholdReg_TextChanged(null, EventArgs.Empty);
            if (rbValue.Checked)
                rbValue_CheckedChanged(null, EventArgs.Empty);
            else if (rbLog10Value.Checked)
                rbLog10Value_CheckedChanged(null, EventArgs.Empty);
            else if (rbLogeValue.Checked)
                rbLogeValue_CheckedChanged(null, EventArgs.Empty);
            else if (rbPwrValue.Checked)
                rbPwrValue_Changed(null, EventArgs.Empty);
            //now plot it
            UpdateResults(lstXYPlotdata);
        }


        //transform type changed
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
                }
                catch
                {
                    string msg = @"Cannot convert thresholds. (" + tbThresholdDec.Text + ", " + tbThresholdReg.Text + ") ";
                    MessageBox.Show(msg);
                    return;
                }

                dblMandateThreshold = tv;
                dblDecisionThreshold = th;
            }
        }


        //transform type changed
        private void rbLog10Value_CheckedChanged(object sender, EventArgs e)
        {
            if (rbLog10Value.Checked)
            {
                double tv = double.NaN;
                double th = double.NaN;
                
                try
                {
                    tv = Math.Log10(Convert.ToDouble(tbThresholdReg.Text.ToString()));
                    th = Math.Log10(Convert.ToDouble(tbThresholdDec.Text.ToString()));
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

                dblMandateThreshold = tv;
                dblDecisionThreshold = th;
            }
        }


        //transform type changed
        private void rbLogeValue_CheckedChanged(object sender, EventArgs e)
        {
            if (rbLogeValue.Checked)
            {
                double tv = double.NaN;
                double th = double.NaN;

                try
                {
                    tv = Math.Log(Convert.ToDouble(tbThresholdReg.Text.ToString()));
                    th = Math.Log(Convert.ToDouble(tbThresholdDec.Text.ToString()));
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

                dblMandateThreshold = tv;
                dblDecisionThreshold = th;
            }
        }


        //transform type changed
        private void rbPwrValue_Changed(object sender, EventArgs e)
        {
            if (rbPwrValue.Checked)
            {
                double tv = double.NaN;
                double th = double.NaN;

                try
                {
                    double power = Convert.ToDouble(txtPwrValue.Text);
                    tv = Math.Pow(Convert.ToDouble(tbThresholdReg.Text), power);
                    th = Math.Pow(Convert.ToDouble(tbThresholdDec.Text), power);
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

                dblMandateThreshold = tv;
                dblDecisionThreshold = th;
            }
        }
    }
}
