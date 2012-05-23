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


        /*protected void Unpack(string key, object value)
        {
            signaller.RaiseUnpackRequest(key, value);
        }*/


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
            foreach (DotSpatial.Extensions.IExtension extension in App.Extensions)
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


        public void Save(string projectFile)
        {
            SerializableDictionary<string, object> dictPackedStates = new SerializableDictionary<string, object>();

            signaller.RaiseSaveRequest(dictPackedStates);

            FileInfo _fi = new FileInfo(projectFile);
            //projectmanager.ProjectName = _fi.Name;

            XmlSerializer serializerDict = new XmlSerializer();
            serializerDict.Serialize(dictPackedStates, projectFile);
        }


        public object PackProjectState()
        {
            return "Returned from VBProjectManager.PackProjectState().";
        }
    }
}
