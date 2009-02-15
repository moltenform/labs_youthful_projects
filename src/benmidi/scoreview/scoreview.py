from Tkinter import *
import util

class ScoreViewCanvasAbstract(Frame):
	#abstract scoreview methods
	def setup(self):
		#draw the clefs
		self.draw_clef(1, 3, 'treble')
		self.draw_clef(1, -3, 'bass')
		
		#~ self.drawStafflines()
		#~ self.draw_text(
		

#Coord system: 0 is at middle C, each unit is one staff ledger line height.
#Xcoord system: 1 is the spacing 

class ScoreViewCanvasTk(ScoreViewCanvasAbstract):
	def __init__(self, master, xscale, yscale):
		Frame.__init__(self, master)
		self.cv = Canvas(self, bd=1, relief=SUNKEN, background='white')
		self.cv.pack(expand=YES,fill=BOTH)
		
		self.xscale = xscale
		self.yscale = yscale
		#self.ybasis will be set in redraw
		
		self.imgbassclef = PhotoImage('bassclef.gif')
		self.imgtrebleclef = PhotoImage('trebleclef.gif')
		self.redraw()
		
		self.cv.create_line((0,20,25,45))
		self.cv.create_line((0,20,25,45))
		#~ self.draw_oval(0,0,30,40)
		self.draw_faintline(0,0,30,40)
		#~ self.draw_text(90,90,u'\xe9')
		#~ self.draw_text(90,90,u'\u1d15d')
		#~ self.draw_text(90,90,unichr(119133))
		
		
	def redraw(self): #To do after every resize/ rescale
		self.clear()
		self.ybasis = int(self.getheight)/2
	
	def coord(self, x,y):
		x2 = x*self.xscale
		y2 = self.ybasis - y*yscale
		return (x2, y2)
	def coordrect(self, x0, y0, x1, y1): x0, y0 = self.coord(x0, y0); x1, y1 = self.coord(x1, y1); return (x0,y0, x1,y1)
	def clear(self): self.cv.delete(ALL)
	def getwidth(self): return self.score.cv.winfo_width()
	def getheight(self): return self.score.cv.winfo_height()
	def draw_line(self, x0, y0, x1, y1):
		self.cv.create_line(self.coordrect(x0, y0, x1, y1)  )
	def draw_faintline(self, x0, y0, x1, y1):
		self.cv.create_line(self.coordrect(x0, y0, x1, y1),fill="gray", dash=(4, 4)  )
		
	def draw_text(self, x, y, s):
		textFormat = ('Times New Roman', 30, 'normal')
		self.cv.create_text(self.coord(x,y),text=s, anchor='nw', font=textFormat )
		
	def draw_oval(self,x0, y0, x1, y1): 
		self.cv.create_oval(self.coordrect(x0, y0, x1, y1), fill='black' )
	def draw_clef(self, x,y, bass_treb):
		if bass_treb=='bass': self.cv.create_image(self.coord(x,y),image=self.imgbassclef)
		else: self.cv.create_image(self.coord(x,y),image=self.imgtrebleclef)

if __name__=='__main__':
	class TestApp:
		def __init__(self, root):
			root.title('Testing score view')
			frameTop = Frame(root, padx='0m' , width=400, height=400)
			frameTop.pack(expand=YES, fill=BOTH)
			self.score = ScoreViewCanvasTk(frameTop, 1,3 )
			self.score.pack(expand=YES, fill=BOTH)
			btn = Button(frameTop, text='test', command=self.test)
			btn.pack()
			
		def test(self):
			print 'l'

	
	root = Tk()
	app = TestApp(root)
	root.mainloop()