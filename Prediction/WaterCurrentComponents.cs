using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
//using LogUtilities;

namespace Prediction
{
    public class WaterCurrentComponents
    {

        public static string strCurrentFlowSpeed = string.Empty;
        public static string strCurrentFlowDirection = string.Empty;
        public static double dblBeachOrientation = double.NaN;
        public static DataTable _dt = null;

        //computed orthogonal current components
        public double dblUcomp = double.NaN;
        public double dblVcomp = double.NaN;

        //computed lists of current components
        public List<double> lstUComp = null;
        public List<double> lstVComp = null;
        private string strMessage = string.Empty;
        private List<string> lstCcompColNamesAdded;
        

        public string CurrentSpeedColName
        {
            set { strCurrentFlowSpeed = value; }
            get { return strCurrentFlowSpeed; }
        }


        public string CurrentDirectionColName
        {
            set { strCurrentFlowDirection = value; }
            get { return strCurrentFlowDirection; }
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
            get { return lstVComp;}
        }


        public string Message
        {
            get { return strMessage; }
        }


        public List<string> CCompColNamesAdded
        {
            get { return lstCcompColNamesAdded; }
        }


        public WaterCurrentComponents()
        {
            lstUComp = new List<double>();
            lstVComp = new List<double>();
        }


        public WaterCurrentComponents(DataTable DT, string currentSpeedColumnName, string currentDirectionColumnName, double BeachOrientation)
        {
            lstUComp = new List<double>();
            lstVComp = new List<double>();
            strCurrentFlowSpeed = currentSpeedColumnName;
            strCurrentFlowDirection = currentDirectionColumnName;
            dblBeachOrientation = BeachOrientation;
            _dt = DT;
            CalcValues();
        }


        public WaterCurrentComponents(double currentspeed, double currentdirection)
        {
            lstUComp = new List<double>();
            lstVComp = new List<double>();
           
            if (CurrentComponents(currentspeed, currentdirection))
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


        public WaterCurrentComponents(double currentspeed, double currentdirection, double beachOrientation)
        {
            lstUComp = new List<double>();
            lstVComp = new List<double>();
            dblBeachOrientation = beachOrientation;

            if (CurrentComponents(currentspeed, currentdirection))
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


        private bool CurrentComponents(double ws, double wd)
        {
            //compute orthogonal U and V components
            try
            {
                dblUcomp = ws * Math.Cos( (wd - dblBeachOrientation) * (Math.PI / 180.0d));
                dblVcomp = (-1) * ws * Math.Sin( (wd - dblBeachOrientation) * (Math.PI / 180.0d));
               
            }
            catch (FormatException ef) { showmessage(ef); return false; }
            catch (ArgumentException ea) { showmessage(ea); return false; }
            catch (OverflowException eo) { showmessage(eo); return false; }
            catch (InvalidOperationException ei) { showmessage(ei); return false; }

            strMessage = "OK";
            return true;
        }


        internal void CalcValues()
        {
            lstCcompColNamesAdded = new List<string>();
            DataTable dt = _dt.Copy();
            string strColA = "CurrentA_comp[" + strCurrentFlowDirection.ToString() + ","
                + strCurrentFlowSpeed.ToString() + "," + dblBeachOrientation.ToString() + "]";
            string strColO = "CurrentO_comp[" + strCurrentFlowDirection.ToString() + ","
                + strCurrentFlowSpeed.ToString() + "," + dblBeachOrientation.ToString() + "]";
            dt.Columns.Add(strColA, typeof(System.Double));
            dt.Columns.Add(strColO, typeof(System.Double));
            lstCcompColNamesAdded.Add(strColA);
            lstCcompColNamesAdded.Add(strColO);
            //populate the wind component lists using datatable information
            try
            {
                int i = 0;
                foreach (DataRow r in _dt.Rows)
                {
                    if (CurrentComponents(Convert.ToDouble(r[strCurrentFlowSpeed].ToString()),
                        Convert.ToDouble(r[strCurrentFlowDirection].ToString())))
                    {
                        lstUComp.Add(dblUcomp);
                        lstVComp.Add(dblVcomp);
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


        internal void showmessage(Exception e)
        {
        }
    }
}
