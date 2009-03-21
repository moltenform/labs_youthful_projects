from Tkinter import *
import scoreview_util
from os import sep as os_sep

class ScoreViewWindow():
	def __init__(self, top, tracknumber, trackdata, ticksPerQtrNote, opts):
		top.title('Track %d Score'%tracknumber)
		
		frameTop = Frame(top, height=400)
		frameTop.pack(expand=YES, fill=BOTH)
		
		self.score = ScoreViewFrame(frameTop, trackdata, ticksPerQtrNote, opts)
		self.score.pack(expand=YES, fill=BOTH)
		
		frbtns = Frame(frameTop)
		frbtns.pack()
		Label(frbtns, text='x zoom:').pack(side=LEFT)
		Button(frbtns, text='+', command=self.score.zoomInX).pack(side=LEFT)
		Button(frbtns, text=' - ', command=self.score.zoomOutX).pack(side=LEFT)
		Label(frbtns, text='  y zoom:').pack(side=LEFT)
		Button(frbtns, text='+', command=self.score.zoomInY).pack(side=LEFT)
		Button(frbtns, text=' - ', command=self.score.zoomOutY).pack(side=LEFT)
		
		Label(frbtns, text='  change octave:').pack(side=LEFT)
		Button(frbtns, text='^', command=self.score.shiftOctaveUp).pack(side=LEFT)
		Button(frbtns, text='v', command=self.score.shiftOctaveDown).pack(side=LEFT)
		


