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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.lblNumObs = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
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
            this.lblnModels = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabProgress = new System.Windows.Forms.TabPage();
            this.zedGraphControl1 = new ZedGraph.ZedGraphControl();
            this.tabResults = new System.Windows.Forms.TabPage();
            this.zedGraphControl2 = new ZedGraph.ZedGraphControl();
            this.tabObsPred2 = new System.Windows.Forms.TabPage();
            this.mlrPlots1 = new VBControls.MLRPlots();
            this.tabROC = new System.Windows.Forms.TabPage();
            this.btnView = new System.Windows.Forms.Button();
            this.listView4 = new System.Windows.Forms.ListView();
            this.colDC = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFN = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFP = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTot = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSens = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSpec = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colAcc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnROCPlot = new System.Windows.Forms.Button();
            this.listView3 = new System.Windows.Forms.ListView();
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.zgcROC = new ZedGraph.ZedGraphControl();
            this.tabResiduals = new System.Windows.Forms.TabPage();
            this.gboxCtrls = new System.Windows.Forms.GroupBox();
            this.gboxResiduals = new System.Windows.Forms.GroupBox();
            this.rbDFFITS = new System.Windows.Forms.RadioButton();
            this.rbCooks = new System.Windows.Forms.RadioButton();
            this.gboxView = new System.Windows.Forms.GroupBox();
            this.rbPlot = new System.Windows.Forms.RadioButton();
            this.rbTable = new System.Windows.Forms.RadioButton();
            this.gboxRebuilds = new System.Windows.Forms.GroupBox();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.tabControl3 = new System.Windows.Forms.TabControl();
            this.tabResidVSFit = new System.Windows.Forms.TabPage();
            this.lblADPVal = new System.Windows.Forms.Label();
            this.lblADStat = new System.Windows.Forms.Label();
            this.zgcResidvFitted = new ZedGraph.ZedGraphControl();
            this.tabFitVSObs = new System.Windows.Forms.TabPage();
            this.mlrPlots2 = new VBControls.MLRPlots();
            this.tabDFFITSCooks = new System.Windows.Forms.TabPage();
            this.gboxResidTable = new System.Windows.Forms.GroupBox();
            this.zgcResidualsPlot = new ZedGraph.ZedGraphControl();
            this.dgvResid = new System.Windows.Forms.DataGridView();
            this.btnViewDataTable = new System.Windows.Forms.Button();
            this.gboxAutoCtrls = new System.Windows.Forms.GroupBox();
            this.tboxAutoConstantThresholdValue = new System.Windows.Forms.TextBox();
            this.rbAutoConstantThreshold = new System.Windows.Forms.RadioButton();
            this.rbAutoIterativeThreshold = new System.Windows.Forms.RadioButton();
            this.lblAutoThreshold = new System.Windows.Forms.Label();
            this.btnGoAuto = new System.Windows.Forms.Button();
            this.gboxIterativeCtrls = new System.Windows.Forms.GroupBox();
            this.lblIterativeThreshold = new System.Windows.Forms.Label();
            this.btnGoIterative = new System.Windows.Forms.Button();
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
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox6.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabControlModelGeneration.SuspendLayout();
            this.tabManual.SuspendLayout();
            this.tabGA.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabProgress.SuspendLayout();
            this.tabResults.SuspendLayout();
            this.tabObsPred2.SuspendLayout();
            this.tabROC.SuspendLayout();
            this.tabResiduals.SuspendLayout();
            this.gboxCtrls.SuspendLayout();
            this.gboxResiduals.SuspendLayout();
            this.gboxView.SuspendLayout();
            this.gboxRebuilds.SuspendLayout();
            this.tabControl3.SuspendLayout();
            this.tabResidVSFit.SuspendLayout();
            this.tabFitVSObs.SuspendLayout();
            this.tabDFFITSCooks.SuspendLayout();
            this.gboxResidTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResid)).BeginInit();
            this.gboxAutoCtrls.SuspendLayout();
            this.gboxIterativeCtrls.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabStats.SuspendLayout();
            this.tabVariableStats.SuspendLayout();
            this.tabModelStats.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // groupBox6
            // 
            this.groupBox6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox6.Controls.Add(this.lblNumObs);
            this.groupBox6.Controls.Add(this.tabControl1);
            this.groupBox6.Controls.Add(this.lblnModels);
            this.groupBox6.Location = new System.Drawing.Point(12, 12);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(374, 628);
            this.groupBox6.TabIndex = 50;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Model Settings";
            // 
            // lblNumObs
            // 
            this.lblNumObs.AutoSize = true;
            this.lblNumObs.Location = new System.Drawing.Point(217, 21);
            this.lblNumObs.Name = "lblNumObs";
            this.lblNumObs.Size = new System.Drawing.Size(141, 13);
            this.lblNumObs.TabIndex = 67;
            this.lblNumObs.Text = "Number of Observations: {0}";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(6, 19);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(362, 595);
            this.tabControl1.TabIndex = 66;
            // 
            // tabPage2
            // 
            this.tabPage2.AutoScroll = true;
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.btnRun);
            this.tabPage2.Controls.Add(this.tabControlModelGeneration);
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(354, 569);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Control Options";
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
            this.tabControlModelGeneration.SelectedIndexChanged += new System.EventHandler(this.tabControlModelGeneration_SelectedIndexChanged);
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
            this.chkSeed.CheckedChanged += new System.EventHandler(this.chkSeed_CheckedChanged);
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
            this.txtCrossoverRate.Validating += new System.ComponentModel.CancelEventHandler(this.txtCrossoverRate_Validating);
            this.txtCrossoverRate.Validated += new System.EventHandler(this.txtCrossoverRate_Validated);
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
            this.txtMutRate.Validating += new System.ComponentModel.CancelEventHandler(this.txtMutRate_Validating);
            this.txtMutRate.Validated += new System.EventHandler(this.txtMutRate_Validated);
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
            this.txtNumGen.Validating += new System.ComponentModel.CancelEventHandler(this.txtNumGen_Validating);
            this.txtNumGen.Validated += new System.EventHandler(this.txtNumGen_Validated);
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
            this.txtPopSize.Validating += new System.ComponentModel.CancelEventHandler(this.txtPopSize_Validating);
            this.txtPopSize.Validated += new System.EventHandler(this.txtPopSize_Validated);
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
            this.groupBox4.Location = new System.Drawing.Point(0, 10);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(351, 330);
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
            this.txtPwrValMET.Leave += new System.EventHandler(this.txtPwr_Leave);
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
            this.rbPwrValMET.CheckedChanged += new System.EventHandler(this.rbPwrValMET_CheckedChanged);
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
            this.rbLogeValMET.CheckedChanged += new System.EventHandler(this.rbLogeValMET_CheckedChanged);
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
            this.rbValMET.CheckedChanged += new System.EventHandler(this.rbValMET_CheckedChanged);
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
            this.rbLog10ValMET.CheckedChanged += new System.EventHandler(this.rbLog10ValMET_CheckedChanged);
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
            this.label26.Location = new System.Drawing.Point(135, 70);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(25, 13);
            this.label26.TabIndex = 5;
            this.label26.Text = "104";
            this.label26.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(135, 48);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(19, 13);
            this.label27.TabIndex = 4;
            this.label27.Text = "61";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(135, 26);
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
            this.label23.ForeColor = System.Drawing.Color.Green;
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
            this.label24.ForeColor = System.Drawing.Color.Blue;
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
            // lblnModels
            // 
            this.lblnModels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblnModels.AutoSize = true;
            this.lblnModels.Location = new System.Drawing.Point(226, 604);
            this.lblnModels.Name = "lblnModels";
            this.lblnModels.Size = new System.Drawing.Size(0, 13);
            this.lblnModels.TabIndex = 65;
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
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(18, 34);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(141, 95);
            this.listBox1.TabIndex = 48;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // tabControl2
            // 
            this.tabControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl2.Controls.Add(this.tabProgress);
            this.tabControl2.Controls.Add(this.tabResults);
            this.tabControl2.Controls.Add(this.tabObsPred2);
            this.tabControl2.Controls.Add(this.tabROC);
            this.tabControl2.Controls.Add(this.tabResiduals);
            this.tabControl2.Location = new System.Drawing.Point(21, 242);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.helpProvider1.SetShowHelp(this.tabControl2, false);
            this.tabControl2.Size = new System.Drawing.Size(685, 370);
            this.tabControl2.TabIndex = 50;
            this.tabControl2.SelectedIndexChanged += new System.EventHandler(this.tabControl2_SelectedIndexChanged);
            // 
            // tabProgress
            // 
            this.tabProgress.AutoScroll = true;
            this.tabProgress.Controls.Add(this.zedGraphControl1);
            this.tabProgress.Location = new System.Drawing.Point(4, 22);
            this.tabProgress.Name = "tabProgress";
            this.tabProgress.Padding = new System.Windows.Forms.Padding(3);
            this.tabProgress.Size = new System.Drawing.Size(677, 344);
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
            this.zedGraphControl1.Size = new System.Drawing.Size(671, 338);
            this.zedGraphControl1.TabIndex = 3;
            // 
            // tabResults
            // 
            this.tabResults.AutoScroll = true;
            this.tabResults.Controls.Add(this.zedGraphControl2);
            this.tabResults.Location = new System.Drawing.Point(4, 22);
            this.tabResults.Name = "tabResults";
            this.tabResults.Padding = new System.Windows.Forms.Padding(3);
            this.tabResults.Size = new System.Drawing.Size(677, 344);
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
            this.zedGraphControl2.Size = new System.Drawing.Size(671, 338);
            this.zedGraphControl2.TabIndex = 4;
            // 
            // tabObsPred2
            // 
            this.tabObsPred2.AutoScroll = true;
            this.tabObsPred2.Controls.Add(this.mlrPlots1);
            this.tabObsPred2.Location = new System.Drawing.Point(4, 22);
            this.tabObsPred2.Name = "tabObsPred2";
            this.tabObsPred2.Size = new System.Drawing.Size(677, 344);
            this.tabObsPred2.TabIndex = 4;
            this.tabObsPred2.Text = "Fitted vs Observed";
            this.tabObsPred2.UseVisualStyleBackColor = true;
            // 
            // mlrPlots1
            // 
            this.mlrPlots1.AutoScroll = true;
            this.mlrPlots1.DependentVarXFrm = VBCommon.Transforms.DependentVariableTransforms.none;
            this.mlrPlots1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mlrPlots1.Exceedances = null;
            this.mlrPlots1.Location = new System.Drawing.Point(0, 0);
            this.mlrPlots1.Name = "mlrPlots1";
            this.mlrPlots1.PowerExponent = double.NaN;
            this.mlrPlots1.PowerTransformExponent = double.NaN;
            this.mlrPlots1.Size = new System.Drawing.Size(677, 344);
            this.mlrPlots1.TabIndex = 0;
            this.mlrPlots1.Transform = VBCommon.Transforms.DependentVariableTransforms.none;
            // 
            // tabROC
            // 
            this.tabROC.AutoScroll = true;
            this.tabROC.Controls.Add(this.btnView);
            this.tabROC.Controls.Add(this.listView4);
            this.tabROC.Controls.Add(this.btnROCPlot);
            this.tabROC.Controls.Add(this.listView3);
            this.tabROC.Controls.Add(this.zgcROC);
            this.helpProvider1.SetHelpKeyword(this.tabROC, "");
            this.tabROC.Location = new System.Drawing.Point(4, 22);
            this.tabROC.Name = "tabROC";
            this.helpProvider1.SetShowHelp(this.tabROC, false);
            this.tabROC.Size = new System.Drawing.Size(677, 344);
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
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // listView4
            // 
            this.listView4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView4.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colDC,
            this.colFN,
            this.colFP,
            this.colTot,
            this.colSens,
            this.colSpec,
            this.colAcc});
            this.listView4.FullRowSelect = true;
            this.listView4.GridLines = true;
            this.listView4.Location = new System.Drawing.Point(172, 8);
            this.listView4.Name = "listView4";
            this.listView4.Size = new System.Drawing.Size(502, 306);
            this.listView4.TabIndex = 53;
            this.listView4.UseCompatibleStateImageBehavior = false;
            this.listView4.View = System.Windows.Forms.View.Details;
            this.listView4.Visible = false;
            this.listView4.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView4_KeyDown);
            // 
            // colDC
            // 
            this.colDC.Text = "Decision Criterion";
            this.colDC.Width = 100;
            // 
            // colFN
            // 
            this.colFN.Text = "False Non-Exceed";
            this.colFN.Width = 120;
            // 
            // colFP
            // 
            this.colFP.Text = "False Exceed";
            this.colFP.Width = 120;
            // 
            // colTot
            // 
            this.colTot.Text = "Total";
            this.colTot.Width = 50;
            // 
            // colSens
            // 
            this.colSens.Text = "Sensitivity";
            this.colSens.Width = 75;
            // 
            // colSpec
            // 
            this.colSpec.Text = "Specificity";
            this.colSpec.Width = 75;
            // 
            // colAcc
            // 
            this.colAcc.Text = "Accuracy";
            this.colAcc.Width = 75;
            // 
            // btnROCPlot
            // 
            this.btnROCPlot.Location = new System.Drawing.Point(46, 217);
            this.btnROCPlot.Name = "btnROCPlot";
            this.btnROCPlot.Size = new System.Drawing.Size(75, 23);
            this.btnROCPlot.TabIndex = 52;
            this.btnROCPlot.Text = "Plot";
            this.btnROCPlot.UseVisualStyleBackColor = true;
            this.btnROCPlot.Click += new System.EventHandler(this.btnROCPlot_Click);
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
            this.listView3.SelectedIndexChanged += new System.EventHandler(this.listView3_SelectedIndexChanged);
            this.listView3.Leave += new System.EventHandler(this.listView3_Leave);
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
            // tabResiduals
            // 
            this.tabResiduals.Controls.Add(this.gboxCtrls);
            this.tabResiduals.Controls.Add(this.gboxRebuilds);
            this.tabResiduals.Controls.Add(this.tabControl3);
            this.tabResiduals.Location = new System.Drawing.Point(4, 22);
            this.tabResiduals.Name = "tabResiduals";
            this.tabResiduals.Size = new System.Drawing.Size(677, 344);
            this.tabResiduals.TabIndex = 5;
            this.tabResiduals.Text = "Residuals";
            this.tabResiduals.UseVisualStyleBackColor = true;
            // 
            // gboxCtrls
            // 
            this.gboxCtrls.Controls.Add(this.gboxResiduals);
            this.gboxCtrls.Controls.Add(this.gboxView);
            this.gboxCtrls.Location = new System.Drawing.Point(3, 173);
            this.gboxCtrls.Name = "gboxCtrls";
            this.gboxCtrls.Size = new System.Drawing.Size(121, 157);
            this.gboxCtrls.TabIndex = 2;
            this.gboxCtrls.TabStop = false;
            this.gboxCtrls.Visible = false;
            // 
            // gboxResiduals
            // 
            this.gboxResiduals.Controls.Add(this.rbDFFITS);
            this.gboxResiduals.Controls.Add(this.rbCooks);
            this.gboxResiduals.Location = new System.Drawing.Point(6, 92);
            this.gboxResiduals.Name = "gboxResiduals";
            this.gboxResiduals.Size = new System.Drawing.Size(96, 59);
            this.gboxResiduals.TabIndex = 4;
            this.gboxResiduals.TabStop = false;
            this.gboxResiduals.Text = "Residuals";
            // 
            // rbDFFITS
            // 
            this.rbDFFITS.AutoSize = true;
            this.rbDFFITS.Checked = true;
            this.rbDFFITS.Location = new System.Drawing.Point(27, 19);
            this.rbDFFITS.Name = "rbDFFITS";
            this.rbDFFITS.Size = new System.Drawing.Size(62, 17);
            this.rbDFFITS.TabIndex = 3;
            this.rbDFFITS.TabStop = true;
            this.rbDFFITS.Text = "DFFITS";
            this.rbDFFITS.UseVisualStyleBackColor = true;
            this.rbDFFITS.CheckedChanged += new System.EventHandler(this.rbDFFITS_CheckedChanged);
            // 
            // rbCooks
            // 
            this.rbCooks.AutoSize = true;
            this.rbCooks.Location = new System.Drawing.Point(27, 36);
            this.rbCooks.Name = "rbCooks";
            this.rbCooks.Size = new System.Drawing.Size(57, 17);
            this.rbCooks.TabIndex = 2;
            this.rbCooks.TabStop = true;
            this.rbCooks.Text = "Cook\'s";
            this.rbCooks.UseVisualStyleBackColor = true;
            this.rbCooks.CheckedChanged += new System.EventHandler(this.rbCooks_CheckedChanged);
            // 
            // gboxView
            // 
            this.gboxView.Controls.Add(this.rbPlot);
            this.gboxView.Controls.Add(this.rbTable);
            this.gboxView.Location = new System.Drawing.Point(6, 19);
            this.gboxView.Name = "gboxView";
            this.gboxView.Size = new System.Drawing.Size(96, 59);
            this.gboxView.TabIndex = 3;
            this.gboxView.TabStop = false;
            this.gboxView.Text = "View";
            // 
            // rbPlot
            // 
            this.rbPlot.AutoSize = true;
            this.rbPlot.Location = new System.Drawing.Point(18, 36);
            this.rbPlot.Name = "rbPlot";
            this.rbPlot.Size = new System.Drawing.Size(43, 17);
            this.rbPlot.TabIndex = 1;
            this.rbPlot.TabStop = true;
            this.rbPlot.Text = "Plot";
            this.rbPlot.UseVisualStyleBackColor = true;
            this.rbPlot.CheckedChanged += new System.EventHandler(this.rbPlot_CheckedChanged);
            // 
            // rbTable
            // 
            this.rbTable.AutoSize = true;
            this.rbTable.Checked = true;
            this.rbTable.Location = new System.Drawing.Point(18, 19);
            this.rbTable.Name = "rbTable";
            this.rbTable.Size = new System.Drawing.Size(52, 17);
            this.rbTable.TabIndex = 0;
            this.rbTable.TabStop = true;
            this.rbTable.Text = "Table";
            this.rbTable.UseVisualStyleBackColor = true;
            this.rbTable.CheckedChanged += new System.EventHandler(this.rbTable_CheckedChanged);
            // 
            // gboxRebuilds
            // 
            this.gboxRebuilds.Controls.Add(this.listBox2);
            this.gboxRebuilds.Controls.Add(this.btnClear);
            this.gboxRebuilds.Location = new System.Drawing.Point(3, 3);
            this.gboxRebuilds.Name = "gboxRebuilds";
            this.gboxRebuilds.Size = new System.Drawing.Size(121, 160);
            this.gboxRebuilds.TabIndex = 1;
            this.gboxRebuilds.TabStop = false;
            this.gboxRebuilds.Text = "Rebuilds";
            // 
            // listBox2
            // 
            this.listBox2.FormattingEnabled = true;
            this.listBox2.Location = new System.Drawing.Point(6, 19);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(100, 95);
            this.listBox2.TabIndex = 49;
            this.listBox2.SelectedIndexChanged += new System.EventHandler(this.listBox2_SelectedIndexChanged);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(24, 131);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 0;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // tabControl3
            // 
            this.tabControl3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl3.Controls.Add(this.tabResidVSFit);
            this.tabControl3.Controls.Add(this.tabFitVSObs);
            this.tabControl3.Controls.Add(this.tabDFFITSCooks);
            this.tabControl3.Location = new System.Drawing.Point(130, 10);
            this.tabControl3.Name = "tabControl3";
            this.tabControl3.SelectedIndex = 0;
            this.tabControl3.Size = new System.Drawing.Size(534, 320);
            this.tabControl3.TabIndex = 0;
            this.tabControl3.SelectedIndexChanged += new System.EventHandler(this.tabControl3_SelectedIndexChanged);
            // 
            // tabResidVSFit
            // 
            this.tabResidVSFit.Controls.Add(this.lblADPVal);
            this.tabResidVSFit.Controls.Add(this.lblADStat);
            this.tabResidVSFit.Controls.Add(this.zgcResidvFitted);
            this.tabResidVSFit.Location = new System.Drawing.Point(4, 22);
            this.tabResidVSFit.Name = "tabResidVSFit";
            this.tabResidVSFit.Padding = new System.Windows.Forms.Padding(3);
            this.tabResidVSFit.Size = new System.Drawing.Size(526, 294);
            this.tabResidVSFit.TabIndex = 0;
            this.tabResidVSFit.Text = "Residuals vs Fitted";
            this.tabResidVSFit.UseVisualStyleBackColor = true;
            // 
            // lblADPVal
            // 
            this.lblADPVal.AutoSize = true;
            this.lblADPVal.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.lblADPVal.Location = new System.Drawing.Point(10, 20);
            this.lblADPVal.Name = "lblADPVal";
            this.lblADPVal.Size = new System.Drawing.Size(35, 13);
            this.lblADPVal.TabIndex = 2;
            this.lblADPVal.Text = "label4";
            // 
            // lblADStat
            // 
            this.lblADStat.AutoSize = true;
            this.lblADStat.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.lblADStat.Location = new System.Drawing.Point(10, 7);
            this.lblADStat.Name = "lblADStat";
            this.lblADStat.Size = new System.Drawing.Size(35, 13);
            this.lblADStat.TabIndex = 1;
            this.lblADStat.Text = "label3";
            // 
            // zgcResidvFitted
            // 
            this.zgcResidvFitted.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zgcResidvFitted.Location = new System.Drawing.Point(3, 3);
            this.zgcResidvFitted.Name = "zgcResidvFitted";
            this.zgcResidvFitted.ScrollGrace = 0D;
            this.zgcResidvFitted.ScrollMaxX = 0D;
            this.zgcResidvFitted.ScrollMaxY = 0D;
            this.zgcResidvFitted.ScrollMaxY2 = 0D;
            this.zgcResidvFitted.ScrollMinX = 0D;
            this.zgcResidvFitted.ScrollMinY = 0D;
            this.zgcResidvFitted.ScrollMinY2 = 0D;
            this.zgcResidvFitted.Size = new System.Drawing.Size(520, 288);
            this.zgcResidvFitted.TabIndex = 0;
            // 
            // tabFitVSObs
            // 
            this.tabFitVSObs.Controls.Add(this.mlrPlots2);
            this.tabFitVSObs.Location = new System.Drawing.Point(4, 22);
            this.tabFitVSObs.Name = "tabFitVSObs";
            this.tabFitVSObs.Padding = new System.Windows.Forms.Padding(3);
            this.tabFitVSObs.Size = new System.Drawing.Size(526, 294);
            this.tabFitVSObs.TabIndex = 1;
            this.tabFitVSObs.Text = "Fitted vs Observed";
            this.tabFitVSObs.UseVisualStyleBackColor = true;
            // 
            // mlrPlots2
            // 
            this.mlrPlots2.DependentVarXFrm = VBCommon.Transforms.DependentVariableTransforms.none;
            this.mlrPlots2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mlrPlots2.Exceedances = null;
            this.mlrPlots2.Location = new System.Drawing.Point(3, 3);
            this.mlrPlots2.Name = "mlrPlots2";
            this.mlrPlots2.PowerExponent = double.NaN;
            this.mlrPlots2.PowerTransformExponent = double.NaN;
            this.mlrPlots2.Size = new System.Drawing.Size(520, 288);
            this.mlrPlots2.TabIndex = 0;
            this.mlrPlots2.Transform = VBCommon.Transforms.DependentVariableTransforms.none;
            // 
            // tabDFFITSCooks
            // 
            this.tabDFFITSCooks.Controls.Add(this.gboxResidTable);
            this.tabDFFITSCooks.Location = new System.Drawing.Point(4, 22);
            this.tabDFFITSCooks.Name = "tabDFFITSCooks";
            this.tabDFFITSCooks.Size = new System.Drawing.Size(526, 294);
            this.tabDFFITSCooks.TabIndex = 2;
            this.tabDFFITSCooks.Text = "DFFITS/Cooks";
            this.tabDFFITSCooks.UseVisualStyleBackColor = true;
            // 
            // gboxResidTable
            // 
            this.gboxResidTable.Controls.Add(this.zgcResidualsPlot);
            this.gboxResidTable.Controls.Add(this.dgvResid);
            this.gboxResidTable.Controls.Add(this.btnViewDataTable);
            this.gboxResidTable.Controls.Add(this.gboxAutoCtrls);
            this.gboxResidTable.Controls.Add(this.gboxIterativeCtrls);
            this.gboxResidTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gboxResidTable.Location = new System.Drawing.Point(0, 0);
            this.gboxResidTable.Name = "gboxResidTable";
            this.gboxResidTable.Size = new System.Drawing.Size(526, 294);
            this.gboxResidTable.TabIndex = 0;
            this.gboxResidTable.TabStop = false;
            this.gboxResidTable.Text = "Residual Table";
            // 
            // zgcResidualsPlot
            // 
            this.zgcResidualsPlot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zgcResidualsPlot.Location = new System.Drawing.Point(3, 16);
            this.zgcResidualsPlot.Name = "zgcResidualsPlot";
            this.zgcResidualsPlot.ScrollGrace = 0D;
            this.zgcResidualsPlot.ScrollMaxX = 0D;
            this.zgcResidualsPlot.ScrollMaxY = 0D;
            this.zgcResidualsPlot.ScrollMaxY2 = 0D;
            this.zgcResidualsPlot.ScrollMinX = 0D;
            this.zgcResidualsPlot.ScrollMinY = 0D;
            this.zgcResidualsPlot.ScrollMinY2 = 0D;
            this.zgcResidualsPlot.Size = new System.Drawing.Size(520, 275);
            this.zgcResidualsPlot.TabIndex = 4;
            // 
            // dgvResid
            // 
            this.dgvResid.AllowUserToDeleteRows = false;
            this.dgvResid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvResid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle17.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle17.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle17.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle17.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle17.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvResid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle17;
            this.dgvResid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle18.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle18.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle18.Format = "N0";
            dataGridViewCellStyle18.NullValue = null;
            dataGridViewCellStyle18.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle18.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle18.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvResid.DefaultCellStyle = dataGridViewCellStyle18;
            this.dgvResid.Location = new System.Drawing.Point(345, 19);
            this.dgvResid.Name = "dgvResid";
            this.dgvResid.ReadOnly = true;
            dataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle19.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle19.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle19.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle19.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle19.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvResid.RowHeadersDefaultCellStyle = dataGridViewCellStyle19;
            dataGridViewCellStyle20.Format = "N4";
            dataGridViewCellStyle20.NullValue = null;
            this.dgvResid.RowsDefaultCellStyle = dataGridViewCellStyle20;
            this.dgvResid.RowTemplate.DefaultCellStyle.Format = "N0";
            this.dgvResid.RowTemplate.DefaultCellStyle.NullValue = null;
            this.dgvResid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvResid.Size = new System.Drawing.Size(175, 254);
            this.dgvResid.TabIndex = 3;
            // 
            // btnViewDataTable
            // 
            this.btnViewDataTable.Location = new System.Drawing.Point(122, 244);
            this.btnViewDataTable.Name = "btnViewDataTable";
            this.btnViewDataTable.Size = new System.Drawing.Size(75, 23);
            this.btnViewDataTable.TabIndex = 2;
            this.btnViewDataTable.Text = "View Data Table";
            this.btnViewDataTable.UseVisualStyleBackColor = true;
            this.btnViewDataTable.Click += new System.EventHandler(this.btnViewDataTable_Click);
            // 
            // gboxAutoCtrls
            // 
            this.gboxAutoCtrls.Controls.Add(this.tboxAutoConstantThresholdValue);
            this.gboxAutoCtrls.Controls.Add(this.rbAutoConstantThreshold);
            this.gboxAutoCtrls.Controls.Add(this.rbAutoIterativeThreshold);
            this.gboxAutoCtrls.Controls.Add(this.lblAutoThreshold);
            this.gboxAutoCtrls.Controls.Add(this.btnGoAuto);
            this.gboxAutoCtrls.Location = new System.Drawing.Point(17, 134);
            this.gboxAutoCtrls.Name = "gboxAutoCtrls";
            this.gboxAutoCtrls.Size = new System.Drawing.Size(326, 100);
            this.gboxAutoCtrls.TabIndex = 1;
            this.gboxAutoCtrls.TabStop = false;
            this.gboxAutoCtrls.Text = "Auto Rebuild";
            // 
            // tboxAutoConstantThresholdValue
            // 
            this.tboxAutoConstantThresholdValue.Location = new System.Drawing.Point(186, 61);
            this.tboxAutoConstantThresholdValue.Name = "tboxAutoConstantThresholdValue";
            this.tboxAutoConstantThresholdValue.Size = new System.Drawing.Size(60, 20);
            this.tboxAutoConstantThresholdValue.TabIndex = 5;
            // 
            // rbAutoConstantThreshold
            // 
            this.rbAutoConstantThreshold.AutoSize = true;
            this.rbAutoConstantThreshold.Location = new System.Drawing.Point(68, 64);
            this.rbAutoConstantThreshold.Name = "rbAutoConstantThreshold";
            this.rbAutoConstantThreshold.Size = new System.Drawing.Size(112, 17);
            this.rbAutoConstantThreshold.TabIndex = 4;
            this.rbAutoConstantThreshold.TabStop = true;
            this.rbAutoConstantThreshold.Text = "constant threshold";
            this.rbAutoConstantThreshold.UseVisualStyleBackColor = true;
            // 
            // rbAutoIterativeThreshold
            // 
            this.rbAutoIterativeThreshold.AutoSize = true;
            this.rbAutoIterativeThreshold.Checked = true;
            this.rbAutoIterativeThreshold.Location = new System.Drawing.Point(68, 41);
            this.rbAutoIterativeThreshold.Name = "rbAutoIterativeThreshold";
            this.rbAutoIterativeThreshold.Size = new System.Drawing.Size(254, 17);
            this.rbAutoIterativeThreshold.TabIndex = 3;
            this.rbAutoIterativeThreshold.TabStop = true;
            this.rbAutoIterativeThreshold.Text = "iterative threshold using <methodExp> = <value>";
            this.rbAutoIterativeThreshold.UseVisualStyleBackColor = true;
            // 
            // lblAutoThreshold
            // 
            this.lblAutoThreshold.AutoSize = true;
            this.lblAutoThreshold.Location = new System.Drawing.Point(60, 21);
            this.lblAutoThreshold.Name = "lblAutoThreshold";
            this.lblAutoThreshold.Size = new System.Drawing.Size(187, 13);
            this.lblAutoThreshold.TabIndex = 2;
            this.lblAutoThreshold.Text = "Stop when all <methodExp> less than:";
            // 
            // btnGoAuto
            // 
            this.btnGoAuto.Location = new System.Drawing.Point(6, 36);
            this.btnGoAuto.Name = "btnGoAuto";
            this.btnGoAuto.Size = new System.Drawing.Size(45, 23);
            this.btnGoAuto.TabIndex = 1;
            this.btnGoAuto.Text = "Go";
            this.btnGoAuto.UseVisualStyleBackColor = true;
            this.btnGoAuto.Click += new System.EventHandler(this.btnGoAuto_Click);
            // 
            // gboxIterativeCtrls
            // 
            this.gboxIterativeCtrls.Controls.Add(this.lblIterativeThreshold);
            this.gboxIterativeCtrls.Controls.Add(this.btnGoIterative);
            this.gboxIterativeCtrls.Location = new System.Drawing.Point(17, 21);
            this.gboxIterativeCtrls.Name = "gboxIterativeCtrls";
            this.gboxIterativeCtrls.Size = new System.Drawing.Size(326, 100);
            this.gboxIterativeCtrls.TabIndex = 0;
            this.gboxIterativeCtrls.TabStop = false;
            this.gboxIterativeCtrls.Text = "Iterative Rebuild";
            // 
            // lblIterativeThreshold
            // 
            this.lblIterativeThreshold.AutoSize = true;
            this.lblIterativeThreshold.Location = new System.Drawing.Point(72, 46);
            this.lblIterativeThreshold.Name = "lblIterativeThreshold";
            this.lblIterativeThreshold.Size = new System.Drawing.Size(122, 13);
            this.lblIterativeThreshold.TabIndex = 1;
            this.lblIterativeThreshold.Text = "<methodExp> = <value>";
            // 
            // btnGoIterative
            // 
            this.btnGoIterative.Location = new System.Drawing.Point(6, 41);
            this.btnGoIterative.Name = "btnGoIterative";
            this.btnGoIterative.Size = new System.Drawing.Size(45, 23);
            this.btnGoIterative.TabIndex = 0;
            this.btnGoIterative.Text = "Go";
            this.btnGoIterative.UseVisualStyleBackColor = true;
            this.btnGoIterative.Click += new System.EventHandler(this.btnGoIterative_Click);
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
            this.groupBox5.Location = new System.Drawing.Point(392, 21);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(720, 619);
            this.groupBox5.TabIndex = 47;
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
            this.tabStats.Size = new System.Drawing.Size(542, 219);
            this.tabStats.TabIndex = 60;
            // 
            // tabVariableStats
            // 
            this.tabVariableStats.Controls.Add(this.listView1);
            this.tabVariableStats.Location = new System.Drawing.Point(4, 22);
            this.tabVariableStats.Name = "tabVariableStats";
            this.tabVariableStats.Padding = new System.Windows.Forms.Padding(3);
            this.tabVariableStats.Size = new System.Drawing.Size(534, 193);
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
            this.listView1.Size = new System.Drawing.Size(528, 187);
            this.listView1.TabIndex = 50;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView1_KeyDown);
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
            this.tabModelStats.Size = new System.Drawing.Size(534, 193);
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
            this.listView2.Size = new System.Drawing.Size(528, 187);
            this.listView2.TabIndex = 58;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            this.listView2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView2_KeyDown);
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
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // frmModel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpProvider1.SetHelpString(this, "");
            this.Name = "frmModel";
            this.helpProvider1.SetShowHelp(this, false);
            this.Size = new System.Drawing.Size(1120, 660);
            this.Load += new System.EventHandler(this.frmModel_Load);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.frmModel_HelpRequested);
            this.Validating += new System.ComponentModel.CancelEventHandler(this.frmModel_Validating);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
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
            this.tabControl2.ResumeLayout(false);
            this.tabProgress.ResumeLayout(false);
            this.tabResults.ResumeLayout(false);
            this.tabObsPred2.ResumeLayout(false);
            this.tabROC.ResumeLayout(false);
            this.tabResiduals.ResumeLayout(false);
            this.gboxCtrls.ResumeLayout(false);
            this.gboxResiduals.ResumeLayout(false);
            this.gboxResiduals.PerformLayout();
            this.gboxView.ResumeLayout(false);
            this.gboxView.PerformLayout();
            this.gboxRebuilds.ResumeLayout(false);
            this.tabControl3.ResumeLayout(false);
            this.tabResidVSFit.ResumeLayout(false);
            this.tabResidVSFit.PerformLayout();
            this.tabFitVSObs.ResumeLayout(false);
            this.tabDFFITSCooks.ResumeLayout(false);
            this.gboxResidTable.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResid)).EndInit();
            this.gboxAutoCtrls.ResumeLayout(false);
            this.gboxAutoCtrls.PerformLayout();
            this.gboxIterativeCtrls.ResumeLayout(false);
            this.gboxIterativeCtrls.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tabStats.ResumeLayout(false);
            this.tabVariableStats.ResumeLayout(false);
            this.tabModelStats.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabProgress;
        private ZedGraph.ZedGraphControl zedGraphControl1;
        private System.Windows.Forms.TabPage tabResults;
        private ZedGraph.ZedGraphControl zedGraphControl2;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btnViewReport;
        private System.Windows.Forms.Label lblnModels;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnClearList;
        private System.Windows.Forms.Button btnAddtoList;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TabControl tabControlModelGeneration;
        private System.Windows.Forms.TabPage tabManual;
        private System.Windows.Forms.CheckBox chkAllCombinations;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TabPage tabGA;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtCrossoverRate;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtMutRate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtNumGen;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtPopSize;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.TextBox txtSeed;
        private System.Windows.Forms.CheckBox chkSeed;
        private System.Windows.Forms.ComboBox cbCriteria;
        private System.Windows.Forms.TextBox txtMaxVIF;
        private System.Windows.Forms.Label lblMaxVIF;
        private System.Windows.Forms.TabControl tabStats;
        private System.Windows.Forms.TabPage tabVariableStats;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.TabPage tabModelStats;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.Label lblNumObs;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMaxVars;
        private System.Windows.Forms.Label lblMaxAndRecommended;
        private System.Windows.Forms.Button btnCrossValidation;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.TabPage tabROC;
        private ZedGraph.ZedGraphControl zgcROC;
        private System.Windows.Forms.ListView listView3;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.Button btnROCPlot;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox tbRegThreshVert;
        private System.Windows.Forms.RadioButton rbLogeValMET;
        private System.Windows.Forms.RadioButton rbLog10ValMET;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.RadioButton rbValMET;
        private System.Windows.Forms.TextBox tbDecThreshHoriz;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView listView4;
        private System.Windows.Forms.ColumnHeader colDC;
        private System.Windows.Forms.ColumnHeader colFP;
        private System.Windows.Forms.ColumnHeader colFN;
        private System.Windows.Forms.ColumnHeader colSpec;
        private System.Windows.Forms.ColumnHeader colSens;
        private System.Windows.Forms.Button btnView;
        private System.Windows.Forms.TextBox txtPwrValMET;
        private System.Windows.Forms.RadioButton rbPwrValMET;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.TabPage tabObsPred2;
        private VBControls.MLRPlots mlrPlots1;
        private System.Windows.Forms.ColumnHeader colAcc;
        private System.Windows.Forms.ColumnHeader colTot;
        private System.Windows.Forms.TabPage tabResiduals;
        private System.Windows.Forms.GroupBox gboxCtrls;
        private System.Windows.Forms.GroupBox gboxResiduals;
        private System.Windows.Forms.RadioButton rbDFFITS;
        private System.Windows.Forms.RadioButton rbCooks;
        private System.Windows.Forms.GroupBox gboxView;
        private System.Windows.Forms.RadioButton rbPlot;
        private System.Windows.Forms.RadioButton rbTable;
        private System.Windows.Forms.GroupBox gboxRebuilds;
        private System.Windows.Forms.TabControl tabControl3;
        private System.Windows.Forms.TabPage tabResidVSFit;
        private System.Windows.Forms.TabPage tabFitVSObs;
        private System.Windows.Forms.TabPage tabDFFITSCooks;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.Button btnClear;
        private ZedGraph.ZedGraphControl zgcResidvFitted;
        private VBControls.MLRPlots mlrPlots2;
        private System.Windows.Forms.GroupBox gboxResidTable;
        private System.Windows.Forms.Button btnViewDataTable;
        private System.Windows.Forms.GroupBox gboxAutoCtrls;
        private System.Windows.Forms.TextBox tboxAutoConstantThresholdValue;
        private System.Windows.Forms.RadioButton rbAutoConstantThreshold;
        private System.Windows.Forms.RadioButton rbAutoIterativeThreshold;
        private System.Windows.Forms.Label lblAutoThreshold;
        private System.Windows.Forms.Button btnGoAuto;
        private System.Windows.Forms.GroupBox gboxIterativeCtrls;
        private System.Windows.Forms.Label lblIterativeThreshold;
        private System.Windows.Forms.Button btnGoIterative;
        private System.Windows.Forms.DataGridView dgvResid;
        private ZedGraph.ZedGraphControl zgcResidualsPlot;
        private System.Windows.Forms.Label lblADPVal;
        private System.Windows.Forms.Label lblADStat;
    }
}