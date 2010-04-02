#pragma warning (disable:4996)
//about fopen, scanf

#include "SDL.h"
#include <stdio.h>
#include <stdlib.h>
#include <memory.h>
#include <math.h>

#include "sdl_util.h"
#include "phaseportrait.h"
#include "basic.h"
#include "menagerie.h"
#include "main.h"
#include "io.h"


Uint32 g_white;


//int PlotHeight=400, PlotWidth=400, PlotX = 400;
int PlotHeight=300, PlotWidth=300, PlotX = 400;
int PhaseHeight = 384, PhaseWidth = 384;

void setSettling(PhasePortraitSettings * settings, int direction);
void setShading(PhasePortraitSettings * settings, int direction);
void setSliding(double * sliding, int direction);
void setZoom(PhasePortraitSettings * settings, int direction);



void tryZoom(int direction, int mouse_x, int mouse_y, PhasePortraitSettings*settings)
{
	if (!(mouse_x>PlotX && mouse_x<PlotX+PlotWidth && mouse_y>0 && mouse_y<PlotHeight))
		return;
	
	double fmousex, fmousey;
	IntPlotCoordsToDouble(settings, mouse_x, mouse_y, &fmousex, &fmousey);
	double fwidth=settings->browsex1-settings->browsex0, fheight=settings->browsex1-settings->browsex0;
	if (direction==-1) {fwidth *= 1.25; fheight*=1.25;}
	else {fwidth *= 0.8; fheight*=0.8;}
	settings->browsex0 = fmousex - fwidth/2;
	settings->browsex1 = fmousex + fwidth/2;
	settings->browsey0 = fmousey - fheight/2;
	settings->browsey1 = fmousey + fheight/2;
	SDL_Delay(700); //prevent fast zoom!
}



