#include "common.h"
#include "configfiles.h"

FastMapsSettings g_GlobalSettings; // the one global settings object.
FastMapsSettings * g_settings = &g_GlobalSettings;

//generate this from the declaration with regex replace
//\W*(\w+)\W+(\w+);
//to {"\1","\2",(void *)&g_settings->\2, 555},



SettingsFieldDescription GlobalFieldDescriptions[] = 
{
{"double","pc1",(void *)&g_settings->pc1, 0},
{"double","pc2",(void *)&g_settings->pc2, 0},
{"double","pc3",(void *)&g_settings->pc3, 0},
{"double","pc4",(void *)&g_settings->pc4, 0},
{"double","pc5",(void *)&g_settings->pc5, 0},
{"double","pc6",(void *)&g_settings->pc6, 0},
{"double","pc1b",(void *)&g_settings->pc1b, 0},
{"double","pc2b",(void *)&g_settings->pc2b, 0},
{"double","pc3b",(void *)&g_settings->pc3b, 0},
{"double","pc4b",(void *)&g_settings->pc4b, 0},
{"double","pc5b",(void *)&g_settings->pc5b, 0},
{"double","pc6b",(void *)&g_settings->pc6b, 0},
{"double","x0",(void *)&g_settings->x0, -2},
{"double","x1",(void *)&g_settings->x1, 2},
{"double","y0",(void *)&g_settings->y0, -2},
{"double","y1",(void *)&g_settings->y1, 2},
{"double","diagram_a_x0",(void *)&g_settings->diagram_a_x0, -2},
{"double","diagram_a_x1",(void *)&g_settings->diagram_a_x1, 2},
{"double","diagram_a_y0",(void *)&g_settings->diagram_a_y0, -2},
{"double","diagram_a_y1",(void *)&g_settings->diagram_a_y1, 2},
{"double","diagram_b_x0",(void *)&g_settings->diagram_b_x0, -2},
{"double","diagram_b_x1",(void *)&g_settings->diagram_b_x1, 2},
{"double","diagram_b_y0",(void *)&g_settings->diagram_b_y0, -2},
{"double","diagram_b_y1",(void *)&g_settings->diagram_b_y1, 2},
{"double","diagram_c_x0",(void *)&g_settings->diagram_c_x0, -2},
{"double","diagram_c_x1",(void *)&g_settings->diagram_c_x1, 2},
{"double","diagram_c_y0",(void *)&g_settings->diagram_c_y0, -2},
{"double","diagram_c_y1",(void *)&g_settings->diagram_c_y1, 2},

{"int","colorMode",(void *)&g_settings->colorMode, 0},
{"int","colorWrapping",(void *)&g_settings->colorWrapping, 0},
{"double","maxValueAddition",(void *)&g_settings->maxValueAddition, 0},
{"double","hueShift",(void *)&g_settings->hueShift, 0},
{"int","settling",(void *)&g_settings->settling, 10},
{"int","drawing",(void *)&g_settings->drawing, 10},
{"double","breatheRadius_c1c2",(void *)&g_settings->breatheRadius_c1c2, 1},
{NULL, NULL, NULL, 0} //must end with null.
};


void plotlineRectangle(SDL_Surface* pSurface, int px0, int px1, int py0, int py1, int newcol)
{
	plotlineVert(pSurface, px0, py0, py1,newcol);
	plotlineVert(pSurface, px1, py0, py1,newcol);
	plotlineHoriz(pSurface, px0, px1, py0,newcol);
	plotlineHoriz(pSurface, px0, px1, py1,newcol);
}
void plotlineHoriz(SDL_Surface* pSurface, int px0, int px1, int py, int newcol)
{
	for (int x=px0; x<px1; x++)
		plotpointcolor(pSurface, x, py, newcol);
}
void plotlineVert(SDL_Surface* pSurface, int px, int py0, int py1, int newcol)
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
	if (newcol!=TRANSLUCENT_RED)
		memcpy ( pPosition , &newcol , pSurface->format->BytesPerPixel ) ;
	else
	{
		//translucently shades the pixel red.
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

SDL_Surface* createSurface(SDL_Surface*reference, int width, int height)
{
	return SDL_CreateRGBSurface( SDL_SWSURFACE, width,height, reference->format->BitsPerPixel, reference->format->Rmask, reference->format->Gmask, reference->format->Bmask, 0 );
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

void massert(BOOL condition, const char* message)
{
	//I'd rather show message in the gui, but if font hasn't been loaded you can't see it.
	if (!condition) 
	{
		 fputs("Error. ", stderr);
		 fputs(message, stderr);
		 exit(1);
	}
}

