'''
Actual schematic of accepted syntax for a part of a note



FullNote = ('.') | (   


Pitch = 


lowernotes = one of 'abcdefg' 
uppernotes = one of 'ABCDEFG'




Note that /c/ can't have accent information. /c/! is not allowed. also chords like /[c|e]/ aren't allowed. and no /o/ percusision. maybe sometime else.
you can set volume events though, maybe
Note [.|c] is illegal


'''
Debug=True

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
	def go(self, s):
		
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
		for trackobj in self.trackObjs: trackobj.currentBend = 0
		self.haveSeenNotes = False #have we seen any notes yet?
		for line in lines:
			line = line.strip()
			if not line:
				continue
			elif line.startswith('('):
				interpretControlString(line)
			elif line.startswith('>>'):
				self.haveSeenNotes = True
				parts = line.split('>>')
				if len(parts)>Maxtracks: raise InterpException("Exceeded maximum of %d tracks."%Maxtracks)
				
				#each track keeps track of its own currentTime. At the end, though, we have to sync them all back up.
				longestTimeSeen = -1
				for i in range(len(parts)):
					self.interpretMusicString(parts[i], i)
					if self.trackObjs[i].currentTime > longestTimeSeen: longestTimeSeen = self.trackObjs[i].currentTime
				
				# restore track sync for all of the tracks.
				for i in range(Maxtracks): self.trackObjs[i].currentTime = longestTimeSeen
			else:
				self.haveSeenNotes = True
				self.interpretMusicString( line, 0 )
				
				# keep track sync for all of the tracks.
				longestTimeSeen = self.trackObjs[0].currentTime
				for i in range(Maxtracks): self.trackObjs[i].currentTime = longestTimeSeen
				
		actualtracks = [trackobj for trackobj in self.trackObjs if len(trackobj.notes) > 0]
		# convert notes of pitch 0 to rests.
		for trackobj in actualtracks:
			trackobj.notes = [note for note in trackobj.notes if (hasattr(note,'pitch') and note.pitch!=0)]
		
		#join all of the tracks, returning a midifile
		mfile = bbuilder.buildMidi(actualtracks)
		print mfile
		return mfile
		
		
	def interpretControlString(self,s):
		assert s.startsWith('(')
		if not s.endsWith(')'): raise InterpException('No closing ) for opening (, line %s'%s)
		# parse this lisp-like expression.
		# use class state,
		self.insertChangeInstrumentEvent(self.currentTime)
		
		#~ if setting=='tempo' and self.haveSeenNotes:
			#~ can't set the tempo after having notes. stop it.

	def interpretMusicString(self,s, track):
		s = s.replace(' ','').replace('\t','')
		
		while s!='':
			if Debug: print 'mainloop:::'+s
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
	
	def pullFullShortenedNote(self,s, track):
		if Debug: print 'pullFullShortenedNote:::'+s
		if not s or s[0] not in '/':
			return False, s
			
		if s.startswith('//'):
			first, remaining = eatChars(s, 2)
			otherIndex = remaining.find('//')
			if otherIndex==-1: raise InterpException('Could not find closing // for expression %s'%s)
			inside, remaining = eatChars(remaining, otherIndex)
			_,remaining = eatChars(remaining, 2) #eat the closing //
			scale = 4.0
		elif s.startswith('/'):
			first, remaining = eatChars(s, 1)
			otherIndex = remaining.find('/')
			if otherIndex==-1: raise InterpException('Could not find closing / for expression %s'%s)
			inside, remaining = eatChars(remaining, otherIndex)
			_,remaining = eatChars(remaining, 1) #eat the closing /
			scale = 2.0
		
		
		#now, inside should be a valid pitch. (can't have any duration/accent information, though).
		result, leftover = self.pullPitch(inside, track)
		if not result: raise InterpException('Invalid note inside shortnote-set %s'%s)
		if leftover != '': raise InterpException('Invalid note, something after note inside shortnote-set %s'%s)
		
		#ok, now modify the note that was made to make it half as long.
		self.trackObjs[track].rewind() #go back to start of note
		self.trackObjs[track].notes[-1].duration *= (1/scale)
		self.trackObjs[track].rest( self.trackObjs[track].notes[-1].duration )  #advance time to end of note
		
		return True, remaining
		
	def pullFullNotePerc(self,s, track):
		if Debug: print 'pullFullNotePerc:::'+s
		if not s: return False, s
			
		percmap = {'o':36 ,'s':38 ,'*':39 ,'=':42 ,'O':43 ,'+':46 ,'0':47 ,'x':49 ,'{}':52,'@':56 ,'X':57 ,'M':67 ,'m':68 ,'w':71 ,'W':72 }
		found = None
		for key in percmap:
			if s.startswith(key):
				found = key
				break
		if found==None:
			return False, s
			
		_, remaining = eatChars(s, key)
		notenumber = percmap[key]
		self.trackObjs[track].note(notenumber, self.normalNoteDuration, percussion=True) #will go into channel 10, percussion
		
		#now, modify the pitch that was created, optionally. allow for duration.
		_, remaining = self.pullPiecesVolumeDurationOptional(remaining, track, False)
		
		return True, remaining
		
		
	
	def pullFullNote(self,s, track):
		if Debug: print 'fullnote:::'+s
		if not s or s[0] not in '.abcdefgABCDEFG':
			return False,s
			
		remaining = s
		if remaining[0]=='.':
			_, remaining = eatChars(s, 1)
			# a rest. doesn't allow duration or anything afterwards, so special-case it.
			# rests are notes with pitch 0. this is so that anything like // trying to modify the last note has something to work with
			self.trackObjs[track].note(0, self.normalNoteDuration)
			return True, remaining
		
		#otherwise, we must have a pitch
		result, remaining = self.pullPitch(remaining, track)
		if not result: raise InterpException('Invalid pitch for note %s'%s)
		
		
		#now, modify the pitch that was created, optionally
		_, remaining = self.pullPiecesVolumeDurationOptional(remaining, track, False)
		
		_, remaining = self.pullPiecesPitchBendOptional(remaining, track)
		
		return True, remaining
		
	def pullFullNoteSet(self,s, track):
		if Debug: print 'pullFullNoteSet:::'+s
		if not s or s[0] not in '[':
			return False, s
		first, remaining = eatChars(s, 1)
		
		otherIndex = remaining.find(']')
		if otherIndex==-1: raise InterpException('Could not find closing ] for expression %s'%s)
		
		inside, remaining = eatChars(remaining, otherIndex)
		remaining = eatChars(remaining, 1) #eat the closing ]
		
		#now process the inside.
		insideParts = inside.split('|') #each part should be a note
		if len(insideParts)==1: raise InterpException('Chords should have more than one note, use syntax [c|e|g] %s'%s)
		
		for part in insideParts:
			#we must have a pitch
			result, leftover = self.pullPitch(part, track)
			if not result: raise InterpException('Invalid note inside note-set %s'%s)
			if leftover != '': raise InterpException('Invalid note, something after note inside note-set %s'%s)
			
			self.objTracks[track].rewind() #rewind, so that next note is placed on top.
		#go forward to compensate for last rewind
		self.objTracks[track].rest( self.objTracks[track].notes[-1].duration )
		
		#now, modify the pitches that were created, optionally
		_, remaining = self.pullPiecesVolumeDurationOptional(remaining, track, len(insideParts))
		
		#allow pitch bends on multiple notes- because this is per channel, it should just work, even though we added >1 notes
		_, remaining = self.pullPiecesPitchBendOptional(remaining, track)
		
		return True, remaining
		
	
	def pullPiecesVolumeDurationOptional(self,s, track, nNotesMultiple=False): #optional, so never 'fails' and returns False
		if Debug: print 'pullPiecesVolumeDurationOptional:::'+s
		remaining= s
		def changeLastVolume(n):
			if not nNotesMultiple:
				self.trackObjs[track].notes[-1].velocity = n
			else:
				for index in range(nNotesMultiple):
					self.trackObjs[track].notes[-1 - index].velocity = n
		def changeLastDuration(n):
			if not nNotesMultiple:
				self.trackObjs[track].rewind() #go back to start of note
				self.trackObjs[track].notes[-1].duration = n*self.normalNoteDuration
				self.trackObjs[track].rest( n*self.normalNoteDuration)  #advance time to end of note
			else:
				self.trackObjs[track].rewind() #go back to start of notes (assumes last nNotes are same length which should be case)
				for index in range(nNotes):
					self.trackObjs[track].notes[-1 - index].duration = n*self.normalNoteDuration
				self.trackObjs[track].rest( n*self.normalNoteDuration)  #advance time to end of note
		
		#now, maybe we will modify that pitch
		# modify volume of note c!
		if remaining.startswith('!!'): changeLastVolume(127); _, remaining = eatChars(remaining, 2)
		elif remaining.startswith('!'): changeLastVolume(100); _, remaining = eatChars(remaining, 1)
		elif remaining.startswith('??'): changeLastVolume(20); _, remaining = eatChars(remaining, 2)
		elif remaining.startswith('?'): changeLastVolume(40); _, remaining = eatChars(remaining, 1)
		if remaining[0] in '!?': raise InterpException('Too many volume modifiers, c!! allowed but not c!!!')
		
		# modify duration of note c,,		
		duration = 1
		while remaining[0]==',':
			_, remaining = eatChars(remaining, 1)
			duration += 1
		if duration!=1: changeLastDuration(addToDuration)
		
		return True, remaining
	
	
	
	
	def pullPiecesPitchBendOptional(self,s, track):
		if Debug: print 'pullPiecesPitchBendOptional:::'+s
		remaining= s
		if not remaining.startswith('~>'): return False, remaining
		_, remaining = eatChars(remaining, 2)
		
		snumber = ''
		while remaining[0] in '0123456789':
			c, remaining = eatChars(remaining, 1)
			snumber += c
		if snumber=='': raise InterpException('Pitch bend: Expected a number, as in c~>25')
		targetBend = int(snumber)
		if targetBend <-100 or targetBend > 100: raise InterpException('Pitch bend: Out of range, -100 to 100')
		
		bStaydetuned = False
		if remaining[0]=='~': bStaydetuned=True; _, remaining = eatChars(remaining, 1)
			
		#now make some pitch bends
		savedTime= self.trackObjs[track].currentTime
		self.trackObjs[track].rewind() #go back to start of note
		dur = self.trackObjs[track].notes[-1].duration
		steps = 100
		prevBend = self.trackObjs[track].currentBend
		timeInc = float(dur) / float(steps)
		bendInc = (targetBend-prevBend) / float(steps)
		for _ in range(steps):
			self.trackObjs[track].insertPitchBendEvent(prevBend)
			self.trackObjs[track].rest(timeInc)
			prevBend += bendInc
			
		#restore time and, by default, restore pitch
		self.trackObjs[track].currentTime = savedTime
		if not bStaydetuned: self.trackObjs[track].insertPitchBendEvent(0); self.trackObjs[track].currentBend = 0
		else: self.trackObjs[track].currentBend = targetBend
		
		return True, remaining
		
	
	def pullPitch(self,s, track):
		if Debug: print 'pullPitch:::'+s
		if not s or s[0] not in 'abcdefgABCDEFG':
			return False,s
			
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
		
		
		
	

		
	
if __name__=='__main__':
	inter = Interp()
	inter.go('\na b c\n')


