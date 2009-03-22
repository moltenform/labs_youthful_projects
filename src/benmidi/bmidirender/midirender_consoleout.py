
from Tkinter import *
import ScrolledText




from Tkinter import *


class BConsoleOutWindow():
	def __init__(self, top,callbackRefresh, callbackOnClose=None):
		top.title('Timidity console output')
		frameTop = Frame(top, padx='15',pady='15' )
		frameTop.pack(expand=YES, fill=BOTH)
		
		#perhaps use a monospace font
		self.txt = ScrolledText.ScrolledText(frameTop,  width=60, height=10)
		self.txt.pack(expand=YES, fill=BOTH)
		
		Button(frameTop, text='Refresh', command=callbackRefresh).pack()
		
		top.bind('<MouseWheel>',self.scroll) #binding for Windows
		top.bind('<Button-4>',self.scroll) #binding for Linux
		top.bind('<Button-5>',self.scroll)
		
		if callbackOnClose!=None:
			def callCallback():
				callbackOnClose()
				top.destroy()
			top.protocol("WM_DELETE_WINDOW", callCallback)
	
	
	def writeToWindow(self, s):
		self.txt.insert(END, s)
	def clear(self):
		self.txt.delete(1.0,END)
		
	def scroll(self, event):
		if event.num == 5 or event.delta == -120:
			self.txt.yview_scroll(5, 'units')
		if event.num == 4 or event.delta == 120:
			self.txt.yview_scroll(-5, 'units')



		



if __name__=='__main__':
	
	def callback():
		global window
		window.clear()
		window.writeToWindow('hi there')
	
	def openit():
		global window
		top = Toplevel()
		window = BConsoleOutWindow(top, callback)
		window.writeToWindow('hello')

	
	root = Tk()
	Button(root, text='open', command=openit).pack()
	root.mainloop()
	

