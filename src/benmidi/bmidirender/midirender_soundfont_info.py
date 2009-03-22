from Tkinter import *
import exceptions
import os
import subprocess

import midirender_util

if sys.platform=='win32':
	sfubarPath = os.path.join('soundfontpreview', 'sfubar.exe')
else:
	sfubarPath = os.path.join('soundfontpreview', 'sfubar')


quickEnter = False #a worse, but quick interface. sometime, should remove all non-quickenter interface
sfubardir = 'soundfontpreview' #directory to look for sfubar.exe and sample mids
timiditydir = 'timidity' #directory of timidity program
labelFieldNames = ['name', 'date', 'author', 'copyright', 'comment']
def pack(o, **kwargs): o.pack(**kwargs); return o
class BSoundFontPreview():
	def setLabels(self, objInfo):
		for name in labelFieldNames:
			att = getattr(objInfo, name)
			self.lblInfoFields[name]['text'] = name+': '+ (att if att else 'None')
	
	def autoSet(self):
		if self.objSf.type=='pat':
			self.varPrevVoiceOnly.set(1)
			self.lblMidi['text'] = 'scale.mid'
		else:
			if len(self.objSf.presets)>50: #most likely a GM set
				self.varPrevVoiceOnly.set(0)
				self.lblMidi['text'] = 'bossa.mid'
			else:
				self.varPrevVoiceOnly.set(1)
				if (len(self.objSf.presets)<4) and self.objSf.presets[0].presetNumber==0: #most likely a piano instrument
					self.lblMidi['text'] = 'grieg.mid'
				else:
					self.lblMidi['text'] = 'scale.mid'
				
	
	def __init__(self, root):
		root.title('SoundFont Information')
		
		frameMain = pack( Frame(root), side=TOP,fill=BOTH, expand=True)
		
		
		frameLoadSf = Frame(frameMain)
		Label(frameLoadSf, text='Soundfont').pack(side=LEFT)
		if quickEnter:
			self.txtSf = StringVar()
			pack(Entry(frameLoadSf, textvariable=self.txtSf ), side=LEFT,padx=5)
		else:
			self.lblCurrentSf = pack(Label(frameLoadSf ), side=LEFT,padx=5)
		
		Button(frameLoadSf,text='Load',command=self.loadSfDialog).pack(side=LEFT,padx=5)
		frameLoadSf.grid(row=0, column=0, columnspan=2)
		
		frameInfo = Frame(frameMain)
		self.lblInfoFields = {}
		for name in labelFieldNames:
			self.lblInfoFields[name] = pack(Label(frameInfo, text=name+': '),side=TOP,anchor='nw')
		frameInfo.grid(row=1, column=0)
		
		frameVoices = Frame(frameMain)
		Label(frameVoices, text='Voices').pack(side=TOP)
		self.lbVoices = pack(midirender_util.ScrolledListbox(frameVoices, width=45, height=9), side=TOP)
		frameVoices.grid(row=1, column=1)
		
		framePreview = Frame(frameMain)
		tupfontbtn = ('Verdana', 14, 'normal')
		Button(framePreview,text='Preview Soundfont', font=tupfontbtn, command=self.previewSf).pack()
		
		frameMidi = Frame(framePreview)
		self.lblMidi = pack(Label(frameMidi, text='scale.mid'), side=LEFT,fill=BOTH)
		Button(frameMidi,text='..',command=self.loadMid).pack(side=LEFT,padx=45)
		frameMidi.pack(side=TOP, anchor='w',pady=35)
		self.varPrevVoiceOnly = IntVar(); self.varPrevVoiceOnly.set(0)
		Checkbutton(framePreview, var=self.varPrevVoiceOnly, text='Preview only selected voice.').pack(side=TOP, anchor='w',pady=1)
		framePreview.grid(row=2, column=0, columnspan=2)
		
		frameMain.grid_columnconfigure(0, weight=1, minsize=20)
		frameMain.grid_columnconfigure(1, weight=1, minsize=20)
		frameMain.grid_rowconfigure(1, weight=1, minsize=20)
		
		
		self.objSf = None
	
		
	
	def loadSfDialog(self):
		if not quickEnter:
			filename = midirender_util.ask_openfile(title="Open SoundFont", types=['.sf2|SoundFont','.pat|Patch','.sbk|SoundFont1'])
			if not filename: return
		else:
			if not self.txtSf.get() or (self.objSf and self.txtSf.get() == self.objSf.filename):
				filename = midirender_util.ask_openfile(title="Open SoundFont", types=['.sf2|SoundFont','.pat|Patch','.sbk|SoundFont1'])
				if not filename: return
			else:
				filename = self.txtSf.get()
				if not os.path.exists(filename): midirender_util.alert("Couldn't find file."); return
					
		self.loadSf(filename)
		
	#other scripts can call this.
	def loadSf(self,filename):
		if quickEnter: self.txtSf.set(filename)
		else: self.lblCurrentSf['text'] = filename
		if filename.lower().endswith('.pat'):
			#gus (gravis ultrasound) patch
			namestart, nameend = os.path.split(filename)
			self.lbVoices.delete(0, END)
			self.lbVoices.insert(END, nameend)
			
			self.objSf = soundfontpreview_get.SoundFontInfo()
			self.objSf.type='pat'
			self.objSf.name = nameend
			self.objSf.filename = filename
		else:
			
			
			try:
				objSf = soundfontpreview_get.getpresets(filename, sfubardir)
			except soundfontpreview_get.SFInfoException, e:
				midirender_util.alert(str(e))
				return
				
			self.setLabels(objSf)
			self.lbVoices.delete(0, END)
			for preset in objSf.presets:
				self.lbVoices.insert(END, str(preset))
		
			self.objSf = objSf
			self.objSf.filename = filename
		
		self.autoSet()
		
	
	def loadMid(self):
		filename = midirender_util.ask_openfile(title="Choose midi song", types=['.mid|Midi'], initialfolder=sfubardir)
		if not filename: return
		self.lblMidi['text'] = filename
		
	def previewSf(self):
		if not self.objSf: return
		
		#check for mid, see if it exists
		curMid = self.lblMidi['text']
		if not os.path.exists(curMid):
			curMid = os.path.join(sfubardir, curMid)
		if not os.path.exists(curMid):
			midirender_util.alert("Couldn't find midi file.")
			return
		
		#get cfg data
		escapedfname = self.objSf.filename.replace('"','\\"')
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
		
		player = midirender_runtimidity.RenderTimidityMidiPlayer()
		player.setConfiguration(strCfg)
		player.playAsync(curMid)
			

