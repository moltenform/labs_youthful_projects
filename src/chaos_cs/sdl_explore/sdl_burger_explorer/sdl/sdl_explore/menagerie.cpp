#include "SDL.h"
#include "phaseportrait.h"
#include "basic.h"
#include <math.h>


double chy_x0 = -3.4, chy_x1=1.1, chy_y0=1.45, chy_y1=2.05;
int chy_width=3072, chy_height=1536;
unsigned short * alldata = NULL;
void loadData()
{
	if (alldata != NULL) return;
	FILE * f = fopen("C:\\pydev\\mainsvn\\chaos_cs\\sdl_explore\\sdl_burger_explorer\\chy.dat", "rb");
	if (!f) {int die = 0; exit(2);}
	alldata = (unsigned short *) malloc( sizeof(unsigned short)*chy_width*chy_height);
	//for (int i=0; i<chy_width*chy_height; i++)
	fread(alldata, sizeof(unsigned short), chy_width*chy_height, f);
	fclose(f);
}

void DrawMenagerie( SDL_Surface* pSmallSurface, PhasePortraitSettings*settings) 
{
	double fx,fy;
	if (SDL_MUSTLOCK(pSmallSurface)) SDL_LockSurface ( pSmallSurface ) ;
int height=PlotHeight;
	int width=PlotWidth;
double X0=settings->browsex0, X1=settings->browsex1, Y0=settings->browsey0, Y1=settings->browsey1;
	
    double dx = (X1 - X0) / width, dy = (Y1 - Y0) / height;
    fx = X0; fy = Y0; //y counts downwards
    
	for (int py=0; py<height; py++)
	{
		fx = X0;
		for (int px = 0; px < width; px++)
		{
			
		//int hits= _drawPhasePortrait(managerieSettings, fx,fy,arr);
		double val = fx+fy; //sqrt((double)hits);// / 20.0;
		//double val = sqrt((double)hits)/10;// / 20.0;
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

		fy += dy;
	}


	if (SDL_MUSTLOCK(pSmallSurface)) SDL_UnlockSurface ( pSmallSurface ) ;
}











typedef struct
{
	int seedsPerAxis;
	int settling;
	int drawing;
	int phaseFigureWidth; //
	int phaseFigureHeight; //
	double phasex0;
	double phasex1;
	double phasey0;
	double phasey1;
} MenagerieSettings;

//MenagerieSettings mSettings = {20,400,16, 100,100, -3.75,0.75,-2.1,2.1};
MenagerieSettings mSettings = {10,100,16, 100,100, -3.75,0.75,-2.1,2.1};
MenagerieSettings* managerieSettings = &mSettings;


int _drawPhasePortrait(MenagerieSettings* settings, double a, double b, int arrAns[])
{
	double sx,sy; int ii;//loop vars
	int totalUniquePoints = 0;
	int arrWidth = settings->phaseFigureWidth;
	int arrHeight = settings->phaseFigureHeight;
	//clear array
	memset(arrAns, 0, sizeof(int)*arrWidth*arrHeight);
	double X0=settings->phasex0, X1=settings->phasex1, Y0=settings->phasey0, Y1=settings->phasey1;
	double x, y, x_;
	
	int nXpoints = settings->seedsPerAxis; int nYpoints = settings->seedsPerAxis;
	double sx0= -2, sx1=2, sy0= -2, sy1=2;
	double sxinc = (nXpoints==1) ? 1e6 : (sx1-sx0)/(nXpoints-1);
	double syinc = (nYpoints==1) ? 1e6 : (sy1-sy0)/(nYpoints-1);
	
//return (int)(a*12 + b*12);

	for (sx=sx0; sx<=sx1; sx+=sxinc) {
	for (sy=sy0; sy<=sy1; sy+=syinc)
	{
			x = sx; y=sy;

			for (ii=0; ii<(settings->settling); ii++)
			{
				x_ = a*x - y*y;
				y = b*y + x*y;
				x = x_;
				if ((ISTOOBIG(x)||ISTOOBIG(y))) break;
			}
			for (ii=0; ii<(settings->drawing); ii++)
			{
				x_ = a*x - y*y;
				y = b*y + x*y;
				x = x_;
				if ((ISTOOBIG(x)||ISTOOBIG(y))) break;

				int px = (int)(arrWidth * ((x - X0) / (X1 - X0)));
				int py = (int)(arrHeight - arrHeight * ((y - Y0) / (Y1 - Y0)));
				if (py >= 0 && py < arrHeight && px>=0 && px<arrWidth)
				    if (arrAns[py + px * arrHeight]==0)
				    { arrAns[py + px * arrHeight]=1; totalUniquePoints++;}
			}
	}
	}
	return totalUniquePoints;
}

void DrawMenagerieOld( SDL_Surface* pSmallSurface, PhasePortraitSettings*settings) 
{
	if (SDL_MUSTLOCK(pSmallSurface)) SDL_LockSurface ( pSmallSurface ) ;
	int* arr = (int*) malloc( sizeof(int)*managerieSettings->phaseFigureWidth*managerieSettings->phaseFigureHeight);

	double fx,fy;

	int height=PlotHeight;
	int width=PlotWidth;

	double X0=settings->browsex0, X1=settings->browsex1, Y0=settings->browsey0, Y1=settings->browsey1;
	
    double dx = (X1 - X0) / width, dy = (Y1 - Y0) / height;
    fx = X0; fy = Y1; //y counts downwards
    for (int px = 0; px < width; px++)
    {
        fy = Y1;
        for (int py=0; py<height; py++)
        {
			//fx,fy
		int hits= _drawPhasePortrait(managerieSettings, fx,fy,arr);

		//double val = fx+fy; //sqrt((double)hits);// / 20.0;
		double val = sqrt((double)hits)/10;// / 20.0;
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


            fy -= dy;
        }
        fx += dx;
    }

	free(arr);
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
