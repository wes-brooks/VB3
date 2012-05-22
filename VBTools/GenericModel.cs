using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml;
using System.Data;
using System.IO;


namespace VBTools
{
    /// <summary>
    /// Make a generic model portable by (de)serialization - uses DataContract serialization interface.
    /// 
    /// Serialization:
    ///     Instantiate with a beach name and then use class accessors to set data types necessary 
    ///     for Virtual Beach model prediction (all public properties), then call the serialize method; 
    ///     returns an xml formatted string. Example:
    ///                 GenericModel m1 = new GenericModel("testBeach");
    ///                 m1.ModelDic = d0; (where, for axample, Dictionary<string, double> d0 = _proj.Model)
    ///                 ...
    ///                 string xmls = m1.Serialize(m1);
    /// Deserialization:
    ///     Instantiate with a null beachname and pass the serialized xml string to the deserialize
    ///     method as type T = "GenericModel".  Returns a Generic or Generic.object where 
    ///     object is any of the GenericModel public members.  Example:
    ///            GenericModel m4 = new GenericModel(null);
    ///            Dictionary<string, double> modeldic = m4.Deserialize<MLRModel>(xmls).ModelDic;
    ///            ...
    ///            DataTable modeldataDT = m4.Deserialize<MLRModel>(xmls).ModelDT;
    /// </summary>
    [DataContract(Name = "GenericModel", Namespace = "VBTools")]
    public class GenericModel : IExtensibleDataObject
    {
        [DataMember(Name = "BeachName")]
        public string _beachName;

        private static GenericModel _model = null;

        //class constructor; pass a beachname or model identifier, null for deserialization
        protected GenericModel(/* string newbeach */)
        {
            //_beachName = newbeach;
        }

        public static GenericModel getMLRModel()
        {
            if (_model == null) _model = new GenericModel(/* "temp" */);
            return _model;
        }

        public static GenericModel intGenericModel()
        {
            _model = null;
            return _model;
        }

        public static void setGenericModel(GenericModel m)
        {
            _model = m;
        }

        # region serializable properties

        public string BeachName
        {
            set { _beachName = value; }
            get { return _beachName; }
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

