using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CodedomEvaluator;
using System.Globalization;

namespace CsGraphingCalc
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            string strResult = CodedomTest.Test();
            if (strResult != "")
                MessageBox.Show(strResult);

            //this.pointPlotBitmapUserControl1.resetZoom();
            this.pointPlotUserControl1.strExp1=getUserExpression(txtEq1.Text);
            this.pointPlotUserControl1.resetZoom();
        }

        //turn into form y = x
        protected string getUserExpression(string s)
        {
            //transform sin into Math.sin
            CodedomEvaluator.CodedomEvaluator cb = new CodedomEvaluator.CodedomEvaluator();
            s = cb.addMathMethods(s);

            //if blank, assume 0
            if (s=="" || s.Trim()=="")
                return "y=0.0;";
            // one line of expression, for example 
            if (!s.Trim().Contains("\r\n") && !s.Trim().Contains(";"))
                return "y = " + s.Trim() + ";";
             
            return s;
        }


        private void btnSetView_Click(object sender, EventArgs e)
        {
            double x0, x1, y0, y1;
            string s = InputBoxForm.GetStrInput("Enter boundaries, separated by commas:", "-10,10,-10,10");
            if (s==null || s=="") return;
            string[] ss = s.Split(new char[] { ',' });
            if (ss.Length != 4) { MessageBox.Show("Couldn't parse boundaries."); return; }
           
            if (!double.TryParse(ss[0].Replace(" ", ""), out x0)) 
                { MessageBox.Show("Couldn't parse boundaries."); return; }
            if (!double.TryParse(ss[1].Replace(" ", ""), out x1))
                { MessageBox.Show("Couldn't parse boundaries."); return; }
            if (!double.TryParse(ss[2].Replace(" ", ""), out y0))
                { MessageBox.Show("Couldn't parse boundaries."); return; }
            if (!double.TryParse(ss[3].Replace(" ", ""), out y1))
                { MessageBox.Show("Couldn't parse boundaries."); return; }

            this.pointPlotUserControl1.setZoomBounds(x0, x1, y0, y1);
            this.pointPlotUserControl1.redraw();
        }

        private void btnEvalAt_Click(object sender, EventArgs e)
        {
            double fx;
            string s = InputBoxForm.GetStrInput("Eval at x=", "1.5");
            if (s==null || s=="") return;
            if (!double.TryParse(s.Replace(" ", ""), out fx)) 
                { MessageBox.Show("Couldn't parse number."); return; }

            StringBuilder sbCode = new StringBuilder();
            sbCode.AppendLine(this.txtInit.Text);
            sbCode.AppendLine("double x="+fx.ToString(CultureInfo.InvariantCulture)+";");
            sbCode.AppendLine("double y;");
            sbCode.AppendLine(getUserExpression(this.txtEq1.Text)); //translate one line into code
            sbCode.AppendLine("ans = y;");

            string strError;
            CodedomEvaluator.CodedomEvaluator cb = new CodedomEvaluator.CodedomEvaluator();
            double fOut = cb.mathEval(sbCode.ToString(), new Dictionary<string, double>(), out strError);
            if (strError != "") { MessageBox.Show(strError); return;  }
            MessageBox.Show("y= "+fOut.ToString());
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Left-drag to zoom in, right-click to undo zoom, middle-click to reset view.\r\nExpressions can be in form \"x+4\" or \"y=x+4;\"\r\nFunctions like sin,exp,sqrt, are supported.");

        }

        private void btnPlot_Click(object sender, EventArgs e)
        {
            string s1 = getUserExpression(this.txtEq1.Text);
            string s2 = getUserExpression(this.txtEq2.Text);

            this.pointPlotUserControl1.bDrawSecond = (this.txtEq2.Text!="");
            this.pointPlotUserControl1.strExp1 = s1;
            this.pointPlotUserControl1.strExp2 = s2;
            this.pointPlotUserControl1.strInit = getUserExpression(this.txtInit.Text) ;
            this.pointPlotUserControl1.redraw();
        }


        



    }
}