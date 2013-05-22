using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace VBCommon
{
    public static class PersistentStackUtilities
    {
        //Generate random strings for use as keys to the Undo/Redo dictionaries
        private static Random random = new Random((int)DateTime.Now.Ticks); //thanks to McAden

        public static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }


        public static string StatesToString(IDictionary<string, IDictionary<string, object>> Plugins)
        {
            IDictionary<string, IDictionary<string, object>> dictPluginStates = Plugins;
            Dictionary<string, object> dictProjectState = new Dictionary<string, object>();

            //loop through each plugin in the dictionary of plugins
            foreach (KeyValuePair<string, IDictionary<string, object>> plugin in dictPluginStates)
            {
                Dictionary<string, object> dictJsonRep = new Dictionary<string, object>();
                Dictionary<string, Type> dictObjectType = new Dictionary<string, Type>();
                List<object> lstContainer = new List<object>();
                string strPluginKey = plugin.Key;
                IDictionary<string, object> dictPluginState = plugin.Value;

                if (dictPluginState == null) break;

                //loop through each element in the plugin to pull value and class type of each
                foreach (KeyValuePair<string, object> element in dictPluginState)
                {
                    if (element.Value != null)
                    {
                        try
                        {
                            string jsonRepresentation = JsonConvert.SerializeObject(element.Value);
                            Type objType = element.Value.GetType();
                            dictJsonRep.Add(element.Key, jsonRepresentation);
                            dictObjectType.Add(element.Key, objType);
                        }
                        catch { }
                    }
                    else
                    {
                        //if a plugin has a null value
                        dictJsonRep.Add(element.Key, "null");
                        dictObjectType.Add(element.Key, typeof(Nullable));
                    }
                }
                //add the values, value class type and then store in a dictionary
                lstContainer.Add(dictJsonRep);
                lstContainer.Add(dictObjectType);
                dictProjectState.Add(strPluginKey, lstContainer);
            }

            //JSON serialization of plugins dictionary
            JsonSerializer serializer = new JsonSerializer();

            using (StringWriter sw = new StringWriter())
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, dictProjectState);
                return sw.ToString();
            }
        }


        public static string StateToString(IDictionary<string, object> Plugin)
        {
            Dictionary<string, Dictionary<string, object>> dictPluginState = new Dictionary<string, Dictionary<string, object>>();
            Dictionary<string, object> dictJsonRep = new Dictionary<string, object>();
            Dictionary<string, Type> dictObjectType = new Dictionary<string, Type>();
            List<object> lstContainer = new List<object>();

            if (Plugin == null) return null;

            //loop through each element in the plugin to pull value and class type of each
            foreach (KeyValuePair<string, object> element in Plugin)
            {
                if (element.Value != null)
                {
                    try
                    {
                        string jsonRepresentation = JsonConvert.SerializeObject(element.Value);
                        Type objType = element.Value.GetType();
                        dictJsonRep.Add(element.Key, jsonRepresentation);
                        dictObjectType.Add(element.Key, objType);
                    }
                    catch { }
                }
                else
                {
                    //if a plugin has a null value
                    dictJsonRep.Add(element.Key, "null");
                    dictObjectType.Add(element.Key, typeof(Nullable));
                }
            }
            //add the values, value class type and then store in a dictionary
            lstContainer.Add(dictJsonRep);
            lstContainer.Add(dictObjectType);


            //JSON serialization of plugins dictionary
            JsonSerializer serializer = new JsonSerializer();

            using (StringWriter sw = new StringWriter())
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, lstContainer);
                return sw.ToString();
            }
        }


        public static IDictionary<string, IDictionary<string, object>> StringToStates(string Serialized)
        {
            string strProjectStateJson = Serialized;
            var dictPackedProjectState = JsonConvert.DeserializeObject<Dictionary<string, object>>(strProjectStateJson);
            IDictionary<string, IDictionary<string, object>> dictPluginStates = new Dictionary<string, IDictionary<string, object>>();

            //loop through plugins, deserialize each
            foreach (var plugin in dictPackedProjectState)
            {
                //Instantiate some objects for later use.
                Dictionary<string, string> dictJsonRep = new Dictionary<string, string>();
                Dictionary<string, Type> dictObjectType = new Dictionary<string, Type>();
                Dictionary<string, object> dictPluginState = new Dictionary<string, object>();

                //Convert the serialization string into an array of JSON objects
                string strPluginKey = plugin.Key;
                string strPluginStateJson = plugin.Value.ToString();
                Newtonsoft.Json.Linq.JArray jarray = (Newtonsoft.Json.Linq.JArray)JsonConvert.DeserializeObject(strPluginStateJson);

                //convert jarray to list in order to pull index for value and index for class type
                List<object> listContainer = jarray.ToObject<List<object>>();
                string strJsonDictJson = listContainer[0].ToString();
                string strObjectTypeDictJson = listContainer[1].ToString();

                //deserialize value and class type of value
                dictJsonRep = JsonConvert.DeserializeObject<Dictionary<string, string>>(strJsonDictJson);
                dictObjectType = JsonConvert.DeserializeObject<Dictionary<string, Type>>(strObjectTypeDictJson);

                //Convert the plugin's JSON into .NET objects and compile a dictionary of the deserialized objects.
                foreach (var pair in dictJsonRep)
                {
                    Type objType = dictObjectType[pair.Key];
                    Type jsonType = objType;
                    string jsonRep = pair.Value;

                    object objDeserialized = JsonConvert.DeserializeObject(jsonRep, jsonType);
                    dictPluginState.Add(pair.Key, objDeserialized);
                }
                dictPluginStates.Add(strPluginKey, dictPluginState);
            }
            return dictPluginStates;
        }


        public static IDictionary<string, object> StringToState(string Serialized)
        {
            string strProjectStateJson = Serialized;
            //Newtonsoft.Json.Linq.JArray jarray = (Newtonsoft.Json.Linq.JArray)JsonConvert.DeserializeObject(strProjectStateJson);
            //var listPackedPluginState = JsonConvert.DeserializeObject<Dictionary<string, object>>(strProjectStateJson);

            //Instantiate some objects for later use.
            Dictionary<string, string> dictJsonRep = new Dictionary<string, string>();
            Dictionary<string, Type> dictObjectType = new Dictionary<string, Type>();
            Dictionary<string, object> dictPluginState = new Dictionary<string, object>();

            //Convert the serialization string into an array of JSON objects
            Newtonsoft.Json.Linq.JArray jarray = (Newtonsoft.Json.Linq.JArray)JsonConvert.DeserializeObject(Serialized);

            //convert jarray to list in order to pull index for value and index for class type
            List<object> listContainer = jarray.ToObject<List<object>>();
            string strJsonDictJson = listContainer[0].ToString();
            string strObjectTypeDictJson = listContainer[1].ToString();

            //deserialize value and class type of value
            dictJsonRep = JsonConvert.DeserializeObject<Dictionary<string, string>>(strJsonDictJson);
            dictObjectType = JsonConvert.DeserializeObject<Dictionary<string, Type>>(strObjectTypeDictJson);

            //Convert the plugin's JSON into .NET objects and compile a dictionary of the deserialized objects.
            foreach (var pair in dictJsonRep)
            {
                Type objType = dictObjectType[pair.Key];
                Type jsonType = objType;
                string jsonRep = pair.Value;

                object objDeserialized = JsonConvert.DeserializeObject(jsonRep, jsonType);
                dictPluginState.Add(pair.Key, objDeserialized);
            }

            return dictPluginState;
        }
    }
}