[Back](../README.md)

## Extract audio from a video (lossless)

* Open CsDownloadVid, click Start, click "Separate audio and video"

* Click "Choose..." and select an mp4 video.

* Click "Extract Audio Channel (lossless)"

* That's it! You'll now see a file `video.mp4_audio.m4a` alongside your input `video.mp4`.

### Advanced

You might also be able to extract audio from other formats, but it's not guarenteed to work. For example, if you have a .3gp video that has audio in mp3 format, you could do the following

* Click "Choose..." and choose the video.

* To the right of the results box, there is a box "Output format" that says "auto". Type "mp3" here.

* Click "Extract Audio Channel (lossless)". If asked if you want to continue, click Yes.

* If successful, the file will be saved, if not successful, you can read the information in the results box. This is a way to determine the format of the audio channel of the video; if the results box has a line like `output: Stream #0:1(und): Audio: aac (HE-AAC)`, this means that the audio is in aac, and you should type m4a as the output format, if the results box has a line like `output: Stream #0:1(und): Audio: mp3`, this means that the audio is in mp3, and you should type mp3 as the output format, and so on. 

