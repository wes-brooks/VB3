using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using VBTools;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;


namespace VBProjectManager
{
    //why does this class act as a proj manager and as a plugin? Is this equivalent to HydroDesktopMainPlugin.cs?
    //or is containerForm the main plugin?
    //combining ProjectManager and HydroDesktopMainPlugin ??
    public class VBProjectManager : Extension, IFormState
    {
        [Import("Shell")]
        public ContainerControl Shell { get; set; }

        private Dictionary<string, Boolean> _tabStates;

        private string strName;

        private VBProjectManager myProjectManager;

        public override void Activate()
        {
            App.DockManager.ActivePanelChanged += DockManager_ActivePanelChanged;
            myProjectManager = new VBProjectManager(App);

            App.HeaderControl.RootItemSelected += HeaderControl_RootItemSelected;
            App.SerializationManager.Serializing += SerializationManager_Serializing;
            App.SerializationManager.Deserializing += SerializationManager_Deserializing;
            App.SerializationManager.NewProjectCreated += SerializationManager_NewProjectCreated;
            App.SerializationManager.IsDirtyChanged += SerializationManager_IsDirtyChanged;

            App.HeaderControl.Add(new SimpleActionItem("My Button Caption", ButtonClick));
            base.Activate();
        }

        public override void Deactivate()
        {
            //if (Shell is Form)
            //{
            //    ((Form)Shell).FormClosing -= HydroDesktopMainPlugin_FormClosing;
            //}

            App.HeaderControl.RemoveAll();
            base.Deactivate();
        }

        public void ButtonClick(object sender, EventArgs e)
        {
            // Your logic goes here.
        }


        public Dictionary<string, Boolean> TabStates
        {
            get { return _tabStates; }
            set { _tabStates = value; }
        }


        public class ProjectChangedStatus : EventArgs
        {
            private string _status;
            public string Status
            {
                set { _status = value; }
                get { return _status; }
            }
        }


        public void PackState(object objPackedStates)
        {
            //This function packs the state of the Project Manager for saving to disk.
        }


        public void UnpackState(object objPackedStates)
        {
            //This function restores the previously packed state of the Project Manager.
        }


        public string ProjectName
        {
            get { return strName; }
            set { strName = value; }
        }

        public VBProjectManager(AppManager mainApp)
        {
            App = mainApp;
        }

        void DockManager_ActivePanelChanged(object sender, DotSpatial.Controls.Docking.DockablePanelEventArgs e)
        {
            //only cared about the map and legend

            //if (e.ActivePanelKey == "kMap")
            //{
            //    App.DockManager.SelectPanel("kLegend");

            //    //if the clicked root item was 'search', then don't select the map root item
            //    //(the user intended to show search tab and map panel)
            //    if (!App.SerializationManager.GetCustomSetting("SearchRootClicked", false))
            //    {
            //        App.HeaderControl.SelectRoot(HeaderControl.HomeRootItemKey);
            //    }
            //    else
            //    {
            //        App.SerializationManager.SetCustomSetting("SearchRootClicked", false);
            //    }
            //}
        }
        void HeaderControl_RootItemSelected(object sender, RootItemEventArgs e)
        {
            //
            //if (e.SelectedRootKey == HeaderControl.HomeRootItemKey)
            //{
            //    App.DockManager.SelectPanel("kMap");
            //    if (latLongDisplay != null)
            //        this.latLongDisplay.ShowCoordinates = true;
            //}
            //else
            //{
            //    if (latLongDisplay != null)
            //        latLongDisplay.ShowCoordinates = false;
            //}
        }
        void SerializationManager_Serializing(object sender, SerializingEventArgs e)
        {
            //myProjectManager.SavingProject();
            //Shell.Text = string.Format("{0} - {1}", HYDRODESKTOP_NAME, GetProjectShortName());
        }
        void SerializationManager_IsDirtyChanged(object sender, EventArgs e)
        {
            if (App.SerializationManager.IsDirty && !(Shell.Text.EndsWith(" *")))
            {
                Shell.Text += " *";
            }
            else if (!App.SerializationManager.IsDirty && Shell.Text.EndsWith(" *"))
            {
                Shell.Text = Shell.Text.Substring(0, Shell.Text.LastIndexOf("*"));
            }
        }

        void SerializationManager_NewProjectCreated(object sender, SerializingEventArgs e)
        {
            //SetupDatabases();
            //Shell.Text = HYDRODESKTOP_NAME;
            ////setup new db information
            ////SeriesControl.SetupDatabase();
            //if (App.Map.Projection != null)
            //{
            //    latLongDisplay.MapProjectionString = App.Map.Projection.ToEsriString();
            //}
        }

        void SerializationManager_Deserializing(object sender, SerializingEventArgs e)
        {
            //try reset projection!
            //if (App.Map.MapFrame.Projection != DotSpatial.Projections.KnownCoordinateSystems.Projected.World.WebMercator)
            //{
            //    //App.Map.MapFrame.Reproject(DotSpatial.Projections.KnownCoordinateSystems.Projected.World.WebMercator);
            //    MapFrameProjectionHelper.ReprojectMapFrame(App.Map.MapFrame, DotSpatial.Projections.KnownCoordinateSystems.Projected.World.WebMercator.ToEsriString());
            //}

            //myProjectManager.OpeningProject();
            //Shell.Text = string.Format("{0} - {1}", HYDRODESKTOP_NAME, GetProjectShortName());
            ////setup new db information
            //SeriesControl.SetupDatabase();
            //if (App.Map.Projection != null)
            //{
            //    latLongDisplay.MapProjectionString = App.Map.Projection.ToEsriString();
            //}

            //disable progress reporting for the layers
        }

    }
}
