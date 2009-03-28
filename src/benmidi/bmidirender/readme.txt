bmidi to wave, turning midis into wavs
Copyright (C) Ben Fisher, 2009. GPL v2.
http://b3nf.com/codepages/bmidi/mtowave
Uses timidity++, a software synthesizer, Masanao Izumo, GPL v2.
Uses sfubar, released in the public domain, Ben Collver.

About:
	bmidi to wave can be used:
	1) to turn midi files into great-sounding wav and mp3 files, so that they can be shared or burned on a CD.
	2) to use as a program for playing midi files, especially useful if your computer's default midi output doesn't sound very good.
	3) to get information about SoundFont files, including author/copyright info and a list of all presets that can be previewed.
	
	
	bmidi to wave is a frontend for the program timidity++, which does the actual playback.
	This program provides a graphical interface and saves you from having to edit configuration files.
	This program is especially useful when trying many soundfonts, and using several different soundfonts to play one midi.
	
Dependencies:
	Requires Python 2.5
	In Linux, you'll need the timidity package. (In some package databases it is called timidity++).
		Open a terminal and type "timidity", and press enter.
		If no file is found, you'll need to install the timidity++ package.
			A command like
			sudo apt-get install timidity
			or 
			sudo yum install timidity++
			depending on your distribution should do the trick.

Usage:
	Unzip the archive into a writable directory.
	Run the file main.py. The main window will open.
	First, open a midi file. There are some provided in sample_midis.
	Now, you can play the file, configure instruments, or save the file to a wave file.

	There are many tools, like lame, that can encode the output .wav file to a .mp3.
	Try playing with the mixer, audio properties window, and so on. 
	The soundfont information tool can also be used to preview what a soundfont sounds like, or see what voices it contains.

	See http://b3nf.com/codepages/bmidi/mtowave for more information.
	At the website, I have collected better soundfonts that can be downloaded.
	Soundfonts like music theoria will sound much better than the ones that come by default.



Notes and known issues:
	In Linux, when manually specifying a .cfg configuration file, it is apparently overridden by the default Timidity configuration file in /etc/timidity.
	The program has to be run in a writable directory. This may be fixed in a later version.
	
	It is potentially confusing that "program", "preset", "voice", "instrument" are all basically the same concept. It's just that Soundfonts and midi files use different terminology.
	24bit wav files created won't open in Windows Media Player, but they are valid and will open in Audacity among other programs.


	This program supports the older timidity method of using .pat files. 
	There are two ways to do this. First, open a midi file.
	1) You can use an existing .cfg, like the cfg for eawpats or freepats. 
		Audio->"Choose Soundfont..."
		In the window that comes up, click Change... to change the current soundfont.
		Notice that near the bottom of the open dialog, "files of type" can be changed to "Timidity Configuration File (.cfg)". choose that, and open the file.
		It shouldn't matter what directory that .cfg file is in.
		Now, playback will use that .cfg file.
		
	2) Also, individual .pat files can be chosen for individual voices. In fact, some voices can be from .pat and some can be from .sf2 at the same time.
		Audio->"Choose Soundfont..."
		In the window that comes up, click Customize.
		Choose an instrument in the list on the left.
		In the right, click Choose soundfont...
		Notice that near the bottom of the open dialog, "files of type" can be changed to "Patch File (.pat)". choose that, and open a patch file.
	Neither of those, though, work in Linux as of now, as said above.
	
	In Linux, this program uses the x86 binary "sfubar" for the SoundFont information tool.
	If running a non x86 system, (or an os with a different binary format like mac os) and want to run that tool, you may need to recompile sfubar.
		The source and makefile are in ./soundfontpreview/sfubar-src/
		Place the compiled binary sfubar in the directory ./soundfontpreview



