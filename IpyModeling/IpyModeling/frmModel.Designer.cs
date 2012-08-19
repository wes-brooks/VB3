namespace GALibForm
{
    partial class frmModel
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabProgress = new System.Windows.Forms.TabPage();
            this.zedGraphControl1 = new ZedGraph.ZedGraphControl();
            this.tabResults = new System.Windows.Forms.TabPage();
            this.zedGraphControl2 = new ZedGraph.ZedGraphControl();
            this.tabObsPred = new System.Windows.Forms.TabPage();
            this.mlrPredObs1 = new VBControls.MLRPredObs();
            this.tabROC = new System.Windows.Forms.TabPage();
            this.btnView = new System.Windows.Forms.Button();
            this.listView4 = new System.Windows.Forms.ListView();
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader14 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader15 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnROCPlot = new System.Windows.Forms.Button();
            this.listView3 = new System.Windows.Forms.ListView();
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.zgcROC = new ZedGraph.ZedGraphControl();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lblNumObs = new System.Windows.Forms.Label();
            this.lblAvailVars = new System.Windows.Forms.Label();
            this.lblDepVars = new System.Windows.Forms.Label();
            this.btnRemoveInputVariable = new System.Windows.Forms.Button();
            this.btnAddInputVariable = new System.Windows.Forms.Button();
            this.lbDepVarName = new System.Windows.Forms.Label();
            this.lblDepVariable = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.lbIndVariables = new System.Windows.Forms.ListBox();
            this.label9 = new System.Windows.Forms.Label();
            this.lbAvailableVariables = new System.Windows.Forms.ListBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnCrossValidation = new System.Windows.Forms.Button();
            this.tabStats = new System.Windows.Forms.TabControl();
            this.tabVariableStats = new System.Windows.Forms.TabPage();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabModelStats = new System.Windows.Forms.TabPage();
            this.listView2 = new System.Windows.Forms.ListView();
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnClearList = new System.Windows.Forms.Button();
            this.btnAddtoList = new System.Windows.Forms.Button();
            this.btnViewReport = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label13 = new System.Windows.Forms.Label();
            this.btnRun = new System.Windows.Forms.Button();
            this.tabControlModelGeneration = new System.Windows.Forms.TabControl();
            this.tabManual = new System.Windows.Forms.TabPage();
            this.chkAllCombinations = new System.Windows.Forms.CheckBox();
            this.label15 = new System.Windows.Forms.Label();
            this.tabGA = new System.Windows.Forms.TabPage();
            this.txtSeed = new System.Windows.Forms.TextBox();
            this.chkSeed = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtCrossoverRate = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtMutRate = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtNumGen = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtPopSize = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.txtPwrValMET = new System.Windows.Forms.TextBox();
            this.rbPwrValMET = new System.Windows.Forms.RadioButton();
            this.rbLogeValMET = new System.Windows.Forms.RadioButton();
            this.rbValMET = new System.Windows.Forms.RadioButton();
            this.rbLog10ValMET = new System.Windows.Forms.RadioButton();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.label26 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.tbRegThreshVert = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.tbDecThreshHoriz = new System.Windows.Forms.TextBox();
            this.lblMaxAndRecommended = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMaxVars = new System.Windows.Forms.TextBox();
            this.txtMaxVIF = new System.Windows.Forms.TextBox();
            this.lblMaxVIF = new System.Windows.Forms.Label();
            this.cbCriteria = new System.Windows.Forms.ComboBox();
            this.PLS = new System.Windows.Forms.TabPage();
            this.ipyPLSControl = new IPyModelingControl.IPyPLSControl();
            this.GBM = new System.Windows.Forms.TabPage();
            this.ipyGBMControl = new IPyModelingControl.IPyGBMControl();
            this.tabControl2.SuspendLayout();
            this.tabProgress.SuspendLayout();
            this.tabResults.SuspendLayout();
            this.tabObsPred.SuspendLayout();
            this.tabROC.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabStats.SuspendLayout();
            this.tabVariableStats.SuspendLayout();
            this.tabModelStats.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabControlModelGeneration.SuspendLayout();
            this.tabManual.SuspendLayout();
            this.tabGA.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.PLS.SuspendLayout();
            this.GBM.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // tabControl2
            // 
            this.tabControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl2.Controls.Add(this.tabProgress);
            this.tabControl2.Controls.Add(this.tabResults);
            this.tabControl2.Controls.Add(this.tabObsPred);
            this.tabControl2.Controls.Add(this.tabROC);
            this.tabControl2.Location = new System.Drawing.Point(8, 238);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.helpProvider1.SetShowHelp(this.tabControl2, false);
            this.tabControl2.Size = new System.Drawing.Size(708, 375);
            this.tabControl2.TabIndex = 50;
            // 
            // tabProgress
            // 
            this.tabProgress.Controls.Add(this.zedGraphControl1);
            this.tabProgress.Location = new System.Drawing.Point(4, 22);
            this.tabProgress.Name = "tabProgress";
            this.tabProgress.Padding = new System.Windows.Forms.Padding(3);
            this.tabProgress.Size = new System.Drawing.Size(700, 349);
            this.tabProgress.TabIndex = 0;
            this.tabProgress.Text = "Progress";
            this.tabProgress.UseVisualStyleBackColor = true;
            // 
            // zedGraphControl1
            // 
            this.zedGraphControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zedGraphControl1.EditButtons = System.Windows.Forms.MouseButtons.Left;
            this.zedGraphControl1.Location = new System.Drawing.Point(3, 3);
            this.zedGraphControl1.Name = "zedGraphControl1";
            this.zedGraphControl1.PanModifierKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)));
            this.zedGraphControl1.ScrollGrace = 0D;
            this.zedGraphControl1.ScrollMaxX = 0D;
            this.zedGraphControl1.ScrollMaxY = 0D;
            this.zedGraphControl1.ScrollMaxY2 = 0D;
            this.zedGraphControl1.ScrollMinX = 0D;
            this.zedGraphControl1.ScrollMinY = 0D;
            this.zedGraphControl1.ScrollMinY2 = 0D;
            this.zedGraphControl1.Size = new System.Drawing.Size(694, 343);
            this.zedGraphControl1.TabIndex = 3;
            // 
            // tabResults
            // 
            this.tabResults.Controls.Add(this.zedGraphControl2);
            this.tabResults.Location = new System.Drawing.Point(4, 22);
            this.tabResults.Name = "tabResults";
            this.tabResults.Padding = new System.Windows.Forms.Padding(3);
            this.tabResults.Size = new System.Drawing.Size(700, 349);
            this.tabResults.TabIndex = 1;
            this.tabResults.Text = "Results";
            this.tabResults.UseVisualStyleBackColor = true;
            // 
            // zedGraphControl2
            // 
            this.zedGraphControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zedGraphControl2.EditButtons = System.Windows.Forms.MouseButtons.Left;
            this.zedGraphControl2.Location = new System.Drawing.Point(3, 3);
            this.zedGraphControl2.Name = "zedGraphControl2";
            this.zedGraphControl2.PanModifierKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)));
            this.zedGraphControl2.ScrollGrace = 0D;
            this.zedGraphControl2.ScrollMaxX = 0D;
            this.zedGraphControl2.ScrollMaxY = 0D;
            this.zedGraphControl2.ScrollMaxY2 = 0D;
            this.zedGraphControl2.ScrollMinX = 0D;
            this.zedGraphControl2.ScrollMinY = 0D;
            this.zedGraphControl2.ScrollMinY2 = 0D;
            this.zedGraphControl2.Size = new System.Drawing.Size(694, 343);
            this.zedGraphControl2.TabIndex = 4;
            // 
            // tabObsPred
            // 
            this.tabObsPred.Controls.Add(this.mlrPredObs1);
            this.tabObsPred.Location = new System.Drawing.Point(4, 22);
            this.tabObsPred.Name = "tabObsPred";
            this.tabObsPred.Size = new System.Drawing.Size(700, 349);
            this.tabObsPred.TabIndex = 2;
            this.tabObsPred.Text = "Observed vs Predicted";
            this.tabObsPred.UseVisualStyleBackColor = true;
            // 
            // mlrPredObs1
            // 
            this.mlrPredObs1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mlrPredObs1.Location = new System.Drawing.Point(3, 3);
            this.mlrPredObs1.Name = "mlrPredObs1";
            this.mlrPredObs1.PowerExponent = double.NaN;
            this.mlrPredObs1.Size = new System.Drawing.Size(686, 345);
            this.mlrPredObs1.TabIndex = 0;
            this.mlrPredObs1.Transform = VBTools.Globals.DependentVariableTransforms.Log10;
            // 
            // tabROC
            // 
            this.tabROC.Controls.Add(this.btnView);
            this.tabROC.Controls.Add(this.listView4);
            this.tabROC.Controls.Add(this.btnROCPlot);
            this.tabROC.Controls.Add(this.listView3);
            this.tabROC.Controls.Add(this.zgcROC);
            this.helpProvider1.SetHelpKeyword(this.tabROC, "");
            this.tabROC.Location = new System.Drawing.Point(4, 22);
            this.tabROC.Name = "tabROC";
            this.helpProvider1.SetShowHelp(this.tabROC, false);
            this.tabROC.Size = new System.Drawing.Size(700, 349);
            this.tabROC.TabIndex = 3;
            this.tabROC.Text = "ROC Curves";
            this.tabROC.UseVisualStyleBackColor = true;
            // 
            // btnView
            // 
            this.btnView.Location = new System.Drawing.Point(46, 259);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(75, 23);
            this.btnView.TabIndex = 54;
            this.btnView.Text = "View Table";
            this.btnView.UseVisualStyleBackColor = true;
            // 
            // listView4
            // 
            this.listView4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listView4.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader11,
            this.columnHeader12,
            this.columnHeader13,
            this.columnHeader14,
            this.columnHeader15});
            this.listView4.FullRowSelect = true;
            this.listView4.GridLines = true;
            this.listView4.Location = new System.Drawing.Point(172, 8);
            this.listView4.Name = "listView4";
            this.listView4.Size = new System.Drawing.Size(502, 306);
            this.listView4.TabIndex = 53;
            this.listView4.UseCompatibleStateImageBehavior = false;
            this.listView4.View = System.Windows.Forms.View.Details;
            this.listView4.Visible = false;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "Decision Criterion";
            this.columnHeader11.Width = 100;
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "FP";
            this.columnHeader12.Width = 35;
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "FN";
            this.columnHeader13.Width = 35;
            // 
            // columnHeader14
            // 
            this.columnHeader14.Text = "Specificity";
            this.columnHeader14.Width = 75;
            // 
            // columnHeader15
            // 
            this.columnHeader15.Text = "Sensitivity";
            this.columnHeader15.Width = 75;
            // 
            // btnROCPlot
            // 
            this.btnROCPlot.Location = new System.Drawing.Point(46, 217);
            this.btnROCPlot.Name = "btnROCPlot";
            this.btnROCPlot.Size = new System.Drawing.Size(75, 23);
            this.btnROCPlot.TabIndex = 52;
            this.btnROCPlot.Text = "Plot";
            this.btnROCPlot.UseVisualStyleBackColor = true;
            // 
            // listView3
            // 
            this.listView3.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader9,
            this.columnHeader10});
            this.listView3.FullRowSelect = true;
            this.listView3.GridLines = true;
            this.listView3.Location = new System.Drawing.Point(3, 8);
            this.listView3.Name = "listView3";
            this.listView3.Size = new System.Drawing.Size(165, 187);
            this.listView3.TabIndex = 51;
            this.listView3.UseCompatibleStateImageBehavior = false;
            this.listView3.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Model Fit";
            this.columnHeader9.Width = 78;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "AUC";
            this.columnHeader10.Width = 78;
            // 
            // zgcROC
            // 
            this.zgcROC.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.zgcROC.EditButtons = System.Windows.Forms.MouseButtons.Left;
            this.zgcROC.Location = new System.Drawing.Point(172, 8);
            this.zgcROC.Name = "zgcROC";
            this.zgcROC.PanModifierKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)));
            this.zgcROC.ScrollGrace = 0D;
            this.zgcROC.ScrollMaxX = 0D;
            this.zgcROC.ScrollMaxY = 0D;
            this.zgcROC.ScrollMaxY2 = 0D;
            this.zgcROC.ScrollMinX = 0D;
            this.zgcROC.ScrollMinY = 0D;
            this.zgcROC.ScrollMinY2 = 0D;
            this.zgcROC.Size = new System.Drawing.Size(493, 306);
            this.zgcROC.TabIndex = 6;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.PLS);
            this.tabControl1.Controls.Add(this.GBM);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1102, 645);
            this.tabControl1.TabIndex = 68;
            // 
            // tabPage1
            // 
            this.tabPage1.AutoScroll = true;
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.lblNumObs);
            this.tabPage1.Controls.Add(this.lblAvailVars);
            this.tabPage1.Controls.Add(this.lblDepVars);
            this.tabPage1.Controls.Add(this.btnRemoveInputVariable);
            this.tabPage1.Controls.Add(this.btnAddInputVariable);
            this.tabPage1.Controls.Add(this.lbDepVarName);
            this.tabPage1.Controls.Add(this.lblDepVariable);
            this.tabPage1.Controls.Add(this.label14);
            this.tabPage1.Controls.Add(this.lbIndVariables);
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.lbAvailableVariables);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1094, 619);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Variable Selection";
            // 
            // lblNumObs
            // 
            this.lblNumObs.AutoSize = true;
            this.lblNumObs.Location = new System.Drawing.Point(117, 34);
            this.lblNumObs.Name = "lblNumObs";
            this.lblNumObs.Size = new System.Drawing.Size(141, 13);
            this.lblNumObs.TabIndex = 71;
            this.lblNumObs.Text = "Number of Observations: {0}";
            // 
            // lblAvailVars
            // 
            this.lblAvailVars.AutoSize = true;
            this.lblAvailVars.Location = new System.Drawing.Point(105, 34);
            this.lblAvailVars.Name = "lblAvailVars";
            this.lblAvailVars.Size = new System.Drawing.Size(19, 13);
            this.lblAvailVars.TabIndex = 67;
            this.lblAvailVars.Text = "    ";
            // 
            // lblDepVars
            // 
            this.lblDepVars.AutoSize = true;
            this.lblDepVars.Location = new System.Drawing.Point(313, 34);
            this.lblDepVars.Name = "lblDepVars";
            this.lblDepVars.Size = new System.Drawing.Size(22, 13);
            this.lblDepVars.TabIndex = 73;
            this.lblDepVars.Text = "     ";
            // 
            // btnRemoveInputVariable
            // 
            this.btnRemoveInputVariable.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnRemoveInputVariable.Location = new System.Drawing.Point(164, 216);
            this.btnRemoveInputVariable.Name = "btnRemoveInputVariable";
            this.btnRemoveInputVariable.Size = new System.Drawing.Size(25, 20);
            this.btnRemoveInputVariable.TabIndex = 72;
            this.btnRemoveInputVariable.Text = "<";
            this.btnRemoveInputVariable.Click += new System.EventHandler(this.btnRemoveInputVariable_Click);
            // 
            // btnAddInputVariable
            // 
            this.btnAddInputVariable.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAddInputVariable.Location = new System.Drawing.Point(164, 190);
            this.btnAddInputVariable.Name = "btnAddInputVariable";
            this.btnAddInputVariable.Size = new System.Drawing.Size(25, 20);
            this.btnAddInputVariable.TabIndex = 71;
            this.btnAddInputVariable.Text = ">";
            this.btnAddInputVariable.Click += new System.EventHandler(this.btnAddInputVariable_Click);
            // 
            // lbDepVarName
            // 
            this.lbDepVarName.AutoSize = true;
            this.lbDepVarName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDepVarName.Location = new System.Drawing.Point(117, 7);
            this.lbDepVarName.Name = "lbDepVarName";
            this.lbDepVarName.Size = new System.Drawing.Size(150, 13);
            this.lbDepVarName.TabIndex = 70;
            this.lbDepVarName.Text = "dependent variable name";
            // 
            // lblDepVariable
            // 
            this.lblDepVariable.AutoSize = true;
            this.lblDepVariable.Location = new System.Drawing.Point(4, 7);
            this.lblDepVariable.Name = "lblDepVariable";
            this.lblDepVariable.Size = new System.Drawing.Size(107, 13);
            this.lblDepVariable.TabIndex = 69;
            this.lblDepVariable.Text = "Dependent Variable: ";
            // 
            // label14
            // 
            this.label14.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(194, 59);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(113, 13);
            this.label14.TabIndex = 68;
            this.label14.Text = "Independent Variables";
            // 
            // lbIndVariables
            // 
            this.lbIndVariables.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbIndVariables.FormattingEnabled = true;
            this.lbIndVariables.HorizontalScrollbar = true;
            this.lbIndVariables.Location = new System.Drawing.Point(192, 75);
            this.lbIndVariables.Name = "lbIndVariables";
            this.lbIndVariables.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbIndVariables.Size = new System.Drawing.Size(160, 446);
            this.lbIndVariables.TabIndex = 67;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 34);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(96, 13);
            this.label9.TabIndex = 66;
            this.label9.Text = "Available Variables";
            // 
            // lbAvailableVariables
            // 
            this.lbAvailableVariables.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbAvailableVariables.FormattingEnabled = true;
            this.lbAvailableVariables.HorizontalScrollbar = true;
            this.lbAvailableVariables.Location = new System.Drawing.Point(3, 75);
            this.lbAvailableVariables.Name = "lbAvailableVariables";
            this.lbAvailableVariables.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbAvailableVariables.Size = new System.Drawing.Size(160, 446);
            this.lbAvailableVariables.TabIndex = 65;
            // 
            // tabPage2
            // 
            this.tabPage2.AutoScroll = true;
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.groupBox5);
            this.tabPage2.Controls.Add(this.btnRun);
            this.tabPage2.Controls.Add(this.tabControlModelGeneration);
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1094, 619);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "MLR";
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.btnCrossValidation);
            this.groupBox5.Controls.Add(this.tabStats);
            this.groupBox5.Controls.Add(this.groupBox1);
            this.groupBox5.Controls.Add(this.btnViewReport);
            this.groupBox5.Controls.Add(this.tabControl2);
            this.groupBox5.Controls.Add(this.listBox1);
            this.groupBox5.Controls.Add(this.label13);
            this.groupBox5.Location = new System.Drawing.Point(357, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(720, 616);
            this.groupBox5.TabIndex = 72;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Model Information";
            // 
            // btnCrossValidation
            // 
            this.btnCrossValidation.Location = new System.Drawing.Point(94, 182);
            this.btnCrossValidation.Name = "btnCrossValidation";
            this.btnCrossValidation.Size = new System.Drawing.Size(65, 35);
            this.btnCrossValidation.TabIndex = 61;
            this.btnCrossValidation.Text = "Cross Validation";
            this.btnCrossValidation.UseVisualStyleBackColor = true;
            this.btnCrossValidation.Click += new System.EventHandler(this.btnCrossValidation_Click);
            // 
            // tabStats
            // 
            this.tabStats.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabStats.Controls.Add(this.tabVariableStats);
            this.tabStats.Controls.Add(this.tabModelStats);
            this.tabStats.Location = new System.Drawing.Point(165, 17);
            this.tabStats.Name = "tabStats";
            this.tabStats.SelectedIndex = 0;
            this.tabStats.Size = new System.Drawing.Size(551, 219);
            this.tabStats.TabIndex = 60;
            // 
            // tabVariableStats
            // 
            this.tabVariableStats.Controls.Add(this.listView1);
            this.tabVariableStats.Location = new System.Drawing.Point(4, 22);
            this.tabVariableStats.Name = "tabVariableStats";
            this.tabVariableStats.Padding = new System.Windows.Forms.Padding(3);
            this.tabVariableStats.Size = new System.Drawing.Size(543, 193);
            this.tabVariableStats.TabIndex = 0;
            this.tabVariableStats.Text = "Variable Statistics";
            this.tabVariableStats.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            this.listView1.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader8});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(3, 3);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(537, 187);
            this.listView1.TabIndex = 50;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Parameter";
            this.columnHeader1.Width = 180;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Coefficient";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader2.Width = 78;
            // 
            // columnHeader3
            // 
            this.columnHeader3.DisplayIndex = 3;
            this.columnHeader3.Text = "Std. Error";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader3.Width = 78;
            // 
            // columnHeader4
            // 
            this.columnHeader4.DisplayIndex = 4;
            this.columnHeader4.Text = "t-Statistic";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader4.Width = 78;
            // 
            // columnHeader5
            // 
            this.columnHeader5.DisplayIndex = 5;
            this.columnHeader5.Text = "P-Value";
            this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader5.Width = 96;
            // 
            // columnHeader8
            // 
            this.columnHeader8.DisplayIndex = 2;
            this.columnHeader8.Text = "Standardized Coefficient";
            this.columnHeader8.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader8.Width = 130;
            // 
            // tabModelStats
            // 
            this.tabModelStats.Controls.Add(this.listView2);
            this.tabModelStats.Location = new System.Drawing.Point(4, 22);
            this.tabModelStats.Name = "tabModelStats";
            this.tabModelStats.Padding = new System.Windows.Forms.Padding(3);
            this.tabModelStats.Size = new System.Drawing.Size(543, 193);
            this.tabModelStats.TabIndex = 1;
            this.tabModelStats.Text = "Model Statistics";
            this.tabModelStats.UseVisualStyleBackColor = true;
            // 
            // listView2
            // 
            this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader6,
            this.columnHeader7});
            this.listView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView2.FullRowSelect = true;
            this.listView2.GridLines = true;
            this.listView2.Location = new System.Drawing.Point(3, 3);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(537, 187);
            this.listView2.TabIndex = 58;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Metric";
            this.columnHeader6.Width = 130;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Value";
            this.columnHeader7.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader7.Width = 88;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnClearList);
            this.groupBox1.Controls.Add(this.btnAddtoList);
            this.groupBox1.Location = new System.Drawing.Point(2, 135);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(86, 82);
            this.groupBox1.TabIndex = 59;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "IV Filter";
            // 
            // btnClearList
            // 
            this.btnClearList.Location = new System.Drawing.Point(6, 53);
            this.btnClearList.Name = "btnClearList";
            this.btnClearList.Size = new System.Drawing.Size(75, 23);
            this.btnClearList.TabIndex = 1;
            this.btnClearList.Text = "Clear List";
            this.btnClearList.UseVisualStyleBackColor = true;
            this.btnClearList.Click += new System.EventHandler(this.btnClearList_Click);
            // 
            // btnAddtoList
            // 
            this.btnAddtoList.Location = new System.Drawing.Point(6, 17);
            this.btnAddtoList.Name = "btnAddtoList";
            this.btnAddtoList.Size = new System.Drawing.Size(74, 23);
            this.btnAddtoList.TabIndex = 0;
            this.btnAddtoList.Text = "Add to List";
            this.btnAddtoList.UseVisualStyleBackColor = true;
            this.btnAddtoList.Click += new System.EventHandler(this.btnAddtoList_Click);
            // 
            // btnViewReport
            // 
            this.btnViewReport.Location = new System.Drawing.Point(94, 142);
            this.btnViewReport.Name = "btnViewReport";
            this.btnViewReport.Size = new System.Drawing.Size(65, 34);
            this.btnViewReport.TabIndex = 58;
            this.btnViewReport.Text = "View Report";
            this.btnViewReport.UseVisualStyleBackColor = true;
            this.btnViewReport.Click += new System.EventHandler(this.btnViewReport_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(18, 34);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(141, 95);
            this.listBox1.TabIndex = 48;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(19, 18);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(50, 13);
            this.label13.TabIndex = 43;
            this.label13.Text = "Best Fits:";
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(132, 540);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(75, 23);
            this.btnRun.TabIndex = 71;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // tabControlModelGeneration
            // 
            this.tabControlModelGeneration.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabControlModelGeneration.Controls.Add(this.tabManual);
            this.tabControlModelGeneration.Controls.Add(this.tabGA);
            this.tabControlModelGeneration.Location = new System.Drawing.Point(0, 340);
            this.tabControlModelGeneration.Name = "tabControlModelGeneration";
            this.tabControlModelGeneration.SelectedIndex = 0;
            this.tabControlModelGeneration.Size = new System.Drawing.Size(351, 194);
            this.tabControlModelGeneration.TabIndex = 61;
            // 
            // tabManual
            // 
            this.tabManual.BackColor = System.Drawing.Color.Transparent;
            this.tabManual.Controls.Add(this.chkAllCombinations);
            this.tabManual.Controls.Add(this.label15);
            this.tabManual.Location = new System.Drawing.Point(4, 25);
            this.tabManual.Name = "tabManual";
            this.tabManual.Size = new System.Drawing.Size(343, 165);
            this.tabManual.TabIndex = 2;
            this.tabManual.Text = "Manual";
            this.tabManual.UseVisualStyleBackColor = true;
            // 
            // chkAllCombinations
            // 
            this.chkAllCombinations.AutoSize = true;
            this.chkAllCombinations.Location = new System.Drawing.Point(27, 54);
            this.chkAllCombinations.Name = "chkAllCombinations";
            this.chkAllCombinations.Size = new System.Drawing.Size(124, 17);
            this.chkAllCombinations.TabIndex = 51;
            this.chkAllCombinations.Text = "Run all combinations";
            this.chkAllCombinations.UseVisualStyleBackColor = true;
            // 
            // label15
            // 
            this.label15.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(-721, 27);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(41, 13);
            this.label15.TabIndex = 49;
            this.label15.Text = "Models";
            // 
            // tabGA
            // 
            this.tabGA.BackColor = System.Drawing.Color.Transparent;
            this.tabGA.Controls.Add(this.txtSeed);
            this.tabGA.Controls.Add(this.chkSeed);
            this.tabGA.Controls.Add(this.label8);
            this.tabGA.Controls.Add(this.txtCrossoverRate);
            this.tabGA.Controls.Add(this.label7);
            this.tabGA.Controls.Add(this.txtMutRate);
            this.tabGA.Controls.Add(this.label6);
            this.tabGA.Controls.Add(this.txtNumGen);
            this.tabGA.Controls.Add(this.label5);
            this.tabGA.Controls.Add(this.txtPopSize);
            this.tabGA.Location = new System.Drawing.Point(4, 25);
            this.tabGA.Name = "tabGA";
            this.tabGA.Padding = new System.Windows.Forms.Padding(3);
            this.tabGA.Size = new System.Drawing.Size(343, 165);
            this.tabGA.TabIndex = 1;
            this.tabGA.Text = "Genetic Algorithm";
            this.tabGA.UseVisualStyleBackColor = true;
            // 
            // txtSeed
            // 
            this.txtSeed.Enabled = false;
            this.txtSeed.Location = new System.Drawing.Point(135, 11);
            this.txtSeed.Name = "txtSeed";
            this.txtSeed.Size = new System.Drawing.Size(63, 20);
            this.txtSeed.TabIndex = 72;
            // 
            // chkSeed
            // 
            this.chkSeed.AutoSize = true;
            this.chkSeed.Location = new System.Drawing.Point(18, 14);
            this.chkSeed.Name = "chkSeed";
            this.chkSeed.Size = new System.Drawing.Size(103, 17);
            this.chkSeed.TabIndex = 71;
            this.chkSeed.Text = "Set Seed Value:";
            this.chkSeed.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(14, 132);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(83, 13);
            this.label8.TabIndex = 37;
            this.label8.Text = "Crossover Rate:";
            // 
            // txtCrossoverRate
            // 
            this.txtCrossoverRate.Location = new System.Drawing.Point(135, 128);
            this.txtCrossoverRate.Name = "txtCrossoverRate";
            this.txtCrossoverRate.Size = new System.Drawing.Size(63, 20);
            this.txtCrossoverRate.TabIndex = 36;
            this.txtCrossoverRate.Text = "0.50";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(14, 101);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 13);
            this.label7.TabIndex = 35;
            this.label7.Text = "Mutation Rate:";
            // 
            // txtMutRate
            // 
            this.txtMutRate.Location = new System.Drawing.Point(135, 97);
            this.txtMutRate.Name = "txtMutRate";
            this.txtMutRate.Size = new System.Drawing.Size(63, 20);
            this.txtMutRate.TabIndex = 34;
            this.txtMutRate.Text = "0.05";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 73);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(119, 13);
            this.label6.TabIndex = 33;
            this.label6.Text = "Number of Generations:";
            // 
            // txtNumGen
            // 
            this.txtNumGen.Location = new System.Drawing.Point(135, 69);
            this.txtNumGen.Name = "txtNumGen";
            this.txtNumGen.Size = new System.Drawing.Size(63, 20);
            this.txtNumGen.TabIndex = 32;
            this.txtNumGen.Text = "100";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 45);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 13);
            this.label5.TabIndex = 31;
            this.label5.Text = "Population Size:";
            // 
            // txtPopSize
            // 
            this.txtPopSize.Location = new System.Drawing.Point(135, 41);
            this.txtPopSize.Name = "txtPopSize";
            this.txtPopSize.Size = new System.Drawing.Size(63, 20);
            this.txtPopSize.TabIndex = 30;
            this.txtPopSize.Text = "100";
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.groupBox8);
            this.groupBox4.Controls.Add(this.lblMaxAndRecommended);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.txtMaxVars);
            this.groupBox4.Controls.Add(this.txtMaxVIF);
            this.groupBox4.Controls.Add(this.lblMaxVIF);
            this.groupBox4.Controls.Add(this.cbCriteria);
            this.groupBox4.Location = new System.Drawing.Point(0, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(351, 337);
            this.groupBox4.TabIndex = 62;
            this.groupBox4.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(21, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 90;
            this.label1.Text = "Evaluation Criteria";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.groupBox7);
            this.groupBox8.Controls.Add(this.groupBox9);
            this.groupBox8.Controls.Add(this.label25);
            this.groupBox8.Controls.Add(this.label23);
            this.groupBox8.Controls.Add(this.tbRegThreshVert);
            this.groupBox8.Controls.Add(this.label24);
            this.groupBox8.Controls.Add(this.tbDecThreshHoriz);
            this.groupBox8.Location = new System.Drawing.Point(2, 140);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(343, 184);
            this.groupBox8.TabIndex = 89;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Model EvaluationThresholds";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.txtPwrValMET);
            this.groupBox7.Controls.Add(this.rbPwrValMET);
            this.groupBox7.Controls.Add(this.rbLogeValMET);
            this.groupBox7.Controls.Add(this.rbValMET);
            this.groupBox7.Controls.Add(this.rbLog10ValMET);
            this.groupBox7.Location = new System.Drawing.Point(4, 75);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(147, 96);
            this.groupBox7.TabIndex = 91;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Threshold Transform";
            // 
            // txtPwrValMET
            // 
            this.txtPwrValMET.Enabled = false;
            this.txtPwrValMET.Location = new System.Drawing.Point(83, 71);
            this.txtPwrValMET.Name = "txtPwrValMET";
            this.txtPwrValMET.Size = new System.Drawing.Size(53, 20);
            this.txtPwrValMET.TabIndex = 19;
            // 
            // rbPwrValMET
            // 
            this.rbPwrValMET.AutoSize = true;
            this.rbPwrValMET.Location = new System.Drawing.Point(16, 74);
            this.rbPwrValMET.Name = "rbPwrValMET";
            this.rbPwrValMET.Size = new System.Drawing.Size(55, 17);
            this.rbPwrValMET.TabIndex = 18;
            this.rbPwrValMET.Text = "Power";
            this.rbPwrValMET.UseVisualStyleBackColor = true;
            // 
            // rbLogeValMET
            // 
            this.rbLogeValMET.AutoSize = true;
            this.rbLogeValMET.Location = new System.Drawing.Point(16, 56);
            this.rbLogeValMET.Name = "rbLogeValMET";
            this.rbLogeValMET.Size = new System.Drawing.Size(37, 17);
            this.rbLogeValMET.TabIndex = 17;
            this.rbLogeValMET.Text = "Ln";
            this.rbLogeValMET.UseVisualStyleBackColor = true;
            // 
            // rbValMET
            // 
            this.rbValMET.AutoSize = true;
            this.rbValMET.Checked = true;
            this.rbValMET.Location = new System.Drawing.Point(16, 20);
            this.rbValMET.Name = "rbValMET";
            this.rbValMET.Size = new System.Drawing.Size(51, 17);
            this.rbValMET.TabIndex = 15;
            this.rbValMET.TabStop = true;
            this.rbValMET.Text = "None";
            this.rbValMET.UseVisualStyleBackColor = true;
            // 
            // rbLog10ValMET
            // 
            this.rbLog10ValMET.AutoSize = true;
            this.rbLog10ValMET.Location = new System.Drawing.Point(16, 38);
            this.rbLog10ValMET.Name = "rbLog10ValMET";
            this.rbLog10ValMET.Size = new System.Drawing.Size(55, 17);
            this.rbLog10ValMET.TabIndex = 16;
            this.rbLog10ValMET.Text = "Log10";
            this.rbLog10ValMET.UseVisualStyleBackColor = true;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.label26);
            this.groupBox9.Controls.Add(this.label27);
            this.groupBox9.Controls.Add(this.label28);
            this.groupBox9.Controls.Add(this.label29);
            this.groupBox9.Controls.Add(this.label30);
            this.groupBox9.Controls.Add(this.label31);
            this.groupBox9.Location = new System.Drawing.Point(157, 75);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(180, 96);
            this.groupBox9.TabIndex = 90;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Current US Regulatory Standards";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(139, 71);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(19, 13);
            this.label26.TabIndex = 5;
            this.label26.Text = "61";
            this.label26.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(133, 48);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(25, 13);
            this.label27.TabIndex = 4;
            this.label27.Text = "104";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(133, 26);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(25, 13);
            this.label28.TabIndex = 3;
            this.label28.Text = "235";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(6, 71);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(117, 13);
            this.label29.TabIndex = 2;
            this.label29.Text = "Enterococci, Saltwater:";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(6, 48);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(125, 13);
            this.label30.TabIndex = 1;
            this.label30.Text = "Enterococci, Freshwater:";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(6, 26);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(97, 13);
            this.label31.TabIndex = 0;
            this.label31.Text = "E. coli, Freshwater:";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label25.Location = new System.Drawing.Point(6, 75);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(114, 13);
            this.label25.TabIndex = 83;
            this.label25.Text = "Dependent Variable is:";
            this.label25.Visible = false;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.ForeColor = System.Drawing.Color.OliveDrab;
            this.label23.Location = new System.Drawing.Point(91, 47);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(148, 13);
            this.label23.TabIndex = 82;
            this.label23.Text = "Regulatory Standard (Vertical)";
            // 
            // tbRegThreshVert
            // 
            this.tbRegThreshVert.Location = new System.Drawing.Point(52, 43);
            this.tbRegThreshVert.Name = "tbRegThreshVert";
            this.tbRegThreshVert.Size = new System.Drawing.Size(33, 20);
            this.tbRegThreshVert.TabIndex = 82;
            this.tbRegThreshVert.Text = "235";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.ForeColor = System.Drawing.Color.Crimson;
            this.label24.Location = new System.Drawing.Point(91, 23);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(145, 13);
            this.label24.TabIndex = 81;
            this.label24.Text = "Decision Criterion (Horizontal)";
            // 
            // tbDecThreshHoriz
            // 
            this.tbDecThreshHoriz.Location = new System.Drawing.Point(52, 18);
            this.tbDecThreshHoriz.Name = "tbDecThreshHoriz";
            this.tbDecThreshHoriz.Size = new System.Drawing.Size(33, 20);
            this.tbDecThreshHoriz.TabIndex = 13;
            this.tbDecThreshHoriz.Text = "235";
            // 
            // lblMaxAndRecommended
            // 
            this.lblMaxAndRecommended.AutoSize = true;
            this.lblMaxAndRecommended.Location = new System.Drawing.Point(68, 79);
            this.lblMaxAndRecommended.Name = "lblMaxAndRecommended";
            this.lblMaxAndRecommended.Size = new System.Drawing.Size(199, 13);
            this.lblMaxAndRecommended.TabIndex = 88;
            this.lblMaxAndRecommended.Text = "Available {0} Recommended {1} Max {2}";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(68, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(201, 13);
            this.label2.TabIndex = 87;
            this.label2.Text = "Maximum Number of Variables in a Model";
            // 
            // txtMaxVars
            // 
            this.txtMaxVars.Location = new System.Drawing.Point(24, 65);
            this.txtMaxVars.Name = "txtMaxVars";
            this.txtMaxVars.Size = new System.Drawing.Size(38, 20);
            this.txtMaxVars.TabIndex = 86;
            this.txtMaxVars.Text = "2";
            // 
            // txtMaxVIF
            // 
            this.txtMaxVIF.Location = new System.Drawing.Point(24, 107);
            this.txtMaxVIF.Name = "txtMaxVIF";
            this.txtMaxVIF.Size = new System.Drawing.Size(38, 20);
            this.txtMaxVIF.TabIndex = 85;
            this.txtMaxVIF.Text = "5";
            // 
            // lblMaxVIF
            // 
            this.lblMaxVIF.AutoSize = true;
            this.lblMaxVIF.Location = new System.Drawing.Point(68, 111);
            this.lblMaxVIF.Name = "lblMaxVIF";
            this.lblMaxVIF.Size = new System.Drawing.Size(70, 13);
            this.lblMaxVIF.TabIndex = 84;
            this.lblMaxVIF.Text = "Maximum VIF";
            // 
            // cbCriteria
            // 
            this.cbCriteria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCriteria.FormattingEnabled = true;
            this.cbCriteria.Items.AddRange(new object[] {
            "Akaike Information Criterion (AIC)",
            "Corrected Akaike Information Criterion (AICC)",
            "R Squared",
            "Adjusted R Squared",
            "PRESS",
            "Bayesian Information Criterion (BIC)",
            "Root Mean Square Error (RMSE)",
            "Sensitivity",
            "Specificity",
            "Accuracy"});
            this.cbCriteria.Location = new System.Drawing.Point(23, 26);
            this.cbCriteria.Name = "cbCriteria";
            this.cbCriteria.Size = new System.Drawing.Size(259, 21);
            this.cbCriteria.TabIndex = 83;
            // 
            // PLS
            // 
            this.PLS.Controls.Add(this.ipyPLSControl);
            this.PLS.Location = new System.Drawing.Point(4, 22);
            this.PLS.Name = "PLS";
            this.PLS.Padding = new System.Windows.Forms.Padding(3);
            this.PLS.Size = new System.Drawing.Size(1094, 619);
            this.PLS.TabIndex = 2;
            this.PLS.Text = "PLS";
            this.PLS.UseVisualStyleBackColor = true;
            this.PLS.Enter += new System.EventHandler(ipyPLSControl.TabPageEntered);

            // 
            // ipyPLSControl
            // 
            this.ipyPLSControl.BackColor = System.Drawing.SystemColors.Control;
            this.ipyPLSControl.Cursor = System.Windows.Forms.Cursors.Default;
            this.ipyPLSControl.IronPythonInterface = null;
            this.ipyPLSControl.Location = new System.Drawing.Point(0, 0);
            this.ipyPLSControl.Method = "PLS";
            this.ipyPLSControl.Name = "ipyPLSControl";
            this.ipyPLSControl.Size = new System.Drawing.Size(1094, 616);
            this.ipyPLSControl.TabIndex = 0;
            this.ipyPLSControl.LogMessageSent += new IPyModelingControl.IPyModelingControl.EventHandler<IPyModelingControl.LogMessageEvent>(this.HandleMessageForLog);
            this.ipyPLSControl.MessageSent += new IPyModelingControl.IPyModelingControl.EventHandler<IPyModelingControl.MessageEvent>(this.HandleMessageForManager);
            this.ipyPLSControl.ModelSaveRequested += new System.EventHandler(this.HandleSaveIPyModelRequest);
            this.ipyPLSControl.ModelingComplete += new System.EventHandler(this.CompletedIPyModeling);
            this.ipyPLSControl.DataRequested += new IPyModelingControl.IPyModelingControl.EventHandler<IPyModelingControl.ModelingCallback>(this.ProvideData);
            this.ipyPLSControl.IronPythonInterfaceRequested += new System.EventHandler(this.HandleRequestForInterface);
            // 
            // GBM
            // 
            this.GBM.Controls.Add(this.ipyGBMControl);
            this.GBM.Location = new System.Drawing.Point(4, 22);
            this.GBM.Name = "GBM";
            this.GBM.Size = new System.Drawing.Size(1094, 619);
            this.GBM.TabIndex = 3;
            this.GBM.Text = "GBM";
            this.GBM.UseVisualStyleBackColor = true;
            this.GBM.Enter += new System.EventHandler(ipyGBMControl.TabPageEntered);
            // 
            // ipyGBMControl
            // 
            this.ipyGBMControl.BackColor = System.Drawing.SystemColors.Control;
            this.ipyGBMControl.IronPythonInterface = null;
            this.ipyGBMControl.Location = new System.Drawing.Point(0, 0);
            this.ipyGBMControl.Method = "GBM";
            this.ipyGBMControl.Name = "ipyGBMControl";
            this.ipyGBMControl.Size = new System.Drawing.Size(1094, 616);
            this.ipyGBMControl.TabIndex = 0;
            this.ipyGBMControl.LogMessageSent += new IPyModelingControl.IPyModelingControl.EventHandler<IPyModelingControl.LogMessageEvent>(this.HandleMessageForLog);
            this.ipyGBMControl.MessageSent += new IPyModelingControl.IPyModelingControl.EventHandler<IPyModelingControl.MessageEvent>(this.HandleMessageForManager);
            this.ipyGBMControl.ModelSaveRequested += new System.EventHandler(this.HandleSaveIPyModelRequest);
            this.ipyGBMControl.ModelingComplete += new System.EventHandler(this.CompletedIPyModeling);
            this.ipyGBMControl.DataRequested += new IPyModelingControl.IPyModelingControl.EventHandler<IPyModelingControl.ModelingCallback>(this.ProvideData);
            this.ipyGBMControl.IronPythonInterfaceRequested += new System.EventHandler(this.HandleRequestForInterface);
            // 
            // frmModel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1120, 660);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpProvider1.SetHelpString(this, "");
            this.Name = "frmModel";
            this.helpProvider1.SetShowHelp(this, false);
            this.Text = "Modeling";
            this.Activated += new System.EventHandler(this.frmModel_Activated);
            this.Load += new System.EventHandler(this.frmModel_Load);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.frmModel_HelpRequested);
            this.Validating += new System.ComponentModel.CancelEventHandler(this.frmModel_Validating);
            this.tabControl2.ResumeLayout(false);
            this.tabProgress.ResumeLayout(false);
            this.tabResults.ResumeLayout(false);
            this.tabObsPred.ResumeLayout(false);
            this.tabROC.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tabStats.ResumeLayout(false);
            this.tabVariableStats.ResumeLayout(false);
            this.tabModelStats.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tabControlModelGeneration.ResumeLayout(false);
            this.tabManual.ResumeLayout(false);
            this.tabManual.PerformLayout();
            this.tabGA.ResumeLayout(false);
            this.tabGA.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.PLS.ResumeLayout(false);
            this.GBM.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label lblAvailVars;
        private System.Windows.Forms.Label lblDepVars;
        private System.Windows.Forms.Button btnRemoveInputVariable;
        private System.Windows.Forms.Button btnAddInputVariable;
        private System.Windows.Forms.Label lbDepVarName;
        private System.Windows.Forms.Label lblDepVariable;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ListBox lbIndVariables;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ListBox lbAvailableVariables;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btnCrossValidation;
        private System.Windows.Forms.TabControl tabStats;
        private System.Windows.Forms.TabPage tabVariableStats;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.TabPage tabModelStats;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnClearList;
        private System.Windows.Forms.Button btnAddtoList;
        private System.Windows.Forms.Button btnViewReport;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabProgress;
        private ZedGraph.ZedGraphControl zedGraphControl1;
        private System.Windows.Forms.TabPage tabResults;
        private ZedGraph.ZedGraphControl zedGraphControl2;
        private System.Windows.Forms.TabPage tabObsPred;
        private VBControls.MLRPredObs mlrPredObs1;
        private System.Windows.Forms.TabPage tabROC;
        private System.Windows.Forms.Button btnView;
        private System.Windows.Forms.ListView listView4;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.ColumnHeader columnHeader14;
        private System.Windows.Forms.ColumnHeader columnHeader15;
        private System.Windows.Forms.Button btnROCPlot;
        private System.Windows.Forms.ListView listView3;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private ZedGraph.ZedGraphControl zgcROC;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.TabControl tabControlModelGeneration;
        private System.Windows.Forms.TabPage tabManual;
        private System.Windows.Forms.CheckBox chkAllCombinations;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TabPage tabGA;
        private System.Windows.Forms.TextBox txtSeed;
        private System.Windows.Forms.CheckBox chkSeed;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtCrossoverRate;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtMutRate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtNumGen;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtPopSize;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.TextBox txtPwrValMET;
        private System.Windows.Forms.RadioButton rbPwrValMET;
        private System.Windows.Forms.RadioButton rbLogeValMET;
        private System.Windows.Forms.RadioButton rbValMET;
        private System.Windows.Forms.RadioButton rbLog10ValMET;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox tbRegThreshVert;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox tbDecThreshHoriz;
        private System.Windows.Forms.Label lblMaxAndRecommended;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMaxVars;
        private System.Windows.Forms.TextBox txtMaxVIF;
        private System.Windows.Forms.Label lblMaxVIF;
        private System.Windows.Forms.ComboBox cbCriteria;
        private System.Windows.Forms.Label lblNumObs;
        private System.Windows.Forms.TabPage PLS;
        public IPyModelingControl.IPyPLSControl ipyPLSControl;
        private System.Windows.Forms.TabPage GBM;
        public IPyModelingControl.IPyGBMControl ipyGBMControl;
    }
}