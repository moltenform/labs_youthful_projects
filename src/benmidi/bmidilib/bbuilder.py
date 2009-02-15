'''
bbuilder.py, Ben Fisher 2008
Simplified interface for building a midi file. (There are many, many midi options that can't be set with this interface).
Read the examples below to see how to use it.

first, add the line "import bbuilder".
making a simple melody
	import bbuilder
	b = bbuilder.BMidiBuilder()
	b.note('c', 1) #pitch 60, duration 1 qtr note
	b.note('d', 1)
	b.note('e', 1)
	b.note('f', 1)
	b.note('g', 4)
	b.save('out.mid')

making a chord
	b = bbuilder.BMidiBuilder()
	b.note('c', 1)
	b.rewind(1)
	b.note('e', 1)
	b.rewind(1)
	b.note('g', 1)
	b.save('out.mid') #doesn't "sort" the events until the end.


more options
	b = bbuilder.BMidiBuilder()
	b.tempo = 60
	b.volume = 20
	b.pan = 127 #out of right speaker
	b.setInstrument('flute') #looks up in list, until first match. b.setInstrument(73) also works
	b.note('c', 1, velocity=127) #velocity is the loudness of a particular note, 1-127
	
	#ways to type same note:
	b.note(61, 1)
	b.note('c#', 1) #use sharps, not flats.
	b.note('c#4', 1)
	
	#octaves
	b.note('c#5', 1)
	b.note('c#6', 1)
	b.save('out.mid')

advanced (2 tracks)
	tr1 = bbuilder.BMidiBuilder()
	tr1.setInstrument('fretless bass')
	tr1.note('c3', 2)
	tr1.note('d3', 2)
	tr1.note('e3', 2)
	tr1.rest(2)
	tr1.note('f3',2)
	
	tr2 = bbuilder.BMidiBuilder()
	tr2.setInstrument('ocarina')
	tr2.note('e4', 2)
	tr2.note('f4', 2)
	tr2.note('g4', 2)
	tr1.rest(2)
	tr1.note('a4',2)
	
	bbuilder.joinTracks( [tr1, tr2], 'out.mid') #tempo of first track used.
'''
import bmidilib
import exceptions


class BuilderException(exceptions.Exception): pass
class BMidiBuilder():
	volume=None
	pan=None
	currentTime=0 #in units of qtr notes
	notes= None
	def __init__(self):
		self.notes = []
		self.currentTime = 0
		
	def note(self, pitch, duration, velocity=60):
		try:
			n = int(pitch)
		except ValueError:
			#interpret string like 'c#4' into a pitch
			lastLetter = pitch[-1]
			if not lastLetter in '0123456789': pitch += '4' #assume octave 4 if none given
			n = bmidilib.nameToPitch(pitch)
			if n==None: 
				raise BuilderException('Unable to interpret note name , use the format "c#" or "c#4".')
		assert duration>0
		assert n>0
		assert velocity>0 and velocity<=127
		self.notes.append( SimpleNote(n, self.currentTime, duration, velocity) )
		self.currentTime += duration
	def rewind(self, timestep):
		self.currentTime -= timestep
		assert self.currentTime > 0
	def rest(self, timestep):
		self.currentTime += timestep
	def build(self, strFilename):
		if len(self.notes)==0: raise BuilderException('Cannot save empty file, has to contain some notes.')
	def buildtrack(self):
		#the first order of business is to sort all of the events.
		evtlist = []
		
		
class SimpleNote():
	def __init__(self, pitch, time, dur, velocity):
		self.pitch=pitch; self.time=time; self.dur=dur; self.velocity = velocity



# could be a class method of BMidiBuilder, but whatever
def joinTracks():
	pass

#~ <MidiFile 4 tracks
  #~ <MidiTrack  -- 7 events
    #~ <MidiEvent SEQUENCER_SPECIFIC_META_EVENT, t=0, channel=None, data='\x00\x00[#\x02\x00\x03\x003\x00'>
    #~ <MidiEvent SEQUENCE_TRACK_NAME, t=0, channel=None, data='WinJammer Demo'>
    #~ <MidiEvent INSTRUMENT_NAME, t=0, channel=None, data='Conductor Track'>
    #~ <MidiEvent MIDI_PORT, t=0, channel=None, data='\x00'>
    #~ <MidiEvent SET_TEMPO, t=0, channel=None, data='\x06\x1a\x80'>
    #~ <MidiEvent TIME_SIGNATURE, t=0, channel=None, data='\x04\x02\x18\x08'>
    #~ <MidiEvent END_OF_TRACK, t=0, channel=None, data=''>
  #~ >
  #~ <MidiTrack  -- 1017 events
    #~ <MidiEvent SEQUENCE_TRACK_NAME, t=0, channel=None, data='WinJammer Demo'>
    #~ <MidiEvent MIDI_PORT, t=0, channel=None, data='\x00'>
    #~ <MidiEvent NOTE_ON, t=480, channel=1, pitch=57, velocity=44>
    #~ <MidiEvent NOTE_ON, t=480, channel=1, pitch=62, velocity=44>
    #~ <MidiEvent NOTE_ON, t=480, channel=1, pitch=65, velocity=44>
    #~ <MidiEvent NOTE_ON, t=564, channel=1, pitch=65, velocity=0>
    #~ <MidiEvent NOTE_ON, t=568, channel=1, pitch=62, velocity=0>
    #~ <MidiEvent NOTE_ON, t=574, channel=1, pitch=57, velocity=0>
    #~ <MidiEvent NOTE_ON, t=600, channel=1, pitch=57, velocity=44>
if __name__=='__main__':
	pass
	
	