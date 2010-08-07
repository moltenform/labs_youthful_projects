
def fx_get_effects():
	return [
		('Reverse',fx_reverse),
		('Octave up',fx_octave_up),
		('Octave down',fx_octave_down),
		('Change freq',fx_change_pitch),
		('Vibrato',fx_vibrato),
		('Flanger',fx_flange),
		'_',
		('Echo',fx_echo),
		('Reverb',fx_reverb),
		('Chorus',fx_chorus),
		('Alien voice',fx_alien),
	]

#These effects replace previous contents of the wave file

def fx_reverse(audiodata):
	audiodata.samples.reverse()

def fx_octave_up(audiodata):
	# Get every other sample
	audiodata.samples = audiodata.samples[0::2]

def fx_octave_down(audiodata):
	# average
	newlength = len(audiodata.samples)*2
	newsamples = [0] * newlength
	for i in xrange(newlength-1):
		if i%2 == 0:
			newsamples[i] = audiodata.samples[i/2]
		else:
			newsamples[i] = (audiodata.samples[i/2] + audiodata.samples[(i/2) + 1])*0.5
			
	# For the very last sample, we evidently have to repeat.
	newsamples[newlength-1] = newsamples[newlength-2]
	audiodata.clear_samples()
	audiodata.samples.extend( newsamples)

def _getSampleAnywhere(audiodata, fPosition):
	# Generalizes the idea of array index. Basically a floating point index into an array
	down = int(fPosition)
	up = down+1
	if (down+1>=len(audiodata.samples)):
		return audiodata.samples[-1]
	
	#~ print down, audiodata.samples[down]
	#~ print fPosition, audiodata.samples[down] * (up-fPosition) + audiodata.samples[up] * (fPosition-down)
	#~ print up, audiodata.samples[up]
	
	return audiodata.samples[down] * (up-fPosition) + audiodata.samples[up] * (fPosition-down)

def fx_change_pitch(audiodata, scaleFreq=0.25):
	import math

	
	newlength = int((len(audiodata.samples)/scaleFreq))
	newsamples = [0] * newlength
	fPosition = 0.0 #floating point
	for i in xrange(newlength-1):
		newsamples[i] = _getSampleAnywhere(audiodata, fPosition)
		fPosition += scaleFreq
		#~ print round(fPosition), audiodata.samples[int(round(fPosition))], fPosition, newsamples[i]
	audiodata.clear_samples()
	audiodata.samples.extend(newsamples)
	
def fx_vibrato(audiodata, freq=2., amnt=0.1):
	import math
	w = 2.*math.pi*freq / audiodata.nSampleRate
	
	newlength = len(audiodata.samples)
	newsamples = [0] * newlength
	fPosition = 0.0
	for i in xrange(newlength-1):
		newsamples[i] = _getSampleAnywhere(audiodata, fPosition)
		fPosition += 1.0 + amnt*math.sin(w*i)
	audiodata.clear_samples()
	audiodata.samples.extend(newsamples)


def fx_flange(audiodata, amount=0.999):
	oldsamples = audiodata.samples
	fx_change_pitch(audiodata, amount)
	
	oldaudio = audiodata.empty_copy()
	oldaudio.samples = oldsamples
	
	audiodata.addwave(oldaudio, 0.5)
	
def fx_echo(audiodata, delay=0.2):
	delayed = audiodata.empty_copy()
	delayed.add_silence(delay)
	delayed.samples.extend(audiodata.samples)
	
	audiodata.addwave(delayed, 0.5)

def fx_reverb(audiodata):
	fx_echo(audiodata, 0.2)
	fx_echo(audiodata, 0.3)
	fx_echo(audiodata, 0.4)
	fx_echo(audiodata, 0.5)
	
def fx_chorus(audiodata):
	# add delays with slight vibrato
	delayed = audiodata.empty_copy()
	delayed.add_silence(.05)
	delayed.samples.extend(audiodata.samples)
	fx_vibrato(delayed, freq=2., amnt=0.07)
	
	audiodata.addwave(delayed, 0.7)
	
	delayed2 = audiodata.empty_copy()
	delayed2.add_silence(.09)
	delayed2.samples.extend(audiodata.samples)
	fx_vibrato(delayed2, freq=2.01, amnt=0.049)
	
	audiodata.addwave(delayed2, 0.7)
	
def fx_alien(audiodata, freq = 300):
	import wave_synthesis
	
	sine = audiodata.empty_copy()
	wave_synthesis.generate_tone(sine, freq, (len(audiodata.samples) - 5)/audiodata.nSampleRate, 'sine', 0.5)
	
	#audiodata.modulatewave(sine)
	fx_modulatewave(audiodata, sine)

def fx_modulatewave(audiodata, other, weight=0.5):
	if audiodata.sampleWidth != other.sampleWidth or audiodata.nSampleRate != other.nSampleRate:
		print 'Different properties, cannot mix.'
		return None
	oweight = 1-weight
	mysamples = audiodata.samples
	othersamples = other.samples
		
	maxval = float(audiodata.ceil[1])
	if audiodata.midval==0:
		c=0
		for i in xrange(len(othersamples)):
			c+=1
			mysamples[i] = int( mysamples[i] * othersamples[i] / maxval)
	else:
		for i in xrange(len(othersamples)):
			mysamples[i] = int( (mysamples[i]-audiodata.midval) * (othersamples[i]-other.midval) / maxval) + audiodata.midval
	audiodata.samples = mysamples
	print 'done',c
	