int main( int argc, char* argv[] )
{
	PhasePortraitSettings ssettings; PhasePortraitSettings * settings = &ssettings;
	double curA=0.0, curB=0.0, prevA=1,prevB=1;

	InitialSettings(settings, PhaseHeight, PhaseWidth, &curA, &curB);
	
	//these settings
	bool bManagerie = true;
	if (bManagerie) loadData();

	//set our at exit function
	atexit ( SDL_Quit ) ; 
	//initialize systems
	SDL_Init ( SDL_INIT_VIDEO ) ; 
	//create a window
	SDL_Surface* pSurface = SDL_SetVideoMode ( SCREENWIDTH , SCREENHEIGHT , SCREENBPP , SCREENFLAGS ) ;
	SDL_Event event ;
	bool bNeedToLock =  SDL_MUSTLOCK(pSurface);
	SDL_EnableKeyRepeat(30 /*SDL_DEFAULT_REPEAT_DELAY=500*/, /*SDL_DEFAULT_REPEAT_INTERVAL=30*/ 30);

	bool bIgnoreMousedown = false, bIgnoreRightMousedown=false;

	g_white = SDL_MapRGB ( pSurface->format , 255,255,255 ) ;

SDL_FillRect ( pSurface , NULL , g_white );

	SDL_EventState(SDL_MOUSEMOTION, SDL_IGNORE);


	//cache the home menagerie.
SDL_Surface* pHomeSurface = SDL_CreateRGBSurface( SDL_SWSURFACE, PlotWidth, PlotHeight, pSurface->format->BitsPerPixel, pSurface->format->Rmask, pSurface->format->Gmask, pSurface->format->Bmask, 0 );
SDL_Surface* pOtherSurface = SDL_CreateRGBSurface( SDL_SWSURFACE, PlotWidth, PlotHeight, pSurface->format->BitsPerPixel, pSurface->format->Rmask, pSurface->format->Gmask, pSurface->format->Bmask, 0 );
SDL_Surface* pSmallerSurface;
 if (bManagerie)
	 DrawMenagerie(pHomeSurface, settings); 
 
 pSmallerSurface = pHomeSurface;
 

 int mouse_x_prev=0,mouse_y_prev=0;
 int mouse_x,mouse_y;

  //message pump
for ( ; ; )
{

    //look for an event
    if ( SDL_PollEvent ( &event ) )
    {
      //an event was found
      if ( event.type == SDL_QUIT ) break ;
      //else if ( event.type == SDL_MOUSEBUTTONDOWN ) break ;
	  else if (event.type==SDL_KEYDOWN)
	  {
		switch(event.key.keysym.sym)
		{
			case SDLK_UP: curB += 0.005; break;
			case SDLK_DOWN: curB -= 0.005; break;
			case SDLK_LEFT: curA -= 0.005; break;
			case SDLK_RIGHT: curA += 0.005; break;
			default: break;
		}
	  }
	  else if (event.type==SDL_KEYUP)
	  {
		switch(event.key.keysym.sym)
		{
			case SDLK_s: if (event.key.keysym.mod & KMOD_CTRL) onSave(settings,curA,curB); break;
			case SDLK_o: if (event.key.keysym.mod & KMOD_CTRL) onOpen(settings,&curA,&curB); break;
			case SDLK_QUOTE: if (event.key.keysym.mod & KMOD_CTRL) onGetExact(settings,&curA,&curB); break;
			case SDLK_SEMICOLON: if (event.key.keysym.mod & KMOD_CTRL) onGetMoreOptions(settings,&curA,&curB); break;
			case SDLK_PAGEUP: break;
			case SDLK_PAGEDOWN: break;
			case SDLK_1: break;
			case SDLK_ESCAPE: return 0; break;
			case SDLK_F4: return 0; break;
			default: break;
		}
	  }
    }




if (LockFramesPerSecond())  //show ALL frames (if slower) or keep it going in time, dropping frames? put stuff in here
{
	if (prevA==curA && prevB == curB)
	{
		// don't need to compute anything.
		//SDL_FillRect ( pSurface , NULL , 0/*black*/ );  //debug by drawing black indicating nothing new is computed.
	}
	else
	{
		SDL_FillRect ( pSurface , NULL , g_white );  //clear surface quickly
		if (bManagerie) BlitMenagerie(pSurface, pSmallerSurface); 
		if (bNeedToLock) SDL_LockSurface ( pSurface ) ;
		DrawPhasePortrait(pSurface, settings, curA,curB);
		DrawPlotGrid(pSurface,settings, curA,curB);
		if (bNeedToLock) SDL_UnlockSurface ( pSurface ) ;
	}
	prevA=curA; prevB=curB;

		SDL_PumpEvents();
		int mod = SDL_GetModState();


int buttons = SDL_GetMouseState(&mouse_x, &mouse_y);
if ((buttons & SDL_BUTTON_LMASK) && !bIgnoreMousedown)
{
	if (mod & KMOD_CTRL ) //a control click. zoom in.
	{
		tryZoom(1, mouse_x, mouse_y, settings);
		bIgnoreMousedown = true; //ignore subsequent events until mouse released, so we don't repeatedly zoom
		prevA=99; //force redraw
		if (bManagerie) DrawMenagerie(pHomeSurface, settings);
	}
	else if (mod & KMOD_SHIFT ) //a shift click. zoom out.
	{
		tryZoom(-1, mouse_x, mouse_y, settings);
		bIgnoreMousedown = true; //ignore subsequent events until mouse released, so we don't repeatedly zoom
		prevA=99; //force redraw
		if (bManagerie) DrawMenagerie(pHomeSurface, settings);
	}
	else
	{
		// clicking and dragging:
		if (mouse_x_prev!= mouse_x || mouse_y_prev!= mouse_y)
		{
			if (mouse_x>PlotX && mouse_x<PlotX+PlotWidth && mouse_y>0 && mouse_y<PlotHeight)
				IntPlotCoordsToDouble(settings, mouse_x, mouse_y, &curA, &curB);
		}
		mouse_x_prev = mouse_x;
		mouse_y_prev = mouse_y;
	}
}
else
bIgnoreMousedown = false;


if ((buttons & SDL_BUTTON_RMASK) && !bIgnoreRightMousedown)
{
//reset view
InitialSettings(settings, PhaseHeight, PhaseWidth, &curA, &curB);
 if (bManagerie)
	 DrawMenagerie(pHomeSurface, settings);
bIgnoreRightMousedown = true;
}
else
bIgnoreRightMousedown = false;

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

void IntPlotCoordsToDouble(PhasePortraitSettings*settings, int mouse_x, int mouse_y, double*outX, double *outY)
{
	*outX = (mouse_x-PlotX)/((double)PlotWidth)*(settings->browsex1-settings->browsex0) + settings->browsex0;
	*outY = ((PlotHeight-mouse_y)-0)/((double)PlotHeight)*(settings->browsey1-settings->browsey0) + settings->browsey0;
}
void DoubleCoordsToInt(PhasePortraitSettings*settings, double fx, double fy, int* outX, int* outY)
{
	*outX = (int)(PlotWidth * (fx- settings->browsex0) / (settings->browsex1 - settings->browsex0) + PlotX);
	*outY = PlotHeight - (int)(PlotHeight * (fy- settings->browsey0) / (settings->browsey1 - settings->browsey0) + 0);
}
