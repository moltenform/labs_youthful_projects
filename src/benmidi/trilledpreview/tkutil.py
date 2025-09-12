
import os
import sys

class Callable(object):
    def __init__(self, func, *args, **kwds):
        self.func = func
        self.args = args
        self.kwds = kwds
    def __call__(self, event=None):
        return self.func(*self.args, **self.kwds)
    def __str__(self):
        return self.func.__name__

gDirectoryHistory = dict()
def _getFileDialogGui(fn, initialdir, types, title):
    if initialdir is None:
        initialdir = gDirectoryHistory.get(repr(types), '.')
    
    kwargs = dict()
    if types is not None:
        aTypes = [(type.split('|')[1], type.split('|')[0]) for type in types]
        defaultExtension = aTypes[0][1]
        kwargs['defaultextension'] = defaultExtension
        kwargs['filetypes'] = aTypes
    
    result = fn(initialdir=initialdir, title=title, **kwargs)
    if result:
        gDirectoryHistory[repr(types)] = os.path.split(result)[0]
        
    return result

def ask_openfile(initialfolder=None, types=None, title='Open'):
    "Specify types in the format ['.png|Png image','.gif|Gif image'] and so on."
    if isPy3OrNewer:
        import tkinter.filedialog as tkFileDialog
    else:
        import tkFileDialog
    return _getFileDialogGui(tkFileDialog.askopenfilename, initialfolder, types, title)

def ask_savefile(initialfolder=None, types=None, title='Save As'):
    "Specify types in the format ['.png|Png image','.gif|Gif image'] and so on."
    if isPy3OrNewer:
        import tkinter.filedialog as tkFileDialog
    else:
        import tkFileDialog
    return _getFileDialogGui(tkFileDialog.asksaveasfilename, initialfolder, types, title)

def alert(message, title=None, icon='info'):
    "Show dialog with information. Icon can be one of 'info','warning','error', defaulting to 'info'."
    if isPy3OrNewer:
        from tkinter import messagebox as tkMessageBox
    else:
        import tkMessageBox
    if icon=='info':
        return tkMessageBox.showinfo(title=title, message=message)
    elif icon=='warning':
        return tkMessageBox.showwarning(title=title, message=message)
    elif icon=='error':
        return tkMessageBox.showerror(title=title, message=message)
 
isPy3OrNewer = sys.version_info[0] > 2
 
