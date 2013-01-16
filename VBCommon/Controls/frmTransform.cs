using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Accord;
using Accord.Math;
using Accord.Statistics;
//using VBTools;
//using VBStatistics;

namespace VBCommon.Controls
{
    /// <summary>
    /// class allows users' selection of various transforms to perform on all independent variables
    /// contained in a datatable.  dependent variable transforms are now handled elsewhere and are no
    /// longer allowed here but the responve variable is still required for estimation of the 
    /// variable/tramsform's Pearson Correlation Coefficient score.
    /// </summary>
    public partial class frmTransform : Form
    {
        private Dictionary<string,bool> _transforms = null;
        //private enum _tnames { log10, antilog10, ln, antiln, inverse, square, squareroot, quadroot, polynomial, power };
        private enum _tnames { log10, antilog10, ln, antiln, inverse, square, squareroot, quadroot, poly, power };

        private double _exp = 1.0d;
        Type _transform = typeof(_tnames);

        private Dictionary<string, bool> _dvtransforms = null;
        private enum _dvtname { none, log10, ln, generalexp };
        private double _dvexp = 1.0d;
        Type _dvtransform = typeof(_dvtname);

        private DataTable _dt = null;
        private int _depVarColIndex = -1;

        private DataTable _dtCopy = null;
        private DataTable _pcDT = null;

        /// <summary>
        /// accessor for table of vatiable/transforms and pearson scores
        /// </summary>
        public DataTable PCDT
        {
            get { return _pcDT; }
        }

        /// <summary>
        /// constructor expects the table to operate on and the response variable identified by index
        /// </summary>
        /// <param name="dt">table</param>
        /// <param name="depVarIndex">response variable index</param>
        public frmTransform(DataTable dt, int depVarIndex)
        {
            //dt should have column-filtered copy of imported datatable
            InitializeComponent();

            _transforms = new Dictionary<string, bool>();
            _dvtransforms = new Dictionary<string, bool>();
            initDics();

            _dt = dt.Copy();
            _depVarColIndex = depVarIndex;

            string dependentVarName = _dt.Columns[_depVarColIndex].ColumnName.ToString();
            lbDepVarName.Text = dependentVarName;
            
        }

        #region region - UI transform list maintenance
        private void initDics()
        {
            //throw new NotImplementedException();
            foreach (string s in Enum.GetNames(_transform))
                _transforms.Add(s, false);
            

            foreach (string s in Enum.GetNames(_dvtransform))
                _dvtransforms.Add(s, false);

        }
        private void tbrvExponent_Validating(object sender, CancelEventArgs e)
        {
            if (!double.TryParse(tbrvExponent.Text, out _dvexp))
            {
                e.Cancel = true;
                tbrvExponent.Select(0, tbrvExponent.Text.Length);
                this.errorProvider1.SetError(tbrvExponent, "Text must convert to a number.");
            }
        }

        private void tbrvExponent_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(tbrvExponent, "");
            if (rbGeneralExp.Checked)
                lbDepVarName.Text = "Power(" + _dvexp.ToString() + ")[" 
                    + _dt.Columns[_depVarColIndex].ColumnName.ToString() + "]";
                
        }
       
        private void tbExponent_Validating(object sender, CancelEventArgs e)
        {
            if (!double.TryParse(tbExponent.Text, out _exp))
            {
                e.Cancel = true;
                tbExponent.Select(0, tbExponent.Text.Length);
                this.errorProvider1.SetError(tbExponent, "Text must convert to a number.");
            }
        }

