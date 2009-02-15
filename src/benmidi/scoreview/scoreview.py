from Tkinter import *
import util

class ScoreViewCanvasAbstract(Frame):
	##SETTINGS
	prefersharpflat = '#'
	shiftnotes = 0
	timestart=0
	timeend=100
	
	#abstract scoreview methods
	def drawBackground(self):
		#draw the clefs
		self.draw_clef(-20, 4, 'treble')
		self.draw_clef(-20, -4, 'bass')
		
		#draw staff lines
		def drawstaffline(y): 
			self.draw_line(0, y, self.getwidth(), y)
		for i in range(1,6): drawstaffline(i*2 + 2); drawstaffline(-2 * i - 2)
			
		#test notes
		#~ self.renderNote(60, 15, 30)
		#~ self.renderNote(61, 30, 30+10)
		#~ self.renderNote(62, 40, 40+10)
		for i in range(14):
			#~ self.renderNote(60 + 19+ i, 10+i*9, 10+i*9+ 10)
			self.renderNote(60- 19 - i, 10+i*9, 10+i*9+ 10)
			
		
		
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
		return util.NoteRenderInfo(posy, sharpflat)
		

#Coord system: 0 is at middle C, each unit is one-half staff ledger line height (one note height).
#Xcoord system: 1 is the spacing 

class ScoreViewCanvasTk(ScoreViewCanvasAbstract):
	def __init__(self, master, xscale, yscale):
		Frame.__init__(self, master)
		self.cv = Canvas(self, bd=1, relief=SUNKEN, background='white')
		self.cv.pack(expand=YES,fill=BOTH)
		
		self.xscale = xscale
		self.yscale = yscale
		#self.ybasis will be set in redraw
		
		self.imgbassclef = PhotoImage(file='bassclef.gif')
		self.imgtrebleclef = PhotoImage(file='trebleclef.gif')
		#Do not redraw right when creating, because we might not know our dimensions
		
	def redraw(self): #To do after every resize/ rescale
		self.clear()
		self.ybasis = int(self.getheight()/2)
		self.drawBackground()
	
	def coord(self, x,y):
		x2 = x*self.xscale + 20 #gives room for the clefs
		y2 = self.ybasis - y*self.yscale
		return (x2, y2)
	def coordrect(self, x0, y0, x1, y1): x0, y0 = self.coord(x0, y0); x1, y1 = self.coord(x1, y1); return (x0,y0, x1,y1)
	def clear(self): self.cv.delete(ALL)
	def getwidth(self): return self.cv.winfo_width()
	def getheight(self): return self.cv.winfo_height()
	def draw_line(self, x0, y0, x1, y1):
		self.cv.create_line(self.coordrect(x0, y0, x1, y1)  )
	def draw_faintline(self, x0, y0, x1, y1):
		self.cv.create_line(self.coordrect(x0, y0, x1, y1),fill="gray", dash=(4, 4)  )
		
	def draw_text(self, x, y, s):
		textFormat = ('Times New Roman', 9, 'normal')
		self.cv.create_text(self.coord(x,y),text=s, anchor='w', font=textFormat )
		
	def draw_oval(self,x0, y0, x1, y1): 
		self.cv.create_oval(self.coordrect(x0, y0, x1, y1), fill='black' )
	def draw_clef(self, x,y, bass_treb):
		if bass_treb=='bass': self.cv.create_image(self.coord(x,y),image=self.imgbassclef)
		else: self.cv.create_image(self.coord(x,y),image=self.imgtrebleclef)
			

if __name__=='__main__':
	class TestApp:
		def __init__(self, root):
			root.title('Testing score view')
			frameTop = Frame(root, padx='0m' , height=400)
			frameTop.pack(expand=YES, fill=BOTH)
			self.score = ScoreViewCanvasTk(frameTop, 1,4 )
			self.score.pack(expand=YES, fill=BOTH)
			Button(frameTop, text='draw', command=self.score.redraw).pack()
			Button(frameTop, text='test', command=self.test).pack()
			
			
		def test(self):
			print 'l'
			print self.score.cv.winfo_width()

	
	root = Tk()
	app = TestApp(root)
	root.mainloop()