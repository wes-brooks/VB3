using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VBCommon
{
    //Defines the event arguments used when broadcasting plugin
    public class BroadCastEventArgs : EventArgs
    {
        private IDictionary<string, object> packedPlugin;

        public BroadCastEventArgs(IDictionary<string, object> dictBroadcast)
        {
            this.packedPlugin = dictBroadcast;
        }

        public IDictionary<string, object> PackedPluginState
        {
            get { return packedPlugin; }
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
}
