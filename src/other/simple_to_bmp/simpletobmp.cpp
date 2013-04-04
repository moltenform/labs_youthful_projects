#include <iostream>
#include <fstream>

using namespace std;

#include <stdio.h>
#include "bmp_io.h"

void error(const char* s) {fputs(s, stderr); }
int main(int argc, char *argv[])
{
	if (argc == 4 && strcmp(argv[1], "i")==0)
	{
		return 0;
	}
	else if (argc == 4 && strcmp(argv[1], "o")==0)
	{
		FILE * f;
		f = fopen(argv[2], "rb");
		if (!f) { error("Could not open file."); return 1;}
		
		char h;
		h = fgetc(f);
		if (h!='S') { error("Does not appear to be a simpleimage file."); return 1;}
		char h1 = fgetc(f);
		char h2 = fgetc(f);
		if (!(h1=='2' && h2=='4')) { error("Unsupported image format, currently only 24bit images supported."); return 1;}
		
		int width, height;
		fread(&width, sizeof(int),1,f);
		fread(&height, sizeof(int),1,f);
		
		printf("width:%d, height:%d", width, height);
		
		unsigned char * r, *g, *b;
		r=new unsigned char[width*height];
		g=new unsigned char[width*height];
		b=new unsigned char[width*height];
		for (int i=0; i<width*height; i++) {r[i]=0; g[i]=0; b[i]=0; }
		
		int index=0;
		for (int y=0; y<height; y++)
		{
			for (int x=0; x<width; x++)
			{
				index = y*width + x;
				r[index]=fgetc(f);
				g[index]=fgetc(f);
				b[index]=fgetc(f);
			}
		}
		fclose(f);
		
		//~ FILE * fout;
		//~ fout = fopen(argv[3],"wb");
		bmp_24_write(argv[3], width, height, r,g,b);
		delete[] r; delete[] g; delete[] b;
		return 0;
	}
	
	if (argc == 2 && strcmp(argv[1], "maketest")==0)
	{
		// an example of how to make a simple image.
		FILE * fout;
		fout = fopen("test.simple","wb");
		if (!fout) return 1;
		
		fputc('S', fout);
		fputc('2', fout);
		fputc('4', fout);
		
		int width = 512; int height = 256;
		fwrite(&width,sizeof(int), 1, fout); 
		fwrite(&height,sizeof(int), 1, fout); 
		
		for (int y=0; y<height; y++)
		{
			for (int x=0; x<width; x++)
			{
				// R, G, B
				fputc( y%256 , fout);
				fputc( x%256 , fout);
				fputc( 0 , fout);
			}
		}
		fclose(fout);
		printf("Wrote sample image to 'test.simple'.");
		return 0;
	}
	
	printf("simpletobmp, convert bytes to bmp.\n By Ben Fisher, LGPL, uses the LGPL bmp_io library by John Burkardt.");
	printf("\n\nUsage:");
	//~ printf("\n simpletobmp i input.bmp input.data");
	printf("\n simpletobmp o data.simple out.bmp");
	printf("\n simpletobmp maketest");
	return 0;
}


