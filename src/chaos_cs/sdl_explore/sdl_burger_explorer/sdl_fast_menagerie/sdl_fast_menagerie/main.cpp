#pragma warning (disable:4996)
//about fopen, scanf

#include <stdio.h>
#include <stdlib.h>
#include <memory.h>
#include <math.h>

#include "common.h"
#include "fastmenagerie.h"
#include "font.h"
#include "phaseportrait.h"
#include "timecounter.h"


Uint32 g_white;
int MenagHeight=512, MenagWidth=512, MenagColorLegend=4; //384
int PhasePlotHeight=128, PhasePlotWidth=128, PhasePlotX=384;
BOOL onKeyUp(SDLKey key, BOOL bControl, BOOL bAlt, BOOL bShift, SDL_Surface*pSurface, MenagFastSettings*settings, double *outA, double *outB);
void tryDrawPhasePortrait(int mouse_x, int mouse_y, MenagFastSettings*settings, double *outA, double *outB);
void tryZoomPlot(int direction, int mouse_x, int mouse_y, MenagFastSettings*settings);
void zoomPortrait(int direction, MenagFastSettings * settings );
void showVals(SDL_Surface* pSurface, double a,double b);

int main( int argc, char* argv[] )
{
	MenagFastSettings ssettings; MenagFastSettings * settings = &ssettings;
	double curA=0.0, curB=0.0;
	char timetext[256]={0};
	InitialSettings(settings, MenagHeight, MenagWidth, &curA, &curB);
	curA=99, curB=99; //don't show the indicator at first, for aesthetic reasons

	SDL_Rect busyIndication;
	busyIndication.x = 0; busyIndication.y = 0; busyIndication.w = 6; busyIndication.h = 6; 

	BOOL shouldRedraw = FALSE, waitingForCompletion=FALSE;
	SDL_Event event;

	atexit ( SDL_Quit ) ;
	SDL_Init ( SDL_INIT_VIDEO ) ;
	//create main window
	Uint32 flags = SCREENFLAGS;
	if ((argc > 1 && StringsEqual(argv[1],"full"))||(argc > 2 && StringsEqual(argv[2],"full")))
		flags |= SDL_FULLSCREEN;
	SDL_Surface* pSurface = SDL_SetVideoMode ( SCREENWIDTH , SCREENHEIGHT , SCREENBPP , flags) ;
	BOOL bNeedToLock =  SDL_MUSTLOCK(pSurface);
	SDL_Surface* pMenagSurface = SDL_CreateRGBSurface( SDL_SWSURFACE, MenagWidth+MenagColorLegend, MenagHeight, pSurface->format->BitsPerPixel, pSurface->format->Rmask, pSurface->format->Gmask, pSurface->format->Bmask, 0 );
	g_white = SDL_MapRGB ( pSurface->format , 255,255,255 ) ;

	while(TRUE) {
		if ( SDL_PollEvent ( &event ) )
		{
			if ( event.type == SDL_QUIT ) return 0;
			else if (event.type==SDL_KEYDOWN && event.key.keysym.sym==SDLK_ESCAPE) return 0;
			else if (event.type==SDL_KEYUP)
			{
				if (event.key.keysym.sym==SDLK_F4 && event.key.keysym.mod & KMOD_ALT)
					return 0;
				BOOL redraw = onKeyUp(event.key.keysym.sym, (event.key.keysym.mod & KMOD_CTRL)!=0,
					(event.key.keysym.mod & KMOD_ALT)!=0,(event.key.keysym.mod & KMOD_SHIFT)!=0, 
					pSurface, settings, &curA, &curB);
				//ususally we'll know if we need to redraw because A and B will change
				shouldRedraw = redraw;
			}
			else if ( event.type == SDL_MOUSEBUTTONDOWN )
			{
				int mouse_x, mouse_y;
				//only called once per click
				int buttons = SDL_GetMouseState(&mouse_x, &mouse_y);
				int mod = SDL_GetModState();

				//control click = zoom in, shift click=zoom out
				if ((buttons & SDL_BUTTON_LMASK) && ((mod & KMOD_CTRL) || (mod & KMOD_SHIFT ) ) )
				{
					if (!g_BusyThread1 && !g_BusyThread2) {
				SDL_FillRect ( pMenagSurface , &busyIndication , 0);
				int direction = (mod & KMOD_CTRL) ? 1 : -1;
				tryZoomPlot(direction, mouse_x, mouse_y, settings);
				startMenagCalculation(settings, direction, pSurface->format);
				waitingForCompletion = TRUE;
				shouldRedraw = TRUE;
				startTimer();
					}
				}
				else if (buttons & SDL_BUTTON_RMASK) //right-click resets
				{
					if (!g_BusyThread1 && !g_BusyThread2) {
				SDL_FillRect ( pMenagSurface , &busyIndication , 0);
				InitialSettings(settings, MenagHeight, MenagWidth, &curA, &curB);
				startMenagCalculation(settings, 0, pSurface->format);
				curA=99, curB=99; //get rid of cursor, phase plot
				waitingForCompletion = TRUE; 
				shouldRedraw = TRUE;
				startTimer();
					}
				}
				else //normal-click draws phase diagram
				{
				tryDrawPhasePortrait(mouse_x, mouse_y, settings, &curA, &curB);
				shouldRedraw = TRUE;
				}
			}
		}

		if (LockFramesPerSecond())
		{
			if (waitingForCompletion && (!g_BusyThread1 && !g_BusyThread2))
			{
				constructMenagerieSurface(settings, pMenagSurface);
				int time = (int) stopTimer();
				snprintf(timetext, sizeof(timetext), "t:%d", time);
				waitingForCompletion = FALSE;
				shouldRedraw = TRUE;
			}


			//BOOL bNeedsNewMenag = prevPlotX0!=settings->browsex0||prevPlotX1!=settings->browsex1||prevPlotY0!=settings->browsey0||prevPlotY1!=settings->browsey1;
			//BOOL bNeedsNewPhasePlot = prevA!=curA || prevB != curB;
			if (!shouldRedraw)
			{
				// don't need to compute anything.
				//SDL_FillRect ( pSurface , NULL , 0 );
				//SDL_UpdateRect ( pSurface , 0 , 0 , 0 , 0 ) ;
			}
			else
			{
				SDL_FillRect ( pSurface , NULL , g_white );  //clear surface quickly
				BlitMenagerie(pSurface, pMenagSurface); 
				if (bNeedToLock) SDL_LockSurface ( pSurface ) ;
				DrawPhasePortrait(pSurface, settings, curA,curB);
				//DrawPhasePortrait2(pSurface, settings, curA,curB);
				DrawPlotGrid(pSurface,settings, curA,curB);
				if (bNeedToLock) SDL_UnlockSurface ( pSurface ) ;
				//prevA = curA; prevB = curB;
				ShowText(timetext, 0, 550, pSurface);
				SDL_UpdateRect ( pSurface , 0 , 0 , 0 , 0 ) ;
				shouldRedraw = FALSE;

			}
		}
	}
	return 0;
}


