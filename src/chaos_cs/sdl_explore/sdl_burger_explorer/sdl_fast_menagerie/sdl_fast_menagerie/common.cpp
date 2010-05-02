#include "common.h"

void IntMenagCoordsToDouble(MenagFastSettings*settings, int mouse_x, int mouse_y, double*outX, double *outY)
{
	*outX = (mouse_x-0)/((double)MenagWidth)*(settings->browsex1-settings->browsex0) + settings->browsex0;
	*outY = ((MenagHeight-mouse_y)-0)/((double)MenagHeight)*(settings->browsey1-settings->browsey0) + settings->browsey0;
}

void DoubleMenagCoordsToInt(MenagFastSettings*settings, double fx, double fy, int* outX, int* outY)
{
	*outX = (int)(MenagWidth * (fx- settings->browsex0) / (settings->browsex1 - settings->browsex0) + 0);
	*outY = MenagHeight - (int)(MenagHeight * (fy- settings->browsey0) / (settings->browsey1 - settings->browsey0) + 0);
}
/*void IntPhaseCoordsToDouble(PhasePortraitSettings*settings, int mouse_x, int mouse_y, double*outX, double *outY)
{
	*outX = (mouse_x-0)/((double)PhaseWidth)*(settings->x1-settings->x0) + settings->x0;
	*outY = ((PhaseHeight-mouse_y)-0)/((double)PhaseHeight)*(settings->y1-settings->y0) + settings->y0;
}*/

BOOL doesFileExist(const char *fname)
{
	FILE * ftmp = fopen(fname, "rb");
	if (!ftmp) return FALSE;
	fclose(ftmp);
	return TRUE;
}


BOOL LockFramesPerSecond() //run no faster than x fps
{
	int framerate=FPS;
	static float lastTime = 0.0f;
	float currentTime = SDL_GetTicks() * 0.001f;
	if((currentTime - lastTime) > (1.0f / framerate))
	{
		lastTime = currentTime;
		return TRUE;
	}
	return FALSE;
}

