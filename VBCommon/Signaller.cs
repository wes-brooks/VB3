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
        public event SerializationEventHandler<UndoRedoEventArgs> UndoEvent;
        public event SerializationEventHandler<UndoRedoEventArgs> RedoEvent;
        public event SerializationEventHandler<UndoRedoEventArgs> UndoStackEvent;
        public event EventHandler TriggerUndoStackEvent;

        //Request that plugins unpack their state
        public delegate void EventHandler<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
        public event EventHandler<UnpackEventArgs> UnpackRequest;

        public delegate void MessageHandler<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
        public event MessageHandler<MessageArgs> MessageReceived;

        public delegate void ActivePluginChangedHandler<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
        public event ActivePluginChangedHandler<DotSpatial.Controls.Docking.DockablePanelEventArgs> ActivePluginChangedEvent;

        public delegate void HeaderClickEventHandler<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
        public event HeaderClickEventHandler<DotSpatial.Controls.Header.RootItemEventArgs> HeaderClickEvent;

        //event for broadcasting
        public delegate void BroadcastEventHandler<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
        public event BroadcastEventHandler<BroadcastEventArgs> BroadcastState;

        private bool bEnableBroadcast = true;


        public Signaller() {}


        //tell plugins to pack their states into a dictionary to pass to other plugins
        public void RaiseBroadcastRequest(object sender, IDictionary<string, object> dictPackedPlugin)
        {
            if (BroadcastState != null && bEnableBroadcast) //has some method been told to handle this event?
            {
                string strSenderKey = "";
                try
                {
                    strSenderKey = ((VBCommon.Interfaces.IPlugin)sender).PanelKey;
                }
                catch { }
                finally
                {
                    bEnableBroadcast = false;

                    if (!dictPackedPlugin.ContainsKey("Sender"))
                    {
                        dictPackedPlugin.Add("Sender", strSenderKey);
                    }

                    BroadcastEventArgs e = new BroadcastEventArgs(sender, dictPackedPlugin);
                    BroadcastState(sender, e);

                    bEnableBroadcast = true;
                }                
            }
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
        public void UnpackProjectState(IDictionary<string, IDictionary<string, object>> dictPackedStates, bool PredictionOnly)
        {
            if (ProjectOpened != null) //Has some method been told to handle this event?
            {
                bEnableBroadcast = false;

                SerializationEventArgs e = new SerializationEventArgs(dictPackedStates, PredictionOnly);
                ProjectOpened(this, e);
                
                bEnableBroadcast = true;
            }
        }


        public void SignalUndo(IDictionary<string, IDictionary<string, object>> Store = null)
        {
            if (UndoEvent != null) //Has some method been told to handle this event?
            {
                bEnableBroadcast = false;

                UndoRedoEventArgs e = new UndoRedoEventArgs(Store);
                UndoEvent(this, e);

                bEnableBroadcast = true;
            }
        }


        public void SignalRedo(IDictionary<string, IDictionary<string, object>> Store = null)
        {
            if (RedoEvent != null) //Has some method been told to handle this event?
            {
                bEnableBroadcast = false;

                UndoRedoEventArgs e = new UndoRedoEventArgs(Store);
                RedoEvent(this, e);

                bEnableBroadcast = true;
            }
        }


        public void TriggerUndoStack()
        {
            if (TriggerUndoStackEvent != null && bEnableBroadcast)
            {
                EventArgs e = new EventArgs();
                TriggerUndoStackEvent(this, e);
            }
        }


        public void UndoStack(IDictionary<string, IDictionary<string, object>> Store)
        {
            if (UndoStackEvent != null)
            {
                UndoRedoEventArgs e = new UndoRedoEventArgs(Store);
                UndoStackEvent(this, e);
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
