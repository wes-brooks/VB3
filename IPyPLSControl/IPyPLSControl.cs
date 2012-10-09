using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using IPyCommon;
using IPyModeling;

namespace IPyPLSControl
{
    //pls method class, inherits modeling control
    public partial class IPyPLSControl : IPyModelingControl
    {
        public IPyPLSControl() : base()
        {   
            InitializeComponent();
            strMethod = "PLS";
        }
        

        //Clear the control
        public new void Clear()
        {
            lvModel.Items.Clear();
            lblDecisionThreshold.Text = "";
            lblNcomp.Text = "";

            base.ClearModelingTab();
        }


        //Enable or disable controls, then raise an event to do the same up the chain in the containing Form.
        private new void ChangeControlStatus(bool enable)
        {
            base.ChangeControlStatus(enable);
        }


		protected override void PopulateResults(dynamic model)
        {
            Cursor.Current = Cursors.WaitCursor;

            //Declare the variables we'll use in this routine
            ListViewItem lvi;
            string[] strArrItem;

            //Extract the variables and their coefficients from the model object
            List<string> lstNames = ((IList<object>)model.Extract("names")).Cast<string>().ToList();
            List<double> lstCoefficients = ((IList<object>)model.Extract("coef")).Cast<double>().ToList();

            //Run the Get_Influence method to get the relative influence (coefficient x standard dev.) of each variable
            dynamic dictInfluence = model.GetInfluence();
            List<string> lstKeys = ((IList<object>)dictInfluence.keys()).Cast<string>().ToList();
            List<double> lstInfluence = ((IList<object>)dictInfluence.values()).Cast<double>().ToList();

            //Clear the old list of model coefficients
            lvModel.Items.Clear();
            Dictionary<ListViewItem, double> dictUnorderedEntries = new Dictionary<ListViewItem, double>();

			//Make a list of each model parameter, its coefficient, and its influence.
            for(int i=0; i<lstNames.Count; i++)
            {
                int intIndex = lstKeys.FindIndex(arg => String.Compare(arg, lstNames[i], CultureInfo.InvariantCulture, CompareOptions.IgnoreSymbols) == 0);
                bool boolMinor = false;
                double dblInfluence;

                //populate a row with the name, coefficient, and influence of this variable.
                strArrItem = new string[4];
                
                //index is -1 if the header wasn't found (e.g. Intercept)
                if (intIndex > -1)
                {
                    dblInfluence = lstInfluence[intIndex];
                    strArrItem[0] = lstKeys[intIndex];
                    strArrItem[1] = String.Format("{0:F4}", lstCoefficients[i]);
                    strArrItem[2] = String.Format("{0:F4}", dblInfluence);
                    if (dblInfluence <= 0.05) boolMinor = true;
                }
                else
                {
                    //Set the influence for the Intercept to 2 (this won't be displayed).
                    //The others all sum to 1 so the Intercept is guaranteed to come first in the list.
                    strArrItem[0] = lstNames[i];
                    strArrItem[1] = String.Format("{0:F4}", lstCoefficients[i]);
                    strArrItem[2] = "";
                    dblInfluence = 2;
                }

                //Create a new ListViewItem, coloring it red if this variable is considered to have minor influence.
                lvi = new ListViewItem(strArrItem);
                if (boolMinor == true)
                    lvi.ForeColor = Color.Red;

                //Add the ListViewItem to the Dictionary of entries, keyed by the influence.
                dictUnorderedEntries.Add(lvi, dblInfluence);
            }

            //Order by the negative of influence because default sort order is ascending and we want descending.
            foreach (KeyValuePair<ListViewItem, double> pair in dictUnorderedEntries.OrderBy(entry => -entry.Value))
            {
                ListViewItem entry = pair.Key;
                lvModel.Items.Add(entry);
            }

            //Now post the decision threshold and the number of PLS components
            lblDecisionThreshold.Text = String.Format("{0:F3}", UntransformThreshold(model.threshold));
            lblNcomp.Text = model.ncomp.ToString();
        }
    }
}
