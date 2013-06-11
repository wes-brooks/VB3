using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
//using LogUtilities;

namespace VBCommon.Spatial
{
    public class WaterCurrentComponents
    {
        public static string _currentFlowSpeed = string.Empty;
        public static string _currentFlowDirection = string.Empty;
        public static double _beachOrientation = double.NaN;
        public static DataTable _dt = null;

        //computed orthogonal current components
        public double _Ucomp = double.NaN;
        public double _Vcomp = double.NaN;

        //computed lists of current components
        public List<double> _UComp = null;
        public List<double> _VComp = null;

        private string _message = string.Empty;

        private List<string> _ccompColNamesAdded;
        
        public string CurrentSpeedColName
        {
            set { _currentFlowSpeed = value; }
            get { return _currentFlowSpeed; }
        }
        public string CurrentDirectionColName
        {
            set { _currentFlowDirection = value; }
            get { return _currentFlowDirection; }
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

        //outputs
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
        public List<string> CCompColNamesAdded
        {
            get { return _ccompColNamesAdded; }
        }

        public WaterCurrentComponents()
        {
            _UComp = new List<double>();
            _VComp = new List<double>();
        }

        public WaterCurrentComponents(DataTable DT, string currentDirectionColumnName, string currentSpeedColumnName, double BeachOrientation)
        {
            _UComp = new List<double>();
            _VComp = new List<double>();
            _currentFlowSpeed = currentSpeedColumnName;
            _currentFlowDirection = currentDirectionColumnName;
            _beachOrientation = BeachOrientation;
            _dt = DT;
            CalcValues();
        }


        public WaterCurrentComponents(double currentdirection, double currentspeed, double beachOrientation)
        {
            _UComp = new List<double>();
            _VComp = new List<double>();

            _beachOrientation = beachOrientation;

            if (CurrentComponents(currentspeed, currentdirection))
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

        private bool CurrentComponents(double ws, double wd)
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

                //change 5/13/2011 vi MC
                //_Ucomp = ws * Math.Cos( (_beachOrientation - wd + 180.0d) * (Math.PI / 180.0d) );
                //_Vcomp = ws * Math.Sin( (_beachOrientation - wd + 180.0d) * (Math.PI / 180.0d) );

                //change 5/19/2011 vi MC
                _Ucomp = ws * Math.Cos((wd - _beachOrientation) * (Math.PI / 180.0d));
                _Vcomp = -1 * ws * Math.Sin( (wd - _beachOrientation) * (Math.PI / 180.0d));
               
            }
            catch (FormatException ef) { showmessage(ef); return false; }
            catch (ArgumentException ea) { showmessage(ea); return false; }
            catch (OverflowException eo) { showmessage(eo); return false; }
            catch (InvalidOperationException ei) { showmessage(ei); return false; }

            _message = "OK";
            return true;

        }

        internal void CalcValues()
        {
            _ccompColNamesAdded = new List<string>();
            DataTable dt = _dt.Copy();
            string colA = "CurrentA_comp[" + _currentFlowDirection.ToString() + "," + _currentFlowSpeed.ToString() + "," + _beachOrientation.ToString() + "]";
            string colO = "CurrentO_comp[" + _currentFlowDirection.ToString() + "," + _currentFlowSpeed.ToString() + "," + _beachOrientation.ToString() + "]";
            dt.Columns.Add(colA, typeof(System.Double));
            dt.Columns.Add(colO, typeof(System.Double));
            _ccompColNamesAdded.Add(colA);
            _ccompColNamesAdded.Add(colO);
            //populate the wind component lists using datatable information
            try
            {
                int i = 0;
                foreach (DataRow r in _dt.Rows)
                {
                    if (CurrentComponents(Convert.ToDouble(r[_currentFlowSpeed].ToString()),
                        Convert.ToDouble(r[_currentFlowDirection].ToString())))
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

        internal void showmessage(Exception e)
        {
            /*VBLogger.getLogger().logEvent("Exception in watercurrent components class: " + e.Message,
                VBLogger.messageIntent.UserAndLogFile, VBLogger.targetSStrip.StatusStrip3);
            _message = "Decomposition failed: " + e.Message;*/
        }
    }
}
