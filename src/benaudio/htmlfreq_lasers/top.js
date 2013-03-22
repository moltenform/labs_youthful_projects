// htmlfreq lasers, Ben Fisher, 2011
// http://halfhourhacks.blogspot.com
// GPL v3 license, see http://www.gnu.org/licenses/gpl.txt for the terms of using this code
// This is a written-on-the-bus spare-time project

var Ra = null; //main raphael instance

var g_sampleRate = 44100;
var WIDTH=720
var HEIGHT=300

var g_audioBackendState = {'frequency' : 210.0, 'sparkliness': 0.0006, 'waveshape' : 0.0, 
	'shape_enum' : 'sine', // or saw, or square, or tri.
	'harmonic': 4 }


function setup()
{
	Ra = Raphael("holder", WIDTH, HEIGHT);
	g_cglobal = new CD2SlideGlobal();

	makesliderzone2d('zonesetbothfreqshape', g_audioBackendState, 'move the dot', 'harmonic', 'frequency', 'harmonic',
		/* dimensions */ 600, 150, /* screencds */ [100, 15], /* rxbounds */ [200.0, 300.0], /* rybounds */ [6, /* down to */ 1.0])
	
	makesliderzone('zonesetsparkle', g_audioBackendState, 'sparkliness', 100, 245-25, 200, 0.002, /* down to */ 0.000)
	var arTypes = ['sine','square', 'saw','tri']
	var cdzone = makesliderzone('zonesetshape', g_audioBackendState, 'waveshape', 100, 300-25, 160, 0.0, 1.0)
	addEnumeratedType(cdzone, arTypes, 'shape_enum', 180)
}


var bHaveSetup = false;
function on_btnplay()
{
	if (!bHaveSetup)
	{
		setup();
		var audiolet = new Audiolet();
		var wrapper = new WrapperNode(audiolet, requestSoundDataLaser);
		wrapper.connect(audiolet.output);
		bHaveSetup = true;
	}
	
	play_start();
}

function on_btnpause() {
	play_stop();
}

