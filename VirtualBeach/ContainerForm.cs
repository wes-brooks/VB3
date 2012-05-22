using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;

namespace VirtualBeach
{
    public partial class ContainerForm : Form
    {
        [Export("Shell", typeof(ContainerControl))]
        private static ContainerControl Shell;

        /// <summary>
        /// Initializes a new instance of the MainForm class.
        /// </summary>
        public ContainerForm()
        {
            InitializeComponent();

            //t
            appManager = new AppManager();
            
            //Set the main application window to be the "Shell" 
            Shell = this;
            appManager.LoadExtensions();

            //Hook up the event handler that will 
            //ProjectSaved += new ContainerForm.ProjectSavedHandler<PackEventArgs>(ProjectSavedListener);
            
        }

        public AppManager appManager { get; set; } 
    }
}
