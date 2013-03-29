using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using VBTools;
using VBTools.IO;
using VBStatistics;
using VBControls;
using Ciloci.Flee;
using LogUtilities;

namespace MLRPrediction
{
    public partial class frmMLRPrediction : DockContent
    {
        private ContextMenu cmforResponseVar = new ContextMenu();
        private ContextMenu cmforPrediction = new ContextMenu();
        private VBProjectManager _projMgr = null;
        private Dictionary<string, double> _model;
        private Dictionary<string, string> _mainEffects;
        private string _modelExpression = "";
        //These are the main effects variables actually include in the model
        //It will also have ID as first element
        private string[] _referencedVariables = null;

        private DataTable _corrDT = null;
        private DataTable _dtVariables = null;
        private DataTable _dtObs = null;
        private DataTable _dtStats = null;

        private string _respVarTransform = "none";

        private DataTable _dtObsOrig = null;
        //private bool _obsTransformed = false;

        private bool _projectOpened;

        private Dictionary<string, double> _currentModel = null;

        //private enum _importedObsState { None, Log10, Ln, Power };
        //private _importedObsState _importedobsstate = _importedObsState.None;
        //private _importedObsState _predictionView = _importedObsState.None;

        private Globals.DependendVariableTransforms _obsState = Globals.DependendVariableTransforms.none;
        private Globals.DependendVariableTransforms _predView = Globals.DependendVariableTransforms.none;

        //Dictionary mapping independent variables to imported data
        private Dictionary<string, string> _columnMap = null;

        private double _importedExp;
        private double _predictionViewExponent;

        

        public frmMLRPrediction()
        {
            InitializeComponent();
            _projMgr = VBProjectManager.GetProjectManager();

            _projMgr.ProjectOpened += new VBProjectManager.ProjectOpenedHandler(ProjectOpenedListener);
            _projMgr.ProjectSaved += new VBProjectManager.ProjectSavedHandler(ProjectSavedListener);

            //VBLogger logger = LogUtilities.VBLogger.getLogger();

        }


        private void ProjectOpenedListener()
        {
            if (_model == null) return;

            Globals.ProjectType pmType = _projMgr._projectType;
            if (pmType == Globals.ProjectType.COMPLETE) return;

            MLRPredInfo predInfo = _projMgr.PredictionInfo;
            if (predInfo == null)
                return;

            string msg = string.Empty;
            //Globals.ProjectType pmType = _projMgr._projectType;
            //if (pmType == Globals.ProjectType.COMPLETE)
            //    msg = "Project File: " + _projMgr.Name;
            //else if (pmType == Globals.ProjectType.MODEL)
            msg = "Model File: " + _projMgr.Name;

            VBLogger.getLogger().logEvent(msg, VBLogger.messageIntent.UserOnly, VBLogger.targetSStrip.StatusStrip1);

            _currentModel = _model;

            //Console.WriteLine("\n*** Prediction: project opened.***\n");

            DataSet ds = null;

            if (!String.IsNullOrWhiteSpace(predInfo.IVData))
            {
                ds = new DataSet();
                ds.ReadXml(new StringReader(predInfo.IVData), XmlReadMode.ReadSchema);
                _dtVariables = ds.Tables[0];
                dgvVariables.DataSource = _dtVariables;
                setViewOnGrid(dgvVariables);
            }

            if (!String.IsNullOrWhiteSpace(predInfo.ObsData))
            {
                ds = new DataSet();
                ds.ReadXml(new StringReader(predInfo.ObsData), XmlReadMode.ReadSchema);
                _dtObs = ds.Tables[0];
                dgvObs.DataSource = _dtObs;
                setViewOnGrid(dgvObs);
            }

            if (!String.IsNullOrWhiteSpace(predInfo.StatData))
            {
                ds = new DataSet();
                ds.ReadXml(new StringReader(predInfo.StatData), XmlReadMode.ReadSchema);
                _dtStats = ds.Tables[0];
                dgvStats.DataSource = _dtStats;
                setViewOnGrid(dgvStats);
            }
            else dgvStats.DataSource = null;

            ds = null;

            if (double.IsNaN(predInfo.RegulatoryStandard))
                txtRegStd.Text = "255";
            else
                txtRegStd.Text = predInfo.RegulatoryStandard.ToString();

            if (double.IsNaN(predInfo.DecisionCriteria))
                txtDecCrit.Text = "255";
            else
                txtDecCrit.Text = predInfo.DecisionCriteria.ToString();

            if (double.IsNaN(predInfo.PowerTransform))
                txtPower.Text = "255";
            else
                txtPower.Text = predInfo.PowerTransform.ToString();

            if (predInfo.DependentVariableTransform == Globals.DependendVariableTransforms.none)
            {
                rbNone.Checked = true;
                lblRVTransform.Text = "RV Transform: " + Globals.DependendVariableTransforms.none.ToString();
            }
            else if (predInfo.DependentVariableTransform == Globals.DependendVariableTransforms.Log10)
            {
                rbLog10.Checked = true;
                lblRVTransform.Text = "RV Transform: " + Globals.DependendVariableTransforms.Log10.ToString();
            }
            else if (predInfo.DependentVariableTransform == Globals.DependendVariableTransforms.Ln)
            {
                rbLn.Checked = true;
                lblRVTransform.Text = "RV Transform: " + Globals.DependendVariableTransforms.Ln.ToString();
            }
            else if (predInfo.DependentVariableTransform == Globals.DependendVariableTransforms.Power)
            {
                rbPower.Checked = true;
                lblRVTransform.Text = "RV Transform: (RV)**" + txtPower.Text;
            }

            _columnMap = predInfo.ColumnMap;

            _projectOpened = true;

            //txtModel.Text = Support.BuildModelExpression(_model, _projMgr.ModelingInfo.DependentVariable, "");
            _modelExpression = Support.BuildModelExpression(_model, null, "");
            txtModel.Text = Support.BuildModelExpression(_model, _projMgr.ModelingInfo.DependentVariable, "g4");

            _importedExp = _projMgr.PredictionInfo.ObsTransformPowerExponent;
            //string sobs = _projMgr.PredictionInfo.ObsTransform;
            _obsState = _projMgr.PredictionInfo.ObsTransformState;

            lblObsTransform.Text = "Obs Transform: " + _obsState.ToString();
            if (_obsState == Globals.DependendVariableTransforms.Power)
            {
                lblObsTransform.Text = "Obs Transform: (Obs)**" + _importedExp.ToString("n");
            }
            setMenuItemState(_obsState, cmforResponseVar);
            //this.Show();
        }

        private void ProjectSavedListener()
        {
            //Globals.ProjectType pmType = _projMgr._projectType;
            //if (pmType == Globals.ProjectType.COMPLETE) return;

            //MLRPredInfo predInfo = new MLRPredInfo();
            MLRPredInfo predInfo = _projMgr.PredictionInfo;
            if (predInfo == null) predInfo = new MLRPredInfo();
 
            StringWriter sw = null;

            dgvVariables.EndEdit();
            _dtVariables = (DataTable)dgvVariables.DataSource;
            if (_dtVariables != null)
            {
                _dtVariables.AcceptChanges();
                _dtVariables.TableName = "Variables";
                sw = new StringWriter();
                _dtVariables.WriteXml(sw, XmlWriteMode.WriteSchema, false);
                predInfo.IVData = sw.ToString();
                sw.Close();
            }

            dgvObs.EndEdit();
            _dtObs = (DataTable)dgvObs.DataSource;
            if (_dtObs != null)
            {
                _dtObs.AcceptChanges();
                _dtObs.TableName = "Observations";
                sw = new StringWriter();
                _dtObs.WriteXml(sw, XmlWriteMode.WriteSchema, false);
                predInfo.ObsData = sw.ToString();
                sw.Close();
            }

            dgvStats.EndEdit();
            _dtStats = (DataTable)dgvStats.DataSource;
            if (_dtStats != null)
            {
                _dtStats.AcceptChanges();
                _dtStats.TableName = "Stats";
                sw = new StringWriter();
                _dtStats.WriteXml(sw, XmlWriteMode.WriteSchema, false);
                predInfo.StatData = sw.ToString();
                sw.Close();
            }

            //this info is now saved via the modelSave dialog
            //predInfo.RegulatoryStandard = Convert.ToDouble(txtRegStd.Text);
            //predInfo.DecisionCriteria = Convert.ToDouble(txtDecCrit.Text);

            //string pwrTrans = txtPower.Text;
            //if (String.IsNullOrWhiteSpace(pwrTrans))
            //    predInfo.PowerTransform = 1.0;

            if (rbNone.Checked)
                predInfo.DependentVariableTransform = Globals.DependendVariableTransforms.none;
            else if (rbLog10.Checked)
                predInfo.DependentVariableTransform = Globals.DependendVariableTransforms.Log10;
            else if (rbLn.Checked)
                predInfo.DependentVariableTransform = Globals.DependendVariableTransforms.Ln;
            else if (rbPower.Checked)
                predInfo.DependentVariableTransform = Globals.DependendVariableTransforms.Power;

            predInfo.ColumnMap = _columnMap;

            _projMgr.PredictionInfo = predInfo;

            _projMgr.PredictionInfo.ObsTransformState = _obsState;
            _projMgr.PredictionInfo.ObsTransformPowerExponent = _importedExp;

        }

