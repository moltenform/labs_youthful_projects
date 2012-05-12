
#include "whichmap.h"
#include "configfiles.h"
#include "font.h"
#include "phaseportrait.h"
#include "animate.h"
#include <stdio.h>
#include <string.h>
#include <math.h>

//deleted the code for saving/loading animations, because people can copy/paste the files themselves.
//(see rev 389, bottom of this file.)

int gParamFramesPerKeyframe = 50;

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
	saveToFile(buf, MAPEXPRESSIONTEXT);
}

//returns False if the frame is blank (fscanf fails, causing loadData to return False.)
BOOL openFrame(int frame)
{
	char buf[256];
	snprintf(buf, sizeof(buf), "%s/framek0%d.cfg", SAVESFOLDER, frame);
	return loadFromFile(buf);
}
BOOL openFrameIntoSettings(int frame, FastMapMapSettings * settingsOut)
{
	char buf[256];
	snprintf(buf, sizeof(buf), "%s/framek0%d.cfg", SAVESFOLDER, frame);
	BOOL ret = loadFromFile(buf);
	if (ret)
		memcpy(settingsOut,g_settings, sizeof(FastMapMapSettings));
	return ret;
}

BOOL gParamAnimateMoreThanAB = FALSE;
// interpolate between two frames. fwhere must be between 0 and 1.
void getFrameInterpolate(double fwhere, FastMapMapSettings * settings1, FastMapMapSettings * settings2, FastMapMapSettings * settingsOut)
{
	//for now, we only interpolate a, b, and colorsStep.
	settingsOut->a = (1-fwhere)*settings1->a + fwhere*settings2->a;
	settingsOut->b = (1-fwhere)*settings1->b + fwhere*settings2->b;
	
	if (gParamAnimateMoreThanAB) 
	{
		settingsOut->colorsStep = (int)((1-fwhere)*settings1->colorsStep + fwhere*settings2->colorsStep);
		settingsOut->settlingTime = (int)((1-fwhere)*settings1->settlingTime + fwhere*settings2->settlingTime);
		settingsOut->drawingTime = (int)((1-fwhere)*settings1->drawingTime + fwhere*settings2->drawingTime);
		settingsOut->seedsPerAxis = (int)((1-fwhere)*settings1->seedsPerAxis + fwhere*settings2->seedsPerAxis);

		settingsOut->colorDiskRadius=(1-fwhere)*settings1->colorDiskRadius + fwhere*settings2->colorDiskRadius;
		settingsOut->basinsMaxColor= ((1-fwhere)*settings1->basinsMaxColor + fwhere*settings2->basinsMaxColor);
		
		//fwhere = (exp(fwhere)-1.0)/(2.718281828 - 1.0); //1-(exp(1-x)-1)/(E-1)
		
		settingsOut->x0 = (1-fwhere)*settings1->x0 + fwhere*settings2->x0;
		settingsOut->x1 = (1-fwhere)*settings1->x1 + fwhere*settings2->x1;
		settingsOut->y0 = (1-fwhere)*settings1->y0 + fwhere*settings2->y0;
		settingsOut->y1 = (1-fwhere)*settings1->y1 + fwhere*settings2->y1;
	}
}

BOOL previewAnimation(SDL_Surface* pSurface, int nframesPerKeyframe, int width)
{
	FastMapMapSettings sframesettings1; FastMapMapSettings sframesettings2;
	FastMapMapSettings * framesettings1 = &sframesettings1;
	FastMapMapSettings * framesettings2 = &sframesettings2;
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
			DrawFigure(pSurface, g_settings->a, g_settings->b, width);
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
	FastMapMapSettings sframesettings1; FastMapMapSettings sframesettings2;
	FastMapMapSettings * framesettings1 = &sframesettings1;
	FastMapMapSettings * framesettings2 = &sframesettings2;
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
		DrawFigure(pFrameSurface, g_settings->a, g_settings->b, width);
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

BOOL renderBreathing(SDL_Surface* pSurface, int width)
{
	char buf[256];
	double fSeconds = 4;
	Dialog_GetDouble("How many seconds (20fps)?", pSurface, &fSeconds);
	if (fSeconds <= 0) return FALSE;
	int nFrames = (int)(20*fSeconds);
	SDL_Surface* pFrameSurface = SDL_CreateRGBSurface( SDL_SWSURFACE, width, width, pSurface->format->BitsPerPixel, pSurface->format->Rmask, pSurface->format->Gmask, pSurface->format->Bmask, 0 );
	double breatheA, breatheB;
	for (int i=0; i<nFrames; i++)
	{
		oscillateBreathing(g_settings->a,g_settings->b,&breatheA, &breatheB);
		SDL_FillRect ( pFrameSurface , NULL , g_white );  //clear surface quickly
		if (SDL_MUSTLOCK(pFrameSurface)) SDL_LockSurface ( pFrameSurface ) ;
		DrawFigure(pFrameSurface, breatheA, breatheB, width);
		if (SDL_MUSTLOCK(pFrameSurface)) SDL_UnlockSurface ( pFrameSurface ) ;
		snprintf(buf, sizeof(buf), "%s/frame%03d.bmp", SAVESFOLDER, i);
		SDL_SaveBMP(pFrameSurface, buf);
	}

	SDL_FreeSurface(pFrameSurface);
	return TRUE;
}

void oscillateBreathing(double curA,double curB,double *outA, double *outB)
{
	static double t=0;
	t+=0.13;
	if (t>3141.5926) t=0.0;

	*outA = curA + sin( t +0.03*cos(t/8.5633) +3.685)/g_settings->breatheRadius;
	*outB = curB + cos( 0.8241*t +0.02*sin(t/9.24123+5.742) )/(g_settings->breatheRadius*1.315);
}

