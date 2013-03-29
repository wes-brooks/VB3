namespace MLRPlugin
{
    partial class ctlMLRPlugin
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpMLRDatasheet = new System.Windows.Forms.TabPage();
            this.dsControl1 = new VBCommon.Controls.DatasheetControl();
            this.tpMLRModel = new System.Windows.Forms.TabPage();
            this.frmModel1 = new GALibForm.frmModel();
            this.tabControl1.SuspendLayout();
            this.tpMLRDatasheet.SuspendLayout();
            this.tpMLRModel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tpMLRDatasheet);
            this.tabControl1.Controls.Add(this.tpMLRModel);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(909, 447);
            this.tabControl1.TabIndex = 0;
            // 
            // tpMLRDatasheet
            // 
            this.tpMLRDatasheet.Controls.Add(this.dsControl1);
            this.tpMLRDatasheet.Location = new System.Drawing.Point(4, 22);
            this.tpMLRDatasheet.Name = "tpMLRDatasheet";
            this.tpMLRDatasheet.Padding = new System.Windows.Forms.Padding(3);
            this.tpMLRDatasheet.Size = new System.Drawing.Size(901, 421);
            this.tpMLRDatasheet.TabIndex = 0;
            this.tpMLRDatasheet.Text = "Data Manipulation";
            this.tpMLRDatasheet.UseVisualStyleBackColor = true;
            // 
            // dsControl1
            // 
            this.dsControl1.CurrentSelectedRowIndex = -1;
            this.dsControl1.DependentVariableTransform = VBCommon.Transforms.DependentVariableTransforms.none;
            this.dsControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dsControl1.DT = null;
            this.dsControl1.DTCI = null;
            this.dsControl1.DTRI = null;
            this.dsControl1.FileName = "";
            this.dsControl1.Location = new System.Drawing.Point(3, 3);
            this.dsControl1.Name = "dsControl1";
            this.dsControl1.Orientation = 0D;
            this.dsControl1.PowerTransformExponent = double.NaN;
            this.dsControl1.ResponseVarColIndex = 1;
            this.dsControl1.ResponseVarColName = "";
            this.dsControl1.SelectColName = "";
            this.dsControl1.SelectedColIndex = -1;
            this.dsControl1.Size = new System.Drawing.Size(895, 415);
            this.dsControl1.State = VBCommon.Controls.DatasheetControl.dtState.dirty;
            this.dsControl1.TabIndex = 0;
            // 
            // tpMLRModel
            // 
            this.tpMLRModel.Controls.Add(this.frmModel1);
            this.tpMLRModel.Location = new System.Drawing.Point(4, 22);
            this.tpMLRModel.Name = "tpMLRModel";
            this.tpMLRModel.Padding = new System.Windows.Forms.Padding(3);
            this.tpMLRModel.Size = new System.Drawing.Size(901, 421);
            this.tpMLRModel.TabIndex = 1;
            this.tpMLRModel.Text = "Model";
            this.tpMLRModel.UseVisualStyleBackColor = true;
            // 
            // frmModel1
            // 
            this.frmModel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmModel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.frmModel1.Location = new System.Drawing.Point(3, 3);
            this.frmModel1.Name = "frmModel1";
            this.frmModel1.Size = new System.Drawing.Size(895, 415);
            this.frmModel1.TabIndex = 0;
            // 
            // ctlMLRPlugin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "ctlMLRPlugin";
            this.Size = new System.Drawing.Size(909, 447);
            this.tabControl1.ResumeLayout(false);
            this.tpMLRDatasheet.ResumeLayout(false);
            this.tpMLRModel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpMLRDatasheet;
        private VBCommon.Controls.DatasheetControl dsControl1;
        private System.Windows.Forms.TabPage tpMLRModel;
        private GALibForm.frmModel frmModel1;
    }
}