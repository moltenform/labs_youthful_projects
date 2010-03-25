
#pragma once


extern Uint32 g_white; //called an "extern" so that it's declared many times and defined once.
enum {
  SCREENWIDTH = 640,
  SCREENHEIGHT = 480,
  SCREENBPP = 0,
  SCREENFLAGS = SDL_ANYFORMAT// | SDL_FULLSCREEN
} ; 

void controller_sets_pos(double *outA, double *outB);
void controller_gets_pos(double a, double b);


int roundDouble(double a);


