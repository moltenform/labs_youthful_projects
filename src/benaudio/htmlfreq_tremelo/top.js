// htmlfreq tremolo, Ben Fisher, 2011
// http://halfhourhacks.blogspot.com
// GPL v3 license, see http://www.gnu.org/licenses/gpl.txt for the terms of using this code
// This is a written-on-the-bus spare-time project


/*
add rand feature.
*/

var Ra = null; //main raphael instance
var g_sampleRate = 44100;
var WIDTH=800
var HEIGHT=600
var g_audioState = null;

var g_fPlaying = false;
var g_CompleteWaveformIndex = 0;
var g_arrayCompleteWaveform = null;
var g_fLayerAudio = false


function CWobbleTop()
{
	this.channels = [];
	for (var i=0; i<3; i++) this.channels.push(new CWobbleChannel())
}
function CWobbleChannel()
{
	this.freq = 0.3; // [0-1], for ui only
	this.freq_a = cvtbasefrequitofreq(this.freq);
	this.wavetype = 'off'; // ['off', 'sine','square', 'saw','tri','noise']
	this.wavetype_f = 0.0
	this.vol = 0.05;
	this.modifiers = [];
	for (var i=0; i<5; i++) this.modifiers.push(new CWobbleModifier())
}
function CWobbleModifier()
{
	this.modtype='off'
	this.modtype_f = 0.0
	this.freq = 0.01 // [0-1], for ui only
	this.freq_a = cvtmodfrequitofreq(this.freq) //period of 0.01 to 8, freq of 0.125 to 100 
	this.width = 1.5
	this.phase = 1.0 //phase shift
}

function setup()
{
	var arWaveTypes = ['off', 'sine','square', 'saw','tri','noise']
	var arModTypes = ['off', 'vib','trem', 'vib10','trem10', /* 'tremrand' ,*/ 'vibc']
	var secondsOfAudio = 15.0;
	
	g_arrayCompleteWaveform = makeArray(Math.floor(secondsOfAudio * g_sampleRate))
	g_audioState = new CWobbleTop();
	g_audioState.channels[0].wavetype='sine'
	g_audioState.channels[0].wavetype_f=0.2
	g_audioState.channels[0].modifiers[0].modtype='vib'
	g_audioState.channels[0].modifiers[0].modtype_f=0.2
	g_audioState.channels[0].modifiers[0].width=0.1
	g_audioState.channels[0].modifiers[0].freq=0.32
	g_audioState.channels[0].modifiers[0].freq_a=cvtmodfrequitofreq(0.32)

	if (document.location.href.indexOf('?')!=-1)
	{
		var retState = onLoadFromJson(document.location.href.substring(document.location.href.indexOf('?')+1))
		if (retState)
			g_audioState = retState
	}
	
	Ra = Raphael("holder", WIDTH, HEIGHT);
	
	for (var x=0; x<g_audioState.channels.length; x++)
	{
		var sx = x.toString()
		var cdzone = makesliderzone('znchanwavetype'+sx, g_audioState.channels[x], 'wavetype_f', 40+300*x, 20, 120, 0, 1 /* unused */)
		$('ui_label_znchanwavetype'+sx).raphael.remove()
		addEnumeratedType(cdzone, arWaveTypes, 'wavetype', -20)
		cdzone.tmp_prevfn = cdzone.onValueChanged;
		cdzone.tmp_whichChannel = x
		cdzone.onValueChanged = function(fx,fy)
		{
			this.tmp_prevfn(fx,fy)
			setenabledChannel(this.tmp_whichChannel)
			//could optimize by only doing this when this.valuesetobj['wavetype'] changes
		}
		
		makesliderzone('znchanvol'+sx, g_audioState.channels[x], 'vol', 40+300*x, 45, 120, 0.0, 0.1, 20)
		cdzone = makesliderzone('znchanfreq'+sx, g_audioState.channels[x], 'freq', 40+300*x, 45+25, 120, 0.0, 1.0 /*will be scaled*/, 20)
		cdzone.onValueChanged = function(fx,fy) { this.valuesetobj.freq_a=cvtbasefrequitofreq(fx);  }
		for (var y=0; y<g_audioState.channels[x].modifiers.length; y++)
		{
			var sy = y.toString()
			
			var cdzone = makesliderzone('znmodtype'+sx+','+sy, g_audioState.channels[x].modifiers[y], 'modtype_f', 40+300*x, 155+60*y, 120/2, 0.0, 1.0 /*unused*/)
			$('ui_label_znmodtype'+sx+','+sy).raphael.remove()
			addEnumeratedType(cdzone, arModTypes, 'modtype', -20)
			cdzone.tmp_prevfn = cdzone.onValueChanged;
			cdzone.tmp_whichChannel = x
			cdzone.onValueChanged = function(fx,fy)
			{
				//sets for the whole channel... a bit unnecessary.
				this.tmp_prevfn(fx,fy)
				setenabledChannel(this.tmp_whichChannel)
				//could optimize by only doing this when this.valuesetobj['modtype'] changes
			}
			makesliderzone('znmodphase'+sx+','+sy, g_audioState.channels[x].modifiers[y], 'phase', 40+70+300*x, 155+60*y, 120/2-10, 0.0, 1.0, 20)
			$('ui_label_znmodphase'+sx+','+sy).raphael.remove()
			$('znmodphase'+sx+','+sy+'_dom_').setAttribute('title','phaseshift')
			$('znmodphase'+sx+','+sy+'_dom_').setAttribute('alt','phaseshift')
			
			makesliderzone('znmodwidth'+sx+','+sy, g_audioState.channels[x].modifiers[y], 'width', 40+300*x, 175+60*y, 120, 0.0, 3.0, 20)
			cdzone = makesliderzone('znmodfreq'+sx+','+sy, g_audioState.channels[x].modifiers[y], 'freq', 40+300*x, 195+60*y, 120, 0.0, 1.0 /*will be scaled*/, 20)
			cdzone.onValueChanged = function(fx,fy) { this.valuesetobj.freq_a=cvtmodfrequitofreq(fx);  }
		}
	}
	setenabledChannel(0)
	setenabledChannel(1)
	setenabledChannel(2)
	cd2_setselect(null) //no current selection
	
}



