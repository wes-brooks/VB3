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
using System.Globalization;
using IPyCommon;
using VBTools;
using Newtonsoft.Json;

namespace IPyModeling
{
    [JsonObject]
    public abstract partial class IPyModelingControl : UserControl
    {
        //Get access to the IronPython interface:
        private dynamic ipyInterface = IPyInterface.Interface;
        protected dynamic ipyModel = null;

        //Class member definitions:
        //Events:
        public delegate void EventHandler<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
        public event EventHandler<LogMessageEvent> LogMessageSent;
        public event EventHandler<MessageEvent> MessageSent;
        public event EventHandler<UpdateModelEvent> UpdateReadyForModel;
        public event EventHandler ModelSaveRequested;
        public event EventHandler ModelingComplete;

        //Delegates to get data from Virtual Beach
        public delegate void RequestData(object sender, EventArgs args);
        public RequestData TabPageEntered;
        public event EventHandler<ModelingCallback> DataRequested;
        public event EventHandler IronPythonInterfaceRequested;
        protected DataTable model_data;

        //Related to control of graphical interface elements:
        protected int intThresholdIndex;
        protected List<double> listCandidateSpecificity;
        protected List<double> listCandidateThresholds;
        protected List<double> listValidationSpecificity = new List<double>();
        protected List<double> listTruePos = new List<double>();
        protected List<double> listTrueNeg = new List<double>();
        protected List<double> listFalsePos = new List<double>();
        protected List<double> listFalseNeg = new List<double>();
        protected bool boolControlStatus = true;
        protected bool boolInitialControlStatus = true;

        //Related to underlying model:
        protected double dblMandateThreshold;
        protected string strMethod;
        protected Transform transform = new Transform(DependentVariableTransforms.none);

        private string strThreshold;
        private string strExponent;
        private bool boolThresholdingButtonsVisible;

        //A flag to indicate whether this modeling tab has been used.
        protected bool boolVirgin;
        protected bool boolClean = true;

        public IPyModelingControl()
        {
            InitializeComponent();
            boolVirgin = true;
            btnSelectModel.Enabled = false;

            //initialize the regulatory threshold
            EventArgs e = new EventArgs();
            rbValue_CheckedChanged(this, e);
            rbLogeValue_CheckedChanged(this, e);
            rbLog10Value_CheckedChanged(this, e);
            rbPower_CheckedChanged(this, e);

            //Request access to the IronPython interface.
            RequestIronPythonInterface();

            //Create the delegate that will raise the event that requests model data
            this.TabPageEntered += new RequestData(RequestModelData);
        }


        //Return the IronPython model object
        [JsonProperty]
        public dynamic Model
        {
            get { return this.ipyModel; }
        }


        //Gets or Sets the method to use to make the model.
        [JsonProperty]
        public string Method
        {
            get { return strMethod; }
            set { strMethod = value; }
        }


        //Get the list of the number of true positives
        [JsonProperty]
        public List<double> TruePositives
        {
            get { return this.listTruePos; }
        }


        //Get the list of the number of true negatives
        [JsonProperty]
        public List<double> TrueNegatives
        {
            get { return this.listTrueNeg; }
        }


        //Get the list of the number of false positives
        [JsonProperty]
        public List<double> FalsePositives
        {
            get { return this.listFalsePos; }
        }


        //Get the list of the number of false negatives
        [JsonProperty]
        public List<double> FalseNegatives
        {
            get { return this.listFalseNeg; }
        }


        //Get the list of observed specificities in cross-validation
        [JsonProperty]
        public List<double> ValidationSpecificities
        {
            get { return this.listValidationSpecificity; }
        }


        //Get the list of unique specificities for setting the model's decision threshold 
        [JsonProperty]
        public List<double> CandidateSpecificities
        {
            get { return this.listCandidateSpecificity; }
        }


        //Get the list of possible threshold values
        [JsonProperty]
        public List<double> Thresholds
        {
            get { return this.listCandidateThresholds; }
        }


        //Get the list of possible threshold values
        [JsonProperty]
        public int ThresholdingIndex
        {
            get { return this.intThresholdIndex; }
        }


        //Get the regulatory threshold
        [JsonProperty]
        public string ThresholdTextbox
        {
            get { return this.tbThreshold.Text; }
        }


