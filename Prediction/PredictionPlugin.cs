using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using DotSpatial.Controls;
using System.ComponentModel.Composition;
using DotSpatial.Controls.Header;
using DotSpatial.Controls.Docking;
using VBCommon;
using VBCommon.PluginSupport;
using VBCommon.Interfaces;


namespace Prediction
{
    public class PredictionPlugin : Extension, IPartImportsSatisfiedNotification, IPlugin
    {                      
        [Import("Shell")]
        private ContainerControl Shell { get; set; }

        //instance of  class
        private frmPrediction _frmPred;
        private Globals.PluginType pluginType = VBCommon.Globals.PluginType.Prediction;
        
        private VBCommon.Signaller signaller;
        private const string strPanelKey = "Prediction";
        private const string strPanelCaption = "Prediction";
        
        //ribbon buttons
        private SimpleActionItem btnImportIV;
        private SimpleActionItem btnIVDataVal;
        private SimpleActionItem btnMakePred;
        private SimpleActionItem btnImportOB;
        private SimpleActionItem btnPlot;
        private SimpleActionItem btnClear;
        private SimpleActionItem btnExportCSV;
        private SimpleActionItem btnSetEnddatURL;
        private SimpleActionItem btnImportFromEnddat;

        private RootItem rootIPyPredictionTab;
        
        //complete and visible flags
        public Boolean boolComplete = false;
        public Boolean boolVisible = true;
        public Boolean boolChanged = false;

        //this plugin was clicked
        private string strTopPlugin = string.Empty;

        private Stack<string> UndoKeys;
        private Stack<string> RedoKeys;
        private string strUndoRedoKey;

        //Raise a message
        public delegate void MessageHandler<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
        public event MessageHandler<VBCommon.PluginSupport.MessageArgs> MessageSent;

        
        //deactivate this plugin
        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            App.DockManager.Remove(strPanelKey);
            _frmPred = null;
            base.Deactivate();
        }


        public void Hide()
        {
            if (boolVisible)
            {
                boolChanged = true;

                App.HeaderControl.RemoveAll();
                App.DockManager.HidePanel(strPanelKey);
            }
            boolVisible = false;
        }


        public void Show()
        {
            if (!boolVisible)
            {
                boolChanged = true;

                AddRibbon("Show");
                App.DockManager.SelectPanel(strPanelKey);
                App.HeaderControl.SelectRoot(strPanelKey);
            }
            boolVisible = true;
        }


        public void MakeActive()
        {
            App.DockManager.SelectPanel(strPanelKey);
            App.HeaderControl.SelectRoot(strPanelKey);
        }


        //initial activation
        public override void Activate()
        {
            _frmPred = new frmPrediction();

            UndoKeys = new Stack<string>();
            RedoKeys = new Stack<string>();
            
            AddPanel();
            AddRibbon("Activate");
            
            //when panel is selected activate seriesview and ribbon tab
            App.DockManager.ActivePanelChanged += new EventHandler<DotSpatial.Controls.Docking.DockablePanelEventArgs>(DockManager_ActivePanelChanged);
            App.HeaderControl.RootItemSelected += new EventHandler<RootItemEventArgs>(HeaderControl_RootItemSelected);
            _frmPred.ButtonStatusEvent += new EventHandler<ButtonStatusEventArgs>(_frmPred_ButtonStatusEvent);
            _frmPred.ControlChangeEvent += new EventHandler(ControlChangeEventHandler);
            _frmPred.RequestModelPluginList += new EventHandler(PassModelPluginList);
            _frmPred.NotifiableChangeEvent += new EventHandler(NotifiableChangeHandler);
            
            boolVisible = true;
            boolComplete = false;

            base.Activate();
            Hide();
        }


        //a root item (plugin) has been selected
        void HeaderControl_RootItemSelected(object sender, RootItemEventArgs e)
        {
            if (e.SelectedRootKey == strPanelKey)
            {
                App.DockManager.SelectPanel(strPanelKey);
            }
        }


