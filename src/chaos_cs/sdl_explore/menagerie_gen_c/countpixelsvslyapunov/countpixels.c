
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h> 

#include "countpixels.h"


#define LOGISTIC (r*p*(1-p))
#define LOGISTIC_D  (r*(1-2*p))
#define CUBIC (r*p*(1-p*p))
#define CUBIC_D  (r*(1-3*p*p))
#define TENT ((p>0.5)?(r*(1-p)):(r*p))
#define TENT_D  ((p>0.5)?-r:r)
#define SECANT (1/cos(p+r))
#define SECANT_D  (tan(p+r)/cos(p+r))
#define EXPRESSION SECANT
#define EXPRESSION_D SECANT_D


int draw1dphaseportrait(int*arr, double r, CountPixelSettings* settings)
{
	int length = settings->height;
	//clear array:
	int i; for (i=0; i<length;i++) arr[i]=0;
	int total = 0;
	double p = settings->p0; //rand()
	for (i=0; i<settings->settling; i++)
		p = EXPRESSION;
	for (i=0; i<settings->drawing; i++)
	{
		p = EXPRESSION;
		int iy = (int)(length * ((p - settings->y0) / (settings->y1 - settings->y0)));
		if (iy >= 0 && iy < length)
		{
			if (arr[iy]==0) total++;
			arr[iy] = 1;
		}
	}
	return total;
}
double drawlyap(double r, CountPixelSettings* settings)
{
	int i;
	double total = 0;
	double p = settings->p0; //rand()
	for (i=0; i<settings->settlingLya; i++)
		p = EXPRESSION;
	
	for (i=0; i<settings->drawingLya; i++)
	{
		p = EXPRESSION;
		total+=log(fabs(EXPRESSION_D)); //note: fabs not abs!
	}
	return exp((total)/settings->drawingLya);
}

void GoPhase(CountPixelSettings* settings)
{
	int i;
	int length = settings->height;
	int*arr = malloc(sizeof(int)*length);
	FILE*fout = fopen("phaseout.txt","w");
	FILE*fout2 = fopen("phaseoutlya.txt","w");
	double r=settings->x0;
	double dr = (settings->x1-settings->x0)/settings->width;
	for (i=0; i<settings->width; i++)
	{
		int total = draw1dphaseportrait(arr, r, settings);
		fprintf(fout, "%d\n", total);
		fprintf(fout2, "%f\n", drawlyap(r,settings));
		r+=dr;
	}
	
	free(arr);
	fclose(fout);
	fclose(fout2);
}

int main(int argc, char *argv[])
{
	CountPixelSettings * settings = malloc(sizeof(CountPixelSettings));
	settings->width = 6400;
	settings->height = 1400; //800;
	
	  //Logistic map
	/*settings->x0 = 2.1;
	settings->x1 = 4.0;
	settings->y0 = 0;
	settings->y1 = 1;*/
	/*//Cubic map
	settings->x0 = 1.75;
	settings->x1 = 3.0;
	settings->y0 = 0; //-1.17; just to see
	settings->y1 = 1.17;*/
	
	//Tent map 
	/*settings->x0 = 0.95;
	settings->x1 = 2;
	settings->y0 = 0; //unknown the best for this...
	settings->y1 = 1; */
	
	//secant map
	settings->x0 = 0.0;// -- note: use rand() instead of p0!
	settings->x1 = 2*3.1416;
	settings->y0 = -5; //unknown the best for this...
	settings->y1 = 5;
	
	settings->p0 = 0.45;
	settings->settling = 100;
	//settings->drawing = 10000;//2000; important!!! before, wasn't drawing enough!
	settings->drawing = 10000;// //90000 for cubic the 
	settings->settlingLya = 100; //20 / 80
	settings->drawingLya = 2000; 
	
	GoPhase(settings);
	
	free(settings);
	return 0;
}


