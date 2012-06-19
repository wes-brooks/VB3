using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VBTools
{
    [Serializable]
    public class DataSheetInfo
    {
        private string _xlmDataSheetTable = string.Empty;
        private Dictionary<string, bool> _dtColInfo = null;
        private Dictionary<string, bool> _dtRowInfo = null;
        private string _depVarColName = string.Empty;
        private int _currentColIndex = -1;
        private bool _clean = true;
        private bool _validated = false;

        public DataSheetInfo()
        { }

        public string XmlDataSheetTable
        {
            set { _xlmDataSheetTable = value; }
            get { return _xlmDataSheetTable; }
        }

        public Dictionary<string, bool> DtColInfo
        {
            set { _dtColInfo = value; }
            get { return _dtColInfo; }
        }

        public Dictionary<string, bool> DtRowInfo
        {
            set { _dtRowInfo = value; }
            get { return _dtRowInfo; }
        }

        public string DepVarColName
        {
            set { _depVarColName = value; }
            get { return _depVarColName; }
        }

        public int CurrentColIndex
        {
            set { _currentColIndex = value; ;}
            get { return _currentColIndex; }
        }

        public bool Clean
        {
            set { _clean = value; }
            get { return _clean; }
        }

        public bool Validated
        {
            set { _validated = value; }
            get { return _validated; }
        }
    }
}
