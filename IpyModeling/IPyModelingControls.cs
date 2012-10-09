using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using DotSpatial.Controls;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Globalization;
using IPyCommon;
using VBCommon;
using System.Threading;
using Newtonsoft.Json;
using VBDatasheet;
using VBProjectManager;


namespace IPyModeling
{
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
        public event EventHandler ModelUpdated;
        public event EventHandler<RunButtonStatusArgs> RunButtonStatusChanged;

        public event EventHandler ManipulateDataTab;
        public event EventHandler ModelTab;
        public event EventHandler VariableTab;
        
        //Delegates to get data from Virtual Beach
        public delegate void RequestData(object sender, EventArgs args);
        public RequestData TabPageEntered;
        public event EventHandler<ModelingCallback> DataRequested;
        public event EventHandler IronPythonInterfaceRequested;
        protected DataTable tblModelData;
        
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
        private List<ListItem> listPredictors = new List<ListItem>();

        //A flag to indicate whether this modeling tab has been used.
        private bool boolComplete = false;
        protected bool boolVirgin = true;
        private bool boolClean = true;
        private bool boolRunning = false;
        protected double dblMandateThreshold;
        protected string strMethod;

        //RunChanged = true:running, false:canceled
        public delegate void BoolChangedEvent(bool val);
        public event BoolChangedEvent boolRunChanged; 

        protected Dictionary<string, object> dictTransform = new Dictionary<string, object>()
        {
            {"Type", DependentVariableTransforms.none.ToString()},
            {"Exponent", 1}
        };

        private DataTable correlationData = null;
        private DataTable _dtFull = null;
        DataTable dt = new DataTable();
        private IDictionary<string, object> dictPackedDatasheetState = null;
        
        //Number of observations in the dataset
        int intNumObs;

        IDictionary<string, object> dictPackedPlugin = new Dictionary<string, object>();


