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
        private WaveAudio lastAudio = new WaveAudio();
        private ControlFeed[] controlArray;
        public Form1()
        {
            InitializeComponent();

            controlArray = new ControlFeed[] { this.controlFeed1, this.controlFeed2, this.controlFeed3, this.controlFeed4, this.controlFeed5 };

            this.presets.SelectedIndex = 0;
            this.setfields(0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WaveAudio test = new WaveAudio(44100, 1);
            test.LengthInSamples = 44100 * 7;
            double freq = 100;//300;
            PeriodicAlternative osc = new PAChangeSquare(4.0);//new PASin();
            for (int i = 0; i < test.LengthInSamples; i++)
            {
                test.data[0][i] = 0.9 * osc.GetValue(i * freq * 2.0 * Math.PI / (double)44100.0);
            }
            //player.Play(test, true);

            WaveAudio win = new WaveAudio(@"C:\Ben's Goodies\FallComparch\EffectsPedal\Effectsc#\Effects\Effects\delii.wav");
            WaveAudio wmixed = WaveAudio.Mix(WaveAudio.Modulate(test, win), 0.7, win, 0.3);
            player.Play(wmixed, true);
        }

        private void presets_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.setfields(this.presets.SelectedIndex);
        }

        private void setfields(int p)
        {
            foreach (ControlFeed feed in this.controlArray)
                feed.Config(0.0, 1.0, 0.0, 0.0, 0); //disable it by setting Mult to 0

            if (p == 0) // default, no effect
            {
                this.controlFeed1.Config(0.0, 1.0, 0.0,   1.0,   0);
            }
            else if (p == 1) // simple echo effect
            {
                this.controlFeed1.Config(0.0, 1.0, 0.0, 1.0, 0);
                this.controlFeed2.Config(0.0, 1.0, 0.05, 1.0, 0);
            }
            else if (p == 2) //  chorus effect
            {
                this.controlFeed1.Config(0.0, 1.0, 0.0, 1.0, 0);
                this.controlFeed2.Config(0.01, 0.25, 0.05, 1.0, 1);
            }
            else if (p == 3) //  flange effect
            {
                this.controlFeed1.Config(0.0, 1.0, 0.0, 1.0, 0);
                this.controlFeed2.Config(0.005, 0.5, 0.005, 1.0, 1);
            }
            else if (p == 4) //  phaser effect
            {
                this.controlFeed1.Config(0.0, 1.0, 0.0, 1.0, 0);
                this.controlFeed2.Config(0.001, 0.25, 0.001, -1, 1);
            }
        }
        private WaveAudio Go(WaveAudio win)
        {
            WaveAudio wout = new WaveAudio(44100, 1);
            wout.LengthInSamples = win.LengthInSamples;

            List<ControlFeed> activeFeeds = new List<ControlFeed>();
            foreach (ControlFeed feed in this.controlArray)
                if (!feed.IsDisabled()) activeFeeds.Add(feed);
            ControlFeed[] feeds = activeFeeds.ToArray();

            //get delays... actually not too efficient in terms of mem. use to precalc this...
            int[][] delays = new int[feeds.Length][];
            for (int i = 0; i < feeds.Length; i++)
            {
                 delays[i] = feeds[i].Go(wout.LengthInSamples);
            }

            // now do this
            for (int i = 0; i < wout.LengthInSamples; i++)
            {
                double val=0;
                for (int j = 0; j < feeds.Length; j++)
                {
                        int index = i - delays[j][i];
                        double scaleFactor = feeds[j].GetMultiply();
                        val += getSample(win, index) * scaleFactor; // !!! !! !!!!
                        
                    // weird stuff here!!!
                    //if (j==0) val += getSample(win, index) * scaleFactor;
                    //else val += (getSample(win, index) * scaleFactor) + 0.7*getSample(wout, index) * scaleFactor;
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
            WaveAudio win = new WaveAudio(@"C:\Ben's Goodies\FallComparch\EffectsPedal\Effectsc#\Effects\Effects\cismed.wav");
            //WaveAudio win = new WaveAudio(@"C:\Ben's Goodies\FallComparch\EffectsPedal\Effectsc#\Effects\Effects\delii.wav");
            WaveAudio wout = Go(win);
            player.Play(wout, true);

            lastAudio = wout;
        }

        private void btnAgain_Click(object sender, EventArgs e)
        {
            player.Play(lastAudio, true);
        }

    }
}