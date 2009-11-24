

import testdepiction
import exceptions
import bisect

import music_util
import trclasses

from mingus.containers import Composition,Track, Instrument, Note, Bar



def notesinterpret(listResults, timesig, quantize):
	print listResults
	startTime = listResults.pop(0)
	assert startTime[0]==None
	startTime = startTime[1]
	
	
	#shift times so that 0 is start time.
	listResults = [(m[0],float(m[1]-startTime),None if m[2]==None else float(m[2]-startTime)) for m in listResults]
	
	#sort results by *start* time
	listResults.sort(key=lambda a: a[1])
	if listResults[-1][0]!=-1: raise NotesinterpretException('must end with tab pulse!')
	if listResults[0][0]!=-1: raise NotesinterpretException('must start with tab pulse!')
	
	#separate tabs
	listNotes = [m for m in listResults if m[0] != -1]
	listPulses = [m[1] for m in listResults if m[0] == -1]
	if not listPulses: raise NotesinterpretException('no tabs!')
	
	
	# how many subdivisions?
	divisions = quantize
	divisions /= 4 #now, since tapping every qtr note
	
	#add a final pulse. Disabled: better just to enforce final event is a tab-pulse
	#~ if len(listPulses)==1: listPulses.append(listPulses[0] + (listPulses[0]-0.0))
	#~ listPulses.append(listPulses[-1] + (listPulses[-1]-listPulses))
	
	#create quantization list (contains all acceptible times).
	#divides linearly. one could also smooth inter-beat timing
	listQuantize=[]
	listQuantize.append(0.0)
	prevTime = 0.0
	for pulseTime in listPulses:
		inc = (pulseTime-prevTime)/divisions
		for i in range(divisions):
			listQuantize.append(prevTime + i*inc)
		prevTime = pulseTime
	
	
	TMPlistNotesUnquantized = listNotes
	#quantize all of the times
	listNotes = [(m[0],findclosest(listQuantize,m[1]),findclosest(listQuantize,m[2])) for m in listNotes]
	#now times are integers, corresponding to multiples of time unit. So if quantize=8, each unit is an 8th note
	
	TMPlistnotes = list(listNotes)
	
	
		
	#now get rid of overlapping notes, using some heuristics.
	listFinal = []
	while listNotes:
		currentPitchGroup = [listNotes.pop(0)]
		currentPitchStartTime = currentPitchGroup[0][1]
		while listNotes and listNotes[0][1]==currentPitchStartTime:
			currentPitchGroup.append(listNotes.pop(0))
		
		#length of this pitch group is the longest in the group...
		currentPitchEndTime = 0
		for note in currentPitchGroup: 
			if note[2]>currentPitchEndTime: currentPitchEndTime=note[2]
		
		#...unless it is interrupted by some other note.
		if listNotes and listNotes[0][1] < currentPitchEndTime:
			currentPitchEndTime = listNotes[0][1]
		
		#can't have a note of length 0.
		if currentPitchEndTime==currentPitchStartTime:
			currentPitchEndTime += 1
		
		#quantize currentPitchEndTime to have it fill to next note?
		if listNotes and currentPitchEndTime != listNotes[0][1]:
			if listNotes[0][1] - currentPitchEndTime < quantize:
				currentPitchEndTime = listNotes[0][1] #bleed into next note.
		assert currentPitchEndTime>currentPitchStartTime
		
		listFinal.append(( [m[0] for m in currentPitchGroup], currentPitchStartTime, currentPitchEndTime))
	
	
	testdepiction.draw(listQuantize, listPulses,TMPlistNotesUnquantized, TMPlistnotes, listFinal)
	return listFinal
	


def convTrClassToMingus(trdoc, timesig, quantize, bIsTreble, bSharps):
	
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
		trclasses.baseDivisions*4: 1.0,
		trclasses.baseDivisions*2: 2.0,
		trclasses.baseDivisions*1: 4.0,
		trclasses.baseDivisions*0.5: 8.0,
		trclasses.baseDivisions*0.25: 16.0,
		trclasses.baseDivisions*0.125: 32.0,
		trclasses.baseDivisions*0.0625: 64.0,
			}
	
	#assume no meter changes
	trpart = trdoc.trscore.trparts[0]
	for trmeasure in trpart.trmeasures:
		for ng in trmeasure.trlayers[0].trnotegroups:
			if ng.pitches==0 or ng.pitches==(0,):
				thepitches = tuple()
			else:
				thepitches = []
				for pitch in ng.pitches:
					pname, poctave = music_util.noteToName(pitch,bSharps)
					thepitches.append(pname+'-'+str(poctave))
			
			dur = ng.endTime-ng.startTime
			if dur not in mapDurs: raise NotesinterpretException('Unknown duration')
			bFit = track.add_notes(thepitches, mapDurs[dur])
			assert bFit
		
			#note that, naturally, will enforce having correct measure lines, since going across barline throughs exception
	
	return comp
	
	
	

