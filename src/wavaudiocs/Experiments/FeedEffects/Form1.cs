using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using CsWaveAudio;

namespace FeedEffects
{
    public partial class Form1 : Form
    {
        private AudioPlayer player = new AudioPlayer();
        private ControlFeed[] controlArray;
        public Form1()
        {
            InitializeComponent();

            this.presets.SelectedIndex = 0;
            this.setfields(0);
            controlArray = new ControlFeed[] { this.controlFeed1, this.controlFeed2, this.controlFeed3, this.controlFeed4, this.controlFeed5 };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WaveAudio test = new WaveAudio(44100, 1);
            test.LengthInSamples = 44100 * 3;
            double freq = 300;
            PeriodicAlternative osc = new PASin();//new PASin();
            for (int i = 0; i < test.LengthInSamples; i++)
            {
                test.data[0][i] = 0.9 * osc.GetValue(i * freq * 2.0 * Math.PI / (double)44100.0);
            }
            player.Play(test, true);

        }

        private void presets_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.setfields(this.presets.SelectedIndex);
        }

        private void setfields(int p)
        {
            if (p == 0) // default, no effect
            {
                this.controlFeed1.Config(0.0, 1.0, 0.0,   1.0,   0);
                this.controlFeed2.Config(0.0, 1.0, 0.0, 0.0, 0);
                this.controlFeed3.Config(0.0, 1.0, 0.0, 0.0, 0);
                this.controlFeed4.Config(0.0, 1.0, 0.0, 0.0, 0);
                this.controlFeed5.Config(0.0, 1.0, 0.0, 0.0, 0);
            }
        }
        private WaveAudio Go(WaveAudio win)
        {
            WaveAudio wout = new WaveAudio(44100, 1);
            wout.LengthInSamples = win.LengthInSamples;

            MessageBox.Show("here");
            //get delays... actually not too efficient in terms of mem. use to precalc this...
            int[][] delays = new int[this.controlArray.Length][];
            for (int i = 0; i < this.controlArray.Length; i++)
            {
                delays[i] = this.controlArray[i].Go(wout.LengthInSamples);
            }

            // now do this
            for (int i = 0; i < wout.LengthInSamples; i++)
            {
                double val=0;
                for (int j = 0; j < this.controlArray.Length; j++)
                {
                    int index = i - delays[j][i];
                    double scaleFactor = this.controlArray[j].GetMultiply();
                    val += getSample(win, index) * scaleFactor;
                }
                wout.data[0][i] = val;
            }


            return wout;
        }
        private double getSample(WaveAudio w, int i)
        {
            if (i < 0) return 0;
            if (i >= w.data[0].Length) return 0;
            return w.data[0][i];
        }

        private void btnTry_Click(object sender, EventArgs e)
        {
            WaveAudio win = new WaveAudio(@"C:\Ben's Goodies\FallComparch\EffectsPedal\Effectsc#\Effects\Effects\cissmall.wav");
            WaveAudio wout = Go(win);
            player.Play(wout, true);

        }

    }
}