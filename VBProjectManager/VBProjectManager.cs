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

        public VBProjectManager()
        {
            signaller.MessageReceived += new VBTools.Signaller.MessageHandler<MessageArgs>(MessageReceived);
            //App.SerializationManager.Serializing += new VBProjectManager.ProjectSavedHandler<PackEventArgs>(ProjectSavedListener);
        }


        private void MessageReceived(object sender, MessageArgs args)
        {

        }


        public override void Activate()
        {
            App.HeaderControl.Add(new SimpleActionItem("My Button Caption", AboutVirtualBeach));
                       
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
            
            base.Activate();
        }


        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            base.Deactivate();
        }


        public void Save(object sender, EventArgs e)
        {
            SerializableDictionary<string, object> pluginStates = new SerializableDictionary<string, object>();
            signaller.RaiseSaveRequest(pluginStates);
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


        private void ProjectOpenedListener(object sender, UnpackEventArgs e)
        {
        }


        public object PackState()
        {
            return null;
            //This function packs the state of the Project Manager for saving to disk.
        }


        public void UnpackState(object objPackedStates)
        {
            //This function restores the previously packed state of the Project Manager.
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
            System.Windows.Forms.MessageBox.Show("Imports satisfied on project manager");
            signaller = GetSignaller();
            signaller.ProjectSaved += new VBTools.Signaller.ProjectSavedHandler<VBTools.PackEventArgs>(ProjectSavedListener);
        }


        private void ProjectSavedListener(object sender, VBTools.PackEventArgs e)
        {
            e.PackedPluginStates.Add("ProjectManager", PackProjectState());
        }
    }
}
