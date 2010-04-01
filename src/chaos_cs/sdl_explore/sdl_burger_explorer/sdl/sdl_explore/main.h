
#pragma once


extern Uint32 g_white; //called an "extern" so that it's declared many times and defined once.
extern int PlotHeight, PlotWidth, PlotX ;


enum {
  SCREENWIDTH = 800,
  SCREENHEIGHT = 600,
  SCREENBPP = 0,
  SCREENFLAGS = SDL_ANYFORMAT// | SDL_FULLSCREEN
} ; 

void controller_sets_pos(double *outA, double *outB);
void controller_gets_pos(double a, double b);


int roundDouble(double a);


