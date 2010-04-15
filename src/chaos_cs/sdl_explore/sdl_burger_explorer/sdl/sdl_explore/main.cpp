#pragma warning (disable:4996)
//about fopen, scanf

#include <stdio.h>
#include <stdlib.h>
#include <memory.h>
#include <math.h>

#include "common.h" //see DYNAMICMENAGERIE setting
#include "phaseportrait.h"
#include "menagerie.h"
#include "io.h"
#include "breathe.h"
#include "font.h"
/*
Todo: basins save to cfg
clean up key event code in main.cpp
could have Menag cache for common values, in mem, and then resets
genetic algorithm for finding interesting areas
dynamic compilation?
bug where clicking always causes zooming in
show filename when opening?
Remove a frame: turns it blank. overwrites file with empty string. "blank" 
*/

#define RedrawMenag() {if (bMenagerie) DrawMenagerie(pSmallerSurface, settings);}
#define ForceRedraw() {RedrawMenag(); prevA=99;}
//cause redraw by making prevA out of date. Because using precomputed menag, it's cheap to redraw it too.

int PlotHeight=300, PlotWidth=300, PlotX = 400;
int PhaseHeight = 384, PhaseWidth = 384;
Uint32 g_white;

void zoomPortrait(int direction, PhasePortraitSettings * settings);
void tryZoomPlot(int direction, int mouse_x, int mouse_y, PhasePortraitSettings*settings);
int displayInstructions(SDL_Surface* pSurface,PhasePortraitSettings * settings);

int main( int argc, char* argv[] )
{

	PhasePortraitSettings ssettings; PhasePortraitSettings * settings = &ssettings;
	double curA=0.0, curB=0.0, prevA=99,prevB=99;
	InitialSettings(settings, PhaseHeight, PhaseWidth, &curA, &curB);
	//load a file if parameter given.
	if (argc > 1 && strcmp(argv[1],"full")!=0)
		loadData(settings, argv[1], &curA, &curB);
	
	//these settings
	BOOL bMenagerie = TRUE;
	if (bMenagerie) loadMenagerieData();

	atexit ( SDL_Quit ) ; 
	SDL_Init ( SDL_INIT_VIDEO ) ; 
	//create main window
	Uint32 flags = SCREENFLAGS;
	if ((argc > 1 && strcmp(argv[1],"full")==0)||(argc > 2 && strcmp(argv[2],"full")==0))
		flags |= SDL_FULLSCREEN;
	SDL_Surface* pSurface = SDL_SetVideoMode ( SCREENWIDTH , SCREENHEIGHT , SCREENBPP , flags) ;

	SDL_Event event;
	BOOL bNeedToLock =  SDL_MUSTLOCK(pSurface);
	SDL_EnableKeyRepeat(30 /*SDL_DEFAULT_REPEAT_DELAY=500*/, /*SDL_DEFAULT_REPEAT_INTERVAL=30*/ 30);
	int mouse_x,mouse_y;

	//cache the home menagerie? might not be a bad idea.
	SDL_Surface* pSmallerSurface = SDL_CreateRGBSurface( SDL_SWSURFACE, PlotWidth, PlotHeight, pSurface->format->BitsPerPixel, pSurface->format->Rmask, pSurface->format->Gmask, pSurface->format->Bmask, 0 );
	if (bMenagerie)
		DrawMenagerie(pSmallerSurface, settings); 
 

	g_white = SDL_MapRGB ( pSurface->format , 255,255,255 ) ;
	SDL_FillRect ( pSurface , NULL , g_white );


while(TRUE)
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
			case SDLK_UP: curB += (event.key.keysym.mod & KMOD_SHIFT) ? 0.0005 : 0.005; break;
			case SDLK_DOWN: curB -= (event.key.keysym.mod & KMOD_SHIFT) ? 0.0005 : 0.005; break;
			case SDLK_LEFT: curA -= (event.key.keysym.mod & KMOD_SHIFT) ? 0.0005 : 0.005; break;
			case SDLK_RIGHT: curA += (event.key.keysym.mod & KMOD_SHIFT) ? 0.0005 : 0.005; break;
			default: break;
		}
	  }
	  else if (event.type==SDL_KEYUP)
	  {
		switch(event.key.keysym.sym)
		{
			case SDLK_ESCAPE: return 0; break;
			case SDLK_F4: return 0; break;
			case SDLK_s: if (event.key.keysym.mod & KMOD_CTRL) onSave(settings,curA,curB, pSurface); ForceRedraw(); break;
			case SDLK_o: if (event.key.keysym.mod & KMOD_CTRL) onOpen(settings,&curA,&curB, (event.key.keysym.mod & KMOD_SHIFT)!=0);ForceRedraw(); break;
			case SDLK_QUOTE: if (event.key.keysym.mod & KMOD_CTRL) onGetExact(settings,&curA,&curB, pSurface);ForceRedraw(); break;
			case SDLK_SEMICOLON: if (event.key.keysym.mod & KMOD_CTRL) onGetMoreOptions(settings, pSurface);ForceRedraw(); break;
			case SDLK_F11: fullscreen(pSurface, FALSE, settings, &curA,&curB); ForceRedraw(); break;
			case SDLK_f: if (event.key.keysym.mod & KMOD_ALT) {fullscreen(pSurface, FALSE, settings, &curA,&curB);ForceRedraw();} break;
			case SDLK_b: if (event.key.keysym.mod & KMOD_ALT) {fullscreen(pSurface, TRUE, settings, &curA,&curB);ForceRedraw();} break;
			case SDLK_g: if (event.key.keysym.mod & KMOD_ALT) {settings->drawBasin = !settings->drawBasin;ForceRedraw();} break;
			case SDLK_PAGEUP: zoomPortrait(1,settings); ForceRedraw(); break;
			case SDLK_PAGEDOWN: zoomPortrait(-1,settings); ForceRedraw(); break;
			case SDLK_SPACE: 
			case SDLK_RETURN: 
			case SDLK_F1: 
			case SDLK_KP_ENTER: 
				displayInstructions(pSurface, settings); ForceRedraw(); break;
			default: 
				if (event.key.keysym.sym >= SDLK_0 && event.key.keysym.sym <= SDLK_9) {
					loadPreset(event.key.keysym.sym - SDLK_0,(event.key.keysym.mod & KMOD_SHIFT)!=0,(event.key.keysym.mod & KMOD_ALT)!=0, settings, &curA, &curB);
					ForceRedraw(); }
				break;
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

		SDL_UpdateRect ( pSurface , 0 , 0 , 0 , 0 ) ;  //put it only here?
		
	}
	prevA=curA; prevB=curB;

}
		//SDL_UpdateRect ( pSurface , 0 , 0 , 0 , 0 ) ;  //is this needed every frame, even when not redrawing?
	
	}
	return 0;
}




