from Tkinter import *
import tkFileDialog

from yalp.wave_file import *
from yalp.wave_fx import *
from yalp.wave_synthesis import *
from yalp import wave_system

class App:
	canvasW = 400
	canvasH = 100
	audiodata = None
	def __init__(self, root):
		root.title('Minieditor - Powered by Yalp')
		frTop = Frame(root)
		frTop.pack(side=TOP)
		self.canvas = Canvas(frTop, width=self.canvasW+5, height=self.canvasH+5, bg='white')
		self.canvas.pack(expand = YES, fill=BOTH)
		self.lblAudioInfo = Label(frTop, text='')
		self.lblAudioInfo.pack(side=TOP)
		
		Label(frTop, text='Manual Synthesis:').pack(side=TOP)
		frManSynth1 = Frame(root)
		Label(frManSynth1, text='Pitch 1:').pack(side=LEFT)
		self.fldFreq1 = Entry(frManSynth1, width=10)
		self.fldFreq1.pack(side=LEFT)
		self.fldFreq1.insert(0,'440')
		Label(frManSynth1, text='Harmonic weights:').pack(side=LEFT)
		self.fldWeights1 = Entry(frManSynth1, width=40)
		self.fldWeights1.pack(side=LEFT)
		self.fldWeights1.insert(0,'1.0')
		frManSynth1.pack(side=TOP)
		
		frManSynth2 = Frame(root)
		Label(frManSynth2, text='Pitch 2:').pack(side=LEFT)
		self.fldFreq2 = Entry(frManSynth2, width=10)
		self.fldFreq2.pack(side=LEFT)
		Label(frManSynth2, text='Harmonic weights:').pack(side=LEFT)
		self.fldWeights2 = Entry(frManSynth2, width=40)
		self.fldWeights2.pack(side=LEFT)
		frManSynth2.pack(side=TOP)
		
		Button(root, text="Synth", command=self.manual_synth).pack(side=BOTTOM)
		self.add_menus(root)
		
	
	def add_menus(self, root):
		root.bind('<Control-Key-n>', Callable(self.new_audio))
		root.bind('<Control-Key-r>', Callable(self.record_audio))
		root.bind('<Control-Key-o>', Callable(self.open_audio))
		root.bind('<Control-Key-s>', Callable(self.save_audio))
		root.bind('<Control-Key-b>', Callable(self.play_audio))
		
		menubar = Menu(root)
		menuFile = Menu(menubar, tearoff=0)
		menuFile.add_command(label="New", command=self.new_audio, underline=0, accelerator='Ctrl+N')
		menuFile.add_command(label="New 16bit 44", command=Callable(self.new_audio,1), underline=3)
		menuFile.add_command(label="Open", command=self.open_audio, underline=0, accelerator='Ctrl+O')
		menuFile.add_command(label="Save As", command=self.save_audio, underline=0, accelerator='Ctrl+S')
		menuFile.add_separator()
		menuFile.add_command(label="Exit", command=root.quit, underline=1)
		menubar.add_cascade(label="File", menu=menuFile, underline=0)
		
		menuAudio = Menu(menubar, tearoff=0)
		menuAudio.add_command(label="Play", command=self.play_audio, accelerator='Ctrl+B')
		menuAudio.add_command(label="Record", command=self.record_audio, accelerator='Ctrl+R')
		menuAudio.add_separator()
		menuAudio.add_command(label="Silence Before", command=self.silence_before)
		menuAudio.add_command(label="Silence After", command=self.silence_after)
		menuAudio.add_command(label="Chop", command=self.chop_audio)
		menuAudio.add_command(label="Append...", command=self.append_audio)
		menuAudio.add_command(label="Mix...", command=self.mix_audio)
		menuAudio.add_command(label="Modulate...", command=self.modulate_audio)
		menuAudio.add_separator()
		menuAudio.add_command(label="Louder", command=Callable(self.amp_audio, 2.0))
		menuAudio.add_command(label="Softer", command=Callable(self.amp_audio, 0.5))
		menubar.add_cascade(label="Audio", menu=menuAudio, underline=0)
		
		menuSynth = Menu(menubar, tearoff=0)
		synthInstruments = get_instruments()
		for synth in synthInstruments:
			if synth == '_':
				menuSynth.add_separator()
			else:
				menuSynth.add_command(label=synth, command=Callable(self.audio_synth, synth))
		menubar.add_cascade(label="Synthesize", menu=menuSynth, underline=0)
		
		menuEffects = Menu(menubar, tearoff=0)
		aeffects = fx_get_effects()
		for effect in aeffects:
			if effect == '_':
				menuEffects.add_separator()
			else:
				menuEffects.add_command(label=effect[0], command=Callable(self.audio_fx, effect[1]))
		menubar.add_cascade(label="FX", menu=menuEffects, underline=1)
		
		root.config(menu=menubar)
		
	def set_status(self, txt):
		self.lblAudioInfo.config(text=txt)
		self.lblAudioInfo.update()
	
	def manual_synth(self):
		freq1 = float(self.fldFreq1.get())
		scaleAll = 0.5 if self.fldFreq2.get() != '' else 1.0
		
		weights = self.fldWeights1.get().split(',')
		sumweights = sum(map(float, weights))
		
		w1 = [ scaleAll * float(weight)/sumweights for weight in weights if weight!=0]  # weights are relative - 1,1,1,1 should be 0.25,0.25,0.25,0.25
		harmonicdata1 = [ [freq1*(i+1), w1[i]] for i in xrange(len(weights))]
		
		if self.fldFreq2.get() == '':
			manualsynth(self.audiodata, 2.0 * self.audiodata.nSampleRate, harmonicdata1)
		else:
			print 'yep'
			freq2 = float(self.fldFreq2.get())
			weights2 = self.fldWeights2.get().split(',')
			sumweights2 = sum(map(float, weights2))
			
			w2 = [ scaleAll * float(weight)/sumweights2 for weight in weights2 if weight!=0] 
			harmonicdata2 = [ [freq2*(i+1), w2[i]] for i in xrange(len(weights2))]
			harmonicdata1.extend(harmonicdata2)
			print len(harmonicdata1)
			manualsynth(self.audiodata, 2.0 * self.audiodata.nSampleRate, harmonicdata1)
		
		self.refresh()
	
	def silence_after(self):
		time = 1 # one second
		self.audiodata.add_silence(time)
		self.refresh()
	def silence_before(self):
		time = 1 # one second
		tmpsamples = self.audiodata.samples
		self.audiodata.clear_samples()
		self.audiodata.add_silence(time)
		self.audiodata.samples.extend(tmpsamples)
		self.refresh()
		
	def chop_audio(self):
		# cut length in half
		self.audiodata.truncate(len(self.audiodata.samples) / 2) 
		self.refresh_picture()
	def audio_synth(self, strInstrument):
		import random
		#~ pitch = random.choice([330])
		pitch = random.choice([440, 220, 330, 400, 330, 330, 220, 110])
		self.set_status('Synth...')
		synthesize(self.audiodata, strInstrument, pitch, 2.) #create 2 seconds of audio
		self.refresh()
		
	def audio_fx(self, fnEffect):
		self.set_status('Effect...')
		fnEffect(self.audiodata)
		self.refresh()
	def amp_audio(self, amt):
		self.audiodata.multiply(amt)
		self.refresh()
		
	def new_audio(self, quality=0):
		if quality==1:
			self.audiodata = WaveFile(nBits=16, nSampleRate = 2*22050)
		else:
			self.audiodata = WaveFile(nBits=8, nSampleRate = 22050)
			
		self.refresh()
	def open_audio(self):
		"""Load a file containing a class and script."""
		strFileName = tkFileDialog.askopenfilename(defaultextension='.wav', filetypes=[('Wave files','.wav')])
		if not strFileName: return
		try:
			newaudio = WaveFile(strFilename=strFileName)
		except:
			print 'Could not open file.'
			return
			
		self.audiodata = newaudio
		self.refresh()
	
	def save_audio(self):
		strFileName = tkFileDialog.asksaveasfilename(defaultextension='.wav', filetypes=[('Wave files','.wav')])
		if not strFileName: return
		try:
			self.audiodata.save_wave(strFileName)
		except:
			print 'Could not save file.'
			return
		print 'Saved.'
		
	def append_audio(self):
		strFileName = tkFileDialog.askopenfilename(defaultextension='.wav', filetypes=[('Wave files','.wav')])
		if not strFileName: return
		try:
			newaudio = WaveFile(strFilename=strFileName)
		except:
			print 'Could not open file.'
			return
		self.audiodata.append(newaudio)
		self.refresh()
		
	def mix_audio(self):
		strFileName = tkFileDialog.askopenfilename(defaultextension='.wav', filetypes=[('Wave files','.wav')])
		if not strFileName: return
		try:
			newaudio = WaveFile(strFilename=strFileName)
		except:
			print 'Could not open file.'
			return
		self.audiodata.addwave(newaudio,0.5)
		self.refresh()
		
	def modulate_audio(self):
		strFileName = tkFileDialog.askopenfilename(defaultextension='.wav', filetypes=[('Wave files','.wav')])
		if not strFileName: return
		try:
			newaudio = WaveFile(strFilename=strFileName)
		except:
			print 'Could not open file.'
			return
		
		fx_modulatewave(audiodata, newaudio,0.5)
		self.refresh()
	
	def play_audio(self): 
		wave_system.audio_play_memory(self.audiodata.wavedata())
	
	def record_audio(self):
		import time
		self.set_status('3')
		time.sleep(0.5)
		self.set_status('2')
		time.sleep(0.5)
		self.set_status('1')
		time.sleep(0.5)
		
		self.set_status('Recording...')
		wave_system.audio_record('temp.wav', 3.0, self.audiodata.sampleWidth, self.audiodata.nSampleRate)
		
		time.sleep(0.2)
		self.audiodata = WaveFile('temp.wav')
		self.refresh()
	
	def refresh(self):
		strText = 'Mono ' + str(self.audiodata.sampleWidth * 8) + 'bit, '+str(self.audiodata.nSampleRate) + 'Hz, ' + str(len(self.audiodata.samples)/float(self.audiodata.nSampleRate)) + 's'
		self.set_status(strText)
		self.refresh_picture()
		
	def refresh_picture(self):
		self.canvas.delete(ALL)
		width = self.canvasW
		sndwidth = len(self.audiodata.samples)
		if sndwidth==0:
			return
		chunksize = int(sndwidth/width)
		nchunks = sndwidth/chunksize
		z = self.canvasH / 2.
		
		for i in range(nchunks):
			level = (self.audiodata.get_level(i*chunksize, (i+1)*chunksize)) * (self.canvasH / 2.)
			self.canvas.create_line(i, z +level,i, z-level,  fill='red')
			
class Callable:
	def __init__(self, func, *args, **kwds):
		self.func = func
		self.args = args
		self.kwds = kwds
	def __call__(self, event=None):
		return apply(self.func, self.args, self.kwds)
	def __str__(self):
		return self.func.__name__

root = Tk()
app = App(root)
app.audiodata = WaveFile(nBits=8, nSampleRate=22050) #Create a small wave file to play with

root.mainloop()


