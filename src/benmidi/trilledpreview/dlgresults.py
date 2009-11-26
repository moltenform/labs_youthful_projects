import notesinterpret
import tkutil

from scoreview import scoreview_intermed
import os
clefsfilepath = 'scoreview'+os.sep+'clefs'

from mingus.extra import MusicXML, LilyPond
from Tkinter import *

def pack(o, **kwargs): o.pack(**kwargs); return o
class DlgResultsWindow():
	def __init__(self):
		self.nEvents = 0
	
	def loadData(self,res, nQuantize): 
		self.nQuantize = nQuantize
		try:
			self.listQuantized = notesinterpret.createQuantizedList(res, nQuantize)
		except notesinterpret.NotesinterpretException, e:
			tkutil.alert('Exception:'+str(e), title='Trilling Recorder')
			return False
		
		self.nEvents = len(self.listQuantized)
		return True
	
	def _createIntermed(self):
		bSharps, bTreble, timesig = self._getSettings()
		return notesinterpret.createIntermediateList(self.listQuantized, timesig, self.nQuantize, bTreble, bSharps)
	
	def _createMingus(self):
		bSharps, bTreble, timesig = self._getSettings()
		intermed = self._createIntermed()
		return notesinterpret.createMingusComposition(intermed, timesig, bTreble, bSharps)
		
	def openWindow(self, top):
		top.title('Trilled Results')
		frtop = Frame(top)
		frtop.pack(expand=YES, fill=BOTH)
		
		s='%d note events were recorded.'%self.nEvents
		pack(Label(frtop, text=s), side=TOP)
		frame2 = pack( Frame(frtop), side=TOP, fill=BOTH, expand=True)
		pack( Button(frame2, text='Save results as MusicXml', command=self.saveXml), side=LEFT, pady=25, padx=5)
		pack( Button(frame2, text='Save results as LilyPond', command=self.saveLily), side=LEFT, pady=25, padx=5)
		pack( Button(frame2, text='Preview Results', command=self.openPreview,default=ACTIVE), side=LEFT, pady=25, padx=5)
		
		frOptions = Frame(top)
		frOptions.pack()
		fr1= Frame(frOptions)
		fr1.grid(row=0, column=0)
		pack(Label(fr1,text='key:'), side=LEFT)
		self.optionskeysig= ['sharps','flats']
		self.varkeysig = StringVar()
		self.varkeysig.set(self.optionskeysig[0])
		pack(OptionMenu(fr1, self.varkeysig, *self.optionskeysig), side=LEFT)
		
		fr2= Frame(frOptions)
		fr2.grid(row=1, column=0)
		pack(Label(fr2,text='clef:'), side=LEFT)
		self.optionsclef= ['treble','bass']
		self.varclef = StringVar()
		self.varclef.set(self.optionsclef[0])
		pack(OptionMenu(fr2, self.varclef, *self.optionsclef), side=LEFT)
		
		fr3= Frame(frOptions)
		fr3.grid(row=2, column=0)
		pack(Label(fr3,text='time signature:'), side=LEFT)
		self.optionstime= ['4/4','2/4','3/4']
		self.vartime = StringVar()
		self.vartime.set(self.optionstime[0])
		pack(OptionMenu(fr3, self.vartime, *self.optionstime), side=LEFT)
	
	def saveLily(self):
		docMingus = self._createMingus()
		filename = tkutil.ask_savefile(title = 'Save Lilypond',types=['.ly|LilyPond'])
		if not filename: return
		
		s=LilyPond.from_Composition(docMingus)
		f=open(filename,'w')
		f.write(s)
		f.close()
		
	def saveXml(self):
		docMingus = self._createMingus()
		filename = tkutil.ask_savefile(title = 'Save MusicXml',types=['.xml|MusicXml'])
		if not filename: return
		
		s=MusicXML.from_Composition(docMingus)
		f=open(filename,'w')
		f.write(s)
		f.close()
		
	def openPreview(self):
		intermed = self._createIntermed()
		bSharps, bTreble, timesig = self._getSettings()
		top=Toplevel()
		top.title('Trilled Results - Rough Preview')
		opts={}
		frScoreViewWithBtns = scoreview_intermed.ScoreViewWindow(top, intermed, bTreble, clefsfilepath, opts)
		top.focus_set()
	
	def _getSettings(self):
		nchosen = self.optionskeysig.index(self.varkeysig.get())
		if nchosen == 0:
			bSharps = True
		else:
			bSharps = False
		nclef = self.optionsclef.index(self.varclef.get())
		if nclef == 0:
			bTreble = True
		else:
			bTreble = False
		
		num,den = self.vartime.get().split('/')
		num=int(num); den=int(den)
		return (bSharps, bTreble, (num,den))

def testWriteToLilypond(docMingus):
	s=LilyPond.from_Composition(docMingus)
	f=open('out.ly','w')
	f.write(s)
	f.close()
	
if __name__=='__main__':
	import Tkinter
	from notesrealtimerecorded import  NotesRealtimeNoteEvent
	import notesinterpret
	
	
	def test1():
		intermed = notesinterpret.IntermediateList((4,4))
		#tests notes, sharps, different durations
		sampleList = [1,2,2,4,8,4,4,8   ,64,64,64,64,32,32,16,16,2,4,  8,8,8,8,4,4, 4,4,8,8,8,8]
		noteList=[]
		curtime = 0
		for n in sampleList:
			noteList.append(NotesRealtimeNoteEvent([60+curtime%7], curtime, curtime+4*intermed.baseDivisions/n))
			noteList[-1].isTied = False
			curtime+=4*intermed.baseDivisions/n
		intermed.noteList = noteList
		intermed.bSharps = True
		return intermed
	
	def test2():
		intermed = notesinterpret.IntermediateList((4,4))
		#tests ties, rests, notes
		rlist = [64,64,64,64,32,32,16,16,8,8,4,4,2,2,1,1]
		noteList=[]
		curtime = 0
		for i in range(len(rlist)):
			n=rlist[i]
			pitch=60+i%3
			if i%2==1: pitch=0 #a rest
			noteList.append(NotesRealtimeNoteEvent([pitch], curtime, curtime+4*intermed.baseDivisions/n))
			noteList[-1].isTied = False
			curtime+=4*intermed.baseDivisions/n
		for i in range(len(rlist)):
			n=rlist[i]
			noteList.append(NotesRealtimeNoteEvent([70], curtime, curtime+4*intermed.baseDivisions/n))
			noteList[-1].isTied = True
			curtime+=4*intermed.baseDivisions/n
		intermed.noteList = noteList
		intermed.bSharps = True
		return intermed
	
	
	nQuantize = 16
	top=Tkinter.Tk()
	app = DlgResultsWindow()
	
	#inject this testing method.
	app._createIntermed = test1
	app.openWindow(top)
	
	top.mainloop()
	
	
	