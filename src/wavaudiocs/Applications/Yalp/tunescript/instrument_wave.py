import tunescript_bank

from yalp.wave_file import *
from yalp.wave_synthesis import *

import yalpsequence

class Wave_bank(Tunescript_bank):
	def __init__(self):
		cachedAudio = {} #Rendered audio samples
		cachedAudioOriginals = {} #The basis pitch and content of the original wave
		cachedAudioBitrates = {}
		
	
	def playSequence(self,seq):
		# First, make sure we have all of the notes in the sequence:
		audiodata= self.buildWave(seq)
		audiodata.play_memory()
		del audiodata.samples
		del audiodata

	def saveSequence(self, seq, filename):
		print 'Attempting to save to',filename
		if not filename.endswith('.wav'): filename += '.wav'
		audiodata= self.buildWave(seq)
		audiodata.save_wave(filename)
		del audiodata.samples
		del audiodata
		print 'Saved to',filename

	def buildWave(self, seq):
		assert seq.instrument[0]=='wave'
		strInstrumentPath = seq.instrument[1]
		
		# strInstrumentPath is the relative path and filename of this.
		
		# First, find the bitrate of this.
		if strInstrumentPath in self.cachedAudioBitrates:
			nBits, nSampleRate = self.cachedAudioBitrates[strInstrumentPath]
		else:
			nBits, nSampleRate = self.determineBitrate(strInstrumentPath)
			self.cachedAudioBitrates[strInstrumentPath] = (nBits, nSampleRate)
		
		# Create an empty wave file and add stuff to it
		seqaudiodata = WaveFile(nBits=nBits, nSampleRate=nSampleRate)
		
		for note in seq.notes:
			if note.pitch == 1: #This represents a rest
				seqaudiodata.add_silence(tunescript_bank.scaleTime(note.duration))
			else:
				if (strInstrumentPath, note.pitch) in self.cachedAudio:
					noteSample = self.cachedAudio[(strInstrumentPath, note.pitch)]
				else:
					noteSample = self.renderNote(strInstrumentPath, note.pitch)
					self.cachedAudio[(strInstrumentPath, note.pitch)] = noteSample
				
				tunescript_bank.add_at_length(seqaudiodata.samples, noteSample.samples, int(tunescript_bank.scaleTime(note.duration) * seqaudiodata.nSampleRate))
		return seqaudiodata
		
	def renderNote(self, strInstrumentPath, n):
		# Render a note at a certain pitch
		print 'Rendering',n, '...'
		
		# Get the original pitch and data for fundamental note of the sound.
		if strInstrumentPath in self.cachedAudioOriginals:
			audioFundamental, audiodata = self.cachedAudioOriginals[strInstrumentPath]
		else:
			audioFundamental, audiodata = self.determineAudioFundamental(strInstrumentPath)
			self.cachedAudioOriginals[strInstrumentPath] = audioFundamental, audiodata
			self.cachedAudio[(strInstrumentPath, audioFundamental)] = audiodata
			
			# We were lucky - the user asked for the fundamental, so we don't need to do anything
			if n==audioFundamental: return audiodata

		
		# Scale the 'fundamental:
		currentFreq = tunescript_bank.midiToFrequency(nBase)
		desiredFreq = tunescript_bank.midiToFrequency(n)
		
		import copy
		import audio_effects
		newaudio = audioFundamental.empty_copy()
		newaudio.samples = (audioFundamental.samples).__deepcopy__()
		audio_effects.fx_change_pitch(newaudio, desiredFreq/currentFreq)
		return newaudio
		

	def determineAudioFundamental(self, strInstrumentPath):
		# First, get the audio data
		audiodata = WaveFile(strInstrumentPath)
		
		def getFundamental(strInstrumentPath):
			import os
			# Now, get the fundamental.
			filepath, filename = os.path.split(strInstrumentPath)
			filename = '.'.join(filename.split('.')[:-1]) #strip extension
			note = filename.split('_')[-1]
			try:
				n = int(note)
			except:
				n = None
			
			# If a number, use that
			if n != None: return n
			
			# If not a number, try to parse as name
			n = tunescript_bank.nameToPitch(note)
			if n != None: return n
			
			return None
		n = getFundamental(strInstrumentPath)
		if n==None: n = 60 #default to middle C
		return n, audiodata
		
	
	def queryVoice(self, strInstname):
		if strInstname=='': return None
		strInstname = strInstname.lower()
		
		# Load everything in the 'bank' folder and in the 'mysounds' folder
		import os
		directories = ('.' +os.sep+ 'tunescript' + os.sep + 'bank', '.'+os.sep+'mysounds')
		
		thewaves = []
		for dir in directories:
			lwaves = os.listdir(dir)
			thewaves.extend( [ (lwave, os.path.join(dir, lwave)) for lwave in lwaves if lwave.endswith('.wav')])
			
		results=[]
		
		for filename_path in thewaves:
			if strin=='_': continue
			if strInstname in filename_path[0].lower():
				results.append(filename_path)
		return results

		
		
