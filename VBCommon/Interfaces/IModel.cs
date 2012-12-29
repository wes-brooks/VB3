using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VBCommon.Interfaces
{
    public interface IModel
    {
        List<double> Predict(System.Data.DataSet data);
        List<double> PredictExceedanceProbability(System.Data.DataSet data);
        string ModelString();
        IDictionary<string, object> GetPackedState();
    }
}