        //add a root item
        public void AddRibbon(string sender)
        {
            rootIPyPredictionTab = new RootItem(strPanelKey, strPanelCaption);
            rootIPyPredictionTab.SortOrder = (short)pluginType;
            App.HeaderControl.Add(rootIPyPredictionTab);
            App.HeaderControl.SelectRoot(strPanelKey);

            //add sub-ribbons
            const string rGroupCaption = "Predict";
            
            btnImportIV = new SimpleActionItem(strPanelKey, "Import Data", btnImportIV_Click);
            btnImportIV.LargeImage = Properties.Resources.ImportIV;
            btnImportIV.GroupCaption = rGroupCaption;
            btnImportIV.Enabled = true;
            App.HeaderControl.Add(btnImportIV);

            btnImportOB = new SimpleActionItem(strPanelKey, "Import Lab Measurements", btnImportOB_Click);
            btnImportOB.LargeImage = Properties.Resources.ImportOB;
            btnImportOB.GroupCaption = rGroupCaption;
            btnImportOB.Enabled = true;
            App.HeaderControl.Add(btnImportOB);
            
            btnIVDataVal = new SimpleActionItem(strPanelKey, "Data Validation", btnIVDataVal_Click);
            btnIVDataVal.LargeImage = Properties.Resources.IVDataVal;
            btnIVDataVal.GroupCaption = rGroupCaption;
            btnIVDataVal.Enabled = false;
            App.HeaderControl.Add(btnIVDataVal);

            btnMakePred = new SimpleActionItem(strPanelKey, "Make Predicitons", btnMakePrediction_Click);
            btnMakePred.LargeImage = Properties.Resources.MakePrediction;
            btnMakePred.GroupCaption = rGroupCaption;
            btnMakePred.Enabled = false;
            App.HeaderControl.Add(btnMakePred);

            //extra
            const string sGroupCaption = "Manipulate";

            btnPlot = new SimpleActionItem(strPanelKey, "Plot", btnPlot_Ck);
            btnPlot.LargeImage = Properties.Resources.Plot;
            btnPlot.GroupCaption = sGroupCaption;
            btnPlot.Enabled = true;
            App.HeaderControl.Add(btnPlot);

            btnClear = new SimpleActionItem(strPanelKey, "Clear", btnClear_Click);
            btnClear.LargeImage = Properties.Resources.Clear;
            btnClear.GroupCaption = sGroupCaption;
            btnClear.Enabled = true;
            App.HeaderControl.Add(btnClear);

            btnExportCSV = new SimpleActionItem(strPanelKey, "Export As CSV", btnExportTable_Ck);
            btnExportCSV.LargeImage = Properties.Resources.ExportAsCSV;
            btnExportCSV.GroupCaption = sGroupCaption;
            btnExportCSV.Enabled = true;
            App.HeaderControl.Add(btnExportCSV);

            //EnDDAT
            const string strEnddatCaption = "EnDDaT";

            btnSetEnddatURL = new SimpleActionItem(strPanelKey, "Set EnDDaT Data Source", btnSetEnddatURL_Click);
            btnSetEnddatURL.LargeImage = Properties.Resources.URL;
            btnSetEnddatURL.GroupCaption = strEnddatCaption;
            btnSetEnddatURL.Enabled = true;
            App.HeaderControl.Add(btnSetEnddatURL);

            btnImportFromEnddat = new SimpleActionItem(strPanelKey, "Import From EnDDaT", btnImportFromEnddat_Click);
            btnImportFromEnddat.LargeImage = Properties.Resources.ImportIV;
            btnImportFromEnddat.GroupCaption = strEnddatCaption;
            btnImportFromEnddat.Enabled = false;
            App.HeaderControl.Add(btnImportFromEnddat);
        }


        //add the panel
        public void AddPanel()
        {
            var dp = new DockablePanel(strPanelKey, strPanelCaption, _frmPred, DockStyle.Fill);
            dp.DefaultSortOrder = (short)pluginType;
            App.DockManager.Add(dp);
        }


