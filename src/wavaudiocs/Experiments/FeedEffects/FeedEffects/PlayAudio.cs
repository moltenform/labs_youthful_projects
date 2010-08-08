using System;
using System.Collections.Generic;
using System.Text;
using System.Media;
using System.IO;
using CsWaveAudio;

namespace CsWaveAudio
{
    /// <summary>
    /// A simple wrapper over .NET SoundPlayer
    /// </summary>
    public class AudioPlayer
    {
        MemoryStream m;
        BinaryWriter bw;
        SoundPlayer pl;
        

        public void Play(WaveAudio a) { Play(a, false); }
        public void Play(WaveAudio a, bool bAsync)
        {
            m = new MemoryStream();
            bw = new BinaryWriter(m);
            a.SaveWaveFile(bw);

            pl = new SoundPlayer(m);
            pl.Stream.Position = 0; // This line is necessary 
           
            if (bAsync)
                pl.Play();
            else
                pl.PlaySync();
        }
        public void Stop()
        {
            pl.Stop();
        }

    }
}
