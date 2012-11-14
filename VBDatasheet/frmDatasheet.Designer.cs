namespace VBDatasheet
{
    partial class frmDatasheet
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
            VBCommon.Metadata.Utilities utilities2 = new VBCommon.Metadata.Utilities();
            this.dsControl1 = new VBCommon.Controls.DatasheetControl();
            this.SuspendLayout();
            // 
            // dsControl1
            // 
            this.dsControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dsControl1.CurrentSelectedRowIndex = -1;
            this.dsControl1.DependentVariableTransform = VBCommon.Transforms.DependentVariableTransforms.none;
            //this.dsControl1.DisabledCols = 0;
            //this.dsControl1.DisabledRows = 0;
            this.dsControl1.DT = null;
            this.dsControl1.DTCI = null;
            this.dsControl1.DTRI = null;
            this.dsControl1.FileName = "";
            //this.dsControl1.HiddenCols = 0;
            this.dsControl1.Location = new System.Drawing.Point(3, 3);
            this.dsControl1.Name = "dsControl1";
            //this.dsControl1.NumberIVs = 0;
            this.dsControl1.PowerTransformExponent = double.NaN;
            this.dsControl1.ResponseVarColIndex = 1;
            this.dsControl1.ResponseVarColName = "";
            this.dsControl1.SelectColName = "";
            this.dsControl1.SelectedColIndex = -1;
            this.dsControl1.Size = new System.Drawing.Size(1063, 491);
            this.dsControl1.State = VBCommon.Controls.DatasheetControl.dtState.dirty;
            this.dsControl1.TabIndex = 0;
            //this.dsControl1.Utils = utilities2;
            // 
            // frmDatasheet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dsControl1);
            this.Name = "frmDatasheet";
            this.Size = new System.Drawing.Size(1066, 494);
            this.Tag = "Data Processing";
            this.ResumeLayout(false);

        }

        #endregion

        private VBCommon.Controls.DatasheetControl dsControl1;



    }
}
