from Tkinter import *
import tkSimpleDialog

import midirender_util
sys.path.append('..')
from bmidilib import bmidilib

class ChooseMidiInstrumentDialog(tkSimpleDialog.Dialog):
	result = None
	def __init__(self, top, title, default=0):
		self.defaultInst = default
		tkSimpleDialog.Dialog.__init__(self, top, title)
		
	def body(self, top):
		frTop = Frame(top)
		frTop.grid(row=0, column=0, columnspan=2)
		
		Label(frTop, text="Choose instrument:").pack(side=TOP)
		
		self.lbPatchChoose = midirender_util.ScrolledListbox(frTop, selectmode=SINGLE, width=30, height=15)
		self.lbPatchChoose.pack(side=TOP)
		
		#fill with all of the voices
		for instnum in range(128):
			s = '%03d %s'%(instnum, bmidilib.getInstrumentName(instnum))
			self.lbPatchChoose.insert(END, s)
		
		self.lbPatchChoose.selection_set(self.defaultInst)
		self.lbPatchChoose.see(self.defaultInst)
		
		self.bind('<MouseWheel>',self.scroll) #binding for Windows
		self.bind('<Button-4>',self.scroll) #binding for Linux
		self.bind('<Button-5>',self.scroll)
	
		return None # initial focus

		
	def apply(self):
		sel = self.lbPatchChoose.curselection() #returns a tuple of selected items5
		if len(sel)==0: 
			self.result = 0
		else:
			self.result = int(sel[0])
		
	def scroll(self, event):
		if event.num == 5 or event.delta == -120:
			self.lbPatchChoose.yview_scroll(5, 'units')
		if event.num == 4 or event.delta == 120:
			self.lbPatchChoose.yview_scroll(-5, 'units')


if __name__=='__main__':
	def start(top):
		def callback():
			print 'hi'
			dlg = ChooseMidiInstrumentDialog(top, 'Choose Instrument', 66)
			print dlg.result
			
		Button(text='go', command=callback).pack()
		

	root = Tk()
	start(root)
	root.mainloop()
