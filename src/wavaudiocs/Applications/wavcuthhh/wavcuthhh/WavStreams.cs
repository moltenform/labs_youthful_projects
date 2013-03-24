using System;
using System.IO;

/* reads header from a .wav file and advances the stream position. */
public class CWavStreamReadHeaders
{
    public uint nCompleteLength = 0;
    public ushort nChannels = 0;
    public uint nSampleRate = 0;
    public uint byteRate = 0;
    public int blockAlign = 0;
    public ushort nBitsPerSample = 0;
    public ushort nAudioFormat = 0;
    public bool bIsValid = false;

    public static CWavStreamReadHeaders ReadWavStreamReadHeaders(BinaryReader r)
    {
        CWavStreamReadHeaders info = new CWavStreamReadHeaders();
        info.bIsValid = false;
        string riff = new string(r.ReadChars(4));
        if (!riff.Equals("RIFF"))
            throw new WavCutException("No 'RIFF' Tag, probably not a valid wav file.");

        info.nCompleteLength = r.ReadUInt32(); // (length of file in bytes) - 8

        string wave = new string(r.ReadChars(4));
        if (!wave.Equals("WAVE"))
            throw new WavCutException("No 'WAVE' tag, probably not a valid wav file.");

        string format = new string(r.ReadChars(4)); // assume that fmt tag is first
        if (!format.Equals("fmt "))
            throw new WavCutException("No 'fmt ' tag");

        uint size = r.ReadUInt32(); // size of fmt header
        if (size != 16)
            throw new WavCutException("Size of fmt header != 16");

        info.nAudioFormat = r.ReadUInt16(); // audio format. 1 refers to uncompressed PCM
       
        // read the format header
        info.nChannels = r.ReadUInt16();
        info.nSampleRate = r.ReadUInt32();
        info.byteRate = r.ReadUInt32();
        info.blockAlign = r.ReadUInt16();
        info.nBitsPerSample = r.ReadUInt16();
        info.bIsValid = true;
        return info;
    }
}

public interface IReadWaveData
{
    bool PreferRawSample();
    void ReceiveRawSample(byte s1a,byte s1b,byte s2a,byte s2b);
    void ReceiveProcessedSample(double sa,double sb);
    void OnFinished();
}

/* helper class for streaming through a wav file without loading into memory. */
public static class CWavStreamReader
{
    const int nBufferSize = 32768;
    public static void StreamThroughWaveFile(string sFileName,IReadWaveData objCallback)
    {
        using (FileStream fs = new FileStream(sFileName,FileMode.Open,FileAccess.Read))
        using (BufferedStream bs = new BufferedStream(fs, nBufferSize))
        using (BinaryReader r = new BinaryReader(bs))
        {
            StreamThroughWaveFile(r, objCallback);
        }
    }
    public static void StreamThroughWaveFile(BinaryReader r,IReadWaveData objCallback)
    {
        bool bRaw = objCallback.PreferRawSample();
        CWavStreamReadHeaders winfo = CWavStreamReadHeaders.ReadWavStreamReadHeaders(r);

        if (winfo.nAudioFormat != 1)
            throw new WavCutException("Only audio format 1 is supported");
        if (winfo.nSampleRate != 44100) throw new WavCutException("expect samplerate==44100");
        if (winfo.nBitsPerSample != 16) throw new WavCutException("expect nBitsPerSample==16");
        if (winfo.nChannels != 2) throw new WavCutException("expect nChannels==2");

        uint nDataSize = 0;
        while (true)
        {
            // Go through chunks. We are looking for "data" chunk
            string sDatatag = new string(r.ReadChars(4));
            nDataSize = r.ReadUInt32();
            if (nDataSize > int.MaxValue)
                throw new WavCutException("File too large.");

            if (sDatatag == "data") // found the data section
            {
                break;
            }
            else // something else, continue looping
            {
                r.BaseStream.Seek(nDataSize,SeekOrigin.Current);
            }
        }

        uint nLengthOfDataChunk = (uint)(nDataSize / (winfo.nChannels*(winfo.nBitsPerSample/8)));
        int nSample = 0;
        const int nReadsize = 1024;
        bool bComplete = false;

        // loop through wave until we reach the expected number of samples.
        // we should not read until eof because there might be a later riff chunk.
        while (!bComplete)
        {
            byte[] rawdata = r.ReadBytes((int)nReadsize);
            if (rawdata == null || rawdata.Length == 0)
                break;

            for (int j = 0; j < rawdata.Length; j += 4)
            {
                if (bRaw)
                {
                    objCallback.ReceiveRawSample(rawdata[j],rawdata[j + 1],rawdata[j + 2],rawdata[j + 3]);
                }
                else
                {
                    short sha1 = rawdata[j + 0]; // intel byte order
                    short sha2 = (short)(((short)rawdata[j + 1]) << 8);
                    short sha = (short)(sha1 + sha2);
                    double sa = sha / ((double)short.MaxValue);
                    short shb1 = rawdata[j + 2]; // intel byte order
                    short shb2 = (short)(((short)rawdata[j + 3]) << 8);
                    short shb = (short)(sha1 + sha2);
                    double sb = shb / ((double)short.MaxValue);
                    if (sa != sb)
                        throw new WavCutException("different here");

                    objCallback.ReceiveProcessedSample(sa,sb);
                }

                nSample++;
                if (nSample >= nLengthOfDataChunk)
                {
                    bComplete = true;
                    break;
                }
            }
        }
        if (nSample != nLengthOfDataChunk)
            Console.WriteLine("warning: eof reached earlier than expected. Expected "+nLengthOfDataChunk+" got "+nSample);

        objCallback.OnFinished();
    }
}

