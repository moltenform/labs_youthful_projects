//Fastmapmap, Ben Fisher, 2010, GPL License.
// gParamBreathing and gParamFramesPerKeyframe are currently just held in memory, not persisted.

//todo: warning when opening wrong formula.
//todo: make compatible w csphaseportrait.
//todo: consider caching the first drawn diagram, of default view.
//todo: support zoom animations? other params animated?
//todo: have settings for faster cpus (i.e. bigger diagram, or bigger plots)
//todo: use a varargs version of showtext? low priority, just for convenience.
//todo: parameters for adjusting color, like for basins, gWithBlueAlso and shifting hue.
//todo: current directory shouldn't matter, so drag/drop is easier. findexedir in Win, hard-coded dir in Linux

#include <stdio.h>
#include <stdlib.h>

#include "common.h"
#include "configfiles.h"
#include "whichmap.h"
#include "coordsdiagram.h"
#include "font.h"
#include "phaseportrait.h"
#include "palette.h"
#include "animate.h"
#include "menagerie.h"

CoordsDiagramStruct diagramsLayout[] = {
	{&g_settings->x0, &g_settings->x1, &g_settings->y0, &g_settings->y1, 0,0,400,400,	0.0,1.0,0.0,1.0},
	{&g_settings->diagramx0, &g_settings->diagramx1, &g_settings->diagramy0, &g_settings->diagramy1, 415,100,200,200,	0.0,1.0,0.0,1.0},
	{NULL,NULL, NULL, NULL, 0,1,0,1,	0.0,1.0,0.0,1.0} //must end with null entry.
};

#include "main_util.h"

