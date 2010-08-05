
try:
	import Tkinter as tkinter
except ImportError:
	import tkinter

def bindShortcuts(root, d):
	for key in d:
		root.bind(key, lambda evt=None, fn=d[key]: fn() ) #it doesn't take an arg
	#effect is something like:
	#root.bind('<Control-r>', self.onBtnSaveWave)
	
def addMenuItems(menu, l):
	assert len(l)%2==0 #length is even
	for menuname, function in _group(l, 2): #iterate through l, two at a time
		if menuname=='_':
			menu.add_separator()
			continue
		
		add_command_opts = {}
		if '|' in menuname:
			menuname, menuaccel = menuname.split('|')
		else:
			menuname, menuaccel = menuname, ''
		menuname, underlinepos = _stringToUnderlinePosition(menuname)
		
		if underlinepos!=-1: add_command_opts['underline'] = underlinepos
		if menuaccel!='': add_command_opts['accelerator'] = menuaccel
		
		menu.add_command(label=menuname, command=function, **add_command_opts)
		
		#effect is something like:
		#menu.add_command(label="Mixer", command=self.openMixerView, underline=0, accelerator='Ctrl+M')
		

def _group(iterable, size):
    import itertools
    it = iter(iterable)
    item = list(itertools.islice(it, size))
    while item:
        yield item
        item = list(itertools.islice(it, size))

def _stringToUnderlinePosition(s):
	# If given a string '&hello', underline the h.
	if '&' in s and '&&' not in s:
		pos = s.find('&')
		text = s.replace('&','',1)
		return text, pos
	else:
		return s , -1
		
class Callable(object):
	def __init__(self, func, *args, **kwds):
		self.func = func
		self.args = args
		self.kwds = kwds
	def __call__(self, event=None):
		return self.func(*self.args, **self.kwds)
	def __str__(self):
		return self.func.__name__

class PseudoFile(object):
	def __init__(self, writefn, name='', encoding=None):
		self.writefn = writefn
		self.name = name
	def write(self, s):
		self.writefn(s)
	def writelines(self, l):
		list(map(self.write, l))
	def flush(self):
		pass
	def isatty(self):
		return True

def makeThread(fn, args):
	import threading
	class MyThread(threading.Thread):
		"""this is a wrapper for threading.Thread that improves the syntax for creating and starting threads - Allen Downey"""
		def __init__(self, target, *args):
			threading.Thread.__init__(self, target=target, args=args)
			self.start()
	MyThread(fn, args)
	
def isDirectory(dir_name):
	import os
	mask = 0o40000
	try: s = os.stat(dir_name)
	except: return 0
	if (mask & s[0]) == mask: return 1
	else: return 0
	
def select_all_binding(fld):
	"""Add Ctrl+A -> select all binding"""
	def sel(fld):
		"""Select all text in the field."""
		fld.basecontrol.tag_add(SEL, '1.0', 'end-1c')
		fld.basecontrol.mark_set(INSERT, '1.0')
		fld.basecontrol.see(INSERT)
		return 'break'
	fld.bind('<Control-a>',lambda event: sel(fld), '+')
		
def gettext(fld):
	return fld.get(1.0, tkinter.END)

def settext(fld,txt):
	fld.delete('1.0', tkinter.END)
	fld.insert(tkinter.END, txt)


		