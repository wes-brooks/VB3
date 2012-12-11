using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using WeifenLuo.WinFormsUI.Docking;
using VBCommon;
using VBCommon.IO;
using VBCommon.Statistics;
using VBCommon.Controls;
using VBCommon.Interfaces;
using VBCommon.Transforms;
using Ciloci.Flee;
using VBProjectManager;
using DotSpatial.Controls;
using Newtonsoft.Json;


namespace Prediction
{
    //Prediction class.
    [JsonObject]
    public partial class frmPrediction : UserControl, IFormState
    {
        private ContextMenu cmforResponseVar = new ContextMenu();
        private Dictionary<string, string> dictMainEffects;

        private IModel model = null;
        public List<Lazy<IModel, IDictionary<string, object>>> models;
        public event EventHandler RequestModelPluginList;

        private string[] strArrReferencedVars = null;
        private DataTable corrDT; 
        private List<ListItem> lstIndVars = null;
        private string strOutputVariable = "Output";

        InputMapper IvMap = null;
        InputMapper ObsMap = null;
        private DataTable dtVariables = null;
        private DataTable dtObs = null;
        private DataTable dtStats = null;

        //private Dictionary<string, object> dictTransform = new Dictionary<string, object>();
        private DataTable dtObsOrig = null;
        private bool boolObsTransformed = false;
        private int intCheckedRVTransform;

        //The transform of the model's original data:
        private DependentVariableTransforms xfrmImported;
        private double dblImportedPowerTransformExp = double.NaN;

        //The transform of the modeling threshold:
        private DependentVariableTransforms xfrmModeled;
        private double dblModeledPowerTransformExp = double.NaN;

        //The transform for entries in the Observations data table:
        private DependentVariableTransforms xfrmObs;
        private double dblObsPowerTransformExp = double.NaN;

        //The transform applied to the regulatory threshold:
        private DependentVariableTransforms xfrmThreshold;
        private double dblThresholdPowerTransformExp = double.NaN;
        
        List<string> listModels = new List<string>();
        IDictionary<string, object> dictPredictionElements = new Dictionary<string, object>();
        private int intSelectedModel = -1;
        private string strMethod;
        
        private string strModelTabClean;
        public event EventHandler ModelTabStateRequested;

        public event EventHandler<ButtonStatusEventArgs> ButtonStatusEvent;


        //constructor
        public frmPrediction()
        {
            InitializeComponent();
        }


        //returns datatable for correlation data
        [JsonProperty]
        public DataTable CorrDT
        {
            get { return this.corrDT; }
        }


        //selected model's index from listbox
        [JsonProperty]
        public int SelectedModel
        {
            get { return intSelectedModel; }
            set { intSelectedModel = value; }
        }


        //Reconstruct the saved prediction state
        public void UnpackState(IDictionary<string, object> dictPackedState)
        {
            if (dictPackedState.Count == 0) return;
                        
            if (dictPackedState.ContainsKey("PredictionElements"))
            {
                dictPredictionElements = (IDictionary<string, object>)dictPackedState["PredictionElements"];

                //Convert the plugin's JSON into .NET objects and compile a dictionary of the deserialized objects.
                Dictionary<string, object> dictTemp = new Dictionary<string, object>();
                foreach (var pair in dictPredictionElements)
                {
                    Type objType = typeof(Dictionary<string, object>);
                    string jsonRep = pair.Value.ToString();

                    object objDeserialized = JsonConvert.DeserializeObject(jsonRep, objType);

                    if (((IDictionary<string, object>)objDeserialized).ContainsKey("VariableMapping"))
                    {
                        objType = typeof(Dictionary<string, object>);
                        jsonRep = ((IDictionary<string, object>)objDeserialized)["VariableMapping"].ToString();

                        object objVariableMapping = JsonConvert.DeserializeObject(jsonRep, objType);
                        ((IDictionary<string, object>)objDeserialized)["VariableMapping"] = (IDictionary<string, object>)objVariableMapping;
                    }

                    if (((IDictionary<string, object>)objDeserialized).ContainsKey("ObsVariableMapping"))
                    {
                        objType = typeof(Dictionary<string, object>);
                        jsonRep = ((IDictionary<string, object>)objDeserialized)["ObsVariableMapping"].ToString();

                        object objVariableMapping = JsonConvert.DeserializeObject(jsonRep, objType);
                        ((IDictionary<string, object>)objDeserialized)["ObsVariableMapping"] = (IDictionary<string, object>)objVariableMapping;
                    }

                    dictTemp.Add(pair.Key, objDeserialized);
                }
                dictPredictionElements = dictTemp;
            }

            if (dictPackedState.ContainsKey("AvailableModels"))
            {
                List<string> listTemp = (List<string>)dictPackedState["AvailableModels"];

                foreach (string method in listTemp)
                    AddModel(method, false);
                
                if (dictPackedState.ContainsKey("AvailableModelsIndex")) { lbAvailableModels.SelectedIndex = (int)dictPackedState["AvailableModelsIndex"]; }                
            }

            if (!dictPackedState.ContainsKey("Model")) { return; }
            Dictionary<string, object> dictModel = (Dictionary<string, object>)dictPackedState["Model"];


            //Unpack the transforms
            xfrmModeled = (DependentVariableTransforms)dictPackedState["xfrmModeled"];
            dblModeledPowerTransformExp = Convert.ToDouble(dictPackedState["ModeledPowerTransformExponent"]);

            xfrmImported = (DependentVariableTransforms)dictPackedState["xfrmImported"];
            dblImportedPowerTransformExp = Convert.ToDouble(dictPackedState["ImportedPowerTransformExponent"]);

            xfrmThreshold = (DependentVariableTransforms)dictPackedState["xfrmThreshold"];
            dblThresholdPowerTransformExp = Convert.ToDouble(dictPackedState["ThresholdPowerTransformExponent"]);

            if (xfrmThreshold == DependentVariableTransforms.none)
                rbNone.Checked = true;
            else if (xfrmThreshold == DependentVariableTransforms.Log10)
                rbLog10.Checked = true;
            else if (xfrmThreshold == DependentVariableTransforms.Ln)
                rbLn.Checked = true;
            else if (xfrmThreshold == DependentVariableTransforms.Power)
                rbPower.Checked = true;

            xfrmObs = (DependentVariableTransforms)dictPackedState["xfrmObs"];
            dblObsPowerTransformExp = Convert.ToDouble(dictPackedState["ObsPowerTransformExponent"]);
            SetTransformCheckmarks(Item: (int)xfrmObs);

            txtPower.Text = dblThresholdPowerTransformExp.ToString();
            txtRegStd.Text = Convert.ToDouble(dictModel["RegulatoryThreshold"]).ToString();
            txtDecCrit.Text = Convert.ToDouble(dictModel["DecisionThreshold"]).ToString();

            //Unpack the current DataGridViews
            dtVariables = DeserializeDataTable(Container: dictPackedState, Slot: "IVData", Title: "Variables");
            if (dtVariables != null)
            {
                dgvVariables.DataSource = dtVariables;
                setViewOnGrid(dgvVariables);
            }

            dtObs = DeserializeDataTable(Container: dictPackedState, Slot: "ObsData", Title: "Observations");
            if (dtObs != null)
            {
                dgvObs.DataSource = dtObs;
                setViewOnGrid(dgvObs);
            }

            dtStats = DeserializeDataTable(Container: dictPackedState, Slot: "StatData", Title: "Stats");
            if (dtStats != null)
            {
                dgvStats.DataSource = dtStats;
                setViewOnGrid(dgvStats);
            }

            if (model != null)
            {
                txtModel.Text = model.ModelString();                
            }
            else
            {
                txtModel.Text = "";
            }
        }


