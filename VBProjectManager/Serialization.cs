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
using VBCommon.PluginSupport;
using Newtonsoft.Json;


namespace VBProjectManager
{
    public partial class VBProjectManager
    {
        //save a project
        public void Save(object sender, EventArgs e)
        {
            string fullName = string.Empty;
            if (ProjectPathName == null)
            {
                //set up location for saved project to be stored
                string strFilterstring = @"VB2 Project Files|*.vbpx|All Files|*.*";
                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.Title = "Enter a file name to save your project information in.";
                saveFile.Filter = strFilterstring;
                saveFile.FilterIndex = 1;
                saveFile.RestoreDirectory = true;
                //save File dialog
                DialogResult dr = saveFile.ShowDialog();
                if (dr != DialogResult.OK)
                { return; }

                //save the file name and path
                strPathName = saveFile.FileName;
                FileInfo fi = new FileInfo(strPathName);
                strProjectName = fi.Name;
            }

            //Dictionary to store each plugin's state for saving
            IDictionary<string, IDictionary<string, object>> dictPluginStates = new Dictionary<string, IDictionary<string, object>>();
            //raise the save request sending dictionary to store plugin's state
            signaller.RaiseSaveRequest(dictPluginStates);

            //loop through plugins to get values and types
            Dictionary<string, object> dictProjectState = new Dictionary<string, object>();

            //loop through each plugin in the dictionary of plugins
            foreach (KeyValuePair<string, IDictionary<string, object>> plugin in dictPluginStates)
            {   
                //holds jsonRepresented values
                Dictionary<string, object> dictJsonRep = new Dictionary<string, object>(); 
                //holds object types
                Dictionary<string, Type> dictObjectType = new Dictionary<string, Type>(); 
                //holds all of the dictionaries
                List<object> lstContainer = new List<object>();  

                string strPluginKey = plugin.Key;                
                //holds the packed plugin
                IDictionary<string, object> dictPluginState = plugin.Value;

                if (dictPluginState == null) break;
                //loop through each element in the plugin to pull value and class type of each
                foreach (KeyValuePair<string, object> element in dictPluginState)
                {                    
                    if (element.Value != null)
                    {
                        try
                        {
                            string jsonRepresentation = JsonConvert.SerializeObject(element.Value);
                            Type objType = element.Value.GetType();
                            //add key and value
                            dictJsonRep.Add(element.Key, jsonRepresentation);
                            //add key and value's type
                            dictObjectType.Add(element.Key, objType);
                        }
                        catch {}
                    }
                    else
                    {
                        //if a plugin has a null value
                        dictJsonRep.Add(element.Key, "null");
                        dictObjectType.Add(element.Key, typeof(Nullable));
                    }
                }
                //add the values, value class type and then store in a dictionary
                lstContainer.Add(dictJsonRep);
                lstContainer.Add(dictObjectType);
                dictProjectState.Add(strPluginKey, lstContainer);
            }

            //JSON serialization of plugins dictionary
            JsonSerializer serializer = new JsonSerializer();

            using (StreamWriter sw = new StreamWriter(strPathName))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, dictProjectState);
            }
        }


        //listens for change in pluginKeyString
        public void strPluginTopChgdListener(VBCommon.PluginSupport.UpdateStrPlugOnTopEventArgs value)
        {
            this.strTopPlugin = value.PluginKeyString;
        }


        public void SaveAs(object sender, EventArgs e)
        {
            //save an opened project 
            string message = "Are you sure you want to replace this file?";
            string messageTitle = "Save As";

            //ask user if they want to replace file, and send them to Save
            DialogResult result = MessageBox.Show(message, messageTitle, MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                ProjectPathName = null;
                Save(sender, e);
            }
            else
                 return;
        }


        public void Open(object sender, EventArgs e)
        {
            //open project
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.InitialDirectory = VB2projectsPath;
            openFile.Filter = @"VB3 Project Files|*.vbpx|All Files|*.*";
            openFile.FilterIndex = 1;
            openFile.RestoreDirectory = true;
            string strFileName = string.Empty;
            //save file name
            if (openFile.ShowDialog() == DialogResult.OK)
                strFileName = openFile.FileName;
            else return;

            //Load a project file from disk and then send it out to be unpacked.
            IDictionary<string, IDictionary<string, object>> dictPluginStates = new Dictionary<string, IDictionary<string, object>>();

            StreamReader streamreader = new StreamReader(strFileName);
            JsonTextReader jsonreader = new JsonTextReader(streamreader);
            string strProjectStateJson = streamreader.ReadToEnd();
            var dictPackedProjectState = JsonConvert.DeserializeObject<Dictionary<string, object>>(strProjectStateJson);
            streamreader.Close();
            jsonreader.Close();

            //loop through plugins, deserialize each
            foreach (var plugin in dictPackedProjectState)
            { 
                //key in pluginStates dict
                string strPluginKey = plugin.Key;
                //hold value
                Dictionary<string, string> dictJsonRep = new Dictionary<string, string>();
                //hold class type of value
                Dictionary<string, Type> dictObjectType = new Dictionary<string, Type>();
                //hold plugin state with value/class type assigned
                Dictionary<string, object> dictPluginState = new Dictionary<string, object>();
                //store plugin elements as a string for deserializing
                string strPluginStateJson = plugin.Value.ToString();
                Newtonsoft.Json.Linq.JArray jarray = (Newtonsoft.Json.Linq.JArray)JsonConvert.DeserializeObject(strPluginStateJson);
                
                //convert jarray to list in order to pull index for value and index for class type
                List<object> listContainer = jarray.ToObject<List<object>>();
                string strJsonDictJson = listContainer[0].ToString();
                string strObjectTypeDictJson = listContainer[1].ToString();

                //deserialize value and class type of value
                dictJsonRep = JsonConvert.DeserializeObject<Dictionary<string, string>>(strJsonDictJson);
                dictObjectType = JsonConvert.DeserializeObject<Dictionary<string, Type>>(strObjectTypeDictJson);
                //loop through each pair and deserialize the value to it's class type
                foreach (var pair in dictJsonRep)
                {
                    Type objType = dictObjectType[pair.Key];
                    Type jsonType = objType;
                    string jsonRep = pair.Value;
                                       
                    object objDeserialized = JsonConvert.DeserializeObject(jsonRep, jsonType);
                    //add the newly constructed key value pair, containing correct class types to a dictionary
                    dictPluginState.Add(pair.Key, objDeserialized);
                }

                //add each plugin dictionary 
                dictPluginStates.Add(strPluginKey, dictPluginState);
            }

            //raise unpacke event, sending packed plugins dictionary
            signaller.UnpackProjectState(dictPluginStates);


            //Make the top plugin active
            foreach (DotSpatial.Extensions.IExtension x in App.Extensions)
            {
                if (x is VBCommon.Interfaces.IPlugin)
                {
                    if (((VBCommon.Interfaces.IPlugin)x).PanelKey.ToString() == openingTopPlugin)
                    {
                        //store plugin
                        VBCommon.Interfaces.IPlugin topPlugin = (VBCommon.Interfaces.IPlugin)x;
                        //just MakeActive() doesn't work.. makes the panel active, but doesn't show tab and ribbon
                        topPlugin.MakeActive();
                    }
                }
            }
        }


        //raise event to pack each plugin for saving and add to PackedPluginStates dictionary
        private void ProjectSavedListener(object sender, VBCommon.PluginSupport.SerializationEventArgs e)
        {
            e.PackedPluginStates.Add(strPluginKey, PackState());
        }


        //raise event to unpack each plugin, sending the plugin dictionary
        private void ProjectOpenedListener(object sender, VBCommon.PluginSupport.SerializationEventArgs e)
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

               
        //pack plugin for saving, returns packed state
        public IDictionary<string, object> PackState()
        {
            IDictionary<string, object> dictPackedState = new Dictionary<string, object>();
            dictPackedState.Add("TopPlugin", TopPlugin);
            dictPackedState.Add("ProjectName", ProjectName);
            
            return dictPackedState;
        }

        
        //unpack plugin, assigning values from incoming dictionary
        public void UnpackState(IDictionary<string, object> dictPackedState)
        {  
            //this.strTopPlugin = (string)dictPackedState["TopPlugin"];
            this.openingTopPlugin = (string)dictPackedState["TopPlugin"];
            this.strPathName = (string)dictPackedState["ProjectName"];
        }
    }
}
