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
using CsWaveAudio.SynthBases;

namespace CsWaveAudio
{
    public class Sine : PeriodicSynthesisBase
    {
        public Sine(double freq, double amplitude) : base(freq, amplitude) { }
        protected override double WaveformFunction(int i)
        {
            return Math.Sin(i * timeScale);
        }
    }

    public class HighSine : HighPeriodicSynthesisBase
    {
        public HighSine(double freq, double amplitude) : base(freq, amplitude) { }
        protected override void WaveformFunction(double[] data)
        {
            for (int i=0; i<data.Length; i++)
                data[i] = Math.Sin(i * timeScale);
        }
    }

    public class Square : PeriodicSynthesisBase
    {
        private readonly double cutpoint;
        public Square(double freq, double amplitude) : this(freq, amplitude, 0.5) { }
        public Square(double freq, double amplitude, double cutpointpos)
            : base(freq, amplitude)
        {
            this.cutpoint = this.period * cutpointpos;
        }
        protected override double WaveformFunction(int i)
        {
            return ((i % period) > cutpoint ? 1.0 : -1.0);
        }
    }

    public class Sawtooth : PeriodicSynthesisBase
    {
        private readonly double slope;
        public Sawtooth(double freq, double amplitude)
            : base(freq, amplitude)
        {
            this.slope = 2.0 / this.period; //because it goes from -1 to 1 in one period
        }
        protected override double WaveformFunction(int i)
        {
            return ((i % period) * slope - 1);
        }
    }

    public class Triangle : PeriodicSynthesisBase
    {
        private readonly double slope;
        public Triangle(double freq, double amplitude)
            : base(freq, amplitude)
        {
            this.slope = 4.0 / this.period; // //because it goes from -1 to 1 in half of a period
        }
        protected override double WaveformFunction(int i)
        {
            double wavePosition = (i % period);
            if (wavePosition < period / 2.0)
                return (wavePosition * slope - 1);
            else
                return ((wavePosition - period / 2) * -slope + 1);
        }
    }


    // On a whim I thought, what if instead of a sine wave I used semi-circles!
    // the result doesn't sound too interesting, but I use it for my "electric organ" effect.
    internal class CircleWave : PeriodicSynthesisBase
    {
        //circle is sqrt(1-x^2). I invented this. It doesn't sound that great.
        private readonly double qtrperiod;
        private readonly double halfperiod;
        public CircleWave(double freq, double amplitude)
            : base(freq, amplitude)
        {
            qtrperiod = period/4.0;
            halfperiod = period/2.0;
        }
        protected override double WaveformFunction(int i)
        {
            if (i % period < halfperiod)
                return Math.Sqrt(1 - Math.Pow(((i % period) / qtrperiod - 1), 2.0));
            else
                return -1.0 * Math.Sqrt(1 - Math.Pow((((i % period) - halfperiod) / qtrperiod - 1), 2.0));
        }
    }

    public class WhiteNoise : AperiodicSimpleSynthesisBase
    {
        private Random rand;
        public WhiteNoise(double amp) : base(amp) { rand = new Random(); }
        protected override double WaveformFunction(int i)
        {
            return (rand.NextDouble() * 2 - 1.0); //random number between -1 and 1
        }
    }

    // like a random walk. Can also be thought of as integral of white noise
    // note: the weights on this may not be correct, at least the spectrum doesn't match with Audacity's brown noise
    public class RedNoise : AperiodicSimpleSynthesisBase
    {

        private Random rand;
        private double location, factor;
        // the default "factor" was changed from 2.0 to 0.1 after comparing with Audacity's brown noise.
        public RedNoise(double amplitude) : this(amplitude, 0.1, null) { }
        public RedNoise(double amplitude, double factor) : this(amplitude, factor, null) { }
        public RedNoise(double amplitude, double factor, Random rand)
            : base(amplitude)
        {
            this.rand = (rand==null) ? new Random() : rand; 
            location = 0; this.factor = factor;
        }
        protected override double WaveformFunction(int i)
        {
            location += (rand.NextDouble() * 2 - 1.0) * factor;
            if (location > 1) location = 1;
            else if (location < -1) location = -1;
            return location;
        }
    }


