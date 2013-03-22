
// possible future features: reverse audio. autorotate. 

function CBeatTop()
{
	this.divisions = 64
	this.tempo = 160/24
	this.channels = [];
	for (var i=0; i<4; i++) this.channels.push(new CBeatChannel())
	changeDivisionNumber(this, this.divisions) //will make sure either 0 or 1 for each
}
function CBeatChannel()
{
	this.samplename = 'off'; // ['off', 'kick','clap', 'hihat','snare']
	this.samplename_f = 0.0
	this.vol = 0.5; //volume
	
	this.rotation = 0.0 // from 0.0 to 1.0
	// this.autoRotation = 0.0
	this.arrHits = []; //use changeDivisionNumber to set this //array of hits from 
}

function setuprequestSoundChannelState(cbeattop)
{
	for (var i=0; i<cbeattop.channels.length;i++)
	{
		if (cbeattop.channels[i].samplename=='off') g_requestSoundChannelState[i].audio = getNone
		else
		{
			var stb = g_mediaTable[cbeattop.channels[i].samplename]
			if (stb===undefined) {errmsg('invalid sample name '+cbeattop.channels[i].samplename); return}
			g_requestSoundChannelState[i].audio = stb
		}
	}
}
// there are 4 beats. 60bpm : 1 measure = 4s. 80bpm 60/60
function createAudioMap(cbeattop)
{
	var lengthOfEntireMeasureInSeconds = 1.0/((cbeattop.tempo)/60);
	var flengthOfEntireMeasureInSamples = (g_sampleRate * lengthOfEntireMeasureInSeconds)
	var nlengthOfEntireMeasureInSamples = Math.floor(g_sampleRate * lengthOfEntireMeasureInSeconds)
	g_nlengthOfEntireMeasureInSamples = nlengthOfEntireMeasureInSamples
	var flengthOfDivisionInSamples = (nlengthOfEntireMeasureInSamples / cbeattop.divisions)
	var map = {}
	for (var i=0; i<cbeattop.channels.length;i++)
	{
		if (cbeattop.channels[i].samplename=='off') continue;
		var foffset = cbeattop.channels[i].rotation * flengthOfEntireMeasureInSamples
		for (var j=0; j<cbeattop.channels[i].arrHits.length; j++)
		{
			if (cbeattop.channels[i].arrHits[j] != 0)
			{
			var nlocation = Math.floor(j*flengthOfDivisionInSamples + foffset)%nlengthOfEntireMeasureInSamples
			for (var k=0; k<20; k++)
			{
				if (map[nlocation] === undefined) break;
				nlocation = (nlocation+1)
				if (nlocation>=nlengthOfEntireMeasureInSamples)
					nlocation=nlocation%nlengthOfEntireMeasureInSamples
			}
			map[nlocation] = i; //the channel number
			}
		}
	}
	
	// set up g_requestSoundChannelState
	setuprequestSoundChannelState(cbeattop)
	return map
}

var g_fPlaying = false

var g_requestSoundDataIndex = 0
var g_nlengthOfEntireMeasureInSamples = null //tempo -can- be changed while playing?
var g_currentAudioMap = {}
var g_requestSoundChannelState = [] //points to {pos: , audio: }
var g_visIsOn = false;
function requestSoundData(soundData)
{
	if (!g_fPlaying) return;
	if (g_visIsOn)
	{
		g_visIsOn = false;
		toggleVis(false);
	}
	else if (g_requestSoundDataIndex < soundData.length)
	{
		g_visIsOn = true;
		toggleVis(true);
	}
	for (var i=0, size=soundData.length; i<size; i++) {
		
		g_requestSoundDataIndex++;
		if (g_requestSoundDataIndex >= g_nlengthOfEntireMeasureInSamples)
			g_requestSoundDataIndex = 0;
		
		// look for triggers and turn on audio
		var mapc = g_currentAudioMap[g_requestSoundDataIndex]
		if (mapc !== undefined)
		{
			g_requestSoundChannelState[mapc].pos = 904 //begin playing that
		}
		 
		// play sounds that have been triggered
		soundData[i] = 0.0
		for (var key=0; key<g_requestSoundChannelState.length; key++)
		{
			if (g_requestSoundChannelState[key].pos > g_requestSoundChannelState[key].audio.length)
			{
				g_requestSoundChannelState[key].pos = 0
			}
			else if (g_requestSoundChannelState[key].pos > 0)
			{
				var cpos = g_requestSoundChannelState[key].pos
				if (g_beatObject.channels[key].vol < 1.5)
				{
				var sm = g_requestSoundChannelState[key].audio[cpos]*g_beatObject.channels[key].vol
					*g_beatObject.channels[key].vol; //make vol quadratic
				}
				else
				{
					var sm = g_requestSoundChannelState[key].audio[cpos]*16; //overdrive
					if (sm>1.0) sm=1.0
					if (sm<-1.0) sm=-1.0
					sm*=(g_beatObject.channels[key].vol-1.5)/4 //but make it quieter
				}
				soundData[i] += sm
				++g_requestSoundChannelState[key].pos
			}
		}
		
	}
}

ondoplay =function() {
	// it seems ok not to reset g_requestSoundDataIndex = 0. depends on what user expects
	g_fPlaying = true;
}
function ondopause() { g_fPlaying = false; }
function changeDivisionNumber(cbeattop, newdivs)
{
	// strip info after the division
	for (var i=0; i<cbeattop.channels.length; i++)
	{
		var newarr = []
		for (var j=0; j<newdivs;j++) 
		{
			if (j < cbeattop.channels[i].arrHits.length && cbeattop.channels[i].arrHits[j])
				newarr[j] = 1
			else
				newarr[j] = 0
		}
		cbeattop.channels[i].arrHits = newarr
	}
	cbeattop.divisions = newdivs
}

g_mediaTable = {}
getNone=[0.0,0.0]

function getmedia()
{
	var g_mediaNames = [
	"kick",get909bass ,
"snr1",get909snare , 
"snr2",get606snare , 
"snr3",get808snare , 
"clap",get909clap , 
"hihat",get909closed, 
"hihat2",get909pedal , 
"beep",getbip , 
"jungle",getjunglesnare]
	
	//unpack.
	function unpack(compressed)
	{
		var ret = new Float32Array(compressed.length*2)
		for (var i=0; i<compressed.length; i++)
		{
			var s1 = (compressed[i]&0xffff0000) >>>16;
			var s2 = (compressed[i]&0x0000ffff);
			var sf1 = ((s1/65536.0)*2)-1
			var sf2 = ((s2/65536.0)*2)-1
			ret[i*2] = sf1
			ret[i*2+1] = sf2
		}
		return ret
	}
	
	for (var j=0; j<g_mediaNames.length; j+=2)
	{
		var sname = g_mediaNames[j]
		var sdata = g_mediaNames[j+1]
		g_mediaTable[sname] = unpack(sdata)
	}
}
