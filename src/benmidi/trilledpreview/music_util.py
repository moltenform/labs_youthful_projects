



#~ class keySignature():
	#~ #immutable class. So F major = (1, False) and D major = (2, True)
	#~ def __init__(self, numberOfAccidentalsInSignature, bAccidentalsAreSharps):
	
	#~ #return one of: 'N', 'b', or '#'. for Scoreview.
	#~ def getNoteSpelling(self, noteNumber, ):
		
	#~ #return list . where middle C is 0. (so for bass clef, will be negative)
	#~ def getVisualKeySignature(self, clef)


#returns  tuple in form ('C', 4) or ('Ab', 4)
def noteToName(noteNumber, preferSharps=True):
	assert noteNumber >= 0
	octave = (noteNumber // 12) - 1
	scalenote = noteNumber % 12
	mapSharps = { 0: 'C', 1: 'C#', 2: 'D', 3: 'D#', 4: 'E', 5: 'F', 6: 'F#', 7: 'G', 8: 'G#', 9: 'A', 10: 'A#', 11: 'B' }
	mapFlats = { 0: 'C', 1: 'Db', 2: 'D', 3: 'Eb', 4: 'E', 5: 'F', 6: 'Gb', 7: 'G', 8: 'Ab', 9: 'A', 10: 'Bb', 11: 'B' }	
	if preferSharps:
		return (mapSharps[scalenote], octave)
	else:
		return (mapFlats[scalenote], octave)


#shortened version of pullPitch from bmidiscript's interpretsyntax.py
#returns None if it can't parse the string, otherwise a midi note number
def parseNoteString(s, defaultOctave=4):
	def eatChars(s, n=1):
		return (s[0:n], s[n:])

	if not s or s[0] not in 'abcdefgABCDEFG':
		return None
		
	noteletter, remaining = eatChars(s, 1)
	pitchnumber = {'c':0, 'd':2, 'e':4, 'f':5, 'g':7, 'a':9, 'b':11}.get(noteletter.lower(), None)
	if pitchnumber==None: return None
	
	# Parse accidentals, like Db, D# , D+, D-, and so on
	if remaining:
		if remaining[0] == 'b' : 
			pitchnumber -= 1
			_, remaining = eatChars(remaining, 1)
		elif (remaining[0] == '#' or remaining[0]=='+') :
			pitchnumber += 1
			_, remaining = eatChars(remaining, 1)
		elif (remaining[0] == '-' ) :
			pitchnumber -= 1
			_, remaining = eatChars(remaining, 1)
	
	octave = defaultOctave
	if remaining and remaining[0] in ('1','2','3','4','5','6','7','8','9'):
		octave = int(remaining[0])
		_, remaining = eatChars(remaining, 1)
	
	if remaining!='':
		return None #there shouldn't be any remaining chars.
	
	return pitchnumber + (octave+1)*12
	


def _tests():
	assert parseNoteString('c')!=None
	assert parseNoteString('c4')==60
	assert parseNoteString('c#4')==61; assert parseNoteString('c+4')==61;assert parseNoteString('c-4')==59;assert parseNoteString('c-4')==59;
	assert parseNoteString('C#4')==61; assert parseNoteString('C+4')==61;assert parseNoteString('C-4')==59;assert parseNoteString('C-4')==59;
	assert parseNoteString('c5')==72
	
if __name__=='__main__':
	_tests()