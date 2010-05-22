#include "phaseportrait.h"
#include "float_cast.h"
#include "whichmap.h"
#include "palette.h"
#include <assert.h>

//note that these don't use the a and b from g_settings, they take separate one.
void DrawPhasePortrait( SDL_Surface* pSurface, double c1, double c2, int width ) 
{
	double sx0= g_settings->seedx0, sx1=g_settings->seedx1, sy0= g_settings->seedy0, sy1=g_settings->seedy1;
	int nXpoints=g_settings->seedsPerAxis; int nYpoints=g_settings->seedsPerAxis;
	int height=width;
	double sxinc = (nXpoints==1) ? 1e6 : (sx1-sx0)/(nXpoints-1);
	double syinc = (nYpoints==1) ? 1e6 : (sy1-sy0)/(nYpoints-1);

	double x_,y_,x,y;
	double X0=g_settings->x0, X1=g_settings->x1, Y0=g_settings->y0, Y1=g_settings->y1;

	for (double sx=sx0; sx<=sx1; sx+=sxinc)
            {
                for (double sy=sy0; sy<=sy1; sy+=syinc)
                {
                    x = sx; y=sy;

					for (int ii=0; ii<(g_settings->settlingTime); ii++)
                    {
						MAPEXPRESSION;
                        x=x_; y=y_;
						if (ISTOOBIG(x)||ISTOOBIG(y)) break;
                    }
					for (int ii=0; ii<(g_settings->drawingTime); ii++)
                    {
						MAPEXPRESSION;
                        x=x_; y=y_;
						if (ISTOOBIG(x)||ISTOOBIG(y)) break;

                        int px = lrint(width * ((x - X0) / (X1 - X0)));
                        int py = lrint(height - height * ((y - Y0) / (Y1 - Y0)));
                        if (py >= 0 && py < height && px>=0 && px<width)
						{
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
}

//double largestSeenBasins = 1e-6; didn't look good.

BOOL bDrawBasinsWithBlueAlso=FALSE;
void DrawBasins( SDL_Surface* pSurface, double c1, double c2, int width) 
{
double fx,fy, x_,y_,x,y; char* pPosition; Uint32 r,g,b, newcol; double val;
int height=width;
double X0=g_settings->x0, X1=g_settings->x1, Y0=g_settings->y0, Y1=g_settings->y1;
double dx = (X1 - X0) / width, dy = (Y1 - Y0) / height;
fx = X0; fy = Y1; //y counts downwards
for (int py=0; py<height; py+=1)
{
	fx=X0;
	for (int px = 0; px < width; px+=1)
	{
		x=fx; y=fy;
		for (int i=0; i<g_settings->basinsTime; i++)
		{
			MAPEXPRESSION;
			x=x_; y=y_;
			if (ISTOOBIG(x)||ISTOOBIG(y)) break;
		}

		if (g_settings->drawingMode==DrawModeBasinsDistance)
		{
			val = sqrt( (x-fx)*(x-fx)+(y-fx)*(y-fx));
		}
		else if (g_settings->drawingMode==DrawModeBasinsX)
		{
			val = sqrt(fabs(x));
		}
		else if (g_settings->drawingMode==DrawModeBasinsDifference)
		{
			double prevx = x; MAPEXPRESSION;
			val = fabs(x_-prevx);
		}
		if (ISTOOBIG(x)||ISTOOBIG(y)) { 
			newcol = SDL_MapRGB( pSurface->format , 0 , 0, 35 ) ; 
		}
		else
		{
			val = val / g_settings->basinsMaxColor;
			if (!bDrawBasinsWithBlueAlso) {
				if (val>=1.0)
					newcol = SDL_MapRGB( pSurface->format , 220 , 220, 255 );
				else {
					int v = (int)(val*255);
					newcol = SDL_MapRGB( pSurface->format , v,v,v );
				}
			}else {
				if (val>=1.0) val=1.0;
				val=val*2-1;
				if (val<=0)
					b=255, r=g= (Uint32) ((1-val)*255.0);
				else
					r=g=b= (Uint32) ((1-val)*255.0);
				newcol = SDL_MapRGB( pSurface->format , r,g,b );
			}
		}
		
		
		pPosition = ( char* ) pSurface->pixels ; //determine position
		pPosition += ( pSurface->pitch * py ); //offset by y
		pPosition += ( pSurface->format->BytesPerPixel * px ); //offset by x
		memcpy ( pPosition , &newcol , pSurface->format->BytesPerPixel ) ;

		fx += dx;
	}
fy -= dy;
}
}

BOOL bMoreQuadrantContrast=FALSE;
void DrawBasinsQuadrant( SDL_Surface* pSurface, double c1, double c2, int width) 
{
int col1 = HSL2RGB(pSurface, 0.60277, 1.0, .45);
int col2 = HSL2RGB(pSurface, 0.69444, 1.0, .45);
int col3 = HSL2RGB(pSurface, 0.133, 1.0, .45);
int col4 = HSL2RGB(pSurface, 0.1055, 1.0, .45);
if (bMoreQuadrantContrast) { int tmp=col2; col2=col4; col4=tmp; }

double fx,fy, x_,y_,x,y; char* pPosition; Uint32 r,g,b, newcol; double hue;
int height=width;
double X0=g_settings->x0, X1=g_settings->x1, Y0=g_settings->y0, Y1=g_settings->y1;
double dx = (X1 - X0) / width, dy = (Y1 - Y0) / height;
fx = X0; fy = Y1; //y counts downwards
for (int py=0; py<height; py+=1)
{
	fx=X0;
	for (int px = 0; px < width; px+=1)
	{
		x=fx; y=fy;
		for (int i=0; i<g_settings->basinsTime; i++)
		{
			MAPEXPRESSION;
			x=x_; y=y_;
			if (ISTOOBIG(x)||ISTOOBIG(y)) break;
		}

		if (ISTOOBIG(x)||ISTOOBIG(y)) { 
			newcol = SDL_MapRGB ( pSurface->format , 0 , 0, 25 ) ; 
		} else {
			if (x>0 && y>0) newcol = col1;
			else if (x>0 && y<0) newcol = col2;
			else if (x<0 && y>0) newcol = col3;
			else if (x<0 && y<0) newcol = col4;
			//hue = (hue+g_settings->basinsHueShift); if (hue>1.0) hue-=1.0;
		}
		pPosition = ( char* ) pSurface->pixels ; //determine position
		pPosition += ( pSurface->pitch * py ); //offset by y
		pPosition += ( pSurface->format->BytesPerPixel * px ); //offset by x
		memcpy ( pPosition , &newcol , pSurface->format->BytesPerPixel ) ;

		fx += dx;
	}
fy -= dy;
}
}

void DrawFigure( SDL_Surface* pSurface, double c1, double c2, int width ) 
{
	switch (g_settings->drawingMode)
	{
		case DrawModePhase:  DrawPhasePortrait(pSurface, c1, c2, width); break;
		case DrawModeBasinsDistance:  
		case DrawModeBasinsDifference:  
		case DrawModeBasinsX:  
			DrawBasins(pSurface, c1, c2, width); break;
		case DrawModeBasinsQuadrant:  DrawBasinsQuadrant(pSurface, c1, c2, width); break;
		case DrawModeColorLine:	DrawColorsLine(pSurface, c1, c2, width); break;
		case DrawModeColorDisk:	DrawColorsDisk(pSurface, c1, c2, width); break;
		default: {assert(0); exit(1); }
	}
}

void RenderLargeFigure( SDL_Surface* pSurface, int width, const char*filename ) 
{
	char filenameext[256];
	snprintf(filenameext, sizeof(filenameext), "%s.bmp", filename);
//create a new surface.
	SDL_Surface* pRenderSurface = SDL_CreateRGBSurface( SDL_SWSURFACE, width, width, pSurface->format->BitsPerPixel, pSurface->format->Rmask, pSurface->format->Gmask, pSurface->format->Bmask, 0 );
	SDL_FillRect ( pRenderSurface , NULL , g_white );
	//increase the amount of drawing.
	int oldDrawingTime = g_settings->drawingTime;
	g_settings->drawingTime *= 32;
	DrawFigure(pRenderSurface, g_settings->a, g_settings->b, width);
	g_settings->drawingTime = oldDrawingTime;
	SDL_SaveBMP(pRenderSurface, filenameext);
	SDL_FreeSurface(pRenderSurface);
}

