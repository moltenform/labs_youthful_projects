
var r = null;
var domSelected = null;
var domToCoordObject = {}; //also a record of what shapes have been created.

	
//holding dom objects is bad... well maybe live with it?
var g_shapeSelectA =null, g_shapeSelectB=null;

var g_zoomLevel = false; //means no zoom, equivalent to 1.0
var g_nShapesToDraw = 300;
var g_nJustPerimeter = 0; //draw just perimeter. trying with a small circle doesn't work as well as this.
var g_allLines = [];//to support loading saved...

var g_mapObjIdToArrow = {} //arrows on lgen shapes. see rendering.js/drawArrow()
	
var mainContextShape = new CRawShape({type:'lgen',x1:200,x2:200,y1:200,y2:100});
var mainContext = contextFromRawShape(mainContextShape);
//note that coords are upside down, so main context is '-90' because y coords are flipped

function setup()
{
	r = Raphael("holder", 640*1.25, 480*1.25);
	
	g_shapeSelectA = r.ellipse(1, 1, 4, 4).attr({"stroke-width": 1});
	g_shapeSelectB = r.ellipse(1, 1, 4, 4).attr({"stroke-width": 1});
	g_shapeSelectA.attr({fill: "#0f0", opacity: 1});
	g_shapeSelectB.attr({fill: "#0f0", opacity: 1});
	
	g_shapeSelectA.drag(onDragResize_move, onDragResize_start, onDragResize_up);
	g_shapeSelectB.drag(onDragResize_move, onDragResize_start,onDragResize_up);
	
	//draw reference context. NOT a selectable object.
	var mainGenPath = r.path('M1,1,L,1,1' ).attr({'stroke-width':6}) 
	updatePath(mainGenPath, mainContextShape) 
	var mainArrow = drawArrow(mainGenPath, mainContextShape,true) //manually draw an arrow.
	mainGenPath.attr( {stroke: '#aaf'}); 
	mainArrow.attr( {stroke: '#aaf', fill:'#aaf'});
	
	//create a normal line
	createNew('l');
	ocoord = domToCoordObject[domSelected.id];
	ocoord.x1 = mainContextShape.x1; ocoord.y1 = mainContextShape.y1;  ocoord.x2 = mainContextShape.x2;  ocoord.y2 = mainContextShape.y2; 
	refreshShape(domSelected);
	
	createNew('lgen');
	ocoord = domToCoordObject[domSelected.id];
	ocoord.x1 = mainContextShape.x1; ocoord.y1 = mainContextShape.y1;  ocoord.x2 = mainContextShape.x2+40;  ocoord.y2 = mainContextShape.y2+50;
	refreshShape(domSelected);
	showSelect();
	
	// note that we load a complete doc before loading json, so that if parsing json fails, we still have usable doc.
	var surlparts = document.location.href.split('?')
	if (surlparts.length==2 && surlparts[1].indexOf('{')!=-1)
	{
		//load the url!
		loadJson(surlparts[1])
	}
}

function refreshShape(domobj)
{
	var ocoord = domToCoordObject[domobj.id];
	updatePath(domobj, ocoord); 
	
	if (ocoord.type == 'lgen')
		drawArrow(domobj, ocoord)

	if (domSelected) showSelect()
}



var onDragResize_start = function () {
	// storing original coordinates
	this.ox = this.attr("cx");
	this.oy = this.attr("cy");
	var ocoord = domToCoordObject[domSelected.id];
	if (ocoord.type=='c' && this===g_shapeSelectB) 
		this.origRadius = ocoord.rx;
	
	this.countRefresh = 0;
}
var onDragResize_move = function (dx, dy) {
	var ocoord = domToCoordObject[domSelected.id];
	if (!ocoord) {alerd('no ocoord obj 234'); return; }
	if (ocoord.type=='c' && this===g_shapeSelectB) dy=0;
	if (ocoord.type=='c' && dx + this.origRadius<1) dx= -this.origRadius+1; //prevent circle < 1 radius
	
	this.attr({cx: this.ox + dx, cy: this.oy + dy});

	if (this!=g_shapeSelectA && this!=g_shapeSelectB) {
		alerd('unknown resizer?'); return; }
	if (ocoord.type.startsWith('l')) {
		if (this===g_shapeSelectA)
		{
			ocoord.x1 = this.ox + dx;
			ocoord.y1 = this.oy + dy;
		}
		else if (this===g_shapeSelectB)
		{
			ocoord.x2 = this.ox + dx;
			ocoord.y2 = this.oy + dy;
		}
	} else if (ocoord.type=='c') {
		if (this===g_shapeSelectA)
		{
			ocoord.x1 = this.ox + dx; //moving the center
			ocoord.y1 = this.oy + dy;
			//move other resizer accordingly.
			g_shapeSelectB.attr({ cx: ocoord.x1  + ocoord.rx, cy: ocoord.y1});
		}
		else if (this===g_shapeSelectB)
		{
			//changing the radius!
			ocoord.rx = dx + this.origRadius;
		}
	}
	refreshShape(domSelected); //redraw shape

	this.countRefresh++;
	if (this.countRefresh > 1) this.countRefresh = 0;
	if (this.countRefresh ==0) doTransformRender();
}
var onDragResize_up = function () {
	//redraw shapes.
	doTransformRender()
};


