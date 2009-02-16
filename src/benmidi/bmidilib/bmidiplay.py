
'''
bmidiplay.py, Ben Fisher 2009

Playing midi files from within Python.
Now that ctypes is evidently included in Python, makes it a lot easier.

'''

import time
from ctypes import windll, c_buffer, c_void_p, c_int, byref


import exceptions
class PlayMidiException(exceptions.Exception): pass


#play using Windows mci. Currently, can only play one file at a time.
#note that this can be used to play .wav files and even .mp3 files.
#based on http://mail.python.org/pipermail/python-win32/2008-August/008059.html

#currently, everything is synchronous. It wouldn't be hard to have an asynchronous version- it would just return instead of time.sleep(),
#and the mci stop command would be 'close cursong' which could be called later.

def playMidiObject(objMidiFile):
	#save it to a temporary file
	import tempfile
	tempfilename = tempfile.gettempdir() + '\\tmpbmidiplay.mid'
	
	try: f=open(tempfilename,'wb')
	except: raise PlayMidiException('Could not create temporary file for midi playback.')
	f.close()
	objMidiFile.open(tempfilename, 'wb')
	objMidiFile.write()
	objMidiFile.close()
	#~ print tempfilename
	
	time.sleep(1.0)
	playMidiFile(tempfilename)
	time.sleep(1.0)
	
	#remove the temporary file
	#~ import os
	#~ try: os.unlink(tempfilename)
	#~ except: pass #raise PlayMidiException('Could not remove temporary file for midi playback.')
	
def playMidiFile(strFilename): #synchronous, waits for the song to be done. so you can't really use this for a long song unless you want to wait.
	mci=Mci()
	mci.send('open "%s" alias cursong'%strFilename) #does strFilename need escaping?
	mci.send('set cursong time format milliseconds')
	buflength = mci.send('status cursong length ')
	#~ print 'Duration : ',buf,' millisecondes'
	mci.send('play cursong from 0 to '+str(buflength))
	time.sleep( (int(buflength)/1000.0) + 1)
	mci.send('close cursong')


class Mci():
	def __init__(self):
		self.fnMciSendString = windll.winmm.mciSendStringA
		self.fnMciGetErrorString = windll.winmm.mciGetErrorStringA

	def send(self,command):
		buffer = c_buffer(255)
		errorcode = self.fnMciSendString(str(command),buffer,254,0)
		if errorcode:
			raise PlayMidiException('MCIError:' + str(self.get_error(errorcode)) + ':'+ str(txt)+':'+str(buf))
			
		return buffer.value

	def get_error(self,error):
		error = int(error)
		buffer = c_buffer(255)
		self.fnMciGetErrorString(error,buffer,254)
		return buffer.value


#experimental real-time playing. 
#This works, but I'm not sure of how robust it is (and I might not be doing it in the best way), so maybe I'll use mci for now.
#so, consider this experimental for now.
class RealTimePlayer():
	#References:
	#http://www.sabren.net/rants/2000/01/20000129a.php3  (uses out-of-date libraries)
	#http://msdn.microsoft.com/en-us/library/ms711632.aspx
	
	def __init__(self):
		self.midiOutOpenErrorCodes= {
			(64+4) : 'MIDIERR_NODEVICE 	No MIDI port was found. This error occurs only when the mapper is opened.',
			(0+4): 'MMSYSERR_ALLOCATED 	The specified resource is already allocated.',
			(0+2): 'MMSYSERR_BADDEVICEID 	The specified device identifier is out of range.',
			(0+11): 'MMSYSERR_INVALPARAM 	The specified pointer or structure is invalid.',
			(0+7): 'MMSYSERR_NOMEM 	The system is unable to allocate or lock memory.', }
		self.midiOutShortErrorCodes={
			(64+6):'MIDIERR_BADOPENMODE 	The application sent a message without a status byte to a stream handle.',
			(64+3):'MIDIERR_NOTREADY 	The hardware is busy with other data.',
			(0+5):'MMSYSERR_INVALHANDLE 	The specified device handle is invalid.',}
		self.winmm = windll.winmm
		
	def countDevices(self):
		return self.winmm.midiOutGetNumDevs()
	def openDevice(self, deviceNumber=-1): #device -1 refers to the default set in midi mapper, a good choice
		#it took me some experimentation to get this to work...
		self.hmidi =  c_void_p()
		rc = self.winmm.midiOutOpen(byref(self.hmidi), deviceNumber, 0, 0, 0)
		if rc!=0:
			raise PlayMidiException( 'Error opening device, '+self.midiOutOpenErrorCodes.get(rc,'Unknown error.'))
	def closeDevice(self):
		rc = self.winmm.midiOutClose(self.hmidi)
		if rc!=0:
			raise PlayMidiException('Error closing device')
	def sendNote(self, pitch, duration=1.0, channel=1, volume=60): #duration in seconds
		midimsg = 0x90 + ((pitch) * 0x100) + (volume * 0x10000) + channel
		mm = c_int(midimsg)
		rc = self.winmm.midiOutShortMsg (self.hmidi, mm)
		if rc!=0:
			raise PlayMidiException( 'Error opening device, '+self.midiOutShortErrorCodes.get(rc,'Unknown error.'))
		
		time.sleep(duration)
		
		# turn it off
		midimsg = 0x80 + ((pitch) * 0x100) + channel
		mm = c_int(midimsg)
		rc = self.winmm.midiOutShortMsg (self.hmidi, mm)
		if rc!=0:
			raise PlayMidiException( 'Error opening device, '+self.midiOutShortErrorCodes.get(rc,'Unknown error.'))
			
		
	

if __name__=='__main__':
	pass
	#~ playMidiFile(r'C:\pydev\mainsvn\benmidi\midis\16keys.mid')
	#~ playMidiFile(r'C:\docume~1\bfisher\locals~1\temp\tmpbmidiplay.mid')
	
	#~ if True:
		#~ import bmidilib
		#~ mfile = bmidilib.BMidiFile()
		#~ mfile.open(r'C:\Projects\midi\midis\testbend.mid', 'rb')
		#~ mfile.read()
		#~ mfile.close()
		
		#~ playMidiObject(mfile)
		
	#~ else:
		#~ pl=RealTimePlayer()
		#~ print pl.countDevices()
		#~ pl.openDevice()
		#~ pl.sendNote(60,0.5)
		#~ pl.sendNote(48,0.5)
		#~ pl.sendNote(48,0.5)
		#~ pl.closeDevice()
		