/* wav file writer. you must call .Save() */
public class CWaveWrite
{
    FileStream m_fs;
    BufferedStream m_bs;
    BinaryWriter m_w;
    byte[] m_arBuffer = new byte[16];
    int m_nSamplesWritten = 0;
    int m_nLengthExpected;
    const int nBufferSize = 4096;

    public CWaveWrite(string sFileDir,int nName,int nLength)
    {
        string sFullname = sFileDir + "\\out_" + nName + ".wav";

        m_fs = new FileStream(sFullname, FileMode.Create);
        m_bs = new BufferedStream(m_fs, nBufferSize);
        m_w = new BinaryWriter(m_bs);

        startWave(m_w,16 /*bits*/,nLength);
        m_nLengthExpected = nLength;
    }
    public void WriteRawSample(byte s1a,byte s1b,byte s2a,byte s2b)
    {
        m_arBuffer[0] = s1a;
        m_arBuffer[1] = s1b;
        m_arBuffer[2] = s2a;
        m_arBuffer[3] = s2b;
        m_w.Write(m_arBuffer,0,4);
        m_nSamplesWritten++;
    }
    public void WriteSample(double sa,double sb)
    {
        double dblvalue; short sh;
        dblvalue = (sa * short.MaxValue); // note that this is signed, so range is minval to maxval
        if (dblvalue > short.MaxValue) dblvalue = short.MaxValue;
        else if (dblvalue < short.MinValue) dblvalue = short.MinValue;
        sh = (short)dblvalue;
        m_arBuffer[0] = (byte)(sh & 0x00FF); // low byte
        m_arBuffer[1] = (byte)(sh >> 8); // high byte

        dblvalue = (sb * short.MaxValue); // note that this is signed, so range is minval to maxval
        if (dblvalue > short.MaxValue) dblvalue = short.MaxValue;
        else if (dblvalue < short.MinValue) dblvalue = short.MinValue;
        sh = (short)dblvalue;
        m_arBuffer[2] = (byte)(sh & 0x00FF); // low byte
        m_arBuffer[3] = (byte)(sh >> 8); // high byte

        m_w.Write(m_arBuffer,0,4);
        m_nSamplesWritten++;
    }
    public void Save()
    {
        if (m_nLengthExpected != m_nSamplesWritten)
            Console.WriteLine("Warning: Expected "+m_nLengthExpected+" samples, got "+m_nSamplesWritten);
        m_w.Flush();
        m_bs.Flush();
        m_fs.Flush();
        m_w.Close();
        m_bs.Close();
        m_fs.Close();
    }
    private void startWave(BinaryWriter w,int nBitsPerSample,int nSamples)
    {
        if (nBitsPerSample != 16)
            throw new NotImplementedException("Only 16 bit supported as of now...");

        int nChannels = 2;
        int nSampleRate = 44100;
        uint nDatasize = (uint)(nSamples * (nBitsPerSample / 8) * nChannels);
        uint nFilesize_minus8 = 4 + (8 + 16) + 8 + nDatasize; //header + fmt chunk + data chunk


        w.Write(new byte[] { (byte)'R',(byte)'I',(byte)'F',(byte)'F' });
        w.Write((uint)nFilesize_minus8); // size of data + headers

        w.Write(new byte[] { (byte)'W',(byte)'A',(byte)'V',(byte)'E' });
        w.Write(new byte[] { (byte)'f',(byte)'m',(byte)'t',(byte)' ' });
        w.Write((uint)16); //size is 16 bytes
        w.Write((ushort)1); //format 1
        w.Write((ushort)nChannels);  //nChannels
        w.Write((uint)nSampleRate); //SampleRate
        w.Write((uint)((nChannels * nBitsPerSample * nSampleRate) / 8)); //ByteRate
        w.Write((ushort)((nChannels * nBitsPerSample) / 8)); //BlockAlign
        w.Write((ushort)nBitsPerSample); //BitsPerSample 

        w.Write(new byte[] { (byte)'d',(byte)'a',(byte)'t',(byte)'a' });

        w.Write((uint)nDatasize); // Data size
    }

}

public class WavCutException : Exception
{
    private string m_sMessage;
    public WavCutException(string s) : base(s) { m_sMessage = s; }
    public WavCutException() { }
    public string GetMessage() { return m_sMessage; }
}

