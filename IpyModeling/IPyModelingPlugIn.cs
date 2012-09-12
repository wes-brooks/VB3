using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using DotSpatial.Controls;
using System.ComponentModel.Composition;
using DotSpatial.Controls.Header;
using DotSpatial.Controls.Docking;
using VBCommon;
using VBCommon.Interfaces;

namespace IPyModeling
{
    //Modeling Plugin
    public abstract class IPyModelingPlugin : Extension, IPartImportsSatisfiedNotification, IPlugin
    {       
        [Import("Shell")]
        private ContainerControl Shell { get; set; }

        protected string strPanelKey = "GenericIronPythonModelPanelKey";
        protected string strPanelCaption = "GenericIronPythonModelPanelCaption";
        private Globals.PluginType pluginType = VBCommon.Globals.PluginType.Modeling;
        private RootItem rootHeaderItem;

        IDictionary<string, object> dictPlugin = null;

        //instance of  class
        protected IPyModeling.IPyModelingControl innerIronPythonControl;
        private VBCommon.Signaller signaller;
        //ribbon buttons
        private SimpleActionItem btnRun;
        private SimpleActionItem btnCancel;
        private SimpleActionItem btnComputeAO;
        private SimpleActionItem btnManipulate;
        private SimpleActionItem btnTransform;
                
        //Raise a message
        public delegate void MessageHandler<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
        public event MessageHandler<VBCommon.PluginSupport.MessageArgs> MessageSent;

        //complete and visible flags
        public Boolean boolComplete;
        public Boolean boolVisible;
        public Boolean boolRunCancelled;
        public Boolean boolStopRun;
        public Boolean boolInitialEntry = true; //first time here flag
     
        private Boolean boolClearModel; 

        //this plugin was clicked
        private string strTopPlugin = string.Empty;


        //property to update topPlugin and raise event when changed
        public string TopPlugin
        {
            get { return strTopPlugin; }
            set
            {
                strTopPlugin = value;
                signaller.RaiseStrPluginChange(strTopPlugin);
            }
        }


        //deactivate this plugin
        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            App.DockManager.Remove(strPanelKey);
            innerIronPythonControl = null;
            base.Deactivate();
        }


        //hide this plugin
        public void Hide()
        {
            //set visible flag to false
            boolVisible = false;
            //hide pluginTab
            App.HeaderControl.RemoveAll();
            //hide plugin panel
            ((VBDockManager.VBDockManager)App.DockManager).HidePanel(strPanelKey);
        }


        //show this plugin
        public void Show()
        {
            //set visible flag to true
            boolVisible = true;
            //show the tab
            AddRibbon("Show");
            //show the panel
            ((VBDockManager.VBDockManager)App.DockManager).SelectPanel(strPanelKey);
            App.HeaderControl.SelectRoot(strPanelKey);   
        }


        //make this plugin the active one
        public void MakeActive()
        {
            boolVisible = true;
            App.DockManager.SelectPanel(strPanelKey);
            App.HeaderControl.SelectRoot(strPanelKey);
        }


        //initial activate method when loaded
        public override void Activate()
        {
            AddPanel();
            AddRibbon("Activate");

            //when panel is selected activate seriesview and ribbon tab
            App.DockManager.ActivePanelChanged += new EventHandler<DotSpatial.Controls.Docking.DockablePanelEventArgs>(DockManager_ActivePanelChanged);
            App.HeaderControl.RootItemSelected += new EventHandler<RootItemEventArgs>(HeaderControl_RootItemSelected);
            innerIronPythonControl.ModelUpdated += new EventHandler(HandleUpdatedModel);
            innerIronPythonControl.boolRunChanged += new IPyModelingControl.BoolChangedEvent(HandleBoolRunChanged);
            innerIronPythonControl.boolStopRun += new IPyModelingControl.BoolStopEvent(HandleBoolStopRun);
            innerIronPythonControl.ManipulateDataTab += new EventHandler(HandleManipulateDataTab);
            innerIronPythonControl.ModelTab += new EventHandler(HandleModelTab);
            innerIronPythonControl.VariableTab += new EventHandler(HandleVariableTab);
            
            base.Activate(); //ensures "enabled" is set to true
        }


