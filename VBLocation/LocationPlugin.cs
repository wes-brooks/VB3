using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.ComponentModel.Composition;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using VBCommon;
using VBCommon.Interfaces;
using VBCommon.PluginSupport;

namespace VBLocation
{
    public class LocationPlugin : Extension, IPartImportsSatisfiedNotification, IPlugin, IBeachSite
    {
        [Import("Shell")]
        private ContainerControl Shell { get; set; }

        private VBCommon.Signaller signaller;

        private frmLocation cLocation;
        private const string strPanelKey = "kVBLocation";
        private const string strPanelCaption = "Location";
        private Globals.PluginType pluginType = VBCommon.Globals.PluginType.Map;
        private RootItem locationTab;
        private SimpleActionItem btnNull;

        //complete and visible flags
        public Boolean boolComplete;
        public Boolean boolVisible = true;

        //raise a message
        public delegate void MessageHandler<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
        public event MessageHandler<MessageArgs> MessageSent;


        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            App.DockManager.Remove(strPanelKey);

            cLocation = null;

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


        //add a datasheet plugin root item
        public void AddRibbon(string sender)
        {
            locationTab = new RootItem(strPanelKey, strPanelCaption);
            locationTab.SortOrder = (short)pluginType;
            App.HeaderControl.Add(locationTab);

            //tell ProjMngr if this is being Shown
            if (sender == "Show")
            {
                //make this the selected root
                App.HeaderControl.SelectRoot(strPanelKey);
            }


            //section for working with data
            const string grpManipulate = "Waste time and space";

            btnNull = new SimpleActionItem(strPanelKey, "Do nothing!", btnNull_Click);
            btnNull.LargeImage = Properties.Resources.USGS;
            btnNull.GroupCaption = grpManipulate;
            btnNull.Enabled = false;
            App.HeaderControl.Add(btnNull);

            //var validateBtn = new SimpleActionItem("Validate", btnValidate_Click) { RootKey = kVBLocation, ToolTipText = "Validate Data", LargeImage = Properties.Resources.validate };
            //App.HeaderControl.Add(validateBtn);

            //var computeBtn = new SimpleActionItem("Compute", btnCompute_Click) { RootKey = kVBLocation, ToolTipText = "Compute" };
            //App.HeaderControl.Add(computeBtn);

            //var manipulateBtn = new SimpleActionItem("Manipulate", btnManipulate_Click) { RootKey = kVBLocation, ToolTipText = "Manipulate", LargeImage = Properties.Resources.manipulate };
            //App.HeaderControl.Add(manipulateBtn);

            //var transformBtn = new SimpleActionItem("Transform", btnTransform_Click) { RootKey = kVBLocation, ToolTipText = "Transform", LargeImage = Properties.Resources.transform };
            //App.HeaderControl.Add(transformBtn);
        }


        public override void Activate()
        {
            cLocation = new frmLocation(this) { Dock = System.Windows.Forms.DockStyle.Fill };
            AddRibbon("Activate");
            AddPanel();

            //when panel is selected activate seriesview and ribbon tab
            App.DockManager.ActivePanelChanged += new EventHandler<DotSpatial.Controls.Docking.DockablePanelEventArgs>(DockManager_ActivePanelChanged);

            //when root item is selected
            App.HeaderControl.RootItemSelected += new EventHandler<RootItemEventArgs>(HeaderControl_RootItemSelected);

            base.Activate(); //ensures "enabled" is set to true
        }


        //add a datasheet panel
        public void AddPanel()
        {
            var dp = new DockablePanel(strPanelKey, strPanelCaption, cLocation, System.Windows.Forms.DockStyle.Fill);
            dp.DefaultSortOrder = (short)pluginType;
            App.DockManager.Add(dp);
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


        public void MakeActive()
        {
            App.DockManager.SelectPanel(strPanelKey);
            App.HeaderControl.SelectRoot(strPanelKey);
        }


        void DockManager_ActivePanelChanged(object sender, DockablePanelEventArgs e)
        {
            if (e.ActivePanelKey == strPanelKey)
            {
                App.DockManager.SelectPanel(strPanelKey);
                App.HeaderControl.SelectRoot(strPanelKey);
               //RefreshDatabasePath();
            }
        }


        void HeaderControl_RootItemSelected(object sender, RootItemEventArgs e)
        {
            if (e.SelectedRootKey == strPanelKey)
            {
                App.DockManager.SelectPanel(strPanelKey);
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


        private void btnNull_Click(object sender, EventArgs e)
        {
            //cLocation.btnImport_Click(sender, e);
        }


        private void btnImport_Click(object sender, EventArgs e)
        {
            //cLocation.btnImport_Click(sender, e);
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            //cDatasheet.btnValidate_Click(sender, e);
        }

        private void btnCompute_Click(object sender, EventArgs e)
        {
            //cDatasheet.btnComputeUV_Click(sender, e);
        }

        private void btnManipulate_Click(object sender, EventArgs e)
        {
            //cDatasheet.btnManipulate_Click(sender, e);
        }

        private void btnTransform_Click(object sender, EventArgs e)
        {
            //cDatasheet.btnTransform_Click(sender, e);
        }

        private void BroadcastStateListener(object sender, BroadCastEventArgs e)
        {
            //listen to others broadcast..receiving something
            //e.PackedPluginState

        }


        public void Broadcast()
        {
            //get packed state, add complete and visible and raise broadcast event
            IDictionary<string, object> dictPackedState = cLocation.PackState();
            dictPackedState.Add("Complete", boolComplete);
            dictPackedState.Add("Visible", boolVisible);
            signaller.RaiseBroadcastRequest(this, dictPackedState);
        }


        private void ProjectSavedListener(object sender, SerializationEventArgs e)
        {
            e.PackedPluginStates.Add(strPanelKey, cLocation.PackState());
        }


        private void ProjectOpenedListener(object sender, SerializationEventArgs e)
        {
            if (e.PackedPluginStates.ContainsKey(strPanelKey))
            {
                //Unpack the state of this plugin.
                cLocation.UnpackState(e.PackedPluginStates[strPanelKey]);
            }
            else
            {
                //Set this plugin to an empty state.
            }
        }


        private void SendMessage(string message)
        {
            if (MessageSent != null) //Has some method been told to handle this event?
            {
                MessageArgs e = new MessageArgs(message);
                MessageSent(this, e);
            }
        }


        private IBeachSite _site = new VBCommon.Spatial.Site();
        public IBeachSite Site
        {
            get { return _site; }
            set { _site = value; }
        }

        /// <summary>
        /// Implementation for IBeachSite interface
        /// </summary>
        #region IBeachSite interface

        
        public VBCommon.Spatial.PointGeo Location
        {
            get { return _site.Location; }
            set { _site.Location = value; }
        }

        public string BeachName
        {
            get { return _site.BeachName; }
            set { _site.BeachName = value; }
        }

        public double Orientation
        {
            get {return _site.Orientation;}
            set { _site.Orientation  = value; }
        }

        public VBCommon.Spatial.PointGeo LeftMarker
        {
            get { return _site.LeftMarker; }
            set { _site.LeftMarker = value; }
        }
       
        public VBCommon.Spatial.PointGeo RightMarker
        {
            get { return _site.RightMarker; }
            set { _site.RightMarker = value; }
        }

        public VBCommon.Spatial.PointGeo WaterMarker
        {
            get { return _site.WaterMarker; }
            set { _site.WaterMarker = value; }
        }

        public IBeachSite Clone()
        {
            return _site.Clone();
        }

        #endregion
    }
}
