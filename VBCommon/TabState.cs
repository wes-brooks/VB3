using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VBTools
{
    public class TabStates
    {
        public Dictionary<string, bool> TabState = new Dictionary<string, bool>()
        {
            {"Location", true},
            {"DataSheet", true},
            {"Modeling", true},
            {"Residuals", true},
            {"IPyResiduals", true},
            {"Prediction", true},
            {"IPyPrediction", true},
        };
    }
}