        //a root item (plugin) has been selected
        void HeaderControl_RootItemSelected(object sender, RootItemEventArgs e)
        {
            if (e.SelectedRootKey == strPanelKey)
            {
                App.DockManager.SelectPanel(strPanelKey);
                //make this the top plugin for ProjMngr to use when opening
                TopPlugin = strPanelKey;
            }
        }


        //add a datasheet plugin root item
        public void AddRibbon(string sender)
        {
            rootHeaderItem = new RootItem(strPanelKey, strPanelCaption);
            rootHeaderItem.SortOrder = (short)pluginType;
            App.HeaderControl.Add(rootHeaderItem);
            
            //tell ProjMngr if this is being Shown
            if (sender == "Show")
            {
                //make this the selected root
                App.HeaderControl.SelectRoot(strPanelKey); 
            }

            
            //add sub-ribbon
            string grpManipulate = "Manipulate Data";

            btnComputeAO = new SimpleActionItem(strPanelKey, "Compute A O", innerIronPythonControl.btnComputeAO_Click);
            btnComputeAO.LargeImage = Properties.Resources.EPAComputeAO;
            btnComputeAO.GroupCaption = grpManipulate;
            btnComputeAO.Enabled = true;
            App.HeaderControl.Add(btnComputeAO);

            btnManipulate = new SimpleActionItem(strPanelKey, "Manipulate", innerIronPythonControl.btnManipulate_Click);
            btnManipulate.LargeImage = Properties.Resources.EPAmanipulate;
            btnManipulate.GroupCaption = grpManipulate;
            btnManipulate.Enabled = true;
            App.HeaderControl.Add(btnManipulate);

            btnTransform = new SimpleActionItem(strPanelKey, "Transform", innerIronPythonControl.btnTransform_Click);
            btnTransform.LargeImage = Properties.Resources.EPAtransform;
            btnTransform.GroupCaption = grpManipulate;
            btnTransform.Enabled = true;
            App.HeaderControl.Add(btnTransform);


            
            string rGroupCaption = "Model";

            btnRun = new SimpleActionItem(strPanelKey, "Run", btnRun_Click);
            btnRun.LargeImage = Properties.Resources.running_process;
            btnRun.GroupCaption = rGroupCaption;
            btnRun.Enabled = false;
            App.HeaderControl.Add(btnRun);

            btnCancel = new SimpleActionItem(strPanelKey, "Cancel", btnCancel_Click);
            btnCancel.LargeImage = Properties.Resources.Cancel;
            btnCancel.GroupCaption = rGroupCaption;
            btnCancel.Enabled = false;
            App.HeaderControl.Add(btnCancel);
        }


        //add the panel content within the plugin
        public void AddPanel()
        {
            var dp = new DockablePanel(strPanelKey, strPanelCaption, innerIronPythonControl, DockStyle.Fill);
            dp.DefaultSortOrder = (short)pluginType;
            App.DockManager.Add(dp);
        }


        //event handler when a plugin is selected from tabs
        void DockManager_ActivePanelChanged(object sender, DotSpatial.Controls.Docking.DockablePanelEventArgs e)
        {
            if (e.ActivePanelKey == strPanelKey)
            {
                App.DockManager.SelectPanel(strPanelKey);
                App.HeaderControl.SelectRoot(strPanelKey);
            }
            if (e.ActivePanelKey.ToString() == "DataSheetPanel" && boolVisible)
                Hide();
            if (e.ActivePanelKey == "IPyPrediction")
                Show();
        }


        //returns panel key name
        public string PanelKey
        {
            get { return strPanelKey; }
        }


        //returns this model's packed state
        public IDictionary<string, object> PackedPlugin
        {
            get { return dictPlugin; }
            set { dictPlugin = value; }
        }


        //returns plugin type (Modeling)
        public Globals.PluginType PluginType
        {
            get { return pluginType; }
        }


        //keep track if model already has a datasheet
        public Boolean InitialEntry
        {
            get { return boolInitialEntry; }
        }


        //return clear model flag
        public Boolean ClearModel
        {
            get { return boolClearModel; }
        }


        //returns complete flag
        public Boolean Complete
        {
            get { return boolComplete; }
        }


        //returns visible flag
        public Boolean VisiblePlugin
        {
            get { return boolVisible; }
        }


