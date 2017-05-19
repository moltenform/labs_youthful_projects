#!/usr/bin/env python2

import os
import sys
import wx
import wxutil
import cellrename_data
import cellrename_engine
import cellrename_grid

class CellRenameMain(wx.Frame):
    sort_reverse_order = False
    menuobj_include_dirs = None
    menuobj_undo = None
    undo_history = None
    grid = None
    statusbar = None
    data = None
    
    def __init__(self, parent, id, title, sDirectory, sFilter, bIncludeDirs=False):
        wx.Frame.__init__(self, parent, id, title, wx.DefaultPosition, wx.Size(550, 200))
        self.data = cellrename_data.CellRenameData(sDirectory, sFilter, bIncludeDirs)
        self.addMenuBar()
        self.menuobj_undo.Enable(False)
        
        # create widgets
        self.grid = cellrename_grid.CellRenameGrid(self, self.data.getLength(), self.onSortEvent)
        self.statusbar = self.CreateStatusBar(2)
        self.statusbar.SetStatusWidths([-1, 280])
        self.statusbar.PushStatusText("Press Ctrl+Enter to rename files.", 0)
        self.statusbar.PushStatusText("", 1)
        
        self.sizer = wx.BoxSizer(wx.VERTICAL)
        self.sizer.Add(self.grid, 1, wx.EXPAND)
        self.SetSizer(self.sizer)
        self.SetAutoLayout(True)
        self.sizer.SetSizeHints(self)

        # load and display the data!
        self.onMenuFileRefresh()
        
    def addMenuBar(self):
        self.current_event_id = 700
        
        def addMenuSeparator(menu):
            menu.AppendSeparator()
        
        def addMenuItem(menu, sName, fnCallback, sHotkey='', kind=None):
            self.current_event_id += 1
            if sHotkey:
                sName += '\t'+sHotkey
            
            self.Bind(wx.EVT_MENU, fnCallback, id=self.current_event_id)
            if kind:
                return menu.Append(self.current_event_id, sName, '', kind=kind)
            else:
                return menu.Append(self.current_event_id, sName, '')
        
        menuFile = wx.Menu()
        addMenuItem(menuFile, '&Open...', self.onMenuFileOpen, 'Ctrl+O')
        addMenuItem(menuFile, '&Refresh', self.onMenuFileRefresh, 'F5')
        addMenuSeparator(menuFile)
        addMenuItem(menuFile, '&Perform Rename', self.onMenuFileRename, 'Ctrl+Enter')
        addMenuSeparator(menuFile)
        addMenuItem(menuFile, 'Filter by &Extension...', self.onMenuFileFilter, 'Ctrl+E')
        self.menuobj_include_dirs = addMenuItem(
            menuFile, '&Include Directories', self.onMenuFileIncludeDirs, 'Ctrl+D', kind=wx.ITEM_CHECK)
        addMenuItem(menuFile, 'Download &URL...', self.onMenuFileDownload, 'Ctrl+U')
        addMenuItem(menuFile, 'Sort by last-&modified time',self.onMenuFileSortMod)
        addMenuItem(menuFile, 'Sort by file-&created time',self.onMenuFileSortCreated)
        addMenuSeparator(menuFile)
        addMenuItem(menuFile, 'E&xit', self.onMenuFileExit)
        
        menuEdit = wx.Menu()
        self.menuobj_undo = addMenuItem(menuEdit, '&Undo Rename', self.onMenuEditUndo, 'Ctrl+Z')
        addMenuSeparator(menuEdit)
        addMenuItem(menuEdit,'Add &Prefix...', self.onMenuEditPrefix, 'Ctrl+Shift+P')
        addMenuItem(menuEdit,'Add &Suffix...', self.onMenuEditSuffix, 'Ctrl+Shift+S')
        addMenuItem(menuEdit,'Add &Number...', self.onMenuEditNumber, 'Ctrl+3')
        addMenuItem(menuEdit,'&Pattern...', self.onMenuEditPattern, 'Ctrl+P')
        addMenuSeparator(menuEdit)
        addMenuItem(menuEdit, '&Replace in Filenames', self.onMenuEditReplace, 'Ctrl+H')
        
        menuHelp = wx.Menu()
        addMenuItem(menuHelp, '&About CellRename', self.onMenuHelpAbout)
        addMenuItem(menuHelp, '&Tips', self.onMenuHelpTips)

        menubar = wx.MenuBar()
        menubar.Append(menuFile, '&File')
        menubar.Append(menuEdit, '&Edit')
        menubar.Append(menuHelp, '&Help')
        self.SetMenuBar(menubar)
        self.Centre()
        
    def writeStatus(self, s):
        self.SetStatusText(s, 0)
    
    def onMenuFileOpen(self, evt):
        dialog = wx.FileDialog(self, style=wx.OPEN, message="Select any file in the target directory:" )
        try:
            if dialog.ShowModal() == wx.ID_OK and dialog.GetPath():
                sPath, sFilename = os.path.split(dialog.GetPath())
                if os.path.exists(sPath):
                    self.data.directory = sPath
                    self.onMenuFileRefresh()
        finally:
            dialog.Destroy()

    def onMenuFileRename(self, evt):
        self.grid.writeToModel(self.data)
        afrom, ato = self.data.prepareLists()
        self.performRename(afrom, ato)
        
    def onMenuFileRefresh(self, evt=None):
        # first, update status bar with current directory
        sRenderDirectory = self.data.directory
        if len(sRenderDirectory) > 40:
            sRenderDirectory = sRenderDirectory[0:40] + '...'
        
        self.statusbar.PushStatusText(sRenderDirectory, 1)
        
        self.data.refresh()
        self.grid.loadFromModel(self.data)
        for entry in os.listdir(self.data.directory):
            if cellrename_engine.marker in entry:
                self.alertRepair()
                break

    def onMenuFileFilter(self, evt):
        sPattern = wxutil.inputDialog(self, 'Enter a filter, like *.mp3.','*')
        if sPattern:
            self.data.filter = sPattern
            self.onMenuFileRefresh()

    def onMenuFileIncludeDirs(self, evt):
        self.data.include_dirs = self.menuobj_include_dirs.IsChecked()
        self.onMenuFileRefresh()
        
    def onMenuFileDownload(self, evt):
        # hack: downloading on the main thread for simplicity.
        self.writeStatus('Downloading...')
        sUrl = wxutil.inputDialog(self, 'Enter a url:','http://www.google.com/')
        if not sUrl:
            self.writeStatus('')
            return
        
        sOutName = wxutil.saveFileDialog(self, 'Save to:')
        if not sOutName:
            self.writeStatus('')
            return
        
        bRet = False
        try:
            import urllib
            urllib.urlretrieve(sUrl, sOutName)
            bRet = True
        except IOError as e:
            self.writeStatus("Download failed: " + str(e))
        except Exception:
            self.writeStatus("Download failed: " + str(sys.exc_info()[0]))
        
        if bRet:
            self.writeStatus('Download complete.')

    def onMenuFileSortMod(self, evt):
        self.onSortEvent('modifiedTime', not self.sort_reverse_order)
        self.sort_reverse_order = not self.sort_reverse_order # reverse order for the next sort

    def onMenuFileSortCreated(self, evt):
        self.onSortEvent('creationTime', self.sort_reverse_order)
        self.sort_reverse_order = not self.sort_reverse_order # reverse order for the next sort

    def onMenuFileExit(self, evt):
        self.Close()

    def onMenuEditUndo(self, evt):
        # attempt to undo last rename
        if not self.undo_history:
            return
        
        sPrevdir, ato, afrom = self.undo_history # note reverse order, because rename to from
        if sPrevdir != self.data.directory:
            wxutil.alertDialog(self, 'Cannot undo, first return to the directory.')
            return
        
        if not wxutil.alertDialog(self, 'Undo renaming these files?', 'Confirm', wx.OK|wx.CANCEL):
            return
        
        self.performRename(afrom, ato)
        self.menuobj_undo.Enable(False)

    def onSuffixOrPrefix(self, bPrefix):
        sAdded = wxutil.inputDialog(self, 'Add a ' + ('prefix' if bPrefix else 'suffix') + '.','')
        if not sAdded:
            return
        
        self.grid.writeToModel(self.data)
        ret = self.data.transformSuffixOrPrefix(bPrefix, sAdded)
        self.grid.loadFromModel(self.data)
        if ret != True and ret:
            wxutil.alertDialog(self, str(ret))
        
    def onMenuEditPrefix(self, evt):
        self.onSuffixOrPrefix(True)
        
    def onMenuEditSuffix(self, evt):
        self.onSuffixOrPrefix(False)

    def onMenuEditNumber(self, evt):
        sFirst = wxutil.inputDialog(self, 'Rename into a sequence like 01, 02, 03, and so on. Please enter the first entry of the sequence (e.g. "1", or "001", or "42").','001')
        if not sFirst:
            return
        
        self.grid.writeToModel(self.data)
        ret = self.data.transformAppendNumber(sFirst)
        self.grid.loadFromModel(self.data)
        if ret!=True and ret:
            wxutil.alertDialog(self, str(ret))
            
    def onMenuEditPattern(self, evt):
        sPattern = wxutil.inputDialog(self, 'Enter a naming pattern. The following can be used:/n/n/t%n=padded number (i.e. 001, 002)/n/t%N=number/n/t%f=file name/n/t%U=uppercase name/n/t%u=lowercase name'.
            replace('/n',os.linesep).replace('/t','     '),'%f')
        if not sPattern:
            return
        
        self.grid.writeToModel(self.data)
        ret = self.data.transformWithPattern(sPattern)
        self.grid.loadFromModel(self.data)
        if ret!=True and ret:
            wxutil.alertDialog(self, str(ret))

    def onMenuEditReplace(self, evt):
        sSearch = wxutil.inputDialog(self, 'Search for string: ')
        if not sSearch:
            return
        sReplace = wxutil.inputDialog(self, 'And replace with: ')
        if sReplace == None:
            return
        
        self.grid.writeToModel(self.data)
        # regex replace, or regex case-insensitive replace (as documented in 'tips')
        if sSearch.startswith('r:'):
            ret = self.data.transformRegexReplace(sSearch[len('r:'):], sReplace,True,True)
        elif sSearch.startswith('ri:'):
            ret = self.data.transformRegexReplace(sSearch[len('ri:'):], sReplace, True, False)
        elif sSearch.startswith('i:'):
            ret = self.data.transformRegexReplace(sSearch[len('i:'):], sReplace, False, False)
        else:
            ret = self.data.transformReplace(sSearch, sReplace)
        
        self.grid.loadFromModel(self.data)
        if ret != True and ret:
            wxutil.alertDialog(self, str(ret))

    def onMenuHelpAbout(self, evt):
        wxutil.alertDialog(self,'''cellrename: renaming files with a spreadsheet-like UI.
        
Ben Fisher, 2008, GPL
http://github.com/downpoured/cellrename
http://halfhourhacks.blogspot.com''','About')

    def onMenuHelpTips(self, evt):
        sTips = r'''Copy a directory path and open CellRename to start in that directory.

In Replace, for case-insensitive matching type "i:" before your query.

In Replace, to use regex, type "r:" before your query. Groups work, so "r:(\w+),(\w+)" to "\2,\1" will turn "first,second" into "second,first". '''
        wxutil.alertDialog(self,sTips,'Tips')

    
    # respond to message sent by grid, sField is the name of the column to sort by
    def onSortEvent(self, sField, bReverse):
        self.grid.writeToModel(self.data)
        self.data.sort(sField, bReverse)
        self.grid.loadFromModel(self.data)
    
    def performRename(self, afrom, ato):
        result = False
        try:
            result = cellrename_engine.renameFiles(self.data.directory, afrom, ato)
        except Exception:
            wxutil.alertDialog(self, 'Exception occurred: '+str(sys.exc_info()[0]))
            return
        
        if result != True:
            wxutil.alertDialog(self, str(result))
            self.writeStatus("")
            return
        
        self.undo_history = self.data.directory, afrom, ato
        self.menuobj_undo.Enable(True)
        self.onMenuFileRefresh()
        self.writeStatus(str(self.data.getLength()) + ' files renamed.')
    
    def alertRepair(self):
        wxutil.alertDialog(self, 'It looks like renaming was not successful. When you click OK, the previous filenames will be placed in the clipboard. Please rename back to normal filenames before continuing, and then restart CellRename.','Error')
        if self.undo_history and self.undo_history[1]:
            afrom = self.undo_history[1]
            s = os.linesep.join(afrom)
            wxutil.setClipboardText(s)

class MyApp(wx.App):
    def OnInit(self):
        # find an appropriate starting directory
        if os.name == 'nt':
            sDirectory = os.path.join(os.environ['USERPROFILE'], 'Documents')
            if not os.path.exists(sDirectory):
                sDirectory = os.path.join(os.environ['USERPROFILE'], 'My Documents')
            if not os.path.exists(sDirectory):
                sDirectory = 'c:\\'
        else:
            sDirectory = os.environ['HOME']
            if not os.path.exists(sDirectory):
                sDirectory = '~/'
            if not os.path.exists(sDirectory):
                sDirectory = '/'
        
        sFilter = '*'
        bIncludeDirs=False
        
        # use a directory from the clipboard if available
        sclip = wxutil.getClipboardText()
        if sclip and os.sep in sclip:
            if os.path.isdir(sclip):
                sDirectory = sclip
            elif os.path.exists(sclip):
                parent = os.path.split(sclip)[0]
                if os.path.isdir(parent): sDirectory = parent
        
        frame = CellRenameMain(None, -1, 'CellRename', sDirectory, sFilter, bIncludeDirs)
        frame.Show(True)
        return True


app = MyApp(0)
app.MainLoop()

