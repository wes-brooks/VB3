using System;
using System.ComponentModel.Composition;
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

        //Request that plugins unpack their state
        public delegate void EventHandler<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
        public event EventHandler<UnpackEventArgs> UnpackRequest;

        public delegate void MessageHandler<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
        public event MessageHandler<MessageArgs> MessageReceived;

        public delegate void ActivePluginChangedHandler<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
        public event ActivePluginChangedHandler<DotSpatial.Controls.Docking.DockablePanelEventArgs> ActivePluginChangedEvent;

        public delegate void HeaderClickEventHandler<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
        public event HeaderClickEventHandler<DotSpatial.Controls.Header.RootItemEventArgs> HeaderClickEvent;

        public delegate void PluginMessageHandler<TArgs>(string sender, TArgs args) where TArgs : EventArgs;
        //public event PluginMessageHandler<PluginArgs> PluginMessageReceived;

        //event for broadcasting
        public delegate void BroadcastEventHandler<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
        public event BroadcastEventHandler<BroadcastEventArgs> BroadcastState;

        public event EventHandler PopulateUndoStackRequested;

        public Signaller() {}


        //tell plugins to pack their states into a dictionary to pass to other plugins
        public void RaiseBroadcastRequest(object sender, IDictionary<string, object> dictPackedPlugin)
        {
            if (BroadcastState != null) //has some method been told to handle this event?
            {
                string strSenderKey = "";
                try
                {
                    strSenderKey = ((VBCommon.Interfaces.IPlugin)sender).PanelKey;
                }
                catch { }
                finally
                {
                    if (!dictPackedPlugin.ContainsKey("Sender"))
                    {
                        dictPackedPlugin.Add("Sender", strSenderKey);
                    }
                    BroadcastEventArgs e = new BroadcastEventArgs(sender, dictPackedPlugin);
                    BroadcastState(sender, e);
                    
                }
            }
        }


        //Tell the plugins to pack their states into the dictionary for saving
        public void RaiseSaveRequest(IDictionary<string, IDictionary<string, object>> dictPackedStates, bool Undo=false, IDictionary<string, IDictionary<string, object>> Store=null)
        {
            if (ProjectSaved != null) //Has some method been told to handle this event?
            {
                SerializationEventArgs e = new SerializationEventArgs(dictPackedStates, Undo, Store);
                ProjectSaved(this, e);
            }
        }


        /*//Tell the plugins to pack their states into the undo stack
        public void SetUndoPoint(IDictionary<string, IDictionary<string, object>> dictPackedStates)
        {
            if (ProjectSaved != null) //Has some method been told to handle this event?
            {
                SerializationEventArgs e = new SerializationEventArgs(dictPackedStates);
                ProjectSaved(this, e);
            }
        }*/


        //Tell the plugins to unpack themselves from the saved state
        public void UnpackProjectState(IDictionary<string, IDictionary<string, object>> dictPackedStates, bool Undo=false)
        {
            if (ProjectOpened != null) //Has some method been told to handle this event?
            {
                SerializationEventArgs e = new SerializationEventArgs(dictPackedStates, Undo);
                ProjectOpened(this, e);
            }
        }


        /*//Raise the unpack event request so that each plugin can unpack it's state
        public void RaiseUnpackRequest(string key, object value)
        {
            if (UnpackRequest != null) //Has some method been told to handle this event?
            {
                UnpackEventArgs e = new UnpackEventArgs(key, value);
                UnpackRequest(this, e);
            }
        }*/


        [System.ComponentModel.Composition.Export("Signalling.PushToUndoStack")]
        public void PushToUndoStack()
        {
            if (PopulateUndoStackRequested != null)
            {
                EventArgs e = new EventArgs();
                PopulateUndoStackRequested(this, e);
            }
        }


        public void ActivePluginChanged(DotSpatial.Controls.Docking.DockablePanelEventArgs Args)
        {
            if (ActivePluginChangedEvent != null)
            {
                ActivePluginChangedEvent(this, Args);
            }
        }


        public void HeaderClicked(DotSpatial.Controls.Header.RootItemEventArgs Args)
        {
            if (HeaderClickEvent != null)
            {
                HeaderClickEvent(this, Args);
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
