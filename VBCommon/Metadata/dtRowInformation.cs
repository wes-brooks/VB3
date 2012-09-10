﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data;

namespace VBCommon.Metadata
{
    /// <summary>
    /// class to carry datatable row enable/disable information
    /// there are no extended properties for table rows so this class
    /// is still necessary for column plots enable/disable functions
    /// </summary>
    //[Serializable]
    public class dtRowInformation
    {
        //table to operte with
        private DataTable _dt = null;
        //dictionary structure to hold table row enable/disable information
        private Dictionary<string, bool> dictRowStatus = null;
        //class variable
        //private static dtRowInformation dtRI = null;


        /// <summary>
        /// method initializes a datatable row info structure to all rows enabled
        /// </summary>
        /// <param name="dt"></param>
        public dtRowInformation(DataTable dt)
        {
            if (dt != null)
            {
                _dt = dt.Copy();
                dictRowStatus = new Dictionary<string, bool>();

                for (int r = 0; r < _dt.Rows.Count; r++)
                {
                    if (_dt.ExtendedProperties.ContainsKey(r.ToString()))
                    {
                        string rowI = r.ToString();
                        string ExtendedPropValue = _dt.ExtendedProperties[rowI].ToString();
                        
                        if (ExtendedPropValue == "false")
                            dictRowStatus.Add(_dt.Rows[r][0].ToString(), false);
                        else
                            dictRowStatus.Add(_dt.Rows[r][0].ToString(), true);
                    }
                    //check to see if enabled col has been added yet first
                    //if (_dt.Columns.Contains("Enabled"))
                    //{
                    //    if (_dt.Rows[r]["Enabled"].ToString() == Convert.ToString(1)) //the row is enabled
                    //        dictRowStatus.Add(_dt.Rows[r][0].ToString(), true);
                    //    else //the row is disabled
                    //        dictRowStatus.Add(_dt.Rows[r][0].ToString(), false);
                    //}
                    else  dictRowStatus.Add(_dt.Rows[r][0].ToString(), true);
                }
            }
        }


        /*/// <summary>
        /// constructor optionally calls method to init the row information structure 
        /// and return the itself - singleton
        /// </summary>
        /// <param name="dt">table to gather info on</param>
        /// <param name="init">if true, initialize; for example, on import</param>
        /// <returns></returns>
        public static dtRowInformation getdtRI(DataTable dt, bool init)
        {
            //pass null after initialization to access the DTRowInfo property
            //or pass init=true (after import) to initialize
            if (this == null || init) dtRI = new dtRowInformation(dt);
            return dtRI;
        }*/


        /// <summary>
        /// method returns the enable/disable status of the row name (key)
        /// </summary>
        /// <param name="key">row name is the column 0 record identifier or date/timestamp value</param>
        /// <returns>true if enabled, false if disabled</returns>
        public bool GetRowStatus(string key)
        {
            //returns the status of a row
            bool boolStatus;
            dictRowStatus.TryGetValue(key, out boolStatus);
            return boolStatus;
        }


        /// <summary>
        /// method set the specified row status to enable/disable
        /// </summary>
        /// <param name="key">record identifer</param>
        /// <param name="val">true iff enable, false if disable</param>
        public void SetRowStatus(string key, bool val)
        {
            //sets the status of a row
            dictRowStatus[key] = val;            
        }

        
        /// <summary>
        /// property - returns a row status dictionary for all rows in table
        /// </summary>
        public Dictionary<string, bool> DTRowInfo
        {
            //returns a row-status dictionary for all rows in the datatable
            set { dictRowStatus = value; }
            get { return dictRowStatus; }
        }
    }
}