        private void tbExponent_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(tbExponent, "");
        }
        #region region - response variable
        private void rbLog10_CheckedChanged(object sender, EventArgs e)
        {
            _dvtransforms[_dvtname.log10.ToString()] = rbLog10.Checked;
            if (rbLog10.Checked) lbDepVarName.Text = "Log10[" + _dt.Columns[_depVarColIndex].ColumnName.ToString() + "]";
        }

        private void rbLn_CheckedChanged(object sender, EventArgs e)
        {
            _dvtransforms[_dvtname.ln.ToString()] = rbLn.Checked;
            if (rbLn.Checked) lbDepVarName.Text = "Ln[" + _dt.Columns[_depVarColIndex].ColumnName.ToString() + "]";
        }

        private void rbGeneralExp_CheckedChanged(object sender, EventArgs e)
        {
            _dvtransforms[_dvtname.generalexp.ToString()] = rbGeneralExp.Checked;
            if (rbGeneralExp.Checked) lbDepVarName.Text = "Power(" + _dvexp.ToString() + ")[" + _dt.Columns[_depVarColIndex].ColumnName.ToString() + "]";
            //if (rbGeneralExp.Checked) lbDepVarName.Text = "Power + [" + _dvexp.ToString() + _dt.Columns[_depVarColIndex].ColumnName.ToString() + "]";
            //if (tbrvExponent.Text == 
        }
        private void btnClearDVTs_Click(object sender, EventArgs e)
        {
            rbLog10.Checked = false;
            rbLn.Checked = false;
            rbGeneralExp.Checked = false;
            lbDepVarName.Text = _dt.Columns[_depVarColIndex].ColumnName.ToString();
            tbrvExponent.Text = "1.0";
            tbrvExponent_Validated(null, null);
            tbrvExponent.Text = "1.0";
            //errorProvider1.SetError(tbrvExponent, "");
        }
        #endregion rv
        #region region independent variables

        private void cbIVLog10_CheckedChanged(object sender, EventArgs e)
        {
            _transforms[_tnames.log10.ToString()] = cbIVLog10.Checked;
        }

        private void cbIVLn_CheckedChanged(object sender, EventArgs e)
        {
            _transforms[_tnames.ln.ToString()] = cbIVLn.Checked;
        }

        //private void cbIVAntiLog10_CheckedChanged(object sender, EventArgs e)
        //{
        //    _transforms[_tnames.antilog10.ToString()] = cbIVAntiLog10.Checked;
        //}

        //private void cbIVAntiLn_CheckedChanged(object sender, EventArgs e)
        //{
        //    _transforms[_tnames.antiln.ToString()] = cbIVAntiLn.Checked;
        //}

        private void cbInverse_CheckedChanged(object sender, EventArgs e)
        {
            _transforms[_tnames.inverse.ToString()] = cbInverse.Checked;
        }

        private void cbSquare_CheckedChanged(object sender, EventArgs e)
        {
            _transforms[_tnames.square.ToString()] = cbSquare.Checked;
        }

        private void cbSqrRoot_CheckedChanged(object sender, EventArgs e)
        {
            _transforms[_tnames.squareroot.ToString()] = cbSqrRoot.Checked;
        }

        private void cbQuadRoot_CheckedChanged(object sender, EventArgs e)
        {
            _transforms[_tnames.quadroot.ToString()] = cbQuadRoot.Checked;
        }

        private void cbPoly_CheckedChanged(object sender, EventArgs e)
        {
            _transforms[_tnames.poly.ToString()] = cbPoly.Checked;
        }

        private void cbGenExponent_CheckedChanged(object sender, EventArgs e)
        {
            //this one needs some validation on the exponent tb
            _transforms[_tnames.power.ToString()] = cbGenExponent.Checked;
        }

        private void cbSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            //ControlCollection cbs = (ControlCollection)this.Controls; 
            Control.ControlCollection cbs = gbIVs.Controls;
            if (cbSelectAll.Checked)
            {
                foreach (Control c in cbs)
                {
                    if (c.GetType() != typeof(CheckBox)) continue;
                    if (c.Name.ToString() == "cbSelectAll") continue;
                    CheckBox cb = (CheckBox)c;
                    cb.Checked = true;
                }
            }
            else
            {
                foreach (Control c in cbs)
                {
                    if (c.GetType() != typeof(CheckBox)) continue;
                    if (c.Name.ToString() == "cbSelectAll") continue;
                    CheckBox cb = (CheckBox)c;
                    cb.Checked = false;
                }
            }

            

        }
        #endregion iv
        #endregion ui maint

        /// <summary>
        /// generate the selected transforms for all ivs. 
        /// these get passed to the pearson coefficient form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGo_Click(object sender, EventArgs e)
        {
            
            Dictionary<object, object> corrResults = transforms();
            frmPcoeff pc = new frmPcoeff(corrResults, lbDepVarName.Text, _dtCopy);
            DialogResult dlgr = pc.ShowDialog();
            if (dlgr != DialogResult.Cancel)
            {
                if (pc.PCDT != null) _pcDT = pc.PCDT;
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
            }
            Close();
        }

        /// <summary>
        /// cancel the operation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// method loops through table columns and performs each of the user-selected transforms on each,
        /// calculates (or otherwise estimates) it pearson coefficient and builds a complex structure
        /// to store the information needed for further processing
        /// 
        /// storage structure is: dictionary (object1, object2) where
        /// object1 == dictionary (list(string1), list(string2)) where string1 == variable name, string2 == trnasform name
        /// object2 == dictionary (list(int), list(double)) where int == column index, double == pearson score
        /// </summary>
        /// <returns>return complex structure dictionary</returns>
        private Dictionary<object, object>  transforms()
        {

            Dictionary<object, object> corrResults = new Dictionary<object, object>();

            Dictionary<List<int>, List<double>> colPcoef = new Dictionary<List<int>, List<double>>();
            Dictionary<List<string>, List<string>> varCol = new Dictionary<List<string>, List<string>>();
            List<string> varname = new List<string>();
            List<string> cols = new List<string>();
            List<int> colndx = new List<int>();
            List<double> coeff = new List<double>();

            int colnum = -1;
            bool skip = false;
            string colname = string.Empty;
            string newcolname = string.Empty;
            List<double> varlist = null;
            List<double> abc = null;
            double parameter = double.NaN;
            List<double> depvar = null;
            double[] deparray = new double[] { };
            double[] vararray = new double[] { };
            double Pcoeff = double.NaN;
            object varval = new object();

            //DataTable dtCopy = _dt.Copy();
            _dtCopy = _dt.Copy();

            //dependent variable...
            string dvt = "none";
            colname = _dt.Columns[_depVarColIndex].ColumnName.ToString();
            foreach (KeyValuePair<string, bool> kv in _dvtransforms)
            {
                if (kv.Value)
                {
                    //transform the dependent variable
                    dvt = kv.Key;
                    colname = kv.Key.ToString() + "[" + colname + "]";
                    break;
                }
            }

            depvar = new List<double>();
            doTransform(out depvar, _dt, _depVarColIndex, dvt, out parameter);
            deparray = depvar.ToArray();
            if (!_dtCopy.Columns.Contains(colname)) colnum = addColumnToDT(colname, colname, _dtCopy, depvar);

            //independent variables...
            foreach (DataColumn dc in _dt.Columns)
            {
                if (_dt.Columns.IndexOf(dc) == 0) continue; //skip datetime
                if (_dt.Columns.IndexOf(dc) == _depVarColIndex) continue; //skip dependent variable column
                var distinctVals = (from row in _dt.Select() select row[dc]).Distinct();
                if (distinctVals.Count() <= 2) continue; //skip CAT vars
                if (!Information.IsNumeric(distinctVals.First().ToString())) continue; //skip alpha data columns

                varlist = new List<double>();
                vararray = new double[_dt.Rows.Count];
                double adjr2 = double.NaN;
                double r2 = double.NaN;

                //...for each valid variable, add the untransformed variable to lists...
                colname = dc.ColumnName.ToString();
                skip = doTransform(out varlist, _dt, _dt.Columns.IndexOf(dc), "none", out parameter);
                if (!skip)
                {
                    vararray = varlist.ToArray();
                    Pcoeff = deparray.Covariance(vararray) / deparray.StandardDeviation() / vararray.StandardDeviation(); //Statistics.Correlation(deparray, vararray);
                    varname.Add(colname);
                    cols.Add(colname);
                    colndx.Add(_dt.Columns.IndexOf(dc));
                    coeff.Add(Pcoeff);
                }

                //foreach selected transform...
                foreach (KeyValuePair<string, bool> kv in _transforms)
                {
                    if (kv.Value)
                    {
                        //...do the transform on the iv and add to lists
                        varlist = new List<double>();
                        
                        vararray.Initialize();
                        adjr2 = double.NaN;
                        r2 = double.NaN;
                        newcolname = kv.Key.ToString().ToUpper() + "[" + colname + "]";
                        //special handling for polynomial transform - compute pearson coeff differently
                        if (kv.Key.ToString() == Enum.GetName(_transform, _tnames.poly))
                        {
                            abc = new List<double>();
                            skip = doTransform(out varlist, _dt, _dt.Columns.IndexOf(dc), out adjr2, out r2, out abc);
                            if (!skip)
                            {
                                vararray = varlist.ToArray();
                                if (adjr2 < 0.1)
                                {
                                    Pcoeff = polyCorrCoeff(adjr2, r2, varlist.Count);
                                }
                                else Pcoeff = Math.Sqrt(adjr2);
                                //Pcoeff = Statistics.Correlation(deparray, vararray);
                                //skip variable transforms that have invalid correlation coefficients....
                                if (Pcoeff.Equals(double.NaN)) continue;
                                newcolname = kv.Key.ToString().ToUpper() + "[" + colname + "," + abc[0].ToString("g8") + "," + abc[1].ToString("g8") + "," + abc[2].ToString("g8") + "]";
                                //newcolname = kv.Key.ToString().ToUpper() + "[" + abc[0].ToString("g8") + "," + abc[1].ToString("g8") + "," + abc[2].ToString("g8") + "," + colname + "]";
                                //newcolname = kv.Key.ToString().ToUpper() + "[" + abc[0].ToString("######.######") + "," + abc[1].ToString("######.######") + "," + abc[2].ToString("#######.#######") + "," + colname + "]";
                                colnum = addColumnToDT(colname, newcolname, _dtCopy, varlist);
                                varname.Add(colname);
                                cols.Add(newcolname);
                                colndx.Add(colnum);
                                coeff.Add(Pcoeff);
                            }
                        }
                        //regular vb2 transform
                        else
                        {
                            skip = doTransform(out varlist, _dt, _dt.Columns.IndexOf(dc), kv.Key.ToString(), out parameter);
                            if (!skip)
                            {
                                vararray = varlist.ToArray();
                                Pcoeff = deparray.Covariance(vararray) / vararray.StandardDeviation() / deparray.StandardDeviation(); // VBCommon.Statistics.Correlation(deparray, vararray);
                                //skip variable transforms that have invalid correlation coefficients....
                                if (Pcoeff.Equals(double.NaN)) continue;
                                if (kv.Key.ToString() == Enum.GetName(_transform, _tnames.power) ||
                                    kv.Key.ToString() == Enum.GetName(_transform, _tnames.inverse))
                                    newcolname = kv.Key.ToString().ToUpper() + "[" + colname + "," + parameter.ToString("g8") + "]";
                                    //newcolname = kv.Key.ToString().ToUpper() + "(" + parameter.ToString("g2") + ")" + "[" + colname + "]";
                                    //newcolname = kv.Key.ToString().ToUpper() + "[" + parameter.ToString("g8") + "," + colname + "]";

                                colnum = addColumnToDT(colname, newcolname, _dtCopy, varlist);
                                varname.Add(colname);
                                cols.Add(newcolname);
                                colndx.Add(colnum);
                                coeff.Add(Pcoeff);
                            }
                        }
                    }
                }


            }
            varCol.Add(varname, cols);
            colPcoef.Add(colndx, coeff);
            corrResults.Add(varCol, colPcoef);

            return corrResults;
        }

        /// <summary>
        /// performs the trnasform calculation 
        /// </summary>
        /// <param name="vals">return an array of transformed values</param>
        /// <param name="dt">table containing operand</param>
        /// <param name="colndx">index in table of operand</param>
        /// <param name="t">the transform to perform</param>
        /// <param name="p">return value of the exponent iff t==power or minnonzerovalue iff t==inverse</param>
        /// <returns>true if can perform transform calculation, false otherwise</returns>
        private bool doTransform(out List<double> vals, DataTable dt, int colndx, string t, out double p)
        {
            //use transform class to compute transforms
            //note: return false iff ok, true iff transform failed
            //private enum _tnames { log10, antilog10, ln, antiln, inverse, square, squareroot, quadroot, polynomial, generalexp };
            vals = null;
            p = double.NaN;
            VBCommon.Transforms.Transformer trans = new VBCommon.Transforms.Transformer(dt, colndx);
            switch (t.ToString())
            {
                case "none":
                    vals = trans.NONE.ToList<double>();
                    break;
                case "square":
                    vals = trans.SQUARE.ToList<double>();
                    break;
                case "sqrroot":
                    vals = trans.SQUAREROOT.ToList<double>();
                    break;
                case "squareroot":
                    vals = trans.SQUAREROOT.ToList<double>();
                    break;
                case "logbasee":
                    vals = trans.LOGE.ToList<double>();
                    break;
                case "ln":
                    vals = trans.LOGE.ToList<double>();
                    break;
                case "inverse":
                    vals = trans.INVERSE.ToList<double>();
                    p = trans.MinNonZero;
                    break;
                case "logbase10":
                    vals = trans.LOG10.ToList<double>();
                    break;
                case "log10":
                    vals = trans.LOG10.ToList<double>();
                    break;
                case "quarticroot":
                    vals = trans.QUARTICROOT.ToList<double>();
                    break;
                case "quadroot":
                    vals = trans.QUARTICROOT.ToList<double>();
                    break;
                //case "antilog10":
                //    vals = trans.ANTILOG10.ToList<double>();
                //    break;
                //case "antiln":
                //    vals = trans.ANTILn.ToList<double>();
                //    break;
                case "power":
                    trans = new VBCommon.Transforms.Transformer(dt, colndx, _exp);
                    vals = trans.POWER.ToList<double>();
                    p = trans.Exponent;
                    break;
            }
            //if transform failed...well this is a stupid test - trans returns a full array of zeros if failed
            //if ( vals.Count < 1 || vals.Equals(null)) return true;
            //int count = _data.Count(n => n == 0);
            int zerocount = vals.Count(n => n == 0);
            if (zerocount == vals.Count) return true;
            return false;

        }

        /// <summary>
        /// method performs a polynomial transform.  this is a special case requiring a regression calculation
        /// and differnt method for calculating the semi-equivalent value for a Pearson score.  the polynomial 
        /// transformation is documented in the user guide.  see the vbtools.polynomial class for further information
        /// </summary>
        /// <param name="vals"></param>
        /// <param name="dt"></param>
        /// <param name="colndx"></param>
        /// <param name="adjr2"></param>
        /// <param name="r2"></param>
        /// <param name="abc"></param>
        /// <returns></returns>
        private bool doTransform(out List<double> vals, DataTable dt, int colndx, out double adjr2, out double r2, out List<double> abc)
        {
            //use polynomial class to compute transforms
            //note: return false iff ok, true iff transform failed
            adjr2 = double.NaN;
            r2 = double.NaN;
            //private enum _tnames { log10, antilog10, ln, antiln, inverse, square, squareroot, quadroot, polynomial, generalexp };
            vals = null;
            abc = new List<double>();

            VBCommon.Statistics.Polynomial poly = new VBCommon.Statistics.Polynomial(_dt, colndx);
            vals = poly.getPolyT.ToList<double>();
            adjr2 = poly.getAdjRsqrd;
            r2 = poly.getRsqrd;
            abc.Add(poly._intercept);
            abc.Add(poly._c1);
            abc.Add(poly._c2);
            int zerocount = vals.Count(n => n == 0);
            if (zerocount == vals.Count) return true;
            return false;
        }

        /// <summary>
        /// method calculates the semi-equivalent of a polynomial transformation Pearson score
        /// </summary>
        /// <param name="adjrsqrd"></param>
        /// <param name="rsqrd"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private double polyCorrCoeff(double adjrsqrd, double rsqrd, int n)
        {
            //throw new NotImplementedException();
            //RatioEstimate1 = 1.015 - 1.856*R-square +1.862*AdjR-Square - 0.000153*n
            //RatioEstimate2 = -6.672*RatioEstimate1^2 + 13.91*RatioEstimate1 - 6.241
            //...then multiple RatioEstimate2 by the square root of the polynomial R-square...
            double est1 = 1.015 - (1.856 * rsqrd) + (1.862 * adjrsqrd) - (0.000153 * n);
            double est2 = (-6.672 * Math.Pow(est1, 2.0)) + (13.91 * est1) - 6.241;
            double ans = est2 * Math.Sqrt(rsqrd);
            return ans;
        }

        /// <summary>
        /// utility method to add a data column and values to a datatable
        /// </summary>
        /// <param name="oldcol">column name to ascertain by comparison to colname whether new column is a transform</param>
        /// <param name="colname">column name to add</param>
        /// <param name="dt">table to operate on</param>
        /// <param name="values">list of values to add</param>
        /// <returns>index of column added to table</returns>
        private int addColumnToDT(string oldcol, string colname, DataTable dt, List<double> values)
        {

            dt.Columns.Add(colname, typeof(double));
            if (oldcol != colname)
            {
                bool isoperated = dt.Columns[oldcol].ExtendedProperties.ContainsKey(VBCommon.Globals.OPERATION);
                if (isoperated) dt.Columns[colname].ExtendedProperties[VBCommon.Globals.OPERATION] = true;
            }
            int colndx = dt.Columns.Count - 1;
            for (int r = 0; r < dt.Rows.Count; r++)
            {
                dt.Rows[r][colndx] = values[r];
            }
            return colndx;
        }

        private void frmTransform_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            /*string apppath = Application.StartupPath.ToString();
            VBCSHelp help = new VBCSHelp(apppath, sender);
            if (!help.Status)
            {
                MessageBox.Show(
                "User documentation is found in the Documentation folder where you installed Virtual Beach"
                + "\nIf your web browser is PDF-enabled, open the User Guide with your browser.",
                "Neither Adobe Acrobat nor Adobe Reader found.",
                MessageBoxButtons.OK);
            }*/
        }



    }


}
