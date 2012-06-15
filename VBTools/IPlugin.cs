using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VBTools
{
    public interface IPlugin
    {
        short PluginType {get;}
        string StrPanelKey { get; }
        void MakeActive();
        void Broadcast(IDictionary<string,object> packedState);
    }
}
