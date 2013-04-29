using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel.Composition;
using System.Data;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Extensions;
using DotSpatial.Controls.Header;
using VBCommon;
using VBCommon.Interfaces;
using VBCommon.PluginSupport;
using VBDockManager;
using GALibForm;


namespace MLRPlugin
{
    public class VBMLRPlugin : Extension, IPartImportsSatisfiedNotification, IPlugin
    {
        [Import("Shell")]
        private ContainerControl Shell { get; set; }

        private VBCommon.Signaller signaller;

        //private ctlMLRPlugin cMlr;
        private ctlMLRModel cMlr;
        private MLRPredPlugin _mlrPredPlugin;

        private frmModel mlrModelForm = null;
        private const string strPanelKey = "kVBMultLinearReg";
        private const string strPanelCaption = "MLR";
        private Globals.PluginType pluginType = VBCommon.Globals.PluginType.Modeling;
        private RootItem mlrTab;
        private SimpleActionItem btnNull;
        private RootItem rootHeaderItem;

        IDictionary<string, object> dictPlugin = null;

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
        public Boolean boolComplete = false;
        private Boolean boolVirgin = true;
        public Boolean boolVisible;
        public Boolean boolRunCancelled;
        public Boolean boolStopRun;
        public Boolean boolInitialEntry = true; //first time here flag
        private Boolean boolChanged = false;

        private Boolean boolClearModel; //needed for IPlugin
        
        private Stack<string> UndoKeys = new Stack<string>();
        private Stack<string> RedoKeys = new Stack<string>();

            //UndoKeys = new Stack<string>();
            //RedoKeys = new Stack<string>();

        public override void Activate()
        {
            UndoKeys = new Stack<string>();
            RedoKeys = new Stack<string>();
            
            //cMlr = new ctlMLRPlugin();
            cMlr = new ctlMLRModel();
            _mlrPredPlugin = new MLRPredPlugin();
            
            mlrModelForm = cMlr.ModelForm;
            mlrModelForm.ModelChanged += new frmModel.ModelChangedEventHandler(ModelChanged);
            cMlr.ModelingTabControl.SelectedIndexChanged += new EventHandler(UpdateControlStatus);
            
            AddPanel();
            AddRibbon("Activate");

            //when panel is selected activate seriesview and ribbon tab
            App.DockManager.ActivePanelChanged += new EventHandler<DotSpatial.Controls.Docking.DockablePanelEventArgs>(DockManager_ActivePanelChanged);
            App.HeaderControl.RootItemSelected += new EventHandler<RootItemEventArgs>(HeaderControl_RootItemSelected);
            
            boolChanged = false;
            boolVisible = true;
            base.Activate();
            Hide();
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            App.DockManager.Remove(strPanelKey);
            cMlr = null;
            base.Deactivate();
        }

        //event handler when a plugin is selected from tabs
        void DockManager_ActivePanelChanged(object sender, DotSpatial.Controls.Docking.DockablePanelEventArgs e)
        {
            if (e.ActivePanelKey == strPanelKey)
            {
                App.DockManager.SelectPanel(strPanelKey);
                App.HeaderControl.SelectRoot(strPanelKey);
            }
            //if (e.ActivePanelKey.ToString() == "DataSheetPanel" && boolVisible)
            //    Hide();
        }

        //a root item (plugin) has been selected
        void HeaderControl_RootItemSelected(object sender, RootItemEventArgs e)
        {
            if (e.SelectedRootKey == strPanelKey)
            {
                App.DockManager.SelectPanel(strPanelKey);
            }
        }

        //add the panel content within the plugin
        public void AddPanel()
        {
            var dp = new DockablePanel(strPanelKey, strPanelCaption, cMlr, System.Windows.Forms.DockStyle.Fill);
            dp.DefaultSortOrder = (short)pluginType;
            App.DockManager.Add(dp);
        }

        //hide this plugin
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


        //show this plugin
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


