using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using DotSpatial.Controls;
using System.ComponentModel.Composition;
using DotSpatial.Controls.Header;
using DotSpatial.Controls.Docking;
using VBCommon;
using VBCommon.Interfaces;
using VBCommon.PluginSupport;
using VBCommon.Controls;
using VBProjectManager;

namespace VBDatasheet
{
    //datasheet plugin
    public class DatasheetPlugIn : Extension, IPartImportsSatisfiedNotification, IPlugin
    {
        //class instance
        private frmDatasheet _frmDatasheet;
        [Import("Shell")]
        private ContainerControl Shell { get; set; }
        private Globals.PluginType pluginType = VBCommon.Globals.PluginType.Datasheet;

        private VBCommon.Signaller signaller;
        protected string strPanelKey = "DataSheetPanel";
        protected string strPanelCaption = "Global Datasheet";
        //buttons
        private SimpleActionItem btnImport;
        private SimpleActionItem btnValidate;
        private SimpleActionItem btnComputeAO;
        private SimpleActionItem btnManipulate;
        private SimpleActionItem btnTransform;
        private SimpleActionItem btnGoToModeling;
        private RootItem rootDatasheetTab;

        private Boolean boolComplete = false;
        private Boolean boolVisible = true;
        private Boolean boolFirstPass = true;
        private Boolean boolClean = true;
        private Boolean boolChanged = false;

        private Stack<string> UndoKeys;
        private Stack<string> RedoKeys;
        private string strUndoRedoKey;

        //raise a message
        public delegate void MessageHandler<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
        public event MessageHandler<VBCommon.PluginSupport.MessageArgs> MessageSent;

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            App.DockManager.Remove(strPanelKey);
            _frmDatasheet = null;
            base.Deactivate();
        }


        //hide plugin
        public void Hide()
        {
            boolVisible = false;
            App.HeaderControl.RemoveAll();
            ((VBDockManager.VBDockManager)App.DockManager).HidePanel(strPanelKey);
        }


        //show plugin
        public void Show()
        {
            if (!boolVisible)
            {
                AddRibbon("Show");
                boolVisible = true;
            }
        }


        //make this plugin the active plugin
        public void MakeActive()
        {
            App.DockManager.SelectPanel(strPanelKey);
            App.HeaderControl.SelectRoot(strPanelKey);
        }


        //initial activation, adding the panel and the ribbon
        public override void Activate()
        {
            _frmDatasheet = new frmDatasheet();
            UndoKeys = new Stack<string>();
            RedoKeys = new Stack<string>();

            AddRibbon("Activate");
            AddPanel();

            //when panel is selected activate seriesview and ribbon tab
            App.DockManager.ActivePanelChanged += new EventHandler<DotSpatial.Controls.Docking.DockablePanelEventArgs>(DockManager_ActivePanelChanged);
            App.HeaderControl.RootItemSelected += new EventHandler<RootItemEventArgs>(HeaderControl_RootItemSelected);

            _frmDatasheet.NotifiableDataEvent += new EventHandler(NotifiableDataEventHandler);
            _frmDatasheet.ControlChangeEvent += new EventHandler(ControlChangeEventHandler);
            base.Activate();
        }


        //select panel if it is the root item selected
        void HeaderControl_RootItemSelected(object sender, RootItemEventArgs e)
        {
            if (e.SelectedRootKey == strPanelKey)
            {
                boolVisible = true;
                App.DockManager.SelectPanel(strPanelKey);
                _frmDatasheet.Refresh();
            }
        }


        //return panel key name
        public string PanelKey
        {
            get { return strPanelKey; }
        }


        //return plugin type (datasheet)
        public Globals.PluginType PluginType
        {
            get { return pluginType; }
        }
        

        //return plugin complete flag
        public Boolean Complete
        {
            get { return boolComplete; }
        }


        //return plugin visible flag
        public Boolean Visible
        {
            get { return boolVisible; }
        }


