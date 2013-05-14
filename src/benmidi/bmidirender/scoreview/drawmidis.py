#Get a note, length. Other settings are a 
#
#



LEDGERLINE_WIDTH = 4
NOTEHEAD_WIDTH = 6
NOTEHEAD_HEIGHT = 5
LINESPACE = 3



class NoteRenderer():
'''Only intended to have one instance of this'''
	scalex = 1.0
	scaley = 1.0
	basey = 200 #where to draw middle C
	
	def __init__(self, canvas):
		self.cv = canvas
		
		
	#public methods
	def setupStaffs(self):
		self.drawStafflines()
	
	def renderNote(self, starttimecode, endtimecode, 
		
	#middle C is note 60, returns a position of 0
	def getnoteposition(self,note, shift, prefersharpflat):
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
			
		return NoteRenderInfo(posy, sharpflat)
	
	
	
	
	
	
	
	
	
	#Private methods
	def drawStafflines(self):
		
		drawLine(cv, 
	
	def drawNotehead(self, x,y
		self.cv.create_oval(, fill is black)
	def drawLedgerline(self
	def drawSharpsymbol(self, x,y, sharpflat):
		x = x-(NOTEHEAD_WIDTH)*self.scalex #show it a bit to the left
		if sharpflat=='#': textToRender = '#'
		else: textToRender = 'b'
		self.cv.create_text((x,y),text=textToRender, anchor='NW', font=('Times New Roman',10) )
	
	#wrappers for Tkinter
	def drawLine(
	def drawDuration 

	
if __name__=='__main__':
	print getnoteposition(60, 0, 'b')
	print getnoteposition(61, 0, 'b')
	print getnoteposition(61, 0, '#')
	print getnoteposition(60 + 12, 0, 'b')
	print getnoteposition(61 + 12, 0, 'b')
	print getnoteposition(61 + 12, 0, '#')
	
	
	