BOOL onKeyUp(SDLKey key, BOOL bControl, BOOL bAlt, BOOL bShift, SDL_Surface*pSurface, MenagFastSettings*settings, double *outA, double *outB)
{
	BOOL redrawPlot=TRUE;
	switch (key)
	{
		//case SDLK_s: if (bControl&&bAlt) saveAllKeyframes(pSurface); else if (bControl) onSave(settings,*outA,*outB, pSurface);  break;
		case SDLK_PAGEUP: zoomPortrait(1,settings);  break;
		case SDLK_PAGEDOWN: zoomPortrait(-1,settings);  break;
		case SDLK_UP: *outB += (bShift) ? 0.0005 : 0.005; break;
		case SDLK_DOWN: *outB -= (bShift) ? 0.0005 : 0.005; break;
		case SDLK_LEFT: *outA -= (bShift) ? 0.0005 : 0.005; break;
		case SDLK_RIGHT: *outA += (bShift) ? 0.0005 : 0.005; break;
		case SDLK_t: if (bControl) togglePhasePortraitTransients(); break;
		case SDLK_QUOTE: if (bControl) showVals(pSurface,*outA,*outB); break;
		default: redrawPlot=FALSE;
	}
	return redrawPlot;
}

void tryDrawPhasePortrait(int mouse_x, int mouse_y, MenagFastSettings*settings, double *outA, double *outB)
{
	double fmousex, fmousey;
	if (mouse_x>0&& mouse_x<MenagWidth && mouse_y>0 && mouse_y<MenagHeight)
	{
		IntMenagCoordsToDouble(settings, mouse_x, mouse_y, &fmousex, &fmousey);
		*outA = fmousex; 
		*outB = fmousey;
	}
}

