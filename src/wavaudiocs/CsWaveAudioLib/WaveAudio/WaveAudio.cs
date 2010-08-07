//Ben Fisher, 2008
//halfhourhacks.blogspot.com
//GPL

using System;
using System.IO;

namespace CsWaveAudio
{
    public partial class WaveAudio
    {
        /// <summary>
        /// Current sample rate. Can be set/retrieved with getSampleRate() and setSampleRate()
        /// Clients should not have to be concerned with the rate.
        /// </summary>
        private int m_currentSampleRate; 

        /// <summary>
        /// Array of samples for each channel.
        /// Each sample is represented as a double in the range -1 to 1
        /// </summary>
        private double[][] m_data;
        public double[][] data
        {
            get { return m_data; }
            set { m_data = value; }
        }
	

        /// <summary>
        /// Construct blank Audio file.
        /// </summary>
        public WaveAudio() : this(44100, 1) { }
        public WaveAudio(int nChannels) : this(44100, nChannels) { }
        public WaveAudio(int sampleRate, int nChannels)
        {
            // One sample, put in each channel
            this.data = new double[nChannels][];
            for (int i = 0; i < this.data.Length; i++)
                this.data[i] = new double[] { 0.0 };
            this.m_currentSampleRate = sampleRate;
        }

        /// <summary>
        /// Construct Audio file from an existing .wav file. Must be uncompressed PCM, 8 or 16 bit.
        /// </summary>
        public WaveAudio(string strFileName)
        {
            if (!File.Exists(strFileName))
                throw new Exception("File not found.");

            using (FileStream fs = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            using (BufferedStream bs = new BufferedStream(fs, 32768))
            using (BinaryReader r = new BinaryReader(bs))
            {
                this.loadWave(r);
            }
        }
        public WaveAudio(BinaryReader r) { this.loadWave(r); }


        /// <summary>
        /// Save to a .wav file, 8 or 16 bit.
        /// </summary>
        public void SaveWaveFile(string strFilename) { this.SaveWaveFile(strFilename, 16); }
        public void SaveWaveFile(string strFilename, int nBitsPerSample)
        {
            using (FileStream fs = new FileStream(strFilename, FileMode.Create))
            using (BufferedStream bs = new BufferedStream(fs, 32768))
            using (BinaryWriter w = new BinaryWriter(bs))
            {
                saveWave(w, nBitsPerSample);
            }
        }
        public void SaveWaveFile(BinaryWriter w, int nBitsPerSample) { saveWave(w, nBitsPerSample); }
        public void SaveWaveFile(BinaryWriter w) { saveWave(w, 16); }



        /// <summary>
        /// Length of audio in seconds.
        /// </summary>
        public double LengthInSeconds
        {
            get
            {
                return this.data[0].Length / (double)this.m_currentSampleRate;
            }
            set
            {
                int nSamples = (int)(value * this.m_currentSampleRate);
                this.LengthInSamples = (nSamples);
            }
        }

        /// <summary>
        /// Length of audio in samples.
        /// </summary>
        public int LengthInSamples
        {
            get { return this.data[0].Length; }
            set
            {
                if (value <= 0) throw new ArgumentException("Invalid # of samples");
                if (this.data[0].Length == value) // if already the same, don't do anything
                    return;
                else
                    for (int i = 0; i < this.data.Length; i++) // resize each of the channels
                        Array.Resize(ref this.data[i], value);
            }
        }



        // Why not use properties for this?
        // I want to impart that methods like .setSampleRate do significant things, not just setting some value
        public int getSampleRate() { return m_currentSampleRate; }
        /// <summary>
        /// Sets sample rate of audio. Should not change the way the sound is heard. Uses linear interpolation.
        /// </summary>
        public void setSampleRate(int newRate)
        {
            double factor = m_currentSampleRate / (double)newRate;
            WaveAudio w2 = Effects.ScalePitchAndDuration(this, factor);
            this.data = w2.data;
            this.m_currentSampleRate = newRate;
        }



        public int getNumChannels() { return this.data.Length; }
        /// <summary>
        /// Set number of channels.
        /// </summary>
        /// <param name="value">New number of channels.</param>
        /// <param name="bFill">Interpolate values, either by cloning existing channels (if adding channel) or mixing channels (if removing channel)</param>
        public void setNumChannels(int value, bool bFill)
        {
            if (value <= 0) throw new ArgumentException("Invalid # of channels");
            if (this.getNumChannels() == value) return; // already the right number
            if (value < this.getNumChannels()) // down mix
            {
                if (bFill && this.getNumChannels() != 2 && value != 1)
                    throw new ArgumentException("If using bFill to average channels, this is only implemented for 2 channels to 1 channel.");
                if (bFill) // downmix from 2 channels to 1, we take average of channels.
                {
                    double[][] newdata = new double[1][];
                    newdata[0] = new double[this.data[0].Length];
                    for (int i = 0; i < this.data[0].Length; i++)
                        newdata[0][i] = 0.5 * this.data[0][i] + 0.5 * this.data[1][i];

                    this.data = newdata;
                }
                else // simply truncate the other channels
                {
                    Array.Resize(ref this.m_data, value);
                }
            }
            else // up mix
            {
                double[][] newdata = new double[value][];

                for (int i = 0; i < this.getNumChannels(); i++)     // copy existing channels
                    newdata[i] = this.data[i];

                for (int j = this.getNumChannels(); j < value; j++) // create new channels
                    newdata[j] = new double[this.data[0].Length];

                if (bFill)                                          // copy first channel to the new ones
                    for (int j = this.getNumChannels(); j < value; j++)
                        Array.Copy(this.data[0], newdata[j], newdata[j].Length);

                this.data = newdata;
            }
        }

        /// <summary>
        /// Create deep copy of audio object.
        /// </summary>
        public WaveAudio Clone()
        {
            WaveAudio copy = new WaveAudio(this.getSampleRate(), this.getNumChannels());
            for (int ch = 0; ch < copy.data.Length; ch++)
            {
                copy.data[ch] = new double[this.data[ch].Length];
                Array.Copy(this.data[ch], copy.data[ch], this.data[ch].Length);
            }
            return copy;
        }

        /// <summary>
        /// Returns a new audio object, containing a slice of the sound.
        /// </summary>
        public WaveAudio GetSlice(double fSecondsStart, double fSecondsEnd)
        {
            WaveAudio slice = new WaveAudio(this.getSampleRate(), this.getNumChannels());
            int nStart = (int)(fSecondsStart * this.m_currentSampleRate);
            int nEnd = (int)(fSecondsEnd * this.m_currentSampleRate);
            if (nEnd <= nStart || nEnd > this.LengthInSamples || nStart < 0) throw new Exception("Invalid slice");
            for (int ch = 0; ch < slice.data.Length; ch++)
            {
                slice.data[ch] = new double[nEnd-nStart];
                Array.Copy(this.data[ch],nStart, slice.data[ch],0, nEnd - nStart);
            }
            return slice;
        }
        
        /// <summary>
        /// returns sample in the left-most channel (shortcut for accessing mono files)
        /// </summary>
        public double this[int index]
        {
            get { return this.data[0][index]; }
            set { this.data[0][index] = value; }
        }

        // get samples at this index, for all channels. Probably not too efficient.
        public double[] getSample(int index)
        {
            double[] ret = new double[this.getNumChannels()];
            for (int i = 0; i < this.getNumChannels(); i++)
                ret[i] = this.data[i][index];
            return ret;
        }
        public void setSample(int index, double[] values)
        {
            for (int i = 0; i < this.getNumChannels(); i++)
                this.data[i][index] = values[i];
        }

    }
}