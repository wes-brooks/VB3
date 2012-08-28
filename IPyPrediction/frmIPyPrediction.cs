using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using VBCommon;
using VBCommon.IO;
using VBCommon.Statistics;
using VBCommon.Controls;
using VBCommon.Interfaces;
using Ciloci.Flee;
using IPyCommon;
using IPyModeling;
using DotSpatial.Controls;
using Newtonsoft.Json;


namespace IPyPrediction
{
    //Prediction class.
    [JsonObject]
    public partial class frmIPyPrediction : UserControl, IFormState
    {
        //Get access to the IronPython interface:
        private dynamic ipyInterface = IPyInterface.Interface;
        protected dynamic ipyModel = null;
        //menu to hold transform types
        private ContextMenu cmforResponseVar = new ContextMenu();
        //main effect variables
        private Dictionary<string, string> dictMainEffects;
        //txtModel text
        private string strModelExpression = "";
        //only the main effects in the model
        private string[] strArrReferencedVars = null;
        private DataTable corrDT; 
        private DataTable modelDT = null;
        private List<ListItem> lstIndVars = null;
        //datatables in prediction
        private DataTable dtVariables = null;
        private DataTable dtObs = null;
        private DataTable dtStats = null;
        //transform types
        private Dictionary<string, object> dictTransform = new Dictionary<string, object>();
        //holds datatable for transform types
        private DataTable dtObsOrig = null;
        //has transform occurred
        private bool boolObsTransformed = false;

        //Added for IronPython-based modeling:
        public event EventHandler IronPythonInterfaceRequested;
        public event EventHandler ModelRequested;

        private string strModelTabClean;
        public event EventHandler ModelTabStateRequested;


        //constructor
        public frmIPyPrediction()
        {
            InitializeComponent();
        }


        //Raise a request for access to the IronPython interface - should be raised when the control is created.
        private void RequestIronPythonInterface()
        {
            if (IronPythonInterfaceRequested != null)
            {
                EventArgs e = new EventArgs();
                IronPythonInterfaceRequested(this, e);
            }
        }


        //Raise a request for an IronPython modeling object that we can work on.
        private void RequestModel()
        {
            if (ModelRequested != null)
            {
                EventArgs e = new EventArgs();
                ModelRequested(this, e);
            }
        }


        //return ironpython interface
        public dynamic IronPythonInterface
        {
            get { return ipyInterface.Interface; }
        }


        //Return the IronPython model object
        public dynamic Model
        {
            get { return this.ipyModel; }
        }


        //returns datatable for correlation data
        [JsonProperty]
        public DataTable CorrDT
        {
            get { return this.corrDT; }
        }


        //Reconstruct the saved prediction state
        public void UnpackState(IDictionary<string, object> dictPackedState)
        {
            if (dictPackedState.Count == 0) return;
            //unpack model
            Dictionary<string, object> dictModel = (Dictionary<string, object>)dictPackedState["ModelByString"];
            if (dictModel["ModelString"] == null)
                return;
            //deserialize model
            ipyModel = ipyInterface.Deserialize(dictModel["ModelString"]);
            //unpack transform type
            dictTransform = (Dictionary<string, object>)dictPackedState["Transform"];
            //determine which box to check
            if (Convert.ToInt32(dictTransform["Type"]) == Convert.ToInt32(VBCommon.DependentVariableTransforms.none))
                rbNone.Checked = true;
            else if (Convert.ToInt32(dictTransform["Type"]) == Convert.ToInt32(VBCommon.DependentVariableTransforms.Log10))
                rbLog10.Checked = true;
            else if (Convert.ToInt32(dictTransform["Type"]) == Convert.ToInt32(VBCommon.DependentVariableTransforms.Ln))
                rbLn.Checked = true;
            else if (Convert.ToInt32(dictTransform["Type"]) == Convert.ToInt32(VBCommon.DependentVariableTransforms.Power))
                rbPower.Checked = true;
            //unpack exponent and thresholds textboxes
            txtPower.Text = dictTransform["Exponent"].ToString();
            txtRegStd.Text = Convert.ToDouble(dictModel["RegulatoryThreshold"]).ToString();
            txtDecCrit.Text = Convert.ToDouble(dictModel["DecisionThreshold"]).ToString();
            
            DataSet ds = null;
            //unpack values
            string swIVVals = string.Empty;
            if (dictPackedState.ContainsKey("IVData"))
                swIVVals = (string)dictPackedState["IVData"];

            if (!String.IsNullOrWhiteSpace(swIVVals))
            {
                ds = new DataSet();
                ds.ReadXml(new StringReader(swIVVals), XmlReadMode.ReadSchema);
                dtVariables = ds.Tables[0];
                dgvVariables.DataSource = dtVariables;
                setViewOnGrid(dgvVariables);
            }

            string swOBVals = string.Empty;
            if (dictPackedState.ContainsKey("ObsData"))
                swOBVals = (string)dictPackedState["ObsData"];
            if (!String.IsNullOrWhiteSpace(swOBVals))
            {
                ds = new DataSet();
                ds.ReadXml(new StringReader(swOBVals), XmlReadMode.ReadSchema);
                dtObs = ds.Tables[0];
                dgvObs.DataSource = dtObs;
                setViewOnGrid(dgvObs);
            }

            string swStatVals = string.Empty;
            if (dictPackedState.ContainsKey("StatData"))
                swStatVals = (string)dictPackedState["StatData"];
            if (!String.IsNullOrWhiteSpace(swStatVals))
            {
                ds = new DataSet();
                ds.ReadXml(new StringReader(swStatVals), XmlReadMode.ReadSchema);
                dtStats = ds.Tables[0];
                dgvStats.DataSource = dtStats;
                setViewOnGrid(dgvStats);
            }
            //rebuild txtModel text
            strModelExpression = ipyInterface.GetModelExpression(ipyModel).Replace("[", "(").Replace("]", ")");
            txtModel.Text = strModelExpression;

            ds = null;
        }


