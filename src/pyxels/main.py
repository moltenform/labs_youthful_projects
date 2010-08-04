
from pix_parser import *
from pix_preview import PreviewImage
from pix_minitimer import Minitimer

import Image, ImageTk
_image_module = Image

import tkutil
import tkutil_dialog
from Tkinter import *
import ScrolledText

Image = _image_module #we want Image module, not Tkinter.Image.

def pack(o, **kwargs): o.pack(**kwargs); return o
class PythonPixelsApp():
	windowInput = None
	windowOutput = None
	imgInput = None
	imgOutput = None
	
	def __init__(self,root):
		root.title('PythonPixels')
		
		tkutil.bindShortcuts(root, {'<Control-n>':self.menu_newImage,
				'<Control-o>': self.menu_openImage,
				'<Control-s>': self.menu_saveImage,
				'<Control-V>': self.menu_pasteImage,
				'<Control-N>': self.menu_newScript,
				'<Control-O>': self.menu_openScript,
				'<Control-S>': self.menu_saveScript,
				'<Control-r>': self.menu_runScript } )
		
		menubar = Menu(root)
		menuFile = Menu(menubar, tearoff=0)
		menubar.add_cascade(label="File", menu=menuFile, underline=0)
		tkutil.addMenuItems(menuFile, 
				['&New Image|Ctrl+N',self.menu_newImage,
				'&Open Image|Ctrl+O', self.menu_openImage,
				'&Save Output|Ctrl+S', self.menu_saveImage,
				'_', None,
				'&Paste Image|Ctrl+Shift+V', self.menu_pasteImage, #note not to interfere with Ctrl+V
				'&Solid color', self.menu_fillColor,
				'_', None,
				'&Batch Process', self.menu_batch,
				'_', None,
				'E&xit', self.menu_quit]
			)
		menuScript = Menu(menubar, tearoff=0)
		menubar.add_cascade(label="Script", menu=menuScript, underline=0)
		tkutil.addMenuItems(menuScript, 
				['&New Script|Ctrl+Shift+N', self.menu_newScript,
				'&Open Script|Ctrl+Shift+O', self.menu_openScript,
				'&Save Script|Ctrl+Shift+S', self.menu_saveScript,
				'_',None,
				'&See Generated Code', self.menu_genScript,
				'&Run Script|Ctrl+R', self.menu_runScript ]
			)
		menuFavorites = self.makeFavoritesMenu()
		menubar.add_cascade(label='Favorites', menu=menuFavorites, underline=1)
		
		root.config(menu=menubar)
		
		
		frameBtns = pack(Frame(root), fill=BOTH, expand=True)
		self.fldScript = pack(ScrolledText.ScrolledText(frameBtns, height=6, width=100, wrap=NONE), side=TOP, fill=BOTH, expand=True)
		self.fldScript.insert(END, '#Flip red and green\nloop:\n\tR=g\n\tG=r-30\nprint "this is Python code!"')
		tkutil.select_all_binding(self.fldScript)
		
		pack(Button(frameBtns, text='Run', command=self.menu_runScript), side=TOP)
		
		self.fldOut = pack(ScrolledText.ScrolledText(frameBtns, height=4, width=100, wrap=NONE), side=TOP, fill=X)
		self.fldOut.insert(END, 'Welcome')
		tkutil.select_all_binding(self.fldScript)
		
		self.realstdout = sys.stdout
		self.realstderr = sys.stderr
		self.newstdout = tkutil.PseudoFile(self.writeStdOut, 'stdout')
		self.newstderr = tkutil.PseudoFile(self.writeStdOut, 'stderr')
		
		# Redirect std out
		sys.stdout = self.newstdout
		self.root = root
		
	def writeStdOut(self,s):
		if s!='\n':
			self.fldOut.insert(END, s.strip())
			self.fldOut.insert(END, '\n')
			self.fldOut.see(END)
			
	def redirectStdErr(self, bEnable):
		if bEnable: sys.stderr = self.newstderr
		else: sys.stderr = self.realstderr
		
	def makeFavoritesMenu(self):
		import os
		favMenu = Menu(tearoff=0)
		astrFolders = os.listdir('scripts')
		astrFolders = [strFolder for strFolder in astrFolders if tkutil.isDirectory(os.path.join('scripts',strFolder)) and not strFolder.startswith('.')]
		
		for strFolder in astrFolders:
			submenu = Menu(favMenu, tearoff=0)
			astrFiles = os.listdir(os.path.join('./scripts',strFolder))
			astrFiles = [strFile for strFile in astrFiles if strFile.endswith('.pyx')]
			for strItem in astrFiles:
				submenu.add('command', label=strItem, command=tkutil.Callable(self.menu_pickFavorite, strFolder, strItem))
			favMenu.add_cascade(label=strFolder, menu=submenu)
		return favMenu
	
	def menu_pickFavorite(self, strFolder, strItem):
		import os
		self._setScriptFile(os.path.join(os.path.join('scripts',strFolder),strItem))
	
	def menu_quit(self):
		self.root.quit()
	def menu_newImage(self):
		strDimensions = tkutil_dialog.ask('What are the dimensions?','256,256')
		if not strDimensions: return
		x,y = strDimensions.split(',')
		self.imgInput = Image.new('RGB', (int(x),int(y)))
		self.updateInput()
		
	def menu_fillColor(self):
		if self.imgInput == None:
			print 'No input image loaded.'
			return
		x,y = self.imgInput.size
		color = tkutil_dialog.ask_color()
		if not color: return
		self.imgInput = Image.new('RGB', (int(x),int(y)),color)
		self.updateInput()
	
	def updateInput(self):
		if self.windowInput == None:
			wnd = Toplevel()
			self.windowInput = PreviewImage(wnd, 'Input',self.imgInput)
		else:
			self.windowInput.setImage(self.imgInput)
	def updateOutput(self):
		if self.windowOutput == None:
			wnd = Toplevel()
			self.windowOutput = PreviewImage(wnd, 'Output',self.imgOutput)
		else:
			self.windowOutput.setImage(self.imgOutput)
	
	def menu_openImage(self):
		filename = tkutil_dialog.ask_openfile(title="Open Image")
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
		
	def menu_saveImage(self):
		if self.imgOutput==None:
			print 'No output image.'
			return
		filename = tkutil_dialog.ask_savefile(title = 'Save Image',types=['.png|Png image','.bmp|Bitmap image','.jpg|Jpeg image','.tif|TIFF image'])
		if not filename: return
		self.imgOutput.save(filename)
		print 'Image saved to ',filename
	
	
	def menu_newScript(self):
		tkutil.settext(self.fldScript , '')
	def menu_openScript(self):
		filename = tkutil_dialog.ask_openfile(title="Open Script", types=['.pyx|Scripts'])
		if not filename: return
		self._setScriptFile(filename)
		
	def _setScriptFile(self, filename):
		f = open(filename,'r')
		alltext = f.read()
		f.close()
		tkutil.settext(self.fldScript , alltext)
		
	def menu_saveScript(self):
		filename = tkutil_dialog.ask_savefile(title = 'Save Script',types=['.pyx|Scripts'])
		if not filename: return
		f = open(filename, 'w')
		f.write(tkutil.gettext(self.fldScript))
		f.close()
		print 'Script saved to ',filename
	def menu_genScript(self):
		strCode = tkutil.gettext(self.fldScript)
		strParsed = parseInput(strCode)
		tkutil.settext(self.fldScript, strParsed)
		
	def menu_runScript(self):
		if self.imgInput == None:
			print 'No input image loaded.'
			return
		mt = Minitimer()
		self.redirectStdErr(True)
		result = runScript(tkutil.gettext(self.fldScript), self.imgInput)
		self.redirectStdErr(False)
		if result != None:
			print 'Took, ' + str(mt.check())
			self.imgOutput = result
			self.updateOutput()
			
	def menu_batch(self):
		import os
		filename = tkutil_dialog.ask_openfile(title="Choose representative file (same directory, type).")
		if not filename: return
		if '.' not in filename:
			print 'Could not find extension.'
			return
		path, filename = os.path.split(filename)
		ext = filename.split('.')[-1]
		files = [file for file in os.listdir(path) if file.endswith('.'+ext)]
		
		outputfilename = tkutil_dialog.ask_savefile(title = 'Choose directory and format of output images.',types=['.png|Png image','.bmp|Bitmap image','.jpg|Jpeg image','.tif|TIFF image'])
		outputpath, outputfilename = os.path.split(outputfilename)
		outputext = outputfilename.split('.')[-1]
		
		confirm = tkutil_dialog.ask_okcancel('Run current script on '+str(len(files))+' files of type .'+ext +' in directory '+path+'?')
		if not confirm: return
		for file in files:
			filename = os.path.join(path, file)
			outputfilename = os.path.join( outputpath, file[0:-len(ext)] + outputext)
			if os.path.exists(outputfilename):
				print 'Skipped: File already exists:',  outputfilename
				continue
			
			try:
				im = Image.open(filename)
				im.load()
			except:
				print 'Skipped: Could not open image.'
				continue
				
			imgResult = runScript(tkutil.gettext(self.fldScript), im)
			if imgResult==None:
				print 'Skipped: Script error.'
				continue
			
			try:
				imgResult.save(outputfilename)
			except:
				print 'Error. Could not save ',outputfilename
			print 'Saved: '+outputfilename
			
		print 'Job complete.'
		
	def menu_pasteImage(self):
		import sys
		if sys.platform!='win32':
			print 'Only supported on Windows...'
			return
		import ImageGrab
		im = ImageGrab.grabclipboard()
		if not isinstance(im, Image.Image):
			print 'Could not paste image.'
			return
		if im.mode != 'RGB':
			print 'Error: For now, only RGB mode images are supported.'
			return
		self.imgInput = im
		self.updateInput()
		# NOTE An apparent error in PIL for py2.5 makes this block the clipboard!
		# The problem stops when the program is closed.
		
	

root = Tk()
app = PythonPixelsApp(root)
root.mainloop()
