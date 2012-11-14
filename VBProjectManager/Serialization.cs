﻿using System;
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
        public void Save(object sender, EventArgs e)
        {
            string fullName = string.Empty;
            if (ProjectPathName == null)
            {
                //set up location for saved project to be stored
                string strFilterstring = @"VB3 Project Files|*.vbpx|All Files|*.*";
                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.Title = "Enter a file name to save your project information in.";
                saveFile.Filter = strFilterstring;
                saveFile.FilterIndex = 1;
                saveFile.RestoreDirectory = true;

                DialogResult dr = saveFile.ShowDialog();
                if (dr != DialogResult.OK)
                { return; }

                strPathName = saveFile.FileName;
                FileInfo fi = new FileInfo(strPathName);
                strProjectName = fi.Name;
            }

            //Dictionary to store each plugin's state for saving
            IDictionary<string, IDictionary<string, object>> dictPluginStates = new Dictionary<string, IDictionary<string, object>>();
            signaller.RaiseSaveRequest(dictPluginStates);

            Dictionary<string, object> dictProjectState = new Dictionary<string, object>();

            //loop through each plugin in the dictionary of plugins
            foreach (KeyValuePair<string, IDictionary<string, object>> plugin in dictPluginStates)
            {
                Dictionary<string, object> dictJsonRep = new Dictionary<string, object>();
                Dictionary<string, Type> dictObjectType = new Dictionary<string, Type>();
                List<object> lstContainer = new List<object>();
                string strPluginKey = plugin.Key;
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
                            dictJsonRep.Add(element.Key, jsonRepresentation);
                            dictObjectType.Add(element.Key, objType);
                        }
                        catch { }
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


        public void SaveAs(object sender, EventArgs e)
        {
            ProjectPathName = null;
            Save(sender, e);
        }


        public void Open(object sender, EventArgs e)
        {
            //open project
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.InitialDirectory = VBProjectsPath;
            openFile.Filter = @"VB3 Project Files|*.vbpx|All Files|*.*";
            openFile.FilterIndex = 1;
            openFile.RestoreDirectory = true;
            string strFileName = string.Empty;

            //Get the name of the selected file or return w/o doing anything if no file was selected.
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
                //Instantiate some objects for later use.
                Dictionary<string, string> dictJsonRep = new Dictionary<string, string>();
                Dictionary<string, Type> dictObjectType = new Dictionary<string, Type>();
                Dictionary<string, object> dictPluginState = new Dictionary<string, object>();

                //Convert the serialization string into an array of JSON objects
                string strPluginKey = plugin.Key;
                string strPluginStateJson = plugin.Value.ToString();
                Newtonsoft.Json.Linq.JArray jarray = (Newtonsoft.Json.Linq.JArray)JsonConvert.DeserializeObject(strPluginStateJson);

                //convert jarray to list in order to pull index for value and index for class type
                List<object> listContainer = jarray.ToObject<List<object>>();
                string strJsonDictJson = listContainer[0].ToString();
                string strObjectTypeDictJson = listContainer[1].ToString();

                //deserialize value and class type of value
                dictJsonRep = JsonConvert.DeserializeObject<Dictionary<string, string>>(strJsonDictJson);
                dictObjectType = JsonConvert.DeserializeObject<Dictionary<string, Type>>(strObjectTypeDictJson);

                //Convert the plugin's JSON into .NET objects and compile a dictionary of the deserialized objects.
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

            //raise unpack event, sending packed plugins dictionary
            signaller.UnpackProjectState(dictPluginStates);

            //Make the top plugin active
            if (dictPluginStates[this.strPluginKey]["TopPlugin"] != null)
            {
                foreach (DotSpatial.Extensions.IExtension x in App.Extensions)
                {
                    if (x is VBCommon.Interfaces.IPlugin)
                    {
                        if (((VBCommon.Interfaces.IPlugin)x).PanelKey == dictPluginStates[strPluginKey]["TopPlugin"].ToString())
                            ((VBCommon.Interfaces.IPlugin)x).MakeActive();
                    }
                }
            }

            //Reset the undo stack
            UndoStack.Clear();
            RedoStack.Clear();
            signaller.PushToUndoStack();
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
            dictPackedState.Add("TopPlugin", strTopPlugin);
            dictPackedState.Add("ProjectName", ProjectName);
            
            return dictPackedState;
        }

        
        //unpack plugin, assigning values from incoming dictionary
        public void UnpackState(IDictionary<string, object> dictPackedState)
        {  
            this.strTopPlugin = (string)dictPackedState["TopPlugin"];
            this.strPathName = (string)dictPackedState["ProjectName"];
        }
    }
}
