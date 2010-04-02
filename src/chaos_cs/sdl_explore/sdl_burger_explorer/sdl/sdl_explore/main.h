
#pragma once




void controller_sets_pos(double *outA, double *outB);
void controller_gets_pos(double a, double b);


int roundDouble(double a);


void IntPlotCoordsToDouble(PhasePortraitSettings*settings, int mouse_x, int mouse_y, double*outX, double *outY);
void DoubleCoordsToInt(PhasePortraitSettings*settings, double fx, double fy, int* outX, int* outY);
