using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;

namespace MultipleLinearRegression
{
    public class DataStore
    {
        private static DataTable _dt = null;
        private static List<string> _fieldList = null;

        public static void SetDataTable(DataTable dt)
        {
            _dt = dt;            
        }
        public static DataTable GetDataTable()
        {
            if (_dt == null)
                throw new Exception("MLRIndividual DataTable is null.");
            else
                return _dt;
        }

        public static void SetFieldList(List<string> fieldList)
        {
            _fieldList = fieldList;
        }
        public static List<string> GetFieldList()
        {
            if (_fieldList == null)
                throw new Exception("MLRIndividual DataTable is null.");
            else
                return _fieldList;
        }

    }
}
