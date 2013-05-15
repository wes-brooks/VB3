using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace VBCommon
{
    public static class SerializationUtilities
    {
        public static void SerializeDataTable(DataTable Data, IDictionary<string, object> Container, string Slot, string Title = null)
        {
            if (Title == null)
                Title = Slot;

            string strSerializedDataTable = null;

            if (Data != null)
            {
                Data.AcceptChanges();
                Data.TableName = Title;
                StringWriter sw = new StringWriter();
                Data.WriteXml(sw, XmlWriteMode.WriteSchema, false);
                strSerializedDataTable = sw.ToString();
                sw.Close();
            }

            //If there's already observations packed up for this modeling method, remove them before adding the new stuff.
            if (Container.ContainsKey(Slot))
                Container.Remove(Slot);
            Container.Add(Slot, strSerializedDataTable);
        }


        public static DataTable DeserializeDataTable(IDictionary<string, object> Container, string Slot, string Title = null)
        {
            DataTable tblData = null;

            if (Title == null)
                Title = Slot;

            if (Container.ContainsKey(Slot))
            {
                if (Container[Slot] != null)
                {
                    string strPackedDataTable = Container[Slot].ToString();

                    if (!String.IsNullOrWhiteSpace(strPackedDataTable))
                    {
                        DataSet ds = new DataSet();
                        ds.ReadXml(new StringReader(strPackedDataTable), XmlReadMode.ReadSchema);
                        tblData = ds.Tables[0];
                    }
                }
            }
            return tblData;
        }
    }
}