
# cellrename, by Ben Fisher. GPLv3.
# https://github.com/moltenform/labs_youthful_projects/tree/master/src/cellrename

import os
import sys
import string
import tempfile
import json
import traceback
import wx
import wxutil
import cellrename_data
import cellrename_engine
import cellrename_grid
import cellrename_comboboxask

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
        addMenuItem(menuFile, 'Sort by last-&modified time',self.onMenuFileSortMod)
        addMenuItem(menuFile, 'Sort by file-&created time',self.onMenuFileSortCreated)
        addMenuSeparator(menuFile)
        addMenuItem(menuFile, 'E&xit', self.onMenuFileExit)
        
        menuEdit = wx.Menu()
        self.menuobj_undo = addMenuItem(menuEdit, '&Undo Rename', self.onMenuEditUndo, 'Ctrl+Z')
        addMenuSeparator(menuEdit)
        addMenuItem(menuEdit, 'Add &Prefix...', self.onMenuEditPrefix, 'Ctrl+Shift+P')
        addMenuItem(menuEdit, 'Add &Suffix...', self.onMenuEditSuffix, 'Ctrl+Shift+S')
        addMenuItem(menuEdit, 'Add &Number...', self.onMenuEditNumber, 'Ctrl+3')
        addMenuItem(menuEdit, 'P&attern...', self.onMenuEditPattern, 'Ctrl+P')
        addMenuSeparator(menuEdit)
        addMenuItem(menuEdit, '&Replace in filenames...', self.onMenuEditReplace, 'Ctrl+H')
        addMenuItem(menuEdit, 'Replace r&egexp in filenames...', self.onMenuEditRegexReplace, 'Ctrl+Shift+H')
        
        menuHelp = wx.Menu()
        addMenuItem(menuHelp, '&About CellRename', self.onMenuHelpAbout)
        addMenuItem(menuHelp, '&Tips', self.onMenuHelpTips)
        addMenuItem(menuHelp, '&Documentation', openOnlineDocs)

        menubar = wx.MenuBar()
        menubar.Append(menuFile, '&File')
        menubar.Append(menuEdit, '&Edit')
        menubar.Append(menuHelp, '&Help')
        self.SetMenuBar(menubar)
        self.Centre()
        
    def writeStatus(self, s):
        self.SetStatusText(s, 0)
    
    def onMenuFileOpen(self, evt):
        try:
            open = wx.OPEN
        except AttributeError:
            open = wx.FD_OPEN
        dialog = wx.FileDialog(self, style=open, message="Select any file in the target directory:" )
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
        if ret != True and ret:
            wxutil.alertDialog(self, str(ret))
            
    def onMenuEditPattern(self, evt):
        sPattern = wxutil.inputDialog(self, 'Enter a naming pattern. The following can be used:/n/n/t%n=padded number (i.e. 001, 002)/n/t%N=number/n/t%f=file name/n/t%U=uppercase name/n/t%u=lowercase name/n/t%t=titlecase name'.
            replace('/n',os.linesep).replace('/t','     '),'%f')
        if not sPattern:
            return
        
        self.grid.writeToModel(self.data)
        ret = self.data.transformWithPattern(sPattern)
        self.grid.loadFromModel(self.data)
        if ret != True and ret:
            wxutil.alertDialog(self, str(ret))

    def onDoReplace(self, bRegex):
        sSearch = askWithHistory(self, 'regexsearch' if bRegex else 'search', 'Search for regex pattern: ' if bRegex else 'Search for string: ')
        if not sSearch:
            return
        
        sReplace = askWithHistory(self, 'regexrepl' if bRegex else 'repl', 'And replace with: ')
        if sReplace == None:
            return
        
        self.grid.writeToModel(self.data)
        
        if bRegex:
            ret = self.data.transformRegexReplace(sSearch, sReplace, True, True)
        else:
            ret = self.data.transformReplace(sSearch, sReplace)
        
        self.grid.loadFromModel(self.data)
        if ret != True and ret:
            wxutil.alertDialog(self, str(ret))

    def onMenuEditReplace(self, evt):
        return self.onDoReplace(False)
    
    def onMenuEditRegexReplace(self, evt):
        return self.onDoReplace(True)

    def onMenuHelpAbout(self, evt):
        wxutil.alertDialog(self, '''cellrename: renaming files with a spreadsheet-like UI.
        
Ben Fisher, 2008, GPL
https://github.com/moltenform/labs_youthful_projects/tree/master/src/cellrename
https://moltenform.com/page/cellrename/doc/''', 'About')

    def onMenuHelpTips(self, evt):
        sTips = r'''Tips

Copy a directory path and open CellRename to start in that directory.

When replacing with regex, capture groups work:
replacing "(\w+),(\w+)" with "\2,\1" will turn "first,second" into "second,first". 

See also, the documentation at
https://github.com/moltenform/labs_youthful_projects/tree/master/src/cellrename
'''
        wxutil.alertDialog(self, sTips, 'Tips')
    
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

