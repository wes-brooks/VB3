using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VBCommon.Controls
{
    public partial class frmChooseWorksheet : Form
    {
        public frmChooseWorksheet(string workbook, string[] worksheetNames)
        {
            InitializeComponent();

            lbWorksheets.Items.Clear();
            string nameTemp = "";

            foreach (string name in worksheetNames)
            {
                nameTemp = name;
                if (nameTemp.StartsWith("'"))
                    nameTemp = nameTemp.Substring(1);

                if (nameTemp.EndsWith("'"))
                    nameTemp = nameTemp.Remove(nameTemp.Length - 1);

                if (nameTemp.EndsWith("$"))
                    nameTemp = nameTemp.Remove(nameTemp.Length - 1);                

                lbWorksheets.Items.Add(nameTemp);                
            }

            lblWorkbookName.Text = workbook;
        }


        public string SelectedWorksheetName
        {
            get
            {
                if (lbWorksheets.SelectedItem == null)
                    return "";

                string name = lbWorksheets.SelectedItem.ToString();
                if ((name == null) || (name == ""))
                    return "";

                if (!name.EndsWith("$"))
                    name += "$";

                return name;
            }
        }

        private void lbWorksheets_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lbWorksheets.SelectedItem == null)
                return;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
