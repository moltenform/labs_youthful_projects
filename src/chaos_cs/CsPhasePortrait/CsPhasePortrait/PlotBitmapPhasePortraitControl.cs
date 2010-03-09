using System;
using System.Collections.Generic;
using System.Text;
using chaosExplorerControl;

namespace CsBifurcation
{
    public class PlotBitmapPhasePortraitControl  : PointPlotBitmapUserControl
    {
        //public string paramP0; //delete
        //public bool bShading;//delete
        //public double paramShading; //delete


        public int paramSettle = 100, paramTotalIters = 640; //will be mult by 1000
        public double param1, param2,param3,param4, paramDarkening = 0.9;
        public string paramExpression, paramInit;
        public int paramAdditionalIters = 1; //for rendering
        
        public PlotBitmapPhasePortraitControl()
        {
            paramExpression = "x_ = 1 - c1*x*x + y;\r\ny_ = c2*x;";
            paramInit = "";
            param1 = 1.4; param2 = 0.3;
        }

        protected override int getControlPaintWidth() { return 400; }
        protected override int getControlPaintHeight() { return 400; }
        public override void setInitialBounds()
        {
            setBounds(-5, 5, -5, 5);
        }

        public override void getData(int width, int height, ref double[] arrAns) //template
        {
            for (int i = 0; i < arrAns.Length; i++) 
                arrAns[i] = 1.0; //set all white

            int nXpoints = 80, nYpoints = 80;
            double sx0= -2, sx1=2, sy0= -2, sy1=2;
            double x, y, x_, y_;

            //INIT CODE

            //iters represents total iters. we divide by how many points. 
            // In this way, if nXpoints changed, density remains roughly constant.
            int nItersPerPoint = (int)Math.Round(((paramTotalIters*1000.0*paramAdditionalIters) / ((double)nXpoints * nYpoints)));
            double sxinc = (nXpoints==1) ? double.MaxValue : (sx1-sx0)/(nXpoints-1);
            double syinc = (nYpoints==1) ? double.MaxValue : (sy1-sy0)/(nYpoints-1);

            System.Diagnostics.Debug.Assert(sx1>sx0 && sy1>sy0 && sxinc>0 && syinc>0 && nXpoints>0 && nYpoints>0);
            double a = param1, b=param2;
            for (double sx=sx0; sx<=sx1; sx+=sxinc)
            {
                for (double sy=sy0; sy<=sy1; sy+=syinc)
                {
                    x = sx; y=sy;

                    for (int ii=0; ii<paramSettle; ii++)
                    {
                        x_ = 1-a*x*x + y;
                        y_ = b*x;
                        x=x_; 
                        y=y_;
                    }
                    for (int ii=0; ii<nItersPerPoint; ii++)
                    {
                        x_ = 1-a*x*x + y;
                        y_ = b*x;
                        x=x_;
                        y=y_;

                        int px = (int)(width * ((x - X0) / (X1 - X0)));
                        int py = (int)(height - height * ((y - Y0) / (Y1 - Y0)));
                        if (py >= 0 && py < height && px>=0 && px<width)
                            arrAns[py + px * height] *= paramDarkening;
                    }
                }
            }

        }

        public void getDataOld(int width, int height, ref double[] elems)
        {
            /*
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
            double dx = (X1 - X0) / width;
            double fx = X0;
            int y;
            double p;
            $$INITCODE$$
            int nIterations = (int) (paramAdditionalIters*additionalShading*10000);
            for (int x = 0; x < width; x++)
            {
                double r = fx;
                p = $$PNAUGHTEXPRESSION$$;
                for (int i = 0; i < nSettletime; i++)
                {
                   $$CODE$$
                }

                for (int i = 0; i < $$NITERS$$; i++)
                {
                    $$CODE$$

                    y = (int)(height - height * ((p - Y0) / (Y1 - Y0)));
                    if (y >= 0 && y < height)
                        arrAns[y + x * height] $$SHADEOPERATION$$
                }
                fx += dx;
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
            */

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


        protected override string getAdditionalParameters()
        {
            string s = 
                "\r\nc1="+param1.ToString() + "\r\nc2="+param2.ToString()+
                "\r\nc3="+param3.ToString() + "\r\nc4="+param4.ToString();
                
            return s + base.getAdditionalParameters();
        }
    }
}
