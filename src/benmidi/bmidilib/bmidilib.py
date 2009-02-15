
"""
bmidilib.py , code for reading/creating midi files in Python
Ben Fisher, 2008
Based on midi.py, placed into the public domain in December 2001 by Will Ware
But has a different class structure, and a different way of representing note events
Tested on a good variety of files. The midi events output are identical, although sometimes the file size is slightly larger. Maybe this has to do with time deltas.
See bbuilder for an example of how to make midi files with these classes.

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
"""

import sys, string, types, exceptions
from midiutil import *
import bmidiconstants


class BMidiFile():
    def __init__(self):
        self.file = None
        self.format = 1
        self.tracks = [ ]
        self.ticksPerQuarterNote = None
        self.ticksPerSecond = None

    def open(self, filename, attrib="rb"):
        if filename == None:
            if attrib in ["r", "rb"]:
                self.file = sys.stdin
            else:
                self.file = sys.stdout
        else:
            self.file = open(filename, attrib)

    def __repr__(self):
        r = "<MidiFile %d tracks\n" % len(self.tracks)
        for t in self.tracks:
            r = r + "  " + `t` + "\n"
        return r + ">"

    def close(self):
        self.file.close()

    def read(self):
        self.readstr(self.file.read())

    def readstr(self, str):
        assert str[:4] == "MThd"
        length, str = getNumber(str[4:], 4)
        assert length == 6
        format, str = getNumber(str, 2)
        self.format = format
        assert format == 0 or format == 1   # dunno how to handle 2
        numTracks, str = getNumber(str, 2)
        division, str = getNumber(str, 2)
        if division & 0x8000:
            framesPerSecond = -((division >> 8) | -128)
            ticksPerFrame = division & 0xFF
            assert ticksPerFrame == 24 or ticksPerFrame == 25 or \
                   ticksPerFrame == 29 or ticksPerFrame == 30
            if ticksPerFrame == 29: ticksPerFrame = 30  # drop frame
            self.ticksPerSecond = ticksPerFrame * framesPerSecond
        else:
            self.ticksPerQuarterNote = division & 0x7FFF
        for i in range(numTracks):
            trk = BMidiTrack()
            str = trk.read(str)
            self.tracks.append(trk)

    def write(self):
        self.file.write(self.writestr())

    def writestr(self):
        division = self.ticksPerQuarterNote
        # Don't handle ticksPerSecond yet, too confusing
        assert (division & 0x8000) == 0
        str = "MThd" + putNumber(6, 4) + putNumber(self.format, 2)
        str = str + putNumber(len(self.tracks), 2)
        str = str + putNumber(division, 2)
        for trk in self.tracks:
            str = str + trk.write()
        return str


class BMidiTrack():
    def __init__(self):
        self.events = [ ]
        self.notelist = [ ]
        self.length = 0

    def read(self, str):
        
        #Keep track of starting/stopping note events.
        seenNoteStarts = { } #keys are tuples in format (channel, pitch). values are reference to the BMidiEvent notestart
        
        time = 0
        assert str[:4] == "MTrk"
        length, str = getNumber(str[4:], 4)
        self.length = length
        mystr = str[:length]
        remainder = str[length:]
        while mystr:            
            dt, mystr = delta_time_read(mystr)
            time = time + dt

            evt = BMidiEvent()
            mystr = evt.read(time, mystr)
            self.events.append(evt)
            
            if evt.type == "NOTE_ON" and evt.velocity!=0:
                seenNoteStarts[(evt.channel, evt.pitch)] = evt
                
            elif (evt.type == "NOTE_OFF" or (evt.velocity == 0 and evt.type == "NOTE_ON")):
                if (evt.channel, evt.pitch) in seenNoteStarts: #otherwise, not much we can do, invalid, but just ignore
                    evtStart = seenNoteStarts[(evt.channel, evt.pitch)]
                    bnote= BNote(evt.channel, evt.pitch, evtStart.time,evt.time - evtStart.time,evtStart, evt)
                    self.notelist.append(bnote)
                
        del seenNoteStarts
        return remainder

    def write(self):
        time = self.events[0].time
        # build str using BMidiEvents
        str = ""
        curtime = 0
        for e in self.events:
            nexttime = e.time
            str = str + delta_time_write(nexttime - curtime)
            str = str + e.write()
            curtime=nexttime
        return "MTrk" + putNumber(len(str), 4) + str

    def __repr__(self):
        r = "<MidiTrack  -- %d events\n" % ( len(self.events))
        for e in self.events:
            r = r + "    " + `e` + "\n"
        return r + "  >"

	
class BNote():
    #NOTE: Modifying instances of this class won't change the midi! To actually, say, change the pitch and duration, the startEvt and endEvts should be modified.
    channel=None
    pitch=None
    timeStart=None
    duration=None
    startEvt = None
    endEvt = None
    def __init__(self, channel, pitch, timeStart,duration,startEvt, endEvt):
        self.channel = channel
        self.pitch= pitch; self.timeStart=timeStart; self.duration=duration; self.startEvt=startEvt; self.endEvt= endEvt
    def __repr__(self):
        return ("<Midi Note ch=%d,time=%d,duration=%d>\n" %
             (self.channel, self.timeStart, self.duration))


