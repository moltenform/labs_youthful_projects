/*Fastpixelpic, Ben Fisher, 2010, GPL License. compare with fastmapscombined rev 460 and 459
//
//Differences from Fastmapscombined:
render can be any size
added other diagram
"active" diagram.
various improvements

note that sse2 is enabled in project settings. Streaming SIMD Extensions 2 (/arch:SSE2)
Fast (/fp:fast)
//todo: upon loading from file, set diagram rects so that params are shown?
*/

#include <stdio.h>
#include <stdlib.h>

#include "common.h"
#include "configfiles.h"
#include "coordsdiagram.h"
#include "font.h"
#include "figure.h"
#include "palette.h"
#include "animate.h"
#include "menagerie.h"

CoordsDiagramStruct diagramsLayout[] = {
	{&g_settings->x0, &g_settings->x1, &g_settings->y0, &g_settings->y1, 225,0,400,400,	0.0,1.0,0.0,1.0},
	{&g_settings->diagram_a_x0, &g_settings->diagram_a_x1, &g_settings->diagram_a_y0, &g_settings->diagram_a_y1, 0,4,192,192,	0.0,1.0,0.0,1.0},
	{&g_settings->diagram_b_x0, &g_settings->diagram_b_x1, &g_settings->diagram_b_y0, &g_settings->diagram_b_y1,  0,4+192+4,192,192,	0.0,1.0,0.0,1.0},
	{&g_settings->diagram_c_x0, &g_settings->diagram_c_x1, &g_settings->diagram_c_y0, &g_settings->diagram_c_y1,  0,4+192+4+192+4,192,192,	0.0,1.0,0.0,1.0},
	{NULL,NULL, NULL, NULL, 0,1,0,1,	0.0,1.0,0.0,1.0} //must end with null entry.
};
double* paramsX[] = {NULL, &g_settings->pc1, &g_settings->pc3, &g_settings->pc5, NULL};
double* paramsY[] = {NULL, &g_settings->pc2, &g_settings->pc4, &g_settings->pc6, NULL};

char* gParamFileToAppendTo = NULL;
//it's important to turn off breathing before saving 
#include "main_util.h"

