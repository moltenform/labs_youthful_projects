using System;
using System.Collections.Generic;
using System.Text;
using chaosExplorerControl;

namespace CsBifurcation
{
    public class PlotBitmapPhasePortraitControl  : PointPlotBitmapUserControl
    {
        public int paramSettle = 100, paramTotalIters = 640; //will be mult by 1000
        public double param1, param2,param3,param4;
        public string paramExpression, paramInit;
        public int paramAdditionalIters = 1; //for rendering
        public const double BLUE = double.MinValue;
        public const double RED = double.NegativeInfinity;
        
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

        
        public void getDataTemplate(int width, int height, ref double[] arrAns)
        {
            double x, y, x_, y_;
            for (int i = 0; i < arrAns.Length; i++)
                arrAns[i] = 1.0; //set all white

            ///can be changed in init:
            int nXpoints = 80, nYpoints = 80;
            double sx0= -2, sx1=2, sy0= -2, sy1=2;
            double additionalDarkening = 1.0;
            ///

            //INIT CODE


            //iters represents total iters. we divide by how many points. 
            // In this way, if nXpoints changed, density remains roughly constant.
            int nItersPerPoint = (int)Math.Round(((paramTotalIters*1000.0*paramAdditionalIters) / ((double)nXpoints * nYpoints)));
            double sxinc = (nXpoints==1) ? double.MaxValue : (sx1-sx0)/(nXpoints-1);
            double syinc = (nYpoints==1) ? double.MaxValue : (sy1-sy0)/(nYpoints-1);

            System.Diagnostics.Debug.Assert(sx1>sx0 && sy1>sy0 && sxinc>0 && syinc>0 && nXpoints>0 && nYpoints>0);
            double shadeAmount = 0.9 * additionalDarkening;
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
                            arrAns[py + px * height] *= shadeAmount;
                    }
                }
            }

        }
        public override void getData(int width, int height, ref double[] elems) //template
        {
            Dictionary<string, double> d = new Dictionary<string, double>();
            d["X0"] = X0; d["X1"] = X1; d["Y0"] = Y0; d["Y1"] = Y1;
            d["fWIDTH"] = width; d["fHEIGHT"] = height;
            d["paramSettle"] = paramSettle;
            d["paramTotalIters"] = paramTotalIters;
            d["paramAdditionalIters"] = paramAdditionalIters;
            d["c1"] = param1; d["c2"] = param2; d["c3"] = param3; d["c4"] = param4;

            
            string sTemplate = @"
            int width=(int)fWIDTH, height=(int)fHEIGHT;
            double x, y, x_, y_;
            for (int i = 0; i < arrAns.Length; i++) 
                arrAns[i] = 1.0; //set all white

            //can be changed in init:
            int nXpoints = 80, nYpoints = 80;
            double sx0= -2, sx1=2, sy0= -2, sy1=2;
            double additionalDarkening = 1.0;

            $$INITCODE$$

            // iters represents total iters. we divide by how many points. 
            // In this way, if nXpoints changed, density remains roughly constant.
            int nItersPerPoint = (int)Math.Round(((paramTotalIters*1000.0*paramAdditionalIters) / ((double)nXpoints * nYpoints)));
            double sxinc = (nXpoints==1) ? double.MaxValue : (sx1-sx0)/(nXpoints-1);
            double syinc = (nYpoints==1) ? double.MaxValue : (sy1-sy0)/(nYpoints-1);

            System.Diagnostics.Debug.Assert(sx1>sx0 && sy1>sy0 && sxinc>0 && syinc>0 && nXpoints>0 && nYpoints>0);
            double shadeAmount = 0.9 * additionalDarkening;
            for (double sx=sx0; sx<=sx1; sx+=sxinc)
            {
                for (double sy=sy0; sy<=sy1; sy+=syinc)
                {
                    x = sx; y=sy;

                    for (int ii=0; ii<paramSettle; ii++)
                    {
                        $$CODE$$
                        x=x_; 
                        y=y_;
                    }
                    for (int ii=0; ii<nItersPerPoint; ii++)
                    {
                        $$CODE$$
                        x=x_;
                        y=y_;

                        int px = (int)(width * ((x - X0) / (X1 - X0)));
                        int py = (int)(height - height * ((y - Y0) / (Y1 - Y0)));
                        if (py >= 0 && py < height && px>=0 && px<width)
                            arrAns[py + px * height] *= shadeAmount;
                    }
                }
            }";
            sTemplate = sTemplate.Replace("$$CODE$$", paramExpression);
            sTemplate = sTemplate.Replace("$$INITCODE$$", paramInit);
            string strErr = "";
            CodedomEvaluator.CodedomEvaluator cde = new CodedomEvaluator.CodedomEvaluator();
            double[] out1 = cde.mathEvalArray(sTemplate, d, width*height, out strErr);
            if (strErr != "")
            { System.Windows.Forms.MessageBox.Show(strErr); return; }

            //because of ref counting, apparently ok to set reference like this instead of copying elements.
            elems = out1;
            System.Diagnostics.Debug.Assert(out1.Length == width*height);
        }

       

        public override void getcolors(double d, out byte r, out byte g, out byte b)
        {
            if (d==BLUE)
            {
                r=0; g=0; b=255;
            }
            else if (double.IsNegativeInfinity(d))
            {
                r=255; g=0; b=0;
            }
            else
            {
                if (d>1.0) d=1.0;
                else if (d<0.0) d=0.0;
                byte shade = (byte)(255.0 * d);
                r=g=b=shade;
            }
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
