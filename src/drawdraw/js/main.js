// drawdraw, Ben Fisher, 2010. released under the GPLv3

// a 'context' is a frame of reference. it specifies what is our current 'rotation'. what is the current 'length'
function CContext()
{
	this.startx = 0.0
	this.starty = 0.0
	this.rotation = 0.0 // degrees. not used for circles.
	this.length = 0.0 // for circles, radius size
}

// a shape, relative to a context. requires a context in order to be drawn. 
function CRelativeShape()
{
	this.type = 'l'
	this.angleToStart = 0.0 // angle from (start pt of context) to (start pt of shape)
	this.lengthToStart = 0.0 // distance from (start pt of context) to (start pt of shape)
	this.rotation = 0.0 // degrees. meaningless for circles.
	this.length = 0.0 // for circles, radius size
}

function getNextContext(context, relativeShape, outContext)
{
	if (relativeShape.type != 'lgen')
	{
		alerd('only linegen should produce next context.');
		return;
	}
	
	outContext.startx = context.startx + context.length * relativeShape.lengthToStart * 
		degcos(context.rotation+relativeShape.angleToStart)
	outContext.starty = context.starty + context.length * relativeShape.lengthToStart * 
		degsin(context.rotation+relativeShape.angleToStart)
	outContext.rotation = context.rotation + relativeShape.rotation
	outContext.length = context.length * relativeShape.length
	return outContext
}

function drawShapeRelativeToContext(context, relativeShape, outRawShape)
{	
	var outstartx = context.startx + context.length * relativeShape.lengthToStart * 
		degcos(context.rotation+relativeShape.angleToStart)
	var outstarty = context.starty + context.length * relativeShape.lengthToStart * 
		degsin(context.rotation+relativeShape.angleToStart)
	var outrotation = context.rotation +relativeShape.rotation
	var outlength = context.length * relativeShape.length
	
	outRawShape.type = relativeShape.type
	outRawShape.x1 = outstartx
	outRawShape.y1 = outstarty

	 //usually just lines are drawn, but allow linegen because of onsetCoords
	if (relativeShape.type.startsWith('l'))
	{
		outRawShape.x2 = outstartx + outlength * degcos(outrotation)
		outRawShape.y2 = outstarty + outlength * degsin(outrotation)
	}
	else if (relativeShape.type=='c')
	{
		outRawShape.rx = outlength
	}

	return outRawShape
}

function rawShapeToRelativeShape(context, rawShape)
{
	var newshape = new CRelativeShape()
	newshape.type = rawShape.type
	newshape.angleToStart = context.rotation + deg(Math.atan2(context.starty-rawShape.y1, context.startx-rawShape.x1))
	newshape.lengthToStart = Math.sqrt((context.startx-rawShape.x1) * (context.startx-rawShape.x1) +
		(context.starty-rawShape.y1) * (context.starty-rawShape.y1)) / context.length
	
	if (rawShape.type=='c')
	{
		newshape.rotation = 0.0
		newshape.length = rawShape.rx / context.length
	}
	else if (rawShape.type.startsWith('l'))
	{
		newshape.rotation =  -context.rotation + deg(Math.atan2(rawShape.y2-rawShape.y1, rawShape.x2-rawShape.x1))
		newshape.length = Math.sqrt((rawShape.x2-rawShape.x1) * (rawShape.x2-rawShape.x1)+
			(rawShape.y2-rawShape.y1) * (rawShape.y2-rawShape.y1)) / context.length
	}
	
	return newshape
}

function transform(contextQueue, relativeShapes, relativeGenerators, nThresholdBeforeDraw, nShapeLimit, adjustX)
{
	var currentRawShape = new CRawShape()
	var arResults = []
	var rawShapes = []
	var rawShapesCircles = []
	var nDrawn = 0
	
	render_hideAllShapes()
	renderAllLines(false);
	if (relativeGenerators.length == 0)
	{
		// no work to do.
		renderAllLines(false);
		return;
	}
	
	if (relativeShapes.length == 0)
	{
		// avoid infinite loop
		renderAllLines(false);
		return;
	} 
	
	var nGeneration=0
	var nTargetGeneration = nShapeLimit / 25;
	while (true)
	{
		var context = contextQueue.shift();
		
		//draw all of the shapes
		for (var i = 0; i < relativeShapes.length; i++) //we've already filtered out the invisible ones.
		{
			nDrawn++;
			if (nDrawn > g_nJustPerimeter)
			{
				drawShapeRelativeToContext(context, relativeShapes[i], currentRawShape)
				
				if (g_zoomLevel)
				{
					//center of it is 200,150
					currentRawShape.x1 = ((currentRawShape.x1 - 200) * g_zoomLevel) + 200;
					currentRawShape.x2 = ((currentRawShape.x2 - 200) * g_zoomLevel) + 200;
					currentRawShape.y1 = ((currentRawShape.y1 - 150) * g_zoomLevel) + 150;
					currentRawShape.y2 = ((currentRawShape.y2 - 150) * g_zoomLevel) + 150;
					currentRawShape.rx *= g_zoomLevel
					
				}
				
				currentRawShape.x1 += adjustX
				currentRawShape.x2 += adjustX
				
				if (currentRawShape.type=='c')
				{
					renderCircle(currentRawShape);
				}
				else
				{
					arResults.push('M');
					arResults.push(currentRawShape.x1);
					arResults.push(currentRawShape.y1);
					arResults.push('L');
					arResults.push(currentRawShape.x2);
					arResults.push(currentRawShape.y2);
				}
			}
			
			if (nDrawn > nShapeLimit)
			{
				renderAllLines(arResults);
				return;
			}
		}
		
		//add next contexts to the queue
		for (var i = 0; i < relativeGenerators.length; i++)
		{
			// todo: consider pulling from a pool instead instead of allocating.
			var nextcontext = new CContext()
			getNextContext( context, relativeGenerators[i], nextcontext)
			contextQueue.push(nextcontext)
		}
		
		nGeneration++;
	}
	
	// unreached
}

rad = function(a)
{
	return (a % 360) * Math.PI / 180
}

deg = function(a)
{
	return (a * 180 / Math.PI) % 360
}

function degcos(a)
{
	return Math.cos((a / 360.0) * 2 * Math.PI);
}

function degsin(a)
{
	return Math.sin((a / 360.0) * 2 * Math.PI); 
}

// raw shape object. used in ui frontend and returned by output.
// type is mandatory.
function CRawShape(obj)
{
	this.type = 'l';
	this.x1 = 0;
	this.x2 = 0;
	this.y1 = 0;
	this.y2 = 0;
	this.rx = 0;
	
	// set attributes based on incoming dict.
	if (obj)
	{
		if (!obj.type)
		{
			alerd('must pass type to rawShape constructor')
		}
		
		if (!obj.x1 || !obj.y1)
		{
			alerd(debugprint(obj));
			alerd('must pass coords to rawShape constructor')
		}
		
		for (var key in obj)
		{
			this[key] = obj[key]
		}
	}
}

function contextFromRawShape(rawShape)
{
	if (!rawShape.type.startsWith('l'))
	{
		alerd('can only be done for lines')
	}
	
	var context = new CContext()
	context.startx = rawShape.x1;
	context.starty = rawShape.y1;
	context.length = Math.sqrt((rawShape.x2 - rawShape.x1) * (rawShape.x2 - rawShape.x1) + 
		(rawShape.y2 - rawShape.y1) * (rawShape.y2 - rawShape.y1))
	context.rotation = deg(Math.atan2(rawShape.y2 - rawShape.y1, rawShape.x2 - rawShape.x1));
	return context
}
