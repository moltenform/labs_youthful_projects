[Back](../README.md)

## Download Videos

Here's how to download videos in CsDownloadVid,

* Open `CsDownloadVid`, click "Start", and click "Download a video".

* Click "Get updates" to ensure you have the latest ytdl.

* Enter a URL and click "Go to next step".

* After a few seconds, you'll see different download quality choices. Select one of the choices and click "Go to next step".

![Screenshot](https://raw.githubusercontent.com/downpoured/labs_youthful_projects/master/csdownloadvid/doc/downloadinternetvids_1.png)

* Choose an output directory by clicking "Browse..."

![Screenshot](https://raw.githubusercontent.com/downpoured/labs_youthful_projects/master/csdownloadvid/doc/downloadinternetvids_2.png)

* Click "Download"

* That's it! The file will be downloaded.

## Additional Features

* For certain videos, especially when getting audio from long videos, ytdl can sometimes stall and never complete the download. If this happens, click "Cancel", click "Use pytube instead of ytdl" in the upper right, click "Get updates", then close and re-open csdownloadvid and try again. The download will now complete, although it might take a few minutes.

* If you click "Advanced options", you can batch-download several videos. You can enter a list of URLs separated by |, or enter the path of a text file with one url per line.

![Screenshot](https://raw.githubusercontent.com/downpoured/labs_youthful_projects/master/csdownloadvid/doc/downloadinternetvids_advanced.png)

* When downloading an m4a file via DASH, the resulting file has headers that sometimes don't work with some media players. We'll tell ytdl to use ffmpeg to correct the headers.

* You can specify the pattern for the output filename, such as  `%(title)s.%(ext)s`.

* To download a playlist: enter the URL of the playlist in the box on the top of the page that says "Step 1: Enter URL of video". Then, click the "Get playlist" button, and follow the instructions given.

* By default, when batch-downloading videos, we pause for 5 seconds between each video.

* You can click "Encode video" to perform custom video encoding. The default parameters will create a good-quality x264 mp4. As another example, the parameters `-i "%in%" -c:a copy -c:v copy "%in%.mp4"`, could be used to convert a .3gp to a more common .mp4 format.



