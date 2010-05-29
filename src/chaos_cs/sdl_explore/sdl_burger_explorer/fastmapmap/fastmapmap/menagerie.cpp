#include "phaseportrait.h"
//#include "menagerie.h"
#include "whichmap.h"
#include "coordsdiagram.h"
#include "float_cast.h"
#include "palette.h"
#include "font.h"
#include "configfiles.h"
#include <math.h>
//see rev. 382 for a version that cached results. we chose not to do that anymore. 


__inline unsigned int standardToColors(SDL_Surface* pSurface, double valin, double estimatedMax)
{
	//double val = sqrt(valin) / sqrt(estimatedMax);
	double val = (valin) / (estimatedMax);
	if (val > estimatedMax) return SDL_MapRGB(pSurface->format, 50,0,0);
	val = ((valin) / (estimatedMax)) * 0.8 /*only use 0.0-0.8, because 0.99~0.0*/;
	return HSL2RGB(pSurface, val, 1,.5);
}


//keep going until find one that goes the whole ways without escaping.
//TODO: (it'd be better to iterate through in a way that jumps around...)
int countPhasePlotLyapunov(SDL_Surface* pSurface,double c1, double c2)
{
	//http://sprott.physics.wisc.edu/chaos/lyapexp.htm
	double d0 = 1e-3, d1, total;
	double x, y, x_,y_; double x2, y2, x2_; double xtmp, ytmp, sx, sy;
	int N = 140; int settle = 80;
	double sx0= g_settings->seedx0, sx1=g_settings->seedx1, sy0= g_settings->seedy0, sy1=g_settings->seedy1;
	//int nXpoints=g_settings->seedsPerAxis; int nYpoints=g_settings->seedsPerAxis;
	int nXpoints=10, nYpoints=10;
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
	return standardToColors(pSurface, val, 1.0);
	// 4 possibilities: all escape: white, negative lyapunov: black, ly>cutoff: dark red, otherwise rainbow ly
}

#define PHASESIZE 128
int arrT1[PHASESIZE*PHASESIZE] = {0};
int arrT2[PHASESIZE*PHASESIZE] = {0};
int whichIDT1 = 2, whichIDT2 = 2, whichID=2;
int countPhasePlotPixels(SDL_Surface* pSurface, double c1, double c2, int whichThread, FastMapMapSettings * boundsettings)
{
	int * arr = whichThread ? arrT1:arrT2;
	//int * whichID = (whichThread)?&whichIDT1:&whichIDT2;
	whichID++;//*whichID++; 
	
	int counted; double x,y,x_,y_, sx,sy;
	double sx0= g_settings->seedx0, sx1=g_settings->seedx1, sy0= g_settings->seedy0, sy1=g_settings->seedy1;
	//int nXpoints=g_settings->seedsPerAxis; int nYpoints=g_settings->seedsPerAxis;
	int nXpoints=10, nYpoints=10;
	double sxinc = (nXpoints==1) ? 1e6 : (sx1-sx0)/(nXpoints-1);
	double syinc = (nYpoints==1) ? 1e6 : (sy1-sy0)/(nYpoints-1);
	double X0=boundsettings->x0, X1=boundsettings->x1, Y0=boundsettings->y0, Y1=boundsettings->y1;

	for (double sxi=sx0; sxi<=sx1; sxi+=sxinc)
    {
    for (double syi=sy0; syi<=sy1; syi+=syinc)
    {
	if (StringsEqual(MAPSUFFIX,BURGERSUF) && sxi==sx0 && syi==sy0) {sx=0.0; sy=0.00001;}
	else {sx=sxi; sy=syi;}
//sx=sxi; sy=syi;
		x=sx; y=sy; 
		for (int i=0; i<80; i++)
		{
			MAPEXPRESSION; x=x_; y=y_;
			if (ISTOOBIG(x) || ISTOOBIG(y)) break;
		}
		
		counted = 0; whichID++; //note incr. in here. effects of previous sx,sy are not counted.
		for (int i=0; i<160; i++)
		{
			if (ISTOOBIG(x) || ISTOOBIG(y)) break;
			MAPEXPRESSION; x=x_; y=y_;
			int px = lrint(PHASESIZE * ((x - X0) / (X1 - X0)));
			int py = lrint(/*PHASESIZE -*/ PHASESIZE * ((y - Y0) / (Y1 - Y0)));
			if (py >= 0 && py < PHASESIZE && px>=0 && px<PHASESIZE)
				if (arr[px+py*PHASESIZE]!=whichID)
				{ arr[px+py*PHASESIZE]=whichID; counted++;}
		}
		if (!(ISTOOBIG(x) || ISTOOBIG(y))) goto FoundTotal;

	}
	}
	//if here, they all escaped.
	return 0;
FoundTotal:
	return standardToColors(pSurface, (double)counted, 160);
		
}



