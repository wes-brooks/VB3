using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using System.Xml;
//using System.Xml.Serialization;
//using System.Runtime.Serialization;
using VBTools;
using Newtonsoft.Json;


namespace VBProjectManager
{

    public partial class VBProjectManager : Extension, IFormState, IPartImportsSatisfiedNotification
    {        
        private Dictionary<string, Boolean> _tabStates;
        private string strPathName;
        private VBTools.Signaller signaller = new VBTools.Signaller();
        private VBLogger logger;
        private string strLogFile;
        private string strPluginKey = "Project Manager";
        public static string VB2projectsPath = null;

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

            //get plugin type for each plugin
            List<short> allPluginTypes = new List<short>();
            
            foreach(DotSpatial.Extensions.IExtension ext in App.Extensions)
            {
                if (ext is IPlugin)
                {
                    IPlugin plugType = (IPlugin)ext;

                    //store pluginType
                    short PType = plugType.PluginType;
                    allPluginTypes.Add(PType);
                }
            }

            //if PType is smallest (datasheet/map), set as activated when open
            int pos = allPluginTypes.IndexOf(allPluginTypes.Min());
            DotSpatial.Extensions.IExtension extension = App.Extensions.ElementAt(pos);
            IPlugin ex = (IPlugin)extension;
            ex.MakeActive();

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
            string fullName = string.Empty;
            if (ProjectPathName == null)
            {
                string filterstring = @"VB2 Project Files|*.vbpx|All Files|*.*";
                SaveFileDialog saveFile = new SaveFileDialog();
                //saveFile.InitialDirectory = Application.ExecutablePath + "//ProjectFiles";
                saveFile.Title = "Enter a file name to save your project information in.";
                saveFile.Filter = filterstring;
                saveFile.FilterIndex = 1;
                saveFile.RestoreDirectory = true;
                DialogResult dr = saveFile.ShowDialog();
                //if (dr != DialogResult.OK)
                //{ return; }

                strPathName = saveFile.FileName;

                //_projMgr.Save(fullName, Globals.ProjectType.COMPLETE);
            }
            //else
            //{
            //    _projMgr.Save();
            //}

            
            Dictionary<string, object> pluginStates = new Dictionary<string, object>();
            signaller.RaiseSaveRequest(pluginStates);

            //JSON
            string json = JsonConvert.SerializeObject(pluginStates);
            StreamWriter sw = new StreamWriter(json.);
            sw.Close();
            
            
            //serialize all the plugins here.
            //XmlSerializer serializerDict = new XmlSerializer(typeof(Dictionary<string, object>));
            //StringWriter sw = new StringWriter();
            //serializerDict.Serialize(sw, pluginStates);

            //dataContract version
            //FileStream writer = new FileStream(strPathName, FileMode.CreateNew);
            //DataContractSerializer serializer = new DataContractSerializer(typeof(SerializableDictionary<string, object>));
            //serializer.WriteObject(writer, pluginStates);
            //writer.Close();

        }


        public void Open(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            //openFile.InitialDirectory = Application.ExecutablePath + "//ProjectFiles";
            openFile.InitialDirectory = VB2projectsPath;
            openFile.Filter = @"VB3 Project Files|*.vbpx|All Files|*.*";
            openFile.FilterIndex = 1;
            openFile.RestoreDirectory = true;
            string fullName = string.Empty;
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                //get a new project manager for this project
                //_projMgr = null;
                //_projMgr = VBProjectManager.GetProjectManager();

                fullName = openFile.FileName;
                //_projMgr.Open(fullName);
                
            }
            
            //Load a project file from disk and then send it out to be unpacked.
            
            Dictionary<string, object> pluginStates = new Dictionary<string, object>();
            
            //JSON
            
            StreamReader sr = new StreamReader(fullName);
            string json = sr.ReadToEnd();


            pluginStates = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            sr.Close();
            
            //deserialize all the plugins here.
            //XmlSerializer deserializerDict = new XmlSerializer(typeof(SerializableDictionary<string, object>));
            //StringReader sr = new StringReader();

            //deserializerDict.Deserialize(sr, .Serialize(sr, pluginStates);
            //XmlDeserializer deserializer = new XmlDeserializer();
            //pluginStates = deserializer.Deserialize(fullName) as SerializableDictionary<string, object>;
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


        public string ProjectPathName
        {
            get { return strPathName; }
            set { strPathName = value; }
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
            signaller.ProjectOpened += new VBTools.Signaller.SerializationEventHandler<VBTools.SerializationEventArgs>(ProjectOpenedListener); //loop through plugins ck for min pluginType to make that active when plugin opened.
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
