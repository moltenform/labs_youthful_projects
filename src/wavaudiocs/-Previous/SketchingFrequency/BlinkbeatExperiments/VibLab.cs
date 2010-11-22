using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using CsWaveAudio;

namespace BlinkbeatExperiments
{
    // User-interface code
    public partial class BlinkbeatsExperiments : Form
    {
        //public VibLab objVibLab;
        
        public void InitVibLab()
        {
            //this.objVibLab = new VibLab();
            vibLab_panel2.Visible = false; vibLab_btnEnablePanel2.Checked = false;
            vibLab_panel3.Visible = false; vibLab_btnEnablePanel3.Checked = false;

            vibLab_valueWidth1 = vibLab_valueWidth2 = vibLab_valueWidth3 = vibLab_valueFreq1 = vibLab_valueFreq2 = vibLab_valueFreq3 = 0.001;
            vibLab_barFreq1.Value = 50;
            vibLab_barWidth1.Value = 10;
            vibLab_updateStatus(vibLab_barFreq1, vibLab_barWidth1, vibLab_lblPanel1, ref vibLab_valueFreq1, ref vibLab_valueWidth1);
        }
        private string m_vibLab_filename;

        private void vibLab_btnEnablePanel1_CheckedChanged(object sender, EventArgs e)
        {
            vibLab_panel1.Visible = vibLab_btnEnablePanel1.Checked;
        }
        private void vibLab_btnEnablePanel2_CheckedChanged(object sender, EventArgs e)
        {
            vibLab_panel2.Visible = vibLab_btnEnablePanel2.Checked;
        }
        private void vibLab_btnEnablePanel3_CheckedChanged(object sender, EventArgs e)
        {
            vibLab_panel3.Visible = vibLab_btnEnablePanel3.Checked;
        }
        private void vibLab_btnOpen_Click(object sender, EventArgs e)
        {
            string strShortname, strFilename; WaveAudio w;
            commonLoadWaveFile(out strFilename, out strShortname, out w);
            if (w == null) { 
                this.m_vibLab_filename = null;  // set to null so play won't work.
                return; 
            }
            else
            {
                m_vibLab_filename = strFilename;
                vibLab_chkSound.Checked = true;
            }
        }

        private double vibLab_valueWidth1, vibLab_valueWidth2, vibLab_valueWidth3;
        private double vibLab_valueFreq1, vibLab_valueFreq2, vibLab_valueFreq3;

        private void vibLab_updateStatus(TrackBar barfreq, TrackBar barwidth, Label lbl, ref double vfreq, ref double vwidth)
        {
            
            vfreq = Math.Max(-1 * 2 * Math.Log((101-barfreq.Value) / 100.0),0.001); //logarithmic scale
            vwidth = (barwidth.Value / 100.0) * 4 + 0.01;
            lbl.Text = Math.Round(vfreq, 3) + " ; " + Math.Round(vwidth, 3);
        }
        private void vibLab_barFreq1_Scroll(object sender, EventArgs e)
        {
            vibLab_updateStatus(vibLab_barFreq1, vibLab_barWidth1, vibLab_lblPanel1, ref vibLab_valueFreq1, ref vibLab_valueWidth1);
        }
        private void vibLab_barFreq2_Scroll(object sender, EventArgs e)
        {
            vibLab_updateStatus(vibLab_barFreq2, vibLab_barWidth2, vibLab_lblPanel2, ref vibLab_valueFreq2, ref vibLab_valueWidth2);
        }
        private void vibLab_barFreq3_Scroll(object sender, EventArgs e)
        {
            vibLab_updateStatus(vibLab_barFreq3, vibLab_barWidth3, vibLab_lblPanel3, ref vibLab_valueFreq3, ref vibLab_valueWidth3);
        }

        private void vibLab_lblPanel1_Click(object sender, EventArgs e)
        {
            vibLab_updatePanelClick(vibLab_lblPanel1, ref vibLab_valueFreq1, ref vibLab_valueWidth1);
        }
        private void vibLab_lblPanel2_Click(object sender, EventArgs e)
        {
            vibLab_updatePanelClick(vibLab_lblPanel2, ref vibLab_valueFreq2, ref vibLab_valueWidth2);
        }
        private void vibLab_lblPanel3_Click(object sender, EventArgs e)
        {
            vibLab_updatePanelClick(vibLab_lblPanel3, ref vibLab_valueFreq3, ref vibLab_valueWidth3);
        }

        private void vibLab_updatePanelClick(Label lbl, ref double vfreq, ref double vwidth)
        {
            string strFreq = SimpleInput.AskSimpleInput("Choose frequency:", "Choose frequency:", "0.200");
            if (strFreq == null || strFreq == "") return;
            string strWidth = SimpleInput.AskSimpleInput("Choose width (amount):", "Choose width:", "0.300");
            if (strWidth == null || strWidth == "") return;
            vfreq = double.Parse(strFreq);
            vwidth = double.Parse(strWidth);

            lbl.Text = Math.Round(vfreq, 3) + " ; " + Math.Round(vwidth, 3);
        }



        private void vibLab_btnPlay_Click(object sender, EventArgs e)
        {
            WaveAudio w = viblab_Generate();
            if (w == null) return;
            pl.Play(w, true); //asynchronous
        }
        private void vibLab_btnStop_Click(object sender, EventArgs e)
        {
            pl.Stop();
        }

        private void vibLab_btnSave_Click(object sender, EventArgs e)
        {
            WaveAudio w = viblab_Generate();
            if (w == null) return;
            commonSaveWaveFile(w);
        }

        // normally one seperates ui and engine, as the other experiments do, but here I did it the fast way.
        public WaveAudio viblab_Generate()
        {
            WaveAudio w;
            // get source
            if (this.vibLab_chkSound.Checked)
            {
                if (this.m_vibLab_filename == null) { MessageBox.Show("You haven't opened a sound."); return null; }
                //I think we already loaded this. So it's kind of wasteful. Too tired to fix this though.

                w = new WaveAudio(this.m_vibLab_filename);
            }
            else
            {
                double freq = (this.vibLab_source_bar.Value / 100.0) * 440 + 100;
                w = new Sine(freq, 0.7).CreateWaveAudio(20.0);
            }
            if (vibLab_panel1.Visible)
            {
                if (vibLab_chkTrem1.Checked)
                    w = Effects.Tremolo(w, this.vibLab_valueFreq1, this.vibLab_valueWidth1);
                else
                    w = Effects.Vibrato(w, this.vibLab_valueFreq1, this.vibLab_valueWidth1);
            }
            if (vibLab_panel2.Visible)
            {
                if (vibLab_chkTrem2.Checked)
                    w = Effects.Tremolo(w, this.vibLab_valueFreq2, this.vibLab_valueWidth2);
                else
                    w = Effects.Vibrato(w, this.vibLab_valueFreq2, this.vibLab_valueWidth2);
            }
            if (vibLab_panel3.Visible)
            {
                if (vibLab_chkTrem3.Checked)
                    w = Effects.Tremolo(w, this.vibLab_valueFreq3, this.vibLab_valueWidth3);
                else
                    w = Effects.Vibrato(w, this.vibLab_valueFreq3, this.vibLab_valueWidth3);
            }

            return w;
        }


    }

    
}
