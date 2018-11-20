[Back](../README.md)

## Split Audio (Lossless)

Sometimes, you want to split one audio file into different files. For example, you might have a long "mixtape" as a .m4a that contains many songs, and you want to split it into individual tracks. This can be done in audio software like Audacity, but then then you would have to re-encode the audio again, and since m4a is a lossy format, some audio data will be lost.

Here is how to split a .m4a, .mp3, .mp4 or video losslessly,

* Open CsDownloadVid, click Start, and click "Split a video or song"

* Click "Choose..." and select your input file

* We'll now need to have a list of the times where the file should be split. The default times, as an example, are 0:20 and 1:30, meaning that the first song ends at 0 minutes 20 seconds, and the second song ends at 1 minute 30 seconds.

![Screenshot](https://raw.githubusercontent.com/downpoured/labs_youthful_projects/master/csdownloadvid/doc/split_cs1.png)

* Unless we already know when the split times are, it's easiest to open the file in an audio editor like Audacity. (Audacity is open source, it can be downloaded [here](https://www.audacityteam.org/download/)).

![Screenshot](https://raw.githubusercontent.com/downpoured/labs_youthful_projects/master/csdownloadvid/doc/split_aud1.png)

* I've opened the file and can visually see the 3 different songs in the mixtape. If I zoom in, I see that the first song ends at 2:29 and the second song ends at 5:30.

* I could simply write down 2:29 and 5:30, that's all I need. If you have Audacity, though, there's a feature that can save time:

    * In Audacity, in the Tracks menu, click Tracks, click Add New, and click Label Track
    
    * Scroll to where a split point should be (the few seconds of silence between songs)
    
    * Click in the silence, in the label track (see picture:)
    ![Screenshot](https://raw.githubusercontent.com/downpoured/labs_youthful_projects/master/csdownloadvid/doc/split_aud2.png)
    
    * Type the letter "a" and press Enter. This creates a label.
    ![Screenshot](https://raw.githubusercontent.com/downpoured/labs_youthful_projects/master/csdownloadvid/doc/split_aud3.png)
    
    * Repeat these steps to create a label named "a" for each split point. You can click/drag to move a label.
    
    * Click the File menu, then click "Export Labels...", and save the .txt file someplace.
    
    * Back in CsDownloadVid, you can click "Import Audacity label track", choose the .txt file, and it will fill in the split points that were added.

![Screenshot](https://raw.githubusercontent.com/downpoured/labs_youthful_projects/master/csdownloadvid/doc/split_cs2.png)

* Click "Split (Lossless)" and that's it! The new .m4a files will be created alongside the input file.
