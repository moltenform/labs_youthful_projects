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


void drawPhasePortraitFir(PhasefircationSettings* settings, double a, double b, Uint32 arrAns[]);

void CreateCache(PhasefircationSettings* settings, double actualB, Uint32 arrBig[])
{
	int arrWidth = settings->width; int arrHeight = settings->height;
	
	for (int i=0; i<arrWidth*arrHeight;i++) arrBig[i]=0; //set all black
	int py;
	
	double dparamA = (settings->a1 - settings->a0)/(settings->height);
	double dparamB = (2.0-1.5)/(settings->height);


	for (double paramA = settings->a0; paramA<settings->a1; paramA += dparamA)
	{
	for (double paramB = 1.5; paramB<2.0; paramB += dparamB)
	{
		//save directly into cache
		drawPhasePortraitFir(settings, paramA, paramB, arrBig);

	}
	}
}
void createPhasefirFromCache(SDL_Surface* pSurface, PhasefircationSettings* settings, double actualStp, Uint32 arrBig[])
{
	int dist = (int) actualStp;
	int arrWidth = settings->width; int arrHeight = settings->height;
	
	Uint32 * arrAns;
	int py, px;
	double vparam = settings->a0;
	double dparam = (settings->a1 - settings->a0) / (settings->height);
	for (py=0; py<(settings->height); py++)
	{
		arrAns = arrBig; //+ py*(settings->height*settings->width);
		
		//draw the middle collumn out...
		px=0;
		int c; for (c=0; c<settings->height; c++)
		{
			//Uint32 r = (char)arrAns[ c*arrWidth + dist ];  //this is for sure the vertical one
			//Uint32 r = (char)arrAns[ dist*arrHeight + c ];  //this is for sure the horizontal one
			//Uint32 r = arrAns[ c*arrHeight + py ] / 4; 
			double val = sqrt((double)arrAns[ c*arrHeight + py ])/35; 
			if (val>1.0) val=1.0; if (val<0.0) val=0.0;

			#define PI 3.14159265
			unsigned char r,g,b;
		//r = (char)( 100*sin(2*PI*val + 4.0*PI/3.0) + 100);
		//g = (char)( 50*sin(2*PI*val + 0) + 50);
		//b = (char)( 50*sin(2*PI*val + 2.0*PI/3.0 ) +50);
		
		val = val*2 - 1; //from -1 to 1
		if (val>=0)
			b=255, r=g= (char) ((1-val)*255);
		else
			r=g=b= (char) ((val+1)*255);

	//		int colx; for (colx=0; colx<settings->width; colx++)
	//	{
//Uint32 r = (char)arrAns[dist * arrWidth + colx]; //Horiz
//Uint32 r = (char)arrAns[coly * arrWidth + (255-coly)/*diagonal*/ + dist]; //Vert
//Uint32 r = (char)arrAns[(255-coly) * arrWidth + (255-coly)/*diagonal*/ + dist]; //Vert


//since it's symmetrical, the diags are the same....

Uint32 newcol = SDL_MapRGB ( pSurface->format , r , g,b ) ;
char* pPosition = ( char* ) pSurface->pixels ; //determine position
  pPosition += ( pSurface->pitch * py ); //offset by y
  pPosition += ( pSurface->format->BytesPerPixel * px ); //offset by x
  memcpy ( pPosition , &newcol , pSurface->format->BytesPerPixel ) ;

  ++px;
		}
		
		
		vparam += dparam;
	}
}





void drawPhasePortraitFir(PhasefircationSettings* settings, double a, double b, Uint32 arrSmall[])
{
	double sx,sy; int i,ii;//loop vars
	int arrWidth = settings->width;
	int arrHeight = settings->height;
	
	//don't clear the array!
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
				    //arrSmall[py* arrWidth + px ] -= (arrSmall[py* arrWidth + px]>>2);
				    //arrSmall[py* arrWidth + px ] -= (arrSmall[py* arrWidth + px]>>4); //minus a 16th of it...
				    arrSmall[py* arrWidth + px ]++;
				    //~ (colR)-((colR)>>2)
				}
			}
	}
	}
}


