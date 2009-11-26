import Image, ImageDraw
import random

import copy

w,h = (10000, 200)
class TestDepiction():
	def __init__(self):
		pass
	def addPreQuantize(self,listSteps, listNoteEvents):
		self.listSteps = copy.deepcopy(listSteps)
		self.listPrequantized = [(note.pitch, note.start,note.end) for note in listNoteEvents]
	
	def addQuantized(self,listNoteEvents):
		self.listPostquantized = [(note.pitch, note.start,note.end) for note in listNoteEvents]
	def draw(self,listPulses, listFinal ):
		im = Image.new('RGB', (w,h*3) , 0xffffff)
		
		listQuantize = self.listSteps
		listPulses = listPulses
		listNotes = self.listPrequantized
		listNotesQuantized = self.listPostquantized
		listFinal = listFinal
		

		maxx = listQuantize[-1]
		def convCoord(x):
			return int((x/maxx) * w)
		def convCoordQ(x):
			return convCoord(listQuantize[x])
		def  convCoordQRegular(x):
			return int((float(x)/(len(listQuantize)-1)) * w)
		
		self.prevpitch=0; self.curlevel=0
		def getnote(x):
			if x!=self.prevpitch: self.curlevel= (self.curlevel+1)%4
			self.prevpitch=x
			return self.curlevel

		draw = ImageDraw.Draw(im)
		#~ draw.line((0, 0) + im.size, fill=128)
		#~ draw.line((0, im.size[1], im.size[0], 0), fill=128)
		
		for q in listQuantize:
			x= convCoord(q)
			draw.line( (x,0,x,h), fill=0xaaaaaa)
		
		for q in listPulses:
			x= convCoord(q)
			draw.line( (x,0,x,h/16), fill=0x4444ff, width=4)
		
		for note in listNotes:
			start, stop = convCoord(note[1]),convCoord(note[2])
			y=h/2 - (getnote(note[0])*h/8)  
			draw.rectangle( (start,y,stop,y+h/8), fill=0x44ff44)
		
		
		for note in listNotesQuantized:
			start, stop = convCoordQ(note[1]),convCoordQ(note[2])
			y=h/2 - (getnote(note[0])*h/8) 
			y+=h/2
			draw.rectangle( (start,y,stop,y+h/8), fill=0xff4444)
		
		
		#draw "final" version
		for q in range(len(listQuantize)):
			x= convCoordQRegular(q)
			draw.line( (x,h*2,x,h*3), fill=0xaaaaaa)
		
		for note in listFinal:
			start, stop = convCoordQRegular(note[1]),convCoordQRegular(note[2])
			for pitch in note[0]:
				y=h/2 - (getnote(pitch)*h/8) 
				draw.rectangle( (start,y + h*2,stop,y+h/8+ h*2), fill=0xffff44)
		
		
		del draw 
		im.save('out.png', "PNG")





if __name__=='__main__':
	pass
	
