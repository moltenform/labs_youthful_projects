"""
BMidiRender
Ben Fisher, 2009, GPL
halfhourhacks.blogspot.com
"""

#midis are modified by:
	#Instrument changes
	#Mixer changes --relies on trackNumber, so don't reorder tracks! (includes transposition)
	#tempo changes
	#get excerpt changes
	
#the Restructuring change can only come first, not later.

from Tkinter import *

icon0='''R0lGODlhFAAUAPcAAAAAAIAAAACAAICAAAAAgIAAgACAgICAgMDAwP8AAAD/AP//AAAA//8A/wD//////wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMwAAZgAAmQAAzAAA/wAzAAAzMwAzZgAzmQAzzAAz/wBmAABmMwBmZgBmmQBmzABm/wCZAACZMwCZZgCZmQCZzACZ/wDMAADMMwDMZgDMmQDMzADM/wD/AAD/MwD/ZgD/mQD/zAD//zMAADMAMzMAZjMAmTMAzDMA/zMzADMzMzMzZjMzmTMzzDMz/zNmADNmMzNmZjNmmTNmzDNm/zOZADOZMzOZZjOZmTOZzDOZ/zPMADPMMzPMZjPMmTPMzDPM/zP/ADP/MzP/ZjP/mTP/zDP//2YAAGYAM2YAZmYAmWYAzGYA/2YzAGYzM2YzZmYzmWYzzGYz/2ZmAGZmM2ZmZmZmmWZmzGZm/2aZAGaZM2aZZmaZmWaZzGaZ/2bMAGbMM2bMZmbMmWbMzGbM/2b/AGb/M2b/Zmb/mWb/zGb//5kAAJkAM5kAZpkAmZkAzJkA/5kzAJkzM5kzZpkzmZkzzJkz/5lmAJlmM5lmZplmmZlmzJlm/5mZAJmZM5mZZpmZmZmZzJmZ/5nMAJnMM5nMZpnMmZnMzJnM/5n/AJn/M5n/Zpn/mZn/zJn//8wAAMwAM8wAZswAmcwAzMwA/8wzAMwzM8wzZswzmcwzzMwz/8xmAMxmM8xmZsxmmcxmzMxm/8yZAMyZM8yZZsyZmcyZzMyZ/8zMAMzMM8zMZszMmczMzMzM/8z/AMz/M8z/Zsz/mcz/zMz///8AAP8AM/8AZv8Amf8AzP8A//8zAP8zM/8zZv8zmf8zzP8z//9mAP9mM/9mZv9mmf9mzP9m//+ZAP+ZM/+ZZv+Zmf+ZzP+Z///MAP/MM//MZv/Mmf/MzP/M////AP//M///Zv//mf///////yH5BAEAAP4ALAAAAAAUABQAQAg7AP0JHEiwoEGCABIqXHiwoUODCR82XBhRosWHFAFc9JeRocOKG0OKHLnRY8mOFjuahIhS4kqSMGM+DAgAOw=='''
icon1='''R0lGODlhFAAUAPcAAAAAAIAAAACAAICAAAAAgIAAgACAgICAgMDAwP8AAAD/AP//AAAA//8A/wD//////wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMwAAZgAAmQAAzAAA/wAzAAAzMwAzZgAzmQAzzAAz/wBmAABmMwBmZgBmmQBmzABm/wCZAACZMwCZZgCZmQCZzACZ/wDMAADMMwDMZgDMmQDMzADM/wD/AAD/MwD/ZgD/mQD/zAD//zMAADMAMzMAZjMAmTMAzDMA/zMzADMzMzMzZjMzmTMzzDMz/zNmADNmMzNmZjNmmTNmzDNm/zOZADOZMzOZZjOZmTOZzDOZ/zPMADPMMzPMZjPMmTPMzDPM/zP/ADP/MzP/ZjP/mTP/zDP//2YAAGYAM2YAZmYAmWYAzGYA/2YzAGYzM2YzZmYzmWYzzGYz/2ZmAGZmM2ZmZmZmmWZmzGZm/2aZAGaZM2aZZmaZmWaZzGaZ/2bMAGbMM2bMZmbMmWbMzGbM/2b/AGb/M2b/Zmb/mWb/zGb//5kAAJkAM5kAZpkAmZkAzJkA/5kzAJkzM5kzZpkzmZkzzJkz/5lmAJlmM5lmZplmmZlmzJlm/5mZAJmZM5mZZpmZmZmZzJmZ/5nMAJnMM5nMZpnMmZnMzJnM/5n/AJn/M5n/Zpn/mZn/zJn//8wAAMwAM8wAZswAmcwAzMwA/8wzAMwzM8wzZswzmcwzzMwz/8xmAMxmM8xmZsxmmcxmzMxm/8yZAMyZM8yZZsyZmcyZzMyZ/8zMAMzMM8zMZszMmczMzMzM/8z/AMz/M8z/Zsz/mcz/zMz///8AAP8AM/8AZv8Amf8AzP8A//8zAP8zM/8zZv8zmf8zzP8z//9mAP9mM/9mZv9mmf9mzP9m//+ZAP+ZM/+ZZv+Zmf+ZzP+Z///MAP/MM//MZv/Mmf/MzP/M////AP//M///Zv//mf///////yH5BAEAAP4ALAAAAAAUABQAQAg9AP0JHEiwoMGCABIKTAjgoMOHECMeZLhQocSLBin60whRI8eHHi1iHEmypMmOFj86DNkwIkuJL0/KnFkwIAA7'''
icon2='''R0lGODlhFAAUAPcAAAAAAIAAAACAAICAAAAAgIAAgACAgICAgMDAwP8AAAD/AP//AAAA//8A/wD//////wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMwAAZgAAmQAAzAAA/wAzAAAzMwAzZgAzmQAzzAAz/wBmAABmMwBmZgBmmQBmzABm/wCZAACZMwCZZgCZmQCZzACZ/wDMAADMMwDMZgDMmQDMzADM/wD/AAD/MwD/ZgD/mQD/zAD//zMAADMAMzMAZjMAmTMAzDMA/zMzADMzMzMzZjMzmTMzzDMz/zNmADNmMzNmZjNmmTNmzDNm/zOZADOZMzOZZjOZmTOZzDOZ/zPMADPMMzPMZjPMmTPMzDPM/zP/ADP/MzP/ZjP/mTP/zDP//2YAAGYAM2YAZmYAmWYAzGYA/2YzAGYzM2YzZmYzmWYzzGYz/2ZmAGZmM2ZmZmZmmWZmzGZm/2aZAGaZM2aZZmaZmWaZzGaZ/2bMAGbMM2bMZmbMmWbMzGbM/2b/AGb/M2b/Zmb/mWb/zGb//5kAAJkAM5kAZpkAmZkAzJkA/5kzAJkzM5kzZpkzmZkzzJkz/5lmAJlmM5lmZplmmZlmzJlm/5mZAJmZM5mZZpmZmZmZzJmZ/5nMAJnMM5nMZpnMmZnMzJnM/5n/AJn/M5n/Zpn/mZn/zJn//8wAAMwAM8wAZswAmcwAzMwA/8wzAMwzM8wzZswzmcwzzMwz/8xmAMxmM8xmZsxmmcxmzMxm/8yZAMyZM8yZZsyZmcyZzMyZ/8zMAMzMM8zMZszMmczMzMzM/8z/AMz/M8z/Zsz/mcz/zMz///8AAP8AM/8AZv8Amf8AzP8A//8zAP8zM/8zZv8zmf8zzP8z//9mAP9mM/9mZv9mmf9mzP9m//+ZAP+ZM/+ZZv+Zmf+ZzP+Z///MAP/MM//MZv/Mmf/MzP/M////AP//M///Zv//mf///////yH5BAEAAP4ALAAAAAAUABQAQAg4AP0JHEiwoMGCABIqTHiwocOHEA8uXBixosSJDCFizPhwIwCLIEOKHBnRY8mNJzGmnEiypUuDAQEAOw=='''


