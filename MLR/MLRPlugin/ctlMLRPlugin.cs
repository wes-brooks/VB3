using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using VBCommon.Controls;

namespace MLRPlugin
{
    public partial class ctlMLRPlugin : UserControl
    {
        public static ctlMLRPlugin MLRControl = null;
        public ctlMLRPlugin()
        {
            InitializeComponent();
            MLRControl = this;
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
            
            pluginState.Add("Model", ModelForm.PackProjectState());

            if (ModelForm.ModelInfo != null)
            {
                Dictionary<string, object> dictTransform = new Dictionary<string, object>();
                dictTransform.Add("Type", ModelForm.ModelInfo.DependentVariableTransform);
                dictTransform.Add("Exponent", ModelForm.ModelInfo.PowerTransformExponent);
                pluginState.Add("Transform", dictTransform);

                pluginState.Add("Predictors",ModelForm.ModelInfo.IndependentVariables.ToList());                
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
            {
                dsControl1.UnpackState(dictProjectState["PackedDatasheetState"] as IDictionary<string, object>);
            }

            MLRCore.MLRDataManager _dataMgr = MLRCore.MLRDataManager.GetDataManager();
            _dataMgr.ModelDataTable = dsControl1.DT.Copy();
            //ModelForm.SetDataTable(dsControl1.DT.Copy());
            ModelForm.SetData(dsControl1.DT.Copy());

            if (dictProjectState.ContainsKey("MLRModelState"))
            {                
                ModelForm.UnpackProjectState(dictProjectState["MLRModelState"] as IDictionary<string, object>);
            }


            this.Show();

        }
    }
}
