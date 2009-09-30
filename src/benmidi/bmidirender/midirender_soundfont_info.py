from Tkinter import *
import tkSimpleDialog
import exceptions
import os
import subprocess

import midirender_util
import midirender_runtimidity

from soundfontpreview import pysf

sampleMidiPath = 'soundfontpreview'
sampleMidiScale = os.path.join(sampleMidiPath, 'scale.mid')

#This window can be one of two things:
#A list of voices to choose from,  (bSelectMode = True), returning object of type SoundFontInfoPreset in dlg.result
#and a simple soundFont information dialog. (bSelectMode = False)


def pack(o, **kwargs): o.pack(**kwargs); return o

class BSoundFontInformation(tkSimpleDialog.Dialog):
	def __init__(self, parent, soundfontfilename, bSelectMode):
		self.bSelectMode = bSelectMode
		self.soundfontfilename = soundfontfilename
		
		if self.bSelectMode: title = 'Choose voice...'
		else: title = 'SoundFont Information'
		tkSimpleDialog.Dialog.__init__(self, parent, title)
		
	def body(self, top):
		self.labelFieldNames = ['name', 'date', 'author', 'copyright', 'comment']
		
		frameMain = pack( Frame(top), side=TOP,fill=BOTH, expand=True)
		
		
		frameLoadSf = Frame(frameMain)
		self.lblCurrentSf = pack(Label(frameLoadSf, text='Soundfont:none.sf2' ), side=LEFT,padx=5)
		frameLoadSf.grid(row=0, column=0, columnspan=2)
		
		
		frameInfo = Frame(frameMain)
		self.lblInfoFields = {}
		for name in self.labelFieldNames:
			self.lblInfoFields[name] = pack(Label(frameInfo, text=name+': '),side=TOP,anchor='nw')
		frameInfo.grid(row=1, column=0)
		
		frameVoices = Frame(frameMain)
		Label(frameVoices, text='Voices').pack(side=TOP)
		self.lbVoices = pack(midirender_util.ScrolledListbox(frameVoices, width=45, height=9), side=TOP)
		frameVoices.grid(row=1, column=1)
		
		
		framePreview = Frame(frameMain)
		frPrevBtns = Frame(framePreview)
		if self.bSelectMode: Label(frPrevBtns, text='Preview voice:').pack(side=LEFT)
		else: Label(frPrevBtns, text='Preview Soundfont:').pack(side=LEFT)
		Button(frPrevBtns,text='Start', command=self.previewSfStart).pack(side=LEFT,padx=2)
		Button(frPrevBtns,text='Stop', command=self.previewSfStop).pack(side=LEFT,padx=2)
		frPrevBtns.pack(pady=5)
		
		if not self.bSelectMode:
			frameMidi = Frame(framePreview)
			self.lblMidi = pack(Label(frameMidi, text=sampleMidiScale), side=LEFT,fill=BOTH)
			Button(frameMidi,text='..',command=self.loadMid).pack(side=LEFT,padx=45)
			frameMidi.pack(side=TOP, anchor='w',pady=35)
			self.varPrevVoiceOnly = IntVar(); self.varPrevVoiceOnly.set(0)
			Checkbutton(framePreview, var=self.varPrevVoiceOnly, text='Preview the selected voice.').pack(side=TOP, anchor='w',pady=1)
			self.currentMidi = sampleMidiScale
		else:
			self.varPrevVoiceOnly = IntVar(); self.varPrevVoiceOnly.set(1)
			self.currentMidi = sampleMidiScale
		

		framePreview.grid(row=2, column=0, columnspan=2)
		
		frameMain.grid_columnconfigure(0, weight=1, minsize=20)
		frameMain.grid_columnconfigure(1, weight=1, minsize=20)
		frameMain.grid_rowconfigure(1, weight=1, minsize=20)
		
		self.player = None
		self.loadSf( self.soundfontfilename)
		
		return None # initial focus
		
		
	def apply(self): #called when Ok or Cancel clicked.
		sel = self.lbVoices.curselection() #returns a tuple of selected items
		if len(sel)==0: 
			self.result = None
		else:
			index = int(sel[0])
			self.result = self.objSf.presets[index] #an object of type SoundFontInfoPreset
	
	
	def loadSf(self,filename):
		self.lbVoices.delete(0, END) #clear existing voices
		self.lblCurrentSf['text'] = filename
		self.objSf = None
		
		if filename.lower().endswith('.pat'):
			#gus (gravis ultrasound) patch
			namestart, nameend = os.path.split(filename)
			self.lbVoices.insert(END, nameend)
			
			self.objSf = SoundFontInfo()
			self.objSf.type='pat'
			self.objSf.name = nameend
			self.objSf.filename = filename
			self.varPrevVoiceOnly.set(1)
		else:
			try:
				objSf = getpresets(filename)
			except SFInfoException, e:
				midirender_util.alert(str(e))
				return
				
			#set labels
			for name in self.labelFieldNames:
				att = getattr(objSf, name)
				att = (att if att else 'None')
				if len(att)>80: att = att[0:80]
				self.lblInfoFields[name]['text'] = name+': '+ att
			
			# fill listbox
			for preset in objSf.presets:
				self.lbVoices.insert(END, str(preset))
		
			self.objSf = objSf
			self.objSf.filename = filename
			self.objSf.type='soundfont'
		
		if self.bSelectMode: self.lbVoices.selection_set(0)
		
	
	def loadMid(self):
		filename = midirender_util.ask_openfile(title="Choose midi song", types=['.mid|Midi'], initialfolder=sampleMidiPath)
		if not filename: return
		self.lblMidi['text'] = filename
		self.currentMidi = filename
		
	def previewSfStart(self):
		if self.player!=None and self.player.isPlaying: return #don't play while something is already playing.
			
		#check for mid, see if it exists
		curMid = self.currentMidi
		if not os.path.exists(curMid):
			midirender_util.alert("Couldn't find midi file.")
			return
		
		#if soundfont couldn't be parsed.
		if self.objSf == None: return
		
		#get cfg data
		escapedfname = self.objSf.filename
		if self.objSf.type!='pat':
			sel = self.lbVoices.curselection()
			if self.varPrevVoiceOnly.get() and len(sel)>0:
				#need to map that voice to bank0, program0
				voiceIndex = int(sel[0])
				
				voiceData = self.objSf.presets[voiceIndex]
				try:
					bank = int(voiceData.bank)
					program = int(voiceData.presetNumber)
				except ValueError:
					midirender_util.alert("Couldn't parse bank/preset number.")
					return
				
				strCfg = '\nbank 0\n'
				strCfg += '000 %font "'+escapedfname+'" %d %d'%(bank, program) + '\n'
			else:
				strCfg = '\n'
				strCfg += 'soundfont "'+escapedfname+'"\n'
				
		else:
			strCfg = '\nbank 0\n'
			strCfg += '000 "'+escapedfname+'" \n'
		
		self.player = midirender_runtimidity.RenderTimidityMidiPlayer()
		self.player.setConfiguration(strCfg)
		self.player.playAsync(curMid)
		
	def previewSfStop(self):
		if self.player == None: return
		self.player.signalStop()
		

