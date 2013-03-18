from Tkinter import *
import midirender_util

from bmidilib import bmidilib

class MixerTrackInfo():
	def __init__(self,trackNumber, enableVar, volWidget, panWidget,transposeVar):
		self.trackNumber = trackNumber; self.enableVar = enableVar; self.volWidget = volWidget; self.panWidget = panWidget; self.transposeVar = transposeVar

class BMixerWindow():
	def __init__(self, top, midiObject, opts, callbackOnClose=None):
		#should only display tracks with note events. that way, solves some problems
		top.title('Mixer')
		
		frameTop = Frame(top, height=600)
		frameTop.pack(expand=YES, fill=BOTH)
		self.frameTop=frameTop
		
		ROW_NAME = 0
		ROW_CHECK = 1
		ROW_VOL = 2
		ROW_PAN = 3
		ROW_TRANSPOSE = 4
		
		Label(frameTop, text='Pan:').grid(row=ROW_PAN, column=0)
		Label(frameTop, text='Volume:').grid(row=ROW_VOL, column=0)
		Label(frameTop, text=' ').grid(row=ROW_NAME, column=0)
		Label(frameTop, text='Enabled:').grid(row=ROW_CHECK, column=0)
		Label(frameTop, text='Transpose:').grid(row=ROW_TRANSPOSE, column=0)
		
		warnMultiple=[]
		self.state = []
		col = 1 #column 0 is the text labels
		for trackNumber in range(len(midiObject.tracks)):
			track = midiObject.tracks[trackNumber]
			if not len(track.notelist): continue #only display tracks with note lists
			
			scpan = Scale(frameTop, from_=-63, to=63, orient=HORIZONTAL,length=32*2) #make it possible to set to 0.
			scpan.grid(row=ROW_PAN, column=col, sticky='EW')
			scvol = Scale(frameTop, from_=127, to=0, orient=VERTICAL)
			scvol.grid(row=ROW_VOL, column=col, sticky='NS')
			Label(frameTop, text='    Track %d    '%trackNumber).grid(row=ROW_NAME, column=col)
			checkvar = IntVar(); 
			Checkbutton(frameTop, text='', var=checkvar).grid(row=ROW_CHECK, column=col)
			transposevar = StringVar(); transposevar.set('0')
			Entry(frameTop, width=4, textvariable=transposevar).grid(row=ROW_TRANSPOSE, column=col)
			
			#defaults
			(firstpan, firstvol, bMultiplePans, bMultipleVols) = getFirstVolumeAndPanEvents(track)
			if bMultipleVols or bMultiplePans: warnMultiple.append(trackNumber)
			scvol.set(100 if (firstvol==None) else firstvol.velocity)
			scpan.set(0 if (firstpan==None) else (firstpan.velocity-64))
			checkvar.set(1)
			
			self.state.append(MixerTrackInfo(trackNumber, checkvar, scvol, scpan,transposevar))
			col += 1
		
		
		if len(warnMultiple)!=0:
			midirender_util.alert('One or more of the tracks (%s) has multiple pan or volume events. You can still use the mixer, but it will only modify the first event.'%','.join(str(n) for n in warnMultiple ))
			
		frameTop.grid_rowconfigure(ROW_VOL, weight=1)
		#~ frameTop.grid_columnconfigure(0, weight=1)
		
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
		
def getFirstVolumeAndPanEvents(trackObject):
	#return format is (evtpan, evtvol, baremanyPan, barmanyVol)
	firstpan=None
	firstvol=None
	bMultiplePans = False; bMultipleVols = False
	for evt in trackObject.events:
		if evt.type=='CONTROLLER_CHANGE':
			if evt.pitch==0x0A: #pan
				if firstpan==None:
					firstpan = evt
				else:
					bMultiplePans = True
			elif evt.pitch==0x07: #vol
				if firstvol==None:
					firstvol = evt
				else:
					bMultipleVols = True
	return (firstpan, firstvol, bMultiplePans, bMultipleVols)



if __name__=='__main__':
	
	
	midiobj = bmidilib.BMidiFile()
	midiobj.open('..\\midis\\16keys.mid', 'rb')
	midiobj.read()
	midiobj.close()
	
	root = Tk()
	opts = {}
	
	def callback():
		print 'callback'
		import copy
		newmidi = copy.deepcopy(midiobj)
		app.createMixedMidi(newmidi)
		newmidi.open('out.mid','wb')
		newmidi.write()
		newmidi.close()
	
	app = BMixerWindow(root,midiobj, opts)
	Button(app.frameTop, text='test', command=callback).grid(row=0,column=0)
	
	root.mainloop()
	
	