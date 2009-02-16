from Tkinter import *
import scoreview_util

class ScoreViewCanvasAbstract(Frame):
	##SETTINGS
	prefersharpflat = '#'
	shiftnotes = 0
	timestart=0
	timeend=100
	
	#abstract scoreview methods
	def drawBackground(self):
		#draw the clefs
		self.draw_clef( 'treble')
		self.draw_clef( 'bass')
		
		#draw staff lines
		def drawstaffline(y): 
			self.draw_line(0, y, self.getwidth(), y)
			self.draw_clef_staffline(y)
		for i in range(1,6): drawstaffline(i*2 + 2); drawstaffline(-2 * i - 2)
			
		#test notes
		#~ self.renderNote(60, 15, 30)
		#~ self.renderNote(61, 30, 30+10)
		#~ self.renderNote(62, 40, 40+10)
		for i in range(14):
			self.renderNote(60 + 19+ i, 10+i*9, 10+i*9+ 10)
			self.renderNote(60- 19 - i, 10+i*9, 10+i*9+ 10)
			
	def renderMidiTrack(self, trackobj, ticksPerQtrNote):
		if len(trackobj.notelist)==0:
			self.drawBackground()
			return
		
		firstx = 0
		# get time of last note-off event, when it stops playing
		lastx = trackobj.notelist[-1].endEvt.time
		
		span = lastx - firstx
		if span==0: span = 1 #prevent division by 0.
		
		#default is to show 1 qtr note = 25 pixels.
		#~ 120 ticksPerQtr means 120 ticks are 25pixels
		#~ 200 ticksPerQtr means 200 ticks are 25pixels
		
		#~ self.xticksToPixels = 25.0/float(ticksPerQtrNote)
		
		#~ ticksPerQtrNote * 25
			
			
		
	def renderNote(self, notenumber, starttimecode, endtimecode):
		#find x position for drawing note
		if starttimecode < self.timestart or starttimecode > self.timeend:
			return
		scale = float(self.timeend - self.timestart)/float(self.getwidth()-self.coord(0,0)[0])
		posx = starttimecode / scale
		endx = min(self.timeend,endtimecode) /scale
		
		o = self.getnoteposition(notenumber, self.shiftnotes, self.prefersharpflat)
		posy = o.posy
		sharpflat = o.sharpflat
		
		
		#draw note head
		fy = posy  #because each line is 2 notes
		if fy >= 0: #treble clef, move up
			fy += 2
		else: #bass clef, move down
			fy -= 2
		self.draw_oval(posx, fy+0.6, posx+3, fy-0.6)
		
		#draw sharp or flat
		if sharpflat == '#':
			self.draw_text(posx-8, fy, '#')
		elif sharpflat == 'b':
			self.draw_text(posx-8, fy, 'b')
		
		#draw ledgers
		def drawledger(x,y): 
			self.draw_line(x-4,y,x+7,y)
			
		if posy==0: drawledger(posx, fy)
		elif posy >= 12:
			tempy = posy - posy%2 #round posy down to nearest even number
			while tempy >= 12: 
				drawledger(posx, tempy + 2); #add two because treble clef, move up
				tempy-=2
		elif posy <= -12:
			tempy = posy + posy%2 #round posy down to nearest even number
			while tempy <= -12: 
				drawledger(posx, tempy - 2); # two because bass clef, move down
				tempy+=2
			
		#draw stem
		def drawstem(x,y, mult): #mult is 1: upwards stem, mult is -1: downwards stem
			self.draw_line(x, y, x, y+mult*7)
		if posy>=0: drawstem(posx + 3, fy, 1) #treble clef, stem up
		else: drawstem(posx, fy, -1)
		
		#draw duration mark
		self.draw_faintline(posx+6, fy, endx, fy)
	
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
		return scoreview_util.NoteRenderInfo(posy, sharpflat)
		

#Coord system: 0 is at middle C, each unit is one-half staff ledger line height (one note height).
#Xcoord system: 1 is the spacing 

class ScoreViewCanvasTk(ScoreViewCanvasAbstract):
	def __init__(self, master, xscale, yscale):
		Frame.__init__(self, master)
		
		assert yscale >= 2 and yscale <= 5
		
		
		frameGrid = Frame(self)
		frameGrid.pack(expand=YES, fill=BOTH)
		
		self.cvClefs = Canvas(frameGrid, bd=1, background='white', width=30)
		self.cv = Canvas(frameGrid, bd=1, background='white')
		hScroll = Scrollbar(frameGrid, orient=HORIZONTAL, command=self.shiftOctaveUp)
		vScroll = Scrollbar(frameGrid, orient=VERTICAL, command=self.shiftOctaveUp)
		
		
		self.cvClefs.grid(row = 0, column=0, sticky='wns', ipadx=0)
		self.cv.grid(row = 0, column=1, sticky='nsew', ipadx=0)
		vScroll.grid(row=0, column=2, sticky='ns')
		hScroll.grid(row=1, column=0, columnspan=2, sticky='we')
		
		frameGrid.grid_rowconfigure(0, weight=1)
		frameGrid.grid_columnconfigure(1, weight=1)
		#~ root.grid_rowconfigure(0, weight=1)
