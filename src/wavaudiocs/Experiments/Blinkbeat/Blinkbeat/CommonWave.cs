using System;
using System.Collections.Generic;
using System.Text;
using CsWaveAudio;
using System.Windows.Forms;

namespace Blinkbeat
{
    internal static class CommonWave
    {
        internal static void commonLoadWaveFile(out string strFilename, out string strShortname, out WaveAudio w)
        {
            w = null; strShortname = null;
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
        internal static void commonSaveWaveFile(WaveAudio w)
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

        internal static string getOpenFilename()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Wave files (*.wav)|*.wav";
            dlg.Title = "Choose wav file...";

            if (dlg.ShowDialog() == DialogResult.OK)
                return dlg.FileName;
            else
                return null;
        }
        internal static string getSaveFilename()
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
