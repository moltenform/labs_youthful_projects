
from Tkinter import *
import sys
import midirender_util

def pack(o, **kwargs): 
	o.pack(**kwargs)
	return o

class AudioOptionManager(object):
	def __init__(self):
		self.mapIdToOptionAndVariable = {}
		self.options = []
	
	def addOption(self, *args, **kwargs):
		option = AudioOption(*args, **kwargs)
		assert option.id not in self.mapIdToOptionAndVariable, 'duplicate ' + option.id
		self.mapIdToOptionAndVariable[option.id] = None
		self.options.append(option)
	
	def get(self, optionid):
		opt, var, seen = self.mapIdToOptionAndVariable[optionid]
		self.mapIdToOptionAndVariable[optionid][2] = True # mark that we've looked up this option
		if opt.vals == 'Str':
			return var.get()
		elif opt.vals == 'Bool':
			return not not var.get()
		elif opt.vals == 'Int':
			v = var.get()
			if not len(v.strip()):
				return None
			try:
				n = int(v)
			except:
				raise AssertionError('needed a number for %s but got %s' % (opt.caption, v))
			if n < opt.lbound:
				raise AssertionError('number for %s needs to be at least %s' % (opt.caption, opt.lbound))
			if n > opt.rbound:
				raise AssertionError('number for %s needs to be smaller than %s' % (opt.caption, opt.rbound))
			return n
		else:
			v = var.get()
			for i in range(len(opt.vals)):
				if opt.vals[i][0] == v:
					return opt.vals[i][1]
			raise AssertionError('unknown value, got %s and expected one of %s' % (v, opt.vals))
	
	def createUI(self, parent):
		for opt in self.options:
			if opt.vals == 'Int' or opt.vals == 'Str':
				var = StringVar()
				var.set(opt.initialval)
				frame = pack(Frame(parent))
				Label(frame, text=opt.caption).pack(side=LEFT)
				Entry(frame, textvariable=var, width=opt.width).pack(side=LEFT)
			elif opt.vals == 'Bool':
				var = IntVar()
				var.set(1 if opt.initialval else 0)
				Checkbutton(parent, variable=var, text=opt.caption).pack()
			else:
				var = StringVar()
				var.set(opt.initialval if opt.initialval else opt.vals[0][0])
				frame = pack(Frame(parent))
				Label(frame, text=opt.caption).pack(side=LEFT)
				OptionMenu(frame, var, *tuple(item[0] for item in opt.vals)).pack(side=LEFT)
			
			self.mapIdToOptionAndVariable[opt.id] = [opt, var, False]
	
	def confirmSeen(self, allowNotSeen):
		print('checking')
		if not self.get('useold'):
			for key in self.mapIdToOptionAndVariable:
				if not key in allowNotSeen:
					if not self.mapIdToOptionAndVariable[key][2]:
						print('Internal note: option %s not checked' % key)

class AudioOption(object):
	def __init__(self, optionid, caption, vals, initialval='', width=10, lbound=0, rbound=1e10):
		self.id = optionid
		self.caption = caption
		self.initialval = initialval
		self.width = width
		self.vals = vals
		self.lbound = lbound
		self.rbound = rbound

