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
            this.button1 = new System.Windows.Forms.Button();
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
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.cbCurrentDirectionColumn);
            this.groupBox2.Controls.Add(this.cbCurrentSpeedColumn);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Location = new System.Drawing.Point(108, 196);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(309, 147);
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
            this.btnCancel.Location = new System.Drawing.Point(287, 422);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(74, 23);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(166, 422);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(74, 23);
            this.button1.TabIndex = 13;
            this.button1.Text = "Ok";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cbWindDirectionColumn);
            this.groupBox1.Controls.Add(this.cbWindSpeedColumn);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(108, 43);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(309, 147);
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
            this.txtRotationAngle.Location = new System.Drawing.Point(274, 358);
            this.txtRotationAngle.Name = "txtRotationAngle";
            this.txtRotationAngle.Size = new System.Drawing.Size(100, 20);
            this.txtRotationAngle.TabIndex = 12;
            this.txtRotationAngle.Validating += new System.ComponentModel.CancelEventHandler(this.txtRotationAngle_Validating);
            this.txtRotationAngle.Validated += new System.EventHandler(this.txtRotationAngle_Validated);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(148, 361);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(98, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Beach Angle (deg):";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // frmUV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(545, 472);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.button1);
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
        private System.Windows.Forms.Button button1;
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
    }
}