
#include "common.h"
#include "palette.h"
#include "float_cast.h"
#include "whichmap.h"
#include "phaseportrait.h"
#include <math.h>

//HSLRGB from http://www.geekymonkey.com/Programming/CSharp/RGB2HSL_HSL2RGB.htm

//source: http://www.geekymonkey.com/Programming/CSharp/RGB2HSL_HSL2RGB.htm
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