        public IPyModelingControl()
        {
            InitializeComponent();
            boolVirgin = true;
        
            //initialize the regulatory threshold
            EventArgs e = new EventArgs();
            rbValue_CheckedChanged(this, e);
            rbLogeValue_CheckedChanged(this, e);
            rbLog10Value_CheckedChanged(this, e);
            rbPower_CheckedChanged(this, e);

            //Request access to the IronPython interface.
            RequestIronPythonInterface();

            //Create the delegate that will raise the event that requests model data
            this.tabPage1.Enter += new EventHandler(DataTabEnter);
            this.TabPageEntered += new RequestData(RequestModelData);
            this.DataRequested += new EventHandler<ModelingCallback>(this.ProvideData);    
           // ResetIPyProject += new EventHandler(this.ResetProject);

            this.dsControl1.NotifiableChangeEvent += new EventHandler(this.UpdateData);
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


        /*//get flag on whether prediction should clear
        [JsonProperty]
        public Boolean ClearPrediction
        {
            get { return boolClearPrediction; }
        }*/


        //Get the list of the number of true positives
        [JsonProperty]
        public List<double> TruePositives
        {
            get { return this.listTruePos; }
        }


        /*// getter/setter for datasheet table
        [JsonProperty]
        public IDictionary<string, object> PackedDatasheetState
        {
            set { dictPackedDatasheetState = value; }
            get { return dictPackedDatasheetState; }
        }*/


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
        public string RegulatoryThreshold
        {
            get { return this.tbThreshold.Text; }
        }


        //Get the exponent for a power transform 
        [JsonProperty]
        public string Exponent
        {
            get { return this.tbExponent.Text; }
        }


        //Get the regulatory threshold
        [JsonProperty]
        public string DecisionThreshold
        {
            get { return this.lblDecisionThreshold.Text; }
        }

        //get list of predictors for prediction tab
        [JsonProperty]
        public List<ListItem> ListPredictors
        {
            get { return this.listPredictors; }
        }


        //Return a flag indicating whether this modeling tab has been touched.
        public bool VirginState
        {
            get { return this.boolVirgin; }
        }

        
        //Return a flag indicating whether this has been modified since the model was last exported. (false : modified)
        public bool Clean
        {
            get { return this.boolClean; }
        }


        //Return a flag indicating whether the plugin is ready to export a completed model
        public bool Complete
        {
            get { return this.boolComplete; }
        }


        //Return a flag indicating whether this modeling tab has been touched.
        public bool ThresholdingButtonsVisible
        {
            get { return this.pnlThresholdingButtons.Visible; }
        }


        //Get the transform of the dependent variable ..commented out when changing Transform to Dictionary
        public Dictionary<string,object> DependentVariableTransform    
        {
            get { return dictTransform; }
        }


        //correlation datatable
        [JsonProperty]
        public DataTable CorrelationDataTable
        {
            get { return correlationData; }
            set { correlationData = value; }
        }


        //set the model with incoming datatable information
        public void SetModelData(DataTable data)
        {
            this.tblModelData = data;

            int intI = 0;
            bool boolFinished = false;
            while (!boolFinished)
            {
                //look at extendedProperties to determine which tranform button to check
                if (data.Columns[intI].ExtendedProperties.ContainsKey("dependentvariable"))
                {
                    if (data.Columns[intI].ExtendedProperties.ContainsKey("responsevardefinedtransform"))
                    {
                        //Unpack the user's selected transformation of the dependent variable.
                        string strTransform = data.Columns[intI].ExtendedProperties["responsevardefinedtransform"].ToString();

                        if (String.Compare(strTransform, DependentVariableTransforms.none.ToString(), 0) == 0)
                            this.rbValue.Checked = true;
                        else if (String.Compare(strTransform, DependentVariableTransforms.Ln.ToString(), 0) == 0)
                            this.rbLoge.Checked = true;
                        else if (String.Compare(strTransform, DependentVariableTransforms.Log10.ToString(), 0) == 0)
                            this.rbLog10.Checked = true;
                        else if (String.Compare(strTransform, DependentVariableTransforms.Power.ToString(), 0) == 0)
                            this.rbPower.Checked = true;
                        else
                            this.rbValue.Checked = true;
                        //disable columns
                        ChangeControlStatus(false);
                        //enable regulatory threshold
                        ChangeThresholdControlStatus(true);
                        boolFinished = true;
                    }
                }
                //Have we checked all of the columns?
                intI++;
                if (data.Columns.Count == intI)
                    boolFinished = true;
            }
        }


        //Clear the control
        public void ClearModelingTab()
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

            pnlThresholdingButtons.Visible = false;
            boolComplete = false;
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


        //Handle a request from an IronPython-based modeling tab to begin the modeling process.
        private void ProvideData(object sender, ModelingCallback CallbackObject)
        {
            Cursor.Current = Cursors.WaitCursor;
            DataTable modelDataTable;
           
            modelDataTable = CreateModelDataTable();
            CallbackObject.MakeModel(modelDataTable);
        }


        //control ribbon active buttons depending on which tab
        protected void DataTabEnter(object sender, EventArgs args)
        {
            if (((TabPage)sender).Text == "Data Manipulation")
            {
                if (ManipulateDataTab != null)
                {
                    EventArgs e = new EventArgs();
                    ManipulateDataTab(this, e);
                }

            }
            if (((TabPage)sender).Text == "Variable Selection")
            {
                if (VariableTab != null)
                {
                    EventArgs e = new EventArgs();
                    VariableTab(this, e);
                }
            }
        }


        //Raise a request for access to the model data - should be raised when the Model tab is entered.
        protected void RequestModelData(object sender, EventArgs args)
        {
            //only have run button enabled when on modeling tab
            if (((TabPage)sender).Text == "Model")
            {
                if (ModelTab != null)
                {
                    EventArgs e = new EventArgs();
                    ModelTab(this, e);
                }
            }
            if (boolVirgin == true)
            {
                if (DataRequested != null)
                {
                    ModelingCallback e = new ModelingCallback(SetModelData);
                    DataRequested(this, e);
                }
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
            VBLogger.GetLogger().LogEvent("60", Globals.messageIntent.UserOnly, Globals.targetSStrip.ProgressBar);
            Cursor.Current = Cursors.WaitCursor;
                        
            if (DataRequested != null)
            {
                ModelingCallback e = new ModelingCallback(MakeModel);
                DataRequested(this, e);
            }
            VBLogger.GetLogger().LogEvent("70", Globals.messageIntent.UserOnly, Globals.targetSStrip.ProgressBar);
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
            Cursor.Current = Cursors.WaitCursor;

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


        //Set column header names in Variable Selection listbox
        public void SetData(IDictionary<string, object> packedState)
        {
            dsControl1.UnpackState((IDictionary<string, object>)packedState["PackedDatasheetState"]); //((IDictionary<string, object>)dictPackedPlugin["PackedDatasheetState"]);
            
            dt = dsControl1.DT;
            this.correlationData = dsControl1.DT;
            _dtFull = dt;
            if (_dtFull == null) return;
            
            tabControl1.SelectedIndex = 0;

            List<string> lstFieldList = new List<string>();
            for (int i = 2; i < _dtFull.Columns.Count; i++)
            {
                if (_dtFull.Columns[i].ExtendedProperties.ContainsKey(VBCommon.Globals.ENABLED))
                {
                    if (_dtFull.Columns[i].ExtendedProperties[VBCommon.Globals.ENABLED].ToString() == "True")
                        lstFieldList.Add(_dtFull.Columns[i].ColumnName);
                }
                else
                    lstFieldList.Add(_dtFull.Columns[i].ColumnName);
            }

            intNumObs = _dtFull.Rows.Count;
            lblNumObs.Text = "Number of Observations: " + intNumObs.ToString();

            lbAvailableVariables.Items.Clear();
            lbIndVariables.Items.Clear();

            for (int i = 0; i < lstFieldList.Count; i++)
            {
                ListItem li = new ListItem(lstFieldList[i], i.ToString());
                lbAvailableVariables.Items.Add(li);
            }

            lblAvailVars.Text = "(" + lbAvailableVariables.Items.Count.ToString() + ")";
            lblDepVars.Text = "(" + lbIndVariables.Items.Count.ToString() + ")";
            lbDepVarName.Text = _dtFull.Columns[1].ColumnName.ToString();
        }


        //Set column header names in Variable Selection listbox
        public void UpdateData(object source, EventArgs e)
        {
            boolClean = false;
            boolComplete = false;
            boolVirgin = false;

            dt = dsControl1.DT;
            this.correlationData = dsControl1.DT;
            _dtFull = dt;
            if (_dtFull == null) return;

            tabControl1.SelectedIndex = 0;

            List<string> lstFieldList = new List<string>();
            for (int i = 2; i < _dtFull.Columns.Count; i++)
            {
                if (_dtFull.Columns[i].ExtendedProperties.ContainsKey(VBCommon.Globals.ENABLED))
                {
                    if (_dtFull.Columns[i].ExtendedProperties[VBCommon.Globals.ENABLED].ToString() == "True")
                        lstFieldList.Add(_dtFull.Columns[i].ColumnName);
                }
                else
                    lstFieldList.Add(_dtFull.Columns[i].ColumnName);
            }

            intNumObs = _dtFull.Rows.Count;
            lblNumObs.Text = "Number of Observations: " + intNumObs.ToString();

            lbAvailableVariables.Items.Clear();
            lbIndVariables.Items.Clear();

            for (int i = 0; i < lstFieldList.Count; i++)
            {
                ListItem li = new ListItem(lstFieldList[i], i.ToString());
                lbAvailableVariables.Items.Add(li);
            }

            lblAvailVars.Text = "(" + lbAvailableVariables.Items.Count.ToString() + ")";
            lblDepVars.Text = "(" + lbIndVariables.Items.Count.ToString() + ")";
            lbDepVarName.Text = _dtFull.Columns[1].ColumnName.ToString();

            ClearModelingTab();
            RaiseUpdateNotification();            
        }


        private void btnAddInputVariable_Click(object sender, EventArgs e)
        {
            List<ListItem> items = new List<ListItem>();
            int intSelectedIndices = lbAvailableVariables.SelectedIndices.Count;
            
            //Make a list of the variables we're adding to the model.
            for (int i = 0; i < intSelectedIndices; i++)
            {
                ListItem li = (ListItem)lbAvailableVariables.Items[lbAvailableVariables.SelectedIndices[i]];
                items.Add(li);
            }
            
            //Move the variables from the "Available" box to the "Added" box
            foreach (ListItem li in items)
            {
                lbAvailableVariables.Items.Remove(li);
                lbIndVariables.Items.Add(li);
            }
            
            lblAvailVars.Text = "(" + lbAvailableVariables.Items.Count.ToString() + ")";
            lblDepVars.Text = "(" + lbIndVariables.Items.Count.ToString() + ")";

            ClearModelingTab();
        }


        private void btnRemoveInputVariable_Click(object sender, EventArgs e)
        {
            List<ListItem> items = new List<ListItem>();

            //Make a list of the variables we're removing from the model.
            for (int i = 0; i < lbIndVariables.SelectedIndices.Count; i++)
            {
                ListItem li = (ListItem)lbIndVariables.Items[lbIndVariables.SelectedIndices[i]];
                items.Add(li);
            }

            //Move the variables from the "Added" box to the "Available" box
            foreach (ListItem li in items)
            {
                lbIndVariables.Items.Remove(li);

                bool boolFoundIdx = false;
                int intJ = 0;
                for (intJ = 0; intJ < lbAvailableVariables.Items.Count; intJ++)
                {
                    ListItem li2 = (ListItem)lbAvailableVariables.Items[intJ];
                    if (Convert.ToInt32(li2.ValueItem) > Convert.ToInt32(li.ValueItem))
                    {
                        lbAvailableVariables.Items.Insert(intJ, li);
                        boolFoundIdx = true;
                        break;
                    }
                }

                if (boolFoundIdx == false)
                    lbAvailableVariables.Items.Insert(intJ, li);
            }

            lblAvailVars.Text = "(" + lbAvailableVariables.Items.Count.ToString() + ")";
            lblDepVars.Text = "(" + lbIndVariables.Items.Count.ToString() + ")";

            ClearModelingTab();
        }


        //used for IronPython-based modeling tab to begin the modeling process
        public DataTable CreateModelDataTable()
        {
            Cursor.Current = Cursors.WaitCursor;

            //Datasheet's packed state coming in
            DataTable dtCorr_ = dsControl1.DT;
            DataView dvCorr_ = dtCorr_.DefaultView;
            
            List<string> list = new List<string>();

            list.Add(dtCorr_.Columns[0].ColumnName);
            list.Add(dtCorr_.Columns[1].ColumnName);
            
            int intNumVars = lbIndVariables.Items.Count;
            for (int i = 0; i < intNumVars; i++)
                list.Add(lbIndVariables.Items[i].ToString());
            
            DataTable dtModel = dvCorr_.ToTable("ModelData", false, list.ToArray());
            
            return dtModel;
        }


        //Enable or disable the regulatory threshold control, then raise an event to do the same up the chain in the containing Form.
        protected void ChangeThresholdControlStatus(bool enable)
        {
            tbThreshold.Invoke((MethodInvoker)delegate {tbThreshold.Enabled = enable;});
        }


        //This button runs the modeling method associated with this pane.
        public void btnRun_Click(object sender, EventArgs e)
        {
            ClearModelingTab();
            Cursor.Current = Cursors.WaitCursor;

            VBLogger.GetLogger().LogEvent("20", Globals.messageIntent.UserOnly, Globals.targetSStrip.ProgressBar);
            
            //check to see if the model tab was clicked first (otherwise will get error half way thru model run)
            if (tblModelData == null)
            {
                MessageBox.Show("You must select the model tab before running the model.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            //Now this modeling tab has been touched.
            boolVirgin = false;
            boolRunning = true;

            SetRunButtonStatus(to: false);
            if (ipyInterface == null) RequestIronPythonInterface();

            VBLogger.GetLogger().LogEvent("30", Globals.messageIntent.UserOnly, Globals.targetSStrip.ProgressBar);

            boolInitialControlStatus = boolControlStatus;
            ChangeControlStatus(false);
            VBLogger.GetLogger().LogEvent("40", Globals.messageIntent.UserOnly, Globals.targetSStrip.ProgressBar);
            StartModeling();
            VBLogger.GetLogger().LogEvent("80", Globals.messageIntent.UserOnly, Globals.targetSStrip.ProgressBar);
            Cursor.Current = Cursors.WaitCursor;

            Application.DoEvents();
            VBLogger.GetLogger().LogEvent("90", Globals.messageIntent.UserOnly, Globals.targetSStrip.ProgressBar);
            
            boolRunning = false;
            SetRunButtonStatus(to: true);
            VBLogger.GetLogger().LogEvent("100", Globals.messageIntent.UserOnly, Globals.targetSStrip.ProgressBar);
            Cursor.Current = Cursors.Default;            
        }


        
        /*//Cancel in-progress model-building
        public void btnCancel_Click(Object sender, EventArgs e)
        {  
            boolRunning = false;
            NotifyPropChanged(boolRunning);

            ChangeControlStatus(boolInitialControlStatus);
            ChangeThresholdControlStatus(true);
            Application.DoEvents();
            
            return;
            
        }*/

        //Run the model-building process.
        //This is the callback function that Virtual Beach will use to run the modeling process.
        protected void MakeModel(DataTable Data)
        {
            Cursor.Current = Cursors.WaitCursor;

            //Set up the local variables we'll need for model-building.
            DataTable tblData = Data;
            double dblSpecificity = 0.9;
            string strTarget = tblData.Columns[1].Caption;
            double dblThreshold = dblMandateThreshold;

            //Remove the ID field:
            tblData.Columns.Remove(tblData.Columns[0].Caption);

            //Run the IronPython model-building code, then call PopulateResults to display the coefficients and the decision threshold.
            dynamic validation_results = ipyInterface.Validate(tblData, strTarget, dblSpecificity, regulatory_threshold: dblThreshold, method: strMethod);
            
            Cursor.Current = Cursors.WaitCursor;
            
            /*//if cancel was clicked, get out of here
            if (stopRun)
            {
                NotifyPropChanged(boolRunning);
                
                return;
            }*/
            
            this.ipyModel = validation_results[1];
            PopulateResults(this.ipyModel);

            //Now extract the valid thresholds and the corresponding specificities
            dblSpecificity = Convert.ToDouble(ipyModel.specificity);
            dynamic thresholding = ipyInterface.GetPossibleSpecificities(this.ipyModel);
            this.listCandidateThresholds = ((IList<object>)(((IList<object>)thresholding)[0])).Cast<double>().ToList();
            this.listCandidateSpecificity = ((IList<object>)(((IList<object>)thresholding)[1])).Cast<double>().ToList();

            //The following is a bit of python code to initialize the decision threshold at the default specificity.
            this.intThresholdIndex = (int)thresholding[1].index(dblSpecificity);

            //Extract the number of false positives, true positives, false negatives, and true negatives.
            object objCounts = ipyInterface.SpecificityChart(validation_results[0]);
            List<object> x = ((IList<object>)((IList<object>)objCounts)[0]).Cast<object>().ToList();
            List<object> tp = ((IList<object>)((IList<object>)objCounts)[1]).Cast<object>().ToList();
            List<object> tn = ((IList<object>)((IList<object>)objCounts)[2]).Cast<object>().ToList();
            List<object> fp = ((IList<object>)((IList<object>)objCounts)[3]).Cast<object>().ToList();
            List<object> fn = ((IList<object>)((IList<object>)objCounts)[4]).Cast<object>().ToList();

            //Convert the validation counts to doubles and move them from the temporary lists above into more permanent lists.
            for (int i = 0; i < x.Count; i++)
            {
                listValidationSpecificity.Add(Convert.ToDouble(x[i]));
                listTruePos.Add(Convert.ToDouble(tp[i]));
                listTrueNeg.Add(Convert.ToDouble(tn[i]));
                listFalsePos.Add(Convert.ToDouble(fp[i]));
                listFalseNeg.Add(Convert.ToDouble(fn[i]));
            }

            /*//if cancel was clicked, get out of here
            if (stopRun)
            {
                NotifyPropChanged(boolRunning);
                stopRun = false;
                return;
            }*/

            InitializeValidationChart();
            AnnotateChart();

            //Enable the thresholding controls and the model-selection button.
            pnlThresholdingButtons.Visible = true;
            ChangeControlStatus(boolInitialControlStatus);
            ChangeThresholdControlStatus(true);

            //Work's done; let's go home.
            boolComplete = true;
            RaiseUpdateNotification();
            return;
        }


        protected void InitializeValidationChart()
        {
            Cursor.Current = Cursors.WaitCursor;

            /*//if cancel was clicked, get out of here
            if (stopRun)
            {
                NotifyPropChanged(boolRunning);
                return;
            }*/

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
            Cursor.Current = Cursors.WaitCursor;

            /*//if cancel was clicked, get out of here
            if (stopRun)
            {
                NotifyPropChanged(boolRunning);
                return;
            }*/

            tabControl1.SelectTab(1);


            //Set the threshold with the given specificity.
            double dblSpecificity = this.listCandidateSpecificity[this.intThresholdIndex];
            ipyModel.Threshold(dblSpecificity);
            lblDecisionThreshold.Text = String.Format("{0:F3}", UntransformThreshold((double)ipyModel.threshold));

            //Locate the specificity annotation
            lblSpec.Text = "Specificity: " + Convert.ToString(Math.Round(value: dblSpecificity, digits: 3));
            int intXLoc = (int)chartValidation.ChartAreas[0].AxisX.ValueToPixelPosition(dblSpecificity) + chartValidation.Location.X - (int)(lblSpec.Size.Width / 2);
            lblSpec.Location = new Point(x: intXLoc, y: 6);
            lblSpec.Visible = true;

            //Format the threshold line and draw it on the chart.
            chartValidation.Annotations.Clear();
            System.Windows.Forms.DataVisualization.Charting.VerticalLineAnnotation myLine = new System.Windows.Forms.DataVisualization.Charting.VerticalLineAnnotation();
            myLine.X = chartValidation.ChartAreas[0].AxisX.ValueToPosition(dblSpecificity);
            myLine.AxisY = chartValidation.ChartAreas[0].AxisY;
            double dblYMax = chartValidation.ChartAreas[0].AxisY.Maximum;
            double dblYMin = chartValidation.ChartAreas[0].AxisY.Minimum;
            myLine.Y = dblYMax;
            myLine.Height = chartValidation.ChartAreas[0].AxisY.ValueToPosition(dblYMin) - chartValidation.ChartAreas[0].AxisY.ValueToPosition(dblYMax);
            myLine.Visible = true;
            myLine.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartValidation.Annotations.Add(myLine);
            chartValidation.Update();

            //Summarize the model's performance in the validation ListView.
            List<double> lstDblCandidates = listValidationSpecificity.Where(arg => arg <= dblSpecificity).ToList();
            int intIndex = listValidationSpecificity.FindIndex(arg => arg == lstDblCandidates.Max());
            string[] strArrValidation = new string[4] { listTruePos[intIndex].ToString(), listTrueNeg[intIndex].ToString(), listFalsePos[intIndex].ToString(), listFalseNeg[intIndex].ToString() };

            //Add the row to the listview, coloring it red if this variable is considered to have minor influence.
            ListViewItem lvi = new ListViewItem(strArrValidation);
            lvValidation.Items.Clear();
            lvValidation.Items.Add(lvi);
        }


        protected double UntransformThreshold(double value)
        {
            if ((DependentVariableTransforms)this.DependentVariableTransform["Type"] == DependentVariableTransforms.none)
                return value;
            else if ((DependentVariableTransforms)this.DependentVariableTransform["Type"] == DependentVariableTransforms.Log10)
                return Math.Pow(10, value);
            else if ((DependentVariableTransforms)this.DependentVariableTransform["Type"] == DependentVariableTransforms.Ln)
                return Math.Exp(value);
            else if ((DependentVariableTransforms)this.DependentVariableTransform["Type"] == DependentVariableTransforms.Power)
                return Math.Pow(value, 1 / (double)this.DependentVariableTransform["Exponent"]);
            else
                return value;
        }


        //Pack State for Serializing
        public IDictionary<string, object> PackState()
        {
            if (dsControl1.DT == null)
                return null;

            IDictionary<string, object> dictPluginState = new Dictionary<string, object>();

            dictPluginState.Add("PackedDatasheetState", dsControl1.PackState());
            dictPluginState.Add("Predictors", listPredictors);

            //Save the state of UI elements.
            dictPluginState.Add("Transform", dictTransform);
            dictPluginState.Add("VirginState", this.boolVirgin);
            dictPluginState.Add("ThresholdingButtonsVisible", true);
            dictPluginState.Add("ActiveTab", tabControl1.SelectedIndex);
            dictPluginState.Add("Method", strMethod);

            //Store the regulatory threshold
            double dblRegulatoryThreshold;
            try { dblRegulatoryThreshold = Convert.ToDouble(RegulatoryThreshold); }
            catch (InvalidCastException) { dblRegulatoryThreshold = -1; }

            //Needed by the prediction plugin: 
            //if changing a saved project need to keep the listPredictions the same, because lbIndVariables aren't saved
            if (ListPredictors.Count == 0)
            {
                foreach (ListItem item in lbIndVariables.Items)
                    listPredictors.Add(item);
            }

            //if just adding to stack, go to different pack to be used in setData()
            if (Model == null)
                return dictPluginState;

            //Pack up the model's state in two ways: one with the model represented by a dynamic object, the other with the model represented by a string
            string strModelString = ipyInterface.Serialize(Model);

            //Store the decision threshold.
            double dblDecisionThreshold;
            try { dblDecisionThreshold = Convert.ToDouble(DecisionThreshold); }
            catch (InvalidCastException) { dblDecisionThreshold = -1; }

            Dictionary<string, object> dictModelState = new Dictionary<string, object>();
            dictModelState.Add("ModelString", strModelString);
            dictModelState.Add("Transform", DependentVariableTransform);
            dictModelState.Add("RegulatoryThreshold", dblRegulatoryThreshold);
            dictModelState.Add("DecisionThreshold", dblDecisionThreshold);
            dictModelState.Add("Method", strMethod);
            dictPluginState.Add("Model", dictModelState);
            
            //Save the lists that we use to make the validation chart
            Dictionary<string, List<double>> dictValidation = new SerializableDictionary<string, List<double>>();
            dictValidation.Add("tpos", this.TruePositives);
            dictValidation.Add("tneg", this.TrueNegatives);
            dictValidation.Add("fpos", this.FalsePositives);
            dictValidation.Add("fneg", this.FalseNegatives);
            dictValidation.Add("specificity", this.ValidationSpecificities);

            dictPluginState.Add("ValidationDictionary", dictValidation);

            //Save the lists that we use to set the model's decision threshold.
            Dictionary<string, List<double>> dictThresholding = new Dictionary<string, List<double>>();
            dictThresholding.Add("specificity", this.listCandidateSpecificity);
            dictThresholding.Add("threshold", this.listCandidateThresholds);
            dictPluginState.Add("ThresholdingDictionary", dictThresholding);
            dictPluginState.Add("ThresholdingIndex", this.intThresholdIndex);

            return dictPluginState;
        }

        
        //Reconstruct the saved modeling state - without the IronPython Model
        public void UnpackState(IDictionary<string, object> dictProjectState)
        {
            this.Show();
            if (dictProjectState.Count <= 3) return; //if only plugin props are here

            this.boolComplete = (bool)dictProjectState["Complete"];

            //unpack the model's datasheet
            this.SetData(dictProjectState);
 
            //Make the modeling tab active during unpacking so we can draw the graph.
            tabControl1.SelectedIndex = 2;

            if (boolComplete)
            {
                //Unpack the lists that go into making the validation chart.
                Dictionary<string, List<double>> dictValidation = (Dictionary<string, List<double>>)dictProjectState["ValidationDictionary"];
                this.listValidationSpecificity = dictValidation["specificity"];
                this.listTruePos = dictValidation["tpos"];
                this.listTrueNeg = dictValidation["tneg"];
                this.listFalsePos = dictValidation["fpos"];
                this.listFalseNeg = dictValidation["fneg"];

                //Unpack the lists that are used to set the model's decision threshold                
                Dictionary<string, List<double>> dictThresholding = (Dictionary<string, List<double>>)dictProjectState["ThresholdingDictionary"];
                this.listCandidateSpecificity = dictThresholding["specificity"];
                this.listCandidateThresholds = dictThresholding["threshold"];
                this.intThresholdIndex = (int)dictProjectState["ThresholdingIndex"];

                //ModelState modelState = (ModelState)dictProjectState["Model"];
                Dictionary<string, object> dictModel = (Dictionary<string, object>)dictProjectState["Model"];
                this.ipyModel = ipyInterface.Deserialize(dictModel["ModelString"]);
                this.Method = (string)dictModel["Method"];

                //Unpack the contents of the threshold and exponent text boxes
                Dictionary<string, object> dictTransform = (Dictionary<string, object>)dictProjectState["Transform"];

                this.tbExponent.Text = dictTransform["Exponent"].ToString();
                this.tbThreshold.Text = ((double)dictModel["RegulatoryThreshold"]).ToString();
                this.lblDecisionThreshold.Text = ((double)dictModel["DecisionThreshold"]).ToString();

                //Unpack the user's selected transformation of the dependent variable.
                if (Convert.ToInt32(dictTransform["Type"]) == Convert.ToInt32(DependentVariableTransforms.none))
                    this.rbValue.Checked = true;
                else if (Convert.ToInt32(dictTransform["Type"]) == Convert.ToInt32(DependentVariableTransforms.Ln))
                    this.rbLoge.Checked = true;
                else if (Convert.ToInt32(dictTransform["Type"]) == Convert.ToInt32(DependentVariableTransforms.Log10))
                    this.rbLog10.Checked = true;
                else if (Convert.ToInt32(dictTransform["Type"]) == Convert.ToInt32(DependentVariableTransforms.Power))
                    this.rbPower.Checked = true;
                else
                    this.rbValue.Checked = true;
                //unpack predictors
                this.listPredictors = (List<ListItem>)dictProjectState["Predictors"];

                //Now make sure the selected transformation is reflected behind the scenes, too.
                EventArgs e = new EventArgs();
                rbValue_CheckedChanged(this, e);
                rbLogeValue_CheckedChanged(this, e);
                rbLog10Value_CheckedChanged(this, e);
                rbPower_CheckedChanged(this, e);

                //Now restore the elements of the user interface.
                this.pnlThresholdingButtons.Visible = (bool)dictProjectState["ThresholdingButtonsVisible"];

                //rebuild model
                PopulateResults(this.ipyModel);
                InitializeValidationChart();
                AnnotateChart();
            }

            //Now set the selected tab to whatever was on top when the project was saved.
            tabControl1.SelectedIndex = (int)(dictProjectState["ActiveTab"]); //model
        }


        protected void btnLeft25_Click(object sender, EventArgs e)
        { 
            if (this.intThresholdIndex >= 25)
                this.intThresholdIndex -= 25;
            else
                this.intThresholdIndex = 0;

            AnnotateChart();
            RaiseUpdateNotification();
        }
        

        protected void btnLeft1_Click(object sender, EventArgs e)
        {
            if (this.intThresholdIndex >= 1)
                this.intThresholdIndex -= 1;
            else
                this.intThresholdIndex = 0;

            AnnotateChart();
            RaiseUpdateNotification();
        }


        protected void btnRight1_Click(object sender, EventArgs e)
        {
            if (this.intThresholdIndex < this.listCandidateThresholds.Count - 1)
                this.intThresholdIndex += 1;
            else
                this.intThresholdIndex = this.listCandidateThresholds.Count - 1;

            AnnotateChart();
            RaiseUpdateNotification();
        }


        protected void btnRight25_Click(object sender, EventArgs e)
        {
            if (this.intThresholdIndex < this.listCandidateThresholds.Count - 25)
                this.intThresholdIndex += 25;
            else
                this.intThresholdIndex = this.listCandidateThresholds.Count - 1;

            AnnotateChart();
            RaiseUpdateNotification();
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
                dictTransform["Type"] = DependentVariableTransforms.none;
            }
            boolClean = false;
        }


        protected void rbLog10Value_CheckedChanged(object sender, EventArgs e)
        {
            if (rbLog10.Checked)
            {
                double tv = double.NaN;

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

                dblMandateThreshold = tv;
                dictTransform["Type"] = DependentVariableTransforms.Log10;
            }
            boolClean = false;
        }


        protected void rbLogeValue_CheckedChanged(object sender, EventArgs e)
        {
            if (rbLoge.Checked)
            {
                double tv = double.NaN;

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
                dictTransform["Type"] = DependentVariableTransforms.Ln;
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

                dictTransform["Type"] = DependentVariableTransforms.Power;
                dictTransform["Exponent"] = Convert.ToDouble(tbExponent.Text);
                dblMandateThreshold = tv;
            }
            boolClean = false;
        }


        protected void tbExponent_Leave(object sender, EventArgs e)
        {
            double dblExponent;

            if (Double.TryParse(tbExponent.Text, out dblExponent) == false)
            {
                string msg = @"Exponent must be a numeric value.";
                MessageBox.Show(msg);
                tbExponent.Focus();
            }
            else
            {
                dblMandateThreshold = Math.Pow(Convert.ToDouble(tbThreshold.Text.ToString()), dblExponent);
                dictTransform["Exponent"] = dblExponent;
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
        protected void RaiseUpdateNotification()
        {
            if (ModelUpdated != null)
            {
                EventArgs e = new EventArgs();
                ModelUpdated(this, e);
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


        //This is the callback method that sets the model
        public void SetModel(Dictionary<string, object> dictPackedModel)
        {
            if (dictPackedModel != null)
            {
                myScatterPlot.SetThresholds(Convert.ToDouble(dictPackedModel["DecisionThreshold"]), Convert.ToDouble(dictPackedModel["RegulatoryThreshold"]));

                myScatterPlot.PowerExponent = Convert.ToDouble(dictTransform["Exponent"]);
                myScatterPlot.Transform = dictTransform["Type"].ToString();

                //_projectOpened = true;
            }
            else
                ipyModel = null;
        }


        public List<double> Predict(DataTable tblForPrediction)
        {
            dynamic dynPredictions = ipyInterface.Predict(ipyModel, tblForPrediction);
            List<double> lstPredictions = ((IList<object>)dynPredictions).Cast<double>().ToList();
            return (lstPredictions);
        }


        public List<double> PredictExceedanceProbability(DataTable tblForPrediction)
        {
            dynamic dynPredictions = ipyInterface.PredictExceedanceProbability(ipyModel, tblForPrediction);
            List<double> lstExceedanceProbability = ((IList<object>)dynPredictions).Cast<double>().ToList();
            return (lstExceedanceProbability);
        }


        public string ModelString()
        {
            return(ipyInterface.GetModelExpression(ipyModel).Replace("[", "(").Replace("]", ")"));
        }


        private void SetRunButtonStatus(bool to)
        {
            if (RunButtonStatusChanged != null)
            {
                RunButtonStatusArgs StatusArgs = new RunButtonStatusArgs(status:to);
                RunButtonStatusChanged(this, StatusArgs);
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
