using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

// actually -- instead of positioning halfway in between the grooves
// I should lean towards 2seconds from the rightmost point
// fixes buffering issue

class CWavcutFindSilence
{
    private const double g_cutoff = 0.0001;
    private const int nFudgeDelay = 4 * 44100; //2 of these are from buffer
    public static void FindSilence(string sWavInput,string[] arLines)
    {
        /*
         * 1- create 1024-averaged array, but use a list<double> to make sure last points are caught.
         * go from left to right.
         * if a transition up to down is seen, look at the next 10 seconds of audio.
         * Find the longest consecutive period of quietness, and 
         * place the split point right in the middle of that period.
         * 
         * This will help with noisy transitions.
         * 
         * 
         * /



        int nSamples = CCountWaveSamples.CountWaveSamples(sWavInput);
        List<int> nLengths = new List<int>();
        for (int i=1; i<arLines.Length; i++)
        {
            int nLength;
            if (arLines[i].Contains("."))
                throw new WavCutException("line contains . "+arLines[i]);
            if (!int.TryParse(arLines[i], out nLength))
                throw new WavCutException("could not parse integer, "+arLines[i]);
            if (i!=1) nLength += nFudgeDelay; //first doesn't get the push
            nLengths.Add(nLength);
        }

        CWaveWrite writetemp = null; // new CWaveWrite(".",90,nSamples /*toomuch...*/);

        double[] arr;
        {
            CGetAmplitudes counter = new CGetAmplitudes(nSamples);
            counter.m_dgbwrite = writetemp;
            CWavStreamReader.StreamThroughWaveFile(sWavInput,counter);
            arr = counter.m_arr;
        }

        //Debugger.Break();
List<int> arCutpoints = new List<int>();
        int nCurrentSample = 0;
        for (int i=0; i<nLengths.Count; i++)
        {
            nCurrentSample += nLengths[i];
            int nIndex = nCurrentSample / CGetAmplitudes.m_nWindowSize;
            Console.WriteLine("Position is "+nCurrentSample/44100.0+"s");
            // get -4s to 4s
            int nWindowSizeIndex = (int)((44100.0 * 8) / CGetAmplitudes.m_nWindowSize);
            int nStartDown=-1, nStopDown=-1;
            int jstart = Math.Max(0,nIndex-nWindowSizeIndex),jend = Math.Min(arr.Length-1,nIndex+nWindowSizeIndex);
            if (arr[jstart] < g_cutoff)
                throw new WavCutException("Findsilence: already silent before 4s");
            if (arr[jend] < g_cutoff)
                throw new WavCutException("Findsilence: silent after 4s");
            for (int j=jstart+1; j<jend; j++)
            {
                if (arr[j]<g_cutoff && arr[j-1]>=g_cutoff)
                {
                    if (nStartDown != -1)
                        throw new WavCutException("Find silence: two transitions");
                    else
                        nStartDown = j;
                }
                else if (arr[j]>=g_cutoff && arr[j-1]<g_cutoff)
                {
                    nStopDown = j;
                }
            }
            if (nStartDown==-1)
                throw new WavCutException("Find silence: no silence found");
            if (nStopDown==-1 || nStartDown>= nStopDown)
                throw new WavCutException("Find silence: error - stopdown==-1");
            // int nNewPosition = ((nStopDown+nStartDown)/2)*CGetAmplitudes.m_nWindowSize;
            // if possible, go 2s from the stop point
            int nNewPosIndex = Math.Max((nStopDown+nStartDown)/2, nStopDown - (2*44100*CGetAmplitudes.m_nWindowSize));
            int nNewPosition = (nNewPosIndex)*CGetAmplitudes.m_nWindowSize;
            Console.WriteLine("Adjusting position by: "+(nNewPosition-nCurrentSample)/44100.0 + "s");
            nCurrentSample = nNewPosition;
            arCutpoints.Add(nNewPosIndex);
        }
        TextWriter tw = new StreamWriter("~temp.txt");
        tw.WriteLine("wavcut");
        foreach (int n in arCutpoints)
            tw.WriteLine(""+n);
        tw.Close();
        

    }

    /* class gets amplitudes */
    public class CGetAmplitudes : IReadWaveData
    {
        public const int m_nWindowSize = 1024;
        public CWaveWrite m_dgbwrite = null;
        int m_nSamples = 0;
        public double[] m_arr; public int m_arrptr=0;
        double[] m_buffer; int m_bufptr = 0;
        double m_fRunningAvg = 0.0;
        int m_n = 0;
        public CGetAmplitudes(int nSamples)
        {
            m_nSamples = nSamples;
            m_arr = new double[nSamples / m_nWindowSize + 2];
            m_buffer = new double[m_nWindowSize];
        }

        /* IReadWaveData implementation */
        public bool PreferRawSample() { return false; }
        public void ReceiveRawSample(byte s1a,byte s1b,byte s2a,byte s2b) {
            throw new NotImplementedException();
        }
        public void ReceiveProcessedSample(double sa,double sb) {
            double fsqr = sa*sa + sb*sb;
            double fprev = m_buffer[m_bufptr];
            m_buffer[m_bufptr] = fsqr;
            m_bufptr = (m_bufptr+1)%(m_nWindowSize);
            m_fRunningAvg += fsqr;
            m_fRunningAvg -= fprev;

            if (m_n % m_nWindowSize == 0)
                if (m_dgbwrite!=null)
                    m_dgbwrite.WriteSample(m_fRunningAvg,m_fRunningAvg > 0.0001 ? 0.7 : 0.1);

            m_n++;
            if (m_n % m_nWindowSize == 0)
                m_arr[m_arrptr++] = m_fRunningAvg; // / m_nWindowSize;
        }
        public void OnFinished() {  }

            
    }


    private static void finalize(string sWavInput)
    {
        // write '~temp.txt'
        CWavcutMain main = new CWavcutMain();
        main.Start(new string[] { "wavcut.exe","~temp.txt",sWavInput });

        // now, try to delete the temp file.
        try
        {
            File.Delete("~temp.txt");
        }
        catch (Exception e)
        {
            Console.WriteLine("failed to delete temporary ~temp.txt "+e.Message);
        }

    }
}

