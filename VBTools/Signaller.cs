using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VBTools
{
    public class Signaller
    {
        //event for when a project is opened
        public delegate void ProjectOpenedHandler();
        public event ProjectOpenedHandler ProjectOpened;

        //event for when a project is saved
        public delegate void ProjectSavedHandler<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
        public event ProjectSavedHandler<PackEventArgs> ProjectSaved;

        //Request that plugins unpack their state... why?
        public delegate void EventHandler<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
        public event EventHandler<UnpackEventArgs> UnpackRequest;
        
        //event for when a project is saved
        public delegate void MessageHandler<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
        public event MessageHandler<MessageArgs> MessageReceived;

        public Signaller()
        {

        }


        public void RaiseSaveRequest(SerializableDictionary<string, object> dictPackedStates)
        {
            if (ProjectSaved != null) //Has some method been told to handle this event?
            {
                PackEventArgs e = new PackEventArgs(dictPackedStates);
                ProjectSaved(this, e);
            }
        }


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
