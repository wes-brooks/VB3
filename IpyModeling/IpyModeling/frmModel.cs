using System;
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using ZedGraph;
using GALib;
using VBStatistics;
using MultipleLinearRegression;
using Combinatorics;
using WeifenLuo.WinFormsUI.Docking;
using VBTools;
using LogUtilities;
using CrossValidation;
using VBControls;
using System.Linq;
using System.Collections;
using System.Xml;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

//For IronPython extensions:
using IPyModelingControl;

namespace GALibForm
{
    public partial class frmModel : DockContent, IFormState
    {
        public event EventHandler ModelSelected;
        private DataTable _dtFull = null;

        private List<IIndividual> _list = null;

        private VBProjectManager _projMgr = null;

        private GALib.GAManager _gaManager = null;
        private ExhaustiveSearchManager _esManager = null;

        private volatile bool _cancelRun = false;
        private Thread _runThread = null;

        //Number of independent variables in the dataset
        private int _numVars;
        //Number of observations in the dataset
        int _numObs;
        //Maximum number of variables (including interaction terms) allowed in model
        int _maxIndVars;
        //Total number of variables (including interaction terms) available to model
        int _totVar;
        //Search space - total possible number of models for evaluation
        long _numModels;
        //
        int _userSpecifiedNumVars;

        //Maximum VIF allowed for model to be valid
        int _maxVIF;

        //Seed for random number generator
        int _seed;

        //Threshold value used for sensitiviy, specificity, accuracy
        double _decisionThreshold;
        double _mandateThreshold;

        //For reporting/plotting of optional eval criteria
        public static bool ThresholdChecked;

        //for ROC curve plotting
        private int[] _ndxs;

        private List<double[]> _XYPlotdata;

        private int _selectedModelIndex = -1;

        Dictionary<string, List<object>> _tableVals = null;

        private enum _mlrState { clean, dirty };
        private _mlrState _state = _mlrState.clean;

        private int _smi = -1;
        private int _pmi = -1;

        //Added for IronPython extension functionality:
        public event EventHandler IronPythonInterfaceAccessRequested;
        public event EventHandler SaveIPyModel;
        public event EventHandler ResetIPyProject;


        public frmModel()
        {
            InitializeComponent();
            _projMgr = VBProjectManager.GetProjectManager();
            _projMgr.ProjectOpened += new VBProjectManager.ProjectOpenedHandler(ProjectOpenedListener);

            //_projMgr.UnpackRequest += new VBProjectManager.EventHandler<UnpackEventArgs>(UnpackState);
            _projMgr.ProjectSaved += new VBProjectManager.ProjectSavedHandler<PackEventArgs>(ProjectSavedListener);

            //set a default model evaluation criterion - currently AIC is index = 0
            cbCriteria.SelectedIndex = 0;
        }

        /// <summary>
        /// Fires when the app opens a project file
        /// </summary>
        /// <param name="projMgr"></param>
        /// 
        public void UnpackState(object objPackedStates)
        {

            ModelingInfo formPackedState = (ModelingInfo)objPackedStates;
            //If either of these are null, then no project to open
            if (formPackedState == null)
                return;
            if (formPackedState.AvailableVariables == null) return;

            //Unpack the saved state of the PLS modeling control.
            this.Show();
            tabControl1.SelectedTab = tabControl1.TabPages["PLS"];

            ipyPLSControl.UnpackProjectState(formPackedState.PlsProject);

            //Return to the tab that was active when the project was saved.
            tabControl1.SelectedIndex = formPackedState.ActiveModelingTab;

            //Console.WriteLine("\n*** Modeling: project opened.***\n");

            //Save dependent variable transform
            if (formPackedState.DependentVariableTransform == Globals.DependentVariableTransforms.none)
                mlrPredObs1.Transform = Globals.DependentVariableTransforms.none;
            else if (formPackedState.DependentVariableTransform == Globals.DependentVariableTransforms.Log10)
                mlrPredObs1.Transform = Globals.DependentVariableTransforms.Log10;
            else if (formPackedState.DependentVariableTransform == Globals.DependentVariableTransforms.Ln)
                mlrPredObs1.Transform = Globals.DependentVariableTransforms.Ln;
            else if (formPackedState.DependentVariableTransform == Globals.DependentVariableTransforms.Power)
                mlrPredObs1.Transform = Globals.DependentVariableTransforms.Power;

            _projMgr.Model = formPackedState.Model;

            //Save available and independent variables
            lbAvailableVariables.Items.Clear();
            for (int i = 0; i < formPackedState.AvailableVariables.Count; i++)
                lbAvailableVariables.Items.Add(formPackedState.AvailableVariables[i]);

            lbIndVariables.Items.Clear();
            for (int i = 0; i < formPackedState.IndependentVariables.Count; i++)
                lbIndVariables.Items.Add(formPackedState.IndependentVariables[i]);

            lblAvailVars.Text = lbAvailableVariables.Items.Count.ToString();
            lblDepVars.Text = lbIndVariables.Items.Count.ToString();

            //Save the chromosomes
            if (_list != null)
                _list.Clear();
            else _list = new List<IIndividual>();

            _projMgr.ModelDataTable = CreateModelDataTable();

            MLRIndividual indiv = null;
            ModelingInfo modInfo = null;
            if (formPackedState.Chromosomes != null)
            {
                for (int i = 0; i < formPackedState.Chromosomes.Count; i++)
                {
                    modInfo = formPackedState;
                    int numGenes = formPackedState.Chromosomes[i].Count;
                    indiv = new MLRIndividual(numGenes, modInfo.MaxGeneValue, (FitnessCriteria)modInfo.FitnessCriteria, modInfo.MaxVIF, modInfo.DecisionThreshold, modInfo.MandatedThreshold);
                    indiv.Chromosome = formPackedState.Chromosomes[i];
                    indiv.Evaluate();
                    _list.Add((IIndividual)indiv);
                }

                UpdateFitnessListBox();
                if (formPackedState.SelectedModel > -1 && indiv.Parameters != null)
                    listBox1.SelectedIndex = formPackedState.SelectedModel;
            }

            _numObs = _projMgr.CorrelationDataTable.Rows.Count;
            lblNumObs.Text = "Number of Observations: " + _numObs.ToString();

            int maxVar = _numObs / 5;
            //int recVar = Math.Min(((_numObs / 10) + 1), availVar);
            int recVar = Math.Min(((_numObs / 10) + 1), (lbIndVariables.Items.Count));
            lblMaxAndRecommended.Text = "Recommended: " + recVar.ToString() + ", Max: " + maxVar.ToString();
            txtMaxVars.Text = recVar.ToString();

            this.Show();
            if (listBox1.Items.Count < 1 && ipyPLSControl.VirginState == true)
            {
                _projMgr._comms.sendMessage("Hide", this);
            }
        }

        private void ProjectOpenedListener()
        {
            //If either of these are null, then no project to open
            if (_projMgr.ModelingInfo == null)
                return;

            if (_projMgr.ModelingInfo.AvailableVariables == null) return;

            //Unpack the saved state of the PLS modeling control.
            this.Show();
            tabControl1.SelectedTab = tabControl1.TabPages["PLS"];
            ipyPLSControl.UnpackProjectState(_projMgr.ModelingInfo.PlsProject);

            //Return to the tab that was active when the project was saved.
            tabControl1.SelectedIndex = _projMgr.ModelingInfo.ActiveModelingTab;

            //Console.WriteLine("\n*** Modeling: project opened.***\n");

            //Save dependent variable transform
            if (_projMgr.ModelingInfo.DependentVariableTransform == Globals.DependentVariableTransforms.none)
                mlrPredObs1.Transform = Globals.DependentVariableTransforms.none;
            else if (_projMgr.ModelingInfo.DependentVariableTransform == Globals.DependentVariableTransforms.Log10)
                mlrPredObs1.Transform = Globals.DependentVariableTransforms.Log10;
            else if (_projMgr.ModelingInfo.DependentVariableTransform == Globals.DependentVariableTransforms.Ln)
                mlrPredObs1.Transform = Globals.DependentVariableTransforms.Ln;
            else if (_projMgr.ModelingInfo.DependentVariableTransform == Globals.DependentVariableTransforms.Power)
                mlrPredObs1.Transform = Globals.DependentVariableTransforms.Power;

            _projMgr.Model = _projMgr.ModelingInfo.Model;

            //Save available and independent variables
            lbAvailableVariables.Items.Clear();
            for (int i = 0; i < _projMgr.ModelingInfo.AvailableVariables.Count; i++)
                lbAvailableVariables.Items.Add(_projMgr.ModelingInfo.AvailableVariables[i]);

            lbIndVariables.Items.Clear();
            for (int i = 0; i < _projMgr.ModelingInfo.IndependentVariables.Count; i++)
                lbIndVariables.Items.Add(_projMgr.ModelingInfo.IndependentVariables[i]);

            lblAvailVars.Text = lbAvailableVariables.Items.Count.ToString();
            lblDepVars.Text = lbIndVariables.Items.Count.ToString();

            //Save the chromosomes
            if (_list != null)
                _list.Clear();
            else _list = new List<IIndividual>();

            _projMgr.ModelDataTable = CreateModelDataTable();

            MLRIndividual indiv = null;
            ModelingInfo modInfo = null;
            if (_projMgr.ModelingInfo.Chromosomes != null)
            {
                for (int i = 0; i < _projMgr.ModelingInfo.Chromosomes.Count; i++)
                {
                    modInfo = _projMgr.ModelingInfo;
                    int numGenes = _projMgr.ModelingInfo.Chromosomes[i].Count;
                    indiv = new MLRIndividual(numGenes, modInfo.MaxGeneValue, (FitnessCriteria)modInfo.FitnessCriteria, modInfo.MaxVIF, modInfo.DecisionThreshold, modInfo.MandatedThreshold);
                    indiv.Chromosome = _projMgr.ModelingInfo.Chromosomes[i];
                    indiv.Evaluate();
                    _list.Add((IIndividual)indiv);
                }

                UpdateFitnessListBox();
                if (_projMgr.ModelingInfo.SelectedModel > -1 && indiv.Parameters != null)
                    listBox1.SelectedIndex = _projMgr.ModelingInfo.SelectedModel;
            }

            _numObs = _projMgr.CorrelationDataTable.Rows.Count;
            lblNumObs.Text = "Number of Observations: " + _numObs.ToString();

            int maxVar = _numObs / 5;
            //int recVar = Math.Min(((_numObs / 10) + 1), availVar);
            int recVar = Math.Min(((_numObs / 10) + 1), (lbIndVariables.Items.Count));
            lblMaxAndRecommended.Text = "Recommended: " + recVar.ToString() + ", Max: " + maxVar.ToString();
            txtMaxVars.Text = recVar.ToString();
            
            this.Show();
            if (listBox1.Items.Count < 1 && ipyPLSControl.VirginState == true)
            {
                _projMgr._comms.sendMessage("Hide", this);
            }
        }

        private void SaveModelingInfo()
        {
           
            //ProjectSavedListener();
        }

        /// <summary>
        /// Fires when the app saves a project file
        /// </summary>
        /// <param name="projMgr"></param>
                

