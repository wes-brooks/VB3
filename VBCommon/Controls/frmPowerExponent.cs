using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VBCommon;

namespace VBCommon.Controls
{
    public partial class frmPowerExponent : Form
    {
        private double dblExp = double.NaN;
        private DataTable _dt = null;
        private int intCndx = -1;
        private double[] dblArrV = null;
        private string strTmessage = string.Empty;


        /// <summary>
        /// class constructor needs the datatable and the index of the column
        /// in the table to operate on
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="cndx"></param>
        public frmPowerExponent(DataTable dt, int cndx)
        {
            InitializeComponent();
            intCndx = cndx;
            _dt = dt;
        }


        /// <summary>
        /// accessor to return a array of transformed values
        /// </summary>
        public double[] TransformedValues
        {
            set { dblArrV = value; }
            get { return dblArrV; }
        }


        public string TransformMessage
        {
            set { strTmessage = value; }
            get { return strTmessage; }
        }


        /// <summary>
        /// accessor to return the exponent used in the transform
        /// </summary>
        public double Exponent
        {
            get { return dblExp; }
        }


        /// <summary>
        /// button go clicked, perform the calculation, save the data and go away
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton8.Checked && string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Must enter a value for exponent.", "Exponent Cannot be Blank", MessageBoxButtons.OK);
                return;
            }
        }


        /// <summary>
        /// button cancel clicked, go away
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }


        #region radio button maintenance for selection of one of the commonly used exponents

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked) dblExp = -1.0d;
        }


        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked) dblExp = 2.0d;
        }


        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked) dblExp = 0.5d;
        }


        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked) dblExp = 3.0d;
        }


        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked) dblExp = 1.0d / 3.0d;
        }


        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton6.Checked) dblExp = 4.0d;
        }


        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton7.Checked) dblExp = 0.25d;
        }

        #endregion


        /// <summary>
        /// user selected to enter a value for an exponent, validate it and save
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton8.Checked)
            {
                if (!string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    dblExp = Convert.ToDouble(textBox1.Text);
                }
            }
        }


        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            if (!double.TryParse(textBox1.Text, out dblExp))
            {
                e.Cancel = true;
                textBox1.Select(0, textBox1.Text.Length);
                this.errorProvider1.SetError(textBox1, "Text must convert to a number.");
            }
        }


        private void textBox1_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(textBox1, "");
        }
    }
}
