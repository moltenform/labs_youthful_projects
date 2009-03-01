
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
def muteTracksMidi(trackArr, midiObject):
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
		self.tempoChanges = tempoChanges
		self.ticksPerQuarterNote = midiObject.ticksPerQuarterNote
	def ticksToSeconds(self, n):
		newarr = list(self.tempoChanges) + [ (n, -1) ] #add a final event, for processing conveniance
		newarr.sort(key=lambda elem: elem[0])
		totalTime = 0
		for i in range(len(newarr)):
			currentTempo = newarr[i][1]
			currentTime = newarr[i][0]
			nextTime = newarr[i+1][0]
			totalTime += (nextTime-currentTime) * 1e-6*currentTempo/self.ticksPerQuarterNote;
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


'''excerpt midi. if give a stop time, then use that.'''
#~ def getMidiExcerpt


'''change all volume events to be multiplied by a certain amount. '''

'''
restructure midi, rearranging tracks by channel
track 0 contains all events with no channel attached (tempo, etc)
track 1 contains events from first channel in order
track 2 contains events from second channel in order
and so on.
'''

'''calculate wall-clock time of midi in minutes/seconds'''
#convert tick to time. because of tempo changes, this isn't easy...




