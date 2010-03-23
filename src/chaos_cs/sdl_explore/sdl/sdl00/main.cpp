
#include "SDL.h"
#include <stdio.h>
#include <stdlib.h>
#include <memory.h>
#include <math.h>

#include "sdl_util.h"
#include "phaseportrait.h"
#include "main.h"
#include "old.h"


Uint32 g_white;





void setSettling(PhasePortraitSettings * settings, int direction);
void setShading(PhasePortraitSettings * settings, int direction);
void setSliding(double * sliding, int direction);
void setZoom(PhasePortraitSettings * settings, int direction);


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



int main( int argc, char* argv[] )
{
	PhasePortraitSettings ssettings; PhasePortraitSettings * settings = &ssettings;
	double curA=0.0, curB=0.0, targetA=1.0, targetB=1.0;

	InitialSettings(settings, SCREENWIDTH, SCREENHEIGHT, &targetA, &targetB);
	
	//these settings
	bool breathe = false;
	double sliding = 10.0;

	//set our at exit function
	atexit ( SDL_Quit ) ; 
	//initialize systems
	SDL_Init ( SDL_INIT_VIDEO ) ; 
	//create a window
	SDL_Surface* pSurface = SDL_SetVideoMode ( SCREENWIDTH , SCREENHEIGHT , SCREENBPP , SCREENFLAGS ) ;
	SDL_Event event ;
	bool bNeedToLock =  SDL_MUSTLOCK(pSurface);
	SDL_EnableKeyRepeat(30 /*SDL_DEFAULT_REPEAT_DELAY=500*/, /*SDL_DEFAULT_REPEAT_INTERVAL=30*/ 30);


	g_white = SDL_MapRGB ( pSurface->format , 255,255,255 ) ;



	double actualA, actualB;
		
  //message pump
  for ( ; ; )
  {
	if (breathe)
	{
		oscillate(curA, curB, &actualA, &actualB);
	}
	else
	{
		actualA = curA; 
		actualB = curB;
	}
	curA += (targetA-curA)/sliding;
	curB += (targetB-curB)/sliding;

    //look for an event
    if ( SDL_PollEvent ( &event ) )
    {
      //an event was found
      if ( event.type == SDL_QUIT ) break ;
	  else if (event.type==SDL_KEYDOWN)
	  {
		  if (event.key.keysym.sym == SDLK_UP) targetB += 0.005;
		  else if (event.key.keysym.sym == SDLK_DOWN) targetB -= 0.005;
		  else if (event.key.keysym.sym == SDLK_LEFT) targetA -= 0.005;
		  else if (event.key.keysym.sym == SDLK_RIGHT) targetA += 0.005;
		  else if (event.key.keysym.sym == SDLK_s) {save(curA,curB);}
		  else if (event.key.keysym.sym == SDLK_ESCAPE) {return 0;}
		  else if (event.key.keysym.sym == SDLK_F4) {return 0;}
			if (event.key.keysym.mod & KMOD_ALT)
			{
				switch(event.key.keysym.sym)
				{
					case SDLK_s: controller_sets_pos(&targetA, &targetB); break;
					case SDLK_g: controller_gets_pos(targetA, targetB); break;
					case SDLK_b: breathe = !breathe; break;
					case SDLK_1: setSettling(settings, 1); break;
					case SDLK_2: setSettling(settings, -1); break;
					case SDLK_3: setShading(settings, 1); break;
					case SDLK_4: setShading(settings, -1); break;
					case SDLK_5: setSliding(&sliding, 1); break;
					case SDLK_6: setSliding(&sliding, -1); break;
					case SDLK_PAGEUP: setZoom(settings, 1); break;
					case SDLK_PAGEDOWN: setZoom(settings, -1); break;
					default: break;
				}
				//force redraw
				if (bNeedToLock) SDL_LockSurface ( pSurface ) ;
				DrawPhasePortrait(pSurface, settings, actualA,actualB);
				if (bNeedToLock) SDL_UnlockSurface ( pSurface ) ;
			}
	  }
    }




if (LockFramesPerSecond())  //show ALL frames (if slower) or keep it going in time, dropping frames? put stuff in here
{
	if (!breathe && VERYCLOSE(targetA, curA) && VERYCLOSE(targetB, curB))
	{
		// don't need to compute anything.
		//SDL_FillRect ( pSurface , NULL , 0/*black*/ );  //debug by drawing black indicating nothing new is computed.
	}
	else
	{
		if (bNeedToLock) SDL_LockSurface ( pSurface ) ;
		DrawPhasePortrait(pSurface, settings, actualA,actualB);
		//DrawBifurc(pSurface, settings, actualA,actualB);
		if (bNeedToLock) SDL_UnlockSurface ( pSurface ) ;
	}
} 
		SDL_UpdateRect ( pSurface , 0 , 0 , 0 , 0 ) ; //apparently needed every frame, even when not redrawing

	
  }

  return 0;
}

void controller_sets_pos(double *outA, double *outB)
{
	double a,b;
	FILE*fp = fopen("C:\\pydev\\mainsvn\\chaos_cs\\sdl_explore\\menagerie\\outTrans.txt", "r");
	fscanf(fp, "%lf\n", &a);
	fscanf(fp, "%lf", &b);
	fclose(fp);
	*outA=a; *outB=b;
}
void controller_gets_pos(double a, double b)
{
	FILE*fp = fopen("C:\\pydev\\mainsvn\\chaos_cs\\sdl_explore\\menagerie\\getTrans.txt", "w");
	fprintf(fp, "%f\n", a);
	fprintf(fp, "%f", b);
	fclose(fp);
}

void setSettling(PhasePortraitSettings * settings, int direction)
{
	double scale = (direction<0) ? 0.95 : 1/0.95;
	settings->settling = roundDouble(settings->settling * scale);
}
void setShading(PhasePortraitSettings * settings, int direction)
{
	double scale = (direction<0) ? 0.95 : 1/0.95;
	settings->drawing = roundDouble(settings->drawing * scale);
	if (settings->drawing <=0) settings->drawing =1;
}
void setSliding(double * sliding, int direction)
{
	double scale = (direction<0) ? 0.95 : 1/0.95;
	*sliding = (*sliding * scale);
}
void setZoom(PhasePortraitSettings * settings, int direction)
{
	double scale = (direction<0) ? .999 : -.999; //note this
	double X0=settings->x0, X1=settings->x1, Y0=settings->y0, Y1=settings->y1;
	double newX0 = X0- (X1-X0)*scale;
    double newX1 = X1+ (X1-X0)*scale;
    double newY0 = Y0- (Y1-Y0)*scale;
    double newY1 = Y1+ (Y1-Y0)*scale;
	settings->x0 = newX0;
	settings->x1 = newX1;
	settings->y0 = newY0;
	settings->y1 = newY1;
}

int roundDouble(double a)
{
	if (fmod(a,1.0)<0.5)
		return (int)a;
	else
		return ((int)a)+1;
}