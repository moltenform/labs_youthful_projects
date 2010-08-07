using System;
using System.Collections.Generic;
using System.Text;

namespace CsWaveAudio
{
    public static class ConstructionUtil
    {
        public static void placeAudioRamp(WaveAudio target, WaveAudio source, int index, int rampWidth)
        {
            for (int ch=0; ch<target.data.Length; ch++)
                for (int j = 0; j < source.data[ch].Length; j++)
                {
                    if (j < rampWidth)
                        target.data[ch][j + index] += source.data[ch][j] * (((double)j) / rampWidth);
                    else if ((source.data[ch].Length - j) < rampWidth)
                        target.data[ch][j + index] += source.data[ch][j] * (((double)source.data[ch].Length - j) / rampWidth);
                    else
                        target.data[ch][j + index] += source.data[ch][j];
                }
        }
        public static WaveAudio GetSliceSample(WaveAudio wthis, int nStart, int nEnd)
        {
            WaveAudio slice = new WaveAudio(wthis.getSampleRate(), wthis.getNumChannels());
            if (nEnd <= nStart || nEnd > wthis.LengthInSamples || nStart < 0) throw new Exception("Invalid slice");
            for (int ch = 0; ch < slice.data.Length; ch++)
            {
                slice.data[ch] = new double[nEnd - nStart];
                Array.Copy(wthis.data[ch], nStart, slice.data[ch], 0, nEnd - nStart);
            }
            return slice;
        }

        public static double[] getAmplitudesOverTime(WaveAudio w, int nPieceSize)
        {
            int nPieces = w.data[0].Length / nPieceSize;
            double[] res = new double[nPieces];
            for (int n = 0; n < nPieces; n++)
            {
                double total = 0;
                for (int i = n * nPieceSize; i < n * nPieceSize + nPieceSize; i++)
                {
                    total += w.data[0][i] * w.data[0][i];
                }
                total /= nPieceSize;
                res[n] = total;
            }
            return res;
        }
        //could also get continuous with window.


        public static WaveAudio lowPassFilter(WaveAudio w, double factor) //0.5
        {
            //http://en.wikipedia.org/wiki/Low-pass_filter
            WaveAudio ret = new WaveAudio(w.getSampleRate(), w.getNumChannels());
            ret.LengthInSamples = w.LengthInSamples;
            for (int ch = 0; ch < w.getNumChannels(); ch++)
            {
                ret.data[ch][0] = w.data[ch][0];
                for (int i=1; i<ret.data[ch].Length; i++)
                    ret.data[ch][i] = (1-factor)*ret.data[ch][i-1] + (factor)*w.data[ch][i];
            }
            return ret;
        }
        public static WaveAudio hiPassFilter(WaveAudio w, double factor) //0.5
        {
            WaveAudio ret = new WaveAudio(w.getSampleRate(), w.getNumChannels());
            ret.LengthInSamples = w.LengthInSamples;
            for (int ch = 0; ch < w.getNumChannels(); ch++)
            {
                ret.data[ch][0] = w.data[ch][0];
                for (int i = 1; i < ret.data[ch].Length; i++)
                    ret.data[ch][i] = factor * ret.data[ch][i - 1] + (factor) * (w.data[ch][i] - w.data[ch][i-1]);
            }
            return ret;
        }


    }
}
