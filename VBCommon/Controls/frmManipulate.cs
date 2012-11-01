using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VBCommon;
using VBCommon.Expressions;
//using VBTools;

namespace VBCommon.Controls
{
    public partial class frmManipulate : Form
    {

        private DataTable _newDt;
        private DataTable _dt;
        private int _depVarColIndex;

        private Expression _currentExp;


        public DataTable NewDataTable
        {
            get { return _newDt; }
        }

        public frmManipulate(DataTable dt, int depVarIndex)
        {
            InitializeComponent();
            _dt = dt;
            _depVarColIndex = depVarIndex;
        }


        //We only perform these operations/manipulations on main effect or transformed columns.
        private void frmManipulate_Load(object sender, EventArgs e)
        {
            int idx = 0;
            //Start from column 1 not 0. Column 0 is always a date or ID.
            for (int i = 1; i < _dt.Columns.Count; i++)
            {
                if (i == _depVarColIndex)
                    continue;

                bool transformed = _dt.Columns[i].ExtendedProperties.ContainsKey("transform");
                bool op = _dt.Columns[i].ExtendedProperties.ContainsKey("operation");
                bool maineffect = _dt.Columns[i].ExtendedProperties.ContainsKey("maineffect");

                if (op == true)
                    continue;

                if ((transformed == true) && (op == true))
                    continue;

                ListItem li = new ListItem(_dt.Columns[i].ColumnName, idx.ToString());
                lbAvailableVariables.Items.Add(li);
                idx++;
            }

            _currentExp = new Expression(VBCommon.Globals.Operations.SUM);

        }

        private void rbSum_CheckedChanged(object sender, EventArgs e)
        {
            string expression = _currentExp.SetOperation(VBCommon.Globals.Operations.SUM);
            txtExpression.Text = expression;
        }

        private void rbMean_CheckedChanged(object sender, EventArgs e)
        {
            string expression = _currentExp.SetOperation(VBCommon.Globals.Operations.MEAN);
            txtExpression.Text = expression;
        }

        private void rbProduct_CheckedChanged(object sender, EventArgs e)
        {
            string expression = _currentExp.SetOperation(VBCommon.Globals.Operations.PROD);
            txtExpression.Text = expression;
        }

        private void rbMax_CheckedChanged(object sender, EventArgs e)
        {
            string expression = _currentExp.SetOperation(VBCommon.Globals.Operations.MAX);
            txtExpression.Text = expression;
        }

        private void rbMin_CheckedChanged(object sender, EventArgs e)
        {
            string expression = _currentExp.SetOperation(VBCommon.Globals.Operations.MIN);
            txtExpression.Text = expression;
        }

        private void clearEditor()
        {
            txtExpression.Text = "";
        }

        private void btnAddIV_Click(object sender, EventArgs e)
        {
            List<ListItem> items = new List<ListItem>();

            int selectedIndices = lbAvailableVariables.SelectedIndices.Count;
            for (int i = 0; i < selectedIndices; i++)
            {
                ListItem li = (ListItem)lbAvailableVariables.Items[lbAvailableVariables.SelectedIndices[i]];
                items.Add(li);
                _currentExp.AddVariable(li.DisplayItem);
            }

            foreach (ListItem li in items)
            {
                lbAvailableVariables.Items.Remove(li);
                lbIndVariables.Items.Add(li);
            }

            txtExpression.Text = _currentExp.ExpressionString;
            
        }

        private void btnRemoveIV_Click(object sender, EventArgs e)
        {
            List<ListItem> items = new List<ListItem>();

            //int selectedIndices = lbIndVariables.SelectedIndices.Count;

            for (int i = 0; i < lbIndVariables.SelectedIndices.Count; i++)
            {
                ListItem li = (ListItem)lbIndVariables.Items[lbIndVariables.SelectedIndices[i]];
                items.Add(li);
                _currentExp.RemoveVariable(li.DisplayItem);
            }

            foreach (ListItem li in items)
            {
                lbIndVariables.Items.Remove(li);

                int j = 0;
                bool foundIdx = false;
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

            txtExpression.Text =  _currentExp.ToString();

        }

        private void btnAddExp_Click(object sender, EventArgs e)
        {
            if (lbIndVariables.Items.Count < 2) return;

            string exp = _currentExp.ExpressionString;

            if (lbExpressions.Items.Contains(exp))
            {
                MessageBox.Show("This expression is already included: " + exp);
                return;
            }

            Expression expCopy = new Expression(_currentExp);
            ListObject lo = new ListObject(exp, expCopy);
            lbExpressions.Items.Add(lo);

        }

        private void btnRemoveExp_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lbExpressions.SelectedIndices.Count; i++)
            {
                ListObject exp = (ListObject)lbExpressions.Items[lbExpressions.SelectedIndices[i]];
                lbExpressions.Items.Remove(exp);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            List<Expression> exps = new List<Expression>();
            for (int i = 0; i < lbExpressions.Items.Count; i++)
            {
                ListObject exp = lbExpressions.Items[i] as ListObject;
                exps.Add(exp.ValueItem as Expression);            
            }

            ExpressionExecution expExec = new ExpressionExecution(_dt, exps.ToArray());
            _newDt = expExec.Execute();
            

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnInteractions_Click(object sender, EventArgs e)
        {
            //int j = 0;
            for (int i = 0; i < lbIndVariables.Items.Count -1; i++)
            {
               // j = i + 1;
                ListItem li = (ListItem)lbIndVariables.Items[i];
                for (int j = i + 1; j < lbIndVariables.Items.Count ; j++)
                {
                    ListItem li2 = (ListItem)lbIndVariables.Items[j];
                    Expression expression = new Expression(VBCommon.Globals.Operations.PROD);
                    expression.AddVariable(li.DisplayItem);
                    string exp = expression.AddVariable(li2.DisplayItem);
                    lbExpressions.Items.Add(new ListObject(exp, expression));
                }
                
                
            }

        }

        private void frmManipulate_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            /*string apppath = Application.StartupPath.ToString();
            VBCSHelp help = new VBCSHelp(apppath, sender);
            if (!help.Status)
            {
                MessageBox.Show(
                "User documentation is found in the Documentation folder where you installed Virtual Beach"
                + "\nIf your web browser is PDF-enabled, open the User Guide with your browser.",
                "Neither Adobe Acrobat nor Adobe Reader found.",
                MessageBoxButtons.OK);
            }*/
        }

        

    }
}
