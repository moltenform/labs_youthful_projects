
#include "whichmap.h"
#include "configfiles.h"
#include "font.h"
#include "phaseportrait.h"
#include <stdio.h>
#include <string.h>


int nFramesPerKeyframe = 50;
void deleteFrame(int frame)
{
	char buf[256];
	snprintf(buf, sizeof(buf), "%s/frame0%d.cfg", SAVESFOLDER, frame);
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
	snprintf(buf, sizeof(buf), "%s/frame0%d.cfg", SAVESFOLDER, frame);
	saveToFile(buf, MAPEXPRESSIONTEXT);
}

//returns false if the frame is blank (because fscanf will fail, causing loadData to return false.)
BOOL openFrame(int frame)
{
	char buf[256];
	snprintf(buf, sizeof(buf), "%s/frame0%d.cfg", SAVESFOLDER, frame);
	return loadFromFile(buf);
}
BOOL openFrameIntoSettings(int frame, FastMapMapSettings * settingsOut)
{
	char buf[256];
	snprintf(buf, sizeof(buf), "%s/frame0%d.cfg", SAVESFOLDER, frame);
	BOOL ret= loadFromFile(buf);
	if (ret)
		memcpy(settingsOut,g_settings, sizeof(FastMapMapSettings));
	return ret;
}

void getFrameInterp(double fwhere, FastMapMapSettings * settings1, FastMapMapSettings * settings2, FastMapMapSettings * settingsOut)
{
	//fwhere must be between 0 and 1.
	//for now, we only interpolate a and b.

	settingsOut->a = (1-fwhere)*settings1->a + fwhere*settings2->a;
	settingsOut->b = (1-fwhere)*settings1->b + fwhere*settings2->b;
}

int dotestanimation(SDL_Surface* pSurface, int nframesPerKeyframe, int width)
{
	FastMapMapSettings sframesettings1; FastMapMapSettings sframesettings2;
	FastMapMapSettings * framesettings1 = &sframesettings1;
	FastMapMapSettings * framesettings2 = &sframesettings2;
	if (! openFrameIntoSettings(1, framesettings1)) goto outsideloop;
	if (! openFrameIntoSettings(2, framesettings2)) goto outsideloop;
	//Important: base all the other parameters on the first frame.
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
		getFrameInterp(t, framesettings1, framesettings2, g_settings);

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

return 0;
}




/*void saveAllKeyframes(SDL_Surface* pSurface)
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
}*/