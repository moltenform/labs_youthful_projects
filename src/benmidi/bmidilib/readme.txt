bmidilib.py , code for reading/creating midi files in Python
Ben Fisher, 2008. GPL.
https://github.com/moltenform/labs_youthful_projects/tree/master/src/benmidi/README.md
Based originally on midi.py, placed into the public domain in December 2001 by Will Ware

Tested on a good variety of files. The midi events output are identical, although sometimes the file size is slightly larger. Maybe this has to do with time deltas.

Example reading a MIDI file:
    (first place the bmidilib directory in the directory containing the script)
    from bmidilib import bmidilib
    
    m = bmidilib.BMidiFile()
    m.open('mymidi.mid')
    m.read()
    m.close()
    print m #lists all of the events in a human-readable way
    
    #loop through only the note events in 2nd track, and print out pitches and durations
    for note in m.tracks[1].notelist:
        print note.pitch, note.duration

Example making a MIDI file:
    # For creating midi files, bbuilder.py should be enough for everyday usage.
    # this is an example in case you are trying to do something beyond bbuilder.
    
    from bmidilib import bmidilib

    file = bmidilib.BMidiFile()
    file.ticksPerQuarterNote = 120

    tr = bmidilib.BMidiTrack()
    file.tracks.append(tr)
    
    #note on event
    startevt = bmidilib.BMidiEvent()
    startevt.type = 'NOTE_ON'
    startevt.time = 0
    startevt.channel = 1
    startevt.pitch = 60
    startevt.velocity = 80
    tr.events.append(startevt)
    
    #note off event (which is note_on with velocity 0)
    endevt = bmidilib.BMidiEvent()
    endevt.type = 'NOTE_ON'
    endevt.time = 120
    endevt.channel = 1
    endevt.pitch = 60
    endevt.velocity = 0
    tr.events.append(endevt)

    evt = bmidilib.BMidiEvent()
    evt.type='END_OF_TRACK'
    evt.time = 500
    evt.data = ''
    tr.events.append(evt)
    
    file.open('out.mid', 'wb')
    file.write()
    file.close()



Class Hierarchy:
BMidiFile
    BMidiTrack[] tracks
    
BMidiTrack
    BMidiEvent[] events
    BNote[] notelist
        Notes in a mid file consist of two events, a "NoteOn" and a later "NoteOff"
        The "notelist" provides a simpler view, as a single BNote object that has duration.
        (If you are creating a BMidiTrack yourself, you do not need to add to the notelist)

BMidiEvent
    Raw information about the midi event.
    Includes raw NoteOn and NoteOff events.
    Does not include "delta times" which are created automatically.
    
BNote
    Information including note, duration
    - Modifying these instances will not actually modify the midi -
    This is just an alternate view of information elsewhere
        So to change the pitch of a note, say, you have to have both
        note.startEvt.pitch = 60
        note.endEvt.pitch = 60
        note.pitch = 60
        

        