
#every qtr note can be divided into this. 
#~ baseDivisions = 40320 #128*9*7*5	the 128 is unnecessary...
#~ baseDivisions = 128 #for debugging only

#actually, since now timing relative to start of measure, this number can be large.
baseDivisions = 221760 #7*9*5*11*64


class TrBase(): pass

class TrDocument(TrBase):
	trscore = None
	
class TrScore(TrBase):
	title = ''
	author = ''
	trparts = None #[]
	
	def __init__(self):
		self.trparts = []
		
class TrPart(TrBase):
	clef = None #'treble' or 'bass'
	trmeasures = None #[]
	lastTime = 0 #saves computation later.
	
	def __init__(self):
		self.trmeasures = [TrMeasure()]
	def addMeasure(self):
		self.trmeasures.append(TrMeasure())
		return self.trmeasures[-1]
	def addNote(self, pitchSet,startTime,endTime, layer=0, **opts):
		note=TrNoteGroup(pitchSet, **opts)
		note.startTime,note.endTime=startTime,endTime
		assert layer==0 #?? debugging
		self.trmeasures[-1].trlayers[layer].trnotegroups.append(note)
		return note
		
	

class TrMeasure(TrBase):
	timesigchange = None #bar 1 must have it
	keysigchange = None #bar 1 must have it. currently 'sharps' or 'flats'
	measureOrnaments = None #[]
	trlayers = None #[]
	
	def __init__(self):
		self.trlayers = [TrLayer()]
		self.measureOrnaments= []

class TrLayer(TrBase):
	trnotegroups = None #[]
	def __init__(self):
		self.trnotegroups = []

class TrNoteGroup(TrBase):
	pitches = None #pitch 0 is a rest
	noteOrnaments = None
	startTime = 0 #relative to measure?
	endTime = 0
	tupletMark = None
	tied = False
	def __init__(self, pitchSet, tied=False):
		self.tied = tied
		self.pitches = tuple(pitchSet)
	

def simpleToFinaleCmd(d):
	import music_util
	hmatoms={8:'8',4:'7',2:'6',1:'5',0.5:'4',0.25:'3',0.125:'2',0.0625:1}
	atoms={}
	for key in hmatoms: atoms[int(key*baseDivisions)]=hmatoms[key]
	part = d.trscore.trparts[0]
	sout=''
	
	for measure in part.trmeasures:
		for notegroup in measure.trlayers[0].trnotegroups:
			duration=notegroup.endTime-notegroup.startTime
			ndur=atoms[duration]
			print notegroup.pitches
			assert len(notegroup.pitches)==1 
			if notegroup.tied: print 'warning:tied note'
			if notegroup.pitches[0]==0:nname='r'
			else: nname = ''.join(map(str,music_util.noteToName(notegroup.pitches[0])))
			sout += ndur+nname+'; '
	print sout
