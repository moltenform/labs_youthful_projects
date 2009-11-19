
import testdepiction
timebase = 7680
#each qtr note divided into this many steps.  (512*3*5)

import exceptions
import bisect

import music_util
import trclasses

class NotesinterpretException(exceptions.Exception): pass


def notesinterpret(listResults, timesig, quantize):
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
	divisions /= 4 
	
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
	

def convToClass(listFinal, timesig, quantize, bIsTreble, bSharps):
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
			assert currentMeasureTime<= nTimestepspermeasure
			if currentMeasureTime == nTimestepspermeasure:
				part.addMeasure()
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
	

if __name__=='__main__':
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
	
	#~ test = [(None,1.1,None), (-1, 1.124412460242852, None), (-1, 1.8425467482599045, None), (-1, 2.5810925690276276, None), (60, 2.6017901716558947, 2.7456453264311524), (62, 3.0538752830317821, 3.1974002282412988), (-1, 3.4844182710372409, None), (-1, 4.368222776917178, None), (60, 3.5050532958797835, 4.6554629149794176), (-1, 5.0242634189540851, None), (60, 5.0448355866457888, 5.1680998562666485), (62, 5.434484829775851, 5.5581024962669838), (-1, 5.8868118713411901, None), (-1, 6.74913319988993, None), (60, 5.907586578741153, 6.9748132793413689), (-1, 7.446408691607453, None), (60, 7.467083106931188, 7.6935753007714665), (62, 7.6940454722597424, 7.8586214169677993), (60, 7.8591720456091485, 8.0870275412098458), (62, 8.044941186659198, 8.2101839758963777), (-1, 8.2930942340437124, None), (-1, 8.9901697003390098, None), (-1, 9.7285794702958057, None), (60, 8.2315802706768597, 9.8719317678643517)]
	#~ notesinterpret(test, (4,4), 4)
	#~ notesinterpret(test, (4,4), 16)
	
	
	bd = trclasses.baseDivisions
	
	assert isDivisible(0, bd*4)
	assert isDivisible(0, bd/4)
	assert isDivisible(bd, bd/2)
	assert isDivisible(bd, bd)
	assert not isDivisible(bd, bd*2)
	assert not isDivisible(bd*3, bd*2)
	assert isDivisible(bd*3, bd)
	
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

		
		


