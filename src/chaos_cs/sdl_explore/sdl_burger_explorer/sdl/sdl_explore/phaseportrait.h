#pragma once

#include "common.h"


#define HENON x_ = 1 - c1*x*x + y; y = c2*x;
#define HENONINIT exprSettings(/*a,b:*/ 1.4,0.3,/*Range of a,b*/ -2,2,-1,3, /*Range of x,y*/ -1.75,1.75,-1.75,1.75,FALSE,ps,pa,pb)
#define BURGER x_ = c1*x - y*y; y= c2*y + x*y;
#define BURGERINIT exprSettings(/*a,b:*/-1.1,1.72,/*Range of a,b*/ -2,2,-.5,3.5, /*Range of x,y*/ -1.75,1.75,-1.75,1.75,TRUE,ps,pa,pb)

#define MAPEXPRESSION HENON
#define INITEXPRESSION HENONINIT



//#define BURGERCHH x_ = fabs(c1*x - y*y); y= c2*y + x*y;
//#define NEW if (rand()&0x01) {x_ = c1*x - y*y; y= c2*y + x*y;} else {x_ = c1*2*x - y*y; y= c2*3*y + x*y;}
//#define GINGERBREAD x_ = 1-y+c1*fabs(x); y=c2*x;

//a benefit of having MAPEXPRESSION as a macro is that it can be STRINGIFYd when saving.
#define STRINGIFY2( x) #x
#define STRINGIFY(x) STRINGIFY2(x)
#define MAPEXPRESSIONTEXT STRINGIFY(MAPEXPRESSION)


extern BOOL g_bMapIsSymmetricalXAxis;
void DrawPlotGrid( SDL_Surface* pSurface, PhasePortraitSettings*settings, double c1, double c2 );
void InitialSettings(PhasePortraitSettings*settings, int width, int height, double *outA, double *outB);
void DrawPhasePortrait( SDL_Surface* pSurface, PhasePortraitSettings*settings, double c1, double c2 );
void DrawBasins( SDL_Surface* pSurface, PhasePortraitSettings*settings, double c1, double c2 );

#define ISTOOBIG(x) ((x)<-1e2 || (x)>1e2)
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

