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
	b.rewind() #automatically rewinds to start of last note
	b.note('e', 1)
	b.rewind()
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
	
	bbuilder.joinTracks( [tr1, tr2], 'out.mid')
	
advanced (inserting raw events)
	b = bbuilder.BMidiBuilder()
	b.tempo = 400
	for i in range(127):
		#insert a raw instrument change event
		evt = bmidilib.BMidiEvent() #event time will be set when inserted
		evt.type='PROGRAM_CHANGE'
		evt.channel=1
		evt.data = i
		b.insertMidiEvent(evt)
		b.note('c3',0.4)
	b.save('!2.mid')
	

Documentation
=================
All Functions
	bbuilder.joinTracks(listTracks, outFilename)

All Public Settings
	b.tempo (note that tempos other than 120bpm will not be precise)
	b.volume
	b.pan

All Public Methods
	b.note(pitch, duration)
	b.rest(duration)
	b.rewind(duration)
	b.setInstrument(instrument)
	b.save(outFilename)
	
'''
import bmidilib
import exceptions


class BuilderException(exceptions.Exception): pass
class BMidiBuilder():
	tempo = 120
	instrument = None
	volume=None
	pan=None
	currentTime=0 #in units of qtr notes
	notes= None
	
	def __init__(self):
		self.notes = []
		self.currentTime = 0
		
	def note(self, pitch, duration, velocity=60, percussion=False):
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
		assert n>=0
		assert velocity>0 and velocity<=127
		self.notes.append( SimpleNote(n, self.currentTime, duration, velocity, percussion=percussion) )
		self.currentTime += duration
	def rewind(self, timestep=None):
		if timestep==None: timestep = self.notes[-1].duration #if given with no args, goes back
		self.currentTime -= timestep
		assert self.currentTime >= 0
	def rest(self, timestep):
		self.currentTime += timestep
	def insertMidiEvent(self, evt, bUseCurrentTime=True):
		if bUseCurrentTime: evt.time=self.ourTimingToTicks(self.currentTime)
		if not isinstance(evt, bmidilib.BMidiEvent): raise BuilderException("Must be a BMidiEvent instance.")
		if evt.channel != None: raise BuilderException("Must leave channel blank, it will be assigned correctly upon build time.")
		self.notes.append(evt)
	def insertMidiEventInstrumentChange(self, n):
		evt = bmidilib.BMidiEvent()
		evt.type='PROGRAM_CHANGE'
		evt.time=self.ourTimingToTicks(self.currentTime)
		evt.channel=None #it will be assigned correctly at build time
		evt.data = n
		self.notes.append(evt)
	def insertPitchBendEvent(self, number): #pitch bend, from -100 to 100, accepts floats.
		if number <-100 or number > 100: raise BuilderException('Pitch bend: Out of range, -100 to 100')
		number = int(round((number/100.0)*8100.0))
		import midiutil
		evt = bmidilib.BMidiEvent()
		evt.type='PITCH_BEND'
		evt.time=self.ourTimingToTicks(self.currentTime)
		data1,data2 = midiutil.pitchBendToData(number) #bit grinding
		evt.pitch = data1
		evt.velocity = data2
		evt.channel=None #it will be assigned correctly at build time
		self.notes.append(evt)
	def setInstrument(self, instrument):
		allinstruments = bmidilib.bmidiconstants.GM_instruments
		try:
			found = int(instrument)
		except ValueError:
			found = bmidilib.bmidiconstants.GM_instruments_lookup(instrument)
			if found==None: raise BuilderException('Unable to find instrument %s, look at the end of "bmidiconstants.py" to see list of instruments, or find an online chart of midi instrument numbers.')
			
		self.instrument = found
		return allinstruments[found] #returns string representation of instrument name
			
	def save(self, strFilename):
		
		midifileobject = build_midi([self])
		midifileobject.open(strFilename, 'wb')
		midifileobject.write()
		midifileobject.close()
		
		
		
	def ourTimingToTicks(self, n):
		#If actual tempo were important, would use a TEMPO event.
		#However, we just use default tempo, and change the TICKS per what _we_ call a "qtr note" (the actual ticksPerQuarterNote is unchanged)
		#For MIDI files the default tempo is apparently 120.
		#Also, the default bmidilib ticksPerQuarterNote is 120.
		#I think that explains why the following works (I made it up, but am not sure where the numbers come from)
		
		#120 ticks/qtr note -> tickscale=120
		#240 ticks/qtr note ->tickscale=60
		factor = self.tempo/120.0
		tickscale = int(round(120.0/factor)) #what we call a qtr note is this many ticks
		return int(round(n * tickscale))
		
	def build_note_events(self, channel): #returns a list of events, not yet a track
		if len(self.notes)==0: raise BuilderException('Cannot save empty file, has to contain some notes.')
		evtlist = []
		
		#create track settings
		if self.volume != None:
			assert self.volume >= 0 and self.volume < 128
			evt = bmidilib.BMidiEvent()
			evt.type='CONTROLLER_CHANGE'
			evt.time = 0
			evt.channel=channel
			evt.pitch = 0x07 #main volume
			evt.velocity = self.volume
			evtlist.append(evt)
		
		if self.pan != None:
			assert self.pan >= 0 and self.pan < 128
			evt = bmidilib.BMidiEvent()
			evt.type='CONTROLLER_CHANGE'
			evt.time = 0
			evt.channel=channel
			evt.pitch = 0x0A #pan
			evt.velocity = self.pan
			evtlist.append(evt)
		
		if self.instrument != None:
			assert self.instrument >= 0
			evt = bmidilib.BMidiEvent()
			evt.type='PROGRAM_CHANGE'
			evt.time = 0
			evt.channel=channel
			evt.data = self.instrument
			evtlist.append(evt)
		
		
		for simplenote in self.notes:
			if isinstance(simplenote, bmidilib.BMidiEvent):
				simplenote.channel = channel #correct channel events at build time.
				evtlist.append(simplenote)
				continue
			
			startevt = bmidilib.BMidiEvent()
			startevt.type = 'NOTE_ON'
			startevt.time = self.ourTimingToTicks(simplenote.time)
			startevt.channel = channel if not simplenote.percussion else 10
			startevt.pitch = simplenote.pitch
			startevt.velocity = simplenote.velocity
			evtlist.append(startevt)
			
			endevt = bmidilib.BMidiEvent()
			endevt.type = 'NOTE_ON'
			endevt.time = self.ourTimingToTicks(simplenote.time + simplenote.duration)
			endevt.channel = channel if not simplenote.percussion else 10
			endevt.pitch = simplenote.pitch
			endevt.velocity = 0
			evtlist.append(endevt)
			
			
		#important to sort all of the events. ("rewind" could have made these be out of order)
		evtlist.sort()
		return evtlist
		

# actually make the MIDI file. returns BMidiFile object
def build_midi(builderObjects):
	assert len(builderObjects) > 0
	file = bmidilib.BMidiFile()
	file.ticksPerQuarterNote = 120
	
	def makeTracknameEvent():
		evt = bmidilib.BMidiEvent()
		evt.type = 'SEQUENCE_TRACK_NAME'
		evt.time = 0
		evt.data = 'bbuilder,BenFisher,2009'
		return evt
	def makeEndTrackEvent(time):
		evt = bmidilib.BMidiEvent()
		evt.type='END_OF_TRACK'
		evt.time = time
		evt.data = ''
		return evt
	
	#create conductor track
	cnd = bmidilib.BMidiTrack()
	file.tracks.append(cnd)
	cnd.events.append( makeTracknameEvent())
	#Here is where the tempo event would go, if we cared about precise timing
	if hasattr(builderObjects[0],'addFasterTempo') and builderObjects[0].addFasterTempo:
		evt = bmidilib.BMidiEvent()
		evt.type = 'SET_TEMPO'
		evt.time = 0
		evt.data = '\x02\x9d\xa4'
		cnd.events.append(evt)
	
	cnd.events.append( makeEndTrackEvent(0))
	
	for i in range(len(builderObjects)):
		channel = i+1
		noteEventList = builderObjects[i].build_note_events(channel) #returns a list of events, not yet a track
		noteEventList.insert(0, makeTracknameEvent())
		noteEventList.append(makeEndTrackEvent(noteEventList[-1].time))
		
		newtrack = bmidilib.BMidiTrack()
		newtrack.events = noteEventList
		file.tracks.append(newtrack)
	return file

# could be a class method of BMidiBuilder, but whatever
def joinTracks(builderObjects, strFilename):
	midifileobject = build_midi(builderObjects)
	midifileobject.open(strFilename, 'wb')
	midifileobject.write()
	midifileobject.close()

class SimpleNote():
	def __init__(self, pitch, time, duration, velocity,percussion=False):
		self.pitch=pitch; self.time=time; self.duration=duration; self.velocity = velocity
		self.percussion = percussion



if __name__=='__main__':
	b= BMidiBuilder()
	#~ b.addFasterTempo = True
	b.setInstrument('star theme')
	b.note('c',1)
	b.note('d',1)
	b.note('e',1)
	b.note('f',1)
	b.save('out.mid')
	
	#~ t1 = BMidiBuilder()
	#~ t1.tempo = 400
	#~ t1.setInstrument('star theme')
	#~ thelen= 20
	#~ t1.note(60, thelen)
	
	
	#~ t1.rewind(); steps = 100
	#~ for i in range(0,99,1):
		#~ t1.insertPitchBendEvent(i)
		#~ t1.rest(0.05)
	
	#~ t1.insertPitchBendEvent(0)
	
	#~ t1.save('out.mid')
	
	
	#~ t1 = BMidiBuilder()
	#~ t1.tempo = 40
	#~ for i in range(4):
		#~ t1.note('c', 0.25)
	
	#~ t2 = BMidiBuilder()
	#~ t2.tempo = 40
	#~ for i in range(5):
		#~ t2.note('g', 0.2)
	
	#~ joinTracks([t1, t2], 'o.mid')
	
	#~ b = BMidiBuilder()
	#~ b.tempo = 400
	#~ b.note('c3',1)
	#~ for j in range(5):
		#~ for i in range(127):
			#~ evt = bmidilib.BMidiEvent() #event time will be set when inserted
			#~ evt.type='PROGRAM_CHANGE'
			#~ evt.channel=1
			#~ evt.data = i
			#~ b.insertMidiEvent(evt)
			#~ b.note('c7',0.4)
			#~ b.rewind(0.3)
			#~ b.note('g6',0.3)
	
	#~ b.save('!.mid')
	#~ print b