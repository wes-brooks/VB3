using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml;
using System.Data;
using System.Xml.Serialization;
using System.IO;


namespace VBTools
{
    /// <summary>
    /// Make a MLR model portable by (de)serialization - uses DataContract serialization interface.
    /// 
    /// Serialization:
    ///     Instantiate with a beach name and then use class accessors to set data types necessary 
    ///     for VB MLR model prediction (all public properties), then call the serialize method; 
    ///     returns an xml formatted string. Example:
    ///                 MLRModel m1 = new MLRModel("testBeach");
    ///                 m1.ModelDic = d0; (where, for axample, Dictionary<string, double> d0 = _proj.Model)
    ///                 ...
    ///                 string xmls = m1.Serialize(m1);
    /// Deserialization:
    ///     Instantiate with a null beachname and pass the serialized xml string to the deserialize
    ///     method as type T = "MLRModel".  Returns a MLRModel or MLRModel.object where 
    ///     object is any of the MLRModel public members.  Example:
    ///            MLRModel m4 = new MLRModel(null);
    ///            Dictionary<string, double> modeldic = m4.Deserialize<MLRModel>(xmls).ModelDic;
    ///            ...
    ///            DataTable modeldataDT = m4.Deserialize<MLRModel>(xmls).ModelDT;
    /// </summary>
    [DataContract(Name = "MLRModel", Namespace = "VBTools")]
    public class MLRModel : IExtensibleDataObject
    {
        [DataMember(Name = "BeachName")]
        public string _beachName;

        [DataMember(Name = "MLRModelVariablesandCoefficientsDIC")]
        public Dictionary<string, double> _modeldic;

        [DataMember(Name = "PolynomialTransformCoefficientsDT")]
        public DataTable _polyinfoDT;

        [DataMember(Name = "ImportedDataDT")]
        public DataTable _inputDT;

        [DataMember(Name = "ModelDataDT")]
        public DataTable _modelDT;

        [DataMember(Name = "WindComponentVariableNamesLIST")]
        public List<string> _windComponentsColNames;

        [DataMember(Name = "CurrentComponentVariableNamesLIST")]
        public List<string> _currentComponentsColNames;

        [DataMember(Name = "BeachOrientation")]
        public double _beachOrientation;

        [DataMember(Name = "DecisionThreshold")]
        public double _decisionThreshold;

        [DataMember(Name = "RegulatoryThreshold")]
        public double _regulatoryThreshold;

        [DataMember(Name = "ModeledDependentVariable")]
        public string _depVarInfo;

        [DataMember(Name = "PredictiveRecordDT")]
        public DataTable _predictiveRecord;

        [DataMember(Name = "SelectedModelRegressionDT")]
        public DataTable _regressionDT; 

        private static MLRModel _model = null;

        //class constructor; pass a beachname or model identifier, null for deserialization
        private MLRModel(/* string newbeach */)
        {
            //_beachName = newbeach;
        }

        public static MLRModel getMLRModel()
        {
            if (_model == null) _model = new MLRModel(/* "temp" */);
            return _model;
        }

        public static MLRModel intMLRModel()
        {
            _model = null;
            return _model;
        }

        public static void setMLRModel(MLRModel m)
        {
            _model = m;
        }

        # region serializable properties

        public DataTable RegressionDataTable
        {
            set { _regressionDT = value; }
            get { return _regressionDT; }
        }

        public DataTable PredictiveRecord
        {
            set { _predictiveRecord = value; }
            get { return _predictiveRecord; }
        }

        public string BeachName
        {
            set { _beachName = value; }
            get { return _beachName; }
        }

        public string ModeledDependentVariable
        {
            set { _depVarInfo = value; }
            get { return _depVarInfo; }
        }

        public double DecisionThreshold
        {
            set { _decisionThreshold = value; }
            get { return _decisionThreshold; }
        }

        public double RegulatoryThreshold
        {
            set { _regulatoryThreshold = value; }
            get { return _regulatoryThreshold; }
        }

        public List<string> WindComponentsColNames
        {
            set { _windComponentsColNames = value; }
            get { return _windComponentsColNames; }
        }

        public List<string> CurrentComponentsColNames
        {
            set { _currentComponentsColNames = value; }
            get { return _currentComponentsColNames; }
        }

        public double BeachOrientation
        {
            set { _beachOrientation = value; }
            get { return _beachOrientation; }
        }

        public Dictionary<string, double> ModelDic
        {
            set { _modeldic = value; }
            get { return _modeldic; }
        }

        public DataTable PolyInfoDT
        {
            set { _polyinfoDT = value; }
            get { return _polyinfoDT; }
        }

        public DataTable InputDT
        {
            set { _inputDT = value; }
            get { return _inputDT; }
        }

        public DataTable ModelDT
        {
            set { _modelDT = value; }
            get { return _modelDT; }
        }

        #endregion

        #region datacontract interface members

        private ExtensionDataObject extensionData_Value;

        public ExtensionDataObject ExtensionData
        {
            get { return extensionData_Value; }
            set { extensionData_Value = value; }
        }

        #endregion


        //methods to generate what we want... portable xml strings for storage and objects for consumption

        //generic serializer; for MLRModel serialization thing = a populated MLRModel class instance.
        public string Serialize(IExtensibleDataObject thing)
        {
            MemoryStream ms = new MemoryStream();
            DataContractSerializer dcs = new DataContractSerializer(thing.GetType());
            dcs.WriteObject(ms, thing);
            return Encoding.UTF8.GetString(ms.ToArray());

        }

        //generic type deserializer; for MLRModel deserialization type T = "MLRModel", pass a 
        //string returned from Serialize().
        public T Deserialize<T>(string dcsString)
        {
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(dcsString));
            DataContractSerializer dcs = new DataContractSerializer(typeof(T));
            return (T)dcs.ReadObject(ms);
        }
    }
}