def convToClassTrClasses(listFinal, timesig, quantize, bIsTreble, bSharps):
	doc = trclasses.TrDocument()
	doc.trscore = trclasses.TrScore()
	part = trclasses.TrPart()
	firstmeasure = part.addMeasure()
	firstmeasure.timesigchange = timesig
	firstmeasure.keysigchange = 'sharps' if bSharps else 'flats'
	doc.trscore.trparts.append(part)
	part.clef = 'treble' if bIsTreble else 'bass'
	
	fQtrnotespermeasure = float(timesig[0]) / (timesig[1]/4)
	nTimestepspermeasure = int( fQtrnotespermeasure * trclasses.baseDivisions)
	
	#insert rests where necessary
	newlistFinal = []
	for i in range(len(listFinal)):
		if listFinal[i][1]==listFinal[i][2]: raise 'Cannot have note of length 0.'
		newlistFinal.append(listFinal[i])
		if i<len(listFinal)-1:
			restLength = listFinal[i+1][1]-listFinal[i][2]
			assert restLength >= 0
			if restLength != 0:
				newlistFinal.append(((0,), listFinal[i][2], listFinal[i+1][1]))
	
	
	def qStepToTimeBase(x):
		return int((float(x) / (quantize/4)) * trclasses.baseDivisions)
	
	
	#insert notes
	currentMeasureTime = 0
	for note in newlistFinal:
		length = qStepToTimeBase(note[2]-note[1])
		results, _ = effectivelyTieLongNotesBarlines(currentMeasureTime, length, nTimestepspermeasure)
		for i in range(len(results)):
			result = results[i]
			bIsTied = i!=len(results)-1
			part.addNote(note[0],currentMeasureTime,currentMeasureTime+result, tied=bIsTied) #addNote(pitch, length)
			currentMeasureTime += result
			#~ print currentMeasureTime, nTimestepspermeasure, 
			assert currentMeasureTime<= nTimestepspermeasure
			if currentMeasureTime == nTimestepspermeasure:
				part.addMeasure()
				currentMeasureTime=0 #?????
				
	part.lastTime = len(part.trmeasures) * nTimestepspermeasure #total time.
	return doc
	


def isDivisible(a,b): return a%b == 0


atomicnotes = [int(trclasses.baseDivisions/n) for n in [ 0.25, 0.5, 1, 2, 4, 8, 16, 32]]
#									whole, half, qtr, 8th, 16, 32, 64, 138

def effectivelyTieLongNotes(currentMeasureTime, length): 
	#spell a long note on the beat. probably only looks well for duple times!
	results = []
	lengthleft = length
	
	#if longer than  whole notes, add whole notes (unnecessary?)
	#~ while lengthleft>atomicnotes[0]: 
		#~ print 'whole'; results.append(atomicnotes[0]); 
		#~ lengthleft-=atomicnotes[0]; 
		#~ currentMeasureTime+=atomicnotes[0]
	
	while lengthleft>0:
		#~ print 'currentMeasureTime %d,lengthleft %d' % (currentMeasureTime,lengthleft)
		for atom in atomicnotes:
			#~ print 'atom %d lengthleft %d' % (atom, lengthleft)
			if atom  <= lengthleft and isDivisible(currentMeasureTime, atom):
				results.append(atom)
				lengthleft -= atom
				currentMeasureTime += atom
				break
		
				
	assert lengthleft == 0
	return results, currentMeasureTime
	
def effectivelyTieLongNotesBarlines(currentMeasureTime, length, measureLength): 
	results = []
	#spell a long note, taking measures into account
	
	if currentMeasureTime+length <= measureLength:
		return effectivelyTieLongNotes(currentMeasureTime, length)
	else:
		lengthleft = length
		firstnotelength = measureLength - currentMeasureTime
		results.extend(effectivelyTieLongNotes(currentMeasureTime, firstnotelength)[0])
		lengthleft-=firstnotelength
		currentMeasureTime += firstnotelength
		
		#add any full measures
		while lengthleft >= measureLength:
			results.extend(effectivelyTieLongNotes(currentMeasureTime, measureLength)[0])
			lengthleft-=measureLength
			currentMeasureTime += measureLength
		
		#add the rest
		if lengthleft != 0:
			results.extend(effectivelyTieLongNotes(currentMeasureTime, lengthleft)[0])
			lengthleft-=lengthleft
			currentMeasureTime += lengthleft
		
		return results, currentMeasureTime
	
	

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
	
