from yalpsequence import *

# There should only be one instance of Midi_bank because it ties the MIDI resources
bank_midi = None
bank_synth = None
bank_wave = None

def get_bank(s):
	# Get the bank. (Only creates a bank if it is used)
	global bank_midi,bank_synth,bank_wave
	if s=='midi':
		import instrument_midi
		if bank_midi==None: bank_midi = instrument_midi.Midi_bank()
		return bank_midi
	elif s=='synth':
		import instrument_synth
		if bank_synth==None: bank_synth = instrument_synth.Synth_bank()
		return bank_synth
	elif s=='wave':
		import instrument_wave
		if bank_wave==None: bank_wave = instrument_synth.Wave_bank()
		return bank_wave
	return None

def playSequence(seq):
	bank = get_bank(seq.instrument[0])
	bank.playSequence(seq)

def shell():
	env = Environment() # Contains state about last duration, etc.
	currentInstrument = ('midi', 1)
	memory_sequences = {}
	sequence = None
	while True:
		strIn = raw_input('>')
		if strIn == '': continue
		if strIn == 'exit' or strIn=='q': return
		
		if strIn[0] == ':':
			strIn = strIn[1:]
			voicetype, voicename = strIn.split(' ',1)
			voicetype = voicetype.lower()
			if voicetype=='midi':
				bank = get_bank('midi')
				result = bank.user_queryVoice(voicename)
				if result!=None:
					# Return format is ('voicename',voicenumber)
					currentInstrument = ('midi', result[1])
					
			elif voicetype=='wave':
				bank = get_bank('wave')
				result = bank.user_queryVoice(voicename)
				if result!=None:
					# Return format is ('filename','fullpath')
					currentInstrument = ('wave', result[1])
			
			elif voicetype=='synth':
				bank = get_bank('synth')
				result = bank.user_queryVoice(voicename)
				if result!=None:
					# Return format is 'synthname'
					currentInstrument = ('synth', result)
			else:
				print 'Invalid bank type. Choose one of midi,wave, or synth.'
				
			continue
			
			
		strSplit = strIn.split()
		
		if len(strSplit) > 1 and strSplit[1] == '=':
			if not _isvalidname(strSplit[0]):
				print 'Name of seq,',strSplit[0],' not valid.'
				continue
			if len(strSplit)==2:
				print 'No sequence'
				continue
			strExpression = ' '.join(strSplit[2:])
			sequence = parse(strExpression, env, currentInstrument)
			if sequence == -1:
				print 'Sequence did not parse'
				continue
			memory_sequences[strSplit[0]] = sequence
			playSequence(sequence)
			continue
		
		# is it a saved sequence?
		if strSplit[0] in memory_sequences:
			nTimes = 1
			strSaveTo = None
			if len(strSplit)>1:
				if strSplit[1].startswith('x'):
					nTimes = int(strSplit[1][1:])
				elif strSplit[1] == '>':
					strSaveTo = strSplit[2]
				else:
					print 'Unknown argument:', strSplit[1]
			import copy
			newseq = copy.deepcopy(memory_sequences[strSplit[0]])
			thenotes = copy.deepcopy(memory_sequences[strSplit[0]].notes)
			for i in range(nTimes-1):
				newseq.notes.extend(thenotes)
			playSequence(newseq)
			
			if strSaveTo != None:
				if newseq.instrument[0]=='midi':print 'Cannot save to midi yet'
				elif newseq.instrument[0]=='synth':
					import instrument_synth
					instrument_synth.saveSequence(newseq, strSaveTo)
			
			continue
		else:
			# Otherwise, treat as a sequence and play it
			strExpression = ' '.join(strSplit)
			sequence = parse(strExpression, env, currentInstrument)
			if sequence == -1:
				print 'Expression did not parse'
				continue
				
			playSequence(sequence)
			
def parse(strIn, env, currentInstrument):
	"""Takes a string, returns a Sequence"""
	seq = YalpSequence()
	seq.notes = []
	seq.instrument = currentInstrument
	ret = seq.AddNotes(strIn)
	print 'notes::::' , strIn
	if len(seq.notes)==0: return -1
	else: return seq

def _isvalidname(strIn):
	if strIn.isalnum():
		return True
	return False

if __name__=='__main__':
	shell()
