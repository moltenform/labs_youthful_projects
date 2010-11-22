using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CsWaveAudio;

namespace BlinkbeatExperiments
{
    public partial class BlinkbeatsExperiments : Form
    {
        AudioPlayer pl;
        public BlinkbeatsExperiments()
        {
            InitializeComponent();
            pl = new AudioPlayer();
            InitVibLab();
        }

        

        

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
                InitCrFeedback();
            else if (tabControl1.SelectedIndex == 2)
                InitFreqSketch();

        }


        private static void commonLoadWaveFile(out string strFilename, out string strShortname, out WaveAudio w)
        {
            w = null;  strShortname = null;
            strFilename = getOpenFilename();
            if (strFilename == null || strFilename == "") return;
            try
            {
                w = new WaveAudio(strFilename);
            }
            catch (Exception er)
            {
                MessageBox.Show("Error when loading wave file: " + er.Message);
                return;
            }
            string[] pathsplit = strFilename.Split(new char[] { '\\' });
            strShortname = pathsplit[pathsplit.Length - 1];
        }
        private static void commonSaveWaveFile(WaveAudio w)
        {
            string strFilename = getSaveFilename();
            if (strFilename == null || strFilename == "") return;
            try
            {
                w.SaveWaveFile(strFilename);
            }
            catch (Exception er)
            {
                MessageBox.Show("Error when saving wave file: " + er.Message);
                return;
            }
        }

        private static string getOpenFilename()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Wave files (*.wav)|*.wav";
            dlg.Title = "Choose wav file...";

            if (dlg.ShowDialog() == DialogResult.OK)
                return dlg.FileName;
            else
                return null;
        }
        private static string getSaveFilename()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Wave files (*.wav)|*.wav";
            dlg.Title = "Save wav file...";

            if (dlg.ShowDialog() == DialogResult.OK)
                return dlg.FileName;
            else
                return null;
        }



      

        

        

       



        

        
    }
}