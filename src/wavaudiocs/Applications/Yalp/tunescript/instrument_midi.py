#Pmidi output interface
from PMIDI.Composer import Sequencer
import time

import tunescript_bank

class Midi_bank(tunescript_bank.Tunescript_bank):
	def __init__(self):		
		# create the sequencer
		self.seq = Sequencer()

	def playSequence(self, seqyalp):
		midiInstrument = seqyalp.instrument[1] #the unrendered form of the instrument
		
		# create a new song
		song = self.seq.NewSong()

		# add an instrument to the song
		inst = song.NewVoice()
		inst.SetInstrumentByNumber(midiInstrument)

		# add a measure to the instrument
		meas = inst.NewMeasure()

		# add some notes to the measure
		# params are (in order): starting tick, duration, pitch, octave
		nCurrentTime = 0
		for note in seqyalp.notes:
			strName, nOctave = tunescript_bank.pitchToName(note.pitch)
			print nCurrentTime,note.duration,strName,nOctave
			
			if note.pitch==1:# This is a rest.
				meas.NewNote(nCurrentTime,note.duration/4,'C',4, 1) #play the note with 1 volume, basically a rest
			else:
				meas.NewNote(nCurrentTime,note.duration/4,strName,nOctave)
			nCurrentTime += note.duration/4

		# play the song
		self.seq.Play()
		# wait; the song plays asynchronously
		time.sleep(5)

	def saveSequence(self, strFilename):
		print 'Saving MIDI files not supported yet.'
		
	def queryVoice(self, strInstname):
		results = []
		if strInstname=='': return None
		strInstname = strInstname.lower()
		import PMIDI.Constants
		for i in range(len(PMIDI.Constants.GM_instruments)):
			str_midi_name = PMIDI.Constants.GM_instruments[i].lower()
			
			if strInstname in str_midi_name:
				results.append((str_midi_name,i))
		return results


if __name__=='__main__':
	parseMidiInstrument('guitar')
	#~ from yalpsequence import *
	#~ seqyalp = YalpSequence()
	#~ seqyalp.AddNotes('c5 d5 e5 f5 8g5 16a5 16b5 16c6')
	
	#~ playSequence(seqyalp)
	
		#~ meas.NewNote(0, 32, 'C', 4)
		#~ meas.NewNote(32, 32, 'D', 4)
		#~ meas.NewNote(63, 32, 'E', 4)
		#~ meas.NewNote(96, 32, 'F', 4)
	