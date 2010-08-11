//Ben Fisher, 2008
//halfhourhacks.blogspot.com
//GPL

using System;

namespace CsWaveAudio
{
    // Effects should always return a new WaveAudio without modifying the existing audio.
    // i.e., don't modify in place. Use .Clone() if necessary.
    public static partial class Effects
    {

        // Uses linear interpolation to find the 4.75th element of an array of doubles and so on.
        // In the future maybe consider better interpolation methods, like cubic, polynomial, or sinc
        internal static double getInterpolatedValue(double[] sampleData, double sampleIndex)
        {
            if (sampleIndex > sampleData.Length - 1) sampleIndex = sampleData.Length - 1;
            else if (sampleIndex < 0 + 1) sampleIndex = 0;

            double proportion = sampleIndex - Math.Truncate(sampleIndex);
            double v1 = sampleData[(int)Math.Truncate(sampleIndex)];
            double v2 = sampleData[(int)Math.Ceiling(sampleIndex)];
            return v2 * proportion + v1 * (1 - proportion);
        }

        
        /// <summary>
        /// Create a new sound by changing the speed/pitch of the old sound. 2.0 = an octave up, 1.0 = the same, 0.5 = down an octave
        /// </summary>
        public static WaveAudio ScalePitchAndDuration(WaveAudio w, double factor)
        {
            if (factor < 0) throw new ArgumentException("Factor must >= 0");
            WaveAudio res = new WaveAudio(w.getSampleRate(), w.getNumChannels());

            // do operation for all channels
            for (int i = 0; i < w.getNumChannels(); i++)
                res.data[i] = scalePitchAndDurationChannel(w.data[i], factor);

            return res;
        }
        private static double[] scalePitchAndDurationChannel(double[] chdata, double factor)
        {
            // the factor is the rate we walk through the file. if factor is 2.0, then we walk twice as fast
            double currentPosition = 0.0;
            int newLength = (int)(chdata.Length / factor);
            double[] newdata = new double[newLength];
            for (int i = 0; i < newLength; i++)
            {
                newdata[i] = Effects.getInterpolatedValue(chdata, currentPosition);
                currentPosition += factor;
            }
            return newdata;
        }


        public static WaveAudio Derivative(WaveAudio wOriginal)
        {
            WaveAudio w = wOriginal.Clone();
            for (int ch = 0; ch < w.getNumChannels(); ch++)
            {
                for (int i = 0; i < w.data[ch].Length; i++)
                {
                    if (i + 1 < w.data[ch].Length)
                        w.data[ch][i] = w.data[ch][i] - w.data[ch][i + 1];
                    else
                        w.data[ch][i] = w.data[ch][i] - 0;
                }
            }
            return w;
        }


        public static WaveAudio Reverse(WaveAudio w1)
        {
            WaveAudio clone = w1.Clone();
            for (int ch = 0; ch < clone.data.Length; ch++)
            {
                Array.Reverse( clone.data[ch]);
            }
            return clone;
        }


        public static WaveAudio Vibrato(WaveAudio wave) { return Vibrato(wave, 0.1, 2.0); }
        public static WaveAudio Vibrato(WaveAudio wave, double freq, double width)
        {
            if (width < 0) throw new ArgumentException("Factor must >= 0");
            WaveAudio newwave = new WaveAudio(wave.getSampleRate(), wave.getNumChannels());
            
            // do operation for all channels
            for (int i = 0; i < wave.getNumChannels(); i++)
                newwave.data[i] = vibratoChannel(wave.data[i], wave.getSampleRate(), width, freq);

            return newwave;
        }
        private static double[] vibratoChannel(double[] chdata, int sampleRate, double width, double vibratofreq)
        {
            // walk through the file at varying speeds
            double currentPosition = 0.0;
            double[] newdata = new double[chdata.Length];
            double vibratoFreqScale = 2.0 * Math.PI * vibratofreq / (double)sampleRate;
            for (int i = 0; i < chdata.Length; i++)
            {
                newdata[i] = Effects.getInterpolatedValue(chdata, currentPosition);
                currentPosition += 1.0 + width * Math.Sin(i * vibratoFreqScale);
            }
            return newdata;
        }
        public static WaveAudio Tremolo(WaveAudio w) { return Tremolo(w, 1.0, 0.2); }
        public static WaveAudio Tremolo(WaveAudio w, double tremfreq, double amp)
        {
            WaveAudio res = new WaveAudio(w.getSampleRate(), w.getNumChannels());
            res.LengthInSamples = w.LengthInSamples;
            double tremeloFreqScale = 2.0 * Math.PI * tremfreq / (double)w.getSampleRate();
            for (int ch=0;ch<w.data.Length; ch++)
            {
                for (int i = 0; i < w.data[ch].Length; i++)
                {
                    double val = w.data[ch][i] * (1 + amp * Math.Sin(tremeloFreqScale * i));
                    if (val > 1.0) val = 1.0;
                    else if (val < -1.0) val = -1.0;
                    res.data[ch][i] = val;
                }
            }
            return res;
        }


        /// <summary>
        /// Flange effect.
        /// </summary>
        public static WaveAudio Flange(WaveAudio w1) { return Flange(w1, 0.999); }
        public static WaveAudio Flange(WaveAudio w1, double parameter)
        {            
            WaveAudio w2 = Effects.ScalePitchAndDuration(w1, parameter);
            return WaveAudio.Mix(w1, w2);
        }

    }
}