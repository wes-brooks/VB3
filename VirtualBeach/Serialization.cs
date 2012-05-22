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


namespace VirtualBeach
{
    public partial class ContainerForm
    {
        //events for when a project is opened
        public event ProjectOpenedHandler ProjectOpened;
        public delegate void ProjectOpenedHandler();

        public delegate void EventHandler<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
        public event EventHandler<UnpackEventArgs> UnpackRequest;

        //events for when a project is saved
        public delegate void ProjectSavedHandler<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
        public event ProjectSavedHandler<PackEventArgs> ProjectSaved;


        protected void RaiseUnpackRequest(string key, object value)
        {
            if (UnpackRequest != null)
            {
                UnpackEventArgs e = new UnpackEventArgs(key, value);
                UnpackRequest(this, e);
            }
        }


        public void Open(string projectFile)
        {
            Dictionary<string, object> dictPackedState = new Dictionary<string, object>();
            Dictionary<string, object> dictState = new Dictionary<string, object>();

            XmlDeserializer deserializer = new XmlDeserializer();

            dictPackedState = deserializer.Deserialize(projectFile) as Dictionary<string, object>;
            
            //get the filename to come over right away
            FileInfo fi = new FileInfo(projectFile);
            //projectmanager.ProjectName = fi.Name;

            //Send the packed states out to the same extensions that created them.
            foreach (DotSpatial.Extensions.IExtension extension in appManager.Extensions)
            {
                //test whether the extension implements a method to unpack its state
                IFormState VBPlugin = extension as IFormState;
                if (VBPlugin != null)
                {
                    if (dictState.ContainsKey(VBPlugin.Name))
                    {
                        VBPlugin.UnpackState(dictPackedState[VBPlugin.Name]);
                    }
                }
            }
        }


        private void ProjectSavedListener(object sender, PackEventArgs e)
        {
            //
        }

        
        public void Save()
        {
            //Save(projectmanager.ProjectName);
        }


        public void Save(string projectFile)
        {
            SerializableDictionary<string, object> dictPacked = new SerializableDictionary<string, object>();

            if (ProjectSaved != null) //something has been added to the list?
            {
                PackEventArgs e = new PackEventArgs(dictPacked);
                ProjectSaved(this, e);
            }

            FileInfo _fi = new FileInfo(projectFile);
            //projectmanager.ProjectName = _fi.Name;

            XmlSerializer serializerDict = new XmlSerializer();
            serializerDict.Serialize(dictPacked, projectFile);
        }        
    }
}
