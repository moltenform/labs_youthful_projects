#include "phaseportrait.h"
#include "whichmap.h"
#include "coordsdiagram.h"
#include "float_cast.h"
#include "palette.h"
#include "font.h"
#include "configfiles.h"
#include <math.h>
/*
see rev. 382 for a version that cached results. we chose not to do that anymore. 
Pros of caching: don't need to recompute same pixels repeatedly. faster.
Cons of caching: to be useful, needs to take lots of minutes.
	detracts from generalizeable 2d map studying (quickly trying different fn).
	takes up ram.
	often interested in one deep area, so cache doesn't help.
we should, though, consider caching the main diagram.
*/

// This file computes the "menagerie diagram", a plot in parameter space.
// there are two methods: computing lyapunov exponent, and counting pixels drawn in a phase portrait


__inline unsigned int standardToColors(SDL_Surface* pSurface, double valin, double estimatedMax)
{
	if (!(g_settings->drawingOptions & maskOptionsDiagramColoring)) {
		//rainbow colors
		double val = (valin) / (estimatedMax);
		if (val > estimatedMax) return SDL_MapRGB(pSurface->format, 50,0,0);
		val = ((valin) / (estimatedMax)) * 0.8 /*only use 0.0-0.8, because 0.99 looks close to 0.0*/;
		return HSL2RGB(pSurface, val, 1,.5);
	} else {
		//black, white, blue
		double val = (valin);
		if (val > (estimatedMax)) return SDL_MapRGB(pSurface->format, 50,0,0);
		if (val > (estimatedMax)*0.75)
		{
			val = (val-(estimatedMax)*0.75)/(estimatedMax*0.25);
			return SDL_MapRGB(pSurface->format,  255-(Uint8)lrint(val*255),255-(Uint8)lrint(val*255),(Uint8)255);
		}
		else
		{
			val = (val)/(estimatedMax*0.75);
			return SDL_MapRGB(pSurface->format, (Uint8) lrint(val*255),(Uint8)lrint(val*255),(Uint8)lrint(val*255));
		}

	}
}


//keep going until find one that goes the whole ways without escaping.
//todo: it'd be better to try seedx/seedy points that are not close to the previous tried...
int countPhasePlotLyapunov(SDL_Surface* pSurface,double c1, double c2)
{
	//http://sprott.physics.wisc.edu/chaos/lyapexp.htm
	double d0 = 1e-3, d1, total;
	double x, y, x_,y_; double x2, y2; double xtmp, ytmp, sx, sy;
	int N = CountPixelsSettle+CountPixelsDraw; int settle = CountPixelsSettle;
	double sx0= g_settings->seedx0, sx1=g_settings->seedx1, sy0= g_settings->seedy0, sy1=g_settings->seedy1;
	int nXpoints=CountPixelsSeedsPerAxis, nYpoints=CountPixelsSeedsPerAxis;
	double sxinc = (nXpoints==1) ? 1e6 : (sx1-sx0)/(nXpoints-1);
	double syinc = (nYpoints==1) ? 1e6 : (sy1-sy0)/(nYpoints-1);
	for (double sxi=sx0; sxi<=sx1; sxi+=sxinc)
    {
    for (double syi=sy0; syi<=sy1; syi+=syinc)
    {
	if (StringsEqual(MAPSUFFIX,BURGERSUF) && sxi==sx0 && syi==sy0) {sx=0.0; sy=0.00001;}
	else {sx=sxi; sy=syi;}
	total = 0.0;
	x=sx; y=sy; 
	x2=sx; y2=sy+d0;
	for (int i=0; i<N; i++)
	{
		MAPEXPRESSION;
		x=x_; y=y_;
		xtmp=x; ytmp=y; x=x2; y=y2;
		
		MAPEXPRESSION;
		x=x_; y=y_;
		x2=x; y2=y; x=xtmp; y=ytmp;

		d1 = sqrt( (x2-x)*(x2-x) + (y2-y)*(y2-y) ); //distance
		x2=x+ (d0/d1)*(x2-x); //also looks interesting when these are commented out
		y2=y+ (d0/d1)*(y2-y);
		if (i>settle) total+= log(d1/d0 );
		if (ISTOOBIG(x) || ISTOOBIG(y))
			break;
	}
	if (!(ISTOOBIG(x) || ISTOOBIG(y))) goto FoundTotal;
	}
	}
	if (ISTOOBIG(x) || ISTOOBIG(y))
		return SDL_MapRGB(pSurface->format, 255,255,255);
FoundTotal:
	double val = total / (N-settle);
	if (val < 0) 
		return 0;
	return standardToColors(pSurface, val, .8);
	// 4 possibilities: all escape: white, negative lyapunov: black, ly>cutoff: dark red, otherwise rainbow ly
}

