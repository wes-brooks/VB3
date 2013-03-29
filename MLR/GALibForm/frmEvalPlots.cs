using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GALib;
using MultipleLinearRegression;
using ZedGraph;
using MLRCore;

namespace GALibForm
{
    public partial class frmEvalPlots : Form
    {
        private ZedGraph.MasterPane _master = null;
        private List<GALib.IIndividual> _mlist = null;
        private DataTable _dt = null;

        public frmEvalPlots(List<GALib.IIndividual> list, DataTable dt)
        {
            InitializeComponent();
            _mlist = list;
            _dt = dt;
        }

        private void renderPlots()
        {

            _master = zgc1.MasterPane;
            _master.PaneList.Clear();

            _master.Title.IsVisible = false;
            _master.Margin.All = 10;

            //add the plots here

            List<double> R2 = new List<double>();
            List<double> adjR2 = new List<double>();
            List<double> AIC = new List<double>();
            List<double> AICC = new List<double>();
            List<double> BIC = new List<double>();
            List<double> Press = new List<double>();
            List<double> RMSE = new List<double>();
            List<double> Sensitivity = new List<double>();
            List<double> Specificity = new List<double>();
            List<double> Accuracy = new List<double>();

            List<double> models = new List<double>() {1,2,3,4,5,6,7,8,9,10};
            List<string> modeltags = new List<string>();
            for (int ndx = 0; ndx < _mlist.Count; ndx++)
            {
                MLRIndividual model = (MLRIndividual)frmModelingReport._mlist[ndx];
                R2.Add(model.R2);
                adjR2.Add(model.AdjustedR2);
                AIC.Add(model.AIC);
                AICC.Add(model.AICC);
                BIC.Add(model.BIC);
                Press.Add(model.Press);
                RMSE.Add(model.RMSE);
                Sensitivity.Add(model.Sensitivity);
                Specificity.Add(model.Specificity);
                Accuracy.Add(model.Accuracy);
                modeltags.Add(ModelExprTag(model));
            }
            addPane ("R2", R2, models, modeltags);
            addPane ("Adjusted R2", adjR2, models, modeltags);
            addPane ("AIC", AIC, models, modeltags);
            addPane ("Corrected AIC", AICC, models, modeltags);
            addPane ("BIC", BIC, models, modeltags);
            addPane ("PRESS", Press, models, modeltags);
            addPane("RMSE", RMSE, models, modeltags);
            if (frmModel.ThresholdChecked)
            {
                addPane("Sensitivity", Sensitivity, models, modeltags);
                addPane("Specificity", Specificity, models, modeltags);
                addPane("Accuracy", Accuracy, models, modeltags);
            }

            zgc1.AxisChange();
            using (Graphics g = this.CreateGraphics())
            {
                _master.SetLayout(g, PaneLayout.SquareColPreferred);
            }
            //zgc1.Refresh();

        }

        private void addPane(string p, List<double> scores, List<double> models, List<string> tags)
        {
            //throw new NotImplementedException();
            string label = p;
            PointPairList ppl = new PointPairList();

            //no way to add a tag with this construct
            //ppl.Add(models.ToArray(), scores.ToArray());

            for (int li = 0; li < models.Count; li++)
            {
                ppl.Add (models[li], scores[li], tags[li]);
            }

            GraphPane pane = new GraphPane();
            BarItem bar = pane.AddBar(label, ppl, Color.Black);
            setScale(pane, scores);
            pane.YAxis.Title.Text = "Model Evaluation Metric Score";
            pane.XAxis.Title.Text = "Best-Fit Model Number";
            _master.Add(pane);

        }

        private void setScale(GraphPane pane, List<double> scores)
        {
            //throw new NotImplementedException();
            double inc = (scores.Max() - scores.Min()) / 10.0;
            double max = scores.Max() + inc;
            double min = scores.Min() - inc;
            if (max == min) min = max / 2.0;
            pane.YAxis.Scale.Max = max;
            pane.YAxis.Scale.Min = min;
            //assumes there are alway 10 models in the list
            pane.XAxis.Scale.Min = 0;
            pane.XAxis.Scale.Max = 11;
        }

        private void frmEvalPlots_Load(object sender, EventArgs e)
        {
            renderPlots();
        }

        private void zgc1_MouseMove(object sender, MouseEventArgs e)
        {
            //inform user which plot mouse is over

        }

        private bool zgc1_MouseMoveEvent(ZedGraphControl sender, MouseEventArgs e)
        {
            //let the user know where their mouse cursor is located

            GraphPane pane = sender.MasterPane.FindPane(e.Location);
            
            if (pane != null)
            {
                //get the pane
                string title = pane.CurveList[0].Label.Text;
                lblModel.Text = title;

                CurveItem curve = null;
                int pointno = -1;
                string modelexpr = string.Empty;

                //get the model in the pane
                pane.FindNearestPoint(e.Location, out curve, out pointno);
                if (curve != null)
                {
                    modelexpr = curve[pointno].Tag.ToString();
                    lblModelExpr.Text = modelexpr;
                }
            }
            else
            {
                lblModel.Text = string.Empty;
            }
            return default(bool);
        }

        private string ModelExprTag(MLRIndividual model)
        {
            //generates the model expression

            string tag = string.Empty;
            string opsign = string.Empty;
            string parameter = string.Empty;
            double coeff = double.NaN;

            tag = MLRDataManager.GetDataManager().ModelDependentVariable + " = ";

            foreach (DataRow r in model.Parameters.Rows)
            {
                parameter = r[0].ToString();
                coeff = (double)r[1];
                if (coeff < 0) opsign = " - ";
                else opsign = " + ";
                if (parameter == "(Intercept)")
                    tag = tag + string.Format("{0:00.####e-00}", coeff);
                else
                    tag = tag + opsign + string.Format("{0:00.####e-00}", Math.Abs(coeff)) +
                        "*" + parameter.ToString();
            }
            return tag;
        }

    }
}
