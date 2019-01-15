
/**
 * Ben Fisher, 2010
 * @license GNU General Public License version 3
 * https://www.gnu.org/licenses/gpl-3.0.txt
 * https://github.com/moltenjs/labs_youthful_projects
 */
 
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
	
	var centerx = 200, centery = 150
	var nGeneration=0
	var nTargetGeneration = nShapeLimit / 25;
	while (true)
	{
		var context = contextQueue.shift();
		
		//draw all of the shapes
		for (var i = 0; i < relativeShapes.length; i++) //we've already filtered out the invisible ones.
		{
			nDrawn++;
			if (nDrawn > g_classesglobal.nJustPerimeter)
			{
				drawShapeRelativeToContext(context, relativeShapes[i], currentRawShape)
				
				if (g_classesglobal.zoomLevel)
				{
					currentRawShape.x1 = ((currentRawShape.x1 - centerx) * g_classesglobal.zoomLevel) + centerx;
					currentRawShape.x2 = ((currentRawShape.x2 - centerx) * g_classesglobal.zoomLevel) + centerx;
					currentRawShape.y1 = ((currentRawShape.y1 - centery) * g_classesglobal.zoomLevel) + centery;
					currentRawShape.y2 = ((currentRawShape.y2 - centery) * g_classesglobal.zoomLevel) + centery;
					currentRawShape.rx *= g_classesglobal.zoomLevel
					
				}
				
				currentRawShape.x1 += adjustX
				currentRawShape.x2 += adjustX
				
				if (currentRawShape.type == 'c')
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

