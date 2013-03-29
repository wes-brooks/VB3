namespace MLRPrediction
{
    partial class frmMLRPredObs
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
            this.btnClose = new System.Windows.Forms.Button();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.mlrPlots1 = new VBControls.MLRPlots();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClose.Location = new System.Drawing.Point(12, 411);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // mlrPlots1
            // 
            this.mlrPlots1.AutoScroll = true;
            this.mlrPlots1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mlrPlots1.Location = new System.Drawing.Point(0, 0);
            this.mlrPlots1.Name = "mlrPlots1";
            this.mlrPlots1.PowerExponent = double.NaN;
            this.mlrPlots1.Size = new System.Drawing.Size(711, 456);
            this.mlrPlots1.TabIndex = 2;
            this.mlrPlots1.Transform = VBTools.Globals.DependendVariableTransforms.none;
            // 
            // frmMLRPredObs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(711, 456);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.mlrPlots1);
            this.Name = "frmMLRPredObs";
            this.Text = "Plot";
            this.Load += new System.EventHandler(this.frmMLRPredObs_Load);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.frmMLRPredObs_HelpRequested);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private VBControls.MLRPlots mlrPlots1;
    }
}