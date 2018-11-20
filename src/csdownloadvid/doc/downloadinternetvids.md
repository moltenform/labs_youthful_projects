[Back](../README.md)

## Download internet videos

* Click "Get updates" to ensure you have the latest ytdl.

* Enter a URL and click "Go to next step".

* After a few seconds, you'll see different download quality choices. Select one of the choices and click "Go to next step".

(downloadinternetvids_1.png)

* (Optional) Choose an output directory by clicking Browse...

* Click "Download"

(downloadinternetvids_2.png)

* That's it! The file will be downloaded.

### More information

* For certain videos, especially when getting audio from long videos, ytdl can sometimes stall and never complete the download. If this happens, click "Cancel", click "Use pytube instead of ytdl" in the upper right, click "Get updates", then close and re-open csdownloadvid and try again. The download will now complete, although it might take a minute.

* If you click "Advanced options", you can batch-download several videos. You can enter a list of URLs separated by |, or enter the path of a text file with one url per line.

(downloadinternetvids_advanced.png)

* When downloading an m4a file via DASH, the resulting file has headers that sometimes don't work with some media players. We'll tell ytdl to use ffmpeg to correct the headers.

* You can specify the output filename. For example, `%(title)s.%(ext)s` would be a shorter filename.

* By default, when batch-downloading videos, we pause for 5 seconds between each video.

* To save a playlist to a list of URLs of each video: first enter the playlist url under Step 1. Then, click the "Get playlist" button.

* You can click "Encode video" to perform custom video encoding. I've found this to be most useful turning an animated .gif to a smaller but visually identical .mp4. (The default parameters say to create a good-quality x264 as a mp4). You could also use it to move from a .3gp to a more common .mp4 format.





