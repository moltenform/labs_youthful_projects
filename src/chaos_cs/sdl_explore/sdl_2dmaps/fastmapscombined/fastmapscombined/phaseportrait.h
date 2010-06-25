
#include "common.h"
#include "coordsdiagram.h" //maybe not the best architecturally


#define DrawModeLeavesPhase 1
#define DrawModeAlternatePhase 10
#define DrawModeRandomPhase 11
#define DrawModeSmearLine 20
#define DrawModeSmearRectangle 21
#define DrawModePhasefircate 30
#define DrawModePhasefircateHoriz 31
#define DrawModeStandardPhase 90
#define DrawModeStandardBasins 91


extern double gParamAcquireCoord;

enum {
  maskOptionsSmearRectangle = 0x1<<31, //Smear line or whole rectangle?
 

} ;


void DrawFigure( SDL_Surface* pSurface, double c1, double c2, int width, int px ); 
void renderLargeFigure( SDL_Surface* pSurface, int width, const char* filename ) ;
void DrawBasinsBasic( SDL_Surface* pSurface, double c1, double c2, CoordsDiagramStruct * diagram); 

#define ISTOOBIG(x) ((x)<-1e2 || (x)>1e2)
#define ISTOOBIGF(x) ((x)<-1e2f || (x)>1e2f)
