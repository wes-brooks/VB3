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

        private Boolean boolComplete;
        private Boolean boolVisible = true;
<<<<<<< HEAD
        private Boolean boolInitialPass = true;
=======
        //keep track if first time through this plugin or coming back from model
        private Boolean boolInitialEntry = true;
        //is model complete
        private Boolean boolModelComplete;
        private Boolean boolChangesMadeDS;
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

>>>>>>> a6b2f91fbf737b2a211f552c2c51c015a079edcb

        //raise a message
        public delegate void MessageHandler<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
        public event MessageHandler<VBCommon.PluginSupport.MessageArgs> MessageSent;

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            App.DockManager.Remove(strPanelKey);
            _frmDatasheet = null;
<<<<<<< HEAD

=======
>>>>>>> a6b2f91fbf737b2a211f552c2c51c015a079edcb
            base.Deactivate();
        }


        //hide plugin
        public void Hide()
<<<<<<< HEAD
        {            
            App.HeaderControl.RemoveAll();
            ((VBDockManager.VBDockManager)App.DockManager).HidePanel(strPanelKey);
            boolVisible = false;
=======
        {
            boolVisible = false;
            App.HeaderControl.RemoveAll();
            ((VBDockManager.VBDockManager)App.DockManager).HidePanel(strPanelKey);
>>>>>>> a6b2f91fbf737b2a211f552c2c51c015a079edcb
        }


        //show plugin
        public void Show()
        {            
            AddRibbon("Show");
            boolVisible = true;
        }


        //make this plugin the active plugin
        public void MakeActive()
        {
<<<<<<< HEAD
=======
            boolVisible = true;
>>>>>>> a6b2f91fbf737b2a211f552c2c51c015a079edcb
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
<<<<<<< HEAD
            _frmDatasheet.NotifiableDataEvent += new EventHandler(NotifiableDataEventHandler);
            base.Activate(); //ensures "enabled" is set to true
=======
            _frmDatasheet.ChangeMade4Stack += new EventHandler(HandleAddToStack);
            base.Activate();
>>>>>>> a6b2f91fbf737b2a211f552c2c51c015a079edcb
        }


        //select panel if it is the root item selected
        void HeaderControl_RootItemSelected(object sender, RootItemEventArgs e)
        {
            if (e.SelectedRootKey == strPanelKey)
            {
                boolVisible = true;
                App.DockManager.SelectPanel(strPanelKey);
                _frmDatasheet.Refresh();
<<<<<<< HEAD
=======
                TopPlugin = strPanelKey;
>>>>>>> a6b2f91fbf737b2a211f552c2c51c015a079edcb
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


<<<<<<< HEAD
        /*//return clear model flag
        public Boolean ClearModel
        {
            get { return boolClearModel; }
        }*/


        /*//return plugin intial entry flag
        public Boolean InitialEntry
        {
            get { return boolInitialEntry; }
        }*/
=======
        //return plugin intial entry flag
        public Boolean InitialEntry
        {
            get { return boolInitialEntry; }
        }


        //return whether model is complete
        public Boolean ModelComplete
        {
            get { return boolModelComplete; }
        }
>>>>>>> a6b2f91fbf737b2a211f552c2c51c015a079edcb


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

            //tell ProjMngr if this is being Shown
            if (sender == "Show")
            {
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
            signaller.BroadcastState += new VBCommon.Signaller.BroadCastEventHandler<VBCommon.PluginSupport.BroadCastEventArgs>(BroadcastStateListener);
            signaller.ProjectSaved += new VBCommon.Signaller.SerializationEventHandler<VBCommon.PluginSupport.SerializationEventArgs>(ProjectSavedListener);
            signaller.ProjectOpened += new VBCommon.Signaller.SerializationEventHandler<VBCommon.PluginSupport.SerializationEventArgs>(ProjectOpenedListener);
            this.MessageSent += new MessageHandler<VBCommon.PluginSupport.MessageArgs>(signaller.HandleMessage);
        }


<<<<<<< HEAD
=======
        
        //listen to other plugin's broadcasting their changes
        private void BroadcastStateListener(object sender, VBCommon.PluginSupport.BroadCastEventArgs e)
        {
            if (((IPlugin)sender).PluginType == Globals.PluginType.Modeling)
                if ((bool)e.PackedPluginState["Complete"])
                    boolModelComplete = true;
        }
        


        //undo was hit, send the packed state to be unpacked
>>>>>>> a6b2f91fbf737b2a211f552c2c51c015a079edcb
        public void UndoLastChange(Dictionary<string, object> packedState)
        {
            _frmDatasheet.UnpackState(packedState);
        }


        public void NotifiableDataEventHandler(object sender, EventArgs e)
        {
<<<<<<< HEAD
            //We are handling a re-raised NotifiableDataEvent. The plugin's state is not considered complete b/c the user hasn't pressed the "Go To Modeling" button.
            boolComplete = false;
=======
            if (boolModelComplete)
                boolChangesMadeDS = true;
>>>>>>> a6b2f91fbf737b2a211f552c2c51c015a079edcb
            Broadcast();
        }


<<<<<<< HEAD
=======
        //broadcast changes to other plugins listening
>>>>>>> a6b2f91fbf737b2a211f552c2c51c015a079edcb
        public void Broadcast()
        {
            IDictionary<string, object> packedState = new Dictionary<string, object>();
            packedState = _frmDatasheet.PackState();

            if (packedState == null)
                return;
<<<<<<< HEAD
            
            //boolClearModel = (bool)packedState["ChangesMadeDS"]; //needed for projectManager BroadcastListener
=======
            packedState.Add("ChangesMadeDS", boolChangesMadeDS);
>>>>>>> a6b2f91fbf737b2a211f552c2c51c015a079edcb
            packedState.Add("Complete", boolComplete);
            packedState.Add("Visible", boolVisible);
            signaller.RaiseBroadcastRequest(this, packedState);
        }


        private void BroadcastStateListener(object sender, VBCommon.PluginSupport.BroadCastEventArgs e)
        {
            if (((IPlugin)sender).PluginType == Globals.PluginType.ProjectManager)
            {
                //Test whether this plugin is meant to restore the packed state.
                //if (e.PackedPluginState.Keys == this.strPanelKey)
                //{

            }
        }


        private void ProjectSavedListener(object sender, VBCommon.PluginSupport.SerializationEventArgs e)
        {
<<<<<<< HEAD
            IDictionary<string, object> dictPackedState = _frmDatasheet.PackState();
            //if ((bool)packedState["DSValidated"])
            //    boolComplete = true;

            dictPackedState.Add("Complete", boolComplete);
            dictPackedState.Add("Visible", boolVisible);
            dictPackedState.Add("InitialPass", boolInitialPass);

            e.PackedPluginStates.Add(strPanelKey, dictPackedState);
=======
            IDictionary<string, object> packedState = _frmDatasheet.PackState();
            if ((bool)packedState["DSValidated"])
                boolComplete = true;
            packedState.Add("Complete", boolComplete);
            packedState.Add("Visible", boolVisible);
            e.PackedPluginStates.Add(strPanelKey, packedState);
>>>>>>> a6b2f91fbf737b2a211f552c2c51c015a079edcb
        }


        private void ProjectOpenedListener(object sender, VBCommon.PluginSupport.SerializationEventArgs e)
        {
            if (e.PackedPluginStates.ContainsKey(strPanelKey))
            {
                IDictionary<string, object> dictPlugin = e.PackedPluginStates[strPanelKey];
<<<<<<< HEAD

                boolComplete = (bool)dictPlugin["Complete"];
                boolInitialPass = (bool)dictPlugin["InitialPass"];

                //check to see if there already is a datasheet open, if so, close it before opening a saved project
                if (this.Visible)
                    this.Hide();

                //then show the opening project datasheet
                if ((bool)dictPlugin["Visible"])
=======

                //check to see if there already is a datasheet open, if so, close it before opening a saved project
                if (VisiblePlugin)
                    Hide();

                boolVisible = (bool)dictPlugin["Visible"];
                boolComplete = (bool)dictPlugin["Complete"];

                
                if (boolVisible)
>>>>>>> a6b2f91fbf737b2a211f552c2c51c015a079edcb
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
<<<<<<< HEAD
                _frmDatasheet.UnpackState(dictPlugin);
=======
                _frmDatasheet.UnpackState(e.PackedPluginStates[strPanelKey]);
>>>>>>> a6b2f91fbf737b2a211f552c2c51c015a079edcb
            }
            else
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


        void btnImport_Click(object sender, EventArgs e)
        {
            _frmDatasheet.btnImportData_Click(sender, e);
            btnValidate.Enabled = true;
        }


        void btnValidate_Click(object sender, EventArgs e)
        {
            _frmDatasheet.btnValidateData_Click(sender, e);
            btnComputeAO.Enabled = true;
            btnManipulate.Enabled = true;
            btnTransform.Enabled = true;
            btnGoToModeling.Enabled = true;
        }


        void btnGoToModeling_Click(object sender, EventArgs e)
        {
<<<<<<< HEAD
            if (!boolInitialPass)
=======
            //only show this dialog if model is complete
            if (boolModelComplete)
>>>>>>> a6b2f91fbf737b2a211f552c2c51c015a079edcb
            {
                //Ask whether the user wants to clobber the modeling and prediction tabs by modifying the data.
                DialogResult dlgr = MessageBox.Show("Changes in data and/or data attributes have occurred.\nPrevious modeling results will be erased. Proceed?", "Proceed to Modeling.", MessageBoxButtons.OKCancel);
                if (dlgr == DialogResult.Cancel)
                {
<<<<<<< HEAD
                    return;
                }
            }

            //Datasheet is complete when go to modeling is clicked
            boolComplete = true;
            boolInitialPass = false;
=======
                    boolModelComplete = false;
                }
            }
            boolComplete = true;
>>>>>>> a6b2f91fbf737b2a211f552c2c51c015a079edcb
            Broadcast();
        }  
    }
}