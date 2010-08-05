
try:
	from Tkinter import *
except ImportError:
	from tkinter import *

try:
	import ImageTk
except ImportError:
	from PIL import ImageTk

class PreviewImage(object):
	def __init__(self, top, strName, imagePIL):
		self.name = strName		
		self.imtk = ImageTk.PhotoImage(imagePIL)
		
		
		top.title(self.name)
		
		top.attributes('-toolwindow',1)
		
		self.imControl = Label(top)
		self.imControl.config(image = self.imtk)
		self.imControl.pack()
		
		top.resizable(0,0)
		top.protocol('WM_DELETE_WINDOW',  lambda: False) #prevent from closing!
		
		
	def setImage(self, imagePIL):
		self.imtk = ImageTk.PhotoImage(imagePIL)
		self.imControl.config(image = self.imtk)



	