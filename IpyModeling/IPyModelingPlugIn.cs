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

        protected string strPanelKey;// = "GenericIronPythonModelPanelKey";
        protected string strPanelCaption;// = "GenericIronPythonModelPanelCaption";
        private Globals.PluginType pluginType = VBCommon.Globals.PluginType.Modeling;
        private RootItem rootHeaderItem;

        //instance of  class
        protected IPyModeling.IPyModelingControl innerIronPythonControl;
        private VBCommon.Signaller signaller;

        //ribbon buttons
        private SimpleActionItem btnRun;
        private SimpleActionItem btnComputeAO;
        private SimpleActionItem btnManipulate;
        private SimpleActionItem btnTransform;
        private SimpleActionItem btnCancel;
        private SimpleActionItem btnDropVariables;
                
        //Raise a message
        public delegate void MessageHandler<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
        public event MessageHandler<VBCommon.PluginSupport.MessageArgs> MessageSent;

        //complete and visible flags
        public Boolean boolComplete = false;
        public Boolean boolVisible = false;
        public Boolean boolRunning = false;
        public Boolean boolVirgin = true;
        public Boolean boolChanged = false;

        private Stack<string> UndoKeys;
        private Stack<string> RedoKeys;

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
            if (boolVisible)
            {
                boolChanged = true;                

                App.HeaderControl.RemoveAll();
                App.DockManager.HidePanel(strPanelKey);
            }
            boolVisible = false;
        }


        public void Show()
        {
            if (!boolVisible)
            {
                boolChanged = true;                

                AddRibbon("Show");
                App.DockManager.SelectPanel(strPanelKey);
                App.HeaderControl.SelectRoot(strPanelKey);
            }
            boolVisible = true;
        }


        public void MakeActive()
        {            
            App.HeaderControl.SelectRoot(strPanelKey); 
            App.DockManager.SelectPanel(strPanelKey);
            //boolVisible = true;       
        }


        public override void Activate()
        {
            UndoKeys = new Stack<string>();
            RedoKeys = new Stack<string>();

            AddPanel();
            AddRibbon("Activate");
            
            //when panel is selected activate seriesview and ribbon tab
            App.DockManager.ActivePanelChanged += new EventHandler<DotSpatial.Controls.Docking.DockablePanelEventArgs>(DockManager_ActivePanelChanged);
            App.HeaderControl.RootItemSelected += new EventHandler<RootItemEventArgs>(HeaderControl_RootItemSelected);
            innerIronPythonControl.ModelUpdated += new EventHandler(HandleUpdate);
            innerIronPythonControl.ModelingTabControl.SelectedIndexChanged += new EventHandler(UpdateControlStatus);
            innerIronPythonControl.ModelingCompleteEvent += new EventHandler(ModelingComplete);
            innerIronPythonControl.ModelingCanceledEvent += new EventHandler(ModelingCanceled);

            boolVirgin = true;
            boolComplete = false;
            boolVisible = true;
            boolRunning = false;
            boolChanged = false;

            base.Activate();
            Hide();
        }


        //a root item (plugin) has been selected
        void HeaderControl_RootItemSelected(object sender, RootItemEventArgs e)
        {
            if (e.SelectedRootKey == strPanelKey)
            {
                App.DockManager.SelectPanel(strPanelKey);
            }
        }


        //add a plugin root item
        public void AddRibbon(string sender)
        {
            rootHeaderItem = new RootItem(strPanelKey, strPanelCaption);
            rootHeaderItem.SortOrder = (short)pluginType;
            App.HeaderControl.Add(rootHeaderItem);
            App.HeaderControl.SelectRoot(strPanelKey); 
            
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

            btnCancel= new SimpleActionItem(strPanelKey, "Cancel", btnCancel_Click);
            btnCancel.LargeImage = Properties.Resources.redX;
            btnCancel.GroupCaption = rGroupCaption;
            btnCancel.Enabled = false;
            App.HeaderControl.Add(btnCancel);

            string strVarsCaption = "Variable Selection";

            btnDropVariables = new SimpleActionItem(strPanelKey, "Drop Variable(s)", btnDropVariables_Click);
            btnDropVariables.LargeImage = Properties.Resources.Remove;
            btnDropVariables.GroupCaption = strVarsCaption;
            btnDropVariables.Enabled = false;
            App.HeaderControl.Add(btnDropVariables);
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


        //event handler when a plugin is selected from tabs
        void ModelingComplete(object sender, EventArgs e)
        {
            boolComplete = true;
            btnDropVariables.Enabled = true;
            btnCancel.Enabled = false;
            btnRun.Enabled = true;
        }


        //event handler when a plugin is selected from tabs
        void ModelingCanceled(object sender, EventArgs e)
        {
            btnCancel.Enabled = false;
            btnRun.Enabled = true;
            btnDropVariables.Enabled = false;
            Broadcast();
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
            signaller.UndoEvent += new VBCommon.Signaller.SerializationEventHandler<VBCommon.PluginSupport.UndoRedoEventArgs>(Undo);
            signaller.RedoEvent += new VBCommon.Signaller.SerializationEventHandler<VBCommon.PluginSupport.UndoRedoEventArgs>(Redo);
            signaller.UndoStackEvent += new VBCommon.Signaller.SerializationEventHandler<VBCommon.PluginSupport.UndoRedoEventArgs>(PushToStack);

            this.MessageSent += new MessageHandler<VBCommon.PluginSupport.MessageArgs>(signaller.HandleMessage);
        }


        //event listener for plugin broadcasting changes
        private void BroadcastStateListener(object sender, VBCommon.PluginSupport.BroadcastEventArgs e)
        {            
            //if datasheet updated itself, set data with changes
            if (((IPlugin)sender).PluginType == Globals.PluginType.Datasheet)
            {
                if (!(bool)((IPlugin)sender).Complete)
                {
                    Hide();
                    return;
                }
                else
                {
                    if (!(bool)e.PackedPluginState["Clean"])
                    {
                        boolComplete = false;
                        boolVirgin = false;
                        innerIronPythonControl.SetData(e.PackedPluginState);
                        boolChanged = true;
                    }
                    Show();
                }
            }                
        }
        

        public void UpdateControlStatus(object sender, EventArgs e)
        {
            try
            {
                string strActiveTabName = innerIronPythonControl.ModelingTabControl.SelectedTab.Name.ToString();

                if (strActiveTabName == "DatasheetTab")
                {
                    btnComputeAO.Enabled = true;
                    btnManipulate.Enabled = true;
                    btnTransform.Enabled = true;
                    btnRun.Enabled = false;
                    btnDropVariables.Enabled = false;
                }
                else if (strActiveTabName == "VariableSelectionTab")
                {
                    btnComputeAO.Enabled = false;
                    btnManipulate.Enabled = false;
                    btnTransform.Enabled = false;
                    btnRun.Enabled = false;
                    btnDropVariables.Enabled = false;
                }
                else if (strActiveTabName == "ModelingTab")
                {
                    btnComputeAO.Enabled = false;
                    btnManipulate.Enabled = false;
                    btnTransform.Enabled = false;
                    btnRun.Enabled = true;
                    btnDropVariables.Enabled = boolComplete;
                    innerIronPythonControl.SetModelData();
                }
                else if (strActiveTabName == "DiagnosticTab")
                {
                    btnComputeAO.Enabled = false;
                    btnManipulate.Enabled = false;
                    btnTransform.Enabled = false;
                    btnRun.Enabled = false;
                    btnDropVariables.Enabled = false;
                }
            }
            catch
            { }
        }


        //when modeling makes changes, event broadcasts changes to those listening
        public void Broadcast()
        {
            boolChanged = true;

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
            signaller.TriggerUndoStack();
            MakeActive();
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

                if (!e.PredictionOnly)
                {
                    if ((bool)dictPlugin["Visible"]) { Show(); }
                    else { Hide(); }
                    boolVisible = (bool)dictPlugin["Visible"];
                }
                else
                {
                    Hide();
                    boolVisible = false;
                }

                innerIronPythonControl.UnpackState(dictPlugin);
                boolComplete = (bool)dictPlugin["Complete"];
                boolVirgin = (bool)dictPlugin["Virgin"];
            }
            else
            {
                innerIronPythonControl.Clear();
            }
        }


        private void PushToStack(object sender, UndoRedoEventArgs args)
        {
            if (boolChanged)
            {
                //go pack state, add complete and visible, and add to dictionary of plugins
                IDictionary<string, object> packedState = innerIronPythonControl.PackState();

                if (packedState != null)
                {
                    packedState.Add("Complete", boolComplete);
                    packedState.Add("Visible", boolVisible);
                    packedState.Add("Virgin", boolVirgin);

                        string strKey = PersistentStackUtilities.RandomString(10);
                        args.Store.Add(strKey, packedState);
                        UndoKeys.Push(strKey);
                        RedoKeys.Clear();
                        boolChanged = false;
                }
            }
            else
            {
                try
                {
                    string strKey = UndoKeys.Peek();
                    UndoKeys.Push(strKey);
                    RedoKeys.Clear();
                }
                catch { }
            }            
        }


        private void Undo(object sender, UndoRedoEventArgs args)
        {
            try
            {
                string strCurrentKey = UndoKeys.Pop();
                string strPastKey = UndoKeys.Peek();
                RedoKeys.Push(strCurrentKey);

                if (strCurrentKey != strPastKey)
                {
                    IDictionary<string, object> dictPlugin = args.Store[strPastKey];

                    if ((bool)dictPlugin["Visible"]) { Show(); }
                    else { Hide(); }

                    boolComplete = (bool)dictPlugin["Complete"];
                    boolVisible = (bool)dictPlugin["Visible"];
                    boolVirgin = (bool)dictPlugin["Virgin"];
                    innerIronPythonControl.UnpackState(dictPlugin);
                }
            }
            catch
            {
                Activate();
            }
        }


        private void Redo(object sender, UndoRedoEventArgs args)
        {
            try
            {
                string strCurrentKey = UndoKeys.Peek();
                string strNextKey = RedoKeys.Pop();
                UndoKeys.Push(strNextKey);

                if (strCurrentKey != strNextKey)
                {
                    IDictionary<string, object> dictPlugin = args.Store[strNextKey];

                    if ((bool)dictPlugin["Visible"]) { Show(); }
                    else { Hide(); }

                    boolComplete = (bool)dictPlugin["Complete"];
                    boolVisible = (bool)dictPlugin["Visible"];
                    boolVirgin = (bool)dictPlugin["Virgin"];
                    innerIronPythonControl.UnpackState(dictPlugin);
                }
            }
            catch
            {
                Activate();
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
            boolComplete = false;

            if (innerIronPythonControl.lbIndVariables.Items.Count == 0)
            {
                MessageBox.Show("You must chose variables first and go to model tab before selecting Run", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            bool bSuccess = innerIronPythonControl.btnRun_Click(sender, e);

            if (boolRunning || !bSuccess)
                return;

            btnCancel.Enabled = true;
            btnRun.Enabled = false;
            btnDropVariables.Enabled = false;
            
            MakeActive();            
        }


        void btnCancel_Click(object sender, EventArgs e)
        {
            innerIronPythonControl.btnCancel_Click(sender, e);
            boolComplete = false;
        }


        void btnDropVariables_Click(object sender, EventArgs e)
        {
            innerIronPythonControl.btnRemoveInputVariablesFromModelingTab_Click(sender, e);
            boolComplete = false;
        }


        //change has been made within modeling, need to update
        private void HandleUpdate(object sender, EventArgs e)
        {
            Broadcast();
        }
    }
}