//drawdraw, Ben Fisher, 2010. released under the GPLv3

var r = null;
var domSelected = null;

// also records what shapes have been created.
var domToCoordObject = {};

// g_zoomLevel of false is equivalent to 1.0
var g_zoomLevel = false;

// number of shapes to draw, can be adjusted by user
var g_nShapesToDraw = 300;

// this option can be enabled by the user, where we'll draw just the perimeter.
var g_nJustPerimeter = 0; 

// the arrow-head lines for lgen shapes
var g_mapObjIdToArrow = {};

// compensate for devicePixelRatio
var g_resizeFactor = 1;
	
// holding dom objects is not ideal... we're past the the days of ie memory leaks though.
var g_shapeSelectA = null;
var g_shapeSelectB = null;
	
var mainContextShape = null, mainContext = null
var g_allLines = [];


function setup()
{
	r = Raphael("holder");
	
	// adjust selection handle size on screens with high dpi
	g_resizeFactor = 1
	if (window.devicePixelRatio !== undefined)
	{
		g_resizeFactor *= window.devicePixelRatio
	}
	if (window.devicePixelRatio !== undefined && window.devicePixelRatio > 1)
	{
		// if the devicePixelRatio is more than one, make it even bigger even after compensating
		g_resizeFactor *= 4
	}
	
	// draw selection handles
	var handlesize = 4 * g_resizeFactor
	g_shapeSelectA = r.ellipse(1, 1, handlesize, handlesize).attr({"stroke-width": 1});
	g_shapeSelectB = r.ellipse(1, 1, handlesize, handlesize).attr({"stroke-width": 1});
	g_shapeSelectA.attr({fill: "#0f0", opacity: 1});
	g_shapeSelectB.attr({fill: "#0f0", opacity: 1});
	
	g_shapeSelectA.drag(onDragResize_move, onDragResize_start, onDragResize_up);
	g_shapeSelectB.drag(onDragResize_move, onDragResize_start, onDragResize_up);
	
	var showReference = false
	
	// draw reference context. not a selectable object.
	if (g_resizeFactor > 1)
	{
		mainContextShape = new CRawShape({type: 'lgen', x1: 40, x2: 40, y1: 200, y2: 100});
	}
	else
	{
		mainContextShape = new CRawShape({type: 'lgen', x1: 200, x2: 200, y1: 200, y2: 100});
	}
	
	mainContext = contextFromRawShape(mainContextShape);
	var mainGenPath = r.path('M1,1,L,1,1' ).attr({'stroke-width':6 * g_resizeFactor}) 
	updatePath(mainGenPath, mainContextShape)
	mainGenPath.attr( {stroke: '#888'}); 
		
	if (showReference)
	{
		// manually draw the main arrow reference. this is not adjustable by the user.
		var mainArrow = drawArrow(mainGenPath, mainContextShape, true)
		mainArrow.attr( {stroke: '#aaf', fill:'#aaf'});
	}
	
	// create a normal line, as an example
	createNew('l');
	ocoord = domToCoordObject[domSelected.id];
	ocoord.x1 = mainContextShape.x1;
	ocoord.y1 = mainContextShape.y1;
	ocoord.x2 = mainContextShape.x2;
	ocoord.y2 = mainContextShape.y2;
	refreshShape(domSelected);

	// create a normal lgen, as an example
	createNew('lgen');
	ocoord = domToCoordObject[domSelected.id];
	ocoord.x1 = mainContextShape.x1;
	ocoord.y1 = mainContextShape.y1;
	ocoord.x2 = mainContextShape.x2 + 40;
	ocoord.y2 = mainContextShape.y2 + 50;
	refreshShape(domSelected);
	showSelect();
	
	// load everything important before loading json, so that if parsing json fails, we still have usable app.
	var surlparts = document.location.href.split('?')
	if (surlparts.length==2 && surlparts[1].indexOf('{') != -1)
	{
		// load the url
		loadJson(surlparts[1])
	}
}

function refreshShape(domobj)
{
	var ocoord = domToCoordObject[domobj.id];
	updatePath(domobj, ocoord); 
	
	if (ocoord.type == 'lgen')
	{
		drawArrow(domobj, ocoord, false)
	}

	if (domSelected)
	{
		showSelect()
	}
}

