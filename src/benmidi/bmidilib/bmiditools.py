
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

#~ def getTrackChannels(trackObject):
	#~ searchFor = {}
	#~ searchFor['CHANNELS'] = 1
	#~ results = getTrackInformation(trackObject, searchFor)
	#~ return results['CHANNELS'].keys()

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
	#~ bSeeChannels = 'CHANNELS' in searchFor
	bSeeInstruments = 'INSTRUMENTS' in searchFor
	if bSeeTracknames: currentName = searchFor['TRACKNAME'] #what to return if no names found
	#~ if bSeeChannels:  currentChannels = {}
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
	
