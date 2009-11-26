"""
musicpad
Ben Fisher, 2007
License: GPL

Used to use real-time midi output (notesrealtimemidi.py), but there was significant delay.
Now uses precomputed .wav files.
"""
from Tkinter import *
import tkFileDialog

import music_util
import dlg1
import dlg2
import dlg3
import dlgresults
import notesrealtimemidi
import notesrealtimewav
import tkutil

#rev.190 is somewhat stable.

def pack(o, **kwargs): o.pack(**kwargs); return o
class Gui1():
	objNotesRealtime = None
	isRecording = False
	
	def __init__(self, root):
		root.title('Trilling Recorder')
		
		frameMain = pack( Frame(root), side=TOP, fill=BOTH, expand=True)
		pack( Label(frameMain, text='(This is a preview of a work-in-progress by Ben Fisher).'), side=TOP)
		pack( Label(frameMain, text='Try typing on your keyboard as if it were a piano:'), side=TOP)
		
		figure = PhotoImage(file='figure.gif')
		label = Label(frameMain, image=figure)
		label.image = figure # keep a reference
		label.pack(side=TOP)
		
		self.lblTransposition = pack( Label(frameMain, text='Transposition: +0   (lowest note is C4)'), side=TOP, anchor='w')
		pack( Label(frameMain, text='(Use up/down arrows to transpose)'), side=TOP, anchor='w')
		
		frame2 = pack( Frame(frameMain), side=TOP, fill=BOTH, expand=True)
		self.btnRecord =  pack( Button(frame2, text='Record and notate...', command=self.startRecordProcess), side=LEFT, pady=25, padx=5)
		self.btnStop =  pack( Button(frame2, text='Stop', command=self.stopRecord), side=LEFT, pady=25, padx=3)
		self.btnStop.config(state=DISABLED)
		pack( Label(frame2, text='\t Notate what is played, and export to Finale or Sibelius.'), side=LEFT)
		self.lblEvents = pack( Label(root, text=''),side=TOP)
	
		manualbindings = {
			('','Up'): tkutil.Callable(self.changeTransposition,12),
			('','Down'): tkutil.Callable(self.changeTransposition,-12),
			('Control+','Up'): tkutil.Callable(self.changeTransposition,1),
			('Control+','Down'): tkutil.Callable(self.changeTransposition,-1),
			('Alt+','F4'):self.onClose }
			
		if True:
			self.objNotesRealtime = notesrealtimewav.NotesRealtimeWav(manualbindings)
		else:
			self.objNotesRealtime = notesrealtimemidi.NotesRealtimeMidi(manualbindings)
		self.objNotesRealtime.addBindings(root)
		self.objNotesRealtime.addNotebindings('notes.cfg')
		root.protocol("WM_DELETE_WINDOW", self.onClose)
		root.bind('<FocusOut>',self.onlosefocus)
		self.root=root
	
	def changeTransposition(self, amount):
		newtrans = self.objNotesRealtime.setTranspose(amount)
		
		s='Transposition: %d   (lowest note is %s)'%(newtrans-60, ''.join(map(str,music_util.noteToName(newtrans))))
		self.lblTransposition.config(text=s)
		
	def startRecordProcess(self):
		res2 = dlg2.Dlg2(self.root)
		if res2.result==None: return
		nQuantize = res2.result
		
		#begin recording
		self._settings = (nQuantize,)
		self.btnStop.config(state=NORMAL)
		self.btnRecord.config(state=DISABLED)
		self.objNotesRealtime.setRecordingMode(True)
		self.isRecording=True
	
	def stopRecord(self):
		self.isRecording=False
		self.btnStop.config(state=DISABLED)
		self.btnRecord.config(state=NORMAL)
		res = self.objNotesRealtime.setRecordingMode(False)
		nQuantize = self._settings[0]
		
		window = dlgresults.DlgResultsWindow()
		result = window.loadData(res, nQuantize)
		if not result: return
		
		#open results window
		top = Toplevel()
		window.openWindow(top)
		top.focus_set()
		
		
	def onlosefocus(self,e=None):
		self.objNotesRealtime.stopallnotes()
		if self.isRecording:
			tkutil.alert('Recording was stopped, because you switched to another program.')
			self.stopRecord()
		
	def onClose(self):
		try:
			self.objNotesRealtime.closeDevice()
		finally:
			self.root.destroy()
			#if not in a finally, it's possible error thrown and app can't close



root = Tk()
app = Gui1(root)
root.mainloop()

