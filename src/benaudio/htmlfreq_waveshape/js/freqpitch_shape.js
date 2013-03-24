// htmlfreq, Ben Fisher, 2011


var frequency = 0, currentSoundSample=0;
var sampleRate = 44100;
var g_fdirty = true;
var g_mybuffer = null;
var prebufferSize = sampleRate / 2; // buffer 500ms


// optimization: make my own buffer. well, let's not do that.


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
			var denom = (g_arrayWaveform[cind][1] - g_arrayWaveform[cind-1][1]);
			var ratio = (phase - g_arrayWaveform[cind-1][1]) / (denom==0 ? 0.0001:denom);
			
			soundData[i] = ratio*(next-prev) + prev;
		}
		if (phase > g_arrayWaveform[cind][1])
		{
			cind++;
		}
		currentcutoff  += (target - currentcutoff)*0.0001;
	}
}

var g_startedAudiolet=false;
function on_btnplay() {
	if (!g_startedAudiolet) {
		try {
			var audiolet = new Audiolet();
			var wrapper = new WrapperNode(audiolet, requestSoundData);
			wrapper.connect(audiolet.output);
		} catch (e1) {
			alert('Failed to play audio. I\'d try a recent version of Firefox or Chrome and see if that works.');
			// continue and attempt to play audio anyways, just in case it does happen to work
		}
		g_startedAudiolet = true;
	}
	
	currentSoundSample = 0;
	phase=0.0;
	g_fPlaying= true;
}

function on_btnpause() {
g_fPlaying = false;
}