        //Pack State for Serializing
        public IDictionary<string, object> PackState()
        {
            //to hold packed state
            IDictionary<string, object> dictPluginState = new Dictionary<string, object>();

            if (ipyModel == null)
                return null;
            //serialize the model
            string strModelString = ipyInterface.Serialize(Model);
            double dblRegulatoryThreshold;
            double dblDecisionThreshold;

            //Save the state of the power transform exponent textbox
            double dblTransformExponent = 1;
            try { dblTransformExponent = Convert.ToDouble(txtPower.Text); }
            catch { } //If the textbox can't be converted to a number, then leave the exponent as 1

            //pack transform type
            Dictionary<string, object> dictTfrmDependentVariableTransform = new Dictionary<string,object>();
            dictTfrmDependentVariableTransform.Add("Exponent", dblTransformExponent);

            if (rbNone.Checked)
                dictTfrmDependentVariableTransform["Type"] = DependentVariableTransforms.none;
            else if (rbLog10.Checked)
                dictTfrmDependentVariableTransform["Type"] = DependentVariableTransforms.Log10;
            else if (rbLn.Checked)
                dictTfrmDependentVariableTransform["Type"] = DependentVariableTransforms.Ln;
            else if (rbPower.Checked)
                dictTfrmDependentVariableTransform["Type"] = DependentVariableTransforms.Power;

            try { dblRegulatoryThreshold = Convert.ToDouble(txtRegStd.Text); }
            catch (InvalidCastException) { dblRegulatoryThreshold = -1; }

            try { dblDecisionThreshold = Convert.ToDouble(txtDecCrit.Text); }
            catch (InvalidCastException) { dblDecisionThreshold = -1; }
            //pack model as string and as model for serializing. need to versions for Json.net serialization (doesn't recognize ironpython characters)
            Dictionary<string, object> dictModel = IPyCommon.Helper.ModelState(model: ipyModel, dblRegulatoryThreshold: dblRegulatoryThreshold, decisionThreshold: dblDecisionThreshold, transform: dictTfrmDependentVariableTransform);
            Dictionary<string, object> dictModelByString = IPyCommon.Helper.ModelState(modelString: strModelString, dblRegulatoryThreshold: dblRegulatoryThreshold, decisionThreshold: dblDecisionThreshold, transform: dictTfrmDependentVariableTransform);
            
            dictPluginState.Add("ModelByObject", dictModel);
            dictPluginState.Add("ModelByString", dictModelByString);
            dictPluginState.Add("Transform", dictTfrmDependentVariableTransform);

            StringWriter sw = null;
            //pack values
            dgvVariables.EndEdit();
            dtVariables = (DataTable)dgvVariables.DataSource;
            if (dtVariables != null)
            {
                dtVariables.AcceptChanges();
                dtVariables.TableName = "Variables";
                sw = new StringWriter();
                dtVariables.WriteXml(sw, XmlWriteMode.WriteSchema, false);
                string swIVValues = sw.ToString();
                dictPluginState.Add("IVData", swIVValues);
                sw.Close();
            }

            dgvObs.EndEdit();
            dtObs = (DataTable)dgvObs.DataSource;
            if (dtObs != null)
            {
                dtObs.AcceptChanges();
                dtObs.TableName = "Observations";
                sw = new StringWriter();
                dtObs.WriteXml(sw, XmlWriteMode.WriteSchema, false);
                string swOBsValues = sw.ToString();
                dictPluginState.Add("ObsData", swOBsValues);
                sw.Close();
            }

            dgvStats.EndEdit();
            dtStats = (DataTable)dgvStats.DataSource;
            if (dtStats != null)
            {
                dtStats.AcceptChanges();
                dtStats.TableName = "Stats";
                sw = new StringWriter();
                dtStats.WriteXml(sw, XmlWriteMode.WriteSchema, false);
                string swStatValues = sw.ToString();
                dictPluginState.Add("StatData", swStatValues);
                sw.Close();
            }           
            return dictPluginState;
        }


