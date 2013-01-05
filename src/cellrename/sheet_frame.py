#http://zetcode.com/wxpython/layout/

import wx
import wxutil
import sheet_data
import sheet_sheet
import engine

ID_fileRefresh=701;ID_fileQuit=702; ID_renamePattern=710;ID_renameRegexp=711;ID_renameReplace=712;ID_renameGo=713;ID_renameRepair=714

class SheetFrame(wx.Frame):
	def __init__(self, parent, id, title, strDirectory, strFilter, bIncludeDirs=False):
		framesize = wx.Size(600, 400)
		wx.Frame.__init__(self, parent, id, title, wx.DefaultPosition, framesize)

		self.directory = strDirectory
		self.sheetdata = sheet_data.SheetData(strDirectory, strFilter, bIncludeDirs)
		self.undoData = [],[]

		menubar = wx.MenuBar()
		file = wx.Menu()
		rename = wx.Menu()
		file.Append(ID_fileRefresh, '&Refresh', 'Refresh file list')
		file.AppendSeparator()
		file.Append(ID_fileQuit, '&Quit', 'Quit the Application')
		
		rename.Append(ID_renamePattern,'Rename on &Pattern')
		rename.Append(ID_renameReplace,'&Replace in filenames')
		rename.Append(ID_renameRegexp,'Regular &Expression')
		rename.AppendSeparator()
		rename.Append(ID_renameGo,'Rename Files')
		rename.Append(ID_renameRepair,'Repair')
		
		menubar.Append(file, '&File')
		menubar.Append(rename, '&Rename')
		
		self.SetMenuBar(menubar)
		self.Centre()
		self.Bind(wx.EVT_MENU, self.menu_OnRefresh, id=ID_fileRefresh)
		self.Bind(wx.EVT_MENU, self.menu_OnQuit, id=ID_fileQuit)
		self.Bind(wx.EVT_MENU, self.menu_renamePattern, id=ID_renamePattern)
		self.Bind(wx.EVT_MENU, self.menu_renameRegexp, id=ID_renameRegexp)
		self.Bind(wx.EVT_MENU, self.menu_renameReplace, id=ID_renameReplace)
		self.Bind(wx.EVT_MENU, self.menu_renameRepair, id=ID_renameRepair)
		self.Bind(wx.EVT_MENU, self.do_rename, id=ID_renameGo)

		#Add everything #1
		panelSizer = wx.BoxSizer(wx.VERTICAL)
		panel = wx.Panel(self)
		panelSizer.Add(panel, 1, wx.EXPAND)
		
		#Create Widgets
		self.ogrid = sheet_sheet.CellRenameGrid(panel,self.sheetdata.length, self.on_sort_event)
		lblResults = wx.StaticText(panel, wx.ID_ANY, "Results: ")
		self.txtResults = wx.TextCtrl(panel, style=wx.TE_MULTILINE, size=(250, 40))
		btnGo = wx.Button(panel, wx.ID_ANY, 'Rename')
		btnGo.Bind(wx.EVT_BUTTON, self.do_rename)
		self.btnUndo = wx.Button(panel, wx.ID_ANY, 'Undo')
		self.btnUndo.Bind(wx.EVT_BUTTON, self.do_undo)
		self.btnUndo.Enable(False)
		
		#Layout Widgets
		rowResults = wx.BoxSizer(wx.HORIZONTAL)
		rowResults.Add( lblResults, 0)
		rowResults.Add( self.txtResults, 0)
		rowResults.Add( btnGo, 0, wx.LEFT, 10)
		rowResults.Add( self.btnUndo, 0, wx.LEFT, 10)
		
		sizerMain = wx.BoxSizer(wx.VERTICAL)
		sizerMain.Add(self.ogrid, 1, wx.EXPAND)
		sizerMain.Add(rowResults, 0, wx.LEFT|wx.TOP, 10)
		
		#Add everything #2
		self.SetSizer(panelSizer)
		panel.SetSizer(sizerMain)
		self.SetAutoLayout(True)
		self.Centre()
		
		#Actually get the data
		self.menu_OnRefresh()
		
		#redirect output
		import sys
		self.realstdout = sys.stdout
		self.realstderr = sys.stderr
		sys.stdout = wxutil.PseudoFile(self.writeStdOut, 'stdout')
		sys.stderr = wxutil.PseudoFile(self.writeStdOut, 'stderr')

	# respond to message sent by grid
	def on_sort_event(self, strField, bReverse): #strField is the name of the column to sort by
		self.sheetdata.reloadFromGrid(self.ogrid)
		self.sheetdata.sort(strField, bReverse)
		self.sheetdata.renderToGrid(self.ogrid)

	def menu_OnQuit(self, evt):
		self.Close()
	
	def menu_OnRefresh(self, evt=None):
		self.sheetdata.refresh()
		
		# This should be the only time the number of rows can change. Make sure to resize
		self.ogrid.ensureRows( self.sheetdata.length)
		self.sheetdata.renderToGrid(self.ogrid)
		
		import os
		for entry in os.listdir(self.directory):
			if engine.marker in entry:
				wxutil.alertDialog(self, 'It appears that renaming was unsuccessful. Use Repair to try recovery.','Error')
				break
		
	def menu_renameReplace(self,evt):
		strSearch = wxutil.inputDialog(self, 'Search for string: ')
		if not strSearch: return
		strReplace = wxutil.inputDialog(self, 'And replace with: ')
		if strReplace==None: return
			
		self.sheetdata.reloadFromGrid(self.ogrid)
		self.sheetdata.transform_replace(strSearch, strReplace)
		self.sheetdata.renderToGrid(self.ogrid)
	def menu_renameRegexp(self,evt):
		strSearch = wxutil.inputDialog(self, 'Regular expression: ')
		if not strSearch: return
		strReplace = wxutil.inputDialog(self, 'Replace with text: (Use \\1 and so on to refer to groups) ')
		if strReplace==None: return
			
		self.sheetdata.reloadFromGrid(self.ogrid)
		self.sheetdata.transform_regexp(strSearch, strReplace)
		self.sheetdata.renderToGrid(self.ogrid)
	def menu_renamePattern(self,evt):
		strPattern = wxutil.inputDialog(self, 'Enter file pattern (%f=filename, %F=name, %E=extension, %U=uppercase, %u=lowercase, %n=number, %N=raw number)','%f')
		if not strPattern: return
		self.sheetdata.reloadFromGrid(self.ogrid)
		self.sheetdata.transform_pattern(strPattern)
		self.sheetdata.renderToGrid(self.ogrid)
	def do_rename(self, evt=None):
		self.sheetdata.reloadFromGrid(self.ogrid)
		afrom, ato = self.sheetdata.prepareLists()
		worked = False #if nothing happened, can't undo
		try:
			result = engine.renameFiles(self.directory, afrom, ato)
			if result==True:
				self.btnUndo.Enable(True)
				self.undoData = afrom, ato
				self.writeResult( str(self.sheetdata.length)+' files successfully renamed.')
				self.menu_OnRefresh()
				
			else:
				self.writeResult(result)
			worked = True
		finally:
			if not worked:
				self.undoData = afrom, ato
				wxutil.alertDialog(self, 'An error occurred. If there are temporary filenames after refreshing, try using repair.','Warning')
			
	def do_undo(self, evt=None):
		#Attempt to undo last rename
		ato, afrom = self.undoData #note reverse order, because rename to from
		result = engine.renameFiles(self.directory, afrom, ato)
		if result==True:
			self.writeResult( 'Successfully undone.')
			self.menu_OnRefresh()
		else:
			self.writeResult(result)
		self.btnUndo.Enable(False)
	def menu_renameRepair(self, evt):
		afrom = self.undoData[0]
		result = engine.repair(self.directory, afrom)
		if result==0: self.writeResult( 'No temporary files found.')
		else: 
			self.writeResult( 'File names repaired.')
			self.menu_OnRefresh()
	
	def writeResult(self, s):
		self.txtResults.AppendText('\n'+s)
	def writeStdOut(self,s):
		if s!='\n':
			self.writeResult( s.strip())

if __name__=='__main__':
	import wx.grid as gridlib
	class CellRenameGrid(gridlib.Grid):
		def __init__(self, parent):
			gridlib.Grid.__init__(self, parent, -1)
			self.CreateGrid(40, 5)
	
	class MyApp(wx.App):
	    def OnInit(self):
		frame = SheetFrame(None, -1, 'RenameCells', 'test', '*.*')
		frame.Show(True)
		return True

	app = MyApp(0)
	app.MainLoop()
