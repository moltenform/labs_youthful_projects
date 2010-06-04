#pragma once

#include "common.h"
#include "fastmenagerie.h"


#define HENON x_ = 1 - c1*x*x + y; y = c2*x;
#define BURGER x_ = c1*x - y*y; y= c2*y + x*y;

#define MAPEXPRESSION BURGER
#define SSESETUP burgerSetup
#define SSEEXPRESSION burgerExpression


void DrawPlotGrid( SDL_Surface* pSurface, MenagFastSettings*settings, double c1, double c2 );
void DrawPhasePortrait( SDL_Surface* pSurface, MenagFastSettings*mfastsettings, double c1, double c2 ) ;
void togglePhasePortraitTransients();

//a benefit of having MAPEXPRESSION as a macro is that it can be STRINGIFYd when saving.
#define STRINGIFY2( x) #x
#define STRINGIFY(x) STRINGIFY2(x)
#define MAPEXPRESSIONTEXT STRINGIFY(MAPEXPRESSION)


#define ISTOOBIG(x) ((x)<-1e2 || (x)>1e2)
#define ISTOOBIGF(x) ((x)<-1e2f || (x)>1e2f)
//floating point comparison. see also <float.h>'s DBL_EPSILON and DBL_MIN. 1e-11 also ok.
#define VERYCLOSE(x1,x2) (fabs((x1)-(x2))<1e-8)


#ifdef _MSC_VER //using Msvc
#include <float.h>
#define ISFINITE(x) (_finite(x))
#else
#define ISFINITE(x) (isfinite(x))
#endif


