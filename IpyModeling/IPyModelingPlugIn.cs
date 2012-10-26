using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using System.Drawing;
using DotSpatial.Controls;
using System.ComponentModel.Composition;
using DotSpatial.Controls.Header;
using DotSpatial.Controls.Docking;
using VBCommon;
using VBCommon.PluginSupport;
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

        //IDictionary<string, object> dictPlugin = null;

        //instance of  class
        protected IPyModeling.IPyModelingControl innerIronPythonControl;
        private VBCommon.Signaller signaller;
        //ribbon buttons
        private SimpleActionItem btnRun;
        //private SimpleActionItem btnCancel;
        private SimpleActionItem btnComputeAO;
        private SimpleActionItem btnManipulate;
        private SimpleActionItem btnTransform;
                
        //Raise a message
        public delegate void MessageHandler<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
        public event MessageHandler<VBCommon.PluginSupport.MessageArgs> MessageSent;

        //complete and visible flags
        public Boolean boolComplete = false;
        public Boolean boolVisible;
        public Boolean boolRunning;
        public Boolean boolVirgin = true;

        private string strTopPlugin = string.Empty;

        
        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            App.DockManager.Remove(strPanelKey);
            innerIronPythonControl = null;
            base.Deactivate();
        }


        public void Hide()
        {
            boolVisible = false;
            App.HeaderControl.RemoveAll();
            ((VBDockManager.VBDockManager)App.DockManager).HidePanel(strPanelKey);
        }


        public void Show()
        {
            if (boolVisible)
            {
                //If this plugin is already visible, then do nothing.
                return;
            }
            else
            {                
                AddRibbon("Show");
                //((VBDockManager.VBDockManager)App.DockManager).SelectPanel(strPanelKey);
                //App.HeaderControl.SelectRoot(strPanelKey);
                boolVisible = true;
            }
        }


        public void ActivePluginChanged(object sender, DotSpatial.Controls.Docking.DockablePanelEventArgs e)
        {
            if (e.ActivePanelKey == strPanelKey)
            {                
                App.DockManager.SelectPanel(strPanelKey);
                App.HeaderControl.SelectRoot(strPanelKey);
            }
            else
            {
                foreach (DotSpatial.Extensions.IExtension x in App.Extensions)
                {
                    if (x is IPlugin)
                    {
                        if (((IPlugin)x).PanelKey == e.ActivePanelKey && ((IPlugin)x).PluginType <= VBCommon.Globals.PluginType.Datasheet)
                            Hide();
                    }
                }
            }
        }


        public void MakeActive()
        {
            App.HeaderControl.SelectRoot(strPanelKey); 
            App.DockManager.SelectPanel(strPanelKey);                       
        }


        public override void Activate()
        {
            AddPanel();
            AddRibbon("Activate");
            boolVisible = true;

            //when panel is selected activate seriesview and ribbon tab
            App.DockManager.ActivePanelChanged += new EventHandler<DotSpatial.Controls.Docking.DockablePanelEventArgs>(DockManager_ActivePanelChanged);
            App.HeaderControl.RootItemSelected += new EventHandler<RootItemEventArgs>(HeaderControl_RootItemSelected);
            innerIronPythonControl.ModelUpdated += new EventHandler(HandleUpdate);
            innerIronPythonControl.ModelingTabControl.SelectedIndexChanged += new EventHandler(UpdateControlStatus);
            //innerIronPythonControl.boolStopRun += new IPyModelingControl.BoolStopEvent(HandleBoolStopRun);
            //innerIronPythonControl.ManipulateDataTab += new EventHandler(HandleManipulateDataTab);
            //innerIronPythonControl.ModelTab += new EventHandler(HandleModelTab);
            //innerIronPythonControl.VariableTab += new EventHandler(HandleVariableTab);
            
            base.Activate();
        }


        //a root item (plugin) has been selected
        void HeaderControl_RootItemSelected(object sender, RootItemEventArgs e)
        {
            if (e.SelectedRootKey == strPanelKey)
            {
                this.MakeActive();
                //App.DockManager.SelectPanel(strPanelKey);
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

            /*btnCancel = new SimpleActionItem(strPanelKey, "Cancel", btnCancel_Click);
            btnCancel.LargeImage = Properties.Resources.Cancel;
            btnCancel.GroupCaption = rGroupCaption;
            btnCancel.Enabled = false;
            App.HeaderControl.Add(btnCancel);*/
        }


        //add the panel content within the plugin
        public void AddPanel()
        {
            //App.DockManager.SelectPanel(strPanelKey);
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
        }


        //returns panel key name
        public string PanelKey
        {
            get { return strPanelKey; }
        }


        //returns plugin type (Modeling)
        public Globals.PluginType PluginType
        {
            get { return pluginType; }
        }


        //returns complete flag
        public Boolean Complete
        {
            get { return boolComplete; }
        }


        //returns visible flag
        public Boolean Visible
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
            signaller.BroadcastState += new VBCommon.Signaller.BroadcastEventHandler<VBCommon.PluginSupport.BroadcastEventArgs>(BroadcastStateListener);
            signaller.ProjectSaved += new VBCommon.Signaller.SerializationEventHandler<VBCommon.PluginSupport.SerializationEventArgs>(ProjectSavedListener);
            signaller.ProjectOpened += new VBCommon.Signaller.SerializationEventHandler<VBCommon.PluginSupport.SerializationEventArgs>(ProjectOpenedListener);

            this.MessageSent += new MessageHandler<VBCommon.PluginSupport.MessageArgs>(signaller.HandleMessage);
        }


        //event listener for plugin broadcasting changes
        private void BroadcastStateListener(object sender, VBCommon.PluginSupport.BroadcastEventArgs e)
        {            
            if (!(bool)((IPlugin)sender).Complete)
                return;

            //if datasheet updated itself, set data with changes
            if (((IPlugin)sender).PluginType == Globals.PluginType.Datasheet)
            {
                IDictionary<string, object> dictPluginState = (IDictionary<string, object>)(e.PackedPluginState);
                if (!(bool)dictPluginState["Complete"])
                {
                    this.Hide();
                    return;
                }
                else if (boolVirgin)
                {
                    if (dictPluginState != null)
                    {
                        if (dictPluginState.Count > 3)
                        {
                            boolVirgin = false;
                            innerIronPythonControl.SetData(e.PackedPluginState);
                            Show();
                        }
                    }
                }
                else
                {
                    if (!(bool)e.PackedPluginState["Clean"])
                    {
                        boolComplete = false;
                        innerIronPythonControl.SetData(e.PackedPluginState);
                    }
                    Show();
                }
            }
            else
            {
                //This handles an undo:
                try
                {
                    if (((IPlugin)sender).PluginType == VBCommon.Globals.PluginType.ProjectManager)
                    {
                        if (e.PackedPluginState["Sender"].ToString() == strPanelKey)
                        {
                            IDictionary<string, object> dictPlugin = e.PackedPluginState;

                            Show();
                            MakeActive();

                            innerIronPythonControl.UnpackState(dictPlugin);

                            boolComplete = (bool)dictPlugin["Complete"];
                            boolVisible = (bool)dictPlugin["Visible"];
                            boolVirgin = (bool)dictPlugin["Virgin"];

                            if (boolVisible)
                                Show();
                            else
                                Hide();
                        }
                    }
                }
                catch { }
            }       
        }



        public void UpdateControlStatus(object sender, EventArgs e)
        {
            try
            {
                string strActiveTabName  =innerIronPythonControl.ModelingTabControl.SelectedTab.Name.ToString();

                if (strActiveTabName == "DatasheetTab")
                {
                    btnComputeAO.Enabled = true;
                    btnManipulate.Enabled = true;
                    btnTransform.Enabled = true;
                    btnRun.Enabled = false;
                }
                else if (strActiveTabName == "VariableSelectionTab")
                {
                    btnComputeAO.Enabled = false;
                    btnManipulate.Enabled = false;
                    btnTransform.Enabled = false;
                    btnRun.Enabled = false;
                }
                else if (strActiveTabName == "ModelingTab")
                {
                    btnComputeAO.Enabled = false;
                    btnManipulate.Enabled = false;
                    btnTransform.Enabled = false;
                    btnRun.Enabled = true;
                    innerIronPythonControl.SetModelData();
                }
                else if (strActiveTabName == "DiagnosticTab")
                {
                    btnComputeAO.Enabled = false;
                    btnManipulate.Enabled = false;
                    btnTransform.Enabled = false;
                    btnRun.Enabled = false;
                }
            }
            catch
            { }
        }


        /*public void HandleManipulateDataTab(object sender, EventArgs e)
        {
            //only have manipulate datasheet buttons enabled when on Manipulate Datasheet tab
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

        
        public void HandleModelTab(object sender, EventArgs e)
        {
            // only have model buttons enabled
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

        
        public void HandleVariableTab(object sender, EventArgs e)
        {
            //no buttons enabled for the variable selection tab
            try
            {
                btnComputeAO.Enabled = false;
                btnManipulate.Enabled = false;
                btnTransform.Enabled = false;
                btnRun.Enabled = false;
            }
            catch
            { }
        }*/


        /*//handles broadcasting each change to be added to the stack
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
                    innerIronPythonControl.ClearModelingTab();
                }
            }
            Broadcast();
        }*/


        //when modeling makes changes, event broadcasts changes to those listening
        public void Broadcast()
        {
            //get packed state, add complete and visible and raise broadcast event
            IDictionary<string, object> dictPackedState = innerIronPythonControl.PackState();
            dictPackedState["Origin"] = strPanelKey;

            if (dictPackedState.ContainsKey("Model"))
            {
                if (dictPackedState["Model"] != null)
                    boolComplete = true;
            }

            dictPackedState.Add("Complete", innerIronPythonControl.Complete);
            dictPackedState.Add("Visible", boolVisible);
            signaller.RaiseBroadcastRequest(this, dictPackedState);
        }


        public IDictionary<string, object> GetPackedState()
        {
            //get packed state, add complete and visible and raise broadcast event
            IDictionary<string, object> dictPackedState = innerIronPythonControl.PackState();
            dictPackedState["Origin"] = strPanelKey;

            if (dictPackedState.ContainsKey("Model"))
            {
                if (dictPackedState["Model"] != null)
                    boolComplete = true;
            }

            dictPackedState.Add("Complete", innerIronPythonControl.Complete);
            dictPackedState.Add("Visible", boolVisible);
            return dictPackedState;
        }


        //event handler for saving project state
        private void ProjectSavedListener(object sender, VBCommon.PluginSupport.SerializationEventArgs e)
        {
            //go pack state, add complete and visible, and add to dictionary of plugins
            IDictionary<string, object> packedState = innerIronPythonControl.PackState();

            if (packedState != null)
            {
                packedState.Add("Complete", boolComplete);
                packedState.Add("Visible", boolVisible);
                packedState.Add("Virgin", boolVirgin);
                
                e.PackedPluginStates.Add(strPanelKey, packedState);
            }
        }


        private void ProjectOpenedListener(object sender, VBCommon.PluginSupport.SerializationEventArgs e)
        {
            if (e.PackedPluginStates.ContainsKey(strPanelKey))
            {
                IDictionary<string, object> dictPlugin = e.PackedPluginStates[strPanelKey];

                //Show();
                MakeActive();

                innerIronPythonControl.UnpackState(dictPlugin);

                boolComplete = (bool)dictPlugin["Complete"];
                boolVisible = (bool)dictPlugin["Visible"];
                boolVirgin = (bool)dictPlugin["Virgin"];

                if (boolVisible)
                    Show();
                else
                    Hide();               
            }
            else
            {
                innerIronPythonControl.Clear();
            }
        }


        private void SendMessage(string message)
        {
            if (MessageSent != null)
            {
                VBCommon.PluginSupport.MessageArgs e = new VBCommon.PluginSupport.MessageArgs(message);
                MessageSent(this, e);
            }
        }


        void btnRun_Click(object sender, EventArgs e)
        {
            if (innerIronPythonControl.lbIndVariables.Items.Count == 0)
            {
                MessageBox.Show("You must chose variables first and go to model tab before selecting Run", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            innerIronPythonControl.btnRun_Click(sender, e);

            if (boolRunning)
                return;

            MakeActive();
            boolComplete = true;
        }


        /*void btnCancel_Click(object sender, EventArgs e)
        {
            boolRunning = false;
            innerIronPythonControl.btnCancel_Click(sender, e);
        }*/

        
        /*//handle when model boolean Running flag changes
        private void HandleRunCancelEnableState(bool val)
        {
            if (boolRunning)
            {
                Cursor.Current = Cursors.WaitCursor;
                btnRun.Enabled = false;
                //btnCancel.Enabled = true;
            }
            else
            {
                btnRun.Enabled = true;
                //btnCancel.Enabled = false;
            }
        }*/


        //change has been made within modeling, need to update
        private void HandleUpdate(object sender, EventArgs e)
        {
            /*if (boolComplete)
            {
                boolComplete = false;
                MakeActive();
                Cursor.Current = Cursors.Default;
            }*/
            Broadcast();
        }
    }
}