function requestSoundData(soundData)
{
	if (!g_fPlaying) return;

	for (var i=0, size=soundData.length; i<size; i++) {
		if (g_CompleteWaveformIndex >= g_arrayCompleteWaveform.length)
			soundData[i] = 0.0;
		else
			soundData[i] = g_arrayCompleteWaveform[g_CompleteWaveformIndex++];
	}
}

var g_prevSerializedState;
var g_startedAudiolet=false;
function on_btnplay() {
	// hack: use Json to see if we need to recalc.
	var sState = JSON.stringify(g_audioState)
	if (sState!=g_prevSerializedState)
	{
		generateTremoloAudio(g_arrayCompleteWaveform, g_audioState);
		g_CompleteWaveformIndex = 0;
		g_prevSerializedState = sState
	}
	else // if (g_CompleteWaveformIndex >= g_arrayCompleteWaveform.length)
		g_CompleteWaveformIndex = 0;
	
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
	
	g_fPlaying = true; 
}
function on_btnpause() { g_fPlaying = false; }
function ontogglelayers() { g_fLayerAudio = !g_fLayerAudio; }

function setenabledZoneid(zoneid, bEnable)
{
	var newop = bEnable?1.0:0.1;
	if ($('ui_label_'+zoneid)) $('ui_label_'+zoneid).raphael.attr({'opacity':newop})
	if ($('ui_lenum_'+zoneid)) $('ui_lenum_'+zoneid).raphael.attr({'opacity':newop})
	$('ui_rect_0_'+zoneid).raphael.attr({'opacity':newop})
	$('ui_rect_1_'+zoneid).raphael.attr({'opacity':newop})
	$('ui_rect_2_'+zoneid).raphael.attr({'opacity':newop})
	$('ui_rect_3_'+zoneid).raphael.attr({'opacity':newop})
	$(zoneid + '_dom_').raphael.attr({'opacity':newop})
}

function setenabledChannel(nchannel)
{
	var bEnable=g_audioState.channels[nchannel].wavetype!='off'
	
	var sx = nchannel.toString()
	setenabledZoneid('znchanvol'+sx, bEnable)
	setenabledZoneid('znchanfreq'+sx, bEnable)
	for (var y=0; y<g_audioState.channels[nchannel].modifiers.length; y++)
	{
		var sy = y.toString()
		var bEnableAll = false
		if (bEnable) bEnableAll = g_audioState.channels[nchannel].modifiers[y].modtype!='off'
		setenabledZoneid('znmodtype'+sx+','+sy, bEnable)
		setenabledZoneid('znmodwidth'+sx+','+sy, bEnableAll)
		setenabledZoneid('znmodphase'+sx+','+sy, bEnableAll)
		setenabledZoneid('znmodfreq'+sx+','+sy, bEnableAll)
	}
}

function on_btnnew()
{
	if (confirm('Start over?'))
		document.location.href = document.location.href.split('?')[0]
}
function on_btnsave()
{
	if (!JSON) {errmsg('Could not find JSON object. Try using latest Firefox or Chrome.'); return; }
	var objtop = {v:1, d:g_audioState}
	var s = JSON.stringify(objtop)
	
	s=s.replace(/"/g,'')
	s=s.replace(/trem10/g, 'RTEN')
	s=s.replace(/vib10/g, 'VTEN')
	s=s.replace(/freq/g, 'FF')
	s=s.replace(/type/g, 'TT')
	var fsturl = document.location.href.split('?')[0];
	prompt('share this link!', fsturl+'?'+s)
}
function onLoadFromJson(s)
{
	s=s.replace(/FF/g, 'freq')
	s=s.replace(/TT/g, 'type')
	
	//restore the quotes
	s = s.replace(/({)([a-zA-Z])/g, '$1"$2')
	s = s.replace(/(:)([a-zA-Z])/g, '$1"$2')
	s = s.replace(/(,)([a-zA-Z])/g, '$1"$2')
	s = s.replace(/([a-zA-Z])(,)/g, '$1"$2')
	s = s.replace(/([a-zA-Z])(:)/g, '$1"$2')
	s = s.replace(/([a-zA-Z])(})/g, '$1"$2')
	
	s=s.replace(/RTEN/g, 'trem10')
	s=s.replace(/VTEN/g, 'vib10')
	
	var topobj;
	
	try {
		topobj = JSON.parse(s)
	}
	catch(err) {
		errmsg('could not parse')
		return null;
	}
	if (!topobj || !topobj.v|| !topobj.d) { errmsg('invalid saved file.'); return null;}
	return topobj.d
}


function cvtbasefrequitofreq(normalized)
{
	var a=30, b=1920 //6 octaves
	var v = Math.exp(normalized) //[0,1] to [1,2.71]
	v = (v-1)/(Math.E-1);
	return a+(b-a)*v
}
function cvtmodfrequitofreq(x)
{
	return (x*x*8.0)+0.01
}

