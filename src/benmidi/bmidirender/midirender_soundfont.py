
from Tkinter import *
import midirender_util

import os

sys.path.append('..')
from bmidilib import bmidilib

soundfontsdir = 'soundfonts'
gm_dir = os.path.join(soundfontsdir,'sf_gm')

def getDefaultSoundfont():
	files = os.listdir(gm_dir)
	files = [file for file in files if file.endswith('.sf2')]
	if len(files)==0:
		return '(None)'
	else:
		return os.path.join(gm_dir, files[0])



def pack(o, **kwargs): o.pack(**kwargs); return o
class BSoundfontWindow():
	haveOpenedCustomize=False
	showCustomize = False
	def toggleCustomize(self):
		if not self.showCustomize:
			if not self.haveOpenedCustomize:
				self.openCustomize()
				self.haveOpenedCustomize = True
			#~ self.frameCustomize.pack(side=TOP, fill=BOTH, expand=YES)
			self.frameCustomize.grid(row=2, column=0, sticky='nsew')
			self.frameTop.grid_rowconfigure(2, weight=1, minsize=10)
			self.frameTop.grid_columnconfigure(0, weight=1, minsize=10)
			self.btnCustomize.config(relief=SUNKEN)
			self.showCustomize = True
		else:
			self.frameCustomize.grid_forget()
			self.btnCustomize.config(relief=RAISED)
			self.showCustomize = False
	
	def __init__(self, top, main_soundfont_reference, midiObject, callbackOnClose=None):
		top.title('Choose Soundfont...')
		
		frameTop = Frame(top, height=600)
		frameTop.pack(expand=YES, fill=BOTH)
		self.frameTop=frameTop
		
		frameAbove = Frame(frameTop, border=3, relief=GROOVE, padx=9, pady=9)
		frameAbove.grid(row=0, column=0, sticky='nsew')
		#~ frameAbove.pack(side=TOP, fill=BOTH)
		self.lblAbove = Label(frameAbove, text='Current soundfont: "%s"'%main_soundfont_reference[0]); 
		self.lblAbove.pack(side=LEFT)
		Button(frameAbove, text='Change...',command=self.setGlobalSoundfont).pack(side=LEFT, padx=20)
		Button(frameAbove, text='Soundfont Information',command=self.nothing).pack(side=LEFT, padx=20)
		
		self.btnCustomize = Button(frameTop, text='Customize', command=self.toggleCustomize)
		self.btnCustomize.grid(row=1, column=0, sticky='w')
		
		
		self.arCustomizationState = None
		self.objProgramsInMidi = getPrograms(midiObject) #so if the instruments change, we'll need to close and reopen this window to see the changes...
		self.main_soundfont_reference = main_soundfont_reference #not a copy. we modify it itself.
		
		
		
		if callbackOnClose!=None:
			def callCallback():
				callbackOnClose()
				top.destroy()
			top.protocol("WM_DELETE_WINDOW", callCallback)
		self.top = top
		self.frameTop=frameTop
		
	
		
	def openCustomize(self):
		self.frameCustomize = Frame(self.frameTop)
		frameLeft = Frame(self.frameCustomize, border=3, relief=GROOVE, padx=9, pady=9)
		frameLeft.grid(row=1, column=0, sticky='nsew')
		frameRight = Frame(self.frameCustomize, border=3, relief=GROOVE, padx=9, pady=9)
		frameRight.grid(row=1, column=1, sticky='nsew')
		self.frameCustomize.grid_columnconfigure(0, weight=1, minsize=20)
		self.frameCustomize.grid_rowconfigure(0, weight=0, minsize=20)
		
		Label(frameLeft, text='Instruments').pack(side=TOP)
		self.lbProgChanges = midirender_util.ScrolledListbox(frameLeft, selectmode=SINGLE, width=25, height=7, exportselection=0)
		self.lbProgChanges.pack(side=TOP, expand=YES, fill=BOTH)
		
		frInst = Frame(frameRight)
		self.lblInstname = pack(Label(frInst, text='001 Acoustic Grand Piano'), side=LEFT)
		frInst.pack(side=TOP)
		
		self.lblInstsf = pack(Label(frameRight, text='Soundfont: default.sf2 bank 0 program 0'), side=TOP, pady=20)
		
		frBtns = Frame(frameRight)
		Button(frBtns, text='Choose soundfont...',command=self.onBtnChooseSoundfont).pack(side=LEFT, anchor='s')
		Button(frBtns, text='Choose voice within soundfont...',command=self.nothing).pack(side=LEFT, anchor='s')
		frBtns.pack(side=TOP)
		
		frPrev = Frame(frameRight)
		Label(frPrev, text='Preview solo:').pack(side=LEFT, anchor='s')
		Button(frPrev, text='Start',command=self.nothing).pack(side=LEFT, anchor='s')
		Button(frPrev, text='Stop',command=self.nothing).pack(side=LEFT, anchor='s')
		frPrev.pack(side=TOP, pady=5)
		
		Label(frameRight,text='\n\nChanges remain in effect while this dialog is open.').pack(side=TOP)
		
		#fill up self.lbProgChanges
		for instnum, tracknum in self.objProgramsInMidi :
			s = '%s (track %d)'%(bmidilib.getInstrumentName(instnum), tracknum)
			self.lbProgChanges.insert(END, s)
		
		self.lbProgChanges.selection_set(0)
		
		
		#create customizationState. note that in self.objProgramsInMidi, instruments are unique, which is nice.
		self.arCustomizationState = []
		for instnum, tracknum in self.objProgramsInMidi:
			state = {}
			state['instrument'] = instnum
			state['soundfont'] = self.main_soundfont_reference[0]
			state['sf_program'] = instnum
			state['sf_bank'] = 0
			self.arCustomizationState.append(state)
		
		self.current=None
		self.pollLbProgChanges() #begin polling the listbox. will call first change.
	
	def getCfgResults(self):
		if not self.showCustomize: 
			return ''
		
		#otherwise, get the data
		return ''
		
	def setGlobalSoundfont(self):
		filename = midirender_util.ask_openfile(initialfolder=gm_dir, title="Choose SoundFont", types=['.sf2|SoundFont','.sbk|SoundFont1','.cfg|Timidity Configuration file'])
		if not filename: return
		
		self.main_soundfont_reference[0] = filename
		self.lblAbove['text'] ='Current soundfont:%s'%filename
		
		
	def nothing(self): pass

	def onChangeLbProgChanges(self):
		index = self.getListboxIndex()
		state = self.arCustomizationState[index]
		
		self.lblInstname.config(text = '%03d %s'%(state['instrument'], bmidilib.getInstrumentName(state['instrument'])))
		if state['soundfont'].endswith('.pat'):
			self.lblInstsf.config(text='Soundfont: %s'%(state['soundfont']))
		else:
			self.lblInstsf.config(text='Soundfont: %s\n bank %d program %d'%(state['soundfont'],state['sf_bank'],state['sf_program']))
		
	def onBtnChooseSoundfont(self):
		filename = midirender_util.ask_openfile(initialfolder=gm_dir, title="Choose SoundFont", types=['.sf2|SoundFont','.sbk|SoundFont1','.pat|Patch file'])
		if not filename: return
		
		index = self.getListboxIndex()
		state = self.arCustomizationState[index]
		state['soundfont'] = filename
		self.onChangeLbProgChanges() #refresh the fields
	
	def destroy(self):
		self.top.destroy()
	def pollLbProgChanges(self):
		now = self.lbProgChanges.curselection()
		if now != self.current:
		    self.onChangeLbProgChanges()
		    self.current = now
		self.frameTop.after(250, self.pollLbProgChanges)
	def getListboxIndex(self):
		currentSelection = self.lbProgChanges.curselection()
		if len(currentSelection)==0: raise 'No Listbox selection.'
		return int(currentSelection[0])
	

