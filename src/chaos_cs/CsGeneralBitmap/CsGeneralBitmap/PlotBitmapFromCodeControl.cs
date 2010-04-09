using System;
using System.Collections.Generic;
using System.Text;
using chaosExplorerControl;

namespace CsBifurcation
{
    public delegate void AltShiftDragDelegate(double nx0, double nx1, double ny0, double ny1);
    public class PlotBitmapFromCodeControl  : PointPlotBitmapUserControl
    {
        public int paramSettle = 100, paramIters = 640;
        public double param1, param2,param3,param4;
        public string paramExpression;
        public bool bIsRendering;
        public bool bIsManualColor=false;

        public event AltShiftDragDelegate OnAltShiftDrag;
        public const double RED = double.NegativeInfinity;
        //negative values blue. will cycle possibly.
        
        public PlotBitmapFromCodeControl()
        {
            paramExpression = "//$$standardloop\r\nval = fx*fy;\r\n//$$endstandardloop";
        }

        protected override int getControlPaintWidth() { return 400; }
        protected override int getControlPaintHeight() { return 400; }
        public override void setInitialBounds()
        {
            setBounds(-5, 5, -5, 5);
        }
        protected override void onAltShiftDrag(double nx0, double nx1, double ny0, double ny1)
        {
            if (OnAltShiftDrag!=null) OnAltShiftDrag(nx0, nx1, ny0, ny1);
        }

        
        public void getDataTemplate(int width, int height, ref double[] arrAns)
        {
            
        }
        public override void getData(int width, int height, ref double[] elems) //template
        {
            Dictionary<string, double> d = new Dictionary<string, double>();
            d["X0"] = X0; d["X1"] = X1; d["Y0"] = Y0; d["Y1"] = Y1;
            d["fWIDTH"] = width; d["fHEIGHT"] = height;
            d["fparamSettle"] = paramSettle; d["fparamIters"] = paramIters;
            d["fIsRendering"] = bIsRendering ? 1.0:0.0;
            d["c1"] = param1; d["c2"] = param2; d["c3"] = param3; d["c4"] = param4;

            string sTemplate=@"
            double val, fx, fy; //for standard loop
            double rval,gval,bval;
            bool bIsRendering = fIsRendering > 0;
            double RED = double.NegativeInfinity;
            int width=(int)fWIDTH, height=(int)fHEIGHT;
            int paramSettle=(int)fparamSettle, paramIters=(int)fparamIters;
            Random R = new Random();
            bool bSwitch=false;

            $$CODE$$
            ";
            if (System.Text.RegularExpressions.Regex.IsMatch(paramExpression, "\\brval\\b"))
            {
                sTemplate = sTemplate.Replace("$$CODE$$", paramExpression);
                sTemplate = sTemplate.Replace("//$$standardloop", sStandardLoopBegin);
                sTemplate = sTemplate.Replace("//$$endstandardloop", sStandardColorLoopEnd);
                bIsManualColor = true;
            }
            else
            {
                sTemplate = sTemplate.Replace("$$CODE$$", paramExpression);
                sTemplate = sTemplate.Replace("//$$standardloop", sStandardLoopBegin);
                sTemplate = sTemplate.Replace("//$$endstandardloop", sStandardLoopEnd);
                bIsManualColor = false;
            }
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
            if (double.IsNegativeInfinity(d))
            {
                r=255; g=0; b=0;
            }
            else if (bIsManualColor && d>63.9 /*64*/ && d<16777280.0+64.0)
            {
                int rgb = (int)(d-64);
                r = (byte)((rgb & 0x00ff0000)>>16);
                g = (byte)((rgb & 0x0000ff00)>>8);
                b = (byte)((rgb & 0x000000ff));
            }
            else
            {
                // negative ones are blue. -1 = blue to 0 = black to 1 = white
                if (d<0.0)
                {
                    if (d<-1.0) d=-1.0;
                    b= (byte)(255.0 * -d);
                    r=g=0;
                }
                else
                {
                    if (d>1.0) d=1.0;
                    byte shade = (byte)(255.0 * d);
                    r=g=b=shade;
                }
            }
        }

        protected override string getAdditionalParameters()
        {
            string s = 
                "\r\nc1="+param1.ToString() + "\r\nc2="+param2.ToString()+
                "\r\nc3="+param3.ToString() + "\r\nc4="+param4.ToString();
                
            return s + base.getAdditionalParameters();
        }
        public static string sStandardLoopBegin = @" 
            val=0;
            double dx = (X1 - X0) / width, dy = (Y1 - Y0) / height;
            fx = X0; fy = Y1; //y counts downwards
            for (int px = 0; px < width; px+=1)
            {
                fy = Y1;
                for (int py=0; py<height; py++)
                {
            ";
        public static string sStandardLoopEnd = @" 
                    arrAns[py + px*height] = val;
                    fy -= dy;
                }
                fx += dx;
            }
        ";
        public static string sStandardColorLoopEnd = @" 
                    int iR=((int)(rval*255)) & 0xff;
                    int iG=((int)(gval*255))& 0xff;
                    int iB=((int)(bval*255))& 0xff;
                    arrAns[py + px*height] = (double)((iR<<16)+(iG<<8)+iB)+64;
                    fy -= dy;
                }
                fx += dx;
            }
        ";

    }
}
