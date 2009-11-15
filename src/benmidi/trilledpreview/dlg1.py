
from Tkinter import *
import tkSimpleDialog

def pack(o, **kwargs): o.pack(**kwargs); return o
class Dlg1(tkSimpleDialog.Dialog):
	def __init__(self, parent, opt=None):
		self.opt = opt
		
		title = 'Settings...'
		tkSimpleDialog.Dialog.__init__(self, parent, title)
		
	def body(self, top):
		
		fr1= Frame(top)
		fr1.grid(row=0, column=0)
		pack(Label(fr1,text='key signature:'))
		self.optionskeysig= ['(sharps)','(flats)']
		self.varkeysig = StringVar()
		self.varkeysig.set(self.optionskeysig[0])
		pack(OptionMenu(fr1, self.varkeysig, *self.optionskeysig), side=LEFT)
		
		fr2= Frame(top)
		fr2.grid(row=0, column=1)
		pack(Label(fr2,text='clef:'), side=LEFT)
		self.optionsclef= ['treble','bass']
		self.varclef = StringVar()
		self.varclef.set(self.optionsclef[0])
		pack(OptionMenu(fr2, self.varclef, *self.optionsclef), side=LEFT)
		
		fr3= Frame(top)
		fr3.grid(row=0, column=2)
		pack(Label(fr3,text='time signature:'), side=LEFT)
		self.optionstime= ['4/4','2/4','3/4','6/8']
		self.vartime = StringVar()
		self.vartime.set(self.optionstime[0])
		pack(OptionMenu(fr3, self.vartime, *self.optionstime), side=LEFT)
		
		
	def apply(self): #called when Ok clicked.
		nchosen = self.optionskeysig.index(self.varkeysig.get())
		if nchosen == 0:
			bSharps = True
		else:
			bSharps = False
		nclef = self.optionsclef.index(self.varclef.get())
		if nclef == 0:
			bTreble = True
		else:
			bTreble = False
		
		num,den = self.vartime.get().split('/')
		num=int(num); den=int(den)
		
		self.result = (bSharps, bTreble, (num,den))

if __name__=='__main__':
	def showdlg():
		dlg = Dlg1(main)
		print  dlg.result
		
	main=Tk()
	Button(main, text='go', command=showdlg).pack()
	main.mainloop()
	
