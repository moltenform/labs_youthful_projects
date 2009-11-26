from Tkinter import *
import scoreview_util
import os
import sys

#todo: whole note rests even when not 4/4
#todo: accdentals are wrong

class ScoreViewWindow():
	def __init__(self, top, intermed,bTrebleClef, clefsfilepath, opts):
		
		frameTop = Frame(top, height=400)
		frameTop.pack(expand=YES, fill=BOTH)
		
		self.score = ScoreViewFrame(frameTop, intermed,bTrebleClef, clefsfilepath, opts)
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
	completeWidth = None #not known yet, to be found when drawing
	completeHeight = 200
	clefWidth = 30
	
	pixelsPerTick = None #not known yet, to be found in __init__
	
	prefersharpflat = '#'
	shiftnotes = 0
	def __init__(self, master, intermed,bTrebleClef, clefsfilepath, opts):
		Frame.__init__(self, master)
		
		# intermed is of type IntermediateList
		
		self.clefspath= clefsfilepath
		self.opts = opts
		self.intermed = intermed
		self.bTrebleClef = bTrebleClef #I guess for now, draw both staves but only populate one.
		
		if self.intermed.bSharps: self.prefersharpflat='#'
		else:	self.prefersharpflat = 'b'
		#don't draw the time sig for now...
		timesig = intermed.timesig; assert timesig!=None
		
		assert self.yScale >= 2 and self.yScale <= 5
		
		# calculate x scaling factor
		visibleWidth = self.defaultWidth # visible width of canvas, in pixels
		desiredMeasuresVisible = 4
		fQtrNotesPerMeasure = float(timesig[0]) / (timesig[1]/4)
		self.ticksPerMeasure = int( fQtrNotesPerMeasure * self.intermed.baseDivisions)
		nTimestepsVisible = desiredMeasuresVisible * self.ticksPerMeasure
		self.pixelsPerTick = float(visibleWidth)/float(nTimestepsVisible)
		
		#find total time. because we add extra space for quick notes, this is only an estimate!!
		total=0
		for note in intermed.noteList: total+= note.end-note.start
		self.lastTick = total
		#~ self.completeWidth = self.scaleTicksToPixels(self.lastTick)
		
		# So the visible boundaries of the canvas are defaultWidth by defaultHeight 
		# but the actual boundaries are completeWidth by completeHeight
		
		self.createWidgets()
		self.redraw()
		
		# consider: if the first note is off screen, scroll until it is in view. (half the screen)
			

	def scaleTicksToPixels(self, x): #This is only used when finding where to place a note, but not used when placing sharp symbols /relative measurements
		return self.pixelsPerTick * float(x) + self.clefWidth
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
		
	def redraw(self):
		self.clear()
		
		mapDurs={ 
		int(self.intermed.baseDivisions*4): 1, #whole note,
		int(self.intermed.baseDivisions*2): 2, #half note
		int(self.intermed.baseDivisions*1): 4, #qtr note, and so on
		int(self.intermed.baseDivisions*0.5): 8,
		int(self.intermed.baseDivisions*0.25): 16,
		int(self.intermed.baseDivisions*0.125): 32,
		int(self.intermed.baseDivisions*0.0625): 64,
			}
		
		currentTime = 0; currentPos=0
		for note in self.intermed.noteList:
			if note.pitch==0 or note.pitch[0]==0: 
				self.drawRest( currentPos, mapDurs[note.end-note.start])
			else:
				pitch=note.pitch[0] #take the first note of the group - we don't support drawing polyphony!
				tielength=note.end-note.start
				if mapDurs[note.end-note.start]>=16: tielength+= 32 #add some space per note
				self.drawNote( currentPos, pitch, mapDurs[note.end-note.start], note.isTied, tielength)
			
			currentTime += note.end-note.start
			currentPos += note.end-note.start 
			if mapDurs[note.end-note.start]>=16: currentPos+= 32 #add some space per note
			
			assert currentTime<=self.ticksPerMeasure
			if currentTime==self.ticksPerMeasure:
				self.drawbarline(currentPos)
				currentPos += self.intermed.baseDivisions/8; #some extra space for barline
				currentTime=0
		
		self.lastTick = currentPos #important change.
		#update complete size of canvas (the pixelsPerTick could have changed)
		self.completeWidth = self.scaleTicksToPixels(self.lastTick)
		self.cv.configure(scrollregion=(0, 0, self.completeWidth, self.completeHeight))
		self.drawBackground(self.opts)
	
	def drawBackground(self, opts):
		self.draw_clef( 'treble')
		self.draw_clef( 'bass')
		
		#draw staff lines
		def drawstaffline(y): 
			self.draw_line(0, y, self.completeWidth, y) 
		for i in range(1,6): 
			drawstaffline(i*2 + 2)
			drawstaffline(-i*2 - 2)
		
	
	def drawbarline(self, xTicks):
		xPixels = self.scaleTicksToPixels(xTicks)
		xPixels -= 6 #draw it slightly before measure to not cut through notes
		self.draw_line(xPixels, 12, xPixels, -12)
		

	def drawNote(self, currentPos, pitch, noteType, isTied, tieLength):
		xPixels = self.scaleTicksToPixels(currentPos)
		
		posy, sharpflat = self.getnoteposition(pitch, self.shiftnotes, self.prefersharpflat)
		
		fy = posy #value 0 refers to middle C.
		if self.bTrebleClef: fy += 2  #treble clef, move up
		else:    	fy -= 2  #bass clef, move down
		
		#draw note head
		self.draw_oval(xPixels, fy+0.6, xPixels+3, fy-0.6, bFilled=noteType>=4)
		
		#draw sharp or flat
		if sharpflat == '#': self.draw_text(xPixels-8, fy, '#')
		elif sharpflat == 'b': self.draw_text(xPixels-8, fy, 'b')
		
		#draw ledgers
		def drawledger(x,y): 
			self.draw_line(x-4,y,x+7,y)
			
		if posy==0: 
			drawledger(xPixels, fy)
			
		elif self.bTrebleClef: #treble clef
			if posy >= 12:
				tempy = posy - posy%2 #round posy down to nearest even number
				while tempy >= 12: 
					drawledger(xPixels, tempy + 2); #add two because treble clef, move up
					tempy-=2
			elif posy <= 0:
				tempy = posy - posy%2 #round posy down to nearest even number
				while tempy <= 0: 
					drawledger(xPixels, tempy + 2); # add two because treble clef, move up
					tempy+=2
		elif not self.bTrebleClef: #bass clef
			if posy >= 0:
				tempy = posy - posy%2 #round posy down to nearest even number
				while tempy >= 0: 
					drawledger(xPixels, tempy - 2); #two because bass clef, move down
					tempy-=2
			elif posy <= -12:
				tempy = posy - posy%2 #round posy down to nearest even number
				while tempy <= -12: 
					drawledger(xPixels, tempy - 2); #two because bass clef, move down
					tempy+=2
		
			
		#draw stem
		def drawstem(x,y, direction, noteType): #direction is 1: upwards stem, direction is -1: downwards stem
			self.draw_line(x, y, x, y+direction*7)
			# draw "tails" on fast notes
			if noteType>=64: self.draw_line(x, y+direction*4, x+5, y+direction*2)
			if noteType>=32: self.draw_line(x, y+direction*5, x+5, y+direction*3)
			if noteType>=16: self.draw_line(x, y+direction*6, x+5, y+direction*4)
			if noteType>=8: self.draw_line(x, y+direction*7, x+5, y+direction*5)
			
		if noteType>=2:
			if self.bTrebleClef: drawstem(xPixels + 3, fy, 1, noteType) #treble clef, stem up
			else: drawstem(xPixels, fy, -1, noteType)
		
		if isTied:
			tiedTo = self.scaleTicksToPixels(currentPos+tieLength) -1 #-4
			self.draw_tiearc(xPixels,posy+3, tiedTo, posy-3, flipped=self.bTrebleClef)
		
	
	def drawRest(self, currentPos, restType):
		xPixels = self.scaleTicksToPixels(currentPos)
		if restType==1 or restType==2:
			if restType==1:
				if self.bTrebleClef: ypos = 7 + 2
				else: ypos = -5 - 2
			else:
				if self.bTrebleClef: ypos = 6 + 2
				else: ypos = -6 - 2
			#draw from ypos to ypos+1
			self.draw_rect(xPixels, ypos,xPixels+5,ypos+1)
		elif restType==4:
			if self.bTrebleClef: startingY = 9 + 2
			else: startingY = -3 - 2

			self.draw_line(xPixels-2, startingY, xPixels+3,startingY-2)
			self.draw_line(xPixels+3, startingY-2, xPixels-2, startingY-4)
			self.draw_line(xPixels-2, startingY-4, xPixels+3, startingY-6)
			self.draw_line(xPixels+3,  startingY-6, xPixels-2,  startingY-8)
		else:
			if self.bTrebleClef: startingY = 9 + 2
			else: startingY = -3 - 2
			#a simplification...
			if restType>=64: self.draw_oval(xPixels+2.0, startingY-5.5, xPixels+0.5, startingY-6.5)
			if restType>=32: self.draw_oval(xPixels+3.0, startingY-3.5, xPixels+1.5, startingY-4.5)
			if restType>=16: self.draw_oval(xPixels+4.0, startingY-1.5, xPixels+2.5, startingY-2.5)
			if restType>=8: self.draw_oval(xPixels+5.0, startingY-.5, xPixels+3.5, startingY+0.5)
			self.draw_line(xPixels+7.0, startingY+0.5, xPixels+4.0, startingY-5.5)
			
		
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
		
		
		
	def createWidgets(self):
		self.ripe = True
		frameGrid = Frame(self)
		frameGrid.pack(expand=YES, fill=BOTH)
		
		self.cv = Canvas(frameGrid, bd=1, background='white', width=self.defaultWidth, height=self.defaultHeight)
		hScroll = Scrollbar(frameGrid, orient=HORIZONTAL, command=self.cv.xview)
		vScroll = Scrollbar(frameGrid, orient=VERTICAL, command=self.cv.yview)
		#self.cv.configure(scrollregion=(0, 0, self.completeWidth, self.completeHeight))
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
	def draw_tiearc(self, x0, y0, x1, y1,flipped=False):
		if flipped:
			self.cv.create_arc(self.coordrect(x0, y0, x1, y1),style=ARC,start=180,extent=180)
		else:
			self.cv.create_arc(self.coordrect(x0, y0, x1, y1),style=ARC,start=0,extent=180)
	def draw_text(self, x, y, s):
		textFormat = ('Times New Roman', 9, 'normal')
		self.cv.create_text(self.coord(x,y),text=s, anchor='w', font=textFormat )
		
	def draw_oval(self,x0, y0, x1, y1, bFilled=True): 
		if bFilled:
			self.cv.create_oval(self.coordrect(x0, y0, x1, y1), fill='black' )
		else:
			self.cv.create_oval(self.coordrect(x0, y0, x1, y1) )
	def draw_rect(self,x0, y0, x1, y1): 
		self.cv.create_rectangle(self.coordrect(x0, y0, x1, y1), fill='black' )
	def draw_clef(self, bass_treb):
		map = {2:'16.gif',3:'24.gif',4:'32.gif',5:'40.gif'}
		desiredImageName = bass_treb + map[self.yScale]
		if desiredImageName not in self.clefimgs:
			fname = os.path.join(self.clefspath, desiredImageName)
			self.clefimgs[desiredImageName] = PhotoImage(file=fname)
		
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
	sys.path.append('..')
	
	root = Tk()
	opts = {}
	opts['show_barlines']=1
	opts['show_stems']=1
	opts['show_durations']=1
	
	app = ScoreViewWindow(root, 1)
	
	
	root.mainloop()
	
	