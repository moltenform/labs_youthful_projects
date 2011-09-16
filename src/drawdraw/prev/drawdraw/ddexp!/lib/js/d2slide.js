// d2slide, Ben Fisher, 2011
// no polling: when you move the slider it will update the value directly.
// (0,0) is the top left corner.

var g_gd2 = new CD2SlideGlobal();

function CD2SlideGlobal()
{
    this.domSelected = null;
    this.map = {}; // from zoneid string to CD2SlideZone
}

/*
will set values in valuesetobj by valuesetkey_x and valuesetkey_y.
DOM objects have id in form zoneid_dom_
*/

function CD2SlideZone(zoneid, namex, namey, valuesetobj, valuesetkey_x, valuesetkey_y)
{
    if (!namex) errmsg('no name provided')
    this.zoneid = zoneid;
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
    this.onValueChanged = null
    this.onValueChangedLarge = null
    this.valuesetobj = valuesetobj;
    this.valuesetkey_x = valuesetkey_x;
    this.valuesetkey_y = valuesetkey_y;
    if (g_gd2.map[this.zoneid] !== undefined) { errmsg('warning, zoneid '+this.zoneid+' already present.') }
    g_gd2.map[this.zoneid] = this;
    
    this.currentNumber=0
}
function CD2SlideZone_ToString(cdzone)
{
    var s = cdzone.valuesetobj[cdzone.valuesetkey_x].toString();
    if (cdzone.namey)
        s+= ', '+cdzone.valuesetobj[cdzone.valuesetkey_y].toString();
    return s;
}

function CD2SlideZone_Add(cdzone, rx /* =null */, ry /* =null */)
{
    if (!rx) rx = (cdzone.rx1+cdzone.rx2)/2.0;
    if (!ry) ry = (cdzone.ry1+cdzone.ry2)/2.0;
    var domobj = Ra.ellipse(1, 1, 4, 4).attr({"stroke-width": 1});
    
    cdzone.currentNumber++;
    if (cdzone.currentNumber>1) {errmsg('multiple not supported anymore.'); return; }
    
    domobj.drag(cd2_ondragmove, cd2_ondragstart, cd2_ondragend)
    domobj.mousedown(cd2_onmousedown)
    domobj.node.id = cdzone.zoneid+'_dom_'
    
    ui_draw_bead(domobj, cdzone);
    
    
    cd2_setselect(domobj);
    if (cdzone.onValueChanged)
        cdzone.onValueChanged(cdzone.valuesetobj[cdzone.valuesetkey_x], cdzone.valuesetobj[cdzone.valuesetkey_y])
    return domobj
}

// refresh UI based on values.
function CD2SlideZone_RefreshUI()
{
    for (var key in g_gd2.map)
    {
        var cdzone = g_gd2.map[key]
        var objRaph = document.getElementById(key).raphael
        
        ui_draw_bead(objRaph, cdzone);
        if (cdzone.onValueChanged)
            cdzone.onValueChanged(cdzone.valuesetobj[cdzone.valuesetkey_x], cdzone.valuesetobj[cdzone.valuesetkey_y])
    }
}

function domObjectToCdzone(domobject)
{
    var sid = domobject.node.id;
    if (!sid) {errmsg('invalid domobj'); return;}
    var obj =  g_gd2.map[ sid.split('_')[0] ]
    if (!obj) { errmsg('could not find '+sid.split('_')[0] ); return;}
    return obj
}


function CD2SlideZone_RenderBorder(cdzone) {
    var dmtmp0 = Ra.path('M1,1,L,5,5' ).attr({'stroke-width':1}); dmtmp0.node.id = 'ui_rect_0_'+cdzone.zoneid
    var dmtmp1 = Ra.path('M1,1,L,5,5' ).attr({'stroke-width':1}); dmtmp1.node.id = 'ui_rect_1_'+cdzone.zoneid
    var dmtmp2 = Ra.path('M1,1,L,5,5' ).attr({'stroke-width':1}); dmtmp2.node.id = 'ui_rect_2_'+cdzone.zoneid
    var dmtmp3 = Ra.path('M1,1,L,5,5' ).attr({'stroke-width':1}); dmtmp3.node.id = 'ui_rect_3_'+cdzone.zoneid
    ui_draw_line(cdzone, dmtmp0, cdzone.rx1, cdzone.ry1,cdzone.rx1, cdzone.ry2);
    ui_draw_line(cdzone, dmtmp1, cdzone.rx2, cdzone.ry1,cdzone.rx2, cdzone.ry2);
    ui_draw_line(cdzone, dmtmp2, cdzone.rx1, cdzone.ry1,cdzone.rx2, cdzone.ry1);
    ui_draw_line(cdzone, dmtmp3, cdzone.rx1, cdzone.ry2,cdzone.rx2, cdzone.ry2);
}

