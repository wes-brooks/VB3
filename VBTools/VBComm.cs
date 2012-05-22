using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VBTools
{
    //class provides message passing communication functionality
    //for the various VB projects by providing event/event handlers
    //to be invoked by the project manager.


    public class VBComm
    {
        private static volatile VBComm me = null;
        private static object syncRoot = new Object();

        public delegate void getMessageEventHandler(String message, object sender);
        public event getMessageEventHandler getMessageEvent;
        public delegate void sendMessageEventHandler(String message, object sender);
        public event sendMessageEventHandler sendMessageEvent;

        public static VBComm getComm()
        {
            if (me == null)
            {
                lock (syncRoot)
                {
                    if (me == null)
                    {
                        me = new VBComm();
                    }
                }
            }
            return me;
        }


        public void addMessageGetHandler(getMessageEventHandler handler)
        {
            this.getMessageEvent += handler;
        }


        public void addMessageSendHandler(sendMessageEventHandler handler)
        {
            this.sendMessageEvent += handler;
        }


        private void OnGetMessageEvent(String message, object sender)
        {
            if (getMessageEvent != null)
            {
                getMessageEvent(message, sender);
            }
        }


        private void OnSendMessageEvent(String message, object sender)
        {
            if (sendMessageEvent != null)
            {
                sendMessageEvent(message, sender);
            }
        }

        public void getMessage(String message, object sender)
        {
            OnGetMessageEvent(message, sender);
        }
        public void sendMessage(String message, object sender)
        {
            OnSendMessageEvent(message, sender);
        }
    }
}
