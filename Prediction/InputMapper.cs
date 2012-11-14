using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VBCommon;
using VBCommon.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Prediction
{
    class InputMapper
    {
        private Dictionary<string, string> dictColMap;
        private DataTable tblMappedData;
        private string[] strArrReferencedVars;
        private string strLeftCaption, strRightCaption;


        public InputMapper()
        {
        }


        public InputMapper(IDictionary<string, object> PackedState)
        {
            this.UnpackState(PackedState);
        }


        public InputMapper(Dictionary<string, string> MainEffects, string LeftCaption, string[] VariableNames, string RightCaption)
        {
            strArrReferencedVars = VariableNames;
            //dictMainEffects = MainEffects;
            strLeftCaption = LeftCaption;
            strRightCaption = RightCaption;
        }


        public InputMapper(string LeftCaption, string[] VariableNames, string RightCaption)
        {
            strArrReferencedVars = VariableNames;
            //dictMainEffects = MainEffects;
            strLeftCaption = LeftCaption;
            strRightCaption = RightCaption;
        }


        public DataTable ImportFile(DataTable tblCurrent)
        {           
            VBCommon.IO.ImportExport import = new ImportExport();
            DataTable dt = import.Input;            
            if (dt == null)
                return(null);

            if (dictColMap == null)
            {
                string[] strArrHeaderCaptions = { strLeftCaption, strRightCaption };

                //Dictionary<string, string> dictFields = new Dictionary<string, string>(dictMainEffects);
                frmColumnMapper colMapper = new frmColumnMapper(strArrReferencedVars, dt, strArrHeaderCaptions, true, false);
                DialogResult dr = colMapper.ShowDialog();

                if (dr == DialogResult.OK)
                {
                    dt = colMapper.MappedTable;
                    dictColMap = colMapper.ColumnMapping;
                    dt = MapInputColumns(dt);

                    if (!CheckUniqueIDs(dt)) 
                        return null;
                    else 
                        return dt;
                }
                else
                    return null;
            }
            else
            {
                //We shouldn't have existing data unless we have an existing mapping, so assume that tblCurrent!=null lands us here.
                dt = MapInputColumns(dt);

                if (tblCurrent != null)
                {
                    tblCurrent.Merge(dt);
                    dt = tblCurrent;
                }

                if (!CheckUniqueIDs(dt))
                    return null;
                else
                    return dt;
            }
        }


        private DataTable MapInputColumns(DataTable tblRawData)
        {
            DataTable dt = new DataTable();

            if (dictColMap.ContainsKey("ID"))
                dt.Columns.Add("ID", typeof(string));     

            foreach (string meKey in dictColMap.Keys)
            {
                if (String.Compare(meKey,"ID",true) != 0)                
                    dt.Columns.Add(meKey, typeof(double));                                                                      
            }

            //Populate the new data table with data from the old.
            for (int i = 0; i < tblRawData.Rows.Count; i++)
            {
                DataRow dr = dt.NewRow();
                foreach (string meKey in dictColMap.Keys)
                {
                    if (String.Compare(meKey, "ID",true) == 0)
                        dr[meKey] = tblRawData.Rows[i][dictColMap[meKey]].ToString();
                    else
                        dr[meKey] = tblRawData.Rows[i][dictColMap[meKey]];
                }
                dt.Rows.Add(dr);
            }

            if (dt.Columns.Contains("ID"))
                dt.Columns["ID"].SetOrdinal(0);

            tblMappedData = dt;
            return tblMappedData;
        }


        private bool CheckUniqueIDs(DataTable dt)
        {
            int errndx = 0;
            if (!RecordIndexUnique(dt, out errndx))
            {
                MessageBox.Show("Unable to import datasets with non-unique record identifiers.\n" +
                                "Fix your datatable by assuring unique record identifier values\n" +
                                "in the ID column and try importing again.\n\n" +
                                "Record Identifier values cannot be blank or duplicated;\nencountered " +
                                "error near row " + errndx.ToString(), "Import Data Error - Cannot Import This Dataset", MessageBoxButtons.OK);
                return false;
            }
            return true;
        }
        

        /// <summary>
        /// test all cells in the ID column for uniqueness
        /// could do this with linq but then how does one find where?
        /// Code copied from Mike's VBDatasheet.frmDatasheet.
        /// </summary>
        /// <param name="dt">table to search</param>
        /// <param name="where">record number of offending timestamp</param>
        /// <returns>true iff all unique, false otherwise</returns>
        public static bool RecordIndexUnique(DataTable dt, out int where)
        {
            Dictionary<string, int> dictTemp = new Dictionary<string, int>();
            int intNdx = -1;
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string strTempval = dr["ID"].ToString();
                    dictTemp.Add(dr["ID"].ToString(), ++intNdx);
                    if (string.IsNullOrWhiteSpace(dr["ID"].ToString()))
                    {
                        where = intNdx++;
                        //MessageBox.Show("Record Identifier values cannot be blank - encountered blank in row " + ndx++.ToString() + ".\n",
                        //    "Import data error", MessageBoxButtons.OK);
                        return false;
                    }
                }
            }
            catch (ArgumentException)
            {
                where = intNdx++;
                //MessageBox.Show("Record Identifier values cannot be duplicated - encountered existing record in row " + ndx++.ToString() + ".\n",
                //    "Import data error", MessageBoxButtons.OK);
                return false;
            }
            where = intNdx;
            return true;
        }


        public IDictionary<string, object> PackState()
        {
            IDictionary<string, object> dictPackedState = new Dictionary<string, object>();

            //dictPackedState.Add("MainEffects", dictMainEffects);
            dictPackedState.Add("ColumnMap", JsonConvert.SerializeObject(dictColMap));
            dictPackedState.Add("ReferencedVariables", strArrReferencedVars);
            dictPackedState.Add("LeftCaption", strLeftCaption);
            dictPackedState.Add("RightCaption", strRightCaption);

            return dictPackedState;
        }


        public void UnpackState(IDictionary<string, object> PackedState)
        {
            IDictionary<string, object> dictPackedState = PackedState;

            if (dictPackedState.ContainsKey("ColumnMap"))
            {
                if (dictPackedState["ColumnMap"].GetType() == typeof(string))
                {
                    Type objType = typeof(Dictionary<string, string>);
                    string jsonRep = dictPackedState["ColumnMap"].ToString();

                    object objVariableMapping = JsonConvert.DeserializeObject(jsonRep, objType);
                    dictColMap = (Dictionary<string, string>)objVariableMapping;
                }
            }

            if (dictPackedState.ContainsKey("ReferencedVariables"))
            {
                if (dictPackedState["ReferencedVariables"].GetType() == typeof(Newtonsoft.Json.Linq.JArray))
                {
                    Type objType = typeof(string[]);
                    string jsonRep = dictPackedState["ReferencedVariables"].ToString();

                    object objVariableMapping = JsonConvert.DeserializeObject(jsonRep, objType);
                    strArrReferencedVars = (string[])objVariableMapping;
                }
            }

            if (dictPackedState.ContainsKey("LeftCaption"))
            {
                if (dictPackedState["LeftCaption"].GetType() == typeof(Newtonsoft.Json.Linq.JObject))
                {
                    Type objType = typeof(string);
                    string jsonRep = dictPackedState["LeftCaption"].ToString();

                    object objVariableMapping = JsonConvert.DeserializeObject(jsonRep, objType);
                    strLeftCaption = objVariableMapping.ToString();
                }
            }

            if (dictPackedState.ContainsKey("RightCaption"))
            {
                if (dictPackedState["RightCaption"].GetType() == typeof(Newtonsoft.Json.Linq.JObject))
                {
                    Type objType = typeof(string);
                    string jsonRep = dictPackedState["RightCaption"].ToString();

                    object objVariableMapping = JsonConvert.DeserializeObject(jsonRep, objType);
                    strRightCaption = objVariableMapping.ToString();
                }
            }
        }
    }
}