        private void ProjectSavedListener(object sender, PackEventArgs e)
        {
            ModelingInfo localModelingInfo = new ModelingInfo();

            //Not really much to save if no models were generated
            if (((_list == null) || (_list.Count < 1)) && ipyPLSControl.VirginState == true)
                return;

            //if (_projMgr.ModelingInfo == null)
            //    _projMgr.ModelingInfo = new ModelingInfo();

            //if the datasheet is dirty, don't want to save modeling data
            if (!_projMgr.DataSheetInfo.Clean) return;

            //Get the state of the IronPython-based modeling methods
            localModelingInfo.PlsProject = ipyPLSControl.PackProjectState();
            localModelingInfo.ActiveModelingTab = tabControl1.SelectedIndex;

            localModelingInfo.Model = _projMgr.Model;

            localModelingInfo.DependentVariable = _projMgr.ModelDataTable.Columns[1].ColumnName;

            //Save dependent variable transform
            if (rbValMET.Checked)
                localModelingInfo.DependentVariableTransform = Globals.DependentVariableTransforms.none;
            else if (rbLog10ValMET.Checked)
                localModelingInfo.DependentVariableTransform = Globals.DependentVariableTransforms.Log10;
            else if (rbLogeValMET.Checked)
                localModelingInfo.DependentVariableTransform = Globals.DependentVariableTransforms.Ln;
            else if (rbPwrValMET.Checked)
            {
                localModelingInfo.DependentVariableTransform = Globals.DependentVariableTransforms.Power;
                localModelingInfo.PowerTransformExponent = Convert.ToDouble(txtPwrValMET.Text);
            }
            //Save available and independent variables
             localModelingInfo.AvailableVariables = new List<ListItem>();
            for (int i = 0; i < lbAvailableVariables.Items.Count; i++)
                localModelingInfo.AvailableVariables.Add((ListItem)lbAvailableVariables.Items[i]);

            localModelingInfo.IndependentVariables = new List<ListItem>();
            for (int i = 0; i < lbIndVariables.Items.Count; i++)
                localModelingInfo.IndependentVariables.Add((ListItem)lbIndVariables.Items[i]);


            //Save the chromosomes
            if (listBox1.Items.Count > 0)
            {
                localModelingInfo.Chromosomes = new List<List<short>>();
                for (int i = 0; i < _list.Count; i++)
                    localModelingInfo.Chromosomes.Add(_list[i].Chromosome);


                MLRIndividual indiv = _list[0] as MLRIndividual;
                localModelingInfo.DecisionThreshold = indiv.DecisionThreshold;
                localModelingInfo.MandatedThreshold = indiv.MandatedThreshold;
                localModelingInfo.FitnessCriteria = (int)indiv.FitnessCriteria;
                localModelingInfo.MaxGeneValue = indiv.MaxGeneValue;
                localModelingInfo.MaxVIF = indiv.MaxVIF;
                localModelingInfo.NumGenes = indiv.NumGenes;
                localModelingInfo.Accuracy = indiv.Accuracy;
                localModelingInfo.AdjustedR2 = indiv.AdjustedR2;
                localModelingInfo.AIC = indiv.AIC;
                localModelingInfo.AICC = indiv.AICC;
                localModelingInfo.BIC = indiv.BIC;
                localModelingInfo.Press = indiv.Press;
                localModelingInfo.R2 = indiv.R2;
                localModelingInfo.RMSE = indiv.RMSE;
                localModelingInfo.Sensitivity = indiv.Sensitivity;
                localModelingInfo.Specificity = indiv.Specificity;
                localModelingInfo.VIF = indiv.VIF;

                localModelingInfo.SelectedModel = _selectedModelIndex;
            }
            else
            {
                localModelingInfo.Chromosomes = null;
            }

            if (_state == _mlrState.clean)
            {
                _projMgr.TabStates.TabState["Residuals"] = true;
                _projMgr.TabStates.TabState["Prediction"] = true;
            }
            else
            {
                _projMgr.TabStates.TabState["Residuals"] = false;
                _projMgr.TabStates.TabState["Prediction"] = false;
            }
            e.DictPacked.Add("frmModel", localModelingInfo);
        }

        //This function creates a datatable with only subset of selected IVs included in it.
        //Can't remeber the motivation for this.  Performance?
        private DataTable CreateModelDataTable()
        {
            DataTable dtCorr = _projMgr.CorrelationDataTable;
            DataView dvCorr = dtCorr.DefaultView;

            List<string> list = new List<string>();

            list.Add(dtCorr.Columns[0].ColumnName);
            list.Add(dtCorr.Columns[1].ColumnName);

            int numVars = lbIndVariables.Items.Count;
            for (int i = 0; i < numVars; i++)
                list.Add(lbIndVariables.Items[i].ToString());
            
            DataTable dtModel = dvCorr.ToTable("ModelData", false, list.ToArray());

            return dtModel;
        }

        private bool VerifyGAModelParams()
        {


            _numVars = lbIndVariables.Items.Count;
            //int numRecommendedVars = _numObs / 3;
            int numRecommendedVars = _numObs / 5;



            if (chkSeed.Checked)
            {
                if (Int32.TryParse(txtSeed.Text, out _seed) == false)
                {
                    string msg = "Seed must be a valid integer";
                    MessageBox.Show(msg);
                    return false;
                }

            }

            _totVar = _numVars;

            return true;

        }

        private bool VerifyManualModelParams()
        {

            //Dont think we need this check
            return true;


            _numVars = lbIndVariables.Items.Count;
            //int numRecommendedVars = _numObs / 3;
            int numRecommendedVars = _numObs / 5;

            if (_numVars > numRecommendedVars)
            {
                string msg;
                //msg = "Models require at least three times as many" + Environment.NewLine;
                msg = "Models require at least five times as many" + Environment.NewLine;
                msg += "observations as there are independent variables." + Environment.NewLine;
                msg += "Please remove some variables from the list.";
                MessageBox.Show(msg);
                return false;
            }

            _totVar = _numVars;
            return true;

        }

        private bool verifyGlobalModelingParams()
        {
            _numVars = lbIndVariables.Items.Count;
            //int numRecommendedVars = _numObs / 3;
            int numRecommendedVars = _numObs / 5;

            _userSpecifiedNumVars = Convert.ToInt32(txtMaxVars.Text);
            if (_userSpecifiedNumVars > numRecommendedVars)
            {
                string msg;
                //msg = "Models require at least three times as many" + Environment.NewLine;
                // msg += "observations as there are independent variables." + Environment.NewLine;
                msg = "The maximum number of variables in model for this dataset (" + _numObs.ToString() + " observations)" + Environment.NewLine;
                msg += " is " + numRecommendedVars.ToString() + ".";
                MessageBox.Show(msg);
                //txtMaxVars.Focus();                
                return false;

            }
            if (_userSpecifiedNumVars > _numVars)
            {
                string msg = "The maximum number of variables in model is limited to the number of independent variables in the list.";
                MessageBox.Show(msg);
                _userSpecifiedNumVars = _numVars;
                //txtMaxVars.Text = _numVars.ToString();
                //txtMaxVars.Focus();
                return false;

            }

            //if (_userSpecifiedNumVars < 2)
            if (_userSpecifiedNumVars < 1)
            {
                //MessageBox.Show("Maximum number of variables must be an integer value > 0.");
                MessageBox.Show("Maximum number of variables must be an integer value > 1.");
                return false;
            }
            //if (Double.TryParse(txtDecisionThreshold.Text, out _decisionThreshold) == false)
            //{
            //    string msg = @"Decision criterion must be a numeric value.";
            //    MessageBox.Show(msg);
            //    return false;
            //}

            //if (Double.TryParse(txtMandateThreshold.Text, out _mandateThreshold) == false)
            //{
            //    //string msg = @"Mandate threshold must be a numeric value.";
            //    string msg = @"Regulatory standard must be a numeric value.";
            //    MessageBox.Show(msg);
            //    return false;
            //}
            if (double.TryParse(tbDecThreshHoriz.Text, out _decisionThreshold) == false)
            {
                string msg = @"Decision criterion must be a numeric value.";
                MessageBox.Show(msg);
                return false;
            }
            else
            {
                mlrPredObs1.SetThresholds(tbDecThreshHoriz.Text, tbRegThreshVert.Text);
                //tbThresholdDec.Text = tbDecThreshHoriz.Text;
                if (rbLog10ValMET.Checked)
                {
                    _decisionThreshold = Math.Log10(_decisionThreshold);
                    mlrPredObs1.Transform = Globals.DependentVariableTransforms.Log10;
                    //rbLog10Value.Checked = true;
                    //_depVar = _depVarInfo.IsLog10.ToString();
                }
                else if (rbLogeValMET.Checked)
                {
                    _decisionThreshold = Math.Log(_decisionThreshold);
                    //rbLogeValue.Checked = true;
                    mlrPredObs1.Transform = Globals.DependentVariableTransforms.Ln;
                    //_depVar = _depVarInfo.IsLoge.ToString();
                }
                else if (rbPwrValMET.Checked)
                {
                    double power = Convert.ToDouble(txtPwrValMET.Text);
                    _decisionThreshold = Math.Log(_decisionThreshold, power);
                    mlrPredObs1.Transform =Globals.DependentVariableTransforms.Power;
                    //rbPwrValue.Checked = true;
                   // _depVar = _depVarInfo.IsPower.ToString();
                }
                else
                {
                    mlrPredObs1.Transform = Globals.DependentVariableTransforms.none;
                    //rbValue.Checked = true;
                   // _depVar = _depVarInfo.IsValue.ToString();
                }

                if (_decisionThreshold < 0 || _decisionThreshold.Equals(double.NaN)) 
                {
                    string msg = @"Decision criterion must be a numeric value greater than 0.";
                    MessageBox.Show(msg);
                    return false;
                } 
            }

            if (double.TryParse(tbRegThreshVert.Text, out _mandateThreshold) == false)
            {
                string msg = @"Regulatory standard must be a numeric value.";
                MessageBox.Show(msg);
                return false;
            }
            else  
            {
                //tbThresholdReg.Text = tbRegThreshVert.Text;
                if (rbLog10ValMET.Checked)
                {
                    _mandateThreshold = Math.Log10(_mandateThreshold);
                    //rbLog10Value.Checked = true;
                }
                else if (rbLogeValMET.Checked)
                {
                    _mandateThreshold = Math.Log(_mandateThreshold);
                    //rbLogeValue.Checked = true;
                }
                else if (rbPwrValMET.Checked)
                {
                    double power = Convert.ToDouble(txtPwrValMET.Text);
                    _mandateThreshold = Math.Log(_mandateThreshold, power);
                    //rbPwrValue.Checked = true;
                }
                else 
                { 
                    //rbValue.Checked = true; 
                }
                if (_mandateThreshold < 0 || _mandateThreshold.Equals(double.NaN))
                {
                    string msg = @"Regulatory standard must be a numeric value greater than 0.";
                    MessageBox.Show(msg);
                    return false;
                }

            }

            if (Int32.TryParse(txtMaxVIF.Text, out _maxVIF) == false)
            {
                string msg = "Maximum Variance Inflation Factor (VIF) must be a valid integer";
                MessageBox.Show(msg);
                return false;
            }

            return true;

        }

