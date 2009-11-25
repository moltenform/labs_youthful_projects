
from notesrealtimerecorded import NotesinterpretException, NotesRealtimeNoteEvent
import testdepiction
import bisect

import music_util
import trclasses

from mingus.containers import Composition,Track, Instrument, Note,NoteContainer, Bar

#quantize is 4, 8, 16, for qtr note, 8th, 16th

def createQuantizedList(objResults, timesig, quantize):
	#quantizes and eliminate overlapping notes
	if isinstance(objResults, NotesinterpretException): raise objResults
	vis = testdepiction.TestDepiction()
	listPulses, listNotes = objResults.listPulses, objResults.listNotes
	
	# how many subdivisions?
	divisions = quantize / 4 #because each pulse is a qtr note
	assert quantize >= 4
	#for example, if quantize by 8th note, and each pulse is a qtr note, there are two possible values per pulse.
	

	#create quantization list (contains all acceptible times).
	#divides linearly. one might also smooth intra-pulse timing with a spline...
	listQuantize=[]
	listQuantize.append(0.0)
	prevTime = 0.0
	for pulseTime in listPulses:
		inc = (pulseTime-prevTime)/divisions
		for i in range(divisions):
			listQuantize.append(prevTime + i*inc)
		prevTime = pulseTime
	
	vis.addPreQuantize(listQuantize, listNotes)
	#quantize all of the times
	for note in listNotes:
		note.start = findclosest(listQuantize, note.start)
		note.end = findclosest(listQuantize, note.end)
		#now times are integers, corresponding to multiples of time unit. So if quantize=8, each unit is an 8th note
	
	vis.addQuantized(listNotes)
	
	
	#now get rid of overlapping notes, using some heuristics.
	#this logic is not currently used since polyphony no longer supported...
	#although could occur if two notes played rapidly
	listFinal = []
	while listNotes:
		currentPitchGroup = [listNotes.pop(0)]
		currentPitchStartTime = currentPitchGroup[0].start
		while listNotes and listNotes[0].start==currentPitchStartTime:
			currentPitchGroup.append(listNotes.pop(0))
		
		#length of this pitch group is the longest in the group...
		currentPitchEndTime = 0
		for note in currentPitchGroup: 
			if note.end>currentPitchEndTime: currentPitchEndTime=note.end
		
		#...unless it is interrupted by some other note.
		if listNotes and listNotes[0].start < currentPitchEndTime:
			currentPitchEndTime = listNotes[0].start
		
		#can't have a note of length 0.
		if currentPitchEndTime==currentPitchStartTime:
			currentPitchEndTime += 1
		
		#quantize currentPitchEndTime to have it fill to next note?
		if listNotes and currentPitchEndTime != listNotes[0].start:
			if listNotes[0].start - currentPitchEndTime < quantize:
				currentPitchEndTime = listNotes[0].start #bleed into next note.
		assert currentPitchEndTime>currentPitchStartTime
		
		listFinal.append(( [m.pitch for m in currentPitchGroup], currentPitchStartTime, currentPitchEndTime))
	
	vis.draw(listPulses, listFinal)
	return listFinal
	

def createIntermediateList(listNotes, timesig, quantize, bIsTreble, bSharps):
	#inserts rests between notes, standardizes all durations to be whole,half,qtr,8th,and so on, tied notes if necessary
	
	divisions = quantize / 4 #because each pulse is a qtr note
	assert quantize >= 4
	
	intermed = IntermediateList(timesig, bSharps=bSharps)
	
	fQtrnotespermeasure = float(timesig[0]) / (timesig[1]/4)
	nTimestepspermeasure = int( fQtrnotespermeasure * intermed.baseDivisions)
	
	#insert rests between notes
	newlist = []
	for i in range(len(listNotes)):
		if listNotes[i][1]==listNotes[i][2]: raise 'Cannot have note of length 0.'
		newlist.append(listNotes[i])
		if i<len(listNotes)-1:
			restLength = listNotes[i+1][1]-listNotes[i][2]
			assert restLength >= 0
			if restLength != 0:
				newlist.append(((0,), listNotes[i][2], listNotes[i+1][1]))
				#~ newlist.append(NotesRealtimeNoteEvent(pitch=(0,), start=listNotes[i][2], end=listNotes[i+1][1]))
	
	#convert to a standard time base, where each unit is 1/baseDivisions of a qtr note.
	def qStepToTimeBase(x):
		return int((float(x) / (divisions) * intermed.baseDivisions))
	
	
	#insert notes, creating tied notes when necessary.
	currentMeasureTime = 0
	for note in newlist:
		length = qStepToTimeBase(note[2]-note[1])
		results = intermed.effectivelyTieLongNotesBarlines(currentMeasureTime, length, nTimestepspermeasure)
		for i in range(len(results)):
			duration = results[i]
			bIsTied = i!=len(results)-1
			
			newnote = NotesRealtimeNoteEvent(pitch=note[0],start=currentMeasureTime,end=currentMeasureTime+duration)
			newnote.isTied = bIsTied
			intermed.noteList.append(newnote)
			
			currentMeasureTime += duration
			
			assert currentMeasureTime<= nTimestepspermeasure
			if currentMeasureTime == nTimestepspermeasure:
				# begin the next measure...
				currentMeasureTime=0
				
	return intermed
	
