
#include "configfiles.h"
#include "font.h"
#include "figure.h"
#include "animate.h"
#include <stdio.h>
#include <string.h>
#include <math.h>

#define SAVESFOLDER "savesgen"

//deleted the code for saving/loading animations, because people can copy/paste the files themselves.
//(see rev 389, bottom of this file.)

int gParamFramesPerKeyframe = 50;
BOOL gParamHasSavedAFrame=FALSE;

//delete the keyframe by overwriting it with a single char. 
//a later attempt to open this will return False.
void deleteFrame(int frame) 
{
	char buf[256];
	snprintf(buf, sizeof(buf), "%s/framek0%d.cfg", SAVESFOLDER, frame);
	FILE * f= fopen(buf, "w");
	if (!f) {exit(1);}
	fputc('$', f); 
	fclose(f);
}
void deleteAllFrames()
{
	for (int i=1; i<=9; i++)
		deleteFrame(i);
}

void saveToFrame(int frame)
{
	char buf[256];
	snprintf(buf, sizeof(buf), "%s/framek0%d.cfg", SAVESFOLDER, frame);
	saveToFile(buf, "NoExpressionText");
	gParamHasSavedAFrame = TRUE;
}

//returns False if the frame is blank (fscanf fails, causing loadData to return False.)
BOOL openFrame(int frame)
{
	char buf[256];
	snprintf(buf, sizeof(buf), "%s/framek0%d.cfg", SAVESFOLDER, frame);
	return loadFromFile(buf);
}
BOOL openFrameIntoSettings(int frame, FastMapsSettings * settingsOut)
{
	char buf[256];
	snprintf(buf, sizeof(buf), "%s/framek0%d.cfg", SAVESFOLDER, frame);
	BOOL ret = loadFromFile(buf);
	if (ret)
		memcpy(settingsOut,g_settings, sizeof(FastMapsSettings));
	return ret;
}

// interpolate between two frames. fwhere must be between 0 and 1.
void getFrameInterpolate(double fwhere, FastMapsSettings * settings1, FastMapsSettings * settings2, FastMapsSettings * settingsOut)
{
	//for now, we only interpolate a, b parameters

	settingsOut->pc1 = (1-fwhere)*settings1->pc1 + fwhere*settings2->pc1;
	settingsOut->pc2 = (1-fwhere)*settings1->pc2 + fwhere*settings2->pc2;
	settingsOut->pc3 = (1-fwhere)*settings1->pc3 + fwhere*settings2->pc3;
	settingsOut->pc4 = (1-fwhere)*settings1->pc4 + fwhere*settings2->pc4;
	settingsOut->pc5 = (1-fwhere)*settings1->pc5 + fwhere*settings2->pc5;
	settingsOut->pc6 = (1-fwhere)*settings1->pc6 + fwhere*settings2->pc6;
	
}

BOOL previewAnimation(SDL_Surface* pSurface, int nframesPerKeyframe, int width)
{
	FastMapsSettings sframesettings1; FastMapsSettings sframesettings2;
	FastMapsSettings * framesettings1 = &sframesettings1;
	FastMapsSettings * framesettings2 = &sframesettings2;
	if (! openFrameIntoSettings(1, framesettings1)) return FALSE;
	if (! openFrameIntoSettings(2, framesettings2)) return FALSE;
	//Important: base all the other parameters on the first keyframe.
	openFrame(1);

	double t = 0.0;
	double dt = 1/((double)nframesPerKeyframe);
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
					case SDLK_ESCAPE:
						goto outsideloop; //return back to the other screen!
						break;
					default: 
						break;
				}
			}
		}

		if (LockFramesPerSecond()) 
		{
			getFrameInterpolate(t, framesettings1, framesettings2, g_settings);

			SDL_FillRect ( pSurface , NULL , g_white );  //clear surface quickly
			if (SDL_MUSTLOCK(pSurface)) SDL_LockSurface ( pSurface ) ;
			DrawFigure(pSurface, width);
			if (SDL_MUSTLOCK(pSurface)) SDL_UnlockSurface ( pSurface ) ;
			
			t+= dt;
			if (t>1)
			{
				currentFrame++;
				//try to get next frame
				BOOL isNextFrame = openFrameIntoSettings(currentFrame+1, framesettings2);
				if (!isNextFrame) 
					goto outsideloop;
				openFrameIntoSettings(currentFrame, framesettings1);
				t=0;

			}
		}

		SDL_UpdateRect ( pSurface , 0 , 0 , 0 , 0 ) ;
	}
	outsideloop:

	return TRUE;
}

