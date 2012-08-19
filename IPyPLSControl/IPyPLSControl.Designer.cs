namespace IPyPLSControl
{
    partial class IPyPLSControl
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
            this.variable_names = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.model_coefficients = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.variable_influence = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label40 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblNcomp = new System.Windows.Forms.Label();
            this.groupBox10.SuspendLayout();
            this.pnlThresholdingButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartValidation)).BeginInit();
            this.groupBox6.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSelectModel
            // 
            this.btnSelectModel.Visible = false;
            // 
            // lvModel
            // 
            this.lvModel.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.variable_names,
            this.model_coefficients,
            this.variable_influence});
            // 
            // variable_names
            // 
            this.variable_names.Text = "Variable";
            this.variable_names.Width = 88;
            // 
            // model_coefficients
            // 
            this.model_coefficients.Text = "Coefficient";
            this.model_coefficients.Width = 78;
            // 
            // variable_influence
            // 
            this.variable_influence.Text = "Influence";
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.ForeColor = System.Drawing.Color.Crimson;
            this.label40.Location = new System.Drawing.Point(55, 164);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(200, 13);
            this.label40.TabIndex = 116;
            this.label40.Text = "Number of PLS components in the model";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 165);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 117;
            // 
            // lblNcomp
            // 
            this.lblNcomp.AutoSize = true;
            this.lblNcomp.Location = new System.Drawing.Point(13, 164);
            this.lblNcomp.Name = "lblNcomp";
            this.lblNcomp.Size = new System.Drawing.Size(0, 13);
            this.lblNcomp.TabIndex = 118;
            // 
            // IPyPLSControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.lblNcomp);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label40);
            this.Name = "IPyPLSControl";
            
            this.groupBox10.ResumeLayout(false);
            this.pnlThresholdingButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartValidation)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.groupBox13.ResumeLayout(false);
            this.groupBox13.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
           

        }
        #endregion

        protected System.Windows.Forms.ColumnHeader variable_names;
        protected System.Windows.Forms.ColumnHeader model_coefficients;
        protected System.Windows.Forms.ColumnHeader variable_influence;
        protected System.Windows.Forms.Label label40;
        protected System.Windows.Forms.Label label2;
        protected System.Windows.Forms.Label lblNcomp;
    }
}
