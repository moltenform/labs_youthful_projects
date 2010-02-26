using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace chaosExplorerControl
{
    public class PointPlotBitmapUserControl : PointPlotUserControl
    {
        protected Bitmap bitmap = null;
        protected Bitmap bitmapRender = null;
        protected double[] dbData = null;
        protected double[] dbDataRender = null;

        public PointPlotBitmapUserControl()
        {
            int paintWidth = getControlPaintWidth();
            int paintHeight = getControlPaintHeight();
            bitmap = new Bitmap(paintWidth, paintHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            dbData = new double[paintWidth * paintHeight];

            // not necessary to fill dbData with 0.0s because doubles default to 0.0
        }

        


        public double simpleFunction(double fx)
        {
            return fx * fx;
        }

        //draw a parabola (override this). 
        public virtual void getData(int width, int height, ref double[] elems)
        {
            double dx = (X1-X0)/width;
            double fx = X0, fy=0.0;
            int y;
            for (int x = 0; x < width; x++)
            {
                fy = simpleFunction(fx);

                for (y = 0; y < height; y++)
                    elems[y + x * height] = 0.1;

                double py = height - height * ((fy - Y0) / (Y1 - Y0));
                y = (int)py;
                if (y >= 0 && y < height)
                {
                    elems[y + x * height] = 0.9;
                }

                fx += dx;
            }
        }

        public virtual void getcolors(double d, out byte r, out byte g, out byte b)
        {
            byte shade = (byte)(255.0 * d);
            r=g=b=shade;
        }

        // the method called for drawing the main image (i.e. not "render" to disk)
        protected override void drawPlot()
        {
            int paintWidth = getControlPaintWidth();
            int paintHeight = getControlPaintHeight();
            createBitmap(paintWidth, paintHeight, ref this.dbData, ref this.bitmap);
        }

        protected void createBitmap(int width, int height, ref double[] elems, ref Bitmap lbitmap)
        {
            getData(width, height, ref elems);

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
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.DrawImageUnscaled(this.bitmap, 0, 0);
        }

        public override void renderToDiskSave(int width, int height, string sFilename)
        {
            if (dbDataRender==null || dbDataRender.Length!=width*height)
            {
                bitmapRender = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                dbDataRender = new double[width*height];
            }
            createBitmap(width, height, ref this.dbDataRender, ref this.bitmapRender);
            this.bitmapRender.Save(sFilename);
           
        }
    }
}
