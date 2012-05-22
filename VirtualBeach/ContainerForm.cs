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
            //don't need the map and legend
            //appManager.Map = new Map();
            //appManager.Legend = new Legend();

            
            //appManager.SatisfyImportsExtensionsActivated +=
            //    delegate(object sender, EventArgs e)
            //    {
            //        // we use this event to ensure that legend and map dockable panels are
            //        // added to the DockManager before any other dockable panels.
            //        //appManager.HeaderControl.Add(new DotSpatial.Controls.Header.HeaderItem());
            //};
            
            //Set the main application window to be the "Shell" 
            Shell = this;
            appManager.LoadExtensions();

            //Hook up the event handler that will 
            //ProjectSaved += new ContainerForm.ProjectSavedHandler<PackEventArgs>(ProjectSavedListener);
            
        }

        public AppManager appManager { get; set; } 
    }
}
