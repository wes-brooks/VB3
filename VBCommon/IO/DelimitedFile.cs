using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

namespace VBCommon.IO
{
    /// <summary>
    /// 
    /// </summary>
    public class DelimitedFile
    {
        private char[] _delimeter = new char[1] { ',' };
        private string _fileName = "";
        private bool _firstRowHeaders = true;


        public DelimitedFile(string fileName, char delimeter, bool firstRowHeaders)
        {
            _fileName = fileName;
            _delimeter[0] = delimeter;
            _firstRowHeaders = firstRowHeaders;
        }

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        public char Delimeter
        {
            get { return _delimeter[0]; }
            set { _delimeter[0] = value; }
        }

        public bool FirstRowHeader
        {
            get { return _firstRowHeaders; }
            set { _firstRowHeaders = value; }
        }


        /// <summary>
        /// Reads the delimeted file and returns a datatable
        /// </summary>
        /// <returns></returns>
        public DataTable Read()
        {           
            StreamReader sr = null;
            DataTable data = null;

            try
            {
                if ((_fileName == null) || (_fileName == ""))
                    throw new Exception("File has not been specified.");
                if (!File.Exists(_fileName))
                    throw new Exception("Could not find file: " + _fileName);

                data = new DataTable();
                sr = new StreamReader(_fileName);

                // Read the header and extract the field names.
                string line = "";

                if (_firstRowHeaders)
                {
                    line = sr.ReadLine();
                    string[] columnNames = line.Split(_delimeter);

                    for (int i = 0; i < columnNames.Length; i++)
                    {
                        data.Columns.Add(columnNames[i].Trim());
                    }
                }

                data.Columns[0].DataType = typeof(String);
                DataRow dr = null;
                line = sr.ReadLine();
                while (line != null && line.Length > 0)
                {
                    string[] cells = line.Split(_delimeter);
                    dr = data.NewRow();
                    for (int i = 0; i < cells.Length; i++)
                    {
                        //cellVal = cells[i];
                        dr[i] = cells[i];
                    }
                    data.Rows.Add(dr);
                    line = sr.ReadLine();
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            finally
            {
                if (sr != null)
                    sr.Close();
            }
							
			return data;		
        }

        public void Write(DataTable dt)
        {
            StreamWriter sw = null;
            try
            {
                if ((_fileName == null) || (_fileName == ""))
                    throw new Exception("File has not been specified.");

                if ((dt == null) || (dt.Rows.Count < 1))
                {
                }

                //Overwrite file if it exists
                sw = new StreamWriter(_fileName,false);
                StringBuilder sb = null;
                
                //Write out column headers if requested
                if (_firstRowHeaders)
                {
                    sb = new StringBuilder();
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        if (i > 0)                        
                            sb.Append(_delimeter[0]);

                        sb.Append(dt.Columns[i].ColumnName);
                    }
                    sw.WriteLine(sb.ToString());
                }

                //Write out the data
                for (int i=0; i<dt.Rows.Count;i++)
                {
                    sb = new StringBuilder();
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {                        
                        if (j > 0)
                            sb.Append(_delimeter[0]);

                        sb.Append(dt.Rows[i][j].ToString());
                    }
                    sw.WriteLine(sb.ToString());
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (sw != null)
                    sw.Close();
            }
        }
    }
}
