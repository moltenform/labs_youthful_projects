

var g_trembackbufChannels=[]; 
function generateTremoloAudio(mainBuffer, audioState)
{
	var bAllOff = true
	if (!g_trembackbufChannels[0]) g_trembackbufChannels[0] = makeArray(mainBuffer.length) 
	if (!g_trembackbufChannels[1]) g_trembackbufChannels[1] = makeArray(mainBuffer.length) 
	
	for (var i=0; i<audioState.channels.length; i++)
	{
		var chn = audioState.channels[i]
		if (chn.wavetype=='off') continue
		bAllOff = false
		
		var table = {'sine': makeSin, 'square':makeSquare, 'saw':makeSaw, 'tri':makeTri, 'noise':makeNoise}
		var fn = table[chn.wavetype]
		if (!fn) { errmsg('Unknown wavetype' + chn.wavetype); fn = null}
		var frequencyHz = chn.freq_a
		fn(g_trembackbufChannels[0], frequencyHz, chn.vol)
		
		// do mods
		for (var j=0; j<chn.modifiers.length; j++)
		{
			var phaseShift = chn.modifiers[j].phase*2*Math.PI
			var modfreq = chn.modifiers[j].freq_a
			
			if (chn.modifiers[j].modtype=='off') continue;
			else if (chn.modifiers[j].modtype=='trem' || chn.modifiers[j].modtype=='trem10' )
			{
				if (chn.modifiers[j].modtype=='trem10') modfreq *= 10;
				operationTremolo(g_trembackbufChannels[0], modfreq,chn.modifiers[j].width, phaseShift)
			}
			else if (chn.modifiers[j].modtype=='vib' || chn.modifiers[j].modtype=='vib10' || chn.modifiers[j].modtype=='vibc' )
			{
				if (chn.modifiers[j].modtype=='vib10') modfreq *= 10;
				if (chn.modifiers[j].modtype=='vibc')
					operationVibratoCoo(g_trembackbufChannels[0], g_trembackbufChannels[1], modfreq,chn.modifiers[j].width,phaseShift)
				else
					operationVibrato(g_trembackbufChannels[0], g_trembackbufChannels[1], modfreq,chn.modifiers[j].width,phaseShift)
				// now swap the temp buffers. 
				var tmp = g_trembackbufChannels[1]; g_trembackbufChannels[1]=g_trembackbufChannels[0]; g_trembackbufChannels[0]=tmp
				
			}
			else
			{ errmsg('Unknown modtype'); throw(false); }
		}
		if (i==0)
		{
			//replace existing audio
			if (!g_fLayerAudio)
			{
				for (var j=0; j<mainBuffer.length; j++)
					mainBuffer[j] = /*instead of +=*/ g_trembackbufChannels[0][j]
			}
			else
			{
				for (var j=0; j<mainBuffer.length; j++)
					mainBuffer[j] = 0.7*mainBuffer[j] + g_trembackbufChannels[0][j]
			}
		}
		else
		{
			for (var j=0; j<mainBuffer.length; j++)
				mainBuffer[j] += g_trembackbufChannels[0][j]
		}
	}
	// if everything turned off, clear buffer
	if (bAllOff && !g_fLayerAudio)
		for (var j=0; j<mainBuffer.length; j++)
			mainBuffer[j] = 0.0
}

function operationTremolo(chdata, tremfreq, amp,phase)
{
	var tremoloFreqScale = 2.0 * Math.PI * tremfreq / 44100.0;
	amp /= 2.0;
	for (var i = 0; i < chdata.length; i++)
	{
	    var val = chdata[i] * amp*(1 + Math.sin(tremoloFreqScale * i + phase));
	    if (val > 1.0) val = 1.0;
	    else if (val < -1.0) val = -1.0;
	    chdata[i] = val;
	}
}

function operationTremoloRand(chdata, tremfreq, amp,phase)
{
	//~ var tremoloFreqScale = 2.0 * Math.PI * tremfreq / 44100.0;
	//~ amp /= 2.0;
	//~ for (var i = 0; i < chdata.length; i++)
	//~ {
	    //~ var val = chdata[i] * amp*(1 + Math.sin(tremoloFreqScale * i + phase));
	    //~ if (val > 1.0) val = 1.0;
	    //~ else if (val < -1.0) val = -1.0;
	    //~ chdata[i] = val;
	//~ }
}


