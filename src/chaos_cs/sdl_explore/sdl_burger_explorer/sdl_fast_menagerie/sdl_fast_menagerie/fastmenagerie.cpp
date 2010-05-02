
#include "common.h"
#include "phaseportrait.h"
#include "fastmenagerie.h"

BOOL g_BusyThread1 = FALSE;
BOOL g_BusyThread2 = FALSE;
void InitialSettings(MenagFastSettings*ps, int width, int height, double *pa, double *pb)
{
	ps->menagSeedsPerAxis = 40;
	ps->menagSettling = 48;
	ps->menagDrawing = 20; //also, # of iters for the Basins mode.
	ps->browsex0 = 0; ps->browsex1 = 1; ps->browsey0=0; ps->browsey1 = 1;
	ps->x0 = 0; ps->x1 = 1; ps->y0=0; ps->y1 = 1;
	
	if (StringsEqual(STRINGIFY(MAPEXPRESSION), STRINGIFY(BURGER)))
	{
		*pa = -1.1; *pb = 1.72;
		ps->browsex0 = -2; ps->browsex1 = 2; ps->browsey0=-0.5; ps->browsey1 = 3.5;
		ps->x0 = -1.75; ps->x1 = 1.75; ps->y0=-1.75; ps->y1 = 1.75;
		ps->seedx0 = -3; ps->seedx1 = 1; ps->seedy0=0 /*it's symmetrical */; ps->seedy1 = 3;
	}
	else //HENON MAP
	{
		*pa = 1.4; *pb = 0.3;
		ps->browsex0 = -2; ps->browsex1 = 2; ps->browsey0=-1; ps->browsey1 = 3;
		ps->x0 = -1.75; ps->x1 = 1.75; ps->y0=-1.75; ps->y1 = 1.75;
		ps->seedx0 = -3; ps->seedx1 = 3; ps->seedy0=-3 ; ps->seedy1 = 3;
		
	}

}

void FastFastMenagerie(MenagFastSettings*ps, SDL_Surface* pSmallSurface)
{
	SDL_FillRect ( pSmallSurface , NULL , 343223 ); 
	

}



void BlitMenagerie(SDL_Surface* pSurface,SDL_Surface* pSmallSurface)
{
	SDL_Rect src, dest;
	src.x = 0;
	src.y = 0;
	src.w = MenagWidth;
	src.h = MenagHeight;
	dest.x = 0;
	dest.y = 0;
	dest.w = MenagWidth;
	dest.h = MenagHeight; 
	SDL_BlitSurface(pSmallSurface, &src, pSurface, &dest);
}
