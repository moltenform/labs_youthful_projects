using System;
using System.Collections.Generic;
using System.Text;
using CsWaveAudio;

namespace WaveAudioTests
{
    class Program
    {
        static void Main(string[] args)
        {
            AudioPlayer pl = new AudioPlayer();

            // Uncomment the tests you wish to run.


            CsWaveAudioTests.synthtests(pl);
            //CsWaveAudioTests.effectstest(pl);
            //CsWaveAudioTests.effectsaudacitytest(pl);
            //CsWaveAudioTests.iotests();
            //CsWaveAudioTests.propertytests();
            //CsWaveAudioTests.operations_test(pl);
            //CsWaveAudioTests.pitchdetect_test(pl);
            //CsWaveAudioTests.padsynthtests(pl);

            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }

    static partial class CsWaveAudioTests
    {
        public static string mediadir = "..\\..\\test_media\\";
        public static void effectstest(AudioPlayer pl)
        {
            WaveAudio w = new WaveAudio(mediadir + "d44k16bit2ch.wav");
            pl.Play(Effects.Derivative(w));
            pl.Play(Effects.Flange(w));
            pl.Play(Effects.Reverse(w));
            pl.Play(Effects.ScalePitchAndDuration(w, 0.75));
            pl.Play(Effects.ScalePitchAndDuration(w, 1.25));
            pl.Play(Effects.Tremolo(w, 1.0, 1.0));
            pl.Play(Effects.Vibrato(w, 0.2, 0.5));

            w.setSampleRate(22050);
            pl.Play(w); // should sound "normal"
        }

        public static void synthtests(AudioPlayer pl)
        {
            // also tests performance, because if there is a long pause, this is taking a bit to calculate.
            //pl.Play(new CsWaveAudio.Triangle(300.0, 0.7).CreateWaveAudio(0.5));
            //pl.Play(new CsWaveAudio.Sawtooth(300.0, 0.7).CreateWaveAudio(0.5));
            //pl.Play(new CsWaveAudio.Square(300.0, 0.7).CreateWaveAudio(0.5));
            //pl.Play(new CsWaveAudio.Sine(300.0, 0.7).CreateWaveAudio(0.5));

            //pl.Play(new CsWaveAudio.SineWaveSum(new double[] { 300.0 }, new double[] { 1.0 }, 0.7).CreateWaveAudio(0.5));
            //pl.Play(new CsWaveAudio.SineWaveOrgan(300.0, 0.7).CreateWaveAudio(1.5));
            //pl.Play(new CsWaveAudio.SineWaveSmooth(300.0, 0.7).CreateWaveAudio(1.5));
            //pl.Play(new CsWaveAudio.ElectricOrgan(300.0, 0.7).CreateWaveAudio(0.5));
            //pl.Play(new CsWaveAudio.SquarePhaser(300.0, 0.7).CreateWaveAudio(3.5));

            //pl.Play(new CsWaveAudio.Splash(0.7).CreateWaveAudio(0.5));
            //pl.Play(new CsWaveAudio.RedNoise(0.1).CreateWaveAudio(0.5));
            //pl.Play(new CsWaveAudio.WhiteNoise(0.7).CreateWaveAudio(0.5));
            //pl.Play(new CsWaveAudio.RedNoiseGlitch(250, 0.5, 5, 0.3).CreateWaveAudio(19.5));

            //note-appears to get quieter, but energy spectrum actually the same- that's just our ears hearing frequencies at different volumen

            WaveAudio j1 = (new CsWaveAudio.RedNoiseSmoothed(100, 0.5 ).CreateWaveAudio(2.5));
            WaveAudio j2 = (new CsWaveAudio.RedNoiseSmoothed(150, 0.5 ).CreateWaveAudio(2.5));
            WaveAudio j3 = (new CsWaveAudio.RedNoiseSmoothed(250, 0.5).CreateWaveAudio(2.5));
            pl.Play(WaveAudio.Mix(WaveAudio.Mix(j1, j3),j2));

        }

        public static void propertytests()
        {
            // these aren't the best tests.
            WaveAudio w1 = new WaveAudio(44100, 2);
            asserteq(w1.data.Length, 2, "channels");
            asserteq(w1.getNumChannels(), 2, "channels");
            assert(w1.data[0] != null && w1.data[1] != null, "channels");
            assert(w1.data[0].Length == 1 && w1.data[1].Length == 1, "004");
            asserteq(w1.getSampleRate(), 44100, "005");

            WaveAudio w1m = new WaveAudio(22050, 1);
            asserteq(w1m.data.Length, 1, "channels");
            assert(w1m.data[0] != null, "channels");
            asserteq(w1m.data[0].Length, 1, "004");
            asserteq(w1m.getSampleRate(), 22050, "005");

            // now set some properties
            w1m.LengthInSamples = 100;
            asserteq(w1m.data[0].Length, 100);
            asserteqf(w1m.LengthInSeconds, 100 / (double)w1m.getSampleRate(), 0.001);
        }