verbose = False
def getpresets(file):
	class nullFile:
		def _null(self, *args, **kwds):
			pass

		def __getattr__(self, name):
			return self._null

	if not os.path.exists(file):
		raise SFInfoException('Could not find the soundfont...')
	
	try:
		#if an error occurs reading the file, catch the exception.
		
		InHandle = open(file, 'rb')
		OutHandle = nullFile()
		Chunk = pysf.SfChunkReader(InHandle)
		Tree = pysf.SfTree(pysf.SfItems(), pysf.SfContainers, None, OutHandle, "")
		Tree.Read(Chunk, 0)
		Presets = pysf.SfZoneList(Tree, pysf.SfZoneType('preset'))
	
		currentFont = SoundFontInfo()
		INAM = Tree.CkIdStr('INAM', None, -1)
		ICRD = Tree.CkIdStr('ICRD', None, -1)
		IENG = Tree.CkIdStr('IENG', None, -1)
		IPRD = Tree.CkIdStr('IPRD', None, -1)
		ICOP = Tree.CkIdStr('ICOP', None, -1)
		ICMT = Tree.CkIdStr('ICMT', None, -1)
		if INAM != None:
			currentFont.name = INAM
		if ICRD != None:
			currentFont.date = ICRD
		if IENG != None:
			currentFont.author = IENG
			if verbose: print 'By: ' + IENG
		if IPRD != None:
			currentFont.product = IPRD
		if ICOP != None:
			currentFont.copyright = ICOP
		if ICMT != None:
			currentFont.comment = ICMT
		for preset in Presets:
			currentFont.presets.append(SoundFontInfoPreset())
			currentFont.presets[-1].name = preset[u'name']
			currentFont.presets[-1].presetNumber = preset[u'presetId']
			if verbose: print '\tProgram: ' + preset[u'presetId']
			currentFont.presets[-1].bank = preset[u'bank']
			if verbose: print '\tBank: ' + preset[u'bank']
		InHandle.close()
		OutHandle.close()
		
	except pysf.PysfException, e:
		raise SFInfoException('Could not parse soundfont: '+str(e))
		
	return currentFont
	
class SFInfoException(exceptions.Exception): pass

class SoundFontInfo():
	type=None
	name = None
	date = None
	author = None
	product = None
	copyright = None
	comment = None
	
	presets = None
	def __init__(self):
		self.presets = []
		#each is a SoundFontInfoPreset
	def __repr__(self):
		s = 'name: %s\n date: %s\n author: %s\n product: %s\n copyright: %s\n comment: %s'%(self.name,self.date,self.author,self.product,self.copyright, self.comment)
		for preset in self.presets: s+= '\n\tPreset '+str(preset)
		return s
		
class SoundFontInfoPreset():
	name=None
	bank=None
	presetNumber=None #a string, for now
	def __repr__(self):
		return '"%s"   bank: %s, instrument: %s'%(self.name, self.bank, self.presetNumber)


	
