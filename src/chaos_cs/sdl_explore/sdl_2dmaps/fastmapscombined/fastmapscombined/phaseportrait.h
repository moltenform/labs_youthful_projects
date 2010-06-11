
#include "common.h"


#define DrawModeLeavesPhase 1
#define DrawModeAlternatePhase 10
#define DrawModeRandomPhase 11
#define DrawModeSmearLine 20
#define DrawModeSmearRectangle 21
#define DrawModePhasefircate 30
#define DrawModePhasefircateHoriz 31
#define DrawModeStandardPhase 90
#define DrawModeStandardBasins 91

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
