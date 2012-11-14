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
using Microsoft.Isam.Esent.Collections.Generic;
using Newtonsoft.Json;

namespace VBProjectManager
{
    //class to manage all plugins
    public partial class VBProjectManager : Extension, IFormState, IPartImportsSatisfiedNotification, IPlugin
    {
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

        private Stack UndoStack = new Stack();
        private Stack RedoStack = new Stack();
        private SimpleActionItem btnUndo;
        private SimpleActionItem btnRedo;
        

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
            short n = 0;

            //Add an Open button to the application ("File") menu.
            var btnOpen = new SimpleActionItem(HeaderControl.ApplicationMenuKey, "Open", Open);
            btnOpen.GroupCaption = HeaderControl.ApplicationMenuKey;
            btnOpen.LargeImage = Resources.open_16x16;
            btnOpen.ToolTipText = "Open a saved project.";
            btnOpen.SortOrder = n;
            App.HeaderControl.Add(btnOpen);
            n++;

            //Add a Save button to the application ("File") menu.
            var btnSave = new SimpleActionItem(HeaderControl.ApplicationMenuKey, "Save", Save);
            btnSave.GroupCaption = HeaderControl.ApplicationMenuKey;
            btnSave.LargeImage = Resources.Save16x16;
            btnSave.ToolTipText = "Save the current project state.";
            btnSave.SortOrder = n;
            App.HeaderControl.Add(btnSave);
            n++;

            //Add a Save As button to the application ("File") menu.
            var btnSaveAs = new SimpleActionItem(HeaderControl.ApplicationMenuKey, "Save As", SaveAs);
            btnSaveAs.GroupCaption = HeaderControl.ApplicationMenuKey;
            btnSaveAs.LargeImage = Resources.SaveAs16x16;
            btnSaveAs.ToolTipText = "test hide panel";
            btnSaveAs.SortOrder = n;
            App.HeaderControl.Add(btnSaveAs);
            n++;

            //Add an item to the application ("File") menu.
            var btnAbout = new SimpleActionItem(HeaderControl.ApplicationMenuKey, "About", AboutVirtualBeach);
            btnAbout.GroupCaption = HeaderControl.ApplicationMenuKey;
            btnAbout.LargeImage = Resources.About_16x16;
            btnAbout.ToolTipText = "Open the 'About VirtualBeach' dialog";
            btnAbout.SortOrder = n;
            App.HeaderControl.Add(btnAbout);
            n++;

            //add an undo button to application ("Edit") menu... ??
            btnUndo = new SimpleActionItem(HeaderControl.ApplicationMenuKey, "Undo", Undo);
            btnUndo.GroupCaption = HeaderControl.ApplicationMenuKey;
            btnUndo.LargeImage = Resources.Undo_16x16;
            btnUndo.ToolTipText = "Undo the last action";
            btnUndo.Enabled = true;
            btnUndo.SortOrder = n;
            App.HeaderControl.Add(btnUndo);
            n++;

            //add a redo button to application ("Edit") menu... ??
            btnRedo = new SimpleActionItem(HeaderControl.ApplicationMenuKey, "Redo", Redo);
            btnRedo.GroupCaption = HeaderControl.ApplicationMenuKey;
            btnRedo.LargeImage = Resources.Redo_16x16;
            btnRedo.ToolTipText = "Redo the last undo action";
            btnRedo.Enabled = true;
            btnRedo.SortOrder = n;
            App.HeaderControl.Add(btnRedo);
            n++;

            //get plugin type for each plugin
            List<Globals.PluginType> lstAllPluginTypes = new List<Globals.PluginType>();

            foreach (DotSpatial.Extensions.IExtension ext in App.Extensions)
            {
                if (ext is IPlugin)
                {
                    if (((IPlugin)ext).PluginType >= 0)
                        lstAllPluginTypes.Add(((IPlugin)ext).PluginType);
                }
            }

