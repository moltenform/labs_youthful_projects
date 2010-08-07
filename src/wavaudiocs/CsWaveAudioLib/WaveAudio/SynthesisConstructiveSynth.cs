//Ben Fisher, 2008
//halfhourhacks.blogspot.com
//GPL

// This file contains more complex synthesis.
// It may in the future hold more instruments.


using System;
using CsWaveAudio.SynthBases;

namespace CsWaveAudio
{
    /// <summary>
    /// Constructive synthesis using many sine waves.
    /// </summary>
    public class SineWaveSum : SineWaveSumBase
    {
        public SineWaveSum(double[] frequencies, double[] weights) : this(frequencies, weights, 1.0, true) { }
        public SineWaveSum(double[] frequencies, double[] weights, double amplitude) : this(frequencies, weights, amplitude, true) { }
        public SineWaveSum(double[] frequencies, double[] weights, double amplitude, bool bNormalizeWeights)
        {
            if (frequencies.Length != weights.Length) throw new Exception("Lengths of freqs and weights do not match");
            this.frequencies = frequencies;
            this.weights = weights;
            this.amplitude = amplitude;

            if (bNormalizeWeights)
                this.normalizeWeights();
        }
    }

    // "Instruments" created through experimentation.

    public class SineWaveOrgan : SineWaveSumBase
    {
        // constructive synthesis. numbers arbitrary, they just sound ok. Ben Fisher, 2007-8.
        public SineWaveOrgan(double freq, double amplitude)
        {
            this.amplitude = amplitude;
            frequencies = new double[16];
            weights = new double[16];
            // fundamental
            frequencies[0] = freq;
            weights[0] = Math.Pow(0.9, 8 - 1);
            // harmonics
            for (int n = 1; n < 8; n++)
            {
                frequencies[n] = freq * (n + 1);
                weights[n] = 0.1 * Math.Pow(0.9, 8 - 1);
            }
            // add an additional wave with slightly higher freq. The resulting beats make this sound better
            for (int n = 8; n < 16; n++)
            {
                frequencies[n] = frequencies[n - 8] * 1.00606;
                weights[n] = weights[n - 8];
            }

            this.normalizeWeights();
        }
    }

    // Intentional beat frequencies "smoothen" the sound and make it more musical.
    public class SineWaveSmooth : SineWaveSumBase
    {
        public SineWaveSmooth(double freq, double amplitude)
        {
            this.amplitude = amplitude;

            frequencies = new double[2];
            weights = new double[2];
            frequencies[0] = freq;
            weights[0] = 0.6;
            frequencies[1] = freq * 1.0006079;
            weights[1] = 0.4;

            this.normalizeWeights();
        }
    }

    //Composites

    public class ElectricOrgan : SynthesisBase
    {
        private readonly double frequency;
        private readonly double amplitude;
        public ElectricOrgan(double freq, double amplitude)
        {
            this.frequency = freq; this.amplitude = amplitude;
        }
        protected override double[] generate(int nSamples) { return null; }
        public override WaveAudio CreateWaveAudio(double fSeconds)
        {
            WaveAudio w1 = new CircleWave(frequency, amplitude).CreateWaveAudio(fSeconds);
            WaveAudio w2 = new CircleWave(frequency * 0.9939577, amplitude).CreateWaveAudio(fSeconds);
            WaveAudio w3 = new CircleWave(frequency * 0.98489425, amplitude).CreateWaveAudio(fSeconds);
            return WaveAudio.Mix(WaveAudio.Mix(w1, 0.66, w2, 0.33), 0.66, w3, 0.33);
        }
    }

    public class SquarePhaser : AperiodicSimpleSynthesisBase
    {
        private readonly double frequency;
        private readonly double scalePhaseFrequency;
        private readonly double halfperiod;

        public SquarePhaser(double freq, double amplitude) : this(freq, amplitude, 0.05) { }
        public SquarePhaser(double freq, double amplitude, double phaseFrequency) : base(amplitude)
        {
            this.frequency = freq; 
            scalePhaseFrequency = phaseFrequency * 2.0 * Math.PI / (double)SampleRate;
            halfperiod = (SampleRate / freq) / 2.0;
        }
        protected override double WaveformFunction(int i)
        {
            double cutpoint = halfperiod + halfperiod * (Math.Sin(i * scalePhaseFrequency))*0.9;
            if (i%(halfperiod*2) < cutpoint) return amplitude;
            else return -amplitude;
        }
    }

    /// <summary>
    /// Like sinewavesum, but it normalizes all of the samples afterwards.
    /// </summary>
    public class SineWaveSumNormalizeAfterwards : CsWaveAudio.SynthBases.SineWaveSumBase
    {
        public SineWaveSumNormalizeAfterwards(double[] frequencies, double[] weights, double amplitude)
        {
            if (frequencies.Length != weights.Length) throw new Exception("Lengths of freqs and weights do not match");
            this.frequencies = frequencies;
            this.weights = weights;
            this.amplitude = amplitude;

            this.normalizeWeights();
        }
        protected override double[] generate(int nSamples)
        {
            double[] outData = new double[nSamples];
            // create array of timescales
            double[] timescales = new double[this.frequencies.Length];
            for (int i = 0; i < timescales.Length; i++)
                timescales[i] = this.frequencies[i] * 2.0 * Math.PI / (double)SampleRate;

            this.sumSineWaves(timescales, outData);
            
            //find max
            double dmax = -999;
            for (int i = 0; i < outData.Length; i++) if (outData[i] > dmax) dmax = outData[i];

            // normalize by this
            for (int i = 0; i < outData.Length; i++) outData[i] /= dmax;
            return outData;
        }
        protected virtual void sumSineWaves(double[] timescales, double[] outData)
        {
            for (int i = 0; i < outData.Length; i++)
            {
                for (int w = 0; w < timescales.Length; w++)
                    outData[i] += amplitude * Math.Sin(i * timescales[w]) * weights[w];
                // don't clip this yet.
            }
        }
    }
 


    /// <summary>
    ///red noise that changes over time
    /// example: new CsWaveAudio.Splash(0.2).CreateWaveAudio(10.0)
    /// </summary>
    public class Splash : AperiodicSimpleSynthesisBase
    {
        private Random rand;
        private double location, factor;
        public Splash(double amp) : this (amp, 0.01) { }
        public Splash(double amp, double factor)
            : base(amp)
        {
            rand = new Random(); location = 0; this.factor = factor;
        }
        protected override double WaveformFunction(int i)
        {
            if (i % 100 == 0) factor += 0.0001;

            location += (rand.NextDouble() * 2 - 1.0) * factor;
            if (location > 1) location = 1;
            else if (location < -1) location = -1;
            return location;

        }
    }


}