    public class PinkNoise : AperiodicSimpleSynthesisBase
    {
        // http://home.earthlink.net/~ltrammell/tech/pinkalg.htm
        // spectrum looks pretty good. 
        // Every sample is multiplied by 30 to increase the amplitude, but this will not affect the spectrum.
        private Random rand;
        private readonly double[] av = new double[] { 4.6306e-003,  5.9961e-003,  8.3586e-003};
        private readonly double[] pv = new double[] { 3.1878e-001, 7.7686e-001, 9.7785e-001 };
        private double[] randreg;
        public PinkNoise(double amplitude) : this(amplitude, null) { }
        public PinkNoise(double amplitude, Random rand)
            : base(amplitude)
        {
            this.rand = (rand==null) ? new Random() : rand; 
            
            // Initialize the randomized sources state
            randreg = new double[av.Length];
            for (int i = 0; i < av.Length; i++)
                randreg[i] = av[i] * 2 * (rand.NextDouble() - 0.5);
        }
        protected override double WaveformFunction(int sampleIndex)
        {
            double rv = rand.NextDouble();
            // Update each generator state per probability schedule
            for (int i = 0; i < av.Length; i++)
            {
                if (rv > pv[i])
                    randreg[i] = av[i] * 2 * (rand.NextDouble() - 0.5);
            }
            // Signal is the sum of the generators
            return (randreg[0] + randreg[1] + randreg[2]) * 30.0;
        }
    }

    public class RedNoiseGlitch : SynthesisBase
    {
        protected readonly double freq;
        protected readonly double amplitude;
        protected readonly int chunksbeforeswitch;
        protected readonly double rednoisefactor;
        
        public RedNoiseGlitch(double freq, double amplitude) : this(freq, amplitude, 10, 0.261) {}
        public RedNoiseGlitch(double freq, double amplitude, int chunksbeforeswitch, double rednoisefactor)
        {
            this.rednoisefactor = rednoisefactor;
            this.chunksbeforeswitch = chunksbeforeswitch;
            this.freq = freq;
            this.amplitude = amplitude;
        }
        protected override double[] generate(int nSamples)
        {
            double[] outData = new double[nSamples];
            Random rand = new Random(); //if a new random is created every time by Rednoise, it doesn't update fast enough!

            int freqInSamples = (int)((1 / freq) * SynthesisBase.SampleRate);
            int numChunks = nSamples / freqInSamples;
            WaveAudio chunk = null;
            for (int i = 0; i < numChunks; i++)
            {
                if (i % this.chunksbeforeswitch == 0)
                {
                    chunk = new RedNoise(amplitude, rednoisefactor, rand).CreateWaveAudio(1 / freq + 0.01);
                }
                Array.Copy(chunk.data[0], 0, outData, i * freqInSamples, freqInSamples);
            }
            //fill in the rest
            if (numChunks * freqInSamples < nSamples)
                Array.Copy(chunk.data[0], 0, outData, numChunks * freqInSamples, nSamples - numChunks * freqInSamples);
            
            return outData;
        }
    }


    public class RedNoiseSmoothed : SynthesisBase
    {
        protected readonly double freq;
        protected readonly double amplitude;
        protected readonly int chunksbeforeswitch;
        protected readonly double rednoisefactor;
        protected readonly double smoothing; //from 0.0 to 1.0

        public RedNoiseSmoothed(double freq, double amplitude) : this(freq, amplitude, 5, 0.2, 0.91) { }
        public RedNoiseSmoothed(double freq, double amplitude, int chunksbeforeswitch, double rednoisefactor, double smoothing)
        {
            this.rednoisefactor = rednoisefactor;
            this.chunksbeforeswitch = chunksbeforeswitch;
            this.freq = freq;
            this.amplitude = amplitude;
            this.smoothing = smoothing;
        }
        protected override double[] generate(int nSamples)
        {
            double[] outData = new double[nSamples];
            Random rand = new Random(); //if a new random is created every time, it doesn't update fast enough!

            int freqInSamples = (int)((1 / freq) * SynthesisBase.SampleRate);
            int numChunks = nSamples / freqInSamples;
            WaveAudio chunk = null;
            for (int i = 0; i < numChunks; i++)
            {
                if (i % this.chunksbeforeswitch == 0)
                {
                    if (chunk == null)
                    {
                        //seed with Sine wave
                        chunk = new Sine(210, amplitude / 4).CreateWaveAudio(1 / freq + 0.01);
                    }
                    else
                    {
                        WaveAudio newchunk = new RedNoise(amplitude, rednoisefactor, rand).CreateWaveAudio(1 / freq + 0.01);
                        chunk = WaveAudio.Mix(chunk, this.smoothing, newchunk, 1 - this.smoothing);
                    }
                }
                Array.Copy(chunk.data[0], 0, outData, i * freqInSamples, freqInSamples);
            }
            //fill in the rest
            if (numChunks * freqInSamples < nSamples)
                Array.Copy(chunk.data[0], 0, outData, numChunks * freqInSamples, nSamples - numChunks * freqInSamples);

            return outData;
        }
    }


}

