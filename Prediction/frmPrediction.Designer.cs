namespace Prediction
{
    partial class frmPrediction
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panelIVs = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgvVariables = new System.Windows.Forms.DataGridView();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.dgvObs = new System.Windows.Forms.DataGridView();
            this.dgvStats = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbProbability = new System.Windows.Forms.RadioButton();
            this.rbRaw = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.txtProbabilityThreshold = new System.Windows.Forms.TextBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.txtPower = new System.Windows.Forms.TextBox();
            this.rbPower = new System.Windows.Forms.RadioButton();
            this.rbLn = new System.Windows.Forms.RadioButton();
            this.rbNone = new System.Windows.Forms.RadioButton();
            this.rbLog10 = new System.Windows.Forms.RadioButton();
            this.label23 = new System.Windows.Forms.Label();
            this.txtRegStd = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.txtDecCrit = new System.Windows.Forms.TextBox();
            this.txtModel = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnIVDataValidation = new System.Windows.Forms.Button();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.lbAvailableModels = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.panel2.SuspendLayout();
            this.panelIVs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVariables)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvObs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStats)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.panel2.Controls.Add(this.panelIVs);
            this.panel2.Location = new System.Drawing.Point(12, 211);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(967, 314);
            this.panel2.TabIndex = 1;
            // 
            // panelIVs
            // 
            this.panelIVs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelIVs.Controls.Add(this.splitContainer1);
            this.panelIVs.Location = new System.Drawing.Point(4, 3);
            this.panelIVs.Name = "panelIVs";
            this.panelIVs.Size = new System.Drawing.Size(960, 308);
            this.panelIVs.TabIndex = 3;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dgvVariables);
            this.splitContainer1.Panel1MinSize = 100;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(960, 308);
            this.splitContainer1.SplitterDistance = 538;
            this.splitContainer1.TabIndex = 0;
            // 
            // dgvVariables
            // 
            this.dgvVariables.AllowUserToOrderColumns = true;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvVariables.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvVariables.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvVariables.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvVariables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvVariables.Location = new System.Drawing.Point(0, 0);
            this.dgvVariables.Name = "dgvVariables";
            this.dgvVariables.Size = new System.Drawing.Size(538, 308);
            this.dgvVariables.TabIndex = 2;
            this.dgvVariables.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgvVariables_CellPainting);
            this.dgvVariables.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvVariables_CellValueChanged);
            this.dgvVariables.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvVariables_DataError);
            this.dgvVariables.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dgvVariables_Scroll);
            this.dgvVariables.SelectionChanged += new System.EventHandler(this.dgvVariables_SelectionChanged);
            this.dgvVariables.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgvVariables_RowsAdded);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.dgvObs);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.dgvStats);
            this.splitContainer2.Size = new System.Drawing.Size(418, 308);
            this.splitContainer2.SplitterDistance = 139;
            this.splitContainer2.TabIndex = 0;
            this.splitContainer2.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer2_SplitterMoved);
            // 
            // dgvObs
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvObs.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvObs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvObs.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvObs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvObs.Location = new System.Drawing.Point(0, 0);
            this.dgvObs.Name = "dgvObs";
            this.dgvObs.Size = new System.Drawing.Size(139, 308);
            this.dgvObs.TabIndex = 3;
            this.dgvObs.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvObs_CellEndEdit);
            this.dgvObs.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvObs_DataError);
            this.dgvObs.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dgvObs_Scroll);
            this.dgvObs.SelectionChanged += new System.EventHandler(this.dgvObs_SelectionChanged);
            this.dgvObs.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgvObs_MouseUp);
            // 
            // dgvStats
            // 
            this.dgvStats.AllowUserToOrderColumns = true;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvStats.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvStats.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvStats.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgvStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvStats.Location = new System.Drawing.Point(0, 0);
            this.dgvStats.Name = "dgvStats";
            this.dgvStats.ReadOnly = true;
            this.dgvStats.Size = new System.Drawing.Size(275, 308);
            this.dgvStats.TabIndex = 4;
            this.dgvStats.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dgvStats_Scroll);
            this.dgvStats.SelectionChanged += new System.EventHandler(this.dgvStats_SelectionChanged);
            this.dgvStats.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgvStats_MouseUp);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 188);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(133, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Predictive Record";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbProbability);
            this.groupBox1.Controls.Add(this.rbRaw);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtProbabilityThreshold);
            this.groupBox1.Controls.Add(this.groupBox7);
            this.groupBox1.Controls.Add(this.label23);
            this.groupBox1.Controls.Add(this.txtRegStd);
            this.groupBox1.Controls.Add(this.label24);
            this.groupBox1.Controls.Add(this.txtDecCrit);
            this.groupBox1.Location = new System.Drawing.Point(202, 63);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(465, 124);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Model Evaluation Thresholds";
            // 
            // rbProbability
            // 
            this.rbProbability.AutoSize = true;
            this.rbProbability.Location = new System.Drawing.Point(27, 51);
            this.rbProbability.Name = "rbProbability";
            this.rbProbability.Size = new System.Drawing.Size(14, 13);
            this.rbProbability.TabIndex = 100;
            this.rbProbability.UseVisualStyleBackColor = true;
            // 
            // rbRaw
            // 
            this.rbRaw.AutoSize = true;
            this.rbRaw.Checked = true;
            this.rbRaw.Location = new System.Drawing.Point(27, 30);
            this.rbRaw.Name = "rbRaw";
            this.rbRaw.Size = new System.Drawing.Size(14, 13);
            this.rbRaw.TabIndex = 99;
            this.rbRaw.TabStop = true;
            this.rbRaw.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Blue;
            this.label3.Location = new System.Drawing.Point(86, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(118, 13);
            this.label3.TabIndex = 98;
            this.label3.Text = "Exceedance Probability";
            // 
            // txtProbabilityThreshold
            // 
            this.txtProbabilityThreshold.Location = new System.Drawing.Point(47, 48);
            this.txtProbabilityThreshold.Name = "txtProbabilityThreshold";
            this.txtProbabilityThreshold.Size = new System.Drawing.Size(33, 20);
            this.txtProbabilityThreshold.TabIndex = 97;
            this.txtProbabilityThreshold.Text = "50";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.txtPower);
            this.groupBox7.Controls.Add(this.rbPower);
            this.groupBox7.Controls.Add(this.rbLn);
            this.groupBox7.Controls.Add(this.rbNone);
            this.groupBox7.Controls.Add(this.rbLog10);
            this.groupBox7.Location = new System.Drawing.Point(312, 12);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(147, 106);
            this.groupBox7.TabIndex = 96;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = " Threshold Transform";
            // 
            // txtPower
            // 
            this.txtPower.Location = new System.Drawing.Point(82, 79);
            this.txtPower.Name = "txtPower";
            this.txtPower.Size = new System.Drawing.Size(45, 20);
            this.txtPower.TabIndex = 19;
            this.txtPower.Text = "1.0";
            // 
            // rbPower
            // 
            this.rbPower.AutoSize = true;
            this.rbPower.Location = new System.Drawing.Point(24, 81);
            this.rbPower.Name = "rbPower";
            this.rbPower.Size = new System.Drawing.Size(55, 17);
            this.rbPower.TabIndex = 18;
            this.rbPower.Text = "Power";
            this.rbPower.UseVisualStyleBackColor = true;
            this.rbPower.CheckedChanged += new System.EventHandler(this.rbPower_CheckedChanged);
            // 
            // rbLn
            // 
            this.rbLn.AutoSize = true;
            this.rbLn.Location = new System.Drawing.Point(24, 60);
            this.rbLn.Name = "rbLn";
            this.rbLn.Size = new System.Drawing.Size(37, 17);
            this.rbLn.TabIndex = 17;
            this.rbLn.Text = "Ln";
            this.rbLn.UseVisualStyleBackColor = true;
            // 
            // rbNone
            // 
            this.rbNone.AutoSize = true;
            this.rbNone.Checked = true;
            this.rbNone.Location = new System.Drawing.Point(24, 18);
            this.rbNone.Name = "rbNone";
            this.rbNone.Size = new System.Drawing.Size(51, 17);
            this.rbNone.TabIndex = 15;
            this.rbNone.TabStop = true;
            this.rbNone.Text = "None";
            this.rbNone.UseVisualStyleBackColor = true;
            this.rbNone.CheckedChanged += new System.EventHandler(this.rbNone_CheckedChanged);
            // 
            // rbLog10
            // 
            this.rbLog10.AutoSize = true;
            this.rbLog10.Location = new System.Drawing.Point(24, 39);
            this.rbLog10.Name = "rbLog10";
            this.rbLog10.Size = new System.Drawing.Size(55, 17);
            this.rbLog10.TabIndex = 16;
            this.rbLog10.Text = "Log10";
            this.rbLog10.UseVisualStyleBackColor = true;
            this.rbLog10.CheckedChanged += new System.EventHandler(this.rbLog10_CheckedChanged);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.ForeColor = System.Drawing.Color.LimeGreen;
            this.label23.Location = new System.Drawing.Point(86, 72);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(148, 13);
            this.label23.TabIndex = 95;
            this.label23.Text = "Regulatory Standard (Vertical)";
            // 
            // txtRegStd
            // 
            this.txtRegStd.Location = new System.Drawing.Point(47, 69);
            this.txtRegStd.Name = "txtRegStd";
            this.txtRegStd.Size = new System.Drawing.Size(33, 20);
            this.txtRegStd.TabIndex = 94;
            this.txtRegStd.Text = "235";
            this.txtRegStd.Leave += new System.EventHandler(this.txtRegStd_Leave);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.ForeColor = System.Drawing.Color.Blue;
            this.label24.Location = new System.Drawing.Point(86, 30);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(145, 13);
            this.label24.TabIndex = 93;
            this.label24.Text = "Decision Criterion (Horizontal)";
            // 
            // txtDecCrit
            // 
            this.txtDecCrit.Location = new System.Drawing.Point(47, 27);
            this.txtDecCrit.Name = "txtDecCrit";
            this.txtDecCrit.Size = new System.Drawing.Size(33, 20);
            this.txtDecCrit.TabIndex = 92;
            this.txtDecCrit.Text = "235";
            this.txtDecCrit.Leave += new System.EventHandler(this.txtDecCrit_Leave);
            // 
            // txtModel
            // 
            this.txtModel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtModel.BackColor = System.Drawing.Color.White;
            this.txtModel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtModel.Location = new System.Drawing.Point(266, 9);
            this.txtModel.Multiline = true;
            this.txtModel.Name = "txtModel";
            this.txtModel.ReadOnly = true;
            this.txtModel.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtModel.Size = new System.Drawing.Size(678, 46);
            this.txtModel.TabIndex = 15;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(198, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 20);
            this.label1.TabIndex = 14;
            this.label1.Text = "Model: ";
            // 
            // btnIVDataValidation
            // 
            this.btnIVDataValidation.Location = new System.Drawing.Point(472, 110);
            this.btnIVDataValidation.Name = "btnIVDataValidation";
            this.btnIVDataValidation.Size = new System.Drawing.Size(75, 34);
            this.btnIVDataValidation.TabIndex = 17;
            this.btnIVDataValidation.Text = "IV Data Validation";
            this.btnIVDataValidation.UseVisualStyleBackColor = false;
            this.btnIVDataValidation.Visible = false;
            // 
            // lbAvailableModels
            // 
            this.lbAvailableModels.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbAvailableModels.FormattingEnabled = true;
            this.lbAvailableModels.Location = new System.Drawing.Point(6, 25);
            this.lbAvailableModels.Name = "lbAvailableModels";
            this.lbAvailableModels.Size = new System.Drawing.Size(134, 108);
            this.lbAvailableModels.TabIndex = 18;
            this.lbAvailableModels.SelectedIndexChanged += new System.EventHandler(this.lbAvailableModels_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lbAvailableModels);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(16, 9);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(146, 143);
            this.groupBox3.TabIndex = 97;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Available Models: ";
            // 
            // frmPrediction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnIVDataValidation);
            this.Controls.Add(this.txtModel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frmPrediction";
            this.Size = new System.Drawing.Size(991, 537);
            this.Load += new System.EventHandler(this.frmIPyPrediction_Load);
            this.panel2.ResumeLayout(false);
            this.panelIVs.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvVariables)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvObs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStats)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        //private System.Windows.Forms.Button btnMakePredictions;
        private System.Windows.Forms.Panel panelIVs;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dgvVariables;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.DataGridView dgvObs;
        private System.Windows.Forms.DataGridView dgvStats;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.RadioButton rbPower;
        private System.Windows.Forms.RadioButton rbLn;
        private System.Windows.Forms.RadioButton rbNone;
        private System.Windows.Forms.RadioButton rbLog10;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox txtRegStd;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox txtDecCrit;
        private System.Windows.Forms.TextBox txtModel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPower;
        private System.Windows.Forms.Button btnIVDataValidation;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.ListBox lbAvailableModels;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtProbabilityThreshold;
        private System.Windows.Forms.RadioButton rbProbability;
        private System.Windows.Forms.RadioButton rbRaw;
    }
}