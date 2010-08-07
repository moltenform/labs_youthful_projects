//Ben Fisher, 2008
//Based on published format and some examples at www.codeproject.com
//GPL

using System;
using System.IO;

namespace CsWaveAudio
{
    public partial class WaveAudio
    {
        // Implementation of loading wave file
        private void loadWave(BinaryReader r)
        {
            string riff = new string(r.ReadChars(4));
            if (!riff.Equals("RIFF"))
                throw new Exception("No 'RIFF' Tag, probably not a valid wav file.");

            uint length = r.ReadUInt32(); // (length of file in bytes) - 8

            string wave = new string(r.ReadChars(4));
            if (!wave.Equals("WAVE"))
                throw new Exception("No 'WAVE' tag, probably not a valid wav file.");

            string format = new string(r.ReadChars(4)); // We assume that fmt tag is first
            if (!format.Equals("fmt "))
                throw new Exception("No 'fmt ' tag");

            uint size = r.ReadUInt32(); // size of fmt header
            if (size != 16)
                throw new Exception("Size of fmt header != 16");

            ushort audioformat = r.ReadUInt16(); // audio format. 1 refers to uncompressed PCM
            if (audioformat != 1)
                throw new Exception("Only audio format 1 is supported");

            ushort nChannels = r.ReadUInt16();
            uint nSampleRate = r.ReadUInt32();
            this.m_currentSampleRate = (int)nSampleRate;
            uint byteRate = r.ReadUInt32();
            int blockAlign = r.ReadUInt16();
            ushort nBitsPerSample = r.ReadUInt16();

            uint dataSize = 0;
            byte[] rawdata;

            int MAXTRIES = 100, nTry = 0;
            for (nTry = 0; nTry < MAXTRIES; nTry++)
            {
                // Go through chunks. We are looking for "data" chunk
                string datatag = new string(r.ReadChars(4));
                dataSize = r.ReadUInt32();
                if (dataSize > int.MaxValue) throw new Exception("Data size too large.");

                if (datatag == "data") // found what we are looking for
                {
                    break;
                }
                else // something else, continue looping
                {
                    r.ReadBytes((int)dataSize);
                }
            }
            if (nTry >= MAXTRIES)
                throw new Exception("Could not find data tag");

            rawdata = r.ReadBytes((int)dataSize); // data size is length of this in bytes

            //if (rawdata.Length != dataSize)
            //   throw new Exception("Could not read correct number of bytes");
            // It would be incorrect if the lengths don't match, but it probably wouldn't cause failure.

            // The next step is to convert data to doubles

            int nSamples = ((8 * rawdata.Length) / (nBitsPerSample * nChannels));

            // Create array of samples for each channel
            double[][] newdata = new double[nChannels][];
            for (int i = 0; i < nChannels; i++)
                newdata[i] = new double[nSamples];

            // Go through data ... this is probably lossless because the precision of doubles is pretty high
            if (nBitsPerSample == 8)
            {
                if (nChannels == 1) // this is an easier case to deal with
                {
                    for (int i = 0; i < rawdata.Length; i += 1)
                        newdata[0][i] = (rawdata[i] / 256.0) * 2.0 - 1.0;   ////note: stored as unsigned.
                }
                else
                {
                    for (int i = 0; i < rawdata.Length; i += 1)
                        newdata[i % nChannels][i / 2] = (rawdata[i] / 256.0) * 2.0 - 1.0;
                }
            }
            else if (nBitsPerSample == 16) //note: signed
            {
                if (nChannels == 1)
                {
                    for (int i = 0; i < rawdata.Length; i += 2)
                    {
                        short sh1 = rawdata[i]; // assumes intel byte order
                        short sh2 = (short)(((short)rawdata[i + 1]) << 8);
                        short sh = (short)(sh1 + sh2);
                        newdata[0][i / 2] = sh / ((double)short.MaxValue);
                    }
                }
                else
                {
                    for (int i = 0; i < rawdata.Length; i += 2)
                    {
                        short sh1 = rawdata[i]; // assumes intel byte order
                        short sh2 = (short)(((short)rawdata[i + 1]) << 8);
                        short sh = (short)(sh1 + sh2);
                        newdata[(i / 2) % nChannels][i / 4] = sh / ((double)short.MaxValue);
                    }
                }
            }
            else
                throw new NotImplementedException("Only 8 or 16 bit supported as of now...");

            this.data = newdata;
        }
        private void saveWave(BinaryWriter w, int bitsPerSample)
        {
            if (bitsPerSample != 8 && bitsPerSample != 16)
                throw new NotImplementedException("Only 8 or 16 bit supported as of now...");

            uint thedatasize = (uint)(this.LengthInSamples * (bitsPerSample / 8) * this.getNumChannels());
            uint thefilesize_minus8 = 4 + (8 + 16) + 8 + thedatasize; //header + fmt chunk + data chunk


            w.Write(new byte[] { (byte)'R', (byte)'I', (byte)'F', (byte)'F' });
            w.Write((uint)thefilesize_minus8); // size of data + headers

            w.Write(new byte[] { (byte)'W', (byte)'A', (byte)'V', (byte)'E' });
            w.Write(new byte[] { (byte)'f', (byte)'m', (byte)'t', (byte)' ' });
            w.Write((uint)16); //size is 16 bytes
            w.Write((ushort)1); //format 1
            w.Write((ushort)this.getNumChannels());  //nChannels
            w.Write((uint)this.getSampleRate()); //SampleRate
            w.Write((uint)((this.getNumChannels() * bitsPerSample * this.getSampleRate()) / 8)); //ByteRate
            w.Write((ushort)((this.getNumChannels() * bitsPerSample) / 8)); //BlockAlign
            w.Write((ushort)bitsPerSample); //BitsPerSample 

            w.Write(new byte[] { (byte)'d', (byte)'a', (byte)'t', (byte)'a' });

            w.Write((uint)thedatasize); // Data size

            byte[] rawdata = new byte[thedatasize];
            if (bitsPerSample == 8)
            {
                if (this.getNumChannels() == 1)
                {
                    for (int i = 0; i < this.LengthInSamples; i++)
                    {
                        byte bvalue = (byte)(((this.data[0][i] / 2.0) + 0.5) * 256);
                        if (bvalue > 255) bvalue = 255;
                        else if (bvalue < 0) bvalue = 0;
                        rawdata[i] = bvalue;
                    }
                }
                else
                {
                    int j = 0; //number of bytes written
                    for (int i = 0; i < this.LengthInSamples; i++)
                    {
                        for (int ch = 0; ch < this.getNumChannels(); ch++)
                        {
                            byte bvalue = (byte)(((this.data[ch][i] / 2.0) + 0.5) * 256);
                            if (bvalue > 255) bvalue = 255;
                            else if (bvalue < 0) bvalue = 0;
                            rawdata[j] = bvalue;
                            j++;
                        }
                    }
                }
            }
            else if (bitsPerSample == 16)
            {


                if (this.getNumChannels() == 1)
                {
                    for (int i = 0; i < this.LengthInSamples; i++)
                    {
                        double dblvalue = (this.data[0][i] * short.MaxValue); // note that this is signed, so range is minval to maxval
                        if (dblvalue > short.MaxValue) dblvalue = short.MaxValue;
                        else if (dblvalue < short.MinValue) dblvalue = short.MinValue;
                        short sh = (short)dblvalue;
                        rawdata[i * 2] = (byte)(sh & 0x00FF); // low byte
                        rawdata[i * 2 + 1] = (byte)(sh >> 8); // high byte
                    }
                }
                else
                {
                    int j = 0;
                    for (int i = 0; i < this.LengthInSamples; i++)
                    {
                        for (int ch = 0; ch < this.getNumChannels(); ch++)
                        {
                            double dblvalue = (this.data[ch][i] * short.MaxValue); // note that this is signed, so range is minval to maxval
                            if (dblvalue > short.MaxValue) dblvalue = short.MaxValue;
                            else if (dblvalue < short.MinValue) dblvalue = short.MinValue;
                            short sh = (short)dblvalue;
                            rawdata[j * 2] = (byte)(sh & 0x00FF); // low byte
                            rawdata[j * 2 + 1] = (byte)(sh >> 8); // high byte
                            j++;
                        }
                    }
                }
            }

            w.Write(rawdata);
        }
    }
}