using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CsWaveAudio;
using System.Diagnostics;

namespace Dropler
{
    public partial class Form1 : Form
    {
        public double fWidth = 1000.0, fHeight = 1000.0;
        int curX, curY;
        Dictionary<Button, bool> isDragging = new Dictionary<Button,bool>();
        Button[] btnCheckpoints0;
        Button[] btnCheckpoints1;
        AudioPlayer pl  = new AudioPlayer();
        double MaxTimeSeconds = 6.0;
        int NMarkers = 100;
        public Form1()
        {
            InitializeComponent();
            //

            this.p0.Visible = false;
            this.p1.Visible = false;

            btnCheckpoints0 = new Button[] { btnPt0_0, btnPt0_1, btnPt0_2, btnPt0_3, btnPt0_4 };
            btnCheckpoints1 = new Button[] { btnPt1_0, btnPt1_1, btnPt1_2, btnPt1_3, btnPt1_4 };
            Button[] otherDraggables = new Button[] { btnL, btnR };
            
            // make them draggable
            foreach (Button b in btnCheckpoints0)
            {
                b.MouseDown += new MouseEventHandler(btnDrag_MouseDown);
                b.MouseUp += new MouseEventHandler(btnDrag_MouseUp);
                b.MouseMove += new MouseEventHandler(btnDrag_MouseMove);
            }
            foreach (Button b in btnCheckpoints1)
            {
                b.MouseDown += new MouseEventHandler(btnDrag_MouseDown);
                b.MouseUp += new MouseEventHandler(btnDrag_MouseUp);
                b.MouseMove += new MouseEventHandler(btnDrag_MouseMove);
            }
            foreach (Button b in otherDraggables)
            {
                b.MouseDown += new MouseEventHandler(btnDrag_MouseDown);
                b.MouseUp += new MouseEventHandler(btnDrag_MouseUp);
                b.MouseMove += new MouseEventHandler(btnDrag_MouseMove);
            }
            chk0.Checked = true;
            chk1.Checked = false; chk1_CheckedChanged(null, null);
            this.Height = 500; this.Width = 500;
            initMarkers();

            lbNew0.Visible = lbNew1.Visible = tbNew.Visible = false;
        }

        void btnDrag_MouseMove(object sender, MouseEventArgs e)
        {
            Button b = sender as Button;
            if (isDragging.ContainsKey(b) && isDragging[b]) 
            {
                b.Top = b.Top + (e.Y - curY);
                b.Left = b.Left + (e.X - curX); 
            }
        }

        void btnDrag_MouseUp(object sender, MouseEventArgs e)
        {
            Button b = sender as Button;
            isDragging[b] = false;
        }

        void btnDrag_MouseDown(object sender, MouseEventArgs e)
        {
            curX = e.X; curY = e.Y;
            Button b = sender as Button;
            isDragging[b] = true;
            hideMarkers();
        }

        

        

        




        private void btnGo_Click(object sender, EventArgs e)
        {
            WaveAudio wout = fullcompute();
           if (wout!=null)
               pl.Play(wout, true);
        }

        private WaveAudio fullcompute()
        {
            double f0, f1;
            try { f0 = double.Parse(this.txtf00.Text); f1 = double.Parse(this.txtf01.Text); }
            catch (FormatException ee) { MessageBox.Show("Not a number."); return null; }
            WaveAudio wout;
            WaveAudio win0 = new ElectricOrgan(f0, 0.5).CreateWaveAudio(MaxTimeSeconds+2);
            WaveAudio win1 = new ElectricOrgan(f1, 0.5).CreateWaveAudio(MaxTimeSeconds+2);

            if (chk0.Checked && chk1.Checked)
            {
                WaveAudio wout0 = compute(win0, btnCheckpoints0);
                WaveAudio wout1 = compute(win1, btnCheckpoints1);
                wout = WaveAudio.Mix(wout0, wout1);
            }
            else if (chk0.Checked && !chk1.Checked)
                wout = compute(win0, btnCheckpoints0);
            else if (!chk0.Checked && chk1.Checked)
                wout = compute(win1, btnCheckpoints1);
            else
                return null;
            return wout;
        }

