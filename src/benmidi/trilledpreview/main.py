"""
musicpad
Ben Fisher, 2007
License: GPL

can't support too much polyphony, see http://www.sjbaker.org/wiki/index.php?title=Keyboards_Are_Evil
[s down] [shift down][s up] [shift up]=error
[shift down][s down][v down][shift up][v up] [s up] = error
fixed those. now only alt causes problems
-find source of delay. use precomputed wav tables?
"""
from Tkinter import *
import tkFileDialog

import music_util
import dlg1
import dlg2
import dlg3
import notesrealtimemidi
import notesrealtimewav
import notesinterpret

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
		self.btnRecord =  pack( Button(frame2, text='Record...', command=self.startRecordProcess), side=LEFT, pady=25, padx=5)
		self.btnStop =  pack( Button(frame2, text='Stop', command=self.stopRecord), side=LEFT, pady=25, padx=3)
		self.btnStop.config(state=DISABLED)
		pack( Label(frame2, text='\t Notate what is played, and export to Finale or Sibelius.'), side=LEFT)
		self.lblEvents = pack( Label(root, text=''),side=TOP)
	
		manualbindings = {
			('','Up'): Callable(self.changeTransposition,12),
			('','Down'): Callable(self.changeTransposition,-12),
			('Control+','Up'): Callable(self.changeTransposition,1),
			('Control+','Down'): Callable(self.changeTransposition,-1),
			('Alt+','F4'):self.onClose }
			
		self.objNotesRealtime = notesrealtimewav.NotesRealtimeWav(manualbindings)
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
		res1 = dlg1.Dlg1(self.root)
		if res1.result==None: return
		bSharps, bTreble, timesig = res1.result
		res2 = dlg2.Dlg2(self.root)
		if res2.result==None: return
		nQuantize = res2.result
		res3 = dlg3.Dlg3(self.root)
		if res3.result==None: return
		
		#begin recording
		self._settings = (bSharps, bTreble, timesig, nQuantize)
		self.btnStop.config(state=NORMAL)
		self.btnRecord.config(state=DISABLED)
		self.objNotesRealtime.setRecordingMode(True)
		self.isRecording=True
	
	def stopRecord(self):
		self.isRecording=False
		self.btnStop.config(state=DISABLED)
		self.btnRecord.config(state=NORMAL)
		res = self.objNotesRealtime.setRecordingMode(False)
		bSharps, bTreble, timesig, nQuantize = self._settings
		try:
			res = notesinterpret.notesinterpret(res, timesig, nQuantize)
			#doc = convToClass(res, timesig, nQuantize, bTreble, bSharps)
		except notesinterpret.NotesinterpretException, e:
			alert('Exception:'+str(e), title='Trilling Recorder')
			return
		
		
	def onlosefocus(self,e=None):
		self.objNotesRealtime.stopallnotes()
		if self.isRecording:
			alert('Recording was stopped, because you switched to another program.')
			self.stopRecord()
		
	def onClose(self):
		try:
			self.objNotesRealtime.closeDevice()
		finally:
			self.root.destroy()
			#if not in a finally, it's possible error thrown and app can't close


class Callable():
	def __init__(self, func, *args, **kwds):
		self.func = func
		self.args = args
		self.kwds = kwds
	def __call__(self, event=None):
		return apply(self.func, self.args, self.kwds)
	def __str__(self):
		return self.func.__name__

def alert(message, title=None, icon='info'):
	""" Show dialog with information. Icon can be one of 'info','warning','error', defaulting to 'info'."""
	import tkMessageBox
	if icon=='info':
		return tkMessageBox.showinfo(title=title, message=message)
	elif icon=='warning':
		return tkMessageBox.showwarning(title=title, message=message)
	elif icon=='error':
		return tkMessageBox.showerror(title=title, message=message)

root = Tk()
app = Gui1(root)
root.mainloop()