        private void frmMLRPrediction_Enter(object sender, EventArgs e)
        {
            //if we have a model selected, go.  otherwise go away
            txtModel.Text = "";

            if ((_projMgr.Model == null) || (_projMgr.Model.Count <= 0))
                return;

            if (_projMgr.ModelingInfo == null)
                return;

            _corrDT = _projMgr.CorrelationDataTable;

            //Lets get all the main effect variables
            _mainEffects = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            //Add ID first so it shows up first in datatable.
            //_mainEffects.Add("ID", "ID");
            for (int i = 2; i < _corrDT.Columns.Count; i++ )
            {
                bool bMainEffect = Support.IsMainEffect(_corrDT.Columns[i].ColumnName, _corrDT);
                if (bMainEffect)
                {                    
                    string colName = _corrDT.Columns[i].ColumnName;
                    _mainEffects.Add(colName, colName);
                }
            }            

            _model = _projMgr.Model;

            if (!_projectOpened)
            {

                lblObsTransform.Text = "Obs Transform: " + Globals.DependendVariableTransforms.none.ToString();

                if (_corrDT.Columns[1].ExtendedProperties.ContainsKey(VBTools.Globals.DEPENDENTVARIBLEDEFINEDTRANSFORM))
                {
                    _respVarTransform = _corrDT.Columns[1].ExtendedProperties[VBTools.Globals.DEPENDENTVARIBLEDEFINEDTRANSFORM].ToString();
                    if (String.Compare(_respVarTransform, Globals.DependendVariableTransforms.none.ToString(), 0) == 0)
                    {
                        rbNone.Checked = true;
                        lblRVTransform.Text = "RV Transform: " + GetTransformType().ToString(); //_importedObsState.None.ToString();
                    }
                    else if (String.Compare(_respVarTransform, Globals.DependendVariableTransforms.Log10.ToString(), 0) == 0)
                    {
                        rbLog10.Checked = true;
                        lblRVTransform.Text = "RV Transform: " + GetTransformType().ToString(); //_importedObsState.Log10.ToString();
                    }
                    else if (String.Compare(_respVarTransform, Globals.DependendVariableTransforms.Ln.ToString(), 0) == 0)
                    {
                        rbLn.Checked = true;
                        lblRVTransform.Text = "RV Transform: " + GetTransformType().ToString(); //_importedObsState.Ln.ToString();
                    }
                    else if (_respVarTransform.Contains(Globals.DependendVariableTransforms.Power.ToString()))
                    {
                        rbPower.Checked = true;
                        double power = getTransformPower(_respVarTransform);
                        txtPower.Text = power.ToString();
                        lblRVTransform.Text = "RV Transform: (RV)**" + txtPower.Text;
                    }
                }
                else
                    rbNone.Checked = true;
            }
            else
            {
                _respVarTransform = _projMgr.PredictionInfo.DependentVariableTransform.ToString();
                if (_respVarTransform.Contains(Globals.DependendVariableTransforms.Power.ToString()))
                {
                    rbPower.Checked = true;
                    double power = _projMgr.PredictionInfo.PowerTransform;
                    txtPower.Text = power.ToString();
                }
            }
            

            //txtModel.Text = Support.BuildModelExpression(_model, _projMgr.ModelingInfo.DependentVariable, "");
            _modelExpression = Support.BuildModelExpression(_model, null, "");
            txtModel.Text = Support.BuildModelExpression(_model, _projMgr.ModelingInfo.DependentVariable, "g4");

            //Lets get only the main effects in the model
            string[] refvars = ExpressionEvaluator.GetReferencedVariables(_modelExpression, _mainEffects.Keys.ToArray());
            List<string> refVarList = new List<string>();
            refVarList.Add("ID");
            refVarList.AddRange(refvars);
            _referencedVariables = refVarList.ToArray();


            if (!_projectOpened)
            {
                if (newModelSelected(_model))
                {
                    dgvVariables.DataSource = CreateEmptyIVsDataTable();
                    dgvObs.DataSource = CreateEmptyObservationsDataTable();
                }
                //_projectOpened = false;
            }
            _projectOpened = false;
            _currentModel = _model;
            
        }

        private bool newModelSelected(Dictionary<string, double> modeldic)
        {
 
            if (_currentModel == null)
            {
                _currentModel = modeldic;
                return true;
            }

            if (_projectOpened)
            {
                _projectOpened = false;
                return true;
            }

            if (_currentModel.Keys.Count == modeldic.Keys.Count)
            {
                foreach (KeyValuePair<string, double> kv in modeldic)
                {
                    if (!_currentModel.ContainsKey(kv.Key) || kv.Value != _currentModel[kv.Key])
                    {
                        _currentModel = modeldic;
                        return true;
                    }

                }
                return false;
            }
            else
            {
                _currentModel = modeldic;
                return true; ;
            }

        }

        private void dgvVariables_Scroll(object sender, ScrollEventArgs e)
        {
            int first = dgvVariables.FirstDisplayedScrollingRowIndex;

            if (dgvObs.Rows.Count > 0)
            {                
                if (first >= dgvObs.Rows.Count)
                    dgvObs.FirstDisplayedScrollingRowIndex = dgvObs.Rows.Count - 1;
                else
                    dgvObs.FirstDisplayedScrollingRowIndex = dgvVariables.FirstDisplayedScrollingRowIndex;
            }
            if (dgvStats.Rows.Count > 0)
            {
                if (first >= dgvStats.Rows.Count)
                    dgvStats.FirstDisplayedScrollingRowIndex = dgvStats.Rows.Count - 1;
                else
                    dgvStats.FirstDisplayedScrollingRowIndex = dgvVariables.FirstDisplayedScrollingRowIndex;
            }
        }

        private void dgvObs_Scroll(object sender, ScrollEventArgs e)
        {
            int first = dgvObs.FirstDisplayedScrollingRowIndex;

            if (dgvVariables.Rows.Count > 0)
            {
                if (first >= dgvVariables.Rows.Count)
                    dgvVariables.FirstDisplayedScrollingRowIndex = dgvVariables.Rows.Count - 1;
                else
                    dgvVariables.FirstDisplayedScrollingRowIndex = dgvObs.FirstDisplayedScrollingRowIndex;            
            }

            if (dgvStats.Rows.Count > 0)
            {
                if (first >= dgvStats.Rows.Count)
                    dgvStats.FirstDisplayedScrollingRowIndex = dgvStats.Rows.Count - 1;
                else
                    dgvStats.FirstDisplayedScrollingRowIndex = dgvObs.FirstDisplayedScrollingRowIndex;
            }
        }

        private void dgvStats_Scroll(object sender, ScrollEventArgs e)
        {
            int first = dgvStats.FirstDisplayedScrollingRowIndex;

            if (dgvVariables.Rows.Count > 0)
            {
                if (first >= dgvVariables.Rows.Count)
                    dgvVariables.FirstDisplayedScrollingRowIndex = dgvVariables.Rows.Count - 1;
                else
                    dgvVariables.FirstDisplayedScrollingRowIndex = dgvStats.FirstDisplayedScrollingRowIndex;
            }

            if (dgvObs.Rows.Count > 0)
            {
                if (first >= dgvObs.Rows.Count)
                    dgvObs.FirstDisplayedScrollingRowIndex = dgvObs.Rows.Count - 1;
                else
                    dgvObs.FirstDisplayedScrollingRowIndex = dgvStats.FirstDisplayedScrollingRowIndex;
            }
        }

        //private void btnChange_Click(object sender, EventArgs e)
        //{
        //    if (btnChange.Text == "Change")
        //    {
        //        txtDecCrit.ReadOnly = false;
        //        txtRegStd.ReadOnly = false;
        //        btnChange.Text = "Save";
        //    }
        //    else if (btnChange.Text == "Save")
        //    {
        //        txtDecCrit.ReadOnly = true;
        //        txtRegStd.ReadOnly = true;
        //        btnChange.Text = "Change";
        //    }
        //}

