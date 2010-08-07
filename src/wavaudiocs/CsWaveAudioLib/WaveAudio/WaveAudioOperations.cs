//Ben Fisher, 2008
//halfhourhacks.blogspot.com
//GPL

using System;

namespace CsWaveAudio
{
    public partial class WaveAudio
    {
        // The following create new audio without modifying original
        public static WaveAudio Concatenate(WaveAudio w1, WaveAudio w2)
        {
            // make sure sample rates match we could be nicer and convert automatically
            if (w1.m_currentSampleRate != w2.m_currentSampleRate) throw new Exception("Sample rates don't match");
            if (w2.getNumChannels() != w2.getNumChannels()) throw new Exception("Number of channels don't match");

            WaveAudio newwave = new WaveAudio(w1.getSampleRate(), w1.getNumChannels());
            newwave.LengthInSamples = w1.LengthInSamples + w2.LengthInSamples;

            for (int ch = 0; ch < w1.getNumChannels(); ch++)
            {
                //          source    sIndex, destination, destIndex,  length
                Array.Copy(w1.data[ch], 0, newwave.data[ch], 0, w1.data[ch].Length);
                Array.Copy(w2.data[ch], 0, newwave.data[ch], w1.data[ch].Length, w2.data[0].Length);
            }
            return newwave;
        }

        // To reuse code, both "Mix" and "Modulate" use a helper fn, ElementWiseCombination
        // This makes the code less straight-forward, but shorter.
        // Alternative would be to find a more elegant way to write ElementWiseCombination.
        public static WaveAudio Mix(WaveAudio w1, WaveAudio w2) { return Mix(w1, 0.5, w2, 0.5); }
        public static WaveAudio Mix(WaveAudio w1, double weight1, WaveAudio w2, double weight2)
        {
            ElementWiseCombinationFn fn = delegate(double v1, double v2)
            {
                return weight1 * v1 + weight2 * v2;
            };
            return elementWiseCombination(w1, w2, fn);
        }
        public static WaveAudio Mix(WaveAudio[] waves)
        {
            // all must have same length, sample rate, and no. of channels
            int nSamples = waves[0].LengthInSamples; foreach (WaveAudio w in waves) if (w.LengthInSamples != nSamples) throw new Exception("When mixing array of sounds, they all must be the same length.");
            int nSampleRate = waves[0].getSampleRate(); foreach (WaveAudio w in waves) if (w.getSampleRate() != nSampleRate) throw new Exception("When mixing array of sounds, they all must have same sample rate.");
            int nChannels = waves[0].getNumChannels(); foreach (WaveAudio w in waves) if (w.getNumChannels() != nChannels) throw new Exception("When mixing array of sounds, they all must have same amount of channels.");
            WaveAudio res = new WaveAudio(waves[0].getSampleRate(), waves[0].getNumChannels());
            res.LengthInSamples = nSamples;
            for (int ch = 0; ch < nChannels; ch++)
            {
                for (int i = 0; i < nSamples; i++)
                {
                    double value = 0; foreach (WaveAudio w in waves) value += w.data[ch][i];
                    res.data[ch][i] = value / waves.Length;
                }
            }
            return res;
        }

        public static WaveAudio Modulate(WaveAudio w1, WaveAudio w2)
        {
            ElementWiseCombinationFn fn = delegate(double v1, double v2)
            {
                return v1 * v2;
            };
            return elementWiseCombination(w1, w2, fn);
        }
        internal delegate double ElementWiseCombinationFn(double d1, double d2);

        

        // The following are done in-place and modify the wave file.

        /// <summary>
        /// Linear fade-in effect. Modifies audio file in-place.
        /// </summary>
        public void FadeIn(double fSeconds)
        {
            int nFadeSamples = (int)(fSeconds * this.m_currentSampleRate);
            if (nFadeSamples > this.LengthInSamples) throw new Exception("Fade longer than effect.");
            double fScale = 1 / (double)nFadeSamples;
            for (int ch = 0; ch < this.data.Length; ch++)
            {
                for (int i = 0; i < nFadeSamples; i++)
                {
                    this.data[ch][i] *= fScale * i;
                }
            }
        }

        /// <summary>
        /// Linear fade-out effect. Modifies audio file in-place.
        /// </summary>
        public void FadeOut(double fSeconds)
        {
            int nFadeSamples = (int)(fSeconds * this.m_currentSampleRate);
            if (nFadeSamples > this.LengthInSamples) throw new Exception("Fade longer than effect.");
            double fScale = 1 / (double)nFadeSamples;
            for (int ch = 0; ch < this.data.Length; ch++)
            {
                for (int i = this.data[ch].Length - nFadeSamples; i < this.data[ch].Length; i++)
                {
                    this.data[ch][i] *= fScale * (this.data[ch].Length - i);
                }
            }
        }

        /// <summary>
        /// Amplify sound by multiplying all samples by an amount. 0.5 = half as loud (linearly not log), 2.0 = twice as loud (may cause clipping)
        /// </summary>
        public void Amplify(double a)
        {
            if (a < 0.0) throw new Exception("Amplify must be positive");
            for (int ch = 0; ch < this.data.Length; ch++)
            {
                for (int i = 0; i < this.data[ch].Length; i++)
                {
                    double val = this.data[ch][i] * a;
                    if (val > 1.0) val = 1.0;
                    else if (val < -1.0) val = -1.0;
                    this.data[ch][i] = val;
                }
            }
        }



        // Helper function. It's long and gross because either sound could be longer.
        // I could be more clever and use Math.Max / Min to have WaveAudio longer, WaveAudio shorter
        // , but at least now it is readable 
        /// <summary>
        /// Element-wise combination of two audio clips. For example, adding, or modulation.
        /// </summary>
        internal static WaveAudio elementWiseCombination(WaveAudio w1, WaveAudio w2, ElementWiseCombinationFn fn)
        {
            if (w1.m_currentSampleRate != w2.m_currentSampleRate) throw new Exception("Sample rates don't match");
            if (w1.getNumChannels() != w2.getNumChannels()) throw new Exception("Number of channels don't match");

            WaveAudio newwave = new WaveAudio(w1.getSampleRate(), w1.getNumChannels());
            newwave.LengthInSamples = Math.Max(w1.LengthInSamples, w2.LengthInSamples);
            double val;
            for (int ch = 0; ch < w1.getNumChannels(); ch++)
            {
                if (w1.LengthInSamples > w2.LengthInSamples)
                {
                    for (int i = 0; i < w1.LengthInSamples; i++)
                    {
                        if (i >= w2.LengthInSamples)
                            val = fn(w1.data[ch][i], 0);
                        else
                            val = fn(w1.data[ch][i], w2.data[ch][i]);

                        if (val > 1.0) val = 1.0;
                        else if (val < -1.0) val = -1.0;
                        newwave.data[ch][i] = val;
                    }
                }
                else
                {
                    for (int i = 0; i < w2.LengthInSamples; i++)
                    {
                        if (i >= w1.LengthInSamples)
                            val = fn(0, w2.data[ch][i]);
                        else
                            val = fn(w1.data[ch][i], w2.data[ch][i]);

                        if (val > 1.0) val = 1.0;
                        else if (val < -1.0) val = -1.0;
                        newwave.data[ch][i] = val;
                    }
                }
            }
            return newwave;
        }
    }

}
