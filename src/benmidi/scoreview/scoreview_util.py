class NoteRenderInfo():
	def __init__(self, posy, sharpflat):
		self.posy = posy
		self.sharpflat = sharpflat
		
		
class Callable():
	def __init__(self, func, *args, **kwds):
		self.func = func
		self.args = args
		self.kwds = kwds
	def __call__(self, event=None):
		return apply(self.func, self.args, self.kwds)
	def __str__(self):
		return self.func.__name__



def ask_openfile(initialfolder='.',title='Open',types=None):
	"""Open a native file dialog that asks the user to choose a file. The path is returned as a string.
	Specify types in the format ['.bmp|Windows Bitmap','.gif|Gif image'] and so on.
	"""
	import tkFileDialog
	# for the underlying tkinter, Specify types in the format type='.bmp' types=[('Windows bitmap','.bmp')]
	if types!=None:
		aTypes = [(type.split('|')[1],type.split('|')[0]) for type in types]
		defaultExtension = aTypes[0][1]
		strFiles = tkFileDialog.askopenfilename(initialdir=initialfolder,title=title,defaultextension=defaultExtension,filetypes=aTypes)
	else:
		strFiles = tkFileDialog.askopenfilename(initialdir=initialfolder,title=title)
	return strFiles

def ask_savefile(initialfolder='.',title='Save As',types=None):
	"""Open a native file "save as" dialog that asks the user to choose a filename. The path is returned as a string.
	Specify types in the format ['.bmp|Windows Bitmap','.gif|Gif image'] and so on.
	"""
	import tkFileDialog
	if types!=None:
		aTypes = [(type.split('|')[1],type.split('|')[0]) for type in types]
		defaultExtension = aTypes[0][1]
		strFiles = tkFileDialog.asksaveasfilename(initialdir=initialfolder,title=title,defaultextension=defaultExtension,filetypes=aTypes)
	else:
		strFiles = tkFileDialog.asksaveasfilename(initialdir=initialfolder,title=title)
	return strFiles
	
	