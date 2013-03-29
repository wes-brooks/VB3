using System;
using System.Collections.Generic;
using System.Text;

namespace GALib
{
    public interface ISelector
    {
        int Select(List<IIndividual> list);
    }
}