        private WaveAudio compute(WaveAudio wSource, Button[] checkpoints)
        {
            int nCheckpoints = checkpoints.Length;
            double timeElapsedPerCheckpoint = MaxTimeSeconds/nCheckpoints;
            int nStepsPerCheckpoint = 200;

            WaveAudio wout = new WaveAudio(44100, 2);
            wout.LengthInSeconds = timeElapsedPerCheckpoint * nCheckpoints + 1;

            // build spline interpolation. t is 0.0,1.0,2.0,...
            double[] splineInputTime = new double[nCheckpoints];
            for (int i = 0; i < nCheckpoints; i++) splineInputTime[i] = (double)i;
            double[] splineInputX = new double[nCheckpoints];
            for (int i = 0; i < nCheckpoints; i++) splineInputX[i] = pixelToX(checkpoints[i].Left);
            double[] splineInputY = new double[nCheckpoints];
            for (int i = 0; i < nCheckpoints; i++) splineInputY[i] = pixelToY(checkpoints[i].Top);
            double[] splineParamX = null, splineParamY=null;
            alglib.spline3.buildcubicspline(splineInputTime, splineInputX, nCheckpoints, 0, 0, 0, 0, ref splineParamX);
            alglib.spline3.buildcubicspline(splineInputTime, splineInputY, nCheckpoints, 0, 0, 0, 0, ref splineParamY);

            // initial values
            double x, y, max = 0;
            double dt = timeElapsedPerCheckpoint / nStepsPerCheckpoint;
            double V = 340.0; //speed of sound
            double prevdistance, distance;
            double t = 0;
            double tInc = 1.0 / nStepsPerCheckpoint;

            for (int ch = 0; ch < 2; ch++) //for left and right channels
            {
                double fPositionInSourceAudio = 0.0, xMe, yMe; 
                int iSampleOut = 0;
                if (ch == 0) { xMe = pixelToX(btnL.Left); yMe = pixelToY(btnL.Top); }
                else  { xMe = pixelToX(btnR.Left); yMe = pixelToY(btnR.Top); }
                
                t = 0;
                x = splineInputX[0]; y = splineInputY[0];
                prevdistance = distance = Math.Sqrt((x-xMe)*(x-xMe) + (y-yMe)*(y-yMe));

                for (int i = 0; i < (nCheckpoints-1)*nStepsPerCheckpoint; i++)
                {
                    x = alglib.spline3.splineinterpolation(ref splineParamX, t);
                    y = alglib.spline3.splineinterpolation(ref splineParamY, t);
                    distance = Math.Sqrt((x-xMe)*(x-xMe) + (y-yMe)*(y-yMe));
                    double vS = (distance - prevdistance) / dt;
                    double freqShift = (V / (V + vS));

                    if (i % 40 == 0) addMarker(x, y);

                    //todo: consider different intensity scaling. Used to be 1/r.
                    //this is where tbNew would be used. vary intensity between (1/r) and (1/(r^3)).
                    double intensity = (distance != 0) ? (1 / (Math.Pow(distance,1.5))) : 10;
                    for (int j = 0; j < dt * 44100; j++)
                    {
                        double val = intensity * getInterpolatedValue(wSource.data[0], fPositionInSourceAudio);
                        if (val > max) max = val;
                        wout.data[ch][iSampleOut++] = val;
                        fPositionInSourceAudio += freqShift;
                    }

                    prevdistance = distance;
                    t += tInc;
                }
            }
            //scale everything to maximize volume
            for (int ch = 0; ch < 2; ch++)
                for (int i = 0; i < wout.data[ch].Length; i++)
                    wout.data[ch][i] /= max;

            return wout;
        }



        private void chk0_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Button b in this.btnCheckpoints0) 
                b.Visible = chk0.Checked;
        }
        private void chk1_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Button b in this.btnCheckpoints1)
                b.Visible = chk1.Checked;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            string sfilename = getSaveFilename();
            if (sfilename != null)
            {
                WaveAudio wout = this.fullcompute();
                if (wout!=null) wout.SaveWaveFile(sfilename);
            }
        }


        // Util methods.

        // Uses linear interpolation to find the 4.75th element of an array of doubles and so on.
        // In the future maybe consider better interpolation methods, like cubic, polynomial, or sinc
        private double getInterpolatedValue(double[] sampleData, double sampleIndex)
        {
            if (sampleIndex > sampleData.Length - 1) sampleIndex = sampleData.Length - 1;
            else if (sampleIndex < 0 + 1) sampleIndex = 0;

            double proportion = sampleIndex - Math.Truncate(sampleIndex);
            double v1 = sampleData[(int)Math.Truncate(sampleIndex)];
            double v2 = sampleData[(int)Math.Ceiling(sampleIndex)];
            return v2 * proportion + v1 * (1 - proportion);
        }
        private int xToPixel(double x)
        {
            return (int)((x / fWidth) * this.Width);
        }
        private double pixelToX(int xpixel)
        {
            return (xpixel / (double)this.Width) * fWidth;
        }
        private int yToPixel(double y)
        {
            return (int)((y / fHeight) * this.Height);
        }
        private double pixelToY(int ypixel)
        {
            return (ypixel / (double)this.Height) * fHeight;
        }
        private static string getSaveFilename()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Wave files (*.wav)|*.wav";
            dlg.Title = "Save wav file...";
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == DialogResult.OK)
                return dlg.FileName;
            else
                return null;
        }

        // "markers" show partical trajectory

        bool markersVis = false;
        Button[] markers;
        int nMarker = 0;
        private void initMarkers()
        {
            markers = new Button[NMarkers];
            Button p0;
            for (int i = 0; i < NMarkers; i++)
            {
                p0 = new System.Windows.Forms.Button();
                p0.BackColor = System.Drawing.Color.Green;
                p0.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                p0.ForeColor = System.Drawing.Color.Green;
                p0.Margin = new System.Windows.Forms.Padding(0);
                p0.Name = "p0";
                p0.Size = new System.Drawing.Size(8, 8);
                p0.Text = " ";
                p0.UseVisualStyleBackColor = false;
                p0.Visible = false;
                markers[i] = p0;
                this.Controls.Add(p0);
            }
        }
        private void addMarker(double x, double y)
        {
            markersVis = true;
            markers[nMarker].Location = new System.Drawing.Point(xToPixel(x), yToPixel(y));
            markers[nMarker].Visible = true;
            nMarker++;
            if (nMarker > NMarkers - 1) nMarker = 0;
        }
        private void hideMarkers()
        {
            if (markersVis)
                foreach (Button m in markers)
                    m.Visible = false;
            markersVis = false;
        }
    }
}