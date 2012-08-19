namespace VBCommon.Controls
{
    partial class frmChooseWorksheet
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
            this.label1 = new System.Windows.Forms.Label();
            this.lblWorkbookName = new System.Windows.Forms.Label();
            this.lbWorksheets = new System.Windows.Forms.ListBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Worksheets in ";
            // 
            // lblWorkbookName
            // 
            this.lblWorkbookName.AutoSize = true;
            this.lblWorkbookName.Location = new System.Drawing.Point(96, 18);
            this.lblWorkbookName.Name = "lblWorkbookName";
            this.lblWorkbookName.Size = new System.Drawing.Size(29, 13);
            this.lblWorkbookName.TabIndex = 1;
            this.lblWorkbookName.Text = "label";
            // 
            // lbWorksheets
            // 
            this.lbWorksheets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbWorksheets.FormattingEnabled = true;
            this.lbWorksheets.Location = new System.Drawing.Point(15, 34);
            this.lbWorksheets.Name = "lbWorksheets";
            this.lbWorksheets.Size = new System.Drawing.Size(253, 160);
            this.lbWorksheets.TabIndex = 2;
            this.lbWorksheets.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbWorksheets_MouseDoubleClick);
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(61, 218);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(150, 218);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // frmChooseWorksheet
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(286, 253);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lbWorksheets);
            this.Controls.Add(this.lblWorkbookName);
            this.Controls.Add(this.label1);
            this.Name = "frmChooseWorksheet";
            this.Text = "Select Excel Workbook";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblWorkbookName;
        private System.Windows.Forms.ListBox lbWorksheets;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}