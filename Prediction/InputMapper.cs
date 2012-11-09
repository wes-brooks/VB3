using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VBCommon;
using VBCommon.IO;
using System.Windows.Forms;

namespace Prediction
{
    class InputMapper
    {
        private Dictionary<string, string> dictMainEffects, dictColMap;
        private DataTable tblMappedData;
        private string[] strArrReferencedVars;
        private string strLeftCaption, strRightCaption;


        public InputMapper(Dictionary<string, string> MainEffects, string LeftCaption, string[] VariableNames, string RightCaption)
        {
            strArrReferencedVars = VariableNames;
            dictMainEffects = MainEffects;
            strLeftCaption = LeftCaption;
            strRightCaption = RightCaption;
        }


        public DataTable ImportFile()
        {           
            VBCommon.IO.ImportExport import = new ImportExport();
            DataTable dt = import.Input;            
            if (dt == null)
                return(null);

            if (dictColMap == null)
            {
                string[] strArrHeaderCaptions = { strLeftCaption, strRightCaption };

                Dictionary<string, string> dictFields = new Dictionary<string, string>(dictMainEffects);
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
                dt = MapInputColumns(dt);

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

            dictPackedState.Add("MainEffects", dictMainEffects);
            dictPackedState.Add("ColumnMap", dictColMap);
            dictPackedState.Add("ReferencedVariables", strArrReferencedVars);
            dictPackedState.Add("LeftCaption", strLeftCaption);
            dictPackedState.Add("RightCaption", strRightCaption);

            return dictPackedState;
        }


        public void PackState(IDictionary<string, object> PackedState)
        {
            IDictionary<string, object> dictPackedState = PackedState;

            dictMainEffects = (Dictionary<string, string>)(dictPackedState["dictMainEffects"]);
            dictColMap = (Dictionary<string, string>)(dictPackedState["ColumnMap"]);
            strArrReferencedVars = (string[])(dictPackedState["ReferencedVariables"]);
            strLeftCaption = dictPackedState["LeftCaption"].ToString();
            strRightCaption = dictPackedState["RightCaption"].ToString();
        }
    }
}
