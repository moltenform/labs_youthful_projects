'''Simple wrappers for wxPython dialogs
Ben Fisher'''

import os
import wx


def alertDialog(window, text, title='Message', style=wx.OK):
    # possible styles: wx.OK, wx.CANCEL, wx.YES_NO
    dlg = wx.MessageDialog(window, text, caption=title, style=style)
    res = dlg.ShowModal()
    dlg.Destroy()
    if res == wx.ID_YES:
        return True
    elif res == wx.ID_NO:
        return False
    elif res == wx.ID_OK:
        return True
    else:
        return False


def inputDialog(window, text, default=''):
    dlg = wx.TextEntryDialog(window, text)
    dlg.SetValue(default)
    if dlg.ShowModal() == wx.ID_OK:
        return dlg.GetValue()
    else:
        return None


def saveFileDialog(window, text, defaultDirectory=None, defaultFilename=None):
    dlg = wx.FileDialog(window, style=wx.SAVE, message=text)

    if defaultDirectory != None:
        dlg.SetDirectory(defaultDirectory)
    if defaultFilename != None:
        dlg.SetFilename(defaultFilename)
    if dlg.ShowModal() == wx.ID_OK:
        return dlg.GetPath()
    else:
        return None


def openFileDialog(window, text, bMultiple=False, defaultDirectory=None, defaultFilename=None):
    try:
        open = wx.OPEN
    except AttributeError:
        open = wx.FD_OPEN
    try:
        multiple = wx.MULTIPLE
    except AttributeError:
        multiple = wx.FD_MULTIPLE
    if bMultiple:
        style = multiple
    else:
        style = open
    dlg = wx.FileDialog(window, style=style, message=text)

    if defaultDirectory != None:
        dlg.SetDirectory(defaultDirectory)
    if defaultFilename != None:
        dlg.SetFilename(defaultFilename)
    if dlg.ShowModal() == wx.ID_OK:
        if bMultiple:
            return dlg.GetPaths()
        else:
            return dlg.GetPath()
    else:
        return None


# other wrappers:
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


def setClipboardText(s):
    bRet = False
    if wx.TheClipboard.Open():
        try:
            otext = wx.TextDataObject()
            otext.SetText(s)
            wx.TheClipboard.Clear()
            wx.TheClipboard.AddData(otext)
            bRet = True
        finally:
            wx.TheClipboard.Close()
    return bRet


if __name__ == '__main__':
    app = wx.PySimpleApp()
    openFileDialog(None, 'Test', True)