        //event handler when a plugin is selected from tabs
        void DockManager_ActivePanelChanged(object sender, DotSpatial.Controls.Docking.DockablePanelEventArgs e)
        {
            if (e.ActivePanelKey == strPanelKey)
            {
                App.DockManager.SelectPanel(strPanelKey);
                App.HeaderControl.SelectRoot(strPanelKey);
            }
        }


        //return the plugin type (Prediction)
        public Globals.PluginType PluginType
        {
            get { return pluginType; }
        }


        //return the panel key name
        public string PanelKey
        {
            get { return strPanelKey; }
        }


        //return the complete flag
        public Boolean Complete
        {
            get { return boolComplete; }
        }


        //return the visible flag
        public Boolean Visible
        {
            get { return boolVisible; }
        }


        //This function imports the signaller from the VBProjectManager
        [System.ComponentModel.Composition.Import("Signalling.GetSignaller", AllowDefault = true)]
        public Func<VBCommon.Signaller> GetSignaller
        {
            get;
            set;
        }  


        public void OnImportsSatisfied()
        {
            //If we've successfully imported a Signaller, then connect its events to our handlers.
            signaller = GetSignaller();
            signaller.BroadcastState += new VBCommon.Signaller.BroadcastEventHandler<VBCommon.PluginSupport.BroadcastEventArgs>(BroadcastStateListener);
            signaller.ProjectSaved += new VBCommon.Signaller.SerializationEventHandler<VBCommon.PluginSupport.SerializationEventArgs>(ProjectSavedListener);
            signaller.ProjectOpened += new VBCommon.Signaller.SerializationEventHandler<VBCommon.PluginSupport.SerializationEventArgs>(ProjectOpenedListener);
            signaller.UndoEvent += new VBCommon.Signaller.SerializationEventHandler<VBCommon.PluginSupport.UndoRedoEventArgs>(Undo);
            signaller.RedoEvent += new VBCommon.Signaller.SerializationEventHandler<VBCommon.PluginSupport.UndoRedoEventArgs>(Redo);
            signaller.UndoStackEvent += new VBCommon.Signaller.SerializationEventHandler<VBCommon.PluginSupport.UndoRedoEventArgs>(PushToStack);
            
            this.MessageSent += new MessageHandler<VBCommon.PluginSupport.MessageArgs>(signaller.HandleMessage);            
        }


        //This function imports the signaller from the VBProjectManager
        [System.ComponentModel.Composition.ImportMany(typeof(VBCommon.Interfaces.IModel))]
        private List<Lazy<IModel, IDictionary<string, object>>> models = null;


        //Expose the list of models via a property so that the control form can pull it out.
        public List<Lazy<IModel, IDictionary<string, object>>> Models
        {
            get { return (models); }
        }


        private void PassModelPluginList(object sender, EventArgs e)
        {
            ((frmPrediction)sender).models = models;
        }


        //event listener for plugin broadcasting changes
        private void BroadcastStateListener(object sender, VBCommon.PluginSupport.BroadcastEventArgs e)
        {
            if (((IPlugin)sender).PluginType == Globals.PluginType.Modeling)
            {
                if ((bool)e.PackedPluginState["Complete"])
                {                    
                    _frmPred.AddModel(e.PackedPluginState["Method"].ToString());
                    Show();
                }
                else
                {
                    int intValidModels = _frmPred.ClearMethod(e.PackedPluginState["Method"].ToString());
                    if (intValidModels == 0)
                        Hide();
                }

                _frmPred.ClearDataGridViews(e.PackedPluginState["Method"].ToString());
                boolComplete = false;
                boolChanged = true;
            }
            else if (((IPlugin)sender).PluginType == Globals.PluginType.Datasheet)
            {
                Hide(); /*
                if (!(bool)((IPlugin)sender).Complete)
                {
                    Hide();
                    return;
                }
                else
                {
                    if (!(bool)e.PackedPluginState["Clean"])
                    {
                        Activate();
                    }
                    Hide();
                }*/
            }                
        }


