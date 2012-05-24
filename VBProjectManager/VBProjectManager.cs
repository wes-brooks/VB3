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

    public partial class VBProjectManager : Extension, IFormState, IPartImportsSatisfiedNotification
    {        
        //
        private Dictionary<string, Boolean> _tabStates;
        private string strName;
        private VBTools.Signaller signaller = new VBTools.Signaller();
        private VBLogger logger;
        private string strLogFile;
        private string strPluginKey = "Project Manager";


        public VBProjectManager()
        {
            strLogFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VirtualBeach.log");
            VBLogger.SetLogFileName(strLogFile);
            logger = VBLogger.GetLogger();

            signaller = new VBTools.Signaller();
        }


        public override void Activate()
        {                       
            //Add an item to the application ("File") menu.
            var aboutButton = new SimpleActionItem(HeaderControl.ApplicationMenuKey, "About", AboutVirtualBeach);
            aboutButton.GroupCaption = HeaderControl.ApplicationMenuKey;
            aboutButton.LargeImage = Resources.info_32x32;
            aboutButton.SmallImage = Resources.info_16x16;
            aboutButton.ToolTipText = "Open the 'About VirtualBeach' dialog.";
            App.HeaderControl.Add(aboutButton);
            
            //Add a Save button to the application ("File") menu.
            var saveButton = new SimpleActionItem(HeaderControl.ApplicationMenuKey, "Save", Save);
            saveButton.GroupCaption = HeaderControl.ApplicationMenuKey;
            saveButton.LargeImage = Resources.save_32x32;
            saveButton.SmallImage = Resources.save_16x16;
            saveButton.ToolTipText = "Save the current project state.";
            App.HeaderControl.Add(saveButton);

            //Add an Open button to the application ("File") menu.
            var openButton = new SimpleActionItem(HeaderControl.ApplicationMenuKey, "Open", Open);
            openButton.GroupCaption = HeaderControl.ApplicationMenuKey;
            openButton.LargeImage = Resources.open_32x32;
            openButton.SmallImage = Resources.open_16x16;
            openButton.ToolTipText = "Open a saved project.";
            App.HeaderControl.Add(openButton);
            
            base.Activate();
        }


        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            base.Deactivate();
        }

        
        private void MessageReceived(object sender, MessageArgs args)
        {
            //Write any messages we receive from the plugins directly to the debug console.
            System.Diagnostics.Debug.WriteLine(args.Message);
        }


        public void Save(object sender, EventArgs e)
        {
            SerializableDictionary<string, object> pluginStates = new SerializableDictionary<string, object>();
            signaller.RaiseSaveRequest(pluginStates);
        }


        public void Open(object sender, EventArgs e)
        {
            //Load a project file from disk and then send it out to be unpacked.
            SerializableDictionary<string, object> pluginStates = new SerializableDictionary<string, object>();
            signaller.UnpackProjectState(pluginStates);
        }


        public void AboutVirtualBeach(object sender, EventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("this is a test.");
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


        public string ProjectName
        {
            get { return strName; }
            set { strName = value; }
        }


        //We export this property so that other Plugins can have access to the signaller.
        public VBTools.Signaller Signaller
        {
            get { return this.signaller; }
        }


        //We export this method so that other Plugins can have access to the signaller.
        [System.ComponentModel.Composition.Export("Signalling.GetSignaller")]
        public VBTools.Signaller ProvideAccessToSignaller()
        {
            return this.Signaller;
        }


        //This function imports the signaller from the VBProjectManager
        [System.ComponentModel.Composition.Import("Signalling.GetSignaller", AllowDefault = true)]
        public Func<VBTools.Signaller> GetSignaller
        {
            get;
            set;
        }


        public void OnImportsSatisfied()
        {
            //If we've successfully imported a Signaller, then connect its events to our handlers.
            signaller = GetSignaller();
            signaller.MessageReceived += new VBTools.Signaller.MessageHandler<MessageArgs>(MessageReceived);
            signaller.ProjectSaved += new VBTools.Signaller.SerializationEventHandler<VBTools.SerializationEventArgs>(ProjectSavedListener);
            signaller.ProjectOpened += new VBTools.Signaller.SerializationEventHandler<VBTools.SerializationEventArgs>(ProjectOpenedListener);
        }
    }
}
