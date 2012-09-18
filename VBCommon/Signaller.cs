using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VBCommon.PluginSupport;

namespace VBCommon
{
    public class Signaller
    {
        //event for when a project is saved
        public delegate void SerializationEventHandler<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
        public event SerializationEventHandler<SerializationEventArgs> ProjectSaved;
        public event SerializationEventHandler<SerializationEventArgs> ProjectOpened;

        //Request that plugins unpack their state... why?
        public delegate void EventHandler<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
        public event EventHandler<UnpackEventArgs> UnpackRequest;

        //event for when a project is saved
        public delegate void MessageHandler<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
        public event MessageHandler<MessageArgs> MessageReceived;

        public delegate void PluginMessageHandler<TArgs>(string sender, TArgs args) where TArgs : EventArgs;
        //public event PluginMessageHandler<PluginArgs> PluginMessageReceived;

        //event for broadcasting
        public delegate void BroadCastEventHandler<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
        public event BroadCastEventHandler<BroadCastEventArgs> BroadcastState;

        //event for updating strPluginKey for which plugin should be on top when proj opened
        public delegate void UpdateStrPluginKey<TArgs>(TArgs args) where TArgs : EventArgs;
        public event UpdateStrPluginKey<UpdateStrPlugOnTopEventArgs> strPluginTopChanged;

        public delegate void HidePluginsHandler();
        public event HidePluginsHandler HideTabsEvent;

        public Signaller() {}


        //update the top plugin str key value for opening projects
        public void RaiseStrPluginChange(string value)
        {
            if (strPluginTopChanged != null)
            {
                UpdateStrPlugOnTopEventArgs e = new UpdateStrPlugOnTopEventArgs(value);
                strPluginTopChanged(e);
            }
        }
        //tell plugins to pack their states into a dictionary to pass to other plugins
        public void RaiseBroadcastRequest(object sender, IDictionary<string, object> dictPackedPlugin)
        {
            if (BroadcastState != null) //has some method been told to handle this event?
            {
                BroadCastEventArgs e = new BroadCastEventArgs(sender, dictPackedPlugin);
                BroadcastState(sender, e);
            }
        }


        //event for hiding plugins when datasheet plugin is selected
        public void HidePlugins()
        {
            if (HideTabsEvent != null)
                HideTabsEvent();
        }


        //Tell the plugins to pack their states into the dictionary for saving
        public void RaiseSaveRequest(IDictionary<string, IDictionary<string, object>> dictPackedStates)
        {
            if (ProjectSaved != null) //Has some method been told to handle this event?
            {
                SerializationEventArgs e = new SerializationEventArgs(dictPackedStates);
                ProjectSaved(this, e);
            }
        }


        //Tell the plugins to unpack themselves from the saved state
        public void UnpackProjectState(IDictionary<string, IDictionary<string, object>> dictPackedStates)
        {
            if (ProjectOpened != null) //Has some method been told to handle this event?
            {
                SerializationEventArgs e = new SerializationEventArgs(dictPackedStates);
                ProjectOpened(this, e);
            }
        }


        //Raise the unpack event request so that each plugin can unpack it's state
        public void RaiseUnpackRequest(string key, object value)
        {
            if (UnpackRequest != null) //Has some method been told to handle this event?
            {
                UnpackEventArgs e = new UnpackEventArgs(key, value);
                UnpackRequest(this, e);
            }
        }


        //Re-raise a message that was received from one of the plugins
        public void HandleMessage(object sender, MessageArgs args)
        {
            if (MessageReceived != null) //Has some method been told to handle this event?
            {
                MessageReceived(this, args);
            }
        }
    }
}
