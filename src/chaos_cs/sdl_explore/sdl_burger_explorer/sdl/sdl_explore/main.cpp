#pragma warning (disable:4996)
//about fopen, scanf

#include "SDL.h"
#include <stdio.h>
#include <stdlib.h>
#include <memory.h>
#include <math.h>

#include "common.h"
#include "phaseportrait.h"
#include "menagerie.h"
#include "io.h"
#include "breathe.h"

//cause redraw by making prevA out of date.
#define ForceRedraw() prevA=99
#define RedrawMenag() {if (bMenagerie) DrawMenagerieQuick(pSmallerSurface, settings);}


int PlotHeight=300, PlotWidth=300, PlotX = 400;
int PhaseHeight = 384, PhaseWidth = 384;
Uint32 g_white;

void zoomPortrait(int direction, PhasePortraitSettings * settings);
void tryZoomPlot(int direction, int mouse_x, int mouse_y, PhasePortraitSettings*settings);



int main( int argc, char* argv[] )
{
	PhasePortraitSettings ssettings; PhasePortraitSettings * settings = &ssettings;
	double curA=0.0, curB=0.0, prevA=1,prevB=1;
	InitialSettings(settings, PhaseHeight, PhaseWidth, &curA, &curB);
	
	//these settings
	bool bMenagerie = true;
	if (bMenagerie) loadData();


	atexit ( SDL_Quit ) ; 
	SDL_Init ( SDL_INIT_VIDEO ) ; 
	//create main window
	SDL_Surface* pSurface = SDL_SetVideoMode ( SCREENWIDTH , SCREENHEIGHT , SCREENBPP , SCREENFLAGS ) ;
	SDL_Event event;
	bool bNeedToLock =  SDL_MUSTLOCK(pSurface);
	SDL_EnableKeyRepeat(30 /*SDL_DEFAULT_REPEAT_DELAY=500*/, /*SDL_DEFAULT_REPEAT_INTERVAL=30*/ 30);
	int mouse_x,mouse_y;

	//cache the home menagerie? might not be a bad idea.
	SDL_Surface* pSmallerSurface = SDL_CreateRGBSurface( SDL_SWSURFACE, PlotWidth, PlotHeight, pSurface->format->BitsPerPixel, pSurface->format->Rmask, pSurface->format->Gmask, pSurface->format->Bmask, 0 );
	if (bMenagerie)
		DrawMenagerie(pSmallerSurface, settings); 
 

	g_white = SDL_MapRGB ( pSurface->format , 255,255,255 ) ;
	SDL_FillRect ( pSurface , NULL , g_white );


while(true)
{
    //look for an event
    if ( SDL_PollEvent ( &event ) )
    {
      //an event was found
      if ( event.type == SDL_QUIT ) break ;
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
			case SDLK_ESCAPE: return 0; break;
			case SDLK_F4: return 0; break;
			case SDLK_s: if (event.key.keysym.mod & KMOD_CTRL) onSave(settings,curA,curB); break;
			case SDLK_o: if (event.key.keysym.mod & KMOD_CTRL) onOpen(settings,&curA,&curB); break;
			case SDLK_QUOTE: if (event.key.keysym.mod & KMOD_CTRL) onGetExact(settings,&curA,&curB); break;
			case SDLK_SEMICOLON: if (event.key.keysym.mod & KMOD_CTRL) onGetMoreOptions(settings,&curA,&curB); break;
			case SDLK_F11: fullscreen(pSurface, false, settings, &curA,&curB); ForceRedraw(); break;
			case SDLK_f: if (event.key.keysym.mod & KMOD_ALT) {fullscreen(pSurface, false, settings, &curA,&curB);ForceRedraw();} break;
			case SDLK_g: if (event.key.keysym.mod & KMOD_ALT) {settings->drawBasin = !settings->drawBasin;ForceRedraw();} break;
			case SDLK_PAGEUP: zoomPortrait(1,settings); ForceRedraw(); break;
			case SDLK_PAGEDOWN: zoomPortrait(-1,settings); ForceRedraw(); break;
			case SDLK_SPACE: displayInstructions(pSurface, settings); ForceRedraw(); break;
			case SDLK_1: break;
			default: break;
		}
	  }
	  else if ( event.type == SDL_MOUSEMOTION )
	  {
		  int buttons = SDL_GetMouseState(&mouse_x, &mouse_y);
		  if ((buttons & SDL_BUTTON_LMASK))
			if (mouse_x>PlotX && mouse_x<PlotX+PlotWidth && mouse_y>0 && mouse_y<PlotHeight)
				IntPlotCoordsToDouble(settings, mouse_x, mouse_y, &curA, &curB);
	  }
      else if ( event.type == SDL_MOUSEBUTTONDOWN )
	  {
			//only called once per click
			int buttons = SDL_GetMouseState(&mouse_x, &mouse_y);
			int mod = SDL_GetModState();

			//control click = zoom in, shift click=zoom out
			if ((buttons & SDL_BUTTON_LMASK) && ((mod & KMOD_CTRL) || (mod & KMOD_SHIFT ) ) )
			{
				int direction = (mod & KMOD_CTRL) ? 1 : -1;
				tryZoomPlot(direction, mouse_x, mouse_y, settings);
				ForceRedraw();
				RedrawMenag();
			}
			else if (buttons & SDL_BUTTON_RMASK) //right-click resets
			{
				InitialSettings(settings, PhaseHeight, PhaseWidth, &curA, &curB);
				ForceRedraw();
				RedrawMenag();
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
		if (bMenagerie) BlitMenagerie(pSurface, pSmallerSurface); 
		if (bNeedToLock) SDL_LockSurface ( pSurface ) ;
		DrawPhasePortrait(pSurface, settings, curA,curB);
		DrawPlotGrid(pSurface,settings, curA,curB);
		if (bNeedToLock) SDL_UnlockSurface ( pSurface ) ;
	}
	prevA=curA; prevB=curB;

}
		SDL_UpdateRect ( pSurface , 0 , 0 , 0 , 0 ) ;  //apparently needed every frame, even when not redrawing
	
	}
	return 0;
}




void tryZoomPlot(int direction, int mouse_x, int mouse_y, PhasePortraitSettings*settings)
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
}
void zoomPortrait(int direction, PhasePortraitSettings * settings )
{
	double fcenterx, fcentery;
	fcenterx= (settings->x1+settings->x0)/2;
	fcentery= (settings->y1+settings->y0)/2;
	double fwidth=settings->x1-settings->x0, fheight=settings->x1-settings->x0;
	if (direction==-1) {fwidth *= 1.25; fheight*=1.25;}
	else {fwidth *= 0.8; fheight*=0.8;}
	settings->x0 = fcenterx - fwidth/2;
	settings->x1 = fcenterx + fwidth/2;
	settings->y0 = fcentery - fheight/2;
	settings->y1 = fcentery + fheight/2;
}