        //Get the exponent for a power transform
        [JsonProperty]
        public string ExponentTextbox
        {
            get { return this.tbExponent.Text; }
        }


        //Get the regulatory threshold
        [JsonProperty]
        public string DecisionThresholdLabel
        {
            get { return this.lblDecisionThreshold.Text; }
        }


        //Return a flag indicating whether this modeling tab has been touched.
        public bool VirginState
        {
            get { return this.boolVirgin; }
        }


        //Return a flag indicating whether this modeling tab has been modified since the model was last exported.
        public bool Clean
        {
            get { return this.boolClean; }
        }


        //Return a flag indicating whether this modeling tab has been touched.
        public bool ThresholdingButtonsVisible
        {
            get { return this.pnlThresholdingButtons.Visible; }
        }


        //Get the transform of the dependent variable
        public Transform DependentVariableTransform
        {
            get { return transform; }
        }


        public void SetModelData(DataTable data)
        {
            this.model_data = data;

            int i = 0;
            bool finished = false;            
            while (!finished)
            {
                if (data.Columns[i].ExtendedProperties.ContainsKey("dependentvariable"))
                {
                    if (data.Columns[i].ExtendedProperties.ContainsKey("responsevardefinedtransform"))
                    {
                        //Unpack the user's selected transformation of the dependent variable.
                        string transform = data.Columns[i].ExtendedProperties["responsevardefinedtransform"].ToString();
                                               
                        if (String.Compare(transform, DependentVariableTransforms.none.ToString(), 0) == 0)
                            this.rbValue.Checked = true;
                        else if (String.Compare(transform, DependentVariableTransforms.Ln.ToString(), 0) == 0)
                            this.rbLoge.Checked = true;
                        else if (String.Compare(transform, DependentVariableTransforms.Log10.ToString(), 0) == 0)
                            this.rbLog10.Checked = true;
                        else if (String.Compare(transform, DependentVariableTransforms.Power.ToString(), 0) == 0)
                            this.rbPower.Checked = true;
                        else
                            this.rbValue.Checked = true;

                        ChangeControlStatus(false);
                        ChangeThresholdControlStatus(true);
                        finished = true;
                    }
                }

                //Have we checked all of the columns?
                i++;
                if (data.Columns.Count == i)
                    finished = true;
            }
        }

        
        //Clear the control
        public void Clear()
        {
            ipyModel = null;

            chartValidation.Series["True positives"].Points.Clear();
            chartValidation.Series["True negatives"].Points.Clear();
            chartValidation.Annotations.Clear();
            chartValidation.Update();

            listValidationSpecificity.Clear();
            listTruePos.Clear();
            listTrueNeg.Clear();
            listFalsePos.Clear();
            listFalseNeg.Clear();

            listCandidateSpecificity = null;
            listCandidateThresholds = null;

            lvModel.Items.Clear();
            lvValidation.Items.Clear();

            tbThreshold.Text = "235";
            lblDecisionThreshold.Text = "";
            lblSpec.Text = "";
            
            btnRun.Text = "Run";
            btnSelectModel.Enabled = false;
            pnlThresholdingButtons.Visible = false;
            boolVirgin = true;
        }


        //Raise a request for access to the IronPython interface - should be raised when the control is created.
        protected void RequestIronPythonInterface()
        {
            if (IronPythonInterfaceRequested != null)
            {
                EventArgs e = new EventArgs();
                IronPythonInterfaceRequested(this, e);
            }
        }


        //Raise a request for access to the model data - should be raised by the containing TabPage when the tab is entered.
        protected void RequestModelData(object sender, EventArgs args)
        {
            if (DataRequested != null)
            {
                ModelingCallback e = new ModelingCallback(SetModelData);
                DataRequested(this, e);
            }
        }


        //Raise a MessageEvent (passes a message to the container, which should be logged)
        protected virtual void Log(string message, LogMessageEvent.Intents intent, LogMessageEvent.Targets target)
        {
            if (LogMessageSent != null)
            {
                LogMessageEvent e = new LogMessageEvent(message, intent, target);
                LogMessageSent(this, e);
            }
        }


        //This method alerts the container that we need data. The container should then use the Set property of sender.data
        protected void StartModeling()
        {
            if (DataRequested != null)
            {
                ModelingCallback e = new ModelingCallback(MakeModel);
                DataRequested(this, e);
            }
        }


