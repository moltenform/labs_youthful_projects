BeatblinkExperiments
Quick experiments in creating unusual sounds
Copyright (C) 2008  Ben Fisher
Uses the CsWaveAudio library.
Raw FFT calculation code by Don Cross, http://web.archive.org/web/20020124104515/http://www.intersrv.com/~dcross/fft.html


Code for the "crazy feedback":
for (int ch = 0; ch < w.getNumChannels(); ch++)
{
    for (int i = 0; i < w.data[ch].Length; i++)
    {
        w.data[ch][i] = w.data[ch][i] + w.data[ch][((int)(i * timeScale) + timeShift) % w.data[ch].Length];
    }
}
The key is the %. Without this it would just be a simple flange effect.
So, feedback occurs when it reads a position that has already been written to.
