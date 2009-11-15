import Image, ImageDraw
import random

def draw(listQuantize, listPulses, listNotes, listNotesQuantized, listFinal):

	w,h = (10000, 200)
	im = Image.new('RGB', (w,h*3) , 0xffffff)

	maxx = listQuantize[-1]
	def convCoord(x):
		return int((x/maxx) * w)
	def convCoordQ(x):
		return convCoord(listQuantize[x])
	def  convCoordQRegular(x):
		return int((float(x)/(len(listQuantize)-1)) * w)
		
	def getnote(x): return {60:0,62:1,64:2,65:3}.get(x,3)

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


def textrender(listFinal):
	#print output
	def renderRhythm(n):
		return str( float(n)/quantize)
	def renderNote(n): return ''.join(map(str,music_util.noteToName(n)))
	prevTime = 0
	for note in listFinal:
		if note[1]!=prevTime:
			print 'Rest, %s beats'%renderRhythm(note[1]-prevTime)
		print 'Notes, %s, %s beats'%(','.join(map(renderNote, note[0])), renderRhythm(note[2]-note[1]))
		prevTime = note[2]



if __name__=='__main__':
	draw(None, None, None)

