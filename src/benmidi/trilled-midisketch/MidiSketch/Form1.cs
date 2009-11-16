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
            //this.use
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

        

      

       /*
        private void DoneRecording()
        {
            //Add unended notes.
            foreach (DictionaryEntry de in this.awaitingend)
            {
                this.recordTicks.Add(midiFilePlayer1.position);
                this.recordNotes.Add(-1 * ((int)de.Key));
            }
            int lasttime = midiFilePlayer1.position;
            //Clear the hashtable
            this.awaitingend = new Hashtable();

            //Now add to the track that was already created!
            int nchannel = this.listTracks.SelectedIndex; //this.midiFilePlayer1.sequence.tracks[0].Count + 1;
            int ntrack = this.listTracks.SelectedIndex;

            IEnumerator enote = recordNotes.GetEnumerator();
            IEnumerator entick = recordTicks.GetEnumerator();
            while (enote.MoveNext())
            {
                entick.MoveNext();

                if ((int)enote.Current > 0) //Noteon
                {
                    builder.Command = ChannelCommand.NoteOn;
                    builder.MidiChannel = nchannel;
                    builder.Data1 = (int)enote.Current;
                    builder.Data2 = 127;
                    builder.Build();
                }
                else if ((int)enote.Current < 0)
                {
                    builder.Command = ChannelCommand.NoteOff;
                    builder.MidiChannel = nchannel;
                    builder.Data1 = (int)enote.Current * -1;
                    builder.Data2 = 0;
                    builder.Build();
                }
                this.midiFilePlayer1.sequence.tracks[ntrack].Insert((int)entick.Current, builder.Result);
            }

           
        }
        private int GetTrackChannel(Track t)
        {
             byte[] a = t.GetMidiEvent(1).MidiMessage.GetBytes();
            foreach( byte byt in a)
            {
                MessageBox.Show(byt.ToString());
            }
             return 3;
        }
        private void LoadNewMidi()
        {
            this.listTracks.Items.Clear();

            List<Track> ttracks = midiFilePlayer1.sequence.tracks;
            int ctracks = ttracks.Count;
            for (int i = 0; i < ctracks; i++)
            {
               // if ((i == 1 && ttracks[i].Count == 4 && ttracks[i].Length == 40 * 96) || (i == 2 && ttracks[i].Count == 4 && ttracks[i].Length == 40 * 96) || (i == 3 && ttracks[i].Count == 6 && ttracks[i].Length == 40 * 96))
               //     continue;
                
                string label = "Track " + (i + 1).ToString() + " : " + ttracks[i].Count.ToString() + " Events : " + ((int)(((double)ttracks[i].Length) / 96.0)).ToString() + " Beats Long";
                this.listTracks.Items.Add(label); //It's the index that counts.
            }
            this.listTracks.SelectedIndex = 0;
        }


        //Sequence Functions

        private void NewSequence()
        {
            if (File.Exists("base.mid")) midiFilePlayer1.Open("base.mid");
            else midiFilePlayer1.Open("..\\..\\base.mid");
            LoadNewMidi();
        }
        private void btnNewSequence_Click(object sender, EventArgs e)
        {
            NewSequence();
        }
        private void btnOpen_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openMidiFileDialog = new System.Windows.Forms.OpenFileDialog();
            if (openMidiFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openMidiFileDialog.FileName;

                try
                {
                    midiFilePlayer1.Open(fileName);
                }
                catch (MidiFileException ex)
                {
                    MessageBox.Show("Error", "Not valid Midi?", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                recording = false;
            }
            LoadNewMidi();
        }
        private void btnSaveMIDI_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog1.Title = "Specify Filename";
            saveFileDialog1.Filter = "Midi|*.mid";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.OverwritePrompt = true;
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            midiFilePlayer1.sequence.Save(saveFileDialog1.FileName);

        }




        ////////////////////////////////////////////////////////////////////////////
        //Track Functions

        private void btnNewTrack_Click(object sender, EventArgs e)
        {
            int ins = parseInstrument();
            if (ins == -1) ins = 0; //If invalid, default to piano

            Track newtrack = new Track();
            //Warning !! Hack ahead !! Bound to fail !!
           // int nchannel = //GetTrackChannel(this.midiFilePlayer1.sequence.tracks[2]);// 
            int nchannel = this.midiFilePlayer1.sequence.tracks[0].Count + 1;

            //Set instrument
            builder.Command = ChannelCommand.ProgramChange;
            builder.MidiChannel = nchannel;
           builder.Data1 = ins;
           builder.Build();
            newtrack.Insert(0, builder.Result);

            //Important part - send this baby to the output device.
            outDevice.Send(builder.Result);

            this.midiFilePlayer1.sequence.Add(newtrack);
            LoadNewMidi(); //Refresh track listing
        }

        private void btnDeleteTrack_Click(object sender, EventArgs e)
        {
            //Only works on files built with this program.
            int track = this.listTracks.SelectedIndex;
            if (track<=0) { MessageBox.Show("You must select a track."); return; }
           // track += 3;
            this.midiFilePlayer1.sequence.Remove(this.midiFilePlayer1.sequence.tracks[track]);
            this.LoadNewMidi();
        }

        private void txtInstrument_TextChanged(object sender, EventArgs e)
        {
            if (parseInstrument() != -1)
                labelInstrument.Text = MidiNoteConverter.instrumentnames[parseInstrument()];

        }
        private int parseInstrument()
        {
            if (txtInstrument.Text.Length != 0)
            {
                int ins = Int32.Parse(txtInstrument.Text);
                if (ins <= 127 && ins >= 0)
                    return ins;
            }
            return -1;
        }



///Recording//////////////////////////////////////////////////

        private void btnRecord_Click_1(object sender, EventArgs e)
        {
            int track = this.listTracks.SelectedIndex;
            if (track <= 0) { MessageBox.Show("You must select a track."); return; }

            DisableSaveLoadEtc(false);

       
            btnRecord.BackColor = Color.DarkOrange;
            this.recording = true;

            recordTicks = new ArrayList();
            recordNotes = new ArrayList();

            //Start the playback too now
            this.midiFilePlayer1.Start();
        }
        private void DisableSaveLoadEtc(bool bwhich)
        {
            this.btnOpenSequence.Enabled = bwhich;
            this.btnNewSequence.Enabled = bwhich;
            this.btnSaveSequence.Enabled = bwhich;
            this.btnDeleteTrack.Enabled = bwhich;
            this.btnNewTrack.Enabled = bwhich;

            this.listTracks.Enabled = bwhich;
            this.btnRecord.Enabled = bwhich;
            this.btnPlay.Enabled = bwhich;
        }


       
        private void StopPlaying()
        {
            DisableSaveLoadEtc(true);
            if (this.recording == false)
            {
             
            }
            else
            {
                midiFilePlayer1.Stop();
                btnRecord.BackColor = Color.White;
                if (MessageBox.Show("Add the new notes to the sequence?", "Confirm Record", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                     DoneRecording();
                }
                this.recording = false;
            }
        }
        */

    }
}