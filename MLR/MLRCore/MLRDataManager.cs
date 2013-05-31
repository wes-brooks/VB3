using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Data;
using System.Xml;
using System.Xml.XPath;
using System.Windows.Forms;
using VBCommon;
//using Serialization;
//using MultipleLinearRegression;


namespace MLRCore
{
    public class MLRDataManager
    {
        private string _projectName = "";
        public Globals.ProjectType _projectType;
        
        private MLRModelingInfo _modelingInfo = null;
        private ResidualAnalysisInfo _residualAnalysisInfo = null;        
 
        private static MLRDataManager _dataMgr = null;               
                      
        private DataTable _correlationData = null;
        private DataTable _modelData = null;   
       
        private List<string> _modelFieldList = null;
        private string _dependentVariable = "";
        
        private Dictionary<string, double> _model = null;
        private bool _modelRunning = false;       
                       
        private MLRDataManager() { }


        /// <summary>
        /// Returns global instance of the VBProjectManager
        /// </summary>
        /// <returns></returns>
        public static MLRDataManager GetDataManager()
        {
            if (_dataMgr == null)
                _dataMgr = new MLRDataManager();

            return _dataMgr;
        }


        public string ModelDependentVariable
        {
            get { return _dependentVariable; }
        }


        public DataTable CorrelationDataTable
        {
            get { return _correlationData; }
            set { _correlationData = value; }
        }


        public DataTable ModelDataTable
        {
            get
            {
                DataTable dt = _modelData.Copy();
                return dt;
            }
            set
            {
                _modelData = value;
                _modelFieldList = new List<string>();

                List<string> lstFieldList = new List<string>();
                for (int i = 1; i < _modelData.Columns.Count; i++)
                {
                    bool bDependentVariableColumn = false;

                    if (_modelData.Columns[i].ExtendedProperties.ContainsKey(VBCommon.Globals.DEPENDENTVAR))
                    {
                        if (_modelData.Columns[i].ExtendedProperties[VBCommon.Globals.DEPENDENTVAR].ToString() == "True")
                        {
                            bDependentVariableColumn = true;
                            _dependentVariable = _modelData.Columns[i].ColumnName;
                        }
                    }

                    if (!bDependentVariableColumn && _modelData.Columns[i].ExtendedProperties.ContainsKey(VBCommon.Globals.ENABLED))
                    {
                        if (_modelData.Columns[i].ExtendedProperties[VBCommon.Globals.ENABLED].ToString() == "True")
                            _modelFieldList.Add(_modelData.Columns[i].ColumnName);
                    }
                    else if (!bDependentVariableColumn)
                        _modelFieldList.Add(_modelData.Columns[i].ColumnName);
                }
            }
        }
        

        /// <summary>
        /// The list of independent variables in the current model
        /// Associated with the ModelDataTable
        /// </summary>
        public List<string> ModelFieldList
        {
            get { return _modelFieldList; }
        }
        

        /*/// <summary>
        /// Given an array of indices, return the corresponding field names from the model data table. 
        /// </summary>
        /// <param name="variableIdxArray"></param>
        /// <returns></returns>
        public string[] GetIndependentVariableList(List<string> variableIdxArray)
        {
            List<string> names = new List<string>();

            int sumGenes = 0;

            for (int i = 0; i < variableIdxArray.Count; i++)
            {
                if (variableIdxArray[i] != "")
                {
                    names.Add(variableIdxArray[i]);
                }
            }

            if (sumGenes < 1)
                return null;

            return names.ToArray();
        }*/
      

        public Dictionary<string, double> Model
        {
            get { return _model; }
            set { _model = value; }
        }


        public bool ModelRunning
        {
            set { _modelRunning = value; }
            get { return _modelRunning; }
        }


        public MLRModelingInfo ModelingInfo
        {
            get { return _modelingInfo; }
            set { _modelingInfo = value; }
        }


        public ResidualAnalysisInfo ResidualAnalysisInfo
        {
            set { _residualAnalysisInfo = value; }
            get { return _residualAnalysisInfo; }
        }        
    }
}
