[Back](../README.md)

## Setup

### Prereqs

If you don't already have Python 3 installed,

* Install Python 3 from [here](https://www.python.org/downloads/)

If you don't already have ffmpeg,

* Download compiled ffmpeg binaries from [here](https://ffmpeg.zeranoe.com/builds/). The default "version", "architecture", and "linking" is fine. Save to any directory you like, you don't have to add to the PATH or add to program files.

### Build from source

If you don't already have Visual Studio,

* Visual Studio Community is free for open source projects, can be downloaded  [here](https://visualstudio.microsoft.com/downloads/)

In a command line,

* Make a destination directory in a writable location, like `C:\path\to\csdownload`

* Run `cd C:\path\to\csdownload`

* Run `git clone https://github.com/moltenjs/labs_youthful_projects.git`

* Use Visual Studio to build `labs_youthful_projects/csdownloadvid/src/csdownloadvid.sln` as Release

* Run `cd C:\path\to\csdownload`

* Run `mkdir output`

* Run `copy labs_youthful_projects/csdownloadvid/src/bin/Release/csdownloadvid.exe .\output\CsDownloadVid.exe`

* Run `copy labs_youthful_projects/csdownloadvid/py/benpytwrapper.py .\output\benpytwrapper.py`

* Run `git clone https://github.com/moltenjs/labs_coordinate_music.git`

* Run `robocopy .\labs_coordinate_music\ben_python_common .\output\ben_python_common /e /mir`

You can now open `C:\path\to\csdownload\output` and open `CsDownloadVid.exe`

### First run

* Open `CsDownloadVid.exe`, click "Start", and click "Download a video"

* If you are running Linux/Mac, use `mono` to start `CsDownloadVid.exe`

* Click the "Get updates" button on the right. You'll need to do this before any videos can be downloaded

* The first time you click Download, you might be asked for the location of `python 3` and `ffmpeg`

* Done!
