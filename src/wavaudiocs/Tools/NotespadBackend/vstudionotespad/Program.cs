using System;
using System.Collections.Generic;
using System.Text;
using CsWaveAudio;

class Program
{
    static string PATH_SAMPLES = @"..\..\..\..\..\..\Sourceaudioclips\";
    static void Main(string[] args)
    {
        WaveAudio wout = CNotespad.Run();
        if (wout != null)
            wout.SaveWaveFile(@"..\..\..\..\output.wav");
    }
    public static WaveAudio loadSample(string s) { return new WaveAudio(Program.PATH_SAMPLES + s); }
}
