using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Prediction
{
    public partial class frmEnddatURL : Form
    {
        private string strEnddatURL = null;


        public frmEnddatURL(string CurrentURL)
        {
            if (CurrentURL != null)
            {
                strEnddatURL = CurrentURL;
            }

            InitializeComponent();
        }


        private void frmEnddatURL_Load(object sender, EventArgs e)
        {
            if (strEnddatURL != null)
            {
                txtEnddatURL.Text = strEnddatURL;
            }
        }


        public string URL
        {
            get { return strEnddatURL; }
        }


        private void btnOK_Click(object sender, EventArgs e)
        {
            strEnddatURL = txtEnddatURL.Text;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