function createNew(stype)
{
	if (stype=='l' || stype=='lgen')
	{
		var newLine = r.path('M1,1,L,1,1' );
		var newO = new CRawShape({type:'l',x1:mainContextShape.x2,y1:mainContextShape.y2, x2:mainContextShape.x2+30, y2:mainContextShape.y2} );
		newO.type=stype
		if (stype=='lgen')
			newLine.attr({stroke:'#922', opacity:1.0})
	}
	else if (stype=='c')
	{
		var newLine = r.circle(1,1,1)
		var newO =  new CRawShape({type:'c',x1:mainContextShape.x2,y1:mainContextShape.y2,rx:20});
	}
	g_allLines.push(newLine)

	domToCoordObject[newLine.id] = newO;
	refreshShape(newLine); //draw at initial position
	newLine.mousedown(onMouseDownSelectIt);
	newLine.attr({"stroke-width": 4});
	domSelected = newLine;
	showSelect();
		r.safari(); //"There is an inconvenient rendering bug is Safari (WebKit): "
	return newLine;
}
function showSelect()
{
	g_shapeSelectA.show();
	g_shapeSelectB.show();
	var ocoord = domToCoordObject[domSelected.id];
	if (!ocoord) {alerd('error234'); return; }

	if (ocoord.type.startsWith('l')) {
		g_shapeSelectA.attr( {cx: ocoord.x1, cy: ocoord.y1})
		g_shapeSelectB.attr( {cx: ocoord.x2, cy: ocoord.y2})
	} else if (ocoord.type=='c') {
		g_shapeSelectA.attr( {cx: ocoord.x1, cy: ocoord.y1})
		g_shapeSelectB.attr( {cx: ocoord.x1 + ocoord.rx, cy: ocoord.y1})
	}
	g_shapeSelectA.toFront();
	g_shapeSelectB.toFront(); //this one should be in front, so that circles aren't stuck small
	
	r.safari(); //"There is an inconvenient rendering bug is Safari (WebKit): "
	//~ doTransformRender(); // //
}
function hideSelect()
{
	g_shapeSelectA.hide();
	g_shapeSelectB.hide();
	//~ doTransformRender(); // //
}
function deleteSelected()
{
	if (!domSelected) return;
	if (domToCoordObject[domSelected.id].type=='lgen')
	{
		g_mapObjIdToArrow[domSelected.id].remove() //remove arrow
		delete g_mapObjIdToArrow[domSelected.id];
	}
	delete domToCoordObject[domSelected.id];
	hideSelect();
	domSelected.remove();
	domSelected = null;
}
function invisSelected()
{
	if (!domSelected) return;
	domToCoordObject[domSelected.id].visible = !domToCoordObject[domSelected.id].visible;
	refreshShape(domSelected)
}
function onMouseDownSelectIt(event)
{
	if (!this) return;
	domSelected = this;
	showSelect();
}
	
var bIsRendering=false;
function doTransformRender()
{
	if (bIsRendering) return;
	bIsRendering=true;
	
	var gens = [];
	var objs = [];
	for (var objId in domToCoordObject)
	{
		if (domToCoordObject[objId].type == 'lgen')
			gens.push(domToCoordObject[objId])
		else if (domToCoordObject[objId].x1 !==undefined)
			objs.push(domToCoordObject[objId])
		else
			alerd('warning: something else in map')//debug
	}
	
	//make main reference into a context
	var initialContext = mainContext;
	
	//convert shapes to be relative to initial context
	for (var i=0; i<gens.length; i++) gens[i] = rawShapeToRelativeShape(initialContext, gens[i])
	for (var i=0; i<objs.length; i++) objs[i] = rawShapeToRelativeShape(initialContext, objs[i])
	
	
	var contextQueue = [initialContext]
	var nAdjustX = 300
	//this does ui too... unclean but efficient

//~ oTimer = Time.createTimer()

	transform(contextQueue, objs, gens, -1/*nThresholdBeforeDraw*/, g_nShapesToDraw, nAdjustX);
//~ alert(oTimer.check())	
	bIsRendering=false;
}

