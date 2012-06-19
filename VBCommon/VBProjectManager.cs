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
using Serialization;
//using MultipleLinearRegression;


namespace VBTools
{
    public class VBProjectManager : IFormState
    {
        //This guy holds references to all the project objects we want to save.
        private object[] _graph;

        private string _projectName = "";
        public Globals.ProjectType _projectType;

        private Site _siteInfo = null;
        private ModelingInfo _modelingInfo = null;
        private ResidualAnalysisInfo _residualAnalysisInfo = null;
        private ResidualAnalysisInfo _ipyResidualAnalysisInfo = null;
        private PredInfo _predInfo = null;
        private PredInfo _ipyPredInfo = null;
        private DataSheetInfo _datasheetInfo = null;
 
        private static VBProjectManager _projMgr = null;
        private string _cachePath = "";                
       
        public VBComm _comms = null;        

        private DataTable _importedData = null;
        private DataTable _correlationData = null;
        private DataTable _modelData = null;
        private DataTable _dataSheetData = null;
       
        private List<string> _modelFieldList = null;
        private string _dependentVariable = "";
        
        private Dictionary<string, double> _model = null;
        private List<string> _modelIndepentVariables = null;

        private bool _modelRunning = false;

        //Events for when the correlation data table is updated
        public event CorrelationDataTableUpdateHandler CorrelationDataTableUpdate;
        public CorrelationDataTableStatus e = null;
        public delegate void CorrelationDataTableUpdateHandler(VBProjectManager projMgr, CorrelationDataTableStatus e);

        //Events for when the model data table is updated
        public event ModelDataTableUpdateHandler ModelDataTableUpdate;
        public ModelDataTableStatus eModel = null;
        public delegate void ModelDataTableUpdateHandler(VBProjectManager projMgr, ModelDataTableStatus eModel);

        //events for when a project is opened
        public event ProjectOpenedHandler ProjectOpened;
        public delegate void ProjectOpenedHandler();

        public delegate void EventHandler<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
        public event EventHandler<UnpackEventArgs> UnpackRequest;

        //events for when a project is saved
        public delegate void ProjectSavedHandler<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
        public event ProjectSavedHandler<PackEventArgs> ProjectSaved;
        

        private TabStates _tabStates = null;
        
                       
        private VBProjectManager()
        {
            ProjectSaved += new VBProjectManager.ProjectSavedHandler<PackEventArgs>(ProjectSavedListener);

            String strPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            _cachePath = strPath + "\\cache";

            _siteInfo = null;
            _comms = new VBComm();            
            Model = new Dictionary<string, double>();
            _modelingInfo = new ModelingInfo();
            _residualAnalysisInfo = new ResidualAnalysisInfo();
            _predInfo = new PredInfo();
            _datasheetInfo = new DataSheetInfo();
            _tabStates = new TabStates();

            _graph = new object[7];
        }

        /// <summary>
        /// Returns global instance of the VBProjectManager
        /// </summary>
        /// <returns></returns>
        public static VBProjectManager GetProjectManager()
        {
            if (_projMgr == null)
                _projMgr = new VBProjectManager();

            return _projMgr;
        }


        public TabStates TabStates
        {
            get { return _tabStates; }
            set { _tabStates = value; }
        }
        

        public DataTable ImportedData
        {
            set { _importedData = value; }
            get { return _importedData; }
        }


        public DataTable CorrelationDataTable
        {
            get { return _correlationData; }
            set 
            { 
                _correlationData = value;

                if (CorrelationDataTableUpdate != null)
                    CorrelationDataTableUpdate(this, null);
            }
        }


        public DataTable ModelDataTable
        {
            get { return _modelData; }
            set
            {
                _modelData = value;

                if (ModelDataTableUpdate != null)
                {
                    _modelFieldList = new List<string>();

                    for (int i = 2; i < _modelData.Columns.Count; i++)
                        _modelFieldList.Add(_modelData.Columns[i].ColumnName);

                    _dependentVariable = _modelData.Columns[1].ColumnName;

                    ModelDataTableUpdate(this, null);
                }
            }
        }


        public DataTable DataSheetDataTable
        {
            set { _dataSheetData = value; }
            get { return _dataSheetData; }
        }
        

