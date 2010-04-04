#pragma once

#include "sdl.h"

enum {
  SCREENWIDTH = 800,
  SCREENHEIGHT = 600,
  SCREENBPP = 0,
  SCREENFLAGS = SDL_ANYFORMAT,// | SDL_FULLSCREEN

  FPS = 20 //limit frames per second
} ;

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
extern int PlotHeight, PlotWidth, PlotX;
 

void IntPlotCoordsToDouble(PhasePortraitSettings*settings, int mouse_x, int mouse_y, double*outX, double *outY);
void DoubleCoordsToInt(PhasePortraitSettings*settings, double fx, double fy, int* outX, int* outY);
bool LockFramesPerSecond();