        //Raise a MessageEvent (passes a message to the container, which should be logged)
        protected virtual void TellManager(string message)
        {
            if (MessageSent != null)
            {
                MessageEvent e = new MessageEvent(message);
                MessageSent(this, e);
            }
        }


        //Enable or disable controls, then raise an event to do the same up the chain in the containing Form.
        protected void ChangeControlStatus(bool enable)
        {
            boolControlStatus = enable;

            rbLog10.Invoke((MethodInvoker)delegate
            {
                rbLog10.Enabled = enable;
            });

            rbLoge.Invoke((MethodInvoker)delegate
            {
                rbLoge.Enabled = enable;
            });

            rbValue.Invoke((MethodInvoker)delegate
            {
                rbValue.Enabled = enable;
            });

            rbPower.Invoke((MethodInvoker)delegate
            {
                rbPower.Enabled = enable;
            });

            tbThreshold.Invoke((MethodInvoker)delegate
            {
                tbThreshold.Enabled = enable;
            });

            tbExponent.Invoke((MethodInvoker)delegate
            {
                tbExponent.Enabled = enable;
            });
        }


        //Enable or disable the regulatory threshold control, then raise an event to do the same up the chain in the containing Form.
        protected void ChangeThresholdControlStatus(bool enable)
        {
            tbThreshold.Invoke((MethodInvoker)delegate
            {
                tbThreshold.Enabled = enable;
            });
        }
		
		//This button runs or cancels the modeling method associated with this pane.
        public void btnRun_Click(object sender, EventArgs e)
        {
            //Now this modeling tab has been touched.
            boolVirgin = false;

			//Start running the model-building code.
            if (btnRun.Text == "Run")
            {
                if (ipyInterface == null) RequestIronPythonInterface();
                btnRun.Text = "Cancel";
                boolInitialControlStatus = boolControlStatus;
                ChangeControlStatus(false);
                StartModeling();

                Log("0", LogMessageEvent.Intents.UserOnly, LogMessageEvent.Targets.ProgressBar);
                Application.DoEvents();
            }

			//Cancel in-progress model-building
            else if (btnRun.Text == "Cancel")
            {
                btnRun.Text = "Run";
                ChangeControlStatus(boolInitialControlStatus);
                ChangeThresholdControlStatus(true);
                Application.DoEvents();
                return;
            }

            Application.DoEvents();
        }
		
		
		//Run the model-building process.
        //This is the callback function that Virtual Beach will use to run the modeling process.
        protected void MakeModel(DataTable Data)
        {
            //Set up the local variables we'll need for model-building.
            DataTable tblData = Data;
            double specificity = 0.9;
            string target = tblData.Columns[1].Caption;
            double threshold = dblMandateThreshold;

            //Remove the ID field:
            tblData.Columns.Remove(tblData.Columns[0].Caption);

            //Run the IronPython model-building code, then call PopulateResults to display the coefficients and the decision threshold.
            dynamic validation_results = ipyInterface.Validate(tblData, target, specificity, regulatory_threshold:threshold, method:strMethod);
            this.ipyModel = validation_results[1];
			PopulateResults(this.ipyModel);
			
			//Now extract the valid thresholds and the corresponding specificities
            specificity = Convert.ToDouble(ipyModel.specificity);
            dynamic thresholding = ipyInterface.GetPossibleSpecificities(this.ipyModel);
            this.listCandidateThresholds = ((IList<object>)(((IList<object>)thresholding)[0])).Cast<double>().ToList();
            this.listCandidateSpecificity = ((IList<object>)(((IList<object>)thresholding)[1])).Cast<double>().ToList();
			
			//The following is a bit of python code to initialize the decision threshold at the default specificity.
            this.intThresholdIndex = (int)thresholding[1].index(specificity);

			//Extract the number of false positives, true positives, false negatives, and true negatives.
            object counts = ipyInterface.SpecificityChart(validation_results[0]);
            List<object> x = ((IList<object>)((IList<object>)counts)[0]).Cast<object>().ToList();
            List<object> tp = ((IList<object>)((IList<object>)counts)[1]).Cast<object>().ToList();
            List<object> tn = ((IList<object>)((IList<object>)counts)[2]).Cast<object>().ToList();
            List<object> fp = ((IList<object>)((IList<object>)counts)[3]).Cast<object>().ToList();
            List<object> fn = ((IList<object>)((IList<object>)counts)[4]).Cast<object>().ToList();

			//Convert the validation counts to doubles and move them from the temporary lists above into more permanent lists.
            for (int i=0;  i<x.Count; i++)
            {
                listValidationSpecificity.Add(Convert.ToDouble(x[i]));
                listTruePos.Add(Convert.ToDouble(tp[i]));
                listTrueNeg.Add(Convert.ToDouble(tn[i]));
                listFalsePos.Add(Convert.ToDouble(fp[i]));
                listFalseNeg.Add(Convert.ToDouble(fn[i]));
            }

            InitializeValidationChart();
            AnnotateChart();

			//Enable the thresholding controls and the model-selection button.
            pnlThresholdingButtons.Visible = true;
            btnRun.Text = "Run";
            ChangeControlStatus(boolInitialControlStatus);
            ChangeThresholdControlStatus(true);
            btnSelectModel.Enabled = true;

			//Work's done; let's go home.
            if (ModelingComplete != null)
            {
                EventArgs e = new EventArgs();
                ModelingComplete(this, e);
            }
            UpdatePredictionTab();
            return;
        }


