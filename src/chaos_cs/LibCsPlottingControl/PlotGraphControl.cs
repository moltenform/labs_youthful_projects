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
        Pen pen1,pen2;
        public PointPlotGraphUserControl()
        {
            int paintWidth = getControlPaintWidth();
            int paintHeight = getControlPaintHeight();
            bitmap = new Bitmap(paintWidth, paintHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            pen1 = new Pen(Color.Red, 1.0f);
            pen2 = new Pen(Color.Blue, 1.0f);

            dbPlotData1=new double[paintWidth];
            dbPlotData2=new double[paintWidth];
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

        public virtual void getPlotPoints(int width, int height, ref double[] data1, ref double[] data2)
        {
            double fx = X0, xinc=(X1-X0)/width;
            for (int i=0; i<height; i++)
            {
                data1[i] = 1/Math.Cos((1/Math.Cos(fx+1.5))+1.5);
                data2[i] = fx; //fx*fx*fx;
                fx+=xinc;
            }

        }
        private void drawPlotSeries(int width, int height, double[] data, Graphics g, Pen pen)
        {
            for (int i=0; i<width-1; i++)
            {
                double fy1 = data[i];
                double fy2 = data[i+1];
                int iy1 = (int)(height - height * ((fy1 - Y0) / (Y1 - Y0)));
                int iy2 = (int)(height - height * ((fy2 - Y0) / (Y1 - Y0)));
                if (iy1 >=0 && iy1<height && iy2 >=0 && iy2<height)
                {
                    g.DrawLine(pen, i, iy1, i+1, iy2);
                }
                else if (iy1 >=0 && iy1<height)
                {
                    //g.DrawLine(pen1, i, iy1, i+1, iy1+1);
                    iy2 = Math.Min(iy2, height-1);
                    iy2 = Math.Max(iy2, 0);
                    g.DrawLine(pen, i, iy1, i+1, iy2);

                }
                else if (iy2 >=0 && iy2<height)
                {
                    iy1 = Math.Min(iy1, height-1);
                    iy1 = Math.Max(iy1, 0);
                    g.DrawLine(pen, i, iy1, i+1, iy2);
                    //g.DrawLine(pen1, i, iy2, i+1, iy2+1);
                }
            }

        }


        protected void createBitmap(int width, int height, ref double[] data1, ref double[] data2, ref Bitmap lbitmap)
        {
            getPlotPoints(width, height, ref data1, ref data2);
            bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.FillRectangle(new SolidBrush(Color.White), 0, 0, width, height);
                if (bAxes)
                {
                    int iZero = (int)(height - height * ((0.0 - Y0) / (Y1 - Y0)));
                    if (iZero >=0 && iZero<height)
                        g.DrawLine(new Pen(Color.LightGray, 1.0f), 0, iZero, width, iZero);
                    int ixZero = (int)(width * ((0.0 - X0)) / (X1 - X0));
                    if (ixZero >=0 && ixZero<width)
                        g.DrawLine(new Pen(Color.LightGray, 1.0f), ixZero, 0, ixZero, height);
                }

                drawPlotSeries(width,height,data1, g, pen1);
                if (bDrawSecond)
                    drawPlotSeries(width, height, data2, g, pen2);

            }


        }

        // the method called for drawing the main image (i.e. not "render" to disk)
        protected override void drawPlot()
        {
            int paintWidth = getControlPaintWidth();
            int paintHeight = getControlPaintHeight();
            createBitmap(paintWidth, paintHeight, ref this.dbPlotData1, ref this.dbPlotData2, ref this.bitmap);
            
        }

    }
}
