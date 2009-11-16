using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Midisketch
{
    public partial class DlgInstrument : Form
    {
        public DlgInstrument()
        {
            InitializeComponent();
            foreach (string ins in instruments)
            {
                listBox1.Items.Add(ins);
            }
            listBox1.SelectedIndex = 0;
            btnOk.Focus();
        }

        public int getWhich()
        {
            return listBox1.SelectedIndex;
        }
        public string[] instruments = {
    "Acoustic Grand Piano",
    "Bright Piano",
    "Electric Grand Piano",
    "Honky-Tonk Piano",
    "Electric piano 1",
    "Electric Piano 2",
    "Harpsichord",
    "Clavinet",
    "Celesta",
    "Glockenspiel",
    "Music Box",
    "Vibraphone",
    "Marimba",
    "Xylophone",
    "Tubular bells",
    "Dulcimer",
    "Drawbar Organ",
    "Percussive Organ",
    "Rock Organ",
    "Church Organ",
    "Reed Organ",
    "Accordion",
    "Harmonica",
    "Tango Accordion",
    "Nylon String Guitar",
    "Steel String Guitar",
    "Jazz Guitar",
    "Clean Electric Guitar",
    "Muted Electric Guitar",
    "Overdrive Guitar",
    "Distortion Guitar",
    "Guitar Harmonics",
    "Acoustic Bass",
    "Fingered Bass",
    "Picked Bass",
    "Fretless Bass",
    "Slap Bass 1",
    "Slap Bass 2",
    "Synth Bass 1",
    "Synth Bass 2",
    "Violin",
    "Viola",
    "Cello",
    "Contrabass",
    "Tremolo Strings",
    "Pizzicato Strings",
    "Orchestral Harp",
    "Timpani",
    "String Ensemble 1",
    "String Ensemble 2",
    "Synth Strings 1",
    "Synth Strings 2",
    "Choir Ahh",
    "Choir Oohh",
    "Synth Voice",
    "Orchestral Hit",
    "Trumpet",
    "Trombone",
    "Tuba",
    "Muted Trumpet",
    "French Horn",
    "Brass Section",
    "Synth Brass 1",
    "Synth Brass 2",
    "Soprano Sax",
    "Alto Sax",
    "Tenor Sax",
    "Baritone Sax",
    "Oboe",
    "English Horn",
    "Bassoon",
    "Clarinet",
    "Piccolo",
    "Flute",
    "Recorder",
    "Pan flute",
    "Blown Bottle",
    "Shakuhachi",
    "Whistle",
    "Ocarina",
    "Square Wave",
    "Sawtooth Wave",
    "Caliope",
    "Chiff",
    "Charang",
    "Voice",
    "Fifths",
    "Bass & Lead",
    "New Age",
    "Warm",
    "PolySynth",
    "Choir",
    "Bowed",
    "Metallic",
    "Halo",
    "Sweep",
    "FX: Rain",
    "FX: Soundtrack",
    "FX: Crystal",
    "FX: Atmosphere",
    "FX: Brightness",
    "FX: Goblins",
    "FX: Echo Drops",
    "FX: Star Theme",
    "Sitar",
    "Banjo",
    "Shamisen",
    "Koto",
    "Kalimba",
    "Bagpipe",
    "Fiddle",
    "Shanai",
    "Tinkle bell",
    "Agogo",
    "Steel Drums",
    "Woodblock",
    "Taiko Drum",
    "Melodic Tom",
    "Synth Drum",
    "Reverse Cymbal",
    "Guitar Fret Noise",
    "Breath Noise",
    "Seashore",
    "Bird Tweet",
    "Telephone Ring",
    "Helicopter",
    "Applause",
    "Gunshot"};
    }
}