using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VBTools
{
    public interface IFormState
    {
        void UnpackState(object objPackedStates);
        object PackState();

        void ProjectSavedLisener(object sender, PackEventArgs e);
        void ProjectOpenedListener(object sender, UnpackEventArgs e);

        string Name {get;}
    }
}
