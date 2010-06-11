
#include "common.h"

#define DrawModePhase 1
#define DrawModeColorLine 2
#define DrawModeColorDisk 3
#define DrawModeBasinsDistance 10
#define DrawModeBasinsX 11
#define DrawModeBasinsDifference 12
#define DrawModeBasinsQuadrant 13
#define DrawModeEscapeTimeLines 20
#define DrawModeEscapeTime 21

enum {
  maskOptionsDiagramMethod = 0x1<<31, //Diagram is lyapunov or countpixels
  maskOptionsDiagramColoring = 0x1<<30, //Diagram is hsl or black/white
  maskOptionsBasinColor = 0x1<<29, //Basin colors include blue.
  maskOptionsQuadrantContrast = 0x1<<28, //Contrast in quadrants.
  maskOptionsEscapeFillIn = 0x1<<27, //Fill in basin when in escapetime mode.
  maskOptionsEscapeAdditionalPass = 0x1<<26, //In escapetime mode draw more lines
  maskOptionsColorShowJustOneLine = 0x1<<25 //when in color mode, show just one line.

} ;


void DrawFigure( SDL_Surface* pSurface, double c1, double c2, int width, int px ); 
void renderLargeFigure( SDL_Surface* pSurface, int width, const char* filename ) ;

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
