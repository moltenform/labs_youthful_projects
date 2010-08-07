//Ben Fisher, 2008
//halfhourhacks.blogspot.com
//GPL
// Raw FFT calculation code by Don Cross, http://web.archive.org/web/20020124104515/http://www.intersrv.com/~dcross/fft.html

using System;


namespace CsWaveAudio
{
    public static class Fourier
    {


        /// <summary>
        /// Find spectrum content of signal. Returns array, where each element represents energy at frequencies.
        /// For example, SpectrumContent(w, 8) returns 8 numbers. The first in the array is the amount of energy at low frequencies, and the last is the energy at highest frequencies.
        /// Note that FFT uses a power of 2 samples, and so all of the signal may not be used.
        /// </summary>
        /// <param name="w">Sound</param>
        /// <param name="nBins">Number of bins to return.</param>
        public static double[] SpectrumContent(WaveAudio w, int nBins)
        {
            if (w.getNumChannels() != 1) throw new Exception("Only mono supported.");
            double[] buffer;

            // FFT uses a power of 2 samples, so we might have to truncate.
            if (!isPowerOfTwo((uint) w.LengthInSamples))
            {
                int nSize = (int)findNearestPowerOfTwo((uint) w.LengthInSamples);
                buffer = new double[nSize];
                Array.Copy(w.data[0], buffer, nSize);
            }
            else 
                buffer = w.data[0];
            return getSpectrumContent(buffer, nBins);
        }

        
        /// <summary>
        /// Find spectrum content of signal, over time. Basically divides signal into smaller chunks and runs SpectrumContent on each.
        /// Optionally provide a size in samples of each chunk (must be power of 2).
        /// </summary>
        /// <param name="w"></param>
        /// <param name="nBins"></param>
        public static double[][] SpectrumContentOverTime(WaveAudio w, int nBins ) { return SpectrumContentOverTime(w, nBins, 1024); }
        public static double[][] SpectrumContentOverTime(WaveAudio w, int nBins, int nSize)
        {
            if (w.getNumChannels() != 1) throw new Exception("Only mono supported.");
            if (!isPowerOfTwo((uint) nSize)) throw new Exception("Size must be power of 2.");

            int nDatapoints = w.LengthInSamples / nSize - 1;
            double[][] res = new double[nDatapoints][];
            double[] buffer = new double[nSize];

            for (int i = 0; i < nDatapoints; i++)
            {
                // get samples from this slice of time. Put it into the buffer
                Array.Copy(w.data[0], i * nSize, buffer, 0, nSize);

                res[i] = getSpectrumContent(buffer, nBins);
            }
            return res;
        }

        // buffer must be power of 2.
        private static double[] getSpectrumContent(double[] buffer, int nBins)
        {
            double[] res = new double[nBins];
            // get raw fourier results.
            double[] reout, imgout;
            RawSamplesToFrequency(buffer, out reout, out imgout);

            // get magnitudes - only from the first half of the results; the rest is not necessary and is a mirror image.
            // we are basically filling bins, like a histogram.
            double scale = nBins * 2 / (double)buffer.Length;
            for (int b = 0; b < buffer.Length / 2; b++)
            {
                res[(int)(b * scale)] += Math.Sqrt(reout[b] * reout[b] + imgout[b] * imgout[b]);
            }
            return res;
        }

        /// <summary>
        /// Get raw FFT results. Indices 0 to Len/2 are the data you will probably be interested in. The other half represents negative frequencies, only significant for complex signals.
        /// Length of buffer must be a power of 2.
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="freqReal"></param>
        /// <param name="freqImag"></param>
        public static void RawSamplesToFrequency(double[] samples, out double[] freqReal, out double[] freqImag)
        {
            fft_double(samples, null, out freqReal, out freqImag, false);
        }
        public static void RawFrequencyToSamples(out double[] samples, double[] freqReal, double[] freqImag)
        {
            double[] tmpImage;
            fft_double(freqReal, freqImag, out samples, out tmpImage, true);
        }

        // Beat detection, algorithm put together by Ben Fisher after reading some things
        // numbers here are arbitrary, based on what seemed to work ok. I'm sure it could be better.
        public static double GuessBpm(WaveAudio input)
        {
            const double minBpm = 60, maxBpm = 150;

            WaveAudio w = Effects.Derivative(input); // take the derivative of samples.

            // divide samples into chunks of size 1024.
            int nBufsize = 1024;
            double chunkframerate = input.getSampleRate() / nBufsize; // the chunks go by at this rate.

            // do first FFT
            double[][] frequencyData = SpectrumContentOverTime(w, 4, nBufsize);

            // create a new signal in time, consisting of the energy at lowest 1/4 freqs of the chunks.
            int slength = (int)Fourier.findNearestPowerOfTwo((uint)frequencyData.Length);
            double[] lowerdata = new double[slength];
            for (int i = 0; i < slength; i++)
                lowerdata[i] = frequencyData[i][0]; // the bottom 1/4 freqs (index 0-255,0Hz to 5512.5Hz  or something ?)

            // now take a second FFT on this new signal. Frequency should be range 0.6 to 2.5 Hz (40 to 150 Bpm).
            double[] reout, imgout;
            RawSamplesToFrequency(lowerdata, out reout, out imgout);

            // only keep track of output inside the range we are interested in
            int minFreqindex = (int)(reout.Length * ((minBpm / 60) / chunkframerate));
            int maxFreqindex = (int)(reout.Length * ((maxBpm / 60) / chunkframerate));

            // find the best candidate
            double highestEnergy = -1; int highestEnergyIndex = -1;
            for (int b = minFreqindex; b < maxFreqindex; b++)
            {
                double magnitude = Math.Sqrt(reout[b] * reout[b] + imgout[b] + imgout[b]);
                if (magnitude > highestEnergy) { highestEnergyIndex = b; highestEnergy = magnitude; }
            }
            double freqHertz = chunkframerate * (highestEnergyIndex / (double)reout.Length);
            double freqInBpm = freqHertz * 60;
            return freqInBpm;
        }

