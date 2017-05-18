
import sys
import os

try:
	from Tkinter import *
	from Tkinter import _cnfmerge
	import tkFileDialog
	import tkMessageBox
	import tkSimpleDialog
except ImportError:
	from tkinter import *
	from tkinter import _cnfmerge
	import tkinter.filedialog as tkFileDialog
	import tkinter.messagebox as tkMessageBox
	import tkinter.simpledialog as tkSimpleDialog

import threading

if getattr(sys, 'frozen', False):
    bmidirenderdirectory = os.path.dirname(sys.executable)
elif __file__:
    bmidirenderdirectory = os.path.dirname(__file__)



sys.path.append('..')
from bmidilib import bmidilib, bmiditools
sys.path.pop()

rememberLastDirectory = {}

class Callable(object):
	def __init__(self, func, *args, **kwds):
		self.func = func
		self.args = args
		self.kwds = kwds

	def __call__(self, event=None):
		return self.func(*self.args, **self.kwds)

	def __str__(self):
		return self.func.__name__

def makeThread(fn, args=None): #fn, args as a tuple
	if args == None:
		args = tuple()
	t = threading.Thread( target=fn, args=args)
	t.start()
	
gDirectoryHistory = dict()
def _getFileDialogGui(fn, initialdir, types, title):
    if initialdir is None:
        initialdir = gDirectoryHistory.get(repr(types), '.')
    
    kwargs = dict()
    if types is not None:
        aTypes = [(type.split('|')[1], type.split('|')[0]) for type in types]
        defaultExtension = aTypes[0][1]
        kwargs['defaultextension'] = defaultExtension
        kwargs['filetypes'] = aTypes
    
    result = fn(initialdir=initialdir, title=title, **kwargs)
    if result:
        gDirectoryHistory[repr(types)] = os.path.split(result)[0]
        
    return result

def ask_openfile(initialfolder=None, types=None, title='Open'):
    "Specify types in the format ['.png|Png image','.gif|Gif image'] and so on."
    if isPy3OrNewer:
        import tkinter.filedialog as tkFileDialog
    else:
        import tkFileDialog
    return _getFileDialogGui(tkFileDialog.askopenfilename, initialfolder, types, title)

def ask_savefile(initialfolder=None, types=None, title='Save As'):
    "Specify types in the format ['.png|Png image','.gif|Gif image'] and so on."
    if isPy3OrNewer:
        import tkinter.filedialog as tkFileDialog
    else:
        import tkFileDialog
    return _getFileDialogGui(tkFileDialog.asksaveasfilename, initialfolder, types, title)

def alert(message, title=None, icon='info'):
    "Show dialog with information. Icon can be one of 'info','warning','error', defaulting to 'info'."
    if isPy3OrNewer:
        from tkinter import messagebox as tkMessageBox
    else:
        import tkMessageBox
    if icon=='info':
        return tkMessageBox.showinfo(title=title, message=message)
    elif icon=='warning':
        return tkMessageBox.showwarning(title=title, message=message)
    elif icon=='error':
        return tkMessageBox.showerror(title=title, message=message)

def ask_yesno(prompt, title=None):
	""" Ask yes or no. Returns True on yes and False on no."""
	return tkMessageBox.askyesno(title=title, message=prompt)
	
def ask_float(prompt, default=None, min=0.0,max=100.0, title=''):
	""" Get input from the user, validated to be an float (decimal number). default refers to the value which is initially in the field. By default, from 0.0 to 100.0; change this by setting max and min. Returns None on cancel."""
	if default:
		return tkSimpleDialog.askfloat(title, prompt, minvalue=min, maxvalue=max, initialvalue=default)
	else:
		return tkSimpleDialog.askfloat(title, prompt, minvalue=min, maxvalue=max)

isPy3OrNewer = sys.version_info[0] > 2

class ScrolledListbox(Listbox): #an imitation of ScrolledText
	def __init__(self, master=None, cnf=None, **kw):
		if cnf is None:
			cnf = {}
		if kw:
			cnf = _cnfmerge((cnf, kw))
		fcnf = {}
		for k in list(cnf.keys()):
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
		methods = list(Pack.__dict__.keys())
		methods = methods + list(Grid.__dict__.keys())
		methods = methods + list(Place.__dict__.keys())

		for m in methods:
			if m[0] != '_' and m != 'config' and m != 'configure':
				setattr(self, m, getattr(self.frame, m))
