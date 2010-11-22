using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using CsWaveAudio;

namespace BlinkbeatExperiments
{
    // User-interface code
    public partial class BlinkbeatsExperiments : Form
    {
        public FreqSketch objFreqSketch;
        public void InitFreqSketch()
        {
            this.objFreqSketch = new FreqSketch();
        }

        private void getFrequenciesFromInputImage(out double[] freqsOut, out double[] weightsOut)
        {
            string strImagelocation = null; freqsOut = null; weightsOut = null;
            if (File.Exists("input.png"))
                strImagelocation = "input.png";
            else if (File.Exists("..\\..\\input.png"))
                strImagelocation = "..\\..\\input.png";
            if (strImagelocation == null)
            {
                MessageBox.Show("Could not find input.png. Please place this in the same directory.");
                return;
            }
            Bitmap bmp = new Bitmap(strImagelocation);
            int width = bmp.Width;
            int height = bmp.Height;
            if (width != 1024) { MessageBox.Show("Width of input image is not 1024."); return; }
            double downscale = 1 / ((double)(width));

            List<double> freqs = new List<double>(40);
            List<double> weights = new List<double>(40);

            Color tcolor = Color.FromArgb(255, 0, 0);
            int x, y;
            for (x = 0; x < width; x++) //less efficent to loop this way but we don't really have a choice
            {
                bool bFound = false;
                for (y = 0; y < height; y++)
                {
                    if (bmp.GetPixel(x, y) == tcolor)
                    { bFound = true; break; }
                }
                if (bFound) 
                { 
                    freqs.Add ( 18.75 * Math.Pow(2.0, x/(((double)width)/5.0))); // convert width to frequency
                    weights.Add(  (height-y) * 0.01); //convert height to amplitude 
                }
            }
            bmp.Dispose();
            freqsOut = freqs.ToArray();
            weightsOut = weights.ToArray();
            this.sketchFreq_lblStatus.Text = freqsOut.Length + " data points.";
        }

        private WaveAudio getWaveAudio()
        {
            double[] freqs, weights;
            getFrequenciesFromInputImage(out freqs, out weights);
            if (freqs != null)
            {
                this.freqSketch_btnPlay.Enabled = false;
                this.freqSketch_btnPlay.Invalidate();
                WaveAudio res = this.objFreqSketch.Generate(freqs, weights, (double)this.freqSketch_flipSeconds.Value, this.sketchFreq_chkFFT.Checked, this.sketchFreq_chkRandomPhases.Checked);
                this.freqSketch_btnPlay.Enabled = true;
                return res;
            }
            else
                return null;
        }

        private void freqSketch_OnBtnPlay(object sender, EventArgs e)
        {
            // no way to stop during playback, but that's ok, it was interfering with user
                WaveAudio w = getWaveAudio();
                if (w == null) return;
                this.pl.Play(w, true); //play asynchronously
                this.crFeedback_btnPlay.Text = "Stop";
        }
        private void freqSketch_onBtnSave(object sender, EventArgs e)
        {
            WaveAudio w = getWaveAudio();
            if (w == null) return;
            commonSaveWaveFile(w);
        }
    }

    public class FreqSketch
    {
        class SineWaveSumNormalizeAfterwardsRandomPhase : SineWaveSumNormalizeAfterwards
        {
            double[] rphases;
            public SineWaveSumNormalizeAfterwardsRandomPhase(double[] freqs, double[] weights, double amp)
                : base(freqs, weights, amp)
            {
                Random r = new Random();
                rphases = new double[freqs.Length];
                for (int i = 0; i < rphases.Length; i++) rphases[i] = r.NextDouble() * 2 * Math.PI;
            }
            protected override void sumSineWaves(double[] timescales, double[] outData)
            {
                for (int i = 0; i < outData.Length; i++)
                {
                    for (int w = 0; w < timescales.Length; w++) //each has its own phase
                        outData[i] += amplitude * Math.Sin(i * timescales[w] + rphases[w]) * weights[w];
                    // don't clip this yet.
                }
            }
        }

        // it'd be hard to keep track of if anything changed (without checking timestamp)
        public WaveAudio Generate(double[] freqs, double[] weights, double fSeconds, bool bUseFFT, bool bRandomPhases)
        {
            WaveAudio result;
            if (!bUseFFT)
            {
                if (bRandomPhases)
                {
                    SineWaveSumNormalizeAfterwardsRandomPhase synth = new SineWaveSumNormalizeAfterwardsRandomPhase(freqs, weights, 0.7);
                    result = synth.CreateWaveAudio(fSeconds);
                }
                else
                {
                    SineWaveSumNormalizeAfterwards synth = new SineWaveSumNormalizeAfterwards(freqs, weights, 0.7);
                    result = synth.CreateWaveAudio(fSeconds);
                }
            }
            else
            {
                int len = 1024 * 16; // and interpolate more? As of now we only have per/pixel resolution.
                double[] imgarr = new double[len]; double[] realarr = new double[len];
                Random r = null; if (bRandomPhases) r= new Random();

                double dScaledown = (len / 2) / (44100.0 / 2); // map 0-22050.0 to 0-512
                for (int i = 0; i < freqs.Length; i++)
                {
                    int index = (int) (freqs[i] * dScaledown);
                    double amplitude = weights[i] * 200;
                    if (!bRandomPhases)
                    { realarr[index] += amplitude; /*realarr[len - index] += amplitude;*/ }
                    else
                    {
                        double phase = r.NextDouble() * 2.0 * Math.PI;
                        realarr[index] += amplitude * Math.Cos(phase); //realarr[len - index] += amplitude * Math.Cos(phase);
                        imgarr[index] += amplitude * Math.Sin(phase); //imgarr[len - index] += amplitude * Math.Sin(phase);
                    }
                    // and, add the mirror for negative frequency
                }

                double[] samples = new double[len];
                Fourier.RawFrequencyToSamples(out samples, realarr, imgarr);
                // now I guess repeat these samples for some time...

                // hacky estimate of time, instead of real math
                int nRepeats = (int)(fSeconds / (len / 44100.0));
                result = new WaveAudio(1);
                result.LengthInSamples = nRepeats * len;
                for (int b=0; b<nRepeats; b++)
                    Array.Copy(samples, 0, result.data[0], b*samples.Length, samples.Length);
                
            }
            return result;
        }
    }
}
