
#include "phaseportrait.h"
#include "menagerie.h"
#include "whichmap.h"
#include "coordsdiagram.h"
#include "palette.h"
#include <math.h>

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

// use the blue/white/red scheme?
//keep going until find one that goes the whole ways without escaping.
//(better to iterate through in a way that jumps around.
int countPhasePlotLyapunov(SDL_Surface* pSurface,double c1, double c2)
{
	//http://sprott.physics.wisc.edu/chaos/lyapexp.htm
	double d0 = 1e-3, d1;
	//int N = 400; int settle = 300; //used to be 20. 1000/600 causes moire patterns
	//int N = 900; int settle = 800; //used to be 20. 1000/600 causes moire patterns
	int N = 900; int settle = 800; //used to be 20. 1000/600 causes moire patterns
	//for (double sx=sx0; sx<=sx1; sx+=sxinc)
    //        {
    //            for (double sy=sy0; sy<=sy1; sy+=syinc)
    //            {
	double total = 0.0;
	double sx = 0.0;double sy=0.00001;
	double x=sx, y=sy, x_,y_;
	double x2=sx, y2=sy+d0, x2_;
	double xtmp, ytmp;
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
		if (ISTOOBIG(x) || ISTOOBIG(y)) return SDL_MapRGB(pSurface->format, 255,255,255);//255<<8; //pure green xxrrggxx
	}
	double val = total / (N-settle);
	if (val < 0) {  
		return 0;//
		/*val = sqrt(sqrt(-val));
		if (val>1.0) val=1.0;
		int v = (int)(val*255);
		return SDL_MapRGB(pSurface->format, 0,0,v);*/
	} 
	double estimatedmax= 1.4;
	val = sqrt(sqrt(val));
	if (val > estimatedmax) return SDL_MapRGB(pSurface->format, 255,0,0);
	val /= estimatedmax;
	return HSL2RGB(pSurface, val, 1.0, 0.5);
	//int v = (int)(val*255);
	//return SDL_MapRGB(pSurface->format, v,v,v);
	
}

void DrawMenagerie( SDL_Surface* pMSurface, CoordsDiagramStruct*diagram) 
{

double fx,fy; char* pPosition; Uint32 newcol;
int width = pMSurface->w; int height=pMSurface->h; 
double X0=*diagram->px0, X1=*diagram->px1, Y0=*diagram->py0, Y1=*diagram->py1;
double dx = (X1 - X0) / width, dy = (Y1 - Y0) / height;
fx = X0; fy = Y1; //y counts downwards

for (int py=0; py<height; py++)
{
	fx=X0;
	for (int px = 0; px < width; px++)
	{
		newcol = countPhasePlotLyapunov(pMSurface, fx, fy);

		pPosition = ( char* ) pMSurface->pixels ; //determine position
		pPosition += ( pMSurface->pitch * py ); //offset by y
		pPosition += ( pMSurface->format->BytesPerPixel * px ); //offset by x
		memcpy ( pPosition , &newcol , pMSurface->format->BytesPerPixel ) ;

		fx += dx;
	}
fy -= dy;
}
}

