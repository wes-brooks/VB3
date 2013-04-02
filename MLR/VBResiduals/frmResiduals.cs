using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using VBTools;
using VBControls;
using ZedGraph;
//using VBStatistics;
using VBCommon.Statistics;
using VBProjectManager;
using Microsoft.VisualBasic;

namespace VBResiduals
{
    public partial class frmResiduals : DockContent
    {
        private const double cutoff = 0.65d;
        private const double cookscutoff = 0.05d;

        //private VBProjectManager _projMgr = null;
        private double[] _dffits = null;
        private double[] _cooks = null;
        private double _dffitsThreshold = cutoff;
        private double _cooksThreshold = cookscutoff;

        private double[] _predictions = null;
        private double[] _standardResiduals = null;
        private double[] _observations = null;
        //private double[] _standardResiduals = new double[]{1};

        private MultipleRegression _model = null;

        private int _dffitsRecno2Remove;
        private int _cooksRecno2Remove;

        private string _dffitsDateTime2Remove;
        private string _cooksDataTime2Remove;

        private double _dffitsResidValue2Remove;
        private double _cooksResidValue2Remove;

        private List<int> _recsRemoved = null;
        private List<double> _residValueRemoved = null;
        private List<string> _residTypeRemoved = null;

        
        private DataSet _modelBuildTables = null;
        private string _currentModelTableName = string.Empty;

        private bool _gobtnState = true;

        private Dictionary<string, double> _selectedModel = null;
        //private bool _newModelSelected = true;
        private int _maxIterations = 0;

        private Dictionary<int, string> _residualInfo = null;

        private bool _projectOpened = false;

        private int _selectedRebuild = -1;

        private enum _residState { clean, dirty };
        private _residState _state = _residState.clean;

        //public delegate bool GoDFFITSRebuild(object sender, System.EventArgs e);
        //public delegate bool GoCooksRebuild(object sender, System.EventArgs e);
        private bool _continue = true;


        public frmResiduals()
        {
            InitializeComponent();
            _projMgr = VBProjectManager.GetProjectManager();
            _modelBuildTables = new DataSet();
            _residualInfo = new Dictionary<int, string>();

            _projMgr.ProjectOpened += new VBProjectManager.ProjectOpenedHandler(ProjectOpenedListener);
            _projMgr.ProjectSaved += new VBProjectManager.ProjectSavedHandler(ProjectSavedListener);
        }

 

        private void ProjectSavedListener()
        {

            //VBTools.Globals.ProjectType pmType = _projMgr._projectType;
            //if (pmType == VBTools.Globals.ProjectType.COMPLETE) return;

            //Something must be seriously messed up if this happens.
            if (_projMgr == null)
                return;

            if (_projMgr.ResidualAnalysisInfo == null)
                _projMgr.ResidualAnalysisInfo = new ResidualAnalysisInfo();

            //don't want to save data if datasheet dirty (or another model selected??)
            if (!_projMgr.DataSheetInfo.Clean)
                return;

            _projMgr.ResidualAnalysisInfo.ReBuildInfo = _residualInfo;
            _projMgr.ResidualAnalysisInfo.ModelIndependentVariables = _projMgr.ModelIndependentVariables;
            _projMgr.ResidualAnalysisInfo.Model = _projMgr.Model;
            _projMgr.ResidualAnalysisInfo.SelectedRebuild = _selectedRebuild;

            if (_state == _residState.clean)
            {
                _projMgr.TabStates.TabState["Prediction"] = true;
            }
            else
            {
                _projMgr.TabStates.TabState["Prediction"] = false;
            }

        }

        private void ProjectOpenedListener()
        {
            //do we want to do anything at all here?  think not.  this guy is never re-populated for either projects or models.
            return;



            //Something must be seriously messed up if this happens.
            if (_projMgr == null)
                return;

            if (_projMgr.ResidualAnalysisInfo == null)
                return;

            if (_projMgr.Model.Count <= 0)
                return;

            VBTools.Globals.ProjectType pmType = _projMgr._projectType;
            if (pmType == VBTools.Globals.ProjectType.COMPLETE) return;

            _projMgr.Model = _projMgr.ResidualAnalysisInfo.Model;
            _projMgr.ModelDataTable = _projMgr.CorrelationDataTable;


            if (_projMgr.ModelIndependentVariables == null) return;

            _projectOpened = true;


            //if (listBox1.Items.Count > 1)
            //    for (int i = 1; i < listBox1.Items.Count; i++)
            //        listBox1.Items.RemoveAt(i);
            //listBox1.Items.Clear();

            //_modelBuildTables = new DataSet();

            //local copy of rebuild info to generate saved rebuilds
            Dictionary<int, string> residualinfo = _projMgr.ResidualAnalysisInfo.ReBuildInfo;

            _currentModelTableName = "Selected Model";

            //fire events to repopulate the form with selected model...
            if (_projMgr.ModelDataTable != null)
            {
                frmResiduals_Enter(null, null);
                //...and each of the rebuilds.
                foreach (KeyValuePair<int, string> kv in residualinfo)
                {
                    switch (kv.Value.ToString())
                    {
                        case ("df"):
                            btnGoDFFITSRebuild_Click(null, null);
                            break;
                        case ("cd"):
                            btnGoCooksIterative_Click(null, null);
                            break;
                    }

                }
                listBox1.SelectedIndex = _projMgr.ResidualAnalysisInfo.SelectedRebuild;

                _state = _residState.clean;
            }

        }


