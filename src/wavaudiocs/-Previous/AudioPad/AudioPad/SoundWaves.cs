//Ben Fisher, 2008
//halfhourhacks.blogspot.com
//GPLv3 Licence

//This class basically assumes you will be working with 16Bit, 44100Hz, 1 channel sounds

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX.DirectSound;

namespace AudioPad
{
    class SoundWaves
    {
        private Device deviceSound;
        public SoundWaves(Device deviceSoundIn)
        {
            // Needs an already-configured sound device
            deviceSound = deviceSoundIn;
        }
        public void playbytes(Byte[] soundbytes, int bitsPerSample, int sampleRate)
        {
            int bytesPerSample;
            if (bitsPerSample == 8) bytesPerSample = 1;
            else if (bitsPerSample == 16) bytesPerSample = 2;
            else throw new NotSupportedException("Supports 8bit or 16bit");

            WaveFormat format = new WaveFormat();
            format.BitsPerSample = (short)bitsPerSample;
            format.Channels = 1;
            format.BlockAlign = (short)bytesPerSample;

            format.FormatTag = WaveFormatTag.Pcm;
            format.SamplesPerSecond = sampleRate; //sampling frequency of your data;   
            format.AverageBytesPerSecond = format.SamplesPerSecond * bytesPerSample;

            BufferDescription desc = new BufferDescription(format);
            desc.DeferLocation = true;
            desc.BufferBytes = soundbytes.Length;

            SecondaryBuffer currentBuffer = new SecondaryBuffer(desc, deviceSound);
            currentBuffer.Write(0, soundbytes, LockFlag.EntireBuffer);
            currentBuffer.Play(0, BufferPlayFlags.Default);
        }

        // The following are methods for generating sound data.
        // They are not intended for real-time playback. The byte arrays are not reused, for one thing.

        // Takes integer index, returns double between -1 and 1
        delegate double SoundDelegate(int i);


        public byte[] sine(double frequency, double amp, double duration)
        {
            double timeScale = frequency * 2 * Math.PI / (double)44100;
            SoundDelegate fn = delegate(int i) { return Math.Sin(i * timeScale); };
            return create16bit_sound(fn, frequency, amp, duration, true);
        }

        public byte[] square(double frequency, double amp, double duration)
        {
            double period = 44100.0 / frequency;
            double cutpoint = (period / 2.0); //duty cycle could be changed here
            SoundDelegate fn = delegate(int i)
            {
                return ((i % period) > cutpoint ? 1.0 : -1.0);
            };
            return create16bit_sound(fn, frequency, amp, duration, true);
        }
        public byte[] sawtooth(double frequency, double amp, double duration)
        {
            double period = 44100.0 / frequency;
            double slope = 2 / period; //because it goes from -1 to 1 in one period
            SoundDelegate fn = delegate(int i)
            {
                return ((i % period) * slope - 1);
            };
            return create16bit_sound(fn, frequency, amp, duration, true);
        }
        public byte[] triangle(double frequency, double amp, double duration)
        {
            double period = 44100.0 / frequency;
            double slope = 4 / period; // it goes from -1 to 1 in half period
            SoundDelegate fn = delegate(int i)
            {
                double v = (i % period);
                if (v < period / 2.0)
                    return (v * slope - 1);
                else
                    return ((v - period / 2) * -slope + 1);
            };
            return create16bit_sound(fn, frequency, amp, duration, true);
        }

        // frequency is unused.
        public byte[] whitenoise(double frequency, double amp, double duration)
        {
            Random r = new Random();
            SoundDelegate fn = delegate(int i)
            {
                return (r.NextDouble() * 2 - 1.0); //random number between -1 and 1
            };
            return create16bit_sound(fn, frequency, amp, duration, false);
        }
        // frequency is here interpreted as a factor
        public byte[] rednoise(double frequency, double amp, double duration)
        {
            double factor = frequency / 440; // ? scales distance that the "particle" moves

            Random r = new Random();
            double location = 0.0; //random walk
            SoundDelegate fn = delegate(int i)
            {
                location += (r.NextDouble() * factor - factor / 2);
                if (location > 1) location = 1;
                else if (location < -1) location = -1;
                return location;
            };
            return create16bit_sound(fn, frequency, amp, duration, false);
        }


        // helper function for creating sound
        private byte[] create16bit_sound(SoundDelegate fn, double frequency, double amp, double duration, bool periodic)
        {
            int sampleRate = 44100;
            int length = (int)(sampleRate * duration);
            byte[] wavedata = new byte[length * 2];

            int waveformPeriod = (int)(sampleRate / frequency);
            for (int i = 0; i < length; i++)
            {
                if (i <= waveformPeriod || !periodic)
                {
                    double dbl = fn(i);
                    short sh = (short)(dbl * amp * short.MaxValue);

                    wavedata[i * 2] = (byte)(sh & 0x00FF); // low byte
                    wavedata[i * 2 + 1] = (byte)(sh >> 8); // high byte
                }
                else  // we have already computed the wave, it is periodic. Good optimization!
                {
                    int prevspot = i % waveformPeriod;
                    wavedata[i * 2] = wavedata[prevspot * 2];
                    wavedata[i * 2 + 1] = wavedata[prevspot * 2 + 1];
                }
            }
            return wavedata;
        }

    }
}
