
#include "coordsdiagram.h"
#include "common.h"

void screenPixelsToDouble(CoordsDiagramStruct * diagram, int mousex, int mousey, double *outX, double *outY)
{
	double dwidth = (*diagram->px1)-(*diagram->px0);
	double dheight = (*diagram->py1)-(*diagram->py0);
	*outX = (mousex-diagram->screen_x)/((double)diagram->screen_width)*dwidth + (*diagram->px0);
	*outY = ((diagram->screen_y+diagram->screen_height) - mousey)/((double)diagram->screen_height)*dheight + (*diagram->py0);
}
void doubleToScreenPixels(CoordsDiagramStruct * diagram, double fx, double fy, int *outX, int *outY)
{
	double dwidth = (*diagram->px1)-(*diagram->px0);
	double dheight = (*diagram->py1)-(*diagram->py0);
	*outX = (int)(diagram->screen_width * (fx- *diagram->px0)/dwidth + diagram->screen_x);
	*outY = diagram->screen_y+diagram->screen_height - (int)(diagram->screen_height * (fy- *diagram->py0)/dheight );
}

int isClickWithinDiagram(CoordsDiagramStruct diagrams[], int x, int y)
{
	int i=0;
	while(diagrams[i].px0 != NULL)
	{
		if (x > diagrams[i].screen_x && x < diagrams[i].screen_x+diagrams[i].screen_width &&
			y > diagrams[i].screen_y && y < diagrams[i].screen_y+diagrams[i].screen_height )
			return i;
		i++;
	}
	return -1;
}

int onClickTryZoom(CoordsDiagramStruct diagrams[],int direction, int x, int y)
{
	int i=isClickWithinDiagram(diagrams, x,y);
	if (i==-1) return -1;

	// zoom in on this one!
	double fmousex, fmousey;
	screenPixelsToDouble(&diagrams[i], x, y, &fmousex, &fmousey);
	double fwidth = (*diagrams[i].px1)-(*diagrams[i].px0);
	double fheight = (*diagrams[i].py1)-(*diagrams[i].py0);
	if (direction==-1) {fwidth *= 1.25; fheight*=1.25;}
	else {fwidth *= 0.8; fheight*=0.8;}
	setZoom(&diagrams[i], fmousex - fwidth/2, fmousex + fwidth/2, fmousey - fheight/2, fmousey + fheight/2);
	return i;
}

void setZoom(CoordsDiagramStruct * diagram, double x0,double x1,double y0,double y1)
{
	diagram->undox0 = *diagram->px0; diagram->undox1 = *diagram->px1;
	diagram->undoy0 = *diagram->py0; diagram->undoy1 = *diagram->py1;
	*diagram->px0 = x0; *diagram->px1 = x1; *diagram->py0 = y0; *diagram->py1 = y1;
}
void undoZoom(CoordsDiagramStruct * diagram)
{
	setZoom( diagram, diagram->undox0, diagram->undox1, diagram->undoy0, diagram->undoy1);
}


inline void plotpoint(SDL_Surface* pSurface, CoordsDiagramStruct * diagram, int px, int py)
{
	if (!(px >= diagram->screen_x && px < diagram->screen_x+diagram->screen_width &&
		py >= diagram->screen_y && py < diagram->screen_y+diagram->screen_height ))
			return;
  char* pPosition = ( char* ) pSurface->pixels ; //determine position
  pPosition += ( pSurface->pitch * py ); //offset by y
  pPosition += ( pSurface->format->BytesPerPixel * px ); //offset by x
  Uint32 newcol = 0x00ff0000; 
  memcpy ( pPosition , &newcol , pSurface->format->BytesPerPixel ) ;
}
void drawPlotGrid( SDL_Surface* pSurface, CoordsDiagramStruct * diagram, double mark1x, double mark1y, double mark2x, double mark2y ) 
{
	//find (0.0,0.0) in screen coords
	int xzero, yzero;
	doubleToScreenPixels(diagram, 0.0, 0.0, &xzero, &yzero);
	int PlotWidth = diagram->screen_width, PlotHeight = diagram->screen_height;
	int PlotX = diagram->screen_x, PlotY = diagram->screen_y;
	
	//draw vertical lines
	for (int y=PlotY; y<PlotY+PlotHeight; y++)
		plotpoint(pSurface,diagram, PlotX, y);
	for (int y=PlotY; y<PlotY+PlotHeight; y++)
		plotpoint(pSurface,diagram, PlotX+PlotWidth-1, y);
	if (xzero>PlotX && xzero<PlotX+PlotWidth) //y axis
		for (int y=PlotY; y<PlotY+PlotHeight; y++)
			plotpoint(pSurface,diagram, xzero, y);

	//draw horizontal lines
	for (int x=PlotX; x<PlotWidth+PlotX; x++)
		plotpoint(pSurface,diagram, x, PlotY);
	for (int x=PlotX; x<PlotWidth+PlotX; x++)
		plotpoint(pSurface,diagram, x, PlotY+PlotHeight-1);
	if (yzero>=PlotY&& yzero<=PlotY+PlotHeight) //x axis
		for (int x=PlotX; x<PlotWidth+PlotX; x++)
			plotpoint(pSurface,diagram, x, yzero);
	
	//draw tic marks at integers
	int xtic, ytic;
	for (double f=-4; f<5; f+=1.0)
	{
		doubleToScreenPixels(diagram, f,f, &xtic, &ytic);
			for (int y=yzero-5; y<yzero+5; y++)
				plotpoint(pSurface,diagram, xtic, y);
			for (int x=xzero-5; x<xzero+5; x++)
				plotpoint(pSurface,diagram, x, ytic);
	}

	// draw the markers
	doubleToScreenPixels(diagram, mark2x,mark2y, &xtic, &ytic);
	if (xtic>PlotX && xtic<PlotX+PlotWidth && ytic>PlotY && ytic<PlotY+PlotHeight)
		plotlineRectangle(pSurface,xtic-4,xtic+5,ytic-4,ytic+5, SDL_MapRGB(pSurface->format, 255,0,255));
	
	doubleToScreenPixels(diagram, mark1x,mark1y, &xtic, &ytic);
	if (xtic>PlotX && xtic<PlotX+PlotWidth && ytic>PlotY && ytic<PlotY+PlotHeight)
		plotlineRectangle(pSurface,xtic-4,xtic+5,ytic-4,ytic+5, SDL_MapRGB(pSurface->format, 255,0,0));

}