        //make this plugin the active one
        public void MakeActive()
        {
            App.DockManager.SelectPanel(strPanelKey);
            App.HeaderControl.SelectRoot(strPanelKey);
        }

        //add a datasheet plugin root item
        public void AddRibbon(string sender)
        {
            rootHeaderItem = new RootItem(strPanelKey, strPanelCaption);
            rootHeaderItem.SortOrder = (short)pluginType;
            App.HeaderControl.Add(rootHeaderItem);
            App.HeaderControl.SelectRoot(strPanelKey);

            //add sub-ribbon
            string grpManipulate = "Manipulate Data";

            btnComputeAO = new SimpleActionItem(strPanelKey, "Compute A O", cMlr.btnComputeAO_Click);
            btnComputeAO.LargeImage = Properties.Resources.EPAComputeAO;
            btnComputeAO.GroupCaption = grpManipulate;
            btnComputeAO.Enabled = true;
            App.HeaderControl.Add(btnComputeAO);

            btnManipulate = new SimpleActionItem(strPanelKey, "Manipulate", cMlr.btnManipulate_Click);
            btnManipulate.LargeImage = Properties.Resources.EPAmanipulate;
            btnManipulate.GroupCaption = grpManipulate;
            btnManipulate.Enabled = true;
            App.HeaderControl.Add(btnManipulate);

            btnTransform = new SimpleActionItem(strPanelKey, "Transform", cMlr.btnTransform_Click);
            btnTransform.LargeImage = Properties.Resources.EPAtransform;
            btnTransform.GroupCaption = grpManipulate;
            btnTransform.Enabled = true;
            App.HeaderControl.Add(btnTransform);

            //add sub-ribbon
            string rGroupCaption = "Model";

            btnRun = new SimpleActionItem(strPanelKey, "Run", btnRun_Click);
            btnRun.LargeImage = Properties.Resources.Run;
            btnRun.GroupCaption = rGroupCaption;
            btnRun.Enabled = false;
            App.HeaderControl.Add(btnRun);

            btnCancel = new SimpleActionItem(strPanelKey, "Cancel", btnCancel_Click);
            btnCancel.LargeImage = Properties.Resources.Cancel;
            btnCancel.GroupCaption = rGroupCaption;
            btnCancel.Enabled = false;
            App.HeaderControl.Add(btnCancel);
        }

        void btnRun_Click(object sender, EventArgs e)
        {

        }

        void btnCancel_Click(object sender, EventArgs e)
        {

        }

        //returns this model's packed state
        public IDictionary<string, object> PackedPlugin
        {
            get { return dictPlugin; }
            set { dictPlugin = value; }
        }

        public Globals.PluginType PluginType
        {
            get { return pluginType; }
        }

        //keep track if model already has a datasheet
        public Boolean InitialEntry
        {
            get { return boolInitialEntry; }
        }


        public string PanelKey
        {
            get { return strPanelKey; }
        }


        public void Broadcast()
        {
            boolChanged = true;
            boolVirgin = false;
            //get packed state, add complete and visible and raise broadcast event
            IDictionary<string, object> dictPackedState = cMlr.PackProjectState();
            dictPackedState["Origin"] = strPanelKey;

            if (dictPackedState.ContainsKey("Model"))
            {
                if (dictPackedState["Model"] != null)
                    boolComplete = true;
            }

            dictPackedState.Add("Complete", cMlr.Complete);
            dictPackedState.Add("Visible", boolVisible);

            signaller.RaiseBroadcastRequest(this, dictPackedState);
            signaller.TriggerUndoStack();
            MakeActive();
        }

        public bool Complete
        {
            get { return boolComplete;; }
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
            if (sender == null || e == null)
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
                else
                {
                    if (!(bool)e.PackedPluginState["Clean"])
                    {
                        boolInitialEntry = false;
                        boolComplete = false;
                        boolVirgin = true;
                        cMlr.UnpackProjectState(e.PackedPluginState);
                        boolChanged = true;
                    }
                    Show();
                }                
            }
        }