        private bool SetModelData()
        {
            //_proj.ModelDataTable = CreateModelDataTable();
            _numVars = lbIndVariables.Items.Count;
            //_numObs = _proj.ModelDataTable.Rows.Count;            
            //int numRecommendedVars = _numObs / 3;
            int numRecommendedVars = _numObs / 5;

            if (_numVars > numRecommendedVars)
            {
                string msg;
                //msg = "Models require at least three times as many" + Environment.NewLine;
                msg = "Models require at least five times as many" + Environment.NewLine;
                msg += "observations as there are independent variables." + Environment.NewLine;
                msg += "Please remove some variables from the list.";
                MessageBox.Show(msg);
                return false;
            }



            if (tabControlModelGeneration.SelectedTab.Name == "tabGA")
            {
                int userSpecifiedNumVars = Convert.ToInt32(txtMaxVars.Text);
                if (userSpecifiedNumVars > _numVars)
                {
                    string msg = "Can not have more independent variables than are in the list";
                    MessageBox.Show(msg);
                    //txtMaxVars.Text = _numVars.ToString();
                    return false;
                }
            }

            _totVar = _numVars;

            return true;

        }

        private void SetData(DataTable dt)
        {            
            _dtFull = dt;

            if (_dtFull == null)
                return;

            tabControl1.SelectedIndex = 0;

            List<string> fieldList = new List<string>();

            for (int i = 2; i < _dtFull.Columns.Count; i++)
            {
                if (_dtFull.Columns[i].ExtendedProperties.ContainsKey(VBTools.Globals.ENABLED))
                {
                    if (_dtFull.Columns[i].ExtendedProperties[VBTools.Globals.ENABLED].ToString() == "True")
                        fieldList.Add(_dtFull.Columns[i].ColumnName);
                }
                else
                    fieldList.Add(_dtFull.Columns[i].ColumnName);
            }


            _numObs = _dtFull.Rows.Count;
            lblNumObs.Text = "Number of Observations: " + _numObs.ToString();

            int maxVar = _numObs / 5;
            int recVar = (_numObs / 10) + 1;
            lblMaxAndRecommended.Text = "Recommended: " + recVar.ToString() + ", Max: " + maxVar.ToString();


            lbAvailableVariables.Items.Clear();
            lbIndVariables.Items.Clear();
            listBox1.Items.Clear();

            for (int i = 0; i < fieldList.Count; i++)
            {
                ListItem li = new ListItem(fieldList[i], i.ToString());
                lbAvailableVariables.Items.Add(li);
            }

            lblAvailVars.Text = "(" + lbAvailableVariables.Items.Count.ToString() + ")";
            lblDepVars.Text = "(" + lbIndVariables.Items.Count.ToString() + ")";

            string respVarTransform;
            Globals.DependentVariableTransforms transform = Globals.DependentVariableTransforms.none;
            if (dt.Columns[1].ExtendedProperties.ContainsKey(VBTools.Globals.DEPENDENTVARIBLEDEFINEDTRANSFORM))
            {
                respVarTransform = dt.Columns[1].ExtendedProperties[VBTools.Globals.DEPENDENTVARIBLEDEFINEDTRANSFORM].ToString();

                if (String.Compare(respVarTransform, Globals.DependentVariableTransforms.Log10.ToString(), 0) == 0)
                {
                    rbLog10ValMET.Checked = true;
                    transform = Globals.DependentVariableTransforms.Log10;
                }
                else if (String.Compare(respVarTransform, Globals.DependentVariableTransforms.Ln.ToString(), 0) == 0)
                {
                    rbLogeValMET.Checked = true;
                   transform = Globals.DependentVariableTransforms.Ln;
                }
                else if (respVarTransform.Contains(Globals.DependentVariableTransforms.Power.ToString()))
                {
                    rbPwrValMET.Checked = true;
                    double power = getTransformPower(respVarTransform);
                    txtPwrValMET.Text = power.ToString();
                    transform = Globals.DependentVariableTransforms.Power;
                    mlrPredObs1.PowerExponent = power;
                }
                else
                {
                    rbValMET.Checked = true;
                    transform = Globals.DependentVariableTransforms.none;
                }
            }
            else
            {
                rbValMET.Checked = true;
                transform = Globals.DependentVariableTransforms.none;
            }

            mlrPredObs1.Transform = transform;
            if (_projMgr.ModelingInfo == null) _projMgr.ModelingInfo = new ModelingInfo();
            _projMgr.ModelingInfo.DependentVariableTransform = transform;
            _projMgr.ModelingInfo.DecisionThreshold = Convert.ToDouble(tbDecThreshHoriz.Text);
            _projMgr.ModelingInfo.MandatedThreshold = Convert.ToDouble(tbRegThreshVert.Text);
            mlrPredObs1.SetThresholds(tbDecThreshHoriz.Text, tbRegThreshVert.Text);
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



        private long CalcNumModels(long maxIndVars, long totalVars)
        {
            long numModels = 0;

            for (int i = 1; i <= maxIndVars; i++)
                numModels += VBStatistics.Utility.Choose(totalVars, i);

            return numModels;
        }


        private void RunManual()
        {
            ResetGraphs();
            ResetOutputFields();

            if (chkAllCombinations.Checked)
            {
                SearchExhaustiveModelList();
                return;
            }

            DataTable dt = _projMgr.ModelDataTable;
            int numVars = lbIndVariables.Items.Count;

            FitnessCriteria fitnessCrit = GetFitnessCriteria();
            
            MLRIndividual indiv = null;
            indiv = new MLRIndividual(numVars, numVars, fitnessCrit, _maxVIF, _decisionThreshold, _mandateThreshold);

            for (int i = 0; i < numVars; i++)
                indiv.Chromosome[i] = (short)(i + 1);

            indiv.Evaluate();

            _list.Clear();
            _list.Add(indiv);

            btnRun.Text = "Run";
            changeControlStatus(true);

            UpdateFitnessListBox();

            return;

        }


        private FitnessCriteria GetFitnessCriteria()
        {
            FitnessCriteria fitnessCriteria = FitnessCriteria.Akaike;

            string criteria = cbCriteria.SelectedItem as string;

            if (String.Compare(criteria, "Akaike Information Criterion (AIC)", true) == 0)
                fitnessCriteria = FitnessCriteria.Akaike;
            else if (String.Compare(criteria, "Adjusted R Squared", true) == 0)
                fitnessCriteria = FitnessCriteria.AdjustedR2;
            else if (String.Compare(criteria, "Corrected Akaike Information Criterion (AICC)", true) == 0)
                fitnessCriteria = FitnessCriteria.AICC;
            else if (String.Compare(criteria, "Bayesian Information Criterion (BIC)", true) == 0)
                fitnessCriteria = FitnessCriteria.BIC;
            else if (String.Compare(criteria, "R Squared", true) == 0)
                fitnessCriteria = FitnessCriteria.R2;
            else if (String.Compare(criteria, "PRESS", true) == 0)
                fitnessCriteria = FitnessCriteria.Press;
            else if (String.Compare(criteria, "Specificity", true) == 0)
                fitnessCriteria = FitnessCriteria.Specificity;
            else if (String.Compare(criteria, "Sensitivity", true) == 0)
                fitnessCriteria = FitnessCriteria.Sensitivity;
            else if (String.Compare(criteria, "Accuracy", true) == 0)
                fitnessCriteria = FitnessCriteria.Accuracy;
            else if (String.Compare(criteria, "Root Mean Square Error (RMSE)", true) == 0)
                fitnessCriteria = FitnessCriteria.RMSE;
            else
                throw new Exception("Invalid Fitness Criteria");

            return fitnessCriteria;
        }

        private void RunGA()
        {
            _projMgr.ModelRunning = true;

            ResetGraphs();
            ResetOutputFields();

            double crossoverRate = Convert.ToDouble(txtCrossoverRate.Text);
            double mutationRate = Convert.ToDouble(txtMutRate.Text);
            int numGen = Convert.ToInt32(txtNumGen.Text);
            int popSize = Convert.ToInt32(txtPopSize.Text);

            FitnessCriteria fitnessCriteria = GetFitnessCriteria();

            _maxIndVars = Convert.ToInt32(txtMaxVars.Text);


            if (chkSeed.Checked)
            {
                int seed = Convert.ToInt32(txtSeed.Text);
                RandomNumbers.SetRandomSeed(seed);
            }
            else
                RandomNumbers.SetRandomSeed();

            _maxVIF = Convert.ToInt32(txtMaxVIF.Text);



            List<IIndividual> initPop = new List<IIndividual>(popSize);
            for (int i = 0; i < popSize; i++)
            {
                initPop.Add(new MLRIndividual(_maxIndVars, _totVar, fitnessCriteria, _maxVIF, _decisionThreshold, _mandateThreshold));
                //initPop.Add(new MLRIndividual(_maxIndVars, _totVar, fitnessCriteria, _maxVIF));
            }

            Population population = new Population(initPop);

            population.CrossoverMethod = new MLROnePointCrossover(crossoverRate);

            if (fitnessCriteria == FitnessCriteria.Akaike)
            {
                population.Comparer = new AscendSort();
                AICSelector aicSelector = new AICSelector();
                aicSelector.Comparer = population.Comparer;
                population.Selector = aicSelector;
            }
            else if (fitnessCriteria == FitnessCriteria.AICC)
            {
                population.Comparer = new AscendSort();
                AICCSelector aiccSelector = new AICCSelector();
                aiccSelector.Comparer = population.Comparer;
                population.Selector = aiccSelector;
            }
            else if (fitnessCriteria == FitnessCriteria.BIC)
            {
                population.Comparer = new AscendSort();
                BICSelector bicSelector = new BICSelector();
                bicSelector.Comparer = population.Comparer;
                population.Selector = bicSelector;
            }
            else if (fitnessCriteria == FitnessCriteria.Press)
            {
                population.Comparer = new AscendSort();
                PressSelector pressSelector = new PressSelector();
                pressSelector.Comparer = population.Comparer;
                population.Selector = pressSelector;
            }
            else if (fitnessCriteria == FitnessCriteria.AdjustedR2)
            {
                population.Comparer = new DescendSort();
                AdjR2Selector adjR2Selector = new AdjR2Selector();
                adjR2Selector.Comparer = population.Comparer;
                population.Selector = adjR2Selector;
            }
            else if (fitnessCriteria == FitnessCriteria.R2)
            {
                population.Comparer = new DescendSort();
                R2Selector r2Selector = new R2Selector();
                r2Selector.Comparer = population.Comparer;
                population.Selector = r2Selector;
            }
            else if (fitnessCriteria == FitnessCriteria.RMSE)
            {
                population.Comparer = new AscendSort();
                RMSESelector rmseSelector = new RMSESelector();
                rmseSelector.Comparer = population.Comparer;
                population.Selector = rmseSelector;
            }
            else if (fitnessCriteria == FitnessCriteria.Sensitivity)
            {
                population.Comparer = new DescendSort();
                SensitivitySelector sensitivitySelector = new SensitivitySelector();
                sensitivitySelector.Comparer = population.Comparer;
                population.Selector = sensitivitySelector;
            }
            else if (fitnessCriteria == FitnessCriteria.Specificity)
            {
                population.Comparer = new DescendSort();
                SpecificitySelector specificitySelector = new SpecificitySelector();
                specificitySelector.Comparer = population.Comparer;
                population.Selector = specificitySelector;
            }
            else if (fitnessCriteria == FitnessCriteria.Accuracy)
            {
                population.Comparer = new DescendSort();
                AccuracySelector accuracySelector = new AccuracySelector();
                accuracySelector.Comparer = population.Comparer;
                population.Selector = accuracySelector;
            }


            population.ChromosomeComparer = new CompareChromosomes();

            population.Initialize();

            population.Mutator = new MLRMutator(mutationRate, _totVar);


            //GALib.GAManager ga = new GALib.GAManager();
            _gaManager = new GALib.GAManager();
            _gaManager.Init(population);
            _gaManager.NumberOfGenerations = numGen;
            _gaManager.GAProgress += new GAManager.GAProgressHandler(GAUpdate);
            _gaManager.GAComplete += new GAManager.GACompleteHandler(GAComplete);

            GAManager ga = new GAManager();
            _runThread = new Thread(_gaManager.Run);
            _runThread.Start();
            //list = ga.Run(population);


        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            //Cursor curTmp = this.Cursor;
            //this.Cursor = Cursors.WaitCursor;
            //btnViewReport.Enabled = false;

            //btnViewRptChangeStatus(false);

            if (verifyGlobalModelingParams() == false)
                return;

            _state = _mlrState.dirty;
            _pmi = -1; 

            if (btnRun.Text == "Run")
            {

                if (tabControlModelGeneration.SelectedTab.Name == "tabGA")
                {
                    if (VerifyGAModelParams() == false)
                        return;


                    _projMgr.ModelDataTable = CreateModelDataTable();
                    _cancelRun = false;
                    btnRun.Text = "Cancel";
                    changeControlStatus(false);
                    RunGA();
                }
                else if (tabControlModelGeneration.SelectedTab.Name == "tabManual")
                {
                    if (VerifyManualModelParams() == false)
                        return;

                    _projMgr.ModelDataTable = CreateModelDataTable();
                    _cancelRun = false;
                    btnRun.Text = "Cancel";
                    changeControlStatus(false);
                    RunManual();
                }
                else if (tabControlModelGeneration.SelectedTab.Name == "tabStepwise")
                {
                    _projMgr.ModelDataTable = CreateModelDataTable();
                    //if (ForwardStepwise
                    //RunStepwise();
                }
                VBLogger.getLogger().logEvent("0", VBLogger.messageIntent.UserOnly, VBLogger.targetSStrip.ProgressBar);
                Application.DoEvents();

                //bool validParams = SetModelData();
                //if (validParams == false)
                //    return;



            }
            else if (btnRun.Text == "Cancel")
            {
                _cancelRun = true;
                if (_gaManager != null)
                    _gaManager.Cancel = true;

                if (_esManager != null)
                    _esManager.Cancel = true;

                btnRun.Text = "Run";
                changeControlStatus(true);
                Application.DoEvents();
                return;
            }


            //if (tabControlModelGeneration.SelectedTab.Name == "tabGA")
            //    RunGA();
            //else if (tabControlModelGeneration.SelectedTab.Name == "tabManual")
            //    RunManual();

            //VBLogger.getLogger().logEvent("0", VBLogger.messageIntent.UserOnly, VBLogger.targetSStrip.ProgressBar);
            //this.Cursor = curTmp;    
            //btnRun.Text = "Run";
            Application.DoEvents();

        }

        private void InitResultsGraph()
        {

            GraphPane myPane = zedGraphControl2.GraphPane;
            if (myPane.CurveList.Count > 0)
                myPane.CurveList.RemoveRange(0, myPane.CurveList.Count);

            myPane.Title.Text = "Results";
            myPane.XAxis.Title.Text = "X";
            myPane.YAxis.Title.Text = "Y";

            PointPairList list = new PointPairList();
            PointPairList list2 = new PointPairList();
            //Threshold line
            PointPairList list3 = new PointPairList();

            // Initially, a curve is added with no data points (list is empty)
            // Color is blue, and there will be no symbols

            LineItem curve = myPane.AddCurve("Y", list, Color.Blue, SymbolType.Square);
            curve.Line.IsVisible = true;
            // Hide the symbol outline
            curve.Symbol.Border.IsVisible = true;
            // Fill the symbol interior with color
            curve.Symbol.Fill = new Fill(Color.Firebrick);


            LineItem curve2 = myPane.AddCurve("YPred", list2, Color.Red, SymbolType.Triangle);
            curve2.Line.IsVisible = true;

            LineItem curve3 = myPane.AddCurve("Threshold", list3, Color.Black, SymbolType.None);
            curve3.Line.IsVisible = true;

            
            
            // Just manually control the X axis range so it scrolls continuously
            // instead of discrete step-sized jumps
            myPane.XAxis.Scale.Min = 0;
            myPane.XAxis.Scale.Max = 60;
            myPane.XAxis.Scale.MinorStep = 1;
            myPane.XAxis.Scale.MajorStep = 5;

            
            // Scale the axes
            zedGraphControl2.AxisChange();
            zedGraphControl2.Refresh();
        }

        //private void InitResultsGraph2()
        //{
        //    _XYPlotdata = new List<double[]>();

        //    GraphPane myPane = zedGraphControl3.GraphPane;
        //    if (myPane.CurveList.Count > 0)
        //        myPane.CurveList.RemoveRange(0, myPane.CurveList.Count - 1);

        //    myPane.Title.Text = "Results";
        //    myPane.XAxis.Title.Text = "Observed";
        //    myPane.YAxis.Title.Text = "Predicted";

        //    myPane.XAxis.MajorGrid.IsVisible = true;
        //    myPane.XAxis.MajorGrid.Color = Color.Gray;

        //    myPane.YAxis.MajorGrid.IsVisible = true;
        //    myPane.YAxis.MajorGrid.Color = Color.Gray;

        //    PointPairList list = new PointPairList();

        //    LineItem curve = myPane.AddCurve("Y", list, Color.Red, SymbolType.Circle);
        //    curve.Line.IsVisible = false;
        //    // Hide the symbol outline
        //    curve.Symbol.Border.IsVisible = true;
        //    // Fill the symbol interior with color
        //    curve.Symbol.Fill = new Fill(Color.Firebrick);

        //    //Vertical and horizontal threshold lines
        //    PointPairList list2 = new PointPairList();
        //    LineItem curve2 = myPane.AddCurve("Decision Threshold", list2, Color.Blue, SymbolType.None);
        //    curve2.Line.IsVisible = false;

        //    PointPairList list3 = new PointPairList();
        //    LineItem curve3 = myPane.AddCurve("Regulatory Threshold", list3, Color.Green, SymbolType.None);
        //    curve3.Line.IsVisible = false;

        //    // Scale the axes
        //    zedGraphControl3.AxisChange();
        //}

        private void ResetGraphs()
        {
            GraphPane myPane = zedGraphControl1.GraphPane;
            LineItem curve = zedGraphControl1.GraphPane.CurveList[0] as LineItem;
            IPointListEdit list = curve.Points as IPointListEdit;
            list.Clear();
        }

        private void ResetOutputFields()
        {
            listBox1.Items.Clear();
            listView1.Items.Clear();
            listView2.Items.Clear();
            //txtAdjustedR2.Text = "";
            //txtAIC.Text = "";
            //txtBIC.Text = "";
            //txtR2.Text = "";
            //txtAICC.Text = "";
            //txtPress.Text = "";

        }

        private void InitProgressGraph()
        {

            GraphPane myPane = zedGraphControl1.GraphPane;

            if (tabControlModelGeneration.SelectedTab.Name == "tabGA")
            {
                myPane.Title.Text = "Genetic Algorithm Dynamic Fitness Update";

                myPane.XAxis.Title.Text = "Percent of Generations Completed";
                myPane.YAxis.Title.Text = "Fitness";
            }
            else if (tabControlModelGeneration.SelectedTab.Name == "tabManual")
            {
                myPane.Title.Text = "Exhaustive Search of Independent Variable Space\n" +
                    "(Percent Complete)";

                myPane.XAxis.Title.Text = "Percent Completed";
                myPane.YAxis.Title.Text = "Fitness";
            }




            // Save 1200 points.  At 50 ms sample rate, this is one minute
            // The RollingPointPairList is an efficient storage class that always
            // keeps a rolling set of point data without needing to shift any data values
            RollingPointPairList list = new RollingPointPairList(1200);

            myPane.CurveList.Clear();
            // Initially, a curve is added with no data points (list is empty)
            // Color is blue, and there will be no symbols
            LineItem curve = myPane.AddCurve("Fitness", list, Color.Blue, SymbolType.None);

            // Sample at 50ms intervals
            //timer1.Interval = 50;
            //timer1.Enabled = true;
            //timer1.Start();

            // Just manually control the X axis range so it scrolls continuously
            // instead of discrete step-sized jumps
            myPane.XAxis.Scale.Min = 0;
            myPane.XAxis.Scale.Max = 30;
            myPane.XAxis.Scale.MinorStep = 1;
            myPane.XAxis.Scale.MajorStep = 5;

            // Scale the axes
            zedGraphControl1.AxisChange();
            zedGraphControl1.Refresh();
        }



        // Set the size and location of the ZedGraphControl
        //private void SetSize()
        //{
        //    // Control is always 10 pixels inset from the client rectangle of the form
        //    Rectangle formRect = this.ClientRectangle;
        //    formRect.Inflate(-10, -10);

        //    if (zedGraphControl1.Size != formRect.Size)
        //    {
        //        zedGraphControl1.Location = formRect.Location;
        //        zedGraphControl1.Size = formRect.Size;
        //    }

        //    if (zedGraphControl2.Size != formRect.Size)
        //    {
        //        zedGraphControl2.Location = formRect.Location;
        //        zedGraphControl2.Size = formRect.Size;
        //    }

        //    if (zedGraphControl3.Size != formRect.Size)
        //    {
        //        zedGraphControl3.Location = formRect.Location;
        //        zedGraphControl3.Size = formRect.Size;
        //    }
        //}


        private void RunComplete(List<IIndividual> updateList)
        {
            _list = new List<IIndividual>();

            //Only keep 10 models at most in list
            if (updateList != null)
            {
                int numModels = Math.Min(10, updateList.Count);
                for (int i = 0; i < numModels; i++)
                    _list.Add(updateList[i]);
            }

            listBox1.Invoke((MethodInvoker)delegate
            {
                listBox1.Items.Clear();
            });

            string item = "";
            int lastm;
            if (_list.Count > 10) lastm = 10;
            else lastm = _list.Count;
            for (int i = 0; i < lastm; i++)
            //for (int i = 0; i < list.Count; i++)
            {
                item = String.Format("{0:F4}", _list[i].Fitness);
                listBox1.Invoke((MethodInvoker)delegate
                {
                    listBox1.Items.Add(item);
                });
            }

            btnRun.Invoke((MethodInvoker)delegate
            {
                btnRun.Text = "Run";
            });

            _projMgr.ModelRunning = false;

            changeControlStatus(true);

            //btnViewRptChangeStatus(true);


        }
        private void GAComplete(GAManager gaManager)
        {
            RunComplete(gaManager.Results);
        }

        private void RunUpdate(double generation, double max)
        {

            Console.WriteLine("Generation: " + generation.ToString() + "   Fitness: " + max.ToString());
            // Make sure that the curvelist has at least one curve
            if (zedGraphControl1.GraphPane.CurveList.Count <= 0)
                return;

            // Get the first CurveItem in the graph
            LineItem curve = zedGraphControl1.GraphPane.CurveList[0] as LineItem;
            if (curve == null)
                return;

            // Get the PointPairList
            IPointListEdit list = curve.Points as IPointListEdit;
            // If this is null, it means the reference at curve.Points does not
            // support IPointListEdit, so we won't be able to modify it
            if (list == null)
                return;

            // Time is measured in seconds
            //double time = (Environment.TickCount - tickStart) / 1000.0;
            double time = generation;

            // 3 seconds per cycle
            list.Add(generation, max);

            // Keep the X scale at a rolling 30 second interval, with one
            // major step between the max X value and the end of the axis
            Scale xScale = zedGraphControl1.GraphPane.XAxis.Scale;
            if (time > xScale.Max - xScale.MajorStep)
            {
                xScale.Max = time + xScale.MajorStep;
                //xScale.Min = xScale.Max - 30.0;
            }

            // Make sure the Y axis is rescaled to accommodate actual data
            zedGraphControl1.AxisChange();
            //zedGraphControl1.Invalidate();
            //zedGraphControl1.Update();
            // Force a redraw
            //
            //listBox1.Refresh();
            zedGraphControl1.Invoke((MethodInvoker)delegate
            {
                zedGraphControl1.Refresh();
            });
            //zedGraphControl1.Refresh();
            //Refresh();

            //tabControl2.Refresh();
            //Application.DoEvents();
        }

        private void GAUpdate(double generation, double max)
        {
            RunUpdate(generation, max);
        }

        private void UpdateResults(List<double[]> data)
        {
            //DataTable dt = _projMgr.CorrelationDataTable;
            //DataTable dt = DataStore.GetDataTable();
            // Make sure that the curvelist has at least one curve
            if (zedGraphControl2.GraphPane.CurveList.Count <= 0)
                return;

            // Get the first CurveItem in the graph
            LineItem curve = zedGraphControl2.GraphPane.CurveList[0] as LineItem;
            if (curve == null)
                return;

            // Get the first CurveItem in the graph
            LineItem curve2 = zedGraphControl2.GraphPane.CurveList[1] as LineItem;
            if (curve2 == null)
                return;

            // Get the PointPairList
            IPointListEdit list = curve.Points as IPointListEdit;
            // If this is null, it means the reference at curve.Points does not
            // support IPointListEdit, so we won't be able to modify it
            if (list == null)
                return;

            // Get the PointPairList
            IPointListEdit list2 = curve2.Points as IPointListEdit;
            // If this is null, it means the reference at curve.Points does not
            // support IPointListEdit, so we won't be able to modify it
            if (list2 == null)
                return;

            list.Clear();
            list2.Clear();

            for (int i = 0; i < data.Count; i++)
            {
                list.Add(i + 1, data[i][0]);
                list2.Add(i + 1, data[i][1]);
            }

            LineItem curve3 = zedGraphControl2.GraphPane.CurveList[2] as LineItem;
            if (curve3 != null)
            {
                //if (chkThreshold.Checked)
                // {
                // Get the PointPairList
                IPointListEdit list3 = curve3.Points as IPointListEdit;
                list3.Clear();
                list3.Add(0, _decisionThreshold);
                list3.Add(data.Count, _decisionThreshold);

                curve3.Line.IsVisible = true;
                //}
                // else
                //{
                //    curve3.Line.IsVisible = false; 
                //}
            }


            // Keep the X scale at a rolling 30 second interval, with one
            // major step between the max X value and the end of the axis
            Scale xScale = zedGraphControl2.GraphPane.XAxis.Scale;
            if (data[data.Count - 1][0] > xScale.Max - xScale.MajorStep)
            {
                //xScale.Max = data[data.Count - 1][0] + xScale.MajorStep;
                //xScale.Min = xScale.Max - 30.0;
            }

            //mikec - get rid of the line at y=0...?
            GraphPane zgc2pane = zedGraphControl2.GraphPane;
            //...best I can do for now
            zgc2pane.XAxis.Cross = 0;


            // Make sure the Y axis is rescaled to accommodate actual data
            zedGraphControl2.AxisChange();
            // Force a redraw
            zedGraphControl2.Invalidate();
            listBox1.Refresh();
            zedGraphControl2.Refresh();
            Application.DoEvents();
        }


        private void frmModel_Load(object sender, EventArgs e)
        {
            _list = new List<IIndividual>();

            _projMgr.CorrelationDataTableUpdate += new VBProjectManager.CorrelationDataTableUpdateHandler(CorrelateDataTableUpdateListener);
            
            SetData(_projMgr.CorrelationDataTable);
            InitProgressGraph();
            InitResultsGraph();
            tabControl2.TabPages[0].Hide();
            lblDepVars.Text = "(" + lbIndVariables.Items.Count.ToString() + ")";
        }


        //Update info as needed if the CorrelationDataTable changes
        private void CorrelateDataTableUpdateListener(VBProjectManager projMgr, CorrelationDataTableStatus e)
        {
            SetData(projMgr.CorrelationDataTable);
            _numObs = projMgr.CorrelationDataTable.Rows.Count;
            lblNumObs.Text = "Number of Observations: " + _numObs.ToString();

            int maxVar = _numObs / 5;
            int recVar = (_numObs / 10) + 1;
            lblMaxAndRecommended.Text = "Recommended: " + recVar.ToString() + ", Max: " + maxVar.ToString();

            //*********added 9/14 mog to init form when datasheet datatable dirty
            InitProgressGraph();
            InitResultsGraph();
            tabControl2.TabPages[0].Hide();
            lblDepVars.Text = "(" + lbIndVariables.Items.Count.ToString() + ")";
            listView1.Items.Clear();
            listView2.Items.Clear();
        }


        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _smi = listBox1.SelectedIndex;
            if (_smi != _pmi)
            {
                if (_pmi == -1)
                {
                    _state = _mlrState.clean;
                }
                else
                {
                    _state = _mlrState.dirty;
                }
                _pmi = _smi;
            }
            else
            {
                _state = _mlrState.clean;
            }

            listView1.Items.Clear();
            listView2.Items.Clear();

            int idx = listBox1.SelectedIndex;
            MLRIndividual ind = (MLRIndividual)_list[idx];


            _selectedModelIndex = idx;

            //tell the main form to enable the model save/saveas menu selection buttons
            OnModelSelected(null);                        

            string[] item = null;
            ListViewItem lvi = null;

            int numColumns = ind.Parameters.Columns.Count;
            for (int i = 0; i < ind.Parameters.Rows.Count; i++)
            {
                item = new string[numColumns];
                item[0] = ind.Parameters.Rows[i][0].ToString();
                item[1] = formatNumber((double)ind.Parameters.Rows[i][1]);
                item[2] = formatNumber((double)ind.Parameters.Rows[i][2]);
                item[3] = formatNumber((double)ind.Parameters.Rows[i][3]);
                item[4] = formatNumber((double)ind.Parameters.Rows[i][4]);
                item[5] = formatNumber((double)ind.Parameters.Rows[i][5]);
                lvi = new ListViewItem(item);
                listView1.Items.Add(lvi);
            }

            DataTable dt = _projMgr.ModelDataTable;

            int numRecords = dt.Rows.Count;
            if (numRecords != ind.PredictedValues.Length)
                return;

            List<double[]> data = new List<double[]>();
            double[] record = null;
            for (int i = 0; i < numRecords; i++)
            {
                record = new double[2];
                record[0] = Convert.ToDouble(dt.Rows[i][1].ToString());
                record[1] = ind.PredictedValues[i];
                data.Add(record);
            }

            //for plotting xyplot when transform radio buttons changed
            _XYPlotdata = data;

            item = new string[2];
            item[0] = "R Squared";
            item[1] = String.Format("{0:F4}", ind.R2);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "Adjusted R Squared";
            item[1] = String.Format("{0:F4}", ind.AdjustedR2);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "Akaike Information Criterion";
            item[1] = String.Format("{0:F4}", ind.AIC);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "Corrected AIC";
            item[1] = String.Format("{0:F4}", ind.AICC);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "Bayesian Info Criterion";
            item[1] = String.Format("{0:F4}", ind.BIC);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "PRESS";
            item[1] = String.Format("{0:F4}", ind.Press);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "RMSE";
            item[1] = String.Format("{0:F4}", ind.RMSE);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            if ((ind.DecisionThreshold != Double.NaN) && (ind.MandatedThreshold != Double.NaN))
            {
                item = new string[2];
                item[0] = "";
                item[1] = "";
                lvi = new ListViewItem(item);
                listView2.Items.Add(lvi);

                item = new string[2];
                item[0] = "Decision Criterion";
                item[1] = string.Format("{0:F4}", ind.DecisionThreshold);
                lvi = new ListViewItem(item);
                listView2.Items.Add(lvi);

                item = new string[2];
                item[0] = "Regulatory Standard";
                item[1] = string.Format("{0:F4}", ind.MandatedThreshold);
                lvi = new ListViewItem(item);
                listView2.Items.Add(lvi);

                ModelErrorCounts mec = new ModelErrorCounts();
                mec.getCounts(ind.DecisionThreshold, ind.MandatedThreshold, ind.PredictedValues, ind.ObservedValues);
                int fp = mec.FPCount;
                int fn = mec.FNCount;

                item = new string[2];
                item[0] = "False Positives";
                item[1] = string.Format("{0:n}", fp);
                lvi = new ListViewItem(item);
                listView2.Items.Add(lvi);

                item = new string[2];
                item[0] = "Specificity";
                item[1] = String.Format("{0:F4}", ind.Specificity);
                lvi = new ListViewItem(item);
                listView2.Items.Add(lvi);

                item = new string[2];
                item[0] = "False Negatives";
                item[1] = string.Format("{0:n}", fn);
                lvi = new ListViewItem(item);
                listView2.Items.Add(lvi);

                item = new string[2];
                item[0] = "Sensitivity";
                item[1] = String.Format("{0:F4}", ind.Sensitivity);
                lvi = new ListViewItem(item);
                listView2.Items.Add(lvi);

                //item = new string[2];
                //item[0] = "Specificity";
                //item[1] = String.Format("{0:F4}", ind.Specificity);
                //lvi = new ListViewItem(item);
                //listView2.Items.Add(lvi);

                item = new string[2];
                item[0] = "Accuracy";
                item[1] = String.Format("{0:F4}", ind.Accuracy);
                lvi = new ListViewItem(item);
                listView2.Items.Add(lvi);

                //for reporting/plotting
                ThresholdChecked = true;
            }
            else
            {
                //for reporting/plotting
                ThresholdChecked = false;
            }


            item = new string[2];
            item[0] = "";
            item[1] = "";
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            item = new string[2];
            item[0] = "Number of Observations";
            item[1] = string.Format("{0}", numRecords);
            lvi = new ListViewItem(item);
            listView2.Items.Add(lvi);

            UpdateResults(data);
            mlrPredObs1.UpdateResults(data);
            //UpdateResults2(data);

            _projMgr.Model = ind.Model;
            _projMgr.ModelIndependentVariables = new List<string>(_projMgr.GetIndependentVariableList(ind.Chromosome));

            SaveModelingInfo();

            //if user selects a model in list....
            _projMgr._comms.sendMessage("Show the MLR Prediction form.", this);
            //also want to show the residuals tab but don't need another call
            //_proj._comms.sendMessage("Show the Residuals form.", this);
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

        private void btnAddInputVariable_Click(object sender, EventArgs e)
        {
            List<ListItem> items = new List<ListItem>();

            int selectedIndices = lbAvailableVariables.SelectedIndices.Count;
            for (int i = 0; i < selectedIndices; i++)
            {
                ListItem li = (ListItem)lbAvailableVariables.Items[lbAvailableVariables.SelectedIndices[i]];
                items.Add(li);
            }

            foreach (ListItem li in items)
            {
                lbAvailableVariables.Items.Remove(li);
                lbIndVariables.Items.Add(li);
            }


            SetCombinations();

            lblAvailVars.Text = "(" + lbAvailableVariables.Items.Count.ToString() + ")";
            lblDepVars.Text = "(" + lbIndVariables.Items.Count.ToString() + ")";

            _state = _mlrState.dirty;
            initControls();

            if (ResetIPyProject != null)
            {
                EventArgs args = new EventArgs();
                ResetIPyProject(this, e);
            }
        }

 
        private void btnRemoveInputVariable_Click(object sender, EventArgs e)
        {
            List<ListItem> items = new List<ListItem>();

            for (int i = 0; i < lbIndVariables.SelectedIndices.Count; i++)
            {
                ListItem li = (ListItem)lbIndVariables.Items[lbIndVariables.SelectedIndices[i]];
                items.Add(li);
            }

            foreach (ListItem li in items)
            {
                lbIndVariables.Items.Remove(li);

                bool foundIdx = false;
                int j = 0;
                for (j = 0; j < lbAvailableVariables.Items.Count; j++)
                {
                    ListItem li2 = (ListItem)lbAvailableVariables.Items[j];
                    if (Convert.ToInt32(li2.ValueItem) > Convert.ToInt32(li.ValueItem))
                    {
                        lbAvailableVariables.Items.Insert(j, li);
                        foundIdx = true;
                        break;
                    }
                }
                if (foundIdx == false)
                    lbAvailableVariables.Items.Insert(j, li);

            }


            SetCombinations();

            lblAvailVars.Text = "(" + lbAvailableVariables.Items.Count.ToString() + ")";
            lblDepVars.Text = "(" + lbIndVariables.Items.Count.ToString() + ")";

            _state = _mlrState.dirty;
            initControls();

            if (ResetIPyProject != null)
            {
                EventArgs args = new EventArgs();
                ResetIPyProject(this, e);
            }
        }

        private void initControls()
        {
            listBox1.Items.Clear();
            listView1.Items.Clear();
            listView2.Items.Clear();
            listView3.Items.Clear();
            listView4.Items.Clear();
            InitProgressGraph();
            InitResultsGraph();
            zgcROC.GraphPane.CurveList.Clear();
            zgcROC.Refresh();
            //huh? init this bad boy somehow...
            mlrPredObs1.UpdateResults(null);
        }


        private void ESComplete(ExhaustiveSearchManager esManager)
        {
            RunComplete(esManager.Results);
        }

        private void ESUpdate(double progress, double max)
        {
            RunUpdate(progress, max);
        }

        private void SearchExhaustiveModelList()
        {
            _projMgr.ModelRunning = true;

            FitnessCriteria fitnessCrit = GetFitnessCriteria();
            int numVars = lbIndVariables.Items.Count;

            _esManager = new ExhaustiveSearchManager(numVars, fitnessCrit, _maxVIF, _decisionThreshold, _mandateThreshold, _userSpecifiedNumVars);

            _esManager.ESProgress += new ExhaustiveSearchManager.ESProgressHandler(ESUpdate);
            _esManager.ESComplete += new ExhaustiveSearchManager.ESCompleteHandler(ESComplete);

            _runThread = new Thread(_esManager.Run);
            _runThread.Start();

            return;

        }

        private void UpdateFitnessListBox()
        {
            listBox1.Items.Clear();

            for (int i = 0; i < _list.Count; i++)
            {
                string item = "";
                item = String.Format("{0:F4}", _list[i].Fitness);
                listBox1.Items.Add(item);
            }
        }


        /// <summary>
        /// Calculate the total number of combinations availabl for the selected number of independent variables
        /// </summary>
        private void SetCombinations()
        {

            int numVars = lbIndVariables.Items.Count;

            List<short> combList = new List<short>();
            short tmp = 0; ;
            for (int i = 0; i < numVars; i++)
            {
                ListItem li = (ListItem)lbIndVariables.Items[i];
                tmp = 1;
                tmp += Convert.ToInt16(li.ValueItem);
                combList.Add(tmp);
            }

            //long totalComb = 0;
            decimal totalComb = 0;
            Combinations<short> combinations = null;
            for (int i = 0; i < numVars; i++)
            {
                combinations = new Combinations<short>(combList.ToArray(), i, GenerateOption.WithoutRepetition);
                totalComb += combinations.Count;

            }

            string nModels = string.Empty;
            if (totalComb > 9999999999)
            {
                nModels = string.Format("{0:000e000}", totalComb);
            }
            else
            {

                if (totalComb < 0)
                {
                    //we've flipped the storage capacity (not of totalComb [decimal type good to 7.8(10)**28], something else)
                    //combinations.Count is only a long - probably this (max 9.2(10)**18)
                    nModels = " more than 9.2e018 ";
                }
                else
                {
                    //lblnModels.Text = string.Format("{0:#,###,###,###}", totalComb);
                    nModels = string.Format("{0:#,###,###,###}", totalComb);
                }
            }

            VBLogger.getLogger().logEvent("Total number of possible models: " + nModels,
                VBLogger.messageIntent.UserAndLogFile, VBLogger.targetSStrip.StatusStrip3);
        }


        private void chkAllCombinations_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAllCombinations.Checked)
            {
                //lblCombinations.Visible = true;
            }
            else
            {
                // lblCombinations.Visible = false;
            }
            SetCombinations();
        }
        

