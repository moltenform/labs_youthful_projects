#pragma warning (disable:4996)

#include "phaseportrait.h"



void saveData(PhasePortraitSettings * settings, char * filename)
{
	FILE * f = fopen(filename, "w");
	fprintf(f,"version,1,w,%d,h,%d,x0,%f,x1,%f,y0,%f,y1,%f,",
		settings->width,settings->height,
		settings->x0, settings->x1, settings->y0, settings->y1);
	fprintf(f,"browsex0,%f,browsex1,%f,browsey0,%f,browsey1,%f," ,
		settings->browsex0, settings->browsex1, settings->browsey0, settings->browsey1);
	fprintf(f,"seeds,%d,settle,%d,drawing,%d" ,
		settings->seedsPerAxis, settings->settling, settings->drawing);
	
	fclose(f);
}

void loadData(PhasePortraitSettings * settings, char * filename)
{
	FILE * f = fopen(filename, "r");
	int version;
	fscanf(f,"version,%d,w,%d,h,%d,x0,%lf,x1,%lf,y0,%lf,y1,%lf,",
		&version,
		&settings->width,&settings->height,
		&settings->x0, &settings->x1, &settings->y0, &settings->y1);
	fscanf(f,"browsex0,%lf,browsex1,%lf,browsey0,%lf,browsey1,%lf," ,
		&settings->browsex0, &settings->browsex1, &settings->browsey0, &settings->browsey1);
	fscanf(f,"seeds,%d,settle,%d,drawing,%d" ,
		&settings->seedsPerAxis, &settings->settling, &settings->drawing);
	
	fclose(f);
}

