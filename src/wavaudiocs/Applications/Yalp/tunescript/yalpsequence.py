

class Environment:
	lastpitch = 60
	lastduration = 64
	lastoctave = 4
class YalpSequence:
	notes = []
	tempo = 60
	instrument = ('midi', 1) #Means midi bank, using instrument 1
	env = None
	bank_midi = None
	bank_wave = None
	bank_synth = None
	def AddNotes(self, strNotes):
		if self.env==None: self.env = Environment()
		astrNotes = strNotes.split()
		for strN in astrNotes:
			note = CreateNote(strN, self.env)
			if note==-1:
				print 'Invalid note:', strN
				return -1
			self.notes.append(note)
		return 1
		
			

class Note:
	pitch = 60 #Use pitch 1 to represent a "rest"
	duration = 4 #Quarter note
	def __init__(self, pitch, duration):
		self.pitch = pitch
		self.duration = duration
	def __repr__(self):
		return strz('Pitch:',self.pitch, 'Dur:', self.duration)
	def __str__(self):
		return strz('Pitch:',self.pitch, 'Dur:', self.duration)

def getOctave(pitch):
	return int(pitch / 12) - 1

def getNoteName(nPitch):
	nOctave = int(nPitch / 12)-1
	nNote = nPitch % 12
	map = {0:'C',1:'C#',2:'D',3:'D#',4:'E',5:'F',6:'F#',7:'G',8:'G#',9:'A',10:'A#',11:'B'}
	return map[nNote].lower() +  str(nOctave)
	
def gowhile(strt, chars): #eat a string until you get something not in chars. Returns the rest, too.
	for i in range(len(strt)):
		if strt[i] not in chars:
			return (strt[0:i], strt[i:])
	return (strt, '')

# Class things for Note
def CreateNote(strn, env):
	def parseDuration(strn,note,env):
		strd, strRest = gowhile(strn, '0123456789:')
		if strd=='': return strRest
		
		dotted = False
		if strd[-1] == ':':
			dotted = True
			strd = strd[:-1]
			
		map = {'1':256, '2':128, '4':64, '8':32, '16':16, '32':8, '64':4, '128':2}
		if strd not in map:
			print 'Invalid duration', strd
			return strRest
		else:
			duration = map[strd]
			if dotted: duration=int(duration * 1.5)
			note.duration = duration
			env.lastduration = duration
			return strRest

	def parsePitch(strn,note,env):
		strd, strRest = gowhile(strn, '.abcdefgr+-#')
		if strd =='.':
			return strRest
		elif strd == 'r': #There is no reason for rest sharp or flat.
			note.pitch = 1
			return strRest
		elif strd == '':
			#print 'Invalid note.', strd
			return strRest
		alter = ''
		if strd[-1] == '-':
			alter = 'flat'
			strd = strd[:-1]
		elif strd[-1] == '+' or strd[-1] == '#':
			alter = 'sharp'
			strd = strd[:-1]
		map = {'c':0, 'd':2, 'e':4, 'f':5, 'g':7, 'a':9, 'b':11}
		if strd not in map:
			print 'Invalid note.',strd
			return strRest
		else:
			pitch = map[strd]
			if alter=='flat': pitch -=1
			elif alter == 'sharp': pitch += 1
			pitch += env.lastoctave*12
			env.lastpitch = pitch
			note.pitch = pitch
			return strRest
		
	def parseOctave(strn,note,env):
		strd, strRest = gowhile(strn, '`_0123456789')
		
		if strd=='': return strRest
		if strd=='`':  #Do not affect the environment in this case
			note.pitch += 12
			return strRest
		elif strd=='_':
			note.pitch -= 12
			return strRest
		
		#print 'currently:', note.pitch
		currentnote, currentoctave = note.pitch%12, getOctave(note.pitch)
		map = {'2':2,'3':3,'4':4,'5':5,'6':6,'7':7,'8':8}
		if strd not in map:
			print 'Invalid octave',strd
			return strRest
		else:
			octave = map[strd]
			env.lastoctave = octave
			note.pitch = currentnote + (octave+1)*12
			return strRest
	# Reject invalid notes
	if strn=='': return -1
	elif gowhile(strn,'0123456789:')[1] == '': return -1 #Don't accept a string of all numbers
	
	# Note that the functions can alter the env
	note = Note(env.lastpitch, env.lastduration)
	strn = parseDuration(strn, note, env)
	strn = parsePitch(strn, note, env)
	strn = parseOctave(strn, note, env)
	if strn != '':
		print 'Unknown note format: ',strn, 'found after note.'
		return -1
	
	return note
	
class Track:
	name = ''
	instrument = 0
	notes = []
	
class Sequence:
	name = ''
	tracks = []

def strz(*args): return ''.join(map(str,args))

def _testParsing():
	class Bag:
		pass

	t=Bag()
	t.npassed = 0
	t.ntests = 0
	def testcase(strn, pitch, dur,t):
		env = Environment()
		n = CreateNote(strn, env)
		t.ntests += 1
		if n==-1 or n.pitch!=pitch or n.duration!=dur:
			print '!FAIL! Expected:',pitch, dur,' Got:',n
		else:
			t.npassed += 1
	testcase('.', 60, 64, t)
	testcase('`', 72, 64, t)
	testcase('_', 48, 64, t)
	testcase('8a#',58, 32, t)
	testcase('8:a#',58, 48, t)
	
	testcase('8.', 60, 32, t)
	testcase('8a#',58, 32, t)
	testcase('8e7',100, 32, t)
	testcase('c4',60, 64, t)
	testcase('c',48, 64, t)
	testcase('d4',62, 64, t)
	
	print t.npassed, ' of ', t.ntests, ' passed.'


if __name__ == '__main__':
	_testParsing()
	
	assert getNoteName(60)=='c4'
	assert getNoteName(100)=='e7'
	assert getNoteName(102)=='f#7'
	assert getNoteName(39)=='d#2'
	
	env = Environment()
	n = CreateNote('b-4', env)
	print n
	
	