        public void Broadcast()
        {
            boolChanged = true;

            IDictionary<string, object> dictPackedState = _frmPred.PackState();
            dictPackedState.Add("Complete", boolComplete);
            dictPackedState.Add("Visible", boolVisible);

            signaller.RaiseBroadcastRequest(this, dictPackedState);
            signaller.TriggerUndoStack();
        }


        private void NotifiableChangeHandler(object sender, EventArgs e)
        {
            Broadcast();
        }


        //event handler for saving project state
        private void ProjectSavedListener(object sender, VBCommon.PluginSupport.SerializationEventArgs e)
        {
            IDictionary<string, object> dictPackedState = _frmPred.PackState();
            dictPackedState.Add("Complete", boolComplete);
            dictPackedState.Add("Visible", boolVisible);

            e.PackedPluginStates.Add(strPanelKey, dictPackedState);
        }


        //event handler for opening project state
        private void ProjectOpenedListener(object sender, VBCommon.PluginSupport.SerializationEventArgs e)
        {
            if (e.PackedPluginStates.ContainsKey(strPanelKey))
            {
                IDictionary<string, object> dictPlugin = e.PackedPluginStates[strPanelKey];

                if ((bool)dictPlugin["Visible"]) { Show(); }
                else { Hide(); }
                boolVisible = (bool)dictPlugin["Visible"];                
                boolComplete = (bool)dictPlugin["Complete"];                

                _frmPred.UnpackState(e.PackedPluginStates[strPanelKey]);
            }
        }


        private void PushToStack(object sender, UndoRedoEventArgs args)
        {
            if (boolChanged)
            {
                IDictionary<string, object> dictPackedState = _frmPred.PackState();
                dictPackedState.Add("Complete", boolComplete);
                dictPackedState.Add("Visible", boolVisible);

                string strKey = PersistentStackUtilities.RandomString(10);
                args.Store.Add(strKey, dictPackedState);
                UndoKeys.Push(strKey);
                RedoKeys.Clear();
                boolChanged = false;
            }
            else
            {
                try
                {
                    string strKey = UndoKeys.Peek();
                    UndoKeys.Push(strKey);
                    RedoKeys.Clear();
                }
                catch { }
            }
        }


        private void Undo(object sender, UndoRedoEventArgs args)
        {
            try
            {
                string strCurrentKey = UndoKeys.Pop();
                string strPastKey = UndoKeys.Peek();
                RedoKeys.Push(strCurrentKey);

                if (strCurrentKey != strPastKey)
                {
                    IDictionary<string, object> dictPlugin = args.Store[strPastKey];
                    if ((bool)dictPlugin["Visible"]) { Show(); }
                    else { Hide(); }

                    boolVisible = (bool)dictPlugin["Visible"];
                    boolComplete = (bool)dictPlugin["Complete"];
                    _frmPred.UnpackState(dictPlugin);
                }
            }
            catch { }
        }


        private void Redo(object sender, UndoRedoEventArgs args)
        {
            try
            {
                string strCurrentKey = UndoKeys.Peek();
                string strNextKey = RedoKeys.Pop();
                UndoKeys.Push(strNextKey);

                if (strCurrentKey != strNextKey)
                {
                    IDictionary<string, object> dictPlugin = args.Store[strNextKey];
                    if ((bool)dictPlugin["Visible"]) { Show(); }
                    else { Hide(); }

                    boolVisible = (bool)dictPlugin["Visible"];
                    boolComplete = (bool)dictPlugin["Complete"];
                    _frmPred.UnpackState(dictPlugin);
                }
            }
            catch { }
        }


        private void SendMessage(string message)
        {
            if (MessageSent != null)
            {
                VBCommon.PluginSupport.MessageArgs e = new VBCommon.PluginSupport.MessageArgs(message);
                MessageSent(this, e);
            }
        }
                