#~ root.grid_columnconfigure(0, weight=1)
		
		self.xscale = xscale
		self.yscale = yscale
		#self.ybasis will be set in redraw
		
		self.clefimgs = {}
		
		#Do not redraw right when creating, because we might not know our dimensions
		
		
	
	def zoomInY(self):
		if self.yscale < 5: 
			self.yscale+=1
			self.redraw()
	def zoomOutY(self):
		if self.yscale > 2: 
			self.yscale-=1
			self.redraw()
	def shiftOctaveUp(self): self.shiftnotes+=12; self.redraw()
	def shiftOctaveDown(self): self.shiftnotes-=12; self.redraw()
	def redraw(self): #To do after every resize/ rescale
		self.clear()
		self.ybasis = int(self.getheight()/2)
		self.drawBackground()
	
	def coord(self, x,y):
		x2 = x*self.xscale + 0# 20 #gives room for the clefs
		y2 = self.ybasis - y*self.yscale
		return (x2, y2)
	def coordrect(self, x0, y0, x1, y1): x0, y0 = self.coord(x0, y0); x1, y1 = self.coord(x1, y1); return (x0,y0, x1,y1)
	def clear(self): self.cvClefs.delete(ALL); self.cv.delete(ALL)
	def getwidth(self): return self.cv.winfo_width()
	def getheight(self): return self.cv.winfo_height()
	def draw_line(self, x0, y0, x1, y1):
		self.cv.create_line(self.coordrect(x0, y0, x1, y1)  )
	def draw_faintline(self, x0, y0, x1, y1):
		self.cv.create_line(self.coordrect(x0, y0, x1, y1),fill="gray", dash=(4, 4)  )
	def draw_clef_staffline(self, y):
		self.cvClefs.create_line(self.coordrect(0, y, self.cvClefs.winfo_width(), y)  )
	def draw_text(self, x, y, s):
		textFormat = ('Times New Roman', 9, 'normal')
		self.cv.create_text(self.coord(x,y),text=s, anchor='w', font=textFormat )
		
	def draw_oval(self,x0, y0, x1, y1): 
		self.cv.create_oval(self.coordrect(x0, y0, x1, y1), fill='black' )
	def draw_clef(self, bass_treb):
		map = {2:'16.gif',3:'24.gif',4:'32.gif',5:'40.gif'}
		desiredImageName = bass_treb + map[self.yscale]
		if desiredImageName not in self.clefimgs:
			self.clefimgs[desiredImageName] = PhotoImage(file='clefs\\'+desiredImageName)
		
		if bass_treb=='bass': 
			xposition = (self.cvClefs.winfo_width()/2) - 0.5
			yposition = self.coord(0, -8.0)[1]
		else:
			xposition = (self.cvClefs.winfo_width()/2)
			yposition = self.coord(0, 7.5)[1]
		self.cvClefs.create_image((xposition,yposition),image=self.clefimgs[desiredImageName])
			

if __name__=='__main__':
	class TestApp():
		def __init__(self, root):
			root.title('Testing score view') #padx='0m'
			
			frameTop = Frame(root, height=400) ;frameTop['background']='white'
			frameTop.pack(expand=YES, fill=BOTH)
			self.score = ScoreViewCanvasTk(frameTop, 1,3 )
			self.score.pack(expand=YES, fill=BOTH)
			
			frbtns = Frame(frameTop)
			frbtns.pack()
			Label(frbtns, text='x zoom:').pack(side=LEFT)
			Button(frbtns, text='+', command=self.score.redraw).pack(side=LEFT)
			Button(frbtns, text=' - ', command=self.score.redraw).pack(side=LEFT)
			Label(frbtns, text='  y zoom:').pack(side=LEFT)
			Button(frbtns, text='+', command=self.score.zoomInY).pack(side=LEFT)
			Button(frbtns, text=' - ', command=self.score.zoomOutY).pack(side=LEFT)
			
			Label(frbtns, text='  change octave:').pack(side=LEFT)
			Button(frbtns, text='^', command=self.score.shiftOctaveUp).pack(side=LEFT)
			Button(frbtns, text='v', command=self.score.shiftOctaveDown).pack(side=LEFT)
			
			Button(frameTop, text='draw', command=self.score.redraw).pack()
			Button(frameTop, text='test', command=self.test).pack()
			
			
		def test(self):
			print 'l'
			print self.score.cv.winfo_width()

	
	root = Tk()
	app = TestApp(root)
	root.mainloop()