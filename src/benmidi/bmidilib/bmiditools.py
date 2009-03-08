
'''
bmiditools.py, Ben Fisher 2008

Common tasks to do with Midi files.

'''

import bmidilib

def getTrackName(trackObject, defaultname='Unnamed Track'):
	searchFor = {}
	searchFor['TRACKNAME'] = defaultname
	results = getTrackInformation(trackObject, searchFor)
	return results['TRACKNAME']

def getTrackChannels(trackObject):
	searchFor = {}
	searchFor['CHANNELS'] = 1
	results = getTrackInformation(trackObject, searchFor)
	return results['CHANNELS'].keys()

def getTrackInstruments(trackObject):
	searchFor = {}
	searchFor['INSTRUMENTS'] = 1
	results = getTrackInformation(trackObject, searchFor)
	return results['INSTRUMENTS']

#Combine common track information gathering into one function, so that it can find all 3 at once without looping 3 times
def getTrackInformation(trackObject, searchFor):
	if not hasattr(trackObject, 'events'):
		raise 'Expected a BMidiTrack object, this does not appear to be one.'
	
	bSeeTracknames = 'TRACKNAME' in searchFor
	bSeeChannels = 'CHANNELS' in searchFor
	bSeeInstruments = 'INSTRUMENTS' in searchFor
	if bSeeTracknames: currentName = searchFor['TRACKNAME'] #what to return if no names found
	if bSeeChannels:  currentChannels = {}
	if bSeeInstruments:  currentInstruments = {}
	
	for evt in trackObject.events:
		if evt.type=='PROGRAM_CHANGE' and bSeeInstruments:
			currentInstruments[evt.data] = 1
		
		if evt.type=='SEQUENCE_TRACK_NAME' and bSeeTracknames:
			currentName = evt.data
			
	
	res = {}
	if bSeeTracknames: res['TRACKNAME'] = currentName
	if bSeeInstruments: res['INSTRUMENTS'] = currentInstruments.keys()
	return res


#create a solo/mute midi. trackArr should be an array the length of midiObject.tracks, with True or False
def muteTracksMidi(midiObject, trackArr):
	import copy
	newmidi = copy.copy(midiObject)
	newmidi.tracks = []
	for i in range(len(midiObject.tracks)):
		if trackArr[i]:
			newmidi.tracks.append(midiObject.tracks[i])
	#but: what about conductor track
	return newmidi
	

class BMidiSecondsLength():
	def __init__(self, midiObject):
		tempoChanges = [] #array of tuples, in format (time, tempo value)
		tempoChanges.append( (0, 500000) )#default tempo
		#get all tempo events
		for track in midiObject.tracks:
			tempoChanges.extend( ( (evt.time, bmidilib.dataToTempo(evt.data)) for evt in track.events if evt.type=='SET_TEMPO'))
		
		tempoChanges.sort(key=lambda item: item[0]) #unlikely but possible that there are tempo events in separate tracks
		self.tempoChanges = tempoChanges
		self.ticksPerQuarterNote = midiObject.ticksPerQuarterNote
	def ticksToSeconds(self, n):
		# tempo is in microseconds per quarter note.
		newarr = list(self.tempoChanges) + [ (n, -1) ] #add a final event, for processing conveniance
		newarr.sort(key=lambda elem: elem[0])
		totalTime = 0
		for i in range(len(newarr)):
			currentTempo = newarr[i][1]
			currentTime = newarr[i][0]
			nextTime = newarr[i+1][0]
			totalTime += (nextTime-currentTime) * 1.0e-6*currentTempo/self.ticksPerQuarterNote;
			if newarr[i+1][1]==-1: #reached the time we are interested in.
				break
		return totalTime
		
	def getTrackLength(self, trackObj):
		if len(trackObj.events) ==0: return 0
		else: return self.ticksToSeconds( trackObj.events[-1].time )
		
	def getOverallLength(self, midiObject):
		thelengths = [self.getTrackLength(track) for track in midiObject.tracks]
		return max(thelengths)
		
	def secondsToString(self, n):
		minutes = int(round(n))//60
		seconds = int(round(n)) % 60
		return '%d:%02d'%(minutes, seconds)


