
#include "common.h"
#include "coordsdiagram.h" //maybe not the best architecturally


#define ColorModeBlackGray 10
#define ColorModeBlackBlue 15
#define ColorModeBlackBlueSqrt 16
#define ColorModeRainbow 20


/*enum {
  maskOptionsSmearRectangle = 0x1<<31, //Smear line or whole rectangle?
} ;*/


void DrawFigure( SDL_Surface* pSurface, int width ); 
void renderLargeFigure( SDL_Surface* pSurface, int width, const char* filename ) ;

#define ISTOOBIG(x) ((x)<-1e2 || (x)>1e2)
#define ISTOOBIGF(x) ((x)<-1e2f || (x)>1e2f)
