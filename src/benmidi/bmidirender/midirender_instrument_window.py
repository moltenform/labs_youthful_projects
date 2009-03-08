
from Tkinter import *
import midirender_util

sys.path.append('..\\bmidilib')
import bmidilib


class BInstrumentWindow():
	def __init__(self, top, midiObject, opts, callbackOnClose=None):
		#should only display tracks with note events. that way, solves some problems
		top.title('Instrument')
		
		frameTop = Frame(top, height=600)
		frameTop.pack(expand=YES, fill=BOTH)
		self.frameTop=frameTop
		
		frameLeft = Frame(frameTop, border=3, relief=GROOVE, padx=9, pady=9)
		frameLeft.grid(row=0, column=0, sticky='nsew')
		frameRight = Frame(frameTop, border=3, relief=GROOVE, padx=9, pady=9)
		frameRight.grid(row=0, column=1, sticky='nsew')
		frameTop.grid_columnconfigure(1, weight=1, minsize=20)
		frameTop.grid_rowconfigure(0, weight=1, minsize=20)
		
		Label(frameLeft, text='Instruments').pack(side=TOP)
		self.lbProgChanges = ScrolledListbox(frameLeft, selectmode=SINGLE, width=30, height=7)
		self.lbProgChanges.pack(side=TOP, expand=YES, fill=BOTH)
		
		self.lblTrackSel = Label(frameRight, text='Track 1'); self.lblTrackSel.pack(side=TOP)
		frInst = Frame(frameRight)
		Label(frInst, text='001 Acoustic Grand Piano').pack(side=LEFT)
		Button(frInst, text='Change...').pack(side=LEFT, padx=45)
		frInst.pack(side=TOP)
		
		
		self.lblPatchPack = Label(frameRight, text='Patch: eawpats\\piano.pat')
		self.lblPatchPack.pack(side=TOP, pady=20)
		frPatch = Frame(frameRight)
		self.lbPatchChoose = ScrolledListbox(frPatch, selectmode=SINGLE, width=30, height=4)
		self.lbPatchChoose.pack(side=LEFT)
		Button(frPatch, text='Set').pack(side=LEFT, anchor='s', padx=5, pady=5)
		frPatch.pack(side=TOP)
		
		Button(frameRight, text='Advanced...').pack(side=TOP)
		
		
		
		
		if callbackOnClose!=None:
			def callCallback():
				callbackOnClose()
				top.destroy()
			top.protocol("WM_DELETE_WINDOW", callCallback)
		
		self.top = top
	
	def destroy(self):
		self.top.destroy()
		
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
				
	return (results, firstTrackWithPercussion)


class ScrolledListbox(Listbox): #an imitation of ScrolledText
	def __init__(self, master=None, cnf=None, **kw):
		if cnf is None:
			cnf = {}
		if kw:
			from Tkinter import _cnfmerge
			cnf = _cnfmerge((cnf, kw))
		fcnf = {}
		for k in cnf.keys():
			if type(k) == ClassType or k == 'name':
				fcnf[k] = cnf[k]
				del cnf[k]
		self.frame = Frame(master, **fcnf)
		self.vbar = Scrollbar(self.frame, name='vbar')
		self.vbar.pack(side=RIGHT, fill=Y)
		cnf['name'] = 'lbox'
		Listbox.__init__(self, self.frame, **cnf)
		self.pack(side=LEFT, fill=BOTH, expand=1)
		self['yscrollcommand'] = self.vbar.set
		self.vbar['command'] = self.yview

		# Copy geometry methods of self.frame -- hack!
		methods = Pack.__dict__.keys()
		methods = methods + Grid.__dict__.keys()
		methods = methods + Place.__dict__.keys()

		for m in methods:
			if m[0] != '_' and m != 'config' and m != 'configure':
				setattr(self, m, getattr(self.frame, m))
		


if __name__=='__main__':
		
	midiobj = bmidilib.BMidiFile()
	midiobj.open('..\\midis\\16keys.mid', 'rb')
	midiobj.read()
	midiobj.close()
	
	root = Tk()
	opts = {}
	
	def callback():
		print 'callback'
	
	app = BInstrumentWindow(root,midiobj, opts)
	#~ Button(app.frameTop, text='test', command=callback).grid(row=0,column=0)
	
	root.mainloop()
	
	