        private void frmResiduals_Enter(object sender, EventArgs e)
        {
            _state = _residState.clean;

            Dictionary<string, double> modeldic = _projMgr.Model;

            //determine if a new model has been selected, do nothing if not
            if (modeldic.Keys.Count > 0)
            {
                if (!newModelSelected(modeldic)) return;
            }

            if (_projMgr.ModelDataTable == null) return;



            _residualInfo = new Dictionary<int, string>();

            DataTable modeldt = _projMgr.ModelDataTable;

            _gobtnState = true;


            //cutoff rebuilds when half data records eliminated
            _maxIterations = modeldt.Rows.Count / 2;

            if (modeldt != null)
            {

                MultipleRegression model = computeModel(modeldt);

                if (model != null)
                {
                    _recsRemoved = new List<int>();
                    _residValueRemoved = new List<double>();
                    _residTypeRemoved = new List<string>();

                    listBox1.Items.Clear();
                    _modelBuildTables.Tables.Clear();

                    DataTable dffits = getDFFITSTable(model, modeldt, true);
                    DataTable cooks = getCooksDistanceTable(model, modeldt, true);
                    updateModelList(model, "original", modeldt, "model");

                    //create rediduals plots
                    createResidPlot("DFFITS", dffits);
                    createResidPlot("CooksDistance", cooks);

                    //predictions vs standardized residuals plot
                    createPlot2(model);

                    //initialize dffits threshold value/label
                    double dffitsThreshold = Convert.ToDouble( (2.0d * Math.Sqrt(((double)(CreateModelDataTable()).Columns.Count - 2) / (double)model.PredictedValues.Length)).ToString());
                    label3.Text = label3.Text.Substring(0, 13) + dffitsThreshold.ToString("f4");
                    double dfcutoff = dffitsThreshold;
                    textBox3.Text = dfcutoff.ToString("f4");
                    _dffitsThreshold = dfcutoff;
                    //rbConstantCutoff.Checked = true;
                    rbIterativeCutoff.Checked = true;

                    //initialize cooksd threshold value/label
                    double cooksThreshold = 4.0d / (double)model.PredictedValues.Length;
                    label10.Text = label10.Text.Substring(0, 10) + cooksThreshold.ToString("f4");
                    tbcookcutoff.Text = cooksThreshold.ToString("f4");
                    _cooksThreshold = cooksThreshold;
                    //rbCookConstantCutoff.Checked = true;
                    rbCooksIterativeCutoff.Checked = true;
                }
            }


        }

        /// <summary>
        /// Get a table of mlr variables/values winnowed from the modeling table
        /// </summary>
        /// <returns></returns>
        private DataTable CreateModelDataTable()
        {
            DataTable dtCorr = _projMgr.CorrelationDataTable;
            DataView dvCorr = dtCorr.DefaultView;

            List<string> list = new List<string>();

            list.Add(dtCorr.Columns[0].ColumnName);
            list.Add(dtCorr.Columns[1].ColumnName);

            //int numVars = lbIndVariables.Items.Count;
            //for (int i = 0; i < numVars; i++)
            //    list.Add(lbIndVariables.Items[i].ToString());
            //list = _projMgr.ModelingInfo.IndependentVariables;

            foreach (KeyValuePair<string, double> kvp in _projMgr.ModelingInfo.Model)
                if (kvp.Key != "(Intercept)") list.Add(kvp.Key);



            DataTable dtModel = dvCorr.ToTable("ModelData", false, list.ToArray());

            return dtModel;
        }

        private bool newModelSelected(Dictionary<string, double> modeldic)
        {
            //_state = _residState.dirty;
            _state = _residState.clean;

            if (_selectedModel == null)
            {
                _selectedModel = modeldic;
                _state = _residState.dirty;
                return true;
            }

            if (_projectOpened)
            {
                _projectOpened = false;
                return true;
            }

            if (_selectedModel.Keys.Count == modeldic.Keys.Count)
            {
                foreach (KeyValuePair<string, double> kv in modeldic)
                {
                    if (!_selectedModel.ContainsKey(kv.Key))
                    {
                        _selectedModel = modeldic;
                        _state = _residState.dirty;
                        return true;
                    }

                }
                _state = _residState.clean;
                return false;
            }
            else
            {
                _selectedModel = modeldic;
                _state = _residState.dirty;
                return true; ;
            }

        }

        private DataTable getDFFITSTable(MultipleRegression model, DataTable dt, bool prepare)
        {
            //given a model, get dffits stat. passed dt is data used in model build
            _dffits = model.DFFITS;

            //build a sorted table/view for UI display
            DataTable dtDFFITS = getResidualsTable("DFFITS", _dffits, dt);
            DataView view = dtDFFITS.DefaultView;
            view.Sort = "DFFITS DESC";
            dgv1.DataSource = view;

            //prepare for next rebuild
            if (prepare)
            {
                //if (!_dffitsRecs2Remove.Contains(_dffitsRecno2Remove))
                {
                    _dffitsRecno2Remove = Convert.ToInt32(dgv1.Rows[0].Cells[0].Value.ToString());
                    _dffitsDateTime2Remove = dgv1.Rows[0].Cells[1].Value.ToString();
                    _dffitsResidValue2Remove = Convert.ToDouble(dgv1.Rows[0].Cells[2].Value.ToString());
                }
            }

            //update the dffits threshold (2*sqr(p/n)) where p = # of predictor variables and n = # of observations
            //(p=cols-2; 1 is timestamp, 2 is dependent variable)
            //double dffitsThreshold = Convert.ToDouble(
            //    (2.0d * Math.Sqrt(((double)dt.Columns.Count - 2) / (double)model.PredictedValues.Length)).ToString());

            //issue with cutoff - need variable count in model, not variable count in datatable
            int p = CreateModelDataTable().Columns.Count - 2;
            double dffitsThreshold = Convert.ToDouble(
                (2.0d * Math.Sqrt(( (double)p) / (double)model.PredictedValues.Length)).ToString());

            label3.Text = label3.Text.Substring(0, 13) + dffitsThreshold.ToString("f4");


            if (rbConstantCutoff.Checked)
            {
                //textBox3.Text = dffitsThreshold.ToString("f4");
                //label3.Text = label3.Text.Substring(0, 13) + dffitsThreshold.ToString("f4");
                //_dffitsThreshold = dffitsThreshold;
            }
            else if (rbIterativeCutoff.Checked)
            {
                //textBox3.Text = dffitsThreshold.ToString("f4");
                _dffitsThreshold = dffitsThreshold;
            }


            return dtDFFITS;
        }

