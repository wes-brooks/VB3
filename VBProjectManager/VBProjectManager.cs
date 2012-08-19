using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VBCommon;
using VBCommon.Interfaces;
using VBCommon.PluginSupport;


namespace VBProjectManager
{
    //class to manage all plugins
    public partial class VBProjectManager : Extension, IFormState, IPartImportsSatisfiedNotification, IPlugin
    {        
        private Dictionary<string, Boolean> dictTabStates;
        private string strPathName;
        private string strProjectName;
        private VBCommon.Signaller signaller = new VBCommon.Signaller();
        private Globals.PluginType _pluginType = VBCommon.Globals.PluginType.ProjectManager;
        private VBLogger logger;
        private string strLogFile;
        private string strPluginKey = "Project Manager";
        public static string VB2projectsPath = null;
        private Boolean boolComplete = false;
        private Boolean boolVisible = false;

        //constructor
        public VBProjectManager()
        {
            strLogFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VirtualBeach.log");
            VBLogger.SetLogFileName(strLogFile);
            logger = VBLogger.GetLogger();

            signaller = new VBCommon.Signaller();
        }


        public override void Activate()
        {       
            //Add an Open button to the application ("File") menu.
            var btnOpen = new SimpleActionItem(HeaderControl.ApplicationMenuKey, "Open", Open);
            btnOpen.GroupCaption = HeaderControl.ApplicationMenuKey;
            btnOpen.LargeImage = Resources.open_16x16;
            //btnOpen.SmallImage = Resources.open_16x16;
            btnOpen.ToolTipText = "Open a saved project.";
            App.HeaderControl.Add(btnOpen);                
            //Add a Save button to the application ("File") menu.
            var btnSave = new SimpleActionItem(HeaderControl.ApplicationMenuKey, "Save", Save);
            btnSave.GroupCaption = HeaderControl.ApplicationMenuKey;
            btnSave.LargeImage = Resources.Save16x16;
            //btnSave.SmallImage = Resources.save_16x16;
            btnSave.ToolTipText = "Save the current project state.";
            App.HeaderControl.Add(btnSave);

            //Add a Save As button to the application ("File") menu.
            var btnSaveAs = new SimpleActionItem(HeaderControl.ApplicationMenuKey, "Save As", SaveAs);
            btnSaveAs.GroupCaption = HeaderControl.ApplicationMenuKey;
            btnSaveAs.LargeImage = Resources.SaveAs16x16;
            //btnSaveAs.SmallImage = Resources.open_16x16;
            btnSaveAs.ToolTipText = "test hide panel";
            App.HeaderControl.Add(btnSaveAs);
            
            //Add an item to the application ("File") menu.
            var btnAbout = new SimpleActionItem(HeaderControl.ApplicationMenuKey, "About", AboutVirtualBeach);
            btnAbout.GroupCaption = HeaderControl.ApplicationMenuKey;
            btnAbout.LargeImage = Resources.About_16x16;
            //btnAbout.SmallImage = Resources.info_16x16;
            btnAbout.ToolTipText = "Open the 'About VirtualBeach' dialog.";
            App.HeaderControl.Add(btnAbout);
                       
            //get plugin type for each plugin
            List<Globals.PluginType> lstAllPluginTypes = new List<Globals.PluginType>();
            Globals.PluginType PType;

            foreach(DotSpatial.Extensions.IExtension ext in App.Extensions)  
            {
                if (ext is IPlugin)    
                {
                    IPlugin plugType = (IPlugin)ext;  
                    //store pluginType
                    PType = plugType.PluginType;  
                    lstAllPluginTypes.Add(PType); 
                }
            }
            
            //if PType is smallest (datasheet/map), set as activated when open
            int pos = lstAllPluginTypes.IndexOf(lstAllPluginTypes.Min());
            DotSpatial.Extensions.IExtension extension = App.Extensions.ElementAt(pos);
            IPlugin ex = (IPlugin)extension;
            ex.MakeActive();
            
            //initialize only Datasheet is shown, all others are hidden
            foreach(DotSpatial.Extensions.IExtension x in App.Extensions)
            {
                if (x is IPlugin)
                {
                    //hide the rest
                    IPlugin pt = (IPlugin)x;
                    if ((Int32)pt.PluginType > (Int32)Globals.PluginType.Datasheet)
                        pt.Hide();
                }
            }
                    
            base.Activate();
        }


        //will always be active (inherits from IPlugin)
        public void MakeActive()
        {
            //App.DockManager.SelectPanel(strPanelKey);
            //App.HeaderControl.SelectRoot(strPanelKey);
        }


