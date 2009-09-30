import exceptions
import os
import midirender_util

import sys
sys.path.append('..')
from bmidilib import bmidiplay

import tempfile
from os import sep as os_sep
tempcfgfilename = tempfile.gettempdir() + os_sep + 'tmpcg.cfg'


class RenderTimidityMidiPlayer(bmidiplay.TimidityMidiPlayer):
	extraParameters = None
	cfgFile = None
	def setParameters(self, arParameters):
		self.extraParameters = arParameters
	def setConfiguration(self, strCfg):
		filename = tempcfgfilename
		f=open(filename, 'w')
		f.write(strCfg)
		f.close()
		self.cfgFile = filename
	
	def _additionalTimidityArgs(self):
		ret = []
		if self.cfgFile != None:
			ret.append('-c')
			ret.append(self.cfgFile)
		
		if self.extraParameters != None:
			ret.extend(self.extraParameters)
		
		return ret
	

def isTimidityInstalled():
	import subprocess
	try:
		process = subprocess.Popen(['timidity'], stdout=subprocess.PIPE) #will just display version info, etc. if suceeds
	except EnvironmentError, e:
		return False
	return True
	

if __name__=='__main__':
	from Tkinter import *
	global mmplayer
	mmplayer = bmidiplay.TimidityMidiPlayer()
	# mmplayerwin.timiditypath = 'timidity\\timidity.exe'
	
	def start(top):
		def start():
			global mmplayer
			mmplayer.playSynchronous('..\\midis\\tempotest.mid')
		
		def startsync():
			global mmplayer
			mmplayer.playAsync('..\\midis\\16keys.mid')
		
		def stop():
			global mmplayer
			mmplayer.signalStop()
		
		Label(text='Default, Without cfg file').pack()
		Button(text='startasync', command=startsync).pack()
		Button(text='startsync', command=start).pack()
		Button(text='stop', command=stop).pack()
		Label(text='With cfg file').pack()

	root = Tk()
	start(root)
	root.mainloop()


#tested with timidity 2.13.
#~ if __name__=='__main__':
	#~ verbose = True
	#~ def callback():
		#~ print 'done'
	
	#~ strConfig = '\n'
	#~ strConfig+= 'soundfont "' + r'C:\Projects\midi\!midi_to_wave\soundfonts_good\sf2_other\vintage_dreams_waves_v2.sf2' + '"\n'
	
	#~ dir = 'timidity'
	#~ runTimidity(strConfig, 'out.mid', dir, fnCallback=callback)
	