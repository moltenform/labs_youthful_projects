#pragma warning (disable:4996)
//about fopen, scanf

//todo: make fully compatible w csphaseportrait.

#include <stdio.h>
#include <stdlib.h>
#include <memory.h>
#include <math.h>

#include "common.h"
#include "configfiles.h"
#include "whichmap.h"
#include "coordsdiagram.h"
#include "font.h"
#include "phaseportrait.h"

CoordsDiagramStruct thediagrams[] = {
	{&g_settings->x0, &g_settings->x1, &g_settings->y0, &g_settings->y1, 0,0,400,400,	0.0,1.0,0.0,1.0},
	{&g_settings->diagramx0, &g_settings->diagramx1, &g_settings->diagramy0, &g_settings->diagramy1, 400,0,200,200,	0.0,1.0,0.0,1.0},
	{NULL,NULL, NULL, NULL, 0,1,0,1,	0.0,1.0,0.0,1.0}
};

#include "main_util.h" 

BOOL breathing = FALSE; 
void oscillate(double curA,double curB,double *outA, double *outB);
void onKeyUp(SDLKey key, BOOL bControl, BOOL bAlt, BOOL bShift, SDL_Surface*pSurface, BOOL *needDrawPhase, BOOL *needDrawDiagram );
int main( int argc, char* argv[] )
{
	int mouse_x,mouse_y; SDL_Event event;
	double *a = &g_settings->a; double *b = &g_settings->b; double oscA, oscB;
	BOOL needDrawPhase = TRUE; BOOL needDrawDiagram = TRUE;
	int PlotX = thediagrams[1].screen_x, PlotY = thediagrams[1].screen_y;
	int PlotWidth = thediagrams[1].screen_width, PlotHeight = thediagrams[1].screen_height;
	BOOL isSuperDrag = FALSE; int superDragIndex=-1; double superDragx0=0, superDragx1=0,superDragy0=0,superDragy1=0;

	initializeObject();
	loadFromFile(MAPDEFAULTFILE); //load defaults
	if (argc > 1 && !StringsEqual(argv[1],"full")) loadFromFile(argv[1]);
	
	atexit ( SDL_Quit ) ;
	SDL_Init ( SDL_INIT_VIDEO ) ;
	//create main window
	Uint32 flags = SCREENFLAGS;
	if ((argc > 1 && StringsEqual(argv[1],"full"))||(argc > 2 && StringsEqual(argv[2],"full")))
		flags |= SDL_FULLSCREEN;
	SDL_Surface* pSurface = SDL_SetVideoMode ( SCREENWIDTH , SCREENHEIGHT , SCREENBPP , flags) ;
	BOOL bNeedToLock =  SDL_MUSTLOCK(pSurface);
	SDL_EnableKeyRepeat(30 /*SDL_DEFAULT_REPEAT_DELAY=500*/, /*SDL_DEFAULT_REPEAT_INTERVAL=30*/ 30);

	SDL_Surface* pSmallerSurface = SDL_CreateRGBSurface( SDL_SWSURFACE, thediagrams[1].screen_width, thediagrams[1].screen_height, pSurface->format->BitsPerPixel, pSurface->format->Rmask, pSurface->format->Gmask, pSurface->format->Bmask, 0 );
	SDL_FillRect ( pSurface , NULL , g_white );

while(TRUE)
{
    if ( SDL_PollEvent ( &event ) )
    {
      if ( event.type == SDL_QUIT ) return 0;
	  else if (event.type==SDL_KEYDOWN)
	  {
		switch(event.key.keysym.sym)
		{
			case SDLK_UP: *b += (event.key.keysym.mod & KMOD_SHIFT) ? 0.0005 : 0.005; needDrawPhase=TRUE; break;
			case SDLK_DOWN: *b -= (event.key.keysym.mod & KMOD_SHIFT) ? 0.0005 : 0.005; needDrawPhase=TRUE; break;
			case SDLK_LEFT: *a -= (event.key.keysym.mod & KMOD_SHIFT) ? 0.0005 : 0.005; needDrawPhase=TRUE; break;
			case SDLK_RIGHT: *a += (event.key.keysym.mod & KMOD_SHIFT) ? 0.0005 : 0.005; needDrawPhase=TRUE; break;
			case SDLK_ESCAPE: return 0; break;
			case SDLK_F4: if (event.key.keysym.mod & KMOD_ALT) return 0; break;
			default: break;
		}
	  }
	  else if (event.type==SDL_KEYUP)
	  {
		  onKeyUp(event.key.keysym.sym, (event.key.keysym.mod & KMOD_CTRL)!=0,
			  (event.key.keysym.mod & KMOD_ALT)!=0,(event.key.keysym.mod & KMOD_SHIFT)!=0, 
				pSurface, &needDrawPhase, &needDrawDiagram);
	  }
	  else if ( event.type == SDL_MOUSEMOTION )
	  {
		  int buttons = SDL_GetMouseState(&mouse_x, &mouse_y);
		  if (isSuperDrag)
		  {
				 if (!(buttons & SDL_BUTTON_LMASK)) //maybe button was released 
				 {isSuperDrag = FALSE; needDrawPhase = TRUE;} //end the super drag.
				 else {
					 plotpointcolor(pSurface, mouse_x, mouse_y, 0x00ff0000);
					SDL_UpdateRect( pSurface , 0 , 0 , 0 , 0 );
				 }
		  }
		  else if ((buttons & SDL_BUTTON_LMASK))
		  {
			  if (mouse_x>PlotX && mouse_x<PlotX+PlotWidth && mouse_y>PlotY && mouse_y<PlotY+PlotHeight)
				screenPixelsToDouble(&thediagrams[1], mouse_x, mouse_y, &g_settings->a, &g_settings->b);
			  needDrawPhase=TRUE;
		  }
	  }
      else if ( event.type == SDL_MOUSEBUTTONDOWN ) //only called once per click
	  {
		  needDrawPhase=TRUE;
			int buttons = SDL_GetMouseState(&mouse_x, &mouse_y);
			int mod = SDL_GetModState();

			if ((buttons & SDL_BUTTON_LMASK) && (mod & KMOD_ALT)) //alt drag, zoom window!
			{
				int index = isClickWithinDiagram(thediagrams, mouse_x, mouse_y);
				if (index!=-1)
				{
					superDragIndex = index;
					screenPixelsToDouble(&thediagrams[superDragIndex], mouse_x,mouse_y,&superDragx0,&superDragy0);
					plotpointcolor(pSurface, mouse_x, mouse_y, 0x00ff0000);
					SDL_UpdateRect( pSurface , 0 , 0 , 0 , 0 );
					isSuperDrag = TRUE;
				}
			}
			//control click = zoom in, shift click=zoom out
			else if ((buttons & SDL_BUTTON_LMASK) && ((mod & KMOD_CTRL) || (mod & KMOD_SHIFT ) ) )
			{
				int direction = (mod & KMOD_CTRL) ? 1 : -1;
				int whichDiagram = onClickTryZoom(thediagrams, direction,mouse_x, mouse_y);
				if (whichDiagram==0) needDrawPhase = TRUE;
				else if (whichDiagram==1) needDrawDiagram = TRUE;
			}
			else if (buttons & SDL_BUTTON_RMASK) //right-click. undo zoom.
			{
				int index = isClickWithinDiagram(thediagrams, mouse_x, mouse_y);
				if (index!=-1)
					undoZoom( &thediagrams[index]);
			}
	  }
	  else if ( event.type == SDL_MOUSEBUTTONUP )
	  {
		  needDrawPhase=TRUE;
		  int buttons = SDL_GetMouseState(&mouse_x, &mouse_y);
		  if (isSuperDrag && (buttons & SDL_BUTTON_LMASK)) {
				//wow, cool. set the zoom!
			  screenPixelsToDouble(&thediagrams[superDragIndex], mouse_x,mouse_y,&superDragx1,&superDragy1);
			  if (superDragx1 > superDragx0 && superDragy1 > superDragy0)
			  {
				  setZoom(&thediagrams[superDragIndex], superDragx0, superDragx1, superDragy0, superDragy1);
				if (superDragIndex==0) needDrawPhase = TRUE;
				else if (superDragIndex==1) needDrawDiagram = TRUE;
			  }
			  isSuperDrag = FALSE;
		  }
	  }
    }

	if (LockFramesPerSecond())  //show ALL frames (if slower) or keep it going in time, dropping frames? put stuff in here
	{
	if (!needDrawDiagram && !needDrawPhase)
	{
		// don't need to compute anything.
		//debug by drawing black indicating nothing new is computed.
		//SDL_FillRect ( pSurface , NULL , 0 );
	}
	else
	{
		if (needDrawDiagram)
		{
			// recompute the figure
			//DrawMenagerie(pSmallerSurface, settings);
			needDrawDiagram = FALSE;
		}
		
		SDL_FillRect ( pSurface , NULL , g_white );  //clear surface quickly
		//BlitMenagerie(pSurface, pSmallerSurface); 
		if (bNeedToLock) SDL_LockSurface ( pSurface ) ;
		if (!breathing) { oscA=*a; oscB = *b; }
		else { oscillate(*a,*b, &oscA, &oscB); }
		DrawFigure(pSurface, oscA, oscB, thediagrams[0].screen_width);
		DrawPlotGrid(pSurface, &thediagrams[1], *a, *b);
		//DrawButtons(pSurface);
		if (bNeedToLock) SDL_UnlockSurface ( pSurface ) ;

		needDrawPhase = FALSE;
	}


		SDL_UpdateRect( pSurface , 0 , 0 , 0 , 0 );
	}
}

	return 0;
}