        /// <summary>
        /// The list if independent variables in the current model
        /// Associated with the ModelDataTable
        /// </summary>
        public List<string> ModelFieldList
        {
            get { return _modelFieldList; }
        }
        

        /// <summary>
        /// The dependent variable in the current model
        /// Associated with the ModelDataTable
        /// </summary>
        public string ModelDependentVariable
        {
            get { return _dependentVariable; }
        }


        /// <summary>
        /// The dependent variable in the current model
        /// Associated with the ModelDataTable
        /// </summary>
        public List<string> ModelIndependentVariables
        {
            get { return _modelIndepentVariables; }
            set { _modelIndepentVariables = value; }
        }


        /// <summary>
        /// Given an array of indices, return the corresponding field names from the model data table. 
        /// </summary>
        /// <param name="variableIdxArray"></param>
        /// <returns></returns>
        public string[] GetIndependentVariableList(List<short> variableIdxArray)
        {
            List<string> names = new List<string>();

            int sumGenes = 0;
            string field = "";
            int geneVal = 0;

            for (int i = 0; i < variableIdxArray.Count; i++)
            {
                if (variableIdxArray[i] > 0)
                {
                    sumGenes += variableIdxArray[i];
                    geneVal = variableIdxArray[i];
                    field = ModelFieldList[geneVal - 1];
                    //field = fieldList[geneVal];
                    names.Add(field);
                }
            }

            if (sumGenes < 1)
                return null;

            return names.ToArray();
        }
      

        /// <summary>
        /// Information describing specific site location
        /// </summary>
        public Site SiteInfo
        {
            get { return _siteInfo; }
            set {_siteInfo = value; }
        }


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

        //Raise a UnpackEvent 
        protected void RaiseUnpackRequest(string key, object value)
        {
            if (UnpackRequest != null) //something has been added to the list?
            {
                UnpackEventArgs e = new UnpackEventArgs(key, value);
                UnpackRequest(this, e);
            }
        }
        public void Open(string projectFile)
        {
            Dictionary<string, object> dictPackedState = new Dictionary<string, object>();
            Dictionary<string, object> dictUnpackedState = new Dictionary<string, object>();

            XmlDeserializer deserializer = new XmlDeserializer();

            dictPackedState = deserializer.Deserialize(projectFile) as Dictionary<string, object>;
            

            //handle project type right away so other methods can get data
            _projectType = (Globals.ProjectType)dictPackedState["_projectType"];

            

            //get the filename to come over right away
            FileInfo fi = new FileInfo(projectFile);
            _projectName = fi.Name;

            //adding value at specified key in dictionary
            dictUnpackedState["_projectType"] = _projectType;
            dictUnpackedState["frmLocation"] = _siteInfo;
            dictUnpackedState["VBProjectManager"] = _projMgr;
            dictUnpackedState["frmModel"] = _modelingInfo;
            dictUnpackedState["frmResiduals"] = _residualAnalysisInfo;
            dictUnpackedState["frmMLRPrediction"] = _predInfo;
            dictUnpackedState["frmDatasheet"] = _datasheetInfo;
            dictUnpackedState["frmIPyPrediction"] = _ipyPredInfo;
            dictUnpackedState["frmIPyResiduals"] = _ipyResidualAnalysisInfo;

            //assign key of packed state to key of unpacked state
            foreach (var pair in dictPackedState)
            {
                string _key = pair.Key;
                dictUnpackedState[_key] = dictPackedState[_key];
                RaiseUnpackRequest(pair.Key, pair.Value); //raises the event.
            }
            //ProjectOpened();
            
        }


        public void Save()
        {
            Save(_projectName, _projectType);
        }

