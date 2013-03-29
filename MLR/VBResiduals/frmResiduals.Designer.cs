namespace VBResiduals
{
    partial class frmResiduals
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.zgcDFFITS = new ZedGraph.ZedGraphControl();
            this.dgv1 = new System.Windows.Forms.DataGridView();
            this.btnGoDFFITSRebuild = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
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
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabResults = new System.Windows.Forms.TabPage();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.zgc2 = new ZedGraph.ZedGraphControl();
            this.tabObsPred = new System.Windows.Forms.TabPage();
            this.mlrPlots1 = new VBControls.MLRPlots();
            this.tabdffits = new System.Windows.Forms.TabPage();
            this.btnUseRebuiltDFFITS = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnViewDTDFFITS = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbConstantCutoff = new System.Windows.Forms.RadioButton();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnGoDFFITSAuto = new System.Windows.Forms.Button();
            this.rbIterativeCutoff = new System.Windows.Forms.RadioButton();
            this.tabCooks = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rbCookConstantCutoff = new System.Windows.Forms.RadioButton();
            this.btnGoCooksAuto = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.tbcookcutoff = new System.Windows.Forms.TextBox();
            this.rbCooksIterativeCutoff = new System.Windows.Forms.RadioButton();
            this.btnUseRebuiltCooks = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.btnViewDTCooks = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.btnGoCooksIterative = new System.Windows.Forms.Button();
            this.dgvCooks = new System.Windows.Forms.DataGridView();
            this.zgcCooks = new ZedGraph.ZedGraphControl();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgv1)).BeginInit();
            this.tabStats.SuspendLayout();
            this.tabVariableStats.SuspendLayout();
            this.tabModelStats.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabResults.SuspendLayout();
            this.tabObsPred.SuspendLayout();
            this.tabdffits.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabCooks.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCooks)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // zgcDFFITS
            // 
            this.zgcDFFITS.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.zgcDFFITS.IsShowPointValues = true;
            this.zgcDFFITS.Location = new System.Drawing.Point(356, 6);
            this.zgcDFFITS.Name = "zgcDFFITS";
            this.zgcDFFITS.ScrollGrace = 0D;
            this.zgcDFFITS.ScrollMaxX = 0D;
            this.zgcDFFITS.ScrollMaxY = 0D;
            this.zgcDFFITS.ScrollMaxY2 = 0D;
            this.zgcDFFITS.ScrollMinX = 0D;
            this.zgcDFFITS.ScrollMinY = 0D;
            this.zgcDFFITS.ScrollMinY2 = 0D;
            this.zgcDFFITS.Size = new System.Drawing.Size(737, 465);
            this.zgcDFFITS.TabIndex = 0;
            // 
            // dgv1
            // 
            this.dgv1.AllowUserToDeleteRows = false;
            this.dgv1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.Format = "N0";
            dataGridViewCellStyle1.NullValue = null;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv1.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgv1.Location = new System.Drawing.Point(6, 17);
            this.dgv1.Name = "dgv1";
            this.dgv1.ReadOnly = true;
            dataGridViewCellStyle2.Format = "N4";
            dataGridViewCellStyle2.NullValue = null;
            this.dgv1.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgv1.RowTemplate.DefaultCellStyle.Format = "N0";
            this.dgv1.RowTemplate.DefaultCellStyle.NullValue = null;
            this.dgv1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv1.Size = new System.Drawing.Size(344, 233);
            this.dgv1.TabIndex = 1;
            // 
            // btnGoDFFITSRebuild
            // 
            this.btnGoDFFITSRebuild.Location = new System.Drawing.Point(111, 268);
            this.btnGoDFFITSRebuild.Name = "btnGoDFFITSRebuild";
            this.btnGoDFFITSRebuild.Size = new System.Drawing.Size(40, 21);
            this.btnGoDFFITSRebuild.TabIndex = 2;
            this.btnGoDFFITSRebuild.Text = "Go";
            this.btnGoDFFITSRebuild.UseVisualStyleBackColor = true;
            this.btnGoDFFITSRebuild.Click += new System.EventHandler(this.btnGoDFFITSRebuild_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(19, 27);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(141, 147);
            this.listBox1.TabIndex = 49;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // tabStats
            // 
            this.tabStats.Controls.Add(this.tabVariableStats);
            this.tabStats.Controls.Add(this.tabModelStats);
            this.tabStats.Location = new System.Drawing.Point(214, 12);
            this.tabStats.Name = "tabStats";
            this.tabStats.SelectedIndex = 0;
            this.tabStats.Size = new System.Drawing.Size(640, 219);
            this.tabStats.TabIndex = 61;
            // 
            // tabVariableStats
            // 
            this.tabVariableStats.Controls.Add(this.listView1);
            this.tabVariableStats.Location = new System.Drawing.Point(4, 22);
            this.tabVariableStats.Name = "tabVariableStats";
            this.tabVariableStats.Padding = new System.Windows.Forms.Padding(3);
            this.tabVariableStats.Size = new System.Drawing.Size(632, 193);
            this.tabVariableStats.TabIndex = 0;
            this.tabVariableStats.Text = "Variable Statistics";
            this.tabVariableStats.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader8});
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(3, 3);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(626, 187);
            this.listView1.TabIndex = 50;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView1_KeyDown);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Parameter";
            this.columnHeader1.Width = 88;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Coefficient";
            this.columnHeader2.Width = 78;
            // 
            // columnHeader3
            // 
            this.columnHeader3.DisplayIndex = 3;
            this.columnHeader3.Text = "Std. Error";
            this.columnHeader3.Width = 78;
            // 
            // columnHeader4
            // 
            this.columnHeader4.DisplayIndex = 4;
            this.columnHeader4.Text = "t-Statistic";
            this.columnHeader4.Width = 78;
            // 
            // columnHeader5
            // 
            this.columnHeader5.DisplayIndex = 5;
            this.columnHeader5.Text = "P-Value";
            this.columnHeader5.Width = 96;
            // 
            // columnHeader8
            // 
            this.columnHeader8.DisplayIndex = 2;
            this.columnHeader8.Text = "StandardizedCoefficient";
            this.columnHeader8.Width = 130;
            // 
            // tabModelStats
            // 
            this.tabModelStats.Controls.Add(this.listView2);
            this.tabModelStats.Location = new System.Drawing.Point(4, 22);
            this.tabModelStats.Name = "tabModelStats";
            this.tabModelStats.Padding = new System.Windows.Forms.Padding(3);
            this.tabModelStats.Size = new System.Drawing.Size(632, 193);
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
            this.listView2.Size = new System.Drawing.Size(626, 187);
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
            this.columnHeader7.Width = 88;
            // 
            // tabControl2
            // 
            this.tabControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl2.Controls.Add(this.tabResults);
            this.tabControl2.Controls.Add(this.tabObsPred);
            this.tabControl2.Controls.Add(this.tabdffits);
            this.tabControl2.Controls.Add(this.tabCooks);
            this.tabControl2.Location = new System.Drawing.Point(12, 246);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(1104, 501);
            this.tabControl2.TabIndex = 62;
            // 
            // tabResults
            // 
            this.tabResults.AutoScroll = true;
            this.tabResults.Controls.Add(this.label8);
            this.tabResults.Controls.Add(this.label9);
            this.tabResults.Controls.Add(this.label7);
            this.tabResults.Controls.Add(this.label5);
            this.tabResults.Controls.Add(this.zgc2);
            this.tabResults.Location = new System.Drawing.Point(4, 22);
            this.tabResults.Name = "tabResults";
            this.tabResults.Padding = new System.Windows.Forms.Padding(3);
            this.tabResults.Size = new System.Drawing.Size(1096, 475);
            this.tabResults.TabIndex = 1;
            this.tabResults.Text = "Residuals vs Fitted";
            this.tabResults.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 109);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(0, 13);
            this.label8.TabIndex = 8;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(16, 132);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(0, 13);
            this.label9.TabIndex = 7;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 73);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "label7";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 51);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "label5";
            // 
            // zgc2
            // 
            this.zgc2.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.zgc2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.zgc2.EditButtons = System.Windows.Forms.MouseButtons.Left;
            this.zgc2.IsShowPointValues = true;
            this.zgc2.Location = new System.Drawing.Point(191, 3);
            this.zgc2.Name = "zgc2";
            this.zgc2.PanModifierKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)));
            this.zgc2.ScrollGrace = 0D;
            this.zgc2.ScrollMaxX = 0D;
            this.zgc2.ScrollMaxY = 0D;
            this.zgc2.ScrollMaxY2 = 0D;
            this.zgc2.ScrollMinX = 0D;
            this.zgc2.ScrollMinY = 0D;
            this.zgc2.ScrollMinY2 = 0D;
            this.zgc2.Size = new System.Drawing.Size(902, 469);
            this.zgc2.TabIndex = 4;
            // 
            // tabObsPred
            // 
            this.tabObsPred.AutoScroll = true;
            this.tabObsPred.Controls.Add(this.mlrPlots1);
            this.tabObsPred.Location = new System.Drawing.Point(4, 22);
            this.tabObsPred.Name = "tabObsPred";
            this.tabObsPred.Size = new System.Drawing.Size(1096, 475);
            this.tabObsPred.TabIndex = 4;
            this.tabObsPred.Text = "Fitted vs Observed";
            this.tabObsPred.UseVisualStyleBackColor = true;
            // 
            // mlrPlots1
            // 
            this.mlrPlots1.AutoScroll = true;
            this.mlrPlots1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mlrPlots1.Location = new System.Drawing.Point(0, 0);
            this.mlrPlots1.Name = "mlrPlots1";
            this.mlrPlots1.PowerExponent = double.NaN;
            this.mlrPlots1.Size = new System.Drawing.Size(1096, 475);
            this.mlrPlots1.TabIndex = 0;
            this.mlrPlots1.Transform = VBTools.Globals.DependendVariableTransforms.none;
            // 
            // tabdffits
            // 
            this.tabdffits.AutoScroll = true;
            this.tabdffits.Controls.Add(this.btnUseRebuiltDFFITS);
            this.tabdffits.Controls.Add(this.label3);
            this.tabdffits.Controls.Add(this.btnViewDTDFFITS);
            this.tabdffits.Controls.Add(this.label1);
            this.tabdffits.Controls.Add(this.zgcDFFITS);
            this.tabdffits.Controls.Add(this.dgv1);
            this.tabdffits.Controls.Add(this.btnGoDFFITSRebuild);
            this.tabdffits.Controls.Add(this.groupBox2);
            this.tabdffits.Location = new System.Drawing.Point(4, 22);
            this.tabdffits.Name = "tabdffits";
            this.tabdffits.Padding = new System.Windows.Forms.Padding(3);
            this.tabdffits.Size = new System.Drawing.Size(1096, 475);
            this.tabdffits.TabIndex = 0;
            this.tabdffits.Text = "DFFITS";
            this.tabdffits.UseVisualStyleBackColor = true;
            // 
            // btnUseRebuiltDFFITS
            // 
            this.btnUseRebuiltDFFITS.Enabled = false;
            this.btnUseRebuiltDFFITS.Location = new System.Drawing.Point(184, 426);
            this.btnUseRebuiltDFFITS.Name = "btnUseRebuiltDFFITS";
            this.btnUseRebuiltDFFITS.Size = new System.Drawing.Size(118, 23);
            this.btnUseRebuiltDFFITS.TabIndex = 14;
            this.btnUseRebuiltDFFITS.Text = "Use Rebuilt Model";
            this.btnUseRebuiltDFFITS.UseVisualStyleBackColor = true;
            this.btnUseRebuiltDFFITS.Visible = false;
            this.btnUseRebuiltDFFITS.Click += new System.EventHandler(this.btnUseRebuiltDFFITS_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(209, 273);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "2*SQR(p/n) = ";
            // 
            // btnViewDTDFFITS
            // 
            this.btnViewDTDFFITS.Location = new System.Drawing.Point(40, 424);
            this.btnViewDTDFFITS.Name = "btnViewDTDFFITS";
            this.btnViewDTDFFITS.Size = new System.Drawing.Size(94, 23);
            this.btnViewDTDFFITS.TabIndex = 12;
            this.btnViewDTDFFITS.Text = "View Data Table";
            this.btnViewDTDFFITS.UseVisualStyleBackColor = true;
            this.btnViewDTDFFITS.Click += new System.EventHandler(this.btnViewDTDFFITS_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(20, 272);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Iterative Rebuild";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbConstantCutoff);
            this.groupBox2.Controls.Add(this.textBox3);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.btnGoDFFITSAuto);
            this.groupBox2.Controls.Add(this.rbIterativeCutoff);
            this.groupBox2.Location = new System.Drawing.Point(12, 305);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(328, 100);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Auto Rebuild";
            // 
            // rbConstantCutoff
            // 
            this.rbConstantCutoff.AutoSize = true;
            this.rbConstantCutoff.Location = new System.Drawing.Point(112, 71);
            this.rbConstantCutoff.Name = "rbConstantCutoff";
            this.rbConstantCutoff.Size = new System.Drawing.Size(112, 17);
            this.rbConstantCutoff.TabIndex = 16;
            this.rbConstantCutoff.TabStop = true;
            this.rbConstantCutoff.Text = "constant threshold";
            this.rbConstantCutoff.UseVisualStyleBackColor = true;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(231, 70);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(49, 20);
            this.textBox3.TabIndex = 15;
            this.textBox3.Text = ".65";
            this.textBox3.Validating += new System.ComponentModel.CancelEventHandler(this.textBox3_Validating);
            this.textBox3.Validated += new System.EventHandler(this.textBox3_Validated);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(109, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(159, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Stop when all DFFITS less than:";
            // 
            // btnGoDFFITSAuto
            // 
            this.btnGoDFFITSAuto.Location = new System.Drawing.Point(34, 44);
            this.btnGoDFFITSAuto.Name = "btnGoDFFITSAuto";
            this.btnGoDFFITSAuto.Size = new System.Drawing.Size(41, 21);
            this.btnGoDFFITSAuto.TabIndex = 12;
            this.btnGoDFFITSAuto.Text = "Go";
            this.btnGoDFFITSAuto.UseVisualStyleBackColor = true;
            this.btnGoDFFITSAuto.Click += new System.EventHandler(this.btnGoDFFITSAuto_Click);
            // 
            // rbIterativeCutoff
            // 
            this.rbIterativeCutoff.AutoSize = true;
            this.rbIterativeCutoff.Location = new System.Drawing.Point(112, 48);
            this.rbIterativeCutoff.Name = "rbIterativeCutoff";
            this.rbIterativeCutoff.Size = new System.Drawing.Size(195, 17);
            this.rbIterativeCutoff.TabIndex = 17;
            this.rbIterativeCutoff.TabStop = true;
            this.rbIterativeCutoff.Text = "iterative threshold using 2*SQR(p/n)";
            this.rbIterativeCutoff.UseVisualStyleBackColor = true;
            // 
            // tabCooks
            // 
            this.tabCooks.AutoScroll = true;
            this.tabCooks.Controls.Add(this.groupBox3);
            this.tabCooks.Controls.Add(this.btnUseRebuiltCooks);
            this.tabCooks.Controls.Add(this.label10);
            this.tabCooks.Controls.Add(this.btnViewDTCooks);
            this.tabCooks.Controls.Add(this.label13);
            this.tabCooks.Controls.Add(this.btnGoCooksIterative);
            this.tabCooks.Controls.Add(this.dgvCooks);
            this.tabCooks.Controls.Add(this.zgcCooks);
            this.tabCooks.Location = new System.Drawing.Point(4, 22);
            this.tabCooks.Name = "tabCooks";
            this.tabCooks.Size = new System.Drawing.Size(1096, 475);
            this.tabCooks.TabIndex = 3;
            this.tabCooks.Text = "Cook\'s Distance";
            this.tabCooks.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rbCookConstantCutoff);
            this.groupBox3.Controls.Add(this.btnGoCooksAuto);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.tbcookcutoff);
            this.groupBox3.Controls.Add(this.rbCooksIterativeCutoff);
            this.groupBox3.Location = new System.Drawing.Point(8, 291);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(341, 119);
            this.groupBox3.TabIndex = 24;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Auto Rebuild";
            // 
            // rbCookConstantCutoff
            // 
            this.rbCookConstantCutoff.AutoSize = true;
            this.rbCookConstantCutoff.Location = new System.Drawing.Point(128, 72);
            this.rbCookConstantCutoff.Name = "rbCookConstantCutoff";
            this.rbCookConstantCutoff.Size = new System.Drawing.Size(112, 17);
            this.rbCookConstantCutoff.TabIndex = 21;
            this.rbCookConstantCutoff.TabStop = true;
            this.rbCookConstantCutoff.Text = "constant threshold";
            this.rbCookConstantCutoff.UseVisualStyleBackColor = true;
            // 
            // btnGoCooksAuto
            // 
            this.btnGoCooksAuto.Location = new System.Drawing.Point(26, 50);
            this.btnGoCooksAuto.Name = "btnGoCooksAuto";
            this.btnGoCooksAuto.Size = new System.Drawing.Size(41, 21);
            this.btnGoCooksAuto.TabIndex = 17;
            this.btnGoCooksAuto.Text = "Go";
            this.btnGoCooksAuto.UseVisualStyleBackColor = true;
            this.btnGoCooksAuto.Click += new System.EventHandler(this.btnGoCooksAuto_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(118, 20);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(186, 13);
            this.label11.TabIndex = 19;
            this.label11.Text = "Stop when all Cooks values less than:";
            // 
            // tbcookcutoff
            // 
            this.tbcookcutoff.Location = new System.Drawing.Point(243, 71);
            this.tbcookcutoff.Name = "tbcookcutoff";
            this.tbcookcutoff.Size = new System.Drawing.Size(52, 20);
            this.tbcookcutoff.TabIndex = 20;
            this.tbcookcutoff.Text = "0.20";
            this.tbcookcutoff.Validating += new System.ComponentModel.CancelEventHandler(this.tbcookcutoff_Validating);
            this.tbcookcutoff.Validated += new System.EventHandler(this.tbcookcutoff_Validated);
            // 
            // rbCooksIterativeCutoff
            // 
            this.rbCooksIterativeCutoff.AutoSize = true;
            this.rbCooksIterativeCutoff.Location = new System.Drawing.Point(128, 48);
            this.rbCooksIterativeCutoff.Name = "rbCooksIterativeCutoff";
            this.rbCooksIterativeCutoff.Size = new System.Drawing.Size(165, 17);
            this.rbCooksIterativeCutoff.TabIndex = 22;
            this.rbCooksIterativeCutoff.TabStop = true;
            this.rbCooksIterativeCutoff.Text = "iterative threshold using 4.0/n";
            this.rbCooksIterativeCutoff.UseVisualStyleBackColor = true;
            // 
            // btnUseRebuiltCooks
            // 
            this.btnUseRebuiltCooks.Enabled = false;
            this.btnUseRebuiltCooks.Location = new System.Drawing.Point(192, 437);
            this.btnUseRebuiltCooks.Name = "btnUseRebuiltCooks";
            this.btnUseRebuiltCooks.Size = new System.Drawing.Size(118, 23);
            this.btnUseRebuiltCooks.TabIndex = 23;
            this.btnUseRebuiltCooks.Text = "Use Rebuilt Model";
            this.btnUseRebuiltCooks.UseVisualStyleBackColor = true;
            this.btnUseRebuiltCooks.Visible = false;
            this.btnUseRebuiltCooks.Click += new System.EventHandler(this.btnUseRebuiltCooks_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(228, 257);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(51, 13);
            this.label10.TabIndex = 22;
            this.label10.Text = "4.0 / n = ";
            // 
            // btnViewDTCooks
            // 
            this.btnViewDTCooks.Location = new System.Drawing.Point(27, 433);
            this.btnViewDTCooks.Name = "btnViewDTCooks";
            this.btnViewDTCooks.Size = new System.Drawing.Size(94, 23);
            this.btnViewDTCooks.TabIndex = 21;
            this.btnViewDTCooks.Text = "View Data Table";
            this.btnViewDTCooks.UseVisualStyleBackColor = true;
            this.btnViewDTCooks.Click += new System.EventHandler(this.btnViewDTCooks_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(18, 259);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(84, 13);
            this.label13.TabIndex = 16;
            this.label13.Text = "Iterative Rebuild";
            // 
            // btnGoCooksIterative
            // 
            this.btnGoCooksIterative.Location = new System.Drawing.Point(109, 255);
            this.btnGoCooksIterative.Name = "btnGoCooksIterative";
            this.btnGoCooksIterative.Size = new System.Drawing.Size(40, 21);
            this.btnGoCooksIterative.TabIndex = 15;
            this.btnGoCooksIterative.Text = "Go";
            this.btnGoCooksIterative.UseVisualStyleBackColor = true;
            this.btnGoCooksIterative.Click += new System.EventHandler(this.btnGoCooksIterative_Click);
            // 
            // dgvCooks
            // 
            this.dgvCooks.AllowUserToDeleteRows = false;
            this.dgvCooks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.Format = "N0";
            dataGridViewCellStyle3.NullValue = null;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvCooks.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvCooks.Location = new System.Drawing.Point(6, 5);
            this.dgvCooks.Name = "dgvCooks";
            this.dgvCooks.ReadOnly = true;
            dataGridViewCellStyle4.Format = "N4";
            dataGridViewCellStyle4.NullValue = null;
            this.dgvCooks.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvCooks.RowTemplate.DefaultCellStyle.Format = "N0";
            this.dgvCooks.RowTemplate.DefaultCellStyle.NullValue = null;
            this.dgvCooks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCooks.Size = new System.Drawing.Size(344, 233);
            this.dgvCooks.TabIndex = 2;
            // 
            // zgcCooks
            // 
            this.zgcCooks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.zgcCooks.IsShowPointValues = true;
            this.zgcCooks.Location = new System.Drawing.Point(356, 5);
            this.zgcCooks.Name = "zgcCooks";
            this.zgcCooks.ScrollGrace = 0D;
            this.zgcCooks.ScrollMaxX = 0D;
            this.zgcCooks.ScrollMaxY = 0D;
            this.zgcCooks.ScrollMaxY2 = 0D;
            this.zgcCooks.ScrollMinX = 0D;
            this.zgcCooks.ScrollMinY = 0D;
            this.zgcCooks.ScrollMinY2 = 0D;
            this.zgcCooks.Size = new System.Drawing.Size(737, 465);
            this.zgcCooks.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnClear);
            this.groupBox1.Controls.Add(this.listBox1);
            this.groupBox1.Location = new System.Drawing.Point(16, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(182, 219);
            this.groupBox1.TabIndex = 63;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Models";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(52, 189);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 50;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // frmResiduals
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1152, 774);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tabControl2);
            this.Controls.Add(this.tabStats);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frmResiduals";
            this.Text = "Residuals";
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.frmResiduals_HelpRequested);
            this.Enter += new System.EventHandler(this.frmResiduals_Enter);
            ((System.ComponentModel.ISupportInitialize)(this.dgv1)).EndInit();
            this.tabStats.ResumeLayout(false);
            this.tabVariableStats.ResumeLayout(false);
            this.tabModelStats.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabResults.ResumeLayout(false);
            this.tabResults.PerformLayout();
            this.tabObsPred.ResumeLayout(false);
            this.tabdffits.ResumeLayout(false);
            this.tabdffits.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabCooks.ResumeLayout(false);
            this.tabCooks.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCooks)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ZedGraph.ZedGraphControl zgcDFFITS;
        private System.Windows.Forms.DataGridView dgv1;
        private System.Windows.Forms.Button btnGoDFFITSRebuild;
        private System.Windows.Forms.ListBox listBox1;
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
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabdffits;
        private System.Windows.Forms.TabPage tabResults;
        private ZedGraph.ZedGraphControl zgc2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnViewDTDFFITS;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnUseRebuiltDFFITS;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.TabPage tabCooks;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.DataGridView dgvCooks;
        private ZedGraph.ZedGraphControl zgcCooks;
        private System.Windows.Forms.Button btnUseRebuiltCooks;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnViewDTCooks;
        private System.Windows.Forms.TextBox tbcookcutoff;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnGoCooksAuto;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button btnGoCooksIterative;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.TabPage tabObsPred;
        private VBControls.MLRPlots mlrPlots1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbIterativeCutoff;
        private System.Windows.Forms.RadioButton rbConstantCutoff;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnGoDFFITSAuto;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rbCooksIterativeCutoff;
        private System.Windows.Forms.RadioButton rbCookConstantCutoff;
        private System.Windows.Forms.Button btnClear;
    }
}

