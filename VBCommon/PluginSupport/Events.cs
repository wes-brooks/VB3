using System;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VBCommon.PluginSupport
{
    //Defines the event arguments used when broadcasting plugin
    public class BroadcastEventArgs : EventArgs
    {
        private object plugin;
        private IDictionary<string, object> packedPlugin;

        public BroadcastEventArgs(object sender, IDictionary<string, object> dictBroadcast)
        {
            this.plugin = sender;
            this.packedPlugin = dictBroadcast;
        }

        public IDictionary<string, object> PackedPluginState
        {
            get { return packedPlugin; }
        }

        public object Sender
        {
            get { return plugin; }
        }
    }


    //Defines the event arguments used when broadcasting plugin
    public class CompositionCatalogRequestArgs : EventArgs
    {
        private VBCommon.Globals.PluginType typeTargeted;
        private System.ComponentModel.Composition.Hosting.AggregateCatalog catalog;

        public CompositionCatalogRequestArgs(System.ComponentModel.Composition.Hosting.AggregateCatalog catalog, VBCommon.Globals.PluginType typeTargeted)
        {
            this.catalog = catalog;
            this.typeTargeted = typeTargeted;
        }

        public VBCommon.Globals.PluginType Type
        {
            get { return typeTargeted; }
            set { typeTargeted = value; }
        }

        public System.ComponentModel.Composition.Hosting.AggregateCatalog Catalog
        {
            get { return catalog; }
            set { catalog = value; }
        }
    }


    //Defines the event arguments used when unpacking a project from its saved state
    public class UnpackEventArgs : EventArgs
    {
        private string strKey;
        private object objValue;

        public UnpackEventArgs(string key, object value)
        {
            this.strKey = key;
            this.objValue = value;
        }

        //Public property to read the key/value ..and get them out
        public string Key
        {
            get { return strKey; }
        }

        public object Value
        {
            get { return objValue; }
        }
    }


    //Defines the event arguments used when packing up a project to be saved
    public class SerializationEventArgs : EventArgs
    {
        private IDictionary<string, IDictionary<string, object>> packedStates;

        public SerializationEventArgs(IDictionary<string, IDictionary<string, object>> dictSerializable)
        {
            this.packedStates = dictSerializable;
        }

        public IDictionary<string, IDictionary<string, object>> PackedPluginStates
        {
            get { return packedStates; }
        }
    }


    //Defines the event arguments for messaging
    public class MessageArgs : EventArgs
    {
        private string strMessage;

        public MessageArgs(string message)
        {
            this.strMessage = message;
        }

        public string Message
        {
            get { return strMessage; }
        }
    }

    //public class PluginArgs : EventArgs
    //{
    //    private Globals.PluginType pType;
    //    private string sndr;

    //    public PluginArgs(string sender, Globals.PluginType plugType)
    //    {
    //        this.sndr = sender;
    //        this.pType = plugType;
    //    }

    //    public string Sender
    //    {
    //        get { return sndr; }
    //    }

    //    public Globals.PluginType PType
    //    {
    //        get { return pType; }
    //    }
    //}
}
