//Ben Fisher, 2008
//halfhourhacks.blogspot.com
//GPLv3 Licence

using System;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace AudioPad
{
    public partial class WaveformPlot : UserControl
    {

        public Color backgroundColor = Color.Black;
        public Color foregroundColor = Color.WhiteSmoke;


        private byte[] waveformData;
        private Point[] pts;
        private int nptsLength;
        private int nBits;
        private int byteslength;

        public WaveformPlot()
        {
            InitializeComponent();

            // When first creating the class, use demo data: 100 samples, 8bit, all are 128.
            waveformData = new byte[100];
            for (int i = 0; i < 100; i++)
                waveformData[i] = 128;

            nBits = 8;
            byteslength = 100;
        }


        public void SetCurve(byte[] waveformDataIn, int byteslengthIn, int nBitsIn)
        {
            this.waveformData = waveformDataIn;
            this.byteslength = byteslengthIn;
            this.nBits = nBitsIn;

            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;

            // shade the background
            graphics.FillRectangle(new SolidBrush(backgroundColor), 0, 0, Width, Height);

            // draw outline
            graphics.DrawRectangle(new Pen(Color.Gray, 2), 2, 2, Width - 2, Height - 2);

            int bytesPerSample = (nBits==8)?1:2;
            int nSamples = byteslength / bytesPerSample;
            if (nSamples != nptsLength)
            {
                pts = new Point[nSamples];
                nptsLength = nSamples;
            }

            // add points
            double fx, fy;            
            for (int i = 0; i < nSamples; i++)
            {
                 fx = (i / (double) nSamples);
                 if (nBits == 8)
                 {
                     fy = waveformData[i]/((double)255) - 0.5; //0 centered, -.5 to .5
                 }
                 else
                 {
                     short b1 = waveformData[i * 2];
                     short b2 = waveformData[i * 2+1];
                     short val = (short)(b1 | ((short)b2 << 8)); // Intentional: This can go negative, intended effect.
                     fy = (val / (double) (short.MaxValue))/2.0; //0 centered, -.5 to .5
                     
                 }
                 int nx = (int)(fx * Width);
                 int ny = Height - (int)(fy * Height + Height/2);
                 pts[i] = new Point(nx, ny); 
            }

            // draw curve
            Pen pen = new Pen(foregroundColor, 1);
            graphics.DrawLines(pen, pts); // presumably faster than many drawline calls
        }



    }
}
