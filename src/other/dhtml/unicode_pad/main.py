"""
UnicodePad
Ben Fisher, 2007

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
	manualbindings = {}
	dictModes = None
	dictHotkeys = None
	textFormat = ('Times New Roman', 12, 'normal')
	undoStack = []
	def __init__(self, root):
		
		root.title('Unicode Pad')
		
		
		
		frameMain = Frame(width=400, height=100)
		frameMain.pack(side=TOP,fill=BOTH, expand=True)
		self.txtContent = ScrolledText.ScrolledText(frameMain)
		self.txtContent.pack(side=TOP,fill=BOTH, expand=True)
		self.txtFilename = Label(frameMain, text="Untitled")
		self.txtFilename.pack(side=TOP)
		self.txtMode = Label(frameMain, text='Normal Mode')
		self.txtMode.pack(side=TOP)
		self.txtMode.bind('<Button-1>', Callable(self.showModes))
		
		self.dictModes, self.dictHotkeys = keymaps.parse()
		
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
		
		menuFont = Menu(menubar, tearoff=0)
		menuFont.add_command(label="Smaller", command=Callable(self.changeFontSize,'-'), accelerator='Ctrl+[')
		menuFont.add_command(label="Larger", command=Callable(self.changeFontSize,'+'), accelerator='Ctrl+]')
		menubar.add_cascade(label="Font", menu=menuFont, underline=0)
		
		menuBindings = Menu(menubar, tearoff=0)
		menuBindings.add_command(label="List modes", command=Callable(self.show_bindings,'modes'))
		menuBindings.add_command(label="List bindings", command=Callable(self.show_bindings,'binding'), underline=0)
		menuBindings.add_command(label="List ASCII", command=Callable(self.show_bindings,'ascii'))
		menuBindings.add_separator()
		menuBindings.add_command(label="Visualize Bindings!", command=self.viz_bindings)
		menuBindings.add_command(label="Edit Key Bindings", command=self.edit_bindings)
		menubar.add_cascade(label="Bindings", menu=menuBindings, underline=0)
		
		self.manualbindings.update({'Alt+F4':root.quit, 'Control+O':self.open_file,'Control+N':self.new_file, 'Control+A':self.selectAll, 'Control+S':self.save_file, 'Control+Shift+S':Callable(self.save_file,True),
			'Control+[':Callable(self.changeFontSize,'-'),'Control+]':Callable(self.changeFontSize,'+')
			})
		
		root.config(menu=menubar)
		
	def _gettext(self):
		return self.txtContent.get(1.0, END)
	def _settext(self, strText):
		self.txtContent.delete('1.0', END)
		self.txtContent.insert(END, strText)
	def _updatefilename(self):
		strName = self.currentFilename if self.currentFilename!='' else 'Untitled'
		if not self.currentSaved: strName += ' *'
		self.txtFilename.config(text=strName)
	def selectAll(self):
		self.txtContent.tag_add(SEL, '1.0', 'end-1c')
		self.txtContent.mark_set(INSERT, '1.0')
		self.txtContent.see(INSERT)
		return 'break'
	def showModes(self, evt=None):
		modekeys = [(hotkey,self.dictHotkeys[hotkey]) for hotkey in self.dictHotkeys if self.dictHotkeys[hotkey][0]=='setmode']
		strShow = ''
		for hotkey in modekeys:
			strShow += hotkey[0] + '(' + self.dictModes[hotkey[1][1]] + ') ,'
		self.txtMode.config(text=strShow)
	def changeFontSize(self, relsize):
		if relsize == '+':
			self.textFormat = (self.textFormat[0], self.textFormat[1] + 2,self.textFormat[2])
		else:
			self.textFormat = (self.textFormat[0], self.textFormat[1] - 2,self.textFormat[2])
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
		return True
	def save_file(self, saveAs = False):
		if self.currentFilename == '' or saveAs:
			strFileName = tkFileDialog.asksaveasfilename()
			if not strFileName: return False
			self.currentFilename = strFileName
			
		f = open(self.currentFilename, 'w')
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
		
		f = open(strFileName, 'r')
		alltext = f.read()
		f.close()
		self._settext(alltext)
		self.currentFilename = strFileName
		self.currentSaved = True
		self._updatefilename()
		return True
		
	def onkey_normal(self, event):
		if event.keycode==16 or event.keycode==17 or event.keycode==0: #I don't know what keycode 0 means
			return
		mods = ''
		
		if event.state & 0x00000004: mods += 'Control+'
		if event.state & 0x00020000: mods += 'Alt+'
		if event.state & 0x00000001: mods += 'Shift+'
		
		if mods == '' and self.currentSaved==True and len(event.char)==1:
			self.currentSaved = False
			self._updatefilename()
		
		keypressed = mods + layouts.layout.get(event.keycode,'')
		if keypressed=='': return #unrecognized key
		
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
		self.txtMode.config(text='Mode:' + self.dictModes[newmode])
	
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
			for i in range(255):
				self.txtContent.insert(INSERT, str(i) +': '+ unichr(i))
				self.txtContent.insert(INSERT, '\n')
	def edit_bindings(self):
		import os
		self.open_file(os.path.join('keymaps','current.py.js'))
	def viz_bindings(self):
		import os
		os.system( os.path.join('keymaps','visualize.html'))
	
root = Tk()
app = App(root)

root.mainloop()