        private DataTable getCooksDistanceTable(MultipleRegression model, DataTable dt, bool prepare)
        {
            _cooks = model.Cooks;

            DataTable dtCooks = getResidualsTable("CooksDist", _cooks, dt);
            DataView view = dtCooks.DefaultView;
            view.Sort = "CooksDist Desc";
            dgvCooks.DataSource = view;

            //int n = model.PredictedValues.Length; // number of observations
            //int p = dt.Columns.Count - 2; // number of variables
            //VBStatistics.FDistribution fstat = new FDistribution(.05, p, n-p);
            //double cutoff = fstat.FStat;


            if (prepare)
            {

                //if (!_cooksRecs2Remove.Contains(_cooksRecno2Remove))
                {
                    //save the record, value and type to remove
                    _cooksRecno2Remove = Convert.ToInt32(dgvCooks.Rows[0].Cells[0].Value.ToString());
                    _cooksDataTime2Remove = dgvCooks.Rows[0].Cells[1].Value.ToString();
                    _cooksResidValue2Remove = Convert.ToDouble(dgvCooks.Rows[0].Cells[2].Value.ToString());
                }

            }

            //update the cooks threshold (4/n) 
            double cooksThreshold = 4.0d / (double)model.PredictedValues.Length;

            label10.Text = label10.Text.Substring(0, 10) + cooksThreshold.ToString("f4");

            if (rbCookConstantCutoff.Checked)
            {
                //tbcookcutoff.Text = cooksThreshold.ToString("f4");
            }
            else if (rbCooksIterativeCutoff.Checked)
            {
                //label10.Text = label10.Text.Substring(0, 10) + _cooksThreshold.ToString("f4");
                _cooksThreshold = cooksThreshold;
            }


            return dtCooks;
        }

        private DataTable getResidualsTable(string residName, double[] residuals, DataTable dtModel)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Record", typeof(System.Int32));
            dt.Columns.Add("Date/Time", typeof(System.String));
            dt.Columns.Add(residName, typeof(System.String));
            object[] values = new object[3];

            for (int rec = 0; rec < dtModel.Rows.Count; rec++)
            {
                int recno = rec;
                values[0] = Convert.ToInt32(string.Format("{0:N0}", recno.ToString()));
                values[1] = dtModel.Rows[rec][0].ToString();
                values[2] = residuals[rec].ToString("F6");
                dt.Rows.Add(values);
            }

            return dt;
        }

        private void btnGoDFFITSRebuild_Click(object sender, EventArgs e)
        {
            //iterative rebuild button clicked, get the current model table name
            //copy the table, remove the record with the largest dffits stat, rebuild the model
            //get the dffits stats and table, update the model list and recreate the plots

            DataTable dt = _modelBuildTables.Tables[_currentModelTableName];
            DataTable opdt = dt.Copy();

            if (opdt.Rows.Count <= 10)
            {
                MessageBox.Show("Data insufficient for further record removal.", "Maximum Number of Records Removed", MessageBoxButtons.OK);
                _continue = false;
                return; // false;
            }

            opdt.Rows[_dffitsRecno2Remove].Delete();
            opdt.AcceptChanges();

            //track removed records here...
            //_recsRemoved.Add(_dffitsRecno2Remove);
            _recsRemoved.Add(getRecordRemoved(_dffitsDateTime2Remove));
            _residValueRemoved.Add(_dffitsResidValue2Remove);
            _residTypeRemoved.Add("DFFITS");

            _model = computeModel(opdt);

            DataTable dffits = getDFFITSTable(_model, opdt, true);
            DataTable cooks = getCooksDistanceTable(_model, opdt, true);

            updateModelList(_model, "next", opdt, "df");
            createResidPlot("DFFITS", dffits);
            createResidPlot("CooksDistance", cooks);
            createPlot2(_model);
            _continue = true;

        }


        private void btnGoCooksIterative_Click(object sender, EventArgs e)
        {
            DataTable dt = _modelBuildTables.Tables[_currentModelTableName];
            DataTable opdt = dt.Copy();

            if (opdt.Rows.Count <= 10)
            {
                MessageBox.Show("Data insufficient for further record removal.", "Maximum Number of Records Removed", MessageBoxButtons.OK);
                _continue = false;
                return; // false;
            }
            //_recsRemoved.Add(_cooksRecno2Remove);
            opdt.Rows[_cooksRecno2Remove].Delete();
            opdt.AcceptChanges();

            //track removed records here...
            //_recsRemoved.Add(_cooksRecno2Remove);
            _recsRemoved.Add(getRecordRemoved(_cooksDataTime2Remove));
            _residValueRemoved.Add(_cooksResidValue2Remove);
            _residTypeRemoved.Add("COOKS");

            _model = computeModel(opdt);

            DataTable dffits = getDFFITSTable(_model, opdt, true);
            DataTable cooks = getCooksDistanceTable(_model, opdt, true);

            updateModelList(_model, "next", opdt, "cd");
            createResidPlot("DFFITS", dffits);
            createResidPlot("CooksDistance", cooks);
            createPlot2(_model);
            _continue = true;

        }

