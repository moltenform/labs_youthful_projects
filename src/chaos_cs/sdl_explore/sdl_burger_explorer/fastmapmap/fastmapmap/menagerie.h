
#include "common.h"
#include "coordsdiagram.h"
void BlitDiagram(SDL_Surface* pSurface,SDL_Surface* pSmallSurface, int px, int py);
void DrawMenagerieMultithreaded( SDL_Surface* pMSurface, CoordsDiagramStruct*diagram) ;
void DrawMenagerie( SDL_Surface* pMSurface, CoordsDiagramStruct*diagram) ;
BOOL CreateMenagCache( SDL_Surface* pSurface );
void toggleMenagerieMode();

