
#include "breathe.h"
#include "common.h"
#include "sdl.h"
#include "phaseportrait.h"
#include <math.h>


void oscillate(double curA,double curB,double *outA, double *outB);

int fullscreen(SDL_Surface* pSurface, bool breathe, PhasePortraitSettings * settings, double targetA, double targetB)
{
	double curA=0.0, curB=0.0, actualA, actualB, prevA, prevB;

	SDL_Event event;
	double sliding = 10.0;

	curA += (targetA-curA)/sliding;
	curB += (targetB-curB)/sliding;

while (true)
{
	if ( SDL_PollEvent ( &event ) )
	{
	//an event was found
	if ( event.type == SDL_QUIT ) break ;
	if (event.type==SDL_KEYDOWN){
		switch(event.key.keysym.sym)
		{
			case SDLK_UP: targetB += 0.005; break;
			case SDLK_DOWN: targetB -= 0.005; break;
			case SDLK_LEFT: targetA -= 0.005; break;
			case SDLK_RIGHT: targetA += 0.005; break;
			default: 
				return 0; //return back to the other screen!
				break;
		}
	}
	}


if (LockFramesPerSecond())  //show ALL frames (if slower) or keep it going in time, dropping frames? put stuff in here
{
	if (prevA==curA && prevB == curB)
	{
		// don't need to compute anything.
		//debug by drawing black indicating nothing new is computed.
		//SDL_FillRect ( pSurface , NULL , 0/*black*/ );
	}
	else
	{
		SDL_FillRect ( pSurface , NULL , g_white );  //clear surface quickly
		//if (bNeedToLock) SDL_LockSurface ( pSurface ) ;
		DrawPhasePortrait(pSurface, settings, curA,curB);
		//if (bNeedToLock) SDL_UnlockSurface ( pSurface ) ;
	}
	prevA=curA; prevB=curB;
}


	if (breathe)
	{
		oscillate(curA, curB, &actualA, &actualB);
	}
	else
	{
		actualA = curA; 
		actualB = curB;
	}

}
}

void oscillate(double curA,double curB,double *outA, double *outB)
{
	static double statePos=0.0, stateFreq=0.0;
	if (statePos>31.415926) statePos=0.0;
	if (stateFreq>31.415926) stateFreq=0.0;
	stateFreq+=0.01;
	statePos+=stateFreq;

	//the frequency itself oscillates
	double oscilFreq = 0.09 + sin(stateFreq)/70;
	*outA = curA+ sin(statePos*.3702342521232353)/550;
	*outB = curB+ cos(statePos)/400; 
}