var onDragResize_start = function ()
{
	// storing original coordinates
	this.ox = this.attr("cx");
	this.oy = this.attr("cy");
	var ocoord = domToCoordObject[domSelected.id];
	if (ocoord.type == 'c' && this === g_shapeSelectB) 
	{
		this.origRadius = ocoord.rx;
	}
	
	this.countRefresh = 0;
}

var onDragResize_move = function (dx, dy)
{
	var ocoord = domToCoordObject[domSelected.id];
	if (!ocoord)
	{
		alerd('no ocoord obj');
		return;
	}
	
	if (ocoord.type == 'c' && this === g_shapeSelectB)
	{
		dy = 0;
	}
	
	if (ocoord.type == 'c' && dx + this.origRadius < 1)
	{
		//prevent making a circle with < 1 radius
		dx= -this.origRadius + 1;
	}
	
	this.attr({cx: this.ox + dx, cy: this.oy + dy});

	if (this != g_shapeSelectA && this != g_shapeSelectB)
	{
		alerd('unknown resizer?');
		return;
	}
	
	if (ocoord.type.startsWith('l')) // line
	{
		if (this === g_shapeSelectA)
		{
			ocoord.x1 = this.ox + dx;
			ocoord.y1 = this.oy + dy;
		}
		else if (this === g_shapeSelectB)
		{
			ocoord.x2 = this.ox + dx;
			ocoord.y2 = this.oy + dy;
		}
	}
	else if (ocoord.type=='c') // circle
	{
		if (this === g_shapeSelectA)
		{
			//moving the center
			ocoord.x1 = this.ox + dx;
			ocoord.y1 = this.oy + dy;
			
			//move other resizer accordingly.
			g_shapeSelectB.attr({ cx: ocoord.x1  + ocoord.rx, cy: ocoord.y1});
		}
		else if (this === g_shapeSelectB)
		{
			// changing the radius!
			ocoord.rx = dx + this.origRadius;
		}
	}
	
	refreshShape(domSelected);

	this.countRefresh++;
	if (this.countRefresh > 1)
	{
		this.countRefresh = 0;
	}
	
	if (this.countRefresh == 0)
	{
		doTransformRender();
	}
}

var onDragResize_up = function()
{
	// redraw shapes.
	doTransformRender()
}

function createNew(stype)
{
	if (stype=='l' || stype=='lgen')
	{
		var newLine = r.path('M1,1,L,1,1' );
		var newO = new CRawShape({type: 'l',x1: mainContextShape.x2, 
			y1: mainContextShape.y2, x2: mainContextShape.x2+30, y2: mainContextShape.y2} );
		newO.type = stype;
		if (stype == 'lgen')
		{
			newLine.attr({stroke:'#922', opacity:1.0})
		}
	}
	else if (stype == 'c')
	{
		var newLine = r.circle(1, 1, 1)
		var newO = new CRawShape({type: 'c', x1: mainContextShape.x2, y1: mainContextShape.y2, rx: 20});
	}
	
	g_allLines.push(newLine)
	domToCoordObject[newLine.id] = newO;
	
	// draw the shape at its initial position
	refreshShape(newLine);
	newLine.mousedown(onMouseDownSelectIt);
	newLine.attr({"stroke-width": 4 * g_resizeFactor});
	domSelected = newLine;
	showSelect();
	
	// raphael.js workaround for rendering bug in Safari.
	r.safari();
	return newLine;
}

function showSelect()
{
	g_shapeSelectA.show();
	g_shapeSelectB.show();
	var ocoord = domToCoordObject[domSelected.id];
	if (!ocoord)
	{
		alerd('error in showSelect');
		return; 
	}

	if (ocoord.type.startsWith('l'))
	{
		g_shapeSelectA.attr( {cx: ocoord.x1, cy: ocoord.y1})
		g_shapeSelectB.attr( {cx: ocoord.x2, cy: ocoord.y2})
	}
	else if (ocoord.type=='c')
	{
		g_shapeSelectA.attr( {cx: ocoord.x1, cy: ocoord.y1})
		g_shapeSelectB.attr( {cx: ocoord.x1 + ocoord.rx, cy: ocoord.y1})
	}
	
	// g_shapeSelectB should be in front, so that circles aren't stuck small
	g_shapeSelectA.toFront();
	g_shapeSelectB.toFront();
	
	// raphael.js workaround for rendering bug in Safari.
	r.safari();
}

