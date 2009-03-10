
from Tkinter import *
import midirender_util
import midirender_choose_voice

sys.path.append('..\\bmidilib')
import bmidilib



class BInstrumentWindow():
	def __init__(self, top, midiObject, opts,whichSelect=0, callbackOnClose=None):
		#should only display tracks with note events. that way, solves some problems
		top.title('Instrument')
		
		frameTop = Frame(top, height=600)
		frameTop.pack(expand=YES, fill=BOTH)
		self.frameTop=frameTop
		
		#~ frameA = Frame(frameTop, border=3, relief=GROOVE, padx=9, pady=9)
		
		frameLeft = Frame(frameTop, border=3, relief=GROOVE, padx=9, pady=9)
		frameLeft.grid(row=0, column=0, sticky='nsew')
		frameRight = Frame(frameTop, border=3, relief=GROOVE, padx=9, pady=9)
		frameRight.grid(row=0, column=1, sticky='nsew')
		frameTop.grid_columnconfigure(1, weight=1, minsize=20)
		frameTop.grid_rowconfigure(0, weight=1, minsize=20)
		
		Label(frameLeft, text='Instruments').pack(side=TOP)
		self.lbProgChanges = midirender_util.ScrolledListbox(frameLeft, selectmode=SINGLE, width=25, height=7, exportselection=0)
		self.lbProgChanges.pack(side=TOP, expand=YES, fill=BOTH)
		
		frInst = Frame(frameRight)
		Label(frInst, text='001 Acoustic Grand Piano').pack(side=LEFT)
		Button(frInst, text='Change...',command=self.changeInstrument).pack(side=LEFT, padx=45)
		frInst.pack(side=TOP)
		
		
		self.lblPatchPack = Label(frameRight, text='Patch: ')
		self.lblPatchPack.pack(side=TOP, pady=20)
		frPatch = Frame(frameRight)
		self.lbPatchChoose = midirender_util.ScrolledListbox(frPatch, selectmode=SINGLE, width=35, height=4, exportselection=0)
		self.lbPatchChoose.pack(side=LEFT)
		Button(frPatch, text='Set').pack(side=LEFT, anchor='s', padx=5, pady=5)
		frPatch.pack(side=TOP)
		
		Button(frameRight, text='Advanced...').pack(side=TOP)
		
		
		#fill up self.lbProgChanges
		res = getFirstProgramChangeEvents(midiObject)
		for tracknum, instnum in res:
			s = 'Track %d - '%tracknum
			s+= ('%03d %s'%(instnum, bmidilib.bmidiconstants.GM_instruments[instnum])) if instnum != -1 else 'Percussion'
			self.lbProgChanges.insert(END, s)
		
		self.lbProgChanges.selection_set(whichSelect)
		self.progChanges = res
		
		
		self.lbPatchChoose.insert(END, '(None)')
		self.lbPatchChoose.insert(END, 'Custom...')
		self.lbPatchChoose.selection_set(0)


		if callbackOnClose!=None:
			def callCallback():
				callbackOnClose()
				top.destroy()
			top.protocol("WM_DELETE_WINDOW", callCallback)
		self.top = top
	def destroy(self):
		self.top.destroy()
	
	def changeInstrument(self):
		current = 68
		dlg = midirender_choose_voice.ChooseMidiInstrumentDialog(self.top, 'Choose Instrument', current)
		print dlg.result
		#should now refresh everything.
		
		
	def createMixedMidi(self, midiObject):
		#NOTE: modifies the midi object itself, not a copy
		
		#remove the tracks that are both in the mixer, AND not enabled
		for trackInfo in self.state:
			trackNumber = trackInfo.trackNumber
			if not trackInfo.enableVar.get():
				#eliminate the track by making an empty one in its place
				midiObject.tracks[trackNumber] = bmidilib.BMidiTrack() 
				evt = bmidilib.BMidiEvent(); evt.type='END_OF_TRACK'; evt.time = 1; evt.data = ''
				midiObject.tracks[trackNumber].events.append(evt)
			else:
				trackObject = midiObject.tracks[trackNumber]
				volValue = trackInfo.volWidget.get()
				panValue = trackInfo.panWidget.get() + 64 #is from 0 to 127, instead of -63 to 63
				transposeValue = trackInfo.transposeVar.get()
				try: transposeValue=int(transposeValue)
				except: trackInfo.transposeVar.set(0); transposeValue = 0
				
				#modify the event directly, if it exists. Otherwise, create and add a new event.
				(firstpan, firstvol, bMultiplePans, bMultipleVols) = getFirstVolumeAndPanEvents(midiObject.tracks[trackNumber])
				if firstpan:  firstpan.velocity = panValue
				else:
					#create a new pan event. 
					evt = bmidilib.BMidiEvent(); evt.type='CONTROLLER_CHANGE'; evt.time = 0; evt.pitch = 0x0A; evt.velocity = panValue
					evt.channel = trackObject.notelist[-1].startEvt.channel #assume that the channel of the track is the channel of the first note.
					trackObject.events.insert(0, evt)
				
				if firstvol:  firstvol.velocity = volValue
				else:
					#create a new vol event. 
					evt = bmidilib.BMidiEvent(); evt.type='CONTROLLER_CHANGE'; evt.time = 0; evt.pitch = 0x07; evt.velocity = volValue
					evt.channel = trackObject.notelist[-1].startEvt.channel
					trackObject.events.insert(0, evt)
				
				#transpose tracks, using the notelist
				if transposeValue!=0:
					for note in trackObject.notelist:
						nextpitch = min(max(note.pitch + transposeValue, 0), 127) #make sure between 0 and 127
						note.startEvt.pitch = nextpitch
						note.endEvt.pitch = nextpitch
						note.pitch = nextpitch
					
		
		return None #as a signal that this modifies, not returns a copy
		
def getFirstProgramChangeEvents(midiObject):
	#return format is (evtpan, evtvol, baremanyPan, barmanyVol)
	firstpan=None
	firstvol=None
	results = [] #(tracknum, instrument). includes all program changes. 
	#also, if there are chan10 events, addes EXACTLY one percussion, instrument=-1.
	haveRecordedPercussion = None
	for tracknum in range(len(midiObject.tracks)):
		trackObject = midiObject.tracks[tracknum]
		for evt in trackObject.events:
			if evt.type=='PROGRAM_CHANGE':
				results.append((tracknum, evt.data))
			elif evt.type=='NOTE_ON' and evt.channel==10 and not haveRecordedPercussion:
				results.append((tracknum, -1))
				haveRecordedPercussion = True
				
	return results



		



		

if __name__=='__main__':
	
	midiobj = bmidilib.BMidiFile()
	midiobj.open('..\\midis\\weird.mid', 'rb')
	midiobj.read()
	midiobj.close()
	
	root = Tk()
	opts = {}
	
	def callback():
		print 'callback'
	
	app = BInstrumentWindow(root,midiobj, opts)
	#~ Button(app.frameTop, text='test', command=callback).grid(row=0,column=0)
	
	root.mainloop()
	
	

