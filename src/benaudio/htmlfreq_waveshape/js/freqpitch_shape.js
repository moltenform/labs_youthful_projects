// htmlfreq, Ben Fisher, 2011


var frequency = 0, currentSoundSample=0;
var sampleRate = 44100;
var g_fdirty = true;
var g_mybuffer = null;
var prebufferSize = sampleRate / 2; // buffer 500ms

//based off of example at https://wiki.mozilla.org/Audio_Data_API
 function AudioDataDestination(sampleRate, readFn)
 {
	// Initialize the audio output.
	var audio = new Audio();
	 if (!audio.mozSetup)
	{
		alert('This browser does not appear to support realtime audio data. Please try with Firefox 4 or greater.')
		return;
	}
	audio.mozSetup(1, sampleRate);

	var currentWritePosition = 0;
	
	var tail = null, tailPosition;

	// The function called with regular interval to populate 
	// the audio output buffer.
	setInterval(function() {
	  var written;
	  // Check if some data was not written in previous attempts.
	  if(tail) {
	    written = audio.mozWriteAudio(tail.subarray(tailPosition));
	    currentWritePosition += written;
	    tailPosition += written;
	    if(tailPosition < tail.length) {
	      // Not all the data was written, saving the tail...
	      return; // ... and exit the function.
	    }
	    tail = null;
	  }

	  // Check if we need add some data to the audio output.
	  var currentPosition = audio.mozCurrentSampleOffset();
	  var available = currentPosition + prebufferSize - currentWritePosition;
	  if(available > 0) {
	    // Request some sound data from the callback function.
	    var soundData = new Float32Array(available);
	    readFn(soundData);

	    // Writting the data.
	    written = audio.mozWriteAudio(soundData);
	    if(written < soundData.length) {
	      // Not all the data was written, saving the tail.
	      tail = soundData;
	      tailPosition = written;
	    }
	    currentWritePosition += written;
	  }
	}, 100);
}

// optimization: make my own buffer. well, let's not do that.

getInterpolatedValue = function(/* double[] */ sampleData, /* double */ sampleIndex)
{
if (sampleIndex > sampleData.length - 1) sampleIndex = sampleData.length - 1;
else if (sampleIndex < 0 + 1) sampleIndex = 0;

var proportion = sampleIndex - Math.floor(sampleIndex);
var v1 = sampleData[Math.floor(sampleIndex)];
var v2 = sampleData[Math.ceil(sampleIndex)];
return v2 * proportion + v1 * (1 - proportion);
}

var g_fPlaying = false;
var phase=0.0;
var g_arrayWaveform = null;
var cind = 1;
var currentcutoff = 0.0;
function requestSoundData(soundData) {
	if (!g_fPlaying)
		return;
	if (g_fdirty) {
		g_arrayWaveform = getArr()
		//~ if (g_arrayWaveform.length==0) return;
		g_fdirty = false;
	}
	
	var currentFreq = g_arrinfoPitch[1]; //see other prog for smooth scaling.
	var k = currentFreq / sampleRate;
	var target = g_arrinfoCutoff[1] ;
	
	for (var i=0, size=soundData.length; i<size; i++) {
		phase += k;
		if (phase > 1.0) { phase -= 1.0; cind = 1; }
		if (phase < currentcutoff)
		{
			soundData[i] = 0.0;
		}
		else
		{
			var prev = g_arrayWaveform[cind-1][2];
			var next = g_arrayWaveform[cind][2];
			var ratio = (phase - g_arrayWaveform[cind-1][1]) / (g_arrayWaveform[cind][1] - g_arrayWaveform[cind-1][1]);
			
			soundData[i] = ratio*(next-prev) + prev;
		}
		if (phase > g_arrayWaveform[cind][1])
		{
			cind++;
		}
		currentcutoff  += (target - currentcutoff)*0.0001;
	}
}

function onplay() {
currentSoundSample = 0;
phase=0.0;
g_fPlaying= true;
}

function onpause() {
g_fPlaying = false;
}

function printdbg()
{
	//~ var af = getArr(); var s = '';
	//~ for (var i=0; i <af.length; i++)
	//~ {
		//~ s += '\n' + af[i][1].toString() + ',' + af[i][2].toString()
	//~ }
	//~ document.getElementById('dbgout').value = s;
	//~ return;
	
	prevphase = phase;
	var art = new Float32Array(5);
	var s = ''; ind = 0;
	document.getElementById('dbgout').value = '';
	for (var pp=0; pp<920; pp++)
	{
		requestSoundData(art);
		
		for(var i=0; i <art.length; i++)
		{
			s += '\n' + (++ind).toString() + ',' + art[i].toString()
		}
		if (phase<prevphase)
			break;
		prevphase = phase;
	}
	document.getElementById('dbgout').value = s;
}
