#region License

/* Copyright (c) 2005 Leslie Sanford
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
 * sell copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software. 
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
 */

#endregion

#region Contact

/*
 * Leslie Sanford
 * Email: jabberdabber@hotmail.com
 */

#endregion

using System;

namespace Multimedia.Midi
{
    


	/// <summary>
	/// Converts a MIDI note number to its corresponding frequency.
	/// </summary>
	public sealed class MidiNoteConverter
	{
        // Notes per octave.
        private const int NotesPerOctave = 12;

        // Offsets the note number.
        private const int NoteOffset = 9;

        // Reference frequency used for calculations.
        private const double ReferenceFrequency = 13.75;

        // Prevents instances of this class from being created - no need for
        // an instance to be created since this class only has static methods.
		private MidiNoteConverter()
		{
		}

        static string[] noteSharps = new string[12] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A","A#","B" };
        static string[] noteFlats = new string[12] { "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B" }; 


        //Written by Ben Fisher, 2006!!!!!!!!!!!!!!!!!!
        public static string RenderNote(int noteNumber, bool sharps)
        {
           int octave = (int) Math.Round((noteNumber - 12.0)/12.0);
            int cnote = noteNumber % 12;
            string cname = sharps ? MidiNoteConverter.noteSharps[cnote] : MidiNoteConverter.noteFlats[cnote];
           return cname + " " + octave.ToString();
        }




        /// <summary>
        /// Converts note to frequency.
        /// </summary>
        /// <param name="noteNumber">
        /// The number of the note to convert.
        /// </param>
        /// <returns>
        /// The frequency of the specified note.
        /// </returns>
        public static double NoteToFrequency(int noteNumber)
        {
            double exponent = (double)(noteNumber - NoteOffset) / NotesPerOctave;

            return ReferenceFrequency * Math.Pow(2.0, exponent);
        }

       public static string[] instrumentnames= {
     "AcousticGrandPiano",
"BrightAcousticPiano",
"ElectricGrandPiano",
"HonkyTonkPiano",
"ElectricPiano1",
"ElectricPiano2",
"Harpsichord",
"Clavinet",
"Celesta",
"Glockenspiel",
"MusicBox",
"Vibraphone",
"Marimba",
"Xylophone",
"TubularBells",
"Dulcimer",
"DrawbarOrgan",
"PercussiveOrgan",
"RockOrgan",
"ChurchOrgan",
"ReedOrgan",
"Accordion",
"Harmonica",
"TangoAccordion", 
"AcousticGuitarNylon", 
"AcousticGuitarSteel",
"ElectricGuitarJazz",
"ElectricGuitarClean",
"ElectricGuitarMuted",
"OverdrivenGuitar",
"DistortionGuitar",
"GuitarHarmonics",
"AcousticBass",
"ElectricBassFinger",
"ElectricBassPick",
"FretlessBass",
"SlapBass1",
"SlapBass2",
"SynthBass1",
"SynthBass2",
"Violin",
"Viola",
"Cello",
"Contrabass",
"TremoloStrings",
"PizzicatoStrings",
"OrchestralHarp",
"Timpani",
"StringEnsemble1",
"StringEnsemble2",
"SynthStrings1",
"SynthStrings2",
"ChoirAahs",
"VoiceOohs",
"SynthVoice",
"OrchestraHit",
"Trumpet",
"Trombone",
"Tuba",
"MutedTrumpet",
"FrenchHorn",
"BrassSection",
"SynthBrass1",
"SynthBrass2",
"SopranoSax",
"AltoSax",
"TenorSax",
"BaritoneSax",
"Oboe",
"EnglishHorn",
"Bassoon",
"Clarinet",
"Piccolo",
"Flute",
"Recorder",
"PanFlute",
"BlownBottle",
"Shakuhachi",
"Whistle",
"Ocarina",
"Lead1Square",
"Lead2Sawtooth",
"Lead3Calliope",
"Lead4Chiff",
"Lead5Charang",
"Lead6Voice",
"Lead7Fifths",
"Lead8BassAndLead",
"Pad1NewAge",
"Pad2Warm",
"Pad3Polysynth",
"Pad4Choir",
"Pad5Bowed",
"Pad6Metallic",
"Pad7Halo",
"Pad8Sweep",
"Fx1Rain",
"Fx2Soundtrack",
"Fx3Crystal",
"Fx4Atmosphere",
"Fx5Brightness",
"Fx6Goblins",
"Fx7Echoes",
"Fx8SciFi",
"Sitar",
"Banjo",
"Shamisen",
"Koto",
"Kalimba",
"BagPipe",
"Fiddle",
"Shanai",
"TinkleBell",
"Agogo",
"SteelDrums",
"Woodblock",
"TaikoDrum",
"MelodicTom",
"SynthDrum",
"ReverseCymbal",
"GuitarFretNoise",
"BreathNoise",
"Seashore",
"BirdTweet",
"TelephoneRing",
"Helicopter",
"Applause",
"Gunshot"
    };

	}

}