        //set the model using packed state of model
        public void SetModel(IDictionary<string,object> dictPackedState)
        {
            //make sure empty model doesnt run through this method
            if (dictPackedState.Count <= 2)
                return;
            Dictionary<string, object> dictModel = (Dictionary<string, object>)dictPackedState["ModelByObject"];
            dictTransform = (Dictionary<string, object>)dictPackedState["Transform"];

            //if ((bool)dictPackedState["CleanPredict"])
            //    ClearDataGridViews();

            if (dictModel != null)
            {
                //datatables serialized as xml string to maintain extendedProperty values
                string strXmlDataTable = (string)dictPackedState["CorrelationDataTable"];
                StringReader sr = new StringReader(strXmlDataTable);
                DataSet ds = new DataSet();
                ds.ReadXml(sr);
                sr.Close();
                corrDT = ds.Tables[0];
                //unpack independent variables and text boxes
                lstIndVars = (List<ListItem>)dictPackedState["Predictors"];
                txtDecCrit.Text = ((double)dictModel["DecisionThreshold"]).ToString();
                txtRegStd.Text = ((double)dictModel["RegulatoryThreshold"]).ToString();
                txtPower.Text = (dictTransform["Exponent"]).ToString();
                
                //This is how VB makes predictions in IronPython:
                ipyModel = dictModel["Model"];
                //format txtModel textbox
                strModelExpression = ipyInterface.GetModelExpression(ipyModel).Replace("[", "(").Replace("]", ")");
                txtModel.Text = strModelExpression;
                //add first 2 column names to list
                List<string> list = new List<string>();
                list.Add(corrDT.Columns[0].ColumnName);
                list.Add(corrDT.Columns[1].ColumnName);

                int intNumVars = lstIndVars.Count;
                for (int i = 0; i < intNumVars; i++)
                    list.Add(lstIndVars[i].ToString());

                //Lets get all the main effect variables
                dictMainEffects = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
                for (int i = 2; i < corrDT.Columns.Count; i++)
                {
                    bool bMainEffect = Support.IsMainEffect(corrDT.Columns[i].ColumnName, corrDT);
                    if (bMainEffect)
                    {
                        string strColName = corrDT.Columns[i].ColumnName;
                        dictMainEffects.Add(strColName, strColName);
                    }
                }

                //determine which transform type box to check
                string strTransformType = dictTransform["Type"].ToString();
                if (String.Compare(strTransformType, VBCommon.DependentVariableTransforms.none.ToString(), 0) == 0)
                    rbNone.Checked = true;
                else if (String.Compare(strTransformType, VBCommon.DependentVariableTransforms.Log10.ToString(), 0) == 0)
                    rbLog10.Checked = true;
                else if (String.Compare(strTransformType, VBCommon.DependentVariableTransforms.Ln.ToString(), 0) == 0)
                    rbLn.Checked = true;
                else if (String.Compare(strTransformType, VBCommon.DependentVariableTransforms.Power.ToString(), 0) == 0)
                {
                    rbPower.Checked = true;
                    txtPower.Text = dictTransform["Exponent"].ToString();
                }
                else
                    rbNone.Checked = true;
                //format txtModel textbox
                strModelExpression = ipyInterface.GetModelExpression(ipyModel).Replace("[", "(").Replace("]", ")");
                txtModel.Text = strModelExpression;

                //Lets get only the main effects in the model
                string[] strArrRefvars = ExpressionEvaluator.GetReferencedVariables(strModelExpression, dictMainEffects.Keys.ToArray());
                List<string> lstRefVar = new List<string>();
                lstRefVar.Add("ID");
                lstRefVar.AddRange(strArrRefvars);
                strArrReferencedVars = lstRefVar.ToArray();
            }
            else
                ipyModel = null;
        }



        private void dgvVariables_Scroll(object sender, ScrollEventArgs e)
        {
            int intFirst = dgvVariables.FirstDisplayedScrollingRowIndex;
            if (dgvObs.Rows.Count > 0)
            {                
                if (intFirst >= dgvObs.Rows.Count)
                    dgvObs.FirstDisplayedScrollingRowIndex = dgvObs.Rows.Count - 1;
                else
                    dgvObs.FirstDisplayedScrollingRowIndex = dgvVariables.FirstDisplayedScrollingRowIndex;
            }
            if (dgvStats.Rows.Count > 0)
            {
                if (intFirst >= dgvStats.Rows.Count)
                    dgvStats.FirstDisplayedScrollingRowIndex = dgvStats.Rows.Count - 1;
                else
                    dgvStats.FirstDisplayedScrollingRowIndex = dgvVariables.FirstDisplayedScrollingRowIndex;
            }
        }


        private void dgvObs_Scroll(object sender, ScrollEventArgs e)
        {
            int intFirst = dgvObs.FirstDisplayedScrollingRowIndex;
            if (dgvVariables.Rows.Count > 0)
            {
                if (intFirst >= dgvVariables.Rows.Count)
                    dgvVariables.FirstDisplayedScrollingRowIndex = dgvVariables.Rows.Count - 1;
                else
                    dgvVariables.FirstDisplayedScrollingRowIndex = dgvObs.FirstDisplayedScrollingRowIndex;            
            }

            if (dgvStats.Rows.Count > 0)
            {
                if (intFirst >= dgvStats.Rows.Count)
                    dgvStats.FirstDisplayedScrollingRowIndex = dgvStats.Rows.Count - 1;
                else
                    dgvStats.FirstDisplayedScrollingRowIndex = dgvObs.FirstDisplayedScrollingRowIndex;
            }
        }


        private void dgvStats_Scroll(object sender, ScrollEventArgs e)
        {
            int intFirst = dgvStats.FirstDisplayedScrollingRowIndex;
            if (dgvVariables.Rows.Count > 0)
            {
                if (intFirst >= dgvVariables.Rows.Count)
                    dgvVariables.FirstDisplayedScrollingRowIndex = dgvVariables.Rows.Count - 1;
                else
                    dgvVariables.FirstDisplayedScrollingRowIndex = dgvStats.FirstDisplayedScrollingRowIndex;
            }

            if (dgvObs.Rows.Count > 0)
            {
                if (intFirst >= dgvObs.Rows.Count)
                    dgvObs.FirstDisplayedScrollingRowIndex = dgvObs.Rows.Count - 1;
                else
                    dgvObs.FirstDisplayedScrollingRowIndex = dgvStats.FirstDisplayedScrollingRowIndex;
            }
        }


        //import IV datatable
        public void btnImportIVs_Click(object sender, EventArgs e)
        {
            VBCommon.IO.ImportExport import = new ImportExport();
            DataTable dt = import.Input;            
            if (dt == null)
                return;

            string[] strArrHeaderCaptions = { "Model Variables", "Imported Variables" };
            Dictionary<string, string> dictFields = new Dictionary<string, string>(dictMainEffects);
            frmColumnMapper colMapper = new frmColumnMapper(strArrReferencedVars, dt, strArrHeaderCaptions, true);
            DialogResult dr = colMapper.ShowDialog();

            if (dr == DialogResult.OK)
            {
                dt = colMapper.MappedTable;

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

            foreach (DataGridViewColumn dvgCol in dgvVariables.Columns)
            {
                dvgCol.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
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
            Dictionary<string, int> dictTemp = new Dictionary<string, int>();
            int intNdx = -1;
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string strTempval = dr["ID"].ToString();
                    dictTemp.Add(dr["ID"].ToString(), ++intNdx);
                    if (string.IsNullOrWhiteSpace(dr["ID"].ToString()))
                    {
                        where = intNdx++;
                        //MessageBox.Show("Record Identifier values cannot be blank - encountered blank in row " + ndx++.ToString() + ".\n",
                        //    "Import data error", MessageBoxButtons.OK);
                        return false;
                    }
                }
            }
            catch (ArgumentException)
            {
                where = intNdx++;
                //MessageBox.Show("Record Identifier values cannot be duplicated - encountered existing record in row " + ndx++.ToString() + ".\n",
                //    "Import data error", MessageBoxButtons.OK);
                return false;
            }
            where = intNdx;
            return true;
        }


