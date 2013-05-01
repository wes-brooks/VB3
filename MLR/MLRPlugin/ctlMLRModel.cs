using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using GALibForm;
using VBCommon;
using VBCommon.Controls;

namespace MLRPlugin
{
    public partial class ctlMLRModel : UserControl
    {
        public static ctlMLRModel MLRControl = null;
        private bool bAllowNotifiableDataEvent = true;

        public ctlMLRModel()
        {
            InitializeComponent();

            MLRControl = this;

            dsControl1.NotifiableChangeEvent +=new EventHandler(dsControl1_NotifiableChangeEvent);
            //frmModel1.VariablesChanged += new EventHandler(ctlVariableSelection1_UpdateVariableList);
            //dsControl1.NotifiableChangeEvent += new EventHandler(dsControl1_NotifiableChangeEvent);
            //frmModel1.ClearList += new EventHandler(frmModel1_ClearList);
            //frmModel1.AddToList += new frmModel.MyEventHandler(frmModel1_AddToList);
        }


        public TabControl ModelingTabControl
        {
            get { return tabControl1; }
        }


        //Return a flag indicating whether the plugin is ready to export a completed model
        public bool Complete
        {
            get { return frmModel1.ModelingComplete; }
        }


        public bool AllowNotifiableDataEvent
        {
            get { return bAllowNotifiableDataEvent; }
            set { bAllowNotifiableDataEvent = value; }
        }


        public void btnComputeAO_Click(object sender, EventArgs e)
        {
            dsControl1.btnComputeAO_Click(sender, e);
        }


        public void btnManipulate_Click(object sender, EventArgs e)
        {
            dsControl1.btnManipulate_Click(sender, e);
        }


        public void btnTransform_Click(object sender, EventArgs e)
        {
            dsControl1.btnTransform_Click(sender, e);
        }


        /*private void frmModel1_ClearList(object sender, EventArgs e)
        {
            ctlVariableSelection1.ClearAll();
        }


        private void frmModel1_AddToList(frmModel.MyEventArg e)
        {
            ctlVariableSelection1.AddToList(e.ModelVars);
        }*/


        //Handle changes in the local datasheet control
        private void dsControl1_NotifiableChangeEvent(object source, EventArgs e)
        {
            if (!bAllowNotifiableDataEvent)
                return;

            /*ctlVariableSelection1.SetData(dsControl1.DT.Copy());
            List<string> lstSelectedVariables = ctlVariableSelection1.SelectedVariables.ToList();
            MLRCore.MLRDataManager _dataMgr = MLRCore.MLRDataManager.GetDataManager();
            _dataMgr.ModelDataTable = dsControl1.DT.Copy();
            //frmModel1.ClearModelingTab();
            frmModel1.InitControls();
            frmModel1.SetData();
            frmModel1.SelectedVariables = lstSelectedVariables;*/

            
            MLRCore.MLRDataManager _dataMgr = MLRCore.MLRDataManager.GetDataManager();
            _dataMgr.ModelDataTable = dsControl1.DT.Copy();
            //frmModel1.ClearModelingTab();
            frmModel1.InitControls();
            frmModel1.SetData();
        }


        /*//Handle changes in the variable selection control
        private void ctlVariableSelection1_SelectionChanged(object sender, EventArgs e)
        {
            //Make a copy of the list
            List<string> lstSelectedVariables = ctlVariableSelection1.SelectedVariables.ToList();
            MLRCore.MLRDataManager _dataMgr = MLRCore.MLRDataManager.GetDataManager();
            _dataMgr.ModelDataTable = dsControl1.DT.Copy();
            frmModel1.InitControls();
            frmModel1.SetData();
            frmModel1.SelectedVariables = lstSelectedVariables;
        }*/


        /*private void ctlVariableSelection1_UpdateVariableList(object sender, EventArgs e)
        {
            List<string> lstSelectedVariables = ctlVariableSelection1.SelectedVariables.ToList();
            //frmModel1.SelectedVariables = lstSelectedVariables;
        }*/


        public GALibForm.frmModel ModelForm
        {
            get { return this.frmModel1; }
        }


        public DatasheetControl LocalDatasheet
        {
            get { return this.dsControl1; }
        }


        /// <summary>
        /// Pack the plugin state
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, object> PackProjectState()
        {
            Dictionary<string, object> pluginState = new Dictionary<string, object>();
            DataTable dt = dsControl1.DT;

            if (dt != null)
            {
                StringWriter sw = null;
                sw = new StringWriter();
                dt.WriteXml(sw, XmlWriteMode.WriteSchema, false);
                string xmlDataTable = sw.ToString();
                sw.Close();
                sw = null;
                pluginState.Add("CorrelationDataTable", xmlDataTable);
            }

            pluginState.Add("PackedDatasheetState", dsControl1.PackState());
            //pluginState.Add("VarSelectionState", ctlVariableSelection1.PackState());
            pluginState.Add("ActiveTab", tabControl1.SelectedIndex);
            pluginState.Add("Model", frmModel1.PackProjectState());

            if (frmModel1.ModelInfo != null)
            {
                Dictionary<string, object> dictTransform = new Dictionary<string, object>();
                dictTransform.Add("Type", frmModel1.ModelInfo.DependentVariableTransform);
                dictTransform.Add("Exponent", frmModel1.ModelInfo.DependentVariablePowerTransformExponent);
                pluginState.Add("Transform", dictTransform);

                pluginState.Add("Predictors", frmModel1.ModelInfo.IndependentVariables.ToList());

                pluginState.Add("xfrmThreshold", frmModel1.ModelInfo.ThresholdTransform);
                pluginState.Add("ThresholdPowerTransformExponent", frmModel1.ModelInfo.ThresholdPowerTransformExponent);

                pluginState.Add("xfrmImported", frmModel1.ModelInfo.DependentVariableTransform);
                pluginState.Add("ImportedPowerTransformExponent", frmModel1.ModelInfo.DependentVariablePowerTransformExponent);
            }

            pluginState.Add("Method", "MLR");

            return pluginState;
        }


        public void UnpackProjectState(IDictionary<string, object> dictProjectState)
        {
            //Something went wrong
            if (dictProjectState == null || dictProjectState.Count < 1)
                return;

            if (dictProjectState.ContainsKey("PackedDatasheetState"))            
                dsControl1.UnpackState(dictProjectState["PackedDatasheetState"] as IDictionary<string, object>);
            
            /*if (dictProjectState.ContainsKey("VarSelectionState"))
            {
                ctlVariableSelection1.UnpackState(dictProjectState["VarSelectionState"] as Dictionary<string, List<ListItem>>);
                frmModel1.SelectedVariables = ctlVariableSelection1.SelectedVariables.ToList();
            }*/

            MLRCore.MLRDataManager _dataMgr = MLRCore.MLRDataManager.GetDataManager();
            _dataMgr.ModelDataTable = dsControl1.DT.Copy();

            frmModel1.InitControls();
            frmModel1.SetData();
            
            if (dictProjectState.ContainsKey("Model"))
                frmModel1.UnpackProjectState(dictProjectState["Model"] as IDictionary<string, object>);

            if (dictProjectState.ContainsKey("ActiveTab"))
                tabControl1.SelectedIndex = (int)(dictProjectState["ActiveTab"]); //model
    
            this.Show();
        }
    }
}