        //add a datasheet plugin root item
        public void AddRibbon(string sender)
        {
            rootDatasheetTab = new RootItem(strPanelKey, strPanelCaption);
            rootDatasheetTab.SortOrder = (short)pluginType;
            App.HeaderControl.Add(rootDatasheetTab);
            App.HeaderControl.SelectRoot(strPanelKey);

            //add sub-ribbons
            const string tGroupCaption = "Add";

            btnImport = new SimpleActionItem(strPanelKey, "Import Data", btnImport_Click);
            btnImport.LargeImage = Properties.Resources.EPAadd_data;
            btnImport.GroupCaption = tGroupCaption;
            btnImport.Enabled = true;
            App.HeaderControl.Add(btnImport);

            //section for validating
            const string grpValidate = "Validate";

            btnValidate = new SimpleActionItem(strPanelKey, "Validate Data", btnValidate_Click);
            btnValidate.LargeImage = Properties.Resources.EPAvalidate;
            btnValidate.GroupCaption = grpValidate;
            btnValidate.Enabled = false;
            App.HeaderControl.Add(btnValidate);

            //section for working with data
            const string grpManipulate = "Work with Data";

            btnComputeAO = new SimpleActionItem(strPanelKey, "Compute A O", _frmDatasheet.btnComputeAO_Click);
            btnComputeAO.LargeImage = Properties.Resources.EPAComputeAO;
            btnComputeAO.GroupCaption = grpManipulate;
            btnComputeAO.Enabled = false;
            App.HeaderControl.Add(btnComputeAO);

            btnManipulate = new SimpleActionItem(strPanelKey, "Manipulate", _frmDatasheet.btnManipulate_Click);
            btnManipulate.LargeImage = Properties.Resources.EPAmanipulate;
            btnManipulate.GroupCaption = grpManipulate;
            btnManipulate.Enabled = false;
            App.HeaderControl.Add(btnManipulate);

            btnTransform = new SimpleActionItem(strPanelKey, "Transform", _frmDatasheet.btnTransform_Click);
            btnTransform.LargeImage = Properties.Resources.EPAtransform;
            btnTransform.GroupCaption = grpManipulate;
            btnTransform.Enabled = false;
            App.HeaderControl.Add(btnTransform);

            btnGoToModeling = new SimpleActionItem(strPanelKey, "Go To Model", btnGoToModeling_Click);
            btnGoToModeling.LargeImage = Properties.Resources.GoToModeling;
            btnGoToModeling.GroupCaption = grpManipulate;
            btnGoToModeling.Enabled = false;
            App.HeaderControl.Add(btnGoToModeling);
        }


        //add a datasheet panel
        public void AddPanel()
        {
            var dp = new DockablePanel(strPanelKey, strPanelCaption, _frmDatasheet, DockStyle.Fill);
            dp.DefaultSortOrder = (short)pluginType;
            App.DockManager.Add(dp);
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
        

        //listen to Model's complete status
        private void BroadcastStateListener(object sender, VBCommon.PluginSupport.BroadcastEventArgs e)
        {
            if (((IPlugin)sender).PluginType == VBCommon.Globals.PluginType.Map & ((IPlugin)sender).Complete)
            {
                _frmDatasheet.SetLocation(e.PackedPluginState);
                boolChanged = true;
            }
        }


        public void NotifiableDataEventHandler(object sender, EventArgs e)
        {
            //We are handling a re-raised NotifiableDataEvent. The plugin's state is not considered complete b/c the user hasn't pressed the "Go To Modeling" button.
            boolComplete = false;
            boolClean = false;
                
            Broadcast();
        }


        public void Broadcast()
        {
            boolChanged = true;
            IDictionary<string, object> dictPackedState = _frmDatasheet.PackState();

            if (dictPackedState == null)
                return;
                        
            dictPackedState.Add("Clean", boolClean);
            dictPackedState.Add("Complete", boolComplete);
            dictPackedState.Add("Visible", boolVisible);
            dictPackedState.Add("FirstPass", boolFirstPass);

            signaller.RaiseBroadcastRequest(this, dictPackedState);
            signaller.TriggerUndoStack();
            MakeActive();
        }


        private void ProjectSavedListener(object sender, VBCommon.PluginSupport.SerializationEventArgs e)
        {
            IDictionary<string, object> dictPackedState = _frmDatasheet.PackState();

            dictPackedState.Add("Clean", boolClean);
            dictPackedState.Add("Complete", boolComplete);
            dictPackedState.Add("Visible", boolVisible);
            dictPackedState.Add("FirstPass", boolFirstPass);

            e.PackedPluginStates.Add(strPanelKey, dictPackedState);
        }


        private void ProjectOpenedListener(object sender, VBCommon.PluginSupport.SerializationEventArgs e)
        {
            if (e.PackedPluginStates.ContainsKey(strPanelKey))
            {
                IDictionary<string, object> dictPlugin = e.PackedPluginStates[strPanelKey];

                if (!e.PredictionOnly)
                {
                    if ((bool)dictPlugin["Visible"])
                    {
                        this.Show();

                        if ((bool)dictPlugin["Complete"])
                        {
                            btnComputeAO.Enabled = true;
                            btnGoToModeling.Enabled = true;
                            btnManipulate.Enabled = true;
                            btnTransform.Enabled = true;
                        }
                    }
                    else { Hide(); }
                }
                else { Hide(); }

                _frmDatasheet.UnpackState(dictPlugin);
                boolVisible = (bool)dictPlugin["Visible"];
                boolComplete = (bool)dictPlugin["Complete"];
                boolClean = (bool)dictPlugin["Clean"];
                boolFirstPass = (bool)dictPlugin["FirstPass"];
            }
            else
            {
                Activate();
            }
        }


        private void PushToStack(object sender, UndoRedoEventArgs args)
        {
            if (boolChanged)
            {
                IDictionary<string, object> dictPackedState = _frmDatasheet.PackState();

                dictPackedState.Add("Clean", boolClean);
                dictPackedState.Add("Complete", boolComplete);
                dictPackedState.Add("Visible", boolVisible);
                dictPackedState.Add("FirstPass", boolFirstPass);

                string strKey = PersistentStackUtilities.RandomString(10);
                args.Store.Add(strKey, dictPackedState);
                UndoKeys.Push(strKey);
                RedoKeys.Clear();
                boolChanged = false;
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

                    boolVisible = (bool)dictPlugin["Visible"];
                    boolComplete = (bool)dictPlugin["Complete"];
                    boolClean = (bool)dictPlugin["Clean"];
                    boolFirstPass = (bool)dictPlugin["FirstPass"];

                    if (boolVisible)
                    {
                        this.Show();

                        if (boolComplete)
                        {
                            btnComputeAO.Enabled = true;
                            btnGoToModeling.Enabled = true;
                            btnManipulate.Enabled = true;
                            btnTransform.Enabled = true;
                        }
                    }
                    _frmDatasheet.UnpackState(dictPlugin);
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

                    boolVisible = (bool)dictPlugin["Visible"];
                    boolComplete = (bool)dictPlugin["Complete"];
                    boolClean = (bool)dictPlugin["Clean"];
                    boolFirstPass = (bool)dictPlugin["FirstPass"];

                    if (boolVisible)
                    {
                        this.Show();

                        if (boolComplete)
                        {
                            btnComputeAO.Enabled = true;
                            btnGoToModeling.Enabled = true;
                            btnManipulate.Enabled = true;
                            btnTransform.Enabled = true;
                        }
                    }
                    _frmDatasheet.UnpackState(dictPlugin);
                }
            }
            catch { }
        }


