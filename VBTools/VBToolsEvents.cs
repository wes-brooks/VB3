using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VBTools
{
    //creates event arguments for unpackaging project

    public class UnpackEventArgs : EventArgs
    {
        private string strKey;
        private object objValue;

        public UnpackEventArgs(string key, object value)
        {
            this.strKey = key;
            this.objValue = value;

        }

        //Public property to read the key/value ..and get them out
        public string Key
        {
            get { return strKey; }  
        }
        public object Value
        { 
            get { return objValue; }
        }
        
    }

    public class PackEventArgs : EventArgs
    {
        private SerializableDictionary<string, object> dictionaryPacked;

        public PackEventArgs (SerializableDictionary<string, object> dictPack)
        {
            this.dictionaryPacked = dictPack;
        }

        public SerializableDictionary<string, object> DictPacked
        {
            get { return dictionaryPacked; }
        }
    
    }
}
