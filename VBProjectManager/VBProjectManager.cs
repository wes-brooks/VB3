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
 //       private Dictionary<string, Boolean> dictTabStates;
        private string strPathName;
        private string strProjectName;
        private string strTopPlugin; //plugin change event changes this value

        private VBCommon.Signaller signaller = new VBCommon.Signaller();
        private Globals.PluginType _pluginType = VBCommon.Globals.PluginType.ProjectManager;
        private VBLogger logger;
        private string strLogFile;
        private string strPluginKey = "Project Manager";
        public static string VBProjectsPath = null;
        private Boolean boolComplete = false;
        private Boolean boolVisible = false;
        public Stack UndoRedoStack = new Stack();
                
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
            btnOpen.ToolTipText = "Open a saved project.";
            App.HeaderControl.Add(btnOpen);      
            
            //Add a Save button to the application ("File") menu.
            var btnSave = new SimpleActionItem(HeaderControl.ApplicationMenuKey, "Save", Save);
            btnSave.GroupCaption = HeaderControl.ApplicationMenuKey;
            btnSave.LargeImage = Resources.Save16x16;
            btnSave.ToolTipText = "Save the current project state.";
            App.HeaderControl.Add(btnSave);

            //Add a Save As button to the application ("File") menu.
            var btnSaveAs = new SimpleActionItem(HeaderControl.ApplicationMenuKey, "Save As", SaveAs);
            btnSaveAs.GroupCaption = HeaderControl.ApplicationMenuKey;
            btnSaveAs.LargeImage = Resources.SaveAs16x16;
            btnSaveAs.ToolTipText = "test hide panel";
            App.HeaderControl.Add(btnSaveAs);
            
            //Add an item to the application ("File") menu.
            var btnAbout = new SimpleActionItem(HeaderControl.ApplicationMenuKey, "About", AboutVirtualBeach);
            btnAbout.GroupCaption = HeaderControl.ApplicationMenuKey;
            btnAbout.LargeImage = Resources.About_16x16;
            btnAbout.ToolTipText = "Open the 'About VirtualBeach' dialog";
            App.HeaderControl.Add(btnAbout);

            //add an undo button to application ("Edit") menu... ??
            var btnUndo = new SimpleActionItem(HeaderControl.ApplicationMenuKey, "Undo", UndoAction);
            btnUndo.GroupCaption = HeaderControl.ApplicationMenuKey;
            btnUndo.LargeImage = Resources.Undo_16x16;
            btnUndo.ToolTipText = "Undo the last action";
            App.HeaderControl.Add(btnUndo);

            //get plugin type for each plugin
            List<Globals.PluginType> lstAllPluginTypes = new List<Globals.PluginType>();

            foreach(DotSpatial.Extensions.IExtension ext in App.Extensions)  
            {
                if (ext is IPlugin)    
                {               
                    if (((IPlugin)ext).PluginType >= 0)
                        lstAllPluginTypes.Add(((IPlugin)ext).PluginType); 
                }
            }

            //Hook up the event handler that fires when the user clicks on a new plugin.
            App.DockManager.ActivePanelChanged += new EventHandler<DotSpatial.Controls.Docking.DockablePanelEventArgs>(DockManager_ActivePanelChanged);

            //if PType is smallest (datasheet/map), set as activated when open
            int pos = lstAllPluginTypes.IndexOf(lstAllPluginTypes.Min());
            DotSpatial.Extensions.IExtension extension = App.Extensions.ElementAt(pos);
            IPlugin ex = (IPlugin)extension;
            if (ex != null)
                ex.MakeActive();
            
            /*//initialize only Datasheet is shown, all others are hidden
            foreach(DotSpatial.Extensions.IExtension x in App.Extensions)
            {
                if (x is IPlugin)
                {
                    IPlugin pt = (IPlugin)x;
                    if ((Int32)pt.PluginType > (Int32)Globals.PluginType.Datasheet)
                        pt.Hide();
                }
            }*/
                    
            base.Activate();
        }


        //IPlugin requires that we override these, but they have no purpose for the VBProjectManager.
        public void MakeActive() {}
        public void Show() {}
        public void Hide() {}
        public void AddRibbon(string sender) {}


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
        

        //holds project full path name
        public string ProjectPathName
        {
            get { return strPathName; }
            set { strPathName = value; }
        }


        /*//once open, store top plugin value here
        public string OpeningTopPlugin
        {
            get { return openingTopPlugin; }
            set { openingTopPlugin = value; }
        }

        //holds top plugin for opening
        public string TopPlugin
        {
            get { return strTopPlugin; }
            set { strTopPlugin = value; }
        }*/


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
        public Boolean Visible
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
            signaller.BroadcastState += new VBCommon.Signaller.BroadcastEventHandler<VBCommon.PluginSupport.BroadcastEventArgs>(BroadcastStateListener);
        }


        //event handler when a plugin is selected from tabs
        void DockManager_ActivePanelChanged(object sender, DotSpatial.Controls.Docking.DockablePanelEventArgs e)
        {
            strTopPlugin = e.ActivePanelKey;

            bool boolHideModeling = false;

            foreach (DotSpatial.Extensions.IExtension x in App.Extensions)
            {
                if (x is VBCommon.Interfaces.IPlugin)
                {
                    if (((VBCommon.Interfaces.IPlugin)x).PluginType == VBCommon.Globals.PluginType.Datasheet && ((VBCommon.Interfaces.IPlugin)x).PanelKey == e.ActivePanelKey)
                        boolHideModeling = true;
                }
            }

            if (boolHideModeling)
            {
            }
        }
        

        //pop off last stack for undo
        public void UndoAction(object sender, EventArgs e)
        {
            //check to see if pop is a model first, first undo on model will accidentally send unpack to _frmDatasheet
            object objCurrentState = UndoRedoStack.Pop();
            object objLastState = UndoRedoStack.Peek();

            IDictionary<string, object> dictLastState = (IDictionary<string, object>)objLastState;
            string strKey;

            signaller.RaiseBroadcastRequest(this, dictLastState);

            /*if ((bool)((Dictionary<string, object>)whatAreWePopping).ContainsKey("PLSPanel") || (bool)((Dictionary<string, object>)whatAreWePopping).ContainsKey("GBMPanel"))
            {
                Dictionary<string, object> modelPop = new Dictionary<string, object>((Dictionary<string, object>)whatAreWePopping);

                //ensure the unpack goes to the model, not datasheet
                foreach (KeyValuePair<string, object> pair in modelPop)
                { stackKey = pair.Key; }

                foreach (DotSpatial.Extensions.IExtension x in App.Extensions)
                {
                    if (x is IPlugin)
                    {
                        IPlugin thisPlug = (IPlugin)x;
                        if (thisPlug.PanelKey == stackKey)
                        { //thisPlug.unpack back 
                            Dictionary<string, object> dictUnpackThis = new Dictionary<string, object>();
                            foreach (KeyValuePair<string, object> getValue in dictLastStackItem)
                            { dictUnpackThis = (Dictionary<string,object>)getValue.Value; }
                         

                           // Broadcast(this, dictUnpackThis);
                        }
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<string, object> pair in dictLastStackItem)
                {
                    stackKey = pair.Key;
                }

                foreach (DotSpatial.Extensions.IExtension x in App.Extensions)
                {
                    if (x is IPlugin)
                    {
                        IPlugin thisPlug = (IPlugin)x;
                        if (thisPlug.PanelKey == stackKey)
                        { //thisPlug.unpack back 
                            Dictionary<string, object> dictUnpackThis = new Dictionary<string, object>();
                            dictUnpackThis = (Dictionary<string, object>)dictLastStackItem[stackKey];
                            //thisPlug.UndoLastChange(dictUnpackThis);
                        }
                    }
                }
            }*/
        }


        //listen to plugin's broadcast in order to update other plugins
        private void BroadcastStateListener(object sender, VBCommon.PluginSupport.BroadcastEventArgs e)
        {
            if (((IPlugin)sender).PluginType != Globals.PluginType.ProjectManager)
            {
                KeyValuePair<string, object> kvpStackObj = new KeyValuePair<string, object>(((VBCommon.Interfaces.IPlugin)sender).PanelKey, e.PackedPluginState);
                UndoRedoStack.Push(kvpStackObj);    
            }
        }


        //projectManager broadcasting itself when a change is made
        public void Broadcast()
        {
            IDictionary<string, object> packedState = PackState();
            signaller.RaiseBroadcastRequest(this, packedState);
        }

    }
}
