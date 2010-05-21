
#include "common.h"

#define DrawModePhase 1
#define DrawModeBasins 2
#define DrawModeColorLine 3
#define DrawModeColorLineJoin 4
#define DrawModeColorDisk 5

void DrawFigure( SDL_Surface* pSurface, double c1, double c2, int width ) ;


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
