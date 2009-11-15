
import sys
import time
from os import sep as os_sep

import exceptions
class PlayMidiException(exceptions.Exception): pass

from ctypes import windll, c_buffer, c_void_p, c_int, byref

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

	def rawNoteOn(self, pitch,  channel=1, v=60):
		midimsg = 0x90 + ((pitch) * 0x100) + (v * 0x10000) + channel
		mm = c_int(midimsg)
		rc = self.winmm.midiOutShortMsg (self.hmidi, mm)
		if rc!=0:
			raise PlayMidiException( 'Error opening device, '+self.midiOutShortErrorCodes.get(rc,'Unknown error.'))
			
	def rawNoteOff(self, pitch,  channel=1):
		midimsg = 0x80 + ((pitch) * 0x100) + channel
		mm = c_int(midimsg)
		rc = self.winmm.midiOutShortMsg (self.hmidi, mm)
		if rc!=0:
			raise PlayMidiException( 'Error opening device, '+self.midiOutShortErrorCodes.get(rc,'Unknown error.'))
			
	
	
	

			