            //Hook up the event handler that fires when the user clicks on a new plugin.
            App.DockManager.ActivePanelChanged += new EventHandler<DotSpatial.Controls.Docking.DockablePanelEventArgs>(DockManager_ActivePanelChanged);

            /*//if PType is smallest (datasheet/map), set as activated when open
            int pos = lstAllPluginTypes.IndexOf(lstAllPluginTypes.Min());
            DotSpatial.Extensions.IExtension extension = App.Extensions.ElementAt(pos);
            IPlugin ex = (IPlugin)extension;
            if (ex != null)
                ex.MakeActive();
                               
            base.Activate();*/
        }


        //IPlugin requires that we override these, but they have no purpose for the VBProjectManager.
        public void MakeActive() { }
        public void Show() { }
        public void Hide() { }
        public void AddRibbon(string sender) { }


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


        public string ProjectName
        {
            get { return strProjectName; }
            set { strProjectName = value; }
        }


        //inherits from IPlugin, ProjectManager's complete flag doesn't change
        public Boolean Complete
        {
            get { return boolComplete; }
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
            signaller.PopulateUndoStackRequested += new EventHandler(PushCurrentState);
        }


        //event handler when a plugin is selected from tabs
        void DockManager_ActivePanelChanged(object sender, DotSpatial.Controls.Docking.DockablePanelEventArgs e)
        {
            strTopPlugin = e.ActivePanelKey;
        }


        //pop off last stack for undo
        public void Undo(object sender, EventArgs e)
        {
            if (UndoStack.Count > 1)
            {
                IDictionary<string, IDictionary<string, object>> dictCurrentState = (IDictionary<string, IDictionary<string, object>>)(UndoStack.Pop());
                IDictionary<string, IDictionary<string, object>> dictLastState = (IDictionary<string, IDictionary<string, object>>)(UndoStack.Peek());

                //raise unpack event, sending packed plugins dictionary
                RedoStack.Push(dictCurrentState);
                signaller.UnpackProjectState(dictLastState);

                if (RedoStack.Count > 0)
                    btnRedo.Enabled = true;                
                if (UndoStack.Count == 1)
                    btnUndo.Enabled = false;
            }
        }


        public void Redo(object sender, EventArgs e)
        {
            if (RedoStack.Count > 0)
            {
                //IDictionary<string, IDictionary<string, object>> dictCurrentState = (IDictionary<string, IDictionary<string, object>>)(UndoStack.Peek());
                IDictionary<string, IDictionary<string, object>> dictLastState = (IDictionary<string, IDictionary<string, object>>)(RedoStack.Pop());

                //raise unpack event, sending packed plugins dictionary
                UndoStack.Push(dictLastState);
                signaller.UnpackProjectState(dictLastState);

                if (RedoStack.Count > 0)
                    btnRedo.Enabled = true;
                if (UndoStack.Count == 1)
                    btnUndo.Enabled = false;
            }
        }

        
        private void BroadcastStateListener(object sender, VBCommon.PluginSupport.BroadcastEventArgs e)
        {
        }


        private void PushCurrentState(object sender, EventArgs e)
        {
            //Dictionary to store each plugin's state for saving
            IDictionary<string, IDictionary<string, object>> dictPluginStates = new Dictionary<string, IDictionary<string, object>>();
            signaller.RaiseSaveRequest(dictPluginStates);
            
            //Manage the undo and redo stacks
            UndoStack.Push(dictPluginStates);
            RedoStack.Clear();

            //Manage the state of the undo/redo buttons.
            btnUndo.Enabled = true;
            btnRedo.Enabled = false;
        }


        public void Broadcast()
        {
            IDictionary<string, object> packedState = PackState();
            signaller.RaiseBroadcastRequest(this, packedState);
        }


        //Generate random strings for use as keys to the Undo/Redo dictionaries
        private static Random random = new Random((int)DateTime.Now.Ticks);//thanks to McAden
        private string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

    }
}
