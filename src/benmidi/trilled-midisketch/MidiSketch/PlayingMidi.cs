using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Multimedia.Midi;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

//todo: if change transposition while holding note, that note is sustained.

namespace VivaceOne
{
    public class PlayingMidi
    {
        string cfgname = "config.txt";

        private MidiFilePlayer midiFilePlayer1;
        private MidiFilePlayer tmpmidiFilePlayer;
        Dictionary<int, int> dictMapKeys = new Dictionary<int, int>();
        Dictionary<int, int> dictKeysHeld = new Dictionary<int, int>();

        ArrayList recordTicks;
        ArrayList recordNotes;

        OutputDevice outDevice;
        ChannelMessageBuilder builder;
        byte transpose;

        bool isRecording;
        bool isDeviceOpen;

        public PlayingMidi()
        {
            this.transpose = 60;
            if (OutputDevice.DeviceCount == 0)
            {
                MessageBox.Show("No MIDI output devices available.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }

            outDevice = new OutputDevice(0);
            builder = new ChannelMessageBuilder();
            isDeviceOpen = true;

            //Set instrument to piano
            builder.Command = ChannelCommand.ProgramChange;
            builder.MidiChannel = 0;
            builder.Data1 = 0;
            builder.Build();
            outDevice.Send(builder.Result);

            ParseCommaDelimited();

            tmpmidiFilePlayer = new MidiFilePlayer(outDevice);
            midiFilePlayer1 = new MidiFilePlayer(outDevice);
            midiFilePlayer1.PlayingFinished += this.midiFilePlayer1_PlayingFinished;
        }
        public void CloseDevice()
        {
            midiFilePlayer1.Dispose();
            tmpmidiFilePlayer.Dispose();
            outDevice.Close();
            isDeviceOpen = false;
        }

        public bool onkey(int key)
        {
            if (dictMapKeys.ContainsKey(key))
            {
                //First see if the note has been played:
                if (dictKeysHeld.ContainsKey(key))
                    return true;

                byte note = (byte)((int)dictMapKeys[key] + this.transpose);

                playmidinote(note);

                dictKeysHeld.Add(key, 1);
                if (this.isRecording)
                {
                    this.recordTicks.Add(midiFilePlayer1.position);
                    this.recordNotes.Add((int)note);
                }
                return true;
            }
            else return false;
        }
        public bool onkeyup(int key)
        {
            //We only care about it if it was a one that was held down before:
            if (dictKeysHeld.ContainsKey(key))
            {
                byte note = (byte)((int)dictMapKeys[key] + this.transpose);
                unplaymidinote(note);

                dictKeysHeld.Remove(key);

                if (this.isRecording)
                {
                    this.recordTicks.Add(midiFilePlayer1.position);
                    this.recordNotes.Add(-1 * ((int)note));
                }
                return true;
            }
            else
                return false;
        }
       
       
        public void stopallnotes()
        {
            foreach (int key in dictKeysHeld.Keys)
            {
                byte note = (byte)((int)dictMapKeys[key] + this.transpose);
                unplaymidinote(note);
                if (this.isRecording)
                {
                    this.recordTicks.Add(midiFilePlayer1.position);
                    this.recordNotes.Add(-1 * ((int)note));
                }
            }
            dictKeysHeld.Clear();
        }

        public void setTransposition(bool bUpwards)
        {
            if (bUpwards && this.transpose<90)
            {
                if (this.transpose % 12 == 0) this.transpose += 7;
                else this.transpose += 5;
            }
            else if (!bUpwards && this.transpose>2)
            {
                if (this.transpose % 12 == 0) this.transpose -= 5;
                else this.transpose -= 7;
            }
            //playmidinote(this.transpose); System.Threading.Thread.Sleep(250); unplaymidinote(this.transpose);
        }
        public string getTransposition()
        {
            return MidiNoteConverter.RenderNote(this.transpose, true);
        }
        private void playmidinote(byte note)
        {
            if (!isDeviceOpen) return ;
            builder.Command = ChannelCommand.NoteOn;
            builder.MidiChannel = (isRecording ? getCurChannelRecording() : getCurChannelPlaying());
            builder.Data1 = note;
            builder.Data2 = 100; //127;
            builder.Build();
            outDevice.Send(builder.Result);
        }
        private void unplaymidinote(byte note)
        {
            if (!isDeviceOpen) return;
            builder.Command = ChannelCommand.NoteOff;
            builder.MidiChannel = (isRecording ? getCurChannelRecording() : getCurChannelPlaying());
            builder.Data1 = note;
            builder.Data2 = 0;
            builder.Build();
            outDevice.Send(builder.Result);
        }
        private void ParseCommaDelimited()
        {
            if (!File.Exists(cfgname)) cfgname = "..\\..\\" + cfgname; //when debug mode.
            StreamReader freader = File.OpenText(cfgname);
            string thisline;
            while ((thisline = freader.ReadLine()) != null)
            {
                thisline = thisline.Trim();
                if (thisline.Length == 0 || thisline[0] == '#') continue;
                string[] items = thisline.Split(',');

                if (items.Length > 2)
                {
                    if (items[2] != "DUR")
                    {
                        // Add some elements to the hash table. There are no duplicate keys
                        int key = Int32.Parse(items[0]);
                        int note = Int32.Parse(items[1]);
                        this.dictMapKeys.Add(key, note);
                    }
                    else
                    {
                        //We don't need durations
                    }
                }
            }
            freader.Close();
        }

      

        public int loadMidiFile(string filename)
        {
            //verify that it was created by us before loading. Returns -1 if not created by us.
            tmpmidiFilePlayer.Open(filename);
            List<Track> ttracks = tmpmidiFilePlayer.sequence.tracks;
            MidiEvent mevt = ttracks[1].GetMidiEvent(1);
            if (!(mevt.AbsoluteTicks == 599880 && mevt.MidiMessage.MessageType == MessageType.Channel))
                return -1;
            //we only support midi files that we created.

            midiFilePlayer1.Open(filename);
            List<Track> ttracks2 = midiFilePlayer1.sequence.tracks;
            int ctracks = ttracks2.Count;
            return ctracks;
        }
        public int removeLastTrack()
        {
            midiFilePlayer1.sequence.tracks.RemoveAt(midiFilePlayer1.sequence.tracks.Count-1);
            return midiFilePlayer1.sequence.tracks.Count;
        }
        public void saveMidiFile(string filename)
        {
            midiFilePlayer1.sequence.Save(filename);
        }
        public string getDetails()
        {
            string s = "";
            List<Track> ttracks = midiFilePlayer1.sequence.tracks;
            int ctracks = ttracks.Count;
            for (int i = 0; i < ctracks; i++)
            {
                string label = "Track " + (i + 1).ToString() + " : " + ttracks[i].Count.ToString() + " Events : " + ((int)(((double)ttracks[i].Length) / 96.0)).ToString() + " Beats Long";
                s += label + "\r\n";
            }
            return s;
        }
        public int counttracks()
        {
            return midiFilePlayer1.sequence.tracks.Count;
        }

        public void beginPlaying()
        {
            stopallnotes();
            midiFilePlayer1.Start();
        }
        public void stopPlaying()
        {
            stopallnotes();
            midiFilePlayer1.Stop();
        }


        public void beginRecording(int instrument)
        {
            stopallnotes();

            //add new track!
            Track newtrack = new Track();
            int nchannel = getCurChannelRecording() + 1;

            //Set instrument
            builder.Command = ChannelCommand.ProgramChange;
            builder.MidiChannel = nchannel;
            builder.Data1 = instrument;
            builder.Build();
            newtrack.Insert(0, builder.Result);
            this.midiFilePlayer1.sequence.Add(newtrack);

            //Also important: send event to the output device.
            outDevice.Send(builder.Result);

            recordTicks = new ArrayList();
            recordNotes = new ArrayList();

            midiFilePlayer1.Start();
            isRecording = true;
        }

        public void stopRecording()
        {
            stopallnotes();

            //fill notes to the track!

            int lasttime = midiFilePlayer1.position;
            midiFilePlayer1.Stop();            
            int nchannel = this.getCurChannelRecording();
            int ntrack = counttracks() - 1; //last track

            Debug.Assert(recordNotes.Count == recordTicks.Count);
            IEnumerator enote = recordNotes.GetEnumerator();
            IEnumerator entick = recordTicks.GetEnumerator();
            while (enote.MoveNext())
            {
                entick.MoveNext();

                if ((int)enote.Current > 0) //Note on
                {
                    builder.Command = ChannelCommand.NoteOn;
                    builder.MidiChannel = nchannel;
                    builder.Data1 = (int)enote.Current;
                    builder.Data2 = 127;
                    builder.Build();
                }
                else if ((int)enote.Current < 0) //Note off
                {
                    builder.Command = ChannelCommand.NoteOff;
                    builder.MidiChannel = nchannel;
                    builder.Data1 = Math.Abs((int)enote.Current);
                    builder.Data2 = 0;
                    builder.Build();
                }
                this.midiFilePlayer1.sequence.tracks[ntrack].Insert((int)entick.Current, builder.Result);
            }

            isRecording = false;
        }

        private int getCurChannelPlaying()
        {
            return (this.counttracks() - 2) + 1;
        }
        private int getCurChannelRecording()
        {
            return (this.counttracks() - 2);
        }
        private void midiFilePlayer1_PlayingFinished(object sender, EventArgs e)
        {
            //This only happens after 41 minutes of recording... we shouldn't get here often.
            System.Threading.Thread.Sleep(250);
            MessageBox.Show("Error: maximum song length reached.");
            if (isRecording)
                stopRecording();
            else
                stopPlaying();
        }
    }
}
