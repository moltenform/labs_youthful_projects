// htmlfreq, Ben Fisher, 2011
// halfhourhacks.blogspot.com
// This is a fun spare-time project, production code it ain't.
// known issue: if all points deleted, throws some exceptions. seems to be recoverable.

function makesliderzone(name, sx, sy, swidth, rx1, rx2)
{
	cdzone = new CD2SlideZone(name, null) // no y axis
	cdzone.sx1=sx;	cdzone.sx2=sx+swidth; cdzone.sy1=sy;	cdzone.sy2 = sy+1;
	cdzone.rx1=rx1;	cdzone.rx2=rx2;	
	cdzone.ry1=0.0;	cdzone.ry2=1.0; //unused
	cd2_zone_render(cdzone);
	
	Ra.text( sx - 50, HEIGHT-sy, name)
	return cdzone
}

var Ra = null;
var g_cglobal = null;
var WIDTH=800
var HEIGHT=600
var g_tip = null;

var g_zoneslshape = null;
var g_arrinfoPitch = null;
var g_arrinfoCutoff = null;

var g_waveformln = null;

function setup()
{
	Ra = Raphael("holder", WIDTH, HEIGHT);
	g_cglobal = new CD2SlideGlobal();
	g_tip = Ra.text(55,-25,'welcome')
	g_waveformln = Ra.path('M1,1,L,2,2' ).attr({'stroke-width':1, 'stroke':'#bbb'});
	var widthwave = 220;
	var v = 16.0; //volume scaling
	var currenty = HEIGHT;
	
	currenty -= 100;
	var zoneslpitch = makesliderzone('frequency', 100, currenty, 400, 100.0, 700.0)
	g_arrinfoPitch = CD2Slide_add(g_cglobal, zoneslpitch, 170.0) // add one control
	
	
	currenty -= 60;
	var zonescutoff = makesliderzone('cut', 100, currenty, widthwave, 0.0, 1.0)
	g_arrinfoCutoff = CD2Slide_add(g_cglobal, zonescutoff, 0.0) // add one control
	
	//~ currenty -= 140;
	g_zoneslshape = new CD2SlideZone('x', 'time')
	g_zoneslshape.sx1=100;
	g_zoneslshape.sx2 = g_zoneslshape.sx1+widthwave;
	g_zoneslshape.sy1=300;
	g_zoneslshape.sy2=380;
	g_zoneslshape.rx1=0.0; g_zoneslshape.rx2=1.0;	g_zoneslshape.ry1=-1/v;	g_zoneslshape.ry2=1/v;
	cd2_zone_render(g_zoneslshape);
	Ra.text( g_zoneslshape.sx1 - 50, HEIGHT-g_zoneslshape.sy2,  'waveform')
	CD2Slide_add(g_cglobal, g_zoneslshape, 0.3, -0.5/v) // add default shape
	CD2Slide_add(g_cglobal, g_zoneslshape, 0.7, 0.5/v) // add default shape
	drawWaveformSketch()
	
	// start audio !
	audioDestination = new AudioDataDestination(sampleRate, requestSoundData);      
}



function add_shape()
{
	CD2Slide_add(g_cglobal, g_zoneslshape)
	drawWaveformSketch()
}
function deleteSelected_shape()
{
	// can only delete waveform ones
	if (g_cglobal.domSelected && g_cglobal.dictIdToInfo[ g_cglobal.domSelected.id ][0]!=g_zoneslshape)
		return;
	deleteSelected()
	drawWaveformSketch()
}
function getArr()
{
	var arr = []
	for (key in g_cglobal.dictIdToInfo)
		if (g_cglobal.dictIdToInfo[key][0] == g_zoneslshape)
			arr.push(g_cglobal.dictIdToInfo[key])
	if (!arr.length)
		return [ [g_zoneslshape, -.1, 0.0], [g_zoneslshape, 1.1, 0.0]]
	sortfn = function(a,b){return a[1] - b[1]}
	arr.sort(sortfn)
	//make it wrap around
	var nnew = [g_zoneslshape, arr[arr.length-1][1] - 1.0, arr[arr.length-1][2] ]
	var nend = [g_zoneslshape, arr[0][1] + 1.0, arr[0][2] ] 
	arr.unshift(nnew)
	arr.push(nend)
	return arr
}


function drawWaveformSketch()
{
	a = getArr()
	// repeat it a few times
	var newa = a.concat()
	for (var i=1; i<3; i++)
		for (var j=0; j<a.length; j++)
		{
			var em = a[j].concat();
			em[1]+= i ;
			newa.push( em)
		}
	ui_draw_long_line(g_zoneslshape, g_waveformln, newa)
	//~ getInterpolatedValue(a, 0.4)
}

