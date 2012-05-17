namespace VBProjectManager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using VBTools;

    using DotSpatial.Controls;
    using DotSpatial.Controls.Header;

    public class VBProjectManager : Extension, IFormState
    {
        private Dictionary<string, Boolean> _tabStates;
        private string strName;

        public override void Activate()
        {
            App.HeaderControl.Add(new SimpleActionItem("My Button Caption", ButtonClick));
            base.Activate();
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            base.Deactivate();
        }

        public void ButtonClick(object sender, EventArgs e)
        {
            // Your logic goes here.
        }


        public Dictionary<string, Boolean> TabStates
        {
            get { return _tabStates; }
            set { _tabStates = value; }
        }


        public class ProjectChangedStatus : EventArgs
        {
            private string _status;
            public string Status
            {
                set { _status = value; }
                get { return _status; }
            }
        }


        public void PackState(object objPackedStates)
        {
            //This function packs the state of the Project Manager for saving to disk.
        }


        public void UnpackState(object objPackedStates)
        {
            //This function restores the previously packed state of the Project Manager.
        }


        public string ProjectName
        {
            get { return strName; }
            set { strName = value; }
        }
    }
}
