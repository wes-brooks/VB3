using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MLRPrediction
{
    public partial class frmBadCellsRpt : Form
    {

        public frmBadCellsRpt(List<string> badCells)
        {
            InitializeComponent();
            listLists(badCells);
            this.TopMost = true;
        }

        private void listLists(List<string> badCells)
        {
            if (badCells.Count > 0)
            {
                foreach (string cell in badCells)
                {
                    rtb.Text += cell + "\n";
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
