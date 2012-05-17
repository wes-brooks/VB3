using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VBTools
{
    public interface IFormState
    {
        void UnpackState(object objPackedStates);
        string Name {get;}
    }
}
