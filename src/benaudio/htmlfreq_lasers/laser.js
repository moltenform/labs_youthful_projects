
var currentFreqApp = 400;
var counter = 0 
var g_fPlaying = false;

function requestSoundData(soundData)
{
	if (!g_fPlaying)
		return;

	var scale = 2* Math.PI / g_sampleRate;
	var factor = g_audioBackendState.sparkliness
	var hm = Math.floor(g_audioBackendState.harmonic)
	if (g_audioBackendState.shape_enum=='sine')
	{
		scale *= 2 //double frequency because hard to hear sine at low freqs
		for (var i=0, size=soundData.length; i<size; i++)
		{
			currentFreqApp  += (g_audioBackendState.frequency - currentFreqApp)*factor;
			counter++;
			
			soundData[i] = (Math.sin((scale) *currentFreqApp*counter));
			soundData[i] += (Math.sin((scale/hm) *currentFreqApp*counter));
			soundData[i] *= 0.01;
		}
	}
	else if (g_audioBackendState.shape_enum=='square')
	{
		for (var i=0, size=soundData.length; i<size; i++)
		{
			currentFreqApp  += (g_audioBackendState.frequency - currentFreqApp)*factor;
			counter++;
			soundData[i] = (Math.sin((scale) *currentFreqApp*counter)) > 0 ? .5 : -.5;
			soundData[i] += (Math.sin((scale/hm) *currentFreqApp*counter)) > 0 ? .5: -.5;
			soundData[i] *= 0.01;
		}
	}
	else if (g_audioBackendState.shape_enum=='saw')
	{
		scale = 1.0 / g_sampleRate;
		for (var i=0, size=soundData.length; i<size; i++)
		{
			currentFreqApp  += (g_audioBackendState.frequency - currentFreqApp)*factor;
			counter++;
			var inp = (scale * currentFreqApp * counter)%1.0;
			soundData[i] = (inp-0.5);
			inp = (scale/hm * currentFreqApp * counter)%1.0;
			soundData[i] += (inp-0.5);
			soundData[i] *= 0.005;
		}
	}
	else if (g_audioBackendState.shape_enum=='tri')
	{
		scale = 1.0 / g_sampleRate;
		scale *= 2 //double frequency because hard to hear sine at low freqs
		for (var i=0, size=soundData.length; i<size; i++)
		{
			currentFreqApp  += (g_audioBackendState.frequency - currentFreqApp)*factor;
			counter++;
			var inp = (scale * currentFreqApp * counter)%1.0;
			if (inp > 0.5) inp = 1 - inp;
			soundData[i] = inp - 0.25;
			
			inp = (scale/hm * currentFreqApp * counter)%1.0;
			if (inp > 0.5) inp = 1 - inp;
			soundData[i] += inp - 0.25;
			soundData[i] *= 0.005;
		}
	}

	
	if (counter > 1e7) counter = 0;
}

function onplay() {
currentSoundSample = 0;
g_fPlaying= true;
}

function onpause() {
g_fPlaying = false;
}

