import exceptions
import os
verbose = False

#tested with timidity 2.13.

import midirender_util

class RunTimidityException(exceptions.Exception): pass



def runTimidity(strCfgText, strMidiFile, timdir,runInThread=False, fnCallback=None):
	timfile =  os.path.join(timdir, 'timidity.exe')
	if not os.path.exists(timfile):
		raise RunTimidityException("Couldn't find timidity.exe.")
	if not os.path.exists(strMidiFile):
		raise RunTimidityException("Couldn't find mid file you were trying to play.")
	
	#write config file
	cfgFile = os.path.join(timdir, 'timidity.cfg') 
	#At first should be in this directory, current directory. but apparently timidity only looks where it is actually located?
	f = open(cfgFile,'w')
	f.write(strCfgText)
	f.close()
	
	#run Timidity. should be fine to just close it. run in different thread?
	dothis=timfile + ' "' + strMidiFile.replace('"','\\"') + '"'
	if verbose: print dothis
	
	if runInThread: #run in a thread, or synchronously?
		midirender_util.makeThread(lambda: actualRun(dothis, fnCallback))
	else:
		actualRun(dothis, fnCallback)

def actualRun(commandLine, fnCallback):
	os.system(commandLine)
	if fnCallback:
		fnCallback()


if __name__=='__main__':
	#~ verbose = True
	def callback():
		print 'done'
	
	strConfig = '\n'
	strConfig+= 'soundfont "' + r'C:\Projects\midi\!midi_to_wave\soundfonts_good\sf2_other\vintage_dreams_waves_v2.sf2' + '"\n'
	
	dir = 'timidity'
	runTimidity(strConfig, 'out.mid', dir, fnCallback=callback)
	
	
	
