
int HSL2RGB(SDL_Surface* pSurface, double h, double sl, double l);
//HSLRGB from http://www.geekymonkey.com/Programming/CSharp/RGB2HSL_HSL2RGB.htm

void switchPalette(SDL_Surface* pSurface);
void DrawColorsLine( SDL_Surface* pSurface, double c1, double c2, int width ) ;
void DrawColorsDisk( SDL_Surface* pSurface, double c1, double c2, int width ); 


extern bool bShowOnlyOneColorLine;