        //event handler for saving project state
        private void ProjectSavedListener(object sender, VBCommon.PluginSupport.SerializationEventArgs e)
        {
            //go pack state, add complete and visible, and add to dictionary of plugins
            IDictionary<string, object> packedState = new Dictionary<string, object>();
            
            if (!boolVirgin)
                packedState = cMlr.PackProjectState();

            packedState.Add("Complete", boolComplete);
            packedState.Add("Visible", boolVisible);
            packedState.Add("Virgin", boolVirgin);

            e.PackedPluginStates.Add(strPanelKey, packedState);
        }


        //event handler for opening project state
        private void ProjectOpenedListener(object sender, VBCommon.PluginSupport.SerializationEventArgs e)
        {
            //if modeling is in the list of packed plugins, go unpack 
            if (e.PackedPluginStates.ContainsKey(strPanelKey))
            {
                dictPlugin = e.PackedPluginStates[strPanelKey];

                //Set the virigin flag
                boolVirgin = true;
                if (dictPlugin.ContainsKey("Virgin"))
                    if (!(bool)(dictPlugin["Virgin"]))
                        boolVirgin = false;

                //repopulate plugin Complete flags from saved project
                boolComplete = (bool)dictPlugin["Complete"];
                boolInitialEntry = false; //opening projects have been entered before

                //check to see if there already is a model open, if so, close it before opening a saved project
                if ((Visible) && (Complete))
                    Hide();

                //make model being open active plugin
                if ((bool)dictPlugin["Visible"] && !e.PredictionOnly)
                    Show();

                if (!boolVirgin)
                {
                    //Disable the notifiabledataevent during the unpacking or it'll overwrite the saved variable selection.
                    cMlr.AllowNotifiableDataEvent = false;
                    cMlr.UnpackProjectState(e.PackedPluginStates[strPanelKey]);
                    cMlr.AllowNotifiableDataEvent = true;
                }
            }
            else
            {
                //Set this plugin to an empty state.
                Activate();
            }
        }
        
        
        private void PushToStack(object sender, UndoRedoEventArgs args)
        {
            if (boolChanged)
            {
                //go pack state, add complete and visible, and add to dictionary of plugins
                IDictionary<string, object> packedState = cMlr.PackProjectState();

                if (packedState != null)
                {
                    packedState.Add("Complete", boolComplete);
                    packedState.Add("Visible", boolVisible);
                    packedState.Add("InitialEntry", boolInitialEntry);

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

                    boolVisible = (bool)dictPlugin["Visible"];
                    boolComplete = (bool)dictPlugin["Complete"];
                    boolInitialEntry = (bool)dictPlugin["InitialEntry"];
                    cMlr.UnpackProjectState(dictPlugin);
                }
            }
            catch { }
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

                    boolVisible = (bool)dictPlugin["Visible"];
                    boolComplete = (bool)dictPlugin["Complete"];
                    boolInitialEntry = (bool)dictPlugin["InitialEntry"];
                    cMlr.UnpackProjectState(dictPlugin);
                }
            }
            catch { }
        }


        public void ModelChanged()
        {
            Broadcast();
        }


        public void UpdateControlStatus(object sender, EventArgs e)
        {
            try
            {
                string strActiveTabName = cMlr.ModelingTabControl.SelectedTab.Name.ToString();

                if (strActiveTabName == "tpData")
                {
                    btnComputeAO.Enabled = true;
                    btnManipulate.Enabled = true;
                    btnTransform.Enabled = true;
                    btnRun.Enabled = false;
                }
                else if (strActiveTabName == "tpVarSelection")
                {
                    btnComputeAO.Enabled = false;
                    btnManipulate.Enabled = false;
                    btnTransform.Enabled = false;
                    btnRun.Enabled = false;
                }
                else if (strActiveTabName == "tpModel")
                {
                    btnComputeAO.Enabled = false;
                    btnManipulate.Enabled = false;
                    btnTransform.Enabled = false;
                    btnRun.Enabled = true;
                    //cMlr.SetModelData();
                }
            }
            catch
            { }
        }
    }
}