        //(inherits from IPlugin)
        public void Show()
        {
            
        }


        //(inherits from IPlugin)
        public void Hide()
        {
            
        }

        //projectManager doesn't have a ribbon, (inherits from IPlugin)
        public void AddRibbon(string sender)
        {

        }

        //projectManager doesn't get deactivated (inherits from IPlugin)
        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            
            base.Deactivate();
        }


        //Write any messages we receive from the plugins directly to the debug console.
        private void MessageReceived(object sender, MessageArgs args)
        {
            
            System.Diagnostics.Debug.WriteLine(args.Message);
        }


        //Information about virtual beach
        public void AboutVirtualBeach(object sender, EventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("this is a test.");
        }


        //not sure if used
        public Dictionary<string, Boolean> TabStates
        {
            get { return dictTabStates; }
            set { dictTabStates = value; }
        }


        //holds project full path name
        public string ProjectPathName
        {
            get { return strPathName; }
            set { strPathName = value; }
        }


        //holds just the project name without path
        public string ProjectName
        {
            get { return strProjectName; }
            set { strProjectName = value; }
        }


        //inherits from IPlugin, ProjectManager's complete flag doesn't change
        public Boolean Complete
        {
            get { return boolComplete;}
        }


        //inherits from IPlugin, ProjectManager's visible flag doesn't change
        public Boolean VisiblePlugin
        {
            get { return boolVisible; }
        }


        //returns the panel key name
        public string PanelKey
        {
            get { return strPluginKey; }
        }


        //returns the projectManager's pluginType (ProjectManager)
        public Globals.PluginType PluginType
        {
            get { return _pluginType; }
        }

        //We export this property so that other Plugins can have access to the signaller.
        public VBCommon.Signaller Signaller
        {
            get { return this.signaller; }
        }


        //We export this method so that other Plugins can have access to the signaller.
        [System.ComponentModel.Composition.Export("Signalling.GetSignaller")]
        public VBCommon.Signaller ProvideAccessToSignaller()
        {
            return this.Signaller;
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
            
            signaller.MessageReceived += new VBCommon.Signaller.MessageHandler<MessageArgs>(MessageReceived);
            signaller.ProjectSaved += new VBCommon.Signaller.SerializationEventHandler<VBCommon.PluginSupport.SerializationEventArgs>(ProjectSavedListener);
            signaller.ProjectOpened += new VBCommon.Signaller.SerializationEventHandler<VBCommon.PluginSupport.SerializationEventArgs>(ProjectOpenedListener); //loop through plugins ck for min pluginType to make that active when plugin opened.
            signaller.BroadcastState += new VBCommon.Signaller.BroadCastEventHandler<VBCommon.PluginSupport.BroadCastEventArgs>(BroadcastStateListener);
            
        }

        
        //listen to plugin's broadcast in order to update other plugins
        private void BroadcastStateListener(object sender, VBCommon.PluginSupport.BroadCastEventArgs e)
        {
            string strPluginType = (((IPlugin)sender).PluginType).ToString();
            //if datasheet is broadcasting any changes
            if (strPluginType == "Datasheet")
            {
                //find modeling plugin, needs to show itself once datasheet broadcasts itself with complete flag raised
                foreach (DotSpatial.Extensions.IExtension ex in App.Extensions)
                {
                    IPlugin plugin = (IPlugin)ex;

                    if (plugin.PluginType.ToString() == "Modeling")
                        //already visible, just update not show again
                        if (plugin.VisiblePlugin)
                            return;
                        //if datasheet is complete, show modeling
                        else if (((IPlugin)sender).Complete)
                            plugin.Show();
                }
                //if modeling is broadcasting itself
            }
            else if (strPluginType == "Modeling")
            {
                //find prediction plugin, needs to show itself once modeling broadcasts itself with complete flag raised
                foreach (DotSpatial.Extensions.IExtension ex in App.Extensions)
                {
                    IPlugin plugin = (IPlugin)ex;
                    if (plugin.PluginType.ToString() == "Prediction")
                        //already visible, just update not show again
                        if (plugin.VisiblePlugin)
                            return;
                        else
                            //modeling is complete, show prediction
                            if (((IPlugin)sender).Complete)
                                plugin.Show();
                }
            }                
        }


        //projectManager broadcasting itself when a change is made
        public void Broadcast()
        {
            IDictionary<string, object> packedState = PackState();
            signaller.RaiseBroadcastRequest(_pluginType, packedState);
        }

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
}