        private void frmModel_Activated(object sender, EventArgs e)
        {
            if (_projMgr.CorrelationDataTable != null)
                lbDepVarName.Text = _projMgr.CorrelationDataTable.Columns[1].ColumnName.ToString();
        }


        private void tabControlModelGeneration_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitProgressGraph();
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

        private void changeControlStatus(bool enable)
        {
            lbAvailableVariables.Invoke((MethodInvoker)delegate
            {
                lbAvailableVariables.Enabled = enable;
            });

            lbAvailableVariables.Invoke((MethodInvoker)delegate
            {
                lbAvailableVariables.Enabled = enable;
            });

            lbIndVariables.Invoke((MethodInvoker)delegate
            {
                lbIndVariables.Enabled = enable;
            });

            btnAddInputVariable.Invoke((MethodInvoker)delegate
            {
                btnAddInputVariable.Enabled = enable;
            });

            btnRemoveInputVariable.Invoke((MethodInvoker)delegate
            {
                btnRemoveInputVariable.Enabled = enable;
            });

            groupBox4.Invoke((MethodInvoker)delegate
            {
                groupBox4.Enabled = enable;
            });

            tabControlModelGeneration.Invoke((MethodInvoker)delegate
            {
                tabControlModelGeneration.Enabled = enable;
            });

            cbCriteria.Invoke((MethodInvoker)delegate
            {
                cbCriteria.Enabled = enable;
            });

            chkSeed.Invoke((MethodInvoker)delegate
            {
                chkSeed.Enabled = enable;
            });

            txtSeed.Invoke((MethodInvoker)delegate
            {
                if (chkSeed.Checked)
                    txtSeed.Enabled = enable;
            });
            
            txtMaxVIF.Invoke((MethodInvoker)delegate
            {
                txtMaxVIF.Enabled = enable;
            });

            btnViewReport.Invoke((MethodInvoker)delegate
            {
                btnViewReport.Enabled = enable;
            });


        }