def createMingusComposition(intermed, timesig, quantize, bIsTreble, bSharps):
	comp = Composition()
	comp.set_title('Trilled Results')
	comp.set_author('Author')
	if bIsTreble: ins = TrebleInstrument('')
	else: ins=BassInstrument('')
	
	track = Track(ins)
	track.name = 'Part 1'
	comp.add_track(track)

	assert len(timesig)==2 and isinstance(timesig[0],int) and isinstance(timesig[1],int)
	firstbar = Bar(meter=timesig)
	track.add_bar(firstbar)
	
	mapDurs={
		intermed.baseDivisions*4: 1.0,
		intermed.baseDivisions*2: 2.0,
		intermed.baseDivisions*1: 4.0,
		intermed.baseDivisions*0.5: 8.0,
		intermed.baseDivisions*0.25: 16.0,
		intermed.baseDivisions*0.125: 32.0,
		intermed.baseDivisions*0.0625: 64.0,
			}
	
	
	for note in intermed.noteList:
		if note.pitch==0 or note.pitch==(0,): # a rest
			thepitches = tuple()
		else: # a note
			thepitches = []
			for pitch in note.pitch:
				pname, poctave = music_util.noteToName(pitch,bSharps)
				thepitches.append(pname+'-'+str(poctave))
		
		dur = note.end - note.start
		if dur not in mapDurs: raise NotesinterpretException('Unknown duration')
		notecontainer = NoteContainer(thepitches)
		notecontainer.tied =  note.isTied
		bFit = track.add_notes(notecontainer, mapDurs[dur])
		assert bFit
	
		#note that, naturally, will enforce having correct measure lines, since going across barline throughs exception
	
	return comp
	
	
	
class IntermediateList():
	#an intermediate list of notes. timings in terms of baseDivisions.
	noteList = None # list of NotesRealtimeNoteEvent.
	bSharps = True
	timesig=None
	baseDivisions = 64 #each qtr note can be divided into this many pieces. 64 units always = 1 qtr note
	
	atomicnotes = [int(baseDivisions/n) for n in [ 0.25, 0.5, 1, 2, 4, 8, 16, 32]]
	#							     whole, half, qtr, 8th, 16, 32, 64, 138
	
	def __init__(self, timesig, bSharps=True):
		self.timesig=timesig
		self.bSharps=bSharps
		self.noteList=[]

	def effectivelyTieLongNotes(self, currentMeasureTime, length): 
		#spell a long note on the beat. for example, turn something 3 beats long into halfnote tied to qtr
		# probably only looks well for duple times!
		#returns list of atoms
		results = []
		lengthleft = length
		
		while lengthleft>0:
			found=False
			for atom in self.atomicnotes:
				if atom  <= lengthleft and isDivisible(currentMeasureTime, atom):
					results.append(atom)
					lengthleft -= atom
					currentMeasureTime += atom
					found=True
					break
			assert found
					
		assert lengthleft == 0
		return results
		
	def effectivelyTieLongNotesBarlines(self, currentMeasureTime, length, measureLength): 
		#spell a long note, taking measures into account (must tie across barlines)
		#returns list of atoms.
		results = []
		
		if currentMeasureTime+length <= measureLength:
			return self.effectivelyTieLongNotes(currentMeasureTime, length)
		else:
			lengthleft = length
			firstnotelength = measureLength - currentMeasureTime
			results.extend(self.effectivelyTieLongNotes(currentMeasureTime, firstnotelength))
			lengthleft-=firstnotelength
			currentMeasureTime += firstnotelength
			
			#add any full measures
			while lengthleft >= measureLength:
				results.extend(self.effectivelyTieLongNotes(currentMeasureTime, measureLength))
				lengthleft-=measureLength
				currentMeasureTime += measureLength
			
			#add the rest
			if lengthleft != 0:
				results.extend(self.effectivelyTieLongNotes(currentMeasureTime, lengthleft))
				lengthleft-=lengthleft
				currentMeasureTime += lengthleft
			
			return results