        private void ProjectSavedListener(object sender, PackEventArgs e)
        {
            List<string> correlationDatasheet = new List<string>();
            StringWriter _sw = null;

            //Save CorrelatedData datatable
            string _xmlCorrelationData = "";
            if (_correlationData != null)
            {
                _correlationData.TableName = "CorrelationData";
                _sw = new StringWriter();
                _correlationData.WriteXml(_sw, XmlWriteMode.WriteSchema, false);
                _xmlCorrelationData = _sw.ToString();
                _sw.Close();
                _sw = null;
            }
            correlationDatasheet.Add(_xmlCorrelationData);
            
            //Save Datasheet info
            _sw = null;
            string _xmlDatasheet = "";
            if (_dataSheetData != null)
            {
                _dataSheetData.TableName = "DataSheetData";
                _sw = new StringWriter();
                _dataSheetData.WriteXml(_sw, XmlWriteMode.WriteSchema, false);
                _xmlDatasheet = _sw.ToString();
                _sw.Close();
                _sw = null;
                _datasheetInfo.XmlDataSheetTable = _xmlDatasheet;
            }
            correlationDatasheet.Add(_xmlDatasheet);

            if(_xmlCorrelationData != null || _datasheetInfo != null)
                e.DictPacked.Add("VBProjectManager", correlationDatasheet);
            else
                throw new Exception("Invalid data table used in Save.");
     
        }

        public void Save(string projectFile, Globals.ProjectType projectType)
        {
            
           SerializableDictionary<string, object> dictPacked = new SerializableDictionary<string, object>();

            dictPacked.Add("_projectType", projectType);

            if (ProjectSaved != null) //something has been added to the list?
            {
                PackEventArgs e = new PackEventArgs(dictPacked);
                ProjectSaved(this, e);
            }

            FileInfo _fi = new FileInfo(projectFile);
            _projectName = _fi.Name;
            
            XmlSerializer serializerDict = new XmlSerializer();
            serializerDict.Serialize(dictPacked, projectFile);
        }

        public void UnpackState(object objPackedStates)
        {
            List<string> localCorrDatasheet = new List<string>();
            localCorrDatasheet = (List<string>)objPackedStates;
            
            string _xmlCorrelationData = localCorrDatasheet[0];
            string _xmlDatasheetInfo = localCorrDatasheet[1];
            
            
            
            //_correlationData
            DataSet ds = null;
           
            if (_xmlCorrelationData != null)
                if (!String.IsNullOrEmpty(_xmlCorrelationData) || (_xmlCorrelationData != ""))
                {
                    _correlationData = new DataTable(_xmlCorrelationData);
                    ds = new DataSet();
                    ds.ReadXml(new StringReader(_xmlCorrelationData), XmlReadMode.ReadSchema);
                    _correlationData = ds.Tables[0].Copy();
                    ds.Dispose();
                }

            //datasheet
            if (!string.IsNullOrEmpty(_datasheetInfo.XmlDataSheetTable))
                {
                    _dataSheetData = new DataTable("DataSheetData");
                    ds = new DataSet();
                    ds.ReadXml(new StringReader(_xmlDatasheetInfo), XmlReadMode.ReadSchema);
                    _dataSheetData = ds.Tables[0].Copy();
                    ds.Dispose();
                }

        }
        
        public ModelingInfo ModelingInfo
        {
            get { return _modelingInfo; }
            set { _modelingInfo = value; }
        }

        public PredInfo PredictionInfo
        {
            get { return _predInfo; }
            set { _predInfo = value; }
        }

        public PredInfo IPyPredictionInfo
        {
            get { return _ipyPredInfo; }
            set { _ipyPredInfo = value; }
        }

        public string Name
        {
            get { return _projectName; }
            set { _projectName = value; }
        }
        
        public ResidualAnalysisInfo ResidualAnalysisInfo
        {
            set { _residualAnalysisInfo = value; }
            get { return _residualAnalysisInfo; }
        }

        public ResidualAnalysisInfo IPyResidualAnalysisInfo
        {
            set { _ipyResidualAnalysisInfo = value; }
            get { return _ipyResidualAnalysisInfo; }
        }

        public DataSheetInfo DataSheetInfo
        {
            set { _datasheetInfo = value; }
            get { return _datasheetInfo; }
        }
    }

    
    public class CorrelationDataTableStatus : EventArgs
    {
        private string _status;
        public string Status
        {
            set { _status = value; }
            get { return _status;  }
        }
    }

    public class ModelDataTableStatus : EventArgs
    {
        private string _status;
        public string Status
        {
            set { _status = value; }
            get { return _status; }
        }
    }

    public class ProjectChangedStatus : EventArgs
    {
        private string _status;
        public string Status
        {
            set { _status = value; }
            get { return _status; }
        }
    }
}
