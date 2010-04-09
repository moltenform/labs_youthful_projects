
#include "breathe.h"
#include "common.h"
#include "sdl.h"
#include "phaseportrait.h"
#include <math.h>


void oscillate(double curA,double curB,double *outA, double *outB);


int dofullscreen(SDL_Surface* pSurface, bool breathe, PhasePortraitSettings * settings, double *targetA, double *targetB)
{
	double curA=0.0, curB=0.0;

	SDL_Event event;
	double sliding = (settings->drawBasin) ? 2 : 10.0;

while (true)
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

SDL_Surface* pInstructions1=NULL;
SDL_Surface* pInstructions2=NULL;
int displayInstructions(SDL_Surface* pSurface, PhasePortraitSettings * settings)
{
	SDL_Event event;
	SDL_Surface *temp=NULL;
	if (pInstructions1==NULL) {
		temp = SDL_LoadBMP("instr1.bmp"); 
		if (temp==NULL) return -1;
		pInstructions1 = SDL_DisplayFormat(temp);
		SDL_FreeSurface(temp);
	}
	if (pInstructions2==NULL) {
		temp = SDL_LoadBMP("instr2.bmp"); 
		if (temp==NULL) return -1;
		pInstructions2 = SDL_DisplayFormat(temp);
		SDL_FreeSurface(temp);
	}
	SDL_FillRect ( pSurface , NULL , g_white );  //clear surface quickly
	SDL_Rect dest;
	dest.x = 50;
	dest.y = 50;
	dest.w = pInstructions1->w;
	dest.h = pInstructions1->h;
	SDL_BlitSurface(pInstructions1, NULL, pSurface, &dest);
	dest.x = PlotX;
	dest.y = PlotHeight+15;
	dest.w = pInstructions2->w;
	dest.h = pInstructions2->h;
	SDL_BlitSurface(pInstructions2, NULL, pSurface, &dest);
	DrawPlotGrid(pSurface,settings, 999,999);

	while (true)
	{
	if ( SDL_PollEvent ( &event ) )
	{
	//an event was found
	if ( event.type == SDL_QUIT ) return 0;
	else if (event.type==SDL_MOUSEBUTTONDOWN) return 0;
	else if (event.type==SDL_KEYUP)
	  {
		switch(event.key.keysym.sym)
		{
			case SDLK_SPACE: 
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
	SDL_UpdateRect ( pSurface , 0 , 0 , 0 , 0 ) ;
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
int fullscreen(SDL_Surface* pSurface, bool breathe, PhasePortraitSettings * settings, double *ptrtargetA, double *ptrtargetB)
{
	int prev = settings->width;
	settings->width = settings->height = 600;
	int ret = dofullscreen(pSurface, breathe, settings, ptrtargetA, ptrtargetB);
	settings->width = settings->height = prev;
	return ret;
}