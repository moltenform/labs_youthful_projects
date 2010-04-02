#pragma once

//seen by both main.cpp and phaseportrait.cpp

extern Uint32 g_white; //called an "extern" so that it's declared many times and defined once.
extern int PlotHeight, PlotWidth, PlotX ;


enum {
  SCREENWIDTH = 800,
  SCREENHEIGHT = 600,
  SCREENBPP = 0,
  SCREENFLAGS = SDL_ANYFORMAT// | SDL_FULLSCREEN
} ; 

void IntPlotCoordsToDouble(PhasePortraitSettings*settings, int mouse_x, int mouse_y, double*outX, double *outY);
void DoubleCoordsToInt(PhasePortraitSettings*settings, double fx, double fy, int* outX, int* outY);