        protected void InitializeValidationChart()
        {
            //Set up the plotting area to show the validation results
            chartValidation.ChartAreas[0].AxisX.Minimum = 0;
            chartValidation.ChartAreas[0].AxisX.Maximum = 1;

            //Make sure the chart is clear before drawing on it.
            chartValidation.Series["True positives"].Points.Clear();
            chartValidation.Series["True negatives"].Points.Clear();

            //Plot the validation results (true positives and true negatives versus specificity).
            for (int i = 0; i < listValidationSpecificity.Count; i++)
            {
                chartValidation.Series["True positives"].Points.AddXY(xValue: listValidationSpecificity[i], yValue: listTruePos[i]);
                chartValidation.Series["True negatives"].Points.AddXY(xValue: listValidationSpecificity[i], yValue: listTrueNeg[i]);
            }

            //Finish drawing the chart and then add the thresholding line.
            chartValidation.Series["True positives"].YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
            chartValidation.Update();
        }


        protected virtual void PopulateResults(dynamic model)
        {
            
        }


        protected void AnnotateChart()
        {
			//Set the threshold with the given specificity.
            double specificity = this.listCandidateSpecificity[this.intThresholdIndex];
            ipyModel.Threshold(specificity);
            lblDecisionThreshold.Text = String.Format("{0:F3}", UntransformThreshold((double)ipyModel.threshold));

            //Locate the specificity annotation
            lblSpec.Text = "Specificity: " + Convert.ToString( Math.Round(value:specificity, digits:3) );
            int xLoc = (int)chartValidation.ChartAreas[0].AxisX.ValueToPixelPosition(specificity) + chartValidation.Location.X - (int)(lblSpec.Size.Width / 2);
            lblSpec.Location = new Point(x:xLoc, y:6);
            lblSpec.Visible = true;

			//Format the threshold line and draw it on the chart.
            chartValidation.Annotations.Clear();
            System.Windows.Forms.DataVisualization.Charting.VerticalLineAnnotation myLine = new System.Windows.Forms.DataVisualization.Charting.VerticalLineAnnotation();
            myLine.X = chartValidation.ChartAreas[0].AxisX.ValueToPosition(specificity);
            myLine.AxisY = chartValidation.ChartAreas[0].AxisY;
            double yMax = chartValidation.ChartAreas[0].AxisY.Maximum;
            double yMin = chartValidation.ChartAreas[0].AxisY.Minimum;
            myLine.Y = yMax;
            myLine.Height = chartValidation.ChartAreas[0].AxisY.ValueToPosition(yMin) - chartValidation.ChartAreas[0].AxisY.ValueToPosition(yMax);
            myLine.Visible = true;
            myLine.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartValidation.Annotations.Add(myLine);
            chartValidation.Update();

			//Summarize the model's performance in the validation ListView.
            List<double> dblCandidates = listValidationSpecificity.Where(arg => arg <= specificity).ToList();
            int index = listValidationSpecificity.FindIndex(arg => arg==dblCandidates.Max());
            string[] listValidation = new string[4] { listTruePos[index].ToString(), listTrueNeg[index].ToString(), listFalsePos[index].ToString(), listFalseNeg[index].ToString() };

            //Add the row to the listview, coloring it red if this variable is considered to have minor influence.
            ListViewItem lvi = new ListViewItem(listValidation);
            lvValidation.Items.Clear();
            lvValidation.Items.Add(lvi);
        }


