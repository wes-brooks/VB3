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
        public Boolean boolInitialEntry = true;

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
            boolVisible = false;
            App.HeaderControl.RemoveAll();
            ((VBDockManager.VBDockManager)App.DockManager).HidePanel(strPanelKey);
        }


        //show this plugin
        public void Show()
        {
            boolVisible = true;
            AddRibbon("Show");
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
            innerIronPythonControl.boolRunChanged += new IPyModelingControl.BoolChangedEvent(HandleRunCancelEnableState);
 //           innerIronPythonControl.boolStopRun += new IPyModelingControl.BoolStopEvent(HandleBoolStopRun);
            innerIronPythonControl.ManipulateDataTab += new EventHandler(HandleManipulateDataTab);
            innerIronPythonControl.ModelTab += new EventHandler(HandleModelTab);
            innerIronPythonControl.VariableTab += new EventHandler(HandleVariableTab);
            innerIronPythonControl.ChangeMade4Stack += new EventHandler(HandleAddToStack);
            
            base.Activate();
        }


        //a root item (plugin) has been selected
        void HeaderControl_RootItemSelected(object sender, RootItemEventArgs e)
        {
            if (e.SelectedRootKey == strPanelKey)
            {
                App.DockManager.SelectPanel(strPanelKey);
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
            //hide this plugin when going back to global datasheet
            if (e.ActivePanelKey.ToString() == "DataSheetPanel" && boolVisible)
                Hide();
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
            signaller.ProjectSaved += new VBCommon.Signaller.SerializationEventHandler<VBCommon.PluginSupport.SerializationEventArgs>(ProjectSavedListener);
            signaller.ProjectOpened += new VBCommon.Signaller.SerializationEventHandler<VBCommon.PluginSupport.SerializationEventArgs>(ProjectOpenedListener);
            this.MessageSent += new MessageHandler<VBCommon.PluginSupport.MessageArgs>(signaller.HandleMessage);
        }


        //undo was hit, send the packed state to be unpacked
        public void UndoLastChange(Dictionary<string, object> packedState)
        {
            innerIronPythonControl.SetData(packedState);
        }


        //event listener for plugin broadcasting changes
        private void BroadcastStateListener(object sender, VBCommon.PluginSupport.BroadCastEventArgs e)
        {
            //get out of here if global ds is just making changes to be added to the stack
            if (!(bool)((IPlugin)sender).Complete)
                return;

            //if datasheet updated itself, set data with changes
            if (((IPlugin)sender).PluginType == Globals.PluginType.Datasheet)
            {
                //no changes made, and not first time here = don't set the data, just show what was there
                if (!(bool)e.PackedPluginState["ChangesMadeDS"] && !InitialEntry)
                {
                    innerIronPythonControl.Refresh();
                    MakeActive();
                }
                else
                {
                    //if changes were made or it is initial pass, set the data
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


        //handles broadcasting each change to be added to the stack
        public void HandleAddToStack(object sender, EventArgs e)
        {            
            if (boolComplete)
            {
                DialogResult dlgr = MessageBox.Show("Changes in data and/or data attributes have occurred.\nPrevious modeling results will be erased. Proceed?", "Proceed to Modeling.", MessageBoxButtons.OKCancel);
                if (dlgr == DialogResult.Cancel)
                {
                    //then use that stack here to undo??
                    return;
                }
                else if (dlgr == DialogResult.OK)
                {
                    boolComplete = false;
                    innerIronPythonControl.Clear();
                }
            }
            Broadcast();
        }


        //when modeling makes changes, event broadcasts changes to those listening
        public void Broadcast()
        {
            IDictionary<string, object> dictPackedState = innerIronPythonControl.PackProjectState();
            if (dictPackedState.ContainsKey("ModelByObject"))
            {
                if (dictPackedState["ModelByObject"] != null)
                    boolComplete = true;
            }

            dictPackedState.Add("Complete", boolComplete);
            dictPackedState.Add("Visible", boolVisible);
            signaller.RaiseBroadcastRequest(this, dictPackedState);
        }


        //event handler for saving project state
        private void ProjectSavedListener(object sender, VBCommon.PluginSupport.SerializationEventArgs e)
        {
            IDictionary<string, object> packedState = innerIronPythonControl.PackProjectState();
            if (packedState != null)
            {
                packedState.Add("Complete", boolComplete);
                packedState.Add("Visible", boolVisible);
                
                e.PackedPluginStates.Add(strPanelKey, packedState);
            }
        }


        //event handler for opening project state
        private void ProjectOpenedListener(object sender, VBCommon.PluginSupport.SerializationEventArgs e)
        {
            if (e.PackedPluginStates.ContainsKey(strPanelKey))
            {
                dictPlugin = e.PackedPluginStates[strPanelKey];
                boolComplete = (bool)dictPlugin["Complete"];
                boolVisible = (bool)dictPlugin["Visible"];
               
                if (boolComplete)
                    boolInitialEntry = false;

                if (boolVisible)
                    Show();
                else
                    Hide();
               
                innerIronPythonControl.UnpackProjectState(e.PackedPluginStates[strPanelKey]);
            }
        }

     
        public void TestMessage(object sender, EventArgs e)
        {
            SendMessage("Message sent from: " + strPanelKey + "!");
        }


        private void SendMessage(string message)
        {
            if (MessageSent != null)
            {
                VBCommon.PluginSupport.MessageArgs e = new VBCommon.PluginSupport.MessageArgs(message);
                MessageSent(this, e);
            }
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

            if (boolRunCancelled)
                return;

            MakeActive();
            boolComplete = true;
        }


        //cancel the running model
        void btnCancel_Click(object sender, EventArgs e)
        {
            boolRunCancelled = true;
            innerIronPythonControl.btnCancel_Click(sender, e);
        }

        
        //handle when model boolean Running flag changes
        private void HandleRunCancelEnableState(bool val)
        {
            if (val)
            {
                Cursor.Current = Cursors.WaitCursor;
                btnRun.Enabled = false;
                btnCancel.Enabled = true;
            }
            else
            {
                btnRun.Enabled = true;
                btnCancel.Enabled = false;
            }
        }


        //change has been made within modeling, need to update
        private void HandleUpdatedModel(object sender, EventArgs e)
        {
            if (boolComplete)
            {
                //if here, changes were made and model has been cleared
                boolComplete = false;
                
                MakeActive();
                Cursor.Current = Cursors.Default;
            }
            Broadcast();
        }
    }
}