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
            bitmap = new Bitmap(WIDTH, HEIGHT, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            dbData = new double[WIDTH * HEIGHT];

            // fill with 0s
            for (int x = 0; x < WIDTH; x++)
                for (int y = 0; y < HEIGHT; y++)
                    dbData[y + x*HEIGHT] = 0; //note HEIGHT x WIDTH
        }

        


        public double simpleFunction(double fx)
        {
            return fx * fx;
        }

        //draw a parabola (override this). 
        public virtual void getData(double[] elems)
        {
            double dx = (X1-X0)/WIDTH;
            double fx = X0, fy=0.0;
            int y;
            for (int x = 0; x < WIDTH; x++)
            {
                fy = simpleFunction(fx);

                for (y = 0; y < HEIGHT; y++)
                    elems[y + x * HEIGHT] = 0.1;

                double py = HEIGHT - HEIGHT * ((fy - Y0) / (Y1 - Y0));
                y = (int)py;
                if (y >= 0 && y < HEIGHT)
                {
                    elems[y + x * HEIGHT] = 0.9;
                }

                fx += dx;
            }
        }

        public virtual void getcolors(double d, out byte r, out byte g, out byte b)
        {
            byte shade = (byte)(255.0 * d);
            r=g=b=shade;
        }

        protected override void drawPlot()
        {
            getData(this.dbData);

            // the return format is BGR, NOT RGB.
            BitmapData bmData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            byte r, g, b;

            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;
            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                int nOffset = stride - WIDTH * 3;
                for (int y = 0; y < HEIGHT; ++y)
                {
                    for (int x = 0; x < WIDTH; ++x)
                    {
                        int elem = y + x * HEIGHT; //this is organized differently, height x width
                        getcolors(this.dbData[elem], out r, out g, out b);

                        p[0] = b;
                        p[1] = g;
                        p[2] = r;
                        p += 3;
                    }
                    p += nOffset;
                }
            }
            bitmap.UnlockBits(bmData);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.DrawImageUnscaled(bitmap, 0, 0);
        }

        protected override void renderToDiskSave(int F, string sFilename)
        {
            if (dbDataRender==null || dbDataRender.Length!=WIDTH *F)
            {
                bitmapRender = new Bitmap(WIDTH*F, HEIGHT*F, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                dbDataRender = new double[HEIGHT*F];
            }
            Bitmap oldbmp = this.bitmap;
            double[] olddata = this.dbData;
            this.bitmap = this.bitmapRender;
            this.dbData = this.dbDataRender;

            try
            {
                HEIGHT *= F; WIDTH *= F;
                drawPlot();
                this.bitmap.Save(sFilename); //don't dispose, since we might use it again
            }
            finally { HEIGHT /= F; WIDTH /= F; }

            this.bitmap = oldbmp;
            this.dbData = olddata;
        }
    }
}
