

from Tkinter import *
import midirender_util



def pack(o, **kwargs): o.pack(**kwargs); return o
class BTimidityOptions():
	def makeOpts(self, name, parent, label, arOptions, arOptionValues, defaultIndex=0):
		assert len(arOptions)==len(arOptionValues)
		d = {}
		for i in range(len(arOptions)):
			d[arOptions[i]] = arOptionValues[i]
			
		var = StringVar()
		var.set( arOptions[defaultIndex] )
		self.optVariables[name] = (var, d)
		#now draw it
		frame = pack(Frame(parent))
		Label(frame, text=label).pack(side=LEFT)
		OptionMenu(frame, var, *tuple(arOptions)).pack(side=LEFT)
		
	def makeStringEntry(self, name, parent, caption, width, defaultval):
		self.optEntryValVariables[name] = StringVar()
		self.optEntryValVariables[name].set(defaultval)
		
		frameTmp = pack(Frame(parent))
		Label(frameTmp, text=caption).pack(side=LEFT)
		en = Entry(frameTmp, textvariable=self.optEntryValVariables[name], width=width)
		en.pack(side=LEFT)
	
	def getOptValue(self, name):
		s = self.optVariables[name][0].get() #rendered name
		return self.optVariables[name][1].get(s, None)
	
	def getStringEntryVal(self, name):
		return self.optEntryValVariables[name].get()
	
	def __init__(self, top, callbackOnClose=None):
		#should only display tracks with note events. that way, solves some problems
		top.title('Audio Options')
		self.optVariables = {}
		self.optEntryValVariables = {}
		
		frameTop = Frame(top, height=600)
		frameTop.pack(expand=YES, fill=BOTH)
		self.frameTop=frameTop
		
		frameAudioOpts = pack(LabelFrame(frameTop, text='Output Format'), expand=YES, fill=BOTH)
		
		self.varStereo=IntVar(); self.varStereo.set(1)
		Checkbutton(frameAudioOpts, variable=self.varStereo, text='Stereo').pack()
		
		self.makeOpts('sample', frameAudioOpts, 'Sampling rate:',
				['22,050Hz','44,100Hz'], [22050,44100], 1)
				
		self.makeOpts('bitrate', frameAudioOpts, 'Quality:',
				['16-bit','24-bit'], [16,24], 0)
		
		frameTimOpts = pack(LabelFrame(frameTop, text='Audio Settings'), expand=YES, fill=BOTH)
		self.varFastDecay=IntVar(); self.varFastDecay.set(0)
		Checkbutton(frameTimOpts, variable=self.varFastDecay, text='Fast decay mode').pack()
		
		self.varNoPolyReduction=IntVar(); self.varNoPolyReduction.set(1)
		Checkbutton(frameTimOpts, variable=self.varNoPolyReduction, text='Max polyphony').pack()
		
		self.makeStringEntry('PsReverb', frameTimOpts, 'Pseudo reverb (0-800ms):', 15, '')
		self.makeStringEntry('AmpTotal', frameTimOpts, 'Amplify:', 15, '100')
		self.makeStringEntry('AmpDrums', frameTimOpts, 'Amplify Percussion:', 15, '100')
		
		self.makeOpts('lpf', frameTimOpts, 'Low pass filter:',
				['Off', '12dB / oct','24dB / oct'], ['d','c','m'], 1)
		
		self.makeOpts('delay', frameTimOpts, 'Stereo delay:',
				['Off', 'Left delay','Right delay','Rotate'], ['d','l','r','b'], 0)
		
		self.makeOpts('reverb', frameTimOpts, 'Reverb:',
				['Off', 'Enable Normal','Global Normal','Enable New','Global New'], ['d','n','g','f','G'], 3)
		
		self.makeOpts('interpolationpoints', frameTimOpts, 'Interpolation points:',
				['1', '4', '7', '10', '13', '16', '19', '22', '25', '28', '31', '34'], ['1', '4', '7', '10', '13', '16', '19', '22', '25', '28', '31', '34'], 8)
		
		self.varPatchesTakePrecedence=IntVar(); self.varPatchesTakePrecedence.set(0)
		Checkbutton(frameTimOpts, variable=self.varPatchesTakePrecedence, text='Patches priority over sondfont').pack()
		
		self.makeStringEntry('AdditionalCmd', frameTimOpts, 'Added cmdline, delimit by |', 35, '')
		self.makeStringEntry('AdditionalCfg', frameTimOpts, 'Added cfg, delimit by |', 35, '')
		
		Label(frameTop, text='Settings remain in effect while this dialog is open.'+' '*15).pack(pady=5)
		

		if callbackOnClose!=None:
			def callCallback():
				callbackOnClose()
				top.destroy()
			top.protocol("WM_DELETE_WINDOW", callCallback)
		self.top = top
	
	def getAdditionalCfg(self, field='AdditionalCfg'):
		additionalCmd = self.getStringEntryVal(field)
		if not additionalCmd:
			return []
		else:
			return additionalCmd.split('|')
			
	def getPatchesTakePrecedence(self):
		return not not self.varPatchesTakePrecedence.get()
		
	def destroy(self):
		self.top.destroy()
	
	def getVariableValueAsIntOrNoneIfDefault(self, val, varname, min, max, defaultValueMapToNone):
		if val == '' or val==defaultValueMapToNone:
			return None
		try: val = int(val); valid = val >= min and val<=max
		except: valid=False
		if not valid:
			midirender_util.alert('%s must be an integer %d to %d' % (varname, min, max))
			return None
		else:
			return val
	
	def createTimidityOptionsList(self, includeRenderOptions=False):
		sample = self.getOptValue('sample')
		bitrate = self.getOptValue('bitrate')
		lpf = self.getOptValue('lpf')
		delay = self.getOptValue('delay')
		reverb = self.getOptValue('reverb')
		interpolationpoints = self.getOptValue('interpolationpoints')
		
		psreverb = self.getVariableValueAsIntOrNoneIfDefault(self.getStringEntryVal('PsReverb'), 'psuedo reverb', 0, 1000, '')
		ampTotal = self.getVariableValueAsIntOrNoneIfDefault(self.getStringEntryVal('AmpTotal'), 'Amplify', 0, 1000, '100')
		ampDrums = self.getVariableValueAsIntOrNoneIfDefault(self.getStringEntryVal('AmpDrums'), 'Amplify percussion', 0, 1000, '100')
		additionalCmd = self.getAdditionalCfg('AdditionalCmd')
		
		if ampTotal != None:
			ampTotal = int(70.0*(ampTotal/100.0))
			ampTotal = min(max(ampTotal, 0), 800) 
			
		if ampDrums != None:
			ampDrums = int(100.0*(ampDrums/100.0))
			ampDrums = min(max(ampDrums, 0), 800) 
		
		stereo = self.varStereo.get()
		decay = self.varFastDecay.get()
		
		arParams = []
		if includeRenderOptions:
			renderOutputOption = '-Ow'
			if stereo: renderOutputOption+='S' #stereo or mono
			else: renderOutputOption+='M'
			
			renderOutputOption+= 's'
				
			if bitrate==8: renderOutputOption+='8' #8-bit audio
			elif bitrate==24: renderOutputOption+='2' #24 bit
			else: renderOutputOption+='1' #16 bit
				
			renderOutputOption+='l' #l for PCM encoding
			arParams.append(renderOutputOption)
			arParams.append('-s')
			arParams.append('%d'%sample) #sampling rate, 44100 or 22050

		if decay:
			arParams.append('-f')
		if psreverb != None:
			arParams.append('-R')
			arParams.append('%d'%psreverb)
		
		if self.varNoPolyReduction.get(): 
			arParams.append('--no-polyphony-reduction')
			
		if ampTotal != None or ampDrums!=None:
			ampOption = '-A'
			if ampTotal != None:
				ampOption+=str(ampTotal)
			if ampDrums!=None:
				ampOption+=','+str(ampDrums)
			arParams.append(ampOption)
			
		if interpolationpoints and interpolationpoints!='25':
			arParams.append('--interpolation=%s'%interpolationpoints)
		
		arParams.append('--voice-lpf')
		arParams.append('%s'%lpf)
		arParams.append('--delay')
		arParams.append('%s'%delay)
		arParams.append('--reverb')
		arParams.append('%s'%reverb)
		arParams.extend(additionalCmd)
		
		return arParams
		

if __name__=='__main__':
	
	
	root = Tk()

	
	
	app = BTimidityOptions(root)
	
	def callback():
		print app.createTimidityOptionsString(True)
	
	Button(app.frameTop, command=callback, text='((test))').pack()
	root.mainloop()
	
	

