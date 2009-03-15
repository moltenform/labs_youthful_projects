
from Tkinter import *
import midirender_util
import midirender_previewsolo
import soundfontpreview


sys.path.append('..\\bmidilib')
import bmidilib

soundfontdir = 'soundfonts'

#customizationState
#holds a {}, maps instrument (assumes bank 0?) to array [SoundFontObject, bank, preset, dictSpecialOptions], or None. none means default

def pack(o, **kwargs): o.pack(**kwargs); return o
class BChangeSoundfontWindow():
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
	
	def changeSoundfont(self):
		dlg = ChooseSoundfontFileDialog(self.frameTop)
		if dlg.result==None: return
		name, filename = dlg.result
		self.lblAbove.config(text='Current soundfont/patch set: "%s"'%name)
		self.currentSoundfont.name = name
		self.currentSoundfont.filename = filename
		
	def openDefaultSoundfontPreview(self):
		self.openSoundfontPreview(self.currentSoundfont.filename)
		
	def openSoundfontPreview(self, filename):
		top = Toplevel()
		previewer = soundfontpreview.BSoundFontPreview(top)
		previewer.loadSf(filename)
		
	def __init__(self, top, currentSoundfont, midiObject, callbackOnClose=None):
		top.title('Choose Soundfont...')
		
		frameTop = Frame(top, height=600)
		frameTop.pack(expand=YES, fill=BOTH)
		self.frameTop=frameTop
		
		frameAbove = Frame(frameTop, border=3, relief=GROOVE, padx=9, pady=9)
		frameAbove.grid(row=0, column=0, sticky='nsew')
		#~ frameAbove.pack(side=TOP, fill=BOTH)
		self.lblAbove = Label(frameAbove, text='Current soundfont/patch set: "%s"'%currentSoundfont.name); 
		self.lblAbove.pack(side=LEFT)
		Button(frameAbove, text='Change...',command=self.changeSoundfont).pack(side=LEFT, padx=20)
		Button(frameAbove, text='Show Soundfont Info',command=self.openDefaultSoundfontPreview).pack(side=LEFT, padx=20)
		
		self.btnCustomize = Button(frameTop, text='Customize', command=self.toggleCustomize)
		self.btnCustomize.grid(row=1, column=0, sticky='w')
		self.customizationState = {} #the important one
		self.objMidi = midiObject
		self.currentSoundfont = currentSoundfont #not a copy. we modify it itself.
		
		if callbackOnClose!=None:
			def callCallback():
				callbackOnClose()
				top.destroy()
			top.protocol("WM_DELETE_WINDOW", callCallback)
		self.top = top
		self.frameTop=frameTop
		
		
		
	def destroy(self):
		self.top.destroy()
		
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
		
		self.lblPatchPack = Label(frameRight, text='Patch: ')
		self.lblPatchPack.pack(side=TOP, pady=20)
		frPatch = Frame(frameRight)
		self.lbPatchChoose = midirender_util.ScrolledListbox(frPatch, selectmode=SINGLE, width=35, height=4, exportselection=0)
		self.lbPatchChoose.pack(side=LEFT)
		Button(frPatch, text='Set').pack(side=LEFT, anchor='s', padx=5, pady=5)
		frPatch.pack(side=TOP)
		
		Button(frameRight, text='Other Soundfont/patch').pack(side=TOP)
		Button(frameRight, text='Set preset within Soundfont').pack(side=TOP)
		Button(frameRight, text='Preview Solo').pack(side=TOP)
		#~ Button(frameRight, text='Advanced...').pack(side=TOP)
		Label(frameRight,text='\n\nChanges remain in effect while this dialog is open.').pack(side=TOP)
		
		#fill up self.lbProgChanges
		res = getPrograms(self.objMidi)
		for instnum, tracknum  in res:
			s = '%s (track %d)'%(bmidilib.bmidiconstants.GM_instruments[instnum], tracknum)
			self.lbProgChanges.insert(END, s)
		
		self.lbProgChanges.selection_set(0)
		self.arProgChanges = res
		
		self.lbPatchChoose.insert(END, 'Default')
		self.lbPatchChoose.selection_set(0)
		
		self.current=None
		self.pollLbProgChanges() #begin polling the listbox. will call first change.
	
	def pollLbProgChanges(self):
		now = self.lbProgChanges.curselection()
		if now != self.current:
		    self.onChangeLbProgChanges(now)
		    self.current = now
		self.frameTop.after(250, self.pollLbProgChanges)

	def onChangeLbProgChanges(self, currentSelection):
		if len(currentSelection)==0: 
			return
		index = int(currentSelection[0])
		instnum,tracknum = self.arProgChanges[index]
		self.lblInstname.config(text = '%03d %s'%(instnum, bmidilib.bmidiconstants.GM_instruments[instnum]))
		
	def addToConfigurationFile(self):
		#apply all of the changes here...
		for i in range(
			if i==current: #-get it from the 
		
	

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

def look_for_soundfont_files():
	import os
	
	
	sfs = os.listdir(os.path.join(soundfontdir, 'sf_gm'))
	sfs = [sf for sf in sfs if (sf.lower().endswith('.sf2') or sf.lower().endswith('.sbk'))]
	paths = [soundfontdir + os.sep + 'sf_gm' + os.sep + fname for fname in sfs]

	sfappend = os.listdir(os.path.join(soundfontdir, 'pat_gm'))
	pats = [sf for sf in sfappend if (sf.lower().endswith('.cfg'))]
	patpaths = [soundfontdir + os.sep + 'pat_gm' + os.sep + fname for fname in sfs]
	
	sfs.extend(pats); paths.extend(patpaths)
	return sfs,paths
	

import tkSimpleDialog
#Simple dialog. returns filename of a soundfont
class ChooseSoundfontFileDialog(tkSimpleDialog.Dialog): 
	result = None
	def __init__(self, top, currentSoundfontName=None, title='Choose SoundFont:'):
		self.currentSoundfontName = currentSoundfontName
		tkSimpleDialog.Dialog.__init__(self, top, title)
		
	def body(self, top):
		frTop = Frame(top)
		frTop.grid(row=0, column=0, columnspan=2)
		
		Label(frTop, text="Choose SoundFont:").pack(side=TOP)
		
		self.lbSfChoose = midirender_util.ScrolledListbox(frTop, selectmode=SINGLE, width=45, height=15)
		self.lbSfChoose.pack(side=TOP)
		
		#fill 
		sfs,paths = look_for_soundfont_files()
		
		self.lbSfChoose.delete(0, END)
		for sfName in sfs:
			self.lbSfChoose.insert(END, sfName)
		
		self.sfs = sfs
		self.paths = paths
		self.alternateFile = None
		
		foundindex=-1
		for i in range(len(sfs)):
			if sfs[i]==self.currentSoundfontName: foundindex=i; break
		if foundindex != -1:
			self.lbSfChoose.selection_set(foundindex)
			self.lbSfChoose.see(foundindex)
		
		Button(frTop, text='Other...',command=self.getOther).pack(side=TOP)
	
		return None # initial focus
	def getOther(self):
		filename = midirender_util.ask_openfile(title="Open SoundFont", types=['.sf2|SoundFont','.sbk|SoundFont1'])
		if not filename: return
		if not self.alternateFile: #this is the first alternate file. make a "other" entry
			self.lbSfChoose.insert(END, 'Other')
			self.alternateFile = filename
		else:
			self.alternateFile = filename
		
		#go to the last one
		self.lbSfChoose.selection_set(END)
		self.lbSfChoose.see(END)
		
	def apply(self):
		sel = self.lbSfChoose.curselection() #returns a tuple of selected items
		if len(sel)==0: 
			self.result = None
		else:
			index = int(sel[0])
			if index==len(self.sfs): #the user picked the last one, "Other"
				self.result = (self.alternateFile, self.alternateFile)
			else:
				self.result = (self.sfs[index], self.paths[index])
		
#There is one shared instance of this. This way the change remains across windows and midi files. 
class SoundFontObject():
	name=''
	filename =''
	def __init__(self, filename=None):
		if filename==None:
			possibleFiles, possiblePaths = look_for_soundfont_files()
			if (len(possibleFiles)!=0):
				self.name = possibleFiles[0]
				self.filename = possiblePaths[0]

if __name__=='__main__':
	if False:
		#test the little dialog
		def start(top):
			def callback():
				print 'hi'
				dlg = ChooseSoundfontFileDialog(top)
				print dlg.result
				
			Button(text='go', command=callback).pack()

		root = Tk()
		start(root)
		root.mainloop()
	else:
		midiobj = bmidilib.BMidiFile()
		midiobj.open('..\\midis\\weird.mid', 'rb')
		midiobj.read()
		midiobj.close()
		
		root = Tk()
		
		def callback():
			print 'callback'
		
		
		app = BChangeSoundfontWindow(root,SoundFontObject(), midiobj, None)
		#~ Button(app.frameTop, text='test', command=callback).grid(row=0,column=0)
		
		root.mainloop()
	
	