        //Pack State for Serializing
        public IDictionary<string, object> PackState()
        {
            IDictionary<string, object> dictPluginState = new Dictionary<string, object>();

            if (listModels.Count > 0)
            {
                dictPluginState.Add("AvailableModels", listModels);
                if (intSelectedModel >= 0) { dictPluginState.Add("AvailableModelsIndex", intSelectedModel); }
            }

            if (model == null)
                return dictPluginState;

            //Serialize the model
            double dblRegulatoryThreshold;
            double dblDecisionThreshold;

            //Save the state of the power transform exponent textbox
            double dblTransformExponent = 1;
            try { dblTransformExponent = Convert.ToDouble(txtPower.Text); }
            catch { } //If the textbox can't be converted to a number, then leave the exponent as 1


            dictPluginState.Add("xfrmModeled", xfrmModeled);
            dictPluginState.Add("ModeledPowerTransformExponent", dblModeledPowerTransformExp);

            dictPluginState.Add("xfrmImported", xfrmImported);
            dictPluginState.Add("ImportedPowerTransformExponent", dblImportedPowerTransformExp);

            dictPluginState.Add("xfrmThreshold", xfrmThreshold);
            dictPluginState.Add("ThresholdPowerTransformExponent", dblThresholdPowerTransformExp);

            dictPluginState.Add("xfrmObs", xfrmObs);
            dictPluginState.Add("ObsPowerTransformExponent", dblObsPowerTransformExp);

            try { dblRegulatoryThreshold = Convert.ToDouble(txtRegStd.Text); }
            catch (InvalidCastException) { dblRegulatoryThreshold = -1; }

            try { dblDecisionThreshold = Convert.ToDouble(txtDecCrit.Text); }
            catch { dblDecisionThreshold = dblRegulatoryThreshold; }

            //Pack model as string and as model for serializing. need to versions for Json.net serialization (which can't serialize IronPython objects)
            Dictionary<string, object> dictModelState = new Dictionary<string, object>();
            dictModelState.Add("Method", strMethod);
            //dictModelState.Add("Transform", dictTransform);
            dictModelState.Add("RegulatoryThreshold", dblRegulatoryThreshold);
            dictModelState.Add("DecisionThreshold", dblDecisionThreshold);

            dictPluginState.Add("Model", dictModelState);
            //dictPluginState.Add("Transform", dictTransform);
            
            //pack values
            dgvVariables.EndEdit();
            dtVariables = (DataTable)dgvVariables.DataSource;
            SerializeDataTable(Data: dtVariables, Container: dictPluginState, Slot: "IVData", Title: "Variables");

            dgvObs.EndEdit();
            dtObs = (DataTable)dgvObs.DataSource;
            SerializeDataTable(Data: dtObs, Container: dictPluginState, Slot: "ObsData", Title: "Observations");

            dgvStats.EndEdit();
            dtStats = (DataTable)dgvStats.DataSource;
            SerializeDataTable(Data: dtStats, Container: dictPluginState, Slot: "StatData", Title: "Stats");

            dictPluginState.Add("PredictionElements", dictPredictionElements);

            return dictPluginState;
        }


        //store packed state and populate listbox
        public void AddModel(string Method, bool ReplacePredictionElements = true)
        {
            //Disconnect the selection-changed handlers for this process
            this.lbAvailableModels.SelectedIndexChanged -= new System.EventHandler(this.lbAvailableModels_SelectedIndexChanged);            

            //If there is already a model from this plugin in the listBox, then remove it.
            if (listModels.Contains(Method))
            {
                //Remove the Method from the lists
                listModels.Remove(Method);
                lbAvailableModels.Items.Remove(Method);
            }

            if (ReplacePredictionElements)
            {
                if (dictPredictionElements.ContainsKey(Method))
                    dictPredictionElements.Remove(Method);
            }

            //Now add the model to the listBox
            lbAvailableModels.Items.Add(Method);
            listModels.Add(Method);

            //...And re-connect the selection-changed event handler
            this.lbAvailableModels.SelectedIndexChanged += new System.EventHandler(this.lbAvailableModels_SelectedIndexChanged);
        }


        //when user selects model to use, send it to SetModel()
        private void lbAvailableModels_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            //try
            //{
                object selectedItem = lbAvailableModels.SelectedItem;

                //didn't select a model
                if (selectedItem == null)
                {
                    txtModel.Clear();
                    txtPower.Clear();
                    txtRegStd.Clear();
                    txtDecCrit.Clear();
                    rbNone.Select();
                    return;
                }

                string strSelectedItem = selectedItem.ToString();
                intSelectedModel = lbAvailableModels.SelectedIndex;

