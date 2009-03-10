import exceptions
import os
verbose = False

#tested with timidity 2.13.

class RunTimidityException(exceptions.Exception): pass



def runTimidity(strCfgText, strMidiFile, timdir):
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
	print dothis
	os.system(dothis )
	

if __name__=='__main__':
	#~ verbose = True
	
	strConfig = '\n'
	strConfig+= 'soundfont "' + r'C:\Projects\midi\!midi_to_wave\soundfonts_good\sf2_other\vintage_dreams_waves_v2.sf2' + '"\n'
	
	dir = 'timidity'
	runTimidity(strConfig, 'out.mid', dir)
	
	
	