function setCoordsSelected()
{
    if (!g_gd2.domSelected) return;
    var cdzone = domObjectToCdzone(g_gd2.domSelected)
    var newx = prompt(cdzone.namex, cdzone.valuesetobj[cdzone.valuesetkey_x])
    if (newx == null) return;
    
    if (cdzone.namey != null)
    {
        var newy = prompt(cdzone.namey, cdzone.valuesetobj[cdzone.valuesetkey_y])
        if (newy == null) return;
        cdzone.valuesetobj[cdzone.valuesetkey_y] = newy
    }
    cdzone.valuesetobj[cdzone.valuesetkey_x] = newx
    
    ui_draw_bead(g_gd2.domSelected, cdzone);
    if (cdzone.onValueChanged) { cdzone.onValueChanged(newx, newy) }
}


function cd2_ondragstart()
{
    this.ox = this.attr("cx");
    this.oy = this.attr("cy");
}
function cd2_ondragmove(dx, dy)
{
    var cdzone = domObjectToCdzone(this)
    var wouldx = this.ox + dx, wouldy = this.oy + dy;
    
    if (wouldx>cdzone.sx2) wouldx = cdzone.sx2
    if (wouldx<cdzone.sx1) wouldx = cdzone.sx1
    var rcoords = s_to_r(cdzone, wouldx, wouldy)
    if (rcoords[1]<cdzone.ry1) { rcoords[1] = cdzone.ry1; wouldy = cdzone.sy1;}
    if (rcoords[1]>cdzone.ry2) { rcoords[1] = cdzone.ry2; wouldy = cdzone.sy2;}
    
    // set the values.
    cdzone.valuesetobj[cdzone.valuesetkey_x] = rcoords[0]
    if (cdzone.namey != null) cdzone.valuesetobj[cdzone.valuesetkey_y] = rcoords[1]
        
    this.attr({cx: wouldx, cy: wouldy});
    if (cdzone.onValueChanged) cdzone.onValueChanged(rcoords[0], rcoords[1])
}
function cd2_ondragend()
{ 
    var cdzone = domObjectToCdzone(this); 
    if (cdzone.onValueChangedLarge) 
        cdzone.onValueChangedLarge();
}
function cd2_onmousedown() { cd2_setselect(this); }
function cd2_setselect(domobj)
{
    if (g_gd2.domSelected)
        g_gd2.domSelected.attr({fill: "#000"}); //, opacity: 1
    if (domobj)
    {
        g_gd2.domSelected = domobj;
        g_gd2.domSelected.attr({fill: "#0f0"});
        g_gd2.domSelected.toFront();
    }
    else
    {
        g_gd2.domSelected = null
    }
}



function ui_draw_bead(domobj, cdzone)
{
    var rx = cdzone.valuesetobj[cdzone.valuesetkey_x]
    var ry = (cdzone.namey ? cdzone.valuesetobj[cdzone.valuesetkey_y] : 0.5)
    var scoords = r_to_s(cdzone, rx, ry)
    domobj.attr( {cx:scoords[0],cy:scoords[1]});
    
}
function ui_draw_line(cdzone, domobj, rx1, ry1, rx2, ry2)
{
    var scoords1 = r_to_s(cdzone,rx1, ry1)
    var scoords2 = r_to_s(cdzone,rx2, ry2)
    var sNewPath = 'M'+scoords1[0].toString()+','+scoords1[1].toString() + ',L'+scoords2[0].toString()+','+scoords2[1].toString();
    domobj.attr('path',sNewPath);
}

