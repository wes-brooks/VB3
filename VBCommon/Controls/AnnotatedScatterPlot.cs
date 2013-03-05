using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VBCommon;
using VBCommon.Statistics;
using VBCommon.Transforms;
using ZedGraph;

namespace VBCommon.Controls
{
    public partial class AnnotatedScatterPlot : UserControl
    {
        private List<double[]> lstXYPlotdata;
        //Threshold value used for sensitiviy, specificity, accuracy
        double dblDecisionThreshold;
        double dblMandateThreshold;
        double dblProbabilityThreshold;
        double dblPowerExp = double.NaN;
        bool boolRawPredictions = true;
        double dblDisplayXfrmExponent;

        //public DependentVariableTransforms xfrmObs = DependentVariableTransforms.none;
        //public DependentVariableTransforms xfrmPred = DependentVariableTransforms.none;
        public DependentVariableTransforms xfrmLast = DependentVariableTransforms.none;
        public DependentVariableTransforms xfrmCurrent = DependentVariableTransforms.none;
        public DependentVariableTransforms xfrmDisplay = DependentVariableTransforms.none;
        //public double dblPredExponent = 1;
        //public double dblObsExponent = 1;
        public double dblLastExponent = 1;
        public double dblCurrentExponent = 1;
        
        //constructor
        public AnnotatedScatterPlot(bool EnableProbabilityThreshold=false)
        {
            InitializeComponent();

            lblProb.Visible = EnableProbabilityThreshold;
            rbRaw.Visible = EnableProbabilityThreshold;
            rbProb.Visible = EnableProbabilityThreshold;
            txtProbabilityThreshold.Visible = EnableProbabilityThreshold;

            dblDecisionThreshold = Convert.ToDouble(tbThresholdDec.Text);
            dblMandateThreshold = Convert.ToDouble(tbThresholdDec.Text);

            InitResultsGraph();
        }

