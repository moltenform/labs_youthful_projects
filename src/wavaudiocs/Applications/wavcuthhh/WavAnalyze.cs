using System;
using System.IO;

/* class displays wav file information to user. */
public class WavAnalyze
{
    public static void Go(string sFile)
    {
        if (!File.Exists(sFile))
        {
            Console.WriteLine("File doesn't seem to exist.");
        }
        else
        {
            FileInfo fInfo = new FileInfo(sFile);
            long nLength = fInfo.Length;
            try
            {
                StartWavAnalyze(sFile,nLength);
            }
            catch (WavCutException e)
            {
                Console.WriteLine("Exception: " + e.GetMessage());
            }
        }
    }

    const int nBufferSize = 32768;
    private static void StartWavAnalyze(string strFileName,long nLength)
    {
        using (FileStream fs = new FileStream(strFileName,FileMode.Open,FileAccess.Read))
        using (BufferedStream bs = new BufferedStream(fs,nBufferSize))
        using (BinaryReader r = new BinaryReader(bs))
        {
            StartWavAnalyze(r,nLength);
        }
    }
    public static void StartWavAnalyze(BinaryReader r, long nActualLength)
    {
        CWavStreamReadHeaders info = CWavStreamReadHeaders.ReadWavStreamReadHeaders(r);

        // display header info
        Console.WriteLine("Audio format: " + info.nAudioFormat + " (1 is uncompressed PCM)");
        Console.WriteLine("Sample rate: " + info.nSampleRate);
        Console.WriteLine("BitsPerSample: " + info.nBitsPerSample);
        Console.WriteLine("Channels: " + info.nChannels);
        if (nActualLength != info.nCompleteLength+8)
            Console.WriteLine("Warning: length of file is "+nActualLength+" but expected "+info.nCompleteLength);

        while (true)
        {
            // are we at the end of the file? if so, exit loop
            byte[] arTest = r.ReadBytes(4);
            if (arTest.Length == 0)
                break;
            else
                r.BaseStream.Seek(-arTest.Length,SeekOrigin.Current);

            // read the next chunk
            string sDatatag = new string(r.ReadChars(4));
            uint nDataSize = r.ReadUInt32();
            Console.WriteLine("TYPE:" + sDatatag + " SIZE:" + nDataSize);
            if (sDatatag == "data")
            {
                long nLengthInSamples = nDataSize / (info.nChannels * (info.nBitsPerSample / 8));
                Console.WriteLine("\tlength in samples " + nLengthInSamples +
                    " length in secs " + (nLengthInSamples / ((double)info.nSampleRate)));
            }
            if (sDatatag != "data" && sDatatag != "LIST")
                Console.WriteLine("warning, datatag is not 'data' or 'LIST'.");
            r.BaseStream.Seek(nDataSize, SeekOrigin.Current);
        }
        Console.WriteLine("Looks ok.");
    }
}
