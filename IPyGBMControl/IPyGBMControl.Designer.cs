namespace IPyGBMControl
{
    partial class IPyGBMControl
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
            this.groupBox10.SuspendLayout();
            this.pnlThresholdingButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartValidation)).BeginInit();
            this.groupBox6.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.SuspendLayout();
            // 
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
            // IPyGBMControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "IPyGBMControl";
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
    }
}
