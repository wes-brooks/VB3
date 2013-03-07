using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using VBCommon;


namespace VBProjectManager
{
    public class PluginStateDictionary : IDictionary<string, IDictionary<string, object>>
    {
        private IDictionary<string, string> dictBackend;
        public ICollection<string> Keys { get { return dictBackend.Keys; } }
        public bool ContainsKey(string key) { return dictBackend.ContainsKey(key); }
        public bool Contains(KeyValuePair<string, IDictionary<string, object>> kvp) { return dictBackend.ContainsKey(kvp.Key); }
        public bool Remove(string key) { return dictBackend.Remove(key); }
        public bool Remove(KeyValuePair<string, IDictionary<string, object>> kvp) { return dictBackend.Remove(kvp.Key); }
        public void Clear() { dictBackend.Clear(); }
        public void CopyTo(KeyValuePair<string, IDictionary<string, object>>[] kvp, int loc) { return; }
        public bool IsReadOnly { get { return dictBackend.IsReadOnly; } }
        public int Count { get { return dictBackend.Count; } }


        public IEnumerator<KeyValuePair<string, IDictionary<string, object>>> GetEnumerator()
        {
            IEnumerator<KeyValuePair<string, string>> ie = dictBackend.GetEnumerator();

            while (ie.MoveNext())
            {
                yield return new KeyValuePair<string, IDictionary<string, object>>(ie.Current.Key, PersistentStackUtilities.StringToState(ie.Current.Value));
            }
        }


        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            IEnumerator<KeyValuePair<string, string>> ie = dictBackend.GetEnumerator();

            while (ie.MoveNext())
            {
                yield return new KeyValuePair<string, IDictionary<string, object>>(ie.Current.Key, PersistentStackUtilities.StringToState(ie.Current.Value));
            }
        }


        public ICollection<IDictionary<string, object>> Values
        {
            get
            {
                ICollection<string> listValues = dictBackend.Values;
                List<IDictionary<string, object>> listOut = new List<IDictionary<string, object>>();

                foreach (string v in listValues)
                {
                    listOut.Add(PersistentStackUtilities.StringToState(v));
                }

                return listOut;
            }
        }


        public IDictionary<string, object> this[string key]
        {
            get { return PersistentStackUtilities.StringToState(dictBackend[key]); }
            set { dictBackend[key] = PersistentStackUtilities.StateToString(value); }
        }


        public void Add(string key, IDictionary<string, object> value)
        {
            dictBackend.Add(key, PersistentStackUtilities.StateToString(value));
        }


        public void Add(KeyValuePair<string, IDictionary<string, object>> kvp)
        {
            dictBackend.Add(kvp.Key, PersistentStackUtilities.StateToString(kvp.Value));
        }


        public bool TryGetValue(string key, out IDictionary<string, object> value)
        {
            string strState;
            bool result = dictBackend.TryGetValue(key, out strState);
            try
            {
                value = PersistentStackUtilities.StringToState(strState);
            }
            catch
            {
                value = null;
                result = false;
            }

            return result;
        }


        public PluginStateDictionary(IDictionary<string, string> Backend)
        {
            this.dictBackend = Backend;
        }
    }
}
