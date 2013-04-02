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

        public ctlMLRModel()
        {
            InitializeComponent();

            MLRControl = this;

            dsControl1.NotifiableChangeEvent +=new EventHandler(dsControl1_NotifiableChangeEvent);
            ctlVariableSelection1.VariablesChanged += new EventHandler(ctlVariableSelection1_UpdateVariableList);
            frmModel1.ClearList += new EventHandler(frmModel1_ClearList);
            frmModel1.AddToList += new frmModel.MyEventHandler(frmModel1_AddToList);
        }


        private void frmModel1_ClearList(object sender, EventArgs e)
        {
            ctlVariableSelection1.ClearAll();
        }


        private void frmModel1_AddToList(frmModel.MyEventArg e)
        {
            ctlVariableSelection1.AddToList(e.ModelVars);
        }


        //Handle changes in the local datasheet control
        private void dsControl1_NotifiableChangeEvent(object source, EventArgs e)
        {
            ctlVariableSelection1.SetData(dsControl1.DT.Copy());
            List<string> lstSelectedVariables = ctlVariableSelection1.SelectedVariables.ToList();
            MLRCore.MLRDataManager _dataMgr = MLRCore.MLRDataManager.GetDataManager();
            _dataMgr.ModelDataTable = dsControl1.DT.Copy();
            frmModel1.InitControls();
            frmModel1.SetData();
            frmModel1.SelectedVariables = lstSelectedVariables;
        }


        //Handle changes in the variable selection control
        private void ctlVariableSelection1_SelectionChanged(object sender, EventArgs e)
        {
            //Make a copy of the list
            List<string> lstSelectedVariables = ctlVariableSelection1.SelectedVariables.ToList();
            MLRCore.MLRDataManager _dataMgr = MLRCore.MLRDataManager.GetDataManager();
            _dataMgr.ModelDataTable = dsControl1.DT.Copy();
            frmModel1.InitControls();
            frmModel1.SetData();
            frmModel1.SelectedVariables = lstSelectedVariables;
        }


        private void ctlVariableSelection1_UpdateVariableList(object sender, EventArgs e)
        {
            List<string> lstSelectedVariables = ctlVariableSelection1.SelectedVariables.ToList();
            frmModel1.SelectedVariables = lstSelectedVariables;
        }


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
            pluginState.Add("VarSelectionState", ctlVariableSelection1.PackState());
            //pluginState.Add("MLRModelState", ModelForm.PackProjectState()); 
            pluginState.Add("Model", ModelForm.PackProjectState());

            if (ModelForm.ModelInfo != null)
            {
                Dictionary<string, object> dictTransform = new Dictionary<string, object>();
                //dictTransform.Add("Type", ModelForm.ModelInfo.ThresholdTransform);
                dictTransform.Add("Type", ModelForm.ModelInfo.DependentVariableTransform);
                dictTransform.Add("Exponent", ModelForm.ModelInfo.PowerTransformExponent);
                pluginState.Add("Transform", dictTransform);

                pluginState.Add("Predictors", ModelForm.ModelInfo.IndependentVariables.ToList());

                pluginState.Add("xfrmThreshold", ModelForm.ModelInfo.DependentVariableTransform);
                //pluginState.Add("xfrmThreshold", VBCommon.Globals.DEPENDENTVARIBLEDEFINEDTRANSFORM);
                pluginState.Add("ThresholdPowerTransformExponent", ModelForm.ModelInfo.PowerTransformExponent);

                pluginState.Add("xfrmImported", ModelForm.ModelInfo.DependentVariableTransform);
                pluginState.Add("ImportedPowerTransformExponent", ModelForm.ModelInfo.PowerTransformExponent);
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


            if (dictProjectState.ContainsKey("VarSelectionState"))
            {
                ctlVariableSelection1.UnpackState(dictProjectState["VarSelectionState"] as Dictionary<string, List<ListItem>>);
                frmModel1.SelectedVariables = ctlVariableSelection1.SelectedVariables.ToList();
            }
            //else
            //{
            //    ctlVariableSelection1_SelectionChanged();
            //}
            MLRCore.MLRDataManager _dataMgr = MLRCore.MLRDataManager.GetDataManager();
            _dataMgr.ModelDataTable = dsControl1.DT.Copy();
            //ctlVariableSelection1.SetData(dsControl1.DT.Copy());
            frmModel1.InitControls();
            frmModel1.SetData();
            //ModelForm.SetDataTable(dsControl1.DT.Copy());
            //ModelForm.SetData(dsControl1.DT.Copy());

            //if (dictProjectState.ContainsKey("MLRModelState"))
            //{
            //    ModelForm.UnpackProjectState(dictProjectState["MLRModelState"] as IDictionary<string, object>);
            //}
            if (dictProjectState.ContainsKey("Model"))
            {
                ModelForm.UnpackProjectState(dictProjectState["Model"] as IDictionary<string, object>);
            }
            
            this.Show();
        }
    }
}
