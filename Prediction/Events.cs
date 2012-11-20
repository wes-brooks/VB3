using System;
using System.Collections.Generic;

namespace Prediction
{
    //Defines the event arguments used when packing up a project to be saved
    public class ButtonStatusEventArgs : EventArgs
    {
        private IDictionary<string, bool> dictButtonStatus;
        private bool boolSetGet;

        public ButtonStatusEventArgs(IDictionary<string, bool> StatusDictionary, bool Set=false)
        {
            this.dictButtonStatus = StatusDictionary;
            boolSetGet = Set;
        }

        public bool Set
        {
            get { return boolSetGet; }
        }

        public IDictionary<string, bool> ButtonStatus
        {
            get { return dictButtonStatus; }
        }
    }
}