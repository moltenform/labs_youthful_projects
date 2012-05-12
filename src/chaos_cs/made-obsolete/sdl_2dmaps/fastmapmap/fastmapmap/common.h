#pragma once

#ifdef _MSC_VER
//#include "SDL.h"
#include "C:\projects\c++\sdl\include\SDL.h"
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
	double x0;
	double x1;
	double y0;
	double y1;
	double diagramx0;
	double diagramx1;
	double diagramy0;
	double diagramy1;
	
	int settlingTime;
	int drawingTime;
	int basinsTime;

	int seedsPerAxis;
	double seedx0;
	double seedx1;
	double seedy0;
	double seedy1;

	int drawingOptions; //a bit field. contains 8 settings.
	int colorsStep;
	double basinsMaxColor;
	double colorDiskRadius;
	double breatheRadius;
} FastMapMapSettings;


typedef struct
{
	const char * fieldType;
	const char * fieldName;
	void * reference;
	int defaultValue; //a default value, for construction. 
} SettingsFieldDescription;

extern FastMapMapSettings * g_settings;

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