// this can be made faster with SSE instructions, but that'd require an SSE version of MAPEXPRESSION
#define PHASESIZE 128
int arrT1[PHASESIZE*PHASESIZE] = {0};
int arrT2[PHASESIZE*PHASESIZE] = {0};
int whichIDT1 = 2, whichIDT2 = 2, whichID=2;
int countPhasePlotPixels(SDL_Surface* pSurface, double c1, double c2, int whichThread, FastMapMapSettings * boundsettings)
{
	int * arr = whichThread ? arrT1:arrT2;
	int * whichID = (whichThread)?&whichIDT1:&whichIDT2;

	int counted; double x,y,x_,y_, sx,sy;
	double sx0= g_settings->seedx0, sx1=g_settings->seedx1, sy0= g_settings->seedy0, sy1=g_settings->seedy1;
	int nXpoints=CountPixelsSeedsPerAxis, nYpoints=CountPixelsSeedsPerAxis;
	double sxinc = (nXpoints==1) ? 1e6 : (sx1-sx0)/(nXpoints-1);
	double syinc = (nYpoints==1) ? 1e6 : (sy1-sy0)/(nYpoints-1);
	double X0=boundsettings->x0, X1=boundsettings->x1, Y0=boundsettings->y0, Y1=boundsettings->y1;

	for (double sxi=sx0; sxi<=sx1; sxi+=sxinc)
    {
    for (double syi=sy0; syi<=sy1; syi+=syinc)
    {
	if (StringsEqual(MAPSUFFIX,BURGERSUF) && sxi==sx0 && syi==sy0) {sx=0.0; sy=0.00001;}
	else {sx=sxi; sy=syi;}
		x=sx; y=sy; 
		for (int i=0; i<CountPixelsSettle; i++)
		{
			MAPEXPRESSION; x=x_; y=y_;
			if (ISTOOBIG(x) || ISTOOBIG(y)) break;
		}
		
		counted = 0; *whichID = (*whichID)+1; //note incr. in here. effects of previous sx,sy are not counted.
		for (int i=0; i<CountPixelsDraw; i++)
		{
			if (ISTOOBIG(x) || ISTOOBIG(y)) break;
			MAPEXPRESSION; x=x_; y=y_;
			int px = lrint(PHASESIZE * ((x - X0) / (X1 - X0)));
			int py = lrint(/*PHASESIZE -*/ PHASESIZE * ((y - Y0) / (Y1 - Y0)));
			if (py >= 0 && py < PHASESIZE && px>=0 && px<PHASESIZE)
				if (arr[px+py*PHASESIZE]!=*whichID)
				{ arr[px+py*PHASESIZE]=*whichID; counted++;}
		}
		if (!(ISTOOBIG(x) || ISTOOBIG(y))) goto FoundTotal;

	}
	}
	//if here, they all escaped.
	return 0;
FoundTotal:
	return standardToColors(pSurface, (double)counted, (double)CountPixelsDraw);
		
}

