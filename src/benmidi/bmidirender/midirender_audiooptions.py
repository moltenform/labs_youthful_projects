
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
		# this is a simple check to check that all options are looked at, not needed in production.
		if False:
			print('checking')
			if 'useold' in self.mapIdToOptionAndVariable and not self.get('useold'):
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
		
		# options that aren't as obscure:
		self.options.addOption('bitdepth', 'Output quality:', [('16-bit', 16), ('24-bit', 24)])
		self.options.addOption('AmpTotal', 'Amplify all notes by %', 'Int')
		self.options.addOption('AmpDrums', 'Amplify percussion by %', 'Int')
		self.options.addOption('maxpolyphony', 'Max polyphony', 'Bool', initialval=True)
		self.options.addOption('PatchesTakePrecedence', 'Patches have priority over soundfonts', 'Bool')
		if sys.platform == 'win32':
			self.options.addOption('useold', 'Use old timidity version (requires .cfg and/or .pat)', 'Bool')
			self.options.addOption('bypasstimidity', 'Bypass all settings and send MIDI to OS', 'Bool')
		
		# room/reverb settings
		self.options.addOption('delay', 'Stereo delay type:', [('(default)', None), ('Off', 'd'), ('Left delay', 'l'), ('Right delay', 'r'), ('Rotate', 'b')])
		self.options.addOption('delayAmount', 'Stereo delay (milliseconds):', 'Int')
		self.options.addOption('chorus', 'Chorus type:', [('(default)', None), ('Disabled', 'd'), ('Enabled', 'n'), ('Surround', 's')])
		self.options.addOption('chorusAmount', 'Chorus strength %:', 'Int', lbound=0, rbound=100)
		self.options.addOption('reverb', 'Reverb type:', [('(default)', None), ('Disabled', 'd'), ('Older Reverb', 'n'), ('Older Reverb, Global', 'g'), ('Newer Reverb', 'f'), ('Newer Reverb, Global', 'G')])
		self.options.addOption('reverbAmount', 'Reverb strength %:', 'Int', lbound=0, rbound=100)
		self.options.addOption('reverbPseudo', 'Global pseudo reverb (1-5000):', 'Int')
		self.options.addOption('lpf', 'Low pass filter:', [('(default)', None), ('Off', 'd'), ('12dB / oct', 'c'), ('24dB / oct', 'm')])
		self.options.addOption('fastdecay', 'Fast decay', [('(default)', None), ('Off', False), ('On', True)])
		
		# advanced settings
		self.options.addOption('antialias', 'Antialias', [('(default)', None), ('Off', False), ('On', True)])
		self.options.addOption('volcurve', 'Power of volume curve, %', 'Int')
		self.options.addOption('points', 'Interpolation points (0-34, default=25)', 'Int')
		self.options.addOption('controlratio', 'Control ratio, smaller for better quality', 'Int')
		self.options.addOption('AdditionalCmd', 'Custom cmdline, delimit by |', 'Str', width=35)
		self.options.addOption('AdditionalCfg', 'Custom cfg, delimit by |', 'Str', width=35)
		
		# decided not to include:
		# sample rate, no reason not to use 44100
		# stereo/mono, no reason not to use stereo
		# mindecay, --decay-time= made no difference
		# noiseshaping, default of 4 seems the best for -EFns=n Enable the nth degree (type) noise shaping filter
		# pure-intonation
		# frequency tables
		
		self.createUI(top)
	
	def getOptionsListImpl(self, includeRenderOptions=False):
		ret = []
		allowNotSeen=dict(useold=1, bitdepth=1, AdditionalCfg=1, bypasstimidity=1, PatchesTakePrecedence=1)
		if includeRenderOptions:
			bitdepth = self.options.get('bitdepth')
			if self.getUseOldTimidity(): # old Timidity doesn't support 24bit output
				bitdepth = max(bitdepth, 16)
			
			opt = '-Ow'
			opt += 'S' # use stereo
			opt += 's' # signed
			if bitdepth == 8:
				opt += '8' # 8-bit audio
			elif bitdepth == 24:
				opt += '2' # 24-bit audio
			else:
				opt += '1' # 16-bit audio
			
			opt += 'l' # PCM encoding
			ret.append(opt)
			ret.append('-s')
			ret.append('44100')
			
		def fromPercentage(optid, map100to, truncate=True):
			if self.options.get(optid) is None:
				return None
			else:
				val = map100to*(self.options.get(optid)/100.0)
				return int(val) if truncate else val
		
		if self.getUseOldTimidity():
			if self.options.get('antialias') is not None and self.options.get('antialias'):
				ret.append('-a')
			if self.options.get('fastdecay') is not None and not self.options.get('fastdecay'):
				ret.append('-f')
			if self.options.get('AmpTotal') is not None:
				ret.append('-A')
				ret.append(str(self.options.get('AmpTotal')))
			if self.options.get('maxpolyphony'):
				ret.append('-p')
				ret.append('48') #maximum supported value
		else:
			if self.options.get('antialias') is not None:
				ret.append('--anti-alias' if self.options.get('antialias') else '--no-anti-alias')
			if self.options.get('fastdecay') is not None:
				ret.append('--fast-decay' if self.options.get('fastdecay') else '--no-fast-decay')
			if self.options.get('maxpolyphony'):
				ret.append('--no-polyphony-reduction')
			if self.options.get('AmpTotal') is not None:
				ret.append('--volume=%d' % fromPercentage('AmpTotal', 70.0))
			if self.options.get('AmpDrums') is not None:
				ret.append('--drum-power=%d' % fromPercentage('AmpDrums', 100.0))

			if self.options.get('delay') is not None:
				opt = '--delay=' + self.options.get('delay')
				if self.options.get('delayAmount') is not None:
					opt += ',%d' % self.options.get('delayAmount')
				ret.append(opt)
			
			if self.options.get('chorus') is not None:
				opt = '--chorus=' + self.options.get('chorus')
				if self.options.get('chorusAmount') is not None:
					opt += ',%d' % fromPercentage('chorusAmount', 127.0)
				ret.append(opt)
			
			if self.options.get('reverb') is not None:
				opt = '--reverb=' + self.options.get('reverb')
				if self.options.get('reverbAmount') is not None:
					opt += ',%d' % fromPercentage('reverbAmount', 127.0)
				ret.append(opt)
			
			if self.options.get('reverbPseudo') is not None:
				ret.append('-R')
				ret.append('%d'%self.options.get('reverbPseudo'))
			
			if self.options.get('lpf') is not None:
				ret.append('--voice-lpf=%s'%self.options.get('lpf'))
			
			if self.options.get('volcurve') is not None:
				ret.append('--volume-curve=%f'%fromPercentage('volcurve', 1.661, truncate=False))
			
			if self.options.get('points') is not None:
				ret.append('--interpolation=%d'%self.options.get('points'))
			
			if self.options.get('controlratio') is not None:
				ret.append('--control-ratio=%d'%self.options.get('controlratio'))
			
		ret.extend(self.getAdditionalCfg('AdditionalCmd'))
		self.options.confirmSeen(allowNotSeen)
		return ret
	
	def getOptionsList(self, includeRenderOptions=False):
		try:
			return self.getOptionsListImpl(includeRenderOptions)
		except:
			e = sys.exc_info()
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
		if sys.platform == 'win32':
			return self.options.get('useold')
		else:
			return False
	
	def getBypassTimidity(self):
		if sys.platform == 'win32':
			return self.options.get('bypasstimidity')
		else:
			return False
		
	def destroy(self):
		self.top.destroy()
