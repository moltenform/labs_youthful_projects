[Back](../README.md)

## Split mp4/m4a/mp3 audio (lossless)

Sometimes, you want to split one audio file into different files. For example, you might have a long "mixtape" as a .m4a that contains many songs, and you want to split it into individual tracks. This can be done in audio software like Audacity, but then then you would have to re-encode the audio again, and since m4a is a lossy format, some audio data will be lost. (You can also use these same steps to losslessly split a .mp3 file or .mp4 video).

Here is how to losslessly split the audio file.

* Open CsDownloadVid, click Start, and click "Split a video or song"

* Click "Choose..." and select your input file

* We'll now need to have a list of the times where the file should be split. The default times, as an example, are 0:20 and 1:30, meaning that the first song ends at 0 minutes 20 seconds, and the second song ends at 1 minute 30 seconds.

(split_cs1.png)

* Unless we already know when the split times are, it's easiest to open the file in an audio editor like Audacity. (Audacity is a free and open source audio editor, it can be downloaded [here](https://www.audacityteam.org/download/)).

(split_aud1.png)

* In this example, there are 3 different songs in the mixtape. Visually, I can see the difference in the songs, because they have different average volumes. If I zoom in, I can see that the first song ends at 2:29, and the second song ends at 5:30.

* I could simply write down the numbers 2:29 and 5:30, that's all I need. If you have Audacity, though, there's a feature that can save time, especially when there are many split points.

    * In Audacity, in the Tracks menu, click Tracks, click Add New, and click Label Track
    
    * Scroll to where a split point should be (the few seconds of silence between songs)
    
    * Click in the silence, in the label track (see picture:)

    (split_aud2.png)
    
    * Type the letter "a" and press Enter. This creates a label.
    
    (split_aud3.png)
    
    * Repeat these steps to create a label named "a" for each split point. You can reposition a label by clicking and dragging to slide it left or right.
    
    * Click the File menu, then click "Export Labels...", and save the .txt file someplace.
    
    * Back in CsDownloadVid, you can click "Import Audacity label track", and it will fill in the split points that you added.

(split_cs2.png)

* Click "Split (Lossless)" and that's it! The new .m4a files will be created alongside the input file.
