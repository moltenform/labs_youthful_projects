
from Tkinter import *
import tkSimpleDialog

def pack(o, **kwargs): o.pack(**kwargs); return o
class Dlg3(tkSimpleDialog.Dialog):
	def __init__(self, parent, opt=None):
		self.opt = opt
		
		title = 'Get ready...'
		tkSimpleDialog.Dialog.__init__(self, parent, title)
		
	def body(self, top):
		
		fr1= Frame(top)
		fr1.pack(side=TOP, padx=30, pady=30)
		pack(Label(fr1,text="Click OK to start recording."),side=TOP)
		pack(Label(fr1,text="Remember to tap the Tab key!"),side=TOP)
		
		
	def apply(self): #called when Ok clicked.
		self.result = True #to indicate Ok clicked

if __name__=='__main__':
	def showdlg():
		dlg = Dlg3(main)
		print  dlg.result
		
	main=Tk()
	Button(main, text='go', command=showdlg).pack()
	main.mainloop()
	
