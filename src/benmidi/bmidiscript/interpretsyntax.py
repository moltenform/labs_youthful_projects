'''
Actual schematic of accepted syntax for a part of a note



FullNote = ('.') | (   


Pitch = 


lowernotes = one of 'abcdefg' 
uppernotes = one of 'ABCDEFG'







'''

import exceptions
class InterpException(exceptions.Exception): pass
import re

import sys; sys.path.append('..\\bmidilib')
import bbuilder

def stripComments(s):
	lines = s.replace('\r\n','\n').split('\n')
	for i in range(len(lines)):
		if '--' in lines[i]:
			lines[i] = lines[i].split('--')[0]
			
	return '\n'.join(lines)
def maxvalue(arr):
	maxfound = -1*sys.maxint
	for v in arr: 
		if v>maxfound: maxfound = v
	return maxfound

def eatChars(s, n=1):
	return (s[0:n], s[n:])
		

Maxtracks = 16
class Interp():
	normalNoteDuration = 4
	def go(s):
		
		s = s.replace('\r\n','\n')
		s = stripComments(s)
		
		#check for misplaced >>. they can only occur at the beginning of a line.
		found = re.findall(r'[^\n]>>', s)
		if found:  raise InterpException('The characters >> can only be at start of line. This was not the case: "%s"'%found[0])
		s = s.replace('\n>>','>>') #combine them onto one line.
		
		lines=s.split('\n')
		
		#create tracks
		self.trackObjs = [bbuilder.BMidiBuilder() for i in range(Maxtracks)] #eventually, will be assigned different channels, but not yet.
		self.state_octave = [5 for i in range(Maxtracks)] #keeps track of current octave for each track
		self.currentTime = 0
		
		for line in lines:
			line = line.strip()
			if not line:
				continue
			elif line.startswith('('):
				interpretControlString(line)
			elif line.startswith('>>'):
				parts = line.split('>>')
				if len(parts)>Maxtracks: raise InterpException("Exceeded maximum of %d tracks."%Maxtracks)
				restimes = []
				savedTime = self.currentTime
				for i in range(len(parts)):
					self.currentTime = savedTime
					self.interpretMusicString(parts[i], i)
					restimes.append(self.currentTime)
				self.currentTime = maxvalue(restimes)
			else:
				self.interpretControlString( line, 0 )
				
		# convert notes of pitch 0 to rests.
		
	def interpretControlString(s):
		assert s.startsWith('(')
		if not s.endsWith(')'): raise InterpException('No closing ) for opening (, line %s'%s)
		# parse this lisp-like expression.
		# use class state,
		self.insertChangeInstrumentEvent(self.currentTime)

	def interpretMusicString(s, track):
		s = s.replace(' ','').replace('\t','')
		
		while s:
			
			result, s = self.pullFullNote(s, track) #a rest counts
			if result: continue #found something, moving on to next entry
			result, s = self.pullFullNotePerc(s, track)
			if result: continue
			result, s = self.pullFullShortenedNote(s, track)
			if result: continue
			result, s = self.pullFullNoteSet(s, track)
			if result: continue
			
			raise InterpException('Cannot parse beginning at string %s'%s)
		
		return True

	def pullFullNote(s, track):
		if not s or s[0] not in '.abcdefgABCDEFG':
			return s, False
		first, remaining = eatChars(s, 1)
		if first=='.':
			# a rest. doesn't allow duration or anything afterwards, so special-case it.
			# rests are notes with pitch 0
			self.trackObjs[track].note(0, self.normalNoteDuration)
			self.currentTime += self.normalNoteDuration
			return True, remaining
		
		#otherwise, we must have a pitch
		result, remainder = self.pullPitch(remaining, track)
		if not result: raise InterpException('Invalid pitch for note %s'%s)
		
		def changeLastVolume(n):
			self.trackObjs[track].notes[-1].velocity = n
		def changeLastDuration(n):
			self.trackObjs[track].notes[-1].velocity = n
		
		#now, maybe we will modify that pitch
		# modify volume of note
		
		if remaining.startswith('!!'): changeLastVolume(127); _, remaining = eatChars(remaining, 2)
		elif remaining.startswith('!'): changeLastVolume(100); _, remaining = eatChars(remaining, 1)
		elif remaining.startswith('??'): changeLastVolume(20); _, remaining = eatChars(remaining, 2)
		elif remaining.startswith('?'): changeLastVolume(40); _, remaining = eatChars(remaining, 1)
		
		
	
	def pullPitch(s, track):
		if not s or s[0] not in 'abcdefgABCDEFG':
			return s,False
			
		noteletter, remaining = eatChars(s, 1)
		pitchnumber = {'c':0, 'd':2, 'e':4, 'f':5, 'g':7, 'a':9, 'b':11}[noteletter.lower()]
		
		
		# Parse accidentals, like c# , c+, c-, and so on
		hasAccidental = False #don't allow multiple accidentals like Ab#
		if noteletter in 'ABCDEFG':
			if remaining[0] == 'b' and not hasAccidental:  #we allow Ab to mean a flat, but not ab to mean a flat. kind of a special case.
				pitchnumber -= 1
				_, remaining = eatChars(remaining, 1)
				hasAccidental = True
		if (remaining[0] == '#' or remaining[0]=='+') and not hasAccidental:
			pitchnumber += 1
			_, remaining = eatChars(remaining, 1)
			hasAccidental = True
		if (remaining[0] == '-' ) and not hasAccidental:
			pitchnumber -= 1
			_, remaining = eatChars(remaining, 1)
			hasAccidental = True
		
		# Parse octave, like c', c4, and so on. c_ c__ c___
		octave = self.state_octave[track] #default to the last octave seen
		hasOctave = False
		
		if remaining.startswith("'''") and not hasOctave:
			octave += 3
			_, remaining = eatChars(remaining, 3)
			hasOctave = True
		elif remaining.startswith("''") and not hasOctave:
			octave += 2
			_, remaining = eatChars(remaining, 2)
			hasOctave = True
		elif remaining.startswith("'") and not hasOctave:
			octave += 1
			_, remaining = eatChars(remaining, 1)
			hasOctave = True
		if remaining.startswith("'"): raise InterpException("Limit of 3 octave tics, c''' is allowed but not c''''.")
		if remaining.startswith("___") and not hasOctave:
			octave -= 3
			_, remaining = eatChars(remaining, 3)
			hasOctave = True
		elif remaining.startswith("__") and not hasOctave:
			octave -= 2
			_, remaining = eatChars(remaining, 2)
			hasOctave = True
		elif remaining.startswith("_") and not hasOctave:
			octave -= 1
			_, remaining = eatChars(remaining, 1)
			hasOctave = True
		if remaining.startswith("_"): raise InterpException("Limit of 3 octave tics, c___ is allowed but not c____.")
		
		if remaining[0] in '123456789' and not hasOctave:
			octave = int(remaining[0])
			_, remaining = eatChars(remaining, 1)
			hasOctave = True
			 self.state_octave[track] = octave #store the octave we've seen
			
		finalpitch = pitchnumber + octave*12
		self.trackObjs[track].note(finalpitch, self.normalNoteDuration)
		return True, remaining
		
		
		
	
	def pullFullShortenedNote(s, track):
		if s[0] not in '/':
			return s,False
		nextIndex = s.find('/',1)
		if nextIndex==-1: 
			pass
		#make the note
		#look up last note made, and multiply its duration by 1/2, and set back current time by 1/2.
		#each normal qtr note has duration "4".
		
	#~ def pullPiecePitch

		
	


'''(split by newlines)

The following must be mutually exclusive!
They mark what to interpret the next as.
(kill all whitespace first- no, don't do this.)

whitespace
	(tab, space)
control
	(newline),paren(,paren), >>

pitch
	abcdefg and ABCDEFG
pitch_perc
	all percussion ones
pitch_rest
	.
pitch_dur
	,
	
pitchset
	[
pitchshort
	/




like a state machine.





result, remaining = pullPitch(remaining)
pullLine-> creates builderObject from one line.
'''

