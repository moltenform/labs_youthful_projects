
/**
 * Ben Fisher, 2010
 * @license GNU General Public License version 3
 * https://www.gnu.org/licenses/gpl-3.0.txt
 * https://github.com/moltenjs/labs_youthful_projects
 */

function on_btnaddline(e) { 
	createNew('l')
    doTransformRender()
}

function on_btnaddcircle(e) { 
	createNew('c')
    doTransformRender()
}

function on_btnaddgen(e) { 
	createNew('lgen'); 
    doTransformRender()
}

function on_btntheta(e) { 
	// manually sets coordinates that the user provides
	if (!domSelected)
	{
		alerd('nothing is selected')
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
    doTransformRender();
}

function on_btndelete(e) { 
	deleteSelected();
    doTransformRender();
}

function on_btnonlyperim(e) { 
    if (g_classesglobal.nJustPerimeter > 0)
	{
		g_classesglobal.nJustPerimeter = 0;
		doTransformRender();
	}
	else
	{
		g_classesglobal.nJustPerimeter = 250;
		g_classesglobal.nShapesToDraw = Math.min(g_classesglobal.nShapesToDraw,500);
		doTransformRender();
	}
}

function on_btndrawmore(e) { 
	g_classesglobal.nShapesToDraw+=25;
    doTransformRender()
}

function on_btndrawless(e) { 
	g_classesglobal.nShapesToDraw-=25;
    doTransformRender()
}

function on_btnzoomin(e) { 
	on_btnzoom(1)
}

function on_btnzoomout(e) { 
	on_btnzoom(-1)
}

function on_btnzoomout(nDir) { 
	if (nDir == 1)
	{
		g_classesglobal.zoomLevel *= 1.25;
	}
	else if (nDir == -1)
	{
		g_classesglobal.zoomLevel /= 1.25;
	}
	
	doTransformRender();
}

function on_btnopenexample(e) { 
	onLoadExample()
}

function on_btnsave(e) { 
	
}

// ui mouse events

var onDragResize_start = function ()
{
	// storing original coordinates
	this.ox = this.attr("cx");
	this.oy = this.attr("cy");
	var oCoord = g_ui.domToCoordObject[domSelected.id];
	if (oCoord.type == 'c' && this === self.shapeSelectB) 
	{
		this.origRadius = oCoord.rx;
	}
	
	this.countRefresh = 0;
}

var onDragResize_move = function (dx, dy)
{
	var oCoord = g_ui.domToCoordObject[domSelected.id];
	if (!oCoord)
	{
		alerd('no oCoord obj');
		return;
	}
	
	if (oCoord.type == 'c' && this === self.shapeSelectB)
	{
		dy = 0;
	}
	
	if (oCoord.type == 'c' && dx + this.origRadius < 1)
	{
		//prevent making a circle with < 1 radius
		dx= -this.origRadius + 1;
	}
	
	this.attr({cx: this.ox + dx, cy: this.oy + dy});

	if (this != self.shapeSelectA && this != self.shapeSelectB)
	{
		alerd('unknown resizer?');
		return;
	}
	
	if (oCoord.type.startsWith('l')) // line
	{
		if (this === self.shapeSelectA)
		{
			oCoord.x1 = this.ox + dx;
			oCoord.y1 = this.oy + dy;
		}
		else if (this === self.shapeSelectB)
		{
			oCoord.x2 = this.ox + dx;
			oCoord.y2 = this.oy + dy;
		}
	}
	else if (oCoord.type=='c') // circle
	{
		if (this === self.shapeSelectA)
		{
			//moving the center
			oCoord.x1 = this.ox + dx;
			oCoord.y1 = this.oy + dy;
			
			//move other resizer accordingly.
			self.shapeSelectB.attr({ cx: oCoord.x1  + oCoord.rx, cy: oCoord.y1});
		}
		else if (this === self.shapeSelectB)
		{
			// changing the radius!
			oCoord.rx = dx + this.origRadius;
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

