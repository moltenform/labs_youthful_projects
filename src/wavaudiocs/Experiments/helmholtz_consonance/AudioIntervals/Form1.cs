//Ben Fisher, 2008
//
//GPLv3 Licence

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX.DirectSound;
using AudioPad;

namespace AudioIntervals
{
    public partial class Form1 : Form
    {
        public enum Model {Holtz, Vas};

        Device deviceSound;
        SoundWaves soundWaves;
        Model currentModel;
        public Form1()
        {
            InitializeComponent();

            deviceSound = new Microsoft.DirectX.DirectSound.Device();
            deviceSound.SetCooperativeLevel(Handle, CooperativeLevel.Priority);
            soundWaves = new SoundWaves(deviceSound);

            imgHoltz.Visible = true;
            imgVas.Visible = false;
            currentModel = Model.Holtz;
        }

       

        private void play_tone(double baseFrequency, int nharmonics)
        {
            double[] freqs = new double[nharmonics];
            double[] weights = new double[nharmonics];
            for (int i = 0; i < nharmonics; i++)
            {
                freqs[i] = baseFrequency * (i + 1);
                weights[i] = 1 / (i + 1);
            }
            byte[] sumsines = soundWaves.sine_sum(freqs, weights, 0.3, 1.0);
            soundWaves.playbytes(sumsines, 16, 44100);
        }

        private void play_interval(double freq1, double freq2, int nharmonics)
        {
            double[] freqs = new double[nharmonics * 2];
            double[] weights = new double[nharmonics * 2];
            for (int i = 0; i < nharmonics; i++)
            {
                freqs[i] = freq1 * (i + 1);
                weights[i] = 1 / (i + 1);
            }
            for (int i = 0; i < nharmonics; i++)
            {
                freqs[i + nharmonics] = freq2 * (i + 1);
                weights[i + nharmonics] = 1 / (i + 1);
            }
            byte[] sumsines = soundWaves.sine_sum(freqs, weights, 0.3, 1.0);
            soundWaves.playbytes(sumsines, 16, 44100);
        }

        private void btnHelmholtz_CheckedChanged(object sender, EventArgs e)
        {
            imgHoltz.Visible = true;
            imgVas.Visible = false;
            currentModel = Model.Holtz;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            imgHoltz.Visible = false;
            imgVas.Visible = true;
            currentModel = Model.Vas;
        }

        private void imgVas_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUpResponse(e.X, e.Y);   
        }
        private void imgHoltz_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUpResponse(e.X, e.Y);
        }

        private void mouseUpResponse(int x, int y)
        {
            double freq = 400.0 + 1/1.3725 * (x - 30);
            
            if (!chkInterval.Checked)
            {
                play_tone(freq, (int)enterNharmonics.Value);
            }
            else
            {
                play_interval(400.0, freq, (int)enterNharmonics.Value);
            }
        }

        

        
    }
}