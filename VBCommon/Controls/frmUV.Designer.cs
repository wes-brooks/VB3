namespace VBCommon.Controls
{
    partial class frmUV
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cbCurrentDirectionColumn = new System.Windows.Forms.ComboBox();
            this.cbCurrentSpeedColumn = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbWindDirectionColumn = new System.Windows.Forms.ComboBox();
            this.cbWindSpeedColumn = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtRotationAngle = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.cbWaveDirection = new System.Windows.Forms.ComboBox();
            this.cbWaveHeight = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.cbCurrentDirectionColumn);
            this.groupBox2.Controls.Add(this.cbCurrentSpeedColumn);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Location = new System.Drawing.Point(12, 165);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(310, 147);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Current Data";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 100);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Direction (deg)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(56, 62);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Speed";
            // 
            // cbCurrentDirectionColumn
            // 
            this.cbCurrentDirectionColumn.FormattingEnabled = true;
            this.cbCurrentDirectionColumn.Location = new System.Drawing.Point(100, 97);
            this.cbCurrentDirectionColumn.Name = "cbCurrentDirectionColumn";
            this.cbCurrentDirectionColumn.Size = new System.Drawing.Size(187, 21);
            this.cbCurrentDirectionColumn.TabIndex = 4;
            // 
            // cbCurrentSpeedColumn
            // 
            this.cbCurrentSpeedColumn.FormattingEnabled = true;
            this.cbCurrentSpeedColumn.Location = new System.Drawing.Point(100, 54);
            this.cbCurrentSpeedColumn.Name = "cbCurrentSpeedColumn";
            this.cbCurrentSpeedColumn.Size = new System.Drawing.Size(187, 21);
            this.cbCurrentSpeedColumn.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 27);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(147, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Specify current data columns:";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(168, 508);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(74, 23);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(248, 508);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(74, 23);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cbWindDirectionColumn);
            this.groupBox1.Controls.Add(this.cbWindSpeedColumn);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(310, 147);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Wind Data";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Direction (deg)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(56, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Speed";
            // 
            // cbWindDirectionColumn
            // 
            this.cbWindDirectionColumn.FormattingEnabled = true;
            this.cbWindDirectionColumn.Location = new System.Drawing.Point(100, 97);
            this.cbWindDirectionColumn.Name = "cbWindDirectionColumn";
            this.cbWindDirectionColumn.Size = new System.Drawing.Size(187, 21);
            this.cbWindDirectionColumn.TabIndex = 2;
            // 
            // cbWindSpeedColumn
            // 
            this.cbWindSpeedColumn.FormattingEnabled = true;
            this.cbWindSpeedColumn.Location = new System.Drawing.Point(100, 54);
            this.cbWindSpeedColumn.Name = "cbWindSpeedColumn";
            this.cbWindSpeedColumn.Size = new System.Drawing.Size(187, 21);
            this.cbWindSpeedColumn.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Specify wind data columns:";
            // 
            // txtRotationAngle
            // 
            this.txtRotationAngle.Location = new System.Drawing.Point(179, 471);
            this.txtRotationAngle.Name = "txtRotationAngle";
            this.txtRotationAngle.Size = new System.Drawing.Size(101, 20);
            this.txtRotationAngle.TabIndex = 12;
            this.txtRotationAngle.Validating += new System.ComponentModel.CancelEventHandler(this.txtRotationAngle_Validating);
            this.txtRotationAngle.Validated += new System.EventHandler(this.txtRotationAngle_Validated);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(75, 474);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(98, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Beach Angle (deg):";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.cbWaveDirection);
            this.groupBox3.Controls.Add(this.cbWaveHeight);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Location = new System.Drawing.Point(12, 318);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(310, 147);
            this.groupBox3.TabIndex = 16;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Wave Data";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(18, 100);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(76, 13);
            this.label8.TabIndex = 4;
            this.label8.Text = "Direction (deg)";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(56, 62);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(38, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "Height";
            // 
            // cbWaveDirection
            // 
            this.cbWaveDirection.FormattingEnabled = true;
            this.cbWaveDirection.Location = new System.Drawing.Point(100, 97);
            this.cbWaveDirection.Name = "cbWaveDirection";
            this.cbWaveDirection.Size = new System.Drawing.Size(187, 21);
            this.cbWaveDirection.TabIndex = 4;
            // 
            // cbWaveHeight
            // 
            this.cbWaveHeight.FormattingEnabled = true;
            this.cbWaveHeight.Location = new System.Drawing.Point(100, 54);
            this.cbWaveHeight.Name = "cbWaveHeight";
            this.cbWaveHeight.Size = new System.Drawing.Size(187, 21);
            this.cbWaveHeight.TabIndex = 3;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(10, 27);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(140, 13);
            this.label10.TabIndex = 0;
            this.label10.Text = "Specify wave data columns:";
            // 
            // frmUV
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(335, 543);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtRotationAngle);
            this.Controls.Add(this.label5);
            this.Name = "frmUV";
            this.Text = "Wind/Current Components";
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.frmUV_HelpRequested);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbCurrentDirectionColumn;
        private System.Windows.Forms.ComboBox cbCurrentSpeedColumn;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbWindDirectionColumn;
        private System.Windows.Forms.ComboBox cbWindSpeedColumn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtRotationAngle;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cbWaveDirection;
        private System.Windows.Forms.ComboBox cbWaveHeight;
        private System.Windows.Forms.Label label10;
    }
}