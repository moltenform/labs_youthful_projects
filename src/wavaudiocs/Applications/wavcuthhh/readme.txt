/*
 * WavCutHHH
 * by Ben Fisher, 2011 http://halfhourhacks.blogspot.com
 * Released under GPL v3
 * 
 * Run wavcut.exe to see usage.
 * Supports drag and drop.
 * Doesn't load the entire file into memory, so it can handle large files.
 * In tests, all samples accounted for in output.
 * */

WavcutHHH will split a large .wav file into several files based on a text file
that specifies cut points.


wavcut.exe input.wav input.txt

First line of the .txt file is 'wavcut'.
Remaining lines of the .txt file should be numbers, specifying where to cut.
Refer to wavcut.exe trombone.wav trombone.txt as an example.


To check wav contents, use

wavcut.exe -check input.wav
