import wave
import math

import cStringIO

import array

# Ben Fisher
# http://www.borg.com/~jglatt/tech/wave.htm

class WaveFile():
	# Either init from a wave file, or create from scratch.
	def __init__(self, strFilename=None, nBits=16, nSampleRate = 22050, nChannels=1):
		
		if strFilename!=None:
			f = wave.open(strFilename, 'rb')
			if f.getcomptype()!='NONE': print 'Compressed wave files not supported.' 
			nBits = 8 * f.getsampwidth()
			nSampleRate = f.getframerate()
			nChannels = f.getnchannels()
		
		if nBits==16:
			self.maxval = 65535 		#32767 to -32767
			self.midval = 0
			self.sampleWidth = 2
			self.samples = array.array('h') #Signed short
			self.ceil = (-32767,32767)
		elif nBits==8:
			self.maxval = 255
			self.midval = 128
			self.sampleWidth = 1
			self.samples = array.array('B') #Unsigned char
			self.ceil = (0,255)
		else:
			print 'Invalid bitrate. Must be 8-bit or 16-bit.'
			return None
		
		if nSampleRate==44100: self.nSampleRate = 44100.
		elif nSampleRate==22050: self.nSampleRate = 22050.
		else:
			print 'Invalid sample rate. Must be 44100 or 22050.'
			return None
		
		if nChannels==1: self.nChannels = 1.
		else:
			if nChannels==2: print 'Stereo not supported at this time...'
			print 'Invalid channels. Must be 1 or 2.'
			return None
		
		# Load data
		if strFilename!=None:
			self.samples.fromstring(f.readframes(-1))
			f.close()
			
	def clear_samples(self):
		if self.sampleWidth==2:
			self.samples = array.array('h') #Signed short
		elif self.sampleWidth==1:
			self.samples = array.array('B') #Unsigned char
	def empty_copy(self):
		# Create a copy with same properties, but no samples
		return WaveFile(nBits=self.sampleWidth*8, nSampleRate=self.nSampleRate, nChannels=self.nChannels)
		
	def addwave(self, other, weight=0.5):
		if self.sampleWidth != other.sampleWidth or self.nSampleRate != other.nSampleRate:
			print 'Different properties, cannot mix.'
			return None
		#~ # transform wave file in place.
		# Naive addition:
		oweight = 1-weight
		othersamples = other.samples
		mysamples = self.samples
		if len(othersamples)>len(mysamples):
			othersamples, mysamples = mysamples, othersamples
		if self.midval==0:
			for i in xrange(len(othersamples)):
				mysamples[i] = int( mysamples[i]*oweight + othersamples[i]*weight )
		else:
			for i in xrange(len(othersamples)):
				mysamples[i] = int( (mysamples[i]-self.midval)*oweight + (othersamples[i]-self.midval)*weight )+self.midval
		self.samples = mysamples
	
	def append(self, other):
		if self.sampleWidth != other.sampleWidth or self.nSampleRate != other.nSampleRate:
			print 'Different properties, cannot append.'
			return None
		self.samples.extend(other.samples)
		
	def truncate(self, nEndFrame):
		self.samples = self.samples[ 0 : nEndFrame]
	
	def multiply(self, amount):
		for i in xrange(len(self.samples)):
			
			val = int((self.samples[i]-self.midval)* amount) + self.midval
			if val > self.ceil[1]: val = self.ceil[1]
			elif val < self.ceil[0]: val = self.ceil[0]
			self.samples[i] = val
	

		
	def save_wave(self,strFilename):
		f = open(strFilename, 'wb')
		f.write( self.wavedata() )
		f.close()
	
	
	def wavedata(self):
		cstring = cStringIO.StringIO()
		f = wave.open(cstring, "wb")
		f.setnchannels(self.nChannels)
		f.setframerate(int(self.nSampleRate))
		f.setsampwidth(self.sampleWidth)
		
		# The samples are already in an ideal format - an array of machine values, so a sequence of bytes.
		f.writeframes(self.samples.tostring())
		
		result = cstring.getvalue()
		f.close()
		return result
		
	def peekdata(self):
		for i in range(80):
			print self.samples[i]
	
	def get_level(self, start, stop):
		total = 0.
		for i in xrange(start, stop):
			total += math.fabs(self.midval - self.samples[i])/4096.
		return (total * 4096. / float(stop-start))/float(self.ceil[1])
		
	def add_silence(self, duration):
		framedur = int(duration * self.nSampleRate)
		self.samples.extend([self.midval for i in xrange(framedur)])
	



#~ wa = chorusharmonics(330)
#~ wa.play_memory()

#~ wc = chorusharmonics(330)
#~ w2 = chorusharmonics(332)

#~ wc.addwave(w2,0.5)
#~ wc.play_memory()

#~ wc.save_wave('c:\\harmonics!2.wav')