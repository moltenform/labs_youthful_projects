
int HSL2RGB(SDL_Surface* pSurface, double h, double sl, double l);
void switchPalette(SDL_Surface* pSurface);
void DrawColorsLine( SDL_Surface* pSurface, double c1, double c2, int width, BOOL bJoin ) ;
void DrawColorsDisk( SDL_Surface* pSurface, double c1, double c2, int width ); 

extern int ColorPalette[1024];
