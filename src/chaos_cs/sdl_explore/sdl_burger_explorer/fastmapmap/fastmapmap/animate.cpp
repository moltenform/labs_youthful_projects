
#include "whichmap.h"
#include "configfiles.h"
#include "font.h"
#include "phaseportrait.h"
#include "animate.h"
#include <stdio.h>
#include <string.h>

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

//returns false if the frame is blank (because fscanf will fail, causing loadData to return false.)
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
	BOOL ret= loadFromFile(buf);
	if (ret)
		memcpy(settingsOut,g_settings, sizeof(FastMapMapSettings));
	return ret;
}

void getFrameInterpolate(double fwhere, FastMapMapSettings * settings1, FastMapMapSettings * settings2, FastMapMapSettings * settingsOut)
{
	//fwhere must be between 0 and 1.
	//for now, we only interpolate a and b.

	settingsOut->a = (1-fwhere)*settings1->a + fwhere*settings2->a;
	settingsOut->b = (1-fwhere)*settings1->b + fwhere*settings2->b;
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

	double t = 0.0;
	double dt = 1/((double)nframesPerKeyframe); //100 frames
	if (dt<=0) return 1; //prevent infinite loop
	int currentFrame = 1;
	int n = 1;

	while (TRUE)
	{
		getFrameInterpolate(t, framesettings1, framesettings2, g_settings);

		SDL_FillRect ( pSurface , NULL , g_white );  //clear surface quickly
		if (SDL_MUSTLOCK(pSurface)) SDL_LockSurface ( pSurface ) ;
		DrawFigure(pSurface, g_settings->a, g_settings->b, width);
		if (SDL_MUSTLOCK(pSurface)) SDL_UnlockSurface ( pSurface ) ;
		snprintf(buf, sizeof(buf), "%s/frame%00d.bmp", SAVESFOLDER, n++);
		SDL_SaveBMP(pSurface, buf);

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
	return TRUE;
}