        //
        protected double UntransformThreshold(double value)
        {
            if (this.DependentVariableTransform.Type == DependentVariableTransforms.none)
                return value;
            else if (this.DependentVariableTransform.Type == DependentVariableTransforms.Log10)
                return Math.Pow(10, value);
            else if (this.DependentVariableTransform.Type == DependentVariableTransforms.Ln)
                return Math.Exp(value);
            else if (this.DependentVariableTransform.Type == DependentVariableTransforms.Power)
                return Math.Pow(value, 1/this.DependentVariableTransform.Exponent);
            else
                return value;
        }
		

        //Save the modeling status
        public IDictionary<string, object> PackProjectState()
        {
            IDictionary<string, object> dictPluginState = new Dictionary<string, object>();

            if (Model == null)
                return dictPluginState;

            //Save the model state
            ModelState modelState = new ModelState(Model);
            modelState.ModelString = ipyInterface.Serialize(Model);
            modelState.DependentVariableTransform = DependentVariableTransform;
            modelState.Method = Method;

            try
            {
                modelState.RegulatoryThreshold = Convert.ToDouble(ThresholdTextbox);
                modelState.DecisionThreshold = Convert.ToDouble(DecisionThresholdLabel);
            }
            catch (InvalidCastException)
            { //return ;
            }

            dictPluginState.Add("Model", modelState);
            
            //Save the project state
            //Save the lists that we use to make the validation chart
            Dictionary<string, List<double>> dictValidation = new SerializableDictionary<string, List<double>>();
            dictValidation.Add("tpos", TruePositives);
            dictValidation.Add("tneg", TrueNegatives);
            dictValidation.Add("fpos", FalsePositives);
            dictValidation.Add("fneg", FalseNegatives);
            dictValidation.Add("specificity", ValidationSpecificities);

            dictPluginState.Add("ValidationDictionary", dictValidation);
            
            //Save the lists that we use to set the model's decision threshold.
            Dictionary<string, List<double>>  dictThresholding = new Dictionary<string, List<double>>();
            dictThresholding.Add("specificity", CandidateSpecificities);
            dictThresholding.Add("threshold", Thresholds);
            dictPluginState.Add("ThresholdingDictionary", dictThresholding);
            dictPluginState.Add("ThresholdingIndex", intThresholdIndex);
            
            //Save the state of UI elements.
            dictPluginState.Add("ThresholdTextbox", strThreshold);
            dictPluginState.Add("ExponentTextbox", strExponent);
            dictPluginState.Add("VirginState", boolVirgin);
            dictPluginState.Add("ThresholdingButtonsVisible", boolThresholdingButtonsVisible);

            return dictPluginState;
        }


