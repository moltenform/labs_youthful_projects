// htmlfreq, Ben Fisher, 2011
// globals: Ra, g_cglobal

function CD2SlideGlobal()
{
	this.domSelected = null;
	this.dictIdToInfo = {}; // from object id to arrinfo [CD2SlideZone, rx, ry]
}

function CD2SlideZone(namex, namey)
{
	if (!namex) errmsg('no name provided')
	this.namex = namex;
	this.namey = namey;
	this.sx1=0.0;
	this.sx2=0.0;
	this.sy1=0.0;
	this.sy2=0.0;
	this.rx1=0.0;
	this.ry1=0.0;
	this.rx2=0.0;
	this.ry2=0.0;
}
function cd2_zone_render(cdzone) {
	dmtmp = Ra.path('M1,1,L,5,5' ).attr({'stroke-width':2}); 
	ui_draw_line(cdzone, dmtmp, cdzone.rx1, cdzone.ry1,cdzone.rx1, cdzone.ry2); dmtmp = Ra.path('M1,1,L,1,1' ).attr({'stroke-width':2}); 
	ui_draw_line(cdzone, dmtmp, cdzone.rx2, cdzone.ry1,cdzone.rx2, cdzone.ry2); dmtmp = Ra.path('M1,1,L,1,1' ).attr({'stroke-width':2}); 
	ui_draw_line(cdzone, dmtmp, cdzone.rx1, cdzone.ry1,cdzone.rx2, cdzone.ry1); dmtmp = Ra.path('M1,1,L,1,1' ).attr({'stroke-width':2}); 
	ui_draw_line(cdzone, dmtmp, cdzone.rx1, cdzone.ry2,cdzone.rx2, cdzone.ry2);
	//~ alert( [cdzone.rx1, cdzone.ry2,cdzone.rx2, cdzone.ry2])
}

function deleteSelected()
{
	if (!g_cglobal.domSelected) return;
	var selected = g_cglobal.domSelected
	delete g_cglobal.dictIdToInfo[selected.id]
	g_cglobal.domSelected = null
	selected.remove()
	selected = null
}
function setCoordsSelected()
{
	if (!g_cglobal.domSelected) return;
	var arrinfo = g_cglobal.dictIdToInfo[g_cglobal.domSelected.id]
	var newx = prompt(arrinfo[0].namex, arrinfo[1])
	if (newx == null) return;
	var newy = arrinfo[2]
	if (arrinfo[0].namey != null)
	{
		var newy = prompt(arrinfo[0].namey, arrinfo[2])
		if (newy == null) return;
	}
	arrinfo[1] = newx, arrinfo[2] = newy;
	ui_draw(g_cglobal.domSelected, arrinfo);
}
function cd2_ondragstart()
{
	this.ox = this.attr("cx");
	this.oy = this.attr("cy");
}
gtmp=0
function cd2_ondragmove(dx, dy)
{
	var arrinfo = g_cglobal.dictIdToInfo[this.id];
	var cdzone = arrinfo[0]
	var wouldx = this.ox + dx, wouldy = this.oy + dy;
	
	if (wouldx>cdzone.sx2) wouldx = cdzone.sx2
	if (wouldx<cdzone.sx1) wouldx = cdzone.sx1
	var rcoords = s_to_r(cdzone, wouldx, wouldy)
	if (rcoords[1]<cdzone.ry1) { rcoords[1] = cdzone.ry1; wouldy = HEIGHT-cdzone.sy1;}
	if (rcoords[1]>cdzone.ry2) { rcoords[1] = cdzone.ry2; wouldy = HEIGHT-cdzone.sy2;}
	
	arrinfo[1] = rcoords[0]; arrinfo[2] = rcoords[1];
	//~ var s = rcoords[0].toString() + (cdzone.namey ? (', '+rcoords[1].toString()) : '')
	//~ g_tip.attr({text:s})
	this.attr({cx: wouldx, cy: wouldy});
	
	g_fdirty = true;
	if (++gtmp > 8)
	{ drawWaveformSketch(); gtmp=0;}
}
function cd2_ondragend() { drawWaveformSketch() }
function cd2_onmousedown() { cd2_setselect(this); }
function cd2_setselect(domobj)
{
	if (g_cglobal.domSelected)
		g_cglobal.domSelected.attr({fill: "#000", opacity: 1});
	g_cglobal.domSelected = domobj;
	g_cglobal.domSelected.attr({fill: "#0f0", opacity: 1});
	g_cglobal.domSelected.toFront();
	//~ var s = g_cglobal.dictIdToInfo[ g_cglobal.domSelected.id ][1].toString() + 
		//~ (cdzone.namey ? (', '+g_cglobal.dictIdToInfo[ g_cglobal.domSelected.id ][2].toString()) : '')
	
	//~ g_tip.attr({text:s})
}

function s_to_r(cd, sx, sy)
{
	sy = HEIGHT-sy;
	var tmpx = (sx-cd.sx1)/(cd.sx2-cd.sx1);
	var tmpy = (sy-cd.sy1)/(cd.sy2-cd.sy1);
	return [
		tmpx*(cd.rx2-cd.rx1)+cd.rx1,
		tmpy*(cd.ry2-cd.ry1)+cd.ry1]
}
function r_to_s(cd, rx, ry)
{
	var tmpx = (rx-cd.rx1)/(cd.rx2-cd.rx1);
	var tmpy = (ry-cd.ry1)/(cd.ry2-cd.ry1);
	return [
		tmpx*(cd.sx2-cd.sx1)+cd.sx1,
		HEIGHT - (tmpy*(cd.sy2-cd.sy1)+cd.sy1)]
}

function CD2Slide_add(cd2global, cdzone, rx /* =null */, ry /* =null */)
{
	if (rx==null) rx = (cdzone.rx1+cdzone.rx2)/2.0;
	if (ry==null) ry = (cdzone.ry1+cdzone.ry2)/2.0;
	var domobj = Ra.ellipse(1, 1, 4, 4).attr({"stroke-width": 1});
	
	domobj.drag(cd2_ondragmove, cd2_ondragstart, cd2_ondragend)
	domobj.mousedown(cd2_onmousedown)
	
	
	var arrinfo = [ cdzone, rx, ry];
	cd2global.dictIdToInfo[ domobj.id ] = arrinfo;
	
	ui_draw(domobj, arrinfo);
	cd2_setselect(domobj);
	//~ alert(arrinfo)
	return arrinfo;
}

function ui_draw(domobj, arrinfo)
{
	var scoords = r_to_s(arrinfo[0], arrinfo[1], arrinfo[2])
	domobj.attr( {cx:scoords[0],cy:scoords[1]});
}
function ui_draw_line(cdzone, domobj, rx1, ry1, rx2, ry2)
{
	var scoords1 = r_to_s(cdzone,rx1, ry1)
	var scoords2 = r_to_s(cdzone,rx2, ry2)
	var sNewPath = 'M'+scoords1[0].toString()+','+scoords1[1].toString() + ',L'+scoords2[0].toString()+','+scoords2[1].toString();
	domobj.attr('path',sNewPath);
}
function ui_draw_long_line(cdzone, domobj, arptsarinfo)
{
	var sNewPath = 'M';
	for (var i=0; i<arptsarinfo.length; i++)
	{
		var scoords1 = r_to_s(cdzone,arptsarinfo[i][1], arptsarinfo[i][2])
		sNewPath += scoords1[0].toString()+','+scoords1[1].toString();
		if (i!=arptsarinfo.length - 1)
			sNewPath += ',L'
	}
	domobj.attr('path',sNewPath);
}


