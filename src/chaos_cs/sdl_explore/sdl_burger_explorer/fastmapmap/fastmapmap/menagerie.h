
#include "common.h"
#include "coordsdiagram.h"
void BlitDiagram(SDL_Surface* pSurface,SDL_Surface* pSmallSurface, int px, int py);
void DrawMenagerie( SDL_Surface* pMSurface, CoordsDiagramStruct*diagram) ;
void DrawMenagerieFromPrecomputed( SDL_Surface* pSmallSurface, CoordsDiagramStruct*diagram) ;
BOOL CreateMenagCache( SDL_Surface* pSurface );


