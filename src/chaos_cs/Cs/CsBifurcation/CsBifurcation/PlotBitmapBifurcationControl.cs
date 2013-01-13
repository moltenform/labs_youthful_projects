using System;
using System.Collections.Generic;
using System.Text;
using chaosExplorerControl;

namespace CsBifurcation
{
    public class PlotBitmapBifurcationControl  : PointPlotBitmapUserControl
    {
        public int paramSettle = 200;
        public double param1, param2,param3,param4, paramShading = 0.1, paramAdditionalIters = 1.0;
        public string paramExpression, paramInit, paramP0;
        public bool bShading=true;
        
        public PlotBitmapBifurcationControl()
        {
            paramExpression = "p=p*r*(1-p);";
            paramInit = "";
            paramP0 = "0.35";
        }

        protected override int getControlPaintWidth() { return 400; }
        protected override int getControlPaintHeight() { return 400; }
        public override void setInitialBounds()
        {
            setBounds(-5, 5, -5, 5);
        }
        protected override string getAdditionalParameters()
        {
            string s = 
                "\r\nc1="+param1.ToString() + "\r\nc2="+param2.ToString()+
                "\r\nc3="+param3.ToString() + "\r\nc4="+param4.ToString()+
                "\r\nshade="+paramShading.ToString() + "\r\nparamSettle="+param4.ToString();
            return s + base.getAdditionalParameters();
        }
        
        public override void getData(int width, int height, ref double[] elems)
        {
            if (paramP0.Trim()=="") paramP0 = "0.5";
            //Pass in: X0,X1,Y0,Y1,WIDTH,HEIGHT,paramShading,paramSettle
            Dictionary<string, double> d = new Dictionary<string, double>();
            d["X0"] = X0; d["X1"] = X1; d["Y0"] = Y0; d["Y1"] = Y1;
            d["fWIDTH"] = width; d["fHEIGHT"] = height;
            d["paramShading"] = paramShading; d["paramSettle"] = paramSettle;
            d["c1"] = param1; d["c2"] = param2; d["c3"] = param3; d["c4"] = param4;
            d["paramAdditionalIters"] = paramAdditionalIters; //can't be set by code or cfg, use additionalShading

            string sTemplate = @"
            double additionalShading=1.0; //can be set by code
            double shadingAmount = paramShading*0.2 + 0.8;
            int nPointsDrawn = (int)(paramShading * 40)+1;
            int nSettletime = (int)paramSettle;
            int width=(int)fWIDTH, height=(int)fHEIGHT;
            for (int i = 0; i < arrAns.Length; i++) arrAns[i] = 1.0; //set all white

            Random R = new Random();
            double _dx = (X1 - X0) / width;
            double fx = X0;
            int _y;
            double p;
            $$INITCODE$$
            int nIterations = (int) (paramAdditionalIters*additionalShading*10000);
            for (int _x = 0; _x < width; _x++)
            {
                double r = fx;
                p = $$PNAUGHTEXPRESSION$$;
                for (int _i = 0; _i < nSettletime; _i++)
                {
                   $$CODE$$
                }

                for (int _i = 0; _i < $$NITERS$$; _i++)
                {
                    $$CODE$$

                    _y = (int)(height - height * ((p - Y0) / (Y1 - Y0)));
                    if (_y >= 0 && _y < height)
                        arrAns[_y + _x * height] $$SHADEOPERATION$$
                }
                fx += _dx;
            }";
            string sItersPer = (bShading)?"nIterations" : "nPointsDrawn";
            string sShadeOperation = (bShading)?"*= shadingAmount;" : "= 0.2;";
            //string sShadeOperation = (bShading)?"-= shadingAmount;" : "= 0.2;";
            //string sShadeOperation = (bShading)?"= (arrAns[y + x * HEIGHT]>shadingAmount)?(arrAns[y + x * HEIGHT]-shadingAmount):shadingAmount;" : "= 0.2;";

            sTemplate = sTemplate.Replace("$$NITERS$$", sItersPer);
            sTemplate = sTemplate.Replace("$$CODE$$", paramExpression);
            sTemplate = sTemplate.Replace("$$INITCODE$$", paramInit);
            sTemplate = sTemplate.Replace("$$PNAUGHTEXPRESSION$$", paramP0);
            sTemplate = sTemplate.Replace("$$SHADEOPERATION$$", sShadeOperation);

            //System.Windows.Forms.Clipboard.SetText(sTemplate);
            string strErr = "";
            CodedomEvaluator.CodedomEvaluator cde = new CodedomEvaluator.CodedomEvaluator();
            double[] out1 = cde.mathEvalArray(sTemplate, d, width*height, out strErr);
            if (strErr != "")
            { System.Windows.Forms.MessageBox.Show(strErr); return; }
            //because of ref counting, apparently ok to set reference like this instead of copying elements.

            System.Diagnostics.Debug.Assert(out1.Length == width*height);
            elems = out1;
            

            /*if (!bShading) //redistribute ink? 
            {
                double totalInkGoal = WIDTH * HEIGHT * 0.05;
                double total = 0.0;
                foreach (double dd in this.dbData) total+=(1-dd);
                //System.Windows.Forms.MessageBox.Show("" + total);
                double scale = totalInkGoal / total ;
                System.Windows.Forms.MessageBox.Show(": " + total + "   "+totalInkGoal + "   "+scale);
                for (int i =0; i<this.dbData.Length; i++) this.dbData[i] = (1-(1-this.dbData[i]) * scale);
            }*/

        }
        
    }
}