def isDivisible(a,b): return a%b == 0


#here's a way to quantize:
#make a big list of acceptible times.
#quantize all starts/stops to that.
#~ [0,0.2343,0.234,0.3456,0.3477,0.3432,0.3454]

def findclosest(lst, n): #returns index of closest match.
	leftpoint = bisect.bisect_left(lst, n)
	
	if leftpoint==0: return 0
	if leftpoint>=len(lst): return len(lst)-1
	difRight = lst[leftpoint] - n
	difLeft = n - lst[leftpoint-1]
	if difRight < difLeft:
		return leftpoint 
	else:
		return leftpoint - 1
	

class TrebleInstrument(Instrument):
	name = ''
	range = (Note('C', 0), Note('C', 10))
	clef = 'Treble'
	def __init__(self, name):
		self.name = name
		Instrument.__init__(self)

class BassInstrument(Instrument):
	name = ''
	range = (Note('C', 0), Note('C', 10))
	clef = 'Bass'
	def __init__(self, name):
		self.name = name
		Instrument.__init__(self)


if __name__=='__main__':
	def testFindClosest():
		testlist = [0.0, 1.0, 1.5, 1.7, 2.0]
		assert findclosest(testlist, 1.61)==3
		assert findclosest(testlist, -0.2)==0
		assert findclosest(testlist, 0.0)==0
		assert findclosest(testlist, 0.1)==0
		assert findclosest(testlist, 0.9)==1
		assert findclosest(testlist, 0.6)==1
		assert findclosest(testlist, 0.4999)==0
		assert findclosest(testlist, 1.1)==1
		assert findclosest(testlist, 1.001)==1
		assert findclosest(testlist, 1.24999)==1
		assert findclosest(testlist, 1.24)==1
		assert findclosest(testlist, 1.2501)==2
		assert findclosest(testlist, 1.5)==2
		assert findclosest(testlist, 1.51)==2
		assert findclosest(testlist, 1.59)==2
		assert findclosest(testlist, 1.601)==3
		assert findclosest(testlist, 99)==4
		assert findclosest(testlist, 2.1)==4
	
	def dataTest(listEvents):
		nQuantize = 8; timesig=(4,4); bTreble=True; bSharps=True
		import notesrealtimerecorded
		raw = notesrealtimerecorded.NotesRealtimeRecordedRaw()
		raw.listRecorded = listEvents
		raw.startTime = 3.6317464929201895e-06
		out = raw.getProcessedResults()
		
		listQuantized = createQuantizedList(out, timesig, nQuantize)
		intermed = createIntermediateList(listQuantized, timesig, nQuantize, bTreble, bSharps)
		docMingus = createMingusComposition(intermed, timesig, nQuantize, bTreble, bSharps)
		
		from mingus.extra import MusicXML, LilyPond
		s=LilyPond.from_Composition(docMingus)
		f=open('out.ly','w')
		f.write(s)
		f.close()
	
	testFirst = [ (-1, 1.3594319948485072, None), (60, 1.379929292689434, 2.0158894496367554), (-1, 2.0164087893852431, None), (-1, 2.5513834858899664, None), (62, 2.0160755068032388, 2.5517243113300712), (-1, 3.169228770695717, None), (64, 2.551906178019832, 3.1695567453405391), (65, 3.1697419644116782, 3.6429870276808924), (-1, 3.7864303982768761, None), (65, 3.7661233480791552, 4.055627181666944), (64, 4.0558101658171637, 4.363614649347892), (-1, 4.3638982049394546, None), (-1, 4.9788758068413719, None), (62, 4.3637157795194641, 4.9793247465809198), (64, 4.9795158323194704, 5.2669185608785476), (62, 5.2671219386821511, 5.554015105271759), (-1, 5.5548263815652552, None), (60, 5.5542020005335875, 6.0478613902046208), (-1, 6.1506713842122389, None), (60, 6.1714058630356652, 6.4583012645461926), (-1, 6.7864605697092788, None), (62, 6.4584901153638246, 6.7868988935744623), (64, 6.7870924935990464, 7.095053421594085), (-1, 7.3821667278941874, None), (65, 7.0952593136837221, 7.3826081247756346), (-1, 7.998778615717919, None), (64, 7.3828042390862523, 8.0198569929977133), (-1, 8.6353701378247791, None), (62, 8.0200617676268919, 8.6763147271510768), (-1, 9.3115337284487278, None), (-1, 9.9065979055997335, None), (-1, 10.480808035658164, None), (60, 8.6964918725703964, 10.954150038622227), (-1, 11.036485922093451, None), (-1, 11.651723333552169, None)]
	testTied = [(-1, 1.3984277331336803, None), (-1, 1.7275737558823816, None), (-1, 2.0561465214154313, None), (-1, 2.551346050964578, None), (-1, 2.9419032307178705, None), (-1, 3.3313323341374392, None), (-1, 3.7209793423465833, None), (-1, 4.1311197372850463, None), (-1, 4.5210530947369012, None), (-1, 4.9106858553251884, None), (-1, 5.3002845333694646, None), (-1, 5.6909766464732252, None), (60, 2.0787918576243629, 5.7119340586582927), (-1, 6.1489574792326955, None), (67, 5.7539287814512736, 6.1496033713782055), (-1, 6.5418861894458651, None), (64, 6.1497444507612, 6.5433112308966646), (62, 6.5434928182213099, 6.9139206239899202), (-1, 6.9352227727267017, None), (65, 6.9141262367144432, 7.1198009802921876), (64, 7.1200077104771697, 7.3043076703882752), (-1, 7.3049393149129287, None), (62, 7.3045135624779123, 7.5315250960666793), (-1, 7.7164131195445229, None), (-1, 8.1890737509934919, None), (60, 7.7170338688296978, 8.3544676767577997)]
	dataTest(testTied)
	
	intermed=IntermediateList((4,4))
	bd = intermed.baseDivisions
	def testTieNotes():
		r = intermed.effectivelyTieLongNotes( 0, bd*3); print [ n/float(bd) for n in r]
		r = intermed.effectivelyTieLongNotes( bd, bd*3); print [ n/float(bd) for n in r]
		r = intermed.effectivelyTieLongNotes( 0 , bd*2); print [ n/float(bd) for n in r]
		r = intermed.effectivelyTieLongNotes( 0 , bd*3.5); print [ n/float(bd) for n in r]
		r = intermed.effectivelyTieLongNotes( 0 , bd*2.25); print [ n/float(bd) for n in r]
		r = intermed.effectivelyTieLongNotes( bd , bd*5); print [ n/float(bd) for n in r]
		
		
		r = intermed.effectivelyTieLongNotesBarlines( 0 , bd*4, bd*4); print [ n/float(bd) for n in r]
		r = intermed.effectivelyTieLongNotesBarlines( 0 , bd*8, bd*4); print [ n/float(bd) for n in r]
		r = intermed.effectivelyTieLongNotesBarlines( 0 , bd*12, bd*4); print [ n/float(bd) for n in r]
		r = intermed.effectivelyTieLongNotesBarlines( 0 , bd*10, bd*4); print [ n/float(bd) for n in r]
		r = intermed.effectivelyTieLongNotesBarlines( bd , bd*5, bd*4); print [ n/float(bd) for n in r]
		r = intermed.effectivelyTieLongNotesBarlines( bd , bd*8, bd*4); print [ n/float(bd) for n in r]
		r = intermed.effectivelyTieLongNotesBarlines( bd*3 , bd*2, bd*4); print [ n/float(bd) for n in r]
	

		


