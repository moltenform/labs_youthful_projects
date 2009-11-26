
from Tkinter import *
import tkSimpleDialog

def pack(o, **kwargs): o.pack(**kwargs); return o
class Dlg2(tkSimpleDialog.Dialog):
	def __init__(self, parent, opt=None):
		self.opt = opt
		
		title = 'Settings...'
		tkSimpleDialog.Dialog.__init__(self, parent, title)
		
	def body(self, top):
		
		pack(Label(top,text="When playing, tap the Tab key in time with your playing, to indicate the pulse."),side=TOP)
		pack(Label(top,text="Tap every quarter note."),side=TOP)
		pack(Label(top,text="Currently your recording must start and end with a tap."),side=TOP)
		
		
		fr3= Frame(top)
		fr3.pack(side=TOP, padx=30, pady=30)
		pack(Label(fr3,text='fastest note:'), side=LEFT)
		self.optionstime= ['4th','8th','16th','32th','64th','128th']
		self.vartime = StringVar()
		self.vartime.set(self.optionstime[2])
		pack(OptionMenu(fr3, self.vartime, *self.optionstime), side=LEFT)
		
		pack(Label(top,text="Click OK to start recording."),side=TOP)
		pack(Label(top,text="Remember to tap the Tab key!"),side=TOP)
		
	def apply(self): #called when Ok clicked.
		self.result = int(self.vartime.get().replace('th',''))

if __name__=='__main__':
	def showdlg():
		dlg = Dlg2(main)
		print  dlg.result
		
	main=Tk()
	Button(main, text='go', command=showdlg).pack()
	main.mainloop()
	
