
try:
    import Tkinter
    import tkFileDialog
    import tkMessageBox
    import tkSimpleDialog
    import tkColorChooser
except ImportError:
    import tkinter as Tkinter
    import tkinter.filedialog as tkFileDialog
    import tkinter.messagebox as tkMessageBox
    import tkinter.simpledialog as tkSimpleDialog
    import tkinter.colorchooser as tkColorChooser

# Wrappers of tk dialogs, hopefully providing a more intuitive interface.

def ask_folder(initialfolder='.',title='Please choose a directory, and then press OK.'):
    """Open a file dialog that asks the user to choose a folder (directory). The path is returned as a string."""
    import tkFileDialog
    strFolder = tkFileDialog.askdirectory(initialdir=initialfolder,title=title)
    return strFolder

def ask_openfile(initialfolder='.',title='Open',types=None):
    """Open a native file dialog that asks the user to choose a file. The path is returned as a string.
    Specify types in the format ['.bmp|Windows Bitmap','.gif|Gif image'] and so on.
    """
    # for the underlying tkinter, Specify types in the format type='.bmp' types=[('Windows bitmap','.bmp')]
    if types!=None:
        aTypes = [(type.split('|')[1],type.split('|')[0]) for type in types]
        defaultExtension = aTypes[0][1]
        strFiles = tkFileDialog.askopenfilename(initialdir=initialfolder,title=title,defaultextension=defaultExtension,filetypes=aTypes)
    else:
        strFiles = tkFileDialog.askopenfilename(initialdir=initialfolder,title=title)
    return strFiles

def ask_savefile(initialfolder='.',title='Save As',types=None):
    """Open a native file "save as" dialog that asks the user to choose a filename. The path is returned as a string.
    Specify types in the format ['.bmp|Windows Bitmap','.gif|Gif image'] and so on.
    """
    if types!=None:
        aTypes = [(type.split('|')[1],type.split('|')[0]) for type in types]
        defaultExtension = aTypes[0][1]
        strFiles = tkFileDialog.asksaveasfilename(initialdir=initialfolder,title=title,defaultextension=defaultExtension,filetypes=aTypes)
    else:
        strFiles = tkFileDialog.asksaveasfilename(initialdir=initialfolder,title=title)
    return strFiles

def ask_color(initialcolor=None, title='Color',format='tuplet'):
    """
    Open color chooser dialog. Specify format='tk' to receive a value in string format, such as '#68d995'
    Specify initialcolor as an RGB triplet such as (200,0,0).
    Returns None upon cancel.
    """
    res = tkColorChooser.askcolor(color=initialcolor, title=title)
    if format=='tk':
        return res[1]
    else:
        return res[0]

def ask(prompt, default=None, title=''):
    """ Get input from the user. default refers to the value which is initially in the field. Returns None on cancel."""
    if default:
        return tkSimpleDialog.askstring(title, prompt, initialvalue=default)
    else:
        return tkSimpleDialog.askstring(title, prompt)

def ask_integer(prompt, default=None, min=0,max=100, title=''):
    """ Get input from the user, validated to be an integer. default refers to the value which is initially in the field. By default, from 0 to 100; change this by setting max and min. Returns None on cancel."""
    if default:
        return tkSimpleDialog.askinteger(title, prompt, minvalue=min, maxvalue=max, initialvalue=default)
    else:
        return tkSimpleDialog.askinteger(title, prompt, minvalue=min, maxvalue=max)

def ask_float(prompt, default=None, min=0.0,max=100.0, title=''):
    """ Get input from the user, validated to be an float (decimal number). default refers to the value which is initially in the field. By default, from 0.0 to 100.0; change this by setting max and min. Returns None on cancel."""
    if default:
        return tkSimpleDialog.askfloat(title, prompt, minvalue=min, maxvalue=max, initialvalue=default)
    else:
        return tkSimpleDialog.askfloat(title, prompt, minvalue=min, maxvalue=max)

def ask_yesno(prompt, title=None):
    """ Ask yes or no. Returns True on yes and False on no."""
    return tkMessageBox.askyesno(title=title, message=prompt)
    
def ask_okcancel(prompt, title=None):
    """ Ask ok or Cancel. Returns True on OK and False on Cancel."""
    return tkMessageBox.askokcancel(title=title, message=prompt)

def alert(message, title=None, icon='info'):
    """ Show dialog with information. Icon can be one of 'info','warning','error', defaulting to 'info'."""
    if icon=='info':
        return tkMessageBox.showinfo(title=title, message=message)
    elif icon=='warning':
        return tkMessageBox.showwarning(title=title, message=message)
    elif icon=='error':
        return tkMessageBox.showerror(title=title, message=message)

if __name__=='__main__':
    root = Tkinter.Tk()
    test = 2
    if test==0:
        root.withdraw()
        print(ask_folder())
        print(ask_openfile())
        print(ask_openfile(initialfolder='../..',title='hello',types=['.bmp|Windows Bitmap','.gif|Gif image']))
        print(ask_savefile())
        print(ask_savefile(initialfolder='../..',title='hello',types=['.bmp|Windows Bitmap','.gif|Gif image']))
        print(ask_color())
        print(ask_color(initialcolor=(200,0,0),title='That color',format='tk'))
    elif test==1:
        root.update()
        print(ask('what is your name?'))
        print(ask('what is your name?','bob','Question?'))
        print(ask_integer('what is a number?'))
        print(ask_integer('what is a big number?','348',100,500,'That number'))
        print(ask_float('what is a decimal number?'))
        print(ask_float('what is a big decimal number?','348',100,500,'That number'))
    elif test==2:
        print(alert('Hello!'))
        print(alert('Hello!','This','warning'))
        print(alert('Hello!','This','error'))
        print(ask_yesno('yes or no?','question'))
        print(ask_okcancel('continue?','uh oh'))
