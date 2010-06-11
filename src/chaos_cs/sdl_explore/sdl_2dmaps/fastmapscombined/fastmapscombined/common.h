#pragma once

#ifdef _MSC_VER
#include "SDL.h"
#else
#include "SDL/SDL.h"
#endif
#ifdef _MSC_VER
#define snprintf _snprintf
#endif

#define FULLSCREEN 0 
#define BOOL int
#define TRUE 1
#define FALSE 0
#define StringsEqual(s1, s2) (strcmp((s1),(s2))==0)

enum {
  SCREENWIDTH = 800,
  SCREENHEIGHT = 600,
  SCREENBPP = 0,
  SCREENFLAGS = SDL_ANYFORMAT,
  FPS = 20 //limit frames per second
} ;

typedef struct
{
	int drawingMode;
	double a;
	double b;
	double a2;
	double b2;
	double diagramx0;
	double diagramx1;
	double diagramy0;
	double diagramy1;
	
	double sx;
	double sy;
	double sx2;
	double sy2;
	double seeddiagramx0;
	double seeddiagramx1;
	double seeddiagramy0;
	double seeddiagramy1;

	double x0;
	double x1;
	double y0;
	double y1;

	int settlingTime;
	int drawingTime;
	int basinsTime;
	int numberSeedsX;
	int numberSeedsY;

	int drawingOptions; //a bit field. contains 8 settings.
	int colorsStep;
	double basinsMaxColor;
	double colorDiskRadius;
	double breatheRadius;
} FastMapsSettings;


typedef struct
{
	const char * fieldType;
	const char * fieldName;
	void * reference;
	int defaultValue; //a default value, for construction. 
} SettingsFieldDescription;

extern FastMapsSettings * g_settings;

#define g_white 0xffffffff

#define TRANSLUCENT_RED 0x00000001

BOOL doesFileExist(const char *fname);
BOOL LockFramesPerSecond();
void plotpointcolor(SDL_Surface* pSurface, int px, int py, int newcol);
void plotlineHoriz(SDL_Surface* pSurface, int px0, int px1, int py, int newcol);
void plotlineVert(SDL_Surface* pSurface, int px, int py0, int py1, int newcol);
void plotlineRectangle(SDL_Surface* pSurface, int px0, int px1, int py0, int py1, int newcol);
void massert(BOOL condition, const char* message);

#define MIN(X, Y)  ((X) < (Y) ? (X) : (Y))
#define MAX(X, Y)  ((X) > (Y) ? (X) : (Y))

