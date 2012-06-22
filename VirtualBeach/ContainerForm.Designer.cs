using DotSpatial.Controls;

namespace VirtualBeach
{
    public partial class ContainerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ContainerForm));
            this.appManager = new DotSpatial.Controls.AppManager();
            //this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // appManager
            // 
            this.appManager.CompositionContainer = null;
            this.appManager.Directories = ((System.Collections.Generic.List<string>)(resources.GetObject("appManager.Directories")));
            this.appManager.DockManager = null;
            this.appManager.HeaderControl = null;
            //this.appManager.Legend = null;
            //this.appManager.Map = null;
            this.appManager.ProgressHandler = null;
            this.appManager.ShowExtensionsDialog = DotSpatial.Controls.ShowExtensionsDialog.Default;
            //// 
            //// propertyGrid1
            //// 
            //this.propertyGrid1.Location = new System.Drawing.Point(82, 109);
            //this.propertyGrid1.Name = "propertyGrid1";
            //this.propertyGrid1.Size = new System.Drawing.Size(8, 15);
            //this.propertyGrid1.TabIndex = 0;
            // 
            // ContainerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 368);
            //this.Controls.Add(this.propertyGrid1);
            this.Name = "ContainerForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

        }

        #endregion

        //private DotSpatial.Controls.AppManager appManager;
        //private System.Windows.Forms.PropertyGrid propertyGrid1;
    }
}

