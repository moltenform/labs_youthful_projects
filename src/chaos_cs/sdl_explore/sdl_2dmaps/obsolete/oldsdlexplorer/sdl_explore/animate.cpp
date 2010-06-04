#pragma warning (disable:4996)

#include "common.h"
#include "animate.h"
#include "io.h"
#include "phaseportrait.h"
#include "font.h"


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

void saveAllKeyframes(SDL_Surface* pSurface)
{
	char*ret = Dialog_GetText("Save animation under what name?",NULL,pSurface);
	if (!ret) return;
	char buf[256];
	char buf2[256];
	snprintf(buf, sizeof(buf), "animsaves/%s0%d.cfg", ret, 1);
	if (doesFileExist(buf) && !Dialog_GetBool("Animation of that name already exists. Replace?",pSurface)) {
		free(ret);
		return;
	}
	//copy all frame files over.
	for (int i=1; i<=9; i++)
	{
		snprintf(buf, sizeof(buf), "saves/frame0%d.cfg", i);
		snprintf(buf2, sizeof(buf2), "animsaves/%s0%d.cfg", ret, i);
		FILE * from = fopen(buf,"rb"); FILE * to = fopen(buf2,"wb");
		while(!feof(from))
		{
			int ch = fgetc(from);
			if(!feof(from)) fputc(ch, to);
		}
		fclose(from); fclose(to);
	}
	free(ret);
}
void openAllKeyframes(SDL_Surface* pSurface)
{
	char*ret = Dialog_GetText("Open animation of what name?",NULL,pSurface);
	if (!ret) return;
	char buf[256];
	char buf2[256];
	snprintf(buf, sizeof(buf), "animsaves/%s0%d.cfg", ret, 1);
	if (!doesFileExist(buf)) {
		Dialog_Message("There does not seem to be an animation of that name.",pSurface);
		free(ret);
		return;
	}
	//copy all frame files over.
	for (int i=1; i<=9; i++)
	{
		snprintf(buf, sizeof(buf), "animsaves/%s0%d.cfg", ret, i);
		snprintf(buf2, sizeof(buf2), "saves/frame0%d.cfg", i);
		FILE * from = fopen(buf,"rb"); FILE * to = fopen(buf2,"wb");
		while(!feof(from))
		{
			int ch = fgetc(from);
			if(!feof(from)) fputc(ch, to);
		}
		fclose(from); fclose(to);
	}
	free(ret);
}

void getFrameInterp(double fwhere, PhasePortraitSettings * settings1, double a1, double b1, PhasePortraitSettings * settings2, double a2, double b2, PhasePortraitSettings * settingsOut, double *aOut, double *bOut)
{
	//fwhere must be between 0 and 1.
	//for now, we only interpolate a and b.
	//copy everything in settings1 to settingsOut
	//memcpy(settingsOut, settings1, sizeof(PhasePortraitSettings));
	*aOut = (1-fwhere)*a1 + fwhere*a2;
	*bOut = (1-fwhere)*b1 + fwhere*b2;
}

int dotestanimation(SDL_Surface* pSurface, int nframesPerKeyframe)
{
	PhasePortraitSettings sframesettings1; PhasePortraitSettings sframesettings2; PhasePortraitSettings sframesettingsCur;
	PhasePortraitSettings * framesettings1 = &sframesettings1;
	PhasePortraitSettings * framesettings2 = &sframesettings2;
	PhasePortraitSettings * framesettingsCur = &sframesettingsCur;
	double a1, b1;
	double a2, b2;
	double curA, curB;
	if (! openFrame(1, framesettings1, &a1, &b1)) goto outsideloop;
	if (! openFrame(2, framesettings2, &a2, &b2)) goto outsideloop;
	//Important: base all the other parameters on the first frame.
	openFrame(1, framesettingsCur, &a1, &b1);

	double t = 0.0;
	double dt = 1/((double)nframesPerKeyframe); //100 frames
	if (dt<=0) return 1; //prevent infinite loop
	int currentFrame = 1;

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
		
		t+= dt;
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

int dowriteanimation(SDL_Surface* pSurface, int nframesPerKeyframe)
{
	SDL_FillRect ( pSurface , NULL , g_white );  //clear surface quickly
	ShowText("Rendering animation...", 30,30, pSurface);
	SDL_UpdateRect ( pSurface , 0 , 0 , 0 , 0 ) ; 

	SDL_Surface* pFrameSurface = SDL_CreateRGBSurface( SDL_SWSURFACE, PhaseWidth, PhaseHeight, pSurface->format->BitsPerPixel, pSurface->format->Rmask, pSurface->format->Gmask, pSurface->format->Bmask, 0 );

	PhasePortraitSettings sframesettings1; PhasePortraitSettings sframesettings2; PhasePortraitSettings sframesettingsCur;
	PhasePortraitSettings * framesettings1 = &sframesettings1;
	PhasePortraitSettings * framesettings2 = &sframesettings2;
	PhasePortraitSettings * framesettingsCur = &sframesettingsCur;
	double a1, b1;
	double a2, b2;
	double curA, curB;
	if (! openFrame(1, framesettings1, &a1, &b1)) goto outsideloop;
	if (! openFrame(2, framesettings2, &a2, &b2)) goto outsideloop;
	//Important: base all the other parameters on the first frame.
	openFrame(1, framesettingsCur, &a1, &b1);

	double t = 0.0;
	double dt = 1/((double)nframesPerKeyframe); //100 frames
	if (dt<=0) return 1; //prevent infinite loop
	int currentFrame = 1;
	char buf[256];
	int n=0;

	while (TRUE)
	{
		getFrameInterp(t, framesettings1, a1, b1, framesettings2, a2, b2, framesettingsCur, &curA, &curB);

		SDL_FillRect ( pFrameSurface , NULL , g_white );  //clear surface quickly
		if (SDL_MUSTLOCK(pFrameSurface)) SDL_LockSurface ( pFrameSurface ) ;
		DrawPhasePortrait(pFrameSurface, framesettingsCur, curA,curB);
		if (SDL_MUSTLOCK(pFrameSurface)) SDL_UnlockSurface ( pFrameSurface ) ;
		snprintf(buf, sizeof(buf), "animout/fr%00d.bmp", n++);
		SDL_SaveBMP(pFrameSurface, buf);

		t+= dt;
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

outsideloop:

	SDL_FreeSurface(pFrameSurface);
return 0;
}
