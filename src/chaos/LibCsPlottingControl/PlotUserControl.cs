using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace chaosExplorerControl
{
    public partial class PointPlotUserControl : UserControl
    {
        protected int WIDTH, HEIGHT;
        public static readonly double RATIO = 1.0; //ratio of WIDTH to HEIGHT, regardless of stretching
        protected double X0, X1, Y0, Y1;
        protected double prevX0, prevX1, prevY0, prevY1;
        protected ClsLineRectangle rubberbanding = new ClsLineRectangle();
        protected virtual int getControlPaintWidth() { return 250; }
        protected virtual int getControlPaintHeight() { return 250; }
        public PointPlotUserControl()
        {
            InitializeComponent();
            WIDTH = getControlPaintWidth(); HEIGHT = getControlPaintHeight();

            this.MouseDown += new MouseEventHandler(rubberbanding.clsLineRectangle_OnMouseDown);
            this.MouseMove += new MouseEventHandler(rubberbanding.clsLineRectangle_OnMouseMove);
            this.MouseMove += new MouseEventHandler(PointPlotUserControl_MouseMove);
            this.MouseUp += new MouseEventHandler(rubberbanding.clsLineRectangle_OnMouseUp);
            this.MouseUp += new MouseEventHandler(PointPlotUserControl_MouseUp);

            setInitialBounds();
            //this.resetZoom(); Have it called by client
        }

        /// These will probably be overridden:

        public virtual void setInitialBounds()
        {
            setBounds(-10,10  , -10,10);
        }
        protected virtual string getAdditionalParameters() { return ""; } //write parameters
        protected virtual void drawPlot()
        {
            return;
        }
        protected virtual void renderToDiskSave(int F /*=4*/, string sFilename)
        {
            throw new NotImplementedException();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.DrawRectangle(new Pen(Color.Blue), 0, 0, WIDTH, HEIGHT);
        }





        public void setBounds(double newX0,double newX1, double newY0, double newY1)
        {
            prevX0 = X0; prevX1=X1; prevY0=Y0; prevY1=Y1;
            X0 = newX0; X1 = newX1; Y0 = newY0; Y1 = newY1;
            System.Diagnostics.Debug.Assert(Y1 > Y0 && X1 > X0);
        }
        public void getBounds(out double outX0, out double outX1, out double outY0, out double outY1)
        {
            outX0=X0; outY0=Y0; outX1=X1; outY1=Y1;
        }

        public void undoZoom()
        {
            setBounds(prevX0, prevX1, prevY0, prevY1);
            //prevX0 = X0; prevX1=X1; prevY0=Y0; prevY1=Y1;
            //X0 = prevX0; X1 = prevX1; Y0 = prevY0; Y1 = prevY1;
            System.Diagnostics.Debug.Assert(Y1 > Y0 && X1 > X0);
        }
        

        protected void PointPlotUserControl_MouseMove(object sender, MouseEventArgs e)
        {
            double fx = (e.X / ((double)WIDTH)) * (X1 - X0) + X0;
            double fy = ((HEIGHT-e.Y) / ((double)HEIGHT)) * (Y1 - Y0) + Y0;
            this.lblCoords.Text = String.Format( "{0:0.0000} , {1:0.0000}", fx , fy);
        }

        protected void PointPlotUserControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) 
            {
                //zoom to the drawn rectangle
                int ix = rubberbanding.SelectRect.X;
                int iy = HEIGHT - (rubberbanding.SelectRect.Y + rubberbanding.SelectRect.Height);
                int ixr = ix + rubberbanding.SelectRect.Width;
                int iyr = HEIGHT - (rubberbanding.SelectRect.Y);
                if (ixr-ix <= 0 || iyr-iy<=0)
                    return; //prevent 0x0 zoom
                double newX0, newY0, newX1, newY1;
                newX0 = (ix / ((double)WIDTH)) * (X1 - X0) + X0;
                newX1 = (ixr / ((double)WIDTH)) * (X1 - X0) + X0;
                newY0 = (iy / ((double)HEIGHT)) * (Y1 - Y0) + Y0;
                newY1 = (iyr / ((double)HEIGHT)) * (Y1 - Y0) + Y0;
                setBounds(newX0, newX1, newY0, newY1);
                redraw();
            }
            else if (e.Button == MouseButtons.Right)
            {
                this.undoZoom();
                redraw();
            }
            else if (e.Button == MouseButtons.Middle)
            {
                this.resetZoom();
            }
        }
        
        public void redraw()
        {
            drawPlot();
            this.Invalidate();
        }
        
        public void resetZoom()
        {
            setInitialBounds();
            redraw();
        }
        
        private void mnuCopyParams_Click(object sender, EventArgs e)
        {
            string s = String.Format("Xleft {0} Xright{1} Ytop {2} Ybottom {3}\r\n", X0, X1, Y0, Y1);
            s+= getAdditionalParameters();
            Clipboard.SetText(s);
        }
        
        public void renderToDisk(int F)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "png files (*.png)|*.png";
            saveFileDialog1.RestoreDirectory = true;
            if (!(saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK && saveFileDialog1.FileName.Length > 0))
                return;
            renderToDiskSave(F, saveFileDialog1.FileName);
        }

        private void mnuRender_Click(object sender, EventArgs e) { this.renderToDisk(4); }
        private void mnuRenderLarge_Click(object sender, EventArgs e) { this.renderToDisk(8); }
        private void mnuReset_Click_1(object sender, EventArgs e) { resetZoom(); }

        private void mnuZmWiden_Click(object sender, EventArgs e)
        {
            double newX0 = X0+ (X1-X0)/4;
            double newX1 = X1- (X1-X0)/4;
            setBounds(newX0, newX1, Y0, Y1);
            redraw();
        }

        private void mnuZmHeighten_Click(object sender, EventArgs e)
        {
            double newY0 = Y0- (Y1-Y0)/4;
            double newY1 = Y1+ (Y1-Y0)/4;
            setBounds(X0, X1, newY0, newY1);
            redraw();
        }

        private void mnuZoomIn_Click(object sender, EventArgs e)
        {
            double newX0, newY0, newX1, newY1;
            newX0 = X0+ (X1-X0)/4;
            newX1 = X1- (X1-X0)/4;
            newY0 = Y0+ (Y1-Y0)/4;
            newY1 = Y1- (Y1-Y0)/4;
            setBounds(newX0, newX1, newY0, newY1);
            redraw();
        }
        private void mnuZoomOut_Click(object sender, EventArgs e)
        {
            double newX0, newY0, newX1, newY1;
            newX0 = X0- (X1-X0)/2;
            newX1 = X1+ (X1-X0)/2;
            newY0 = Y0- (Y1-Y0)/2;
            newY1 = Y1+ (Y1-Y0)/2;
            setBounds(newX0, newX1,newY0, newY1);
            redraw();
        }

        private void mnuZoomUndo_Click(object sender, EventArgs e)
        {
            undoZoom();
            redraw();
        }
       
    }

    public class ClsLineRectangle
    {
        public Rectangle SelectRect = new Rectangle();
        Point ps = new Point();
        Point pe = new Point();
        public bool isBanding = false;

        public ClsLineRectangle()
        {
        }

        public void clsLineRectangle_OnMouseDown(Object sender, MouseEventArgs e)
        {
            SelectRect.Width = 0;
            SelectRect.Height = 0;
            SelectRect.X = e.X;
            SelectRect.Y = e.Y;

            ps.X = e.X;
            ps.Y = e.Y;
            pe = ps;
            isBanding = true;
        }

        public void clsLineRectangle_OnMouseMove(Object sender, MouseEventArgs e)
        {
            if (isBanding && e.Button == MouseButtons.Left)
            {
               // Form thisform = (Form)sender;
                PointPlotUserControl thisform = (PointPlotUserControl)sender;

                // First DrawReversible to toggle to the background color
                // Second DrawReversible to toggle to the specified color

                ControlPaint.DrawReversibleFrame(thisform.RectangleToScreen(SelectRect), Color.Black, FrameStyle.Dashed);
                int w = e.X - SelectRect.X;
                int h = e.Y - SelectRect.Y;
                double ratio = PointPlotUserControl.RATIO; //0.75;
                if (w * ratio == h)
                {
                    SelectRect.Width = w;
                    SelectRect.Height = h;
                }
                else if (w * ratio <= h)
                {
                    SelectRect.Width = (int)(h / ratio);
                    SelectRect.Height = h;
                }
                else
                {
                    SelectRect.Width = w;
                    SelectRect.Height = (int)(w * ratio);
                }
                ControlPaint.DrawReversibleFrame(thisform.RectangleToScreen(SelectRect), Color.Black, FrameStyle.Dashed);
            }
        }

        public void clsLineRectangle_OnMouseUp(Object sender, MouseEventArgs e)
        {
           // Form thisform = (Form)sender;
            PointPlotUserControl thisform = (PointPlotUserControl)sender;
            ControlPaint.DrawReversibleFrame(thisform.RectangleToScreen(SelectRect), Color.Black, FrameStyle.Dashed);
            isBanding = false;
       
        }
    }
}