class NotesinterpretException(exceptions.Exception): pass
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
	
	def dataTest():
		nQuantize = 8; timesig=(4,4); bTreble=True; bSharps=True
		test2 = [(None, 3.6317464929201895e-06, None), (-1, 1.3594319948485072, None), (60, 1.379929292689434, 2.0158894496367554), (-1, 2.0164087893852431, None), (-1, 2.5513834858899664, None), (62, 2.0160755068032388, 2.5517243113300712), (-1, 3.169228770695717, None), (64, 2.551906178019832, 3.1695567453405391), (65, 3.1697419644116782, 3.6429870276808924), (-1, 3.7864303982768761, None), (65, 3.7661233480791552, 4.055627181666944), (64, 4.0558101658171637, 4.363614649347892), (-1, 4.3638982049394546, None), (-1, 4.9788758068413719, None), (62, 4.3637157795194641, 4.9793247465809198), (64, 4.9795158323194704, 5.2669185608785476), (62, 5.2671219386821511, 5.554015105271759), (-1, 5.5548263815652552, None), (60, 5.5542020005335875, 6.0478613902046208), (-1, 6.1506713842122389, None), (60, 6.1714058630356652, 6.4583012645461926), (-1, 6.7864605697092788, None), (62, 6.4584901153638246, 6.7868988935744623), (64, 6.7870924935990464, 7.095053421594085), (-1, 7.3821667278941874, None), (65, 7.0952593136837221, 7.3826081247756346), (-1, 7.998778615717919, None), (64, 7.3828042390862523, 8.0198569929977133), (-1, 8.6353701378247791, None), (62, 8.0200617676268919, 8.6763147271510768), (-1, 9.3115337284487278, None), (-1, 9.9065979055997335, None), (-1, 10.480808035658164, None), (60, 8.6964918725703964, 10.954150038622227), (-1, 11.036485922093451, None), (-1, 11.651723333552169, None)]
		res = notesinterpret(test2, timesig, nQuantize)
		trdoc = convToClassTrClasses(res, timesig, nQuantize, bTreble, bSharps)
		docMingus = convTrClassToMingus(trdoc, timesig, nQuantize, bTreble, bSharps)
		
		from mingus.extra import MusicXML, LilyPond
		s=LilyPond.from_Composition(docMingus)
		f=open('out.ly','w')
		f.write(s)
		f.close()
		
	#~ test = [(None,1.1,None), (-1, 1.124412460242852, None), (-1, 1.8425467482599045, None), (-1, 2.5810925690276276, None), (60, 2.6017901716558947, 2.7456453264311524), (62, 3.0538752830317821, 3.1974002282412988), (-1, 3.4844182710372409, None), (-1, 4.368222776917178, None), (60, 3.5050532958797835, 4.6554629149794176), (-1, 5.0242634189540851, None), (60, 5.0448355866457888, 5.1680998562666485), (62, 5.434484829775851, 5.5581024962669838), (-1, 5.8868118713411901, None), (-1, 6.74913319988993, None), (60, 5.907586578741153, 6.9748132793413689), (-1, 7.446408691607453, None), (60, 7.467083106931188, 7.6935753007714665), (62, 7.6940454722597424, 7.8586214169677993), (60, 7.8591720456091485, 8.0870275412098458), (62, 8.044941186659198, 8.2101839758963777), (-1, 8.2930942340437124, None), (-1, 8.9901697003390098, None), (-1, 9.7285794702958057, None), (60, 8.2315802706768597, 9.8719317678643517)]
	#~ notesinterpret(test, (4,4), 4)
	#~ notesinterpret(test, (4,4), 16)
	
	
	bd = trclasses.baseDivisions
	def testTieNotes():
		r,_ = effectivelyTieLongNotes( 0, bd*3); print [ n/float(bd) for n in r]
		r,_ = effectivelyTieLongNotes( bd, bd*3); print [ n/float(bd) for n in r]
		r,_ = effectivelyTieLongNotes( 0 , bd*2); print [ n/float(bd) for n in r]
		r,_ = effectivelyTieLongNotes( 0 , bd*3.5); print [ n/float(bd) for n in r]
		r,_ = effectivelyTieLongNotes( 0 , bd*2.25); print [ n/float(bd) for n in r]
		r,_ = effectivelyTieLongNotes( bd , bd*5); print [ n/float(bd) for n in r]
		
		
		r,_ = effectivelyTieLongNotesBarlines( 0 , bd*4, bd*4); print [ n/float(bd) for n in r]
		r,_ = effectivelyTieLongNotesBarlines( 0 , bd*8, bd*4); print [ n/float(bd) for n in r]
		r,_ = effectivelyTieLongNotesBarlines( 0 , bd*12, bd*4); print [ n/float(bd) for n in r]
		r,_ = effectivelyTieLongNotesBarlines( 0 , bd*10, bd*4); print [ n/float(bd) for n in r]
		r,_ = effectivelyTieLongNotesBarlines( bd , bd*5, bd*4); print [ n/float(bd) for n in r]
		r,_ = effectivelyTieLongNotesBarlines( bd , bd*8, bd*4); print [ n/float(bd) for n in r]
		r,_ = effectivelyTieLongNotesBarlines( bd*3 , bd*2, bd*4); print [ n/float(bd) for n in r]
	
	print atomicnotes

	dataTest()	
		


