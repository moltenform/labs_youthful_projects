
#pragma once

#include "SDL.h"
#include "common.h"

void DrawPlotGrid( SDL_Surface* pSurface, PhasePortraitSettings*settings, double c1, double c2 );
void InitialSettings(PhasePortraitSettings*settings, int width, int height, double *outA, double *outB);
void DrawPhasePortrait( SDL_Surface* pSurface, PhasePortraitSettings*settings, double c1, double c2 );
void DrawBasins( SDL_Surface* pSurface, PhasePortraitSettings*settings, double c1, double c2 );

#define ISTOOBIG(x) ((x)<-1e3 || (x)>1e3)
//floating point comparison. see also <float.h>'s DBL_EPSILON and DBL_MIN. 1e-11 also ok.
#define VERYCLOSE(x1,x2) (fabs((x1)-(x2))<1e-8)


#ifdef _MSC_VER //using Msvc
#include <float.h>
#define ISFINITE(x) (_finite(x))
#else
#define ISFINITE(x) (isfinite(x))
#endif

//Apparently usually in format UURRGGBB where UU is unused
//or something like this... don't use it because makes too many assumptions, not portable.
#define ASSUME32BITHACKGETR(col) (((col) & 0x0000ff00)>>8)
#define ASSUME32BITHACKSET(r,g,b) (((r)<<8)+((b)<<16)+(g)) 

