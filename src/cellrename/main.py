#http://zetcode.com/wxpython/layout/

import wx
import wxutil
import os
import sheet_frame
import sys

ID_opendirectory = 101; ID_quit = 103; ID_helpabout = 104

class RenameCellsMain(wx.Frame):
	def __init__(self, parent, id, title):
		framesize = wx.Size(380, 200)
		wx.Frame.__init__(self, parent, id, title, wx.DefaultPosition, framesize)

		menubar = wx.MenuBar()
		file = wx.Menu()
		help = wx.Menu()
		file.Append(ID_opendirectory, '&Open Directory', 'Open a new document')
		#~ file.Append(ID_openfiles, '&Open Files', 'Open files')
		file.AppendSeparator()
		file.Append(ID_quit, '&Quit', 'Quit the Application')
		help.Append(ID_helpabout, 'About','About application')
		
		menubar.Append(file, '&File')
		menubar.Append(help, '&Help')
		
		self.SetMenuBar(menubar)
		self.Centre()
		self.Bind(wx.EVT_MENU, self.menu_OnQuit, id=ID_quit)
		self.Bind(wx.EVT_MENU, self.menu_OpenDirectory, id=ID_opendirectory)
		self.Bind(wx.EVT_MENU, self.menu_HelpAbout, id=ID_helpabout)
		
		
		#Add everything #1
		panelSizer = wx.BoxSizer(wx.VERTICAL)
		panel = wx.Panel(self)
		panelSizer.Add(panel, 1, wx.EXPAND)
		
		
		#Create Widgets
		lblDir = wx.StaticText(panel, wx.ID_ANY, "  Directory: ")
		btnDir = wx.Button(panel, -1, '...')
		lblFilesOfType = wx.StaticText(panel, wx.ID_ANY, "  Files of Type: ")
		self.chkIncludeDirectories = wx.CheckBox(panel, wx.ID_ANY, "  Include Directories")
		btnGo = wx.Button(panel, -1, 'Proceed')
		
		self.comboDir = wx.ComboBox(panel, -1, choices=[], style=wx.CB_DROPDOWN)
		self.comboDir.SetValue('C:\\')
		self.txtExtensions = wx.TextCtrl(panel, size=(40,20))
		self.txtExtensions.SetValue('*')
		
		#Layout Widgets
		sizerMain = wx.BoxSizer(wx.VERTICAL)
		
		rowDir = wx.BoxSizer(wx.HORIZONTAL)
		rowDir.Add(lblDir, 0, wx.ADJUST_MINSIZE) #Add (item, proportion=0, flag=0, border=0, userData=None)
		rowDir.Add(self.comboDir, 1, wx.EXPAND)
		rowDir.Add((15, 1), 0, wx.ADJUST_MINSIZE)
		rowDir.Add(btnDir, 0, wx.ADJUST_MINSIZE)
		
		rowExt = wx.BoxSizer(wx.HORIZONTAL)
		rowExt.Add(lblFilesOfType, 0, wx.ADJUST_MINSIZE)
		rowExt.Add(self.txtExtensions, 0, wx.ADJUST_MINSIZE)
		
		rowInclude = wx.BoxSizer(wx.HORIZONTAL)
		rowInclude.Add((15, 1), 0, wx.ADJUST_MINSIZE)
		rowInclude.Add(self.chkIncludeDirectories, 0, wx.ADJUST_MINSIZE)
		
		rowProceed = wx.BoxSizer(wx.HORIZONTAL)
		rowProceed.Add(btnGo, 0, wx.LEFT|wx.BOTTOM,10)
	
		sizerMain.Add(rowDir, 0, wx.EXPAND|wx.TOP, 10)
		sizerMain.Add(rowExt, 0, wx.EXPAND|wx.TOP, 10)
		sizerMain.Add(rowInclude, 0, wx.EXPAND|wx.TOP, 10)
		sizerMain.Add(rowProceed, 0, wx.ALIGN_CENTER|wx.TOP, 20)
		
		
		#Add everything #2
		self.SetSizer(panelSizer)
		panel.SetSizer(sizerMain)
		self.SetAutoLayout(True)
		
		self.Centre()
		
		# wx.EVT_KILL_FOCUS is not a Command Event i.e. it doesn't propagate up the hierarchy of widgets until it gets handled
		btnDir.Bind(wx.EVT_BUTTON, self.on_btnDirectory)
		btnGo.Bind(wx.EVT_BUTTON, self.on_btnProceed)
		#~ self.txtExtensions.Bind(wx.EVT_SET_FOCUS, self.on_focusExtension)
		
		#prevent too small
		self.SetMinSize(self.GetSize())

		# Make drop target
		dt = FileDrop(self.on_dropFiles)
		self.comboDir.SetDropTarget(dt)

		#if a file/folder was dropped onto the script, load it
		if len(sys.argv)==2:
			self.loadDirectory(sys.argv[1])


	def menu_OnQuit(self, evt):
		self.Close()
	def menu_HelpAbout(self, evt):
		wxutil.alertDialog(self,'RenameCells 0.1, by Ben Fisher, 2008','About')

	def menu_OpenDirectory(self, evt):
		self.on_btnDirectory(None)
		
	#~ def on_focusExtension(self, evt):
		#~ pass
		#~ self.txtExtensions.SetSelection(0,2)
		#~ self.txtExtensions.Refresh()
	
	def on_dropFiles(self, filenames):
		if not filenames: return
		self.loadDirectory(filenames[0])
	
	def on_btnDirectory(self, evt):
		strFile = wxutil.openFileDialog(self, 'Choose a file in the directory to be added.')
		if not strFile: return
		self.loadDirectory(strFile)
		
	def loadDirectory(self, strFile):
		if wxutil.isDirectory(strFile):
			#it is a directory, leave it
			self.comboDir.SetValue(strFile)
			self.comboDir.Append(strFile)
		else:
			path,name = os.path.split(strFile)
			self.comboDir.SetValue(path)
			self.comboDir.Append(path)
			if '.' in name:
				name1, name2 = name.rsplit('.',1)
				self.txtExtensions.SetValue('*.'+name2)
		
	def on_btnProceed(self, evt):
		strDirectory = self.comboDir.GetValue()
		strFilter = self.txtExtensions.GetValue()
		bIncludeDirs = self.chkIncludeDirectories.GetValue()
		
		# Make sure a valid directory was chosen
		if not wxutil.isDirectory(strDirectory):
			wxutil.alertDialog(self,'Apparently not a folder.','Invalid')
			return
			
		# Test if there are actually any files in the directory.
		import fnmatch
		afrom = os.listdir(strDirectory)
		afrom = [fname for fname in afrom if fnmatch.fnmatch(fname, strFilter)]
		if not afrom:
			wxutil.alertDialog(self,'There are no matching files in the folder.','Invalid')
			return
			
		frame = sheet_frame.SheetFrame(None, -1, 'RenameCells', strDirectory, strFilter, bIncludeDirs)
		frame.Show(True)


class FileDrop(wx.FileDropTarget):
	def __init__(self, fnCallback):
		wx.FileDropTarget.__init__(self)
		self.fnCallback = fnCallback
	def OnDropFiles(self, x, y, filenames):
		self.fnCallback(filenames)



class MyApp(wx.App):
    def OnInit(self):
        frame = RenameCellsMain(None, -1, 'RenameCells')
        frame.Show(True)
        return True

app = MyApp(0)
app.MainLoop()
