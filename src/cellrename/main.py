
import wx
import wxutil
import sheet_data
import sheet_sheet
import os, sys
import cellrename_engine

class SheetFrame(wx.Frame):
	sortingAtoZ = True
	menuIncludeDirs = None
	menuUndo = None
	ogrid = None
	sheetdata = None
	undoData = None
	statusbar = None
	# the current directory and other settings are held in the sheetdata object
	
	def __init__(self, parent, id, title, strDirectory, strFilter, bIncludeDirs=False):
		wx.Frame.__init__(self, parent, id, title, wx.DefaultPosition, wx.Size(600, 400))
		self.sheetdata = sheet_data.SheetData(strDirectory, strFilter, bIncludeDirs)

		# use a helper function to add menu items, since I don't need the event id
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
		addMenuItem(menuFile, '&Filter by Extension...', self.menu_onFileFilter, 'Ctrl+E')
		self.menuIncludeDirs = addMenuItem(
			menuFile, '&Include Directories', self.menu_onFileIncludeDirs, 'Ctrl+D', kind=wx.ITEM_CHECK)
		addMenuItem(menuFile, 'Download &URL...', self.menu_onFileDownload, 'Ctrl+U')
		addMenuItem(menuFile, 'Sort by last-&modified time',self.menu_onFileSortMod)
		addMenuItem(menuFile, 'Sort by file-&created time',self.menu_onFileSortCreated)
		addMenuSeparator(menuFile)
		addMenuItem(menuFile, 'E&xit', self.menu_onFileExit)
		
		menuEdit = wx.Menu()
		self.menuUndo = addMenuItem(menuEdit,'Undo', self.menu_onEditUndo, 'Ctrl+Z')
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
		self.ogrid = sheet_sheet.CellRenameGrid(self, self.sheetdata.getLength(), self.on_sort_event)
		self.statusbar = self.CreateStatusBar(2)
		self.statusbar.SetStatusWidths([-1, 280])
		self.statusbar.PushStatusText("Press Ctrl+Enter to rename files.", 0)
		self.statusbar.PushStatusText("", 1) #will be updated by menu_onFileRefresh
		
		self.sizer = wx.BoxSizer(wx.VERTICAL)
		self.sizer.Add(self.ogrid, 1, wx.EXPAND)
		self.SetSizer(self.sizer)
		self.SetAutoLayout(True)
		self.sizer.SetSizeHints(self)

		# set initial state
		self.undoData = None
		self.menuUndo.Enable(False)
		
		# load and display the data!
		self.menu_onFileRefresh()
		
	
	# repond to Menu callbacks

	def menu_onFileOpen(self, evt):
		dialog = wx.FileDialog(self, style = wx.OPEN, message ="Select any file in the target directory:" )
		if dialog.ShowModal() == wx.ID_OK:
			localpath, localfilename = os.path.split(dialog.GetPath())
			if os.path.exists(localpath):
				self.sheetdata.directory = localpath
				self.menu_onFileRefresh()
		dialog.Destroy()

	def menu_onFileRename(self, evt=None):
		self.performRename()
		
	def menu_onFileRefresh(self, evt=None):
		# first, update status bar with current directory
		sRenderDirectory = self.sheetdata.directory
		if len(sRenderDirectory)>42: sRenderDirectory=sRenderDirectory[0:42]+'...'
		self.statusbar.PushStatusText(sRenderDirectory, 1)
		
		self.sheetdata.refresh()

		# This should be the only time the number of rows can change. Make sure to resize grid.
		self.ogrid.ensureRows(self.sheetdata.getLength())
		self.sheetdata.renderToGrid(self.ogrid)
		
		for entry in os.listdir(self.sheetdata.directory):
			if cellrename_engine.marker in entry:
				wxutil.alertDialog(self, 'Renaming was apparently not successful. Use Repair to try recovery.','Error')
				break

	def menu_onFileFilter(self, evt):
		strPattern = wxutil.inputDialog(self, 'Enter a filter, like *.mp3.','*.*')
		if not strPattern: return
		self.sheetdata.filter = strPattern
		self.menu_onFileRefresh()

	def menu_onFileIncludeDirs(self, evt):
		self.sheetdata.includeDirs = self.menuIncludeDirs.IsChecked()
		self.menu_onFileRefresh()
		
	def menu_onFileDownload(self, evt):
		# hack: downloading on the main thread for simplicity.
		self.SetStatusText('Downloading...', 0)
		strUrl = wxutil.inputDialog(self, 'Enter a url:','http://www.google.com/')
		if not strUrl: self.SetStatusText('', 0); return
		strOutName = wxutil.saveFileDialog(self, 'Choose destination')
		if not strOutName: self.SetStatusText('', 0); return
		bRet = False
		try:
			import urllib
			urllib.urlretrieve(strUrl, strOutName)
			bRet = True
		except IOError as e:
			self.SetStatusText("Download failed: " + str(e), 0)
		except:
			self.SetStatusText("Download failed: " + str(sys.exc_info()[0]), 0)
		
		if bRet: self.SetStatusText('Download complete.', 0)

	def menu_onFileSortMod(self, evt):
		self.on_sort_event('modifiedTime', self.sortingAtoZ)
		self.sortingAtoZ = not self.sortingAtoZ # reverse order for the next sort

	def menu_onFileSortCreated(self, evt):
		self.on_sort_event('creationTime', self.sortingAtoZ)
		self.sortingAtoZ = not self.sortingAtoZ # reverse order for the next sort

	def menu_onFileExit(self, evt):
		self.Close()

	def menu_onEditUndo(self, evt):
		#Attempt to undo last rename
		if not self.undoData:
			return
		ato, afrom = self.undoData #note reverse order, because rename to from
		result = cellrename_engine.renameFiles(self.sheetdata.directory, afrom, ato)
		if result==True:
			self.writeResult( 'Successfully undone.')
			self.menu_onFileRefresh()
		else:
			self.writeResult(result)
		self.menuUndo.Enable(False)

	def onSuffixOrPrefix(bPrefix):
		strAdded = wxutil.inputDialog(self, 'Add a '+('prefix' if bPrefix else 'suffix')+'.','')
		if not strAdded: return
		self.sheetdata.reloadFromGrid(self.ogrid)
		self.sheetdata.transform_suffixorprefix(True, strAdded)
		self.sheetdata.renderToGrid(self.ogrid)
	def menu_onEditPrefix(self, evt):
		onSuffixOrPrefix(True)
	def menu_onEditSuffix(self, evt):
		onSuffixOrPrefix(False)

	def menu_onEditNumber(self, evt):
		strFirst = wxutil.inputDialog(self, 'Rename into a sequence like 01, 02, 03, and so on. Please enter the first entry of the sequence (e.g. "1", or "001", or "42").','001')
		if not strFirst: return
		self.sheetdata.reloadFromGrid(self.ogrid)
		ret = self.sheetdata.transform_addnumber(strFirst)
		self.sheetdata.renderToGrid(self.ogrid)
		if ret!=True and ret: wxutil.alertDialog(self, ret)
			
	def menu_onEditPattern(self, evt):
		strPattern = wxutil.inputDialog(self, 'Enter a naming pattern. The following can be used:/n/n/t%n=padded number (i.e. 001, 002)/n/t%N=number/n/t%f=file name/n/t%U=uppercase name/n/t%u=lowercase name'.
			replace('/n','\r\n').replace('/t','     ')	,'%f')
		if not strPattern: return
		self.sheetdata.reloadFromGrid(self.ogrid)
		self.sheetdata.transform_pattern(strPattern)
		self.sheetdata.renderToGrid(self.ogrid)

	def menu_onEditReplace(self, evt):
		strSearch = wxutil.inputDialog(self, 'Search for string: ')
		if not strSearch: return
		strReplace = wxutil.inputDialog(self, 'And replace with: ')
		if strReplace==None: return
		
		self.sheetdata.reloadFromGrid(self.ogrid)
		# regex replace, or regex case-insensitive replace (as documented in 'tips')
		if strSearch.startswith('r:'):
			self.sheetdata.transform_regexreplace(strSearch[len('r:'):], strReplace,True,True)
		elif strSearch.startswith('ri:'):
			self.sheetdata.transform_regexreplace(strSearch[len('ri:'):], strReplace,True,False)
		elif strSearch.startswith('i:'):
			self.sheetdata.transform_regexreplace(strSearch[len('i:'):], strReplace,False,False)
		else:
			self.sheetdata.transform_replace(strSearch, strReplace)
		self.sheetdata.renderToGrid(self.ogrid)
	
	
	def menu_onHelpAbout(self, evt):
		wxutil.alertDialog(self,'RenameCells 0.2, by Ben Fisher, 2008.\nhttp://halfhourhacks.blogspot.com\n\nReleased under the GPLv3 license.','About')

	def menu_onHelpTips(self, evt):
		sTips = r'''Copy a directory path and open CellRename to start in that directory.

In Replace, to use case-insensitive matching put i: before your query.

In Replace, to use regex, put r: before your query. Groups work, so "r:(\w+),(\w+)" to "\2,\1" will turn "first,second" into "second,first". '''
		wxutil.alertDialog(self,sTips,'Tips')

	
	#################
	
	# respond to message sent by grid
	def on_sort_event(self, strField, bReverse): #strField is the name of the column to sort by
		self.sheetdata.reloadFromGrid(self.ogrid)
		self.sheetdata.sort(strField, bReverse)
		self.sheetdata.renderToGrid(self.ogrid)
		


	def do_rename(self, evt=None):
		self.sheetdata.reloadFromGrid(self.ogrid)
		afrom, ato = self.sheetdata.prepareLists()
		worked = False #if nothing happened, can't undo
		try:
			worked = cellrename_engine.renameFiles(self.sheetdata.directory, afrom, ato)
			
		finally:
			if not worked:
				self.undoData = afrom, ato
				wxutil.alertDialog(self, 'An error occurred. If there are temporary filenames after refreshing, try using repair.','Warning')
		
		if worked == True:
				self.menuUndo.Enable(True)
				self.undoData = afrom, ato
				
				self.menu_onFileRefresh()
				self.SetStatusText( str(self.sheetdata.getLength())+' files renamed.', 0)
		else:
				self.SetStatusText( str(result), 0)
			
	
	def menu_renameRepair(self, evt):
		afrom = self.undoData[0]
		result = cellrename_engine.repair(self.sheetdata.directory, afrom)
		if result==0: self.writeResult( 'No temporary files found.')
		else: 
			self.writeResult( 'File names repaired.')
			self.menu_onFileRefresh()



class MyApp(wx.App):
	def OnInit(self):
		if os.name=='nt':
			strDirectory = os.path.join(os.environ['USERPROFILE'], 'Documents')
		else:
			strDirectory = os.environ['$HOME']
		
		strFilter='*'
		bIncludeDirs=False
		
		# use a directory from the clipboard if available
		sclip = wxutil.getClipboardText()
		if sclip and (':' in sclip or '\\\\' in sclip) and '\\' in sclip:
			if wxutil.isDirectory(sclip):
				strDirectory = sclip
			elif os.path.exists(sclip):
				parent = os.path.split(sclip)[0]
				if wxutil.isDirectory(parent): strDirectory = parent
		
		frame = SheetFrame(None, -1, 'RenameCells', strDirectory, strFilter, bIncludeDirs)
		frame.Show(True)
		return True

app = MyApp(0)
app.MainLoop()

