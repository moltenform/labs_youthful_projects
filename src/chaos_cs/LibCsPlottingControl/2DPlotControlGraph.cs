using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace chaosExplorerControl
{
    public class PointPlotGraphUserControl : PointPlotUserControl
    {
        protected double[] dbPlotData1;
        protected double[] dbPlotData2;

        public bool bAxes = true;
        public bool bDrawSecond=false;
        Bitmap bitmap = null;
        Bitmap bitmapRender = null;
        Pen pen1,pen2;
        public PointPlotGraphUserControl()
        {
            bitmap = new Bitmap(WIDTH, HEIGHT, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            pen1 = new Pen(Color.Red, 1.0f);
            pen2 = new Pen(Color.Blue, 1.0f);

            dbPlotData1=new double[WIDTH];
            dbPlotData2=new double[WIDTH];
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (bitmap != null)
            {
                Graphics graphics = e.Graphics;
                graphics.DrawImageUnscaled(bitmap, 0, 0);
                //graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                //graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //graphics.DrawImage(bitmap, new Rectangle(0, 0, WIDTH, HEIGHT), new Rectangle(0, 0, bitmap.Width, bitmap.Height), GraphicsUnit.Pixel);

            }
        }
        public virtual void getPlotPoints()
        {
            double fx = X0, xinc=(X1-X0)/WIDTH;
            for (int i=0; i<WIDTH; i++)
            {
                dbPlotData1[i] = 1/Math.Cos((1/Math.Cos(fx+1.5))+1.5);
                dbPlotData2[i] = fx; //fx*fx*fx;
                fx+=xinc;
            }

        }
        private void drawPlotSeries(double[] data, Graphics g, Pen pen)
        {
            for (int i=0; i<WIDTH-1; i++)
            {
                double fy1 = data[i];
                double fy2 = data[i+1];
                int iy1 = (int)(HEIGHT - HEIGHT * ((fy1 - Y0) / (Y1 - Y0)));
                int iy2 = (int)(HEIGHT - HEIGHT * ((fy2 - Y0) / (Y1 - Y0)));
                if (iy1 >=0 && iy1<HEIGHT && iy2 >=0 && iy2<HEIGHT)
                {
                    g.DrawLine(pen, i, iy1, i+1, iy2);
                }
                else if (iy1 >=0 && iy1<HEIGHT)
                {
                    //g.DrawLine(pen1, i, iy1, i+1, iy1+1);
                    iy2 = Math.Min(iy2, HEIGHT-1);
                    iy2 = Math.Max(iy2, 0);
                    g.DrawLine(pen, i, iy1, i+1, iy2);

                }
                else if (iy2 >=0 && iy2<HEIGHT)
                {
                    iy1 = Math.Min(iy1, HEIGHT-1);
                    iy1 = Math.Max(iy1, 0);
                    g.DrawLine(pen, i, iy1, i+1, iy2);
                    //g.DrawLine(pen1, i, iy2, i+1, iy2+1);
                }
            }

        }

        protected override void drawPlot()
        {
            getPlotPoints();
            bitmap = new Bitmap(WIDTH, HEIGHT, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.FillRectangle(new SolidBrush(Color.White), 0, 0, WIDTH, HEIGHT);
                if (bAxes)
                {
                    int iZero = (int)(HEIGHT - HEIGHT * ((0.0 - Y0) / (Y1 - Y0)));
                    if (iZero >=0 && iZero<HEIGHT)
                        g.DrawLine(new Pen(Color.LightGray, 1.0f), 0, iZero, WIDTH, iZero);
                    int ixZero = (int)(WIDTH * ((0.0 - X0)) / (X1 - X0));
                    if (ixZero >=0 && ixZero<WIDTH)
                        g.DrawLine(new Pen(Color.LightGray, 1.0f), ixZero, 0, ixZero, HEIGHT);
                }

                drawPlotSeries(dbPlotData1, g, pen1);
                if (bDrawSecond)
                    drawPlotSeries(dbPlotData2, g, pen2);

            }
        }
    }
}
