    public static void gennotes()
        {
            for (int i=48; i<=91; i++)
            {
                double freq = Sine.FrequencyFromMidiNote(i);
                WaveAudio w = new Sine(freq, 0.5).CreateWaveAudio(0.3);
                //cut it to a multiple
                
                int waveformPeriod = (int)(Sine.SampleRate / freq);
                w.LengthInSamples = waveformPeriod * ((i<60)?10:20);
                w.SaveWaveFile(@"C:\pydev\mainsvn2\benmidi\trilled\trilled-wav\media\" + i+".wav");
            }
        }
