namespace VBCommon.Controls
{
    partial class VariableSelection
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblNumAvailVars = new System.Windows.Forms.Label();
            this.lblNumIndVars = new System.Windows.Forms.Label();
            this.btnRemoveInputVariable = new System.Windows.Forms.Button();
            this.btnAddInputVariable = new System.Windows.Forms.Button();
            this.lblDepVarName = new System.Windows.Forms.Label();
            this.lblDepVariable = new System.Windows.Forms.Label();
            this.lblIndVars = new System.Windows.Forms.Label();
            this.lbIndVariables = new System.Windows.Forms.ListBox();
            this.lblAvailVars = new System.Windows.Forms.Label();
            this.lbAvailableVariables = new System.Windows.Forms.ListBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.lblNumAvailVars);
            this.groupBox1.Controls.Add(this.lblNumIndVars);
            this.groupBox1.Controls.Add(this.btnRemoveInputVariable);
            this.groupBox1.Controls.Add(this.btnAddInputVariable);
            this.groupBox1.Controls.Add(this.lblDepVarName);
            this.groupBox1.Controls.Add(this.lblDepVariable);
            this.groupBox1.Controls.Add(this.lblIndVars);
            this.groupBox1.Controls.Add(this.lbIndVariables);
            this.groupBox1.Controls.Add(this.lblAvailVars);
            this.groupBox1.Controls.Add(this.lbAvailableVariables);
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.MinimumSize = new System.Drawing.Size(260, 400);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(260, 412);
            this.groupBox1.TabIndex = 84;
            this.groupBox1.TabStop = false;
            this.groupBox1.Resize += new System.EventHandler(this.groupBox1_Resize);
            // 
            // lblNumAvailVars
            // 
            this.lblNumAvailVars.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNumAvailVars.AutoSize = true;
            this.lblNumAvailVars.Location = new System.Drawing.Point(108, 34);
            this.lblNumAvailVars.Name = "lblNumAvailVars";
            this.lblNumAvailVars.Size = new System.Drawing.Size(19, 13);
            this.lblNumAvailVars.TabIndex = 86;
            this.lblNumAvailVars.Text = "    ";
            // 
            // lblNumIndVars
            // 
            this.lblNumIndVars.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNumIndVars.AutoSize = true;
            this.lblNumIndVars.Location = new System.Drawing.Point(228, 34);
            this.lblNumIndVars.Name = "lblNumIndVars";
            this.lblNumIndVars.Size = new System.Drawing.Size(22, 13);
            this.lblNumIndVars.TabIndex = 93;
            this.lblNumIndVars.Text = "     ";
            // 
            // btnRemoveInputVariable
            // 
            this.btnRemoveInputVariable.Location = new System.Drawing.Point(122, 137);
            this.btnRemoveInputVariable.Name = "btnRemoveInputVariable";
            this.btnRemoveInputVariable.Size = new System.Drawing.Size(14, 22);
            this.btnRemoveInputVariable.TabIndex = 92;
            this.btnRemoveInputVariable.Text = "<";
            this.btnRemoveInputVariable.Click += new System.EventHandler(this.btnRemoveInputVariable_Click);
            // 
            // btnAddInputVariable
            // 
            this.btnAddInputVariable.Location = new System.Drawing.Point(122, 110);
            this.btnAddInputVariable.Name = "btnAddInputVariable";
            this.btnAddInputVariable.Size = new System.Drawing.Size(14, 21);
            this.btnAddInputVariable.TabIndex = 91;
            this.btnAddInputVariable.Text = ">";
            this.btnAddInputVariable.Click += new System.EventHandler(this.btnAddInputVariable_Click);
            // 
            // lblDepVarName
            // 
            this.lblDepVarName.AutoSize = true;
            this.lblDepVarName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDepVarName.Location = new System.Drawing.Point(110, 16);
            this.lblDepVarName.Name = "lblDepVarName";
            this.lblDepVarName.Size = new System.Drawing.Size(80, 13);
            this.lblDepVarName.TabIndex = 90;
            this.lblDepVarName.Text = "depvar name";
            // 
            // lblDepVariable
            // 
            this.lblDepVariable.AutoSize = true;
            this.lblDepVariable.Location = new System.Drawing.Point(6, 16);
            this.lblDepVariable.Name = "lblDepVariable";
            this.lblDepVariable.Size = new System.Drawing.Size(107, 13);
            this.lblDepVariable.TabIndex = 89;
            this.lblDepVariable.Text = "Dependent Variable: ";
            // 
            // lblIndVars
            // 
            this.lblIndVars.AutoSize = true;
            this.lblIndVars.Location = new System.Drawing.Point(139, 34);
            this.lblIndVars.Name = "lblIndVars";
            this.lblIndVars.Size = new System.Drawing.Size(83, 13);
            this.lblIndVars.TabIndex = 88;
            this.lblIndVars.Text = "Indep. Variables";
            // 
            // lbIndVariables
            // 
            this.lbIndVariables.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lbIndVariables.FormattingEnabled = true;
            this.lbIndVariables.HorizontalScrollbar = true;
            this.lbIndVariables.Location = new System.Drawing.Point(142, 50);
            this.lbIndVariables.MinimumSize = new System.Drawing.Size(100, 4);
            this.lbIndVariables.Name = "lbIndVariables";
            this.lbIndVariables.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbIndVariables.Size = new System.Drawing.Size(111, 355);
            this.lbIndVariables.TabIndex = 87;
            // 
            // lblAvailVars
            // 
            this.lblAvailVars.AutoSize = true;
            this.lblAvailVars.Location = new System.Drawing.Point(3, 34);
            this.lblAvailVars.Name = "lblAvailVars";
            this.lblAvailVars.Size = new System.Drawing.Size(96, 13);
            this.lblAvailVars.TabIndex = 85;
            this.lblAvailVars.Text = "Available Variables";
            // 
            // lbAvailableVariables
            // 
            this.lbAvailableVariables.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lbAvailableVariables.FormattingEnabled = true;
            this.lbAvailableVariables.HorizontalScrollbar = true;
            this.lbAvailableVariables.Location = new System.Drawing.Point(6, 50);
            this.lbAvailableVariables.MinimumSize = new System.Drawing.Size(100, 4);
            this.lbAvailableVariables.Name = "lbAvailableVariables";
            this.lbAvailableVariables.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbAvailableVariables.Size = new System.Drawing.Size(111, 355);
            this.lbAvailableVariables.TabIndex = 84;
            // 
            // VariableSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.MinimumSize = new System.Drawing.Size(260, 0);
            this.Name = "VariableSelection";
            this.Size = new System.Drawing.Size(260, 412);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblNumAvailVars;
        private System.Windows.Forms.Label lblNumIndVars;
        private System.Windows.Forms.Button btnRemoveInputVariable;
        private System.Windows.Forms.Button btnAddInputVariable;
        private System.Windows.Forms.Label lblDepVarName;
        private System.Windows.Forms.Label lblDepVariable;
        private System.Windows.Forms.Label lblIndVars;
        private System.Windows.Forms.ListBox lbIndVariables;
        private System.Windows.Forms.Label lblAvailVars;
        private System.Windows.Forms.ListBox lbAvailableVariables;

    }
}