import midirender_util
import bmixer

sys.path.append('..\\bmidilib')
import bmidilib
import bmiditools

sys.path.append('..\\scoreview')
import scoreview
import listview

class App():
	def __init__(self, root):
		root.title('Bmidi Render')
		
		frameMain = Frame(root)
		frameMain.pack(side=TOP,fill=BOTH, expand=True)
		
		self.lblFilename = Label(frameMain, text='No file opened.')
		self.lblFilename.pack(side=TOP, anchor='w')
		
		def testevt(evt=None):
			print 'hi'
		
		
		#~ self.lblBank = Label(frameMain, text='Default bank: eawpats')
		#~ self.lblBank.pack(side=TOP, anchor='w')
		#~ self.lblBank.bind('<Button-1>', testevt)
		
		import time
		def threado():
			i = 0
			while True: #and stillPlaying. and app still open. this might/will lose sync with midi over time, but that's ok.
				if i>=sc['to']: break
				sc.set(i); 
				time.sleep(0.25)
				i+=0.25
		def nothing():
			midirender_util.makeThread(threado)
		
		
		
		#to "pause" and "resume" we simply play starting somewhere else
		
		self.icon0 = PhotoImage(data=icon0)
		self.icon1 = PhotoImage(data=icon1)
		self.icon2 = PhotoImage(data=icon2)
		
		frameDecogroup = Frame(frameMain, borderwidth=1, relief=GROOVE, padx=3, pady=3)
		self.timeSlider = Scale(frameDecogroup, orient=HORIZONTAL,resolution=1, width=5) #use resolution=0.1 to see decimals
		self.timeSlider.pack(side=TOP, fill=X)
		sc= self.timeSlider
		
		frameBtns = Frame(frameDecogroup)
		Button(frameBtns, image=self.icon0, text='Play mid', command=nothing).pack(side=LEFT, padx=4)
		Button(frameBtns, image=self.icon1, text='Pause mid', command=nothing).pack(side=LEFT, padx=4)
		Button(frameBtns, image=self.icon2,  text='Stop mid', command=nothing,relief=SUNKEN).pack(side=LEFT, padx=4)
		
		Checkbutton(frameBtns, text='Timidity').pack(side=LEFT, padx=35)
		#~ Button(frameBtns, text='Preview Wav', command=nothing).pack(side=LEFT, padx=25)
		Button(frameBtns, text='Save Wav', command=nothing).pack(side=LEFT, padx=2)
		
		frameBtns.pack(side=TOP, fill=X, pady=2)
		
		frameBtns2 = Frame(frameMain, borderwidth=3)
		
		Button(frameBtns2, text='Preview wav', command=nothing).pack(side=LEFT, padx=4)
		Button(frameBtns2, text='Render wav', command=nothing).pack(side=LEFT, padx=4)
		Button(frameBtns2, text='Show Mixer', command=nothing).pack(side=LEFT, padx=4)
		Label(frameBtns2, text='Default bank: eawpats').pack(side=LEFT, padx=4)
		#~ b.config(relief=SUNKEN)
		# a scale for when to play? use a scale control, for play position
		#frameBtns2.pack(side=TOP)
		
		frameDecogroup.pack(side=TOP, fill=X)
		
		frameGrid = Frame(frameMain, borderwidth=1)
		frameGrid.pack(side=TOP,fill=BOTH, expand=True, anchor='w', pady=15)
		#~ frameGrid.grid_rowconfigure(0, weight=1, minsize=10)
		frameGrid.grid_columnconfigure(1, weight=1, minsize=20)
		
		self.haveDrawnHeaders = False
		
		self.objMidi = None
		self.frameGrid = frameGrid
		
		#had a memory leak problem before, even though used del on all of the widgets. dang tk.
		self.gridwidgets = {} #index is tuple (row, column)
		self.gridbuttons = {} #index is tuple (row, column)
		
		#These are 
		self.listviews = {} #index is track number. 
		self.scoreviews = {}
		
		self.create_menubar(root)
	
	def drawColumnHeaders(self):
		opts = {}
		Label(self.frameGrid, text='#', **opts).grid(row=0, column=0)
		Label(self.frameGrid, text='Track', **opts).grid(row=0, column=1)
		Label(self.frameGrid, text='Channel', **opts).grid(row=0, column=2)
		Label(self.frameGrid, text='Instrument', **opts).grid(row=0, column=3)
		Label(self.frameGrid, text='Begins', **opts).grid(row=0, column=4)
		Label(self.frameGrid, text='Notes', **opts).grid(row=0, column=5)
		Label(self.frameGrid, text=' ', **opts).grid(row=0, column=6)
		Label(self.frameGrid, text=' ', **opts).grid(row=0, column=7)
		Label(self.frameGrid, text=' ', **opts).grid(row=0, column=8)
		self.haveDrawnHeaders = True
	
	def create_menubar(self,root):
		root.bind('<Alt-F4>', lambda x:root.quit)
		root.bind('<Control-o>', self.menu_openMidi)
		menubar = Menu(root)
		
		menuFile = Menu(menubar, tearoff=0)
		menubar.add_cascade(label="File", menu=menuFile, underline=0)
		menuFile.add_command(label="Open Midi", command=self.menu_openMidi, underline=0, accelerator='Ctrl+O')
		menuFile.add_separator()
		menuFile.add_command(label="Save modified midi...", command=self.menu_openMidi, underline=0)
		menuFile.add_separator()
		menuFile.add_command(label="Exit", command=root.quit, underline=1)
		
		menuAudio = Menu(menubar, tearoff=0)
		menubar.add_cascade(label="Audio", menu=menuAudio, underline=0)
		menuAudio.add_command(label="Play", command=self.menu_openMidi, underline=0)
		menuAudio.add_command(label="Pause", command=self.menu_openMidi, underline=0)
		menuAudio.add_command(label="Stop", command=self.menu_openMidi, underline=0)
		menuAudio.add_separator()
		menuAudio.add_command(label="Change tempo...", command=self.menu_openMidi, underline=0)
		menuAudio.add_separator()
		menuAudio.add_command(label="Choose Patch Bank...", command=self.menu_openMidi, underline=0)
		menuAudio.add_command(label="Configure Instrument Patches...", command=self.menu_openMidi, underline=0)
		
		
		self.objOptionsDuration = IntVar(); self.objOptionsDuration.set(0)
		self.objOptionsBarlines = IntVar(); self.objOptionsBarlines.set(1)
		menuView = Menu(menubar, tearoff=0)
		menubar.add_cascade(label="View", menu=menuView, underline=0)
		menuView.add_command(label="Mixer", command=self.menu_openMidi, underline=0)
		menuView.add_command(label="Console output", command=self.menu_openMidi, underline=0)
		menuView.add_separator()
		menuView.add_checkbutton(label="Show Durations in score", variable=self.objOptionsDuration, underline=0, onvalue=1, offvalue=0)
		menuView.add_checkbutton(label="Show Barlines in score", variable=self.objOptionsBarlines, underline=5, onvalue=1, offvalue=0)
		
		
		menuHelp = Menu(menubar, tearoff=0)
		menuHelp.add_command(label='About', command=(lambda: midirender_util.alert('BMidiRender, by Ben Fisher 2009\nhalfhourhacks.blogspot.com\n\nA graphical frontend for Timidity.','benmidi Render')))
		menubar.add_cascade(label="Help", menu=menuHelp, underline=0)
		
		root.config(menu=menubar)
		
		
	def menu_openMidi(self, evt=None):
		filename = midirender_util.ask_openfile(title="Open Midi File", types=['.mid|Mid file'])
		if not filename: return

		#first, see if it loads successfully.
		try:
			newmidi = bmidilib.BMidiFile()
			newmidi.open(filename, 'rb')
			newmidi.read()
			newmidi.close()
		except e:
			midirender_util.alert('Could not load midi: exception %s'%str(e), title='Could not load midi',icon='error')
			return
		self.lblFilename['text'] = filename
		self.loadMidiObj(newmidi)
			
	def loadMidiObj(self, newmidi):
		self.objMidi = newmidi
		
		if not self.haveDrawnHeaders: self.drawColumnHeaders()
		
		#close any open views
		for key in self.listviews: self.listviews[key].destroy()
		for key in self.scoreviews: self.scoreviews[key].destroy()
		self.listviews = {}; self.scoreviews = {}
		
		#hide all of the old widgets
		for key in self.gridwidgets:
			w= self.gridwidgets[key]
			if w.master.is_smallframe==1:
				w.master.grid_forget()
		for key in self.gridbuttons:
			self.gridbuttons[key].grid_forget()
		
		def addLabel(text, y, x, isButton=False):
			#Only create a new widget when necessary. This way, don't need to allocate every time a file is opened.
			if (x,y+1) not in self.gridwidgets:
				smallFrame = Frame(self.frameGrid, borderwidth=1, relief=RIDGE) #GROOVE
				smallFrame.is_smallframe=1
				if isButton: lbl = Button(smallFrame, text=text, relief=GROOVE,anchor='w'); lbl.pack(anchor='w', fill=BOTH)
				else: lbl = Label(smallFrame, text=text); lbl.pack(anchor='w')
				
				self.gridwidgets[(x,y+1)] = lbl
			
			self.gridwidgets[(x,y+1)]['text'] = text
			self.gridwidgets[(x,y+1)].master.grid(row=y+1, column=x, sticky='nsew')
		
		lengthTimer = bmiditools.BMidiSecondsLength(self.objMidi)
		overallLengthSeconds = lengthTimer.getOverallLength(self.objMidi)
		self.timeSlider['to'] = max(1.0, overallLengthSeconds); print overallLengthSeconds
		warnMultipleChannels = False
		for rownum in range(len(self.objMidi.tracks)):
			trackobj = self.objMidi.tracks[rownum]
			
			# Track Number
			addLabel( str(rownum), rownum, 0)
			
			# Track Name
			defaultname = 'Condtrack' if rownum==0 else ('Track %d'%rownum)
			searchFor = {'TRACKNAME':defaultname, 'INSTRUMENTS':1 }
			res = bmiditools.getTrackInformation(trackobj, searchFor)
			addLabel( res['TRACKNAME'], rownum, 1)
			
			# Track Channel(s)
			chanarray = self.findNoteChannels(trackobj)
			if len(chanarray)==0: channame='None'
			elif len(chanarray)>1: channame='(Many)'; warnMultipleChannels=True
			else: channame = str(chanarray[0])
			addLabel( channame, rownum, 2)
			
			countednoteevts = self.countNoteEvents(trackobj)
			
			# Track Instrument(s)
			instarray = res['INSTRUMENTS']
			if len(instarray)==0: instname='None'
			elif len(instarray)>1: instname='(Many)'
			else: instname = str(instarray[0]) + ' (' + bmidilib.bmidiconstants.GM_instruments[instarray[0]] + ')'
			if channame=='10': instname = '(Percussion channel)'
			isButton = countednoteevts>0
			addLabel( instname, rownum, 3, isButton)
			#minor bug. If you first, say, open a midi without a conductor track, then it will be a button from then on.
			
			#Track Time
			if len(trackobj.notelist)==0: strTime = lengthTimer.secondsToString(0)
			else: strTime = lengthTimer.secondsToString(lengthTimer.ticksToSeconds(trackobj.notelist[0].time))
			addLabel( strTime, rownum, 4)
			
			# Track Notes
			addLabel( str(countednoteevts), rownum, 5)
			
			#Buttons
			if (rownum, 0) not in self.gridbuttons:
				btn = Button(self.frameGrid, text='Mixer', command=midirender_util.Callable(self.openScoreView, rownum))
				self.gridbuttons[(rownum, 0)] = btn
			self.gridbuttons[(rownum, 0)].grid(row=rownum+1, column=6)
			
			if (rownum, 1) not in self.gridbuttons:
				btn = Button(self.frameGrid, text='Score', command=midirender_util.Callable(self.openListView, rownum))
				self.gridbuttons[(rownum, 1)] = btn
			self.gridbuttons[(rownum, 1)].grid(row=rownum+1, column=7)
			
			if (rownum, 2) not in self.gridbuttons:
				btn = Button(self.frameGrid, text='List', command=midirender_util.Callable(self.openListView, rownum))
				self.gridbuttons[(rownum, 2)] = btn
			self.gridbuttons[(rownum, 2)].grid(row=rownum+1, column=8)
		
		if warnMultipleChannels:
			resp = midirender_util.ask_yesno('This midi file has notes from different channels in the same track (format 0). It will play back fine, but features such as the Mixer may not work as intended. Click "yes" to import it as a format 1 file, or "no" to leave it. ')
			if resp:
				newmidi = bmiditools.restructureMidi(self.objMidi)
				self.loadMidiObj(newmidi)
				return
	
	
	# These aren't in bmiditools for now
	# they assume notelist is valid, and notelist is only valid if we've just read from a file
	def countNoteEvents(self, trackObject):
		return len(trackObject.notelist)
	
	def findNoteChannels(self, trackObject):
		channelsSeen = {}
		for note in trackObject.notelist:
			channelsSeen[note.channel] = 1
		return channelsSeen.keys()
	
	def openScoreView(self, n):
		if len(self.objMidi.tracks[n].notelist)==0:
			midirender_util.alert('No notes to show in this track.')
			return
		opts = {}
		opts['show_durations'] = self.objOptionsDuration.get()
		opts['show_barlines'] = self.objOptionsBarlines.get()
		opts['show_stems'] = 1; opts['prefer_flats'] = 0
		
		top = Toplevel()
		window = scoreview.ScoreViewWindow(top, n, self.objMidi.tracks[n],self.objMidi.ticksPerQuarterNote, opts)
		self.scoreviews[n] = top
			
	def openListView(self, n):
		opts = {}
		top = Toplevel()
		window = listview.ListViewWindow(top, n, self.objMidi.tracks[n], opts)
		self.listviews[n] = top
	
root = Tk()
app = App(root)
root.mainloop()

