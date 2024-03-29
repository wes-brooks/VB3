﻿using System;
using System.Collections.Generic;
using System.Data;

namespace IPyModeling
{
    //This event is raised to pass a message to the container's logger.
    public class LogMessageEvent : EventArgs
    {
        private string strMessage;

        public enum Intents { LogFileOnly, UserOnly, UserAndLogFile };
        public enum Targets { None, StatusStrip1, StatusStrip2, StatusStrip3, ProgressBar }

        private Intents intent;
        private Targets target;


        public LogMessageEvent(string message, Intents intent, Targets target)
        {
            this.strMessage = message;
            this.intent = intent;
            this.target = target;
        }


        //Public property to read the message
        public string Message
        {
            get { return strMessage; }
        }


        //Override the ToString() method to return the message.
        public override string ToString()
        {
            return Message;
        }


        //Public property to read the intent
        public Intents Intent
        {
            get { return intent; }
        }


        //Public property to read the target
        public Targets Target
        {
            get { return target; }
        }
    }


    //This event is raised to pass a message to the container's project manager.
    public class MessageEvent : EventArgs
    {
        private string strMessage; //string for key, object for value

        public MessageEvent(string message)
        {
            this.strMessage = message;
        }


        //Public property to read the message ..and get the obj out
        public string Message
        {
            get { return strMessage; }
        }


        //Override the ToString() method to return the message.
        public override string ToString()
        {
            return Message;
        }
    }


    //This event is raised to pass a message to the container's project manager.
    public class ModelingCallback : EventArgs
    {
        public delegate void ModelingDelegate(DataTable Data);
        private ModelingDelegate funcRunModeling;

        public ModelingCallback(ModelingDelegate callback)
        {
            this.funcRunModeling = callback;
        }


        //Public property to read the message
        public void MakeModel(DataTable Data)
        {
            this.funcRunModeling(Data);
        }
    }
}