using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VBTools
{
    //class to track datasheet state:
    //track processing that has occured and if data saved in project

    public class DataSheetState
    {
        public enum CurrentState
        {
            imported,
            missingck,
            windproc,
            varspeced,
            interacted,
            transformed,
            manualproc,
            modeled
        };

        private CurrentState _state;

        public CurrentState State
        {
            set { _state = value; }
            get { return _state; }
        }

        private bool _saved = false;

        public bool Saved
        {
            set { _saved = value; }
            get { return _saved; }
        }



    }
}
