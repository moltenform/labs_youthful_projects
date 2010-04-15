
#include "breathe.h"
#include "common.h"
#include "font.h"
#include "phaseportrait.h"
#include <math.h>


void oscillate(double curA,double curB,double *outA, double *outB);


int dofullscreen(SDL_Surface* pSurface, BOOL breathe, PhasePortraitSettings * settings, double *targetA, double *targetB)
{
	double curA=0.0, curB=0.0;

	SDL_Event event;
	double sliding = (settings->drawBasin) ? 2 : 10.0;

while (TRUE)
{
	curA += (*targetA-curA)/sliding;
	curB += (*targetB-curB)/sliding;

	if ( SDL_PollEvent ( &event ) )
	{
	//an event was found
	if ( event.type == SDL_QUIT ) return 0;
	else if (event.type==SDL_MOUSEBUTTONDOWN) return 0;
	else if (event.type==SDL_KEYDOWN){
		
		switch(event.key.keysym.sym)
		{
			case SDLK_UP: *targetB += (event.key.keysym.mod & KMOD_SHIFT) ? 0.0005 : 0.005; break;
			case SDLK_DOWN: *targetB -= (event.key.keysym.mod & KMOD_SHIFT) ? 0.0005 : 0.005; break;
			case SDLK_LEFT: *targetA -= (event.key.keysym.mod & KMOD_SHIFT) ? 0.0005 : 0.005; break;
			case SDLK_RIGHT: *targetA += (event.key.keysym.mod & KMOD_SHIFT) ? 0.0005 : 0.005; break;
			default: break;
		}
	  }
	else if (event.type==SDL_KEYUP)
	  {
		switch(event.key.keysym.sym)
		{
			case SDLK_UP:
			case SDLK_DOWN: 
			case SDLK_LEFT: 
			case SDLK_RIGHT:
			case SDLK_f: 
			case SDLK_RALT:
			case SDLK_LALT:
			case SDLK_RSHIFT: 
			case SDLK_LSHIFT:
			case SDLK_RCTRL:
			case SDLK_LCTRL:
				break;
			default: 
				return 0; //return back to the other screen!
				break;
		}
	}
	}

	if (LockFramesPerSecond()) 
	{
		if (!breathe && VERYCLOSE(curA,*targetA) && VERYCLOSE(curB,*targetB))
		{
			// don't need to compute anything.
		}
		else
		{
			SDL_FillRect ( pSurface , NULL , g_white );  //clear surface quickly
			if (SDL_MUSTLOCK(pSurface)) SDL_LockSurface ( pSurface ) ;
			if (breathe)
			{
				double oa, ob;
				oscillate(curA, curB, &oa, &ob);
				DrawPhasePortrait(pSurface, settings, oa,ob);
			}
			else
			{
				DrawPhasePortrait(pSurface, settings, curA,curB);
			}
			if (SDL_MUSTLOCK(pSurface)) SDL_UnlockSurface ( pSurface ) ;
		}
		


	}

	SDL_UpdateRect ( pSurface , 0 , 0 , 0 , 0 ) ;  //apparently needed every frame, even when not redrawing

}
return 0;
}


void oscillate(double curA,double curB,double *outA, double *outB)
{
	static double t=0;
	t+=0.13;
	if (t>3141.5926) t=0.0;

	*outA = curA + sin( t +0.03*cos(t/8.5633) +3.685)/200;
	*outB = curB + cos( 0.8241*t +0.02*sin(t/9.24123+5.742) )/263;
}
int fullscreen(SDL_Surface* pSurface, BOOL breathe, PhasePortraitSettings * settings, double *ptrtargetA, double *ptrtargetB)
{
	int prev = settings->width;
	settings->width = settings->height = 600;
	int ret = dofullscreen(pSurface, breathe, settings, ptrtargetA, ptrtargetB);
	settings->width = settings->height = prev;
	return ret;
}