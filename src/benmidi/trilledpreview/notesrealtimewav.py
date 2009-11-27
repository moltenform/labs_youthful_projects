import notesrealtimerecorded

import sys
if sys.platform != "win32": 
	s= 'Platform not currently supported. The Windows-only winsound module is currently used. This will be addressed in a future release.'
	print s
	raise s

import winsound
#linux: see http://stackoverflow.com/questions/307305/play-a-sound-with-python

fntimer = notesrealtimerecorded.fntimer

#does not support polyphony. in output from recording, notes never overlap.
#in fact, polyphony doesn't work well anyways, because this can obscure recognition of Tab key
#todo: check threading


mediadir=r'media'+'\\'

class NotesRealtimeWav():
	manualbindings = None
	keyCodesCurrentlyHeld = None
	transposition =60 #default start at c4
	bRecordingMode = False
	objRecording = None
	
	def __init__(self, manualbindings):
		self.manualbindings = manualbindings
		self.keyCodesCurrentlyHeld = {} #map from Keycode to Notenumber
		
		self.theLastNote=None
		self.objRecording = notesrealtimerecorded.NotesRealtimeRecordedRaw()
		
	def addBindings(self,tkTopLevel):
		
		#add bindings to toplevel.
		
		#key press/release events
		#(they keep repeating the event when held)
		tkTopLevel.bind_all('<Key>', self._onkey)
		tkTopLevel.bind_all('<Any-Alt-Key>', self._onkey)
		tkTopLevel.bind_all('<Tab>', self._onkey)
		tkTopLevel.bind_all('<Shift-Tab>', self._onkey)
		
		#key release events
		tkTopLevel.bind_all('<KeyRelease>', self._onkeyrelease)
		tkTopLevel.bind_all('<Any-Alt-KeyRelease>', self._onkeyrelease)
		
	
	def addNotebindings(self, filename):
		self.notebindings = {}
		f = open(filename,'r')
		for line in f:
			line = line.strip()
			if not line: continue
			key,note,_ = line.split(',')
			self.notebindings[int(key)] = int(note)
		f.close()
	
	def setTranspose(self, amount):
		
		self.transposition += amount
		return self.transposition
		
	def setRecordingMode(self, b):
		if b:
			self.bRecordingMode = True
			self.objRecording.beginRecording()
			return None
		else:
			self.bRecordingMode = False
			# return results of recording
			ret = self.objRecording.getProcessedResults()
			self.objRecording.clear()
			return ret
	
	def _onkey(self, event):
		if event.keycode==16 or event.keycode==17 or event.keycode==0:
			return
		if event.keycode==9:
			self._ontab()
			return 'break'
		
		bNoModifierKeys = event.state==0
		
		mods = self._getkeyboardmods(event.state)
		if (mods,event.keysym) in self.manualbindings:
			self.manualbindings[(mods,event.keysym)]()
			return 'break'
		
		
		if event.keycode in self.notebindings:
			if bNoModifierKeys:
				if event.keycode not in self.keyCodesCurrentlyHeld:
					
					#Because we only support one tone at once -- turn off all other notes
					newdict={}
					for keycode in self.keyCodesCurrentlyHeld:
						notenumber = self.keyCodesCurrentlyHeld[keycode]
						if notenumber != -1:
							notenumber, startTime=notenumber
							if self.bRecordingMode:
								self.objRecording.recNoteEvent(notenumber, startTime)
							
							newdict[keycode] = -1 #signifies we've processed it
						else:
							newdict[keycode] = notenumber
					self.keyCodesCurrentlyHeld = newdict
					
					
					notenumber = self.notebindings[event.keycode] + self.transposition
					self.theLastNote = event.keycode
					winsound.PlaySound(mediadir+str(notenumber)+'.wav',winsound.SND_FILENAME|winsound.SND_ASYNC|winsound.SND_LOOP)
					
					if self.bRecordingMode: startTime = fntimer()
					else: startTime=0
					self.keyCodesCurrentlyHeld[event.keycode] = notenumber,startTime #(need to record notenumber because transposition may have changed)
					
					
					
					
			else:
				self.keyCodesCurrentlyHeld[event.keycode] = -1 #means that this note was played with a modifier key.
		
	def _onkeyrelease(self, event):
		if event.keycode==16 or event.keycode==17 or event.keycode==0:
			return
		
		
		bNoModifierKeys = event.state==0
		
		#~ if bNoModifierKeys and event.keycode in self.notebindings:
		#to fix Shift bugs, don't require noModifierKeys. just call on everything.
		if event.keycode in self.notebindings:
			if event.keycode not in self.keyCodesCurrentlyHeld:
				#in theory, it should be. also we clear keyCodesCurrentlyHeld when app loses focus.
				#so this shouldn't happen.
				#if it does, though, stop all playing notes.
				#the transposition could have changed, so just stop playing everything
				print 'warning, unexpected keyrelease', event.state, event.keysym
				winsound.PlaySound(None,0)
			else:
				notenumber = self.keyCodesCurrentlyHeld[event.keycode]
				if notenumber != -1:
					notenumber, startTime=notenumber
					
					if event.keycode==self.theLastNote:
						winsound.PlaySound(None,0)
					
					if self.bRecordingMode:
						self.objRecording.recNoteEvent(notenumber, startTime)
				
				#~ print 'release note',notenumber
				del self.keyCodesCurrentlyHeld[event.keycode]
	
	def _ontab(self):
		if self.bRecordingMode:
			self.objRecording.recPulseEvent()
	
	def _getkeyboardmods(self, eventstate):
		mods = ''
		if eventstate & 0x00000004: mods += 'Control+'
		if eventstate & 0x00020000: mods += 'Alt+'
		if eventstate & 0x00000001: mods += 'Shift+'
		return mods
			
	def stopallnotes(self): 
		#stop all currently playing notes
		winsound.PlaySound(None,0)
		self.keyCodesCurrentlyHeld = {}
		self.theLastNote=None

	def closeDevice(self):
		pass
		
	
	
	

