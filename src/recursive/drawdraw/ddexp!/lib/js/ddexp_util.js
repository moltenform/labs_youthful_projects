gcleanup = []
g_slidestate = {c1:0.5, c2:0.5}

function includejs(jsFile) //contents will not be defined immediately.
{
  document.write('<scr'+'ipt type="text/javascript" src="' + jsFile + '"></scr' + 'ipt>'); 
} 

//set these to null, they will be overridden if set later
onuserslidermove = null, onuserslidermovelarge=null, onuserload=null, onuserkeycode=null;

function setup()
{
	Ra = Raphael("holder", 640*1.25, 480*1.25);
	var cdzone = makesliderzone('zonesetc1', g_slidestate, 'c1', 60, 20, 120, 0, 1 /* unused */)
	if (onuserslidermove) cdzone.onValueChanged = onuserslidermove
	if (onuserslidermovelarge) cdzone.onValueChangedLarge = onuserslidermovelarge
	cdzone = makesliderzone('zonesetc2', g_slidestate, 'c2', 60+200, 20, 120, 0, 1 /* unused */)
	if (onuserslidermove) cdzone.onValueChanged = onuserslidermove
	if (onuserslidermovelarge) cdzone.onValueChangedLarge = onuserslidermovelarge	
	
	
	$('user_output').value='';
	if (onuserload) onuserload()
}

shapes_clear = function() {
	clearshapes()
	$('user_output').value='';
}
print_clear = function() { $('user_output').value = ''; }
print = function(s)
{
	//unfortunately, doesn't seem like we can set scroll
	s = s.toString();
	$('user_output').value += '\r\n' + s;
}

reload = function()
{
	 print_clear()
	shapes_clear()
	if (onuserload) onuserload()
}

o_onKeyDown = function(event)
{
	if (onuserkeycode) 
		onuserkeycode(event.keyCode)
}


clearshapes=function()
{
	for(var i=0; i<gcleanup.length; i++)
		gcleanup[i].remove()
	gcleanup = []
}

rad=function(a){return a%360*Math.PI/180};
deg=function(a){return a*180/Math.PI%360};
function degcos(a) { return Math.cos((a/360.0)*2*Math.PI); }
function degsin(a) { return Math.sin((a/360.0)*2*Math.PI); }

function onzoomin() { g_zoomMult += 15; reload(); }
function onzoomout() { g_zoomMult -= 15; reload(); }
g_zoomMult = 45;
drawline = function (x1,y1,x2,y2, optColor) {
	x1*=g_zoomMult;y1*=g_zoomMult; x2*=g_zoomMult; y2*=g_zoomMult;
	x1+=100; x2+=100;
	y1=500-y1; y2=500-y2;
	var sNewPath = 'M'+x1.toString()+','+y1.toString() + ',L'+x2.toString()+','+y2.toString();
	var o= Ra.path(sNewPath );
	gcleanup.push(o)
	if (optColor)
		o.attr({stroke:optColor})
	return o
}
drawpoint = function(x,y)
{
	return drawline(x-0.01, y,x+0.01,y)
}

//////////////////////////////////////

spiral_get_val_iter = function(r, th, dofn)
{
	var x=0, y=0
	var curl=1.0, curth=0.0
	for (var i=0; i<100; i++)
	{
		nx = x+curl*degcos(curth)
		ny = y+curl*degsin(curth)
		curth+=th
		curl *= r
		if (dofn) dofn(x,y,nx,ny)
		x=nx;
		y=ny;
	}
	return [x,y]
}
spiral_get_val_now = function(r, th)
{
	var q = r*r + 1 - 2*r*degcos(th)
	var x = (1-r*degcos(th))/q;
	var y = (r*degsin(th))/q;
	return [x,y]
}
spiral_draw = function(r,th, optcolor)
{
	var fn = function(x,y,nx,ny) { return drawline(x,y,nx,ny,optcolor) }
	spiral_get_val_iter(r,th,fn)
}


