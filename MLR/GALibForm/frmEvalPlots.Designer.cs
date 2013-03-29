namespace GALibForm
{
    partial class frmEvalPlots
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
            this.zgc1 = new ZedGraph.ZedGraphControl();
            this.lblModel = new System.Windows.Forms.Label();
            this.lblModelExpr = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // zgc1
            // 
            this.zgc1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.zgc1.IsShowPointValues = true;
            this.zgc1.Location = new System.Drawing.Point(12, 84);
            this.zgc1.Name = "zgc1";
            this.zgc1.ScrollGrace = 0;
            this.zgc1.ScrollMaxX = 0;
            this.zgc1.ScrollMaxY = 0;
            this.zgc1.ScrollMaxY2 = 0;
            this.zgc1.ScrollMinX = 0;
            this.zgc1.ScrollMinY = 0;
            this.zgc1.ScrollMinY2 = 0;
            this.zgc1.Size = new System.Drawing.Size(772, 494);
            this.zgc1.TabIndex = 0;
            this.zgc1.MouseMoveEvent += new ZedGraph.ZedGraphControl.ZedMouseEventHandler(this.zgc1_MouseMoveEvent);
            // 
            // lblModel
            // 
            this.lblModel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblModel.AutoSize = true;
            this.lblModel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblModel.Location = new System.Drawing.Point(347, 9);
            this.lblModel.Name = "lblModel";
            this.lblModel.Size = new System.Drawing.Size(116, 20);
            this.lblModel.TabIndex = 1;
            this.lblModel.Text = "Eval Criterion";
            // 
            // lblModelExpr
            // 
            this.lblModelExpr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblModelExpr.Location = new System.Drawing.Point(12, 31);
            this.lblModelExpr.Name = "lblModelExpr";
            this.lblModelExpr.Size = new System.Drawing.Size(772, 50);
            this.lblModelExpr.TabIndex = 2;
            this.lblModelExpr.Text = "Model";
            // 
            // frmEvalPlots
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(796, 590);
            this.Controls.Add(this.lblModelExpr);
            this.Controls.Add(this.lblModel);
            this.Controls.Add(this.zgc1);
            this.Name = "frmEvalPlots";
            this.Text = "Model Evaluation Criteria";
            this.Load += new System.EventHandler(this.frmEvalPlots_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ZedGraph.ZedGraphControl zgc1;
        private System.Windows.Forms.Label lblModel;
        private System.Windows.Forms.Label lblModelExpr;
    }
}