def transposeMidi(midiObject, amt, tracknum='all'): #track can be number, like 0 1 2 3 or string 'all'
	#note: this uses the notelist structure, and so should only be used on a freshly opened midi, not one created from scratch.
	def transposeTrack(trackObject, amt):
		for note in trackObject.notelist:
			note.pitch += amt
			note.startEvt.pitch += amt
			note.endEvt.pitch += amt
	if tracknum=='all':
		for trackObject in midiObject.tracks: transposeTrack(trackObject, amt)
	else:
		transposeTrack(midiObject.tracks[tracknum], amt)

'''excerpt midi. cuts out all of the notes. has to leave the non-note events, though... '''
def getMidiExcerpt(midiObject, nTics): #note: is destructive, modifies things
	
	#remove all note events, and pitch wheel events, before it.
	for track in midiObject.tracks: 
		track.events = [evt for evt in track.events if not (evt.time < nTics and (evt.type=='NOTE_ON' or evt.type=='NOTE_OFF' or evt.type=='PITCH_BEND'))]
	
	#ok, there is ONE latest event of every type, per channel. just find that one.
	latestMetaEvents = {}
	#latestMetaEvents[ (channel, eventType) ] = (track, event) 
	def getKey(evt): #return tuple to be used as a key for latestMetaEvents.
		if evt.type=='CONTROLLER_CHANGE': return (evt.channel, evt.type, evt.pitch)
		else: return (evt.channel, evt.type)
	
	for track in midiObject.tracks:
		beginningIndex=0
		for i in range(len(track.events)):
			evt = track.events[i]
			if evt.time > nTics:
				evt.time -= (nTics - spaceForEvents)
			else:
				key =  getKey(evt)
				if key not in latestMetaEvents or (key in latestMetaEvents and latestMetaEvents[key][1].time < evt.time): 
					latestMetaEvents[key] = (track, evt)
					
				#record and eliminate it.
				track.events[i] = None
		#get rid of Nones
		track.events = [evt for evt in track.events if not (evt==None)]
		
	#re-add the meta events (they're per-channel, not per track)
	for value in latestMetaEvents.itervalues():
		track, evt = value
		track.insert(0, evt)
	
	return None #as a signal that this modifies, not returns a copy
	
				
	


'''change all volume events to be multiplied by a certain amount. '''

'''
restructure midi, rearranging tracks by channel
track 0 contains all events with no channel attached (tempo, etc)
track 1 contains events from first channel in order
track 2 contains events from second channel in order
and so on.
'''

#first, create 16 tracks, then remove empty ones later
def restructureMidi(midiObject):
	import copy
	newmidi = copy.copy(midiObject)
	newmidi.tracks = [ bmidilib.BMidiTrack() for i in range(17) ] #conductor track and then a track for all 16 channels
	
	for oldtrack in midiObject.tracks:
		for evt in oldtrack.events:
			if evt.channel == None:
				if evt.type!='END_OF_TRACK' and evt.type!='SEQUENCE_TRACK_NAME':
					newmidi.tracks[0].events.append(evt)
			else:
				#channel is 1 based. There cannot be a channel 0 because of the way bmidilib interprets channel.
				newmidi.tracks[evt.channel].events.append(evt) 
	
	#get rid of empty tracks
	newmidi.tracks = [ trackObj for trackObj in newmidi.tracks if len(trackObj.events) > 0]
	
	#add end of track messages, for each track
	for track in newmidi.tracks:
		evt = bmidilib.BMidiEvent()
		evt.type='END_OF_TRACK'
		evt.time = track.events[-1].time + 1
		evt.data = ''
		track.events.append(evt)
		
		track.createNotelist() #add the notes
		
	return newmidi
	
	
#~ class VolumeAndPanModifier
#~ def getFirstVolumeAndPanEvents(midiObject, 


