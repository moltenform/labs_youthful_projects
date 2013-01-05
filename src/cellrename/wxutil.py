
# Simple wrappers for wxPython dialogs
# Ben Fisher

import wx

def alertDialog(window, text,title='Message', style = wx.OK):
	#Possible styles: wx.OK, wx.CANCEL, wx.YES_NO
	#alert = wx.MessageDialog(self, text, style)

	dlg = wx.MessageDialog(window, text, caption=title, style=style)
	res = dlg.ShowModal()
	dlg.Destroy()
	if res == wx.ID_YES: return True
	elif res == wx.ID_NO: return False
	if res == wx.ID_OK: return True
	else: return False

def inputDialog(window, text, default=''):
	dlg = wx.TextEntryDialog(window, text)
	dlg.SetValue(default)
	if dlg.ShowModal() == wx.ID_OK:
		return dlg.GetValue()
	else:
		return None
		

def saveFileDialog(window, text, defaultDirectory=None, defaultFilename=None):
	dialog = wx.FileDialog(window, style = wx.SAVE, message=text)
	
	if defaultDirectory!=None: dialog.SetDirectory(defaultDirectory)
	if defaultFilename!=None: dialog.SetFilename(defaultFilename)
	if dialog.ShowModal() == wx.ID_OK:
		return dialog.GetPath()
	else:
		return None

def openFileDialog(window, text, bMultiple=False, defaultDirectory=None, defaultFilename=None):
	if bMultiple: style = wx.MULTIPLE
	else: style = wx.OPEN
	dialog = wx.FileDialog(window, style=style, message=text)
	
	if defaultDirectory!=None: dialog.SetDirectory(defaultDirectory)
	if defaultFilename!=None: dialog.SetFilename(defaultFilename)
	if dialog.ShowModal() == wx.ID_OK:
		if bMultiple:
			return dialog.GetPaths()
		else:
			return dialog.GetPath()
	else:
		return None

# Other wrappers:
def getClipboardText():
	sRet = None
	if wx.TheClipboard.Open():
		try:
			otext = wx.TextDataObject()
			if wx.TheClipboard.GetData(otext):
				sRet = otext.GetText()
		finally:
			wx.TheClipboard.Close()
	return sRet

class PseudoFile(object):
	def __init__(self, writefn, name='', encoding=None):
		self.writefn = writefn
		self.name = name
	def write(self, s):
		self.writefn(s)
	def writelines(self, l):
		map(self.write, l)
	def flush(self):
		pass
	def isatty(self):
		return True

def isDirectory(dir_name):
	import os
	mask = 0x4000
	try: s = os.stat(dir_name)
	except: return 0
	if (mask & s[0]) == mask: return 1
	else: return 0

if __name__=='__main__':
	app = wx.PySimpleApp()
	print openFileDialog(None, 'hello', True)