using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CsWaveAudio;
using System.IO;

namespace Blinkbeat
{
    public partial class Form1 : Form
    {
        private WaveAudio m_currentSound;
        private bool savedNum, savedScroll, savedCaps;
        private AudioPlayer m_audioPlayer;

        private double[][] m_fourierResults;
        private int m_currentIndex = 0;
        private double m_normalizeFactor;
        private double m_blinkThresholdFactor;


        public Form1()
        {
            InitializeComponent();
            this.m_audioPlayer = new AudioPlayer();

            this.savedNum = SetKeyState.GetNumState();
            this.savedScroll = SetKeyState.GetScrollState();
            this.savedCaps = SetKeyState.GetCapsState();

            if (File.Exists(@"..\..\cis.wav"))
                this.m_currentSound = new WaveAudio(@"..\..\cis.wav");
            else if (File.Exists(@"cis.wav"))
                this.m_currentSound = new WaveAudio(@"cis.wav");

            if (m_currentSound != null)
            {
                this.m_currentSound.setNumChannels(1, true);
                this.lblFilename.Text = "Loaded: cis.wav";
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            string strFilename, strShortname; WaveAudio w;
            CommonWave.commonLoadWaveFile(out strFilename, out strShortname, out w);
            if (w==null)
                return;
            this.lblFilename.Text = "Loaded: " + strShortname;
            w.setNumChannels(1, true); //convert to mono
            this.m_currentSound = w;
        }


        private void setMeters(double[] values, double normFactor)
        {
            const int METERL = 400;
            if (values.Length != 8) throw new Exception("must be 8 vals");
            for (int i = 0; i < values.Length; i++)
            {
                int val = (int)(values[i] / normFactor * METERL);
                if (val > METERL) val = METERL;
                else if (val < 0) val = 0;
                getMeter(i).Width = val;
            }
        }
        private Button getMeter(int i)
        {
            switch (i)
            {
                case 0: return this.btnBar1;
                case 1: return this.btnBar2;
                case 2: return this.btnBar3;
                case 3: return this.btnBar4;
                case 4: return this.btnBar5;
                case 5: return this.btnBar6;
                case 6: return this.btnBar7;
                case 7: return this.btnBar8;
                default: throw new Exception("no meter");
            }
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            if (this.m_currentSound == null) { MessageBox.Show("No sound loaded."); return; }

            int timerInterval;
            Blinkbeat.CalculateBeatBlink(this.m_currentSound, out this.m_fourierResults, out this.m_normalizeFactor, out this.m_blinkThresholdFactor, out timerInterval);
            this.timerPulse.Interval = timerInterval;

            this.m_currentIndex = 0;
            this.timerPulse_Tick(null, null); //first step
            m_audioPlayer.Play(this.m_currentSound, true); //play asynchronously

            // go!
            this.timerPulse.Enabled = true;
        }

        private void timerPulse_Tick(object sender, EventArgs e)
        {
            if (this.m_currentIndex >= this.m_fourierResults.Length)
            {
                SetKeyState.SetNumState(false);
                SetKeyState.SetScrollState(false);
                SetKeyState.SetCapsState(false);
                this.timerPulse.Enabled = false;
            }
            else
            {
                SetKeyState.SetNumState((this.m_fourierResults[this.m_currentIndex][0] > m_blinkThresholdFactor));
                SetKeyState.SetCapsState((this.m_fourierResults[this.m_currentIndex][1] > m_blinkThresholdFactor));
                SetKeyState.SetScrollState((this.m_fourierResults[this.m_currentIndex][2] > m_blinkThresholdFactor));

                this.setMeters(this.m_fourierResults[this.m_currentIndex], this.m_normalizeFactor);
                this.m_currentIndex++;

                this.lblStatus.Text = this.m_currentIndex.ToString();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            SetKeyState.SetNumState(this.savedNum);
            SetKeyState.SetScrollState(this.savedScroll);
            SetKeyState.SetCapsState(this.savedCaps);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (this.m_currentSound == null) { MessageBox.Show("No sound loaded."); return; }
            this.m_audioPlayer.Stop();
            this.timerPulse.Enabled = false;
        }

        private void btnBPM_Click(object sender, EventArgs e)
        {
            if (this.m_currentSound == null) { MessageBox.Show("No sound loaded."); return; }
            double bpmGuess = Fourier.GuessBpm(this.m_currentSound);
            this.lblBpm.Text = Math.Round(bpmGuess, 3).ToString();
        }



        // Drag and drop doesn't seem to work now.
        private void unused_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
                e.Effect = DragDropEffects.All;

            // otherwise, don't do anything.
        }

        private void unused_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length == 0) return;
            string strFirstFile = files[0];
            if (!strFirstFile.ToLowerInvariant().EndsWith(".wav"))
            {
                MessageBox.Show("Drag a standard .wav file into this form.");
                return;
            }
            // ... load the wave ...
        }



    }
}