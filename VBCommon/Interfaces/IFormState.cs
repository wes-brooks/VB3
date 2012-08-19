using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VBCommon.Interfaces
{
    public interface IFormState
    {
        //Each plugin's underlying class must have following methods/properties

        void UnpackState(IDictionary<string, object> objPackedState);
        IDictionary<string, object> PackState();
        string Name { get; }
    }
}
