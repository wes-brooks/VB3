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
using Newtonsoft.Json;


namespace VBProjectManager
{

    public partial class VBProjectManager : Extension, IFormState, IPartImportsSatisfiedNotification
    {        
        private Dictionary<string, Boolean> _tabStates;
        private string strPathName;
        private string projectName;
        private VBCommon.Signaller signaller = new VBCommon.Signaller();
        private VBLogger logger;
        private string strLogFile;
        private string strPluginKey = "Project Manager";
        public static string VB2projectsPath = null;

        public VBProjectManager()
        {
            strLogFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VirtualBeach.log");
            VBLogger.SetLogFileName(strLogFile);
            logger = VBLogger.GetLogger();

            signaller = new VBCommon.Signaller();
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
            List<Globals.PluginType> allPluginTypes = new List<Globals.PluginType>();
            
            foreach(DotSpatial.Extensions.IExtension ext in App.Extensions)
            {
                if (ext is IPlugin)
                {
                    IPlugin plugType = (IPlugin)ext;

                    //store pluginType
                    Globals.PluginType PType = plugType.PluginType;
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
                FileInfo fi = new FileInfo(strPathName);
                projectName = fi.Name;
                //_projMgr.Save(fullName, Globals.ProjectType.COMPLETE);
            }
            //else {  _projMgr.Save(); }

            IDictionary<string, IDictionary<string, object>> pluginStates = new Dictionary<string, IDictionary<string, object>>();
            signaller.RaiseSaveRequest(pluginStates);

            //loop through plugins to get values and types
            Dictionary<string, object> dictProjectState = new Dictionary<string, object>();
           
            foreach (KeyValuePair<string,IDictionary<string,object>> plugin in pluginStates)
            { 
                Dictionary<string, object> dictJsonRep = new Dictionary<string, object>(); //holds jsonRepresented values
                Dictionary<string, object> dictObjRep = new Dictionary<string, object>(); //holds object types
                object[] arrayDictHolder = new object[2];  //holds all of the dictionaries
                string pluginKey = plugin.Key;
                IDictionary<string, object> pluginValue = new Dictionary<string, object>(); 
                pluginValue = plugin.Value;
                
                if (pluginValue == null) break;
                foreach (KeyValuePair<string, object> element in pluginValue)
                {
                    string jsonRepresentation = JsonConvert.SerializeObject(element.Value);
                    object objType = element.Value.GetType();
                    dictJsonRep.Add(element.Key, jsonRepresentation);
                    dictObjRep.Add(element.Key, objType);
                }
                arrayDictHolder[0] = dictJsonRep;
                arrayDictHolder[1] = dictObjRep;
                dictProjectState.Add(pluginKey, arrayDictHolder);
            }
            
            ////JSON
            JsonSerializer serializer = new JsonSerializer();

            using (StreamWriter sw = new StreamWriter(strPathName))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, dictProjectState);
            }
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
            IDictionary<string, IDictionary<string, object>> pluginStates = new Dictionary<string, IDictionary<string, object>>();
                       
            StreamReader sr = new StreamReader(fullName);
            JsonTextReader reader = new JsonTextReader(sr);
            string json = sr.ReadToEnd();
            var dictProjectState = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            sr.Close();
            reader.Close();
            
            foreach (var projectStatePair in dictProjectState)
            {
                string pluginKey = projectStatePair.Key; //key in pluginStates dict
                Dictionary<string, object> dictJsonRep = new Dictionary<string, object>();
                Dictionary<string, object> dictObjRep = new Dictionary<string, object>();
                Dictionary<string, object> pluginValues = new Dictionary<string, object>();
                string stpluginValue = projectStatePair.Value.ToString();
                
                Newtonsoft.Json.Linq.JArray ja = (Newtonsoft.Json.Linq.JArray)JsonConvert.DeserializeObject(stpluginValue);
                object[] arrayHolder = ja.ToObject<object[]>();
                string strArray0 = arrayHolder[0].ToString();
                string strArray1 = arrayHolder[1].ToString();

                dictJsonRep = JsonConvert.DeserializeObject<Dictionary<string, object>>(strArray0);
                dictObjRep = JsonConvert.DeserializeObject<Dictionary<string, object>>(strArray1);

                foreach (var pair in dictJsonRep)
                {
                    string strVariableKey = pair.Key;
                    object objType = dictObjRep[pair.Key];
                    string jsonRep = (string)pair.Value;

                    Type jsonType = Type.GetType(objType as string);
                    object jsonReprValue = null;

                    jsonReprValue = JsonConvert.DeserializeObject(jsonRep, jsonType);
                    pluginValues.Add(strVariableKey, jsonReprValue);
                }
                pluginStates.Add(pluginKey, pluginValues);
            }            
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


        public string ProjectName
        {
            get { return projectName; }
            set { projectName = value; }
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
            signaller.ProjectSaved += new VBCommon.Signaller.SerializationEventHandler<VBCommon.SerializationEventArgs>(ProjectSavedListener);
            signaller.ProjectOpened += new VBCommon.Signaller.SerializationEventHandler<VBCommon.SerializationEventArgs>(ProjectOpenedListener); //loop through plugins ck for min pluginType to make that active when plugin opened.
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
