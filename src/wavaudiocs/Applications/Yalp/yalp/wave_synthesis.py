from wave_file import *

# This generates a tone
def generate_tone(audiodata, fFreq, fSeconds, waveType='sine', amplitude=0.5): #amplitude between 0 and 1
	"""add to the contents of this wave file with a tone, fSeconds long"""
	nSamples = int(fSeconds * audiodata.nSampleRate)
	amplitude= amplitude * audiodata.maxval * 0.5
	midval = audiodata.midval
	
	period = audiodata.nSampleRate / float(fFreq)
	
	# If I want to optimize this, I should set up generators, yield statement!
	if waveType=='sine':
		w = fFreq*2*math.pi / audiodata.nSampleRate
		audiodata.samples.extend(  (amplitude * math.sin(w*x))+midval for x in xrange(nSamples))
	elif waveType=='square':
		halfperiod = period/2.
		audiodata.samples.extend(  (midval-amplitude if (x % period > halfperiod) else midval+amplitude)  for x in xrange(nSamples))
	elif waveType=='sawtooth':
		slope = 2*amplitude/period
		start = audiodata.midval - amplitude
		audiodata.samples.extend(( slope*(x%period)+start  for x in xrange(nSamples) ))
	elif waveType=='triangle':
		halfperiod = period/2.
		slope = 4*amplitude/period
		start = audiodata.midval - amplitude
		stop = audiodata.midval + amplitude + amplitude*2
		audiodata.samples.extend(((slope*(x%period)+start  if (x % period < halfperiod) else -slope*(x%period)+stop)  for x in xrange(nSamples) ))
	elif waveType=='circle':
		# circle is sqrt(1-x^2). I invented this. It doesn't sound that great.
		halfamp = amplitude/2.
		qtrperiod = period/4.
		halfperiod =  period/2.
		audiodata.samples.extend((amplitude*math.sqrt(1-((x%period)/qtrperiod-1)**2.)+midval  if (x % period < halfperiod) else  -amplitude*math.sqrt(1-(((x%period)-halfperiod)/qtrperiod-1)**2.)+midval   for x in xrange(nSamples) ))
	else:
		print 'Unknown wave type.'


# These effects append to the previous contents of the wave file.

def get_instruments():
	return ['sine','square','sawtooth','triangle','circle','_',
	'whitenoise', 'rednoise', 'squarephase','_',
	'smooth','organ', '_',
	
	#Experimental
	'chorusphase','detunedorgan','harmonics',
	]
	
def synthesize(audiodata, strInstrument, freq, length, amplitude=0.5):
	if strInstrument in ['sine','square','sawtooth','triangle','circle']:
		generate_tone(audiodata, freq, length, strInstrument,amplitude)
	else:
		nsamples = audiodata.nSampleRate * length
		
		if strInstrument == 'whitenoise':
			import random
			audiodata.samples.extend(audiodata.midval + random.random() * amplitude * .5 * audiodata.maxval * .5 for i in xrange(nsamples))
		elif strInstrument == 'rednoise':
			import random
			startpos=audiodata.midval
			opts = [-1,1]
			val = 0
			res = [0] * int(nsamples)
			for i in xrange(nsamples):
				val += random.choice(opts)
				if val > audiodata.ceil[1]: val = audiodata.ceil[1]
				if val < audiodata.ceil[0]: val = audiodata.ceil[0]
				res[i] = (val)
			audiodata.samples.extend(res)
		elif strInstrument == 'squarephase':
			synth_squarephase(audiodata, freq, nsamples, amplitude)
		elif strInstrument == 'smooth':
			# originally 329,331
			manualsynth(audiodata, nsamples, [[freq, amplitude*0.6], [freq*1.0006079, amplitude*0.4]])
		elif strInstrument == 'organ':
			#~ manualsynth(audiodata, nsamples, [[freq*(i+1),amplitude*(0.3**i) ] for i in range(7) ])#, [[(freq+1.00606)**i,amplitude*(0.1**i) ] for i in range(8) ])
			manualsynth(audiodata, nsamples, _extend([[freq, .5*0.9**(8-1)]], [[freq*(i+1),.5*0.1*0.9**((8-1)-i)]  for i in range(1,8)],[[freq*1.00606, .5*0.9**(8-1)]], [[freq*1.00606*(i+1),.5*0.1*0.9**((8-1)-i)]  for i in range(1,8)]))
		elif strInstrument == 'harmonics':
			manualsynth(audiodata, nsamples, [[freq, amplitude*0.6], [freq*2, amplitude*0.2], [freq*3, amplitude*0.5]])
		#experimental
		elif strInstrument == 'chorusphase':
			synth_chorusph(audiodata, freq, nsamples, amplitude)
		elif strInstrument == 'detunedorgan':
			synth_detunedorgan(audiodata, freq, nsamples, amplitude)

def _extend(a, a2=None, a3=None, a4=None):
	if a2!=None: a.extend(a2)
	if a3!=None: a.extend(a3)
	if a4!=None: a.extend(a4)
	return a

def manualsynth(audiodata, nSamples, arData):
	import math
	#arData in the form [(fFreq, amplitude), ...]
	
	#set frequencies, overwriting old frequency
	for wave in arData:
		#~ print wave[0], wave[1]
		wave[0] =  wave[0]*2*math.pi / audiodata.nSampleRate
		
	midval = audiodata.midval
	amp = audiodata.maxval * .5
	audiodata.samples.extend(  
		
		sum(( wave[1]*amp*math.sin(wave[0]*i)  for wave in arData))+midval
		for i in xrange(int(nSamples))
		)
	
	
def synth_squarephase(audiodata,fFreq, nSamples, amplitude=0.5):
	amplitude= amplitude * audiodata.maxval * 0.5
	midval = audiodata.midval
	period = audiodata.nSampleRate / float(fFreq)
	audiodata.samples.extend(  [ (midval-amplitude if (x % period > (x/float(nSamples) * period)) else midval+amplitude)  for x in xrange(nSamples)] )

def synth_chorusph(audiodata, freq, nSamples, amplitude=0.5):
	fSeconds = nSamples / float(audiodata.nSampleRate)
	wc, we = audiodata.empty_copy(), audiodata.empty_copy()
	
	synthesize(wc,'squarephase', freq, fSeconds, amplitude)
	synthesize(we,'squarephase', freq *1.0006079, fSeconds, amplitude)
	
	wc.addwave(we, 0.4)
	audiodata.append(wc)

def synth_detunedorgan(audiodata, freq, nSamples, amplitude=0.5):
	fSeconds = nSamples / float(audiodata.nSampleRate)
	wc, we, wf = audiodata.empty_copy(), audiodata.empty_copy(), audiodata.empty_copy()
	
	generate_tone(wc, freq ,fSeconds,'circle',amplitude)
	generate_tone(we, freq*0.9939577,fSeconds,'circle',amplitude)
	generate_tone(wf, freq*0.98489425,fSeconds,'circle',amplitude)
	wc.addwave(we, 0.33)
	wc.addwave(wf, 0.33)
	audiodata.append(wc)
	

def chorusharmonics(fund, nharm=8.):
	wcomb = None
	for h in range(1,nharm):
		wnew = WaveFile(nBits=8, nSampleRate=22050)
		freq = fund * h
		generate_tone(wnew, freq,3.0,'sine', 0.5)
		
		if wcomb==None:
			wcomb = wnew
		else:
			wcomb.addwave(wnew,0.1 )
	return wcomb


