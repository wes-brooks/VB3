using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
//using LogUtilities;


namespace VBCommon.Spatial
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

        public static string _windSpeedColName = string.Empty;
        public static string _windDirColName = string.Empty;
        public static double _beachOrientation = double.NaN;
        public static DataTable _dt = null;

        //computed orthogonal wind components
        public double _Ucomp = double.NaN;
        public double _Vcomp = double.NaN;

        //computed lists of wind components
        public List<double> _UComp = null;
        public List<double> _VComp = null;

        private string _message = string.Empty;

        private List<string> _wcompColNamesAdded;

        public WindComponents()
        {
            _UComp = new List<double>();
            _VComp = new List<double>();
        }

        public WindComponents(DataTable DT, string WindDirectionColumnName, string WindSpeedColumnName, double BeachOrientation)
        {
            _UComp = new List<double>();
            _VComp = new List<double>();
            _windSpeedColName = WindSpeedColumnName;
            _windDirColName = WindDirectionColumnName;
            _beachOrientation = BeachOrientation;
            _dt = DT;
            CalcValues();
        }


        public WindComponents(double winddirection, double windspeed, double beachOrientation)
        {
            _UComp = new List<double>();
            _VComp = new List<double>();
           
            _beachOrientation = beachOrientation;

            if (WindCmponents(windspeed, winddirection))
            {
                _UComp.Add(_Ucomp);
                _VComp.Add(_Vcomp);
            }
            else
            {
                _UComp = null;
                _VComp = null;
            }

        }

        //inputs
        public string WindSpeedColumnName
        {
            set { _windSpeedColName = value; }
            get { return _windSpeedColName; }
        }


        public string WindDirectionColumnName
        {
            set { _windDirColName = value; }
            get { return _windDirColName; }
        }


        public double BeachOrientation
        {
            set { _beachOrientation = value; }
            get { return _beachOrientation; }
        }


        public DataTable DT
        {
            set { _dt = value; }
            get { return _dt; }
        }


        public List<double> UComp
        {
            get 
            {
                return _UComp; 
            }
        }
        public List<double> VComp
        {
            get
            {
                return _VComp; 
            }
        }


        public string Message
        {
            get { return _message; }
        }


        public List<string> WCompColNamesAdded
        {
            get { return _wcompColNamesAdded; }
        }


        internal void CalcValues()
        {

            _wcompColNamesAdded = new List<string>();
            DataTable dt = _dt.Copy();
            string colA = "WindA_comp[" + _windDirColName.ToString() + "," + _windSpeedColName.ToString() + "," + _beachOrientation.ToString() + "]";
            string colO = "WindO_comp[" + _windDirColName.ToString() + "," + _windSpeedColName.ToString() + "," + _beachOrientation.ToString() + "]";
            dt.Columns.Add(colA, typeof(System.Double));
            dt.Columns.Add(colO, typeof(System.Double));
            _wcompColNamesAdded.Add(colA);
            _wcompColNamesAdded.Add(colO);
            //populate the wind component lists using datatable information
            try
            {
                int i = 0;
                foreach (DataRow r in _dt.Rows)
                {
                    if (WindCmponents(Convert.ToDouble(r[_windSpeedColName].ToString()),
                        Convert.ToDouble(r[_windDirColName].ToString())))
                    {
                        _UComp.Add(_Ucomp);
                        _VComp.Add(_Vcomp);
                        //update datatable
                        dt.Rows[i][colA] = (object)_Ucomp;
                        dt.Rows[i][colO] = (object)_Vcomp;
                        i++;
                    }
                    else
                    {
                        //something didn't compute
                        _UComp = null;
                        _VComp = null;
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
                //_Ucomp = ws * Math.Sin(_beachOrientation * (Math.PI / 180.0d) -
                //    wd * (Math.PI / 180.0d));
                //_Vcomp = (-1) * ws * Math.Cos(_beachOrientation * (Math.PI / 180.0d) -
                //    wd * (Math.PI / 180.0d));
                //change via MC 5/2010
                //_Vcomp = ws * Math.Sin(_beachOrientation * (Math.PI / 180.0d) -
                //    wd * (Math.PI / 180.0d));
                //_Ucomp = ws * Math.Cos(_beachOrientation * (Math.PI / 180.0d) -
                //    wd * (Math.PI / 180.0d));
                //change via MC 5/2011
                _Vcomp = ws * Math.Sin( (wd - _beachOrientation) * Math.PI / 180.0d);
                _Ucomp = (-1) * ws * Math.Cos( (wd - _beachOrientation) * Math.PI / 180.0d);
            }
            catch (FormatException ef) { showmessage(ef); return false; }
            catch (ArgumentException ea) { showmessage(ea); return false; }
            catch (OverflowException eo) { showmessage(eo); return false; }
            catch (InvalidOperationException ei) { showmessage(ei); return false; }

            _message = "OK";
            return true;            
        } 

        internal void showmessage(Exception e)
        {
            /*VBLogger.getLogger().logEvent("Exception in wind components class: " + e.Message,
                VBLogger.messageIntent.UserAndLogFile, VBLogger.targetSStrip.StatusStrip3);
            _message = "Decomposition failed: " + e.Message;*/
        }
    }


    


}
