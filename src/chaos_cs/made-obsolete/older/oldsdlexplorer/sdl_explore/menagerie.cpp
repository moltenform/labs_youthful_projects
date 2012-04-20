#include "phaseportrait.h"
#include "common.h"
#include <math.h>
#include "assert.h"
#include "font.h"

int chy_width=3072, chy_height=1536;
unsigned short * alldata = NULL;
void loadMenagerieData()
{
	if (! StringsEqual(MAPEXPRESSIONTEXT, STRINGIFY(BURGER))) return;
	if (alldata != NULL) return;
	FILE * f = fopen("data/chy.dat", "rb");
	if (!f) { assert(0); exit(1); }
	alldata = (unsigned short *) malloc( sizeof(unsigned short)*chy_width*chy_height);
	fread(alldata, sizeof(unsigned short), chy_width*chy_height, f);
	fclose(f);
}

void DrawMenagerieFromPrecomputed( SDL_Surface* pSmallSurface, PhasePortraitSettings*settings) 
{
double chy_x0 = -3.4, chy_x1=1.1, chy_y0=1.45, chy_y1=2.05;
	double fx,fy;
	int height=PlotHeight;
	int width=PlotWidth;
	double X0=settings->browsex0, X1=settings->browsex1, Y0=settings->browsey0, Y1=settings->browsey1;
	
    double dx = (X1 - X0) / width, dy = (Y1 - Y0) / height;
    fx = X0; fy = Y1; //y counts downwards
    
	for (int py=0; py<height; py++)
	{
		fx = X0;
		for (int px = 0; px < width; px++)
		{
int hits;
if (fx<chy_x0 || fx>chy_x1 || fy<chy_y0 || fy>chy_y1)
hits=0;
else
{
	int indexx = (int) (chy_width * (fx-chy_x0)/(chy_x1-chy_x0));
	int indexy = (int) (chy_height * (fy-chy_y0)/(chy_y1-chy_y0));
	hits = alldata[ indexy*chy_width + indexx];
}
		double val = sqrt((double)hits)/40;

		if (val>1.0) val=1.0; if (val<0.0) val=0.0;
		val = val*2 - 1; //from -1 to 1
		Uint32 r,g,b;
		if (val>=0)
			b=255, r=g= (Uint32) ((1-val)*255.0);
		else
			r=g=b= (Uint32) ((val+1)*255.0);

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











typedef struct
{
	int seedsPerAxis;
	int settling;
	int drawing;
	int phaseFigureWidth; 
	int phaseFigureHeight;
	double phasex0;
	double phasex1;
	double phasey0;
	double phasey1;
} MenagerieSettings;
//if symmetrical, we could make phasey0 0, but that wouldn't really be more efficient.

//MenagerieSettings mSettings = {5,150,16, 384,384, -3,1,-3,3};
MenagerieSettings mSettings = {10,150,16, 384,384, -10,10,-10,10};
MenagerieSettings mSettingsHigher = {5,150,16, 384,384, -3,1,-3,3};



//note the bounds here: should it be square? should it change based on which map? ?
//MenagerieSettings mSettings = {20,400,16, 100,100, -3.75,0.75,-2.1,2.1};
//MenagerieSettings mSettings = {10,100,16, 100,100, -3.75,0.75,-2.1,2.1};
//MenagerieSettings mSettings = {10,50,16, 256,256, -3.75,0.75,-2.1,2.1};
//MenagerieSettings mSettings = {5,150,16, 256,256, -3.75,0.75,-2.1,2.1}; //{5,150,16, 1024,1024, -6,6,-6,6};
//
//MenagerieSettings mSettingsHigher = {10,150,16, 256,256, -3.75,0.75,-2.1,2.1};
//MenagerieSettings mSettingsHigher = {5,150,16, 256,256, -3.75,0.75,-2.1,2.1};
//MenagerieSettings mSettingsHigher = {5,150,16, 1024,1024, -6,6,-6,6};
//MenagerieSettings mSettingsHigher = {5,150,16, 256,256, -6,6,-6,6};
//
MenagerieSettings* managerieSettings = &mSettings;
MenagerieSettings* managerieSettingsHigher = &mSettingsHigher;

inline double _getLyapunov(MenagerieSettings* settings, double c1, double c2, int arrAns[])
{
	//http://sprott.physics.wisc.edu/chaos/lyapexp.htm
	//double d0 = 1e-12;
	double d0 = 1e-3, d1;
	int N = 200; int settle = 20; //used to be 20
	double total = 0.0;
	double sx = -1.0, sy = -1.5;
	double x=sx, y=sy, x_;
	double x2=sx, y2=sy+d0, x2_;
	for (int i=0; i<N; i++)
	{
		//x_ = 1 - c1*x*x + y; y = c2*x; 
		//x=x_;
		//x2_ = 1 - c1*x2*x2 + y2; y2 = c2*x2; 
		//x2=x2_;
		x_ = c1*x - y*y; y= c2*y + x*y;
		x=x_;
		x2_ = c1*x2 - y2*y2; y2= c2*y2 + x2*y2;
		x2=x2_;

		d1 = sqrt( (x2-x)*(x2-x) + (y2-y)*(y2-y) ); //distance
		x2=x+ (d0/d1)*(x2-x); //also looks interesting when these are commented out
		y2=y+ (d0/d1)*(y2-y);
		if (i>settle) total+= log(d1/d0 );
		if (ISTOOBIG(x) || ISTOOBIG(y)) return -300;
	}
	return total / (N-settle);
}



int CURRENTID=2;
int _drawPhasePortrait(MenagerieSettings* settings, double c1, double c2, int arrAns[])
{
	CURRENTID++;
	double sx,sy; int ii;//loop vars
	int totalUniquePoints = 0;
	int arrWidth = settings->phaseFigureWidth;
	int arrHeight = settings->phaseFigureHeight;
	//clear array
	//memset(arrAns, 0, sizeof(int)*arrWidth*arrHeight); DON"T DO THIS it's SLOW!!!
	double X0=settings->phasex0, X1=settings->phasex1, Y0=settings->phasey0, Y1=settings->phasey1;
	double x, y, x_;
	
	int nXpoints = settings->seedsPerAxis; int nYpoints = settings->seedsPerAxis;
	double sx0= -2, sx1=2, sy0= -2, sy1=2;
	double sxinc = (nXpoints==1) ? 1e6 : (sx1-sx0)/(nXpoints-1);
	double syinc = (nYpoints==1) ? 1e6 : (sy1-sy0)/(nYpoints-1);
	
	for (sx=sx0; sx<=sx1; sx+=sxinc) {
	for (sy=sy0; sy<=sy1; sy+=syinc)
	{
			x = sx; y=sy;

			for (ii=0; ii<(settings->settling/4); ii++)
			{
				MAPEXPRESSION;
				x = x_;
				MAPEXPRESSION;
				x = x_;
				MAPEXPRESSION;
				x = x_;
				MAPEXPRESSION;
				x = x_;
				if ((ISTOOBIG(x)||ISTOOBIG(y))) break;
			}
			for (ii=0; ii<(settings->drawing); ii++)
			{
				if ((ISTOOBIG(x)||ISTOOBIG(y))) break;
				MAPEXPRESSION;
				x = x_;

				int px = (int)(arrWidth * ((x - X0) / (X1 - X0)));
				int py = (int)(arrHeight - arrHeight * ((y - Y0) / (Y1 - Y0)));
				if (py >= 0 && py < arrHeight && px>=0 && px<arrWidth)
				    if (arrAns[py + px * arrHeight]!=CURRENTID)
				    { arrAns[py + px * arrHeight]=CURRENTID; totalUniquePoints++;}
			}
	}
	}
	return totalUniquePoints;
}
#include "fastmenag.h"


#define LOWQSTEP 1
int *arr=NULL;
void DrawMenagerieLowQuality( SDL_Surface* pSmallSurface, PhasePortraitSettings*settings) 
{
	//SDL_FillRect ( pSmallSurface , NULL , g_white );
	
	if (arr==NULL)
		arr = (int*) malloc( sizeof(int)*managerieSettings->phaseFigureWidth*managerieSettings->phaseFigureHeight);

	double fx,fy;

	int height=PlotHeight;
	int width=PlotWidth;

	double X0=settings->browsex0, X1=settings->browsex1, Y0=settings->browsey0, Y1=settings->browsey1;
	
    double dx = (X1 - X0) / width, dy = (Y1 - Y0) / height;
    fx = X0; fy = Y1; //y counts downwards
	 char* pPosition;
    for (int px = 0; px < width; px+=LOWQSTEP)
    {
        fy = Y1;
        for (int py=0; py<height; py+=LOWQSTEP)
        {
		Uint32 r,g,b;

		//int hits= _drawPhasePortrait(managerieSettings, fx,fy,arr);
		int hits= _drawPhasePortraitVector(managerieSettings, fx,fy,arr);
		double val = sqrt((double)hits)/10;// / 20.0;
		if (val>1.0) val=1.0; if (val<0.0) val=0.0;
		val = val*2 - 1; //from -1 to 1
		if (val>=0)
			b=255, r=g= (Uint32) ((1-val)*255.0);
		else
			r=g=b= (Uint32) ((val+1)*255.0);

			/*
			Lyapunov coloring::
			double ly = _getLyapunov(managerieSettings, fx,fy,arr);
			if (ly==-300) { r=0, g=60, b=0;
			} else {
		double val = ly ; ///5;
				if (val>0) val = sqrt(((val)));
				else val = -sqrt((-val));
				val /= 1;
		
		/*int hits= _drawPhasePortrait(managerieSettings, fx,fy,arr);
		double val = sqrt((double)hits)/10;// / 20.0; 

		if (val>1.0) val=1.0; if (val<-1.0) val=-1.0;

		if (val>=0)
			b=255, r=g= (Uint32) ((1-val)*255.0);
		else
			r=g=b= (Uint32) ((val+1)*255.0);
		}*/
  
for (int ncy=0; ncy<LOWQSTEP; ncy++){
for (int ncx=0; ncx<LOWQSTEP; ncx++){

  pPosition = ( char* ) pSmallSurface->pixels ; //determine position
  pPosition += ( pSmallSurface->pitch * (py+ncy) ); //offset by y
  pPosition += ( pSmallSurface->format->BytesPerPixel * (px+ncx) ); //offset by x
  Uint32 newcol = SDL_MapRGB ( pSmallSurface->format , r , g , b ) ;
  memcpy ( pPosition , &newcol , pSmallSurface->format->BytesPerPixel ) ;
}}
            fy -= dy*LOWQSTEP;
        }
        fx += dx*LOWQSTEP;
    }
}

int *arrHigher=NULL;
#define HQSTEPSIZE 2
void DrawMenagerieHigherQuality( SDL_Surface* pSmallSurface, PhasePortraitSettings*settings) 
{
	if (arrHigher==NULL)
		arrHigher = (int*) malloc( sizeof(int)*managerieSettingsHigher->phaseFigureWidth*managerieSettingsHigher->phaseFigureHeight);

	double fx,fy;

	int height=PlotHeight;
	int width=PlotWidth;

	double X0=settings->browsex0, X1=settings->browsex1, Y0=settings->browsey0, Y1=settings->browsey1;
	
    double dx = (X1 - X0) / width, dy = (Y1 - Y0) / height;
    fx = X0; fy = Y1; //y counts downwards
	 char* pPosition;
    for (int px = 0; px < width; px+=HQSTEPSIZE)
    {
        fy = Y1;
        for (int py=0; py<height; py+=HQSTEPSIZE)
        {
		int hits= _drawPhasePortrait(managerieSettingsHigher, fx,fy,arrHigher);
		double val = sqrt((double)hits)/15;
		if (val>1.0) val=1.0; if (val<0.0) val=0.0;
		val = val*2 - 1; //from -1 to 1
		Uint32 r,g,b;
		if (val>=0)
			b=255, r=g= (Uint32) ((1-val)*255.0);
		else
			r=g=b= (Uint32) ((val+1)*255.0);
  
for (int ncy=0; ncy<HQSTEPSIZE; ncy++){
for (int ncx=0; ncx<HQSTEPSIZE; ncx++){

  pPosition = ( char* ) pSmallSurface->pixels ; //determine position
  pPosition += ( pSmallSurface->pitch * (py+ncy) ); //offset by y
  pPosition += ( pSmallSurface->format->BytesPerPixel * (px+ncx) ); //offset by x
  Uint32 newcol = SDL_MapRGB ( pSmallSurface->format , r , g , b ) ;
  memcpy ( pPosition , &newcol , pSmallSurface->format->BytesPerPixel ) ;
}}
            fy -= dy*HQSTEPSIZE;
        }
        fx += dx*HQSTEPSIZE;
    }
}

void DrawMenagerie( SDL_Surface* pSmallSurface, PhasePortraitSettings*settings, BOOL bHigherQuality) 
{
	if (SDL_MUSTLOCK(pSmallSurface)) SDL_LockSurface ( pSmallSurface ) ;
	//if the burger's map, use precomputed. else dynamically compute.
	if (StringsEqual(MAPEXPRESSIONTEXT, STRINGIFY(BURGER)))
		DrawMenagerieFromPrecomputed(pSmallSurface,settings);
	else if (!bHigherQuality)
		DrawMenagerieLowQuality(pSmallSurface, settings);
	else
		DrawMenagerieHigherQuality(pSmallSurface, settings);
	if (SDL_MUSTLOCK(pSmallSurface)) SDL_UnlockSurface ( pSmallSurface ) ;

}

void BlitMenagerie(SDL_Surface* pSurface,SDL_Surface* pSmallSurface)
{
	SDL_Rect src, dest;
	src.x = 0;
	src.y = 0;
	src.w = PlotWidth;
	src.h = PlotHeight;
	dest.x = PlotX;
	dest.y = 0;
	dest.w = PlotWidth;
	dest.h = PlotHeight; 
	SDL_BlitSurface(pSmallSurface, &src, pSurface, &dest);
}