# runningStatus appears to want to be an attribute of a MidiTrack. But
# it doesn't seem to do any harm to implement it as a global.
runningStatus = None
class BMidiEvent():
    def __init__(self):
        self.time = None
        self.channel = self.pitch = self.velocity = self.data = None
    def __cmp__(self, other):
        # assert self.time != None and other.time != None
        return cmp(self.time, other.time)
    def __repr__(self):
        r = ("<MidiEvent %s, t=%s, channel=%s" %
             (self.type,
              repr(self.time),
              repr(self.channel)))
        for attrib in ["pitch", "data", "velocity"]:
            if getattr(self, attrib) != None:
                r = r + ", " + attrib + "=" + repr(getattr(self, attrib))
        return r + ">"
    def read(self, time, str):
        global runningStatus
        self.time = time
        # do we need to use running status?
        if not (ord(str[0]) & 0x80):
            str = runningStatus + str
        runningStatus = x = str[0]
        x = ord(x)
        y = x & 0xF0
        z = ord(str[1])

        if bmidiconstants.channelVoiceMessages.has_value(y):
            self.channel = (x & 0x0F) + 1
            self.type = bmidiconstants.channelVoiceMessages.whatis(y)
            if (self.type == "PROGRAM_CHANGE" or
                self.type == "CHANNEL_KEY_PRESSURE"):
                self.data = z
                return str[2:]
            else:
                # Most likely adding a new note-on or note-off!
                self.pitch = z
                self.velocity = ord(str[2])
                
                return str[3:]

        elif y == 0xB0 and bmidiconstants.channelModeMessages.has_value(z):
            self.channel = (x & 0x0F) + 1
            self.type = bmidiconstants.channelModeMessages.whatis(z)
            if self.type == "LOCAL_CONTROL":
                self.data = (ord(str[2]) == 0x7F)
            elif self.type == "MONO_MODE_ON":
                self.data = ord(str[2])
            return str[3:]

        elif x == 0xF0 or x == 0xF7:
            self.type = {0xF0: "F0_SYSEX_EVENT", 0xF7: "F7_SYSEX_EVENT"}[x]
            length, str = getVariableLengthNumber(str[1:])
            self.data = str[:length]
            return str[length:]

        elif x == 0xFF:
            if not bmidiconstants.metaEvents.has_value(z):
                print "Unknown meta event: FF %02X" % z
                sys.stdout.flush()
                raise "Unknown midi event type"
            self.type = bmidiconstants.metaEvents.whatis(z)
            length, str = getVariableLengthNumber(str[2:])
            self.data = str[:length]
            return str[length:]

        raise "Unknown midi event type"

    def write(self):
        sysex_event_dict = {"F0_SYSEX_EVENT": 0xF0, "F7_SYSEX_EVENT": 0xF7}
        if bmidiconstants.channelVoiceMessages.hasattr(self.type):
            x = chr((self.channel - 1) +
                    getattr(bmidiconstants.channelVoiceMessages, self.type))
            if (self.type != "PROGRAM_CHANGE" and
                self.type != "CHANNEL_KEY_PRESSURE"):
                data = chr(self.pitch) + chr(self.velocity)
            else:
                data = chr(self.data)
            return x + data

        elif bmidiconstants.channelModeMessages.hasattr(self.type):
            x = getattr(bmidiconstants.channelModeMessages, self.type)
            x = (chr(0xB0 + (self.channel - 1)) +
                 chr(x) +
                 chr(self.data))
            return x

        elif sysex_event_dict.has_key(self.type):
            str = chr(sysex_event_dict[self.type])
            str = str + putVariableLengthNumber(len(self.data))
            return str + self.data

        elif bmidiconstants.metaEvents.hasattr(self.type):
            str = chr(0xFF) + chr(getattr(bmidiconstants.metaEvents, self.type))
            str = str + putVariableLengthNumber(len(self.data))
            return str + self.data

        else:
            raise "unknown midi event type: " + self.type


def delta_time_read(oldstr):
    time, newstr = getVariableLengthNumber(oldstr)
    return time, newstr

def delta_time_write(deltatime):
    return putVariableLengthNumber(deltatime)






def main(argv):
    m = BMidiFile()
    #~ m.open('..\\midis\\bossa.mid')
    m.open('..\\midis\\16keys.mid')
    m.read()
    m.close()
    
    #~ print m.ticksPerQuarterNote
    #~ print m.tracks[2].notelist
    print m
    
    #~ m.open('..\\midis\\bossa_ben_out.mid', "wb")
    #~ m.write()
    #~ m.close()
    

if __name__ == "__main__":
    main(sys.argv)
