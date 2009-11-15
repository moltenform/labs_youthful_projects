
#every qtr note can be divided into this. 
#~ baseDivisions = 40320 #128*9*7*5	the 128 is unnecessary...
#~ baseDivisions = 128 #debug!
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
		self.trmeasures = []
	def addMeasure(self):
		self.trmeasures.append(TrMeasure())
		return self.trmeasures[-1]
	def addNote(self, pitchSet,layer=0, **opts):
		note=TrNoteGroup(pitchSet, **opts)
		self.trmeasures[-1].trlayers[layer].append(note)
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
	


