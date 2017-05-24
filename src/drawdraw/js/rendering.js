// drawdraw, Ben Fisher, 2010. released under the GPLv3

// draws only the path.
// colors, arrows, and any ornamentation should be drawn elsewhere. (e.g. top.js/refreshShape())
function updatePath(domobj, rawShape)
{
	if (rawShape.type=='l' || rawShape.type=='lgen') 
	{
		var sNewPath = 'M' + rawShape.x1.toString() + ',' + rawShape.y1.toString() +
			',L' + rawShape.x2.toString() + ',' + rawShape.y2.toString();
		domobj.attr('path',sNewPath);
	}
	else if (rawShape.type=='c')
	{
		domobj.attr({cx:rawShape.x1,cy:rawShape.y1, r:rawShape.rx});
	}
	else
	{
		alerd('error: unknown shapetype');
	}
}

var grcpoolCircles = []
var ngrcpoolCircles = -1
function render_hideAllShapes()
{
	for (var i=0; i < grcpoolCircles.length; i++) 
	{
		grcpoolCircles[i].hide()
	}
	
	ngrcpool = -1;
	ngrcpoolCircles = -1;
}

function renderCircle(rawShape)
{
	ngrcpoolCircles++;
	if (ngrcpoolCircles >= grcpoolCircles.length)
	{
		grcpoolCircles.push(r.circle(1,1,1));
	}
	
	grcpoolCircles[ngrcpoolCircles].show()
	updatePath(grcpoolCircles[ngrcpoolCircles], rawShape)
}

var g_oneLineGraphic = null;
function renderAllLines(arResults)
{
	if (!g_oneLineGraphic)
	{
		g_oneLineGraphic = r.path('M1,1,L,1,1' );
	}
	
	if (!arResults)
	{
		g_oneLineGraphic.attr( 'path', 'M1,1,L,1,1' );
	}
	else
	{
		var allpath = arResults.join(' ')
		g_oneLineGraphic.attr('path', allpath)
	}
}

function drawArrow(domobj, rawShape, noAdd)
{
	// credit: http://taitems.tumblr.com/post/549973287/drawing-arrows-in-raphaeljs
	if (!g_mapObjIdToArrow[domobj.id])
	{
		g_mapObjIdToArrow[domobj.id] = r.path('M1,1,L,1,1' );
	}
	
	var arsize = 6 * g_resizeFactor;
	var anglerad = Math.atan2(rawShape.x1 - rawShape.x2, rawShape.y2 - rawShape.y1);
	angle = (anglerad / (2 * Math.PI)) * 360;
	var locx = rawShape.x2// + 4 * g_resizeFactor * Math.cos(anglerad)
	var locy = rawShape.y2// + 4 * g_resizeFactor * Math.sin(anglerad)
	g_mapObjIdToArrow[domobj.id].attr({path:
		"M" + locx + " " + locy + " L" + (locx - arsize) + " " + (locy - arsize) +
		" L" + (locx - arsize) + " " + (locy + arsize) + " L" + locx + " " + locy });
		
	g_mapObjIdToArrow[domobj.id].attr("fill", "#922").attr("stroke", "#922").rotate((90+angle), locx, locy);
	g_mapObjIdToArrow[domobj.id].attr('stroke-width', g_resizeFactor);
	
	//move arrow head behind the line
	g_mapObjIdToArrow[domobj.id].toBack(); 
	if (noAdd)
	{
		// don't add to dict
		var ret = g_mapObjIdToArrow[domobj.id];
		delete g_mapObjIdToArrow[domobj.id];
		return ret
	}
	else
	{
		return g_mapObjIdToArrow[domobj.id]
	}
}

function removeArrows()
{
	for (var key in g_mapObjIdToArrow)
	{
		g_mapObjIdToArrow[key].remove()
	}
	
	g_mapObjIdToArrow = {}
}