void onKeyUp(SDLKey key, BOOL bControl, BOOL bAlt, BOOL bShift, SDL_Surface*pSurface, BOOL *needDrawPhase, BOOL *needDrawDiagram )
{
	BOOL wasKeyCombo =TRUE;
	if (bControl && !bAlt)
	switch (key)
	{
		case SDLK_n:  { initializeObject(); loadFromFile(MAPDEFAULTFILE); *needDrawDiagram=TRUE;} break;
		case SDLK_s:  util_savefile(pSurface); break;
		case SDLK_o:  util_openfile(pSurface); break;
		case SDLK_c:  util_showVals(pSurface); break;
		case SDLK_QUOTE: util_onGetExact(pSurface); break;
		case SDLK_SEMICOLON: if (bShift) util_onGetSeed(pSurface); else util_onGetMoreOptions(pSurface); break;
		default: wasKeyCombo =FALSE;
	}
	else if (!bControl && bAlt)
	switch (key)
	{
		case SDLK_q: g_settings->drawingMode = DrawModePhase; break;
		case SDLK_w: g_settings->drawingMode = DrawModeBasins; break;
		case SDLK_e: g_settings->drawingMode = DrawModeColorLine; break;
		case SDLK_r: g_settings->drawingMode = DrawModeColorDisk; break;
		case SDLK_b: breathing = !breathing; break;
		default: wasKeyCombo =FALSE;
	}
	else wasKeyCombo =FALSE;

	if (wasKeyCombo) *needDrawPhase = TRUE;
}


void oscillate(double curA,double curB,double *outA, double *outB)
{
	static double t=0;
	t+=0.13;
	if (t>3141.5926) t=0.0;

	*outA = curA + sin( t +0.03*cos(t/8.5633) +3.685)/200;
	*outB = curB + cos( 0.8241*t +0.02*sin(t/9.24123+5.742) )/263;
}