function hideSelect()
{
	g_shapeSelectA.hide();
	g_shapeSelectB.hide();
}

function deleteSelected()
{
	if (!domSelected)
	{
		return;
	}
	
	if (domToCoordObject[domSelected.id].type == 'lgen')
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
	if (domSelected)
	{
		domToCoordObject[domSelected.id].visible = !domToCoordObject[domSelected.id].visible;
		refreshShape(domSelected)
	}
}

function onMouseDownSelectIt(event)
{
	if (this)
	{
		domSelected = this;
		showSelect();
	}
}
	
var bIsRendering = false;
var debugMeasureTiming = false;
function doTransformRender()
{
	if (bIsRendering)
	{
		// prevent reentrance
		return;
	}
	
	bIsRendering = true;
	
	var gens = [];
	var objs = [];
	for (var objId in domToCoordObject)
	{
		if (domToCoordObject[objId].type == 'lgen')
		{
			gens.push(domToCoordObject[objId])
		}
		else if (domToCoordObject[objId].x1 !== undefined)
		{
			objs.push(domToCoordObject[objId])
		}
		else
		{
			alerd('warning: something else in map')
		}
	}
	
	//make main reference into a context
	var initialContext = mainContext;
	
	//convert shapes to be relative to initial context
	for (var i = 0; i < gens.length; i++)
	{
		gens[i] = rawShapeToRelativeShape(initialContext, gens[i])
	}
	
	for (var i = 0; i < objs.length; i++)
	{
		objs[i] = rawShapeToRelativeShape(initialContext, objs[i])
	}
	
	var contextQueue = [initialContext]
	var nAdjustX = 300

	if (debugMeasureTiming)
	{
		oTimer = Time.createTimer()
	}
	
	// it's a bit unclean that we do both the computation and the ui here
	transform(contextQueue, objs, gens, -1/*nThresholdBeforeDraw*/, g_nShapesToDraw, nAdjustX);
	
	if (debugMeasureTiming)
	{
		alert(oTimer.check())	
	}
	
	bIsRendering = false;
}

function clearAll()
{
	// remove it all...
	for (var i = 0; i < g_allLines.length; i++)
	{
		g_allLines[i].remove()
	}
	
	g_allLines = []
	removeArrows()
	
	// remove rendered
	render_hideAllShapes()
	renderAllLines(false)
	
	domToCoordObject = {}
	domSelected = null
	g_idFirstShape = -1
	g_zoomLevel = false; // means no zoom, equivalent to 1.0
	g_nShapesToDraw = 300;
	g_nJustPerimeter = 0; // draw just perimeter. trying with a small circle doesn't work as well as this.
	hideSelect()
}