        private void SendMessage(string message)
        {
            if (MessageSent != null)
            {
                VBCommon.PluginSupport.MessageArgs e = new VBCommon.PluginSupport.MessageArgs(message);
                MessageSent(this, e);
            }
        }


        void DockManager_ActivePanelChanged(object sender, DotSpatial.Controls.Docking.DockablePanelEventArgs e)
        {
            if (e.ActivePanelKey == strPanelKey)
            {
                App.DockManager.SelectPanel(strPanelKey);
                App.HeaderControl.SelectRoot(strPanelKey);
            } 
        }


        void btnImport_Click(object sender, EventArgs e)
        {
            if (!boolFirstPass && !boolClean)
            {
                //Ask whether the user wants to clobber the modeling and prediction tabs by modifying the data.
                DialogResult dlgr = MessageBox.Show("Importing new data will clear any existing data and models. Proceed?", "Proceed with import.", MessageBoxButtons.OKCancel);
                if (dlgr == DialogResult.Cancel)
                {
                    return;
                }
            }

            bool bSuccess = _frmDatasheet.btnImportData_Click(sender, e);
            if (bSuccess)
            {
                boolClean = false;
                btnValidate.Enabled = true;
                btnComputeAO.Enabled = false;
                btnManipulate.Enabled = false;
                btnTransform.Enabled = false;
                btnGoToModeling.Enabled = false;
            }
        }


        void btnValidate_Click(object sender, EventArgs e)
        {
            bool bSuccess = _frmDatasheet.btnValidateData_Click(sender, e);
            if (bSuccess)
            {
                btnComputeAO.Enabled = true;
                btnManipulate.Enabled = true;
                btnTransform.Enabled = true;
                btnGoToModeling.Enabled = true;
            }
            else
            {
                btnComputeAO.Enabled = false;
                btnManipulate.Enabled = false;
                btnTransform.Enabled = false;
                btnGoToModeling.Enabled = false;
            }
        }


        void btnGoToModeling_Click(object sender, EventArgs e)
        {
            if (!boolFirstPass && !boolClean)
            {
                //Ask whether the user wants to clobber the modeling and prediction tabs by modifying the data.
                DialogResult dlgr = MessageBox.Show("Changes in data and/or data attributes have occurred.\nPrevious modeling results will be erased. Proceed?", "Proceed to Modeling.", MessageBoxButtons.OKCancel);
                if (dlgr == DialogResult.Cancel)
                {
                    return;
                }
            }

            //Datasheet is complete when go to modeling is clicked
            boolComplete = true;            
            Broadcast();
            
            //once you leave here, changes made to ds clear for next time here
            boolFirstPass = false;
            boolClean = true;
        }

        private void ControlChangeEventHandler(object sender, EventArgs e)
        {
            this.boolChanged = true;
        }
    }
}