class ScoreViewFrame(Frame):
	yScale = 3 # scale, half of distance between two staff lines.
	defaultWidth=600
	defaultHeight=200
	completeWidth = None #not known yet
	completeHeight = 200
	clefWidth = 30
	
	pixelsPerTick = None #not known yet
	
	prefersharpflat = '#'
	shiftnotes = 0
	def __init__(self, master, trackdata, ticksPerQtrNote, opts):
		Frame.__init__(self, master)
		
		self.opts = opts
		self.trackdata = trackdata
		if 'prefer_flats' in opts and opts['prefer_flats']: self.prefersharpflat = 'b'
		
		# Calculate scaling factors
		assert self.yScale >= 2 and self.yScale <= 5
		
		# calculate x scaling factor
		self.ticksPerQtrNote = ticksPerQtrNote
		visibleWidth = self.defaultWidth #600, visible width of canvas, in pixels
		desiredMeasuresVisible = 4
		measureInTicks = ticksPerQtrNote * 4
		measureWidth = visibleWidth/desiredMeasuresVisible
		self.pixelsPerTick = float(measureWidth)/float(measureInTicks)
		
		self.calculateCompleteWidth()
		
		# So the visible boundaries of the canvas are defaultWidth by defaultHeight 
		# but the actual boundaries are completeWidth by completeHeight
		
		self.createWidgets()
		self.redraw()
		
		# if the first note is off screen, scroll until it is in view. (half the screen)
		if len(self.trackdata.notelist) and self.scaleTicksToPixels(self.trackdata.notelist[0].time) > self.defaultWidth+700 :
			percentage = (self.scaleTicksToPixels(self.trackdata.notelist[0].time)- (self.defaultWidth/2)) / float(self.completeWidth)
			self.cv.xview_moveto(percentage)
			

	def scaleTicksToPixels(self, x): #This is only used when finding where to place a note, but not used when placing sharp symbols /relative measurements
		return self.pixelsPerTick * x + self.clefWidth
	def scaleYPositionToPixels(self, y):  #These coordinates are centered around the middle between the staffs. each unit is half of distance between two staff lines.
		return (self.completeHeight/2.0) - y*self.yScale
	def zoomInY(self):
		if self.yScale < 5:  self.yScale+=1;   self.redraw()
	def zoomOutY(self):
		if self.yScale > 2:  self.yScale-=1;    self.redraw()
	def zoomInX(self):
		self.pixelsPerTick *= 1.25;   self.redraw()
	def zoomOutX(self):
		self.pixelsPerTick /= 1.25;   self.redraw()
	
	def shiftOctaveUp(self): self.shiftnotes+=12; self.redraw()
	def shiftOctaveDown(self): self.shiftnotes-=12; self.redraw()
	def calculateCompleteWidth(self):
		# get time of last note-off event, when song stops playing
		if len(self.trackdata.notelist)==0: self.lastTick = 100
		else: self.lastTick = self.trackdata.notelist[-1].endEvt.time  + 100
		self.completeWidth = int(round(self.pixelsPerTick * self.lastTick)) + self.clefWidth
		
	def redraw(self):
		#update complete size of canvas (the pixelsPerTick could have changed)
		self.calculateCompleteWidth()
		self.cv.configure(scrollregion=(0, 0, self.completeWidth, self.completeHeight))
		
		self.clear()
		self.drawBackground(self.opts)
		
		for note in self.trackdata.notelist:
			self.drawNote( note.pitch, note.time, note.time+note.duration)
		
	
	def drawBackground(self, opts):
		self.draw_clef( 'treble')
		self.draw_clef( 'bass')
		
		#draw staff lines
		def drawstaffline(y): 
			self.draw_line(0, y, self.completeWidth, y)
		for i in range(1,6): 
			drawstaffline(i*2 + 2)
			drawstaffline(-i*2 - 2)
		
		# draw barlines
		def drawbarline(xTicks):
			xPixels = self.scaleTicksToPixels(xTicks)
			xPixels -= 6 #draw it slightly before measure to not cut through notes
			#~ self.draw_faintline(xPixels, 10, xPixels, -10)
			self.draw_line(xPixels, 12, xPixels, -12)
		if 'show_barlines' in self.opts and self.opts['show_barlines']:
			for xTicks in range(4 * self.ticksPerQtrNote, self.lastTick, 4 * self.ticksPerQtrNote): #doesn't start at 0, to not draw first line
				drawbarline(xTicks)
		

	def drawNote(self, notenumber, starttimecode, endtimecode):
		#find x position for drawing note
		
		xPixels = self.scaleTicksToPixels(starttimecode)
		xPixelsEnd = self.scaleTicksToPixels(endtimecode)
		
		posy, sharpflat = self.getnoteposition(notenumber, self.shiftnotes, self.prefersharpflat)
		
		fy = posy
		if fy >= 0:	fy += 2  #treble clef, move up
		else:    	fy -= 2  #bass clef, move down
		
		#draw note head
		self.draw_oval(xPixels, fy+0.6, xPixels+3, fy-0.6)
		
		#draw sharp or flat
		if sharpflat == '#':
			self.draw_text(xPixels-8, fy, '#')
		elif sharpflat == 'b':
			self.draw_text(xPixels-8, fy, 'b')
		
		#draw ledgers
		def drawledger(x,y): 
			self.draw_line(x-4,y,x+7,y)
			
		if posy==0: drawledger(xPixels, fy)
		elif posy >= 12:
			tempy = posy - posy%2 #round posy down to nearest even number
			while tempy >= 12: 
				drawledger(xPixels, tempy + 2); #add two because treble clef, move up
				tempy-=2
		elif posy <= -12:
			tempy = posy + posy%2 #round posy down to nearest even number
			while tempy <= -12: 
				drawledger(xPixels, tempy - 2); # two because bass clef, move down
				tempy+=2
			
		#draw stem
		def drawstem(x,y, mult): #mult is 1: upwards stem, mult is -1: downwards stem
			self.draw_line(x, y, x, y+mult*7)
		if 'show_stems' in self.opts and self.opts['show_stems']:
			if posy>=0: drawstem(xPixels + 3, fy, 1) #treble clef, stem up
			else: drawstem(xPixels, fy, -1)
		
		#draw duration mark
		if 'show_durations' in self.opts and self.opts['show_durations']:
			self.draw_faintline(xPixels+6, fy, xPixelsEnd, fy)
	
	def getnoteposition(self,note, shift, prefersharpflat): #middle C is note 60, returns a position of 0
		note += shift
		note -= 60 # so that it is centered around middle C, 0
		octave = note // 12
		scaledegree = note % 12
		
		sharps = {
		0: ('c',0),
		1: ('c#',0),
		2: ('d',1),
		3: ('d#',1),
		4: ('e',2),
		5: ('f',3),
		6: ('f#',3),
		7: ('g',4),
		8: ('g#',4),
		9: ('a',5),
		10: ('a#',5),
		11: ('b',6),
		}
		posy = octave*7
		sharpflat = ''
		if '#' in sharps[scaledegree][0]:
			if prefersharpflat == 'b':
				posy += sharps[scaledegree][1] +1 #is a flat
				sharpflat = 'b'
			else:
				posy += sharps[scaledegree][1] #works
				sharpflat = '#'
		else:
			posy += sharps[scaledegree][1]
			sharpflat = ''
		return (posy, sharpflat)
		
		
	def threadEvents(self, g=None):
		if self.ripe!=True: return
		self.ripe= False
		import time
		time.sleep(1)
		
		pos = self.cv.xview()[0] * self.completeWidth
		self.cv.coords(self.myRect, (pos, 0, pos+20, self.completeHeight))
		self.ripe= True
		
	def createWidgets(self):
		self.ripe = True
		frameGrid = Frame(self)
		frameGrid.pack(expand=YES, fill=BOTH)
		
		self.cv = Canvas(frameGrid, bd=1, background='white', width=self.defaultWidth, height=self.defaultHeight)
		hScroll = Scrollbar(frameGrid, orient=HORIZONTAL, command=self.cv.xview)
		vScroll = Scrollbar(frameGrid, orient=VERTICAL, command=self.cv.yview)
		self.cv.configure(scrollregion=(0, 0, self.completeWidth, self.completeHeight))
		self.cv.configure(xscrollcommand=hScroll.set, yscrollcommand=vScroll.set)
		
		self.cv.grid(row = 0, column=0, sticky='nsew', ipadx=0)
		vScroll.grid(row=0, column=1, sticky='ns')
		hScroll.grid(row=1, column=0, sticky='we')
		
		frameGrid.grid_rowconfigure(0, weight=1)
		frameGrid.grid_columnconfigure(0, weight=1)
		
		
		self.clefimgs = {}
		
	
	def clear(self): self.cv.delete(ALL)
	#The following are unusual: The Y coordinates are scaled, since they are not in pixels. The X coordinates are not scaled, they are already in pixels
	def coord(self, x,y):
		return x, self.scaleYPositionToPixels(y)
	def coordrect(self, x0, y0, x1, y1): 
		return (x0, self.scaleYPositionToPixels(y0), x1, self.scaleYPositionToPixels(y1))
	def draw_line(self, x0, y0, x1, y1):
		self.cv.create_line(self.coordrect(x0, y0, x1, y1)  )
	def draw_faintline(self, x0, y0, x1, y1):
		self.cv.create_line(self.coordrect(x0, y0, x1, y1),fill="gray", dash=(4, 4)  )
	def draw_text(self, x, y, s):
		textFormat = ('Times New Roman', 9, 'normal')
		self.cv.create_text(self.coord(x,y),text=s, anchor='w', font=textFormat )
		
	def draw_oval(self,x0, y0, x1, y1): 
		self.cv.create_oval(self.coordrect(x0, y0, x1, y1), fill='black' )
	def draw_clef(self, bass_treb):
		map = {2:'16.gif',3:'24.gif',4:'32.gif',5:'40.gif'}
		desiredImageName = bass_treb + map[self.yScale]
		if desiredImageName not in self.clefimgs:
			self.clefimgs[desiredImageName] = PhotoImage(file='clefs'+os_sep+desiredImageName)
		
		if bass_treb=='bass': 
			xposition = 12
			yposition = self.scaleYPositionToPixels(-8.0)
		else:
			xposition = 12
			yposition = self.scaleYPositionToPixels(7.5)
		self.cv.create_image((xposition,yposition),image=self.clefimgs[desiredImageName])
	
	#~ def getwidth(self): return self.cv.winfo_width()
	#~ def getheight(self): return self.cv.winfo_height()
			
		
	
		

	
	
	
	
			

if __name__=='__main__':
			
	
	sys.path.append('..\\bmidilib')
	import bmidilib
	
	newmidi = bmidilib.BMidiFile()
	#~ newmidi.open('simple.mid', 'rb')
	newmidi.open('..\\midis\\16keys.mid', 'rb')
	newmidi.read()
	newmidi.close()
	
	root = Tk()
	opts = {}
	opts['show_barlines']=1
	opts['show_stems']=1
	opts['show_durations']=1
	
	app = ScoreViewWindow(root, 1, newmidi.tracks[1],newmidi.ticksPerQuarterNote, opts)
	
	
	root.mainloop()
	
	