function saveToJson()
{
	if (!JSON)
	{
		alerd('Could not find JSON object. Try using latest Firefox or Chrome.');
		return;
	}
	
	var arRes = []
	for (var objId in domToCoordObject)
	{
		// first, strip out functions
		var newobj = {}
		for (var key in domToCoordObject[objId])
		{
			if (typeof domToCoordObject[objId][key] != 'function')
			{
				newobj[key] = domToCoordObject[objId][key]
			}
		}
		
		arRes.push(newobj)
	}
	
	// in the future, it might be interesting to save the basis mainContextShape.
	// for now we don't need it because it doesn't change. and we don't want to add extra characters
	// same with the zoomlevel... fewer characters the better
	var objTop = {vn:'0.1', nshapes:g_nShapesToDraw, pm:g_nJustPerimeter, shapes:arRes}
	
	var s= JSON.stringify(objTop)
	s = s.replace(/"/g,'')
	s = s.replace(/x1/g, 'X').replace(/x2/g, 'A').replace(/y1/g, 'Y').replace(/y2/g, 'B')
	s = s.replace(/:/g, '=')
	return s
}

function onSave()
{
	var s = saveToJson()
	var fsturl = document.location.href.split('?')[0];
	prompt('share this link!', fsturl + '?' + s)
}

function onloadJson()
{
	var sOldJson = saveToJson()
	var openD = prompt('Save diagram by copying string into text document. Load new diagram by pasting new string here (no newlines):', sOldJson)
	if (openD && openD!= sOldJson && confirm('delete existing and load new?'))
	{
		loadJson(openD)
	}
}

function loadJson(s)
{
	// we'll use a few search/replace tricks to "condense" the json.
	// for example:
	// ab{1{a {b .A. anApple banA
	
	s = s.replace(/=/g, ':')
	s = s.replace(/({)([a-zA-Z])/g, '$1"$2')
	s = s.replace(/(:)([a-zA-Z])/g, '$1"$2')
	s = s.replace(/(,)([a-zA-Z])/g, '$1"$2')
	s = s.replace(/([a-zA-Z])(,)/g, '$1"$2')
	s = s.replace(/([a-zA-Z])(:)/g, '$1"$2')
	s = s.replace(/([a-zA-Z])(})/g, '$1"$2')
	s = s.replace(/\bA\b/g, 'x2').replace(/\bB\b/g, 'y2').replace(/\bX\b/g, 'x1').replace(/\bY\b/g, 'y1')
	
	try
	{
		var obj = JSON.parse(s)
	}
	catch(err)
	{
		alerd('could not parse')
		return;
	}
	
	if (!obj['shapes'])
	{
		alerd('Could not find shapes.');
		return;
	}
	
	clearAll()
	var version = obj['vn']
	if (obj['nshapes'])
	{
		g_nShapesToDraw = parseInt(obj['nshapes'])
	}
	
	if (obj['pm'])
	{
		g_nJustPerimeter = parseInt(obj['pm'])
	}
	
	var shapes = obj['shapes']
	for (var i = 0; i < shapes.length; i++)
	{
		var ocoord = new CRawShape(shapes[i])
		createNew(ocoord.type)
		domToCoordObject[domSelected.id] = ocoord
		refreshShape(domSelected)
	}
	
	domSelected = null;
	hideSelect();
	doTransformRender();
}

function onSetCoords()
{
	// manually sets coordinates that the user provides
	if (!domSelected)
	{
		return;
	}
	
	var shp = domToCoordObject[domSelected.id]
	var newxy = prompt('Move shape by (x,y)', '0,0')
	if (!newxy || newxy.split(',').length != 2)
	{
		return;
	}
	
	var x = parseFloat(newxy.split(',')[0]) * mainContext.length
	var y = parseFloat(newxy.split(',')[1]) * mainContext.length
	if (isNaN(x) || isNaN(y))
	{
		return;
	}
	
	shp.x1 += x;
	shp.x2 += x;
	shp.y1 += y;
	shp.y2 += y;
	refreshShape(domSelected)
	
	// reuse code, even if it's a bit unclear
	var newrelshape = rawShapeToRelativeShape(mainContext, shp)
	var newrl = prompt('Write new (angle in degrees, length)',
		newrelshape.rotation + ',' + newrelshape.length)
	
	if (!newrl || newrl.split(',').length!=2)
	{ 
		return;
	}
	
	var r = parseFloat(newrl.split(',')[0])
	var l = parseFloat(newrl.split(',')[1])
	if (isNaN(r) || isNaN(l))
	{
		return;
	}
	
	newrelshape.rotation = r;
	newrelshape.length = l;
	var shpResult = new CRawShape();
	drawShapeRelativeToContext(mainContext, newrelshape, shpResult);
	shp.x2 = shpResult.x2;
	shp.y2 = shpResult.y2;
	shp.rx = shpResult.rx;
	
	refreshShape(domSelected)
}

function onZoom(nDir)
{
	if (!g_zoomLevel)
	{
		g_zoomLevel = 1.0; // if it's not set, it's 1.0
	}
	
	if (nDir == 1)
	{
		g_zoomLevel *= 1.25;
	}
	else if (nDir == -1)
	{
		g_zoomLevel /= 1.25;
	}
	
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
		g_nShapesToDraw = Math.min(g_nShapesToDraw,500);
		doTransformRender();
	}
}
