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
        //complete and visible flags
        private Boolean boolComplete;
        private Boolean boolVisible = true;
        //keep track if first time through this plugin or coming back from model
        private Boolean boolInitialEntry = true;

        //clear model flag
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


        //raise a message
        public delegate void MessageHandler<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
        public event MessageHandler<VBCommon.PluginSupport.MessageArgs> MessageSent;

        //deactivate this plugin
        public override void Deactivate()
        {
            //remove ribbon tab
            App.HeaderControl.RemoveAll();
            //remove plugin panel
            App.DockManager.Remove(strPanelKey);

            _frmDatasheet = null;

            //this line to ensure "enabled is set to false
            base.Deactivate();
        }


        //hide plugin
        public void Hide()
        {
            //set visible to false
            boolVisible = false;
            //remove plugin tab
            App.HeaderControl.RemoveAll();
            //hide bottom tab
            ((VBDockManager.VBDockManager)App.DockManager).HidePanel(strPanelKey);
            
        }


        //show plugin
        public void Show()
        {
            boolVisible = true;
            AddRibbon("Show");
        }


        //make this plugin the active plugin
        public void MakeActive()
        {
            //set visible flag to true
            boolVisible = true;

            App.DockManager.SelectPanel(strPanelKey);
            App.HeaderControl.SelectRoot(strPanelKey);
        }


        //initial activation, adding the panel and the ribbon
        public override void Activate()
        {
            _frmDatasheet = new frmDatasheet();
            AddRibbon("Activate");
            AddPanel();

            //when panel is selected activate seriesview and ribbon tab
            App.DockManager.ActivePanelChanged += new EventHandler<DotSpatial.Controls.Docking.DockablePanelEventArgs>(DockManager_ActivePanelChanged);

            //when root item is selected
            App.HeaderControl.RootItemSelected += new EventHandler<RootItemEventArgs>(HeaderControl_RootItemSelected);
            _frmDatasheet.ChangeMade4Stack += new EventHandler(HandleAddToStack);
            base.Activate(); //ensures "enabled" is set to true
        }


        //select panel if it is the root item selected
        void HeaderControl_RootItemSelected(object sender, RootItemEventArgs e)
        {
            if (e.SelectedRootKey == strPanelKey)
            {
                boolVisible = true;
                //signaller.HidePlugins();
                App.DockManager.SelectPanel(strPanelKey);
                _frmDatasheet.Refresh();
                //make this the top plugin for ProjMngr to use when opening
                TopPlugin = strPanelKey;
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


        //return clear model flag
        public Boolean ClearModel
        {
            get { return boolClearModel; }
        }


        //return plugin intial entry flag
        public Boolean InitialEntry
        {
            get { return boolInitialEntry; }
        }


        //return plugin complete flag
        public Boolean Complete
        {
            get { return boolComplete; }
        }


        //return plugin visible flag
        public Boolean VisiblePlugin
        {
            get { return boolVisible; }
        }


        //add a datasheet plugin root item
        public void AddRibbon(string sender)
        {
            rootDatasheetTab = new RootItem(strPanelKey, strPanelCaption);
            rootDatasheetTab.SortOrder = (short)pluginType;
            App.HeaderControl.Add(rootDatasheetTab);

            //tell ProjMngr if this is being Shown
            if (sender == "Show")
            {
                //make this the selected root
                App.HeaderControl.SelectRoot(strPanelKey);
            }

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
  //          signaller.BroadcastState += new VBCommon.Signaller.BroadCastEventHandler<VBCommon.PluginSupport.BroadCastEventArgs>(BroadcastStateListener);
            signaller.ProjectSaved += new VBCommon.Signaller.SerializationEventHandler<VBCommon.PluginSupport.SerializationEventArgs>(ProjectSavedListener);
            signaller.ProjectOpened += new VBCommon.Signaller.SerializationEventHandler<VBCommon.PluginSupport.SerializationEventArgs>(ProjectOpenedListener);
            this.MessageSent += new MessageHandler<VBCommon.PluginSupport.MessageArgs>(signaller.HandleMessage);
        }


        ////listen to other plugin's broadcasting their changes
        //private void BroadcastStateListener(object sender, VBCommon.PluginSupport.BroadCastEventArgs e)
        //{

        //}


        //undo was hit, send the packed state to be unpacked
        public void UndoLastChange(Dictionary<string, object> packedState)
        {
            _frmDatasheet.UnpackState(packedState);
        }


        //handles broadcasting each change to be added to the stack
        public void HandleAddToStack(object sender, EventArgs e)
        {
            Broadcast();
        }



        //broadcast changes to other plugins listening
        public void Broadcast()
        {
            //pack the datasheet's state to pass on for modeling to use
            IDictionary<string, object> packedState = new Dictionary<string, object>();
            packedState = _frmDatasheet.PackState();
            //if null, response var wasn't transformed
            if (packedState == null)
                return;
            boolClearModel = (bool)packedState["ChangesMadeDS"]; //needed for projectManager BroadcastListener
            packedState.Add("Complete", boolComplete);
            packedState.Add("Visible", boolVisible);
            signaller.RaiseBroadcastRequest(this, packedState);
        }


        //event listener for saving packed state of datasheet
        private void ProjectSavedListener(object sender, VBCommon.PluginSupport.SerializationEventArgs e)
        {
            //call packState, returing packed state of plugin
            IDictionary<string, object> packedState = _frmDatasheet.PackState();
            if ((bool)packedState["DSValidated"])
                boolComplete = true;
            //add complete and visible flags to dictionary
            packedState.Add("Complete", boolComplete);
            packedState.Add("Visible", boolVisible);
            //add packed state to dictionary of all packed states for saving
            e.PackedPluginStates.Add(strPanelKey, packedState);
        }


        //Unpack the state of this plugin.
        private void ProjectOpenedListener(object sender, VBCommon.PluginSupport.SerializationEventArgs e)
        {
            if (e.PackedPluginStates.ContainsKey(strPanelKey))
            {
                IDictionary<string, object> dictPlugin = e.PackedPluginStates[strPanelKey];
                //Repopulate plugin Complete flag with saved project
                boolComplete = (bool)dictPlugin["Complete"];

                //check to see if there already is a datasheet open, if so, close it before opening a saved project
                if (VisiblePlugin)
                    Hide();
                //then show the opening project datasheet
                if ((bool)dictPlugin["Visible"])
                {
                    Show();

                    //enable the buttons for opening project
                    if (boolComplete)
                    {
                        btnComputeAO.Enabled = true;
                        btnGoToModeling.Enabled = true;
                        btnManipulate.Enabled = true;
                        btnTransform.Enabled = true;
                    }
                }

                //unpack it
                _frmDatasheet.UnpackState(e.PackedPluginStates[strPanelKey]);
            }
            else
            {
                //Set this plugin to an empty state.
                Activate();
            }
        }


        private void SendMessage(string message)
        {
            if (MessageSent != null) //Has some method been told to handle this event?
            {
                VBCommon.PluginSupport.MessageArgs e = new VBCommon.PluginSupport.MessageArgs(message);
                MessageSent(this, e);
            }
        }


        //event triggered when another panel is selected from tabs
        void DockManager_ActivePanelChanged(object sender, DotSpatial.Controls.Docking.DockablePanelEventArgs e)
        {
            if (e.ActivePanelKey == strPanelKey)
            {
                App.DockManager.SelectPanel(strPanelKey);
                App.HeaderControl.SelectRoot(strPanelKey);
            }
            //if ((e.ActivePanelKey == "PLSPanel" || e.ActivePanelKey == "GBMPanel") && boolComplete)
            //{
            //    boolInitialEntry = false;
            //    _frmDatasheet.btnGoToModel_Click(); //set frm's initialPass to false too.
            //}
        }


        //importing datasheet
        void btnImport_Click(object sender, EventArgs e)
        {
            _frmDatasheet.btnImportData_Click(sender, e);
            btnValidate.Enabled = true;
        }


        //validate datasheet
        void btnValidate_Click(object sender, EventArgs e)
        {
            _frmDatasheet.btnValidateData_Click(sender, e);
            btnComputeAO.Enabled = true;
            btnManipulate.Enabled = true;
            btnTransform.Enabled = true;
            btnGoToModeling.Enabled = true;
        }


        //ready to go to modeling now
        void btnGoToModeling_Click(object sender, EventArgs e)
        {
            //_frmDatasheet.btnGoToModel_Click(sender, e); //go change the initial pass flag

            //only show this dialog if model is complete...check initial entry flag
            if (!boolInitialEntry)
            {
                DialogResult dlgr = MessageBox.Show("Changes in data and/or data attributes have occurred.\nPrevious modeling results will be erased. Proceed?", "Proceed to Modeling.", MessageBoxButtons.OKCancel);
                if (dlgr == DialogResult.OK)
                {
                    boolClearModel = true;
                }
            }

            //datasheet is complete when go to modeling is clicked
            boolComplete = true;
            //broadcast changes
            Broadcast();

        }  
    }
}