BOOL renderAnimation(SDL_Surface* pSurface, int nframesPerKeyframe, int width)
{
	char buf[256];
	FastMapsSettings sframesettings1; FastMapsSettings sframesettings2;
	FastMapsSettings * framesettings1 = &sframesettings1;
	FastMapsSettings * framesettings2 = &sframesettings2;
	if (! openFrameIntoSettings(1, framesettings1)) return FALSE;
	if (! openFrameIntoSettings(2, framesettings2)) return FALSE;
	//Important: base all the other parameters on the first keyframe.
	openFrame(1);

	SDL_Surface* pFrameSurface = SDL_CreateRGBSurface( SDL_SWSURFACE, width, width, pSurface->format->BitsPerPixel, pSurface->format->Rmask, pSurface->format->Gmask, pSurface->format->Bmask, 0 );
	double t = 0.0;
	double dt = 1/((double)nframesPerKeyframe);
	if (dt<=0) return 1; //prevent infinite loop
	int currentFrame = 1;
	int n = 1;

	while (TRUE)
	{
		getFrameInterpolate(t, framesettings1, framesettings2, g_settings);

		SDL_FillRect ( pFrameSurface , NULL , g_white );  //clear surface quickly
		if (SDL_MUSTLOCK(pFrameSurface)) SDL_LockSurface ( pFrameSurface ) ;
		DrawFigure(pFrameSurface, width);
		if (SDL_MUSTLOCK(pFrameSurface)) SDL_UnlockSurface ( pFrameSurface ) ;
		snprintf(buf, sizeof(buf), "%s/frame%03d.bmp", SAVESFOLDER, n++);
		SDL_SaveBMP(pFrameSurface, buf);

		t+= dt;
		if (t>1)
		{
			currentFrame++;
			//try to get next frame
			BOOL isNextFrame = openFrameIntoSettings(currentFrame+1, framesettings2);
			if (!isNextFrame) 
				goto outsideloop;
			openFrameIntoSettings(currentFrame, framesettings1);
			t=0;
		}
	
	}
outsideloop:
	SDL_FreeSurface(pFrameSurface);
	return TRUE;
}

//caller should turn off actual breathing before calling this.
BOOL renderBreathing(SDL_Surface* pSurface, int width)
{
	char buf[256];
	double fSeconds = 4;
	Dialog_GetDouble("How many seconds (20fps)?", pSurface, &fSeconds);
	if (fSeconds <= 0) return FALSE;
	int nFrames = (int)(20*fSeconds);
	SDL_Surface* pFrameSurface = SDL_CreateRGBSurface( SDL_SWSURFACE, width, width, pSurface->format->BitsPerPixel, pSurface->format->Rmask, pSurface->format->Gmask, pSurface->format->Bmask, 0 );
	double localsavedC1=g_settings->pc1; double localsavedC2=g_settings->pc2;
	for (int i=0; i<nFrames; i++)
	{
		oscillateBreathing(localsavedC1,localsavedC2,&g_settings->pc1,&g_settings->pc2);
		SDL_FillRect ( pFrameSurface , NULL , g_white );  //clear surface quickly
		if (SDL_MUSTLOCK(pFrameSurface)) SDL_LockSurface ( pFrameSurface ) ;
		DrawFigure(pFrameSurface, width);
		if (SDL_MUSTLOCK(pFrameSurface)) SDL_UnlockSurface ( pFrameSurface ) ;
		snprintf(buf, sizeof(buf), "%s/frame%03d.bmp", SAVESFOLDER, i);
		SDL_SaveBMP(pFrameSurface, buf);
	}
	g_settings->pc1=localsavedC1; g_settings->pc2= localsavedC2;
	SDL_FreeSurface(pFrameSurface);
	return TRUE;
}

void oscillateBreathing(double curA,double curB,double *outA, double *outB)
{
	static double t=0;
	t+=0.13;
	if (t>3141.5926) t=0.0;

	*outA = curA + sin( t +0.03*cos(t/8.5633) +3.685)/g_settings->breatheRadius_c1c2;
	*outB = curB + cos( 0.8241*t +0.02*sin(t/9.24123+5.742) )/(g_settings->breatheRadius_c1c2*1.315);
}