        public static void iotests()
        {
            iotests_perfile(mediadir + "d22k8bit1ch.wav", 8, 1, 22050);
            iotests_perfile(mediadir + "d22k8bit2ch.wav", 8, 2, 22050);
            iotests_perfile(mediadir + "d22k16bit1ch.wav", 16, 1, 22050);
            iotests_perfile(mediadir + "d44k8bit1ch.wav", 8, 1, 44100);
            iotests_perfile(mediadir + "d44k16bit1ch.wav", 16, 1, 44100);
            iotests_perfile(mediadir + "d44k16bit2ch.wav", 16, 2, 44100);
        }
        static void iotests_perfile(string strFilename, int nBits, int nChannels, int nRate)
        {
            WaveAudio w01 = new WaveAudio(strFilename);
            asserteq(w01.getNumChannels(), nChannels);
            asserteq(w01.getSampleRate(), nRate, "011");
            asserteqf(w01.LengthInSamples, 90725 * (nRate / 22050), 1.0, "012"); //note give 1.0 tolerance
            asserteqf(w01.LengthInSeconds, 4.1145124, "013");

            asserteq(w01.data.Length, nChannels);
            asserteq(w01.data[0].Length, w01.LengthInSamples);
            for (int i = 0; i < nChannels; i++)
                asserteq(w01.data[i].Length, w01.LengthInSamples);

            // test converting to other rates / quality
            w01.SaveWaveFile(mediadir + "ttout\\o_" + nRate + "_" + nBits + "_" + nRate + "_" + nChannels + ".wav", nBits);
            nBits = (nBits == 8) ? 16 : 8;
            w01.SaveWaveFile(mediadir + "ttout\\ot_" + nRate + "_" + nBits + "_" + nRate + "_" + nChannels + ".wav", nBits);
        }
        public static void operations_test(AudioPlayer pl)
        {
            WaveAudio noteLongLow = new Triangle(Triangle.FrequencyFromMidiNote(60), 0.5).CreateWaveAudio(1.0);
            WaveAudio noteShortHi = new Triangle(Triangle.FrequencyFromMidiNote(64), 0.5).CreateWaveAudio(0.5);

            pl.Play(WaveAudio.Concatenate(noteLongLow, noteShortHi));
            pl.Play(WaveAudio.Concatenate(noteShortHi, noteLongLow));
            pl.Play(WaveAudio.Mix(noteShortHi, noteLongLow));
            pl.Play(WaveAudio.Mix(noteLongLow, noteShortHi));
            WaveAudio tmp = new Sine(200,1.0).CreateWaveAudio(4.0);
            tmp.setNumChannels(2, true);
            pl.Play(WaveAudio.Modulate(new WaveAudio(mediadir+"d44k16bit2ch.wav"), tmp));

            WaveAudio cp;
            cp = noteLongLow.Clone(); cp.FadeIn(0.3); pl.Play(cp);
            cp = noteLongLow.Clone(); cp.FadeOut(0.3); pl.Play(cp);
            cp = noteLongLow.Clone(); cp.Amplify(0.5); pl.Play(cp);
            cp = noteLongLow.Clone(); cp.Amplify(2.0); pl.Play(cp);
            
        }

        public static void pitchdetect_test(AudioPlayer pl)
        {
            PitchDetection.PitchDetectAlgorithm algorithm = PitchDetection.PitchDetectAlgorithm.Amdf;
            string[] strFiles = new string[] { "btrombone1.wav", "btrombone2.wav", "cello.wav", "cello2.wav", "flute.wav", "organ.wav", "piano.wav", "sing01.wav", "sing02.wav", "sing03.wav", "trumpet.wav" };
            foreach (string s in strFiles)
                pitchdetectwav(pl, s, algorithm);
        }

        // Test between 20Hz and 500Hz. This works very well, although it is hard to eliminate octave errors
        // This test will play original, then a sine at the frequency it detected. The two should line up.
        private static void pitchdetectwav(AudioPlayer pl, string strFilename, PitchDetection.PitchDetectAlgorithm algorithm)
        {
            string strInstdir = mediadir + @"pitchdetect\";
            WaveAudio w = new WaveAudio(strInstdir + strFilename);
            if (w.getNumChannels() != 1) w.setNumChannels(1, true);
            double dfreq = PitchDetection.DetectPitch(w, 50,500,algorithm);
            WaveAudio testPitch = new Triangle(dfreq, 0.7).CreateWaveAudio(1.0);
            pl.Play(w);
            pl.Play(testPitch);
        }

        public static void padsynthtests(AudioPlayer pl)
        {
            WaveAudio menchoir = new PadSynthesisChoir(110.0, 0.7, 1.0).CreateWaveAudio(4.0);
            pl.Play(menchoir);
            
            WaveAudio ww01 = new PadSynthesisEnsemble(Sine.FrequencyFromMidiNote(59), 0.7).CreateWaveAudio(4.0);
            WaveAudio ww02 = new PadSynthesisEnsemble(Sine.FrequencyFromMidiNote(64), 0.7).CreateWaveAudio(4.0);
            WaveAudio ww03 = new PadSynthesisEnsemble(Sine.FrequencyFromMidiNote(67), 0.7).CreateWaveAudio(4.0);
            WaveAudio ww04 = new PadSynthesisEnsemble(Sine.FrequencyFromMidiNote(69 + 12), 0.7).CreateWaveAudio(4.0);
            pl.Play(WaveAudio.Mix(new WaveAudio[] { ww01, ww02, ww03, ww04 }));
            
            WaveAudio guitar1 = new PadSynthesisExtended(110.0, 0.7).CreateWaveAudio(4.0);
            WaveAudio guitar2 = new PadSynthesisExtended(110.0*1.5, 0.7).CreateWaveAudio(4.0);
            pl.Play(WaveAudio.Mix(guitar1,guitar2));
        }

        public static void effectsaudacitytest(AudioPlayer pl)
        {
            WaveAudio w = new WaveAudio(@"C:\pydev\yalp\Subversion\csaudio\c_audio\simon.wav");
            WaveAudio w2 = Effects.Wahwah(w);
            pl.Play(w2);

            WaveAudio w3 = Effects.Phaser(w);
            pl.Play(w3);
        }
    }
}
