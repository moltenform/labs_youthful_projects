
#include "common.h"
#include "animate.h"
#include "io.h"
#include "phaseportrait.h"

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

//returns false if the frame is blank (because fscanf will fail, causing loadData to return false.)
BOOL openFrame(int frame, PhasePortraitSettings * settings, double *outA, double *outB)
{
	char buf[256];
	snprintf(buf, sizeof(buf), "saves/frame0%d.cfg", frame);
	return loadData(settings, buf, outA, outB);
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
	if (! openFrame(0, framesettings1, &a1, &b1)) goto outsideloop;
	if (! openFrame(1, framesettings2, &a2, &b2)) goto outsideloop;
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
			BOOL isNextFrame = openFrame(currentFrame+1, framesettings2, &a2, &b2);
			if (!isNextFrame) 
				goto outsideloop;
			openFrame(currentFrame, framesettings1, &a1, &b1);
			t=0;

		}
	}

	SDL_UpdateRect ( pSurface , 0 , 0 , 0 , 0 ) ;  //apparently needed every frame, even when not redrawing
}
outsideloop:

return 0;
}

