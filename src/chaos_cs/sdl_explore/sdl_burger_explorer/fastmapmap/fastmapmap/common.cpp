#include "common.h"
#include "configfiles.h"


FastMapMapSettings g_GlobalSettings; // the one important settings object.
FastMapMapSettings * g_settings = &g_GlobalSettings;


//regex replace \W*(\w+)\W+(\w+);
//with {"\1","\2",(void *)&g_settings->\2},

SettingsFieldDescription GlobalFieldDescriptions[] = 
{
{"int","drawingMode",(void *)&g_settings->drawingMode, 1},
{"double","a",(void *)&g_settings->a, 0},
{"double","b",(void *)&g_settings->b, 0},
{"double","x0",(void *)&g_settings->x0, -1},
{"double","x1",(void *)&g_settings->x1, 1},
{"double","y0",(void *)&g_settings->y0, -1},
{"double","y1",(void *)&g_settings->y1, 1},
{"double","diagramx0",(void *)&g_settings->diagramx0, -1},
{"double","diagramx1",(void *)&g_settings->diagramx1, 1},
{"double","diagramy0",(void *)&g_settings->diagramy0, -1},
{"double","diagramy1",(void *)&g_settings->diagramy1, 1},
	
{"int","settlingTime",(void *)&g_settings->settlingTime, 48},
{"int","drawingTime",(void *)&g_settings->drawingTime, 20},
{"int","basinsTime",(void *)&g_settings->basinsTime, 20},
{"int","lyapunovTime",(void *)&g_settings->lyapunovTime, 60},

{"int","seedsPerAxis",(void *)&g_settings->seedsPerAxis, 40},
{"double","seedx0",(void *)&g_settings->seedx0, -2},
{"double","seedx1",(void *)&g_settings->seedx1, 2},
{"double","seedy0",(void *)&g_settings->seedy0, -2},
{"double","seedy1",(void *)&g_settings->seedy1, 2},

{"int","colorsStep",(void *)&g_settings->colorsStep, 0},
{NULL, NULL, NULL, 0} //important to end with null.
};


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