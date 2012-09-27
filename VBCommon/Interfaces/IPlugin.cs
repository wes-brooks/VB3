using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VBCommon.Interfaces
{
    public interface IPlugin
    {
        //Each plugin must have following methods/properties
        Globals.PluginType PluginType { get; }
        string PanelKey { get; }
        void MakeActive();
        void Broadcast();
        Boolean Complete { get; }
        Boolean Visible { get; }
        void AddRibbon(string sender);
        //void AddPanel();
        void Hide();
        void Show();
<<<<<<< HEAD
        //void UndoLastChange(Dictionary<string,object>packedState);
=======
       
>>>>>>> a6b2f91fbf737b2a211f552c2c51c015a079edcb
    }
}