        //return horizonal threshold value
        public bool RawPredictions
        {
            get { return rbRaw.Checked; }
            set
            { 
                rbRaw.Checked = value;
                rbProb.Checked = !value;
            }
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
        public void SetThresholds(double DecisionThreshold, double MandateThreshold, double ProbabilityThreshold=50)
        {
            dblDecisionThreshold = DecisionThreshold;
            dblMandateThreshold = MandateThreshold;
            dblProbabilityThreshold = ProbabilityThreshold;
            tbThresholdDec.Text = dblDecisionThreshold.ToString();
            tbThresholdReg.Text = dblMandateThreshold.ToString();
            txtProbabilityThreshold.Text = dblProbabilityThreshold.ToString();
        }


        //set threshold using parameters
        public void SetThresholds(string DecisionThreshold, string MandateThreshold, string ProbabilityThreshold="50")
        {
            dblDecisionThreshold = Convert.ToDouble(DecisionThreshold);
            dblMandateThreshold = Convert.ToDouble(MandateThreshold);
            dblProbabilityThreshold = Convert.ToDouble(ProbabilityThreshold);
            tbThresholdDec.Text = DecisionThreshold;
            tbThresholdReg.Text = MandateThreshold;
            txtProbabilityThreshold.Text = ProbabilityThreshold;
        }


        // getter/setter for transform type
        public string DisplayTransform
        {
            set
            {
                if (value == "none") { xfrmDisplay = VBCommon.Transforms.DependentVariableTransforms.none; }
                else if (value == "Ln") { xfrmDisplay = VBCommon.Transforms.DependentVariableTransforms.Ln; }                
                else if (value == "Log10"){ xfrmDisplay = VBCommon.Transforms.DependentVariableTransforms.Log10; }
                else if (value == "Power") { xfrmDisplay = VBCommon.Transforms.DependentVariableTransforms.Power; }                
            }
            get
            {
                return xfrmDisplay.ToString();
            }
        }


        // getter/setter for transform type
        public double DisplayTransformExponent
        {
            set { dblDisplayXfrmExponent = value; }
            get { return dblDisplayXfrmExponent; }
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
                    //dblDecisionThreshold = Convert.ToDouble(tbThresholdDec.Text);
                    //dblMandateThreshold = Convert.ToDouble(tbThresholdReg.Text);
                    rbValue_CheckedChanged(this, args);
                }
                else if (value == "Ln")
                {
                    rbLogeValue.Checked = true;
                    //dblDecisionThreshold = Math.Exp(Convert.ToDouble(tbThresholdDec.Text));
                    //dblMandateThreshold = Math.Exp(Convert.ToDouble(tbThresholdReg.Text));
                    rbLogeValue_CheckedChanged(this, args);
                }
                else if (value == "Log10")
                {
                    rbLog10Value.Checked = true;
                    //dblDecisionThreshold = Math.Pow(10, Convert.ToDouble(tbThresholdDec.Text));
                    //dblMandateThreshold = Math.Pow(10, Convert.ToDouble(tbThresholdReg.Text));
                    rbLog10Value_CheckedChanged(this, args);
                }
                else if (value == "Power")
                {
                    rbPwrValue.Checked = true;
                    double pwr = Convert.ToDouble(txtPwrValue.Text);
                    //dblDecisionThreshold = Math.Pow(Convert.ToDouble(tbThresholdDec.Text), 1/pwr);
                    //dblMandateThreshold = Math.Pow(Convert.ToDouble(tbThresholdReg.Text), 1/pwr);
                    rbPwrValue_CheckedChanged(this, args);
                }
                UpdateResults(lstXYPlotdata);
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
            if (RawPredictions)
            {
                if (dblDecisionThreshold > maxX) maxX = dblMandateThreshold; //_decisionThreshold;
                if (dblMandateThreshold > maxY) maxY = dblDecisionThreshold; //_mandateThreshold;
            }
            else
            {
                if (dblProbabilityThreshold > maxX) maxX = dblMandateThreshold; //_decisionThreshold;
                if (dblMandateThreshold > maxY) maxY = dblProbabilityThreshold; //_mandateThreshold;
            }

            //find the model error counts for the XYplot display
            ModelErrorCounts mec = new ModelErrorCounts();
            if (RawPredictions) { mec.getCounts(dblDecisionThreshold, dblMandateThreshold, data); }
            else { mec.getCounts(dblProbabilityThreshold, dblMandateThreshold, data); }
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
                if (RawPredictions)
                {
                    list2.Add(minX - 1, dblDecisionThreshold);
                    list2.Add(maxX + 1, dblDecisionThreshold);
                }
                else
                {
                    list2.Add(minX - 1, dblProbabilityThreshold);
                    list2.Add(maxX + 1, dblProbabilityThreshold);
                }
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
                xfrmLast = xfrmCurrent;
                xfrmCurrent = DependentVariableTransforms.none;

                dblLastExponent = dblCurrentExponent;
                dblCurrentExponent = 1;

                double tv = double.NaN;
                double th = double.NaN;
                try
                {
                    //tv = VBCommon.Transforms.Apply.TransformThreshold(VBCommon.Transforms.Apply.UntransformThreshold(Convert.ToDouble(tbThresholdReg.Text), xfrmCurrent, dblCurrentExponent), DependentVariableTransforms.none);
                    tv = VBCommon.Transforms.Apply.TransformThreshold(Convert.ToDouble(tbThresholdReg.Text), xfrmDisplay, dblDisplayXfrmExponent);
                    //if (rbRaw.Checked) { th = VBCommon.Transforms.Apply.TransformThreshold(VBCommon.Transforms.Apply.UntransformThreshold(Convert.ToDouble(tbThresholdDec.Text), xfrmCurrent, dblCurrentExponent), DependentVariableTransforms.none); }
                    if (rbRaw.Checked) { th = VBCommon.Transforms.Apply.TransformThreshold(Convert.ToDouble(tbThresholdDec.Text), xfrmDisplay, dblDisplayXfrmExponent); }
                    else { th = Convert.ToDouble(txtProbabilityThreshold.Text); }

                    //tbThresholdReg.Text = tv.ToString();
                    //if (rbRaw.Checked) { tbThresholdDec.Text = th.ToString(); }

                    //tv = Convert.ToDouble(tbThresholdReg.Text.ToString());
                    //th = Convert.ToDouble(tbThresholdDec.Text.ToString());
                }
                catch
                {
                    string msg = @"Cannot convert thresholds. (" + tbThresholdDec.Text + ", " + tbThresholdReg.Text + ") ";
                    MessageBox.Show(msg);
                    return;
                }

                dblMandateThreshold = tv;
                if (rbRaw.Checked) { dblDecisionThreshold = th; }
                else { dblProbabilityThreshold = th; }

                //lstXYPlotdata = TransformData(lstXYPlotdata);
            }
        }


