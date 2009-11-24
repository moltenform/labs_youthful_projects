import sys
import time

import win32midi


#~ can't support too much polyphony, see http://www.sjbaker.org/wiki/index.php?title=Keyboards_Are_Evil
#~ [s down] [shift down][s up] [shift up]=error
#~ [shift down][s down][v down][shift up][v up] [s up] = error
#~ fixed those. now only alt causes problems


# On Windows, the best timer is time.clock()
# On most other platforms the best timer is time.time()
if sys.platform == "win32": fntimer = time.clock
else: fntimer = time.time
    

class NotesRealtimeMidi():
	manualbindings = None
	midiplayerdevice = None
	keyCodesCurrentlyHeld = None
	transposition =60 #default start at c4
	bRecordingMode = False
	listRecorded = None
	
	def __init__(self, manualbindings):
		self.manualbindings = manualbindings
		self.keyCodesCurrentlyHeld = {} #map from Keycode to Notenumber
		self.midiplayerdevice = win32midi.RealTimePlayer()
		self.midiplayerdevice.openDevice()
		
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
			self.listRecorded = []
			self.listRecorded.append((None, fntimer(), None))
			self.bRecordingMode = True
			return None
		else:
			l = self.listRecorded
			self.listRecorded = None
			self.bRecordingMode = False
			return l
	
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
					#~ print 'play note',notenumber
					notenumber = self.notebindings[event.keycode] + self.transposition
					self.midiplayerdevice.rawNoteOn(notenumber)
					if self.bRecordingMode: tm = fntimer()
					else: tm=0
					self.keyCodesCurrentlyHeld[event.keycode] = notenumber,tm #(need to record notenumber because transposition may have changed)
			else:
				self.keyCodesCurrentlyHeld[event.keycode] = -1
		
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
				self.stopallnotes()
			else:
				notenumber = self.keyCodesCurrentlyHeld[event.keycode]
				if notenumber != -1:
					notenumber, tm=notenumber
					self.midiplayerdevice.rawNoteOff(notenumber)
					if self.bRecordingMode:
						self.listRecorded.append((notenumber, tm, fntimer()))
				
				#~ print 'release note',notenumber
				del self.keyCodesCurrentlyHeld[event.keycode]
	
	def _ontab(self):
		print 't'
		if self.bRecordingMode:
			self.listRecorded.append((-1, fntimer(), None))
	
	def _getkeyboardmods(self, eventstate):
		mods = ''
		if eventstate & 0x00000004: mods += 'Control+'
		if eventstate & 0x00020000: mods += 'Alt+'
		if eventstate & 0x00000001: mods += 'Shift+'
		return mods
			
	def stopallnotes(self):  #usually called when lost focus.
		#stop all currently playing notes
		for keyCode in self.keyCodesCurrentlyHeld:
			notenumber = self.keyCodesCurrentlyHeld[keyCode]
			if notenumber!=-1:
				self.midiplayerdevice.rawNoteOff(notenumber)
		self.keyCodesCurrentlyHeld = {}
		

	def closeDevice(self):
		if self.midiplayerdevice != None:
			print 'closing device'
			self.midiplayerdevice.closeDevice()
			self.midiplayerdevice= None
		
	
	
	

