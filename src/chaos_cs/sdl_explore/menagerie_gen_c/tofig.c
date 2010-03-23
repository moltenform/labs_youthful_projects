
#include "menag.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>

int main(int argc, char *argv[])
{
	char * filein;
	char * fileout;
	if (argc!=3)
	{
		puts("Give args:\nin.dat\nout.bmp");
		return 1;
	}
	filein = argv[1];
	fileout = argv[2];
	
	FILE * fin = fopen(filein, "rb");
	if (!fin) {puts("error opening input file"); return 1;}
	FigureSettings * settings = malloc(sizeof(FigureSettings));
	fread( settings, sizeof(FigureSettings), 1, fin);
	int width = settings->width;
	int height = settings->height;
	printf("Converting file with this view:\n");
	printf("x0:%f\tx1:%f\ny0:%f\ty1:%f\n", settings->x0,settings->x1,settings->y0,settings->y1);
	
	
	FILE * fout = fopen(fileout, "wb");
	if (!fout) {puts("error opening output file"); return 1;}
	fputc('S', fout);
	fputc('2', fout);
	fputc('4', fout);
	fwrite(&width,sizeof(int), 1, fout); 
	fwrite(&height,sizeof(int), 1, fout); 
	
	int i, hits;
	int maxV = 0, oversat=0;
	for (i=0; i<width*height; i++)
	{
		fread( &hits, sizeof(int), 1, fin);
		
		int val = (int)(sqrt((double)hits)) * 6;
		if (val>maxV) {maxV=val;}
		if (val>255){oversat++; val=255;} if (val<0) val=0;
		char r,g,b; r=g=b= (char) val;
		
		//fprintf(fout, "%d %d (%d)\n", hits, val, r);
		fputc( r, fout);
		fputc(g , fout);
		fputc( b , fout);
	}
	fclose(fout);
	
	printf("Max val is '%d', oversat percent %f.\n",maxV, oversat/((double)width*height));
	printf("Wrote sample image to '%s'.",fileout);
	
	free(settings);
	return 0;
}