        //transform type changed
        private void rbLog10Value_CheckedChanged(object sender, EventArgs e)
        {
            if (rbLog10Value.Checked)
            {
                double tv = double.NaN;
                double th = double.NaN;
                
                /*try
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
                }*/
                xfrmLast = xfrmCurrent;
                xfrmCurrent = DependentVariableTransforms.Log10;

                dblLastExponent = dblCurrentExponent;
                dblCurrentExponent = 1;

                try
                {
                    //tv = VBCommon.Transforms.Apply.TransformThreshold(VBCommon.Transforms.Apply.UntransformThreshold(Convert.ToDouble(tbThresholdReg.Text), xfrmCurrent, dblCurrentExponent), DependentVariableTransforms.Log10);
                    //if (rbRaw.Checked) { th = VBCommon.Transforms.Apply.TransformThreshold(VBCommon.Transforms.Apply.UntransformThreshold(Convert.ToDouble(tbThresholdDec.Text), xfrmCurrent, dblCurrentExponent), DependentVariableTransforms.Log10); }
                    tv = VBCommon.Transforms.Apply.TransformThreshold(VBCommon.Transforms.Apply.UntransformThreshold(Convert.ToDouble(tbThresholdReg.Text), DependentVariableTransforms.Log10), xfrmDisplay, dblDisplayXfrmExponent);
                    if (rbRaw.Checked) { th = VBCommon.Transforms.Apply.TransformThreshold(VBCommon.Transforms.Apply.UntransformThreshold(Convert.ToDouble(tbThresholdDec.Text), DependentVariableTransforms.Log10), xfrmDisplay, dblDisplayXfrmExponent); }
                    else { th = Convert.ToDouble(txtProbabilityThreshold.Text); }

                    //tbThresholdReg.Text = tv.ToString();
                    //if (rbRaw.Checked) { tbThresholdDec.Text = th.ToString(); }
                    //tv = Math.Log10(Convert.ToDouble(tbThresholdReg.Text.ToString()));
                    //th = Math.Log10(Convert.ToDouble(tbThresholdDec.Text.ToString()));
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
                if (rbRaw.Checked) { dblDecisionThreshold = th; }
                else { dblProbabilityThreshold = th; }

                //lstXYPlotdata = TransformData(lstXYPlotdata);
            }
        }


        //transform type changed
        private void rbLogeValue_CheckedChanged(object sender, EventArgs e)
        {
            if (rbLogeValue.Checked)
            {
                double tv = double.NaN;
                double th = double.NaN;

                xfrmLast = xfrmCurrent;
                xfrmCurrent = DependentVariableTransforms.Ln;

                dblLastExponent = dblCurrentExponent;
                dblCurrentExponent = 1;

                try
                {
                    //tv = VBCommon.Transforms.Apply.TransformThreshold(VBCommon.Transforms.Apply.UntransformThreshold(Convert.ToDouble(tbThresholdReg.Text), xfrmCurrent, dblCurrentExponent), DependentVariableTransforms.Ln);
                    //if (rbRaw.Checked) { th = VBCommon.Transforms.Apply.TransformThreshold(VBCommon.Transforms.Apply.UntransformThreshold(Convert.ToDouble(tbThresholdDec.Text), xfrmCurrent, dblCurrentExponent), DependentVariableTransforms.Ln); }
                    tv = VBCommon.Transforms.Apply.TransformThreshold(VBCommon.Transforms.Apply.UntransformThreshold(Convert.ToDouble(tbThresholdReg.Text), DependentVariableTransforms.Ln), xfrmDisplay, dblDisplayXfrmExponent);
                    if (rbRaw.Checked) { th = VBCommon.Transforms.Apply.TransformThreshold(VBCommon.Transforms.Apply.UntransformThreshold(Convert.ToDouble(tbThresholdDec.Text), DependentVariableTransforms.Ln), xfrmDisplay, dblDisplayXfrmExponent); }
                    else { th = Convert.ToDouble(txtProbabilityThreshold.Text); }

                    //tbThresholdReg.Text = tv.ToString();
                    //if (rbRaw.Checked) { tbThresholdDec.Text = th.ToString(); }

                    //tv = Math.Log(Convert.ToDouble(tbThresholdReg.Text.ToString()));
                    //th = Math.Log(Convert.ToDouble(tbThresholdDec.Text.ToString()));
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
                if (rbRaw.Checked) { dblDecisionThreshold = th; }
                else { dblProbabilityThreshold = th; }

                //lstXYPlotdata = TransformData(lstXYPlotdata);
            }
        }


        //transform type changed
        private void rbPwrValue_Changed(object sender, EventArgs e)
        {
            if (rbPwrValue.Checked)
            {
                double tv = double.NaN;
                double th = double.NaN;
                double power;                               

                try
                {
                    xfrmLast = xfrmCurrent;
                    xfrmCurrent = DependentVariableTransforms.Power;

                    dblLastExponent = dblCurrentExponent;
                    dblCurrentExponent = power = Convert.ToDouble(txtPwrValue.Text);
                    //tv = VBCommon.Transforms.Apply.TransformThreshold(VBCommon.Transforms.Apply.UntransformThreshold(Convert.ToDouble(tbThresholdReg.Text), xfrmCurrent, dblCurrentExponent), DependentVariableTransforms.Power, power);
                    //if (rbRaw.Checked) { th = VBCommon.Transforms.Apply.TransformThreshold(VBCommon.Transforms.Apply.UntransformThreshold(Convert.ToDouble(tbThresholdDec.Text), xfrmCurrent, dblCurrentExponent), DependentVariableTransforms.Power, power); }
                    tv = VBCommon.Transforms.Apply.TransformThreshold(VBCommon.Transforms.Apply.UntransformThreshold(Convert.ToDouble(tbThresholdReg.Text), DependentVariableTransforms.Power, dblCurrentExponent), xfrmDisplay, dblDisplayXfrmExponent);
                    if (rbRaw.Checked) { th = VBCommon.Transforms.Apply.TransformThreshold(VBCommon.Transforms.Apply.UntransformThreshold(Convert.ToDouble(tbThresholdDec.Text), DependentVariableTransforms.Power, dblCurrentExponent), xfrmDisplay, dblDisplayXfrmExponent); }
                    else { th = Convert.ToDouble(txtProbabilityThreshold.Text); }

                    //tbThresholdReg.Text = tv.ToString();
                    //if (rbRaw.Checked) { tbThresholdDec.Text = th.ToString(); }

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

                dblMandateThreshold = tv;
                if (rbRaw.Checked) { dblDecisionThreshold = th; }
                else { dblProbabilityThreshold = th; }

                //lstXYPlotdata = TransformData(lstXYPlotdata);
            }
        }

        private List<double[]> TransformData(List<double[]> Data)
        {
            List<double[]> transformed = new List<double[]>();

            foreach (double[] dataPair in Data)
            {
                double[] transformedPair = new double[2];
                transformedPair[0] = VBCommon.Transforms.Apply.UntransformThreshold(dataPair[0], xfrmLast, dblLastExponent);
                if (rbRaw.Checked) { transformedPair[1] = VBCommon.Transforms.Apply.UntransformThreshold(dataPair[1], xfrmLast, dblLastExponent); }
                else { transformedPair[1] = dataPair[1]; }

                transformedPair[0] = VBCommon.Transforms.Apply.TransformThreshold(transformedPair[0], xfrmCurrent, dblCurrentExponent);
                if (rbRaw.Checked) { transformedPair[1] = VBCommon.Transforms.Apply.TransformThreshold(transformedPair[1], xfrmCurrent, dblCurrentExponent); }
                else { transformedPair[1] = dataPair[1]; }

                transformed.Add(transformedPair);
            }

            return transformed;
        }
    }
}