void tryZoomPlot(int direction, int mouse_x, int mouse_y, MenagFastSettings*settings)
{
	double fmousex, fmousey;
	if (mouse_x>0&& mouse_x<MenagWidth && mouse_y>0 && mouse_y<MenagHeight)
	{
		IntMenagCoordsToDouble(settings, mouse_x, mouse_y, &fmousex, &fmousey);
		double fwidth=settings->browsex1-settings->browsex0, fheight=settings->browsex1-settings->browsex0;
		if (direction==-1) {fwidth *= 2; fheight*=2;}
		else {fwidth *= 0.5; fheight*=0.5;}
		settings->browsex0 = fmousex - fwidth/2;
		settings->browsex1 = fmousex + fwidth/2;
		settings->browsey0 = fmousey - fheight/2;
		settings->browsey1 = fmousey + fheight/2;
	}
}

void showVals(SDL_Surface* pSurface, double a,double b)
{
char buf[256];
snprintf(buf, sizeof(buf),"a:%f b:%f", a,b);
Dialog_Message(buf, pSurface);
}


//Uint32 g_white;
//
//void zoomPortrait(int direction, PhasePortraitSettings * settings);
//void tryZoomPlot(int direction, int mouse_x, int mouse_y, PhasePortraitSettings*settings);
//int displayInstructions(SDL_Surface* pSurface,PhasePortraitSettings * settings);
//BOOL onKeyUp(SDLKey key, BOOL bControl, BOOL bAlt,BOOL bShift, SDL_Surface*pSurface, PhasePortraitSettings*settings, double *outA, double *outB);
//
//void testThreading();
//void testThreadingTwo();
//
//
//
//int main( int argc, char* argv[] )
//{
//	PhasePortraitSettings ssettings; PhasePortraitSettings * settings = &ssettings;
//	double curA=0.0, curB=0.0, prevA=99,prevB=99, prevPlotX1=99, prevPlotY1=99;
//	InitialSettings(settings, PhaseHeight, PhaseWidth, &curA, &curB);
//	//load a file if parameter given.
//	if (argc > 1 && !StringsEqual(argv[1],"full"))
//		loadData(settings, argv[1], &curA, &curB);
//	
//
//	atexit ( SDL_Quit ) ;
//	SDL_Init ( SDL_INIT_VIDEO ) ;
//	//create main window
//	Uint32 flags = SCREENFLAGS;
//	if ((argc > 1 && StringsEqual(argv[1],"full"))||(argc > 2 && StringsEqual(argv[2],"full")))
//		flags |= SDL_FULLSCREEN;
//	SDL_Surface* pSurface = SDL_SetVideoMode ( SCREENWIDTH , SCREENHEIGHT , SCREENBPP , flags) ;
//
//	SDL_Event event;
//	BOOL bNeedToLock =  SDL_MUSTLOCK(pSurface);
//	SDL_EnableKeyRepeat(30 /*SDL_DEFAULT_REPEAT_DELAY=500*/, /*SDL_DEFAULT_REPEAT_INTERVAL=30*/ 30);
//	int mouse_x,mouse_y;
//
//	//cache the home menagerie? 
//	SDL_Surface* pMenagSurface = SDL_CreateRGBSurface( SDL_SWSURFACE, PlotWidth, PlotHeight, pSurface->format->BitsPerPixel, pSurface->format->Rmask, pSurface->format->Gmask, pSurface->format->Bmask, 0 );
//	DrawMenagerie(pMenagSurface, settings, FALSE); 
//	//and set this.
//	prevPlotX1 = settings->browsex1; prevPlotY1 = settings->browsey1;
//
//	g_white = SDL_MapRGB ( pSurface->format , 255,255,255 ) ;
//	SDL_FillRect ( pSurface , NULL , g_white );
//
//

