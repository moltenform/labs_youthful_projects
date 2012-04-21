using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Drawing;

namespace BurgerManagerie
{
    public class DrawMenagerie
    {
        string sFolder = @"..\..\..\";
        public void RedrawImage()
        {
            TextReader tr = new StreamReader(sFolder+"dataYES.txt");
            int width=450, height=450;
            double[] bigdata = new double[width * height];
            for (int i=0; i<bigdata.Length; i++)
                bigdata[i] = Math.Sqrt(double.Parse(tr.ReadLine()))/45.0;
            tr.Close();
            Bitmap b = createBitmap(width, height, bigdata);
            b.Save(sFolder+"ReadoutYes.png");
        }

        public void DrawPhasePortrait()
        {
            int smwidth=100, smheight=100;
            double[] smimage = new double[smwidth * smheight];

            int[] smalldata = new int[smwidth * smheight];
            PhasePortrait(0.9624, 1.508, smalldata);
            for (int i=0; i<smalldata.Length; i++)
                smimage[i] = (smalldata[i]==0)?1.0:0.0;
            Bitmap b = createBitmap(smwidth, smheight, smimage);
            b.Save(sFolder+"out.png");
        }

        public void Go()
        {
            bool bC;
            Random R = new Random();
            string sSuffix = InputBoxForm.GetStrInput("Suffix:", R.Next(100).ToString());
            if (sSuffix == null || sSuffix =="") return;
            int width = (int)getString("Width:", "200", out bC);
            if (!bC) return;
            int height = (int)getString("Height:", "200", out bC);
            if (!bC) return;
            double x0 = getString("X0:", "-3.5", out bC);
            if (!bC) return;
            double x1 = getString("X1:", "0", out bC);
            if (!bC) return;
            double y0 = getString("Y0:", "0.5", out bC);
            if (!bC) return;
            double y1 = getString("Y1:", "1.5", out bC);
            if (!bC) return;
            double isBurger = getString("burgerPos,henonNeg:", "1", out bC);
            if (!bC) return;
            createManag(width, height, x0, x1, y0, y1, isBurger>0, sSuffix);


        }
        private double getString(string prompt, string def, out bool bSuccess)
        {
            string s = InputBoxForm.GetStrInput(prompt, def);
            if (s == null || s =="") { bSuccess=false; return 0; }
            bSuccess = true;
            return double.Parse(s);
        }
        private int paramSettle=400;

        private void createManag(int width, int height, double X0, double X1, double Y0, double Y1, bool bIsBurger, string sSuffix)
        {
            double[] bigdata = new double[width * height];
            TextWriter tw= new StreamWriter(sFolder+"data"+sSuffix+".txt");

            int smwidth=100, smheight=100;
            int[] smalldata = new int[smwidth * smheight];

            //double X0=-3.5, X1=1.5, Y0=-2.5, Y1=2.5; double val, fx, fy; //sqr axes
            //double X0=-3.5, X1=1.5, Y0=0.0, Y1=2.5; double val, fx, fy; //interest
            //double X0=-3.5, X1=-1, Y0=0.0, Y1=2.5; double val, fx, fy; //interest
            //double X0=-1, X1=1, Y0=0.5, Y1=2.5; 
            double fx, fy;
            double dx = (X1 - X0) / width, dy = (Y1 - Y0) / height;
            fx = X0; fy = Y1; //y counts downwards
            for (int px = 0; px < width; px+=1)
            {
                fy = Y1;
                for (int py=0; py<height; py++)
                {
                    PhasePortrait(fx, fy, smalldata);
                    double total = 0.0;
                    for (int i=0; i<smalldata.Length; i++)
                        if (smalldata[i]!=0)
                            total++;
                    bigdata[py + px*height] = Math.Sqrt(total)/20;
                    tw.WriteLine(total.ToString());
                    fy -= dy;
                }
                fx += dx;
            }
            Bitmap b = createBitmap(width, height, bigdata);
            tw.Close();
            b.Save(sFolder+"Allout"+sSuffix+".png");
        }

        private void PhasePortrait(double c1, double c2, int[] arrAns)
        {
            for (int i = 0; i < arrAns.Length; i++)
                arrAns[i] = 0; //CLEAR ARRAY

            //LOCAL vars, not MEMBERS
            int width=100, height=100;
            double X0=-3.7416875, X1=0.728875, Y0=-2.1446875, Y1=2.1446875;

            double x, y, x_, y_;

            Random R = new Random();

            //can be changed in init:
            int nXpoints = 80/4, nYpoints = 80/4;
            double sx0= -2, sx1=2, sy0= -2, sy1=2;
            double additionalDarkening = 1.0;

            additionalDarkening=0.1;

            int nItersPerPoint = 16;
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
                        x_ = c1*x - y*y;
                        y_ = c2*y + x*y;

                        x=x_;
                        y=y_;
                        if (double.IsInfinity(x)||double.IsInfinity(y))
                            break;
                    }
                    for (int ii=0; ii<nItersPerPoint; ii++)
                    {
                        x_ = c1*x - y*y;
                        y_ = c2*y + x*y;

                        x=x_;
                        y=y_;

                        int px = (int)(width * ((x - X0) / (X1 - X0)));
                        int py = (int)(height - height * ((y - Y0) / (Y1 - Y0)));
                        if (py >= 0 && py < height && px>=0 && px<width)
                            arrAns[py + px * height]++;
                        if (double.IsInfinity(x)||double.IsInfinity(y))
                            break;
                    }
                }
            }

        }
        protected Bitmap createBitmap(int width, int height, double[] elems)
        {
            Bitmap lbitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            // the return format is BGR, NOT RGB.
            BitmapData bmData = lbitmap.LockBits(new Rectangle(0, 0, lbitmap.Width, lbitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            byte r, g, b;

            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;
            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                int nOffset = stride - width * 3;
                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        int elem = y + x * height; //this is organized differently, height x width
                        getcolors(elems[elem], out r, out g, out b);

                        p[0] = b;
                        p[1] = g;
                        p[2] = r;
                        p += 3;
                    }
                    p += nOffset;
                }
            }
            lbitmap.UnlockBits(bmData);
            return lbitmap;
        }
        public void getcolors(double d, out byte r, out byte g, out byte b)
        {
            if (double.IsNegativeInfinity(d))
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
    }
}
