[Back](../README.md)

## csdownloadvid Setup

(These instructions are for Windows, csdownloadvid can be used in Linux via Mono, but this isn't the primary use case).

If you don't already have Python 3 installed,

* Install Python 3 from [here](https://www.python.org/downloads/)

If you don't already have ffmpeg,

* Download compiled ffmpeg binaries from [here](https://ffmpeg.zeranoe.com/builds/). The default "version", "architecture", and "linking" is fine. Save to any directory, you don't have to add to the PATH or add to program files.

## Building from source

If you don't already have Visual Studio,

* [Visual Studio Community](https://visualstudio.microsoft.com/downloads/) is free as-in beer.

* Make a destination directory in a writable location, like `C:\path\to\csdownload`

* Open a command line,

* Run `cd C:\path\to\csdownload`

* Run `git clone https://github.com/downpoured/labs_youthful_projects.git`

* Use Visual Studio to build `labs_youthful_projects/csdownloadvid/src/csdownloadvid.sln` as Release.

* Run `cd C:\path\to\csdownload`

* Run `mkdir output`

* Run `copy labs_youthful_projects/csdownloadvid/src/bin/Release/csdownloadvid.exe .\output`

* Run `copy labs_youthful_projects/csdownloadvid/py/benpytwrapper.py .\output`

* Run `git clone https://github.com/downpoured/labs_coordinate_music.git`

* Run `robocopy .\labs_coordinate_music\ben_python_common .\output\ben_python_common /e /mir`

You can now open `C:\path\to\csdownload\output` and run `csdownloadvid.exe`.

## First run

* Run `csdownloadvid.exe`, click "Start", and click "Download a video".

* You might be asked for the location of Python 3.

* Click the "Get updates" button on the right. You'll need to do this before any videos can be downloaded.
