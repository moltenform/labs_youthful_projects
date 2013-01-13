using System;
using System.Collections.Generic;
using System.Text;

namespace CsWaveAudio
{
    public static class ConstructionUtil
    {
        public static void placeAudio(WaveAudio target, WaveAudio source, int index)
        {
            for (int ch=0; ch<target.data.Length; ch++)
                for (int j = 0; j < source.data[0].Length; j++)
                    if (j + index < target.data[0].Length)
                    {
                        target.data[ch][j + index] += source.data[ch][j];
                    }
        }
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

        public static double[] getAmplitudesOverTimeSegment(WaveAudio w, int nPieceSize)
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
        
        //get continuous value, with a window. the 'instantaneous amplitude'
        //note: only from 1st channel of audio.
        public static double[] getAmplitudesOverTimeContinuous(WaveAudio w, int nWindowSize)
        {
            double[] res = new double[w.LengthInSamples];
            double current=0;
            for (int i=0; i<w.LengthInSamples; i++)
            {
                current += w.data[0][i]*w.data[0][i];
                if (i>nWindowSize)
                    current-= w.data[0][i-nWindowSize]*w.data[0][i-nWindowSize];

                res[i] = current / nWindowSize;
            }
            return res;
        }
        public static double getAvgAmplitude(WaveAudio w)
        {
            double total=0;
            for (int i=0; i<w.LengthInSamples; i++)
                total += w.data[0][i]*w.data[0][i];
            return total/w.LengthInSamples;
        }
        public static double getLargest(double[] d, out int index)
        {
            index=0;
            double largest = double.NegativeInfinity;
            if (d.Length==0) return 0;
            for (int i=0; i<d.Length; i++)
                if (d[i]>largest) { largest=d[i]; index=i; }
            return largest;
        }
        public static double getSmallest(double[] d, out int index)
        {
            index=0;
            double smallest = double.PositiveInfinity;
            if (d.Length==0) return 0;
            for (int i=0; i<d.Length; i++)
                if (d[i]<smallest) { smallest=d[i]; index=i; }
            return smallest;
        }



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

        public class FModCutFrequencies : FourierModifier
        {
            double cutOffLow, cutOffHi;
            public FModCutFrequencies(double cutOffLowin, double cutOffHiin) { this.cutOffHi = cutOffHiin; this.cutOffLow = cutOffLowin; }
            protected override void modifyAngular(double[] freqMag, double[] freqAng, double[] freqMagOut, double[] freqAngOut)
            {
                
                //well, bufsize/2 is 22,050 hz.
                int cLow = (int)((cutOffLow/22050.0) * (freqAng.Length));
                int cHigh = (int)((cutOffHi/22050.0) * (freqAng.Length));
                for (int i=0; i<freqMagOut.Length; i++)
                {
                    if (i>cLow && i<cHigh)
                    {
                        freqAngOut[i] = freqAng[i];
                        freqMagOut[i] = freqMag[i];
                    }
                    else
                    {
                        freqAngOut[i] = 0;
                        freqMagOut[i] = 0;
                    }
                }
            }
        }

