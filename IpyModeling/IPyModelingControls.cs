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
//using Combinatorics;
using System.Threading;
using Newtonsoft.Json;
using VBDatasheet;


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
        public event EventHandler ChangeMade4Stack;


        public event EventHandler ManipulateDataTab;
        public event EventHandler ModelTab;
        public event EventHandler VariableTab;

        public delegate void BoolChangedEvent(bool val);
        public event BoolChangedEvent boolRunChanged;
        
        public delegate void BoolStopEvent(bool val);
        public event BoolStopEvent boolStopRun;

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
        private List<ListItem> listPredictors = new List<ListItem>();
        private bool boolRunning = false;       //stop run when cancel is clicked
        private bool boolStopRunning = false;   //stop run when model tab isn't active

        //check to see if cancel was clicked during run
        private volatile bool stopRun = false;
       
        //Related to underlying model:
        protected double dblMandateThreshold;
        protected string strMethod;
        private bool boolClearPrediction; //if model has changed since prediction ran

        protected Dictionary<string, object> dictTransform = new Dictionary<string, object>()
        {
            {"Type", DependentVariableTransforms.none.ToString()},
            {"Exponent", 1}
        };

        private DataTable correlationData = null;
        private DataTable _dtFull = null;
        DataTable dt = new DataTable();
        private IDictionary<string, object> dictPackedDatasheetState = null;

        //A flag to indicate whether this modeling tab has been used.
        protected bool boolVirgin;
        protected bool boolClean = true;

        //Number of observations in the dataset
        int intNumObs;

        private enum _mlrState { clean, dirty };
        private _mlrState _state = _mlrState.clean;

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


        //get flag on whether prediction should clear
        [JsonProperty]
        public Boolean ClearPrediction
        {
            get { return boolClearPrediction; }
        }


        //Get the list of the number of true positives
        [JsonProperty]
        public List<double> TruePositives
        {
            get { return this.listTruePos; }
        }


        // getter/setter for datasheet table
        [JsonProperty]
        public IDictionary<string, object> PackedDatasheetState
        {
            set { dictPackedDatasheetState = value; }
            get { return dictPackedDatasheetState; }
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


        //get the flag on whether model is running or not
        public bool ModelRunning
        {
            get { return boolRunning; }
            set
            {
                this.boolRunning = value;
                if (boolRunChanged != null)
                {
                    boolRunChanged(value);
                }
            }
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

        
        //Return a flag indicating whether this modeling tab has been modified since the model was last exported. (false : modified)
        public bool Clean
        {
            get { return this.boolClean; }
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
            this.model_data = data;

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


        //Handle a request from an IronPython-based modeling tab to begin the modeling process.
        private void ProvideData(object sender, ModelingCallback CallbackObject)
        {
            //make cursor hourglass
            Cursor.Current = Cursors.WaitCursor;

            DataTable modelDataTable;
            //first check to see if cancel was hit
           
            modelDataTable = CreateModelDataTable();
            CallbackObject.MakeModel(modelDataTable);
        }

        protected void DataTabEnter(object sender, EventArgs args)
        {
            if (((TabPage)sender).Text == "Data Manipulation")
            {
                //event for enabling only manipulate buttons
                if (ManipulateDataTab != null)
                {
                    EventArgs e = new EventArgs();
                    ManipulateDataTab(this, e);
                }

            }
            if (((TabPage)sender).Text == "Variable Selection")
            {
                //event for ensuring no buttons are enabled
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


        //this method alerts the plugin that the boolean Running property has changed. This changes the text in the Run button
        protected void NotifyPropChanged(bool val)
        {
            if (boolRunChanged != null)
            {
                boolRunChanged(val);
            }
        }

        
        //this method alerts the plugin that run has been canceled. used to test before setting boolComplete to true in plugin
        protected void StopRunning(bool val)
        {
            if (boolStopRun != null)
            {
                boolStopRun(val);
            }
        }


        //This method alerts the container that we need data. The container should then use the Set property of sender.data
        protected void StartModeling()
        {
            Cursor.Current = Cursors.WaitCursor;

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

        
        //maintain the model's ds when no changes made to global
        public void UnhideDatasheet(DataTable dt)
        {
            dsControl1.UnhideModelDS(dt);
        }


        //Set column header names in Variable Selection listbox
        public void SetData(IDictionary<string, object> packedState)
        {
            //Datasheet's packed state coming in
            dictPackedPlugin = packedState;

            //setting data from datasheet will go through, setting data from an undo will catch
            try
            {
                //check to see if we should clear the model
                if ((bool)packedState["ChangesMadeDS"])
                {

                    //clear the prediction
                    if (model_data != null)
                    {
                        boolClearPrediction = true;
                        UpdatePredictionTab();
                    }
                    //clear model
                    Clear();
                }
            }
            catch { }


            dsControl1.UnpackState((IDictionary<string, object>)dictPackedPlugin["PackedDatasheetState"]);
            
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

            //Broadcast the 
            if (ChangeMade4Stack != null)
            {
                EventArgs ev = new EventArgs();
                ChangeMade4Stack(this, ev);
            }

            //clear the model
            //Clear();
            //need to clear the prediction now
            boolClearPrediction = true;
            UpdatePredictionTab(); 
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

            _state = _mlrState.dirty;
            Clear();
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

            _state = _mlrState.dirty;
            Clear();
        }


        /// <summary>
        /// Calculate the total number of combinations availabl for the selected number of independent variables
        /// </summary>
        /*private void SetCombinations()
        {
            //number of independent variables in the listbox
            int intNumVars = lbIndVariables.Items.Count;

            List<short> combList = new List<short>();
            short shtTmp = 0; ;
            for (int i = 0; i < intNumVars; i++)
            {
                ListItem li = (ListItem)lbIndVariables.Items[i];
                shtTmp = 1;
                shtTmp += Convert.ToInt16(li.ValueItem);
                combList.Add(shtTmp);
            }
            //determine combinations
            decimal decTotalComb = 0;
            Combinations<short> combinations = null;
            for (int i = 0; i < intNumVars; i++)
            {
                combinations = new Combinations<short>(combList.ToArray(), i, GenerateOption.WithoutRepetition);
                decTotalComb += combinations.Count;

            }

            string nModels = string.Empty;
            if (decTotalComb > 9999999999)
            {
                nModels = string.Format("{0:000e000}", decTotalComb);
            }
            else
            {
                if (decTotalComb < 0)
                {
                    //we've flipped the storage capacity (not of totalComb [decimal type good to 7.8(10)**28], something else)
                    //combinations.Count is only a long - probably this (max 9.2(10)**18)
                    nModels = " more than 9.2e018 ";
                }
                else
                {
                    nModels = string.Format("{0:#,###,###,###}", decTotalComb);
                }
            }
        }*/


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


        //This button runs or cancels the modeling method associated with this pane.
        public void btnRun_Click(object sender, EventArgs e)
        {
            //clear the model before running a model
            Clear();

            //show it's doing something
            Cursor.Current = Cursors.WaitCursor;

            //check to see if the model tab was clicked first (otherwise will get error half way thru model run)
            if (model_data == null)
            {
                MessageBox.Show("You must select the model tab before running the model.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //throw flag to exit out of broadcasting anything
                boolStopRunning = true;
                StopRunning(boolStopRunning);
                return;
            }

            //Now this modeling tab has been touched.
            boolVirgin = false;

            //Now the model is running, enable cancel/disable run buttons
            boolRunning = true;
            NotifyPropChanged(boolRunning);

            //Start running the model-building code.
            if (ipyInterface == null) RequestIronPythonInterface();
            
            boolInitialControlStatus = boolControlStatus;
            ChangeControlStatus(false);

            StartModeling();

            //keep waiting..
            Cursor.Current = Cursors.WaitCursor;

            //check to see if cancel was hit
            if (stopRun)
            {
                NotifyPropChanged(boolRunning);
                return;
            }

            Log("0", LogMessageEvent.Intents.UserOnly, LogMessageEvent.Targets.ProgressBar);
            Application.DoEvents();

            //Now the model is done running, disable cancel/enable run buttons
            boolRunning = false;
            NotifyPropChanged(boolRunning);

            //all done
            Cursor.Current = Cursors.Default;
        }


        
        //Cancel in-progress model-building
        public void btnCancel_Click(Object sender, EventArgs e)
        {  
            //the model run has been canceled
            stopRun = true;
            boolRunning = false;
            NotifyPropChanged(boolRunning);

            ChangeControlStatus(boolInitialControlStatus);
            ChangeThresholdControlStatus(true);
            Application.DoEvents();
            return;
            
        }

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
            
            //if cancel was clicked, get out of here
            if (stopRun)
            {
                NotifyPropChanged(boolRunning);
                return;
            }
            
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

            //if cancel was clicked, get out of here
            if (stopRun)
            {
                NotifyPropChanged(boolRunning);
                return;
            }

            InitializeValidationChart();
            AnnotateChart();

            //Enable the thresholding controls and the model-selection button.
            pnlThresholdingButtons.Visible = true;
            ChangeControlStatus(boolInitialControlStatus);
            ChangeThresholdControlStatus(true);

            //Work's done; let's go home.
            if (ModelUpdated != null)
            {
                EventArgs e = new EventArgs();
                ModelUpdated(this, e);
            }
            return;
        }


        protected void InitializeValidationChart()
        {

            Cursor.Current = Cursors.WaitCursor;
            //if cancel was clicked, get out of here
            if (stopRun)
            {
                NotifyPropChanged(boolRunning);
                return;
            }

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

            //if cancel was clicked, get out of here
            if (stopRun)
            {
                NotifyPropChanged(boolRunning);
                return;
            }

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
        public IDictionary<string, object> PackProjectState()
        {
            if (dsControl1.DT == null)
                return null;

            IDictionary<string, object> dictPluginState = new Dictionary<string, object>();

            //save correlationData as xml for serializing to save extendedProperties in tbl
            /*StringWriter sw = null;
            sw = new StringWriter();
            correlationData.WriteXml(sw, XmlWriteMode.WriteSchema, false);
            string xmlDataTable = sw.ToString();
            sw.Close();
            sw = null;
            dictPluginState.Add("CorrelationDataTable", xmlDataTable);*/

            dictPluginState.Add("PackedDatasheetState", dsControl1.PackState());
            dictPluginState.Add("Predictors", listPredictors);

            //Save the state of UI elements.
            dictPluginState.Add("Transform", dictTransform);
            dictPluginState.Add("VirginState", this.boolVirgin);
            dictPluginState.Add("ThresholdingButtonsVisible", true);

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

            dictPluginState.Add("CleanPredict", this.ClearPrediction);

            //if just adding to stack, go to different pack to be used in setData()
            if (Model == null)
                return dictPluginState;

            //Pack up the model's state in two ways: one with the model represented by a dynamic object, the other with the model represented by a string
            string strModelString = ipyInterface.Serialize(Model);

            //Store the decision threshold.
            double dblDecisionThreshold;
            try { dblDecisionThreshold = Convert.ToDouble(DecisionThreshold); }
            catch (InvalidCastException) { dblDecisionThreshold = -1; }

            //add both versions of the model
            Dictionary<string, object> dictModelObject = IPyCommon.Helper.ModelState(model: ipyModel, method: strMethod, dblRegulatoryThreshold: dblRegulatoryThreshold, decisionThreshold: dblDecisionThreshold, transform: DependentVariableTransform);
            Dictionary<string, object> dictModelByString = IPyCommon.Helper.ModelState(modelString: strModelString, method: strMethod, dblRegulatoryThreshold: dblRegulatoryThreshold, decisionThreshold: dblDecisionThreshold, transform: DependentVariableTransform);
            dictPluginState.Add("ModelByObject", dictModelObject);
            dictPluginState.Add("ModelByString", dictModelByString);
            
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
        public void UnpackProjectState(IDictionary<string, object> dictProjectState)
        {
            this.Show();
            if (dictProjectState.Count <= 3) return; //if only "Complete" and "Visible" are present
            
            //Unpack the virgin status of the project
            this.boolVirgin = (bool)dictProjectState["VirginState"];

            //unpack the model's datasheet
            PackedDatasheetState = (IDictionary<string, object>)dictProjectState["PackedDatasheetState"];
            dsControl1.UnpackState(PackedDatasheetState);

            //unpack the saved state of PLS modeling control
            tabControl1.SelectedTab = tabControl1.TabPages[2]; //model

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
                Dictionary<string, List<double>> dictThresholding = (Dictionary<string, List<double>>)dictProjectState["ThresholdingDictionary"];
                this.listCandidateSpecificity = dictThresholding["specificity"];
                this.listCandidateThresholds = dictThresholding["threshold"];
                this.intThresholdIndex = (int)dictProjectState["ThresholdingIndex"];

                //ModelState modelState = (ModelState)dictProjectState["Model"];
                Dictionary<string, object> dictModelString = (Dictionary<string, object>)dictProjectState["ModelByString"];
                this.ipyModel = ipyInterface.Deserialize(dictModelString["ModelString"]);
                this.Method = (string)dictModelString["Method"];

                //Unpack the contents of the threshold and exponent text boxes
                Dictionary<string, object> dictTransform = (Dictionary<string, object>)dictProjectState["Transform"];

                this.tbExponent.Text = dictTransform["Exponent"].ToString();
                this.tbThreshold.Text = ((double)dictModelString["RegulatoryThreshold"]).ToString();
                this.lblDecisionThreshold.Text = ((double)dictModelString["DecisionThreshold"]).ToString();

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

                //unpack xmlDataTable and convert back to DataTable
                /*string xmlDataTable = (string)dictProjectState["CorrelationDataTable"];
                StringReader sr = new StringReader(xmlDataTable);
                DataSet ds = new DataSet();
                ds.ReadXml(sr);
                sr.Close();
                this.correlationData = ds.Tables[0];*/

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
        }


        protected void btnLeft25_Click(object sender, EventArgs e)
        { 
            boolClearPrediction = true;
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
            boolClearPrediction = true;
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
            boolClearPrediction = true;
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
            boolClearPrediction = true;
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
        protected void UpdatePredictionTab()
        {
            //Work's done; let's go home.
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

        //This event is raised when model-building is complete. It tells us to enable models saving and to display the Residuals tab.
        private void CompletedIPyModeling(object sender, EventArgs e)
        {
        }


        private const double cutoff = 0.65d;
        private const double cookscutoff = 0.05d;

        private double[] _dffits = null;
        private double[] _cooks = null;
        private double _dffitsThreshold = cutoff;
        private double _cooksThreshold = cookscutoff;

        private double[] _predictions = null;
        private double[] _standardResiduals = null;
        private double[] _observations = null;
        
        private int _dffitsRecno2Remove;
        private int _cooksRecno2Remove;

        private string _dffitsDateTime2Remove;
        private string _cooksDataTime2Remove;

        private double _dffitsResidValue2Remove;
        private double _cooksResidValue2Remove;

        private List<int> _recsRemoved = null;
        private List<double> _residValueRemoved = null;
        private List<string> _residTypeRemoved = null;

        
        private DataSet _modelBuildTables = null;
        private string _currentModelTableName = string.Empty;

        private bool _gobtnState = true;

        private Dictionary<string, double> _selectedModel = null;
        private int _maxIterations = 0;

        private Dictionary<int, string> _residualInfo = null;

        private bool _projectOpened = false;

        private int _selectedRebuild = -1;

        private enum _residState { clean, dirty };
        //private _residState _state = _residState.dirty;

        //Added for IronPython-based modeling:
        //public event EventHandler IronPythonInterfaceRequested;
        public event EventHandler ModelRequested;


        //This is the callback method that sets the model
        public void SetModel(Dictionary<string, object> dictPackedModel)
        {
            if (dictPackedModel != null)
            {
                myScatterPlot.SetThresholds(Convert.ToDouble(dictPackedModel["DecisionThreshold"]), Convert.ToDouble(dictPackedModel["RegulatoryThreshold"]));

                myScatterPlot.PowerExponent = Convert.ToDouble(dictTransform["Exponent"]);
                myScatterPlot.Transform = dictTransform["Type"].ToString();

                _projectOpened = true;
            }
            else
                ipyModel = null;
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

        /*
        private void frmResiduals_Enter(object sender, EventArgs e)
        {
            if (ipyInterface == null) RequestIronPythonInterface();
            RequestModel();
            
            if (ipyModel == null)
                return;

            if (_projMgr.IPyResidualAnalysisInfo == null)
                return;

            _state = _residState.clean;


            if (_projMgr.CorrelationDataTable == null) return;

            _residualInfo = new Dictionary<int, string>();

            DataTable modeldt = _projMgr.CorrelationDataTable;

            _gobtnState = true;

            //cutoff rebuilds when half data records eliminated
            _maxIterations = modeldt.Rows.Count / 2;

            if (modeldt != null)
            {
                if (ipyModel != null)
                {
                    _recsRemoved = new List<int>();
                    _residValueRemoved = new List<double>();
                    _residTypeRemoved = new List<string>();

                    //listBox1.Items.Clear();
                    _modelBuildTables.Tables.Clear();

                    //DataTable dffits = getDFFITSTable(model, modeldt, true);
                    //DataTable cooks = getCooksDistanceTable(model, modeldt, true);
                    //updateModelList(model, "original", modeldt, "model");

                    //create rediduals plots
                    //createResidPlot("DFFITS", dffits);
                    //createResidPlot("CooksDistance", cooks);

                    //predictions vs standardized residuals plot
                    createPlot2(ipyModel);
                }
            }
        }


        //Get or set the interface to IronPython. 
        public dynamic IronPythonInterface
        {
            get { return this.ipyInterface; }
            set { this.ipyInterface = value; }
        }


        private bool newModelSelected(Dictionary<string, double> modeldic)
        {
            _state = _residState.dirty;
            if (_selectedModel == null)
            {
                _selectedModel = modeldic;
                return true;
            }

            if (_projectOpened)
            {
                _projectOpened = false;
                return true;
            }

            if (_selectedModel.Keys.Count == modeldic.Keys.Count)
            {
                foreach (KeyValuePair<string, double> kv in modeldic)
                {
                    if (!_selectedModel.ContainsKey(kv.Key))
                    {
                        _selectedModel = modeldic;
                        return true;
                    }
                }
                _state = _residState.clean;
                return false;
            }
            else
            {
                _selectedModel = modeldic;
                return true; ;
            }
        }


        private DataTable getDFFITSTable(MultipleRegression model, DataTable dt, bool prepare)
        {
            //given a model, get dffits stat. passed dt is data used in model build
            _dffits = model.DFFITS;

            //build a sorted table/view for UI display
            DataTable dtDFFITS = getResidualsTable("DFFITS", _dffits, dt);
            DataView view = dtDFFITS.DefaultView;
            view.Sort = "DFFITS DESC";
            dgv1.DataSource = view;

            //prepare for next rebuild
            if (prepare)
            {
                //if (!_dffitsRecs2Remove.Contains(_dffitsRecno2Remove))
                {
                    _dffitsRecno2Remove = Convert.ToInt32(dgv1.Rows[0].Cells[0].Value.ToString());
                    _dffitsDateTime2Remove = dgv1.Rows[0].Cells[1].Value.ToString();
                    _dffitsResidValue2Remove = Convert.ToDouble(dgv1.Rows[0].Cells[2].Value.ToString());
                }
            }

            //update the dffits threshold (2*sqr(p/n)) where p = # of predictor variables and n = # of observations
            //(p=cols-2; 1 is timestamp, 2 is dependent variable)
            double dffitsThreshold = Convert.ToDouble(
                (2.0d * Math.Sqrt(((double)dt.Columns.Count - 2) / (double)model.PredictedValues.Length)).ToString());
            label3.Text = label3.Text.Substring(0, 13) + dffitsThreshold.ToString("f6");

            return dtDFFITS;
        }


        private DataTable getCooksDistanceTable(MultipleRegression model, DataTable dt, bool prepare)
        {
            _cooks = model.Cooks;

            DataTable dtCooks = getResidualsTable("CooksDist", _cooks, dt);
            DataView view = dtCooks.DefaultView;
            view.Sort = "CooksDist Desc";
            dgvCooks.DataSource = view;

            if (prepare)
            {

                //if (!_cooksRecs2Remove.Contains(_cooksRecno2Remove))
                {
                    //save the record, value and type to remove
                    _cooksRecno2Remove = Convert.ToInt32(dgvCooks.Rows[0].Cells[0].Value.ToString());
                    _cooksDataTime2Remove = dgvCooks.Rows[0].Cells[1].Value.ToString();
                    _cooksResidValue2Remove = Convert.ToDouble(dgvCooks.Rows[0].Cells[2].Value.ToString());
                }
            }

            //update the cooks threshold (4/n) 
            _cooksThreshold = 4.0d / (double)model.PredictedValues.Length;
            tbcookcutoff.Text = _cooksThreshold.ToString("f4");
            label10.Text = label10.Text.Substring(0, 10) + _cooksThreshold.ToString("f4");

            return dtCooks;
        }


        private DataTable getResidualsTable(string residName, double[] residuals, DataTable dtModel)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Record", typeof(System.Int32));
            dt.Columns.Add("Date/Time", typeof(System.String));
            dt.Columns.Add(residName, typeof(System.String));
            object[] values = new object[3];

            for (int rec = 0; rec < dtModel.Rows.Count; rec++)
            {
                int recno = rec;
                values[0] = Convert.ToInt32(string.Format("{0:N0}", recno.ToString()));
                values[1] = dtModel.Rows[rec][0].ToString();
                values[2] = residuals[rec].ToString("F6");
                dt.Rows.Add(values);
            }
            return dt;
        }


        private void btnGoDFFITSRebuild_Click(object sender, EventArgs e)
        {
            //iterative rebuild button clicked, get the current model table name
            //copy the table, remove the record with the largest dffits stat, rebuild the model
            //get the dffits stats and table, update the model list and recreate the plots

            DataTable dt = _modelBuildTables.Tables[_currentModelTableName];
            DataTable opdt = dt.Copy();

            opdt.Rows[_dffitsRecno2Remove].Delete();

            //track removed records here...
            _recsRemoved.Add(getRecordRemoved(_dffitsDateTime2Remove));
            _residValueRemoved.Add(_dffitsResidValue2Remove);
            _residTypeRemoved.Add("DFFITS");

            _model = computeModel(opdt);

            DataTable dffits = getDFFITSTable(_model, opdt, true);
            DataTable cooks = getCooksDistanceTable(_model, opdt, true);

            updateModelList(_model, "next", opdt, "df");
            createResidPlot("DFFITS", dffits);
            createResidPlot("CooksDistance", cooks);
            createPlot2(_model);
        }


        private void btnGoCooksIterative_Click(object sender, EventArgs e)
        {
            DataTable dt = _modelBuildTables.Tables[_currentModelTableName];
            DataTable opdt = dt.Copy();

            //_recsRemoved.Add(_cooksRecno2Remove);
            opdt.Rows[_cooksRecno2Remove].Delete();

            //track removed records here...
            _recsRemoved.Add(getRecordRemoved(_cooksDataTime2Remove));
            _residValueRemoved.Add(_cooksResidValue2Remove);
            _residTypeRemoved.Add("COOKS");

            _model = computeModel(opdt);

            DataTable dffits = getDFFITSTable(_model, opdt, true);
            DataTable cooks = getCooksDistanceTable(_model, opdt, true);

            updateModelList(_model, "next", opdt, "cd");
            createResidPlot("DFFITS", dffits);
            createResidPlot("CooksDistance", cooks);
            createPlot2(_model);
        }


        private int getRecordRemoved(string datetimestamp)
        {
            //wouldn't have to do this if grids weren't sorted (and with records recursively removed)
            int ndx = -1;
            foreach (DataRow r in _modelBuildTables.Tables[0].Rows)
            {
                ndx++;
                if (r[0].ToString() != datetimestamp) continue;
                //otherwise we found it
                return ndx;
            }
            return -1;
        }


        private void btnGoDFFITSAuto_Click(object sender, EventArgs e)
        {
            //auto rebuild until dffits < cutoff 
            //this will need some exit criteria before we run out of data
            //and/or mlr models cannot be build due to lack of data

            double maxDFFITS = Math.Abs(Convert.ToDouble(dgv1[2, 0].Value.ToString()));

            while (maxDFFITS >= _dffitsThreshold)
            {
                btnGoDFFITSRebuild_Click(null, new EventArgs());
                maxDFFITS = Math.Abs(Convert.ToDouble(dgv1[2, 0].Value.ToString()));

                DataTable dt = _modelBuildTables.Tables[_currentModelTableName];
                if (dt.Rows.Count < _maxIterations) break;
            }
        }


        private void updateModelList(MultipleRegression model, string name, DataTable dt, string residType)
        {
            //track models by their build tables and name

            //create a table name
            string listitem = string.Empty;
            int n = _modelBuildTables.Tables.Count;
            if (name == "original")
            {
                listitem = "SelectedModel";
            }
            else
            {
                listitem = "Rebuild" + n;
            }

            //track rebuild info
            _residualInfo.Add(n, residType);

            //copy the model table and save it
            DataTable newDT = new DataTable();
            newDT = dt.Copy();
            newDT.TableName = listitem.ToString();
            if (!_modelBuildTables.Tables.Contains(listitem.ToString()))
                _modelBuildTables.Tables.Add(newDT);

            //add the model (name) to the UI list
            listBox1.Items.Add(listitem);
            //and save the name for use elsewhere
            _currentModelTableName = listitem;

            //update the model stats UI lists
            updateListViews(model);
            textBox3.Text = _dffitsThreshold.ToString("f6");

            //and check if we can still rebuild models with remaining data... this need work...
            if (newDT.Rows.Count < 3) _gobtnState = false;
            btnGoDFFITSRebuild.Enabled = _gobtnState;
            btnGoDFFITSAuto.Enabled = _gobtnState;
        }


        private void updateListViews(MultipleRegression model)
        {
            //given a mlr model, update the UI lists with its stats
            /*listView1.Items.Clear();
            listView2.Items.Clear();

            string[] item = null;
            ListViewItem lvi = null;

            //show variable statistics
            int numColumns = model.Parameters.Columns.Count;
            for (int i = 0; i < model.Parameters.Rows.Count; i++)
            {
                item = new string[numColumns];
                item[0] = model.Parameters.Rows[i][0].ToString();
                item[1] = formatNumber((double)model.Parameters.Rows[i][1]);
                item[2] = formatNumber((double)model.Parameters.Rows[i][2]);
                item[3] = formatNumber((double)model.Parameters.Rows[i][3]);
                item[4] = formatNumber((double)model.Parameters.Rows[i][4]);
                item[5] = formatNumber((double)model.Parameters.Rows[i][5]);
                lvi = new ListViewItem(item);
                listView1.Items.Add(lvi);
            }

            //show model statistics
            item = new string[2];
            item[0] = "R Squared";
            item[1] = String.Format("{0:F4}", model.R2);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "Adjusted R Squared";
            item[1] = String.Format("{0:F4}", model.AdjustedR2);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "Akaike Information Criterion";
            item[1] = String.Format("{0:F4}", model.AIC);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "Corrected AIC";
            item[1] = String.Format("{0:F4}", model.AICC);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "Bayesian Info Criterion";
            item[1] = String.Format("{0:F4}", model.BIC);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "PRESS";
            item[1] = String.Format("{0:F4}", model.Press);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "RMSE";
            item[1] = String.Format("{0:F4}", model.RMSE);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            ModelErrorCounts mec = new ModelErrorCounts();
            mec.getCounts(mlrPredObs1.ThresholdHoriz, mlrPredObs1.ThresholdVert, model.PredictedValues, model.ObservedValues);

            item = new string[2];
            item[0] = "";
            item[1] = "";
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "Decision Criterion";
            item[1] = string.Format("{0:F4}", mlrPredObs1.ThresholdHoriz);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "Regulatory Standard";
            item[1] = string.Format("{0:F4}", mlrPredObs1.ThresholdVert);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "False Positives";
            item[1] = string.Format("{0:n}", mec.FPCount);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "Specificity";
            item[1] = String.Format("{0:F4}", mec.Specificity);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "False Negatives";
            item[1] = string.Format("{0:n}", mec.FNCount);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "Sensitivity";
            item[1] = String.Format("{0:F4}", mec.Sensitivity);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "Accuracy";
            item[1] = String.Format("{0:F4}", mec.Accuracy);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "";
            item[1] = "";
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            int numRecords = model.PredictedValues.Length;
            item = new string[2];
            item[0] = "Number of Observations";
            item[1] = string.Format("{0}", numRecords);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);*/
/*        }


        private string formatNumber(double number)
        {
            string fmtstring = string.Empty;
            if (number.Equals(double.NaN)) return fmtstring;
            if (Math.Abs(number) >= 10000 || Math.Abs(number) <= 0.0001)
                fmtstring = "{0:0.000#e-00}";
            else fmtstring = "{0:f4}";
            return string.Format(fmtstring, number);
        }


        private void createResidPlot(string plotname, DataTable residuals)
        {
            ZedGraphControl zgcCtrl = null;

            //switch to correct plot control
            if (plotname.Contains("DFFITS")) zgcCtrl = zgcDFFITS;
            else if (plotname.Contains("Cooks")) zgcCtrl = zgcCooks;
            else return;

            GraphPane pane = zgcCtrl.GraphPane;
            pane.Title.Text = "Residuals";
            if (pane.CurveList.Count > 0) pane.CurveList.Clear();
            PointPairList pts = getPPList(residuals, false);
            LineItem curve = pane.AddCurve(plotname, pts, Color.Blue, SymbolType.Default);
            pane.XAxis.Title.Text = "Record";
            pane.YAxis.Title.Text = plotname;
            curve.Line.IsVisible = false;

            if (zgcCtrl.Equals(zgcDFFITS))
            {
                //add threshold traces
                PointPairList dthreshold1 = new PointPairList();
                dthreshold1.Add(0.0d, _dffitsThreshold);
                dthreshold1.Add(Convert.ToDouble(residuals.Rows.Count.ToString()), _dffitsThreshold);

                PointPairList dthreshold2 = new PointPairList();
                dthreshold2.Add(0.0d, -_dffitsThreshold);
                dthreshold2.Add(Convert.ToDouble(residuals.Rows.Count.ToString()), -_dffitsThreshold);

                LineItem dcurve1 = pane.AddCurve("cutoff = " + _dffitsThreshold.ToString("f4"), dthreshold1, Color.Red, SymbolType.None);
                dcurve1.Line.IsVisible = true;
                LineItem dcurve2 = pane.AddCurve("-cutoff  = -" + _dffitsThreshold.ToString("f4"), dthreshold2, Color.Red, SymbolType.None);
                dcurve2.Line.IsVisible = true;
            }
            else if (zgcCtrl.Equals(zgcCooks))
            {
                //add threshold trace
                PointPairList cthreshold = new PointPairList();
                cthreshold.Add(0.0d, _cooksThreshold);
                cthreshold.Add(Convert.ToDouble(residuals.Rows.Count.ToString()), _cooksThreshold);

                LineItem dcurve1 = pane.AddCurve("cutoff = " + _cooksThreshold.ToString("f4"), cthreshold, Color.Red, SymbolType.None);
                dcurve1.Line.IsVisible = true;
            }
            zgcCtrl.AxisChange();
            zgcCtrl.Refresh();
        }


        private void createPlot2(dynamic model)
        {
            MasterPane master = zgc2.MasterPane;
            master.PaneList.Clear();

            master.Title.IsVisible = false;
            master.Margin.All = 10;

            //create the predictions vs studentized residuals plot;
            _predictions = (double[])((IList<object>)ipyModel.fitted).Cast<double>().ToArray();
            _standardResiduals = (double[])((IList<object>)ipyModel.residual).Cast<double>().ToArray();
            _observations = (double[])((IList<object>)ipyModel.actual).Cast<double>().ToArray();
            
            /*AndersonDarlingNormality adtest = new AndersonDarlingNormality();
            adtest.getADstat(_standardResiduals);
            double adstat1 = adtest.ADStat;
            double adstat1pval = adtest.ADStatPval;*/ /*

            //label5.Text = "A.D. Normality Statistic = " + adstat1.ToString("f4");
            //label7.Text = "A.D . Statistic P-value = " + adstat1pval.ToString("f4");

            label5.Text = "";
            label7.Text = "";

            GraphPane gpResidPlot = addPlotResid(_predictions, _standardResiduals);
            List<double[]> data = new List<double[]>();
            double[] record = null;
            for (int i = 0; i < _predictions.Length; i++)
            {
                record = new double[2];
                record[0] = _observations[i];
                record[1] =  _predictions[i];
                data.Add(record);
            }
            
            mlrPredObs1.UpdateResults(data);

            master.Add(gpResidPlot);

            using (Graphics g = this.CreateGraphics())
            { master.SetLayout(g, PaneLayout.SquareColPreferred); }

            zgc2.IsShowPointValues = true;
            zgc2.AxisChange();
            master.AxisChange();
            zgc2.Refresh();
        }


        private GraphPane addPlotResid(double[] _predictions, double[] _standardResiduals)
        {
            GraphPane pane = new GraphPane();
            pane.Title.Text = "Predictions vs Residuals";
            if (pane.CurveList.Count > 0) pane.CurveList.Clear();
            LineItem curve = pane.AddCurve(null, _predictions, _standardResiduals, Color.Blue, SymbolType.Circle);
            pane.XAxis.Title.Text = "Predictions";
            pane.YAxis.Title.Text = "Residuals";
            curve.Line.IsVisible = false;
            zgc2.AxisChange();
            zgc2.Refresh();

            return pane;
        }


        private MultipleRegression computeModel(DataTable dt)
        {
            if (_projMgr.ModelIndependentVariables == null) return null;
            //given a datatable, build a model
            MultipleRegression model = null;
            string[] idvars = _projMgr.ModelIndependentVariables.ToArray();
            string dvar = _projMgr.ModelDependentVariable;
            if (dt != null)
            {
                model = new MultipleRegression(dt, dvar, idvars);
                try { model.Compute();}
                catch { return null; }
            }
            return model;
        }


        private PointPairList getPPList(DataTable dt, bool xdate)
        {
            //create a point list for the dffits plot

            double x;
            double y;
            string tag = string.Empty;
            PointPairList plist = new PointPairList();
            foreach (DataRow r in dt.Rows)
            {
                if (xdate)
                {
                    DateTime date = Convert.ToDateTime(r[0].ToString());
                    tag = date.ToShortDateString();
                    x = new ZedGraph.XDate(date);
                    y = Convert.ToDouble(r[1].ToString());
                    tag += " (" + y.ToString() + ")";
                    plist.Add(x, y, tag);
                }
                else
                {
                    tag = r[0].ToString();
                    x = Convert.ToDouble(r[0].ToString());
                    y = Convert.ToDouble(r[2].ToString());
                    plist.Add(x, y, tag);
                }
            }
            return plist;
        }


        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //user selected a model in list, get its data table, build its model, 
            //update the model stats lists and create the plots for the model
            if (listBox1.SelectedIndex < 0) return;

            _selectedRebuild = listBox1.SelectedIndex;

            string modelname = listBox1.SelectedItem.ToString();
            DataTable dt = _modelBuildTables.Tables[modelname];
            if (dt != null)
            {
                MultipleRegression model = computeModel(dt);
                updateListViews(model);

                DataTable dffits = getDFFITSTable(model, dt, false);
                DataTable cooks = getCooksDistanceTable(model, dt, false);

                createResidPlot("DFFITS", dffits);
                createResidPlot("CooksDistance", cooks);

                createPlot2(model);

                SaveModel(dt, model);

                _state = _residState.dirty;
            }
        }


        private void SaveModel(DataTable dt, MultipleRegression model)
        {
            _projMgr.Model = model.Model;
        }


        private void textBox3_Validating(object sender, CancelEventArgs e)
        {
            //validate the user input for cutoff value
            //will probably need some smarter critera for minimum value permitted...

            double newcutoff = double.NaN;
            string teststring = textBox3.Text;
            if (!double.TryParse(teststring, out newcutoff))
            {
                MessageBox.Show("Must convert to number", "Invalid entry", MessageBoxButtons.OK);
                return;
            }
            else if (newcutoff > 3.0d)
            {
                MessageBox.Show("Value must be less than 3", "Invalid entry", MessageBoxButtons.OK);
                return;
            }
            else
            {
                _dffitsThreshold = newcutoff;
            }
        }


        private void btnViewDTDFFITS_Click(object sender, EventArgs e)
        {
            //view datatable
            frmDataTable frmDT = new frmDataTable(_recsRemoved, _residValueRemoved, 
                _residTypeRemoved, _modelBuildTables.Tables[0]);
            frmDT.ShowDialog();
        }


        private List<T> mergeLists<T> (List<T> list1, List<T> list2)
        {
            List<T> mergedList = new List<T>();
            mergedList.InsertRange(0, list1);
            mergedList.InsertRange(mergedList.Count, list2);
            return mergedList;
        }


        private void btnUseRebuiltDFFITS_Click(object sender, EventArgs e)
        {
            //push model data to project manager (for prediction)
            if (listBox1.SelectedIndex < 0)
            {
                MessageBox.Show("Select a model from the 'Models' list.", "Selection Required", MessageBoxButtons.OK);
                return;
            }

            string modelname = listBox1.SelectedItem.ToString();
            DataTable dt = _modelBuildTables.Tables[modelname];
            MultipleRegression model = computeModel(dt);

            //save the model in the project manager
            _projMgr.Model = model.Model;
            _projMgr._comms.sendMessage("Show the MLR Prediction form.", this);
        }


        private void btnViewDTCooks_Click(object sender, EventArgs e)
        {
            btnViewDTDFFITS_Click(this, new EventArgs());
        }


        private void btnUseRebuiltCooks_Click(object sender, EventArgs e)
        {
            btnUseRebuiltDFFITS_Click(this, new EventArgs());
        }


        private void btnGoCooksAuto_Click(object sender, EventArgs e)
        {
            //auto rebuild until cooks < cutoff 
            //this will need some exit criteria before we run out of data
            //and/or mlr models cannot be build due to lack of data

            double maxCOOKS = Math.Abs(Convert.ToDouble(dgvCooks [2, 0].Value.ToString()));

            while (maxCOOKS >= _cooksThreshold)
            {
                btnGoCooksIterative_Click(null, new EventArgs());
                maxCOOKS = Math.Abs(Convert.ToDouble(dgvCooks[2, 0].Value.ToString()));

                DataTable dt = _modelBuildTables.Tables[_currentModelTableName];
                if (dt.Rows.Count < _maxIterations) break;
            }
        }


        private void frmResiduals_HelpRequested(object sender, HelpEventArgs hlpevent)
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


        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.C) && (e.Modifiers == Keys.Control))
                CopyListViewToClipboard(listView1);
        }


        private void listView2_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.C) && (e.Modifiers == Keys.Control))
                CopyListViewToClipboard(listView2);
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
        } */
    }
}