def getPrograms(midiObject):
	seenVoices = {}
	results = [] #(instrument, tracknum). includes unique program changes (won't record same voice twice)
	haveRecordedPercussion = False
	for tracknum in range(len(midiObject.tracks)):
		trackObject = midiObject.tracks[tracknum]
		for evt in trackObject.events:
			if evt.type=='PROGRAM_CHANGE':
				if evt.data not in seenVoices:
					seenVoices[evt.data] = 1
					results.append((evt.data, tracknum))
			elif evt.type=='NOTE_ON' and evt.channel==10 and not haveRecordedPercussion:
				haveRecordedPercussion = True
	return results



#~ def getFirstProgramChangeEvents(midiObject):
	#~ firstpan=None
	#~ firstvol=None
	#~ results = [] #(tracknum, instrument). includes all program changes. 
	#~ #also, if there are chan10 events, addes EXACTLY one percussion, instrument=-1.
	#~ haveRecordedPercussion = None
	#~ for tracknum in range(len(midiObject.tracks)):
		#~ trackObject = midiObject.tracks[tracknum]
		#~ for evt in trackObject.events:
			#~ if evt.type=='PROGRAM_CHANGE':
				#~ results.append((tracknum, evt.data))
			#~ elif evt.type=='NOTE_ON' and evt.channel==10 and not haveRecordedPercussion:
				#~ results.append((tracknum, -1))
				#~ haveRecordedPercussion = True
				
	#~ return results
	
if __name__=='__main__':
	midiobj = bmidilib.BMidiFile()
	midiobj.open('..\\midis\\weird.mid', 'rb')
	midiobj.read()
	midiobj.close()
	
	root = Tk()
	
	def callback():
		print 'callback'
	
	main_reference = [os.path.join(gm_dir,'TimGM6mb.sf2')]
	app = BSoundfontWindow(root,main_reference, midiobj, None)
	#~ Button(app.frameTop, text='test', command=callback).grid(row=0,column=0)
	
	root.mainloop()