        private void frmModel_Validating(object sender, CancelEventArgs e)
        {
            if (_projMgr.ModelRunning)
                e.Cancel = true;
        }

        private void txtMaxVars_Validating(object sender, CancelEventArgs e)
        {
            if (VerifyGAModelParams() == false)
                e.Cancel = true;

        }

        private void chkSeed_CheckedChanged(object sender, EventArgs e)
        {
            txtSeed.Enabled = chkSeed.Checked;
        }

        private void btnViewReport_Click(object sender, EventArgs e)
        {
            frmModelingReport frmRpt = new frmModelingReport();

            frmRpt.addHeader();

            FitnessCriteria fc = GetFitnessCriteria();
            string sfc = "Model Evaluation Criterion: " + fc.ToString() + "\n";
            frmRpt.addModelEvalCriterion(sfc);

            string[] models = new string[listBox1.Items.Count];
            listBox1.Items.CopyTo(models, 0);

            frmRpt.addList(models, _list);

            frmRpt.Show();
        }

        private void btnViewRptChangeStatus(bool status)
        {
            btnViewReport.Invoke((MethodInvoker)delegate
            {
                btnViewReport.Enabled = status;
            });
        }

        private void btnClearList_Click(object sender, EventArgs e)
        {
            for (int li = 0; li < lbIndVariables.Items.Count; li++)
                lbIndVariables.SetSelected(li, true);

            List<ListItem> items = new List<ListItem>();

            for (int i = 0; i < lbIndVariables.SelectedIndices.Count; i++)
            {
                ListItem li = (ListItem)lbIndVariables.Items[lbIndVariables.SelectedIndices[i]];
                items.Add(li);
            }

            foreach (ListItem li in items)
            {
                lbIndVariables.Items.Remove(li);

                bool foundIdx = false;
                int j = 0;
                for (j = 0; j < lbAvailableVariables.Items.Count; j++)
                {
                    ListItem li2 = (ListItem)lbAvailableVariables.Items[j];
                    if (Convert.ToInt32(li2.ValueItem) > Convert.ToInt32(li.ValueItem))
                    {
                        lbAvailableVariables.Items.Insert(j, li);
                        foundIdx = true;
                        break;
                    }
                }
                if (foundIdx == false)
                    lbAvailableVariables.Items.Insert(j, li);
            }
            
            SetCombinations();

            lblAvailVars.Text = "(" + lbAvailableVariables.Items.Count.ToString() + ")";
            lblDepVars.Text = "(" + lbIndVariables.Items.Count.ToString() + ")";
        }

