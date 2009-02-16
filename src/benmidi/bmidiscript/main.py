"""
BMidiScript
Ben Fisher, 2009, GPL
halfhourhacks.blogspot.com

"""
from Tkinter import *

import midiscript_util

sys.path.append('..\\bmidilib')
import bmidilib
import bbuilder
import bmidiplay

class App:
	def __init__(self, root):
		root.title('tunescript')
		
		frameMain = Frame(root, background='white')
		frameMain.pack(side=TOP,fill=BOTH, expand=True)
		
		self.lblStatus = Label(frameMain, text='Welcome.', background='white')
		self.lblStatus.pack(side=TOP, anchor='n')
		
		tupfont = ('Verdana', 20, 'normal')
		self.txtMain = Text(frameMain, font=tupfont, width=30, height=10)
		self.txtMain.pack(side=TOP,fill=BOTH, expand=True)
		
		tupfontbtn = ('Verdana', 14, 'normal')
		frameBtns = Frame(frameMain, bg='black')
		frameBtns.pack()
		self.btnPlay = Button(frameMain, command=self.playMidi, width=20,height=3, text='Play', font=tupfontbtn)
		self.btnPlay.pack(side=LEFT, padx=15)
		self.btnSaveMid = Button(frameMain,command=self.saveMidi,  width=20,height=3, text='Save mid', font=tupfontbtn)
		self.btnSaveMid.pack(side=LEFT, padx=15)
		
		
		
		self.create_menubar(root)
		
		def selectAll(fld):
			"""Select all text in the field."""
			fld.tag_add(SEL, '1.0', 'end-1c')
			fld.mark_set(INSERT, '1.0')
			fld.see(INSERT)
			return 'break'
		
		self.txtMain.bind('<Control-a>',lambda event: selectAll(self.txtMain), '+')
		
		self.mode = 'tune' #as opposed to 'code'.
		self.loadExample(0)
		
		
	def create_menubar(self,root):
		root.bind('<Alt-F4>', lambda x:root.quit)
		root.bind('<Control-s>', self.saveMidi)
		root.bind('<Control-space>', self.playMidi)
		menubar = Menu(root)
		
		menuFile = Menu(menubar, tearoff=0)
		menubar.add_cascade(label="File", menu=menuFile, underline=0)
		menuFile.add_command(label="Tunescript Mode", command=self.setModeTune, underline=0)
		menuFile.add_command(label="Code bbuilder Mode", command=self.setModeCode, underline=0)
		menuFile.add_separator()
		menuFile.add_command(label="Play", command=self.playMidi, underline=0, accelerator='Ctrl+space')
		menuFile.add_command(label="Save mid", command=self.saveMidi, underline=0, accelerator='Ctrl+S')
		menuFile.add_separator()
		menuFile.add_command(label="Exit", command=root.quit, underline=1)
		
		menuExamples = Menu(menubar, tearoff=0)
		menubar.add_cascade(label="Examples", menu=menuExamples, underline=0)
		menuExamples.add_command(label="Hello", command=midiscript_util.Callable(self.loadExample,1))
		
		menuHelp = Menu(menubar, tearoff=0)
		menuHelp.add_command(label='About', command=(lambda: midiscript_util.alert('tunescript, by Ben Fisher 2009\n\nhalfhourhacks.blogspot.com','benmidi tunescript')))
		menubar.add_cascade(label="Help", menu=menuHelp, underline=0)
		
		root.config(menu=menubar)
		
		
	def status(self, s): self.lblStatus['text']=s
	def saveMidi(self, evt=None):
		filename = midiscript_util.ask_savefile(title="Save Midi File", types=['.mid|Mid file'])
		if not filename: return
		
		mfile = self.createMidiFile()
		mfile.open(filename,'wb')
		mfile.write()
		mfile.close()
	
	def playMidi(self, evt=None):
		mfile = self.createMidiFile()
		bmidiplay.playMidiObject(mfile) #creates a temporary .mid file, and then uses mci to play it
	
		
	def createMidiFile(self):
		if self.mode=='tune': return self.createMidiFileTunescript(self.getText())
		else: return self.createMidiFileBuilder(self.getText())
	
	def createMidiFileBuilder(self, txt):
		provided = {}
		provided['Builder'] =bbuilder.BMidiBuilder #reference to class
		provided['alert'] = midiscript_util.alert #reference to fn
		provided['status'] = self.status #reference to fn
		
		#run the code
		evaler = midiscript_util.Interpreter(provided)
		res = evaler.run_code(txt)
		if res!=True:
			midiscript_util.alert(res)
			return
		
		#we are most interested in what they set "result" to.
		if 'result' not in provided or not provided['result']:
			midiscript_util.alert('You must set "result" to something.')
			return
			
		res = provided['result']
		#either a sequence or a value
		if isinstance(res, bbuilder.BMidiBuilder):
			#is a single instance
			mfileobject = bbuilder.build_midi([res])
		else:
			mfileobject = bbuilder.build_midi(res)
		return mfileobject
		
		
		
	def createMidiFileTunescript(self, txt):
		pass
		
	def getText(self):
		return self.txtMain.get(1.0, END)
	def setText(self, s):
		self.txtMain.delete('1.0', END)
		self.txtMain.insert(END, s)
	
	def loadExample(self, exnum):
		print 'loading example '+str(exnum)
		#ask for confirmation if it is non-empty and not equal to a default example
		if self.getText().strip()!='' and self.getText().strip()!=self.getExample(0, 'tune').strip() and \
				self.getText().strip()!=self.getExample(0, 'code').strip():
			confirm = midiscript_util.ask_yesno('Replace existing text?','Replace')
			if not confirm: return
				
		s = self.getExample(exnum, self.mode)
		self.setText(s)
		
	
	def getExample(self, exnum, mode):
		if mode=='tune':
			s=''
			if exnum==0:
				s='''abc'''.replace('\t','')
		else:
			s=''
			if exnum==0:
				s='''b = Builder()
				b.note('c', 1)
				b.note('d', 1)
				b.note('e', 1)
				b.note('f', 1)
				result = b'''.replace('\t','')
			# other example: result = [tr1, tr2]. and that gets created. cool.
		return s
	
	def setModeTune(self): self.mode = 'tune'; self.status('Switched to tunescript mode'); self.loadExample(0)
	def setModeCode(self): self.mode = 'code'; self.status('Switched to code mode'); self.loadExample(0)
		
	

root = Tk()
app = App(root)
root.mainloop()

