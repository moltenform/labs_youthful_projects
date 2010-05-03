#pragma once

#if WIN32
#include "SDL.h"
#else
#include "SDL/SDL.h"
#endif
#if WIN32
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
#if FULLSCREEN
  SCREENFLAGS = SDL_ANYFORMAT | SDL_FULLSCREEN,
#else
  SCREENFLAGS = SDL_ANYFORMAT,
#endif

  FPS = 20 //limit frames per second
} ;
typedef struct
{
	/*double x0;
	double x1;
	double y0;
	double y1;*/

	double browsex0;
	double browsex1;
	double browsey0;
	double browsey1;
	
	double seedx0;
	double seedx1;
	double seedy0;
	double seedy1;
	int menagSeedsPerAxis;
	int menagSettling;
	int menagDrawing;
} MenagFastSettings;

typedef struct
{
	int width;
	int height;
	double x0;
	double x1;
	double y0;
	double y1;

	double browsex0;
	double browsex1;
	double browsey0;
	double browsey1;
	
	int seedsPerAxis;
	int settling;
	int drawing;
	int drawBasin;
} PhasePortraitSettings;


//called an "extern" so that it's declared many times and defined once.
extern Uint32 g_white; 
extern int MenagHeight, MenagWidth;
extern int PhasePlotHeight, PhasePlotWidth, PhasePlotX;
 

void IntMenagCoordsToDouble(MenagFastSettings*settings, int mouse_x, int mouse_y, double*outX, double *outY);
void DoubleMenagCoordsToInt(MenagFastSettings*settings, double fx, double fy, int* outX, int* outY);
BOOL doesFileExist(const char *fname);
BOOL LockFramesPerSecond();

