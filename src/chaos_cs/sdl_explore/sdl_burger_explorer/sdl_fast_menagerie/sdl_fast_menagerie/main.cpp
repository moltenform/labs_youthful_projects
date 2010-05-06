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
BOOL tryZoomCustom(int mouse_x, int mouse_y, MenagFastSettings*settings);
void testThisDimension(SDL_Surface* pSurface, MenagFastSettings*settings,double a,double b);

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

				if ((buttons &SDL_BUTTON_LMASK) && (mod & KMOD_ALT)) //middle-click-custom zoom rect!
				{
					double fmousex,fmousey;
			IntMenagCoordsToDouble(settings, mouse_x, mouse_y, &fmousex, &fmousey);

					if (!g_BusyThread1 && !g_BusyThread2) {
						if (!(mod & KMOD_SHIFT)) {settings->browsex0 = fmousex; settings->browsey1 = fmousey;  }
						else {settings->browsex1 = fmousex; settings->browsey0 = fmousey;

							SDL_FillRect ( pMenagSurface , &busyIndication , 72);
							startMenagCalculation(settings, 0, pSurface->format);
							waitingForCompletion = TRUE;
							shouldRedraw = TRUE;
							startTimer(); 
						}
					} 
				}
				//control click = zoom in, shift click=zoom out
				else if ((buttons & SDL_BUTTON_LMASK) && ((mod & KMOD_CTRL) || (mod & KMOD_SHIFT ) ) )
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
				snprintf(timetext, sizeof(timetext), "t:%d   [%f,%f,%f,%f]", time,settings->browsex0,settings->browsex1,settings->browsey0,settings->browsey1);
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
		case SDLK_q: if (bControl) testThisDimension(pSurface,settings,*outA,*outB); break;
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
BOOL tryZoomCustom(int mouse_x, int mouse_y, MenagFastSettings*settings)
{
	double fmousex, fmousey; BOOL ret = FALSE;
	if (mouse_x>0&& mouse_x<MenagWidth && mouse_y>0 && mouse_y<MenagHeight)
	{
		IntMenagCoordsToDouble(settings, mouse_x, mouse_y, &fmousex, &fmousey);
		static volatile BOOL bFirst = TRUE; static volatile double savedx0=0, savedy0=0;
		if (bFirst)
		{
			savedx0 = fmousex;
			savedy0 = fmousey;
		}
		else 
		{
			if (fmousex>savedx0 && fmousey>savedy0) {
			settings->browsex0 = savedx0;
			settings->browsex1 = fmousex;
			settings->browsey0 = savedy0;
			settings->browsey1 = fmousey;
			ret = TRUE;
			}
		}
		bFirst = !bFirst;
	}
	return ret;
}



void showVals(SDL_Surface* pSurface, double a,double b)
{
char buf[256];
snprintf(buf, sizeof(buf),"a:%f b:%f", a,b);
Dialog_Message(buf, pSurface);
}


extern int* arrThread1;
extern int* arrThread2;
int alternateCountPhasePlotSSEGetDimensionSMALLER(MenagFastSettings*settings,double c1, double c2, int whichThread);
int alternateCountPhasePlotSSEGetDimensionBIGGER(MenagFastSettings*settings,double c1, double c2, int whichThread);
void testThisDimension(SDL_Surface* pSurface,MenagFastSettings*settings, double a,double b)
{
	arrThread1 = arrThread2 = (int*)malloc(sizeof(int)*1024 * 1024); //single threaded so the same.
	char buf[256];
a = -.69471; b=1.770903;
int smaller = alternateCountPhasePlotSSEGetDimensionSMALLER(settings,a,b,0);
int bigger = alternateCountPhasePlotSSEGetDimensionBIGGER(settings,a,b,0);
snprintf(buf, sizeof(buf),"a:%f b:%f \nsmall:%d big:%d rat:%f", a,b, smaller,bigger, bigger/(double)smaller);
Dialog_Message(buf, pSurface);

a = -.531865; b=1.770355;
 smaller = alternateCountPhasePlotSSEGetDimensionSMALLER(settings,a,b,0);
 bigger = alternateCountPhasePlotSSEGetDimensionBIGGER(settings,a,b,0);
snprintf(buf, sizeof(buf),"a:%f b:%f \nsmall:%d big:%d rat:%f", a,b, smaller,bigger, bigger/(double)smaller);
Dialog_Message(buf, pSurface);
a = -.104865; b=1.764326;
 smaller = alternateCountPhasePlotSSEGetDimensionSMALLER(settings,a,b,0);
 bigger = alternateCountPhasePlotSSEGetDimensionBIGGER(settings,a,b,0);
snprintf(buf, sizeof(buf),"a:%f b:%f \nsmall:%d big:%d rat:%f", a,b, smaller,bigger, bigger/(double)smaller);
Dialog_Message(buf, pSurface);
}

