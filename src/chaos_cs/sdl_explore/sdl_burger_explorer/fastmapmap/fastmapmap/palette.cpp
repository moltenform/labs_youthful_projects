
#include "common.h"
#include "palette.h"
#include "float_cast.h"
#include "whichmap.h"
#include <math.h>
int ColorPalette[1024];

void switchPalette(SDL_Surface* pSurface)
{
	static int whichPallete=0;
	whichPallete = (whichPallete+1)%2 ;
	if (whichPallete == 0) //black/white
	{
		for (int i=0; i<1024; i++)
		{
			int v = (int)(255.0*i/1024.0);
			ColorPalette[i] = SDL_MapRGB ( pSurface->format , v,v,v );
		}
		ColorPalette[0] = SDL_MapRGB ( pSurface->format , 45,0,0 );
	}
	else if (whichPallete == 1) //rainbow
	{
		for (int i=0; i<1024; i++)
		{
			double h = (i/1024.0);
			ColorPalette[i] = HSL2RGB(pSurface, h, 0.5, 0.5);
		}
	}
	else if (whichPallete == 2) //black/blue
	{
		/*val = val*2 - 1; //from -1 to 1
		Uint32 r,g,b;
		if (val<=0)
			b=255, r=g= (Uint32) ((1+val)*255.0);
		else
			r=g=b= (Uint32) ((1-val)*255.0);

		for (int i=0; i<1024/2; i++)
		{
			int v = (int)(255.0*i/(1024.0/2));
			ColorPalette[i] = SDL_MapRGB ( pSurface->format , 255,255-v,255-v );
		}
		for (int i=0; i<1024/2; i++)
		{
			int v = (int)(255.0*i/(1024.0/2));
			ColorPalette[i] = SDL_MapRGB ( pSurface->format , 255,255-v,255-v );
		}*/
	}
}


void DrawColorsLine( SDL_Surface* pSurface, double c1, double c2, int width, BOOL bJoin ) 
{
	int iter = g_settings->colorsStep;
	if (iter<0) return;
	double X0=g_settings->x0, X1=g_settings->x1, Y0=g_settings->y0, Y1=g_settings->y1;
	int height=width;

	double PI = 3.14159, x_, y_; int px,py;
	double xradius=1.0, yradius=1.0;//double xradius=0.5, yradius=0.5; w henon cool
	for (double t=0; t<1.0; t+=0.001)
	{
		int color = HSL2RGB(pSurface,t, .5,.5);
		double x = cos(t * 2*PI)*xradius;
		double y = sin(t * 2*PI)*yradius;

		if (iter==1) { //special case: when iter==1 we show both circle and first iter
			px = lrint(width * ((x - X0) / (X1 - X0)));
            py = lrint(height - height * ((y - Y0) / (Y1 - Y0)));
			plotpointcolor(pSurface, px,py,color);
		}
		for (int i=0; i<iter; i++) { 
			if (bJoin && i!=0) 
			{
				px = lrint(width * ((x - X0) / (X1 - X0)));
				py = lrint(height - height * ((y - Y0) / (Y1 - Y0)));
				plotpointcolor(pSurface, px,py,color);
			}
			MAPEXPRESSION;
			x=x_; y=y_;
		}
		px = lrint(width * ((x - X0) / (X1 - X0)));
        py = lrint(height - height * ((y - Y0) / (Y1 - Y0)));
		plotpointcolor(pSurface, px,py,color);
	}
}

void DrawColorsDisk( SDL_Surface* pSurface, double c1, double c2, int width ) 
{
	int iter = g_settings->colorsStep;
	if (iter<0) return;
	double X0=g_settings->x0, X1=g_settings->x1, Y0=g_settings->y0, Y1=g_settings->y1;
	int height=width;

	double PI = 3.14159, x_, y_; int px,py;
	double xradius=1, yradius=1;//double xradius=0.5, yradius=0.5; w henon cool
	for (double t=0; t<1.0; t+=0.001)
	{
		for (double r=0; r<xradius; r+=0.005)
		{
			double lum = (r/xradius)*0.7;
			int color = HSL2RGB(pSurface,t, .5,lum);
			double x = cos(t * 2*PI)*r;
			double y = sin(t * 2*PI)*r;

			for (int i=0; i<iter; i++) { 
			MAPEXPRESSION;
			x=x_; y=y_;
			}
			px = lrint(width * ((x - X0) / (X1 - X0)));
			py = lrint(height - height * ((y - Y0) / (Y1 - Y0)));
			plotpointcolor(pSurface, px,py,color);
		}
	}
}

// Given H,S,L in range of 0-1
// Returns a Color (RGB struct) in range of 0-255
int HSL2RGB(SDL_Surface* pSurface, double h, double sl, double l)
{
    double v;
    double r,g,b;

    r = l;   // default to gray
    g = l;
    b = l;
    v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);
    if (v > 0)
    {
          double m;
          double sv;
          int sextant;
          double fract, vsf, mid1, mid2;

          m = l + l - v;
          sv = (v - m ) / v;
          h *= 6.0;
          sextant = (int)h;
          fract = h - sextant;
          vsf = v * sv * fract;
          mid1 = m + vsf;
          mid2 = v - vsf;
          switch (sextant)
          {
                case 0:
                      r = v;
                      g = mid1;
                      b = m;
                      break;
                case 1:
                      r = mid2;
                      g = v;
                      b = m;
                      break;
                case 2:
                      r = m;
                      g = v;
                      b = mid1;
                      break;
                case 3:
                      r = m;
                      g = mid2;
                      b = v;
                      break;
                case 4:
                      r = mid1;
                      g = m;
                      b = v;
                      break;
                case 5:
                      r = v;
                      g = m;
                      b = mid2;
                      break;
          }
    }
    int ir,ig,ib;
    ir = (int)(r * 255.0f);
    ig = (int)(g * 255.0f);
    ib = (int)(b * 255.0f);
	if (!pSurface)
		return (ir<<16) + (ig<<8) + ib;
	else
    return SDL_MapRGB ( pSurface->format , ir,ig,ib ) ;
	//(ir<<16) + (ig<<8) + ib;
}
