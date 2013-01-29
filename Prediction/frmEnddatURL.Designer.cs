namespace Prediction
{
    partial class frmEnddatURL
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.txtEnddatURL = new System.Windows.Forms.TextBox();
            this.gbTimestamp = new System.Windows.Forms.GroupBox();
            this.cbTimezone = new System.Windows.Forms.ComboBox();
            this.tbTimestamp = new System.Windows.Forms.TextBox();
            this.rbMostRecent = new System.Windows.Forms.RadioButton();
            this.rbManual = new System.Windows.Forms.RadioButton();
            this.gbTimestamp.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(615, 80);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(696, 80);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // txtEnddatURL
            // 
            this.txtEnddatURL.Location = new System.Drawing.Point(12, 12);
            this.txtEnddatURL.Name = "txtEnddatURL";
            this.txtEnddatURL.Size = new System.Drawing.Size(759, 20);
            this.txtEnddatURL.TabIndex = 0;
            // 
            // gbTimestamp
            // 
            this.gbTimestamp.Controls.Add(this.cbTimezone);
            this.gbTimestamp.Controls.Add(this.tbTimestamp);
            this.gbTimestamp.Controls.Add(this.rbMostRecent);
            this.gbTimestamp.Controls.Add(this.rbManual);
            this.gbTimestamp.Location = new System.Drawing.Point(12, 38);
            this.gbTimestamp.Name = "gbTimestamp";
            this.gbTimestamp.Size = new System.Drawing.Size(283, 67);
            this.gbTimestamp.TabIndex = 3;
            this.gbTimestamp.TabStop = false;
            this.gbTimestamp.Text = "Timestamp for retrieving EnDDaT data";
            // 
            // cbTimezone
            // 
            this.cbTimezone.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbTimezone.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbTimezone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTimezone.FormattingEnabled = true;
            this.cbTimezone.Items.AddRange(new object[] {
            "Local time",
            "-0:00 (UTC/GMT)",
            "-4:00 (EDT)",
            "-5:00 (CDT/EST)",
            "-6:00 (MDT/CST)",
            "-7:00 (PDT/MST)",
            "-8:00 (ADT/PST)",
            "-9:00 (HDT/AST)",
            "-10:00 (HST)"});
            this.cbTimezone.Location = new System.Drawing.Point(117, 18);
            this.cbTimezone.Name = "cbTimezone";
            this.cbTimezone.Size = new System.Drawing.Size(116, 21);
            this.cbTimezone.TabIndex = 4;
            // 
            // tbTimestamp
            // 
            this.tbTimestamp.Location = new System.Drawing.Point(72, 18);
            this.tbTimestamp.Name = "tbTimestamp";
            this.tbTimestamp.Size = new System.Drawing.Size(39, 20);
            this.tbTimestamp.TabIndex = 2;
            this.tbTimestamp.Text = "00:00";
            this.tbTimestamp.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // rbMostRecent
            // 
            this.rbMostRecent.AutoSize = true;
            this.rbMostRecent.Checked = true;
            this.rbMostRecent.Location = new System.Drawing.Point(6, 42);
            this.rbMostRecent.Name = "rbMostRecent";
            this.rbMostRecent.Size = new System.Drawing.Size(133, 17);
            this.rbMostRecent.TabIndex = 1;
            this.rbMostRecent.TabStop = true;
            this.rbMostRecent.Text = "Most recently available";
            this.rbMostRecent.UseVisualStyleBackColor = true;
            // 
            // rbManual
            // 
            this.rbManual.AutoSize = true;
            this.rbManual.Location = new System.Drawing.Point(6, 19);
            this.rbManual.Name = "rbManual";
            this.rbManual.Size = new System.Drawing.Size(60, 17);
            this.rbManual.TabIndex = 0;
            this.rbManual.TabStop = true;
            this.rbManual.Text = "Daily at";
            this.rbManual.UseVisualStyleBackColor = true;
            // 
            // frmEnddatURL
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(783, 110);
            this.Controls.Add(this.gbTimestamp);
            this.Controls.Add(this.txtEnddatURL);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Name = "frmEnddatURL";
            this.Text = "Set EnDDaT URL";
            this.Load += new System.EventHandler(this.frmEnddatURL_Load);
            this.gbTimestamp.ResumeLayout(false);
            this.gbTimestamp.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox txtEnddatURL;
        private System.Windows.Forms.GroupBox gbTimestamp;
        private System.Windows.Forms.TextBox tbTimestamp;
        private System.Windows.Forms.RadioButton rbMostRecent;
        private System.Windows.Forms.RadioButton rbManual;
        private System.Windows.Forms.ComboBox cbTimezone;
    }
}