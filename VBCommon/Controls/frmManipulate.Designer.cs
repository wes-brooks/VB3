namespace VBCommon.Controls
{
    partial class frmManipulate
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
            this.btnOk = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnInteractions = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbMin = new System.Windows.Forms.RadioButton();
            this.rbMax = new System.Windows.Forms.RadioButton();
            this.rbProduct = new System.Windows.Forms.RadioButton();
            this.rbMean = new System.Windows.Forms.RadioButton();
            this.rbSum = new System.Windows.Forms.RadioButton();
            this.txtExpression = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lbAvailableVariables = new System.Windows.Forms.ListBox();
            this.lbIndVariables = new System.Windows.Forms.ListBox();
            this.btnRemoveExp = new System.Windows.Forms.Button();
            this.lbExpressions = new System.Windows.Forms.ListBox();
            this.btnAddExp = new System.Windows.Forms.Button();
            this.btnRemoveIV = new System.Windows.Forms.Button();
            this.btnAddIV = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(155, 367);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(77, 23);
            this.btnOk.TabIndex = 13;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnInteractions);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.txtExpression);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.lbAvailableVariables);
            this.groupBox1.Controls.Add(this.lbIndVariables);
            this.groupBox1.Controls.Add(this.btnRemoveExp);
            this.groupBox1.Controls.Add(this.lbExpressions);
            this.groupBox1.Controls.Add(this.btnAddExp);
            this.groupBox1.Controls.Add(this.btnRemoveIV);
            this.groupBox1.Controls.Add(this.btnAddIV);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(538, 335);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Build Expression";
            // 
            // btnInteractions
            // 
            this.btnInteractions.Location = new System.Drawing.Point(402, 188);
            this.btnInteractions.Name = "btnInteractions";
            this.btnInteractions.Size = new System.Drawing.Size(120, 23);
            this.btnInteractions.TabIndex = 38;
            this.btnInteractions.Text = "2nd Order Interactions";
            this.btnInteractions.UseVisualStyleBackColor = true;
            this.btnInteractions.Click += new System.EventHandler(this.btnInteractions_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbMin);
            this.groupBox2.Controls.Add(this.rbMax);
            this.groupBox2.Controls.Add(this.rbProduct);
            this.groupBox2.Controls.Add(this.rbMean);
            this.groupBox2.Controls.Add(this.rbSum);
            this.groupBox2.Location = new System.Drawing.Point(204, 117);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(322, 36);
            this.groupBox2.TabIndex = 37;
            this.groupBox2.TabStop = false;
            // 
            // rbMin
            // 
            this.rbMin.AutoSize = true;
            this.rbMin.Location = new System.Drawing.Point(129, 13);
            this.rbMin.Name = "rbMin";
            this.rbMin.Size = new System.Drawing.Size(66, 17);
            this.rbMin.TabIndex = 33;
            this.rbMin.Text = "Minimum";
            this.rbMin.UseVisualStyleBackColor = true;
            this.rbMin.CheckedChanged += new System.EventHandler(this.rbMin_CheckedChanged);
            // 
            // rbMax
            // 
            this.rbMax.AutoSize = true;
            this.rbMax.Location = new System.Drawing.Point(55, 12);
            this.rbMax.Name = "rbMax";
            this.rbMax.Size = new System.Drawing.Size(69, 17);
            this.rbMax.TabIndex = 32;
            this.rbMax.Text = "Maximum";
            this.rbMax.UseVisualStyleBackColor = true;
            this.rbMax.CheckedChanged += new System.EventHandler(this.rbMax_CheckedChanged);
            // 
            // rbProduct
            // 
            this.rbProduct.AutoSize = true;
            this.rbProduct.Location = new System.Drawing.Point(256, 12);
            this.rbProduct.Name = "rbProduct";
            this.rbProduct.Size = new System.Drawing.Size(62, 17);
            this.rbProduct.TabIndex = 31;
            this.rbProduct.Text = "Product";
            this.rbProduct.UseVisualStyleBackColor = true;
            this.rbProduct.CheckedChanged += new System.EventHandler(this.rbProduct_CheckedChanged);
            // 
            // rbMean
            // 
            this.rbMean.AutoSize = true;
            this.rbMean.Location = new System.Drawing.Point(198, 13);
            this.rbMean.Name = "rbMean";
            this.rbMean.Size = new System.Drawing.Size(52, 17);
            this.rbMean.TabIndex = 30;
            this.rbMean.Text = "Mean";
            this.rbMean.UseVisualStyleBackColor = true;
            this.rbMean.CheckedChanged += new System.EventHandler(this.rbMean_CheckedChanged);
            // 
            // rbSum
            // 
            this.rbSum.AutoSize = true;
            this.rbSum.Checked = true;
            this.rbSum.Location = new System.Drawing.Point(6, 12);
            this.rbSum.Name = "rbSum";
            this.rbSum.Size = new System.Drawing.Size(46, 17);
            this.rbSum.TabIndex = 29;
            this.rbSum.TabStop = true;
            this.rbSum.Text = "Sum";
            this.rbSum.UseVisualStyleBackColor = true;
            this.rbSum.CheckedChanged += new System.EventHandler(this.rbSum_CheckedChanged);
            // 
            // txtExpression
            // 
            this.txtExpression.Location = new System.Drawing.Point(205, 159);
            this.txtExpression.Name = "txtExpression";
            this.txtExpression.ReadOnly = true;
            this.txtExpression.Size = new System.Drawing.Size(317, 20);
            this.txtExpression.TabIndex = 36;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(202, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 13);
            this.label2.TabIndex = 35;
            this.label2.Text = "Variables in Expression";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 13);
            this.label1.TabIndex = 34;
            this.label1.Text = "Independent Variables";
            // 
            // lbAvailableVariables
            // 
            this.lbAvailableVariables.FormattingEnabled = true;
            this.lbAvailableVariables.Location = new System.Drawing.Point(16, 41);
            this.lbAvailableVariables.Name = "lbAvailableVariables";
            this.lbAvailableVariables.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbAvailableVariables.Size = new System.Drawing.Size(145, 251);
            this.lbAvailableVariables.TabIndex = 33;
            // 
            // lbIndVariables
            // 
            this.lbIndVariables.FormattingEnabled = true;
            this.lbIndVariables.Location = new System.Drawing.Point(204, 42);
            this.lbIndVariables.Name = "lbIndVariables";
            this.lbIndVariables.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbIndVariables.Size = new System.Drawing.Size(322, 69);
            this.lbIndVariables.TabIndex = 32;
            // 
            // btnRemoveExp
            // 
            this.btnRemoveExp.Location = new System.Drawing.Point(269, 188);
            this.btnRemoveExp.Name = "btnRemoveExp";
            this.btnRemoveExp.Size = new System.Drawing.Size(59, 23);
            this.btnRemoveExp.TabIndex = 31;
            this.btnRemoveExp.Text = "Remove";
            this.btnRemoveExp.UseVisualStyleBackColor = true;
            this.btnRemoveExp.Click += new System.EventHandler(this.btnRemoveExp_Click);
            // 
            // lbExpressions
            // 
            this.lbExpressions.FormattingEnabled = true;
            this.lbExpressions.Location = new System.Drawing.Point(205, 217);
            this.lbExpressions.Name = "lbExpressions";
            this.lbExpressions.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbExpressions.Size = new System.Drawing.Size(317, 108);
            this.lbExpressions.TabIndex = 30;
            // 
            // btnAddExp
            // 
            this.btnAddExp.Location = new System.Drawing.Point(204, 188);
            this.btnAddExp.Name = "btnAddExp";
            this.btnAddExp.Size = new System.Drawing.Size(59, 23);
            this.btnAddExp.TabIndex = 29;
            this.btnAddExp.Text = "Add";
            this.btnAddExp.UseVisualStyleBackColor = true;
            this.btnAddExp.Click += new System.EventHandler(this.btnAddExp_Click);
            // 
            // btnRemoveIV
            // 
            this.btnRemoveIV.Location = new System.Drawing.Point(171, 76);
            this.btnRemoveIV.Name = "btnRemoveIV";
            this.btnRemoveIV.Size = new System.Drawing.Size(25, 23);
            this.btnRemoveIV.TabIndex = 22;
            this.btnRemoveIV.Text = "<";
            this.btnRemoveIV.UseVisualStyleBackColor = true;
            this.btnRemoveIV.Click += new System.EventHandler(this.btnRemoveIV_Click);
            // 
            // btnAddIV
            // 
            this.btnAddIV.Location = new System.Drawing.Point(171, 47);
            this.btnAddIV.Name = "btnAddIV";
            this.btnAddIV.Size = new System.Drawing.Size(25, 23);
            this.btnAddIV.TabIndex = 21;
            this.btnAddIV.Text = ">";
            this.btnAddIV.UseVisualStyleBackColor = true;
            this.btnAddIV.Click += new System.EventHandler(this.btnAddIV_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(281, 367);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(77, 23);
            this.btnCancel.TabIndex = 22;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmManipulate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(678, 413);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnOk);
            this.Name = "frmManipulate";
            this.Text = "Manipulate";
            this.Load += new System.EventHandler(this.frmManipulate_Load);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.frmManipulate_HelpRequested);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnRemoveExp;
        private System.Windows.Forms.ListBox lbExpressions;
        private System.Windows.Forms.Button btnAddExp;
        private System.Windows.Forms.Button btnRemoveIV;
        private System.Windows.Forms.Button btnAddIV;
        private System.Windows.Forms.ListBox lbIndVariables;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lbAvailableVariables;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbMax;
        private System.Windows.Forms.RadioButton rbProduct;
        private System.Windows.Forms.RadioButton rbMean;
        private System.Windows.Forms.RadioButton rbSum;
        private System.Windows.Forms.TextBox txtExpression;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.RadioButton rbMin;
        private System.Windows.Forms.Button btnInteractions;
        private System.Windows.Forms.HelpProvider helpProvider1;
    }
}