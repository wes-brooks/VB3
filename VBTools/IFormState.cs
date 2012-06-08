using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VBTools
{
    public interface IFormState
    {
        void UnpackState(IDictionary<string, object> objPackedState);
        IDictionary<string,object> PackState();

        //void ProjectSavedListener(object sender, PackEventArgs e);
        //void ProjectOpenedListener(object sender, UnpackEventArgs e);

        string Name {get;}
    }
}
