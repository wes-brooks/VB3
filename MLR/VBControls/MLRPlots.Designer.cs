namespace VBControls
{
    partial class MLRPlots
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txbSpecificity = new System.Windows.Forms.TextBox();
            this.txbAccuracy = new System.Windows.Forms.TextBox();
            this.txbSensitivity = new System.Windows.Forms.TextBox();
            this.lblAccuracy = new System.Windows.Forms.Label();
            this.lblSpecificity = new System.Windows.Forms.Label();
            this.lblSensitivity = new System.Windows.Forms.Label();
            this.tbFN = new System.Windows.Forms.TextBox();
            this.tbFP = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.txtPwrValue = new System.Windows.Forms.TextBox();
            this.rbPwrValue = new System.Windows.Forms.RadioButton();
            this.rbLogeValue = new System.Windows.Forms.RadioButton();
            this.rbLog10Value = new System.Windows.Forms.RadioButton();
            this.rbValue = new System.Windows.Forms.RadioButton();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.tbThresholdReg = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.tbThresholdDec = new System.Windows.Forms.TextBox();
            this.zgc = new ZedGraph.ZedGraphControl();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboxPlotList = new System.Windows.Forms.ComboBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.colProbExceed = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFN = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFP = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTot = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSensitivity = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSpecificity = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colAccuracy = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txbSpecificity);
            this.groupBox3.Controls.Add(this.txbAccuracy);
            this.groupBox3.Controls.Add(this.txbSensitivity);
            this.groupBox3.Controls.Add(this.lblAccuracy);
            this.groupBox3.Controls.Add(this.lblSpecificity);
            this.groupBox3.Controls.Add(this.lblSensitivity);
            this.groupBox3.Controls.Add(this.tbFN);
            this.groupBox3.Controls.Add(this.tbFP);
            this.groupBox3.Controls.Add(this.label16);
            this.groupBox3.Controls.Add(this.label17);
            this.groupBox3.Location = new System.Drawing.Point(3, 247);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(226, 150);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Model Evaluation";
            // 
            // txbSpecificity
            // 
            this.txbSpecificity.Location = new System.Drawing.Point(150, 45);
            this.txbSpecificity.Name = "txbSpecificity";
            this.txbSpecificity.ReadOnly = true;
            this.txbSpecificity.Size = new System.Drawing.Size(43, 20);
            this.txbSpecificity.TabIndex = 12;
            this.txbSpecificity.TabStop = false;
            // 
            // txbAccuracy
            // 
            this.txbAccuracy.Location = new System.Drawing.Point(150, 118);
            this.txbAccuracy.Name = "txbAccuracy";
            this.txbAccuracy.ReadOnly = true;
            this.txbAccuracy.Size = new System.Drawing.Size(43, 20);
            this.txbAccuracy.TabIndex = 13;
            this.txbAccuracy.TabStop = false;
            // 
            // txbSensitivity
            // 
            this.txbSensitivity.Location = new System.Drawing.Point(150, 93);
            this.txbSensitivity.Name = "txbSensitivity";
            this.txbSensitivity.ReadOnly = true;
            this.txbSensitivity.Size = new System.Drawing.Size(43, 20);
            this.txbSensitivity.TabIndex = 11;
            this.txbSensitivity.TabStop = false;
            // 
            // lblAccuracy
            // 
            this.lblAccuracy.AutoSize = true;
            this.lblAccuracy.Location = new System.Drawing.Point(89, 121);
            this.lblAccuracy.Name = "lblAccuracy";
            this.lblAccuracy.Size = new System.Drawing.Size(55, 13);
            this.lblAccuracy.TabIndex = 10;
            this.lblAccuracy.Text = "Accuracy:";
            // 
            // lblSpecificity
            // 
            this.lblSpecificity.AutoSize = true;
            this.lblSpecificity.Location = new System.Drawing.Point(86, 48);
            this.lblSpecificity.Name = "lblSpecificity";
            this.lblSpecificity.Size = new System.Drawing.Size(58, 13);
            this.lblSpecificity.TabIndex = 9;
            this.lblSpecificity.Text = "Specificity:";
            // 
            // lblSensitivity
            // 
            this.lblSensitivity.AutoSize = true;
            this.lblSensitivity.Location = new System.Drawing.Point(87, 97);
            this.lblSensitivity.Name = "lblSensitivity";
            this.lblSensitivity.Size = new System.Drawing.Size(57, 13);
            this.lblSensitivity.TabIndex = 8;
            this.lblSensitivity.Text = "Sensitivity:";
            // 
            // tbFN
            // 
            this.tbFN.Location = new System.Drawing.Point(150, 69);
            this.tbFN.Name = "tbFN";
            this.tbFN.ReadOnly = true;
            this.tbFN.Size = new System.Drawing.Size(43, 20);
            this.tbFN.TabIndex = 7;
            this.tbFN.TabStop = false;
            // 
            // tbFP
            // 
            this.tbFP.Location = new System.Drawing.Point(150, 22);
            this.tbFP.Name = "tbFP";
            this.tbFP.ReadOnly = true;
            this.tbFP.Size = new System.Drawing.Size(43, 20);
            this.tbFP.TabIndex = 6;
            this.tbFP.TabStop = false;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(16, 72);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(128, 13);
            this.label16.TabIndex = 5;
            this.label16.Text = "False Negatives (Type II):";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(25, 25);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(119, 13);
            this.label17.TabIndex = 4;
            this.label17.Text = "False Positives (Type I):";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupBox10);
            this.groupBox2.Controls.Add(this.btnUpdate);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.tbThresholdReg);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.tbThresholdDec);
            this.groupBox2.Location = new System.Drawing.Point(3, 65);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(226, 177);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Plot Thresholds";
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.txtPwrValue);
            this.groupBox10.Controls.Add(this.rbPwrValue);
            this.groupBox10.Controls.Add(this.rbLogeValue);
            this.groupBox10.Controls.Add(this.rbLog10Value);
            this.groupBox10.Controls.Add(this.rbValue);
            this.groupBox10.Location = new System.Drawing.Point(63, 71);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(155, 100);
            this.groupBox10.TabIndex = 86;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Threshold Transform";
            // 
            // txtPwrValue
            // 
            this.txtPwrValue.Enabled = false;
            this.txtPwrValue.Location = new System.Drawing.Point(72, 74);
            this.txtPwrValue.Name = "txtPwrValue";
            this.txtPwrValue.Size = new System.Drawing.Size(53, 20);
            this.txtPwrValue.TabIndex = 90;
            this.txtPwrValue.Text = "1.0";
            this.txtPwrValue.Leave += new System.EventHandler(this.txtPwr_Leave);
            // 
            // rbPwrValue
            // 
            this.rbPwrValue.AutoSize = true;
            this.rbPwrValue.Location = new System.Drawing.Point(5, 77);
            this.rbPwrValue.Name = "rbPwrValue";
            this.rbPwrValue.Size = new System.Drawing.Size(55, 17);
            this.rbPwrValue.TabIndex = 89;
            this.rbPwrValue.Text = "Power";
            this.rbPwrValue.UseVisualStyleBackColor = true;
            this.rbPwrValue.CheckedChanged += new System.EventHandler(this.rbPwrValue_CheckedChanged);
            // 
            // rbLogeValue
            // 
            this.rbLogeValue.AutoSize = true;
            this.rbLogeValue.Location = new System.Drawing.Point(5, 57);
            this.rbLogeValue.Name = "rbLogeValue";
            this.rbLogeValue.Size = new System.Drawing.Size(37, 17);
            this.rbLogeValue.TabIndex = 88;
            this.rbLogeValue.Text = "Ln";
            this.rbLogeValue.UseVisualStyleBackColor = true;
            this.rbLogeValue.CheckedChanged += new System.EventHandler(this.rbLogeValue_CheckedChanged_1);
            // 
            // rbLog10Value
            // 
            this.rbLog10Value.AutoSize = true;
            this.rbLog10Value.Location = new System.Drawing.Point(5, 38);
            this.rbLog10Value.Name = "rbLog10Value";
            this.rbLog10Value.Size = new System.Drawing.Size(55, 17);
            this.rbLog10Value.TabIndex = 87;
            this.rbLog10Value.Text = "Log10";
            this.rbLog10Value.UseVisualStyleBackColor = true;
            this.rbLog10Value.CheckedChanged += new System.EventHandler(this.rbLog10Value_CheckedChanged_1);
            // 
            // rbValue
            // 
            this.rbValue.AutoSize = true;
            this.rbValue.Checked = true;
            this.rbValue.Location = new System.Drawing.Point(5, 20);
            this.rbValue.Name = "rbValue";
            this.rbValue.Size = new System.Drawing.Size(51, 17);
            this.rbValue.TabIndex = 86;
            this.rbValue.TabStop = true;
            this.rbValue.Text = "None";
            this.rbValue.UseVisualStyleBackColor = true;
            this.rbValue.CheckedChanged += new System.EventHandler(this.rbValue_CheckedChanged_1);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(5, 114);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(52, 23);
            this.btnUpdate.TabIndex = 83;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnXYPlot_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.Color.Green;
            this.label10.Location = new System.Drawing.Point(70, 48);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(148, 13);
            this.label10.TabIndex = 82;
            this.label10.Text = "Regulatory Standard (Vertical)";
            // 
            // tbThresholdReg
            // 
            this.tbThresholdReg.Location = new System.Drawing.Point(31, 44);
            this.tbThresholdReg.Name = "tbThresholdReg";
            this.tbThresholdReg.Size = new System.Drawing.Size(33, 20);
            this.tbThresholdReg.TabIndex = 82;
            this.tbThresholdReg.Text = "235";
            this.tbThresholdReg.TextChanged += new System.EventHandler(this.tbThresholdReg_TextChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ForeColor = System.Drawing.Color.Blue;
            this.label11.Location = new System.Drawing.Point(70, 24);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(145, 13);
            this.label11.TabIndex = 81;
            this.label11.Text = "Decision Criterion (Horizontal)";
            // 
            // tbThresholdDec
            // 
            this.tbThresholdDec.Location = new System.Drawing.Point(31, 19);
            this.tbThresholdDec.Name = "tbThresholdDec";
            this.tbThresholdDec.Size = new System.Drawing.Size(33, 20);
            this.tbThresholdDec.TabIndex = 13;
            this.tbThresholdDec.Text = "235";
            this.tbThresholdDec.TextChanged += new System.EventHandler(this.tbThresholdDec_TextChanged);
            // 
            // zgc
            // 
            this.zgc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.zgc.EditButtons = System.Windows.Forms.MouseButtons.Left;
            this.zgc.Location = new System.Drawing.Point(235, 13);
            this.zgc.Name = "zgc";
            this.zgc.PanModifierKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)));
            this.zgc.ScrollGrace = 0D;
            this.zgc.ScrollMaxX = 0D;
            this.zgc.ScrollMaxY = 0D;
            this.zgc.ScrollMaxY2 = 0D;
            this.zgc.ScrollMinX = 0D;
            this.zgc.ScrollMinY = 0D;
            this.zgc.ScrollMinY2 = 0D;
            this.zgc.Size = new System.Drawing.Size(582, 387);
            this.zgc.TabIndex = 10;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cboxPlotList);
            this.groupBox1.Location = new System.Drawing.Point(3, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(226, 54);
            this.groupBox1.TabIndex = 85;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select View";
            // 
            // cboxPlotList
            // 
            this.cboxPlotList.FormattingEnabled = true;
            this.cboxPlotList.Location = new System.Drawing.Point(13, 20);
            this.cboxPlotList.Name = "cboxPlotList";
            this.cboxPlotList.Size = new System.Drawing.Size(202, 21);
            this.cboxPlotList.TabIndex = 16;
            this.cboxPlotList.SelectedIndexChanged += new System.EventHandler(this.cboxPlotList_SelectedIndexChanged);
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colProbExceed,
            this.colFN,
            this.colFP,
            this.colTot,
            this.colSensitivity,
            this.colSpecificity,
            this.colAccuracy});
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(235, 12);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(582, 385);
            this.listView1.TabIndex = 86;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.Visible = false;
            this.listView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView1_KeyDown);
            // 
            // colProbExceed
            // 
            this.colProbExceed.Text = "Probability of Exceedance";
            this.colProbExceed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.colProbExceed.Width = 220;
            // 
            // colFN
            // 
            this.colFN.Text = "False Non-Exceed";
            this.colFN.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.colFN.Width = 120;
            // 
            // colFP
            // 
            this.colFP.Text = "False Exceed";
            this.colFP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.colFP.Width = 120;
            // 
            // colTot
            // 
            this.colTot.Text = "Total";
            this.colTot.Width = 80;
            // 
            // colSensitivity
            // 
            this.colSensitivity.Text = "Sensitivity";
            this.colSensitivity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.colSensitivity.Width = 160;
            // 
            // colSpecificity
            // 
            this.colSpecificity.Text = "Specificity";
            this.colSpecificity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.colSpecificity.Width = 160;
            // 
            // colAccuracy
            // 
            this.colAccuracy.Text = "Accuracy";
            this.colAccuracy.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.colAccuracy.Width = 160;
            // 
            // MLRPlots
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.zgc);
            this.Name = "MLRPlots";
            this.Size = new System.Drawing.Size(822, 412);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txbAccuracy;
        private System.Windows.Forms.TextBox txbSpecificity;
        private System.Windows.Forms.TextBox txbSensitivity;
        private System.Windows.Forms.Label lblAccuracy;
        private System.Windows.Forms.Label lblSpecificity;
        private System.Windows.Forms.Label lblSensitivity;
        private System.Windows.Forms.TextBox tbFN;
        private System.Windows.Forms.TextBox tbFP;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.TextBox txtPwrValue;
        private System.Windows.Forms.RadioButton rbPwrValue;
        private System.Windows.Forms.RadioButton rbLogeValue;
        private System.Windows.Forms.RadioButton rbLog10Value;
        private System.Windows.Forms.RadioButton rbValue;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbThresholdReg;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tbThresholdDec;
        public ZedGraph.ZedGraphControl zgc;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cboxPlotList;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader colProbExceed;
        private System.Windows.Forms.ColumnHeader colFN;
        private System.Windows.Forms.ColumnHeader colFP;
        private System.Windows.Forms.ColumnHeader colSensitivity;
        private System.Windows.Forms.ColumnHeader colSpecificity;
        private System.Windows.Forms.ColumnHeader colAccuracy;
        private System.Windows.Forms.ColumnHeader colTot;
    }
}