        private int getRecordRemoved(string datetimestamp)
        {
            //wouldn't have to do this if grids weren't sorted (and with records recursively removed)
            int ndx = -1;
            foreach (DataRow r in _modelBuildTables.Tables[0].Rows)
            {
                ndx++;
                if (r[0].ToString() != datetimestamp) continue;
                //otherwise we found it
                return ndx;
            }
            return -1;
        }

        private void btnGoDFFITSAuto_Click(object sender, EventArgs e)
        {
            //auto rebuild until dffits < cutoff 
            //this will need some exit criteria before we run out of data
            //and/or mlr models cannot be built due to lack of data

            double maxDFFITS = Math.Abs(Convert.ToDouble(dgv1[2, 0].Value.ToString()));

            while (maxDFFITS >= _dffitsThreshold)
            {
                if (_continue)
                    btnGoDFFITSRebuild_Click(null, new EventArgs());
                else
                   break;

                maxDFFITS = Math.Abs(Convert.ToDouble(dgv1[2, 0].Value.ToString()));

                DataTable dt = _modelBuildTables.Tables[_currentModelTableName];
                if (dt.Rows.Count < _maxIterations) break;
            }

        }

        private void btnGoCooksAuto_Click(object sender, EventArgs e)
        {
            //auto rebuild until cooks < cutoff 
            //this will need some exit criteria before we run out of data
            //and/or mlr models cannot be build due to lack of data

            double maxCOOKS = Math.Abs(Convert.ToDouble(dgvCooks[2, 0].Value.ToString()));

            while (maxCOOKS >= _cooksThreshold)
            {
                if (_continue)
                    btnGoCooksIterative_Click(null, new EventArgs());
                else
                    break;

                maxCOOKS = Math.Abs(Convert.ToDouble(dgvCooks[2, 0].Value.ToString()));

                DataTable dt = _modelBuildTables.Tables[_currentModelTableName];
                if (dt.Rows.Count < _maxIterations) break;

            }
        }

        private void updateModelList(MultipleRegression model, string name, DataTable dt, string residType)
        {
            //track models by their build tables and name

            //create a table name
            string listitem = string.Empty;
            int n = _modelBuildTables.Tables.Count;
            if (name == "original")
            {
                listitem = "SelectedModel";
            }
            else
            {
                listitem = "Rebuild" + n;
            }

            //track rebuild info
            _residualInfo.Add(n, residType);

            //copy the model table and save it
            DataTable newDT = new DataTable();
            newDT = dt.Copy();
            newDT.TableName = listitem.ToString();
            if (!_modelBuildTables.Tables.Contains(listitem.ToString()))
                _modelBuildTables.Tables.Add(newDT);

            //add the model (name) to the UI list
            listBox1.Items.Add(listitem);
            //and save the name for use elsewhere
            _currentModelTableName = listitem;

            //update the model stats UI lists
            updateListViews(model);
            //textBox3.Text = _dffitsThreshold.ToString("f4");

            //and check if we can still rebuild models with remaining data... this need work...
            if (newDT.Rows.Count < 3) _gobtnState = false;
            btnGoDFFITSRebuild.Enabled = _gobtnState;
            btnGoDFFITSAuto.Enabled = _gobtnState;

        }

        private void updateListViews(MultipleRegression model)
        {
            //given a mlr model, update the UI lists with its stats
            listView1.Items.Clear();
            listView2.Items.Clear();

            string[] item = null;
            ListViewItem lvi = null;

            //show variable statistics
            int numColumns = model.Parameters.Columns.Count;
            for (int i = 0; i < model.Parameters.Rows.Count; i++)
            {
                item = new string[numColumns];
                item[0] = model.Parameters.Rows[i][0].ToString();
                item[1] = formatNumber((double)model.Parameters.Rows[i][1]);
                item[2] = formatNumber((double)model.Parameters.Rows[i][2]);
                item[3] = formatNumber((double)model.Parameters.Rows[i][3]);
                item[4] = formatNumber((double)model.Parameters.Rows[i][4]);
                item[5] = formatNumber((double)model.Parameters.Rows[i][5]);
                lvi = new ListViewItem(item);
                listView1.Items.Add(lvi);
            }

            //show model statistics
            item = new string[2];
            item[0] = "R Squared";
            item[1] = String.Format("{0:F4}", model.R2);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "Adjusted R Squared";
            item[1] = String.Format("{0:F4}", model.AdjustedR2);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "Akaike Information Criterion";
            item[1] = String.Format("{0:F4}", model.AIC);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "Corrected AIC";
            item[1] = String.Format("{0:F4}", model.AICC);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "Bayesian Info Criterion";
            item[1] = String.Format("{0:F4}", model.BIC);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "PRESS";
            item[1] = String.Format("{0:F4}", model.Press);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "RMSE";
            item[1] = String.Format("{0:F4}", model.RMSE);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);


            //MLRPredObs mlrobs = new MLRPredObs();

            ModelErrorCounts mec = new ModelErrorCounts();
            //mec.getCounts(mlrPredObs1.ThresholdHoriz, mlrPredObs1.ThresholdVert, model.PredictedValues, model.ObservedValues);
            mec.getCounts(mlrPlots1.ThresholdHoriz, mlrPlots1.ThresholdVert, model.PredictedValues, model.ObservedValues);

