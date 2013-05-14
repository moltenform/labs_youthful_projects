		
class Callable():
	def __init__(self, func, *args, **kwds):
		self.func = func
		self.args = args
		self.kwds = kwds
	def __call__(self, event=None):
		return apply(self.func, self.args, self.kwds)
	def __str__(self):
		return self.func.__name__

class Interpreter():
	"""this object encapsulates the environment where user-provided code will execute"""
	def __init__(self, locals):
		self.locals = locals

	def run_code(self, source):
		import exceptions
		code = compile(source, '<user-provided code>', 'exec')
		try:
			exec code in self.locals
		except KeyboardInterrupt:
			pass
		except exceptions.Exception,e:
			return e 
		return True



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

def alert(message, title=None, icon='info'):
	""" Show dialog with information. Icon can be one of 'info','warning','error', defaulting to 'info'."""
	import tkMessageBox
	if icon=='info':
		return tkMessageBox.showinfo(title=title, message=message)
	elif icon=='warning':
		return tkMessageBox.showwarning(title=title, message=message)
	elif icon=='error':
		return tkMessageBox.showerror(title=title, message=message)
	
def ask_yesno(prompt, title=None):
	""" Ask yes or no. Returns True on yes and False on no."""
	import tkMessageBox
	return tkMessageBox.askyesno(title=title, message=prompt)