
#include "common.h"

#define DrawModePhase 1
#define DrawModeColorLine 2
#define DrawModeColorDisk 3
#define DrawModeBasinsDistance 10
#define DrawModeBasinsX 11
#define DrawModeBasinsDifference 12
#define DrawModeBasinsQuadrant 13

void DrawFigure( SDL_Surface* pSurface, double c1, double c2, int width, int px0 ) ;
void renderLargeFigure( SDL_Surface* pSurface, int width, const char* filename ) ;
extern BOOL gParamDrawBasinsWithBlueAlso;

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