int main( int argc, char* argv[] )
{
	int mouse_x,mouse_y; SDL_Event event;
	int activeDiag = -1;
	
	BOOL needRedraw = TRUE, needDrawDiagram=TRUE;

	BOOL isSuperDrag = FALSE, isSuperDragSqr=FALSE; int superDragIndex=-1, superDragPx=0, superDragPy=0; double superDragx0=0, superDragx1=0,superDragy0=0,superDragy1=0;
	BOOL bShowDiagram = TRUE;

	initializeObjectToDefaults();
	loadFromFile(MAPDEFAULTFILE); //load defaults
	

	atexit ( SDL_Quit ) ;
	SDL_Init ( SDL_INIT_VIDEO ) ;
	//create main window
	Uint32 flags = SCREENFLAGS;
	if ((argc > 1 && StringsEqual(argv[1],"-full"))||(argc > 2 && StringsEqual(argv[2],"-full")))
		flags |= SDL_FULLSCREEN;
	SDL_Surface* pSurface = SDL_SetVideoMode ( SCREENWIDTH , SCREENHEIGHT , SCREENBPP , flags) ;
	BOOL bNeedToLock =  SDL_MUSTLOCK(pSurface);
	SDL_EnableKeyRepeat(30 /*SDL_DEFAULT_REPEAT_DELAY=500*/, /*SDL_DEFAULT_REPEAT_INTERVAL=30*/ 30);

	SDL_Surface* pMainFigure = createSurface(pSurface, diagramsLayout[0].screen_width, diagramsLayout[0].screen_height);
	SDL_Surface* pDiagram1Surface = createSurface(pSurface, diagramsLayout[1].screen_width, diagramsLayout[1].screen_height);
	SDL_Surface* pDiagram2Surface = createSurface(pSurface, diagramsLayout[2].screen_width, diagramsLayout[2].screen_height);
	SDL_Surface* pDiagram3Surface = createSurface(pSurface, diagramsLayout[3].screen_width, diagramsLayout[3].screen_height);
	SDL_FillRect ( pSurface , NULL , g_white );
	if (argc > 1 && !StringsEqual(argv[1],"-full")) 
		gParamFileToAppendTo = argv[1]; //use this as the file to append to.

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
			case SDLK_UP:
			case SDLK_DOWN:{
				if (activeDiag==-1) break;
				double width = (*diagramsLayout[activeDiag].py1- *diagramsLayout[activeDiag].py0);
				double amount = getAmountToMove(width,event.key.keysym.mod&KMOD_SHIFT,event.key.keysym.mod&KMOD_CTRL,event.key.keysym.mod&KMOD_ALT);
				if (event.key.keysym.sym==SDLK_UP) *paramsY[activeDiag]+=amount; 
				else *paramsY[activeDiag]-=amount;
				needRedraw=TRUE; }
				break;
			case SDLK_LEFT:
			case SDLK_RIGHT: {
				if (activeDiag==-1) break;
				double width = (*diagramsLayout[activeDiag].px1- *diagramsLayout[activeDiag].px0);
				double amount = getAmountToMove(width,event.key.keysym.mod&KMOD_SHIFT,event.key.keysym.mod&KMOD_CTRL,event.key.keysym.mod&KMOD_ALT);
				if (event.key.keysym.sym==SDLK_RIGHT) *paramsX[activeDiag]+=amount;
				else *paramsX[activeDiag]-=amount;
				needRedraw=TRUE; }
				break;
			case SDLK_SPACE: { util_shifthue(event.key.keysym.mod & KMOD_SHIFT); needRedraw=TRUE; break; }
			case SDLK_ESCAPE: return 0; break;
			case SDLK_F4: if (event.key.keysym.mod & KMOD_ALT) return 0; break;
			default: break;
		}
	  }
	  else if (event.type==SDL_KEYUP)
	  {
		  onKeyUp(event.key.keysym.sym, (event.key.keysym.mod & KMOD_CTRL)!=0,
			  (event.key.keysym.mod & KMOD_ALT)!=0,(event.key.keysym.mod & KMOD_SHIFT)!=0, 
			   pSurface, activeDiag, &needRedraw, &needDrawDiagram);
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
			  int diagram = isClickWithinDiagram(diagramsLayout, mouse_x, mouse_y);
			  if (diagram == 1) screenPixelsToDouble(&diagramsLayout[1], mouse_x, mouse_y, &g_settings->pc1, &g_settings->pc2);
			  else if (diagram == 2) screenPixelsToDouble(&diagramsLayout[2], mouse_x, mouse_y, &g_settings->pc3, &g_settings->pc4);
			  else if (diagram == 3) screenPixelsToDouble(&diagramsLayout[3], mouse_x, mouse_y, &g_settings->pc5, &g_settings->pc6);
			  if (diagram!=0) activeDiag = diagram;
			  needRedraw=TRUE;
		  }
		  else if ((buttons & SDL_BUTTON_RMASK))
		  {
			  int diagram = isClickWithinDiagram(diagramsLayout, mouse_x, mouse_y);
			  if (diagram == 1) screenPixelsToDouble(&diagramsLayout[1], mouse_x, mouse_y, &g_settings->pc1b, &g_settings->pc2b);
			  else if (diagram == 2) screenPixelsToDouble(&diagramsLayout[2], mouse_x, mouse_y, &g_settings->pc3b, &g_settings->pc4b);
			  else if (diagram == 3) screenPixelsToDouble(&diagramsLayout[3], mouse_x, mouse_y, &g_settings->pc5b, &g_settings->pc6b);
			  if (diagram!=0) activeDiag = diagram;
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
				else if (whichDiagram!=-1) needDrawDiagram = TRUE;
			}
			else if ((buttons & SDL_BUTTON_RMASK)&&(mod & KMOD_SHIFT)) //shift-right-click. undo zoom.
			{
				int index = isClickWithinDiagram(diagramsLayout, mouse_x, mouse_y);
				if (index!=-1)
					undoZoom( &diagramsLayout[index]);
				if (index!=0) needDrawDiagram=TRUE;
			}
			else if (buttons & SDL_BUTTON_LMASK) //did you click on a button?
			{
				if (didClickOnButton(pSurface, mouse_x, mouse_y)==1)
				{ bShowDiagram=!bShowDiagram; needDrawDiagram=TRUE; }
				else 
				{
					//set the active diagram, even when not dragging.
					int diagram = isClickWithinDiagram(diagramsLayout, mouse_x, mouse_y);
					if (diagram!=0) activeDiag = diagram;
				}
				turnOffBreathing(); //turn off breathing when click.
			}
			else if (buttons & SDL_BUTTON_MIDDLE) //reset the view, but leave the rest of settings intact.
			{ 
				FastMapsSettings tmp; memcpy(&tmp, g_settings, sizeof(FastMapsSettings));
				loadFromFile(MAPDEFAULTFILE);
				tmp.diagram_a_x0=g_settings->diagram_a_x0;tmp.diagram_a_x1=g_settings->diagram_a_x1;tmp.diagram_a_y0=g_settings->diagram_a_y0;tmp.diagram_a_y1=g_settings->diagram_a_y1;
				tmp.diagram_b_x0=g_settings->diagram_b_x0;tmp.diagram_b_x1=g_settings->diagram_b_x1;tmp.diagram_b_y0=g_settings->diagram_b_y0;tmp.diagram_b_y1=g_settings->diagram_b_y1;
				tmp.diagram_c_x0=g_settings->diagram_c_x0;tmp.diagram_c_x1=g_settings->diagram_c_x1;tmp.diagram_c_y0=g_settings->diagram_c_y0;tmp.diagram_c_y1=g_settings->diagram_c_y1;
				tmp.x0=g_settings->x0;tmp.x1=g_settings->x1;tmp.y0=g_settings->y0;tmp.y1=g_settings->y1;
				memcpy(g_settings, &tmp,  sizeof(FastMapsSettings));
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
				else if (superDragIndex!=-1) needDrawDiagram = TRUE;
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
			/*SDL_LockSurface ( pSmallerSurface ) ;
			DrawMenagerieMultithreaded(pSmallerSurface, &diagramsLayout[1]);
			SDL_UnlockSurface ( pSmallerSurface ) ;*/
			needDrawDiagram = FALSE;
		}
		
		SDL_FillRect ( pSurface , NULL , g_white );  //clear surface
		if (bShowDiagram) 
		{
			//blitDiagram(pSurface, pSmallerSurface, diagramsLayout[1].screen_x,diagramsLayout[1].screen_y); 
			//double whicha = g_settings->a, whichb = g_settings->b;
			//DrawBasinsBasic(pSurface, whicha, whichb, &diagramsLayout[2]);
		}
		if (bNeedToLock) SDL_LockSurface ( pSurface ) ;

		if (gParamBreathing) { oscillateBreathing(savedC1,savedC2, &g_settings->pc1, &g_settings->pc2); }
		DrawFigure(pMainFigure, diagramsLayout[0].screen_width);
		blitDiagram(pSurface, pMainFigure, diagramsLayout[0].screen_x,diagramsLayout[0].screen_y);
		if (bShowDiagram) drawPlotGrid(pSurface, &diagramsLayout[1], g_settings->pc1, g_settings->pc2, g_settings->pc1b, g_settings->pc2b);
		if (bShowDiagram) drawPlotGrid(pSurface, &diagramsLayout[2], g_settings->pc3, g_settings->pc4, g_settings->pc3b, g_settings->pc4b);
		if (bShowDiagram) drawPlotGrid(pSurface, &diagramsLayout[3], g_settings->pc5, g_settings->pc6, g_settings->pc5b, g_settings->pc6b);
		drawButtons(pSurface);
		drawActive(pSurface, activeDiag);
		if (bNeedToLock) SDL_UnlockSurface ( pSurface ) ;

		needRedraw = FALSE;
	}


		SDL_UpdateRect( pSurface , 0 , 0 , 0 , 0 );
	}
}
	freeFonts();
	return 0;
}



