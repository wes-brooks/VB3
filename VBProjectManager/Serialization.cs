using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Data;
using System.Xml;
using System.Xml.XPath;
using System.Windows.Forms;
using Serialization;
using VBTools;


namespace VBProjectManager
{
    public partial class VBProjectManager
    {
        /*public void Open(string projectFile)
        {
            SerializableDictionary<string, object> dictPackedState = new SerializableDictionary<string, object>();

            XmlDeserializer deserializer = new XmlDeserializer();

            dictPackedState = deserializer.Deserialize(projectFile) as SerializableDictionary<string, object>;
            
            //get the filename to come over right away
            FileInfo fi = new FileInfo(projectFile);
            //projectmanager.ProjectName = fi.Name;

            signaller.UnpackProjectState(dictPackedState);
        }


        public void Save(string projectFile)
        {
            SerializableDictionary<string, object> dictPackedStates = new SerializableDictionary<string, object>();

            signaller.RaiseSaveRequest(dictPackedStates);

            FileInfo _fi = new FileInfo(projectFile);
            //projectmanager.ProjectName = _fi.Name;

            XmlSerializer serializerDict = new XmlSerializer();
            serializerDict.Serialize(dictPackedStates, projectFile);
        }*/
        

        private void ProjectSavedListener(object sender, VBTools.SerializationEventArgs e)
        {
            e.PackedPluginStates.Add(strPluginKey, PackState());
        }


        private void ProjectOpenedListener(object sender, VBTools.SerializationEventArgs e)
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


        public object PackState()
        {
            return "Returned from VBProjectManager.PackProjectState().";
        }


        public void UnpackState(object objPackedState)
        {
            //This function restores the previously packed state of the Project Manager.
        }
    }
}
