namespace VBCommon.Controls
{
    partial class frmTransform
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
            this.cbIVLog10 = new System.Windows.Forms.CheckBox();
            this.cbInverse = new System.Windows.Forms.CheckBox();
            this.cbSquare = new System.Windows.Forms.CheckBox();
            this.cbSqrRoot = new System.Windows.Forms.CheckBox();
            this.cbQuadRoot = new System.Windows.Forms.CheckBox();
            this.cbPoly = new System.Windows.Forms.CheckBox();
            this.cbGenExponent = new System.Windows.Forms.CheckBox();
            this.tbExponent = new System.Windows.Forms.TextBox();
            this.btnGo = new System.Windows.Forms.Button();
            this.cbSelectAll = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbResponseVar = new System.Windows.Forms.GroupBox();
            this.btnClearDVTs = new System.Windows.Forms.Button();
            this.rbGeneralExp = new System.Windows.Forms.RadioButton();
            this.tbrvExponent = new System.Windows.Forms.TextBox();
            this.rbLn = new System.Windows.Forms.RadioButton();
            this.rbLog10 = new System.Windows.Forms.RadioButton();
            this.gbIVs = new System.Windows.Forms.GroupBox();
            this.cbIVLn = new System.Windows.Forms.CheckBox();
            this.lbDepVarName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.gbResponseVar.SuspendLayout();
            this.gbIVs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // cbIVLog10
            // 
            this.cbIVLog10.AutoSize = true;
            this.cbIVLog10.Location = new System.Drawing.Point(36, 24);
            this.cbIVLog10.Name = "cbIVLog10";
            this.cbIVLog10.Size = new System.Drawing.Size(56, 17);
            this.cbIVLog10.TabIndex = 22;
            this.cbIVLog10.Text = "Log10";
            this.cbIVLog10.UseVisualStyleBackColor = true;
            this.cbIVLog10.CheckedChanged += new System.EventHandler(this.cbIVLog10_CheckedChanged);
            // 
            // cbInverse
            // 
            this.cbInverse.AutoSize = true;
            this.cbInverse.Location = new System.Drawing.Point(36, 70);
            this.cbInverse.Name = "cbInverse";
            this.cbInverse.Size = new System.Drawing.Size(61, 17);
            this.cbInverse.TabIndex = 24;
            this.cbInverse.Text = "Inverse";
            this.cbInverse.UseVisualStyleBackColor = true;
            this.cbInverse.CheckedChanged += new System.EventHandler(this.cbInverse_CheckedChanged);
            // 
            // cbSquare
            // 
            this.cbSquare.AutoSize = true;
            this.cbSquare.Location = new System.Drawing.Point(36, 93);
            this.cbSquare.Name = "cbSquare";
            this.cbSquare.Size = new System.Drawing.Size(60, 17);
            this.cbSquare.TabIndex = 25;
            this.cbSquare.Text = "Square";
            this.cbSquare.UseVisualStyleBackColor = true;
            this.cbSquare.CheckedChanged += new System.EventHandler(this.cbSquare_CheckedChanged);
            // 
            // cbSqrRoot
            // 
            this.cbSqrRoot.AutoSize = true;
            this.cbSqrRoot.Location = new System.Drawing.Point(36, 116);
            this.cbSqrRoot.Name = "cbSqrRoot";
            this.cbSqrRoot.Size = new System.Drawing.Size(83, 17);
            this.cbSqrRoot.TabIndex = 26;
            this.cbSqrRoot.Text = "SquareRoot";
            this.cbSqrRoot.UseVisualStyleBackColor = true;
            this.cbSqrRoot.CheckedChanged += new System.EventHandler(this.cbSqrRoot_CheckedChanged);
            // 
            // cbQuadRoot
            // 
            this.cbQuadRoot.AutoSize = true;
            this.cbQuadRoot.Location = new System.Drawing.Point(36, 139);
            this.cbQuadRoot.Name = "cbQuadRoot";
            this.cbQuadRoot.Size = new System.Drawing.Size(75, 17);
            this.cbQuadRoot.TabIndex = 27;
            this.cbQuadRoot.Text = "QuadRoot";
            this.cbQuadRoot.UseVisualStyleBackColor = true;
            this.cbQuadRoot.CheckedChanged += new System.EventHandler(this.cbQuadRoot_CheckedChanged);
            // 
            // cbPoly
            // 
            this.cbPoly.AutoSize = true;
            this.cbPoly.Location = new System.Drawing.Point(36, 162);
            this.cbPoly.Name = "cbPoly";
            this.cbPoly.Size = new System.Drawing.Size(76, 17);
            this.cbPoly.TabIndex = 28;
            this.cbPoly.Text = "Polynomial";
            this.cbPoly.UseVisualStyleBackColor = true;
            this.cbPoly.CheckedChanged += new System.EventHandler(this.cbPoly_CheckedChanged);
            // 
            // cbGenExponent
            // 
            this.cbGenExponent.AutoSize = true;
            this.cbGenExponent.Location = new System.Drawing.Point(36, 185);
            this.cbGenExponent.Name = "cbGenExponent";
            this.cbGenExponent.Size = new System.Drawing.Size(111, 17);
            this.cbGenExponent.TabIndex = 29;
            this.cbGenExponent.Text = "General Exponent";
            this.cbGenExponent.UseVisualStyleBackColor = true;
            this.cbGenExponent.CheckedChanged += new System.EventHandler(this.cbGenExponent_CheckedChanged);
            // 
            // tbExponent
            // 
            this.tbExponent.Location = new System.Drawing.Point(153, 182);
            this.tbExponent.Name = "tbExponent";
            this.tbExponent.Size = new System.Drawing.Size(28, 20);
            this.tbExponent.TabIndex = 32;
            this.tbExponent.Text = "1.0";
            this.tbExponent.Validating += new System.ComponentModel.CancelEventHandler(this.tbExponent_Validating);
            this.tbExponent.Validated += new System.EventHandler(this.tbExponent_Validated);
            // 
            // btnGo
            // 
            this.btnGo.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnGo.Location = new System.Drawing.Point(299, 134);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(71, 23);
            this.btnGo.TabIndex = 33;
            this.btnGo.Text = "Go";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // cbSelectAll
            // 
            this.cbSelectAll.AutoSize = true;
            this.cbSelectAll.Location = new System.Drawing.Point(56, 215);
            this.cbSelectAll.Name = "cbSelectAll";
            this.cbSelectAll.Size = new System.Drawing.Size(70, 17);
            this.cbSelectAll.TabIndex = 34;
            this.cbSelectAll.Text = "Select All";
            this.cbSelectAll.UseVisualStyleBackColor = true;
            this.cbSelectAll.CheckedChanged += new System.EventHandler(this.cbSelectAll_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(299, 186);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(71, 23);
            this.btnCancel.TabIndex = 35;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // gbResponseVar
            // 
            this.gbResponseVar.Controls.Add(this.btnClearDVTs);
            this.gbResponseVar.Controls.Add(this.rbGeneralExp);
            this.gbResponseVar.Controls.Add(this.tbrvExponent);
            this.gbResponseVar.Controls.Add(this.rbLn);
            this.gbResponseVar.Controls.Add(this.rbLog10);
            this.gbResponseVar.Location = new System.Drawing.Point(80, 284);
            this.gbResponseVar.Name = "gbResponseVar";
            this.gbResponseVar.Size = new System.Drawing.Size(237, 135);
            this.gbResponseVar.TabIndex = 36;
            this.gbResponseVar.TabStop = false;
            this.gbResponseVar.Text = "Dependent Variable";
            this.gbResponseVar.Visible = false;
            // 
            // btnClearDVTs
            // 
            this.btnClearDVTs.Location = new System.Drawing.Point(76, 94);
            this.btnClearDVTs.Name = "btnClearDVTs";
            this.btnClearDVTs.Size = new System.Drawing.Size(71, 23);
            this.btnClearDVTs.TabIndex = 35;
            this.btnClearDVTs.Text = "Clear";
            this.btnClearDVTs.UseVisualStyleBackColor = true;
            this.btnClearDVTs.Click += new System.EventHandler(this.btnClearDVTs_Click);
            // 
            // rbGeneralExp
            // 
            this.rbGeneralExp.AutoSize = true;
            this.rbGeneralExp.Location = new System.Drawing.Point(36, 60);
            this.rbGeneralExp.Name = "rbGeneralExp";
            this.rbGeneralExp.Size = new System.Drawing.Size(110, 17);
            this.rbGeneralExp.TabIndex = 34;
            this.rbGeneralExp.TabStop = true;
            this.rbGeneralExp.Text = "General Exponent";
            this.rbGeneralExp.UseVisualStyleBackColor = true;
            this.rbGeneralExp.CheckedChanged += new System.EventHandler(this.rbGeneralExp_CheckedChanged);
            // 
            // tbrvExponent
            // 
            this.tbrvExponent.Location = new System.Drawing.Point(153, 59);
            this.tbrvExponent.Name = "tbrvExponent";
            this.tbrvExponent.Size = new System.Drawing.Size(28, 20);
            this.tbrvExponent.TabIndex = 33;
            this.tbrvExponent.Text = "1.0";
            this.tbrvExponent.Validating += new System.ComponentModel.CancelEventHandler(this.tbrvExponent_Validating);
            this.tbrvExponent.Validated += new System.EventHandler(this.tbrvExponent_Validated);
            // 
            // rbLn
            // 
            this.rbLn.AutoSize = true;
            this.rbLn.Location = new System.Drawing.Point(143, 30);
            this.rbLn.Name = "rbLn";
            this.rbLn.Size = new System.Drawing.Size(37, 17);
            this.rbLn.TabIndex = 23;
            this.rbLn.TabStop = true;
            this.rbLn.Text = "Ln";
            this.rbLn.UseVisualStyleBackColor = true;
            this.rbLn.CheckedChanged += new System.EventHandler(this.rbLn_CheckedChanged);
            // 
            // rbLog10
            // 
            this.rbLog10.AutoSize = true;
            this.rbLog10.Location = new System.Drawing.Point(36, 30);
            this.rbLog10.Name = "rbLog10";
            this.rbLog10.Size = new System.Drawing.Size(55, 17);
            this.rbLog10.TabIndex = 22;
            this.rbLog10.TabStop = true;
            this.rbLog10.Text = "Log10";
            this.rbLog10.UseVisualStyleBackColor = true;
            this.rbLog10.CheckedChanged += new System.EventHandler(this.rbLog10_CheckedChanged);
            // 
            // gbIVs
            // 
            this.gbIVs.Controls.Add(this.cbSquare);
            this.gbIVs.Controls.Add(this.cbIVLog10);
            this.gbIVs.Controls.Add(this.cbSelectAll);
            this.gbIVs.Controls.Add(this.cbInverse);
            this.gbIVs.Controls.Add(this.cbSqrRoot);
            this.gbIVs.Controls.Add(this.tbExponent);
            this.gbIVs.Controls.Add(this.cbQuadRoot);
            this.gbIVs.Controls.Add(this.cbPoly);
            this.gbIVs.Controls.Add(this.cbIVLn);
            this.gbIVs.Controls.Add(this.cbGenExponent);
            this.gbIVs.Location = new System.Drawing.Point(24, 24);
            this.gbIVs.Name = "gbIVs";
            this.gbIVs.Size = new System.Drawing.Size(237, 247);
            this.gbIVs.TabIndex = 37;
            this.gbIVs.TabStop = false;
            this.gbIVs.Text = "Available Transforms";
            // 
            // cbIVLn
            // 
            this.cbIVLn.AutoSize = true;
            this.cbIVLn.Location = new System.Drawing.Point(36, 46);
            this.cbIVLn.Name = "cbIVLn";
            this.cbIVLn.Size = new System.Drawing.Size(38, 17);
            this.cbIVLn.TabIndex = 30;
            this.cbIVLn.Text = "Ln";
            this.cbIVLn.UseVisualStyleBackColor = true;
            this.cbIVLn.CheckedChanged += new System.EventHandler(this.cbIVLn_CheckedChanged);
            // 
            // lbDepVarName
            // 
            this.lbDepVarName.AutoSize = true;
            this.lbDepVarName.Location = new System.Drawing.Point(279, 71);
            this.lbDepVarName.Name = "lbDepVarName";
            this.lbDepVarName.Size = new System.Drawing.Size(113, 13);
            this.lbDepVarName.TabIndex = 38;
            this.lbDepVarName.Text = "<Dependent Variable>";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(279, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 13);
            this.label1.TabIndex = 39;
            this.label1.Text = "Dependent Variable:";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // frmTransform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 318);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbDepVarName);
            this.Controls.Add(this.gbIVs);
            this.Controls.Add(this.gbResponseVar);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnGo);
            this.Name = "frmTransform";
            this.Text = "Transforms to Perform ";
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.frmTransform_HelpRequested);
            this.gbResponseVar.ResumeLayout(false);
            this.gbResponseVar.PerformLayout();
            this.gbIVs.ResumeLayout(false);
            this.gbIVs.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbIVLog10;
        private System.Windows.Forms.CheckBox cbInverse;
        private System.Windows.Forms.CheckBox cbSquare;
        private System.Windows.Forms.CheckBox cbSqrRoot;
        private System.Windows.Forms.CheckBox cbQuadRoot;
        private System.Windows.Forms.CheckBox cbPoly;
        private System.Windows.Forms.CheckBox cbGenExponent;
        private System.Windows.Forms.TextBox tbExponent;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.CheckBox cbSelectAll;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox gbResponseVar;
        private System.Windows.Forms.GroupBox gbIVs;
        private System.Windows.Forms.RadioButton rbLn;
        private System.Windows.Forms.RadioButton rbLog10;
        private System.Windows.Forms.RadioButton rbGeneralExp;
        private System.Windows.Forms.TextBox tbrvExponent;
        private System.Windows.Forms.Label lbDepVarName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnClearDVTs;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.CheckBox cbIVLn;
        private System.Windows.Forms.HelpProvider helpProvider1;
    }
}