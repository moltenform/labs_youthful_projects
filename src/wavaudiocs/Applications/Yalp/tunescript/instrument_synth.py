import tunescript_bank

from yalp.wave_file import *
from yalp.wave_synthesis import *


class Synth_bank(Tunescript_bank):
	def __init__():
		self.sampleSize = 1
		self.sampleRate = 22050
		self.synthList = get_instruments()

		# Note that we contain a cache in this file
		self.cachedAudio = {}
		
	def playSequence(self, seq):
		# First, make sure we have all of the notes in the sequence:
		audiodata = self.buildWave(seq)
		audiodata.play_memory()
		del audiodata.samples
		del audiodata

	def saveSequence(self, seq, filename):
		print 'Attempting to save to',filename
		if not filename.endswith('.wav'): filename += '.wav'
		audiodata = self.buildWave(seq)
		audiodata.save_wave(filename)
		del audiodata.samples
		del audiodata
		print 'Saved to',filename

	def buildWave(self, seq):
		seqaudiodata = WaveFile(nBits=8*self.sampleSize, nSampleRate=self.sampleRate)
		assert seq.instrument[0]=='synth'
		strInstrument = seq.instrument[1]
		for note in seq.notes:
			if note.pitch == 1:
				seqaudiodata.add_silence(tunescript_bank.scaleTime(note.duration))
			else:
				if (strInstrument, note.pitch) not in self.cachedAudio: # If there's a note we don't have, we'll have to cache it.
					self.cacheNote(strInstrument, note.pitch)
				noteSample = self.cachedAudio[(strInstrument, note.pitch)]
				
				tunescript_bank.add_at_length( seqaudiodata.samples, noteSample.samples, int(tunescript_bank.scaleTime(note.duration) * seqaudiodata.nSampleRate))
		return seqaudiodata
		
	def cacheNote(self, strInstrument, n):
		if (strInstrument, n) in self.cachedAudio:
			return
		print 'Caching',n, '...'
		audiodata = WaveFile(nBits=8*self.sampleSize, nSampleRate=self.sampleRate)
		freq = tunescript_bank.midiToFrequency(n)
		synthesize(audiodata, strInstrument, freq, 2., 0.5) #2 seconds should be sufficient
		self.cachedAudio[(strInstrument, n)] = audiodata


	def queryVoice(self, strInstname):
		results=[]
		if strInstname=='': return None
		strInstname = strInstname.lower()
		for strin in self.synthList:
			if strin=='_': continue
			if strInstname in strin.lower():
				results.append(strin)
		return results
		

#~ cacheNote('sine', 70)
#~ cachedAudio[('sine', 60)].play_memory()