        private void btnImportIVs_Click(object sender, EventArgs e)
        {
            VBTools.ImportExport import = new ImportExport();
            DataTable dt = import.Input;            
            if (dt == null)
                return;

            string[] headerCaptions = { "Model Variables", "Imported Variables" };

           
            Dictionary<string, string> fields = new Dictionary<string, string>(_mainEffects);
            //if (_mainEffects.ContainsKey("ID") == false)                          
            //    fields.Add("ID", "ID");
            
            //frmColumnMapper colMapper =  new frmColumnMapper(_mainEffects, dt, headerCaptions, true);
            frmColumnMapper colMapper = new frmColumnMapper(_referencedVariables, dt, headerCaptions, true, _columnMap);
                
            DialogResult dr = colMapper.ShowDialog();

            try
            {
                if (dr == DialogResult.OK)
                {
                    dt = colMapper.MappedTable;
                    _columnMap = colMapper.ColumnMap;

                    int errndx = 0;
                    if (!recordIndexUnique(dt, out errndx))
                    {
                        MessageBox.Show("Unable to import datasets with non-unique record identifiers.\n" +
                                        "Fix your datatable by assuring unique record identifier values\n" +
                                        "in the ID column and try importing again.\n\n" +
                                        "Record Identifier values cannot be blank or duplicated;\nencountered " +
                                        "error near row " + errndx.ToString(), "Import Data Error - Cannot Import This Dataset", MessageBoxButtons.OK);
                        return;
                    }

                    dgvVariables.DataSource = dt;
                }
                else
                    return;
            }
            catch (Exception re)
            {
                MessageBox.Show("Open file error: " + re.Message);
                return;
            }
            
            //DataColumn dc = new DataColumn("IVRowIdx", typeof(int));
            //dt.Columns.Add(dc);
            //dc.SetOrdinal(0);
            foreach (DataGridViewColumn dvgCol in dgvVariables.Columns)
            {
                dvgCol.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    dt.Rows[i]["IVRowIdx"] = i;
            //}
            //dgvVariables.Columns[0].Visible = false;

            setViewOnGrid(dgvVariables);            
            btnMakePredictions.Enabled = false;
        }

        /// <summary>
        /// test all cells in the daetime column for uniqueness
        /// could do this with linq but then how does one find where?
        /// Code copied from Mike's VBDatasheet.frmDatasheet.
        /// </summary>
        /// <param name="dt">table to search</param>
        /// <param name="where">record number of offending timestamp</param>
        /// <returns>true iff all unique, false otherwise</returns>
        private bool recordIndexUnique(DataTable dt, out int where)
        {
            Dictionary<string, int> temp = new Dictionary<string, int>();
            int ndx = -1;
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string tempval = dr["ID"].ToString();
                    temp.Add(dr["ID"].ToString(), ++ndx);
                    if (string.IsNullOrWhiteSpace(dr["ID"].ToString()))
                    {
                        where = ndx++;
                        //MessageBox.Show("Record Identifier values cannot be blank - encountered blank in row " + ndx++.ToString() + ".\n",
                        //    "Import data error", MessageBoxButtons.OK);
                        return false;
                    }
                }
            }
            catch (ArgumentException)
            {
                where = ndx++;
                //MessageBox.Show("Record Identifier values cannot be duplicated - encountered existing record in row " + ndx++.ToString() + ".\n",
                //    "Import data error", MessageBoxButtons.OK);
                return false;
            }
            where = ndx;
            return true;
        }

        private void btnImportObs_Click(object sender, EventArgs e)
        {
            VBTools.ImportExport import = new ImportExport();
            DataTable dt = import.Input;
            if (dt == null)
                return;

            string[] headerCaptions = { "Obs IDs", "Obs" };
            //Dictionary<string, string> obsColumns = new Dictionary<string, string>();
           // obsColumns.Add("ID", "ID");
            //obsColumns.Add("Observation", "Observation");

            string[] obsColumns = { "ID", "Observation" };

            frmColumnMapper colMapper = new frmColumnMapper(obsColumns, dt, headerCaptions, true);
            DialogResult dr = colMapper.ShowDialog();

            if (dr == DialogResult.OK)
            {
                dt = colMapper.MappedTable;
                dgvObs.DataSource = dt;
            }
            else
                return;

            foreach (DataGridViewColumn dvgCol in dgvObs.Columns)
                dvgCol.SortMode = DataGridViewColumnSortMode.NotSortable;

            setViewOnGrid(dgvObs);

            //reset the observation transform flag on import in case they've set it previously
            //_obsTransformed = false;

            //default the imported observation state to none
            _obsState = Globals.DependendVariableTransforms.none;
            lblObsTransform.Text = "Obs Transform: " + _obsState.ToString();
            setMenuItemState(_obsState, cmforResponseVar);
                        
        }

        private void btnMakePredictions_Click(object sender, EventArgs e)
        {
            string[] vars = _model.Keys.ToArray();
            
            _dtVariables = (DataTable)dgvVariables.DataSource;
            if (_dtVariables == null)
                return;

            if (_dtVariables.Rows.Count < 1)
                return;

            dgvVariables.EndEdit();
           
            _dtVariables.AcceptChanges();

            //List<string> badCells = getBadCells(_dtVariables, false);
            //if ((badCells != null) && (badCells.Count > 0))
            //{
            //    frmBadCellsRpt frmBadCells = new frmBadCellsRpt(badCells);
            //    frmBadCells.Text = "Please fix the bad data in the Indpendent Variables grid";
            //    frmBadCells.Show();
            //    return;
            //}


            dgvObs.EndEdit();            
            _dtObs = (DataTable)dgvObs.DataSource;
            if (_dtObs != null)
                _dtObs.AcceptChanges();

            //badCells = getBadCells(_dtObs, true);
            //if ((badCells != null) && (badCells.Count > 0))
            //{
            //    frmBadCellsRpt frmBadCells = new frmBadCellsRpt(badCells);
            //    frmBadCells.Text = "Please fix the bad data in the Observations grid";
            //    frmBadCells.Show();
            //    return;
            //}

            ExpressionEvaluator expEval = new ExpressionEvaluator();
            //List<double> lstPredictions = expEval.Evaluate(_modelExpression, _dtVariables);
            DataTable dtPredictions = expEval.Evaluate(_modelExpression, _dtVariables);//, false);
            //_dtStats = GeneratePredStats(lstPredictions, _dtObs);
            _dtStats = GeneratePredStats(dtPredictions, _dtObs);
             if (_dtStats == null)
                 return;

            dgvStats.DataSource = _dtStats;

            //reset the prediction view and menu
            _predView = Globals.DependendVariableTransforms.none;
            setMenuItemState(_predView, cmforPrediction);

            foreach (DataGridViewColumn dvgCol in dgvStats.Columns)
                dvgCol.SortMode = DataGridViewColumnSortMode.NotSortable;

            setViewOnGrid(dgvStats);            

        }

