
import os, sys
import exceptions
import wx
import wxutil
import cellrename_data
import cellrename_engine
import cellrename_grid

class CellRenameMain(wx.Frame):
	sortingAtoZ = True
	menuIncludeDirs = None
	menuUndo = None
	undoHistory = None
	grid = None
	statusbar = None
	data = None # holds current directory and filter.
	
	def __init__(self, parent, id, title, strDirectory, strFilter, bIncludeDirs=False):
		wx.Frame.__init__(self, parent, id, title, wx.DefaultPosition, wx.Size(550, 200))
		self.data = cellrename_data.CellRenameData(strDirectory, strFilter, bIncludeDirs)

		# use a helper function to add menu items
		self.currentEventId=700
		def addMenuSeparator(menu): menu.AppendSeparator()
		def addMenuItem(menu, strName, fnCallback, strShortcut='', kind=None):
			self.currentEventId+=1
			if strShortcut: strName+='\t'+strShortcut
			self.Bind(wx.EVT_MENU, fnCallback, id=self.currentEventId)
			if kind: return menu.Append(self.currentEventId, strName, '', kind=kind)
			else: return menu.Append(self.currentEventId, strName, '')
		
		# populate Menu Bar
		menuFile = wx.Menu()
		addMenuItem(menuFile, '&Open...', self.menu_onFileOpen, 'Ctrl+O')
		addMenuItem(menuFile, '&Refresh', self.menu_onFileRefresh, 'F5')
		addMenuSeparator(menuFile)
		addMenuItem(menuFile, '&Perform Rename', self.menu_onFileRename, 'Ctrl+Enter')
		addMenuSeparator(menuFile)
		addMenuItem(menuFile, 'Filter by &Extension...', self.menu_onFileFilter, 'Ctrl+E')
		self.menuIncludeDirs = addMenuItem(
			menuFile, '&Include Directories', self.menu_onFileIncludeDirs, 'Ctrl+D', kind=wx.ITEM_CHECK)
		addMenuItem(menuFile, 'Download &URL...', self.menu_onFileDownload, 'Ctrl+U')
		addMenuItem(menuFile, 'Sort by last-&modified time',self.menu_onFileSortMod)
		addMenuItem(menuFile, 'Sort by file-&created time',self.menu_onFileSortCreated)
		addMenuSeparator(menuFile)
		addMenuItem(menuFile, 'E&xit', self.menu_onFileExit)
		
		menuEdit = wx.Menu()
		self.menuUndo = addMenuItem(menuEdit,'&Undo Rename', self.menu_onEditUndo, 'Ctrl+Z')
		addMenuSeparator(menuEdit)
		addMenuItem(menuEdit,'Add &Prefix...', self.menu_onEditPrefix, 'Ctrl+Shift+P')
		addMenuItem(menuEdit,'Add &Suffix...', self.menu_onEditSuffix, 'Ctrl+Shift+S')
		addMenuItem(menuEdit,'Add &Number...', self.menu_onEditNumber, 'Ctrl+3')
		addMenuItem(menuEdit,'&Pattern...', self.menu_onEditPattern, 'Ctrl+P')
		addMenuSeparator(menuEdit)
		addMenuItem(menuEdit,'&Replace in Filenames', self.menu_onEditReplace, 'Ctrl+H')
		
		menuHelp = wx.Menu()
		addMenuItem(menuHelp, '&About CellRename', self.menu_onHelpAbout)
		addMenuItem(menuHelp, '&Tips', self.menu_onHelpTips)

		menubar = wx.MenuBar()
		menubar.Append(menuFile, '&File')
		menubar.Append(menuEdit, '&Edit')
		menubar.Append(menuHelp, '&Help')
		self.SetMenuBar(menubar)
		self.Centre()
		
		# Create widgets
		self.grid = cellrename_grid.CellRenameGrid(self, self.data.getLength(), self.onSortEvent)
		self.statusbar = self.CreateStatusBar(2)
		self.statusbar.SetStatusWidths([-1, 280])
		self.statusbar.PushStatusText("Press Ctrl+Enter to rename files.", 0)
		self.statusbar.PushStatusText("", 1) #will be updated by menu_onFileRefresh
		
		self.sizer = wx.BoxSizer(wx.VERTICAL)
		self.sizer.Add(self.grid, 1, wx.EXPAND)
		self.SetSizer(self.sizer)
		self.SetAutoLayout(True)
		self.sizer.SetSizeHints(self)

		# set initial state
		self.undoHistory = None
		self.menuUndo.Enable(False)
		
		# load and display the data!
		self.menu_onFileRefresh()
		
	def writeStatus(self, s):
		self.SetStatusText(s, 0)
	
	def menu_onFileOpen(self, evt):
		dialog = wx.FileDialog(self, style = wx.OPEN, message ="Select any file in the target directory:" )
		if dialog.ShowModal() == wx.ID_OK and dialog.GetPath():
			spath, sfilename = os.path.split(dialog.GetPath())
			if os.path.exists(spath):
				self.data.directory = spath
				self.menu_onFileRefresh()
		dialog.Destroy()

	def menu_onFileRename(self, evt):
		self.grid.writeToModel(self.data)
		afrom, ato = self.data.prepareLists()
		self.performRename(afrom, ato)
		
	def menu_onFileRefresh(self, evt=None):
		# first, update status bar with current directory
		sRenderDirectory = self.data.directory
		if len(sRenderDirectory)>40: sRenderDirectory=sRenderDirectory[0:40]+'...'
		self.statusbar.PushStatusText(sRenderDirectory, 1)
		
		self.data.refresh()
		self.grid.loadFromModel(self.data)
		for entry in os.listdir(self.data.directory):
			if cellrename_engine.marker in entry:
				self.alertRepair()
				break

	def menu_onFileFilter(self, evt):
		strPattern = wxutil.inputDialog(self, 'Enter a filter, like *.mp3.','*')
		if not strPattern: return
		self.data.filter = strPattern
		self.menu_onFileRefresh()

	def menu_onFileIncludeDirs(self, evt):
		self.data.includeDirs = self.menuIncludeDirs.IsChecked()
		self.menu_onFileRefresh()
		
	def menu_onFileDownload(self, evt):
		# hack: downloading on the main thread for simplicity.
		self.writeStatus('Downloading...')
		strUrl = wxutil.inputDialog(self, 'Enter a url:','http://www.google.com/')
		if not strUrl: self.writeStatus(''); return
		strOutName = wxutil.saveFileDialog(self, 'Save to:')
		if not strOutName: self.writeStatus(''); return
		bRet = False
		try:
			import urllib
			urllib.urlretrieve(strUrl, strOutName)
			bRet = True
		except IOError as e:
			self.writeStatus("Download failed: " + str(e))
		except:
			self.writeStatus("Download failed: " + str(sys.exc_info()[0]))
		
		if bRet: self.writeStatus('Download complete.')

	def menu_onFileSortMod(self, evt):
		self.onSortEvent('modifiedTime', self.sortingAtoZ)
		self.sortingAtoZ = not self.sortingAtoZ # reverse order for the next sort

	def menu_onFileSortCreated(self, evt):
		self.onSortEvent('creationTime', self.sortingAtoZ)
		self.sortingAtoZ = not self.sortingAtoZ # reverse order for the next sort

	def menu_onFileExit(self, evt):
		self.Close()

	def menu_onEditUndo(self, evt):
		#Attempt to undo last rename
		if not self.undoHistory:
			return
		prevdir, ato, afrom = self.undoHistory #note reverse order, because rename to from
		if prevdir!=self.data.directory:
			wxutil.alertDialog(self, 'Cannot undo, first return to the directory.')
			return
		if not wxutil.alertDialog(self, 'Undo renaming these files?', 'Confirm', wx.OK|wx.CANCEL):
			return
		self.performRename(afrom, ato)
		self.menuUndo.Enable(False)

	def onSuffixOrPrefix(self, bPrefix):
		strAdded = wxutil.inputDialog(self, 'Add a '+('prefix' if bPrefix else 'suffix')+'.','')
		if not strAdded: return
		
		self.grid.writeToModel(self.data)
		self.data.transform_suffixorprefix(bPrefix, strAdded)
		self.grid.loadFromModel(self.data)
	def menu_onEditPrefix(self, evt):
		self.onSuffixOrPrefix(True)
	def menu_onEditSuffix(self, evt):
		self.onSuffixOrPrefix(False)

	def menu_onEditNumber(self, evt):
		strFirst = wxutil.inputDialog(self, 'Rename into a sequence like 01, 02, 03, and so on. Please enter the first entry of the sequence (e.g. "1", or "001", or "42").','001')
		if not strFirst: return
		self.grid.writeToModel(self.data)
		ret = self.data.transform_addnumber(strFirst)
		self.grid.loadFromModel(self.data)
		if ret!=True and ret: wxutil.alertDialog(self, ret)
			
	def menu_onEditPattern(self, evt):
		strPattern = wxutil.inputDialog(self, 'Enter a naming pattern. The following can be used:/n/n/t%n=padded number (i.e. 001, 002)/n/t%N=number/n/t%f=file name/n/t%U=uppercase name/n/t%u=lowercase name'.
			replace('/n',os.linesep).replace('/t','     ')	,'%f')
		if not strPattern: return
		self.grid.writeToModel(self.data)
		self.data.transform_pattern(strPattern)
		self.grid.loadFromModel(self.data)

	def menu_onEditReplace(self, evt):
		strSearch = wxutil.inputDialog(self, 'Search for string: ')
		if not strSearch: return
		strReplace = wxutil.inputDialog(self, 'And replace with: ')
		if strReplace==None: return
		
		self.grid.writeToModel(self.data)
		# regex replace, or regex case-insensitive replace (as documented in 'tips')
		if strSearch.startswith('r:'):
			self.data.transform_regexreplace(strSearch[len('r:'):], strReplace,True,True)
		elif strSearch.startswith('ri:'):
			self.data.transform_regexreplace(strSearch[len('ri:'):], strReplace,True,False)
		elif strSearch.startswith('i:'):
			self.data.transform_regexreplace(strSearch[len('i:'):], strReplace,False,False)
		else:
			self.data.transform_replace(strSearch, strReplace)
		self.grid.loadFromModel(self.data)
	
	
	def menu_onHelpAbout(self, evt):
		wxutil.alertDialog(self,'''CellRename 0.2, by Ben Fisher, 2008.
http://halfhourhacks.blogspot.com
https://github.com/downpoured/cellrename

Released under the GPLv3 license.''','About')

	def menu_onHelpTips(self, evt):
		sTips = r'''Copy a directory path and open CellRename to start in that directory.

In Replace, for case-insensitive matching type "i:" before your query.

In Replace, to use regex, type "r:" before your query. Groups work, so "r:(\w+),(\w+)" to "\2,\1" will turn "first,second" into "second,first". '''
		wxutil.alertDialog(self,sTips,'Tips')

	
	# respond to message sent by grid, strField is the name of the column to sort by
	def onSortEvent(self, strField, bReverse):
		self.grid.writeToModel(self.data)
		self.data.sort(strField, bReverse)
		self.grid.loadFromModel(self.data)
		
	
	def performRename(self, afrom, ato):
		result = False
		try:
			result = cellrename_engine.renameFiles(self.data.directory, afrom, ato)
		except:
			wxutil.alertDialog(self, 'Exception occurred: '+str(sys.exc_info()[0]))
			return
		
		if result != True:
			wxutil.alertDialog(self, result)
			self.writeStatus("")
			return
		
		self.undoHistory = self.data.directory, afrom, ato
		self.menuUndo.Enable(True)
		self.menu_onFileRefresh()
		self.writeStatus( str(self.data.getLength())+' files renamed.')
	
	def alertRepair(self):
		wxutil.alertDialog(self, 'It looks like renaming was not successful. When you click OK, the previous filenames will be placed in the clipboard. Please rename back to normal filenames before continuing, and then restart CellRename.','Error')
		if self.undoHistory and self.undoHistory[1]:
			afrom = self.undoHistory[1]
			s = os.linesep.join(afrom)
			wxutil.setClipboardText(s)
			


class MyApp(wx.App):
	def OnInit(self):
		# find an appropriate starting directory
		if os.name=='nt':
			strDirectory = os.path.join(os.environ['USERPROFILE'], 'Documents')
			if not os.path.exists(strDirectory):
				strDirectory = os.path.join(os.environ['USERPROFILE'], 'My Documents')
			if not os.path.exists(strDirectory):
				strDirectory = 'c:\\'
		else:
			strDirectory = os.environ['HOME']
			if not os.path.exists(strDirectory):
				strDirectory = '~/'
			if not os.path.exists(strDirectory):
				strDirectory = '/'
		
		strFilter='*'
		bIncludeDirs=False
		
		# use a directory from the clipboard if available
		sclip = wxutil.getClipboardText()
		if sclip and os.sep in sclip:
			if os.path.isdir(sclip):
				strDirectory = sclip
			elif os.path.exists(sclip):
				parent = os.path.split(sclip)[0]
				if os.path.isdir(parent): strDirectory = parent
		
		frame = CellRenameMain(None, -1, 'CellRename', strDirectory, strFilter, bIncludeDirs)
		frame.Show(True)
		return True


app = MyApp(0)
app.MainLoop()