        //This function imports the signaller from the VBProjectManager
        [System.ComponentModel.Composition.Import("Signalling.GetSignaller", AllowDefault = true)]
        public Func<VBCommon.Signaller> GetSignaller
        {
            get;
            set;
        }


        public void OnImportsSatisfied()
        {
            //If we've successfully imported a Signaller, then connect its events to our handlers.
            signaller = GetSignaller();
            signaller.BroadcastState += new VBCommon.Signaller.BroadCastEventHandler<VBCommon.PluginSupport.BroadCastEventArgs>(BroadcastStateListener);
          //  signaller.strPluginTopChanged += new VBCommon.Signaller.UpdateStrPluginKey<VBCommon.PluginSupport.UpdateStrPlugOnTopEventArgs>(strPluginTopChgdListener);
            signaller.ProjectSaved += new VBCommon.Signaller.SerializationEventHandler<VBCommon.PluginSupport.SerializationEventArgs>(ProjectSavedListener);
            signaller.ProjectOpened += new VBCommon.Signaller.SerializationEventHandler<VBCommon.PluginSupport.SerializationEventArgs>(ProjectOpenedListener);
            this.MessageSent += new MessageHandler<VBCommon.PluginSupport.MessageArgs>(signaller.HandleMessage);
        }

        
        //event listener for plugin broadcasting changes
        private void BroadcastStateListener(object sender, VBCommon.PluginSupport.BroadCastEventArgs e)
        {
            //if datasheet updated itself, set data with changes, passing datasheet's plugin packed state
            if (((IPlugin)sender).PluginType == Globals.PluginType.Datasheet)
            {
                //no changes made, and not first time here = don't set the data, just show what was there
                if (!(bool)e.PackedPluginState["ChangesMadeDS"] && !InitialEntry)
                {
                    if (dictPlugin != null)
                    {
                        if (dictPlugin.Count > 3)
                        {
                            Dictionary<string, object> dictDatasheet = (Dictionary<string, object>)dictPlugin["PackedDatasheetState"];
                            string xmlDT = (string)dictDatasheet["XmlDataTable"];
                            StringReader sr = new StringReader(xmlDT);
                            DataSet ds = new DataSet();
                            ds.ReadXml(sr);
                            sr.Close();
                            DataTable dt = ds.Tables[0];
                            innerIronPythonControl.UnhideDatasheet(dt);
                            MakeActive();
                        }
                    }
                }
                else
                {
                    boolInitialEntry = false;
                    //this tells projectManager that the modeling isn't complete, so don't show prediction when unhidden
                    if ((bool)e.PackedPluginState["ChangesMadeDS"])
                        boolComplete = false;
                    
                    innerIronPythonControl.SetData(e.PackedPluginState);
                }
            }
            //if the prediction is broadcasting, this project is opening and needs model to show itself if prediction is complete
            if (((IPlugin)sender).PluginType == Globals.PluginType.Prediction)
                if ((bool)e.PackedPluginState["Complete"])
                    Show();
        }


        ////listens for change in pluginKeyString
        //private void strPluginTopChgdListener(VBCommon.PluginSupport.UpdateStrPlugOnTopEventArgs value)
        //{

        //}


        //only have manipulate datasheet buttons enabled when on Manipulate Datasheet tab
        public void HandleManipulateDataTab(object sender, EventArgs e)
        {
            try
            {
                btnComputeAO.Enabled = true;
                btnManipulate.Enabled = true;
                btnTransform.Enabled = true;
                btnRun.Enabled = false;
            }
            catch
            { }
        }


        // only have model buttons enabled
        public void HandleModelTab(object sender, EventArgs e)
        {
            try
            {
                btnComputeAO.Enabled = false;
                btnManipulate.Enabled = false;
                btnTransform.Enabled = false;
                btnRun.Enabled = true;
            }
            catch
            { }
        }


        //no buttons enabled for the variable selection tab
        public void HandleVariableTab(object sender, EventArgs e)
        {
            try
            {
                btnComputeAO.Enabled = false;
                btnManipulate.Enabled = false;
                btnTransform.Enabled = false;
                btnRun.Enabled = false;
            }
            catch
            { }
        }

