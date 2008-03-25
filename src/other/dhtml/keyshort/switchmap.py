from Tkinter import *
import os

class App:
	def __init__(self, root):
		root.title('Keyshort-switch map')
		lblMain = Label(root, text='Use the Maps menu to choose a map')
		lblMain.pack(side=TOP, padx=50, pady=50)
		
		
		maps = os.listdir('./maps')
		maps = [fname for fname in maps if fname.endswith('.js')]
		
		menubar = Menu(root)
		menuMaps = Menu(menubar, tearoff=0)
		for fname in maps:
			menuMaps.add_command(label=fname, command = Callable(self.changeMap, fname))
		
		menubar.add_cascade(label="Maps", menu=menuMaps, underline=0)
		root.config(menu=menubar)
		
		
	
	
	def changeMap(self, s):
		import shutil
		print s
		os.remove('./currentmap.js')
		shutil.copy('./maps/'+s, './currentmap.js')

		
		
	
class Callable:
	def __init__(self, func, *args, **kwds):
		self.func = func
		self.args = args
		self.kwds = kwds
	def __call__(self, event=None):
		return apply(self.func, self.args, self.kwds)
	def __str__(self):
		return self.func.__name__
	

root = Tk()
app = App(root)
root.mainloop()