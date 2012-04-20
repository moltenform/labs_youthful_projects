#include "common.h"
#include "phaseportrait.h"
#include <math.h>

/*inline*/ void plotpoint(SDL_Surface* pSurface, int px, int py, int color)
{
if (!(px<=MenagWidth && px>=0 && py>=0 && py<=MenagHeight))
	 return;
  char* pPosition = ( char* ) pSurface->pixels ; //determine position
  pPosition += ( pSurface->pitch * py ); //offset by y
  pPosition += ( pSurface->format->BytesPerPixel * px ); //offset by x
  Uint32 newcol = color; //0;
  memcpy ( pPosition , &newcol , pSurface->format->BytesPerPixel ) ;
}
void DrawPlotGrid( SDL_Surface* pSurface, MenagFastSettings*settings, double c1, double c2 ) 
{
	//find zero, zero
	int red = SDL_MapRGB ( pSurface->format , 255,0,0 ) ;
	int xzero, yzero; int PlotX = 0, PlotHeight=MenagHeight, PlotWidth=MenagWidth;
	DoubleMenagCoordsToInt(settings, 0.0, 0.0, &xzero, &yzero);

	for (int y=0; y<PlotHeight; y++)
		plotpoint(pSurface, PlotX, y, red);
	if (xzero>PlotX && xzero<PlotX+PlotWidth)
		for (int y=0; y<PlotHeight; y++)
			plotpoint(pSurface, (xzero), y, red);
	for (int y=0; y<PlotHeight; y++)
		plotpoint(pSurface, PlotX+PlotWidth, y, red);

	for (int x=PlotX; x<PlotWidth+PlotX; x++)
		plotpoint(pSurface, x, 1, red);
	if (yzero>=0&& yzero<=PlotHeight)
		for (int x=PlotX; x<PlotWidth+PlotX; x++)
			plotpoint(pSurface, x, yzero, red);
	for (int x=PlotX; x<PlotWidth+PlotX; x++)
		plotpoint(pSurface, x, PlotHeight, red);
	
	//draw crosshairs
	int ia, ib;
	DoubleMenagCoordsToInt(settings, c1, c2, &ia, &ib);
	for (int x=ia-4; x<ia+5; x++)
		for (int y=ib-4; y<ib+5; y+=8)
				plotpoint(pSurface, x, y, red);
	for (int x=ia-4; x<ia+5; x+=8)
		for (int y=ib-4; y<ib+5; y++)
				plotpoint(pSurface, x, y, red);
	
}
//PhasePortraitSettings thephasesettings = {384,384, -3,3,-3,3, 0,1,0,1, 40,48,20, 0};
//PhasePortraitSettings thephasesettings = {384,384, -3,3,-3,3, 0,1,0,1, 15,100,100, 0};
//PhasePortraitSettings thephasesettings = {384,384, -3,3,-3,3, 0,1,0,1, 1,100,3000, 0};
PhasePortraitSettings phaseWithoutTransients = {384,384, -3,3,-3,3, -1.0,1.0,0.0,1.0, 10,7000,500, 0};
PhasePortraitSettings phaseWithTransients = {384,384, -3,3,-3,3, -FLT_EPSILON,FLT_EPSILON,-0.000001,0.000001, 1,100,1000, 0};
PhasePortraitSettings * currentphasesettings = &phaseWithoutTransients;
void togglePhasePortraitTransients() {currentphasesettings=(currentphasesettings==&phaseWithoutTransients)?&phaseWithTransients:&phaseWithoutTransients; }
void DrawPhasePortrait( SDL_Surface* pSurface, MenagFastSettings*mfastsettings, double c1, double c2 ) 
{


	//double sx0= -2, sx1=2, sy0= -2, sy1=2;
	double sx0= 0.01, sx1=2, sy0= 0.01, sy1=2;
	PhasePortraitSettings * settings = currentphasesettings;

	int nXpoints=settings->seedsPerAxis;
	int nYpoints=settings->seedsPerAxis;
	int height=settings->height;
	int width=settings->width;
	double sxinc = (nXpoints==1) ? 1e6 : (sx1-sx0)/(nXpoints-1);
	double syinc = (nYpoints==1) ? 1e6 : (sy1-sy0)/(nYpoints-1);

	double x_,x,y;
	double X0=settings->x0, X1=settings->x1, Y0=settings->y0, Y1=settings->y1;


	for (double sx=sx0; sx<=sx1; sx+=sxinc)
            {
                for (double sy=sy0; sy<=sy1; sy+=syinc)
                {
                    x = sx; y=sy;

					for (int ii=0; ii<(settings->settling); ii++)
                    {
						MAPEXPRESSION;
                        x=x_; 
						if (ISTOOBIG(x)||ISTOOBIG(y)) break;
                    }
					for (int ii=0; ii<(settings->drawing); ii++)
                    {
						MAPEXPRESSION;
                        x=x_;
						if (ISTOOBIG(x)||ISTOOBIG(y)) break;

                        int px = (int)(width * ((x - X0) / (X1 - X0)));
                        int py = (int)(height - height * ((y - Y0) / (Y1 - Y0)));
                        if (py >= 0 && py < height && px>=0 && px<width)
						{
							px += PhasePlotX;
							//get pixel color, mult by 0.875 (x-x>>3)
  Uint32 col = 0 ; Uint32 colR;
  
  char* pPosition = ( char* ) pSurface->pixels ; //determine position
  pPosition += ( pSurface->pitch * py ); //offset by y
  pPosition += ( pSurface->format->BytesPerPixel * px ); //offset by x
  memcpy ( &col , pPosition , pSurface->format->BytesPerPixel ) ; //copy pixel data

  SDL_Color color ;
  SDL_GetRGB ( col , pSurface->format , &color.r , &color.g , &color.b ) ;
  colR = color.r;
							
						// a quick mult, stops at 7, but whatever
						//int newcolor = (color.r)-((color.r)>>3);
						Uint32 newcolor = (colR)-((colR)>>2);
						//int newcolor = ((color.r)>>2)+((color.r)>>3); //5/8

  Uint32 newcol = SDL_MapRGB ( pSurface->format , newcolor , newcolor , newcolor ) ;
  memcpy ( pPosition , &newcol , pSurface->format->BytesPerPixel ) ;

				}
            }
        }
    }
			//BEGIN: draw fixed pt
	double a =c1, b=c2;
	 x = 1-b;  y=sqrt((1-b)*(a-1));
int ppx = (int)(width * ((x - X0) / (X1 - X0)));
int ppy = (int)(height - height * ((y - Y0) / (Y1 - Y0)));
if (ppy >= 0 && ppy < height && ppx>=0 && ppx<width)
{
	ppx += PhasePlotX;
	for (int px=ppx-2; px<ppx+2; px++) {
		int py = ppy;
  char* pPosition = ( char* ) pSurface->pixels ; //determine position
  pPosition += ( pSurface->pitch * py ); //offset by y
  pPosition += ( pSurface->format->BytesPerPixel * px ); //offset by x

    Uint32 newcol = SDL_MapRGB ( pSurface->format , 255 , 0 , 0 ) ;
  memcpy ( pPosition , &newcol , pSurface->format->BytesPerPixel ) ;
  //END: draw fixed pt
	}
}
 y=-sqrt((1-b)*(a-1));
 ppx = (int)(width * ((x - X0) / (X1 - X0)));
 ppy = (int)(height - height * ((y - Y0) / (Y1 - Y0)));
if (ppy >= 0 && ppy < height && ppx>=0 && ppx<width)
{
	ppx += PhasePlotX;
	for (int px=ppx-2; px<ppx+2; px++) {
		int py = ppy;
  char* pPosition = ( char* ) pSurface->pixels ; //determine position
  pPosition += ( pSurface->pitch * py ); //offset by y
  pPosition += ( pSurface->format->BytesPerPixel * px ); //offset by x

    Uint32 newcol = SDL_MapRGB ( pSurface->format , 255 , 0 , 0 ) ;
  memcpy ( pPosition , &newcol , pSurface->format->BytesPerPixel ) ;
  //END: draw fixed pt
	}
}
return;
}

void zoomPortrait(int direction, MenagFastSettings * fmenagsettings )
{
	PhasePortraitSettings * settings = currentphasesettings;
	double fcenterx, fcentery;
	fcenterx= (settings->x1+settings->x0)/2;
	fcentery= (settings->y1+settings->y0)/2;
	double fwidth=settings->x1-settings->x0, fheight=settings->x1-settings->x0;
	if (direction==-1) {fwidth *= 1.25; fheight*=1.25;}
	else {fwidth *= 0.8; fheight*=0.8;}
	settings->x0 = fcenterx - fwidth/2;
	settings->x1 = fcenterx + fwidth/2;
	settings->y0 = fcentery - fheight/2;
	settings->y1 = fcentery + fheight/2;

}