        //Reconstruct the saved modeling status
        public void UnpackProjectState(IDictionary<string, object> dictProjectState)
        {
            //Dictionary<string, object> dictPluginState = new Dictionary<string, object>();
            //dictPluginState = (Dictionary<string, object>)objProjectState;

            if (dictProjectState.Count == 0) return;

            //Unpack the virgin status of the project
            this.boolVirgin = (bool)dictProjectState["VirginState"];

            if (!boolVirgin)
            {
                //Unpack the lists that go into making the validation chart.
                Dictionary<string, List<double>> dictValidation = (Dictionary<string, List<double>>)dictProjectState["ValidationDictionary"];
                
                this.listValidationSpecificity = dictValidation["specificity"];
                this.listTruePos = dictValidation["tpos"];
                this.listTrueNeg = dictValidation["tneg"];
                this.listFalsePos = dictValidation["fpos"];
                this.listFalseNeg = dictValidation["fneg"];

                //Unpack the lists that are used to set the model's decision threshold
                Dictionary<string, List<double>> dictThresholding = new Dictionary<string, List<double>>();

                dictThresholding = (Dictionary<string, List<double>>)dictProjectState["ThresholdingDictionary"];

                this.listCandidateSpecificity = dictThresholding["specificity"];
                this.listCandidateThresholds = dictThresholding["threshold"];

                this.intThresholdIndex = (int)dictProjectState["ThresholdingIndex"];

                //Unpack the contents of the threshold and exponent text boxes
                this.tbExponent.Text = (string)dictProjectState["ExponentTextbox"];
                this.tbThreshold.Text = (string)dictProjectState["ThresholdTextBox"];

                //Unpack the user's selected transformation of the dependent variable.
                ModelState modelState = (ModelState)dictProjectState["Model"];

                if (modelState.DependentVariableTransform.Type == DependentVariableTransforms.none)
                    this.rbValue.Checked = true;
                //if (project.ModelState.DependentVariableTransform.Type == DependentVariableTransforms.none)
                else if (modelState.DependentVariableTransform.Type == DependentVariableTransforms.Ln)
                    this.rbLoge.Checked = true;
                else if (modelState.DependentVariableTransform.Type == DependentVariableTransforms.Log10)
                    this.rbLog10.Checked = true;
                else if (modelState.DependentVariableTransform.Type == DependentVariableTransforms.Power)
                    this.rbPower.Checked = true;
                else
                    this.rbValue.Checked = true;

                //Now make sure the selected transformation is reflected behind the scenes, too.
                EventArgs e = new EventArgs();
                rbValue_CheckedChanged(this, e);
                rbLogeValue_CheckedChanged(this, e);
                rbLog10Value_CheckedChanged(this, e);
                rbPower_CheckedChanged(this, e);

                //Restore the model state.
                UnpackModelState(modelState);

                //Now restore the elements of the user interface.
                this.pnlThresholdingButtons.Visible = (bool)dictProjectState["ThresholdingButtonsVisible"];
                this.btnSelectModel.Enabled = (bool)dictProjectState["ThresholdingButtonsVisible"];
                PopulateResults(this.ipyModel);
                InitializeValidationChart();
                AnnotateChart();
            }             
        }


        //Save the model status
        public ModelState PackModelState()
        {
            if (ThresholdingButtonsVisible)
            {
                ProjectState project = new ProjectState(this);
                ModelState model = project.ModelState;
                boolClean = true;
                return model;
            }
            else
                return null;
        }


        //Reconstruct the saved modeling status
        public void UnpackModelState(ModelState model)
        {
            if (ipyInterface == null) RequestIronPythonInterface();
            this.ipyModel = ipyInterface.Deserialize(model.ModelString);
            this.Method = model.Method;
        }
		

        //Handle a click of the modeling "Go" button
        public void btnSelectModel_Click(object sender, EventArgs e)
        {
            //IDictionary<string, object> packedState = new Dictionary<string, object>();
            //packedState = PackProjectState();
            //IPyModeling.IPyModelingPlugin frmState = new IPyModeling.IPyModelingPlugin();
            //frmState.Broadcast(packedState);;
            if (ModelSaveRequested != null)
            {
                EventArgs args = new EventArgs();
                ModelSaveRequested(this, args);
            }
        }


        protected void btnLeft25_Click(object sender, EventArgs e)
        {
            if (this.intThresholdIndex >= 25)
                this.intThresholdIndex -= 25;
            else
                this.intThresholdIndex = 0;

            AnnotateChart();
            UpdatePredictionTab();
            boolClean = false;
        }


        protected void btnLeft1_Click(object sender, EventArgs e)
        {
            if (this.intThresholdIndex >= 1)
                this.intThresholdIndex -= 1;
            else
                this.intThresholdIndex = 0;

            AnnotateChart();
            UpdatePredictionTab();
            boolClean = false;
        }


        protected void btnRight1_Click(object sender, EventArgs e)
        {
            if (this.intThresholdIndex < this.listCandidateThresholds.Count - 1)
                this.intThresholdIndex += 1;
            else
                this.intThresholdIndex = this.listCandidateThresholds.Count - 1;

            AnnotateChart();
            UpdatePredictionTab();
            boolClean = false;
        }


        protected void btnRight25_Click(object sender, EventArgs e)
        {
            if (this.intThresholdIndex < this.listCandidateThresholds.Count - 25)
                this.intThresholdIndex += 25;
            else
                this.intThresholdIndex = this.listCandidateThresholds.Count - 1;

            AnnotateChart();
            UpdatePredictionTab();
            boolClean = false;
        }


