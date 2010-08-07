using System;
using System.Collections.Generic;
using System.Text;

namespace CsWaveAudio
{
    //Effects from Audacity, as opposed to the ones I came up with myself.

    //These could easily be made to act in-place, but for consistency with other effects, they return a copy
    public static partial class Effects
    {
        public static WaveAudio Wahwah(WaveAudio wOriginal) { return Wahwah(wOriginal, 1.5, 0.7, 0.1, 2.5);   }
        public static WaveAudio Wahwah(WaveAudio wOriginal, double freq) { return Wahwah(wOriginal, freq, 0.7, 0.1, 2.5); }
        public static WaveAudio Wahwah(WaveAudio wOriginal, double freq, double depth, double freqofs, double res)
        {
            double startphaseleft = 0;
            double startphaseright = startphaseleft + Math.PI; //note that left and right channels should start pi out of phase
            double freq_scaled = 2.0 * Math.PI * freq / (double)wOriginal.getSampleRate();

            WaveAudio w = wOriginal.Clone();
            if (w.getNumChannels() == 1)
                effect_wahwahaud_impl(w.data[0], startphaseleft, freq_scaled, depth, freqofs, res);
            else
            {
                effect_wahwahaud_impl(w.data[0], startphaseleft, freq_scaled, depth, freqofs, res);
                effect_wahwahaud_impl(w.data[1], startphaseright, freq_scaled, depth, freqofs, res);
            }
            return w;
        }

        public static WaveAudio Phaser(WaveAudio wOriginal) { return Phaser(wOriginal, .7, -60, 130, 4, 255); }
        public static WaveAudio Phaser(WaveAudio wOriginal, double freq) { return Phaser(wOriginal, freq, -60, 130, 4, 255); }
        public static WaveAudio Phaser(WaveAudio wOriginal, double freq, double fb, int depth, int stages, int drywet)
        {
            double startphaseleft = 0;
            double startphaseright = startphaseleft + Math.PI; //note that left and right channels should start pi out of phase
            double freq_scaled = 2.0 * Math.PI * freq / (double)wOriginal.getSampleRate();

            WaveAudio w = wOriginal.Clone();
            if (w.getNumChannels() == 1)
                effect_phaseraud_impl(w.data[0], freq_scaled, startphaseleft, fb, depth, stages, drywet);
            else
            {
                effect_phaseraud_impl(w.data[0], freq_scaled, startphaseleft, fb, depth, stages, drywet);
                effect_phaseraud_impl(w.data[1], freq_scaled, startphaseright, fb, depth, stages, drywet);
            }
            return w;
        }

        private const int fxphaseraudlfoskipsamples = 20;
        private const double fxphaseraudlfoshape = 4.0;
        private const int fxphaseraudMAX_STAGES = 24;
        internal static void effect_phaseraud_impl(double[] data, double freq_scaled, double startphase, double fb, int depth, int stages, int drywet)
        {
                int length = data.Length;
                if (data==null) return;
            // state variables	
            ulong skipcount;
            double[] old = new double[fxphaseraudMAX_STAGES];
            double gain;
            double fbout;
            double lfoskip;
            double phase;

            // initialize state variables
            int i, j;
            for (j = 0; j < stages; j++) old[j] = 0;   
            skipcount = 0;
            gain = 0;
            fbout = 0;
            lfoskip = freq_scaled; //not in Hz, already converted.
            phase = startphase;

            double m, tmp, in_f, out_f;
            for (i = 0; i < length; i++)
            {
	            in_f = data[i];

	            m = in_f + fbout * fb / 100;
	            if (((skipcount++) % fxphaseraudlfoskipsamples) == 0) 
	            {
		            gain = (1 + Math.Cos(skipcount * lfoskip + phase)) / 2; //compute sine between 0 and 1
                    gain = (Math.Exp(gain * fxphaseraudlfoshape) - 1) / (Math.Exp(fxphaseraudlfoshape) - 1); // change lfo shape
		            gain = 1 - gain / 255 * depth;      // attenuate the lfo
	            }
	            // phasing routine
	            for (j = 0; j < stages; j++)
	            {
		            tmp = old[j];
		            old[j] = gain * tmp + m;
		            m = tmp - gain * old[j];
	            }
	            fbout = m;
	            out_f = (m * drywet + in_f * (255 - drywet)) / 255;
            	
	            if (out_f < -1.0) out_f = -1.0; // Prevent clipping
	            else if (out_f > 1.0) out_f = 1.0;
	            data[i] = out_f;
            }
        }


            /* Parameters:
       freq - LFO frequency 
       startphase - LFO startphase in RADIANS - usefull for stereo WahWah
       depth - Wah depth
       freqofs - Wah frequency offset
       res - Resonance
       depth and freqofs should be from 0(min) to 1(max) !
       res should be greater than 0 !  */
        private const int fxwahwahaudlfoskipsamples = 30;
        internal static void effect_wahwahaud_impl(double[] data, double startphase, double freq_scaled, double depth, double freqofs, double res)
        {
	        if (data==null) return;
            int length = data.Length;
	        // state variables
	        double phase;
	        double lfoskip;
	        ulong skipcount;
	        double xn1, xn2, yn1, yn2;
	        double b0, b1, b2, a0, a1, a2;

	        // initialize variables
	        lfoskip = freq_scaled; //already converted from Hz
	        skipcount = 0;
	        xn1 = 0; xn2 = 0; yn1 = 0; yn2 = 0; b0 = 0;  b1 = 0; b2 = 0; a0 = 0; a1 = 0;  a2 = 0;
	        phase = startphase;

	            double frequency, omega, sn, cs, alpha;
	            double in_f, out_f;
	            int i;
	            for (i = 0; i < length; i++)
	            {
		            in_f = data[i];

		            if ((skipcount++) % fxwahwahaudlfoskipsamples == 0)
		            {
			            frequency = (1 + Math.Cos(skipcount * lfoskip + phase)) / 2;
			            frequency = frequency * depth * (1 - freqofs) + freqofs;
                        frequency = Math.Exp((frequency - 1) * 6);
			            omega = Math.PI * frequency;
                        sn = Math.Sin(omega);
                        cs = Math.Cos(omega);
			            alpha = sn / (2 * res);
			            b0 = (1 - cs) / 2;
			            b1 = 1 - cs;
			            b2 = (1 - cs) / 2;
			            a0 = 1 + alpha;
			            a1 = -2 * cs;
			            a2 = 1 - alpha;
		            }
		            out_f = (b0 * in_f + b1 * xn1 + b2 * xn2 - a1 * yn1 - a2 * yn2) / a0;
		            xn2 = xn1;
		            xn1 = in_f;
		            yn2 = yn1;
		            yn1 = out_f;
            		
		            if (out_f < -1.0) out_f = -1.0; // Prevent clipping
		            else if (out_f > 1.0) out_f = 1.0;
		            data[i] = out_f;
	        }
        }

    }
}