//}
//
//BOOL onKeyUp(SDLKey key, BOOL bControl, BOOL bAlt, BOOL bShift, SDL_Surface*pSurface, PhasePortraitSettings*settings, double *outA, double *outB)
//{
//	BOOL needtodraw = TRUE;
//	//some of these needlessly set needtodraw, but whatever.
//	switch (key)
//	{
//		case SDLK_s: if (bControl&&bAlt) saveAllKeyframes(pSurface); else if (bControl) onSave(settings,*outA,*outB, pSurface);  break;
//		case SDLK_o: if (bControl&&bAlt) openAllKeyframes(pSurface); else if (bControl) onOpen(settings,outA,outB, bShift); break;
//		case SDLK_QUOTE: if (bControl) onGetExact(settings,outA,outB, pSurface); break;
//		case SDLK_SEMICOLON: if (bControl) onGetMoreOptions(settings, pSurface); break;
//		case SDLK_F11: fullscreen(pSurface, FALSE, settings, outA,outB);  break;
//		case SDLK_f: if (bAlt) {fullscreen(pSurface, FALSE, settings, outA,outB);} break;
//		case SDLK_b: if (bAlt) {fullscreen(pSurface, TRUE, settings, outA,outB);} break;
//		case SDLK_g: if (bAlt) {settings->drawBasin = !settings->drawBasin;} break;
//		case SDLK_PAGEUP: zoomPortrait(1,settings);  break;
//		case SDLK_PAGEDOWN: zoomPortrait(-1,settings);  break;
//		case SDLK_BACKSPACE: if (bControl&&bShift&&Dialog_GetBool("Delete all frames?",pSurface)) {deleteFrames();*outA=*outB=0;} break;
//		case SDLK_0: 
//			if (bAlt) {
//				if (Dialog_GetBool("Render animation?",pSurface)) dowriteanimation(pSurface, framesPerKeyframe); 
//			} else 
//				dotestanimation(pSurface, framesPerKeyframe); 
//			break;
//		case SDLK_MINUS: if (bAlt) Dialog_GetInt("Frames per key frame:",pSurface,&framesPerKeyframe); break;
//		case SDLK_SPACE: 
//		case SDLK_RETURN: 
//		case SDLK_KP_ENTER: 
//			displayInstructions(pSurface, settings);  break;
//		default: 
//			if (key>=SDLK_F1 && key <= SDLK_F9) 
//				loadPreset(key+1 - SDLK_F1,bShift,bAlt, settings, outA, outB);
//				 
//			else if (key >= SDLK_1 && key <= SDLK_9)
//			{
//				if (bAlt) openFrame(key-SDLK_0, settings, outA,outB);
//				else if (bControl&&bShift&&Dialog_GetBool("Delete frame?",pSurface)) {deleteFrame(key-SDLK_0); openFrame(0, settings, outA,outB);}
//				else if (bControl) saveToFrame(key-SDLK_0, settings, *outA,*outB);
//			}
//			else 
//				needtodraw = FALSE;
//			break;
//	}
//
//
//	return needtodraw;
//}
//
//
//void tryZoomPlot(int direction, int mouse_x, int mouse_y, PhasePortraitSettings*settings)
//{
//	double fmousex, fmousey;
//	if (mouse_x>PlotX && mouse_x<PlotX+PlotWidth && mouse_y>0 && mouse_y<PlotHeight)
//	{
//	IntPlotCoordsToDouble(settings, mouse_x, mouse_y, &fmousex, &fmousey);
//	double fwidth=settings->browsex1-settings->browsex0, fheight=settings->browsex1-settings->browsex0;
//	if (direction==-1) {fwidth *= 1.25; fheight*=1.25;}
//	else {fwidth *= 0.8; fheight*=0.8;}
//	settings->browsex0 = fmousex - fwidth/2;
//	settings->browsex1 = fmousex + fwidth/2;
//	settings->browsey0 = fmousey - fheight/2;
//	settings->browsey1 = fmousey + fheight/2;
//	}
//	else if (mouse_x>0 && mouse_x<PhaseWidth && mouse_y>0 && mouse_y<PhaseHeight)
//	{
//	IntPhaseCoordsToDouble(settings, mouse_x, mouse_y, &fmousex, &fmousey);
//	double fwidth=settings->x1-settings->x0, fheight=settings->x1-settings->x0;
//	if (direction==-1) {fwidth *= 1.25; fheight*=1.25;}
//	else {fwidth *= 0.8; fheight*=0.8;}
//	settings->x0 = fmousex - fwidth/2;
//	settings->x1 = fmousex + fwidth/2;
//	settings->y0 = fmousey - fheight/2;
//	settings->y1 = fmousey + fheight/2;
//	}
//}
//void zoomPortrait(int direction, PhasePortraitSettings * settings )
//{
//	double fcenterx, fcentery;
//	fcenterx= (settings->x1+settings->x0)/2;
//	fcentery= (settings->y1+settings->y0)/2;
//	double fwidth=settings->x1-settings->x0, fheight=settings->x1-settings->x0;
//	if (direction==-1) {fwidth *= 1.25; fheight*=1.25;}
//	else {fwidth *= 0.8; fheight*=0.8;}
//	settings->x0 = fcenterx - fwidth/2;
//	settings->x1 = fcenterx + fwidth/2;
//	settings->y0 = fcentery - fheight/2;
//	settings->y1 = fcentery + fheight/2;
//}
//*/
//
//int displayInstructions(SDL_Surface* pSurface, PhasePortraitSettings * settings)
//{
//	SDL_FillRect ( pSurface , NULL , g_white );  //clear surface quickly
//	
//	
//	ShowText(
//		"Features\n"
//		"---------\n"
//		"\n"
//		"Ctrl+S\n"
//		"Ctrl+O\n"
//		"\n"
//		"Alt+F\n"
//		"Alt+B\n"
//		"Alt+G\n"
//		"\n"
//		"Ctrl+'\n"
//		"Ctrl+;\n"
//		"PgUp\n"
//		"PgDn\n"
//		"Space\n"
//		"Esc\n"
//		"\n"
//		"\n"
//		"Arrow keys\n"
//		"Ctrl-click\n"
//		"Shift-click\n"
//		"Right-click\n"
//		, 30, 30, pSurface);
//	ShowText(
//		"\n"
//		"\n"
//		"\n"
//		"Save\n"
//		"Open, cycling through saved files.\n"
//		"\n"
//		"Full screen\n"
//		"Breathing\n"
//		"Show Basins\n"
//		"\n"
//		"Set exact values\n"
//		"More settings\n"
//		"Zoom in\n"
//		"Zoom out\n"
//		"Show this screen\n"
//		"Close this screen\n"
//		"\n"
//		"\n"
//		"Move around\n"
//		"Zoom in\n"
//		"Zoom out\n"
//		"Reset view\n"
//		, 190, 30, pSurface);
//	//DrawPlotGrid(pSurface,settings, 999,999);
//
//	SDL_Event event;
//	while (TRUE)
//	{
//	if ( SDL_PollEvent ( &event ) )
//	{
//	//an event was found
//	if ( event.type == SDL_QUIT ) return 0;
//	else if (event.type==SDL_MOUSEBUTTONDOWN) return 0;
//	else if (event.type==SDL_KEYUP)
//	  {
//		switch(event.key.keysym.sym)
//		{
//			case SDLK_SPACE: 
//			case SDLK_RALT:
//			case SDLK_LALT:
//			case SDLK_RSHIFT: 
//			case SDLK_LSHIFT:
//			case SDLK_RCTRL:
//			case SDLK_LCTRL:
//				break;
//			default: 
//				return 0; //return back to the other screen!
//				break;
//		}
//	}
//	}
//	SDL_UpdateRect ( pSurface , 0 , 0 , 0 , 0 ) ;
//	}
//	return 0;
//}