        protected void rbValue_CheckedChanged(object sender, EventArgs e)
        {
            if (rbValue.Checked)
            {
                double tv = double.NaN;
				
                try
                {
                    tv = Convert.ToDouble(tbThreshold.Text.ToString());
                }
                catch
                {
                    string msg = @"Cannot convert threshold. (" + tbThreshold.Text + ") ";
                    MessageBox.Show(msg);
                    return;
                }

                dblMandateThreshold = tv;
                transform.Type = DependentVariableTransforms.none;
            }
            boolClean = false;
        }


        protected void rbLog10Value_CheckedChanged(object sender, EventArgs e)
        {
            if (rbLog10.Checked)
            {
                double tv = double.NaN;
				
                //ms has no fp error checking... check for all conditions.
                //log10(x) when x == 0 results in NaN and when x < 0 results in -Infinity
                try
                {
                    tv = Math.Log10(Convert.ToDouble(tbThreshold.Text.ToString()));
                }
                catch
                {
                    string msg = @"Cannot Log 10 transform threshold. (" + tbThreshold.Text + ") ";
                    MessageBox.Show(msg);
                    return;
                }
				
                if (tv.Equals(double.NaN))
                {
                    string msg = @"Entered value must be greater than 0. (" + tbThreshold.Text + ") ";
                    MessageBox.Show(msg);
                    return;
                }
				
                if (tv < 0 )
                {
                    string msg = @"Entered value must be greater than 0. (" + tbThreshold.Text + ") ";
                    MessageBox.Show(msg);
                    return;
                }

                dblMandateThreshold = tv;
                transform.Type = DependentVariableTransforms.Log10;
            }
            boolClean = false;
        }


        protected void rbLogeValue_CheckedChanged(object sender, EventArgs e)
        {
            if (rbLoge.Checked)
            {
                double tv = double.NaN;
				
                //ms has no fp error checking... check for all conditions.
                //loge(x) when x == 0 results in NaN and when x < 0 results in -Infinity
                try
                {
                    tv = Math.Log(Convert.ToDouble(tbThreshold.Text.ToString()));
                }
                catch
                {
                    string msg = @"Cannot Log e transform threshold. (" + tbThreshold.Text + ") ";
                    MessageBox.Show(msg);
                    return;
                }
				
                if (tv.Equals(double.NaN))
                {
                    string msg = @"Entered value must be greater than 0. (" + tbThreshold.Text + ") ";
                    MessageBox.Show(msg);
                    return;
                }
				
                if (tv < 0)
                {
                    string msg = @"Entered value must be greater than 0. (" + tbThreshold.Text + ") ";
                    MessageBox.Show(msg);
                    return;
                }

                dblMandateThreshold = tv;
                transform.Type = DependentVariableTransforms.Ln;
            }
            boolClean = false;
        }


        protected void rbPower_CheckedChanged(object sender, EventArgs e)
        {
            if (rbPower.Checked)
                tbExponent.Enabled = true;
            else
                tbExponent.Enabled = false;

            if (rbPower.Checked)
            {
                double tv = double.NaN;

                //ms has no fp error checking... check for all conditions.
                //loge(x) when x == 0 results in NaN and when x < 0 results in -Infinity
                try
                {
                    tv = Math.Pow(Convert.ToDouble(tbThreshold.Text.ToString()), Convert.ToDouble(tbExponent.Text.ToString()));
                }
                catch
                {
                    string msg = @"Cannot exponentiate threshold. (threshold: " + tbThreshold.Text + ", exponent: " + tbExponent.Text + ") ";
                    MessageBox.Show(msg);
                    return;
                }

                if (tv.Equals(double.NaN))
                {
                    string msg = @"Entered value must be greater than 0. (" + tbThreshold.Text + ") ";
                    MessageBox.Show(msg);
                    return;
                }

                if (Convert.ToDouble(tbExponent.Text.ToString()) == 0)
                {
                    string msg = @"Exponent cannot be zero.";
                    MessageBox.Show(msg);
                    return;
                }

                transform.Type = DependentVariableTransforms.Power;
                transform.Exponent = Convert.ToDouble(tbExponent.Text);
                dblMandateThreshold = tv;
            }
            boolClean = false;
        }


