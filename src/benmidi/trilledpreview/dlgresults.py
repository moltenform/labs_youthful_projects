import notesinterpret
import tkutil

from scoreview import scoreview_intermed
import os
clefsfilepath = 'scoreview'+os.sep+'clefs'

from mingus.extra import MusicXML, LilyPond
from Tkinter import *

def pack(o, **kwargs): o.pack(**kwargs); return o
class DlgResultsWindow():
	def __init__(self, top, intermed, docMingus, bTreble):
		top.title('Trilled Results')
		frtop = Frame(top)
		frtop.pack(expand=YES, fill=BOTH)
		
		self.intermed = intermed
		self.docMingus = docMingus
		self.bTreble = bTreble
		
		s='%d note events were recorded.'%len(self.intermed.noteList)
		pack(Label(frtop, text=s), side=TOP)
		frame2 = pack( Frame(frtop), side=TOP, fill=BOTH, expand=True)
		pack( Button(frame2, text='Save results as MusicXml', command=self.saveXml), side=LEFT, pady=25, padx=5)
		pack( Button(frame2, text='Save results as LilyPond', command=self.saveLily), side=LEFT, pady=25, padx=5)
		pack( Button(frame2, text='Preview Results', command=self.openPreview), side=LEFT, pady=25, padx=5)
	
	def saveLily(self):
		filename = tkutil.ask_savefile(title = 'Save Lilypond',types=['.ly|LilyPond'])
		if not filename: return
		
		s=LilyPond.from_Composition(self.docMingus)
		f=open(filename,'w')
		f.write(s)
		f.close()
		
	def saveXml(self):
		filename = tkutil.ask_savefile(title = 'Save MusicXml',types=['.xml|MusicXml'])
		if not filename: return
		
		s=MusicXML.from_Composition(self.docMingus)
		f=open(filename,'w')
		f.write(s)
		f.close()
		
	def openPreview(self):
		top=Toplevel()
		top.title('Trilled Results - Rough Preview')
		opts={}
		frScoreViewWithBtns = scoreview_intermed.ScoreViewWindow(top, intermed,self.bTreble, clefsfilepath, opts)


def testWriteToLilypond(docMingus):
	s=LilyPond.from_Composition(docMingus)
	f=open('out.ly','w')
	f.write(s)
	f.close()
	
if __name__=='__main__':
	import Tkinter
	from notesrealtimerecorded import  NotesRealtimeNoteEvent
	import notesinterpret
	intermed = notesinterpret.IntermediateList((4,4))
	
	def test1():
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
	
	def test2():
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
	
	test1()
	bTreble=True
	docMingus=None
	
	top=Tkinter.Tk()
	app = DlgResultsWindow(top,intermed,docMingus,bTreble)
	top.mainloop()
	
	
	