using System;
using System.Collections.Generic;
using System.Text;
using chaosExplorerControl;

namespace CsBifurcation
{
    public class PlotBitmapBifurcationControl  : PointPlotBitmapUserControl
    {
        public int paramSettle = 200;
        public double param1, param2, paramShading = 0.1;
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
        private void getData_TemplateShading(double[] elems)
        {
            double shadingAmount = paramShading/5;
            for (int i = 0; i < elems.Length; i++) elems[i] = 1.0; //set all white

            Random R = new Random();
            double dx = (X1 - X0) / WIDTH;
            double fx = X0;
            int y;
            double p;
            //$$INITCODE$$
            for (int x = 0; x < WIDTH; x++)
            {
                double r = fx;
                p = 0.35; //$$PNAUGHTEXPRESSION$$
                for (int i = 0; i < paramSettle; i++)
                {
                    p = r*p*(1-p); //$$CODE$$
                }

                for (int i = 0; i < 10000; i++)
                {
                    p = r*p*(1-p); //$$CODE$$
                    
                    y = (int)(HEIGHT - HEIGHT * ((p - Y0) / (Y1 - Y0)));
                    if (y >= 0 && y < HEIGHT)
                        elems[y + x * HEIGHT] -= shadingAmount;
                }
                fx += dx;
            }
        }
        private void getData_TemplatePoints(double[] elems)
        {
            int nPointsDrawn = (int)(paramShading * 40)+1;
            for (int i = 0; i < elems.Length; i++) elems[i] = 1.0; //set all white

            Random R = new Random();
            double dx = (X1 - X0) / WIDTH;
            double fx = X0;
            int y;
            double p;
            //$$INITCODE$$
            for (int x = 0; x < WIDTH; x++)
            {
                double r = fx;
                p = 0.35; //$$PNAUGHTEXPRESSION$$
                for (int i = 0; i < paramSettle; i++)
                {
                    p = r*p*(1-p); //$$CODE$$
                }

                for (int i = 0; i < nPointsDrawn; i++)
                {
                    p = r*p*(1-p); //$$CODE$$

                    y = (int)(HEIGHT - HEIGHT * ((p - Y0) / (Y1 - Y0)));
                    if (y >= 0 && y < HEIGHT)
                        elems[y + x * HEIGHT] = 0.2;
                }
                fx += dx;
            }
        }

        public override void getData(int width, int height, ref double[] elems)
        {
            //getData_TemplateShadingLanding(elems);
            //return;

            //Pass in: X0,X1,Y0,Y1,WIDTH,HEIGHT,paramShading,paramSettle
            Dictionary<string, double> d = new Dictionary<string, double>();
            d["X0"] = X0; d["X1"] = X1; d["Y0"] = Y0; d["Y1"] = Y1;
            d["fWIDTH"] = width; d["fHEIGHT"] = height;
            d["paramShading"] = paramShading; d["paramSettle"] = paramSettle;

            //provide: paramShading, paramSettle
            string sTemplate = @"
            //double shadingAmount = paramShading/5;
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
            string sItersPer = (bShading)?"10000" : "nPointsDrawn";
            //string sShadeOperation = (bShading)?"-= shadingAmount;" : "= 0.2;";
            string sShadeOperation = (bShading)?"*= shadingAmount;" : "= 0.2;";
            //string sShadeOperation = (bShading)?"= (arrAns[y + x * HEIGHT]>shadingAmount)?(arrAns[y + x * HEIGHT]-shadingAmount):shadingAmount;" : "= 0.2;";

            sTemplate = sTemplate.Replace("$$NITERS$$", sItersPer);
            sTemplate = sTemplate.Replace("$$CODE$$", paramExpression);
            sTemplate = sTemplate.Replace("$$INITCODE$$", paramInit);
            sTemplate = sTemplate.Replace("$$PNAUGHTEXPRESSION$$", paramP0);
            sTemplate = sTemplate.Replace("$$SHADEOPERATION$$", sShadeOperation);

            System.Windows.Forms.Clipboard.SetText(sTemplate);
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

        private void getData_TemplateShadingLanding(double[] elems)
        {
            double shadingAmount = paramShading/5;
            for (int i = 0; i < elems.Length; i++) elems[i] = 1.0; //set all white

            Random R = new Random();
            double dx = (X1 - X0) / WIDTH;
            double fx = X0;
            int y;
            double p;
            //$$INITCODE$$
            int landings=0;
            while (landings < 100*WIDTH)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    double r = fx;
                    p = 0.35; //$$PNAUGHTEXPRESSION$$
                    for (int i = 0; i < paramSettle; i++)
                    {
                        p = r*p*(1-p); //$$CODE$$
                    }

                    for (int i = 0; i < 10000; i++)
                    {
                        p = r*p*(1-p); //$$CODE$$

                        y = (int)(HEIGHT - HEIGHT * ((p - Y0) / (Y1 - Y0)));
                        if (y >= 0 && y < HEIGHT)
                        {
                            elems[y + x * HEIGHT] -= shadingAmount;
                            landings++;
                        }
                    }
                    fx += dx;
                }
            }
        }

    }
}
