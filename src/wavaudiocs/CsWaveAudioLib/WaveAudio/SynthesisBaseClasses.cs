//Ben Fisher, 2008
//halfhourhacks.blogspot.com
//GPL

//Generate 44100Hz, 1 channel sounds
/*
 * Usage:
 * Sine s = new Sine(440.0, 1.0)
 * WaveAudio w = s.CreateWaveFile(2.5) // make 2.5 seconds of audio
 * 
 * */

using System;

namespace CsWaveAudio
{
    namespace SynthBases
    {
        


        public abstract class SynthesisBase
        {
            public const int SampleRate = 44100;
            protected abstract double[] generate(int nLengthInSamples);
            public virtual WaveAudio CreateWaveAudio(double fSeconds)
            {
                WaveAudio res = new WaveAudio(SampleRate, 1);
                int nSamples = (int) (fSeconds * res.getSampleRate());
                res.data[0] = this.generate(nSamples);
                return res;
            }

            public static double FrequencyFromMidiNote(int n)
            {
                return 8.1758 * Math.Pow(2.0, (n / 12.0));
            }
        }

        public abstract class HighPeriodicSynthesisBase : SynthesisBase
        {
            // There isn't a reason for periodicsynth objects to be mutable
            protected readonly double timeScale;
            protected readonly double timeScaleOne; //one of these is a whole period.
            protected readonly double freq;
            protected readonly double amplitude;
            protected readonly double period;

            public HighPeriodicSynthesisBase(double freq, double amplitude)
            {
                this.amplitude = amplitude;
                this.freq = freq;
                this.timeScale = freq * 2.0 * Math.PI / (double)SampleRate;
                this.timeScaleOne = freq * 1.0 / (double)SampleRate;
                this.period = SampleRate / freq;
            }
            protected abstract void WaveformFunction(double[]data);
            protected override double[] generate(int nSamples)
            {
                double[] outData = new double[nSamples];
                WaveformFunction(outData);
                return outData;
            }
        }

        public abstract class PeriodicSynthesisBase : SynthesisBase
        {
            // There isn't a reason for periodicsynth objects to be mutable
            protected readonly double timeScale;
            protected readonly double freq;
            protected readonly double amplitude;
            protected readonly double period;

            public PeriodicSynthesisBase(double freq) : this(freq, 1.0) { }
            public PeriodicSynthesisBase(double freq, double amplitude)
            {
                this.amplitude = amplitude;
                this.freq = freq;
                this.timeScale = freq * 2.0 * Math.PI / (double)SampleRate;
                this.period = SampleRate / freq;
            }
            protected abstract double WaveformFunction(int i);
            protected override double[] generate(int nSamples)
            {
                double[] outData = new double[nSamples];
                // could be as simple as: for (int i = 0; i < outData.Length; i++) { outData[i] = WaveformFunction(i) * amplitude; }
                // but, because it is periodic, we can optimize. There is a slight amount of rounding involved which shouldn't be audible.

                int waveformPeriod = (int)(SampleRate / this.freq);
                for (int i = 0; i < outData.Length; i++)
                {
                    if (i < waveformPeriod)
                        outData[i] = WaveformFunction(i) * amplitude;
                    else
                        outData[i] = outData[i % waveformPeriod];
                }
                return outData;
            }
        }

        public abstract class AperiodicSimpleSynthesisBase : SynthesisBase
        {
            protected readonly double amplitude;
            protected abstract double WaveformFunction(int i);
            public AperiodicSimpleSynthesisBase(double amplitude)
            {
                this.amplitude = amplitude;
            }
            protected override double[] generate(int nSamples)
            {
                double[] outData = new double[nSamples];
                for (int i = 0; i < outData.Length; i++)
                {
                    outData[i] = WaveformFunction(i) * amplitude;
                }
                return outData;
            }
        }

        public abstract class SineWaveSumBase : SynthesisBase
        {
            protected double[] frequencies;
            protected double[] weights;
            protected double amplitude;

            protected void normalizeWeights()
            {
                // normalize weights, so that if someone passes in [1,1] => [0.5, 0.5] and [3,1] => [0.75, 0.25] and so on
                double fTotal = 0.0;
                for (int i = 0; i < weights.Length; i++)
                    fTotal += weights[i];

                if (fTotal != 0.0)
                    for (int i = 0; i < weights.Length; i++)
                        weights[i] /= fTotal;
            }
            protected override double[] generate(int nSamples)
            {
                double[] outData = new double[nSamples];
                // create array of timescales
                double[] timescales = new double[this.frequencies.Length];
                for (int i = 0; i < timescales.Length; i++)
                    timescales[i] = this.frequencies[i] * 2.0 * Math.PI / (double)SampleRate;

                for (int i = 0; i < outData.Length; i++)
                {
                    for (int w = 0; w < timescales.Length; w++)
                        outData[i] += amplitude * Math.Sin(i * timescales[w]) * weights[w];

                    if (outData[i] > 1.0) outData[i] = 1.0;
                    else if (outData[i] < -1.0) outData[i] = -1.0;
                }
                return outData;
            }
        }


    }
}
