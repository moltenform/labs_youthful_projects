"""
ScoreView
Ben Fisher, 2009, GPL
halfhourhacks.blogspot.com

"""
from Tkinter import *

import scoreview_util

sys.path.append('..\\bmidilib')
import bmidilib

class App:
	def __init__(self, root):
		root.title('Midi Scoreview')
		
		frameMain = Frame(root, width=600, height=200)
		frameMain.pack(side=TOP,fill=BOTH, expand=True)
		
		self.lblFilename = Label(frameMain, text='No file opened.')
		self.lblFilename.pack(side=TOP, anchor='w')
		
		frameGrid = Frame(frameMain)
		frameGrid.pack(side=TOP,fill=BOTH, expand=True, anchor='w')
		
		Label(frameGrid, text='#').grid(row=0, column=0)
		Label(frameGrid, text='Track').grid(row=0, column=1)
		Label(frameGrid, text='Channel').grid(row=0, column=2)
		Label(frameGrid, text='Instrument').grid(row=0, column=3)
		Label(frameGrid, text='Notes').grid(row=0, column=4)
		Label(frameGrid, text=' ').grid(row=0, column=5)
		Label(frameGrid, text=' ').grid(row=0, column=6)
		
		
		
		#~ Label(frameGrid, text="First").grid(row=0)
		#~ Label(frameGrid, text="Second").grid(row=1)
		#~ e1 = Entry(frameGrid)
		#~ e2 = Entry(frameGrid)
		#~ e1.grid(row=0, column=1)
		#~ e2.grid(row=1, column=1)
		
		
		#use grid_forget to remove old widgets
		
		self.create_menubar(root)
		
		
		
	def create_menubar(self,root):
		root.bind('<Alt-F4>', lambda x:root.quit)
		menubar = Menu(root)
		
		menuFile = Menu(menubar, tearoff=0)
		menubar.add_cascade(label="File", menu=menuFile, underline=0)
		menuFile.add_command(label="Open Midi", command=self.menu_openMidi, underline=0, accelerator='Ctrl+O')
		menuFile.add_separator()
		menuFile.add_command(label="Exit", command=root.quit, underline=1)
		
		self.objOptionsDuration = IntVar(); self.objOptionsDuration.set(1)
		self.objOptionsStems = IntVar(); self.objOptionsStems.set(1)
		self.objOptionsBarlines = IntVar(); self.objOptionsBarlines.set(0)
		menuOptions = Menu(menubar, tearoff=0)
		menubar.add_cascade(label="Options", menu=menuOptions, underline=0)
		menuOptions.add_checkbutton(label="Show Durations", variable=self.objOptionsDuration, underline=0, onvalue=1, offvalue=0)
		menuOptions.add_checkbutton(label="Show Stems", variable=self.objOptionsStems, underline=5, onvalue=1, offvalue=0)
		menuOptions.add_checkbutton(label="Show Barlines", variable=self.objOptionsBarlines, underline=5, onvalue=1, offvalue=0)
		
		
		menuHelp = Menu(menubar, tearoff=0)
		menuHelp.add_command(label='About', command=(lambda: scoreview_util.alert('ScoreView, by Ben Fisher 2009\n\nhalfhourhacks.blogspot.com','benmidi ScoreView')))
		menubar.add_cascade(label="Help", menu=menuHelp, underline=0)
		
		root.config(menu=menubar)
		
		
	def menu_openMidi(self):
		filename = scoreview_util.ask_openfile(title="Open Midi File", types=['.mid|Mid file'])
		if not filename: return
		
		self.loadMidi(filename)


	
root = Tk()
app = App(root)
root.mainloop()

