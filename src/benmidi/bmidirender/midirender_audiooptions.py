

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
	
	def getOptValue(self, name):
		s = self.optVariables[name][0].get() #rendered name
		return self.optVariables[name][1].get(s, None)
	
	def __init__(self, top, callbackOnClose=None):
		#should only display tracks with note events. that way, solves some problems
		top.title('Audio Options')
		self.optVariables = {}
		
		frameTop = Frame(top, height=600)
		frameTop.pack(expand=YES, fill=BOTH)
		self.frameTop=frameTop
		
		frameAudioOpts = pack(LabelFrame(frameTop, text='Output Format'), expand=YES, fill=BOTH)
		
		self.varStereo=IntVar(); self.varStereo.set(1)
		Checkbutton(frameAudioOpts, variable=self.varStereo, text='Stereo').pack()
		
		self.makeOpts('sample', frameAudioOpts, 'Sampling rate:',
				['22,050Hz','44,100Hz'], [22050,44100], 1)
		
		#self.makeOpts('bitrate', frameAudioOpts, 'Quality:',
		#		['8-bit', '16-bit','24-bit'], [8,16,24], 1)
		#8 bit audio seems to make a hissing sound. Perhaps a signed/unsigned issue...
		#The 24bit audio seems to be correct, although it is not read by Windows Media player, only Audacity
		
		self.makeOpts('bitrate', frameAudioOpts, 'Quality:',
				['16-bit','24-bit'], [16,24], 0)
		
		frameTimOpts = pack(LabelFrame(frameTop, text='Audio Settings'), expand=YES, fill=BOTH)
		self.varFastDecay=IntVar(); self.varFastDecay.set(0)
		Checkbutton(frameTimOpts, variable=self.varFastDecay, text='Fast decay mode').pack()
		
		framePsReverb = pack(Frame(frameTimOpts))
		Label(framePsReverb, text='Pseudo reverb:').pack(side=LEFT)
		self.varPsReverb = StringVar()
		self.varPsReverb.set('0')
		en = Entry(framePsReverb, textvariable=self.varPsReverb, width=15)
		en.pack(side=LEFT)
		
		
		self.makeOpts('lpf', frameTimOpts, 'Low pass filter:',
				['Off', '12dB / oct','24dB / oct'], ['d','c','m'], 1)
		
		self.makeOpts('delay', frameTimOpts, 'Stereo delay:',
				['Off', 'Left delay','Right delay','Rotate'], ['d','l','r','b'], 0)
		
		self.makeOpts('reverb', frameTimOpts, 'Reverb:',
				['Off', 'Enable Normal','Global Normal','Enable New','Global New'], ['d','n','g','f','G'], 3)
		
		
		Label(frameTop, text='Settings remain in effect while this dialog is open.'+' '*15).pack(pady=5)
		#8bit is unsigned, rest should be signed
		

		if callbackOnClose!=None:
			def callCallback():
				callbackOnClose()
				top.destroy()
			top.protocol("WM_DELETE_WINDOW", callCallback)
		self.top = top
	def destroy(self):
		self.top.destroy()
	
	
	def createTimidityOptionsList(self, includeRenderOptions=False):
		sample = self.getOptValue('sample')
		bitrate = self.getOptValue('bitrate')
		lpf = self.getOptValue('lpf')
		delay = self.getOptValue('delay')
		reverb = self.getOptValue('reverb')
		try: psreverb = int(self.varPsReverb.get()); valid = psreverb >= 0 and psreverb<=1000
		except: valid=False
		if not valid: midirender_util.alert('psuedo reverb must be an integer 0 to 1000'); return None
		
		stereo =self.varStereo.get()
		decay =self.varFastDecay.get()
		
		res = ''
		if includeRenderOptions:
			res+= ' -Ow'
			if stereo: res+='S' #stereo or mono
			else: res+='M'
			
			# if bitrate==8: res+='u' #8-bit wavs are typically unsigned
			# else: res+= 's'
			res+= 's'
				
			if bitrate==8: res+='8' #8-bit audio
			elif bitrate==24: res+='2' #24 bit
			else: res+='1' #16 bit
				
			res+='l' #l for linear encoding
			
			res+=' -s %d'%sample #sampling rate, 44100 or 22050

		if decay: res+=' -f'
		res += ' -R %d'%psreverb
		res += ' --voice-lpf %s'%lpf
		res += ' --delay %s'%delay
		res += ' --reverb %s'%reverb
		
		arParams = res.split(' ')
		return arParams
		

if __name__=='__main__':
	
	
	root = Tk()

	
	
	app = BTimidityOptions(root)
	
	def callback():
		print app.createTimidityOptionsString(True)
	
	Button(app.frameTop, command=callback, text='((test))').pack()
	root.mainloop()
	
	

