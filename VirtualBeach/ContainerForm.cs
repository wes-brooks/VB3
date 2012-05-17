using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using VBTools;

namespace VirtualBeach
{
    public partial class ContainerForm : Form
    {
        [Export("Shell", typeof(ContainerControl))]
        private static ContainerControl Shell;

        private VBProjectManager.VBProjectManager projectmanager = new VBProjectManager.VBProjectManager();

        public ContainerForm()
        {
            InitializeComponent();

            //Hook up the event handler that will 
            ProjectSaved += new ContainerForm.ProjectSavedHandler<PackEventArgs>(ProjectSavedListener);

            appManager.SatisfyImportsExtensionsActivated +=
                delegate(object sender, EventArgs e)
                {
                    //appManager.HeaderControl.Add(new DotSpatial.Controls.Header.HeaderItem());
                };
            
            //Set the main application window to be the "Shell", which is where 
            Shell = this;
            appManager.LoadExtensions();


            
        }


    }
}
