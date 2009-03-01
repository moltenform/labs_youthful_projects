scoreview.py , code for visualizing midi files in Python
Copyright (C) Ben Fisher, 2009. GPL.

Usage:
Run main.py
Choose Open from the File menu to open a midi file. (Example midi files are given in the folder example_midis).
Click one of the "Score" or "List" buttons to view events for a particular track.

From the options menu you can also configure:
Show Durations
	A gray dashed line in the score view indicates the duration of notes.
Show Stems
	To show note stems.
Show Barlines
	At times, the midi song doesn't line up with the measures, and so the barlines are not meaningful. This provides a way to hide them.
Prefer Flats
	Because this program doesn't know about key signatures, it displays all accidentals as either sharps or flats.

Note that at this time, all notes are shown as "quarter notes". To add different symbols for different durations would not add much benefit.


Using the Scoreview component in your own program is also possible.
It derives from Tkinter's frame, and behaves as such.



