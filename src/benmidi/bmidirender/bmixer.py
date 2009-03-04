from Tkinter import *

class MixerTrackInfo():
	def __init__(self, enableVar, volWidget, panWidget):
		self.enableVar = enableVar; self.volWidget = volWidget; self.panWidget = panWidget

class BMixerWindow():
	def __init__(self, top, midiObject, opts):
		#should only display tracks with note events. that way, solves some problems
		
		top.title('Mixer')
		
		frameTop = Frame(top, height=600)
		frameTop.pack(expand=YES, fill=BOTH)
		
		ROW_NAME = 0
		ROW_CHECK = 1
		ROW_VOL = 2
		ROW_PAN = 3
		
		Label(frameTop, text='Pan:').grid(row=ROW_PAN, column=0)
		Label(frameTop, text='Volume:').grid(row=ROW_VOL, column=0)
		Label(frameTop, text='').grid(row=ROW_NAME, column=0)
		Label(frameTop, text='Enabled:').grid(row=ROW_CHECK, column=0)
		
		
		
		
		sc = Scale(frameTop, from_=-126, to=126, orient=HORIZONTAL,length=42*2)
		sc.grid(row=ROW_PAN, column=1, sticky='EW')
		sc = Scale(frameTop, from_=127, to=0, orient=VERTICAL)
		sc.grid(row=ROW_VOL, column=1, sticky='NS')
		Label(frameTop, text='    Track 1    ').grid(row=ROW_NAME, column=1)
		chk = Checkbutton(frameTop, text='')
		chk.select()
		chk.grid(row=ROW_CHECK, column=1)
		
		sc = Scale(frameTop, from_=-126, to=126, orient=HORIZONTAL,length=42*2)
		sc.grid(row=ROW_PAN, column=2, sticky='EW')
		sc = Scale(frameTop, from_=127, to=0, orient=VERTICAL)
		sc.grid(row=ROW_VOL, column=2, sticky='NS')
		Label(frameTop, text='    Track 2    ').grid(row=ROW_NAME, column=2)
		chk = Checkbutton(frameTop, text='')
		chk.select()
		chk.grid(row=ROW_CHECK, column=2)
		
		
		frameTop.grid_rowconfigure(ROW_VOL, weight=1)
		#~ frameTop.grid_columnconfigure(0, weight=1)
		
		#~ frameOne.pack(side=LEFT, expand=YES, fill=BOTH)

		#~ frameTwo = Frame(frameTop)
		#~ sc = Scale(frameTwo, from_=127, to=0, orient=VERTICAL)
		#~ sc.pack()
		#~ Label(frameTwo, text='hi there').pack()
		#~ frameTwo.pack(side=LEFT, expand=YES, fill=BOTH)
	
	def createMixedMidi(self, midiObject):
		pass
		
	

if __name__=='__main__':
	sys.path.append('..\\bmidilib')
	import bmidilib
	
	newmidi = bmidilib.BMidiFile()
	newmidi.open('..\\midis\\16keys.mid', 'rb')
	newmidi.read()
	newmidi.close()
	
	root = Tk()
	opts = {}
	
	app = BMixerWindow(root,newmidi, opts)
	
	
	root.mainloop()
	
	