//consider when we must turn off breathing: before save, before render, before renderBreathing
//don't yet call turnOffBreathing(); on each keyup, because can be fun to be zooming while breathing/so on
void onKeyUp(SDLKey key, BOOL bControl, BOOL bAlt, BOOL bShift, SDL_Surface*pSurface, int activeDiag, BOOL *needRedraw, BOOL *needDrawDiagram )
{	
	BOOL wasKeyCombo = TRUE; //assume it was a shortcut, drop to default if wasn't
	if (!bControl && !bAlt)
	switch (key)
	{
		//change drawing mode
		//case SDLK_1: g_settings->coloringMode = bShift? ColorModeBlackBlueSqrt : ColorModeBlackBlue; break;
		

		//increase/decrease iters
		case SDLK_MINUS: util_incr(-1, bShift); break;
		case SDLK_EQUALS: util_incr(1, bShift); break;

		//simulate mouseclick in the center of the diagram
		case SDLK_PAGEUP: onClickTryZoom(diagramsLayout, 1,diagramsLayout[0].screen_width/2, diagramsLayout[0].screen_height/2); break;
		case SDLK_PAGEDOWN: onClickTryZoom(diagramsLayout, -1,diagramsLayout[0].screen_width/2, diagramsLayout[0].screen_height/2); break;
		case SDLK_b: g_settings->breatheRadius_c1c2 += bShift? 20 : -20; break;

		case SDLK_u: util_changeMaxValue(bShift); break;

		case SDLK_RETURN: if (!previewAnimation(pSurface, gParamFramesPerKeyframe, diagramsLayout[0].screen_width)) Dialog_Message("Not enough keyframes to animate.",pSurface); break;
		case SDLK_DELETE: if (Dialog_GetBool("Delete all keyframes?",pSurface)) deleteAllFrames(); break;
		
		default: wasKeyCombo = FALSE;
	}
	else if (bControl && !bAlt)
	switch (key)
	{
		//ctrl-n reverts to saved. ctrl-shift-n resets everything.
		case SDLK_n:  { turnOffBreathing(); initializeObjectToDefaults(); if (!bShift) loadFromFile(MAPDEFAULTFILE); *needDrawDiagram=TRUE;} break; //revert to saved version
		case SDLK_s:  {
			turnOffBreathing();
			BOOL bRes = appendToFilePython(gParamFileToAppendTo); 
			Dialog_Message(bRes?"Saved.": "Save Failed.",pSurface); break; }
		//case SDLK_o:  util_openfile(pSurface); *needDrawDiagram=TRUE; break;
		case SDLK_c:  util_showVals(pSurface); break;
		case SDLK_r: char* c; if(c=Dialog_GetText("Save 1600x1600 bmp as:","",pSurface)) {renderLargeFigure(pSurface,1600,c); free(c);} break;
		case SDLK_QUOTE: util_onGetExact(pSurface, activeDiag); break;
		case SDLK_SEMICOLON: util_onGetMoreOptions(pSurface); break;
		case SDLK_b: turnOffBreathing(); renderBreathing(pSurface,diagramsLayout[0].screen_width); break;
		case SDLK_RETURN: Dialog_Message("Rendering animation. This may take some time.", pSurface);
						 if (!renderAnimation(pSurface, gParamFramesPerKeyframe, diagramsLayout[0].screen_width)) 
							 Dialog_Message("Not enough keyframes to animate.",pSurface); break;

		
		default: wasKeyCombo = FALSE;
	}
	else if (!bControl && bAlt)
	switch (key)
	{
		case SDLK_b: if (gParamBreathing) turnOffBreathing(); else turnOnBreathing(); break;

		case SDLK_w: util_changeWrapping(); break;
		//case SDLK_SPACE: util_shifthue(bShift); break; in mousedown
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

