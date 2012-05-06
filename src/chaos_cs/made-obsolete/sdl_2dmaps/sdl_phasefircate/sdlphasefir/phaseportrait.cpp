#define FixA 1

#include "phaseportrait.h"
#include <math.h>

void InitialSettings(PhasefircationSettings*inputSettings, int width, int height, double *outStp, double *outB)
{
	inputSettings->height = 256;
	inputSettings->width = 256;
#if FixA
	inputSettings->a0 = -1.2;
	inputSettings->a1 = 1.0;
	*outB = 1.72; 
#else
	inputSettings->a0 = 1.4;
	inputSettings->a1 = 2.1;
	*outB = -1; 
#endif

	inputSettings->seedsPerAxis=40;
	inputSettings-> settling = 48;
	inputSettings-> drawing = 20;
	inputSettings->phasex0 = -1.75; inputSettings->phasex1 = 1.75; 
	inputSettings->phasey0 = -1.75; inputSettings->phasey1 = 1.75;
	
	*outStp = (double) (inputSettings->height/2 - 6); 
	
}


void drawPhasePortraitFir(PhasefircationSettings* settings, double a, double b, uchar arrAns[]);

void CreateCache(PhasefircationSettings* settings, double actualB, uchar arrBig[])
{
	int arrWidth = settings->width; int arrHeight = settings->height;
	
	int py;
	double vparam = settings->a0;
	double dparam = (settings->a1 - settings->a0) / (settings->height);
	for (py=0; py<(settings->height); py++)
	{
		//save directly into cache
#if FixA
		drawPhasePortraitFir(settings, vparam, actualB, arrBig+py*(arrWidth*arrHeight));
#else
		drawPhasePortraitFir(settings, actualB, vparam, arrBig+py*(arrWidth*arrHeight));
#endif

		vparam += dparam;
	}
}
void createPhasefirFromCache(SDL_Surface* pSurface, PhasefircationSettings* settings, double actualStp, uchar arrBig[])
{
	int dist = (int) actualStp;
	int arrWidth = settings->width; int arrHeight = settings->height;
	
	uchar * arrAns;
	int py, px;
	double vparam = settings->a0;
	double dparam = (settings->a1 - settings->a0) / (settings->height);
	for (py=0; py<(settings->height); py++)
	{
		arrAns = arrBig+ py*(settings->height*settings->width);
		
		//draw the middle collumn out...
		px=0;
		int c; for (c=0; c<settings->height; c++)
		{
			//Uint32 r = (char)arrAns[ c*arrWidth + dist ];  //this is for sure the vertical one
			//Uint32 r = (char)arrAns[ dist*arrWidth + c ];  //this is for sure the horizontal one
			//Uint32 r = (char)arrAns[ dist*arrWidth +((c/2)*arrWidth) + c ];  //this is horiz with slight diag
			//Uint32 r = (char)arrAns[ c*arrWidth + dist + py ];  //stepping
			Uint32 r = (char)arrAns[ c*arrWidth + dist + py/8 ];  //stepping


//Uint32 r = (char)arrAns[dist * arrWidth + colx]; //Horiz
//Uint32 r = (char)arrAns[coly * arrWidth + (255-coly)/*diagonal*/ + dist]; //Vert
//Uint32 r = (char)arrAns[(255-coly) * arrWidth + (255-coly)/*diagonal*/ + dist]; //Vert


//since it's symmetrical, the diags are the same....

Uint32 newcol = SDL_MapRGB ( pSurface->format , r , r,r ) ;
char* pPosition = ( char* ) pSurface->pixels ; //determine position
  pPosition += ( pSurface->pitch * py ); //offset by y
  pPosition += ( pSurface->format->BytesPerPixel * px ); //offset by x
  memcpy ( pPosition , &newcol , pSurface->format->BytesPerPixel ) ;

  ++px;

		}
		
		
		vparam += dparam;
	}
}





void drawPhasePortraitFir(PhasefircationSettings* settings, double a, double b, uchar arrSmall[])
{
	double sx,sy; int i,ii;//loop vars
	int arrWidth = settings->width;
	int arrHeight = settings->height;
	//clear array
	for (i=0; i<arrWidth*arrHeight;i++) arrSmall[i]=255; //note: all white
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

			for (ii=0; ii<(settings->settling); ii++)
			{
				x_ = a*x - y*y;
				y = b*y + x*y;
				x = x_;
				if (ISTOOBIG(x)||ISTOOBIG(y)) break;
			}
			for (ii=0; ii<(settings->drawing); ii++)
			{
				x_ = a*x - y*y;
				y = b*y + x*y;
				x = x_;
				if (ISTOOBIG(x)||ISTOOBIG(y)) break;

				int px = (int)(arrWidth * ((x - X0) / (X1 - X0)));
				int py = (int)(arrHeight - arrHeight * ((y - Y0) / (Y1 - Y0)));
				if (py >= 0 && py < arrHeight && px>=0 && px<arrWidth)
				{
				    arrSmall[py* arrWidth + px ] -= (arrSmall[py* arrWidth + px]>>2);
				    //~ (colR)-((colR)>>2)
				}
			}
	}
	}
}