void tryZoomPlot(int direction, int mouse_x, int mouse_y, PhasePortraitSettings*settings)
{
	double fmousex, fmousey;
	if (mouse_x>PlotX && mouse_x<PlotX+PlotWidth && mouse_y>0 && mouse_y<PlotHeight)
	{
	IntPlotCoordsToDouble(settings, mouse_x, mouse_y, &fmousex, &fmousey);
	double fwidth=settings->browsex1-settings->browsex0, fheight=settings->browsex1-settings->browsex0;
	if (direction==-1) {fwidth *= 1.25; fheight*=1.25;}
	else {fwidth *= 0.8; fheight*=0.8;}
	settings->browsex0 = fmousex - fwidth/2;
	settings->browsex1 = fmousex + fwidth/2;
	settings->browsey0 = fmousey - fheight/2;
	settings->browsey1 = fmousey + fheight/2;
	}
	else if (mouse_x>0 && mouse_x<PhaseWidth && mouse_y>0 && mouse_y<PhaseHeight)
	{
	IntPhaseCoordsToDouble(settings, mouse_x, mouse_y, &fmousex, &fmousey);
	double fwidth=settings->x1-settings->x0, fheight=settings->x1-settings->x0;
	if (direction==-1) {fwidth *= 1.25; fheight*=1.25;}
	else {fwidth *= 0.8; fheight*=0.8;}
	settings->x0 = fmousex - fwidth/2;
	settings->x1 = fmousex + fwidth/2;
	settings->y0 = fmousey - fheight/2;
	settings->y1 = fmousey + fheight/2;
	}
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

int displayInstructions(SDL_Surface* pSurface, PhasePortraitSettings * settings)
{
	SDL_FillRect ( pSurface , NULL , g_white );  //clear surface quickly
	
	
	ShowText(
		"Features\n"
		"---------\n"
		"\n"
		"Ctrl+S\n"
		"Ctrl+O\n"
		"\n"
		"Alt+F\n"
		"Alt+B\n"
		"Alt+G\n"
		"\n"
		"Ctrl+'\n"
		"Ctrl+;\n"
		"PgUp\n"
		"PgDn\n"
		"Space\n"
		"Esc\n"
		"\n"
		"\n"
		"Arrow keys\n"
		"Ctrl-click\n"
		"Shift-click\n"
		"Right-click\n"
		, 30, 30, pSurface);
	ShowText(
		"\n"
		"\n"
		"\n"
		"Save\n"
		"Open, cycling through saved files.\n"
		"\n"
		"Full screen\n"
		"Breathing\n"
		"Show Basins\n"
		"\n"
		"Set exact values\n"
		"More settings\n"
		"Zoom in\n"
		"Zoom out\n"
		"Show this screen\n"
		"Close this screen\n"
		"\n"
		"\n"
		"Move around\n"
		"Zoom in\n"
		"Zoom out\n"
		"Reset view\n"
		, 190, 30, pSurface);
	//DrawPlotGrid(pSurface,settings, 999,999);

	SDL_Event event;
	while (TRUE)
	{
	if ( SDL_PollEvent ( &event ) )
	{
	//an event was found
	if ( event.type == SDL_QUIT ) return 0;
	else if (event.type==SDL_MOUSEBUTTONDOWN) return 0;
	else if (event.type==SDL_KEYUP)
	  {
		switch(event.key.keysym.sym)
		{
			case SDLK_SPACE: 
			case SDLK_RALT:
			case SDLK_LALT:
			case SDLK_RSHIFT: 
			case SDLK_LSHIFT:
			case SDLK_RCTRL:
			case SDLK_LCTRL:
				break;
			default: 
				return 0; //return back to the other screen!
				break;
		}
	}
	}
	SDL_UpdateRect ( pSurface , 0 , 0 , 0 , 0 ) ;
	}
	return 0;
}
