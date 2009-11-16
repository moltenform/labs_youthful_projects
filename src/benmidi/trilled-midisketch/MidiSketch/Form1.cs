using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Collections;
using System.IO;
using Multimedia.Midi;
using System.Diagnostics;

namespace Midisketch
{
    public partial class Midisketch : Form
    {
        PlayingMidi plmidi;
        bool bRecording = false;
        bool bPlaying = false;

        public Midisketch()
        {
            InitializeComponent();

            this.KeyDown += new KeyEventHandler(onKeyDown);
            this.KeyUp += new KeyEventHandler(onKeyUp);
            this.Deactivate += new EventHandler(onDeactivate);

            this.KeyPreview = true; //very important. keys get sent to form, before to control w focus.

            this.plmidi = new PlayingMidi();
            mnuNewMidi_Click(null, null);
            btnRecord.Focus();
            btnStop.Enabled = false;
            
        }
        void onDeactivate(object sender, EventArgs e)
        {
            this.plmidi.stopallnotes();
        }
        private void onFormClosing(object sender, FormClosingEventArgs e)
        {
            this.plmidi.CloseDevice();
        }
        void onKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            int key = (int)e.KeyCode;

            if ((key == 38 || key == 40))
            {
                if (key == 38)
                    this.plmidi.setTransposition(true);
                else if (key == 40)
                    this.plmidi.setTransposition(false);

                txtReference.Text = this.plmidi.getTransposition();
            }
            else
            {
                bool bHandled = this.plmidi.onkey(key);
                e.Handled = bHandled;
            }
        }
        void onKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            int key = (int)e.KeyCode;
            bool bHandled = this.plmidi.onkeyup(key);
            e.Handled = bHandled;
        }





        private void refreshLoadMidi()
        {
            int nTracks = plmidi.counttracks();
            this.lblLayers.Text = "Layers: " + (nTracks - 1);
            if (nTracks <= 2)
                btnPlay.Enabled = btnRemove.Enabled = false;
            else
                btnPlay.Enabled = btnRemove.Enabled = true;
        }

        private void mnuNewMidi_Click(object sender, EventArgs e)
        {
            if (bRecording || bPlaying) return;
            string name;
            if (File.Exists("base.mid")) name = ("base.mid");
            else name = ("..\\..\\base.mid");
            int res = plmidi.loadMidiFile(name);
            Debug.Assert(res != -1);
            refreshLoadMidi();
        }
        private void mnuOpenMidi_Click(object sender, EventArgs e)
        {
            if (bRecording || bPlaying) return;
            int ntracks;
            System.Windows.Forms.OpenFileDialog openMidiFileDialog = new System.Windows.Forms.OpenFileDialog();
            openMidiFileDialog.RestoreDirectory = true;
            if (openMidiFileDialog.ShowDialog() != DialogResult.OK)
                return;
            
            string fileName = openMidiFileDialog.FileName;

            try
            {
                ntracks = plmidi.loadMidiFile(fileName);
            }
            catch (MidiFileException ex)
            {
                MessageBox.Show("Invalid midi", "could not read midi file.", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            if (ntracks == -1)
                MessageBox.Show("Currently, this program can only open midi files that it has created itself.");
            else
                refreshLoadMidi();
        }





        private void btnPlay_Click(object sender, EventArgs e)
        {
            plmidi.beginPlaying();
            btnPlay.Enabled = btnDetails.Enabled = btnRecord.Enabled = btnRemove.Enabled = false;
            btnStop.Enabled = true;
            bPlaying = true;
            btnStop.Text = "Stop playing";
        }
        private void btnRecord_Click(object sender, EventArgs e)
        {
            if (plmidi.counttracks() > 13) { MessageBox.Show("Maximum number of layers reached."); return; }
            DlgInstrument dlg = new DlgInstrument();
            if (dlg.ShowDialog() != DialogResult.OK)
                return;
            
            plmidi.beginRecording(dlg.getWhich());
            btnPlay.Enabled = btnDetails.Enabled = btnRecord.Enabled = btnRemove.Enabled = false;
            btnStop.Enabled = true;
            bRecording = true;
            btnStop.Text = "Stop recording";
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStop.Text = "Stop";
            if (bRecording && !bPlaying)
            {
                plmidi.stopRecording();
                btnPlay.Enabled = btnDetails.Enabled = btnRecord.Enabled = btnRemove.Enabled = true;
                btnStop.Enabled = false;
                refreshLoadMidi();
                bRecording = false;
            }
            else if (bPlaying && !bRecording)
            {
                plmidi.stopPlaying();
                btnPlay.Enabled = btnDetails.Enabled = btnRecord.Enabled = btnRemove.Enabled = true;
                btnStop.Enabled = false;
                bPlaying = false;
            }
            else { throw new Exception("Not executed."); }
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {
            MessageBox.Show(plmidi.getDetails());
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete recently added recording?", "Confirm Deletion", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;
            plmidi.removeLastTrack();
            refreshLoadMidi();
        }


   








        private void mnuSaveMidi_Click(object sender, EventArgs e)
        {
            if (bRecording || bPlaying) return;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.Title = "Specify Filename";
            saveFileDialog1.Filter = "Midi|*.mid";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.OverwritePrompt = true;
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            this.plmidi.saveMidiFile(saveFileDialog1.FileName);
        }
        private void mnuAbout_Click(object sender, EventArgs e)
        {
            if (bRecording || bPlaying) return;
            MessageBox.Show("MidiSketch\r\nby Ben Fisher, GPLv3 license. \r\n\r\nC# MIDI Toolkit, By Leslie Sanford (MIT license)");
        }
        private void mnuExit_Click(object sender, EventArgs e)
        {
            if (bRecording || bPlaying) return;
            this.Close();
        }

    }
}