#specify mydirectory if not running from the same folder as sfubar.exe.
#tested with sfubar 0.9.
# Because this program uses its debugging information, other sfubar versions may not work.
verbose = False
def getpresets(file):
	sfubar = sfubarPath
	if not os.path.exists(sfubar):
		raise SFInfoException('Could not find required file '+sfubarPath)
	if not os.path.exists(file):
		raise SFInfoException('Could not find the soundfont...')
		
	outtmpfilename = 'outtmp_sfinfo.txt'
	if os.path.exists(outtmpfilename): os.unlink('out.txt')
	if os.path.exists(outtmpfilename): raise SFInfoException("Couldn't delete %s file."%outtmpfilename)
		
	process = subprocess.Popen([sfubar, '--sfdebug', file, outtmpfilename])
	process.wait()
	
	try:
		f = open(outtmpfilename,'r')
	except IOError:
		raise SFInfoException("That soundfont might not exist. Couldn't open output file.")
	currentFont = SoundFontInfo()
	for line in f:
		if '=' in line:
			part1, part2 = line.strip().split('=',1)
			if part1=='RIFF.LIST.INAM.zstr':
				currentFont.name = part2
				if verbose: print 'Name: '+part2
			elif part1=='RIFF.LIST.ICRD.zstr':
				currentFont.date = part2
			elif part1=='RIFF.LIST.IENG.zstr':
				currentFont.author = part2
				if verbose: print 'By: '+part2
			elif part1=='RIFF.LIST.IPRD.zstr':
				currentFont.product = part2
			elif part1=='RIFF.LIST.ICOP.zstr':
				currentFont.copyright = part2
			elif part1=='RIFF.LIST.ICMT.zstr':
				currentFont.comment = part2
			else:
				if 'RIFF.LIST.phdr' in part1 and 'achPresetName' in part1:
					if part2=='EOP': 
						break
					if verbose: print '\n'+part2
						
					currentFont.presets.append( SoundFontInfoPreset() )
					currentFont.presets[-1].name=part2
				elif 'RIFF.LIST.phdr' in part1 and 'wPreset.' in part1:
					if verbose: print '\tProgram: '+part2
					currentFont.presets[-1].presetNumber= part2
				elif 'RIFF.LIST.phdr' in part1 and 'wBank.' in part1:
					if verbose:  print '\tBank: '+part2
					currentFont.presets[-1].bank= part2

	f.close()
	return currentFont
	
class SFInfoException(exceptions.Exception): pass
	
if __name__=='__main__':
	root = Tk()
	app = BSoundFontPreview(root)
	root.mainloop()