        protected void tbExponent_Leave(object sender, EventArgs e)
        {
            double exponent;

            if (Double.TryParse(tbExponent.Text, out exponent) == false)
            {
                string msg = @"Exponent must be a numeric value.";
                MessageBox.Show(msg);
                tbExponent.Focus();
            }
            else
            {
                dblMandateThreshold = Math.Pow(Convert.ToDouble(tbThreshold.Text.ToString()), exponent);
                transform.Exponent = exponent;
            }
            boolClean = false;
        }


        protected void tbThresholdReg_TextChanged(object sender, EventArgs e)
        {
            if (Double.TryParse(tbThreshold.Text, out dblMandateThreshold) == false)
            {
                string msg = @"Regulatory standard must be a numeric value.";
                MessageBox.Show(msg);
                return;
            }
            boolClean = false;
        }


        //This method is called whenever there is a change to the model or the threshold. It updates the prediction and diagnostic tabs.
        protected void UpdatePredictionTab()
        {
            //Work's done; let's go home.
            if (UpdateReadyForModel != null)
            {
                UpdateModelEvent e = new UpdateModelEvent(this.PackModelState());
                UpdateReadyForModel(this, e);
            }
        }

        private void tbThreshold_TextChanged(object sender, EventArgs e)
        {
            double tv = double.NaN;

            if (rbValue.Checked)
            {
                try
                {
                    tv = Convert.ToDouble(tbThreshold.Text.ToString());
                }
                catch
                {
                    string msg = @"Cannot convert threshold. (" + tbThreshold.Text + ") ";
                    MessageBox.Show(msg);
                    return;
                }
            }                            
            else if (rbLog10.Checked)
            {
                //ms has no fp error checking... check for all conditions.
                //log10(x) when x == 0 results in NaN and when x < 0 results in -Infinity
                try
                {
                    tv = Math.Log10(Convert.ToDouble(tbThreshold.Text.ToString()));
                }
                catch
                {
                    string msg = @"Cannot Log 10 transform threshold. (" + tbThreshold.Text + ") ";
                    MessageBox.Show(msg);
                    return;
                }

                if (tv.Equals(double.NaN))
                {
                    string msg = @"Entered value must be greater than 0. (" + tbThreshold.Text + ") ";
                    MessageBox.Show(msg);
                    return;
                }

                if (tv < 0)
                {
                    string msg = @"Entered value must be greater than 0. (" + tbThreshold.Text + ") ";
                    MessageBox.Show(msg);
                    return;
                }
            }
            else if (rbLoge.Checked)
            {
                //ms has no fp error checking... check for all conditions.
                //loge(x) when x == 0 results in NaN and when x < 0 results in -Infinity
                try
                {
                    tv = Math.Log(Convert.ToDouble(tbThreshold.Text.ToString()));
                }
                catch
                {
                    string msg = @"Cannot Log e transform threshold. (" + tbThreshold.Text + ") ";
                    MessageBox.Show(msg);
                    return;
                }

                if (tv.Equals(double.NaN))
                {
                    string msg = @"Entered value must be greater than 0. (" + tbThreshold.Text + ") ";
                    MessageBox.Show(msg);
                    return;
                }

                if (tv < 0)
                {
                    string msg = @"Entered value must be greater than 0. (" + tbThreshold.Text + ") ";
                    MessageBox.Show(msg);
                    return;
                }
            }
            else if (rbPower.Checked)
            {
                //ms has no fp error checking... check for all conditions.
                //loge(x) when x == 0 results in NaN and when x < 0 results in -Infinity
                try
                {
                    tv = Math.Pow(Convert.ToDouble(tbThreshold.Text.ToString()), Convert.ToDouble(tbExponent.Text.ToString()));
                }
                catch
                {
                    string msg = @"Cannot exponentiate threshold. (threshold: " + tbThreshold.Text + ", exponent: " + tbExponent.Text + ") ";
                    MessageBox.Show(msg);
                    return;
                }

                if (tv.Equals(double.NaN))
                {
                    string msg = @"Entered value must be greater than 0. (" + tbThreshold.Text + ") ";
                    MessageBox.Show(msg);
                    return;
                }

                if (Convert.ToDouble(tbExponent.Text.ToString()) == 0)
                {
                    string msg = @"Exponent cannot be zero.";
                    MessageBox.Show(msg);
                    return;
                }
            }

            dblMandateThreshold = tv;
            boolClean = false;
        }
    }
}