void DrawMenagerie( SDL_Surface* pMSurface, CoordsDiagramStruct*diagram) 
{
int LegendWidth=4;
double fx,fy; char* pPosition; Uint32 newcol;
int width = pMSurface->w - LegendWidth; int height=pMSurface->h; 
double X0=*diagram->px0, X1=*diagram->px1, Y0=*diagram->py0, Y1=*diagram->py1;
double dx = (X1 - X0) / width, dy = (Y1 - Y0) / height;
fx = X0; fy = Y1; //y counts downwards

FastMapMapSettings currentSettings, boundsettings;
memcpy(&currentSettings, g_settings, sizeof(FastMapMapSettings));
loadFromFile(MAPDEFAULTFILE); //load defaults
memcpy(&boundsettings, g_settings, sizeof(FastMapMapSettings));
memcpy(g_settings, &currentSettings, sizeof(FastMapMapSettings));
//now, boundsettings holds a copy of default bounds, which we'll use in countpixels.

for (int py=0; py<height; py++)
{
	fx=X0;
	for (int px = 0; px < width; px++)
	{
		newcol = countPhasePlotPixels(pMSurface,fx,fy,1,&boundsettings);//countPhasePlotLyapunov(pMSurface, fx, fy);

		pPosition = ( char* ) pMSurface->pixels ; //determine position
		pPosition += ( pMSurface->pitch * py ); //offset by y
		pPosition += ( pMSurface->format->BytesPerPixel * px ); //offset by x
		memcpy ( pPosition , &newcol , pMSurface->format->BytesPerPixel ) ;

		fx += dx;
	}
fy -= dy;
}
//draw color legend. todo: cache this image?
	for (int px=pMSurface->w - LegendWidth; px<pMSurface->w; px++)
	for (int py=0; py<height; py++) {
	int color = standardToColors(pMSurface, (double)py, height);
	char* pPosition = ( char* ) pMSurface->pixels ; //determine position
	pPosition += ( pMSurface->pitch * (height-py-1) ); //offset by y
	pPosition += ( pMSurface->format->BytesPerPixel * (px) ); //offset by x
	memcpy ( pPosition , &color , pMSurface->format->BytesPerPixel ) ;
	}
}


/*BOOL g_BusyThread1=TRUE, g_BusyThread2=TRUE;
SDL_Surface* tmmpsfs;
BOOL CreateMenagCache( SDL_Surface* pSurface )
{


	threadOnesProgress = 0.0;
	g_BusyThread1=g_BusyThread2=TRUE;
	//spawn the 2 threads. then return, without blocking.
	int iZero=0, iOne=1;
	SDL_Thread *thread1 = SDL_CreateThread(CalcMenagerieThread, &iZero);
	SDL_Thread *thread2 =  SDL_CreateThread(CalcMenagerieThread, &iOne);
	while (TRUE)
	{
		SDL_Event event;
		if ( SDL_PollEvent ( &event ) )
		{
			if ( event.type == SDL_QUIT ) { return FALSE; break; }
		}
		SDL_Delay(250);
		snprintf(buffer, sizeof(buffer), "%% %d complete", (int)(threadOnesProgress*100));
		SDL_FillRect ( pSurface , NULL , g_white );
		ShowText(buffer, 50, 150, pSurface);
		ShowText("Computing menagerie diagram (only done once)...", 50, 50, pSurface);
		SDL_UpdateRect ( pSurface , 0 , 0 , 0 , 0 ) ;
		if (!g_BusyThread1 &&!g_BusyThread2)
			break; //we're done.
	}
	
}
*/

