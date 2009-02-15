#!/usr/bin/env python

"""
midi.py -- MIDI classes and parser in Python
Placed into the public domain in December 2001 by Will Ware

Python MIDI classes: meaningful data structures that represent MIDI events
and other objects. You can read MIDI files to create such objects, or
generate a collection of objects and use them to write a MIDI file.

Helpful MIDI info:
http://crystal.apana.org.au/ghansper/midi_introduction/midi_file_form...
http://www.argonet.co.uk/users/lenny/midi/mfile.html
"""

import sys, string, types, exceptions
from willware_midiutil import *


# runningStatus appears to want to be an attribute of a MidiTrack. But
# it doesn't seem to do any harm to implement it as a global.
runningStatus = None

class MidiEvent:

    def __init__(self, track):
        self.track = track
        self.time = None
        self.channel = self.pitch = self.velocity = self.data = None

    def __cmp__(self, other):
        # assert self.time != None and other.time != None
        return cmp(self.time, other.time)

    def __repr__(self):
        r = ("<MidiEvent %s, t=%s, track=%s, channel=%s" %
             (self.type,
              repr(self.time),
              self.track.index,
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

        if channelVoiceMessages.has_value(y):
            self.channel = (x & 0x0F) + 1
            self.type = channelVoiceMessages.whatis(y)
            if (self.type == "PROGRAM_CHANGE" or
                self.type == "CHANNEL_KEY_PRESSURE"):
                self.data = z
                return str[2:]
            else:
                self.pitch = z
                self.velocity = ord(str[2])
                channel = self.track.channels[self.channel - 1]
                if (self.type == "NOTE_OFF" or
                    (self.velocity == 0 and self.type == "NOTE_ON")):
                    channel.noteOff(self.pitch, self.time)
                elif self.type == "NOTE_ON":
                    channel.noteOn(self.pitch, self.time, self.velocity)
                return str[3:]

        elif y == 0xB0 and channelModeMessages.has_value(z):
            self.channel = (x & 0x0F) + 1
            self.type = channelModeMessages.whatis(z)
            if self.type == "LOCAL_CONTROL":
                self.data = (ord(str[2]) == 0x7F)
            elif self.type == "MONO_MODE_ON":
                self.data = ord(str[2])
            return str[3:]

        elif x == 0xF0 or x == 0xF7:
            self.type = {0xF0: "F0_SYSEX_EVENT",
                         0xF7: "F7_SYSEX_EVENT"}[x]
            length, str = getVariableLengthNumber(str[1:])
            self.data = str[:length]
            return str[length:]

        elif x == 0xFF:
            if not metaEvents.has_value(z):
                print "Unknown meta event: FF %02X" % z
                sys.stdout.flush()
                raise "Unknown midi event type"
            self.type = metaEvents.whatis(z)
            length, str = getVariableLengthNumber(str[2:])
            self.data = str[:length]
            return str[length:]

        raise "Unknown midi event type"

    def write(self):
        sysex_event_dict = {"F0_SYSEX_EVENT": 0xF0,
                            "F7_SYSEX_EVENT": 0xF7}
        if channelVoiceMessages.hasattr(self.type):
            x = chr((self.channel - 1) +
                    getattr(channelVoiceMessages, self.type))
            if (self.type != "PROGRAM_CHANGE" and
                self.type != "CHANNEL_KEY_PRESSURE"):
                data = chr(self.pitch) + chr(self.velocity)
            else:
                data = chr(self.data)
            return x + data

        elif channelModeMessages.hasattr(self.type):
            x = getattr(channelModeMessages, self.type)
            x = (chr(0xB0 + (self.channel - 1)) +
                 chr(x) +
                 chr(self.data))
            return x

        elif sysex_event_dict.has_key(self.type):
            str = chr(sysex_event_dict[self.type])
            str = str + putVariableLengthNumber(len(self.data))
            return str + self.data

        elif metaEvents.hasattr(self.type):
            str = chr(0xFF) + chr(getattr(metaEvents, self.type))
            str = str + putVariableLengthNumber(len(self.data))
            return str + self.data

        else:
            raise "unknown midi event type: " + self.type

"""
register_note() is a hook that can be overloaded from a script that
imports this module. Here is how you might do that, if you wanted to
store the notes as tuples in a list. Including the distinction
between track and channel offers more flexibility in assigning voices.

import midi
notelist = [ ]
def register_note(t, c, p, v, t1, t2):
    notelist.append((t, c, p, v, t1, t2))
midi.register_note = register_note
"""

def register_note(track_index, channel_index, pitch, velocity,
                  keyDownTime, keyUpTime):
    pass

class MidiChannel:
    """A channel (together with a track) provides the continuity connecting
    a NOTE_ON event with its corresponding NOTE_OFF event. Together, those
    define the beginning and ending times for a Note."""

    def __init__(self, track, index):
        self.index = index
        self.track = track
        self.pitches = { }

    def __repr__(self):
        return "<MIDI channel %d>" % self.index

    def noteOn(self, pitch, time, velocity):
        self.pitches[pitch] = (time, velocity)

    def noteOff(self, pitch, time):
        if self.pitches.has_key(pitch):
            keyDownTime, velocity = self.pitches[pitch]
            register_note(self.track.index, self.index, pitch, velocity,
                          keyDownTime, time)
            del self.pitches[pitch]
        # The case where the pitch isn't in the dictionary is illegal,
        # I think, but we probably better just ignore it.

class DeltaTime(MidiEvent):
    type = "DeltaTime"

    def read(self, oldstr):
        self.time, newstr = getVariableLengthNumber(oldstr)
        return self.time, newstr

    def write(self):
        str = putVariableLengthNumber(self.time)
        return str

class MidiTrack:
    def __init__(self, index):
        self.index = index
        self.events = [ ]
        self.channels = [ ]
        self.length = 0
        for i in range(16):
            self.channels.append(MidiChannel(self, i+1))

    def read(self, str):
        time = 0
        assert str[:4] == "MTrk"
        length, str = getNumber(str[4:], 4)
        self.length = length
        mystr = str[:length]
        remainder = str[length:]
        while mystr:
            delta_t = DeltaTime(self)
            dt, mystr = delta_t.read(mystr)
            time = time + dt
            self.events.append(delta_t)
            e = MidiEvent(self)
            mystr = e.read(time, mystr)
            self.events.append(e)
        return remainder

    def write(self):
        time = self.events[0].time
        # build str using MidiEvents
        str = ""
        for e in self.events:
            str = str + e.write()
        return "MTrk" + putNumber(len(str), 4) + str

    def __repr__(self):
        r = "<MidiTrack %d -- %d events\n" % (self.index, len(self.events))
        for e in self.events:
            r = r + "    " + `e` + "\n"
        return r + "  >"

class MidiFile:
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
            trk = MidiTrack(i)
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

def main(argv):
    m = MidiFile()
    m.open('..\\midis\\bossa.mid')
    m.read()
    m.close()
    
    m.open('..\\midis\\bossaActOut.mid', "wb")
    m.write()
    m.close()
    
    #~ print m
    #~ import getopt
    #~ infile = None
    #~ outfile = None
    #~ printflag = 0
    #~ optlist, args = getopt.getopt(argv[1:], "i:o:p")
    #~ for (option, value) in optlist:
        #~ if option == '-i':
            #~ infile = value
        #~ elif option == '-o':
            #~ outfile = value
        #~ elif option == '-p':
            #~ printflag = 1

    #~ m = MidiFile()
    #~ m.open(infile)
    #~ m.read()
    #~ m.close()

    #~ if printflag:
        #~ print m
    #~ else:
        #~ m.open(outfile, "wb")
        #~ m.write()
        #~ m.close()

if __name__ == "__main__":
    main(sys.argv)