        public void ClearDataGridViews()
        {
            //when changes made to modeling, clear the prediction tables (reset)
            this.dgvStats.DataSource = null;
            this.dgvObs.DataSource = null;
            this.dgvVariables.DataSource = null;
        }


        //Import OB datatable
        public void btnImportObs_Click(object sender, EventArgs e)
        {
            VBCommon.IO.ImportExport import = new ImportExport();
            DataTable dt = import.Input;
            if (dt == null)
                return;

            string[] strArrHeaderCaptions = { "Obs IDs", "Obs" };
            string[] strArrObsColumns = { "ID", "Observation" };

            frmColumnMapper colMapper = new frmColumnMapper(strArrObsColumns, dt, strArrHeaderCaptions, true);
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
        }


        //make predictions based on imported ob and iv datatables
        public void btnMakePredictions_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (ipyInterface == null) RequestIronPythonInterface();
            if (ipyModel == null) RequestModel();

            dtVariables = (DataTable)dgvVariables.DataSource;
            if (dtVariables == null)
                return;

            if (dtVariables.Rows.Count < 1)
                return;
            //end edits and accept changes on ob and iv tables
            dgvVariables.EndEdit();
            dtVariables.AcceptChanges();

            dgvObs.EndEdit();            
            dtObs = (DataTable)dgvObs.DataSource;
            if (dtObs != null)
                dtObs.AcceptChanges();
            //create table used for prediction
            DataTable tblForPrediction = dtVariables.AsDataView().ToTable();
            tblForPrediction.Columns.Remove("ID");
            
            //formatting
            string[] strArrExpressions = strModelExpression.Split('+');
            foreach(string var in strArrExpressions)
            {
                int intIndx;
                string strVariable = var.Trim();
                if((intIndx = strVariable.IndexOf('(')) != -1)
                    if((intIndx = strVariable.IndexOf(')', intIndx)) != -1)
                        intIndx=0;
            }

            //This pattern should match any variable transformation
            string pattern = @"(MAX|MEAN|PROD|SUM|MIN)\(([^\+]*)\)";
            Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
            Match m = r.Match(strModelExpression);

            Cursor.Current = Cursors.WaitCursor;
                        
            if(m.Success)
            {
                //Create a list that will hold any matched transformations.
                while(m.Success)
                {
                    //Reconstruct the expression from the matched string. Default operation is summation.
                    Globals.Operations op = Globals.Operations.SUM;
                    switch(m.Groups[1].Value)
                    {
                        case "MAX":
                             op = Globals.Operations.MAX;
                            break;
                        case "MEAN":
                            op = Globals.Operations.MEAN;
                            break;
                        case "PROD":
                            op = Globals.Operations.PROD;
                            break;
                        case "SUM":
                            op = Globals.Operations.SUM;
                            break;
                        case "MIN":
                            op = Globals.Operations.MIN;
                            break;
                    }

                    //Create the Expression object.
                    string[] strArrVars = m.Groups[2].Value.Split(',');
                    
                    //Add this expression to the list and continue on.
                    m = m.NextMatch();
                }
            }
            //make prediction
            dynamic dynPredictions = ipyInterface.Predict(ipyModel, tblForPrediction);
            List<double> lstPredictions = ((IList<object>)dynPredictions).Cast<double>().ToList();

            Cursor.Current = Cursors.WaitCursor;

            //create prediction table to show prediction
            DataTable dtPredictions = new DataTable();

            dtPredictions.Columns.Add("ID", typeof(string));
            dtPredictions.Columns.Add("CalcValue", typeof(double));

            for (int i = 0; i < lstPredictions.Count; i++)
            {
                DataRow dr = dtPredictions.NewRow();
                dr["ID"] = dtVariables.Rows[i]["ID"].ToString();
                dr["CalcValue"] = lstPredictions[i];
                dtPredictions.Rows.Add(dr);
            }

            dtStats = GeneratePredStats(dtPredictions, dtObs, tblForPrediction);
            
            Cursor.Current = Cursors.WaitCursor;

             if (dtStats == null)
                 return;

            dgvStats.DataSource = dtStats;
            foreach (DataGridViewColumn dvgCol in dgvStats.Columns)
                dvgCol.SortMode = DataGridViewColumnSortMode.NotSortable;

            setViewOnGrid(dgvStats);            
        }


