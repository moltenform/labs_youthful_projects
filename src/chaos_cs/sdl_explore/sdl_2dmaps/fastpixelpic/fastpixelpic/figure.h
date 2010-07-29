
#include "common.h"
#include "coordsdiagram.h" //maybe not the best architecturally


#define ColorModeBlackWhite 0
#define ColorModeRainbow 1
#define ColorModeBlackWhiteBlue 2
//for sqrt ones, do that in code.

#define ColorWrappingTrunc 0
#define ColorWrappingCycle 1
#define ColorWrappingTruncWarning 2


/*enum {
  maskOptionsSmearRectangle = 0x1<<31, //Smear line or whole rectangle?
} ;*/


void DrawFigure( SDL_Surface* pSurface, int width ); 
void renderLargeFigure( SDL_Surface* pSurface, int width, const char* filename ) ;

#define ISTOOBIG(x) ((x)<-1e2 || (x)>1e2)
#define ISTOOBIGF(x) ((x)<-1e2f || (x)>1e2f)
#define ISBIGPOS(x) ((x)>5e2)
#define ISBIGNEG(x) ((x)<-5e2)
