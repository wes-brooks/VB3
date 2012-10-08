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
        void Hide();
        void Show();
    }
}