        //when modeling makes changes, event broadcasts changes to those listening
        public void Broadcast()
        {
            //get packed state, add complete and visible and raise broadcast event
            IDictionary<string, object> dictPackedState = innerIronPythonControl.PackProjectState();
            if (dictPackedState.ContainsKey("ModelByObject"))
            {
                if (dictPackedState["ModelByObject"] != null)
                    boolComplete = true;
            }
            else
                dictPackedState.Add("CleanPredict", true); //if the model has been cleared, lets clear the prediction too

            dictPackedState.Add("Complete", boolComplete);
            dictPackedState.Add("Visible", boolVisible);
            signaller.RaiseBroadcastRequest(this, dictPackedState);
        }


        //event handler for saving project state
        private void ProjectSavedListener(object sender, VBCommon.PluginSupport.SerializationEventArgs e)
        {
            //go pack state, add complete and visible, and add to dictionary of plugins
            IDictionary<string, object> packedState = innerIronPythonControl.PackProjectState();
            packedState.Add("Complete", boolComplete);
            packedState.Add("Visible", boolVisible);

            e.PackedPluginStates.Add(strPanelKey, packedState);
        }


        //event handler for opening project state
        private void ProjectOpenedListener(object sender, VBCommon.PluginSupport.SerializationEventArgs e)
        {
            //if modeling is in the list of packed plugins, go unpack 
            if (e.PackedPluginStates.ContainsKey(strPanelKey))
            {
                dictPlugin = e.PackedPluginStates[strPanelKey];
                //repopulate plugin Complete flags from saved project
                boolComplete = (bool)dictPlugin["Complete"];
                if (boolComplete)
                    boolInitialEntry = false; //opening projects have been entered before
                //check to see if there already is a PLS model open, if so, close it before opening a saved project
                if ((VisiblePlugin) && (Complete))
                    Hide();
                
                //make model being open active plugin
                if ((bool)dictPlugin["Visible"])
                    Show();
               
                innerIronPythonControl.UnpackProjectState(e.PackedPluginStates[strPanelKey]);
            }
            else
            {
                //Set this plugin to an empty state.
                Activate();
            }
        }

     
        public void TestMessage(object sender, EventArgs e)
        {
            
            SendMessage("Message sent from: " + strPanelKey + "!");
        }


        private void SendMessage(string message)
        {
            if (MessageSent != null) //Has some method been told to handle this event?
            {
                VBCommon.PluginSupport.MessageArgs e = new VBCommon.PluginSupport.MessageArgs(message);
                MessageSent(this, e);
            }
        }

        
        private void HandleBoolStopRun(bool val)
        {
            boolStopRun = val;
        }
            

        //request to run the modeling method, and then enable the prediction page.
        void btnRun_Click(object sender, EventArgs e)
        {
            if (innerIronPythonControl.lbIndVariables.Items.Count == 0)
            {
                MessageBox.Show("You must chose variables first and go to model tab before selecting Run", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            innerIronPythonControl.btnRun_Click(sender, e);

            //check to see if run was clicked before model tab was active
            if (boolStopRun)
                return;
            //check to see if the model has been canceled first
            if (boolRunCancelled)
                return;

            ////this model is complete and ready for prediction
            //boolComplete = true;
            //Broadcast();

            //make modeling the focus again (Broadcast() makes Prediction visible and 'on top')
            MakeActive();
        }


        //cancel the running model
        void btnCancel_Click(object sender, EventArgs e)
        {
            boolRunCancelled = true;
            innerIronPythonControl.btnCancel_Click(sender, e);
        }

        
        
        //handle when model boolean Running flag changes
        private void HandleBoolRunChanged(bool val)
        {
            if (val)
            //make cancel button enabled
            {
                Cursor.Current = Cursors.WaitCursor;
                btnRun.Enabled = false;
                btnCancel.Enabled = true;
            }
            else
            //make run button enabled
            {
                btnRun.Enabled = true;
                btnCancel.Enabled = false;
            }
        }

        //change has been made within modeling, need to update
        private void HandleUpdatedModel(object sender, EventArgs e)
        {
            //if here, changes were made and model has been cleared
            boolComplete = false;
            Broadcast();
            //bring the focus back to Modeling away from Prediction
            MakeActive();
            Cursor.Current = Cursors.Default;
        }
    }
}