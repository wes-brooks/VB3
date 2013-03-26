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
            bool bPredictionOnly = false;
            if (sender.ToString() == "PredictionOnly")
                bPredictionOnly = true;

            string fullName = string.Empty;
            if (ProjectPathName == null)
            {
                //set up location for saved project to be stored
                string strFilterstring;
                if (bPredictionOnly) { strFilterstring = @"VB3 Model Files|*.vbmx|All Files|*.*"; }
                else { strFilterstring = @"VB3 Project Files|*.vbpx|All Files|*.*"; }

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

            string strProjectStateJson = PersistentStackUtilities.StatesToString(dictPluginStates);
            File.WriteAllText(strPathName, strProjectStateJson);

            logger.LogEvent(String.Format("Project File Name: {0}", strProjectName), Globals.messageIntent.UserOnly, Globals.targetSStrip.StatusStrip1);
            logger.LogEvent(String.Format("Project saved to {0}", strProjectName), Globals.messageIntent.LogFileOnly, Globals.targetSStrip.None);
        }


        public void SaveAs(object sender, EventArgs e)
        {
            ProjectPathName = null;
            Save(sender, e);
        }


        public void SaveAs_PredictionOnly(object sender, EventArgs e)
        {
            ProjectPathName = null;
            Save("PredictionOnly", e);
        }
        

        public void Open(object sender, EventArgs e)
        {
            //open project
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.InitialDirectory = VBProjectsPath;
            openFile.Filter = @"All VB3 Files|*.vbmx;*.vbpx|VB3 Project Files|*.vbpx|VB3 Model Files|*.vbmx|All Files|*.*";
            openFile.FilterIndex = 1;
            openFile.RestoreDirectory = true;
            string strFileName = string.Empty;

            //Get the name of the selected file or return w/o doing anything if no file was selected.
            if (openFile.ShowDialog() == DialogResult.OK)
                strFileName = openFile.FileName;
            else return;
            strPathName = strFileName;
            
            //If the file name ends in .vbmx then open this model for prediction only.
            bool bPredictionOnly;
            if (String.Equals(strFileName.Split('.').Last(), "vbmx", comparisonType: StringComparison.InvariantCultureIgnoreCase))
                { bPredictionOnly = true; }
            else { bPredictionOnly = false; }
            
            //Load a project file from disk and then send it out to be unpacked.           
            StreamReader streamreader = new StreamReader(strFileName);
            JsonTextReader jsonreader = new JsonTextReader(streamreader);
            string strProjectStateJson = streamreader.ReadToEnd();
            streamreader.Close();
            jsonreader.Close();

            IDictionary<string, IDictionary<string, object>> dictPluginStates = PersistentStackUtilities.StringToStates(strProjectStateJson);

            //raise unpack event, sending packed plugins dictionary
            signaller.UnpackProjectState(dictPluginStates, bPredictionOnly);

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
            UndoKeys.Clear();
            RedoKeys.Clear();
            signaller.TriggerUndoStack();

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


        private void PushToStack(object sender, UndoRedoEventArgs args)
        {
            if (boolChanged)
            {
                IDictionary<string, object> dictPackedState = PackState();

                string strKey = PersistentStackUtilities.RandomString(10);
                args.Store.Add(strKey, dictPackedState);
                UndoKeys.Push(strKey);
                RedoKeys.Clear();
                boolChanged = false;
            }
            else
            {
                try
                {
                    string strKey = UndoKeys.Peek();
                    UndoKeys.Push(strKey);
                    RedoKeys.Clear();
                }
                catch
                {
                    IDictionary<string, object> dictPackedState = PackState();
                    string strKey = PersistentStackUtilities.RandomString(10);
                    args.Store.Add(strKey, dictPackedState);
                    UndoKeys.Push(strKey);
                    RedoKeys.Clear();
                    boolChanged = false;
                }
            }
        }


        private void Undo(object sender, UndoRedoEventArgs args)
        {
            try
            {
                string strCurrentKey = UndoKeys.Pop();
                string strPastKey = UndoKeys.Peek();
                RedoKeys.Push(strCurrentKey);

                if (strCurrentKey != strPastKey)
                {
                    IDictionary<string, object> dictPlugin = args.Store[strPastKey];
                    this.UnpackState(dictPlugin);
                }
            }
            catch { }
        }


        private void Redo(object sender, UndoRedoEventArgs args)
        {
            try
            {
                string strCurrentKey = UndoKeys.Peek();
                string strNextKey = RedoKeys.Pop();
                UndoKeys.Push(strNextKey);

                if (strCurrentKey != strNextKey)
                {
                    IDictionary<string, object> dictPlugin = args.Store[strNextKey];
                    this.UnpackState(dictPlugin);
                }
            }
            catch { }
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