function operationVibrato(chdata, outbuffer, vibratofreq, width,phase)
{
    // walk through the file at varying speeds
    var currentPosition = 0.0;
	if (chdata.length != outbuffer.length) errmsg('expect lengths same')
 var vibratoFreqScale = 2.0 * Math.PI * vibratofreq / 44100.0;
    for (var i = 0; i < outbuffer.length; i++)
    {
	outbuffer[i] = getInterpolatedValue(chdata, currentPosition);
	currentPosition += 1.0 + width * Math.sin(i * vibratoFreqScale + phase);
    }
}


function operationVibratoCoo(chdata, outbuffer, vibratofreq, width,phase)
{
	// walk through the file at varying speeds
	var currentPosition = 0.0;
	if (chdata.length != outbuffer.length) errmsg('expect lengths same')
	var vibratoFreqScale = 2.0 * Math.PI * vibratofreq / 44100.0;
	var prevv = 0
	for (var i = 0; i < outbuffer.length; i++)
	{
		var somesin = Math.sin(i * vibratoFreqScale) //take part of the period when slope positive and > 0
		if (somesin > 0.0 && somesin > prevv )
			outbuffer[i] = getInterpolatedValue(chdata, currentPosition);
		else 
			outbuffer[i] = 0.0
		prevv = somesin
		currentPosition += 1.0 + width * Math.sin(i * vibratoFreqScale + phase);
	}
}

function operationVibratoTri(chdata, outbuffer, vibratofreq, width,phase)
{
    // walk through the file at varying speeds
    var currentPosition = 0.0;
	if (chdata.length != outbuffer.length) errmsg('expect lengths same')
 var k = vibratofreq / 44100.0;
	width *=2
	phase /= Math.PI*2;
    for (var i = 0; i < outbuffer.length; i++)
    {
	outbuffer[i] = getInterpolatedValue(chdata, currentPosition);
	    var inp = (k * i + phase)%1.0;
	    if (inp > 0.5) inp=1-inp;
	currentPosition += 1.0 + width * (inp-0.25);
    }
}


function getInterpolatedValue(sampleData, sampleIndex)
{
    if (sampleIndex > sampleData.length - 1) sampleIndex = sampleData.length - 1;
    else if (sampleIndex < 0 + 1) sampleIndex = 0;

    var proportion = sampleIndex - Math.floor(sampleIndex);
    var v1 = sampleData[Math.floor(sampleIndex)];
	var v2 = sampleData[Math.ceil(sampleIndex)];
    return v2 * proportion + v1 * (1 - proportion);
}


function makeSin(soundData, frequency, volume) {
var k = 2* Math.PI * frequency / g_sampleRate;
for (var i=0, size=soundData.length; i<size; i++) {
  soundData[i] = Math.sin(k * i) * volume;
}        
}
function makeSquare(soundData, frequency, volume) {
var k = frequency / g_sampleRate;
for (var i=0, size=soundData.length; i<size; i++) {
	var inp = (k * i)%1.0;
  soundData[i] = (inp > 0.5) ? volume : -volume;
}
}
function makeSaw(soundData, frequency, volume) {
var k = frequency / g_sampleRate;
for (var i=0, size=soundData.length; i<size; i++) {
	var inp = (k * i)%1.0;
  soundData[i] = (inp-0.5)*volume;
}
}
function makeNoise(soundData, frequency, volume) {
var pos = 0.0;
for (var i=0, size=soundData.length; i<size; i++) {
	pos += (Math.random()-0.5)*0.05*volume;
	if (pos > 1.0) pos=1.0;
	if (pos < -1.0) pos = -1.0;
	soundData[i] = pos;
}
}
function makeTri(soundData, frequency, volume) {
var k = frequency / g_sampleRate;
	volume*=2;
for (var i=0, size=soundData.length; i<size; i++) {
    var inp = (k * i)%1.0;
    if (inp > 0.5) inp=1-inp;
  soundData[i] = (inp-0.25)*volume;
}
}

