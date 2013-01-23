using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VBCommon.Controls
{
    public partial class VariableSelection : UserControl
    {
        public event EventHandler SelectionChanged;

        //public ListBox AvailableVariables = null;
        //public ListBox IndependentVariables = null;

        public VariableSelection()
        {
            InitializeComponent();

            //AvailableVariables = lbAvailableVariables;
            //IndependentVariables = lbIndVariables;
        }

        /// <summary>
        /// Set the data for variable selection
        /// </summary>
        /// <param name="dt"></param>
        public void SetData(DataTable dt)
        {
            if (dt == null || dt.Columns.Count < 2)
                return;

            lbAvailableVariables.Items.Clear();
            lbIndVariables.Items.Clear();

            List<string> fieldList = new List<string>();
            //Remember, first column is Date or ID and second is the response variable.
            for (int i = 2; i < dt.Columns.Count; i++)
            {
                if (dt.Columns[i].ExtendedProperties.ContainsKey(VBCommon.Globals.ENABLED))
                {
                    if (dt.Columns[i].ExtendedProperties[VBCommon.Globals.ENABLED].ToString() == "True")
                        fieldList.Add(dt.Columns[i].ColumnName);
                }
                else
                    fieldList.Add(dt.Columns[i].ColumnName);
            }

            for (int i=0;i<fieldList.Count;i++)
                lbAvailableVariables.Items.Add(new ListItem(fieldList[i], i.ToString()));
            
            lblDepVarName.Text = dt.Columns[1].ColumnName;
            
        }

        public void ClearAll()
        {
            List<ListItem> items = new List<ListItem>();

            for (int i = 0; i < lbIndVariables.Items.Count; i++)
            {
                ListItem li = (ListItem)lbIndVariables.Items[i];
                items.Add(li);
            }

            foreach (ListItem li in items)
            {
                lbIndVariables.Items.Remove(li);

                bool foundIdx = false;
                int j = 0;
                for (j = 0; j < lbAvailableVariables.Items.Count; j++)
                {
                    ListItem li2 = (ListItem)lbAvailableVariables.Items[j];
                    if (Convert.ToInt32(li2.ValueItem) > Convert.ToInt32(li.ValueItem))
                    {
                        lbAvailableVariables.Items.Insert(j, li);
                        foundIdx = true;
                        break;
                    }
                }
                if (foundIdx == false)
                    lbAvailableVariables.Items.Insert(j, li);

            }


            //SetCombinations();

            lblNumAvailVars.Text = "(" + lbAvailableVariables.Items.Count.ToString() + ")";
            lblNumIndVars.Text = "(" + lbIndVariables.Items.Count.ToString() + ")";

            //if (SelectionChanged != null)
            //    SelectionChanged(this, null);
        }

        public void Add2List(List<string> varlist)
        {
            List<ListItem> items = new List<ListItem>();

            //int selectedIndices = lbAvailableVariables.SelectedIndices.Count;
            for (int i = 0; i < lbAvailableVariables.Items.Count; i++)
            {
                ListItem li = (ListItem)lbAvailableVariables.Items[i];
                if (varlist.Contains<string>(li.DisplayItem))
                {
                    //ListItem li1 = (ListItem)lbAvailableVariables.Items[i];
                    items.Add(li);
                }
            }

            foreach (ListItem li in items)
            {
                lbAvailableVariables.Items.Remove(li);
                lbIndVariables.Items.Add(li);
            }


            //SetCombinations();

            lblNumAvailVars.Text = "(" + lbAvailableVariables.Items.Count.ToString() + ")";
            lblNumIndVars.Text = "(" + lbIndVariables.Items.Count.ToString() + ")";

            if (SelectionChanged != null)
                SelectionChanged(this, null);
        }

        private void btnAddInputVariable_Click(object sender, EventArgs e)
        {
            List<ListItem> items = new List<ListItem>();

            int selectedIndices = lbAvailableVariables.SelectedIndices.Count;
            for (int i = 0; i < selectedIndices; i++)
            {
                ListItem li = (ListItem)lbAvailableVariables.Items[lbAvailableVariables.SelectedIndices[i]];
                items.Add(li);
            }

            foreach (ListItem li in items)
            {
                lbAvailableVariables.Items.Remove(li);
                lbIndVariables.Items.Add(li);
            }


            //SetCombinations();

            lblNumAvailVars.Text = "(" + lbAvailableVariables.Items.Count.ToString() + ")";
            lblNumIndVars.Text = "(" + lbIndVariables.Items.Count.ToString() + ")";

            if (SelectionChanged != null)
                SelectionChanged(this, e);
            //_IndepVarCount = lbIndVariables.Items.Count;

            //_state = _mlrState.dirty;
            //listBox1.Items.Clear();
            //initControls();
        
        }

        private void btnRemoveInputVariable_Click(object sender, EventArgs e)
        {
            List<ListItem> items = new List<ListItem>();

            for (int i = 0; i < lbIndVariables.SelectedIndices.Count; i++)
            {
                ListItem li = (ListItem)lbIndVariables.Items[lbIndVariables.SelectedIndices[i]];
                items.Add(li);
            }

            foreach (ListItem li in items)
            {
                lbIndVariables.Items.Remove(li);

                bool foundIdx = false;
                int j = 0;
                for (j = 0; j < lbAvailableVariables.Items.Count; j++)
                {
                    ListItem li2 = (ListItem)lbAvailableVariables.Items[j];
                    if (Convert.ToInt32(li2.ValueItem) > Convert.ToInt32(li.ValueItem))
                    {
                        lbAvailableVariables.Items.Insert(j, li);
                        foundIdx = true;
                        break;
                    }
                }
                if (foundIdx == false)
                    lbAvailableVariables.Items.Insert(j, li);

            }


            //SetCombinations();

            lblNumAvailVars.Text = "(" + lbAvailableVariables.Items.Count.ToString() + ")";
            lblNumIndVars.Text = "(" + lbIndVariables.Items.Count.ToString() + ")";

            if (SelectionChanged != null)
                SelectionChanged(this, e);

            //_state = _mlrState.dirty;
            //listBox1.Items.Clear();
            //initControls();
        }

        public List<string> SelectedVariables
        {
            get 
            {
                List<string> list = new List<string>();
                for (int i = 0; i < lbIndVariables.Items.Count; i++)
                {
                    ListItem li = lbIndVariables.Items[i] as ListItem;
                    list.Add(li.DisplayItem);
                }
                return list;
            }
        }

        private void groupBox1_Resize(object sender, EventArgs e)
        {
            Control control = (Control)sender;

            int width = control.Width;

            if (width <= control.MinimumSize.Width)
                return;

            lbAvailableVariables.Width = (width - 100) / 2;

            int xLoc = lbAvailableVariables.Location.X + lbAvailableVariables.Width + 50;
            lbIndVariables.Location = new Point(xLoc, lbAvailableVariables.Location.Y);
            lbIndVariables.Width = (width - 100) / 2;

            xLoc = lbAvailableVariables.Location.X + lbAvailableVariables.Width + 10;
            btnAddInputVariable.Location = new Point(xLoc, btnAddInputVariable.Location.Y);
            btnRemoveInputVariable.Location = new Point(xLoc, btnRemoveInputVariable.Location.Y);
        }


    }
}
