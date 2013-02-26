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

            string strProjectStateJson = Utilities.StatesToString(dictPluginStates);
            File.WriteAllText(strPathName, strProjectStateJson);

            logger.LogEvent(String.Format("Project File Name: {0}", strProjectName), Globals.messageIntent.UserOnly, Globals.targetSStrip.StatusStrip1);
            logger.LogEvent(String.Format("Project saved to {0}", strProjectName), Globals.messageIntent.LogFileOnly, Globals.targetSStrip.None);
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
            strPathName = strFileName; 
            
            //Load a project file from disk and then send it out to be unpacked.           
            StreamReader streamreader = new StreamReader(strFileName);
            JsonTextReader jsonreader = new JsonTextReader(streamreader);
            string strProjectStateJson = streamreader.ReadToEnd();
            streamreader.Close();
            jsonreader.Close();

            IDictionary<string, IDictionary<string, object>> dictPluginStates = Utilities.StringToStates(strProjectStateJson);

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

            logger.LogEvent(String.Format("Project File Name: {0}", strPathName), Globals.messageIntent.UserOnly, Globals.targetSStrip.StatusStrip1);
            logger.LogEvent(String.Format("Project opened from {0}", strPathName), Globals.messageIntent.LogFileOnly, Globals.targetSStrip.None);
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
            this.ProjectName = (string)dictPackedState["ProjectName"];
        }
    }
}
