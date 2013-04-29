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
        private SimpleActionItem btnFindDataSources;
        private SimpleActionItem btnClearDataSources;

        //complete and visible flags
        public Boolean boolComplete;
        public Boolean boolVisible;
        private Boolean boolChanged = false;

        //this plugin was clicked
        private string strTopPlugin = string.Empty;

        //raise a message
        public delegate void MessageHandler<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
        public event MessageHandler<MessageArgs> MessageSent;
	
        private Stack<string> UndoKeys;
        private Stack<string> RedoKeys;


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
            if (boolVisible)
            {
                boolChanged = true;
                App.HeaderControl.RemoveAll();
                ((VBDockManager.VBDockManager)App.DockManager).HidePanel(strPanelKey);
                boolVisible = false;
            }
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
            const string grpManipulate = "Data";

            btnFindDataSources = new SimpleActionItem(strPanelKey, "Zoom In", ZoomIn);
            btnFindDataSources.LargeImage = Properties.Resources.Zoom_In_icon;
            btnFindDataSources.GroupCaption = grpManipulate;
            btnFindDataSources.Enabled = true;
            App.HeaderControl.Add(btnFindDataSources);

            btnClearDataSources = new SimpleActionItem(strPanelKey, "Zoom Out", ZoomOut);
            btnClearDataSources.LargeImage = Properties.Resources.Zoom_Out_icon;
            btnClearDataSources.GroupCaption = grpManipulate;
            btnClearDataSources.Enabled = true;
            App.HeaderControl.Add(btnClearDataSources);
        }


        public override void Activate()
        {
            UndoKeys = new Stack<string>();
            RedoKeys = new Stack<string>();
        
            cLocation = new frmLocation();
            cLocation.LocationFormEvent += new EventHandler(LocationEventHandler);

            AddPanel();
            AddRibbon("Activate");
            
            //when panel is selected activate seriesview and ribbon tab
            App.DockManager.ActivePanelChanged += new EventHandler<DotSpatial.Controls.Docking.DockablePanelEventArgs>(DockManager_ActivePanelChanged);
            App.HeaderControl.RootItemSelected += new EventHandler<RootItemEventArgs>(HeaderControl_RootItemSelected);
            
	        boolChanged = false;
            base.Activate(); 
	    
	        boolVisible = true;
	        Show();
        }


        public void AddPanel()
        {
            var dp = new DockablePanel(strPanelKey, strPanelCaption, cLocation, System.Windows.Forms.DockStyle.Fill);
            dp.DefaultSortOrder = (short)pluginType;
            App.DockManager.Add(dp);
        }


        public void Show()
        {            
		if (!boolVisible)
		{
			boolChanged = true;
			AddRibbon("Show");
			((VBDockManager.VBDockManager)App.DockManager).SelectPanel(strPanelKey);
			App.HeaderControl.SelectRoot(strPanelKey);
			boolVisible = true;
		}
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


        private void LocationEventHandler(object sender, EventArgs e)
        {
            Broadcast();
        }


        private void ZoomIn(object sender, EventArgs e)
        {
            if (cLocation.ZoomControl.Value < cLocation.ZoomControl.Maximum)
                cLocation.ZoomControl.Value += 1;
        }

        private void ZoomOut(object sender, EventArgs e)
        {
            if (cLocation.ZoomControl.Value > cLocation.ZoomControl.Minimum)
                cLocation.ZoomControl.Value -= 1;
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


        private void BroadcastStateListener(object sender, BroadcastEventArgs e)
        {
            /*if (((IPlugin)(e.Sender)).PanelKey == strPanelKey)
            {
                //Unpack the state of this plugin.
                cLocation.UnpackState(new Dictionary<string, object>());
            }       */   
        }


        public void Broadcast()
        {
	        boolChanged = true;
            //get packed state, add complete and visible and raise broadcast event
            IDictionary<string, object> dictPackedState = cLocation.PackState();

            boolComplete = (bool)dictPackedState["Complete"];
            dictPackedState.Add("Visible", boolVisible);

            signaller.RaiseBroadcastRequest(this, dictPackedState);
            signaller.TriggerUndoStack();
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
			
			if (!e.PredictionOnly)
			{
                Show();
			}
			else
			{
				Hide();
			}
        }

	
        private void PushToStack(object sender, UndoRedoEventArgs args)
        {
            if (boolChanged)
            {
                //go pack state, add complete and visible, and add to dictionary of plugins
                IDictionary<string, object> packedState = cLocation.PackState();

                if (packedState != null)
                {                    
                    packedState.Add("Visible", boolVisible);

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
                    cLocation.UnpackState(dictPlugin);
                }
            }
            catch
            {
                //Activate();
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
                    cLocation.UnpackState(dictPlugin);
                }
            }
            catch
            {
                //Activate();
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
