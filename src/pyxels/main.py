from link_djoser import *

from pix_parser import *
from pix_preview import PreviewImage
from pix_minitimer import Minitimer

import Image, ImageTk
import Tkinter

class PythonPixelsApp(DjApplication):
	windowInput = None
	windowOutput = None
	imgInput = None
	imgOutput = None
	
	def layout(self,window):
		window.title = 'PythonPixels'
				
		m = DjMenu(window,
			['&File',
				'&New Image|Ctrl+N',self.menu_newImage,
				'&Open Image|Ctrl+O', self.menu_openImage,
				'&Save Output|Ctrl+S', self.menu_saveImage,
				
				
				'_',
				'&Paste Image|Ctrl+Shift+V', self.menu_pasteImage, #note not to interfere with Ctrl+V
				'&Solid color', self.menu_fillColor,
				'_',
				'&Batch Process', self.menu_batch,
				'_',
				
				'E&xit', self.menu_quit
			],
			['&Script',
				'&New Script|Ctrl+Shift+N', self.menu_newScript,
				'&Open Script|Ctrl+Shift+O', self.menu_openScript,
				'&Save Script|Ctrl+Shift+S', self.menu_saveScript,
				'_',
				'&See Generated Code', self.menu_genScript,
				'&Run Script|Ctrl+R', self.menu_runScript,
			]
		)
		window.set_shortcut_keys(m._keyshortcuts)
		
		menuFavorites = self.makeFavoritesMenu()
		m.basecontrol.add_cascade(label='Favorites', menu=menuFavorites, underline=1)
		window.menubar = m
		
		window.frame(V, layout='fill,expand')
		self.fldScript = window.field('#Flip red and green\nloop:\n\tR=g\n\tG=r-30\nprint "this is Python code!"', layout='fill,expand', scrollbar=True, height=6, width=100, wrap='none')
		self.fldScript.select_all_binding()
		self.btnRun = window.button('Run', self.menu_runScript)
		self.fldOut = window.field('Welcome', layout='fillx', scrollbar=True, height=4, width=100, wrap='none')
		window.endframe()
		
		self.realstdout = sys.stdout
		self.realstderr = sys.stderr
		self.newstdout = PseudoFile(self.writeStdOut, 'stdout')
		self.newstderr = PseudoFile(self.writeStdOut, 'stderr')
		
		# Redirect std out
		sys.stdout = self.newstdout
		
		window.add_all()
		
	def writeStdOut(self,s):
		if s!='\n':
			self.fldOut.text += s.strip()
			self.fldOut.scrollto_end()
			
	def redirectStdErr(self, bEnable):
		if bEnable: sys.stderr = self.newstderr
		else: sys.stderr = self.realstderr
		
	def makeFavoritesMenu(self):
		import os
		def isDirectory(dir_name):
			mask = 040000
			try: s = os.stat(dir_name)
			except: return 0
			if (mask & s[0]) == mask: return 1
			else: return 0
		
		favMenu = Tkinter.Menu(tearoff=0)
		astrFolders = os.listdir('scripts')
		astrFolders = [strFolder for strFolder in astrFolders if isDirectory(os.path.join('scripts',strFolder)) and not strFolder.startswith('.')]
		
		for strFolder in astrFolders:
			submenu = Tkinter.Menu(favMenu, tearoff=0)
			astrFiles = os.listdir(os.path.join('./scripts',strFolder))
			astrFiles = [strFile for strFile in astrFiles if strFile.endswith('.pyx')]
			for strItem in astrFiles:
				submenu.add('command', label=strItem, command=Callable(self.menu_pickFavorite, strFolder, strItem))
			favMenu.add_cascade(label=strFolder, menu=submenu)
		return favMenu
	
	def menu_pickFavorite(self, strFolder, strItem):
		import os
		self._setScriptFile(os.path.join(os.path.join('scripts',strFolder),strItem))
	
	def menu_quit(self):
		self.quit()
	def menu_newImage(self):
		strDimensions = ask('What are the dimensions?','256,256')
		if not strDimensions: return
		x,y = strDimensions.split(',')
		self.imgInput = Image.new('RGB', (int(x),int(y)))
		self.updateInput()
		
	def menu_fillColor(self):
		if self.imgInput == None:
			print 'No input image loaded.'
			return
		x,y = self.imgInput.size
		color = ask_color()
		if not color: return
		self.imgInput = Image.new('RGB', (int(x),int(y)),color)
		self.updateInput()
	
	def updateInput(self):
		if self.windowInput == None:
			self.windowInput = PreviewImage('Input',self.imgInput)
		else:
			self.windowInput.setImage(self.imgInput)
	def updateOutput(self):
		if self.windowOutput == None:
			self.windowOutput = PreviewImage('Output',self.imgOutput)
		else:
			self.windowOutput.setImage(self.imgOutput)
	
	def menu_openImage(self):
		filename = ask_openfile(title="Open Image")
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
		filename = ask_savefile(title = 'Save Image',types=['.png|Png image','.bmp|Bitmap image','.jpg|Jpeg image','.tif|TIFF image'])
		if not filename: return
		self.imgOutput.save(filename)
		print 'Image saved to ',filename
	
	
	def menu_newScript(self):
		self.fldScript.text = ''
	def menu_openScript(self):
		filename = ask_openfile(title="Open Script", types=['.pyx|Scripts'])
		if not filename: return
		self._setScriptFile(filename)
		
	def _setScriptFile(self, filename):
		f = open(filename,'r')
		alltext = f.read()
		f.close()
		self.fldScript.text = alltext
		
	def menu_saveScript(self):
		filename = ask_savefile(title = 'Save Script',types=['.pyx|Scripts'])
		if not filename: return
		f = open(filename, 'w')
		f.write(self.fldScript.text)
		f.close()
		print 'Script saved to ',filename
	def menu_genScript(self):
		strCode = self.fldScript.text
		strParsed = parseInput(strCode)
		self.fldScript.text = strParsed
		
	def menu_runScript(self):
		if self.imgInput == None:
			print 'No input image loaded.'
			return
		mt = Minitimer()
		self.redirectStdErr(True)
		result = runScript(self.fldScript.text, self.imgInput)
		self.redirectStdErr(False)
		if result != None:
			print 'Took, ' + str(mt.check())
			self.imgOutput = result
			self.updateOutput()
			
	def menu_batch(self):
		import os
		filename = ask_openfile(title="Choose representative file (same directory, type).")
		if not filename: return
		if '.' not in filename:
			print 'Could not find extension.'
			return
		path, filename = os.path.split(filename)
		ext = filename.split('.')[-1]
		files = [file for file in os.listdir(path) if file.endswith('.'+ext)]
		
		outputfilename = ask_savefile(title = 'Choose directory and format of output images.',types=['.png|Png image','.bmp|Bitmap image','.jpg|Jpeg image','.tif|TIFF image'])
		outputpath, outputfilename = os.path.split(outputfilename)
		outputext = outputfilename.split('.')[-1]
		
		confirm = ask_okcancel('Run current script on '+str(len(files))+' files of type .'+ext +' in directory '+path+'?')
		if not confirm: return
		for file in files:
			filename = os.path.join(path, file)
			outputfilename = os.path.join( outputpath, file[0:-len(ext)] + outputext)
			if os.path.exists(outputfilename):
				print 'Skipped: File already exists:',  outputfilename
				continue
			
			try:
				im = Image.open(filename)
			except:
				print 'Skipped: Could not open image.'
				continue
				
			imgResult = runScript(self.fldScript.text, im)
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
		# NOTE An apparent error in PIL makes this block the clipboard!
		# The problem stops when the program is closed.
		
	
m = PythonPixelsApp()
m.run()