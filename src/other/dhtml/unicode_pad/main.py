"""
MiniMath
Ben Fisher, 2007
License: GPL
"""
from Tkinter import *
import tkFileDialog
import tkMessageBox
import ScrolledText
import codecs

from wrappers import *
import layouts
import keymaps


class App:
	currentMode = ''
	currentFilename = ''
	currentSaved = True
	currentMap, currentMapname = 'default.py.js', ''
	manualbindings = {}
	dictModes = None
	dictHotkeys = None
	textFormat = ('Times New Roman', 12, 'normal')
	undoStack = []
	undoIndex = -1
	enableUndo = True
	def __init__(self, root):
		root.title('Unicode Pad')
		
		frameMain = Frame(width=400, height=100)
		frameMain.pack(side=TOP,fill=BOTH, expand=True)
		self.txtContent = ScrolledText.ScrolledText(frameMain, wrap=WORD)
		self.txtContent.pack(side=TOP,fill=BOTH, expand=True)
		self.txtFilename = Label(frameMain, text="Untitled")
		self.txtFilename.pack(side=TOP)
		self.txtMode = Label(frameMain)
		self.txtMode.pack(side=TOP)
		
		self.dictModes, self.dictHotkeys, self.currentMapname = keymaps.parse()
		self._updateMode()
		
		root.bind_all('<Key>', self.onkey_normal)
		root.bind_all('<Any-Alt-Key>', self.onkey_normal)
		root.bind_all('<Tab>', self.onkey_normal)
		root.bind_all('<Shift-Tab>', self.onkey_normal)
		self.txtContent.bind('<Key>', self.onkey_normal)
		self.txtContent.bind('<Any-Alt-Key>', self.onkey_normal)
		self.txtContent.bind('<Tab>', self.onkey_normal)
		self.txtContent.bind('<Shift-Tab>', self.onkey_normal)
		
		self._create_menubar(root)
		
		self.txtContent['font'] = self.textFormat #Update font
		
	def _create_menubar(self,root):
		#~ root.bind('<Control-Key-o>', wrappers.Callable(self.open_shape))
		root.bind('<Alt-F4>', root.quit)
		
		menubar = Menu(root)
		menuFile = Menu(menubar, tearoff=0)
		menuFile.add_command(label="New", command=self.new_file, underline=0, accelerator='Ctrl+N')
		menuFile.add_command(label="Open", command=self.open_file, underline=0, accelerator='Ctrl+O')
		menuFile.add_command(label="Save", command=self.save_file, underline=0, accelerator='Ctrl+S')
		menuFile.add_command(label="Save As...", command=Callable(self.save_file,True), underline=6, accelerator='Ctrl+Shift+S')
		menuFile.add_separator()
		menuFile.add_command(label="Exit", command=root.quit, underline=1)
		menubar.add_cascade(label="File", menu=menuFile, underline=0)
		
		menuEdit = Menu(menubar, tearoff=0)
		menuEdit.add_command(label="Undo", command=self.edit_undo, underline=0, accelerator='Ctrl+Z')
		menuEdit.add_command(label="Redo", command=self.edit_redo, underline=0, accelerator='Ctrl+Y')
		menubar.add_cascade(label="Edit", menu=menuEdit, underline=0)
		
		menuFont = Menu(menubar, tearoff=0)
		menuFont.add_command(label="Smaller", command=Callable(self.changeFontSize,'-'), accelerator='Ctrl+[')
		menuFont.add_command(label="Larger", command=Callable(self.changeFontSize,'+'), accelerator='Ctrl+]')
		menuFont.add_command(label="Change...", command=self.changeFont)
		menubar.add_cascade(label="Font", menu=menuFont, underline=0)
		
		menuBindings = Menu(menubar, tearoff=0)
		menuBindings.add_command(label="List modes", command=Callable(self.show_bindings,'modes'))
		menuBindings.add_command(label="List bindings", command=Callable(self.show_bindings,'bindings'), underline=0)
		menuBindings.add_command(label="List ASCII", command=Callable(self.show_bindings,'ascii'))
		menuBindings.add_command(label="Selected Text...", command=self.analyzeText)
		menuBindings.add_command(label="Insert Character...", command=self.insertCharacter)
		menuBindings.add_separator()
		menuBindings.add_command(label="Visualize Bindings", command=self.viz_bindings)
		menuBindings.add_command(label="Edit Key Bindings", command=self.edit_bindings)
		menuBindings.add_separator()
		for filename in keymaps.get_available():
			menuBindings.add_command(label=filename.replace('.py.js',''), command=Callable(self.change_keymap,filename))
		menubar.add_cascade(label="Characters", menu=menuBindings, underline=0)
		
		menuHelp = Menu(menubar, tearoff=0)
		menuHelp.add_command(label='About', command=(lambda: tkMessageBox.showinfo('Unicode Pad','Unicode Pad, by Ben Fisher 2007')))
		menuHelp.add_command(label='Help', command=self.showDocs)
		menuHelp.add_separator()
		menuHelp.add_command(label="Visualize Bindings", command=self.viz_bindings)
		menubar.add_cascade(label="Help", menu=menuHelp, underline=0)
		
		self.manualbindings.update({'Alt+F4':root.quit, 'Control+O':self.open_file,'Control+N':self.new_file, 'Control+A':self.selectAll, 'Control+S':self.save_file, 'Control+Shift+S':Callable(self.save_file,True),
			'Control+[':Callable(self.changeFontSize,'-'),'Control+]':Callable(self.changeFontSize,'+'),
			'Control+Z':self.edit_undo, 'Control+Y':self.edit_redo,
			})
		
		root.config(menu=menubar)
		
	def _gettext(self):
		return self.txtContent.get(1.0, END)
	def _settext(self, strText):
		self.txtContent.delete('1.0', END)
		self.txtContent.insert(END, strText)
	def _updatefilename(self):
		if self.currentFilename!='':
			strName = self.currentFilename  
		else:
			strName ='Untitled'
		if not self.currentSaved: strName += ' *'
		self.txtFilename.config(text=strName)
	def selectAll(self):
		self.txtContent.tag_add(SEL, '1.0', 'end-1c')
		self.txtContent.mark_set(INSERT, '1.0')
		self.txtContent.see(INSERT)
		return 'break'
		
	def changeFontSize(self, relsize):
		if relsize == '+':
			self.textFormat = (self.textFormat[0], self.textFormat[1] + 2,self.textFormat[2])
		else:
			self.textFormat = (self.textFormat[0], self.textFormat[1] - 2,self.textFormat[2])
		self.txtContent['font'] = self.textFormat
	def changeFont(self):
		import tkSimpleDialog
		newfont = tkSimpleDialog.askstring('Font:','Choose new font (i.e. Verdana).')
		if not newfont: return
		self.textFormat = (newfont, self.textFormat[1],self.textFormat[2])
		self.txtContent['font'] = self.textFormat
	
	def checkSaved(self):
		if self.currentSaved: return True
		ret = tkMessageBox._show("Pad", "Save changes to file?",
		icon=tkMessageBox.QUESTION, 
		type=tkMessageBox.YESNOCANCEL)
		if ret=='yes':
			ret = self.save_file()
			if not ret: return False
			else: return True
		elif ret == 'no':
			return True
		elif ret == 'cancel':
			return False

	
	def new_file(self):
		if not self.checkSaved():
			return False
			
		self._settext('')
		self.currentSaved = True
		self.currentFilename = ''
		self._updatefilename()
		self.enableUndo = True
		return True
	def save_file(self, saveAs = False):
		if self.currentFilename == '' or saveAs:
			strFileName = tkFileDialog.asksaveasfilename()
			if not strFileName: return False
			self.currentFilename = strFileName
			
		f = codecs.open(self.currentFilename, 'w','utf-8')
		f.write(self._gettext())
		f.close()
		self.currentSaved = True
		self._updatefilename()
	def open_file(self, strFileName = None):
		if not self.checkSaved():
			return False
		if strFileName==None:
			strFileName = tkFileDialog.askopenfilename()
			if not strFileName: return False
		
		
		f = codecs.open(strFileName, 'r')
		alltext = f.read()
		f.close()
		if len(alltext) > 1000000:
			tkMessageBox.showinfo('Warning: this file is large, so Undo/Redo are disabled!')
			self.enableUndo = False
		else:
			self.enableUndo = True
		
		self._settext(alltext)
		self.currentFilename = strFileName
		self.currentSaved = True
		self._updatefilename()
		return True
		
	def onkey_normal(self, event):
		if event.keycode==16 or event.keycode==17 or event.keycode==0:
			return
			
		mods = ''
		if event.state & 0x00000004: mods += 'Control+'
		if event.state & 0x00020000: mods += 'Alt+'
		if event.state & 0x00000001: mods += 'Shift+'
		
		keypressed = mods + layouts.layout.get(event.keycode,'')
		if keypressed=='': return #unrecognized key
			
		if mods == '' and self.currentSaved==True and len(event.char)==1:
			self.currentSaved = False
			self._updatefilename()
			
		if mods == '' and len(event.char)==1: self.check_undo_event(event)
		
		if keypressed in self.manualbindings:
			self.manualbindings[keypressed]()
			return 'break'
			
		if self.currentMode + '/' + keypressed in self.dictHotkeys:
			self.insert_character(self.dictHotkeys[self.currentMode + '/' + keypressed])
			return 'break'
			
		if keypressed in self.dictHotkeys:
			hotkey = self.dictHotkeys[keypressed]
			if hotkey[0]=='setmode':
				self.change_mode(hotkey[1])
				return 'break'
			elif hotkey[0] == 'char':
				self.insert_character(hotkey)
				return 'break'

	def change_mode(self, newmode):
		self.currentMode = newmode
		self._updateMode()
		
	def _updateMode(self):
		self.txtMode.config(text='Map: ' + self.currentMapname + ', Mode:' + self.dictModes[self.currentMode])
	
	def insert_character(self, hotkey):
		try:
			# If there is a selection, erase it
			self.txtContent.delete( self.txtContent.index("sel.first"), self.txtContent.index("sel.last"))
		except TclError:
			# There was no text selected, so simply insert the character
			pass
		char = hotkey[1]
		self.txtContent.insert(INSERT, unichr(char))
		return 'break'
		
	def show_bindings(self, strParam):
		ret = self.new_file()
		if ret==False: return
		
		if strParam == 'ascii':
			for i in range(32,256):
				self.txtContent.insert(INSERT, str(i) +': '+ unichr(i) + '\n')
		elif strParam == 'modes':
			modekeys = [(hotkey,self.dictHotkeys[hotkey]) for hotkey in self.dictHotkeys if self.dictHotkeys[hotkey][0]=='setmode']
			strShow = ''
			for hotkey in modekeys:
				strShow += hotkey[0].replace(' ','Space') + ' (' + self.dictModes[hotkey[1][1]] + ') \n'
			self.txtContent.insert(INSERT,strShow)
		elif strParam == 'bindings':
			self.txtContent.insert(INSERT, str(self.dictHotkeys).replace("'),","'),\n"))
	
	def edit_bindings(self):
		import os
		self.open_file(os.path.join('keymaps',self.currentMap))
	def viz_bindings(self):
		import os
		kdir = os.path.join (os.path.abspath(os.path.dirname(sys.argv[0])), 'keymaps')
		kf = os.path.join(kdir, 'visualize.html')
		# Copy file to js:
		try:
			import shutil
			shutil.copy(os.path.join(kdir, self.currentMap), os.path.join(kdir, '_current.js'))
		except:
			print 'Warning: could not copy file.'
		if os.path.exists(kf):
			makeThread(os.system, kf)
		else:
			tkMessageBox.showinfo(message='Could not find visualize.html.')
			
	def change_keymap(self, filename):
		import os
		self.dictModes, self.dictHotkeys, self.currentMapname = keymaps.parse(filename)
		self.currentMode = ''
		self.currentMap = filename
		self._updateMode()
		
	def check_undo_event(self, evt):
		if self.enableUndo and (len(self.undoStack)==0 or evt.time - self.undoStack[0][0] > 5000):
			self.undoStack.insert(0,(evt.time, self._gettext()))
			self.undoIndex = 0
			if len(self.undoStack)>100:
				self.undoStack = self.undoStack[0:100]
		
	def edit_undo(self):
		if self.enableUndo:
			if len(self.undoStack)==0: return
			if self.undoIndex < len(self.undoStack)-1:
				self.undoIndex +=1
			self._settext(self.undoStack[self.undoIndex][1])
			
	def edit_redo(self):
		if self.enableUndo:
			if self.undoIndex <= 0: return
			if self.undoIndex > 0:
				self.undoIndex -=1
			self._settext(self.undoStack[self.undoIndex][1])
	def analyzeText(self):
		try:
			strText = self.txtContent.get( self.txtContent.index("sel.first"), self.txtContent.index("sel.last"))
		except:
			return
		tkMessageBox.showinfo('Values', ', '.join([str(ord(c)) for c in strText]))
			
	def insertCharacter(self):
		import tkSimpleDialog
		chars = tkSimpleDialog.askstring('Characters:','Enter Unicode values, separated by commas.(i.e. 293,111,349)')
		if not chars: return
		self.txtContent.insert(INSERT, ''.join([unichr(int(char)) for char in chars.replace(' ','').split(',')]))
	
	def showDocs(self):
		strDocs = """
Unicode Pad, By Ben Fisher

This program is a lightweight text editor intended for writing text in other languages. Most word processors have a "Insert Symbol" option for inserting a character, but this process is too slow. If you do all of your typing in another language, one can set the system language, but this will be a system-wide change and is not very customizeable. In this program, on the other hand, it is simple to set up your own keymaps and choose what keys create which characters. One can also conveniently visualize these bindings.

Because some languages require access to more than 26 symbols, this program uses the concept of "modes." For example, the default keymap has a mode called Grave Accent. In this mode, typing a vowel like o will produce o with a grave accent. The program begins in Normal Mode, but you can press Control+L to enter Grave Accent mode. View the available modes for the current keymap by choosing List Modes from the Characters menu.

When you enter a mode, you stay in that mode until you press the key combination, typically Control+Space, to return to Normal mode.

Edit the current keymap by choosing "Edit key bindings" from the Characters menu. (Changes take place when the mode is selected from the Characters menu). Create a new map by creating a .py.js file in the keymaps directory.
		"""
		self._displayContent(strDocs)
		
	def _displayContent(self, strText):
		ret = self.new_file()
		if ret==False: return
		self._settext(strText)
	
root = Tk()
app = App(root)

root.mainloop()