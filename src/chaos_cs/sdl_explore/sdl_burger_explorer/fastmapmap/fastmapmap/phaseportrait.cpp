
#include "phaseportrait.h"
#include "float_cast.h"
#include "whichmap.h"
#include "palette.h"
#include <assert.h>


//note that these don't use the a and b from g_settings, they take separate one.
void DrawPhasePortrait( SDL_Surface* pSurface, double c1, double c2, int width ) 
{
	double sx0= g_settings->seedx0, sx1=g_settings->seedx1, sy0= g_settings->seedy0, sy1=g_settings->seedy1;
	int nXpoints=g_settings->seedsPerAxis;
	int nYpoints=g_settings->seedsPerAxis;
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

void DrawBasins( SDL_Surface* pSurface, double c1, double c2, int width) 
{
	double fx,fy, x_,y_,x,y; char* pPosition;
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
		/*double distance = sqrt( (x-fx)*(x-fx)+(y-fx)*(y-fx)) / 20;
		if (y<0) distance *= -1;
		double val = distance;*/
		double val;
		if (ISTOOBIG(x)||ISTOOBIG(y))
			val=1.0;
		else{
			//double diffx = (x) - (c1*x - y*y);
			//double diffy = (y) - (c2*y + x*y);
            val = sqrt(fabs(x));
			if (y<0) val*=.8;
		}

		if (val>1.0) val=1.0; if (val<0.0) val=0.0;
		val = val*2 - 1; //from -1 to 1
		Uint32 r,g,b;
		if (val<=0)
			b=255, r=g= (Uint32) ((1+val)*255.0);
		else
			r=g=b= (Uint32) ((1-val)*255.0);
			

  pPosition = ( char* ) pSurface->pixels ; //determine position
  pPosition += ( pSurface->pitch * (py) ); //offset by y
  pPosition += ( pSurface->format->BytesPerPixel * (px) ); //offset by x
  Uint32 newcol = SDL_MapRGB ( pSurface->format , r , g , b ) ;
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
		case DrawModeBasins:  DrawBasins(pSurface, c1, c2, width); break;
		case DrawModeColorLine:	DrawColorsLine(pSurface, c1, c2, width, FALSE); break;
		case DrawModeColorLineJoin:	DrawColorsLine(pSurface, c1, c2, width, TRUE); break;
		case DrawModeColorDisk:	DrawColorsDisk(pSurface, c1, c2, width); break;
		default: {assert(0); exit(1); }
	}
}