                //First, pack up the data/observations/predictions for the current plugin
                if (strMethod != null)
                {
                    //Make an entry for this model in dictPredictionElements if none exists
                    if (!dictPredictionElements.ContainsKey(strMethod))
                        dictPredictionElements.Add(strMethod, new Dictionary<string, object>());

                    dgvVariables.EndEdit();
                    dtVariables = (DataTable)dgvVariables.DataSource;
                    if (dtVariables != null)
                        SerializeDataTable(Data: dtVariables, Container: (IDictionary<string, object>)dictPredictionElements[strMethod], Slot: "IVData", Title: "Variables");

                    dgvObs.EndEdit();
                    dtObs = (DataTable)dgvObs.DataSource;
                    if (dtObs != null)
                        SerializeDataTable(Data: dtObs, Container: (IDictionary<string, object>)dictPredictionElements[strMethod], Slot: "ObsData", Title: "Observations");

                    dgvStats.EndEdit();
                    dtStats = (DataTable)dgvStats.DataSource;
                    if (dtStats != null)
                        SerializeDataTable(Data: dtStats, Container: (IDictionary<string, object>)dictPredictionElements[strMethod], Slot: "StatData", Title: "Stats");

                    //Pack up the data transformations
                    PackTransformations(Container: (IDictionary<string, object>)dictPredictionElements[strMethod]);

                    if (ButtonStatusEvent != null)
                    {
                        IDictionary<string, bool> dictButtonStates = new Dictionary<string, bool>();
                        ButtonStatusEventArgs args = new ButtonStatusEventArgs(dictButtonStates);
                        //IDictionary<string, bool> 
                        ButtonStatusEvent(this, args);


                        ((IDictionary<string, object>)dictPredictionElements[strMethod])["PredictionButtonEnabled"] = dictButtonStates["PredictionButtonEnabled"];
                        ((IDictionary<string, object>)dictPredictionElements[strMethod])["ValidationButtonEnabled"] = dictButtonStates["ValidationButtonEnabled"];
                        //((IDictionary<string, object>)dictPredictionElements[strMethod])["ValidationButtonEnabled"] = btnMakePred.Enabled;
                    }
                }

                //Clear the column mappings
                IvMap = null;
                ObsMap = null;

                //clear the grids if another model's info has been used
                if (corrDT != null)
                {
                    this.dgvStats.DataSource = null;
                    this.dgvObs.DataSource = null;
                    this.dgvVariables.DataSource = null;
                }

                SetModel(strSelectedItem);
            //}
            //catch { }
        }


        private void PackTransformations(IDictionary<string, object> Container)
        {
            //Save the regulatory threshold as entered in the textbox
            if (Container.ContainsKey("RegulatoryThreshold"))
                Container.Remove("RegulatoryThreshold");
            Container.Add("RegulatoryThreshold", Convert.ToDouble(txtRegStd.Text));

            //Save the decision threshold as entered in the textbox
            if (Container.ContainsKey("DecisionThreshold"))
                Container.Remove("DecisionThreshold");
            Container.Add("DecisionThreshold", Convert.ToDouble(txtDecCrit.Text));
            
            //Transform for dtObs
            if (Container.ContainsKey("xfrmObs"))
                Container.Remove("xfrmObs");
            Container.Add("xfrmObs", xfrmObs);

            //Exponent for dtObs Power transformation
            if (Container.ContainsKey("xfrmObsExponent"))
                Container.Remove("xfrmObsExponent");
            Container.Add("xfrmObsExponent", dblObsPowerTransformExp);

            //Transform for regulatory criterion
            if (Container.ContainsKey("xfrmThreshold"))
                Container.Remove("xfrmThreshold");
            Container.Add("xfrmThreshold", xfrmThreshold);

            //Exponent for regulatory criterion Power transformation
            if (Container.ContainsKey("xfrmThresholdExponent"))
                Container.Remove("xfrmThresholdExponent");
            Container.Add("xfrmThresholdExponent", dblThresholdPowerTransformExp);
        }


