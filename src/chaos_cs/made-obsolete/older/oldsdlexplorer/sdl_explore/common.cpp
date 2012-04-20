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

BOOL doesFileExist(const char *fname)
{
	FILE * ftmp = fopen(fname, "rb");
	if (!ftmp) return FALSE;
	fclose(ftmp);
	return TRUE;
}

// You must free the result if result is non-NULL.
char *str_replace(char *orig, char *rep, char *with) {
    char *result; // the return string
    char *ins;    // the next insert point
    char *tmp;    // varies
    int len_rep;  // length of rep
    int len_with; // length of with
    int len_front; // distance between rep and end of last rep
    int count;    // number of replacements

    if (!orig)
        return NULL;
    if (!rep || !(len_rep = strlen(rep)))
        return NULL;
    if (!(ins = strstr(orig, rep))) 
        return NULL;
    if (!with)
        with = "";
    len_with = strlen(with);

    for (count = 0; tmp = strstr(ins, rep); ++count) {
        tmp += ins - tmp;
        ins = tmp;
    }

    // first time through the loop, all the variable are set correctly
    // from here on,
    //    tmp points to the end of the result string
    //    ins points to the next occurrence of rep in orig
    //    orig points to the remainder of orig after "end of rep"
    tmp = result = (char*) malloc(strlen(orig) + (len_with - len_rep) * count + 1);

    if (!result)
        return NULL;

    while (1) {
        len_front = ins - orig;
        tmp = strncpy(tmp, orig, len_front) + len_front;
        tmp = strcpy(tmp, with) + len_with;
        orig += len_front + len_rep; // move to next "end of rep"
        if (--count) break;
        ins = strstr(orig, rep);
    }
    strcpy(tmp, orig);
    return result;
}



/*
some sdl references:
//http://www.linuxjournal.com/article/4401?page=0,1
//http://www.gamedev.net/reference/programming/features/sdl2/page5.asp
//http://gpwiki.org/index.php/SDL:Tutorials:Keyboard_Input_using_an_Event_Loop
//http://www.gameprogrammer.com/fastevents/fastevents1.html
*/

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

