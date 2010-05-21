
#include "common.h"
typedef struct
{
	double * px0;
	double * px1;
	double * py0;
	double * py1;
	int screen_x;
	int screen_y;
	int screen_width;
	int screen_height;
	
	double undox0, undox1, undoy0, undoy1;
} CoordsDiagramStruct;

int onClickTryZoom(CoordsDiagramStruct diagrams[],int direction, int x, int y);
int isClickWithinDiagram(CoordsDiagramStruct diagrams[], int x, int y);
void doubleToScreenPixels(CoordsDiagramStruct * diagram, double fx, double fy, int *outX, int *outY);
void screenPixelsToDouble(CoordsDiagramStruct * diagram, int mousex, int mousey, double *outX, double *outY);
void setZoom(CoordsDiagramStruct * diagram, double x0,double x1,double y0,double y1);
void undoZoom(CoordsDiagramStruct * diagram);
void DrawPlotGrid( SDL_Surface* pSurface, CoordsDiagramStruct * diagram, double c1, double c2 ) ;
 
