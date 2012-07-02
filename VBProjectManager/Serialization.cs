using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using System.Windows.Forms;
using VBCommon;
using Newtonsoft.Json;


namespace VBProjectManager
{
    public partial class VBProjectManager
    {
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

            foreach (KeyValuePair<string, IDictionary<string, object>> plugin in pluginStates)
            {                
                Dictionary<string, object> dictJsonRep = new Dictionary<string, object>(); //holds jsonRepresented values
                Dictionary<string, Type> dictObjectType = new Dictionary<string, Type>(); //holds object types
                List<object> listContainer = new List<object>();  //holds all of the dictionaries

                string strPluginKey = plugin.Key;                
                IDictionary<string, object> dictPluginState = plugin.Value;

                if (dictPluginState == null) break;
                foreach (KeyValuePair<string, object> element in dictPluginState)
                {                    
                    if (element.Value != null)
                    {
                        try
                        {
                            string jsonRepresentation = JsonConvert.SerializeObject(element.Value);
                            Type objType = element.Value.GetType();
                            dictJsonRep.Add(element.Key, jsonRepresentation);
                            dictObjectType.Add(element.Key, objType);
                        }
                        catch {}
                    }
                    else
                    {
                        dictJsonRep.Add(element.Key, "null");
                        dictObjectType.Add(element.Key, typeof(Nullable));
                    }
                }
                listContainer.Add(dictJsonRep);
                listContainer.Add(dictObjectType);
                dictProjectState.Add(strPluginKey, listContainer);
            }

            //JSON
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
            string strFileName = string.Empty;

            if (openFile.ShowDialog() == DialogResult.OK)
                strFileName = openFile.FileName;

            //Load a project file from disk and then send it out to be unpacked.
            IDictionary<string, IDictionary<string, object>> dictPluginStates = new Dictionary<string, IDictionary<string, object>>();

            StreamReader streamreader = new StreamReader(strFileName);
            JsonTextReader jsonreader = new JsonTextReader(streamreader);
            string strProjectStateJson = streamreader.ReadToEnd();
            var dictPackedProjectState = JsonConvert.DeserializeObject<Dictionary<string, object>>(strProjectStateJson);
            streamreader.Close();
            jsonreader.Close();

            foreach (var plugin in dictPackedProjectState)
            {
                string strPluginKey = plugin.Key; //key in pluginStates dict
                Dictionary<string, string> dictJsonRep = new Dictionary<string, string>();
                Dictionary<string, Type> dictObjectType = new Dictionary<string, Type>();
                Dictionary<string, object> dictPluginState = new Dictionary<string, object>();
                string strPluginStateJson = plugin.Value.ToString();

                Newtonsoft.Json.Linq.JArray jarray = (Newtonsoft.Json.Linq.JArray)JsonConvert.DeserializeObject(strPluginStateJson);
                List<object> listContainer = jarray.ToObject<List<object>>();
                string strJsonDictJson = listContainer[0].ToString();
                string strObjectTypeDictJson = listContainer[1].ToString();

                dictJsonRep = JsonConvert.DeserializeObject<Dictionary<string, string>>(strJsonDictJson);
                dictObjectType = JsonConvert.DeserializeObject<Dictionary<string, Type>>(strObjectTypeDictJson);

                foreach (var pair in dictJsonRep)
                {
                    Type objType = dictObjectType[pair.Key];
                    Type jsonType = objType;
                    string jsonRep = pair.Value;
                                       
                    object objDeserialized = JsonConvert.DeserializeObject(jsonRep, jsonType);
                    dictPluginState.Add(pair.Key, objDeserialized);
                }
                dictPluginStates.Add(strPluginKey, dictPluginState);
            }
            signaller.UnpackProjectState(dictPluginStates);
        }


        private void ProjectSavedListener(object sender, VBCommon.SerializationEventArgs e)
        {
            e.PackedPluginStates.Add(strPluginKey, PackState());
        }


        private void ProjectOpenedListener(object sender, VBCommon.SerializationEventArgs e)
        {
            if (e.PackedPluginStates.ContainsKey(strPluginKey))
            {
                this.UnpackState(e.PackedPluginStates[strPluginKey]);
            }
            else
            {
                //Set this plugin to an empty state.
            }
        }


       


        public IDictionary<string, object> PackState()
        {
            IDictionary<string, object> dictPackedState = new Dictionary<string, object>();

            dictPackedState.Add("ProjectName", ProjectName);
            //Add other information, like the order of plugins, and which ones are open.

            return dictPackedState;
        }

        
        public void UnpackState(IDictionary<string, object> dictPackedState)
        {  
            this.strPathName = (string)dictPackedState["ProjectName"];
        }
    }
}