        private void dgvVariables_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            dgvVariables.EndEdit();
            _dtVariables = (DataTable)dgvVariables.DataSource;
            _dtVariables.AcceptChanges();
        }

        private void dgvObs_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            dgvObs.EndEdit();
            _dtObs = (DataTable)dgvVariables.DataSource;
            _dtObs.AcceptChanges();
        }


        //private DataTable GeneratePredStats(List<double> lstPredictions, DataTable dtObs)
        private DataTable GeneratePredStats(DataTable dtPredictions, DataTable dtObs)
        {
            Globals.DependendVariableTransforms dvt =  GetTransformType();
            if (dvt == Globals.DependendVariableTransforms.Power)
            {
                if (ValidateNumericTextBox(txtPower) == false)
                    return null;

            }


            DataTable dt = new DataTable();

            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("Model_Prediction", typeof(double));
            //dt.Columns.Add("Untransformed", typeof(double));
            dt.Columns.Add("Error_Type", typeof(string));
            dt.Columns.Add("Decision_Criterion", typeof(string));
            dt.Columns.Add("Exceedance_Probability", typeof(double));
            dt.Columns.Add("Regulatory_Standard", typeof(string));
            
           

            //double decCrit = Convert.ToDouble(txtDecCrit.Text); 
            //double regStd = Convert.ToDouble(txtRegStd.Text);
            double predValue = 0.0d;

            double decCrit = Convert.ToDouble(txtDecCrit.Text);
            decCrit = GetDCRSTransformedValue(decCrit);

            double regStd = Convert.ToDouble(txtRegStd.Text);
            regStd = GetDCRSTransformedValue(regStd);


            string id = "";
            DataTable modeldt = CreateModelDataTable();
            bool newModel = true;
            for (int i = 0; i < dtPredictions.Rows.Count; i++)
            {
                predValue = (double)dtPredictions.Rows[i]["CalcValue"];
                DataRow dr = dt.NewRow();
                id = (string)dtPredictions.Rows[i]["ID"];
                dr["ID"] = id;
                dr["Model_Prediction"] = GetInverseTransformValue (predValue);
                dr["Decision_Criterion"] = GetStringThreshold(txtDecCrit.Text); // decCrit;
                //dr["Exceedance_Probability"] = Statistics.PExceed(predValue, decCrit, _projMgr.ModelingInfo.RMSE);
                //TODO! Need row of computed values corresponding to model table values; NOT the variable values in the grid!
                //dr["Exceedance_Probability"] = Statistics.PExceed(_dtVariables.Rows[i], CreateModelDataTable(), predValue, decCrit, _projMgr.ModelingInfo.RMSE);
                //CreateModelDataTable() only needs to be done once outside containing loop
                //dr["Exceedance_Probability"] = Statistics.PExceed(getMLRxVals(_dtVariables.Rows[i]), modeldt, predValue, decCrit, _projMgr.ModelingInfo.RMSE);
                dr["Exceedance_Probability"] = Statistics.PExceed(getMLRxVals(_dtVariables.Rows[i]), modeldt, predValue, decCrit, _projMgr.ResidualAnalysisInfo.SelectedModelRMSE, newModel);
                dr["Regulatory_Standard"] = GetStringThreshold(txtRegStd.Text); // regStd;
                dr["Error_Type"] = GetErrorType(predValue, dr, dtObs, decCrit, regStd, (double)dr["Exceedance_Probability"]);

                dt.Rows.Add(dr);
                newModel = false;
            }

            return dt;            

        }


        /// <summary>
        /// match the id of the observation table to the id in the prediction table
        /// (should only have predictions for a records in the variables table)
        /// and compare the prediction value to the observation value for the id
        /// record to determine model failures (FP/FN).  Report model failures either 
        /// as a function of MLR prediction and decision criterion or using Probability
        /// of exceedance and the UI probability (radiobutton/terxtbox selected).
        /// </summary>
        /// <param name="prediction">model prediction value</param>
        /// <param name="drPrediction">prediction record to get the id (prediction may be transformed)</param>
        /// <param name="dtObs">observation table</param>
        /// <param name="decCrit">decision threshold</param>
        /// <param name="regStd">regulatory threshold</param>
        /// <returns>string - empty if no matching obs id or if predict/obs values in sync; false pos or false neg if not</returns>
        private string GetErrorType(double prediction, DataRow drPrediction, DataTable dtObs, double decCrit, double regStd, double pEx)
        {
            string errtype = string.Empty;
            if ((dtObs != null) && (dtObs.Rows.Count > 0))
            {
                double pred;
                double thresh;
                //if we're using P(Exceed) the threshold used is the textbox value and the prediction used is P(exceed) of the prediction
                if (rbPE.Checked)
                {
                    pred = pEx;
                    thresh = Convert.ToDouble(tbPExceed.Text.ToString());
                    drPrediction["Decision_Criterion"] = thresh.ToString("f2");

                }
                //...else the threshold is the decision criterion and the prediction used is the MLR prediction
                else
                {
                    pred = prediction;
                    thresh = decCrit;
                }

                string idPredict = drPrediction[0].ToString();
                DataRow[] rowsObs = dtObs.Select("ID = '" + idPredict + "'");
                if (rowsObs != null && rowsObs.Length > 0)
                {
                    double obs;
                    if (double.TryParse(rowsObs[0][1].ToString(), out obs))
                    {
                        //double obs = (double)rowsObs[0][1];
                        double obst = GetEquivalentObsState(obs, _obsState);

                        if ((pred >= thresh) && (obst < regStd))
                            errtype = "False Exceed";
                        else if ((obst >= regStd) && (pred < thresh))
                            errtype = "False Non-Exceed";                      
                    }
                }
            }

            return errtype;
        }


        private double GetEquivalentObsState(double value, Globals.DependendVariableTransforms testvariable)
        {
            //get the observation value in the same transformation state as the response variable.
            //response variable state is determined from the UI RV transform label (itself set via 
            //datatable entended properties from onformenter, or from predictioninfo on modelopen 
            //(projectopen listener).  returned values are used in determining model failures (FN/FP
            //counts)
            double retVal = 0.0d;


            if (lblRVTransform.Text.Contains("none"))
            {
                if (testvariable == Globals.DependendVariableTransforms.none)
                {
                    retVal = value;
                }
                else if (testvariable == Globals.DependendVariableTransforms.Log10)
                {
                    retVal = Math.Pow(10.0,value);
                }
                else if (testvariable == Globals.DependendVariableTransforms.Ln)
                {
                    retVal = Math.Pow(Math.E, value);
                }
                else if (testvariable == Globals.DependendVariableTransforms.Power)
                {
                    retVal = Math.Pow(value, 1.0/_importedExp);
                }
            }

            else if (lblRVTransform.Text.Contains("Log10"))
            {
                if (testvariable == Globals.DependendVariableTransforms.none)
                {
                    retVal = Math.Log10(value);
                }
                else if (testvariable == Globals.DependendVariableTransforms.Log10)
                {
                    retVal = value;
                }
                else if (testvariable == Globals.DependendVariableTransforms.Ln)
                {
                    retVal = Math.Pow(Math.E, value);
                    retVal = Math.Log10(retVal);
                    //retVal = Math.Log(value) / Math.Log(10.0d);
                }
                else if (testvariable == Globals.DependendVariableTransforms.Power)
                {
                    retVal = Math.Pow(value, 1.0/_importedExp);
                    retVal = Math.Log10(retVal);
                }
            }

            else if (lblRVTransform.Text.Contains("Ln"))
            {
                if (testvariable == Globals.DependendVariableTransforms.none)
                {
                    retVal = Math.Log(value);
                }
                else if (testvariable == Globals.DependendVariableTransforms.Log10)
                {
                    retVal = Math.Pow(10.0, value);
                    retVal = Math.Log(retVal);
                    //retVal = Math.Log10(value) / Math.Log10(Math.E);
                    //retVal = Math.Log(10.0) * Math.Log10(value);
                }
                else if (testvariable == Globals.DependendVariableTransforms.Ln)
                {
                    retVal = value;
                }
                else if (testvariable == Globals.DependendVariableTransforms.Power)
                {
                    retVal = Math.Pow(value, 1.0/_importedExp);
                    retVal = Math.Log(retVal);
                }
            }

            else if (lblRVTransform.Text.Contains("(RV)**"))
            {
                if (testvariable == Globals.DependendVariableTransforms.none)
                {
                    retVal = Math.Pow(value, Convert.ToDouble(txtPower.Text));
                }
                else if (testvariable == Globals.DependendVariableTransforms.Log10)
                {
                    retVal = Math.Pow(10.0, value);
                    retVal = Math.Pow(retVal, Convert.ToDouble(txtPower.Text));
                }
                else if (testvariable == Globals.DependendVariableTransforms.Ln)
                {
                    retVal = Math.Pow(Math.E, value);
                    retVal = Math.Pow(retVal, Convert.ToDouble(txtPower.Text));
                }
                else if (testvariable == Globals.DependendVariableTransforms.Power)
                {
                    retVal = Math.Pow(value, 1.0/_importedExp);
                    retVal = Math.Pow(retVal, Convert.ToDouble(txtPower.Text));
                }
            }
 

               return retVal;
        }

        private double GetDCRSTransformedValue(double value)
        {
            //get the threshold value converted, driven by UI threshold radio button values
            //radio button values are set onformenter by RV specification in dataprocessing
            //via table extended properties or by predictioninfo data in the modelopen 
            //(via projectopenlistener) case. this will put thresholds in the same transform
            //state as the response variable for use in exceedence probability calculations
            //and model error determination (FN/FP counts).
            double retValue = 0.0;

            if (rbLog10.Checked)
                retValue = Math.Log10(value);
            else if (rbLn.Checked)
                retValue = Math.Log(value);
            else if (rbPower.Checked)
            {
                double power = Convert.ToDouble(txtPower.Text);
                retValue = Math.Pow(value, power);
            }
            else
                retValue = value;

            return retValue;

        }

        private double GetInverseTransformValue(double value)
        {
            double retValue = 0.0d;
            Globals.DependendVariableTransforms dvt = GetTransformType();

            if (dvt == Globals.DependendVariableTransforms.Log10)
            {
                retValue = Math.Pow(10, value);
            }
            else if (dvt == Globals.DependendVariableTransforms.Ln)
            {
                retValue = Math.Pow(Math.E, value);
            }
            else if (dvt == Globals.DependendVariableTransforms.Power)
            {
                double power = getTransformPower(_respVarTransform);
                if (power == double.NaN)
                    power = 1.0;
                retValue = Math.Sign(value) * Math.Pow(Math.Abs(value), (1.0 / power));
            }
            else
                retValue = value;

            return retValue;
        }

        private string GetStringThreshold(string threshold)
        {
            string retString = string.Empty;
            if (rbLog10.Checked)
                retString = "Log10(" + threshold + ")";
            else if (rbLn.Checked)
                retString = "Ln(" + threshold + ")";
            else if (rbPower.Checked)
                retString = "(" + threshold + ")" + "**" + txtPower.Text;
            else if (rbNone.Checked)
                retString = threshold;


            return retString;
        }

        private double getTransformPower(string pwrTransform)
        {
            if (String.IsNullOrWhiteSpace(pwrTransform))
                return double.NaN;

            char[] delim = ",".ToCharArray();
            string[] svals = pwrTransform.Split(delim);

            double power = 1.0;
            if (svals.Length != 2)
                 return double.NaN;

            if (!Double.TryParse(svals[1], out power))
                return double.NaN;

            return power;
            
        }

        private VBTools.Globals.DependendVariableTransforms GetTransformType()
        {
            Globals.DependendVariableTransforms dvt = Globals.DependendVariableTransforms.none;

            if (String.Compare(_respVarTransform, Globals.DependendVariableTransforms.Log10.ToString(), 0) == 0)
                dvt = Globals.DependendVariableTransforms.Log10;
            else if (String.Compare(_respVarTransform, Globals.DependendVariableTransforms.Ln.ToString(), 0) == 0)
                dvt = Globals.DependendVariableTransforms.Ln;
            else if (_respVarTransform.Contains(Globals.DependendVariableTransforms.Power.ToString()))
                dvt = Globals.DependendVariableTransforms.Power;

            return dvt;
        }

        //Keeps the Import Obs button lined up with the Obs grid
        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            //btnImportObs.Left =  e.X + 10;
        }


        private void splitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {
            int left1 = splitContainer1.Panel2.Left;
            int left2 = splitContainer2.Panel2.Left;
            //btnPlot.Left = left1 + left2 + 10;
        }

        //keeps the Import Obs button level with the Import IVs button
        private void btnImportIVs_Move(object sender, EventArgs e)
        {
           // btnImportObs.Top = btnImportIVs.Top;
           // btnPlot.Top = btnImportIVs.Top;
        }

        private void dgvVariables_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            //int rowIdx = e.RowIndex;
            //if (rowIdx < dgvObs.Rows.Count)
            //    dgvObs.Rows[rowIdx].Selected = true;

            //if (rowIdx < dgvStats.Rows.Count)
            //    dgvStats.Rows[rowIdx].Selected = true;
            
        }

        private void dgvObs_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            //int rowIdx = e.RowIndex;
            //if (rowIdx < dgvVariables.Rows.Count)
            //    dgvVariables.Rows[rowIdx].Selected = true;

            //if (rowIdx < dgvStats.Rows.Count)
            //    dgvStats.Rows[rowIdx].Selected = true;
        }

        private void dgvStats_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            //int rowIdx = e.RowIndex;
            //if (rowIdx < dgvVariables.Rows.Count)
            //    dgvVariables.Rows[rowIdx].Selected = true;

            //if (rowIdx < dgvObs.Rows.Count)
            //    dgvObs.Rows[rowIdx].Selected = true;
        }

 
 

        private void dgvVariables_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            //int rowIdx = e.RowIndex;
            //if (rowIdx < dgvObs.Rows.Count)
            //    dgvObs.Rows[rowIdx].Selected = false;

            //if (rowIdx < dgvStats.Rows.Count)
            //    dgvStats.Rows[rowIdx].Selected = false;
        }

        private void dgvObs_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            //int rowIdx = e.RowIndex;
            //if (rowIdx < dgvVariables.Rows.Count)
            //    dgvVariables.Rows[rowIdx].Selected = false;

            //if (rowIdx < dgvStats.Rows.Count)
            //    dgvStats.Rows[rowIdx].Selected = false;
        }

        private void dgvStats_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            //int rowIdx = e.RowIndex;
            //if (rowIdx < dgvVariables.Rows.Count)
            //    dgvVariables.Rows[rowIdx].Selected = false;

            //if (rowIdx < dgvObs.Rows.Count)
            //    dgvObs.Rows[rowIdx].Selected = false;
        }

        private void btnExportTable_Click(object sender, EventArgs e)
        {

            dgvVariables.EndEdit();
            _dtVariables = (DataTable)dgvVariables.DataSource;
            if (_dtVariables != null)
                _dtVariables.AcceptChanges();
            else
                return;

            dgvObs.EndEdit();
            _dtObs = (DataTable)dgvObs.DataSource;
            if (_dtObs != null)
                _dtObs.AcceptChanges();

            dgvStats.EndEdit();
            _dtStats = (DataTable)dgvStats.DataSource;
            if (_dtStats != null)
                _dtStats.AcceptChanges();

            if ((_dtVariables == null) && (_dtObs == null) && (_dtStats == null))
                return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Export Prediction Data";
            sfd.Filter = @"CSV Files|*.csv|All Files|*.*";

            DialogResult dr = sfd.ShowDialog();
            if (dr == DialogResult.Cancel)
                return;

            int maxRowsVars = _dtVariables.Rows.Count;
            int maxRowsObs = 0;
            int maxRowsStats = 0;
            
            if (_dtObs != null)
                maxRowsObs = _dtObs.Rows.Count;
            if (_dtStats != null)
                maxRowsStats = _dtStats.Rows.Count;

            int maxRows = Math.Max(maxRowsVars, Math.Max(maxRowsObs, maxRowsStats));

            StringBuilder sb = new StringBuilder();

            //Write out the column headers
            if (_dtVariables != null)
            {
                for (int i = 0; i < _dtVariables.Columns.Count; i++)
                {
                    if (i > 0)
                        sb.Append(",");

                    if (_dtVariables.Columns[i].Caption == "ID")
                        sb.Append("ObsID");
                    else
                        sb.Append(_dtVariables.Columns[i].ColumnName);
                }
            }

            if (_dtObs != null)
            {
                for (int i = 0; i < _dtObs.Columns.Count; i++)
                {
                    sb.Append(",");
                    sb.Append(_dtObs.Columns[i].ColumnName);
                }
            }

            if (_dtStats != null)
            {
                for (int i = 0; i < _dtStats.Columns.Count; i++)
                {
                    sb.Append(",");
                    sb.Append(_dtStats.Columns[i].ColumnName);
                }
            } //Finished writing out column headers
            sb.Append(Environment.NewLine);

            //write out the data
            for (int i = 0; i < maxRows; i++)
            {
                for (int j = 0; j < _dtVariables.Columns.Count; j++)
                {
                    if (j > 0)
                        sb.Append(",");

                    if (i < _dtVariables.Rows.Count)
                        sb.Append(_dtVariables.Rows[i][j].ToString());
                    else
                        sb.Append("");
                }
               

                if (_dtObs != null)
                {
                    for (int j = 0; j < _dtObs.Columns.Count; j++)
                    {                        
                        sb.Append(",");

                        if (i < _dtObs.Rows.Count)
                            sb.Append(_dtObs.Rows[i][j].ToString());
                        else
                            sb.Append("");
                    }    
                }

                if (_dtStats != null)
                {
                    for (int j = 0; j < _dtStats.Columns.Count; j++)
                    {
                        sb.Append(",");

                        if (i < _dtStats.Rows.Count)
                            sb.Append(_dtStats.Rows[i][j].ToString());
                        else
                            sb.Append("");
                    }
                }

                sb.Append(Environment.NewLine);
            } //End writing out data
            

            string fileName = sfd.FileName;

            StreamWriter sw = new StreamWriter(fileName);
            sw.Write(sb.ToString());
            sw.Close();

            
        }

        private StringBuilder AddCommanSerparatedColumns(DataTable dt, StringBuilder sb)
        {
            if ((dt == null) || (dt.Columns.Count < 1))
                return sb;

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (i > 0)
                    sb.Append(",");

                sb.Append(dt.Columns[i].ColumnName);
            }

            return sb;
        }

        private StringBuilder AddRow(DataTable dt, StringBuilder sb)
        {
            if ((dt == null) || (dt.Columns.Count < 1))
                return sb;

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (i > 0)
                    sb.Append(",");

                sb.Append(dt.Columns[i].ColumnName);
            }

            return sb;
        }

        private void btnImportTable_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Open Prediction Data";
            ofd.Filter = @"VB2 Prediction Files|*.vbpred|All Files|*.*";
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.Cancel)
                return;

            string fileName = ofd.FileName;

            DataSet ds = new DataSet();
            ds.ReadXml(fileName, XmlReadMode.ReadSchema);

            if (ds.Tables.Contains("Variables") == false)
            {
                MessageBox.Show("Invalid Prediction Dataset.  Does not contain variable information.");
                return;
            }


            _dtVariables = ds.Tables["Variables"];
            _dtObs = ds.Tables["Observations"];
            _dtStats = ds.Tables["Stats"];

            dgvVariables.DataSource = _dtVariables;
            dgvObs.DataSource = _dtObs;
            dgvStats.DataSource = _dtStats;

        }

        private void btnClearTable_Click(object sender, EventArgs e)
        {
            dgvVariables.DataSource = null;
            dgvObs.DataSource = null;
            dgvStats.DataSource = null;

            if (_dtVariables != null)
                _dtVariables.Clear();            
            _dtVariables = null;

            if (_dtObs != null)
                _dtObs.Clear();            
            _dtObs = null;

            if (_dtStats != null)
                _dtStats.Clear();            
            _dtStats = null;

            dgvVariables.DataSource = CreateEmptyIVsDataTable();
            dgvObs.DataSource = CreateEmptyObservationsDataTable();
        }

        private void btnPlot_Click(object sender, EventArgs e)
        {
            //frmMLRPredPlot frmPlot = new frmMLRPredPlot(_dtObs, _dtStats);
            dgvObs.EndEdit();
            dgvStats.EndEdit();
            
            DataTable dtObs = dgvObs.DataSource as DataTable;
            DataTable dtStats = dgvStats.DataSource as DataTable;
            //DataTable dtIVs = dgvVariables.DataSource as DataTable;

            if ((dtObs == null) || (dtObs.Rows.Count < 1))
            {
                MessageBox.Show("Plotting requires Observation data.");
                return;
            }

            if ((dtStats == null) || (dtStats.Rows.Count < 1))
            {
                MessageBox.Show("Plotting requires Prediction data.");
                return;
            }

            //observations and predictions have to be synched up...
            DataTable dt = dtObs.Copy();
            List<int> recs = new List<int>();
            for (int i = 0; i < dtObs.Rows.Count; i++)
            {
                string idObs = dtObs.Rows[i]["ID"].ToString();
                bool found = false;
                for (int j = 0; j < dtStats.Rows.Count; j++)
                {
                    found = false;
                    string idStats = dtStats.Rows[j]["ID"].ToString();
                    if (idStats == idObs)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found) recs.Add(i);// dt.Rows[i].Delete();
            }
            for (int li = 0; li < recs.Count; li++)
            {
                int rec = recs[li];
                dt.Rows[rec].Delete();
            }
            dt.AcceptChanges();

            //use the global dtStats - local screws up plotting because of view state of predictions.
            frmMLRPredObs frmPlot = new frmMLRPredObs(dt, _dtStats, txtDecCrit.Text, txtRegStd.Text,
                GetTransformType(), Convert.ToDouble(txtPower.Text), _obsState, _importedExp,
                _predView, _predictionViewExponent);
            frmPlot.Show();

            //frmMLRPredPlot frmP = new frmMLRPredPlot(dtObs, dtStats);
            //frmP.Show();
        }

        private DataTable CreateEmptyIVsDataTable()
        {
            //We are going to put an ID column first.
            //ID is used to link IV and Obs records.

            DataTable dt = new DataTable("Variables");

            dt.Columns.Add("ID", typeof(string));

            for (int i = 1; i < _referencedVariables.Length;i++)
                dt.Columns.Add(_referencedVariables[i], typeof(double));
                       
            return dt;            
        }

        private DataTable CreateEmptyObservationsDataTable()
        {
            DataTable dt = new DataTable("Observations");
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("Observation", typeof(double));

            return dt;
        }

        private void dgvVariables_SelectionChanged(object sender, EventArgs e)
        {

            dgvObs.SelectionChanged -= new EventHandler(dgvObs_SelectionChanged);
            dgvStats.SelectionChanged -= new EventHandler(dgvStats_SelectionChanged);

            DataGridViewSelectedRowCollection selRowCol = dgvVariables.SelectedRows;
            if (dgvObs.DataSource != null)
                dgvObs.ClearSelection();

            if (dgvStats.DataSource != null)
                dgvStats.ClearSelection();

            

            for (int i=0;i<selRowCol.Count;i++)
            {
                if (dgvObs.DataSource != null)
                {
                    if (selRowCol[i].Index < dgvObs.Rows.Count)
                        dgvObs.Rows[selRowCol[i].Index].Selected = true;
                }

                if (dgvStats.DataSource != null)
                {
                    if (selRowCol[i].Index < dgvStats.Rows.Count)
                        dgvStats.Rows[selRowCol[i].Index].Selected = true;
                }
            }

            dgvObs.SelectionChanged += new EventHandler(dgvObs_SelectionChanged);
            dgvStats.SelectionChanged += new EventHandler(dgvStats_SelectionChanged);
        }

        private void dgvObs_SelectionChanged(object sender, EventArgs e)
        {
            dgvVariables.SelectionChanged -= new EventHandler(dgvVariables_SelectionChanged);
            dgvStats.SelectionChanged -= new EventHandler(dgvStats_SelectionChanged);

            DataGridViewSelectedRowCollection selRowCol = dgvObs.SelectedRows;

           
            if (dgvVariables.DataSource != null)
                dgvVariables.ClearSelection();

            if (dgvStats.DataSource != null)
                dgvStats.ClearSelection();

            for (int i = 0; i < selRowCol.Count; i++)
            {
                if (dgvVariables.DataSource != null)
                {
                    if (selRowCol[i].Index < dgvVariables.Rows.Count)
                        dgvVariables.Rows[selRowCol[i].Index].Selected = true;
                }

                if (dgvStats.DataSource != null)
                {
                    if (selRowCol[i].Index < dgvStats.Rows.Count)
                        dgvStats.Rows[selRowCol[i].Index].Selected = true;
                }
            }

            dgvVariables.SelectionChanged += new EventHandler(dgvVariables_SelectionChanged);
            dgvStats.SelectionChanged += new EventHandler(dgvStats_SelectionChanged);
        }

        private void dgvStats_SelectionChanged(object sender, EventArgs e)
        {
            dgvVariables.SelectionChanged -= new EventHandler(dgvVariables_SelectionChanged);
            dgvObs.SelectionChanged -= new EventHandler(dgvObs_SelectionChanged);

            DataGridViewSelectedRowCollection selRowCol = dgvStats.SelectedRows;
            if (dgvVariables.DataSource != null)
                dgvVariables.ClearSelection();

            if (dgvObs.DataSource != null)
                dgvObs.ClearSelection();

            for (int i = 0; i < selRowCol.Count; i++)
            {
                if (dgvVariables.DataSource != null)
                {
                    if (selRowCol[i].Index < dgvVariables.Rows.Count)
                        dgvVariables.Rows[selRowCol[i].Index].Selected = true;
                }

                if (dgvObs.DataSource != null)
                {
                    if (selRowCol[i].Index < dgvObs.Rows.Count)
                        dgvObs.Rows[selRowCol[i].Index].Selected = true;
                }
            }

            dgvVariables.SelectionChanged += new EventHandler(dgvVariables_SelectionChanged);
            dgvObs.SelectionChanged += new EventHandler(dgvObs_SelectionChanged);
        }


        public void setViewOnGrid(DataGridView dgv)
        {
            if (dgv.Rows.Count <= 1) return;

            string testcellval = string.Empty;
            //double numval = double.NaN;
            for (int col = 0; col < dgv.Columns.Count; col++)
            {
                testcellval = dgv[col, 0].Value.ToString();
                double result;
                bool isNum = Double.TryParse(testcellval, out result); 
                if (isNum)
                {
                    dgv.Columns[col].ValueType = typeof(System.Double);
                    dgv.Columns[col].DefaultCellStyle.Format = "g4";
                }
                else
                {
                    dgv.Columns[col].ValueType = typeof(System.String);
                }

            }
        }

        private List<string> getBadCells(DataTable dt, bool skipFirstColumn)
        {
            double result;
            if (dt == null)
                return null;

            List<string> cells = new List<string>();
            foreach (DataColumn dc in dt.Columns)
            {
                if (skipFirstColumn)
                {
                    if (dt.Columns.IndexOf(dc) == 0)
                        continue;
                }
                foreach (DataRow dr in dt.Rows)
                {
                    if (string.IsNullOrEmpty(dr[dc].ToString()))
                        cells.Add("Row " + dr[0].ToString() + " Column " + dc.Caption + " has blank cell.");
                    else if (!Double.TryParse(dr[dc].ToString(), out result) && dt.Columns.IndexOf(dc) != 0)
                        cells.Add("Row " + dr[0].ToString() + " Column " + dc.Caption + " has non-numeric cell value: '" + dr[dc].ToString() + "'");
                }
            }


            return cells;
        }

        private void dgvVariables_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            string err = "Data value must be numeric.";
            dgvVariables.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
            MessageBox.Show(err);
        }

        private void dgvObs_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            string err = "Data value must be numeric.";
            dgvObs.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
            MessageBox.Show(err);
        }

        private void rbPower_CheckedChanged(object sender, EventArgs e)
        {
            if (rbPower.Checked)
                txtPower.Enabled = true;
            else
                txtPower.Enabled = false;
        }



        private bool ValidateNumericTextBox(TextBox txtBox)
        {
            double val = 1.0;
            if (!Double.TryParse(txtBox.Text, out val))
            {
                MessageBox.Show(txtPower.Text + "Invalid number.");
                txtPower.Focus();
                return false;
            }

            return true;

        }

        private void txtDecCrit_Leave(object sender, EventArgs e)
        {
            double result;
            if (!Double.TryParse(txtDecCrit.Text, out result))
            {
                MessageBox.Show("Invalid number.");
                txtDecCrit.Focus();
            }

        }

        private void txtRegStd_Leave(object sender, EventArgs e)
        {
            double result;
            if (!Double.TryParse(txtRegStd.Text, out result))
            {
                MessageBox.Show("Invalid number.");
                txtRegStd.Focus();
            }
        }


        private void btnIVDataValidation_Click(object sender, EventArgs e)
        {
            DataTable dt = dgvVariables.DataSource as DataTable;
            if ((dt == null) ||(dt.Rows.Count < 1))
                return;

            DataTable dtCopy = dt.Copy();
            DataTable dtSaved = dt.Copy();
            frmMissingValues frmMissVal = new frmMissingValues(dgvVariables, dtCopy);
            frmMissVal.ShowDialog();
            if (frmMissVal.Status)
            {
                int errndx;
                if (!recordIndexUnique(frmMissVal.ValidatedDT, out errndx))
                {
                    MessageBox.Show("Unable to process datasets with non-unique record identifiers.\n" +
                                    "Fix your datatable by assuring unique record identifier values\n" +
                                    "in the ID column and try validating again.\n\n" +
                                    "Record Identifier values cannot be blank or duplicated;\nencountered " +
                                    "error near row " + errndx.ToString(), "Data Validation Error - Cannot Process This Dataset", MessageBoxButtons.OK);
                    return;
                }
                dgvVariables.DataSource = frmMissVal.ValidatedDT;
                btnMakePredictions.Enabled = true;
            }
            else
            {
                dgvVariables.DataSource = dtSaved;
            }

        }

        private void dgvObs_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            showContextMenus((DataGridView)sender, e);
        }

        private void dgvStats_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            //showPredictionContextMenu((DataGridView)sender, e);
            showContextMenus((DataGridView)sender, e);
        }
        private void showContextMenus(DataGridView dgv, MouseEventArgs me)
        {
            DataGridView.HitTestInfo ht = dgv.HitTest(me.X, me.Y);
            int colndx = ht.ColumnIndex;
            int rowndx = ht.RowIndex;

            //DataTable _dt = (DataTable)dgvObs.DataSource;
            if (rowndx > 0 && colndx > 0) return; //cell hit, go away

            if (rowndx < 0 && colndx == 1)
            {
                setMenuItemState(_obsState, cmforResponseVar);
                     if (dgv.Name == "dgvObs")
                         cmforResponseVar.Show(dgv, new Point(me.X, me.Y));
                     else if (dgv.Name == "dgvStats")
                         cmforPrediction.Show(dgv, new Point(me.X, me.Y));
            }
        }

        //private void showPredictionContextMenu(DataGridView dgv, MouseEventArgs me)
        //{
        //    DataGridView.HitTestInfo ht = dgv.HitTest(me.X, me.Y);
        //    int colndx = ht.ColumnIndex;
        //    int rowndx = ht.RowIndex;

        //    //DataTable _dt = (DataTable)dgvObs.DataSource;
        //    if (rowndx > 0 && colndx > 0) return; //cell hit, go away

        //    if (rowndx < 0 && colndx >= 1)
        //    {
        //        setMenuItemState();
        //        cmforResponseVar.Show(dgv, new Point(me.X, me.Y));
        //    }
        //}

        private void frmMLRPrediction_Load(object sender, EventArgs e)
        {
            //cmforResponseVar.MenuItems.Add("Imported Observations are:");
            //cmforResponseVar.MenuItems[0].MenuItems.Add("None", new EventHandler(none));
            //cmforResponseVar.MenuItems[0].MenuItems.Add("Log10", new EventHandler(log10T));
            //cmforResponseVar.MenuItems[0].MenuItems.Add("Ln", new EventHandler(lnT));
            //cmforResponseVar.MenuItems[0].MenuItems.Add("Power", new EventHandler(powerT));
            cmforResponseVar.MenuItems.Add("Set observation transform:");
            cmforResponseVar.MenuItems[0].MenuItems.Add("None", new EventHandler(handler));
            cmforResponseVar.MenuItems[0].MenuItems.Add("Log10", new EventHandler(handler));
            cmforResponseVar.MenuItems[0].MenuItems.Add("Ln", new EventHandler(handler));
            cmforResponseVar.MenuItems[0].MenuItems.Add("Power", new EventHandler(handler));

            cmforPrediction.MenuItems.Add("Set prediction view:");
            cmforPrediction.MenuItems[0].MenuItems.Add("None", new EventHandler(handler2));
            cmforPrediction.MenuItems[0].MenuItems.Add("Log10", new EventHandler(handler2));
            cmforPrediction.MenuItems[0].MenuItems.Add("Ln", new EventHandler(handler2));
            cmforPrediction.MenuItems[0].MenuItems.Add("Power", new EventHandler(handler2));


            //cmforResponseVar.MenuItems.Add("Compare to Untransformed Predictions", new EventHandler(c2up));
            //cmforResponseVar.MenuItems.Add("Compare to Predictions", new EventHandler(c2p));

        }

        private void handler(object o, EventArgs e)
        {
            MenuItem mi = (MenuItem)o;
            string cmitem = mi.Text;
            if (cmitem == "None")
            {
                _obsState = Globals.DependendVariableTransforms.none;
                lblObsTransform.Text = "Obs Transform: " + _obsState.ToString();
            }
            else if (cmitem == "Log10")
            {
                _obsState = Globals.DependendVariableTransforms.Log10;
                lblObsTransform.Text = "Obs Transform: " + _obsState.ToString();
            }
            else if (cmitem == "Ln")
            {
                _obsState = Globals.DependendVariableTransforms.Ln;
                lblObsTransform.Text = "Obs Transform: " + _obsState.ToString();
                
            }
            else if (cmitem == "Power")
            {
                _obsState = Globals.DependendVariableTransforms.Power;
                frmPowerExponent getExpDlg = new frmPowerExponent(_dtObs, 1, false);
                getExpDlg.ShowDialog();
                _importedExp = getExpDlg.Exponent;
                //lblObsTransform.Text = "Obs Transform: " + _importedobsstate.ToString();
                lblObsTransform.Text = "Obs Transform: (Obs)**" + _importedExp.ToString("n");
            }


            setMenuItemState(_obsState, cmforResponseVar);
        }

        private void handler2(object o, EventArgs e)
        {
            MenuItem mi = (MenuItem)o;
            string cmitem = mi.Text;

            if (cmitem == "None")
            {
                //_importedobsstate = _importedObsState.None;
                _predView = Globals.DependendVariableTransforms.none;
                //lblObsTransform.Text = "Obs Transform: " + _importedobsstate.ToString();
            }
            else if (cmitem == "Log10")
            {
                _predView = Globals.DependendVariableTransforms.Log10;
                //lblObsTransform.Text = "Obs Transform: " + _importedobsstate.ToString();
            }
            else if (cmitem == "Ln")
            {
                _predView = Globals.DependendVariableTransforms.Ln;
                //lblObsTransform.Text = "Obs Transform: " + _importedobsstate.ToString();

            }
            else if (cmitem == "Power")
            {
                _predView = Globals.DependendVariableTransforms.Power;
                frmPowerExponent getExpDlg = new frmPowerExponent(_dtObs, 1, false);
                getExpDlg.ShowDialog();
                _predictionViewExponent = getExpDlg.Exponent;
                //lblObsTransform.Text = "Obs Transform: " + _importedobsstate.ToString();
                //lblObsTransform.Text = "Obs Transform: (Obs)**" + _importedExp.ToString("n");
            }

            ExpressionEvaluator expEval = new ExpressionEvaluator();
            DataTable dtPredictions = expEval.Evaluate(_modelExpression, _dtVariables);//, false);
            //_dtStats = GeneratePredView(dtPredictions, _dtObs);
            dgvStats.DataSource = GeneratePredView(dtPredictions, _dtObs);

            setMenuItemState(_predView, cmforPrediction);
        }

        private DataTable GeneratePredView(DataTable dtPredictions, DataTable _dtObs)
        {
            DataTable dt = _dtStats.Copy();
            //_importedObsState teststate = _importedObsState.None;
            //if (lblRVTransform.Text.Contains("None"))
            //    teststate = _importedObsState.None;
            //else if (lblRVTransform.Text.Contains("Log10"))
            //    teststate = _importedObsState.Log10;
            //else if (lblRVTransform.Text.Contains("Ln"))
            //    teststate = _importedObsState.Ln;
            //else if (lblRVTransform.Text.Contains("(RV)**"))
            //    teststate = _importedObsState.Power;

            for (int dr = 0; dr < dtPredictions.Rows.Count; dr++)
            {
                if (_predView == Globals.DependendVariableTransforms.none )
                {
                    if (lblRVTransform.Text.Contains("None"))
                        dt.Rows[dr]["Model_Prediction"] = (double)dtPredictions.Rows[dr]["CalcValue"];
                    else if (lblRVTransform.Text.Contains("Log10"))
                        dt.Rows[dr]["Model_Prediction"] = Math.Pow(10.0, (double)dtPredictions.Rows[dr]["CalcValue"]);
                    else if (lblRVTransform.Text.Contains("Ln"))
                        dt.Rows[dr]["Model_Prediction"] = Math.Pow(Math.E, (double)dtPredictions.Rows[dr]["CalcValue"]);
                    else if (lblRVTransform.Text.Contains("(RV)**"))
                    {
                        dt.Rows[dr]["Model_Prediction"] = Math.Pow((double)dtPredictions.Rows[dr]["CalcValue"], 1.0 / _predictionViewExponent);
                    }

                }
                else if ( _predView == Globals.DependendVariableTransforms.Log10)
                {
                    dt.Rows[dr]["Model_Prediction"] = Math.Sign((double)dt.Rows[dr]["Model_Prediction"]) * Math.Log10(Math.Abs( (double)dt.Rows[dr]["Model_Prediction"]));
                }
                else if ( _predView == Globals.DependendVariableTransforms.Ln)
                {
                    dt.Rows[dr]["Model_Prediction"] = Math.Sign((double)dt.Rows[dr]["Model_Prediction"]) * Math.Log(Math.Abs( (double)dt.Rows[dr]["Model_Prediction"]));
                }
                else if ( _predView ==  Globals.DependendVariableTransforms.Power)
                {
                    dt.Rows[dr]["Model_Prediction"] = Math.Pow((double)dtPredictions.Rows[dr]["CalcValue"], _predictionViewExponent);
                }
            }
            return dt;
        }

        //private void none(object o, EventArgs e)
        //{
        //    if (_dtObsOrig != null)
        //    {
        //        DataColumn dc = _dtObsOrig.Columns[1];
        //        dc.ColumnName = "Observation";
        //        dgvObs.DataSource = _dtObsOrig;
        //        //_obsTransformed = false;


        //    }
        //    //_importedobsstate = _importedObsState.None;
        //}

        //private void log10T(object o, EventArgs e)
        //{

        //    dgvObs.EndEdit();
        //    DataTable _dt = (DataTable)dgvObs.DataSource;
        //    DataTable _dtCopy = _dt.Copy();
        //    if (_dt == null) return;
        //    _dtObsOrig = _dt;
        //    //can only transform the response variable and the response variable can not be a transformed ME or and interacted ME
        //    Transform t = new Transform(_dt, 1);
        //    double[] newvals = new double[_dt.Rows.Count];
        //    newvals = t.LOG10;
        //    if (t.Message != "")
        //    {
        //        MessageBox.Show("Cannot Log10 transform variable. " + t.Message, "VB Transform Rule", MessageBoxButtons.OK);
        //        return;
        //    }

        //    //_obsTransformed = true;

        //    for (int i = 0; i < _dtCopy.Rows.Count; i++)
        //    {
        //        _dtCopy.Rows[i][1] = newvals[i];
        //    }

        //    DataColumn dc = _dtCopy.Columns["Observation"];
        //    dc.ColumnName = "LOG10[Observation]";

        //    dgvObs.DataSource = _dtCopy;

        //    //_importedobsstate = _importedObsState.Log10;
        //}

        //private void lnT(object o, EventArgs e)
        //{
        //    dgvObs.EndEdit();
        //    DataTable _dt = (DataTable)dgvObs.DataSource;
        //    DataTable _dtCopy = _dt.Copy();
        //    if (_dt == null) return;
        //    _dtObsOrig = _dt;
        //    Transform t = new Transform(_dt, 1);
        //    double[] newvals = new double[_dt.Rows.Count];
        //    newvals = t.LOGE;
        //    if (t.Message != "")
        //    {
        //        MessageBox.Show("Cannot Ln transform variable. " + t.Message, "VB Transform Rule", MessageBoxButtons.OK);
        //        return;
        //    }

        //    //_obsTransformed = true;

        //    for (int i = 0; i < _dtCopy.Rows.Count; i++)
        //    {
        //        _dtCopy.Rows[i][1] = newvals[i];
        //    }

        //    DataColumn dc = _dtCopy.Columns["Observation"];
        //    dc.ColumnName = "LN[Observation]";

        //    dgvObs.DataSource = _dtCopy;

        //    //_importedobsstate = _importedObsState.Ln;
        //}

        //private void powerT(object o, EventArgs e)
        //{
        //    dgvObs.EndEdit();
        //    DataTable _dt = (DataTable)dgvObs.DataSource;
        //    DataTable _dtCopy = _dt.Copy();
        //    if (_dt == null) return;
        //    _dtObsOrig = _dt;

        //    frmPowerExponent frmExp = new frmPowerExponent(_dt, 1);
        //    DialogResult dlgr = frmExp.ShowDialog();
        //    if (dlgr != DialogResult.Cancel)
        //    {
        //        double[] newvals = new double[_dt.Rows.Count];
        //        newvals = frmExp.TransformedValues;
        //        if (frmExp.TransformMessage != "")
        //        {
        //            MessageBox.Show("Cannot Power transform variable. " + frmExp.TransformMessage, "VB Transform Rule", MessageBoxButtons.OK);
        //            return;
        //        }

        //        //_obsTransformed = true;
        //        string sexp = frmExp.Exponent.ToString("n2");
        //        for (int i = 0; i < _dtCopy.Rows.Count; i++)
        //        {
        //            _dtCopy.Rows[i][1] = newvals[i];
        //        }

        //        DataColumn dc = _dtCopy.Columns["Observation"];
        //        dc.ColumnName = "POWER[" + sexp + ",Observation]";
        //        dgvObs.DataSource = _dtCopy;

        //        //_importedobsstate = _importedObsState.Power;
        //    }

        //}

        private void setMenuItemState(Globals.DependendVariableTransforms testvariable, ContextMenu menu)
        {
            if (testvariable == Globals.DependendVariableTransforms.none)
            {
                menu.MenuItems[0].MenuItems[0].Checked = true;
                menu.MenuItems[0].MenuItems[1].Checked = !menu.MenuItems[0].MenuItems[0].Checked;
                menu.MenuItems[0].MenuItems[2].Checked = !menu.MenuItems[0].MenuItems[0].Checked;
                menu.MenuItems[0].MenuItems[3].Checked = !menu.MenuItems[0].MenuItems[0].Checked;
                //cmforResponseVar.MenuItems[0].MenuItems[0].Checked = true;
                //cmforResponseVar.MenuItems[0].MenuItems[1].Checked = !cmforResponseVar.MenuItems[0].MenuItems[0].Checked;
                //cmforResponseVar.MenuItems[0].MenuItems[2].Checked = !cmforResponseVar.MenuItems[0].MenuItems[0].Checked;
                //cmforResponseVar.MenuItems[0].MenuItems[3].Checked = !cmforResponseVar.MenuItems[0].MenuItems[0].Checked;
            }
            else if (testvariable == Globals.DependendVariableTransforms.Log10)
            {
                menu.MenuItems[0].MenuItems[1].Checked = true;
                menu.MenuItems[0].MenuItems[0].Checked = !menu.MenuItems[0].MenuItems[1].Checked;
                menu.MenuItems[0].MenuItems[2].Checked = !menu.MenuItems[0].MenuItems[1].Checked;
                menu.MenuItems[0].MenuItems[3].Checked = !menu.MenuItems[0].MenuItems[1].Checked;
                //cmforResponseVar.MenuItems[0].MenuItems[1].Checked = true;
                //cmforResponseVar.MenuItems[0].MenuItems[0].Checked = !cmforResponseVar.MenuItems[0].MenuItems[1].Checked;
                //cmforResponseVar.MenuItems[0].MenuItems[2].Checked = !cmforResponseVar.MenuItems[0].MenuItems[1].Checked;
                //cmforResponseVar.MenuItems[0].MenuItems[3].Checked = !cmforResponseVar.MenuItems[0].MenuItems[1].Checked;
            }
            else if (testvariable == Globals.DependendVariableTransforms.Ln)
            {
                menu.MenuItems[0].MenuItems[2].Checked = true;
                menu.MenuItems[0].MenuItems[1].Checked = !menu.MenuItems[0].MenuItems[2].Checked;
                menu.MenuItems[0].MenuItems[0].Checked = !menu.MenuItems[0].MenuItems[2].Checked;
                menu.MenuItems[0].MenuItems[3].Checked = !menu.MenuItems[0].MenuItems[2].Checked;
                //cmforResponseVar.MenuItems[0].MenuItems[2].Checked = true;
                //cmforResponseVar.MenuItems[0].MenuItems[1].Checked = !cmforResponseVar.MenuItems[0].MenuItems[2].Checked;
                //cmforResponseVar.MenuItems[0].MenuItems[0].Checked = !cmforResponseVar.MenuItems[0].MenuItems[2].Checked;
                //cmforResponseVar.MenuItems[0].MenuItems[3].Checked = !cmforResponseVar.MenuItems[0].MenuItems[2].Checked;
            }
            else if (testvariable == Globals.DependendVariableTransforms.Power)
            {
                menu.MenuItems[0].MenuItems[3].Checked = true;
                menu.MenuItems[0].MenuItems[1].Checked = !menu.MenuItems[0].MenuItems[3].Checked;
                menu.MenuItems[0].MenuItems[2].Checked = !menu.MenuItems[0].MenuItems[3].Checked;
                menu.MenuItems[0].MenuItems[0].Checked = !menu.MenuItems[0].MenuItems[3].Checked;
                //cmforResponseVar.MenuItems[0].MenuItems[3].Checked = true;
                //cmforResponseVar.MenuItems[0].MenuItems[1].Checked = !cmforResponseVar.MenuItems[0].MenuItems[3].Checked;
                //cmforResponseVar.MenuItems[0].MenuItems[2].Checked = !cmforResponseVar.MenuItems[0].MenuItems[3].Checked;
                //cmforResponseVar.MenuItems[0].MenuItems[0].Checked = !cmforResponseVar.MenuItems[0].MenuItems[3].Checked;
            }
        }

        private void dgvVariables_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //If user has edited the ID column, make sure the IDs are still unique.
            //if (String.Compare(dgvVariables.Columns[e.ColumnIndex].Name, "ID", true) == 0)
            //{
            //    string id = dgvVariables.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();



            //}
           
            btnMakePredictions.Enabled = false;
        }

        private void frmMLRPrediction_HelpRequested(object sender, HelpEventArgs hlpevent)
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

        private void dgvVariables_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            StringFormat sf = new StringFormat();
            int count = dgvVariables.RowCount;
            sf.Alignment = StringAlignment.Center;
            if(( e.ColumnIndex < 0) && (e.RowIndex >= 0) && (e.RowIndex < count) )
            {
                e.PaintBackground(e.ClipBounds, true);
                e.Graphics.DrawString((e.RowIndex + 1).ToString(), this.Font, Brushes.Black, e.CellBounds, sf);
                e.Handled = true;
            }
        }

        private void dgvObs_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex == 1)
            {
                string t = "Observation transform is set to " + _obsState.ToString();
                dgvObs.Columns[e.ColumnIndex].ToolTipText = t;
            }
        }

        private void dgvStats_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex == 1)
            {
                string t = "Prediction view is set to " + _predView.ToString();
                dgvStats.Columns[e.ColumnIndex].ToolTipText = t;
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


        /// <summary>
        /// get a row of mlr model variable values from a row of input grid table main effects
        /// </summary>
        /// <param name="drComponentXs">a table row from the input grid table</param>
        /// <returns>a vector (row) of mlr variable values</returns>
        private DataRow getMLRxVals(DataRow drComponentXs)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(string));
            DataRow dr = dt.NewRow();
            dr["ID"] = drComponentXs[0];

            foreach (KeyValuePair<string, double> kvp in _projMgr.Model)
            {
                if (kvp.Key == "(Intercept)") continue;
                //the parser confuses functions with variables when brackets encountered in expressions 
                string exp = kvp.Key.Replace('[', '(');
                exp = exp.Replace(']', ')');
                ExpressionEvaluator expEval = new ExpressionEvaluator();
                dt.Columns.Add(exp, typeof(double));
                dr[exp] = expEval.Evaluate(exp, drComponentXs);
            }

            dt.Rows.Add(dr);
 

            return dt.Rows[0];
        }

        private void tbPExceed_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(tbPExceed, "");
        }

        private void tbPExceed_Validating(object sender, CancelEventArgs e)
        {
            double pThreshold;
            if (!double.TryParse(tbPExceed.Text, out pThreshold))
            {
                e.Cancel = true;
                tbPExceed.Select(0, tbPExceed.Text.Length);
                this.errorProvider1.SetError(tbPExceed, "Probability threshold must be a number between 0 and 100.");
                return;
            }

            if ((pThreshold > 100.0) || (pThreshold < 0.0))
            {
                e.Cancel = true;
                tbPExceed.Select(0, tbPExceed.Text.Length);
                this.errorProvider1.SetError(tbPExceed, "Probability threshold must be a number between 0 and 100.");
                return;
            }
        }



    }
}
