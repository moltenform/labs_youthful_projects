
# On Windows, the best timer is time.clock()
# On most other platforms the best timer is time.time()
import sys
import time
if sys.platform == "win32": fntimer = time.clock
else: fntimer = time.time
import exceptions

class NotesinterpretException(exceptions.Exception): pass
	
#structure to hold 

class NotesRealtimeRecordedRaw():
	#raw results from recording. simple class that is fast to insert into, since done at real time.
	#here, a note number of -1 refers to a Pulse event.
	startTime = 0
	listRecorded = None
	def __init__(self):
		self.listRecorded = []
	
	def clear(self):
		self.listRecorded = []
		
	def beginRecording(self):
		self.startTime = fntimer()
		self.listRecorded = []
		
	def recPulseEvent(self):
		self.listRecorded.append((-1, fntimer(), None))
	
	def recNoteEvent(self, noteNumber, startTime):
		self.listRecorded.append((noteNumber, startTime, fntimer()))
	
	def getProcessedResults(self):
		#returns either a NotesRealtimeRecorded or a NotesinterpretException
		
		listResults = self.listRecorded
		#~ print self.startTime
		#~ print listResults
		
		#sort results by *start* time
		listResults.sort(key=lambda a: a[1])
		if listResults[-1][0]!=-1: return NotesinterpretException('must end with tab pulse!')
		if listResults[0][0]!=-1: return NotesinterpretException('must start with tab pulse!')
		
		#add a final pulse? Disabled: better just to enforce final event is a tab-pulse
		#~ if len(listPulses)==1: listPulses.append(listPulses[0] + (listPulses[0]-0.0))
		#~ listPulses.append(listPulses[-1] + (listPulses[-1]-listPulses))
	
		
		result = NotesRealtimeRecorded()
		startTime = self.startTime
		for tp in listResults:
			pitch, start, end = tp
			if pitch==-1:
				# a tab event
				result.listPulses.append( start-startTime )
			else:
				# a note event
				result.listNotes.append(NotesRealtimeNoteEvent(pitch,start-startTime,end-startTime))
		
		if not result.listPulses: return NotesinterpretException('no tabs!')
		return result
	
class NotesRealtimeRecorded():
	listNotes = None #list of NotesRealtimeNoteEvent
	listPulses = None #list of floats, 
	def __init__(self):
		self.listNotes=[]
		self.listPulses=[]

class NotesRealtimeNoteEvent():
	pitch = 0
	start = 0
	end = 0
	def __init__(self, pitch, start,end):
		self.pitch=pitch
		self.start=start
		self.end=end
	
	