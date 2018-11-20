[Back](../README.md)

## Fade-out a mp4/m4a song (lossless)

Sometimes you'll have a song that goes on for longer than you like. It's in "aac" format (a .m4a or .mp4). You could open the song in Audacity, delete the last part of the song, and add a fade-out. This would work, but then you to have to re-encode the audio again, and since m4a is a lossy format, some audio data will be lost.

Let's say the song is 6:00 long, and we want to keep only the first 4:00, with an 8 second fade-out. We'll re-encode these last 8 seconds, but we can leave the rest of the song untouched.

If you don't have qaac set up:

* If you don't have Apple's iTunes or QuickTime installed,

    * Download the exe to install iTunes for Windows from [here](https://support.apple.com/kb/DL1615?locale=en_US)
    
    * Open the `iTunes6464Setup.exe` file in 7zip or WinRar
    
    * Extract the file `AppleApplicationSupport.msi`
    
    * Run `AppleApplicationSupport.msi`, which will install the x86 aac encoder (smaller than installing all of iTunes)

* Download qaac, such as [qaac_2.66.zip](https://sites.google.com/site/qaacpage/cabinet/qaac_2.66.zip?attredirects=0&d=1) from [Google sites](https://sites.google.com/site/qaacpage/cabinet)

* Unzip the qaac zip file

Now that the prereqs are ready,

* Open CsDownloadVid, click Start, click "Split a video or song"

* Click "Choose..." to select the input song

* Check the "Add fadeout" box, and type in "8" for an 8-second long fadeout

* Type 4:00 into the box asking about split points

![Screenshot](https://raw.githubusercontent.com/downpoured/labs_youthful_projects/master/csdownloadvid/doc/addfadeout.png)

* Click "Split (Lossless)"

* If this is the first time, you'll probably be asked for the location of `qaac.exe`

* That's it! You should see a new file "song.m4a_fadeout.m4a" alongside your input "song.m4a"

There will sometimes be a quiet sound artifact heard right at the transition. In my example, if there is a quiet sound at 4:00 in the output, I'll try the process again using 4:01 instead of 4:00. [Read more about the method used to losslessly add fadeout](addfadeout_theory.md).

