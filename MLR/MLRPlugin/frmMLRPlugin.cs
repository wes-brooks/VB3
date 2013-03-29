using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VBCommon.Controls;

namespace MLRPlugin
{
    public partial class frmMLRPlugin : UserControl
    {
        public frmMLRPlugin()
        {
            InitializeComponent();
        }

        public GALibForm.frmModel ModelForm
        {
            get { return this.frmModel1; }
        }

        public DatasheetControl LocalDatasheet
        {
            get { return this.dsControl1; }
        }
    }
}
