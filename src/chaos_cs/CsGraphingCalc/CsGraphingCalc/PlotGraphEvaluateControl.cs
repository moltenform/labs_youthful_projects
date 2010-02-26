using System;
using System.Collections.Generic;
using System.Text;
using chaosExplorerControl;

namespace CsGraphingCalc
{
    public class PlotGraphEvaluateControl : PointPlotGraphUserControl
    {
        protected override int getControlPaintWidth() { return 500; }
        protected override int getControlPaintHeight() { return 500; }
        public string strExp1 = "y=0.0;"; // must be valid c# assigning to y.
        public string strExp2 = "y=0.0;"; // must be valid c# assigning to y.
        public string strInit = ""; // if set, must be valid c# code

        public PlotGraphEvaluateControl()
        {
            this.setInitialBounds();
        }

        public override void getPlotPoints(int width, int height, ref double[] data1, ref double[] data2)
        {
            /*
             * Create this code:
            double x = X0, xinc=(X1-X0)/width;
            for (int i=0; i<width; i++)
            {
                dbPlotData1[i] = x;
                dbPlotData2[i] = x;
                x+=xinc;
            }*/

            StringBuilder sbCode = new StringBuilder();
            sbCode.AppendLine("double y;");
            sbCode.AppendLine("double x = X0, xinc=(X1-X0)/width;");
            sbCode.AppendLine(strInit);
            sbCode.AppendLine("for (int i=0; i<width; i++) {");
            sbCode.AppendLine("$$code$$");
            sbCode.AppendLine("arrAns[i] = y;");
            sbCode.AppendLine("x+=xinc;");
            sbCode.AppendLine("}");

            Dictionary<string, double> d = new Dictionary<string, double>();
            d["X0"] = X0; d["X1"] = X1; d["Y0"] = Y0; d["Y1"] = Y1;
            d["width"] = width; d["height"] = height;

            string strErr = "";
            CodedomEvaluator.CodedomEvaluator cde = new CodedomEvaluator.CodedomEvaluator();
            double[] out1 = cde.mathEvalArray(sbCode.ToString().Replace("$$code$$", strExp1), d, width, out strErr);
            if (strErr != "")
                { System.Windows.Forms.MessageBox.Show(strErr); return; }

            //because of ref counting, apparently ok to just set reference like this.
            //for (int i=0; i<out1.Length; i++)
            //    this.dbPlotData1[i] = out1[i];
            System.Diagnostics.Debug.Assert(this.dbPlotData1.Length == out1.Length);
            this.dbPlotData1 = out1;

            if (bDrawSecond)
            {
                cde = new CodedomEvaluator.CodedomEvaluator();
                double[] out2 = cde.mathEvalArray(sbCode.ToString().Replace("$$code$$", strExp2), d, width, out strErr);
                if (strErr != "")
                { System.Windows.Forms.MessageBox.Show(strErr); return; }
                this.dbPlotData2 = out2;
            }
        }


    }
}
