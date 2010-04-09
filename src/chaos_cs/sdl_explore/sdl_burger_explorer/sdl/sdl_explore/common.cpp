#include "common.h"

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
void IntPhaseCoordsToDouble(PhasePortraitSettings*settings, int mouse_x, int mouse_y, double*outX, double *outY)
{
	*outX = (mouse_x-0)/((double)PhaseWidth)*(settings->x1-settings->x0) + settings->x0;
	*outY = ((PhaseHeight-mouse_y)-0)/((double)PhaseHeight)*(settings->y1-settings->y0) + settings->y0;
}


/*
some sdl references:
//http://www.linuxjournal.com/article/4401?page=0,1
//http://www.gamedev.net/reference/programming/features/sdl2/page5.asp
//http://gpwiki.org/index.php/SDL:Tutorials:Keyboard_Input_using_an_Event_Loop
//http://www.gameprogrammer.com/fastevents/fastevents1.html
*/

bool LockFramesPerSecond() //run no faster than x fps
{
	int framerate=FPS;
	static float lastTime = 0.0f;
	float currentTime = SDL_GetTicks() * 0.001f;
	if((currentTime - lastTime) > (1.0f / framerate))
	{
		lastTime = currentTime;
		return true;
	}
	return false;
}