        //import iv data, sends to form click event
        void btnImportIV_Click(object sender, EventArgs e)
        {
            bool validated = _frmPred.btnImportIVs_Click(sender, e);
            if (validated)
            {
                btnIVDataVal.Enabled = true;
                btnMakePred.Enabled = true;
                Broadcast();
            }
        }


        //import iv data, sends to form click event
        void btnSetEnddatURL_Click(object sender, EventArgs e)
        {
            bool validated = _frmPred.btnSetEnddatURL_Click(sender, e);
            if (validated)
            {
                btnImportFromEnddat.Enabled = true;
                Broadcast();
            }
            else
            {
                btnImportFromEnddat.Enabled = false;
            }
        }


        //import iv data, sends to form click event
        void btnImportFromEnddat_Click(object sender, EventArgs e)
        {
            bool validated = _frmPred.btnImportFromEnddat_Click(sender, e);
            if (validated)
            {
                btnIVDataVal.Enabled = true;
                btnMakePred.Enabled = true;
                Broadcast();
            }
        }


        //import ob data, sends to form click event
        void btnImportOB_Click(object sender, EventArgs e)
        {
            _frmPred.btnImportObs_Click(sender, e);
            Broadcast();
        }

        void ControlChangeEventHandler(object sender, EventArgs e)
        {
            this.boolChanged = true;
        }

        
        // validate data, sends to form click event and enables make prediction button
        void btnIVDataVal_Click(object sender, EventArgs e)
        {
            bool validated = _frmPred.btnIVDataValidation_Click(sender, e);
            if (validated)
            {
                btnMakePred.Enabled = true;
                Broadcast();
            }
            else
                btnMakePred.Enabled = false;
        }


        //make prediction, sends to form click event and sets complete to true
        void btnMakePrediction_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            _frmPred.btnMakePredictions_Click(sender, e);
           boolComplete = true;
           Broadcast();
           Cursor.Current = Cursors.Default;
        }


        //plot, sends to form click event
        void btnPlot_Ck(object sender, EventArgs e)
        {
            _frmPred.btnPlot_Click(sender, e);
        }


        //clear, sends to form click event
        void btnClear_Click(object sender, EventArgs e)
        {
            DialogResult dgr = MessageBox.Show("This will clear the prediction plugin of all imported data for all models.", "Proceed?", MessageBoxButtons.OKCancel);
            if (dgr == DialogResult.OK)
            {
                _frmPred.btnClear_Click(sender, e);
                btnIVDataVal.Enabled = false;
                btnMakePred.Enabled = false;
                btnImportFromEnddat.Enabled = false;

                Broadcast();
            }
        }


        //export table, sends to form click event
        void btnExportTable_Ck(object sender, EventArgs e)
        {
            _frmPred.btnExportTable_Click(sender, e);
        }


        void _frmPred_ButtonStatusEvent(object sender, ButtonStatusEventArgs args)
        {
            if (args.Set == false)
            {
                args.ButtonStatus.Add("PredictionButtonEnabled", btnMakePred.Enabled);
                args.ButtonStatus.Add("ValidationButtonEnabled", btnIVDataVal.Enabled);
                args.ButtonStatus.Add("EnddatImportButtonEnabled", btnImportFromEnddat.Enabled);
            }
            else
            {
                if (args.ButtonStatus.ContainsKey("ValidationButtonEnabled")) { btnIVDataVal.Enabled = args.ButtonStatus["ValidationButtonEnabled"]; }
                if (args.ButtonStatus.ContainsKey("PredictionButtonEnabled")) { btnMakePred.Enabled = args.ButtonStatus["PredictionButtonEnabled"]; }
                if (args.ButtonStatus.ContainsKey("EnddatImportButtonEnabled")) { btnImportFromEnddat.Enabled = args.ButtonStatus["EnddatImportButtonEnabled"]; }
            }
        }
    }
}
