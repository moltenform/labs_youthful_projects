
#include "common.h"
#include "animate.h"
#include "io.h"

void deleteFrame(int frame)
{
	char buf[256];
	snprintf(buf, sizeof(buf), "saves/frame0%d.cfg", frame);
	FILE * f= fopen(buf, "w");
	if (!f) {exit(1);}
	fputc('$', f); 
	fclose(f);
}
void deleteFrames()
{
for (int i=1; i<=9; i++)
	deleteFrame(i);
}

void saveToFrame(int frame, PhasePortraitSettings * settings, double a, double b)
{
	char buf[256];
	snprintf(buf, sizeof(buf), "saves/frame0%d.cfg", frame);
	saveData(settings, buf, a,b);
}

void openFrame(int frame, PhasePortraitSettings * settings, const char * filename, double *outA, double *outB)
{
	char buf[256];
	snprintf(buf, sizeof(buf), "saves/frame0%d.cfg", frame);
	loadData(settings, buf, outA, outB);
}

void getFrameInterp(double fwhere, PhasePortraitSettings * settings1, double a1, double b1, PhasePortraitSettings * settings2, double a2, double b2, PhasePortraitSettings * settingsOut, double *aOut, double *bOut)
{
	//fwhere must be between 0 and 1.
	//for now, we only interpolate a and b.
	//copy everything in settings1 to settingsOut
	memcpy(settingsOut, settings1, sizeof(PhasePortraitSettings));
	*aOut = fwhere*a1 + (1-fwhere)*a2;
	*bOut = fwhere*b1 + (1-fwhere)*b2;
}

int dotestanimation(SDL_Surface* pSurface)
{
	PhasePortraitSettings sframesettings1; PhasePortraitSettings sframesettings2; PhasePortraitSettings sframesettingsCur;
	PhasePortraitSettings * framesettings1 = &sframesettings1;
	PhasePortraitSettings * framesettings2 = &sframesettings2;
	PhasePortraitSettings * framesettingsCur = &sframesettingsCur;
	double a1, b1;
	double a2, b2;
	double curA, curB;
	BOOL bStart = loadData(framesettings1, "saves/frame00.cfg", &a1, &b1);
	if (!bStart) goto outsideloop; 
	bStart = loadData(framesettings2, "saves/frame01.cfg", &a2, &b2);
	if (!bStart) goto outsideloop; 
	double t = 0.0;
	int currentFrame = 0;

	SDL_Event event;

while (TRUE)
{
	

	if ( SDL_PollEvent ( &event ) )
	{
	//an event was found
	if ( event.type == SDL_QUIT ) goto outsideloop;
	else if (event.type==SDL_MOUSEBUTTONDOWN) goto outsideloop;
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
				goto outsideloop; //return back to the other screen!
				break;
		}
	}
	}

	if (LockFramesPerSecond()) 
	{
		getFrameInterp(t, framesettings1, a1, b1, framesettings2, a2, b2, framesettingsCur, &curA, &curB);

		SDL_FillRect ( pSurface , NULL , g_white );  //clear surface quickly
		if (SDL_MUSTLOCK(pSurface)) SDL_LockSurface ( pSurface ) ;
		DrawPhasePortrait(pSurface, framesettingsCur, curA,curB);
		if (SDL_MUSTLOCK(pSurface)) SDL_UnlockSurface ( pSurface ) ;
		
		t+= 0.0001;
		if (t>1)
		{
			currentFrame++;
			//try to get next frame
			BOOL isNextFrame = 

		}
	}

	SDL_UpdateRect ( pSurface , 0 , 0 , 0 , 0 ) ;  //apparently needed every frame, even when not redrawing
}
outsideloop:

return 0;
}