// 1) constructs cdzone
// 2) renders cdzone
// 3) adds one slider, with id 0. the id is index to valuesetkeys
function makesliderzone(zoneid, valuesetobj, name /*both index and rendered text*/, sx, sy, swidth, rx1, rx2, xlabelpos)
{
    if (!xlabelpos) xlabelpos = 50
    var cdzone = new CD2SlideZone(zoneid, name, null /* no y axis */, valuesetobj, name, null /* no y axis */) 
    cdzone.sx1=sx;	cdzone.sx2=sx+swidth; cdzone.sy1=sy;	cdzone.sy2 = sy+1;
    cdzone.rx1=rx1;	cdzone.rx2=rx2;
    cdzone.ry1=0.0;	cdzone.ry2=1.0; //unused because there is no y axis
    CD2SlideZone_RenderBorder(cdzone);
    
    CD2SlideZone_Add(cdzone)	
    var dtxt = Ra.text( sx - xlabelpos, sy, name)
    dtxt.node.id = 'ui_label_'+cdzone.zoneid
    return cdzone
}

function makesliderzone2d(zoneid, valuesetobj, name, namey, xindex, yindex, swidth, sheight, screencds, rxbounds, rybounds)
{
    var cdzone = new CD2SlideZone(zoneid, name, namey, valuesetobj, xindex, yindex) 
    // note: we swap the order here because y screen is reversed.
    var tmp = rybounds[0]; rybounds[0]=rybounds[1]; rybounds[1]=tmp;
    
    cdzone.sx1=screencds[0];	cdzone.sx2=screencds[0]+swidth;
    cdzone.sy1=screencds[1];	cdzone.sy2 = screencds[1]+sheight;
    cdzone.rx1=rxbounds[0];	cdzone.rx2=rxbounds[1];
    cdzone.ry1=rybounds[0];	cdzone.ry2=rybounds[1];
    
    CD2SlideZone_RenderBorder(cdzone);
    CD2SlideZone_Add(cdzone)
    
    var dtxt = Ra.text( screencds[0]-50, screencds[1] + sheight/2, name)
    dtxt.node.id = 'ui_label_'+cdzone.zoneid
    return cdzone
}

function s_to_r(cd, sx, sy)
{
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
        (tmpy*(cd.sy2-cd.sy1)+cd.sy1)]
}

function addEnumeratedType(cdzone, arrValues, valuesetkey, domlabelposition)
{
    cdzone.rx1 = 0.0; cdzone.rx2 = arrValues.length;
    var domlabel = null;
    if (domlabelposition && cdzone.valuesetobj[valuesetkey] && arrValues.indexOf(cdzone.valuesetobj[valuesetkey])==-1)
    {
        // restrict drawing label to valid strings
        errmsg('not set to valid constant')
        domlabel = Ra.text( cdzone.sx1 + domlabelposition, cdzone.sy1, '')
        domlabel.node.id='ui_lenum_'+cdzone.zoneid
    }
    else if (domlabelposition)
    {
        domlabel = Ra.text( cdzone.sx1 + domlabelposition, cdzone.sy1, cdzone.valuesetobj[valuesetkey])
        domlabel.node.id='ui_lenum_'+cdzone.zoneid
    }
    // keep array in the closure.
    cdzone.onValueChanged = function(newx, newy)
    {
        var enval = Math.floor(newx)
        if (enval >= arrValues.length) enval = arrValues.length-1;
        if (enval <= 0) enval = 0;
        var whichMode = arrValues[enval]
        if (domlabel) domlabel.attr({text: whichMode})
        cdzone.valuesetobj[valuesetkey] = whichMode
    }
    // update ui position, because rx2 has changed
    ui_draw_bead($(cdzone.zoneid+'_dom_').raphael, cdzone)
}

function addPositionIndicator(cdzone, domlabelposition)
{
    var domlabel = Ra.text( cdzone.sx1 + domlabelposition, cdzone.sy1, CD2SlideZone_ToString(cdzone))
    cdzone.onValueChanged = function(newx, newy)
    {
        var s = CD2SlideZone_ToString(this)
        domlabel.attr({text: s})
    }
}

