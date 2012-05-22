using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using LogUtilities;

namespace VBTools
{
    public class CurrentComponents : WindComponents
    {
        //perform the same calculation for current decomposition as for wind - just give it different data
        //see base class WindComponents for its members/properties/methods that are used here
        public static string _currentFlowSpeed = string.Empty;
        public static string _cirrentFlowDirection = string.Empty;

        public string CurrentSpeedColName
        {
            set { _currentFlowSpeed = value; }
            get { return _currentFlowSpeed; }
        }
        public string CurrentDirectionColName
        {
            set { _cirrentFlowDirection = value; }
            get { return _cirrentFlowDirection; }
        }

        public CurrentComponents(DataTable DT, string CurrentSpeedColumnName, string CurrentDirectionColumnName, double BeachOrientation)
        {
            //save the wind data column names
            string temp1 = _windSpeedColName;
            string temp2 = _windDirColName;

            //_UComp = new List<double>();
            //_VComp = new List<double>();
            _currentFlowSpeed = CurrentSpeedColumnName;
            _cirrentFlowDirection = CurrentDirectionColumnName;
            //pretend current data is wind data... update baseclass members and datatable
            _windSpeedColName = CurrentSpeedColumnName;
            _windDirColName = CurrentDirectionColumnName;
            _beachOrientation = BeachOrientation;
            _dt = DT;
            //note: overridden in this class...
            CalcValues();

            //reset the wind data column names
            _windSpeedColName = temp1;
            _windDirColName = temp2;

        }

        public CurrentComponents(double currentspeed, double currentdirection)
        {
            //_UComp = new List<double>();
            //_VComp = new List<double>();
            VBProjectManager _proj = VBProjectManager.GetProjectManager();
            _beachOrientation = _proj.SiteInfo.Orientation;

            //use baseclass method to get components
            if (WindCmponents(currentspeed, currentdirection))
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

        new private void CalcValues()
        {
            //override the baseclass calc method to add current specific columns to table

            DataTable dt = _dt.Copy();
            dt.Columns.Add("Current U-comp", typeof(System.Double));
            dt.Columns.Add("Current V-comp", typeof(System.Double));

            try
            {
                int i = 0;
                foreach (DataRow r in _dt.Rows)
                {
                    //...but use the baseclass method to perform decomposition
                    if (WindCmponents(Convert.ToDouble(r[_windSpeedColName].ToString()),
                        Convert.ToDouble(r[_windDirColName].ToString())))
                    {
                        _UComp.Add(_Ucomp);
                        _VComp.Add(_Vcomp);
                        //update datatable copy
                        dt.Rows[i]["Current U-comp"] = (object)_Ucomp;
                        dt.Rows[i]["Current V-comp"] = (object)_Vcomp;
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

            //update the baseclass member datatable
            _dt = dt;
        }

    }
}