class WindowAudioOptions(object):
	def __init__(self, top, callbackOnClose=None):
		top.title('Audio Options')
		
		if callbackOnClose!=None:
			def callCallback():
				callbackOnClose()
				top.destroy()
			top.protocol("WM_DELETE_WINDOW", callCallback)
		
		self.top = top
		self.options = AudioOptionManager()
		self.options.addOption('bitdepth', 'Wav quality:', [('8-bit', 8), ('16-bit', 16), ('24-bit', 24)], initialval='16-bit')
		if sys.platform == 'win32':
			self.options.addOption('useold', 'Old timidity (note: supports .cfg only)', 'Bool')
		
		self.options.addOption('fastdecay', 'Fast decay', [('(default)', None), ('Off', False), ('On', True)])
		self.options.addOption('maxpolyphony', 'Max polyphony', 'Bool', initialval=True)
		self.options.addOption('PsReverb', 'Pseudo reverb (milliseconds):', 'Int')
		self.options.addOption('AmpTotal', 'Amplify all notes (default=100):', 'Int')
		self.options.addOption('AmpDrums', 'Amplify percussion (default=100):', 'Int')
		self.options.addOption('lpf', 'Low pass filter:', [('(default)', None), ('Off', 'd'), ('12dB / oct', 'c'), ('24dB / oct', 'm')])
		self.options.addOption('delay', 'Stereo delay:', [('Off', 'd'), ('Left delay', 'l'), ('Right delay', 'r'), ('Rotate', 'b')])
		self.options.addOption('reverb', 'Reverb type:', [('(default)', None), ('Off', 'd'), ('Enable Normal', 'n'), ('Global Normal', 'g'), ('Enable New', 'f'), ('Global New', 'G')])
		self.options.addOption('points', 'Interpolation points:', [('(default)', None), ('1', '1'), ('4', '4'), ('7', '7'), ('10', '10'), ('13', '13'), ('16', '16'), ('19', '19'), ('22', '22'), ('25 (normal)', '25'), ('28', '28'), ('31', '31'), ('34', '34')])
		self.options.addOption('PatchesTakePrecedence', 'Patches have priority over soundfonts', 'Bool')
		self.options.addOption('AdditionalCmd', 'Custom cmdline, delimit by |', 'Str', width=35)
		self.options.addOption('AdditionalCfg', 'Custom cfg, delimit by |', 'Str', width=35)
		self.createUI(top)
	
	def createTimidityOptionsListImpl(self, includeRenderOptions=False):
		ret = []
		
		allowNotSeen=dict(useold=1, AdditionalCfg=1, PatchesTakePrecedence=1)
		if includeRenderOptions:
			bitdepth = self.options.get('bitdepth')
			opt = '-Ow'
			opt +='S' # use stereo
			opt += 's' # signed
			if bitdepth==8: opt+='8' #8-bit audio
			elif bitdepth==24: opt+='2' if self.getUseOldTimidity() else '1' #24 bit
			else: opt+='1' #16 bit
			opt+='l' # PCM encoding
			ret.append(opt)
			ret.append('-s')
			ret.append('44100')
		else:
			allowNotSeen['bitdepth'] = 1
		
		
		if self.getUseOldTimidity():
			# also supports:  -a   Enable the antialiasing filter
			if self.options.get('fastdecay') is not None and not self.options.get('fastdecay'):
				ret.append('-f')
			if self.options.get('AmpTotal') is not None:
				ret.append('-A')
				ret.append(str(ampTotal))
			if self.options.get('maxpolyphony'):
				ret.append('-p')
				ret.append('48') #maximum supported value
		else:
			ampTotal = int(70.0*(self.options.get('AmpTotal')/100.0)) if self.options.get('AmpTotal') is not None else None
			ampDrums = int(100.0*(self.options.get('AmpDrums')/100.0)) if self.options.get('AmpDrums') is not None else None
			if ampTotal is not None or ampDrums is not None:
				ampOption = '-A'
				if ampTotal != None:
					ampOption+=str(ampTotal)
				if ampDrums!=None:
					ampOption+=','+str(ampDrums)
				ret.append(ampOption)
			if self.options.get('fastdecay') is not None:
				if self.options.get('fastdecay'):
					ret.append('--fast-decay')
				else:
					ret.append('--no-fast-decay')
			if self.options.get('maxpolyphony'):
				ret.append('--no-polyphony-reduction')
			if self.options.get('points') is not None:
				ret.append('--interpolation=%s'%self.options.get('points'))
			
		ret.extend(self.getAdditionalCfg('AdditionalCmd'))
		self.options.confirmSeen(allowNotSeen)
		return ret
			
		#~ todo: enable these.
		#~ arParams.append('-R')
		#~ arParams.append('%d'%psreverb)
		#~ arParams.append('--voice-lpf')
		#~ arParams.append('%s'%lpf)
		#~ arParams.append('--delay')
		#~ arParams.append('%s'%delay)
		#~ arParams.append('--reverb')
		#~ arParams.append('%s'%reverb)
	
	def createTimidityOptionsList(self, includeRenderOptions=False):
		return self.createTimidityOptionsListImpl(includeRenderOptions)
		try:
			return self.createTimidityOptionsListImpl(includeRenderOptions)
		except:
			e = sys.exc_info()[0]
			midirender_util.alert('Alert: ' + str(e))
			return None
	
	def createUI(self, top):
		frameTop = Frame(top, height=600)
		frameTop.pack(expand=YES, fill=BOTH)
		self.frameTop=frameTop
		frameOpts = pack(LabelFrame(frameTop, text='Audio Options'), expand=YES, fill=BOTH)
		self.options.createUI(frameOpts)
		Label(frameTop, text='Settings remain in effect while this dialog is open.' + ' '*15).pack(pady=5)
	
	def getAdditionalCfg(self, field='AdditionalCfg'):
		additionalCmd = self.options.get(field)
		if not additionalCmd:
			return []
		else:
			return additionalCmd.split('|')
			
	def getPatchesTakePrecedence(self):
		return self.options.get('PatchesTakePrecedence')
	
	def getUseOldTimidity(self):
		return self.options.get('useold')
		
	def destroy(self):
		self.top.destroy()