            item = new string[2];
            item[0] = "";
            item[1] = "";
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            //item[0] = "Decision Criterion"; 
            item[0] = "Transformed DC";
            //item[1] = string.Format("{0:F4}", mlrPlots1.ThresholdHoriz);
            item[1] = string.Format("{0:F4}", _projMgr.ModelingInfo.DecisionThreshold);

            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            //item[0] = "Regulatory Standard";
            item[0] = "Transformed RS";
            //item[1] = string.Format("{0:F4}", mlrPlots1.ThresholdVert);
            item[1] = string.Format("{0:F4}", _projMgr.ModelingInfo.MandatedThreshold);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "False Positives";
            item[1] = string.Format("{0:n}", mec.FPCount);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "Specificity";
            item[1] = String.Format("{0:F4}", mec.Specificity);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "False Negatives";
            item[1] = string.Format("{0:n}", mec.FNCount);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "Sensitivity";
            item[1] = String.Format("{0:F4}", mec.Sensitivity);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            //item = new string[2];
            //item[0] = "Specificity";
            //item[1] = String.Format("{0:F4}", mec.Specificity);
            //lvi = new ListViewItem(item);
            //listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "Accuracy";
            item[1] = String.Format("{0:F4}", mec.Accuracy);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "";
            item[1] = "";
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            int numRecords = model.PredictedValues.Length;
            item = new string[2];
            item[0] = "Number of Observations";
            item[1] = string.Format("{0}", numRecords);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);
        }

        private string formatNumber(double number)
        {
            string fmtstring = string.Empty;
            if (number.Equals(double.NaN)) return fmtstring;
            if (Math.Abs(number) >= 10000 || Math.Abs(number) <= 0.0001)
                fmtstring = "{0:0.000#e-00}";
            else fmtstring = "{0:f4}";
            return string.Format(fmtstring, number);
        }

        private void createResidPlot(string plotname, DataTable residuals)
        {
            ZedGraphControl zgcCtrl = null;

            //switch to correct plot control
            if (plotname.Contains("DFFITS")) zgcCtrl = zgcDFFITS;
            else if (plotname.Contains("Cooks")) zgcCtrl = zgcCooks;
            else return;

            GraphPane pane = zgcCtrl.GraphPane;
            pane.Title.Text = "Residuals";
            if (pane.CurveList.Count > 0) pane.CurveList.Clear();
            PointPairList pts = getPPList(residuals, false);
            LineItem curve = pane.AddCurve(plotname, pts, Color.Blue, SymbolType.Default);
            pane.XAxis.Title.Text = "Record";
            pane.YAxis.Title.Text = plotname;
            curve.Line.IsVisible = false;

            if (zgcCtrl.Equals(zgcDFFITS))
            {
                //add threshold traces
                PointPairList dthreshold1 = new PointPairList();
                dthreshold1.Add(0.0d, _dffitsThreshold);
                dthreshold1.Add(Convert.ToDouble(residuals.Rows.Count.ToString()), _dffitsThreshold);

                PointPairList dthreshold2 = new PointPairList();
                dthreshold2.Add(0.0d, -_dffitsThreshold);
                dthreshold2.Add(Convert.ToDouble(residuals.Rows.Count.ToString()), -_dffitsThreshold);

                LineItem dcurve1 = pane.AddCurve("cutoff = " + _dffitsThreshold.ToString("f4"), dthreshold1, Color.Red, SymbolType.None);
                dcurve1.Line.IsVisible = true;
                LineItem dcurve2 = pane.AddCurve("-cutoff  = -" + _dffitsThreshold.ToString("f4"), dthreshold2, Color.Red, SymbolType.None);
                dcurve2.Line.IsVisible = true;
            }
            else if (zgcCtrl.Equals(zgcCooks))
            {
                //add threshold trace
                PointPairList cthreshold = new PointPairList();
                cthreshold.Add(0.0d, _cooksThreshold);
                cthreshold.Add(Convert.ToDouble(residuals.Rows.Count.ToString()), _cooksThreshold);

                LineItem dcurve1 = pane.AddCurve("cutoff = " + _cooksThreshold.ToString("f4"), cthreshold, Color.Red, SymbolType.None);
                dcurve1.Line.IsVisible = true;
            }

            zgcCtrl.AxisChange();
            zgcCtrl.Refresh();
        }

        private void createPlot2(MultipleRegression model)
        {

            MasterPane master = zgc2.MasterPane;
            master.PaneList.Clear();

            master.Title.IsVisible = false;
            master.Margin.All = 10;

            //create the predictions vs studentized residuals plot;
            _predictions = model.PredictedValues;
            _standardResiduals = model.StudentizedResiduals;
            _observations = model.ObservedValues;
            
            
            //AndersonDarlingNormality adtest = new AndersonDarlingNormality();
            //adtest.getADstat(_standardResiduals);
            //double adstat1 = adtest.ADStat;
            //double adstat1pval = adtest.ADStatPval;

            //get different values of adstat with direct call (above)
            //...from this below which uses the adtestofnormality directly in the linearmodel object... (see statistics.multipleregression.compute)
            //won't match numbers passing residuals or standardized residuals to the test above - revisit this when possible
            double adstat1 = model.ADResidNormStatVal;
            double adstat1pval = model.ADResidPvalue;

            label5.Text = "A.D. Normality Statistic = " + adstat1.ToString("f4");
            label7.Text = "A.D. Statistic P-value = " + adstat1pval.ToString("f4");
            //label8.Text = "W.S. Normality Statistic = " + model.WSResidNormStatVal.ToString("f4");
            //label9.Text = "W.S. Statistic P-value = " + model.WSResidPvalue.ToString("f4");

            GraphPane gpResidPlot = addPlotResid(_predictions, _standardResiduals);
            List<double[]> data = new List<double[]>();
            double[] record = null;
            for (int i = 0; i < _predictions.Length; i++)
            {
                record = new double[2];
                record[0] = _observations[i];
                record[1] =  _predictions[i];
                data.Add(record);
            }

            double dec, man;
            if (_projMgr.ModelingInfo.DependentVariableTransform == VBTools.Globals.DependendVariableTransforms.Log10)
            {
                dec = Math.Pow(10, _projMgr.ModelingInfo.DecisionThreshold);
                man = Math.Pow(10, _projMgr.ModelingInfo.MandatedThreshold);
                //mlrPredObs1.SetThresholds(dec, man);
                mlrPlots1.SetThresholds(dec, man);
                mlrPlots1.Transform = VBTools.Globals.DependendVariableTransforms.Log10;
            }
            else if (_projMgr.ModelingInfo.DependentVariableTransform == VBTools.Globals.DependendVariableTransforms.Ln)
            {
                dec = Math.Pow(Math.E, _projMgr.ModelingInfo.DecisionThreshold);
                man = Math.Pow(Math.E, _projMgr.ModelingInfo.MandatedThreshold);
                //mlrPredObs1.SetThresholds(dec, man);
                mlrPlots1.SetThresholds(dec, man);
                mlrPlots1.Transform = VBTools.Globals.DependendVariableTransforms.Ln;
            }
            else if (_projMgr.ModelingInfo.DependentVariableTransform == VBTools.Globals.DependendVariableTransforms.Power)
            {
                double power = _projMgr.ModelingInfo.PowerTransformExponent;
                dec = Math.Sign(_projMgr.ModelingInfo.DecisionThreshold) * Math.Pow(Math.Abs(_projMgr.ModelingInfo.DecisionThreshold), (1.0 / power));
                man = Math.Sign(_projMgr.ModelingInfo.MandatedThreshold) * Math.Pow(Math.Abs(_projMgr.ModelingInfo.MandatedThreshold), (1.0 / power));

                //mlrPredObs1.SetThresholds(dec, man);
                mlrPlots1.SetThresholds(dec, man);
                mlrPlots1.PowerExponent = power;
                mlrPlots1.Transform = VBTools.Globals.DependendVariableTransforms.Power;
                
            }

            //mlrPredObs1.UpdateResults(data);
            mlrPlots1.UpdateResults(data, model.RMSE, MLRPlots.Exceedance.model);
            //GraphPane gpXYPlot = addPlotXY(_predictions, _observations);

            master.Add(gpResidPlot);
            //master.Add(gpXYPlot); 

            //GraphPane pane = zgc2.GraphPane;
            //pane.Title.Text = "Predictions vs Studentized Residuals";
            //if (pane.CurveList.Count > 0) pane.CurveList.Clear();
            //LineItem curve = pane.AddCurve(null, _predictions, _standardResiduals, Color.Blue, SymbolType.Circle);
            //pane.XAxis.Title.Text = "Predictions";
            //pane.YAxis.Title.Text = "Studentized Residuals";
            //curve.Line.IsVisible = false;
            //zgc2.AxisChange();
            //zgc2.Refresh();

            using (Graphics g = this.CreateGraphics())
            { master.SetLayout(g, PaneLayout.SquareColPreferred); }

            zgc2.IsShowPointValues = true;
            zgc2.AxisChange();
            master.AxisChange();
            zgc2.Refresh();

        }

        //private GraphPane addPlotXY(double[] _predictions, double[] _observations)
        //{
        //    GraphPane pane = new GraphPane();
        //    pane.Title.Text = "Observation vs Predictions";
        //    if (pane.CurveList.Count > 0) pane.CurveList.Clear();
        //    LineItem curve = pane.AddCurve(null, _observations, _predictions, Color.Blue, SymbolType.Circle);
        //    pane.XAxis.Title.Text = "Observations";
        //    pane.YAxis.Title.Text = "Predictions";
        //    curve.Line.IsVisible = false;
        //    zgc2.AxisChange();
        //    zgc2.Refresh();

        //    return pane;
        //}

        private GraphPane addPlotResid(double[] _predictions, double[] _standardResiduals)
        {
            GraphPane pane = new GraphPane();
            //pane.Title.Text = "Predictions vs Studentized Residuals";
            pane.Title.Text = "Studentized Residuals vs Fitted";
            if (pane.CurveList.Count > 0) pane.CurveList.Clear();
            LineItem curve = pane.AddCurve(null, _predictions, _standardResiduals, Color.Blue, SymbolType.Circle);
            //pane.XAxis.Title.Text = "Predictions";
            pane.XAxis.Title.Text = "Fitted";
            pane.YAxis.Title.Text = "Studentized Residuals";
            curve.Line.IsVisible = false;
            zgc2.AxisChange();
            zgc2.Refresh();

            return pane;
        }

        private MultipleRegression computeModel(DataTable dt)
        {
            if (_projMgr.ModelIndependentVariables == null) return null;
            //given a datatable, build a model
            MultipleRegression model = null;
            string[] idvars = _projMgr.ModelIndependentVariables.ToArray();
            string dvar = _projMgr.ModelDependentVariable;
            if (dt != null)
            {
                model = new MultipleRegression(dt, dvar, idvars);
                try { model.Compute();}
                catch { return null; }
            }
            return model;
        }

        private PointPairList getPPList(DataTable dt, bool xdate)
        {
            //create a point list for the dffits plot

            double x;
            double y;
            string tag = string.Empty;
            PointPairList plist = new PointPairList();
            foreach (DataRow r in dt.Rows)
            {
                if (xdate)
                {
                    DateTime date = Convert.ToDateTime(r[0].ToString());
                    tag = date.ToShortDateString();
                    x = new ZedGraph.XDate(date);
                    y = Convert.ToDouble(r[1].ToString());
                    tag += " (" + y.ToString() + ")";
                    plist.Add(x, y, tag);
                }
                else
                {
                    tag = r[0].ToString();
                    x = Convert.ToDouble(r[0].ToString());
                    y = Convert.ToDouble(r[2].ToString());
                    plist.Add(x, y, tag);
                }
            }

            return plist;


        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //user selected a model in list, get its data table, build its model, 
            //update the model stats lists and create the plots for the model
            if (listBox1.SelectedIndex < 0) return;

            _selectedRebuild = listBox1.SelectedIndex;

            string modelname = listBox1.SelectedItem.ToString();
            DataTable dt = _modelBuildTables.Tables[modelname];
            if (dt != null)
            {
                MultipleRegression model = computeModel(dt);
                _projMgr.ResidualAnalysisInfo.SelectedModelRMSE = model.RMSE;
                updateListViews(model);

                DataTable dffits = getDFFITSTable(model, dt, false);
                DataTable cooks = getCooksDistanceTable(model, dt, false);

                createResidPlot("DFFITS", dffits);
                createResidPlot("CooksDistance", cooks);

                createPlot2(model);

                SaveModel(dt, model);

                _state = _residState.dirty;
            }
        }

        private void SaveModel(DataTable dt, MultipleRegression model)
        {
            //throw new NotImplementedException();
            //update the serialized model string with the new model dictionary, model datatable

            //Dictionary<string, double> parameters = new Dictionary<string, double>();
            //for (int i = 0; i < model.Parameters.Rows.Count; i++)
            //{
            //    parameters.Add(model.Parameters.Rows[i][0].ToString(), Convert.ToDouble(model.Parameters.Rows[i][1]));
            //}


            _projMgr.Model = model.Model;
            //MLRModel m = MLRModel.getMLRModel();

            //update with new info
            //m.ModelDic = parameters;
            //m.ModelDT = dt;
            //m.RegressionDataTable = dt;

        }

 
        //private void btnUseRebuilt_Click(object sender, EventArgs e)
        //{
        //    //push model data to project manager (for prediction)
        //    if (listBox1.SelectedIndex < 0)
        //    {
        //        MessageBox.Show("Select a model from the 'Models' list.", "Selection Required", MessageBoxButtons.OK);
        //        return;
        //    }

        //    string modelname = listBox1.SelectedItem.ToString();
        //    DataTable dt = _modelBuildTables.Tables[modelname];
        //    MultipleRegression model = computeModel(dt);

        //    //save the model in the project manager
        //    _projMgr.Model = null;
        //    Dictionary<string, double> parameters = new Dictionary<string, double>();
        //    for (int i = 0; i < model.Parameters.Rows.Count; i++)
        //    {
        //        parameters.Add(model.Parameters.Rows[i][0].ToString(), Convert.ToDouble(model.Parameters.Rows[i][1]));
        //    }
        //    _projMgr.Model = parameters;

        //    _projMgr._comms.sendMessage("Show the Prediction form.", this);
        //}

        private void btnViewDTDFFITS_Click(object sender, EventArgs e)
        {
            //view datatable
            
            //_recs2Remove = mergeLists(_dffitsRecs2Remove, _cooksRecs2Remove);
            //_residValue2Remove = mergeLists(_dffitsresidValue2Remove, _cooksresidValue2Remove);
            //_residType2Remove = mergeLists(_dffitsResidType, _cooksResidType);

            frmDataTable frmDT = new frmDataTable(_recsRemoved, _residValueRemoved, 
                _residTypeRemoved, _modelBuildTables.Tables[0]);
            frmDT.ShowDialog();
        }

        //private List<T> mergeLists<T> (List<T> list1, List<T> list2)
        //{
        //    List<T> mergedList = new List<T>();
        //    mergedList.InsertRange(0, list1);
        //    mergedList.InsertRange(mergedList.Count, list2);
        //    return mergedList;

        //}

        private void btnUseRebuiltDFFITS_Click(object sender, EventArgs e)
        {
            //push model data to project manager (for prediction)
            if (listBox1.SelectedIndex < 0)
            {
                MessageBox.Show("Select a model from the 'Models' list.", "Selection Required", MessageBoxButtons.OK);
                return;
            }

            string modelname = listBox1.SelectedItem.ToString();
            DataTable dt = _modelBuildTables.Tables[modelname];
            MultipleRegression model = computeModel(dt);

            //save the model in the project manager
            _projMgr.Model = model.Model;
            //Dictionary<string, double> parameters = new Dictionary<string, double>();
            //for (int i = 0; i < model.Parameters.Rows.Count; i++)
            //{
            //    parameters.Add(model.Parameters.Rows[i][0].ToString(), Convert.ToDouble(model.Parameters.Rows[i][1]));
            //}
            //_projMgr.Model = parameters;

            //save is now done automatically when user selects model from listbox

            //update the serialized model string with the new model dictionary, model datatable
            //string sm = _projMgr.SerializedModel;
            //instance a MLRModel and get the model in MLRModel form
            //MLRModel m = new MLRModel(null);
            //MLRModel m = MLRModel.getMLRModel();
            //m = m.Deserialize<MLRModel>(sm);
            //update with new info
            //m.ModelDic = parameters;
            //m.ModelDT = dt;
            //convert it back into a string and push it back out to the project manager
            //sm = m.Serialize(m);
            //_projMgr.SerializedModel = sm;

            _projMgr._comms.sendMessage("Show the Prediction form.", this);
        }

        private void btnViewDTCooks_Click(object sender, EventArgs e)
        {
            btnViewDTDFFITS_Click(this, new EventArgs());
        }

        private void btnUseRebuiltCooks_Click(object sender, EventArgs e)
        {
            btnUseRebuiltDFFITS_Click(this, new EventArgs());
        }

        private void frmResiduals_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string apppath = Application.StartupPath.ToString();
            VBCSHelp help = new VBCSHelp(apppath, sender);
            if (!help.Status)
            {
                MessageBox.Show(
                "User documentation is found in the Documentation folder where you installed Virtual Beach"
                + "\nIf your web browser is PDF-enabled, open the User Guide with your browser.",
                "Neither Adobe Acrobat nor Adobe Reader found.",
                MessageBoxButtons.OK);
            }
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.C) && (e.Modifiers == Keys.Control))
                CopyListViewToClipboard(listView1);
        }

        private void listView2_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.C) && (e.Modifiers == Keys.Control))
                CopyListViewToClipboard(listView2);
        }

        public void CopyListViewToClipboard(ListView lv)
        {
            StringBuilder buffer = new StringBuilder();

            for (int i = 0; i < lv.Columns.Count; i++)
            {
                buffer.Append(lv.Columns[i].Text);
                buffer.Append("\t");
            }

            buffer.Append(Environment.NewLine);

            for (int i = 0; i < lv.Items.Count; i++)
            {
                if (lv.Items[i].Selected)
                {
                    for (int j = 0; j < lv.Columns.Count; j++)
                    {
                        buffer.Append(lv.Items[i].SubItems[j].Text);
                        buffer.Append("\t");
                    }

                    buffer.Append(Environment.NewLine);
                }
            }

            Clipboard.SetText(buffer.ToString());
        }

        private void textBox3_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(textBox3, "");
        }

        private void textBox3_Validating(object sender, CancelEventArgs e)
        {
            //validate the user input for cutoff value
            //will probably need some smarter critera for minimum value permitted...

            double newcutoff = double.NaN;
            string teststring = textBox3.Text;
            if (!double.TryParse(teststring, out newcutoff))
            {
                e.Cancel = true;
                textBox3.Select(0, textBox3.Text.Length);
                this.errorProvider1.SetError(textBox3, "Text must convert to a number.");
                //MessageBox.Show("Must convert to number", "Invalid entry", MessageBoxButtons.OK);
                return;
            }
            else if (newcutoff >= 3.0d)
            {
                e.Cancel = true;
                textBox3.Select(0, textBox3.Text.Length);
                this.errorProvider1.SetError(textBox3, "Value must be less than 3");
                //MessageBox.Show("Value must be less than 3", "Invalid entry", MessageBoxButtons.OK);
                return;
            }
            else
            {
                _dffitsThreshold = newcutoff;
            }

        }

        private void tbcookcutoff_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(tbcookcutoff, "");
        }

        private void tbcookcutoff_Validating(object sender, CancelEventArgs e)
        {
            double newcutoff = double.NaN;
            string teststring = tbcookcutoff.Text;
            if (!double.TryParse(teststring, out newcutoff))
            {
                e.Cancel = true;
                tbcookcutoff.Select(0, tbcookcutoff.Text.Length);
                this.errorProvider1.SetError(tbcookcutoff, "Text must convert to a number.");
                return;
            }
            else if (newcutoff >= 4 * _cooksThreshold)
            {
                e.Cancel = true;
                tbcookcutoff.Select(0, tbcookcutoff.Text.Length);
                this.errorProvider1.SetError(tbcookcutoff, "Value must be less than " + (4 * _cooksThreshold).ToString("f4"));
                //MessageBox.Show("Value must be less than 3", "Invalid entry", MessageBoxButtons.OK);
                return;
            }
            else
            {
               _cooksThreshold = newcutoff;
            }
        }

        /// <summary>
        /// starts the residual process all over
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            _residualInfo = new Dictionary<int, string>();

            DataTable modeldt = _projMgr.ModelDataTable;

            MultipleRegression model = computeModel(modeldt);

            _gobtnState = true;

            //cutoff rebuilds when half data records eliminated
            _maxIterations = modeldt.Rows.Count / 2;

            _recsRemoved = new List<int>();
            _residValueRemoved = new List<double>();
            _residTypeRemoved = new List<string>();

            listBox1.Items.Clear();
            //for (int i = listBox1.Items.Count - 1; i > 0;  i--)
            //    listBox1.Items.RemoveAt(i);

            _modelBuildTables.Tables.Clear();
            //for (int i = _modelBuildTables.Tables.Count - 1; i > 0; i--)
            //    _modelBuildTables.Tables.RemoveAt(i);

            DataTable dffits = getDFFITSTable(model, modeldt, true);
            DataTable cooks = getCooksDistanceTable(model, modeldt, true);
            updateModelList(model, "original", modeldt, "model");

            //create rediduals plots
            createResidPlot("DFFITS", dffits);
            createResidPlot("CooksDistance", cooks);

            //predictions vs standardized residuals plot
            createPlot2(model);

            //initialize dffits threshold value/label
            double dffitsThreshold = Convert.ToDouble((2.0d * Math.Sqrt(((double)(CreateModelDataTable()).Columns.Count - 2) / (double)model.PredictedValues.Length)).ToString());
            label3.Text = label3.Text.Substring(0, 13) + dffitsThreshold.ToString("f4");
            double dfcutoff = dffitsThreshold;
            textBox3.Text = dfcutoff.ToString("f4");
            _dffitsThreshold = dfcutoff;
            //rbConstantCutoff.Checked = true;
            rbIterativeCutoff.Checked = true;

            //initialize cooksd threshold value/label
            double cooksThreshold = 4.0d / (double)model.PredictedValues.Length;
            label10.Text = label10.Text.Substring(0, 10) + cooksThreshold.ToString("f4");
            tbcookcutoff.Text = cooksThreshold.ToString("f4");
            _cooksThreshold = cooksThreshold;
            //rbCookConstantCutoff.Checked = true;
            rbCooksIterativeCutoff.Checked = true;

             //resets the model in prediction if they've selected one of the rebuilds they've just cleared
            listBox1.SelectedIndex = 0;
            _continue = true;
        }


    }


    
}