        //finish editing variables table
        private void dgvVariables_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            dgvVariables.EndEdit();
            dtVariables = (DataTable)dgvVariables.DataSource;
            dtVariables.AcceptChanges();
        }


        //finish editing ob table
        private void dgvObs_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            dgvObs.EndEdit();
            dtObs = (DataTable)dgvVariables.DataSource;
            dtObs.AcceptChanges();
        }


        //generate prediction data for table
        private DataTable GeneratePredStats(DataTable dtPredictions, DataTable dtObs, DataTable dtRaw)
        {
            VBCommon.DependentVariableTransforms dvt = GetTransformType();
            if (dvt == VBCommon.DependentVariableTransforms.Power)
            {
                if (ValidateNumericTextBox(txtPower) == false)
                    return null;
            }
            //store decCrit as dbl
            double dblCrit = Convert.ToDouble(txtDecCrit.Text);
            dblCrit = GetTransformedValue(dblCrit);
            //store reg std as dbl
            double regStd = Convert.ToDouble(txtRegStd.Text);
            regStd = GetTransformedValue(regStd);

            DataTable dt = new DataTable();
            //add columns
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("Model_Prediction", typeof(double));
            dt.Columns.Add("Decision_Criterion", typeof(double));
            dt.Columns.Add("Exceedance_Probability", typeof(double));
            dt.Columns.Add("Regulatory_Standard", typeof(double));
            dt.Columns.Add("Error_Type", typeof(string));
            dt.Columns.Add("Untransformed", typeof(double));

            double dblPredValue = 0.0;
            string strId = "";
            dynamic dynExceedanceProbability = ipyInterface.PredictExceedanceProbability(ipyModel, dtRaw);
            List<double> lstExceedanceProbability = ((IList<object>)dynExceedanceProbability).Cast<double>().ToList();
            for (int i = 0; i < dtPredictions.Rows.Count; i++)
            {
                dblPredValue = (double)dtPredictions.Rows[i]["CalcValue"];
                DataRow dr = dt.NewRow();
                strId = (string)dtPredictions.Rows[i]["ID"];
                dr["ID"] = strId;
                dr["Model_Prediction"] = dblPredValue;
                dr["Decision_Criterion"] = dblCrit;
                dr["Exceedance_Probability"] = lstExceedanceProbability[i];
                dr["Regulatory_Standard"] = regStd;

                if (dvt == VBCommon.DependentVariableTransforms.Log10)
                    dr["Untransformed"] = Math.Pow(10, dblPredValue);
                else if (dvt == VBCommon.DependentVariableTransforms.Ln)
                    dr["Untransformed"] = Math.Pow(Math.E, dblPredValue);
                else if (dvt == VBCommon.DependentVariableTransforms.Power)
                {
                    double dblPower = (double)dictTransform["Exponent"];
                    if (dblPower == double.NaN)
                        dblPower = 1.0;

                    dr["Untransformed"] = Math.Sign(dblPredValue) * Math.Pow(Math.Abs(dblPredValue), (1.0 / dblPower));
                }
                else //No transform
                    dr["Untransformed"] = dblPredValue;

                //determine if we have an error and its type
                //No guarentee we have same num of obs as we do predictions or that we have any obs at all
                if ((dtObs != null) && (dtObs.Rows.Count > 0))
                {
                    string strErrType = "";
                    DataRow[] rows = dtObs.Select("ID = '" + strId + "'");

                    if ((rows != null) && (rows.Length > 0))
                    {
                        double dblObs = (double)rows[0][1];
                        if ((dblPredValue > dblCrit) && (dblObs < regStd))
                            strErrType = "False Positive";
                        else if ((dblObs > regStd) && (dblPredValue < dblCrit))
                            strErrType = "False Negative";
                    }
                    dr["Error_Type"] = strErrType;
                }
                dt.Rows.Add(dr);
            }
            return dt;            
        }


        private double GetTransformPower(string pwrTransform)
        {
            if (String.IsNullOrWhiteSpace(pwrTransform))
                return double.NaN;

            char[] chrArrDelim = ",".ToCharArray();
            string[] strArrSvals = pwrTransform.Split(chrArrDelim);

            double dblPower = 1.0;
            if (strArrSvals.Length != 2)
                 return double.NaN;

            if (!Double.TryParse(strArrSvals[1], out dblPower))
                return double.NaN;

            return dblPower;
        }

        //get the transform types for plotting
        private VBCommon.DependentVariableTransforms GetTransformType()
        {
            VBCommon.DependentVariableTransforms dvt = VBCommon.DependentVariableTransforms.none;

            if (String.Compare(dictTransform["Type"].ToString(), VBCommon.DependentVariableTransforms.Log10.ToString(), 0) == 0)
                dvt = VBCommon.DependentVariableTransforms.Log10;
            else if (String.Compare(dictTransform["Type"].ToString(), VBCommon.DependentVariableTransforms.Ln.ToString(), 0) == 0)
                dvt = VBCommon.DependentVariableTransforms.Ln;
            else if (String.Compare(dictTransform["Type"].ToString(), VBCommon.DependentVariableTransforms.Power.ToString(), 0) == 0)
                dvt = VBCommon.DependentVariableTransforms.Power;

            return dvt;
        }


        //Backconvert to get the output on its original scale
        private double UntransformThreshold(double value)
        {
            if (rbNone.Checked)
                return value;
            else if (rbLog10.Checked)
                return Math.Pow(10, value);
            else if (rbLn.Checked)
                return Math.Exp(value);
            else if (rbPower.Checked)
                return Math.Sign(value) * Math.Pow(Math.Abs(value), 1 / GetTransformPower(txtPower.Text));
            else
                return value;
        }


        private void splitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {
            int left1 = splitContainer1.Panel2.Left;
            int left2 = splitContainer2.Panel2.Left;
        }

        
        //load the prediction form
        private void frmIPyPrediction_Load(object sender, EventArgs e)
        {
            cmforResponseVar.MenuItems.Add("Transform");
            cmforResponseVar.MenuItems[0].MenuItems.Add("Log10", new EventHandler(Log10T));
            cmforResponseVar.MenuItems[0].MenuItems.Add("Ln", new EventHandler(LnT));
            cmforResponseVar.MenuItems[0].MenuItems.Add("Power", new EventHandler(PowerT));
            cmforResponseVar.MenuItems.Add("Untransform", new EventHandler(Untransform));
        }


        private void Untransform(object o, EventArgs e)
        {
            if (dtObsOrig != null)
            {
                DataColumn dc = dtObsOrig.Columns[1];
                dc.ColumnName = "Observation";
                dgvObs.DataSource = dtObsOrig;
                boolObsTransformed = false;
            }
        }


        /// <summary>
        /// response variable transform log10
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void Log10T(object o, EventArgs e)
        {
            //save changes to ob table
            dgvObs.EndEdit();
            DataTable _dt = (DataTable)dgvObs.DataSource;
            DataTable _dtCopy = _dt.Copy();
            if (_dt == null) return;
            dtObsOrig = _dt;
            //save how many rows and set tranform flag to true
            double[] dblArrNewvals = new double[_dt.Rows.Count];
            boolObsTransformed = true;

            for (int i=0;i<_dtCopy.Rows.Count;i++)
            {
                _dtCopy.Rows[i][1] = dblArrNewvals[i];                
            }

            DataColumn dc = _dtCopy.Columns["Observation"];
            dc.ColumnName = "LOG10[Observation]";
            //save new dttable to obs table
            dgvObs.DataSource = _dtCopy;
        }


        // response variable transform LnT
        private void LnT(object o, EventArgs e)
        {
            dgvObs.EndEdit();
            DataTable _dt = (DataTable)dgvObs.DataSource;
            DataTable _dtCopy = _dt.Copy();
            if (_dt == null) return;
            dtObsOrig = _dt;

            double[] dblArrNewvals = new double[_dt.Rows.Count];
            boolObsTransformed = true;

            for (int i = 0; i < _dtCopy.Rows.Count; i++)
            {
                _dtCopy.Rows[i][1] = dblArrNewvals[i];
            }
            DataColumn dc = _dtCopy.Columns["Observation"];
            dc.ColumnName = "LN[Observation]";

            dgvObs.DataSource = _dtCopy;
        }

        // response variable transform PowerT
        private void PowerT(object o, EventArgs e)
        {
            dgvObs.EndEdit();
            DataTable _dt = (DataTable)dgvObs.DataSource;
            DataTable _dtCopy = _dt.Copy();
            if (_dt == null) return;
            dtObsOrig = _dt;
            
            frmPowerExponent frmExp = new frmPowerExponent(_dt, 1);
            DialogResult dlgr = frmExp.ShowDialog();
            if (dlgr != DialogResult.Cancel)
            {
                double[] dblNewvals = new double[_dt.Rows.Count];
                dblNewvals = frmExp.TransformedValues;
                if (frmExp.TransformMessage != "")
                {
                    MessageBox.Show("Cannot Power transform variable. " + frmExp.TransformMessage, "VB Transform Rule", MessageBoxButtons.OK);
                    return;
                }

                boolObsTransformed = true;
                string strSexp = frmExp.Exponent.ToString("n2");
                for (int i = 0; i < _dtCopy.Rows.Count; i++)
                {
                    _dtCopy.Rows[i][1] = dblNewvals[i];
                }

                DataColumn dc = _dtCopy.Columns["Observation"];
                dc.ColumnName = "POWER[" + strSexp+ ",Observation]";
                dgvObs.DataSource = _dtCopy;
            }
        }
        

        //export the predictions table
        public void btnExportTable_Click(object sender, EventArgs e)
        {
            //end and save edits on all 3 tables
            dgvVariables.EndEdit();
            dtVariables = (DataTable)dgvVariables.DataSource;
            if (dtVariables != null)
                dtVariables.AcceptChanges();
            else
                return;

            dgvObs.EndEdit();
            dtObs = (DataTable)dgvObs.DataSource;
            if (dtObs != null)
                dtObs.AcceptChanges();

            dgvStats.EndEdit();
            dtStats = (DataTable)dgvStats.DataSource;
            if (dtStats != null)
                dtStats.AcceptChanges();

            if ((dtVariables == null) && (dtObs == null) && (dtStats == null))
                return;
            //save exported as
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Export Prediction Data";
            sfd.Filter = @"CSV Files|*.csv|All Files|*.*";

            DialogResult dr = sfd.ShowDialog();
            if (dr == DialogResult.Cancel)
                return;

            int intMaxRowsVars = dtVariables.Rows.Count;
            int intMaxRowsObs = 0;
            int intMaxRowsStats = 0;
            
            if (dtObs != null)
                intMaxRowsObs = dtObs.Rows.Count;
            if (dtStats != null)
                intMaxRowsStats = dtStats.Rows.Count;

            int intMaxRows = Math.Max(intMaxRowsVars, Math.Max(intMaxRowsObs, intMaxRowsStats));

            StringBuilder sb = new StringBuilder();

            //Write out the column headers
            if (dtVariables != null)
            {
                for (int i = 0; i < dtVariables.Columns.Count; i++)
                {
                    if (i > 0)
                        sb.Append(",");

                    sb.Append(dtVariables.Columns[i].ColumnName);
                }
            }

            if (dtObs != null)
            {
                for (int i = 0; i < dtObs.Columns.Count; i++)
                {
                    sb.Append(",");
                    sb.Append(dtObs.Columns[i].ColumnName);
                }
            }

            if (dtStats != null)
            {
                for (int i = 0; i < dtStats.Columns.Count; i++)
                {
                    sb.Append(",");
                    sb.Append(dtStats.Columns[i].ColumnName);
                }
            } //Finished writing out column headers
            

            sb.Append(Environment.NewLine);

            //write out the data
            for (int i = 0; i < intMaxRows; i++)
            {
                for (int j = 0; j < dtVariables.Columns.Count; j++)
                {
                    if (j > 0)
                        sb.Append(",");

                    if (i < dtVariables.Rows.Count)
                        sb.Append(dtVariables.Rows[i][j].ToString());
                    else
                        sb.Append("");
                }
               

                if (dtObs != null)
                {
                    for (int j = 0; j < dtObs.Columns.Count; j++)
                    {                        
                        sb.Append(",");

                        if (i < dtObs.Rows.Count)
                            sb.Append(dtObs.Rows[i][j].ToString());
                        else
                            sb.Append("");
                    }    
                }

                if (dtStats != null)
                {
                    for (int j = 0; j < dtStats.Columns.Count; j++)
                    {
                        sb.Append(",");

                        if (i < dtStats.Rows.Count)
                            sb.Append(dtStats.Rows[i][j].ToString());
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


        //add comma separators to the column names
        private StringBuilder AddCommaSeparatedColumns(DataTable dt, StringBuilder sb)
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


        //add a row with commas added
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


        //Import table
        public void btnImportTable_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Open Prediction Data";
            ofd.Filter = @"VB2 Prediction Files|*.vbpred|All Files|*.*";
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.Cancel)
                return;
            //save filename
            string fileName = ofd.FileName;
            //read the incoming table
            DataSet ds = new DataSet();
            ds.ReadXml(fileName, XmlReadMode.ReadSchema);

            if (ds.Tables.Contains("Variables") == false)
            {
                MessageBox.Show("Invalid Prediction Dataset.  Does not contain variable information.");
                return;
            }
            //save values
            dtVariables = ds.Tables["Variables"];
            dtObs = ds.Tables["Observations"];
            dtStats = ds.Tables["Stats"];

            dgvVariables.DataSource = dtVariables;
            dgvObs.DataSource = dtObs;
            dgvStats.DataSource = dtStats;
        }


        //clear tables. set all to null
        public void btnClearTable_Click(object sender, EventArgs e)
        {
            dgvVariables.DataSource = null;
            dgvObs.DataSource = null;
            dgvStats.DataSource = null;

            if (dtVariables != null)
                dtVariables.Clear();            
            dtVariables = null;

            if (dtObs != null)
                dtObs.Clear();            
            dtObs = null;

            if (dtStats != null)
                dtStats.Clear();            
            dtStats = null;

            dgvVariables.DataSource = CreateEmptyIVsDataTable();
            dgvObs.DataSource = CreateEmptyObservationsDataTable();
        }


        //plot predictions
        public void btnPlot_Click(object sender, EventArgs e)
        {
            dgvObs.EndEdit();
            dgvStats.EndEdit();

            //ensure there is observation and prediction data
            DataTable dtObs = dgvObs.DataSource as DataTable;
            DataTable dtStats = dgvStats.DataSource as DataTable;

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
            //start plotting
            frmPredictionScatterPlot frmPlot = new frmPredictionScatterPlot(dtObs, dtStats);
            frmPlot.Show();
            //get the transform for plotting
            Int32 intTransform = Convert.ToInt32(dictTransform["Type"]);
            string strTransform = null;
            switch (intTransform)
            {
                case 0:
                    strTransform = "none";
                    break;
                case 1:
                    strTransform = "Log10";
                    break;
                case 2:
                    strTransform = "Ln";
                    break;
                case 3:
                    strTransform = "Power";
                    break;
            }
            //get the exponent for plotting
            var exp = dictTransform["Exponent"];
            double exponent = Convert.ToDouble(exp);
            //configure the plot display
            frmPlot.ConfigureDisplay(Convert.ToDouble(txtDecCrit.Text), Convert.ToDouble(txtRegStd.Text), strTransform, exponent);
        }


        private DataTable CreateEmptyIVsDataTable()
        {
            //We are going to put an ID column first.
            //ID is used to link IV and Obs records.

            DataTable dt = new DataTable("Variables");
            dt.Columns.Add("ID", typeof(string));

            for (int i = 1; i < strArrReferencedVars.Length;i++)
                dt.Columns.Add(strArrReferencedVars[i], typeof(double));
                       
            return dt;            
        }


        private DataTable CreateEmptyObservationsDataTable()
        {
            DataTable dt = new DataTable("Observations");
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("Observation", typeof(double));

            return dt;
        }

        //selected variable change 
        private void dgvVariables_SelectionChanged(object sender, EventArgs e)
        {
            //unsubscribe
            dgvObs.SelectionChanged -= new EventHandler(dgvObs_SelectionChanged);
            dgvStats.SelectionChanged -= new EventHandler(dgvStats_SelectionChanged);
            //clear data
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
            //resubscribe
            dgvObs.SelectionChanged += new EventHandler(dgvObs_SelectionChanged);
            dgvStats.SelectionChanged += new EventHandler(dgvStats_SelectionChanged);
        }


        //selected observation change
        private void dgvObs_SelectionChanged(object sender, EventArgs e)
        {
            //unsubscribe
            dgvVariables.SelectionChanged -= new EventHandler(dgvVariables_SelectionChanged);
            dgvStats.SelectionChanged -= new EventHandler(dgvStats_SelectionChanged);

            DataGridViewSelectedRowCollection selRowCol = dgvObs.SelectedRows;
            //clear all
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
            //resubscribe
            dgvVariables.SelectionChanged += new EventHandler(dgvVariables_SelectionChanged);
            dgvStats.SelectionChanged += new EventHandler(dgvStats_SelectionChanged);
        }


        //selected stats change
        private void dgvStats_SelectionChanged(object sender, EventArgs e)
        {
            //unsubscribe
            dgvVariables.SelectionChanged -= new EventHandler(dgvVariables_SelectionChanged);
            dgvObs.SelectionChanged -= new EventHandler(dgvObs_SelectionChanged);
            //clear all
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
            //resubscribe
            dgvVariables.SelectionChanged += new EventHandler(dgvVariables_SelectionChanged);
            dgvObs.SelectionChanged += new EventHandler(dgvObs_SelectionChanged);
        }


        public void setViewOnGrid(DataGridView dgv)
        {
            Cursor.Current = Cursors.WaitCursor;

            //utility method used to set numerical precision displayed in grid

            //seems to be the only way I can figure to get a string in col 1 that may
            //(or may not) be a date and numbers in all other columns.
            //in design mode set "no format" for the dgv defaultcellstyle
            if (dgv.Rows.Count <= 1) return;

            string testcellval = string.Empty;
            for (int col = 0; col < dgv.Columns.Count; col++)
            {
                testcellval = dgv[col, 0].Value.ToString();
                double result;
                bool isNum = Double.TryParse(testcellval, out result); //try a little visualbasic magic

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
            double dblResult;
            if (dt == null)
                return null;
            //look for blank and non numeric cell values
            List<string> lstCells = new List<string>();
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
                        lstCells.Add("Row " + dr[0].ToString() + " Column " + dc.Caption + " has blank cell.");
                    else if (!Double.TryParse(dr[dc].ToString(), out dblResult) && dt.Columns.IndexOf(dc) != 0)
                        lstCells.Add("Row " + dr[0].ToString() + " Column " + dc.Caption + " has non-numeric cell value: '" + dr[dc].ToString() + "'");
                }
            }
            return lstCells;
        }


        //event handler for error
        private void dgvVariables_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            string strErr = "Data value must be numeric.";
            dgvVariables.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
            MessageBox.Show(strErr);
        }


        //event handler for error
        private void dgvObs_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            string err = "Data value must be numeric.";
            dgvObs.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
            MessageBox.Show(err);
        }


        //power transform type checked
        private void rbPower_CheckedChanged(object sender, EventArgs e)
        {
            if (rbPower.Checked)
                txtPower.Enabled = true;
            else
                txtPower.Enabled = false;
        }


        //check what is entered in the power textbox
        private bool ValidateNumericTextBox(TextBox txtBox)
        {
            double dblVal = 1.0;
            if (!Double.TryParse(txtBox.Text, out dblVal))
            {
                MessageBox.Show(txtPower.Text + "Invalid number.");
                txtPower.Focus();
                return false;
            }
            return true;
        }


        //check value entered in decCrit textbox when leaving
        private void txtDecCrit_Leave(object sender, EventArgs e)
        {
            double dblResult;
            if (!Double.TryParse(txtDecCrit.Text, out dblResult))
            {
                MessageBox.Show("Invalid number.");
                txtDecCrit.Focus();
            }
        }


        //check value entered in regStd textbox when leaving
        private void txtRegStd_Leave(object sender, EventArgs e)
        {
            double dblResult;
            if (!Double.TryParse(txtRegStd.Text, out dblResult))
            {
                MessageBox.Show("Invalid number.");
                txtRegStd.Focus();
            }
        }


        //transform for log10
        private double GetTransformedValue(double value)
        {
            double dblRetValue = 0.0;
           
           if (rbLog10.Checked)
                dblRetValue = Math.Log10(value);
            else if (rbLn.Checked)
                dblRetValue = Math.Log(value);
            else if (rbPower.Checked)
            {
                double power = Convert.ToDouble(txtPower.Text);
                dblRetValue = Math.Pow(value, power);
            }
           else
               dblRetValue = value;

            return dblRetValue;
        }


        //validate imported datatable
        public void btnIVDataValidation_Click(object sender, EventArgs e)
        {
            //check for non unique records, blank records
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


        //mouseup after right-click on obs
        private void dgvObs_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            showContextMenus((DataGridView)sender, e);
        }


        //show the contextmenu after mouseup to select transform type
        private void showContextMenus(DataGridView dgv, MouseEventArgs me)
        {
            DataGridView.HitTestInfo ht = dgv.HitTest(me.X, me.Y);
            int intColndx = ht.ColumnIndex;
            int intRowndx = ht.RowIndex;

            DataTable _dt = (DataTable)dgvObs.DataSource;
            if (intRowndx > 0 && intColndx > 0) return; //cell hit, go away
            //get transform user selected
            if (intRowndx < 0 && intColndx >= 0)
            {
                if (intColndx == 1)
                {
                    if (!boolObsTransformed)
                    {
                        cmforResponseVar.MenuItems[0].Enabled = true; //we can transform a response variable
                        cmforResponseVar.MenuItems[1].Enabled = false; //but we cannot untransform an untransformed variable
                    }
                    else
                    {
                        cmforResponseVar.MenuItems[0].Enabled = false; //but we cannot transform a transformed response
                        cmforResponseVar.MenuItems[1].Enabled = true; //but we can untransform a transformed response
                    }
                    cmforResponseVar.Show(dgv, new Point(me.X, me.Y));
                }
            }
        }


        private void dgvVariables_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //If user has edited the ID column, make sure the IDs are still unique.
            btnMakePredictions.Enabled = false;
        }


        //create the cells in the datagridview
        private void dgvVariables_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            StringFormat sf = new StringFormat();
            int intCount = dgvVariables.RowCount;
            sf.Alignment = StringAlignment.Center;
            if(( e.ColumnIndex < 0) && (e.RowIndex >= 0) && (e.RowIndex < intCount) )
            {
                e.PaintBackground(e.ClipBounds, true);
                e.Graphics.DrawString((e.RowIndex + 1).ToString(), this.Font, Brushes.Black, e.CellBounds, sf);
                e.Handled = true;
            }
        }

        //event handler 
        private string ModelTabStatus()
        {
            strModelTabClean = null;

            if(ModelTabStateRequested != null)
            {
                EventArgs e = new EventArgs();
                ModelTabStateRequested(this, e);

                while(strModelTabClean == null)
                { }
            }
            return strModelTabClean;
        }


        //set the modeltabstate clean flag
        public string ModelTabState
        {
            set { strModelTabClean = value; }
        }
    }
}