/*int CalcMenagerieThread(void* pWhichHalf)
{
	int whichHalf = * (int*)pWhichHalf;
	unsigned char * localarrayOfResults = (whichHalf)? cachedEntire : cachedEntire + (CACHEH/2)*CACHEW;
	//note: should have loaded defaults before this.
	double X0=g_settings->diagramx0, X1=g_settings->diagramx1, Y0=g_settings->diagramy0, Y1=g_settings->diagramy1;
	//double X0=-2.5,X1=1,Y0=1.5,Y1=2;
	double dx = (X1 - X0) / CACHEW, dy = (Y1 - Y0) / CACHEH;
	double fx = X0, fy = Y1; //y counts down?
	if (!whichHalf) fy -= (g_settings->diagramy1 - g_settings->diagramy0)/2; //only compute half per thread.
	else fy = Y1;

	for (int py=0; py<CACHEH/2; py++) //Note the /2!
	{
		fx=X0;
		for (int px = 0; px < CACHEW; px++)
		{
			//int count = countPhasePlotPixels( fx,fy, whichHalf);
			//unsigned char val = (count==0)? 0 : (unsigned char)(count+1);
			//localarrayOfResults[py*CACHEW + px] = val;
			//localarrayOfResults[py*CACHEW + px] = countPhasePlotPixels( fx,fy, whichHalf)==0?10:40;
			//localarrayOfResults[py*CACHEW + px] = (whichHalf)? 30:40;
			localarrayOfResults[py*CACHEW + px] = (unsigned char) 
				(countPhasePlotLyapunov(tmmpsfs,fx,fy)&0xff) ;
				
			fx += dx;
		}
		fy -= dy;
		if (whichHalf) threadOnesProgress = py/((double)(CACHEH/2));
	}
	if (whichHalf) g_BusyThread2 = FALSE;
	else g_BusyThread1 = FALSE;
	return 0;
}*/

/*
void DrawMenagerieFromCache( SDL_Surface* pSmallSurface, CoordsDiagramStruct*diagram) 
{
double chy_x0 = cacheX0, chy_x1=cacheX1, chy_y0=cacheY0, chy_y1=cacheY1;
	double fx,fy;
	int height=200;//diagram->screen_height;
	int width=200;//diagram->screen_width;
	double X0=*diagram->px0, X1=*diagram->px1, Y0=*diagram->py0, Y1=*diagram->py1;
    double dx = (X1 - X0) / width, dy = (Y1 - Y0) / height;
    fx = X0; fy = Y1; //y counts downwards
    
	for (int py=0; py<height; py++)
	{
		fx = X0;
		for (int px = 0; px < width; px++)
		{
int hits;
if (fx<chy_x0 || fx>chy_x1 || fy<chy_y0 || fy>chy_y1)
hits=-1; //should compute it here, I suppose.
else
{
	int indexx = (int) (CACHEW * (fx-chy_x0)/(chy_x1-chy_x0));
	int indexy = CACHEH-(int) (CACHEH * (fy-chy_y0)/(chy_y1-chy_y0));
	hits = cachedEntire[ indexy*CACHEW + indexx];
}
		Uint32 r,g,b;
		if (hits==0) {r=b=0; g=50; }  
		else if (hits==-1) {b=g=0; r=50; } else {
		double val = sqrt((double)hits)/16.0;

		if (val>1.0) val=1.0; if (val<0.0) val=0.0;
		val = val*2 - 1; //from -1 to 1
		if (val>=0)
			b=255, r=g= (Uint32) ((1-val)*255.0);
		else
			r=g=b= (Uint32) ((val+1)*255.0);
		}

char* pPosition = ( char* ) pSmallSurface->pixels ; //determine position
pPosition += ( pSmallSurface->pitch * py ); //offset by y
pPosition += ( pSmallSurface->format->BytesPerPixel * px ); //offset by x
Uint32 newcol = SDL_MapRGB ( pSmallSurface->format , r , g , b ) ;
memcpy ( pPosition , &newcol , pSmallSurface->format->BytesPerPixel ) ;

			fx += dx;
		}

		fy -= dy;
	}
}
*/


void BlitDiagram(SDL_Surface* pSurface,SDL_Surface* pSmallSurface, int px, int py)
{
	//SDL_FillRect ( pSmallSurface , NULL , 345232 );
	SDL_Rect dest;
	dest.x = px;
	dest.y = py;
	dest.w = pSmallSurface->w;
	dest.h = pSmallSurface->h; 
	SDL_BlitSurface(pSmallSurface, NULL, pSurface, &dest);
}