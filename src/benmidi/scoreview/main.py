"""
ScoreView
Ben Fisher, 2009, GPL
halfhourhacks.blogspot.com

"""
from Tkinter import *
import tkFileDialog

import bmidilib

class App:
	def __init__(self, root):
		root.title('Minimath')
		
		frameMain = Frame(width=400, height=100)
		frameMain.pack(side=TOP,fill=BOTH, expand=True)
		
		
		frameRightCol = Frame(frameMain, padx=5)
		self.txtContent = ScrolledText.ScrolledText(frameRightCol, wrap=NONE)
		
		
		self._create_menubar(root)
		
		
		
	def _create_menubar(self,root):
		root.bind('<Alt-F4>', lambda x:root.quit)
		menubar = Menu(root)
		menuFile = Menu(menubar, tearoff=0)
		
		#~ menuFile.add_separator()
		menuFile.add_command(label="Exit", command=root.quit, underline=1)
		menubar.add_cascade(label="File", menu=menuFile, underline=0)
		
		#~ menuEdit = Menu(menubar, tearoff=0)
		#~ menuEdit.add_command(label="Undo", command=self.edit_undo, underline=0, accelerator='Ctrl+Z')
		#~ menuEdit.add_command(label="Redo", command=self.edit_redo, underline=0, accelerator='Ctrl+Y')
		#~ menubar.add_cascade(label="Edit", menu=menuEdit, underline=0)
		
		menuHelp = Menu(menubar, tearoff=0)
		menuHelp.add_command(label='About', command=(lambda: tkMessageBox.showinfo('benmidi ScoreView','ScoreView, by Ben Fisher 2009\n\nhalfhourhacks.blogspot.com')))
		menuHelp.add_command(label='Help', command=self.showDocs)
		menubar.add_cascade(label="Help", menu=menuHelp, underline=0)
		
		root.config(menu=menubar)
		
		
	def menu_openMidi(self):
		filename = ask_openfile(title="Open Midi File")
		if not filename: return
		try:
			im = Image.open(filename)
		except:
			print 'Error: Could not open image.'
			return
		if im.mode != 'RGB':
			print 'Error: For now, only RGB mode images are supported.'
			return
		self.imgInput = im
		self.updateInput()


	
root = Tk()
app = App(root)

root.mainloop()