BOOL gParamBreathing = FALSE; 
int main( int argc, char* argv[] )
{
	int mouse_x,mouse_y; SDL_Event event;
	double *a = &g_settings->a; double *b = &g_settings->b; double oscA, oscB;
	BOOL needRedraw = TRUE, needDrawDiagram=TRUE;
	int PlotX = diagramsLayout[1].screen_x, PlotY = diagramsLayout[1].screen_y;
	int PlotWidth = diagramsLayout[1].screen_width, PlotHeight = diagramsLayout[1].screen_height;
	BOOL isSuperDrag = FALSE, isSuperDragSqr=FALSE; int superDragIndex=-1, superDragPx=0, superDragPy=0; double superDragx0=0, superDragx1=0,superDragy0=0,superDragy1=0;
	BOOL bShowDiagram = FALSE;

	initializeObjectToDefaults();
	loadFromFile(MAPDEFAULTFILE); //load defaults

	atexit ( SDL_Quit ) ;
	SDL_Init ( SDL_INIT_VIDEO ) ;
	//create main window
	Uint32 flags = SCREENFLAGS;
	if ((argc > 1 && StringsEqual(argv[1],"full"))||(argc > 2 && StringsEqual(argv[2],"full")))
		flags |= SDL_FULLSCREEN;
	SDL_Surface* pSurface = SDL_SetVideoMode ( SCREENWIDTH , SCREENHEIGHT , SCREENBPP , flags) ;
	BOOL bNeedToLock =  SDL_MUSTLOCK(pSurface);
	SDL_EnableKeyRepeat(30 /*SDL_DEFAULT_REPEAT_DELAY=500*/, /*SDL_DEFAULT_REPEAT_INTERVAL=30*/ 30);

	SDL_Surface* pSmallerSurface = SDL_CreateRGBSurface( SDL_SWSURFACE, diagramsLayout[1].screen_width, diagramsLayout[1].screen_height, pSurface->format->BitsPerPixel, pSurface->format->Rmask, pSurface->format->Gmask, pSurface->format->Bmask, 0 );
	SDL_FillRect ( pSurface , NULL , g_white );
	if (argc > 1 && !StringsEqual(argv[1],"full")) 
		loadFromFile(argv[1]);

	initFont();
	// holding alt and dragging is termed a "super drag" and will set a custom zoom window.
	// currently translucent red lines persist until mouse is released, this is a known issue.
while(TRUE)
{
    if ( SDL_PollEvent ( &event ) )
    {
      if ( event.type == SDL_QUIT ) return 0;
	  else if (event.type==SDL_KEYDOWN)
	  {
		switch(event.key.keysym.sym)
		{
		case SDLK_UP: *b += (event.key.keysym.mod & KMOD_SHIFT) ? 
						(g_settings->diagramy1-g_settings->diagramy0)/1024 : 0.005; needRedraw=TRUE; break;
			case SDLK_DOWN: *b -= (event.key.keysym.mod & KMOD_SHIFT) ? 
						(g_settings->diagramy1-g_settings->diagramy0)/1024 : 0.005; needRedraw=TRUE; break;
			case SDLK_LEFT: *a -= (event.key.keysym.mod & KMOD_SHIFT) ? 
						(g_settings->diagramx1-g_settings->diagramx0)/1024 : 0.005; needRedraw=TRUE; break;
			case SDLK_RIGHT: *a += (event.key.keysym.mod & KMOD_SHIFT) ? 
						(g_settings->diagramx1-g_settings->diagramx0)/1024 : 0.005; needRedraw=TRUE; break;
			case SDLK_ESCAPE: return 0; break;
			case SDLK_F4: if (event.key.keysym.mod & KMOD_ALT) return 0; break;
			default: break;
		}
	  }
	  else if (event.type==SDL_KEYUP)
	  {
		  onKeyUp(event.key.keysym.sym, (event.key.keysym.mod & KMOD_CTRL)!=0,
			  (event.key.keysym.mod & KMOD_ALT)!=0,(event.key.keysym.mod & KMOD_SHIFT)!=0, 
				pSurface, &needRedraw, &needDrawDiagram);
	  }
	  else if ( event.type == SDL_MOUSEMOTION )
	  {
		  int buttons = SDL_GetMouseState(&mouse_x, &mouse_y);
		  // if we're in the middle of a super drag, draw the rectangles.
		  if (isSuperDrag)
		  {
				 if (!(buttons & SDL_BUTTON_LMASK)) //maybe button was released 
				 {isSuperDrag = FALSE; needRedraw = TRUE;} //end the super drag.
				 else {
					 if (isSuperDragSqr)
					 {
						 int d1= MIN(mouse_x-superDragPx, mouse_y-superDragPy);
						 plotlineRectangle(pSurface, superDragPx, superDragPx+d1, superDragPy, superDragPy+d1,TRANSLUCENT_RED);
					 }
					 else
						 plotlineRectangle(pSurface, superDragPx, mouse_x, superDragPy,mouse_y,TRANSLUCENT_RED);
					SDL_UpdateRect( pSurface , 0 , 0 , 0 , 0 );
				 }
		  }
		  // otherwise, moving a dot around in the diagram.
		  else if ((buttons & SDL_BUTTON_LMASK))
		  {
			  if (mouse_x>PlotX && mouse_x<PlotX+PlotWidth && mouse_y>PlotY && mouse_y<PlotY+PlotHeight)
				screenPixelsToDouble(&diagramsLayout[1], mouse_x, mouse_y, &g_settings->a, &g_settings->b);
			  needRedraw=TRUE;
		  }
	  }
      else if ( event.type == SDL_MOUSEBUTTONDOWN ) //only called once per click
	  {
			needRedraw=TRUE;
			int buttons = SDL_GetMouseState(&mouse_x, &mouse_y);
			int mod = SDL_GetModState();

			if ((buttons & SDL_BUTTON_LMASK) && (mod & KMOD_ALT)) //alt drag, start a super drag
			{
				int index = isClickWithinDiagram(diagramsLayout, mouse_x, mouse_y);
				if (index!=-1)
				{
					superDragIndex = index;
					screenPixelsToDouble(&diagramsLayout[superDragIndex], mouse_x,mouse_y,&superDragx0,&superDragy1); /*note, y1 here is correct. */
					superDragPx =mouse_x; superDragPy = mouse_y;
					isSuperDragSqr = !(mod & KMOD_SHIFT);
					SDL_UpdateRect( pSurface , 0 , 0 , 0 , 0 );
					isSuperDrag = TRUE;
				}
			}
			//control click = zoom in; shift click=zoom out
			else if ((buttons & SDL_BUTTON_LMASK) && ((mod & KMOD_CTRL) || (mod & KMOD_SHIFT ) ) )
			{
				int direction = (mod & KMOD_CTRL) ? 1 : -1;
				int whichDiagram = onClickTryZoom(diagramsLayout, direction,mouse_x, mouse_y);
				if (whichDiagram==0) needRedraw = TRUE;
				else if (whichDiagram==1) needDrawDiagram = TRUE;
			}
			else if (buttons & SDL_BUTTON_RMASK) //right-click. undo zoom.
			{
				int index = isClickWithinDiagram(diagramsLayout, mouse_x, mouse_y);
				if (index!=-1)
					undoZoom( &diagramsLayout[index]);
				if (index==1) needDrawDiagram=TRUE;
			}
			else if (buttons & SDL_BUTTON_LMASK) //did you click on a button?
			{
				if (didClickOnButton(pSurface, mouse_x, mouse_y)==1)
				{ bShowDiagram=!bShowDiagram; needDrawDiagram=TRUE; }

				gParamBreathing = FALSE; //turn off breathing when click.
			}
			else if (buttons & SDL_BUTTON_MIDDLE) //reset the view, but leave the rest of settings intact.
			{ 
				FastMapMapSettings tmp; memcpy(&tmp, g_settings, sizeof(FastMapMapSettings));
				loadFromFile(MAPDEFAULTFILE);
				tmp.diagramx0=g_settings->diagramx0;tmp.diagramx1=g_settings->diagramx1;tmp.diagramy0=g_settings->diagramy0;tmp.diagramy1=g_settings->diagramy1;
				tmp.x0=g_settings->x0;tmp.x1=g_settings->x1;tmp.y0=g_settings->y0;tmp.y1=g_settings->y1;
				memcpy(g_settings, &tmp,  sizeof(FastMapMapSettings));
				needDrawDiagram = needRedraw = TRUE;
			}
	  }
	  else if ( event.type == SDL_MOUSEBUTTONUP )
	  {
		needRedraw=TRUE;
		SDL_GetMouseState(&mouse_x, &mouse_y);

		// when done with a super drag, set the zoom.
		if (isSuperDrag && (event.button.button == SDL_BUTTON_LEFT))
		{
			if (isSuperDragSqr)
			{
				int d1= MIN(mouse_x-superDragPx, mouse_y-superDragPy);
				screenPixelsToDouble(&diagramsLayout[superDragIndex], superDragPx+d1,superDragPy+d1,&superDragx1,&superDragy0); /* note: y0 here is correct. */
			}
			else
				screenPixelsToDouble(&diagramsLayout[superDragIndex], mouse_x,mouse_y,&superDragx1,&superDragy0); /* note: y0 here is correct. */

			if (superDragx1 > superDragx0 && superDragy1 > superDragy0) //if you try to drag off the screen, cancel it.
			{
				setZoom(&diagramsLayout[superDragIndex], superDragx0, superDragx1, superDragy0, superDragy1);
				if (superDragIndex==0) needRedraw = TRUE;
				else if (superDragIndex==1) needDrawDiagram = TRUE;
			}
			isSuperDrag = FALSE;
		}
	  }
    }

	if (LockFramesPerSecond())  //we try to keep in time, dropping frames if necessary. also, on a fast machine don't draw too fast.
	{
	if (!needRedraw && !gParamBreathing && (!needDrawDiagram||!bShowDiagram))
	{
		// we don't need to compute anything. we hopefully spend a lot of time here, unless in 'breathing' mode.
		
		//SDL_FillRect ( pSurface , NULL , 0 ); //good to debug by indicating when nothing new is computed (should be nearly always).
	}
	else
	{
		if (needDrawDiagram && bShowDiagram)
		{
			// compute the diagram
			SDL_LockSurface ( pSmallerSurface ) ;
			DrawMenagerieMultithreaded(pSmallerSurface, &diagramsLayout[1]);
			SDL_UnlockSurface ( pSmallerSurface ) ;
			needDrawDiagram = FALSE;
		}
		
		SDL_FillRect ( pSurface , NULL , g_white );  //clear surface
		if (bShowDiagram) 
			blitDiagram(pSurface, pSmallerSurface, diagramsLayout[1].screen_x,diagramsLayout[1].screen_y); 
		if (bNeedToLock) SDL_LockSurface ( pSurface ) ;
		if (!gParamBreathing) { oscA=*a; oscB = *b; }
		else { oscillateBreathing(*a,*b, &oscA, &oscB); }
		DrawFigure(pSurface, oscA, oscB, diagramsLayout[0].screen_width);
		if (bShowDiagram) drawPlotGrid(pSurface, &diagramsLayout[1], *a, *b);
		drawButtons(pSurface);
		if (bNeedToLock) SDL_UnlockSurface ( pSurface ) ;

		needRedraw = FALSE;
	}


		SDL_UpdateRect( pSurface , 0 , 0 , 0 , 0 );
	}
}
	freeFonts();
	return 0;
}



