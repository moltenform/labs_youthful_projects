using System;
using System.Windows.Forms;
using CsWaveAudio;
using System.IO;

namespace BlinkbeatExperiments
{
    // User-interface code
    public partial class BlinkbeatsExperiments : Form
    {
        public CrFeedback objCrFeedback;
        private void CrFeedback_onbtnOpen(object sender, EventArgs e)
        {
            string strShortname, strFilename; WaveAudio w;
            commonLoadWaveFile(out strFilename, out strShortname, out w);
            if (w==null) return;
            this.objCrFeedback.SetAudioFile(w);
            this.objCrFeedback.SetNeedUpdate();
            this.crFeedbackLblWavFile.Text = strShortname;

            this.crFeedback_btnPlay.Enabled = true;
            
        }
        private void crFeedback_onBtnPlay(object sender, EventArgs e)
        {
            if (this.crFeedback_btnPlay.Text == "Play")
            {
                WaveAudio w = this.objCrFeedback.Generate(double.Parse(this.crFeedbackTxtScale.Text), int.Parse(this.crFeedbackTxtShift.Text));
                this.pl.Play(w, true); //play asynchronously
                this.crFeedback_btnPlay.Text = "Stop";
            }
            else
            {
                this.pl.Stop();
                this.crFeedback_btnPlay.Text = "Play";
            }
        }
        private void crFeedback_onBtnSave(object sender, EventArgs e)
        {
            commonSaveWaveFile(this.objCrFeedback.Generate(double.Parse(this.crFeedbackTxtScale.Text), int.Parse(this.crFeedbackTxtShift.Text)));
        }
        public void InitCrFeedback()
        {
            if (this.objCrFeedback != null) return; //we've already loaded this.
            
            this.objCrFeedback = new CrFeedback();
            
            this.crFeedbackTxtScale.Text = "0.915"; crFeedbackSclBar.Value = 91;
            this.crFeedbackTxtShift.Text = "300"; crFeedbackShiftBar.Value = 50;
            
            // look for demo 
            string strDemoLoc = null;
            if (File.Exists("cisco.wav"))
                strDemoLoc = "cisco.wav";
            if (File.Exists("..\\..\\samples\\cisco.wav"))
                strDemoLoc = "..\\..\\samples\\cisco.wav";

            if (strDemoLoc != null)
            {
                this.objCrFeedback.SetAudioFile(new WaveAudio(strDemoLoc));
                this.crFeedbackLblWavFile.Text = "cisco.wav";
                this.crFeedback_btnPlay.Enabled = true;
            }
            else
            {
                this.crFeedback_btnPlay.Enabled = false;
                this.CrFeedback_btnOpen.Focus();
            }
        }
        private void crFeedbackSclBar_Scroll(object sender, EventArgs e)
        {
            double val = (crFeedbackSclBar.Value/(100.0)) * 0.5 + 0.5; //from 0.5 to 1.0
            crFeedbackTxtScale.Text = val.ToString();
        }

        private void crFeedbackShiftBar_Scroll(object sender, EventArgs e)
        {
            int val = (int) ((crFeedbackShiftBar.Value/(100.0)) * 600 + 1);
            crFeedbackTxtShift.Text = val.ToString();
        }
    }

    // Actual generation code
    public class CrFeedback
    {
        private double timeScaleLast; private int timeShiftLast; // remember previous calculation.
        private bool bNeedUpdate = true;
        private WaveAudio wLast;
        private WaveAudio wInput;
        public void SetAudioFile(WaveAudio w)
        {
            this.wInput = w;
        }

        public WaveAudio Generate(double timeScale, int timeShift)
        {
            if (wLast != null && timeScale == this.timeScaleLast && timeShift == this.timeShiftLast && !bNeedUpdate)
                return wLast;

            WaveAudio w = wInput.Clone();
            for (int ch = 0; ch < w.getNumChannels(); ch++)
            {
                for (int i = 0; i < w.data[ch].Length; i++)
                {
                    w.data[ch][i] = w.data[ch][i] + w.data[ch][((int)(i * timeScale) + timeShift) % w.data[ch].Length];
                }
            }
            this.timeScaleLast = timeScale;
            this.timeShiftLast = timeShift;
            this.wLast = w;
            this.bNeedUpdate = false;
            return w;
        }


        public void SetNeedUpdate()
        {
            this.bNeedUpdate = true;
        }
    }
}
