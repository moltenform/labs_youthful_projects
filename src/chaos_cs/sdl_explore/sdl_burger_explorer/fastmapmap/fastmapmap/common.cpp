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

{"int","colorsStep",(void *)&g_settings->colorsStep, 1},
{"double","basinsMaxColor",(void *)&g_settings->basinsMaxColor, 4.0},
{NULL, NULL, NULL, 0} //must end with null.
};


void plotlinerect(SDL_Surface* pSurface, int px0, int px1, int py0, int py1, int newcol)
{
	plotlinevertcolor(pSurface, px0, py0, py1,newcol);
	plotlinevertcolor(pSurface, px1, py0, py1,newcol);
	plotlinehorizcolor(pSurface, px0, px1, py0,newcol);
	plotlinehorizcolor(pSurface, px0, px1, py1,newcol);
}
void plotlinehorizcolor(SDL_Surface* pSurface, int px0, int px1, int py, int newcol)
{
	for (int x=px0; x<px1; x++)
		plotpointcolor(pSurface, x, py, newcol);
}
void plotlinevertcolor(SDL_Surface* pSurface, int px, int py0, int py1, int newcol)
{
	for (int y=py0; y<py1; y++)
		plotpointcolor(pSurface, px, y, newcol);
}
void plotpointcolor(SDL_Surface* pSurface, int px, int py, int newcol)
{
	if (!(px >= 0 && px < pSurface->w && py >= 0 && py < pSurface->h ))
			return;
	
  char* pPosition = ( char* ) pSurface->pixels ; //determine position
  pPosition += ( pSurface->pitch * py ); //offset by y
  pPosition += ( pSurface->format->BytesPerPixel * px ); //offset by x
  if (newcol!=1)
  memcpy ( pPosition , &newcol , pSurface->format->BytesPerPixel ) ;
  else
  {
	//semi transparent...
	  int col;
	  memcpy ( &col , pPosition , pSurface->format->BytesPerPixel ) ; //copy pixel data
		SDL_Color color ;
		SDL_GetRGB ( col , pSurface->format , &color.r , &color.g , &color.b ) ;
		color.r = color.r + (255-color.r)/16;
		color.g = color.g + (0-color.g)/16;
		color.b = color.b + (0-color.b)/16;
		Uint32 outcol = SDL_MapRGB ( pSurface->format , color.r , color.g , color.b ) ;
		 memcpy ( pPosition , &outcol , pSurface->format->BytesPerPixel ) ;
  }
}


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