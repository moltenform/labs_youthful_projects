
import midirender_util

import sys; sys.path.append('..')
from bmidilib import bmidilib

STANDARDTEMPO = 500000 #midi spec, 120 bpm if no tempo evts
def queryChangeTempo(midiObject, default=None):
	if default==None: default=1.0
	tempoChangeEvts = getTempoChangeEvents(midiObject)
	firstTempo = STANDARDTEMPO if len(tempoChangeEvts)==0 else bmidilib.dataToTempo(tempoChangeEvts[0].data)
	# convert to bpm
	# tempo is in microseconds per beat (quarter note).
	microsecondsPerBeat = float(firstTempo)
	beatsPerSecond = (1/microsecondsPerBeat)/(1.0e-6)
	beatsPerMinute = beatsPerSecond * 60
	
	strPrompt = 'The current bpm is %d.' % int(beatsPerMinute)
	strPrompt += '\n Increase or decrease the tempo by what proportion?\n\n (Enter 2.0 to be twice as fast, 0.5 to be twice as slow, 1.0 to remain the same.)'
	res = midirender_util.ask_float(strPrompt, default=default, min=0.0,max=100.0, title='Change Tempo')
	if res==None or res==False: return None
	
	return res


def doChangeTempo(midiObject, proportion):
	tempoChangeEvts = getTempoChangeEvents(midiObject)
	if len(tempoChangeEvts)==0:
		evt = bmidilib.BMidiEvent()
		evt.type = 'SET_TEMPO'
		evt.time = 0
		evt.data = bmidilib.tempoToData( int(STANDARDTEMPO / float(proportion)))
		midiObject.tracks[0].events.insert(0, evt)
	else:
		for evt in tempoChangeEvts:
			oldTempo = bmidilib.dataToTempo(evt.data)
			evt.data = bmidilib.tempoToData( int(oldTempo / float(proportion)))
	
	return None #signal that we modified the object, not returned a copy

def getTempoChangeEvents(midiObject):
	#return format is (evtpan, evtvol, baremanyPan, barmanyVol)
	tempoevents = []
	for trackObject in midiObject.tracks:
		for evt in trackObject.events:
			if evt.type=='SET_TEMPO':
				tempoevents.append(evt)
	
	tempoevents.sort(key=lambda evt:evt.time)
	return tempoevents
	
