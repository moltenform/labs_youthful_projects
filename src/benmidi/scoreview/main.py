"""
ScoreView
Ben Fisher, 2009, GPL
halfhourhacks.blogspot.com

"""
from Tkinter import *

import scoreview_util
import listview
import scoreview

sys.path.append('..')
from bmidilib import bmidilib, bmiditools

class App:
	def __init__(self, root):
		root.title('Midi Scoreview')
		
		frameMain = Frame(root)
		frameMain.pack(side=TOP,fill=BOTH, expand=True)
		
		self.lblFilename = Label(frameMain, text='No file opened.')
		self.lblFilename.pack(side=TOP, anchor='w')
		
		frameGrid = Frame(frameMain, background='white', borderwidth=1)
		frameGrid.pack(side=TOP,fill=BOTH, expand=True, anchor='w')
		
		opts = {'background':'white'}
		Label(frameGrid, text='#', **opts).grid(row=0, column=0)
		Label(frameGrid, text='Track', **opts).grid(row=0, column=1)
		Label(frameGrid, text='Channel', **opts).grid(row=0, column=2)
		Label(frameGrid, text='Instrument', **opts).grid(row=0, column=3)
		Label(frameGrid, text='Notes', **opts).grid(row=0, column=4)
		Label(frameGrid, text=' ', **opts).grid(row=0, column=5)
		Label(frameGrid, text=' ', **opts).grid(row=0, column=6)
		
		self.objMidi = None
		self.frameGrid = frameGrid
		
		#had a memory leak problem before, even though used del on all of the widgets. dang tk.
		self.gridwidgets = {} #index is tuple (row, column)
		self.gridbuttons = {} #index is tuple (row, column)
		
		#These are 
		self.listviews = {} #index is track number. 
		self.scoreviews = {}
		
		self.create_menubar(root)
		
	def create_menubar(self,root):
		root.bind('<Alt-F4>', lambda x:root.quit)
		root.bind('<Control-o>', self.menu_openMidi)
		menubar = Menu(root)
		
		menuFile = Menu(menubar, tearoff=0)
		menubar.add_cascade(label="File", menu=menuFile, underline=0)
		menuFile.add_command(label="Open Midi", command=self.menu_openMidi, underline=0, accelerator='Ctrl+O')
		menuFile.add_separator()
		menuFile.add_command(label="Exit", command=root.quit, underline=1)
		
		self.objOptionsDuration = IntVar(); self.objOptionsDuration.set(0)
		self.objOptionsStems = IntVar(); self.objOptionsStems.set(1)
		self.objOptionsBarlines = IntVar(); self.objOptionsBarlines.set(1)
		self.objOptionsFlats = IntVar(); self.objOptionsFlats.set(0)
		menuOptions = Menu(menubar, tearoff=0)
		menubar.add_cascade(label="Options", menu=menuOptions, underline=0)
		menuOptions.add_checkbutton(label="Show Durations", variable=self.objOptionsDuration, underline=0, onvalue=1, offvalue=0)
		menuOptions.add_checkbutton(label="Show Stems", variable=self.objOptionsStems, underline=5, onvalue=1, offvalue=0)
		menuOptions.add_checkbutton(label="Show Barlines", variable=self.objOptionsBarlines, underline=5, onvalue=1, offvalue=0)
		menuOptions.add_checkbutton(label="Prefer flats", variable=self.objOptionsFlats, underline=7, onvalue=1, offvalue=0)
		
		
		menuHelp = Menu(menubar, tearoff=0)
		menuHelp.add_command(label='About', command=(lambda: scoreview_util.alert('ScoreView, by Ben Fisher 2009\n\nhalfhourhacks.blogspot.com','benmidi ScoreView')))
		menubar.add_cascade(label="Help", menu=menuHelp, underline=0)
		
		root.config(menu=menubar)
		
		
	def menu_openMidi(self, evt=None):
		filename = scoreview_util.ask_openfile(title="Open Midi File", types=['.mid|Mid file'])
		if not filename: return
		self.loadMidi(filename)
	
	def loadMidi(self, filename):
		#first, see if it loads successfully.
		try:
			newmidi = bmidilib.BMidiFile()
			newmidi.open(filename, 'rb')
			newmidi.read()
			newmidi.close()
		except e:
			scoreview_util.alert('Could not load midi: exception %s'%str(e), title='Could not load midi',icon='error')
			return

		self.objMidi = newmidi
		self.lblFilename['text'] = filename
		
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
		
		def addLabel(text, y, x):
			#Only create a new widget when necessary. This way, don't need to allocate every time a file is opened.
			if (x,y+1) not in self.gridwidgets:
				smallFrame = Frame(self.frameGrid,background='white', borderwidth=1, relief=RIDGE) #GROOVE
				smallFrame.is_smallframe=1
				lbl = Label(smallFrame, text=text,background='white')
				lbl.pack(anchor='w')
				self.gridwidgets[(x,y+1)] = lbl
			
			self.gridwidgets[(x,y+1)]['text'] = text
			self.gridwidgets[(x,y+1)].master.grid(row=y+1, column=x, sticky='nsew')
			
		
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
			elif len(chanarray)>1: channame='(Many)'
			else: channame = str(chanarray[0])
			addLabel( channame, rownum, 2)
			
			# Track Instrument(s)
			instarray = res['INSTRUMENTS']
			if len(instarray)==0: instname='None'
			elif len(instarray)>1: instname='(Many)'
			else: instname = str(instarray[0]) + ' (' + bmidilib.bmidiconstants.GM_instruments[instarray[0]] + ')'
			if channame=='10': instname = '(Percussion channel)'
			addLabel( instname, rownum, 3)
			
			# Track Notes
			addLabel( str(self.countNoteEvents(trackobj)), rownum, 4)
			
			#Buttons
			if (rownum, 0) not in self.gridbuttons:
				btn = Button(self.frameGrid, text='Score', command=scoreview_util.Callable(self.openScoreView, rownum))
				self.gridbuttons[(rownum, 0)] = btn
			self.gridbuttons[(rownum, 0)].grid(row=rownum+1, column=5)
			
			if (rownum, 1) not in self.gridbuttons:
				btn = Button(self.frameGrid, text='List', command=scoreview_util.Callable(self.openListView, rownum))
				self.gridbuttons[(rownum, 1)] = btn
			self.gridbuttons[(rownum, 1)].grid(row=rownum+1, column=6)
			
	
	
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
		#~ if n in self.scoreviews:
			#~ pass #already open
		#~ else:
		if len(self.objMidi.tracks[n].notelist)==0:
			scoreview_util.alert('No notes to show in this track.')
			return
		opts = {}
		opts['show_durations'] = self.objOptionsDuration.get()
		opts['show_stems'] = self.objOptionsStems.get()
		opts['show_barlines'] = self.objOptionsBarlines.get()
		opts['prefer_flats'] = self.objOptionsFlats.get()
		
		top = Toplevel()
		window = scoreview.ScoreViewWindow(top, n, self.objMidi.tracks[n],self.objMidi.ticksPerQuarterNote, opts)
		self.scoreviews[n] = top
			
	def openListView(self, n):
		#~ if n in self.listviews:
			#~ pass #already open
		#~ else:
		opts = {}
		top = Toplevel()
		window = listview.ListViewWindow(top, n, self.objMidi.tracks[n], opts)
		self.listviews[n] = top
	
root = Tk()
app = App(root)
root.mainloop()

