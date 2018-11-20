[Back](../README.md)

## Fade-out a mp4/m4a/aac audio (lossless)

Sometimes you'll have a song that goes on for longer than you like. It's an mp4 audio file (aka .m4a or "aac"). Maybe it's a video game soundtrack that repeats several times, and you want to shorten the song. You could easily open the song in Audacity, delete the last part of the song, and add a fade-out. This would work, but then you to have to re-encode the audio again, and since m4a is a lossy format, some audio data will be lost.

Let's say the song is 6:00 long, and we want to keep only the first 4:00, with an 8 second fade-out. We'll re-encode these last 8 seconds, but we can leave the rest of the song untouched.

We'll use qaac, which is a wrapper around Apple's aac encoder. If you don't have qaac set up:

* If you don't have Apple's iTunes or QuickTime installed,

    * Download the exe to install iTunes for Windows from [here](https://support.apple.com/kb/DL1615?locale=en_US)
    
    * Open the `iTunes6464Setup.exe` file in 7zip or WinRar
    
    * Extract the file `AppleApplicationSupport.msi`
    
    * Run `AppleApplicationSupport.msi`, which will install the x86 aac encoder (much smaller than installing all of iTunes).

* Download qaac_2.xx.zip, such as [qaac_2.66.zip](https://sites.google.com/site/qaacpage/cabinet/qaac_2.66.zip?attredirects=0&d=1) from [Google sites](https://sites.google.com/site/qaacpage/cabinet)

* Unzip the zip file.

Now that the prereqs are ready, you can use CsDownloadVid,

* Open CsDownloadVid, click Start, click "Split a video or song"

* Click "Choose..." to select the input song.

* Check the "Add fadeout" box, and type in "8" for an 8-second long fadeout.

* Type 4:00 into the box asking about split points.

(Todo: add picture)

* Click "Split (Lossless)"

* If this is the first time, you'll probably be asked for the location of `qaac.exe`

* That's it! You should see a new file "song.m4a_fadeout.m4a" alongside your input "song.m4a".
