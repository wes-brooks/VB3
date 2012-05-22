using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace VBTools
{
    [Serializable]
    public class ResidualAnalysisInfo
    {
        private Dictionary<int, string> _reBuildNType = null;
        private List<string> _modelIndependentVariables = null;
        private Dictionary<string, double> _model = null;
        private int _selectedRebuild = 0;
        //private DataTable _selectedModelDT = null;

        public ResidualAnalysisInfo()
        {}

        public Dictionary<int, string> ReBuildInfo
        {
            set { _reBuildNType = value; }
            get { return _reBuildNType; }
        }

        public List<string> ModelIndependentVariables
        {
            set {_modelIndependentVariables = value;}
            get {return _modelIndependentVariables;}
        }

        public Dictionary<string, double> Model
        {
            set { _model = value; }
            get { return _model; }
        }

        public int SelectedRebuild
        {
            set { _selectedRebuild = value; }
            get { return _selectedRebuild; }
        }

        //public DataTable SelectedModelDT
        //{
        //    set { _selectedModelDT = value; }
        //    get { return _selectedModelDT; }
        //}
    }
}
