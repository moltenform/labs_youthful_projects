		
		
class Callable():
	def __init__(self, func, *args, **kwds):
		self.func = func
		self.args = args
		self.kwds = kwds
	def __call__(self, event=None):
		return apply(self.func, self.args, self.kwds)
	def __str__(self):
		return self.func.__name__

def makeThread(fn, args=None): #fn, args as a tuple
	if args==None: args=tuple()
	import threading
	t = threading.Thread( target=fn, args=args)
	t.start()
	
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
	
def ask_float(prompt, default=None, min=0.0,max=100.0, title=''):
	""" Get input from the user, validated to be an float (decimal number). default refers to the value which is initially in the field. By default, from 0.0 to 100.0; change this by setting max and min. Returns None on cancel."""
	import tkSimpleDialog
	if default:
		return tkSimpleDialog.askfloat(title, prompt, minvalue=min, maxvalue=max, initialvalue=default)
	else:
		return tkSimpleDialog.askfloat(title, prompt, minvalue=min, maxvalue=max)

from Tkinter import *
class ScrolledListbox(Listbox): #an imitation of ScrolledText
	def __init__(self, master=None, cnf=None, **kw):
		if cnf is None:
			cnf = {}
		if kw:
			from Tkinter import _cnfmerge
			cnf = _cnfmerge((cnf, kw))
		fcnf = {}
		for k in cnf.keys():
			if type(k) == ClassType or k == 'name':
				fcnf[k] = cnf[k]
				del cnf[k]
		self.frame = Frame(master, **fcnf)
		self.vbar = Scrollbar(self.frame, name='vbar')
		self.vbar.pack(side=RIGHT, fill=Y)
		cnf['name'] = 'lbox'
		Listbox.__init__(self, self.frame, **cnf)
		self.pack(side=LEFT, fill=BOTH, expand=1)
		self['yscrollcommand'] = self.vbar.set
		self.vbar['command'] = self.yview

		# Copy geometry methods of self.frame -- hack!
		methods = Pack.__dict__.keys()
		methods = methods + Grid.__dict__.keys()
		methods = methods + Place.__dict__.keys()

		for m in methods:
			if m[0] != '_' and m != 'config' and m != 'configure':
				setattr(self, m, getattr(self.frame, m))