using System;
using System.Collections.Generic;
using System.Text;
using CsWaveAudio;
using System.Windows.Forms;

namespace Blinkbeat
{
   public static class Blinkbeat
    {


       public static void CalculateBeatBlink(WaveAudio w, out double[][] results, out double normalizeFactor, out double threshold, out int nTimerInterval)
       {
          // w = Effects.Derivative(w); // makes it better, possibly.

           // do the fourier stuff
           int BUFSIZE = 2048;
           results = Fourier.SpectrumContentOverTime(w, 8, BUFSIZE);

           // guess what the highest value is for normalizing
           double max = -1;
           for (int i = 0; i < 8; i++)
               max = Math.Max(max, results[0][i]);
           normalizeFactor = max;

           // there isn't a significance to the number 70, it just seemed to work ok.
           threshold = 70.0;

          
           // This is in milliseconds, not exact, and so over time they will go out of sync.
           // (Not really any convienient way to prevent this.)
           nTimerInterval = (int)Math.Round((BUFSIZE / 44100.0) * 1000);


       }


      

    }
}