void onKeyUp(SDLKey key, BOOL bControl, BOOL bAlt, BOOL bShift, SDL_Surface*pSurface, BOOL *needRedraw, BOOL *needDrawDiagram )
{
	BOOL wasKeyCombo = TRUE;
	if (!bControl && !bAlt)
	switch (key)
	{
		//change drawing mode
		case SDLK_1: g_settings->drawingMode = bShift? DrawModeBasinsX : DrawModeBasinsDistance; break;
		case SDLK_2: g_settings->drawingMode = bShift? DrawModeBasinsDifference : DrawModeBasinsQuadrant ; break;
		case SDLK_3: 
			if (bShift) util_getNumberFrames(pSurface);
			else g_settings->drawingMode = DrawModePhase; break;
		case SDLK_4: g_settings->drawingMode = bShift? DrawModeColorDisk : DrawModeColorLine; break;
		case SDLK_5: g_settings->drawingMode = bShift? DrawModeEscapeTime : DrawModeEscapeTimeLines; break;
		case SDLK_TAB: util_switchModes(bShift); break;

		//increase/decrease iters
		case SDLK_MINUS: util_incr(-1, bShift); break;
		case SDLK_EQUALS: util_incr(1, bShift); break;

		//simulate mouseclick in the center of the diagram
		case SDLK_PAGEUP: onClickTryZoom(diagramsLayout, 1,diagramsLayout[0].screen_width/2, diagramsLayout[0].screen_height/2); break;
		case SDLK_PAGEDOWN: onClickTryZoom(diagramsLayout, -1,diagramsLayout[0].screen_width/2, diagramsLayout[0].screen_height/2); break;
		case SDLK_b: g_settings->breatheRadius += bShift? 20 : -20; break;

		case SDLK_RETURN: if (!previewAnimation(pSurface, gParamFramesPerKeyframe, diagramsLayout[0].screen_width)) Dialog_Message("Not enough keyframes to animate.",pSurface); break;
		case SDLK_DELETE: if (Dialog_GetBool("Delete all keyframes?",pSurface)) deleteAllFrames(); break;
		default: wasKeyCombo = FALSE;
	}
	else if (bControl && !bAlt)
	switch (key)
	{
		case SDLK_n:  { initializeObjectToDefaults(); loadFromFile(MAPDEFAULTFILE); *needDrawDiagram=TRUE;} break; //resets.
		case SDLK_s:  util_savefile(pSurface); break;
		case SDLK_o:  util_openfile(pSurface); *needDrawDiagram=TRUE; break;
		case SDLK_c:  util_showVals(pSurface); break;
		case SDLK_r: char* c; if(c=Dialog_GetText("Save 1600x1600 bmp as:","",pSurface)) {renderLargeFigure(pSurface,1600,c); free(c);} break;
		case SDLK_QUOTE: util_onGetExact(pSurface); break;
		case SDLK_SEMICOLON: if (bShift) util_onGetSeed(pSurface); else util_onGetMoreOptions(pSurface); break;
		case SDLK_b:  renderBreathing(pSurface,diagramsLayout[0].screen_width); break;
		case SDLK_RETURN: Dialog_Message("Rendering animation. This may take some time.", pSurface);
						 if (!renderAnimation(pSurface, gParamFramesPerKeyframe, diagramsLayout[0].screen_width)) 
							 Dialog_Message("Not enough keyframes to animate.",pSurface); break;
		default: wasKeyCombo = FALSE;
	}
	else if (!bControl && bAlt)
	switch (key)
	{
		case SDLK_b: gParamBreathing = !gParamBreathing; break;
		//bitwise operation. use xor. 1 causes bit to flip, 0 causes it to remain.
		case SDLK_1: g_settings->drawingOptions ^= maskOptionsBasinColor; break;
		case SDLK_2: g_settings->drawingOptions ^= maskOptionsQuadrantContrast; break;
		case SDLK_4: g_settings->drawingOptions ^= maskOptionsColorShowJustOneLine; break;
		case SDLK_5: if (!bShift) g_settings->drawingOptions ^= maskOptionsEscapeFillIn; 
					 else g_settings->drawingOptions ^= maskOptionsEscapeAdditionalPass; break;

		case SDLK_c: g_settings->drawingOptions ^= maskOptionsDiagramColoring; *needDrawDiagram=TRUE; break;
		case SDLK_d: g_settings->drawingOptions ^= maskOptionsDiagramMethod; *needDrawDiagram=TRUE; break;
		default: wasKeyCombo =FALSE;
	}

	if (key>=SDLK_F1 && key<=SDLK_F9)
	{
		if (!bControl && !bShift && !bAlt) { BOOL ret=openFrame(key-SDLK_F1 + 1); if (!ret) Dialog_Messagef(pSurface,"Keyframe hasn't been saved yet, press Ctrl-F%d to save it.",key-SDLK_F1+1); }
		if (bControl && bShift && !bAlt){ if (Dialog_GetBool("Delete frame?",pSurface)) deleteFrame(key-SDLK_F1 + 1); }
		else if (bControl && !bShift && !bAlt) { saveToFrame(key-SDLK_F1 + 1); Dialog_Message("Saved keyframe.", pSurface);}
		wasKeyCombo=TRUE;
	}

	if (wasKeyCombo) *needRedraw = TRUE;
}