function clearAll()
{
	//apparently can't iterate over objs, so we had to hold on...
	//remove it all...
	for (var i=0;i<g_allLines.length;i++)
		g_allLines[i].remove()
	g_allLines=[]
	removeArrows()
	
	//remove rendered
	render_hideAllShapes()
	renderAllLines(false)
	
	domToCoordObject = {}
	domSelected = null
	g_idFirstShape = -1
	
	g_zoomLevel = false; //means no zoom, equivalent to 1.0
	g_nShapesToDraw = 300;
	g_nJustPerimeter = 0; //draw just perimeter. trying with a small circle doesn't work as well as this.
		
	hideSelect()
}
function saveToJson()
{
	if (!JSON) {alerd('Could not find JSON object. Try using latest Firefox or Chrome.'); return; }
	//must strip out functions...
	
	var arRes = []
	for (var objId in domToCoordObject)
	{
		var newobj = {}
		for (var key in domToCoordObject[objId])
			if (typeof domToCoordObject[objId][key]!='function')
				newobj[key] = domToCoordObject[objId][key]
		
		arRes.push(newobj)
	}
	// we ignore basis mainContextShape when loading, but it might be good to save, for posterity
	//~ var objTop = {vn:'0.1', nshapes:g_nShapesToDraw, pm:g_nJustPerimeter, shapes:arRes, basis:mainContextShape}
	var objTop = {vn:'0.1', nshapes:g_nShapesToDraw, pm:g_nJustPerimeter, shapes:arRes}
	//in a url, dont waste chars by saving basis...
	//zm:g_zoomLevel -- don't save zoom level
	var s= JSON.stringify(objTop)
	s = s.replace(/"/g,'')
	s=s.replace(/x1/g, 'X').replace(/x2/g, 'A').replace(/y1/g, 'Y').replace(/y2/g, 'B')
	s = s.replace(/:/g, '=')
	return s
}

function onSave()
{
	var s = saveToJson()
	var fsturl = document.location.href.split('?')[0];
	prompt('share this link!', fsturl+'?'+s)
}

function onloadJson()
{
	var sOldJson = saveToJson()
	var openD = prompt('Save diagram by copying string into text document. Load new diagram by pasting new string here (no newlines):', sOldJson)
	if (openD && openD!= sOldJson && confirm('delete existing and load new?'))
		loadJson(openD)
}
function loadJson(s)
{
		// ab{1{a {b .A. anApple banA
		
		s = s.replace(/=/g, ':')
		s = s.replace(/({)([a-zA-Z])/g, '$1"$2')
		s = s.replace(/(:)([a-zA-Z])/g, '$1"$2')
		s = s.replace(/(,)([a-zA-Z])/g, '$1"$2')
		s = s.replace(/([a-zA-Z])(,)/g, '$1"$2')
		s = s.replace(/([a-zA-Z])(:)/g, '$1"$2')
		s = s.replace(/([a-zA-Z])(})/g, '$1"$2')
		s=s.replace(/\bA\b/g, 'x2').replace(/\bB\b/g, 'y2').replace(/\bX\b/g, 'x1').replace(/\bY\b/g, 'y1')
		
		
		try {
			var obj = JSON.parse(s)
		}
		catch(err) {
			alerd('could not parse')
			return;
		}
		
		if (!obj['shapes']) { alerd('Could not find shapes.'); return; }
		clearAll()
		
		var version = obj['vn']
		if (obj['nshapes']) g_nShapesToDraw = parseInt(obj['nshapes'])
		if (obj['pm']) g_nJustPerimeter = parseInt(obj['pm'])
		if (!obj['shapes']) { alerd('Could not find shapes.'); return; }
		var shapes = obj['shapes']
		
		//~ return;
		for (var i=0; i<shapes.length; i++)
		{
			var ocoord = new CRawShape(shapes[i])
			createNew(ocoord.type)
			domToCoordObject[domSelected.id] = ocoord
			
			refreshShape(domSelected)
		}
		domSelected = null
		hideSelect()
		
		doTransformRender();
}

function onSetCoords()
{
	//manually sets coords
	if (!domSelected) return;
	var shp = domToCoordObject[domSelected.id]
	var newxy = prompt('Move shape by (x,y)', '0,0')
	if (!newxy || newxy.split(',').length!=2) return;
	var x = parseFloat(newxy.split(',')[0]) * mainContext.length
	var y = parseFloat(newxy.split(',')[1]) * mainContext.length
	if (isNaN(x) || isNaN(y))
		return;
	shp.x1 += x; shp.x2 += x; shp.y1+=y; shp.y2+=y;
	refreshShape(domSelected)
	
	//reuse code, even if it's a bit unclear
	var newrelshape=  rawShapeToRelativeShape(mainContext , shp)
	var newrl = prompt('Write new (angle in degrees, length)', newrelshape.rotation+','+ newrelshape.length)
	if (!newrl || newrl.split(',').length!=2) return;
	var r = parseFloat(newrl.split(',')[0])
	var l = parseFloat(newrl.split(',')[1])
	if (isNaN(r) || isNaN(l))
		return;
	newrelshape.rotation=r; newrelshape.length=l;
	var shpResult = new CRawShape();
	drawShapeRelativeToContext(mainContext, newrelshape, shpResult)
	shp.x2=shpResult.x2;shp.y2=shpResult.y2;shp.rx=shpResult.rx
	
	refreshShape(domSelected)
}
function onZoom(nDir)
{
	if (!g_zoomLevel) g_zoomLevel = 1.0; //being false means 1.0
	if (nDir==1) g_zoomLevel *= 1.25;
	else if (nDir==-1) g_zoomLevel /= 1.25;
	doTransformRender();
}

function onOnlyPerim()
{
	if (g_nJustPerimeter > 0)
	{
		g_nJustPerimeter = 0;
		doTransformRender();
	}
	else
	{
		g_nJustPerimeter = 250;
		g_nShapesToDraw=Math.min(g_nShapesToDraw,500);
		doTransformRender();
	}
}
