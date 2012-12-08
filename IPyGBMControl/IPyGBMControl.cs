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


namespace IPyGBMControl
{
    //gbm method class, inherits modeling control
    public partial class IPyGBMControl : IPyModelingControl
    {
        public IPyGBMControl() : base()
        {
            InitializeComponent();
            strMethod = "GBM";
        }
        

        //Clear the control
        public new void Clear()
        {
            lvModel.Items.Clear();
            lblDecisionThreshold.Text = "";
            base.ClearModelingTab();
        }


        //Enable or disable controls, then raise an event to do the same up the chain in the containing Form.
        private new void ChangeControlStatus(bool enable)
        {
            base.ChangeControlStatus(enable);
        }


        protected override void PopulateResults(dynamic model)
        {
            //Declare the variables we'll use in this routine
            ListViewItem lvi;
            string[] strArrItem;

            //Run the Get_Influence method to get the relative influence (coefficient x standard dev.) of each variable
            dynamic dictInfluence = model.GetInfluence();
            List<string> listKeys = ((IList<object>)dictInfluence.keys()).Cast<string>().ToList();
            List<double> listInfluence = ((IList<object>)dictInfluence.values()).Cast<double>().ToList();

            //Clear the old list of model coefficients
            lvModel.Items.Clear();
            Dictionary<ListViewItem, double> dictUnorderedEntries = new Dictionary<ListViewItem, double>();

            //Make a list of each model parameter, its coefficient, and its influence.
            for (int i = 0; i < listKeys.Count; i++)
            {
                int intIndex = i; 
                bool boolMinor = false;
                double dblInfluence = 2;

                //populate a row with the name, coefficient, and influence of this variable.
                strArrItem = new string[4];

                //index is -1 if the header wasn't found (e.g. Intercept)
                if (intIndex > -1)
                {
                    dblInfluence = listInfluence[intIndex];
                    strArrItem[0] = listKeys[intIndex];
                    strArrItem[1] = "na";
                    strArrItem[2] = String.Format("{0:F4}", dblInfluence);
                    if (dblInfluence <= 0.05) boolMinor = true;
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
            lblDecisionThreshold.Text = String.Format("{0:F3}", model.threshold); //VBCommon.Transforms.Apply.UntransformThreshold(model.threshold));
            //lblNcomp.Text = model.ncomp.ToString();
        }
	}
}
