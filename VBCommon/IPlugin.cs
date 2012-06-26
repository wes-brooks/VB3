using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VBCommon
{
    public interface IPlugin
    {
        Globals.PluginType PluginType {get;}
        string PanelKey { get; }
        void MakeActive();
        void Broadcast();
    }
}