        #region Math

        private static void fft_double(double[] p_lpRealIn, double[] p_lpImagIn, out double[] p_lpRealOut, out double[] p_lpImagOut, bool p_bInverseTransform)
        {
            p_lpRealOut = new double[p_lpRealIn.Length];
            p_lpImagOut = new double[p_lpRealIn.Length];

            if (p_lpRealIn == null) return;
            uint p_nSamples = (uint)p_lpRealIn.Length;

            uint NumBits;
            uint i, j, k, n;
            uint BlockSize, BlockEnd;

            double angle_numerator = 2.0 * Math.PI;
            double tr, ti;

            if (!isPowerOfTwo(p_nSamples))
                throw new Exception("Not power of two.");

            if (p_bInverseTransform) angle_numerator = -angle_numerator;

            NumBits = numberOfBitsNeeded(p_nSamples);

            for (i = 0; i < p_nSamples; i++)
            {
                j = ReverseBits(i, NumBits);
                p_lpRealOut[j] = p_lpRealIn[i];
                p_lpImagOut[j] = (p_lpImagIn == null) ? 0.0 : p_lpImagIn[i];
            }

            BlockEnd = 1;
            double[] ar = new double[3];
            double[] ai = new double[3];
            for (BlockSize = 2; BlockSize <= p_nSamples; BlockSize <<= 1)
            {
                double delta_angle = angle_numerator / (double)BlockSize;
                double sm2 = Math.Sin(-2 * delta_angle);
                double sm1 = Math.Sin(-delta_angle);
                double cm2 = Math.Cos(-2 * delta_angle);
                double cm1 = Math.Cos(-delta_angle);
                double w = 2 * cm1;

                for (i = 0; i < p_nSamples; i += BlockSize)
                {
                    ar[2] = cm2;
                    ar[1] = cm1;

                    ai[2] = sm2;
                    ai[1] = sm1;

                    for (j = i, n = 0; n < BlockEnd; j++, n++)
                    {
                        ar[0] = w * ar[1] - ar[2];
                        ar[2] = ar[1];
                        ar[1] = ar[0];

                        ai[0] = w * ai[1] - ai[2];
                        ai[2] = ai[1];
                        ai[1] = ai[0];

                        k = j + BlockEnd;
                        tr = ar[0] * p_lpRealOut[k] - ai[0] * p_lpImagOut[k];
                        ti = ar[0] * p_lpImagOut[k] + ai[0] * p_lpRealOut[k];

                        p_lpRealOut[k] = p_lpRealOut[j] - tr;
                        p_lpImagOut[k] = p_lpImagOut[j] - ti;

                        p_lpRealOut[j] += tr;
                        p_lpImagOut[j] += ti;
                    }
                }

                BlockEnd = BlockSize;
            }

            if (p_bInverseTransform)
            {
                double denom = (double)p_nSamples;

                for (i = 0; i < p_nSamples; i++)
                {
                    p_lpRealOut[i] /= denom;
                    p_lpImagOut[i] /= denom;
                }
            }
        }
        private static bool isPowerOfTwo(uint p_nX)
        {
            if (p_nX < 2) return false;

            if ((p_nX & (p_nX - 1)) == 0)
                return true;
            else
                return false;
        }

        //rounds down to nearest power of two.
        private static uint findNearestPowerOfTwo(uint n)
        {
            uint p = 1;
            while (p <= n)
                p *= 2;

            return p / 2;
        }
        private static uint numberOfBitsNeeded(uint p_nSamples)
        {
            int i;
            if (p_nSamples < 2)
                return 0;

            for (i = 0; ; i++)
                if ((p_nSamples & (1 << i)) != 0) return (uint)i;
        }
        private static uint ReverseBits(uint p_nIndex, uint p_nBits)
        {
            uint i, rev;
            for (i = rev = 0; i < p_nBits; i++)
            {
                rev = (rev << 1) | (p_nIndex & 1);
                p_nIndex >>= 1;
            }
            return rev;
        }
        private static double Index_to_frequency(uint p_nBaseFreq, uint p_nSamples, uint p_nIndex)
        {
            if (p_nIndex >= p_nSamples)
                return 0.0;
            else if (p_nIndex <= p_nSamples / 2)
                return ((double)p_nIndex / (double)p_nSamples * p_nBaseFreq);
            else
                return (-(double)(p_nSamples - p_nIndex) / (double)p_nSamples * p_nBaseFreq);
        }
        #endregion
    }
}
