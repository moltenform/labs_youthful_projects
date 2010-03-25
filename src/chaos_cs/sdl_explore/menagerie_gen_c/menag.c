
 //less /proc/cpuinfo
 //runs debian. sudo apt-get install libc6-dev
 // menag.exe out.dat d 64 64 -3.5 1.5 1.5 2
 //~ menag.exe out\out.dat d 128 128 -3.4 1.1 1.3 2.05
 /*
 to run in background:
 >screen
 (ok when message pops up)
 > ./menag.out theout.dat d 128 128 -3.4 1.1 1.3 2.05
 (press Ctrl+a, d)
 says Screen detached. now can log out.
 to reconnect, type
 >screen -r
 Sweet.
 */

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h> //isfinite

#ifdef _MSC_VER //using Msvc
#include <float.h>
#define ISFINITE(x) (_finite(x))
#else
#define ISFINITE(x) (isfinite(x))
#endif

/*
output format:
int width,
int height,
double x0, double x1, double y0, double y1,
int gridpixels, int seeds, int settling, int drawing
*/
#include "menag.h"

int drawPhasePortrait(FigureSettings* settings, double a, double b, int arrAns[])
{
	double sx,sy; int i,ii;//loop vars
	int totalUniquePoints = 0;
	int arrWidth = settings->phaseFigureWidth;
	int arrHeight = settings->phaseFigureHeight;
	//clear array
	for (i=0; i<arrWidth*arrHeight;i++) arrAns[i]=0;
	double X0=settings->phasex0, X1=settings->phasex1, Y0=settings->phasey0, Y1=settings->phasey1;
	double x, y, x_;
	
	int nXpoints = settings->seedsPerAxis; int nYpoints = settings->seedsPerAxis;
	double sx0= -2, sx1=2, sy0= -2, sy1=2;
	double sxinc = (nXpoints==1) ? 1e6 : (sx1-sx0)/(nXpoints-1);
	double syinc = (nYpoints==1) ? 1e6 : (sy1-sy0)/(nYpoints-1);
	
	for (sx=sx0; sx<=sx1; sx+=sxinc) {
	for (sy=sy0; sy<=sy1; sy+=syinc)
	{
			x = sx; y=sy;

			for (ii=0; ii<(settings->settling); ii++)
			{
				x_ = a*x - y*y;
				y = b*y + x*y;
				x = x_;
				if (!(ISFINITE(x)&&ISFINITE(y))) break;
			}
			for (ii=0; ii<(settings->drawing); ii++)
			{
				x_ = a*x - y*y;
				y = b*y + x*y;
				x = x_;
				if (!(ISFINITE(x)&&ISFINITE(y))) break;

				int px = (int)(arrWidth * ((x - X0) / (X1 - X0)));
				int py = (int)(arrHeight - arrHeight * ((y - Y0) / (Y1 - Y0)));
				if (py >= 0 && py < arrHeight && px>=0 && px<arrWidth)
				    if (arrAns[py + px * arrHeight]==0)
				    { arrAns[py + px * arrHeight]=1; totalUniquePoints++;}
			}
	}
	}
	return totalUniquePoints;
}

void createMenagerie(char*filename, FigureSettings* settings)
{
	FILE*fout=fopen(filename,"wb");
	if (fout==NULL) { puts("Error: could not open file"); return; }
	fwrite(settings, sizeof(FigureSettings), 1, fout);
	
	//create phase portrait array
	int arrWidth = settings->phaseFigureWidth; int arrHeight = settings->phaseFigureHeight;
	int * arrAns = malloc(sizeof(int)*arrWidth*arrHeight);
	
	double fx, fy; int px, py; int total;
	double X0=settings->x0, X1=settings->x1, Y0=settings->y0, Y1=settings->y1;
	double dx = (X1 - X0) / (settings->width), dy = (Y1 - Y0) / (settings->height);
	fx = X0; fy = Y0; //fy counts upwards
	for (py=0; py<(settings->height); py++)
	{
	fx = X0;
	for (px = 0; px < (settings->width); px++)
	{
		if (fx<-1.65 && fy>1.63)
			total = 0;//1700; //hack:shortcut
		else
			total = drawPhasePortrait(settings, fx, fy, arrAns);
		fwrite(&total, sizeof(int), 1, fout);
		//~ fprintf(fout, "%d\n", total);
	    fx += dx;
	}
	fy += dy;
	printf("percent %f\n",py/((double)settings->height));
	}
	
	free(arrAns);
	fclose(fout);
}


int main(int argc, char *argv[])
{
	char * filename;
	if (argc!=9)
	{
		puts("Give args:file.dat\nd(draw) or r(read)\nwidth\nheight\nx0\nx1\ny0\ny1");
		return 1;
	}
	filename = argv[1];
	if (strcmp(argv[2], "d")==0)
	{
	FigureSettings * inputSettings = malloc(sizeof(FigureSettings));
	inputSettings->width = atoi(argv[3]);
	inputSettings->height = atoi(argv[4]);
	inputSettings->x0 = atof(argv[5]);
	inputSettings->x1 = atof(argv[6]);
	inputSettings->y0 = atof(argv[7]);
	inputSettings->y1 = atof(argv[8]);
	
	//default values
	inputSettings->seedsPerAxis=20;
	inputSettings-> settling = 400;
	inputSettings-> drawing = 16;
	inputSettings-> phaseFigureWidth=100; inputSettings-> phaseFigureHeight=100;
	inputSettings->phasex0 = -3.75; inputSettings->phasex1 = 0.75; 
	inputSettings->phasey0 = -2.1; inputSettings->phasey1 = 2.1;
	 //since always symmetrical, use phasey0 as -.1 to 2.1? to 4.1? just have better resolution ?
	createMenagerie(filename, inputSettings);
	free(inputSettings);
	}
	else
	{
		puts("unknown operation. Use 'd'.");
	}
	
	return 0;
}


