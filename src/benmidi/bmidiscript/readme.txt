tunescript, a music toy
Copyright (C) Ben Fisher, 2009. GPL.
http://b3nf.com/codepages/bmidi/

Why:
	It's a musical toy: a fun way to enter in some music.
	It can be helpful for expressing rhythms, say to quickly record some rhythm you came up with.

Usage:
	Open "main.py" in a recent version of Python, like 2.5
	Type in something, like 'c d e f g'.
	Press "play". You should hear some sounds.
	
	If you don't hear anything, 
		check your volume settings, in particular the volume for "SW Synth." 
		Make sure that it is turned up and try again.
	In Linux,
		You may not have the required program "timidity" installed.
		In a terminal, if you type in "timidity" but do not get a response, you might have to do something along the lines of 
			sudo apt-get install timidity
			or 
			sudo yum install timidity++
		in a terminal to install the timidity++ package.
		
Getting started:
	Load one of the examples from the Examples menu.
	Play around with it, and make your own songs.
	If you come up with something cool, send it to me at boinjyboing@hotmail.com
	
	

There are two modes, "tunescript" mode and "Code" mode. 
	The tunescript mode is the interesting and fun one
	Code mode simply evaluates the Python code you provide as a way to quickly make scripts using bbuilder.py.
	In the Examples menu, the second set of examples are in Code mode.
	The examples given should be used for reference.





Index of "tunescript" syntax: 
	(Kind of like cheating, you should explore on your own.)
	See the end of bmidiconstants.py for a list of all instrument names.

Directives. These must occur on their own line.
================
(tempo 300)		Sets tempo. Must occur before any notes.
(voice "flute")		Set instrument
(voice 'flute')
(voice "73")			Set instrument by number
(voice 2 "flute")		Set instrument of a certain track

(volume 100)		Set volume, from 0 to 100.
(volume 2 100)		Set volume of a certain track

(balance right 100)	Make all of the way to the right speaker (0-100)
(balance left 100)	Make all of the way to the left speaker
(balance right 0)		Centered
(balance 2 right 100)	Set balance of a certain track


Pitches
================

c			The pitch c. uses the last octave specified.
c#
A
Ab			Note that "Ab" means "a flat". "ab" means "pitch a, then pitch b".

c3			Use a specific octave.

c'			An octave higher
c''			
c'''
c_			An octave lower
c__
c___

^ 			raises current octave
v			lowers current octave


Rhythm
=================
,		increases the length of the note, by one unit. "c,," is 3 times longer than "c"
.		rest for one unit
/c/		a pitch within forward slashes is played twice as quickly. 
		These can't have other modifications; /c/! or /c/, are not allowed. 
		Percussion, like /o/ is supported.
//c//		played 4 times as quickly


Note modifications
=================
c!		accented
c!!
c?		quieter
c??

c~>100		pitch bend all of the way. typically this bends up 2 semitones.
			bending occurs from the previous "bend amount" to the new amount. 
c,,,~>100		same as previous, but slower bending

a!,,		example of multiple modifications

Harmony
================
[c|e|g]	play the notes c e and g at the same time.
		can be modified with duration, accent and pitch bend

>> e f g
>> c d e
The above lines will set up two tracks to play simultaneously. 
Each "track" is separate, and settings like the last octave used are distinct.
The instruments set with (voice 2 'flute') 

The longest of the two lines will be used, in other words
>> c d e f g f e d c d e
>> c_
will become like
>> c  d e f g f e d c d e
>> c_.  . . . . . . . . . 
so that the lengths are even.

Percussion
===============
Also, certain notes are percussion and sound like percussion regardless of track/voice setting. These are:
o		Bass Drum 1
s		Snare Drum 1
*		Hand Clap
=		Closed Hi-hat
O		Low Tom 1
+		Open Hi-hat
0		Mid Tom 1
x		Crash Cymbal 1
{}		Chinese Cymbal
@		Cowbell
X		Crash Cymbal 2
M		High Agogo
m		Low Agogo
W		Short Whistle
w		Long Whistle

For cymbals that may be long, duration can be used, as in
{},,,,
s!


Currently unsupported:
	/c/!
	chords like /[c|e]/ 
	[.|c] is illegal
	
Newly supported:
	/o/ percussion 

