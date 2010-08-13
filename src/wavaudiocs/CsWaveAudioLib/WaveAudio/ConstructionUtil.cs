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

        public abstract class FourierModifier
        {
            public WaveAudio doModify(WaveAudio w) { return doModify(w, 2048); }
            public WaveAudio doModify(WaveAudio src, int bufsize) 
            {
                WaveAudio wout = new WaveAudio(src.getSampleRate(), 1);
                wout.LengthInSamples = src.LengthInSamples;
                
                //reuse the buffers.
                double[] ffreqmaghalfout=new double[bufsize/2], ffreqanghalfout=new double[bufsize/2];
                double[] ffreqmaghalfin=new double[bufsize/2], ffreqanghalfin=new double[bufsize/2];
                double[] fbuffertime = new double[bufsize];
                for (int partnum=0; partnum<src.LengthInSamples/bufsize; partnum++)
                {
                    //copy into buffer.
                    Array.Copy(src.data[0], partnum*bufsize, fbuffertime, 0, bufsize);
                    double[] ffreqreal, ffreqimag;
                    Fourier.RawSamplesToFrequency(fbuffertime, out ffreqreal, out ffreqimag);
                    //we only care about the first half of these results.
                    for (int i=0; i<bufsize/2; i++)
                    {
                        ffreqmaghalfin[i] = Math.Sqrt(ffreqreal[i]*ffreqreal[i]+ffreqimag[i]*ffreqimag[i]);
                        ffreqanghalfin[i] = Math.Atan2(ffreqimag[i], ffreqreal[i]);
                    }
                    this.modifyAngular(ffreqmaghalfin, ffreqanghalfin, ffreqmaghalfout, ffreqanghalfout);
                    for (int i=0; i<bufsize/2; i++)
                    {
                        ffreqreal[i] = ffreqmaghalfout[i]*Math.Sin(ffreqanghalfout[i]);
                        ffreqimag[i] = ffreqmaghalfout[i]*Math.Cos(ffreqanghalfout[i]);
                    }
                    double[] fbufout;
                    Fourier.RawFrequencyToSamples(out fbufout, ffreqreal, ffreqimag);
                    Array.Copy(fbufout, 0, wout.data[0], partnum*bufsize, bufsize);
                }
                return wout;
            }
            protected abstract void modifyAngular(double[] freqMag, double[] freqAng, double[] freqMagOut, double[] freqAngOut);
            
        }

        /*public class FourierModifier
        {
            int bufsize; bool bModRectangular = false;
            public FourierModifier(int bufsize) { this.bufsize=bufsize;
            }
            public WaveAudio modify(WaveAudio src)
            {
                WaveAudio wout = new WaveAudio(src.getSampleRate(), 1);
                wout.LengthInSamples = src.LengthInSamples;
                
                int BUFSIZE=bufsize;
                //re-use all of these buffers
                double[] ffreqrealhalfout=new double[max], ffreqimaghalfout=new double[max];
                for (int i=0; i<src.LengthInSamples/BUFSIZE; i++)
                {
                    double[] fbuffertime = new double[BUFSIZE];
                    Array.Copy(src.data[0], i*BUFSIZE, fbuffertime, 0, BUFSIZE);
                    double[] ffreqreal, ffreqimag;
                    Fourier.RawSamplesToFrequency(fbuffertime, out ffreqreal, out ffreqimag);
                    int max = ffreqreal.Length/2; //only modify first half.
                    double[] ffreqrealhalf=new double[max], ffreqimaghalf=new double[max];
                    Array.Copy(ffreqreal, ffreqrealhalf, max);
                    Array.Copy(ffreqimag, ffreqimaghalf, max);
                    
                    modifyRectangular
                }

            }
            protected virtual void modifyRectangular(double[] freqReal, double[] freqImag, double[] freqRealOut, double[] freqImagOut)
            {
            }
            protected virtual void modifyAngular(double[] freqMag, double[] freqAng, double[] freqMagOut, double[] freqAngOut)
            {

            }
        }

        public static void getFourierFrequencies(WaveAudio w, out double[] freqReal, out double[] freqImag, int bufsize)
        {

        }

        public static WaveAudio setFourierFrequencies(double[] freqReal, double[] freqImag)
        {

        }*/

    }
}
