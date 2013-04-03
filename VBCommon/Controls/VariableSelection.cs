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
        public event EventHandler VariablesChanged;


        public VariableSelection()
        {
            InitializeComponent();
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
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                bool bDependentVariableColumn = false;

                if (dt.Columns[i].ExtendedProperties.ContainsKey(VBCommon.Globals.DEPENDENTVAR))
                {
                    if (dt.Columns[i].ExtendedProperties[VBCommon.Globals.DEPENDENTVAR].ToString() == "True")
                    {
                        bDependentVariableColumn = true;
                        lblDepVarName.Text = dt.Columns[i].ColumnName;
                    }
                }

                if (!bDependentVariableColumn && dt.Columns[i].ExtendedProperties.ContainsKey(VBCommon.Globals.ENABLED))
                {
                    if (dt.Columns[i].ExtendedProperties[VBCommon.Globals.ENABLED].ToString() == "True")
                        fieldList.Add(dt.Columns[i].ColumnName);
                }
                else if (!bDependentVariableColumn)
                    fieldList.Add(dt.Columns[i].ColumnName);
            }

            for (int i=0;i<fieldList.Count;i++)
                lbAvailableVariables.Items.Add(new ListItem(fieldList[i], i.ToString()));        
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
            
            lblNumAvailVars.Text = "(" + lbAvailableVariables.Items.Count.ToString() + ")";
            lblNumIndVars.Text = "(" + lbIndVariables.Items.Count.ToString() + ")";

            if (VariablesChanged != null)
                VariablesChanged(this, null);
        }


        public void AddToList(List<string> varlist)
        {
            List<ListItem> items = new List<ListItem>();

            for (int i = 0; i < lbAvailableVariables.Items.Count; i++)
            {
                ListItem li = (ListItem)lbAvailableVariables.Items[i];
                if (varlist.Contains<string>(li.DisplayItem))
                {
                    items.Add(li);
                }
            }

            foreach (ListItem li in items)
            {
                lbAvailableVariables.Items.Remove(li);
                lbIndVariables.Items.Add(li);
            }

            lblNumAvailVars.Text = "(" + lbAvailableVariables.Items.Count.ToString() + ")";
            lblNumIndVars.Text = "(" + lbIndVariables.Items.Count.ToString() + ")";

            if (VariablesChanged != null)
                VariablesChanged(this, null);
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

            lblNumAvailVars.Text = "(" + lbAvailableVariables.Items.Count.ToString() + ")";
            lblNumIndVars.Text = "(" + lbIndVariables.Items.Count.ToString() + ")";

            if (VariablesChanged != null)
                VariablesChanged(this, e);  
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
            
            lblNumAvailVars.Text = "(" + lbAvailableVariables.Items.Count.ToString() + ")";
            lblNumIndVars.Text = "(" + lbIndVariables.Items.Count.ToString() + ")";

            if (VariablesChanged != null)
                VariablesChanged(this, e);
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


        public Dictionary<string, List<ListItem>> PackState()
        {
            Dictionary<string, List<ListItem>> dctState = new Dictionary<string, List<ListItem>>();
            List<ListItem> listAvail = new List<ListItem>();
            List<ListItem> listInd = new List<ListItem>(); ;

            for (int i = 0; i < lbAvailableVariables.Items.Count; i++)
                listAvail.Add(lbAvailableVariables.Items[i] as ListItem);

            for (int i = 0; i < lbIndVariables.Items.Count; i++)
                listInd.Add(lbIndVariables.Items[i] as ListItem);

            dctState.Add("AvailableVariables", listAvail);
            dctState.Add("IndependentVariables", listInd);

            return dctState;
        }


        public void UnpackState(Dictionary<string, List<ListItem>> state)
        {
            List<ListItem> list = null;
            if (state.Keys.Contains("AvailableVariables"))
            {
                list = state["AvailableVariables"];
                for (int i = 0; i < list.Count; i++)
                    lbAvailableVariables.Items.Add(list[i]);
            }

            list = null;

            if (state.Keys.Contains("IndependentVariables"))
            {
                list = state["IndependentVariables"];
                for (int i = 0; i < list.Count; i++)
                    lbIndVariables.Items.Add(list[i]);
            }
        }
    }
}