        public abstract class FourierModifierSmooth
        {
            public int overlap = 4, overlapRamp = 4;
            public FourierModifierSmooth() { }
            public FourierModifierSmooth(int noverlap) { overlap=overlapRamp =noverlap; }
            public FourierModifierSmooth(int noverlap, int noverlapRamp) { overlap=noverlap; overlapRamp= noverlapRamp; }
            public WaveAudio doModify(WaveAudio w) { return doModify(w, 2048); }
            public WaveAudio doModify(WaveAudio src, int bufsize) 
            {
                WaveAudio wout = new WaveAudio(src.getSampleRate(), 1);
                wout.LengthInSamples = src.LengthInSamples;
                
                //reuse the buffers.
                double[] ffreqmaghalfout=new double[bufsize/2], ffreqanghalfout=new double[bufsize/2];
                double[] ffreqmaghalfin=new double[bufsize/2], ffreqanghalfin=new double[bufsize/2];
                double[] fbuffertime = new double[bufsize];
                
                for (int index=0; index<src.LengthInSamples-bufsize; index+=bufsize/overlap)
                {
                    //copy into buffer.
                    Array.Copy(src.data[0], index, fbuffertime, 0, bufsize);
                    double[] ffreqreal, ffreqimag;
                    Fourier.RawSamplesToFrequency(fbuffertime, out ffreqreal, out ffreqimag);
                    //we only care about the first half of these results.
                    for (int i=0; i<bufsize/2; i++)
                    {
                        ffreqmaghalfin[i] = Math.Sqrt(ffreqreal[i]*ffreqreal[i]+ffreqimag[i]*ffreqimag[i]);
                        ffreqanghalfin[i] = Math.Atan2(ffreqimag[i], ffreqreal[i]);
                    }
                    this.modifyAngular(ffreqmaghalfin, ffreqanghalfin, ffreqmaghalfout, ffreqanghalfout);
                    for (int i=0; i<ffreqreal.Length/2; i++)
                    {
                        ffreqreal[i] = ffreqmaghalfout[i]*Math.Sin(ffreqanghalfout[i]);
                        ffreqimag[i] = ffreqmaghalfout[i]*Math.Cos(ffreqanghalfout[i]);
                    }
                    for (int i=ffreqreal.Length/2; i<ffreqreal.Length; i++)
                    {
                        ffreqreal[i]=ffreqimag[i]=0;
                    }
                    double[] fbufout;
                    Fourier.RawFrequencyToSamples(out fbufout, ffreqreal, ffreqimag);
                    WaveAudio ww = new WaveAudio(44100, 1);
                    ww.data[0] = fbufout;
                    //Array.Copy(fbufout, 0, wout.data[0], partnum*bufsize, bufsize);
                    ConstructionUtil.placeAudioRamp(wout, ww, index, (bufsize/overlapRamp));
                }
                return wout;
            }
            protected abstract void modifyAngular(double[] freqMag, double[] freqAng, double[] freqMagOut, double[] freqAngOut);
            private string sPlotDir = null; //default to not drawing plots
            public void drawPlots(string sDir) {this.sPlotDir=sDir; }
            protected void drawPlots(double[] a1, double[] a2, double[] a3, double[] a4)
            {
                if (sPlotDir==null) return;
                ConstructionUtil.plotArray(a1,sPlotDir+"\\data1.js",-1,true);
                ConstructionUtil.plotArray(a2,sPlotDir+"\\data3.js",-1,false);
                ConstructionUtil.plotArray(a3,sPlotDir+"\\data2.js",-1,true);
                ConstructionUtil.plotArray(a4,sPlotDir+"\\data4.js",-1,false);
            }
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
                    if (partnum==0) this.drawPlots(ffreqmaghalfin, ffreqanghalfin, ffreqmaghalfout, ffreqanghalfout);
                    for (int i=0; i<ffreqreal.Length/2; i++)
                    {
                        ffreqreal[i] = ffreqmaghalfout[i]*Math.Sin(ffreqanghalfout[i]);
                        ffreqimag[i] = ffreqmaghalfout[i]*Math.Cos(ffreqanghalfout[i]);
                    }
                    for (int i=ffreqreal.Length/2; i<ffreqreal.Length; i++)
                    {
                        ffreqreal[i]=ffreqimag[i]=0;
                    }
                    double[] fbufout;
                    Fourier.RawFrequencyToSamples(out fbufout, ffreqreal, ffreqimag);
                    Array.Copy(fbufout, 0, wout.data[0], partnum*bufsize, bufsize);
                }
                return wout;
            }
            protected abstract void modifyAngular(double[] freqMag, double[] freqAng, double[] freqMagOut, double[] freqAngOut);
            private string sPlotDir = null; //default to not drawing plots
            public void drawPlots(string sDir) { this.sPlotDir=sDir; }
            protected void drawPlots(double[] a1, double[] a2, double[] a3, double[] a4)
            {
                if (sPlotDir==null) return;
                ConstructionUtil.plotArray(a1, sPlotDir+"\\data1.js", -1,true);
                ConstructionUtil.plotArray(a2,sPlotDir+"\\data3.js",-1,false);
                ConstructionUtil.plotArray(a3,sPlotDir+"\\data2.js",-1,true);
                ConstructionUtil.plotArray(a4,sPlotDir+"\\data4.js",-1,false);
            }
        }

        public abstract class FourierModifierRectangular
        {
            public WaveAudio doModify(WaveAudio w) { return doModify(w, 2048); }
            public WaveAudio doModify(WaveAudio src, int bufsize)
            {
                WaveAudio wout = new WaveAudio(src.getSampleRate(), 1);
                wout.LengthInSamples = src.LengthInSamples;

                //reuse the buffers.
                double[] freqRealIn=new double[bufsize/2], freqRealOut=new double[bufsize/2];
                double[] freqImagIn=new double[bufsize/2], freqImagOut=new double[bufsize/2];
                double[] fbuffertime = new double[bufsize];
                for (int partnum=0; partnum<src.LengthInSamples/bufsize; partnum++)
                {
                    //copy into buffer.
                    Array.Copy(src.data[0], partnum*bufsize, fbuffertime, 0, bufsize);
                    double[] ffreqreal, ffreqimag;
                    Fourier.RawSamplesToFrequency(fbuffertime, out ffreqreal, out ffreqimag);
                    //we only care about the first half of these results.
                    Array.Copy(ffreqreal, freqRealIn, bufsize/2);
                    Array.Copy(ffreqimag, freqImagIn, bufsize/2);
                    this.modifyRectangular(freqRealIn, freqImagIn, freqRealOut, freqImagOut);
                    Array.Copy(freqRealOut, ffreqreal, bufsize/2);
                    Array.Copy(freqImagOut, ffreqimag, bufsize/2);
                    double[] fbufout;
                    Fourier.RawFrequencyToSamples(out fbufout, ffreqreal, ffreqimag);
                    Array.Copy(fbufout, 0, wout.data[0], partnum*bufsize, bufsize);
                }
                return wout;
            }
            protected abstract void modifyRectangular(double[] freqRealIn, double[] freqImagIn, double[] freqRealOut, double[] freqImagOut);
        }


        /// <summary>
        /// Get Interpolated value. Values outside the range will result in defaulting to nearest sample.
        /// </summary>
        /// <param name="sampleData">Data</param>
        /// <param name="sampleIndex">Floating-point index</param>
        /// <returns></returns>
        public static double getI(double[] sampleData, double sampleIndex)
        {
            if (sampleIndex > sampleData.Length - 1) sampleIndex = sampleData.Length - 1;
            else if (sampleIndex < 0 + 1) sampleIndex = 0;

            double proportion = sampleIndex - Math.Truncate(sampleIndex);
            double v1 = sampleData[(int)Math.Truncate(sampleIndex)];
            double v2 = sampleData[(int)Math.Ceiling(sampleIndex)];
            return v2 * proportion + v1 * (1 - proportion);
        }

        public static double[] cutArray(double[] arin, int everyother)
        {
            double[] arout = new double[arin.Length/everyother + 1];
            for (int i=0; i<arin.Length; i+= everyother)
                arout[i/everyother] = arin[i];
            return arout;
        }
        
        public static bool plotArray(double[] ar, string sFilename) { return plotArray(ar, sFilename, -1, false); }
        public static bool plotArray(double[] ar,string sFilename,int nApproxSize) { return plotArray(ar,sFilename,nApproxSize, false); }
        public static bool plotArray(double[] ar, string sFilename, int nApproxSize, bool bLogscale)
        {
            int skip = 1;
            if (nApproxSize > 0) skip = (int)((double)ar.Length / nApproxSize);
            skip = Math.Max(skip,1);
            if (ar==null) return false;
            using (System.IO.TextWriter tw = new System.IO.StreamWriter(sFilename))
            {
               tw.Write("data = [");
               for (int i=0; i<ar.Length; i+= skip)
                   if (!bLogscale)
                       tw.Write("{x:"+i+",y:"+ar[i].ToString(System.Globalization.CultureInfo.InvariantCulture)+"},");
                   else
                       tw.Write("{x:"+Math.Log(i+1, 2).ToString(System.Globalization.CultureInfo.InvariantCulture)+",y:"+Math.Log(ar[i]+1,2).ToString(System.Globalization.CultureInfo.InvariantCulture)+"},");
               tw.Write("];");
            }
            return true;
        }
        public delegate double FN_1(double x);
        public static bool plotFunction(FN_1 fn, string sFilename, double x1, double x2, int nPoints)
        {
            double[] ar = new double[nPoints];
            for (int i = 0; i < nPoints; i++)
            {
                ar[i] = fn( (((double)i)/nPoints)* (x2-x1) + x1 );
            }
            return plotArray(ar, sFilename);
        }

        public static void expectLength(WaveAudio w, double seconds)
        {
            if (Math.Abs(w.LengthInSeconds - seconds)>0.1)
                throw new Exception("Length is "+w.LengthInSeconds+", "+seconds+" expected");
        }
    }
}
