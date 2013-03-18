/*
 * WavCutHHH
 * by Ben Fisher, 2011 http://halfhourhacks.blogspot.com
 * Released under GPL v3
 * 
 * Run wavcut.exe to see usage.
 * Supports drag and drop.
 * Doesn't load the entire file into memory, so it can handle large files.
 * In tests, all samples accounted for in output.
 * */

using System;
using System.Collections.Generic;
using System.IO;

public class CWavcutMain
{
    public void Start(string[] args)
    {
        // parse arguments. we expect exactly one .wav and exactly one .txt.
        string sWav = null, sTxt = null;
        if (args.Length > 2 && args[1] == "-check")
        {
            WavAnalyze.Go(args[2]);
            return;
        }
        if (args.Length > 1 && args[1].EndsWith(".txt")) sTxt = args[1];
        if (args.Length > 2 && args[2].EndsWith(".txt")) sTxt = args[2];
        if (args.Length > 1 && args[1].EndsWith(".wav")) sWav = args[1];
        if (args.Length > 2 && args[2].EndsWith(".wav")) sWav = args[2];

        // display usage.
        if (sWav == null || sTxt == null)
        {
            Console.WriteLine("You must provide both a .txt file and a .wav file. Drag and drop is supported.");
            Console.WriteLine("\n\nWavcut, by Ben Fisher, 2011");
            Console.WriteLine("Wavcut will split a large .wav file into several files based on a text file");
            Console.WriteLine("that specifies cut points.\n\n");
            Console.WriteLine("wavcut.exe input.wav input.txt\n");
            Console.WriteLine("First line of the .txt file is 'wavcut'.");
            Console.WriteLine("Remaining lines of the .txt file should be numbers, specifying where to cut.");
            Console.WriteLine("Refer to wavcut.exe trombone.wav trombone.txt as an example.\n\n");
            Console.WriteLine("To check wav contents, use\n");
            Console.WriteLine("wavcut.exe -check input.wav\n");
        }
        else
        {
            // start wavcut.
            string sOut = ".\\output";
            if (!Directory.Exists(sOut))
                sOut = ".";
            try
            {
                if (StartWavCut(sTxt, sWav, sOut))
                    Console.WriteLine("Completed successfully.");
                else
                    Console.WriteLine("Errors, did not complete.");
            }
            catch (WavCutException e)
            {
                Console.WriteLine("Exception: " + e.GetMessage());
            }
        }

        // waiting on a key makes it easier to use with drag/drop.
        Console.WriteLine("Press Enter to continue...");
        Console.ReadKey();
    }

    private bool StartWavCut(string sTxtInput, string sWavInput, string sOut)
    {
        if (!File.Exists(sTxtInput) || !File.Exists(sWavInput))
            throw new WavCutException("File does not exist.");

        string[] arLines = File.ReadAllLines(sTxtInput);
        if (arLines == null || arLines.Length < 3)
            throw new WavCutException("not enough lines");

        if (arLines[0] != "wavcut")
            throw new WavCutException("The first line of the input text file should be wavcut, see trombone.txt");

        // read input file
        List<int> arOffsets = new List<int>();
        int nPrevframe = 0;
        for (int i = 1; i < arLines.Length; i++)
        {
            int nFrame;
            if (arLines[i] == "" || arLines[i] == null) continue;
            if (arLines[i].Contains("."))
                throw new WavCutException("We don't support decimals");
            if (!int.TryParse(arLines[i],out nFrame))
                throw new WavCutException("Line" + (i + 1) + " could not parse int");
            
            arOffsets.Add(nFrame);
            if (nFrame <= nPrevframe) throw new WavCutException("samples must be increasing");
            nPrevframe = nFrame;
        }
        if (arOffsets.Count <= 0) throw new WavCutException("Error, no points given");

        // count number of samples
        int nSamples;
        {
            CCountWaveSamples counter = new CCountWaveSamples();
            CWavStreamReader.StreamThroughWaveFile(sWavInput,counter);
            nSamples = counter.GetCount();
            if (nSamples <= 0) throw new WavCutException("Error, could not read samples");
        }
        arOffsets.Add(nSamples);

        // construct objects that specify the intervals
        List<WaveSegment> arSegments = new List<WaveSegment>();
        for (int i = 0; i < arOffsets.Count; i++)
        {
            if (nSamples <= arOffsets[i] && i<arOffsets.Count-1)
                throw new WavCutException("Error, offset is greater than length of file");
            int length = (i == 0) ? arOffsets[i] : arOffsets[i] - arOffsets[i - 1];
            Console.WriteLine("track " + i+ " length(s) "+length/44100.0);
            WaveSegment seg = new WaveSegment();
            seg.m_writer = new CWaveWrite(sOut,i,length);
            seg.m_nStopPoint = arOffsets[i];
            arSegments.Add(seg);
        }

        // stream through the audio
        CCutWaveProcessor processor = new CCutWaveProcessor(arSegments);
        CWavStreamReader.StreamThroughWaveFile(sWavInput,processor);
        if (nSamples != processor.GetNumberSamplesSeen())
            Console.WriteLine("Warning, processor did not see all samples.");

        // save files and close file handles.
        for (int i = 0; i < arSegments.Count; i++)
        {
            arSegments[i].m_writer.Save();
        }

        return true;
    }
}

/* class passes each incoming sample forward to the one of the output wav files. */
public class CCutWaveProcessor : IReadWaveData
{
    List<WaveSegment> m_list;
    int m_whichTrack = 0;
    int m_nSample = 0;
    public CCutWaveProcessor(List<WaveSegment> list) { m_list = list; }
    public int GetNumberSamplesSeen() { return m_nSample; }

    /* IReadWaveData implementation */
    public bool PreferRawSample() { return true; }
    public void ReceiveRawSample(byte s1a, byte s1b, byte s2a, byte s2b)
    {
        if (m_nSample == m_list[m_whichTrack].m_nStopPoint)
            m_whichTrack++;

        m_list[m_whichTrack].m_writer.WriteRawSample(s1a,s1b,s2a,s2b);
        m_nSample++;
    }
    public void ReceiveProcessedSample(double sa,double sb) { throw new NotImplementedException(); }
    public void OnFinished() { }
}

/* class simply counts the number of samples. */
public class CCountWaveSamples : IReadWaveData
{
    int m_n = 0;
    int m_nResult = -1;
    public int GetCount() { return m_nResult; }

    /* IReadWaveData implementation */
    public bool PreferRawSample() { return true; }
    public void ReceiveRawSample(byte s1a,byte s1b,byte s2a,byte s2b) { m_n++; }
    public void ReceiveProcessedSample(double sa,double sb) { m_n++; }
    public void OnFinished() { m_nResult = m_n; }
}

public struct WaveSegment
{
    public CWaveWrite m_writer;
    public int m_nStopPoint;
}


class Program
{
    static void Main(string[] args)
    {
        CWavcutMain main = new CWavcutMain();
        main.Start(Environment.GetCommandLineArgs());
    }
}