def historyLocation():
    return tempfile.gettempdir() + os.sep + 'cellrename_tmp_history.txt'

def stripHistory(obj):
    def isOkChar(c):
        return c in string.printable and c != '\t' and c != '\r' and c != '\n' and c != '\v' and c != '\f'
    
    maxHistoryEntryLen = 100
    maxHistoryPerKey = 10
    keys = [key for key in obj]
    for key in keys:
        obj[key] = obj[key][0:maxHistoryPerKey]
        for i, entry in enumerate(obj[key]):
            obj[key][i] = obj[key][i][0:maxHistoryEntryLen]
            obj[key][i] = ''.join(c for c in obj[key][i] if isOkChar(c))

def loadHistory():
    if not os.path.exists(historyLocation()):
        return {}
    try:
        contents = open(historyLocation(), 'r').read()
        obj = json.loads(contents)
        stripHistory(obj)
        return obj
    except:
        traceback.print_exc()
        return {}

def saveHistory(obj):
    stripHistory(obj)
    try:
        f = open(historyLocation(), 'w')
        f.write(json.dumps(obj))
        f.close()
    except:
        traceback.print_exc()

def askWithHistory(parent, key, prompt):
    history = loadHistory()
    if key not in history:
        history[key] = []
            
    arChoices = history[key]
    val = cellrename_comboboxask.comboboxAsk(None, id=-1, title='CellRename', prompt=prompt, arChoices=arChoices)
    if val and val not in arChoices:
        arChoices.insert(0, val)
        saveHistory(history)
    
    return val

def openOnlineDocs(context=None):
    import webbrowser
    url = 'https://github.com/moltenform/labs_youthful_projects/tree/master/src/cellrename/README.md'
    webbrowser.open(url, new=1)

def isListableDir(s):
    if not os.path.isdir(s):
        return False
    try:
        os.listdir(s)
        return True
    except:
        return False
    
class MyApp(wx.App):
    def OnInit(self):
        # find an appropriate starting directory
        if os.name == 'nt':
            userprof = os.environ.get('USERPROFILE', '')
            sDirectory = os.path.join(userprof, 'Documents')
            if not isListableDir(sDirectory):
                sDirectory = os.path.join(userprof, 'My Documents')
            if not isListableDir(sDirectory):
                sDirectory = u'C:\\'
        else:
            sDirectory = os.environ.get('HOME', '/')
            if not isListableDir(sDirectory):
                sDirectory = u'/'
        
        if sys.version_info[0] <= 2:
            sDirectory = unicode(sDirectory)
        sFilter = '*'
        bIncludeDirs = False
        
        # use a directory from the clipboard if available
        try:
            sclip = wxutil.getClipboardText()
        except:
            sclip = None
        if sclip and os.sep in sclip:
            if isListableDir(sclip):
                sDirectory = sclip
            elif os.path.exists(sclip):
                parent = os.path.split(sclip)[0]
                if isListableDir(parent):
                    sDirectory = parent
        
        frame = CellRenameMain(None, -1, 'CellRename', sDirectory, sFilter, bIncludeDirs)
        frame.Show(True)
        return True

if getattr(sys, 'frozen', False):
    sys.stdout = open(tempfile.gettempdir() + os.sep + 'cellrename_tmpstdout.txt', 'w')
    sys.stderr = open(tempfile.gettempdir() + os.sep + 'cellrename_tmpstderr.txt', 'w')

app = MyApp(0)
app.MainLoop()

