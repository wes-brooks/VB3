using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
//using LogUtilities;


namespace Prediction
{
    public class WindComponents
    {
        //given a data source (datatable), a windspeed column name, a winddirection column name,
        //a beach orientation angle, compute the orthogonal beach components (on/across shore)
        //the mathematics assume all inputs are doubles (or convertable strings) and angles are
        //degree measures plus/minus from North (conventionally straight up)

        //usage 1: datasheet project addition of component value columns to imported dataset.
        //  set the datatable, windspeed and direction column names and beach orientation properties
        //  and retreive the component property lists UComp and VComp
        //  or..
        //  use the constructor accepting datatable, column names and beachangle and then
        //  retreive the U/VComp lists

        //usage 2: prediction project where a wind component ends up in the model and model evaluation(s)
        //  are to be computed from speed and direction data inputs.
        //  use the constructor accepting windspeed and direction values and retrieve the U/VComp lists.

        public static string strWindSpeedColName = string.Empty;
        public static string strWindDirColName = string.Empty;
        public static double dblBeachOrientation = double.NaN;
        public static DataTable _dt = null;

        //computed orthogonal wind components
        public double dblUcomp = double.NaN;
        public double dblVcomp = double.NaN;

        //computed lists of wind components
        public List<double> lstUComp = null;
        public List<double> lstVComp = null;

        private string strMessage = string.Empty;

        private List<string> lstWcompColNamesAdded;

        public WindComponents()
        {
            lstUComp = new List<double>();
            lstVComp = new List<double>();
        }


        public WindComponents(DataTable DT, string WindSpeedColumnName, string WindDirectionColumnName, double BeachOrientation)
        {
            lstUComp = new List<double>();
            lstVComp = new List<double>();
            strWindSpeedColName = WindSpeedColumnName;
            strWindDirColName = WindDirectionColumnName;
            dblBeachOrientation = BeachOrientation;
            _dt = DT;
            CalcValues();
        }


        public WindComponents(double windspeed, double winddirection)
        {
            lstUComp = new List<double>();
            lstVComp = new List<double>();

            if (WindCmponents(windspeed, winddirection))
            {
                lstUComp.Add(dblUcomp);
                lstVComp.Add(dblVcomp);
            }
            else
            {
                lstUComp = null;
                lstVComp = null;
            }
        }


        public WindComponents(double windspeed, double winddirection, double beachOrientation)
        {
            lstUComp = new List<double>();
            lstVComp = new List<double>();
            dblBeachOrientation = beachOrientation;

            if (WindCmponents(windspeed, winddirection))
            {
                lstUComp.Add(dblUcomp);
                lstVComp.Add(dblVcomp);
            }
            else
            {
                lstUComp = null;
                lstVComp = null;
            }
        }


        //inputs
        public string WindSpeedColumnName
        {
            set { strWindSpeedColName = value; }
            get { return strWindSpeedColName; }
        }


        public string WindDirectionColumnName
        {
            set { strWindDirColName = value; }
            get { return strWindDirColName; }
        }


        public double BeachOrientation
        {
            set { dblBeachOrientation = value; }
            get { return dblBeachOrientation; }
        }


        public DataTable DT
        {
            set { _dt = value; }
            get { return _dt; }
        }


        //outputs
        public List<double> UComp
        {
            get
            { return lstUComp; }
        }


        public List<double> VComp
        {
            get
            { return lstVComp; }
        }


        public string Message
        {
            get { return strMessage; }
        }


        public List<string> WCompColNamesAdded
        {
            get { return lstWcompColNamesAdded; }
        }


        internal void CalcValues()
        {
            lstWcompColNamesAdded = new List<string>();
            DataTable dt = _dt.Copy();
            string strColA = "WindA_comp[" + strWindDirColName.ToString() + ","
                + strWindSpeedColName.ToString() + "," + dblBeachOrientation.ToString() + "]";
            string strColO = "WindO_comp[" + strWindDirColName.ToString() + ","
                + strWindSpeedColName.ToString() + "," + dblBeachOrientation.ToString() + "]";
            dt.Columns.Add(strColA, typeof(System.Double));
            dt.Columns.Add(strColO, typeof(System.Double));
            lstWcompColNamesAdded.Add(strColA);
            lstWcompColNamesAdded.Add(strColO);
            //populate the wind component lists using datatable information
            try
            {
                int i = 0;
                foreach (DataRow r in _dt.Rows)
                {
                    if (WindCmponents(Convert.ToDouble(r[strWindSpeedColName].ToString()),
                        Convert.ToDouble(r[strWindDirColName].ToString())))
                    {
                        lstUComp.Add(dblUcomp);
                        lstVComp.Add(dblVcomp);
                        //update datatable
                        dt.Rows[i][strColA] = (object)dblUcomp;
                        dt.Rows[i][strColO] = (object)dblVcomp;
                        i++;
                    }
                    else
                    {
                        //something didn't compute
                        lstUComp = null;
                        lstVComp = null;
                        break;
                    }
                }
            }
            catch (FormatException ef) { showmessage(ef); }
            catch (ArgumentException ea) { showmessage(ea); }
            catch (OverflowException eo) { showmessage(eo); }
            catch (InvalidOperationException ei) { showmessage(ei); }

            _dt = dt;
        }


        internal bool WindCmponents(double ws, double wd)
        {
            //compute orthogonal U and V components
            try
            {
                dblVcomp = ws * Math.Sin(dblBeachOrientation * (Math.PI / 180.0d) -
                    wd * (Math.PI / 180.0d));
                dblUcomp = ws * Math.Cos(dblBeachOrientation * (Math.PI / 180.0d) -
                    wd * (Math.PI / 180.0d));
                dblVcomp = ws * Math.Sin((wd - dblBeachOrientation) * Math.PI / 180.0d);
                dblUcomp = (-1) * ws * Math.Cos((wd - dblBeachOrientation) * Math.PI / 180.0d);
            }
            catch (FormatException ef) { showmessage(ef); return false; }
            catch (ArgumentException ea) { showmessage(ea); return false; }
            catch (OverflowException eo) { showmessage(eo); return false; }
            catch (InvalidOperationException ei) { showmessage(ei); return false; }

            strMessage = "OK";
            return true;
        }


        internal void showmessage(Exception e)
        {
        }
    }
}