        private void btnAddtoList_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count < 1) return;

            for (int lvi = 1; lvi < listView1.Items.Count; lvi++)
            {
                string iv = listView1.Items[lvi].SubItems[0].Text;

                foreach (ListItem li in lbAvailableVariables.Items)
                {
                    if (li.ToString() == iv)
                    //found it, select it, die
                    {
                        int ndx = lbAvailableVariables.Items.IndexOf(li);
                        lbAvailableVariables.SetSelected(ndx, true);
                        break;
                    }
                }
            }
            //if (lbAvailableVariables.SelectedItems.Count > 0)
                //btnAddInputVariable_Click(this, new EventArgs());

            List<ListItem> items = new List<ListItem>();

            int selectedIndices = lbAvailableVariables.SelectedIndices.Count;
            for (int i = 0; i < selectedIndices; i++)
            {
                ListItem li = (ListItem)lbAvailableVariables.Items[lbAvailableVariables.SelectedIndices[i]];
                items.Add(li);
            }

            foreach (ListItem li in items)
            {
                lbAvailableVariables.Items.Remove(li);
                lbIndVariables.Items.Add(li);
            }


            SetCombinations();

            lblAvailVars.Text = "(" + lbAvailableVariables.Items.Count.ToString() + ")";
            lblDepVars.Text = "(" + lbIndVariables.Items.Count.ToString() + ")";
        }


        private void lblDepVars_TextChanged(object sender, EventArgs e)
        {
            int availVar = lbIndVariables.Items.Count;
            int maxVar = Math.Min((_numObs / 5), availVar);
            int recVar = Math.Min(((_numObs / 10) + 1), availVar);

            lblMaxAndRecommended.Text = "Available: " + availVar.ToString() +
                ", Recommended: " + recVar.ToString() +
                ", Max: " + maxVar.ToString();

            txtMaxVars.Text = recVar.ToString();
        }


        private void btnCrossValidation_Click(object sender, EventArgs e)
        {
            frmCrossValidation crossValForm = new frmCrossValidation(_list, _numObs);
            crossValForm.ShowDialog(this);

        }

        private void frmModel_HelpRequested(object sender, HelpEventArgs hlpevent)
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

        #region roc prototype;

        /// <summary>
        /// given a model and decision threshold calculate (and store) information for a roc curve
        /// </summary>
        /// <param name="model">mlr model</param>
        /// <param name="decisionThreshold">threshold value</param>
        /// <param name="rocPars">roc curve information </param>
        /// <returns>point pair for plotting</returns>
        private PointPair ROCpoint(MLRIndividual model, double decisionThreshold, out ROCParameters rocPars)
        {
            //for a given threshold and model, categorize predictions relative to observations
            //and calculate the sensitivity and specificity evaluation criteria, return theses
            //values as a zedgraph point.

            PointPair pp = new PointPair();
            double[] measurement = model.ObservedValues;
            double[] est = model.PredictedValues;
            int truePos = 0;
            int trueNeg = 0;
            int falsePos = 0;
            int falseNeg = 0;
            double pred = double.NaN;
            double obs = double.NaN;
            double specificity = double.NaN;
            double sensitivity = double.NaN;

            for (int i = 0; i < est.Length; i++)
            {
                pred = est[i];
                obs = measurement[i];
                if ((pred > decisionThreshold) && (obs > _mandateThreshold))
                    truePos++;
                else if ((pred > decisionThreshold) && (obs < _mandateThreshold))
                    falsePos++;
                else if ((pred < decisionThreshold) && (obs > _mandateThreshold))
                    falseNeg++;
                else if ((pred < decisionThreshold) && (obs < _mandateThreshold))
                    trueNeg++;
            }

            sensitivity = (double)truePos / (double)(truePos + falseNeg);
            specificity = (double)trueNeg / (double)(trueNeg + falsePos);


            if (!sensitivity.Equals(double.NaN) && !specificity.Equals(double.NaN))
            {
                pp.X = (1.0d - specificity);
                pp.Y = sensitivity;
            }

            ROCParameters rocpars = new ROCParameters(decisionThreshold, sensitivity, specificity, falsePos, falseNeg);
            rocPars = rocpars;

            return pp;

        }

        /// <summary>
        /// given a mlr model, vary the threshold by increments to find model sensitivity and specificity
        /// for each increment - these become roc plotting points for the model.  also, save the roc trace 
        /// data for passing to the caller for table display.  plotting points need to be sorted and aggregated
        /// (weedppl(ppl)) for calculating auc (area-under-curve via integration)
        /// </summary>
        /// <param name="model">given mlr model</param>
        /// <param name="rocTableVals"></param>
        /// <returns>null if number of pts lt 10, otherwise a pointpair list fro plotting</returns>
        private PointPairList ROCpoints(MLRIndividual model, out List<object> rocTableVals)
        {
            const int interations = 50;

            //vary the decision threshold by increments
            //calculate the ROC point for the decision threshold increment
            //accumulate points for all increments and return pointpairlist

            PointPairList ppl = new PointPairList();
            PointPair pp = new PointPair();
            ROCParameters rocParameters = null;
            List<object> rocTableVal = new List<object>();

            double maxPred = model.PredictedValues.Max();
            double minPred = model.PredictedValues.Min();
            double inc = (maxPred - minPred) / (double)interations;
            double threshold = minPred;

            while (threshold < maxPred)
            {
                threshold += inc;
                pp = ROCpoint(model, threshold, out rocParameters);
                if (!pp.IsInvalid)
                {
                    ppl.Add(pp);
                    rocTableVal.Add(rocParameters.ROCPars);
                }


            }

            rocTableVals = rocTableVal;

            //how many points is the minimum???
            if (ppl.Count > 10)
            {
                //sort for integral calc
                ppl.Sort();
                //get rid of multiple X datapoints
                ppl = weedppl(ppl);
                return ppl;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// method aggregates a pointpair list Y values by X values, in effect removing duplicate X values
        /// this is required for the auc integration calculation (which crashes when x(i) == x(i+1), i.e. when
        /// delta x == 0)
        /// </summary>
        /// <param name="ppl"></param>
        /// <returns></returns>
        private PointPairList weedppl(PointPairList ppl)
        {
            //throw new NotImplementedException();

            //aggregate points by X values; Y becomes avarage of Ys for same Xs
            //prove yer smarter than me and implement this with linq...  ppl is already sorted by X ascending.
            PointPairList ppl2 = new PointPairList();

            //double[] x = new double[ppl.Count];
            //double[] y = new double[ppl.Count];

            double x; double y;
            double Xbase = ppl[0].X;
            double Ybase = ppl[0].Y;
            double Ytot = 0.0d;
            int ctr = 0;
            for (int i = 1; i < ppl.Count; i++)
            {
                x = ppl[i].X;
                y = ppl[i].Y;
                if (x == Xbase)
                {
                    Ytot = Ytot + y;
                    ctr++;
                    continue;
                }
                else
                {
                    //Xbase = x;
                    if (ctr != 0)
                    {
                        y = Ytot / (double)ctr;
                        ctr = 0;
                        Ytot = 0;

                    }
                    ppl2.Add(Xbase, y);
                    Xbase = x;
                }

            }

            //var xes = x.Distinct<double>();

            return ppl2;
        }

        /// <summary>
        /// generate roc plot data for all models in the best-fit models list and makes the plots.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl2.SelectedIndex == 3) //do nothing if not roc curves tab selected
            {
                if (listBox1.Items.Count < 1) return;

                if (_list.Count > 0) //if we have models, make plots
                {
                    double lowerbound = 0.0d;
                    double upperbound = 1.0d;
                    listView3.Items.Clear();
                    GraphPane pane = zgcROC.GraphPane;
                    if (pane.CurveList.Count > 0) pane.CurveList.Clear();
                    pane.Title.Text = "Receiver Operating Characteristic Curves \n for Best-Fit Models";
                    pane.XAxis.Title.Text = "1 - Specificity";
                    pane.YAxis.Title.Text = "Sensitivity";

                    object[] colors = new object[] {Color.DarkSeaGreen, Color.Green, Color.Indigo, 
                        Color.ForestGreen, Color.CadetBlue,
                        Color.DarkBlue, Color.DarkViolet, Color.DarkCyan, 
                        Color.Blue, Color.DarkGray};

                    object[] symbols = new object[] {SymbolType.Circle, SymbolType.Diamond, SymbolType.Plus,
                        SymbolType.Square, SymbolType.Star, SymbolType.Triangle,
                        SymbolType.TriangleDown, SymbolType.XCross, SymbolType.VDash, SymbolType.HDash};


                    string[] item = new string[2];

                    PointPairList ppl = new PointPairList();

                    //generate the data, make the plots, and fill the listbox with model/areaundercurve info
                    int maxmodelptr = -1;
                    double maxmodelauc = 0;

                    //if there are more models in list than showing... use 10 or use the number in the list
                    int maxNmodels = _list.Count <= 10 ? _list.Count : 10;

                    //initialize a structure for storing roc curve information for all curves...
                    _tableVals = new Dictionary<string,List<object>>();
                    //...and one for individual model
                    List<object> modelRocVals = null;

                    //for (int li = 0; li < list.Count; li++)
                    for (int li = 0; li < maxNmodels; li++)
                    {
                        //PointPairList ppl = ROCpoints( (MLRIndividual)list[li]);
                        ppl = ROCpoints((MLRIndividual)_list[li], out modelRocVals);
                        string key = _list[li].Fitness.ToString("##.####");
                        if (!_tableVals.ContainsKey(key))
                        {
                            _tableVals.Add(key, modelRocVals);
                        }
                        if (ppl != null && ppl.Count > 0)
                        {
                            //BasicArrayPointList ppl = ROCpoints((MLRIndividual)list[li]);
                            LineItem curve = pane.AddCurve(_list[li].Fitness.ToString("##.####"), ppl,
                                (Color)colors[li], (SymbolType)symbols[li]);

                            double[] X = new double[ppl.Count];
                            double[] Y = new double[ppl.Count];
                            for (int i = 0; i < ppl.Count; i++) //there must be a smarter way to do this
                            {
                                X[i] = ppl[i].X;
                                Y[i] = ppl[i].Y;
                            }

                            //performing integral evaluation with EM Piecewise constant curve instance
                            //(another possibility is the Piecewise linear curve class... splines and 
                            //other curve definitions seem like overkill and delegate realfunctions seem
                            //like the wrong approach entirely - requires curve fitting/definition and all
                            //we have are data points for VERY similar curves)
                            Calculus calc = new Calculus(X, Y, lowerbound, upperbound);
                            double auc = calc.PieceWiseConstantCurveIntegrate();

                            //maybe piecewise linear curve is better?
                            calc = new Calculus(X, Y, lowerbound, upperbound);
                            double auc2 = calc.PieceWiseLinearCurveIntegrate();
                            double idiff = auc - auc2;

                            //comment next line to use piecewiseconstant integral calc
                            auc = auc2;


                            item[0] = _list[li].Fitness.ToString("##.####");
                            //item[1] = string.Format("{0:f6}", auc.ToString());
                            item[1] = auc.ToString("#.######");
                            ListViewItem lvi = new ListViewItem(item);
                            listView3.Items.Add(lvi);

                            if (auc > maxmodelauc)
                            {
                                maxmodelauc = auc;
                                maxmodelptr = li;
                            }
                        }
                    }

                    //add the no information trace
                    PointPair orig = new PointPair(0, 0);
                    PointPair extent = new PointPair(1, 1);
                    PointPairList ppl2 = new PointPairList();
                    ppl2.Add(orig);
                    ppl2.Add(extent);
                    LineItem curve2 = pane.AddCurve("", ppl2, Color.Black);
                    //curve2.Tag = "NoInfo";
                    curve2.IsVisible = true;

                    pane.XAxis.Scale.Max = 1.0;
                    pane.YAxis.Scale.Max = 1.0;
                    zgcROC.AxisChange();

                    if (pane.CurveList.Count > 1)
                    {
                        //highlight the max auc trace 
                        LineItem bestcurve = (LineItem)pane.CurveList[maxmodelptr];
                        //bestcurve.Symbol.Size = 12.0f;
                        bestcurve.Line.Width = 3.0f;
                        bestcurve.Line.Color = Color.Red;

                        //highlight the model with the max auc in the listbox
                        //listView3.Items[maxmodelptr].BackColor = Color.Pink;
                        listView3.Items[maxmodelptr].ForeColor = Color.Red;

                        //dump the roc parameters to the table (list)
                        string temp = listView3.Items[maxmodelptr].Text;
                        List<object> vals = _tableVals[temp];
                        updateList(vals);

                    }
                    else
                    {
                        MessageBox.Show("The current values for the decision criterion and regulatory standard do not permit ROC curves to be calculated.",
                            "No Appropriate Plot Data", MessageBoxButtons.OK);
                    }
                }
            }
        }

        /// <summary>
        /// populates the roc table (list) with information for the selected model
        /// </summary>
        /// <param name="data"></param>
        private void updateList(List<object> data)
        {
            listView4.Items.Clear();
            foreach (object element in data)
            {
                ROCParameters rocp = new ROCParameters((object[])element);
                string[] item = new string[5];
                item[0] = rocp.DecisionThreshold.ToString("f4");
                item[3] = rocp.Specificity.ToString("f4");
                item[4] = rocp.Sensitivity.ToString("f4");
                item[1] = rocp.FalsePosCount.ToString("n0");
                item[2] = rocp.FalseNegCount.ToString("n0");

                ListViewItem lvi = new ListViewItem(item);
                listView4.Items.Add(lvi);
            }

        }

        /// <summary>
        /// plots auc curves for selected models chosen from
        /// the model/auc list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnROCPlot_Click(object sender, EventArgs e)
        {
            //do plot manipulations for user selected models
            if (_ndxs != null)
            {
                if (_ndxs.Length > 0) plotROCCurves();
            }
        }

        /// <summary>
        /// method for gathering an index list from the model/auc list
        /// of selected models for subsequent plotting of roc curves of 
        /// those selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView3_Leave(object sender, EventArgs e)
        {
            //collect a list of indicies of user selected models
            int[] ndxs = new int[listView3.SelectedIndices.Count];
            listView3.SelectedIndices.CopyTo(ndxs, 0);
            if (ndxs.Length > 0) _ndxs = ndxs;

        }

        /// <summary>
        /// method for getting the first selected model from the model/auc list
        /// and updating the roc table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView3.SelectedItems.Count > 0)
            {
                ListViewItem li = listView3.SelectedItems[0];
                string modelName = li.Text;
                if (!string.IsNullOrWhiteSpace(modelName))
                {
                    List<object> vals = _tableVals[modelName];
                    updateList(vals);
                }
            }

        }

        /// <summary>
        /// method for copy/paste of the roc table functionality
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView4_KeyDown(object sender, KeyEventArgs e)
        {
            {
                if ((e.Modifiers & Keys.Control) != 0)
                {
                    if (e.KeyCode == Keys.C)
                    {
                        ////string s = string.Empty;
                        //string s = "Threshold\tFalsePos\tFalseNeg\tSpecificity\tSensitivity\n";
                        //foreach (ListViewItem item in listView4.Items)
                        //{
                        //    string si = string.Empty;
                        //    for (int i = 0; i < item.SubItems.Count; i++)
                        //    {
                        //        si += item.SubItems[i].Text + "\t";
                        //    }
                        //    s += si + "\n";

                        //}
                        //Clipboard.SetText(s);

                        //use kurt's code instead....
                        CopyListViewToClipboard(listView4);
                    }
                }
            }
        }

        /// <summary>
        /// visibility switcher for the roc plot or roc table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnView_Click(object sender, EventArgs e)
        {
            if (btnView.Text == "View Table")
            {
                //plot is not visible, table is, toggle button text
                btnView.Text = "View Plot";
                listView4.Visible = true;
                zgcROC.Visible = false;
            }
            else if (btnView.Text == "View Plot")
            {
                //table is not visible, plot is, toggle button text
                btnView.Text = "View Table";
                listView4.Visible = false;
                zgcROC.Visible = true;
            }
        }

        /// <summary>
        /// performs plotting of roc curves.  actually, curves for all models are already plotted,
        /// this method just makes those not selected from the list invisible, others visible
        /// </summary>
        private void plotROCCurves()
        {
            //CurveItem visible/invisible switcher; provides UI clues in list of models as well

            GraphPane pane = zgcROC.GraphPane;

            for (int ci = 0; ci < pane.CurveList.Count; ci++)
            {
                CurveItem curve = pane.CurveList[ci];
                if (_ndxs.Contains(ci))
                {
                    curve.IsVisible = true;
                    listView3.Items[ci].BackColor = Color.Empty;
                }
                else
                {
                    curve.IsVisible = false;
                    if (ci < listView3.Items.Count) //need this for the noinfo curve - it's not in the list
                        listView3.Items[ci].BackColor = Color.LightGray;
                }
            }

            //turn the NoInfo curve back on
            pane.CurveList[""].IsVisible = true;

            zgcROC.Refresh();
        }

        #endregion;
                   

        /// <summary>
        /// //tell main to enable the modelSave/SaveAs menu options
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnModelSelected(EventArgs e)
        {
            EventHandler eh = ModelSelected;
            if (eh != null) eh(this, e);
        }


        private void rbPwrValMET_CheckedChanged(object sender, EventArgs e)
        {
            if (rbPwrValMET.Checked)
                txtPwrValMET.Enabled = true;
            else
                txtPwrValMET.Enabled = false;
        }


        private void txtPwr_Leave(object sender, EventArgs e)
        {
            double power;
            TextBox txtBox = (TextBox)sender;

            if (!Double.TryParse(txtBox.Text, out power))
            {
                MessageBox.Show("Invalid number.");
                txtBox.Focus();
            }
        }


        private void txtPopSize_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(txtPopSize, "");
        }


        private void txtPopSize_Validating(object sender, CancelEventArgs e)
        {
            int popSize;
            if (!int.TryParse(txtPopSize.Text, out popSize))
            {
                e.Cancel = true;
                txtPopSize.Select(0, txtPopSize.Text.Length);
                this.errorProvider1.SetError(txtPopSize, "Text must convert to an integer.");
                return;
            }

            if (popSize < 25)
            {
                e.Cancel = true;
                txtPopSize.Select(0, txtPopSize.Text.Length);
                this.errorProvider1.SetError(txtPopSize, "Population size must be 25 or greater.");
                return;
            }
        }


        private void txtNumGen_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(txtNumGen, "");
        }

        private void txtNumGen_Validating(object sender, CancelEventArgs e)
        {
            int numGen;
            if (!int.TryParse(txtNumGen.Text, out numGen))
            {
                e.Cancel = true;
                txtNumGen.Select(0, txtNumGen.Text.Length);
                this.errorProvider1.SetError(txtNumGen, "Text must convert to an integer.");
                return;
            }

            if (numGen < 25)
            {
                e.Cancel = true;
                txtNumGen.Select(0, txtNumGen.Text.Length);
                this.errorProvider1.SetError(txtNumGen, "Number of generations must be 25 or greater.");
                return;
            }
        }


        private void txtMutRate_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(txtMutRate, "");
        }


        private void txtMutRate_Validating(object sender, CancelEventArgs e)
        {
            double mutRate;
            if (!double.TryParse(txtMutRate.Text, out mutRate))
            {
                e.Cancel = true;
                txtMutRate.Select(0, txtMutRate.Text.Length);
                this.errorProvider1.SetError(txtMutRate, "Mutation rate must be a number between 0 and 1.");
                return;
            }

            if ((mutRate > 1.0) || (mutRate < 0.0))
            {
                e.Cancel = true;
                txtMutRate.Select(0, txtMutRate.Text.Length);
                this.errorProvider1.SetError(txtMutRate, "Mutation rate must be a number between 0 and 1.");
                return;
            }
        }


        private void txtCrossoverRate_Validating(object sender, CancelEventArgs e)
        {
            double crossRate;
            if (!double.TryParse(txtCrossoverRate.Text, out crossRate))
            {
                e.Cancel = true;
                txtCrossoverRate.Select(0, txtCrossoverRate.Text.Length);
                this.errorProvider1.SetError(txtCrossoverRate, "Crossover rate must be a number between 0 and 1.");
                return;
            }

            if ((crossRate > 1.0) || (crossRate < 0.0))
            {
                e.Cancel = true;
                txtCrossoverRate.Select(0, txtCrossoverRate.Text.Length);
                this.errorProvider1.SetError(txtCrossoverRate, "Crossover rate must be a number between 0 and 1.");
                return;
            }
        }


        private void txtCrossoverRate_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(txtCrossoverRate, "");
        }


        //Added by Wesley for IronPython integration:


        //Handle a request from an IronPython-based modeling tab to begin the modeling process.
        private void ProvideData(object sender, IPyModelingControl.ModelingCallback CallbackObject)
        {
            _projMgr.ModelDataTable = CreateModelDataTable();
            CallbackObject.MakeModel(_projMgr.ModelDataTable);
        }


        //Handle a request from an IronPython-based modeling tab to enter a message in the Log.
        private void HandleMessageForLog(object sender, IPyModelingControl.LogMessageEvent MessageObject)
        {
            VBLogger.getLogger().logEvent("0", (LogUtilities.VBLogger.messageIntent)MessageObject.Intent, (LogUtilities.VBLogger.targetSStrip)MessageObject.Target);
        }


        //Handle a request from an IronPython-based modeling tab to communicate with the Project Manager.
        private void HandleMessageForManager(object sender, IPyModelingControl.MessageEvent MessageObject)
        {
            _projMgr._comms.sendMessage(MessageObject.Message, this);
        }


        //Propagate a request for access to the IronPython interface to the MainForm.
        private void HandleRequestForInterface(object sender, EventArgs e)
        {
            if (IronPythonInterfaceAccessRequested != null)
            {
                IronPythonInterfaceAccessRequested(sender, e);
            }
        }


        //Get the IronPython-based model from the control and save it to disk.
        private void HandleSaveIPyModelRequest(object sender, EventArgs e)
        {
            IPyModeling.ModelState modelState = ((IPyModelingControl.IPyModelingControl)sender).PackModelState();
            if (SaveIPyModel != null)
                SaveIPyModel(this, e);
            _projMgr._comms.sendMessage("Show the IPy Prediction form.", this);
        }


        //This event is raised when model-building is complete. It tells us to enable models saving and to display the Residuals tab.
        private void CompletedIPyModeling(object sender, EventArgs e)
        {
            if (ModelSelected != null)
                 ModelSelected(this, e);
            _projMgr._comms.sendMessage("Show the IPy Residuals form.", this);
            _projMgr._comms.sendMessage("Show the IPy Prediction form.", this);
        }
    }
}