// Each thread creates half of the diagram. The PassToThread struct simply passes arguments to function.
#define LegendWidth 4
typedef struct { int whichHalf; SDL_Surface* pMSurface; CoordsDiagramStruct*diagram; FastMapMapSettings * bounds; } PassToThread;
int DrawMenagerieThread( void* pStruct) 
{
	PassToThread*p = (PassToThread *)pStruct; int whichHalf = p->whichHalf; SDL_Surface*pMSurface=p->pMSurface;CoordsDiagramStruct*diagram=p->diagram; 
	FastMapMapSettings * boundsettings=p->bounds;

	double fx,fy; char*pPosition; Uint32 newcol;
	int width = pMSurface->w - LegendWidth; int height=pMSurface->h; 
	double X0=*diagram->px0, X1=*diagram->px1, Y0=*diagram->py0, Y1=*diagram->py1;
	double dx = (X1 - X0) / width, dy = (Y1 - Y0) / height;
	fx = X0; fy = Y1; //y counts downwards
	if (whichHalf) fy = Y1; 
	else fy = Y1 - dy;
	//don't assign to threads upper/lower half, instead interleave for better load balancing. (want both threads to finish at about same time)

	for (int py=(whichHalf?0:1); py<(height); py+=2)
	{
		fx=X0;
		for (int px = 0; px < width; px++)
		{
			if (!(g_settings->drawingOptions & maskOptionsDiagramMethod)) 
				newcol = countPhasePlotLyapunov(pMSurface, fx, fy);
			else 
				newcol = countPhasePlotPixels(pMSurface,fx,fy,whichHalf,boundsettings);

			pPosition = ( char* ) pMSurface->pixels ; //determine position
			pPosition += ( pMSurface->pitch * py ); //offset by y
			pPosition += ( pMSurface->format->BytesPerPixel * px ); //offset by x
			memcpy ( pPosition , &newcol , pMSurface->format->BytesPerPixel ) ;

			fx += dx;
		}
	fy -= 2*dy;
	}
	return 0;
}
PassToThread pThread1; PassToThread pThread2;
//#include "timecounter.h"
void DrawMenagerieMultithreaded( SDL_Surface* pMSurface, CoordsDiagramStruct*diagram) 
{
	//draw a color legend. todo: cache this?
	for (int px=pMSurface->w - LegendWidth; px<pMSurface->w; px++)
	for (int py=0; py<pMSurface->h; py++) {
		int color = standardToColors(pMSurface, (double)py, (double) pMSurface->h);
		char* pPosition = ( char* ) pMSurface->pixels ; //determine position
		pPosition += ( pMSurface->pitch * (pMSurface->h - py-1) ); //offset by y
		pPosition += ( pMSurface->format->BytesPerPixel * (px) ); //offset by x
		memcpy ( pPosition , &color , pMSurface->format->BytesPerPixel ) ;
	}
	//put into boundsettings a copy of default bounds, which we'll use in countpixels.
	FastMapMapSettings currentSettings, boundsettings;
	memcpy(&currentSettings, g_settings, sizeof(FastMapMapSettings));
	loadFromFile(MAPDEFAULTFILE); //load defaults
	memcpy(&boundsettings, g_settings, sizeof(FastMapMapSettings));
	memcpy(g_settings, &currentSettings, sizeof(FastMapMapSettings));

	//pass arguments via structure. note: pThread1 apparently should be global.
	pThread1.whichHalf=0; pThread1.bounds=&boundsettings; pThread1.diagram=diagram; pThread1.pMSurface=pMSurface; 
	pThread2.whichHalf=1; pThread2.bounds=&boundsettings; pThread2.diagram=diagram; pThread2.pMSurface=pMSurface; 
//startTimer();
	
	SDL_Thread *thread2 =  SDL_CreateThread(DrawMenagerieThread, &pThread2);
	DrawMenagerieThread(&pThread1);
	SDL_WaitThread(thread2, NULL);

//printf("time:%d", (int)stopTimer());
}


void blitDiagram(SDL_Surface* pSurface,SDL_Surface* pSmallSurface, int px, int py)
{
	SDL_Rect dest;
	dest.x = px;
	dest.y = py;
	dest.w = pSmallSurface->w;
	dest.h = pSmallSurface->h; 
	SDL_BlitSurface(pSmallSurface, NULL, pSurface, &dest);
}