        public void SetModel(string strModelPlugin)
        {
            IDictionary<string, object> dictPackedState = null;
            strMethod = strModelPlugin;

            //Load the interface that links us to the selected modeling plugin:
            foreach (Lazy<IModel, IDictionary<string, object>> module in models)
            {
                if (module.Metadata["PluginKey"].ToString() == strModelPlugin)
                {                    
                    model = module.Value;
                    dictPackedState = model.GetPackedState();
                }
            }

            if (dictPackedState != null)
            {
                //make sure empty model doesnt run through this method
                if (dictPackedState.Count <= 2)
                    return;

                Dictionary<string, object> dictModel = (Dictionary<string, object>)dictPackedState["Model"];
                //dictTransform = (Dictionary<string, object>)dictPackedState["Transform"];

                if (dictModel != null)
                {
                    Dictionary<string, object> dictPackedDatasheet = (Dictionary<string, object>)dictPackedState["PackedDatasheetState"];
                    strOutputVariable = dictPackedDatasheet["DepVarColName"].ToString();

                    //datatables serialized as xml string to maintain extendedProperty values
                    string strXmlDataTable = (string)dictPackedDatasheet["XmlDataTable"];
                    StringReader sr = new StringReader(strXmlDataTable);
                    DataSet ds = new DataSet();
                    ds.ReadXml(sr);
                    sr.Close();
                    corrDT = ds.Tables[0];

                    //unpack independent variables and text boxes
                    lstIndVars = (List<ListItem>)dictPackedState["Predictors"];

                    txtModel.Text = model.ModelString();

                    List<string> list = new List<string>();
                    list.Add(corrDT.Columns[0].ColumnName);
                    list.Add(corrDT.Columns[1].ColumnName);

                    int intNumVars = lstIndVars.Count;
                    for (int i = 0; i < intNumVars; i++)
                        list.Add(lstIndVars[i].ToString());

                    //Lets get all the main effect variables
                    dictMainEffects = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
                    for (int i = 1; i < corrDT.Columns.Count; i++)
                    {
                        bool bIsResponseVariable = (corrDT.Columns[i].ColumnName == strOutputVariable);
                        bool bMainEffect = Support.IsMainEffect(corrDT.Columns[i].ColumnName, corrDT);
                        if (bMainEffect && !bIsResponseVariable)
                        {
                            string strColName = corrDT.Columns[i].ColumnName;
                            dictMainEffects.Add(strColName, strColName);
                        }
                    }

                    //determine which transform type box to check
                    xfrmModeled = (DependentVariableTransforms)dictPackedState["xfrmThreshold"];
                    dblModeledPowerTransformExp = Convert.ToDouble(dictPackedState["ThresholdPowerTransformExponent"]);

                    xfrmImported = (DependentVariableTransforms)dictPackedState["xfrmImported"];
                    dblImportedPowerTransformExp = Convert.ToDouble(dictPackedState["ImportedPowerTransformExponent"]);
                                        
                    //format txtModel textbox
                    string strModelExpression = model.ModelString();
                    txtModel.Text = strModelExpression;

                    //Lets get only the main effects in the model
                    string[] strArrRefvars = ExpressionEvaluator.GetReferencedVariables(strModelExpression, dictMainEffects.Keys.ToArray());
                    List<string> lstRefVar = new List<string>();
                    lstRefVar.Add("ID");
                    lstRefVar.AddRange(strArrRefvars);
                    strArrReferencedVars = lstRefVar.ToArray();

                    //We may need to restore some previously used elements
                    if (!dictPredictionElements.ContainsKey(strModelPlugin))
                    {
                        dictPredictionElements.Add(strModelPlugin, new Dictionary<string, object>());

                        //Use the thresholds from the packed model object
                        txtDecCrit.Text = ((double)dictModel["DecisionThreshold"]).ToString();
                        txtRegStd.Text = ((double)dictModel["RegulatoryThreshold"]).ToString();

                        //Grab default transforms from the model.
                        xfrmThreshold = xfrmModeled;
                        dblThresholdPowerTransformExp = dblModeledPowerTransformExp;

                        xfrmObs = DependentVariableTransforms.none;
                        dblObsPowerTransformExp = 1;
                        SetTransformCheckmarks(Item: (int)xfrmObs);

                        //Use the transforms from the packed model object
                        txtPower.Text = dblThresholdPowerTransformExp.ToString();

                        //Initially, the validation and prediction buttons should be disabled.
                        if (ButtonStatusEvent != null)
                        {
                            IDictionary<string, bool> dictButtonStates = new Dictionary<string, bool>();
                            dictButtonStates["PredictionButtonEnabled"] = false;
                            dictButtonStates["ValidationButtonEnabled"] = false;

                            ButtonStatusEventArgs args = new ButtonStatusEventArgs(dictButtonStates, Set: true);
                            ButtonStatusEvent(this, args);
                        }
                    }
                    else
                    {
                        IDictionary<string, object> dictNewModel = (IDictionary<string, object>)dictPredictionElements[strModelPlugin];

                        if (dictNewModel.ContainsKey("ObsVariableMapping"))
                        {
                            ObsMap = new InputMapper((IDictionary<string, object>)(dictNewModel["ObsVariableMapping"]));
                        }

                        if (dictNewModel.ContainsKey("VariableMapping"))
                        {
                            IvMap = new InputMapper((IDictionary<string, object>)(dictNewModel["VariableMapping"]));
                        }

                        if (dictNewModel.ContainsKey("PredictionButtonEnabled") && dictNewModel.ContainsKey("ValidationButtonEnabled"))
                        {
                            if (ButtonStatusEvent != null)
                            {
                                IDictionary<string, bool> dictButtonStates = new Dictionary<string, bool>();
                                dictButtonStates["PredictionButtonEnabled"] = (bool)((IDictionary<string, object>)dictPredictionElements[strMethod])["PredictionButtonEnabled"];
                                dictButtonStates["ValidationButtonEnabled"] = (bool)((IDictionary<string, object>)dictPredictionElements[strMethod])["ValidationButtonEnabled"];

                                ButtonStatusEventArgs args = new ButtonStatusEventArgs(dictButtonStates, Set: true);
                                ButtonStatusEvent(this, args);
                            }
                        }

                        if (dictNewModel.ContainsKey("xfrmThreshold"))
                        {
                            //If these were saved in the prediction elements, great! Otherwise, grab defaults from the model.
                            xfrmThreshold = (DependentVariableTransforms)(Convert.ToInt32(dictNewModel["xfrmThreshold"]));
                            dblThresholdPowerTransformExp = Convert.ToDouble(dictNewModel["xfrmThresholdExponent"]);
                            txtRegStd.Text = ((double)dictNewModel["RegulatoryThreshold"]).ToString();
                            txtDecCrit.Text = ((double)dictNewModel["DecisionThreshold"]).ToString();
                        }
                        else
                        {
                            //Here we are grabbing defaults from the model.
                            xfrmThreshold = xfrmModeled;
                            dblThresholdPowerTransformExp = dblModeledPowerTransformExp;
                            txtDecCrit.Text = ((double)dictModel["DecisionThreshold"]).ToString();
                            txtRegStd.Text = ((double)dictModel["RegulatoryThreshold"]).ToString();
                        }

                        dtVariables = DeserializeDataTable(Container: dictNewModel, Slot: "IVData", Title: "Variables");
                        if (dtVariables != null)
                        {
                            dgvVariables.DataSource = dtVariables;
                            setViewOnGrid(dgvVariables);
                        }

                        //First, establish the default (will be overwritten if a version is found within the prediction elements)
                        xfrmObs = DependentVariableTransforms.none;
                        dblObsPowerTransformExp = 1;

                        dtObs = DeserializeDataTable(Container: dictNewModel, Slot: "ObsData", Title: "Observations");
                        if (dtObs != null)
                        {
                            dgvObs.DataSource = dtObs;
                            setViewOnGrid(dgvObs);

                            if (dictNewModel.ContainsKey("xfrmObs"))
                            {
                                //Use the thresholds and transforms from the prediction elements
                                xfrmObs = (DependentVariableTransforms)(Convert.ToInt32(dictNewModel["xfrmObs"]));
                                dblObsPowerTransformExp = Convert.ToDouble(dictNewModel["xfrmObsExponent"]);
                            }
                            else
                            {
                                //Here we are using the default (no transformation).
                                xfrmObs = DependentVariableTransforms.none;
                                dblObsPowerTransformExp = 1;
                            }
                        }
                        SetTransformCheckmarks(Item: (int)xfrmObs);


                        dtStats = DeserializeDataTable(Container: dictNewModel, Slot: "StatData", Title: "Stats");
                        if (dtStats != null)
                        {
                            dgvStats.DataSource = dtStats;
                            setViewOnGrid(dgvStats);
                        }
                    }

                    //determine which transform type box to check
                    txtPower.Text = dblThresholdPowerTransformExp.ToString();
                    if (xfrmThreshold == DependentVariableTransforms.none)
                        rbNone.Checked = true;
                    else if (xfrmThreshold == DependentVariableTransforms.Log10)
                        rbLog10.Checked = true;
                    else if (xfrmThreshold == DependentVariableTransforms.Ln)
                        rbLn.Checked = true;
                    else if (xfrmThreshold == DependentVariableTransforms.Power)
                    {
                        rbPower.Checked = true;                        
                    }
                    else
                        rbNone.Checked = true;
                }
            }
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
        public bool btnImportIVs_Click(object sender, EventArgs e)
        {
            bool boolNewMapping = false;

            //Get the currently existing data, if there is any.
            DataTable tblRaw = null;
            dtVariables = (DataTable)dgvVariables.DataSource;
            if (dtVariables != null)
            {
                if (dtVariables.Rows.Count >= 1)
                {
                    dgvVariables.EndEdit();
                    dtVariables.AcceptChanges();
                    tblRaw = dtVariables.AsDataView().ToTable();
                }
            }

            //check to ensure user chose a model first
            if (dictMainEffects == null)
            {
                MessageBox.Show("You must first pick a model from the Available Models");
                return(false);
            }

            if (IvMap == null)
            {
                IvMap = new InputMapper(dictMainEffects, "Model Variables", strArrReferencedVars, "Imported Variables");
                boolNewMapping = true;
            }

            DataTable dt = IvMap.ImportFile(tblRaw);
            if (dt == null)
                return (false);

            dgvVariables.DataSource = dt;

            foreach (DataGridViewColumn dvgCol in dgvVariables.Columns)
            {
                dvgCol.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            setViewOnGrid(dgvVariables);

            //Store the imported data in case we want to move to another modeling method
            dgvVariables.EndEdit();
            dtVariables = (DataTable)dgvVariables.DataSource;
            
            //Store prediction elements in case we want to navigate away from this model and then come back.
            SerializeDataTable(Data: dtVariables, Container: (IDictionary<string, object>)dictPredictionElements[strMethod], Slot: "IVData", Title: "Variables");
            if (boolNewMapping)
                ((IDictionary<string, object>)(dictPredictionElements[strMethod])).Add("VariableMapping", IvMap.PackState());

            return (true);
        }



        public void ClearDataGridViews(string Method)
        {
            if (((IDictionary<string, object>)dictPredictionElements).ContainsKey(Method))
            {
                SerializeDataTable(Data: null, Container: (IDictionary<string, object>)dictPredictionElements[Method], Slot: "IVData");
                SerializeDataTable(Data: null, Container: (IDictionary<string, object>)dictPredictionElements[Method], Slot: "ObsData");
                SerializeDataTable(Data: null, Container: (IDictionary<string, object>)dictPredictionElements[Method], Slot: "StatData");
            }

            //when changes made to modeling, clear the prediction tables (reset)
            if (Method == strMethod)
            {
                this.dgvStats.DataSource = null;
                this.dgvObs.DataSource = null;
                this.dgvVariables.DataSource = null;
                this.IvMap = null;
                this.ObsMap = null;
                txtModel.Text = "";
                txtDecCrit.Text = "";
            }            
        }


        public int ClearMethod(string Method)
        {
            //If there isn't a model from this plugin in the listBox, then we've got nothing to do.
            if (listModels.Contains(Method))
            {
                //Disconnect the selection-changed event handler while we work
                this.lbAvailableModels.SelectedIndexChanged -= new System.EventHandler(this.lbAvailableModels_SelectedIndexChanged);
                
                //Remove the Method from the lists
                listModels.Remove(Method);
                lbAvailableModels.Items.Remove(Method);
                if (dictPredictionElements.ContainsKey(Method))
                    dictPredictionElements.Remove(Method);

                //...Now reconnect the seleciton-changed event handler.
                this.lbAvailableModels.SelectedIndexChanged += new System.EventHandler(this.lbAvailableModels_SelectedIndexChanged);
            }
            return listModels.Count;
        }
        

        //Import OB datatable
        public void btnImportObs_Click(object sender, EventArgs e)
        {
            bool boolNewMapping = false;

            //Get the currently existing data, if there is any.
            DataTable tblRaw = null;
            dtVariables = (DataTable)dgvVariables.DataSource;
            if (dtVariables != null)
            {
                if (dtVariables.Rows.Count >= 1)
                {
                    dgvVariables.EndEdit();
                    dtVariables.AcceptChanges();
                    tblRaw = dtVariables.AsDataView().ToTable();
                }
            }

            //check to ensure user chose a model first
            if (dictMainEffects == null)
            {
                MessageBox.Show("You must first pick a model from the Available Models");
                return;
            }

            if (ObsMap == null)
            {
                string[] strArrObsColumns = { "ID", strOutputVariable };
                ObsMap = new InputMapper("Obs IDs", strArrObsColumns, "Obs");
                boolNewMapping = true;
            }

            DataTable dt = ObsMap.ImportFile(tblRaw);
            if (dt == null)
                return;

            dgvObs.DataSource = dt;

            foreach (DataGridViewColumn dvgCol in dgvObs.Columns)
            {
                dvgCol.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            setViewOnGrid(dgvObs);

            //Store the imported data in case we want to move to another modeling method
            dgvObs.EndEdit();
            dtObs = (DataTable)dgvObs.DataSource;
            SerializeDataTable(Data: dtObs, Container: (IDictionary<string, object>)dictPredictionElements[strMethod], Slot: "ObsData", Title: "Observations");
            if (boolNewMapping)
                ((IDictionary<string, object>)(dictPredictionElements[strMethod])).Add("ObsVariableMapping", ObsMap.PackState());
        }


        //make predictions based on imported ob and iv datatables
        public void btnMakePredictions_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            VBLogger.GetLogger().LogEvent("0", Globals.messageIntent.UserOnly, Globals.targetSStrip.ProgressBar);
            
            dtVariables = (DataTable)dgvVariables.DataSource;
            if (dtVariables == null)
                return;

            if (dtVariables.Rows.Count < 1)
                return;

            dgvVariables.EndEdit();
            dtVariables.AcceptChanges();
            dgvObs.EndEdit();      

            VBLogger.GetLogger().LogEvent("10", Globals.messageIntent.UserOnly, Globals.targetSStrip.ProgressBar);      
            dtObs = (DataTable)dgvObs.DataSource;
            if (dtObs != null)
                dtObs.AcceptChanges();

            DataTable tblRaw = dtVariables.AsDataView().ToTable();
            tblRaw.Columns.Remove("ID");
            List<int[]> lstBadCells = VBCommon.IO.ImportExport.GetBadCellsByRow(tblRaw, "");
            if (lstBadCells.Count > 0)
            {
                MessageBox.Show("There are errors in the data");
                return;
            }

            VBLogger.GetLogger().LogEvent("20", Globals.messageIntent.UserOnly, Globals.targetSStrip.ProgressBar);

            DataTable tblForPrediction = BuildPredictionTable(tblRaw, model.ModelString());

            VBLogger.GetLogger().LogEvent("30", Globals.messageIntent.UserOnly, Globals.targetSStrip.ProgressBar);
            

            Cursor.Current = Cursors.WaitCursor;
            VBLogger.GetLogger().LogEvent("40", Globals.messageIntent.UserOnly, Globals.targetSStrip.ProgressBar);


            VBLogger.GetLogger().LogEvent("50", Globals.messageIntent.UserOnly, Globals.targetSStrip.ProgressBar);

            //make prediction
            List<double> lstPredictions = model.Predict(tblForPrediction);

            VBLogger.GetLogger().LogEvent("60", Globals.messageIntent.UserOnly, Globals.targetSStrip.ProgressBar);
            Cursor.Current = Cursors.WaitCursor;

            //create prediction table to show prediction
            DataTable dtPredictions = new DataTable();
            dtPredictions.Columns.Add("ID", typeof(string));
            dtPredictions.Columns.Add("CalcValue", typeof(double));

            VBLogger.GetLogger().LogEvent("70", Globals.messageIntent.UserOnly, Globals.targetSStrip.ProgressBar);
            
            for (int i = 0; i < lstPredictions.Count; i++)
            {
                DataRow dr = dtPredictions.NewRow();
                dr["ID"] = dtVariables.Rows[i]["ID"].ToString();
                dr["CalcValue"] = lstPredictions[i];
                dtPredictions.Rows.Add(dr);
            }

            VBLogger.GetLogger().LogEvent("80", Globals.messageIntent.UserOnly, Globals.targetSStrip.ProgressBar);
            
            dtStats = GeneratePredStats(dtPredictions, dtObs, tblForPrediction);

            VBLogger.GetLogger().LogEvent("90", Globals.messageIntent.UserOnly, Globals.targetSStrip.ProgressBar);
            Cursor.Current = Cursors.WaitCursor;

             if (dtStats == null)
                 return;

            dgvStats.DataSource = dtStats;
            foreach (DataGridViewColumn dvgCol in dgvStats.Columns)
                dvgCol.SortMode = DataGridViewColumnSortMode.NotSortable;

            setViewOnGrid(dgvStats);

            //Store the predictions in case we want to move to another modeling method
            dgvStats.EndEdit();
            dtStats = (DataTable)dgvStats.DataSource;
            SerializeDataTable(Data: dtStats, Container: (IDictionary<string, object>)dictPredictionElements[strMethod], Slot: "StatData", Title: "Stats");

            VBLogger.GetLogger().LogEvent("100", Globals.messageIntent.UserOnly, Globals.targetSStrip.ProgressBar);            
        }


        private DataTable BuildPredictionTable(DataTable tblRaw, string strModelExpression)
        {
            DataTable tblForPrediction = new DataTable();


            string[] strExpressionChunks = strModelExpression.Split('+');
            string[] strExpressions = new string[strExpressionChunks.Count()];

            for (int k=0; k<strExpressionChunks.Count(); k++)
            {
                string var = strExpressionChunks[k];
                int intIndx;
                string strVariable = var.Trim();
                if ((intIndx = strVariable.IndexOf('(')) != -1)
                    if ((intIndx = strVariable.IndexOf(')', intIndx)) != -1)
                        intIndx = 0;
                strExpressions[k] = strVariable;
            }

            //Do any transformations/manipulations of the raw data.
            ExpressionEvaluator ee = new ExpressionEvaluator();
            tblForPrediction = ee.EvaluateTable(strExpressions, tblRaw);
            return tblForPrediction;
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
            //VBCommon.Transforms.DependentVariableTransforms dvt = GetTransformType();
            if (xfrmThreshold == VBCommon.Transforms.DependentVariableTransforms.Power)
            {
                if (ValidateNumericTextBox(txtPower) == false)
                    return null;
            }

            double dblCrit = Convert.ToDouble(txtDecCrit.Text);
            dblCrit = VBCommon.Transforms.Apply.UntransformThreshold(dblCrit, xfrmThreshold, dblThresholdPowerTransformExp);
            dblCrit = VBCommon.Transforms.Apply.TransformThreshold(dblCrit, xfrmImported, dblImportedPowerTransformExp);

            double regStd = Convert.ToDouble(txtRegStd.Text);
            regStd = VBCommon.Transforms.Apply.UntransformThreshold(regStd, xfrmThreshold, dblThresholdPowerTransformExp);
            regStd = VBCommon.Transforms.Apply.TransformThreshold(regStd, xfrmImported, dblImportedPowerTransformExp);

            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("Model_Prediction", typeof(double));
            dt.Columns.Add("Decision_Criterion", typeof(double));
            dt.Columns.Add("Exceedance_Probability", typeof(double));
            dt.Columns.Add("Regulatory_Standard", typeof(double));
            dt.Columns.Add("Error_Type", typeof(string));
            dt.Columns.Add("Untransformed", typeof(double));

            double dblPredValue = 0.0;
            string strId = "";

            List<double> lstExceedanceProbability = model.PredictExceedanceProbability(dtRaw);
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

                dr["Untransformed"] = VBCommon.Transforms.Apply.UntransformThreshold(dblPredValue, xfrmImported, dblImportedPowerTransformExp);

                /*if (dvt == VBCommon.Transforms.DependentVariableTransforms.Log10)
                    dr["Untransformed"] = Math.Pow(10, dblPredValue);
                else if (dvt == VBCommon.Transforms.DependentVariableTransforms.Ln)
                    dr["Untransformed"] = Math.Pow(Math.E, dblPredValue);
                else if (dvt == VBCommon.Transforms.DependentVariableTransforms.Power)
                {
                    double dblPower = (double)dictTransform["Exponent"];
                    if (dblPower == double.NaN)
                        dblPower = 1.0;

                    dr["Untransformed"] = Math.Sign(dblPredValue) * Math.Pow(Math.Abs(dblPredValue), (1.0 / dblPower));
                }
                else //No transform
                    dr["Untransformed"] = dblPredValue;*/

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

        /*//get the transform types for plotting
        private VBCommon.Transforms.DependentVariableTransforms GetTransformType()
        {
            VBCommon.Transforms.DependentVariableTransforms dvt = VBCommon.Transforms.DependentVariableTransforms.none;

            if (String.Compare(dictTransform["Type"].ToString(), VBCommon.Transforms.DependentVariableTransforms.Log10.ToString(), 0) == 0)
                dvt = VBCommon.Transforms.DependentVariableTransforms.Log10;
            else if (String.Compare(dictTransform["Type"].ToString(), VBCommon.Transforms.DependentVariableTransforms.Ln.ToString(), 0) == 0)
                dvt = VBCommon.Transforms.DependentVariableTransforms.Ln;
            else if (String.Compare(dictTransform["Type"].ToString(), VBCommon.Transforms.DependentVariableTransforms.Power.ToString(), 0) == 0)
                dvt = VBCommon.Transforms.DependentVariableTransforms.Power;

            return dvt;
        }

        private double UntransformThreshold(double Value, VBCommon.Transforms.DependentVariableTransforms Transform, double Exponent = 1)
        {
            if (Transform == VBCommon.Transforms.DependentVariableTransforms.none)
                return Value;
            else if (Transform == VBCommon.Transforms.DependentVariableTransforms.Log10)
                return Math.Pow(10, Value);
            else if (Transform == VBCommon.Transforms.DependentVariableTransforms.Ln)
                return Math.Exp(Value);
            else if (Transform == VBCommon.Transforms.DependentVariableTransforms.Power)
                return Math.Sign(Value) * Math.Pow(Math.Abs(Value), 1 / Exponent);
            else
                return Value;
        }


        private double TransformThreshold(double Value, VBCommon.Transforms.DependentVariableTransforms Transform, double Exponent = 1)
        {
            if (Transform == VBCommon.Transforms.DependentVariableTransforms.none)
                return Value;
            else if (Transform == VBCommon.Transforms.DependentVariableTransforms.Log10)
                return Math.Log10(Value);
            else if (Transform == VBCommon.Transforms.DependentVariableTransforms.Ln)
                return Math.Log(Value);
            else if (Transform == VBCommon.Transforms.DependentVariableTransforms.Power)
                return Math.Pow(Math.Abs(Value), Exponent);
            else
                return Value;
        }*/


        private void splitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {
            int left1 = splitContainer1.Panel2.Left;
            int left2 = splitContainer2.Panel2.Left;
        }

        
        //load the prediction form
        private void frmIPyPrediction_Load(object sender, EventArgs e)
        {
            cmforResponseVar.MenuItems.Add("Define Transform:");
            cmforResponseVar.MenuItems[0].MenuItems.Add("none", new EventHandler(DefineTransformForRV));
            cmforResponseVar.MenuItems[0].MenuItems.Add("Log10", new EventHandler(DefineTransformForRV));
            cmforResponseVar.MenuItems[0].MenuItems.Add("Ln", new EventHandler(DefineTransformForRV));
            cmforResponseVar.MenuItems[0].MenuItems.Add("Power", new EventHandler(DefineTransformForRV));
            //cmforResponseVar.MenuItems.Add("Untransform", new EventHandler(Untransform));

            //Request that the prediction plugin pass along its list of modeling plugins.
            if (RequestModelPluginList != null)
            {
                EventArgs args = new EventArgs();
                RequestModelPluginList(this, args);
            }
        }


        public void DefineTransformForRV(object o, EventArgs e)
        {
            //menu response from right click, determine which transform was selected
            MenuItem mi = (MenuItem)o;
            string transform = mi.Text;
            if (transform == VBCommon.Transforms.DependentVariableTransforms.Power.ToString())
            {
                frmPowerExponent frmExp = new frmPowerExponent(dtObs, 1);
                DialogResult dlgr = frmExp.ShowDialog();
                if (dlgr != DialogResult.Cancel)
                {
                    string sexp = frmExp.Exponent.ToString("n2");
                    transform += "," + sexp;
                    xfrmObs = VBCommon.Transforms.DependentVariableTransforms.Power;
                    dblObsPowerTransformExp = Convert.ToDouble(sexp);
                    dtObs.Columns[1].ExtendedProperties[VBCommon.Globals.DEPENDENTVARIBLEDEFINEDTRANSFORM] = transform;
                    SetTransformCheckmarks(Item: 3);
                    //state = dtState.dirty;
                    //NotifyContainer();
                }
            }
            else
            {
                if (String.Compare(transform, "Log10", true) == 0)
                {
                    xfrmObs = VBCommon.Transforms.DependentVariableTransforms.Log10;
                    SetTransformCheckmarks(Item: 1);
                }
                else if (String.Compare(transform, "Ln", true) == 0)
                {
                    xfrmObs = VBCommon.Transforms.DependentVariableTransforms.Ln;
                    SetTransformCheckmarks(Item: 2);
                }
                else if (String.Compare(transform, "none", true) == 0)
                {
                    xfrmObs = VBCommon.Transforms.DependentVariableTransforms.none;
                    SetTransformCheckmarks(Item: 0);
                }

                dtObs.Columns[1].ExtendedProperties[VBCommon.Globals.DEPENDENTVARIBLEDEFINEDTRANSFORM] = transform;
                //state = dtState.dirty;
                //NotifyContainer();
            }
        }


        private void SetTransformCheckmarks(int Item)
        {
            int i;
            intCheckedRVTransform = Item;

            //Handle the defined transforms' menu:
            for (i = 0; i < 4; i++)
            {
                if (Item == i)
                    cmforResponseVar.MenuItems[0].MenuItems[i].Checked = true;
                else
                    cmforResponseVar.MenuItems[0].MenuItems[i].Checked = false;
            }
        }


        /*private void Untransform(object o, EventArgs e)
        {
            if (dtObsOrig != null)
            {
                DataColumn dc = dtObsOrig.Columns[1];
                dc.ColumnName = "Observation";
                dgvObs.DataSource = dtObsOrig;
                boolObsTransformed = false;

                foreach (MenuItem item in cmforResponseVar.MenuItems[0].MenuItems) { item.Checked = false; }
            }
        }*/


        /*/// <summary>
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

            if (cmforResponseVar.MenuItems[0].MenuItems.ContainsKey("Log10"))
            {
                foreach (MenuItem item in cmforResponseVar.MenuItems[0].MenuItems) { item.Checked = false; }
                cmforResponseVar.MenuItems[0].MenuItems["Log10"].Checked = true;
            }
        }


        // response variable transform LnT
        private void LnT(object o, EventArgs e)
        {                      
            //cmforResponseVar.
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

            if (cmforResponseVar.MenuItems[0].MenuItems.ContainsKey("Ln"))
            {
                foreach (MenuItem item in cmforResponseVar.MenuItems[0].MenuItems) { item.Checked = false; }
                cmforResponseVar.MenuItems[0].MenuItems["Ln"].Checked = true;
            }
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

                if (cmforResponseVar.MenuItems[0].MenuItems.ContainsKey("Power"))
                {
                    foreach (MenuItem item in cmforResponseVar.MenuItems[0].MenuItems) { item.Checked = false; }                        
                    cmforResponseVar.MenuItems[0].MenuItems["Power"].Checked = true;
                }
            }
        }*/
        

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

            IvMap = null;
            ObsMap = null;
            dictPredictionElements = new Dictionary<string, object>(); ;
            lbAvailableModels.ClearSelected();

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
            frmPredictionScatterPlot frmPlot = new frmPredictionScatterPlot(dtObs, dtStats, ObservationColumn:strOutputVariable);
            frmPlot.Show();
            //get the transform for plotting
            Int32 intTransform = Convert.ToInt32(xfrmObs);
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
            var exp = dblThresholdPowerTransformExp;
            double exponent = Convert.ToDouble(exp);
            //configure the plot display
            frmPlot.ConfigureDisplay(decisionThreshold:Convert.ToDouble(txtDecCrit.Text), regulatoryThreshold:Convert.ToDouble(txtRegStd.Text), ObservationTransform:xfrmObs, ObservationExponent:dblObsPowerTransformExp, PredictionTransform:xfrmImported, PredictionExponent:dblImportedPowerTransformExp, ThresholdTransformation:xfrmThreshold, ThresholdExponent:dblThresholdPowerTransformExp);
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


        private void rbNone_CheckedChanged(object sender, EventArgs e)
        {
            if (rbNone.Checked)
            {
                xfrmThreshold = DependentVariableTransforms.none;
                dblThresholdPowerTransformExp = Double.NaN;
            }
        }


        private void rbLog10_CheckedChanged(object sender, EventArgs e)
        {
            if (rbLog10.Checked)
            {
                xfrmThreshold = DependentVariableTransforms.Log10;
                dblThresholdPowerTransformExp = Double.NaN;
            }
        }


        private void rbLn_CheckedChanged(object sender, EventArgs e)
        {
            if (rbLn.Checked)
            {
                xfrmThreshold = DependentVariableTransforms.Ln;
                dblThresholdPowerTransformExp = Double.NaN;
            }
        }


        private void rbPower_CheckedChanged(object sender, EventArgs e)
        {
            if (rbPower.Checked)
            {
                frmPowerExponent frmExp = new frmPowerExponent(dtObs, 1);
                DialogResult dlgr = frmExp.ShowDialog();
                if (dlgr != DialogResult.Cancel)
                {
                    string sexp = frmExp.Exponent.ToString("n2");
                    xfrmThreshold = DependentVariableTransforms.Power;
                    dblThresholdPowerTransformExp = Convert.ToDouble(sexp);
                    txtPower.Text = sexp.ToString();
                    //state = dtState.dirty;
                    //NotifyContainer();
                }
                //txtPower.Enabled = true;
            }
            //else
                //txtPower.Enabled = false;
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


        //validate imported datatable
        public bool btnIVDataValidation_Click(object sender, EventArgs e)
        {
            //check for non unique records, blank records
            DataTable dt = dgvVariables.DataSource as DataTable;
            if ((dt == null) ||(dt.Rows.Count < 1))
                return(false);

            DataTable dtCopy = dt.Copy();
            DataTable dtSaved = dt.Copy();
            frmMissingValues frmMissVal = new frmMissingValues(dgvVariables, dtCopy);
            frmMissVal.ShowDialog();
            if (frmMissVal.Status)
            {
                int errndx;
                if (!InputMapper.RecordIndexUnique(frmMissVal.ValidatedDT, out errndx))
                {
                    MessageBox.Show("Unable to process datasets with non-unique record identifiers.\n" +
                                    "Fix your datatable by assuring unique record identifier values\n" +
                                    "in the ID column and try validating again.\n\n" +
                                    "Record Identifier values cannot be blank or duplicated;\nencountered " +
                                    "error near row " + errndx.ToString(), "Data Validation Error - Cannot Process This Dataset", MessageBoxButtons.OK);
                    return(false);
                }
                dgvVariables.DataSource = frmMissVal.ValidatedDT;
                //btnMakePredictions.Enabled = true;
                return (true);
            }
            else
            {
                dgvVariables.DataSource = dtSaved;
                return (false);
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
                    /*if (!boolObsTransformed)
                    { 
                        //we can transform a response variable
                        cmforResponseVar.MenuItems[0].Enabled = true;
                        //but we cannot untransform an untransformed variable
                        //cmforResponseVar.MenuItems[1].Enabled = false;
                    }
                    else
                    {
                        //but we cannot transform a transformed response
                        cmforResponseVar.MenuItems[0].Enabled = false; 
                        //but we can untransform a transformed response
                        //cmforResponseVar.MenuItems[1].Enabled = true;
                    }*/
                    cmforResponseVar.Show(dgv, new Point(me.X, me.Y));
                }
            }
        }


        private void dgvVariables_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //If user has edited the ID column, make sure the IDs are still unique.
            //btnMakePredictions.Enabled = false;
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


        private void SerializeDataTable(DataTable Data, IDictionary<string, object> Container, string Slot, string Title=null)
        {
            if (Title == null)
                Title = Slot;

            string strSerializedDataTable = null;

            if (Data != null)
            {
                Data.AcceptChanges();
                Data.TableName = Title;
                StringWriter sw = new StringWriter();
                Data.WriteXml(sw, XmlWriteMode.WriteSchema, false);
                strSerializedDataTable = sw.ToString();
                sw.Close();
            }

            //If there's already observations packed up for this modeling method, remove them before adding the new stuff.
            if (Container.ContainsKey(Slot))
                Container.Remove(Slot);
            Container.Add(Slot, strSerializedDataTable);
        }


        private DataTable DeserializeDataTable(IDictionary<string, object> Container, string Slot, string Title = null)
        {
            DataTable tblData = null;

            if (Title == null)
                Title = Slot;

            if (Container.ContainsKey(Slot))
            {
                if (Container[Slot] != null)
                {
                    string strPackedDataTable = Container[Slot].ToString();

                    if (!String.IsNullOrWhiteSpace(strPackedDataTable))
                    {
                        DataSet ds = new DataSet();
                        ds.ReadXml(new StringReader(strPackedDataTable), XmlReadMode.ReadSchema);
                        tblData = ds.Tables[0];
                    }
                }
            }
            return tblData;
        }
    }
}
