using System;
using System.Collections.Generic;
using System.Data;

namespace VBCommon.Spatial
{
    public class WaveComponents
    {
        public static double _beachOrientation = double.NaN;
        public static DataTable _dt = null;
        private string _message;
        public double _Ucomp;
        public List<double> _UComp;
        public double _Vcomp;
        public List<double> _VComp;
        public static string _waveDirection = string.Empty;
        public static string _waveHeight = string.Empty;
        private List<string> _wcompColNamesAdded;

        public WaveComponents()
        {
            this._Ucomp = double.NaN;
            this._Vcomp = double.NaN;
            this._message = string.Empty;
            this._UComp = new List<double>();
            this._VComp = new List<double>();
        }


        public WaveComponents(double wavedirection, double waveheight, double beachOrientation)
        {
            this._Ucomp = double.NaN;
            this._Vcomp = double.NaN;
            this._message = string.Empty;
            this._UComp = new List<double>();
            this._VComp = new List<double>();
            _beachOrientation = beachOrientation;
            if (this.ComputeWaveComponents(waveheight, wavedirection))
            {
                this._UComp.Add(this._Ucomp);
                this._VComp.Add(this._Vcomp);
            }
            else
            {
                this._UComp = null;
                this._VComp = null;
            }
        }


        public WaveComponents(DataTable DT, string waveDirectionColumnName, string waveHeightColumnName, double BeachOrientation)
        {
            this._Ucomp = double.NaN;
            this._Vcomp = double.NaN;
            this._message = string.Empty;
            this._UComp = new List<double>();
            this._VComp = new List<double>();
            _waveHeight = waveHeightColumnName;
            _waveDirection = waveDirectionColumnName;
            _beachOrientation = BeachOrientation;
            _dt = DT;
            this.CalcValues();
        }


        internal void CalcValues()
        {
            this._wcompColNamesAdded = new List<string>();
            DataTable table = _dt.Copy();
            string columnName = "WaveA_comp[" + _waveDirection.ToString() + "," + _waveHeight.ToString() + "," + _beachOrientation.ToString() + "]";
            string str2 = "WaveO_comp[" + _waveDirection.ToString() + "," + _waveHeight.ToString() + "," + _beachOrientation.ToString() + "]";
            table.Columns.Add(columnName, typeof(double));
            table.Columns.Add(str2, typeof(double));
            this._wcompColNamesAdded.Add(columnName);
            this._wcompColNamesAdded.Add(str2);
            try
            {
                int num = 0;
                foreach (DataRow row in _dt.Rows)
                {
                    if (this.ComputeWaveComponents(Convert.ToDouble(row[_waveHeight].ToString()), Convert.ToDouble(row[_waveDirection].ToString())))
                    {
                        this._UComp.Add(this._Ucomp);
                        this._VComp.Add(this._Vcomp);
                        table.Rows[num][columnName] = this._Ucomp;
                        table.Rows[num][str2] = this._Vcomp;
                        num++;
                    }
                    else
                    {
                        this._UComp = null;
                        this._VComp = null;
                        break;
                    }
                }
            }
            catch (FormatException exception)
            {
                this.showmessage(exception);
            }
            catch (ArgumentException exception2)
            {
                this.showmessage(exception2);
            }
            catch (OverflowException exception3)
            {
                this.showmessage(exception3);
            }
            catch (InvalidOperationException exception4)
            {
                this.showmessage(exception4);
            }
            _dt = table;
        }


        private bool ComputeWaveComponents(double ws, double wd)
        {
            try
            {
                this._Ucomp = ws * Math.Cos((wd - _beachOrientation) * Math.PI / 180.0d);
                this._Vcomp = -1 * ws * Math.Sin((wd - _beachOrientation) * Math.PI / 180.0d);
            }
            catch (FormatException exception)
            {
                this.showmessage(exception);
                return false;
            }
            catch (ArgumentException exception2)
            {
                this.showmessage(exception2);
                return false;
            }
            catch (OverflowException exception3)
            {
                this.showmessage(exception3);
                return false;
            }
            catch (InvalidOperationException exception4)
            {
                this.showmessage(exception4);
                return false;
            }
            this._message = "OK";
            return true;
        }


        internal void showmessage(Exception e) { }


        public double BeachOrientation
        {
            get
            {
                return _beachOrientation;
            }
            set
            {
                _beachOrientation = value;
            }
        }


        public DataTable DT
        {
            get
            {
                return _dt;
            }
            set
            {
                _dt = value;
            }
        }


        public string Message
        {
            get
            {
                return this._message;
            }
        }


        public List<double> UComp
        {
            get
            {
                return this._UComp;
            }
        }


        public List<double> VComp
        {
            get
            {
                return this._VComp;
            }
        }


        public string WaveDirectionColName
        {
            get
            {
                return _waveDirection;
            }
            set
            {
                _waveDirection = value;
            }
        }


        public string WaveHeightColName
        {
            get
            {
                return _waveHeight;
            }
            set
            {
                _waveHeight = value;
            }
        }


        public List<string> WCompColNamesAdded
        {
            get { return this._wcompColNamesAdded; }
        }
    }
}
