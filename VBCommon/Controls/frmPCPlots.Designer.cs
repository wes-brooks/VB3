namespace VBCommon.Controls
{
    partial class frmPCPlots
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
            this.zgc1 = new ZedGraph.ZedGraphControl();
            this.cbSelectPlot = new System.Windows.Forms.ComboBox();
            this.lv1 = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.SuspendLayout();
            // 
            // zgc1
            // 
            this.zgc1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.zgc1.Location = new System.Drawing.Point(12, 97);
            this.zgc1.Name = "zgc1";
            this.zgc1.ScrollGrace = 0D;
            this.zgc1.ScrollMaxX = 0D;
            this.zgc1.ScrollMaxY = 0D;
            this.zgc1.ScrollMaxY2 = 0D;
            this.zgc1.ScrollMinX = 0D;
            this.zgc1.ScrollMinY = 0D;
            this.zgc1.ScrollMinY2 = 0D;
            this.zgc1.Size = new System.Drawing.Size(743, 363);
            this.zgc1.TabIndex = 0;
            // 
            // cbSelectPlot
            // 
            this.cbSelectPlot.FormattingEnabled = true;
            this.cbSelectPlot.Location = new System.Drawing.Point(12, 17);
            this.cbSelectPlot.Name = "cbSelectPlot";
            this.cbSelectPlot.Size = new System.Drawing.Size(121, 21);
            this.cbSelectPlot.TabIndex = 1;
            this.cbSelectPlot.SelectedIndexChanged += new System.EventHandler(this.cbSelectPlot_SelectedIndexChanged);
            // 
            // lv1
            // 
            this.lv1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lv1.GridLines = true;
            this.lv1.Location = new System.Drawing.Point(139, 12);
            this.lv1.Name = "lv1";
            this.lv1.Size = new System.Drawing.Size(616, 79);
            this.lv1.TabIndex = 2;
            this.lv1.UseCompatibleStateImageBehavior = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Select Plots";
            // 
            // frmPCPlots
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(767, 469);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lv1);
            this.Controls.Add(this.cbSelectPlot);
            this.Controls.Add(this.zgc1);
            this.Name = "frmPCPlots";
            this.Text = "frmPCPlots";
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.frmPCPlots_HelpRequested);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ZedGraph.ZedGraphControl zgc1;
        private System.Windows.Forms.ComboBox cbSelectPlot;
        private System.Windows.Forms.ListView lv1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.HelpProvider helpProvider1;
    }
}