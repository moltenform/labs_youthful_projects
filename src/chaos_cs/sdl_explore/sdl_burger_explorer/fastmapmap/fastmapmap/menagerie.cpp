
#include "phaseportrait.h"
#include "menagerie.h"
#include "whichmap.h"
#include "coordsdiagram.h"
#include "float_cast.h"
#include "palette.h"
#include "font.h"
#include <math.h>

void BlitDiagram(SDL_Surface* pSurface,SDL_Surface* pSmallSurface, int px, int py)
{
	//SDL_FillRect ( pSmallSurface , NULL , 345232 );
	SDL_Rect dest;
	dest.x = px;
	dest.y = py;
	dest.w = pSmallSurface->w;
	dest.h = pSmallSurface->h; 
	SDL_BlitSurface(pSmallSurface, NULL, pSurface, &dest);
}

// use the blue/white/red scheme?
//keep going until find one that goes the whole ways without escaping.
//(better to iterate through in a way that jumps around.
int countPhasePlotLyapunov(SDL_Surface* pSurface,double c1, double c2)
{
	//http://sprott.physics.wisc.edu/chaos/lyapexp.htm
	double d0 = 1e-3, d1, total;
	double x, y, x_,y_; double x2, y2, x2_; double xtmp, ytmp;
	//int N = 400; int settle = 300; //used to be 20. 1000/600 causes moire patterns
	//int N = 900; int settle = 800; //used to be 20. 1000/600 causes moire patterns
	int N = 140; int settle = 80; //used to be 20. 1000/600 causes moire patterns
	double sx0= g_settings->seedx0, sx1=g_settings->seedx1, sy0= g_settings->seedy0, sy1=g_settings->seedy1;
	int nXpoints=g_settings->seedsPerAxis; int nYpoints=g_settings->seedsPerAxis;
	double sxinc = (nXpoints==1) ? 1e6 : (sx1-sx0)/(nXpoints-1);
	double syinc = (nYpoints==1) ? 1e6 : (sy1-sy0)/(nYpoints-1);
	for (double sx=sx0; sx<=sx1; sx+=sxinc)
    {
    for (double sy=sy0; sy<=sy1; sy+=syinc)
    {
		if (StringsEqual(MAPSUFFIX,BURGERSUF) && sx==sx0 && sy==sy0) {sx=0.0; sy=0.00001;}
	total = 0.0;
	x=sx; y=sy; 
	x2=sx; y2=sy+d0;
	for (int i=0; i<N; i++)
	{
		MAPEXPRESSION;
		x=x_; y=y_;
		xtmp=x; ytmp=y; x=x2; y=y2;
		
		MAPEXPRESSION;
		x=x_; y=y_;
		x2=x; y2=y; x=xtmp; y=ytmp;

		d1 = sqrt( (x2-x)*(x2-x) + (y2-y)*(y2-y) ); //distance
		x2=x+ (d0/d1)*(x2-x); //also looks interesting when these are commented out
		y2=y+ (d0/d1)*(y2-y);
		if (i>settle) total+= log(d1/d0 );
		if (ISTOOBIG(x) || ISTOOBIG(y)) //return SDL_MapRGB(pSurface->format, 255,255,255);//255<<8; //pure green xxrrggxx
			break;
	}
	if (!(ISTOOBIG(x) || ISTOOBIG(y))) goto FoundTotal;
	}
	}
	if (ISTOOBIG(x) || ISTOOBIG(y))
		return SDL_MapRGB(pSurface->format, 255,255,255);
FoundTotal:
	double val = total / (N-settle);
	if (val < 0) {  
		return 0;//
		/*val = sqrt(sqrt(-val));
		if (val>1.0) val=1.0;
		int v = (int)(val*255);
		return SDL_MapRGB(pSurface->format, 0,0,v);*/
	} 
	double estimatedmax= 1.4;
	val = sqrt(sqrt(val));
	if (val > estimatedmax) return SDL_MapRGB(pSurface->format, 255,0,0);
	val /= estimatedmax;
	//return HSL2RGB(pSurface, val, 1.0, 0.5);
	int v = 140+(int)(val*100);
	return SDL_MapRGB(pSurface->format, v,v,v);
	
}

//TODO: make thread-safe
#define PHASESIZE 128
int arr[PHASESIZE*PHASESIZE] = {0};
int whichID = 2;
int countPhasePlotPixels(SDL_Surface* pSurface,double c1, double c2, int whichThread)
{
	int total; double x,y,x_,y_;
	double sx0= g_settings->seedx0, sx1=g_settings->seedx1, sy0= g_settings->seedy0, sy1=g_settings->seedy1;
	int nXpoints=g_settings->seedsPerAxis; int nYpoints=g_settings->seedsPerAxis;
	double X0=g_settings->x0, X1=g_settings->x1, Y0=g_settings->y0, Y1=g_settings->y1;
	//use the default ones? this diagram should incorporate the whole thing?
	double sxinc = (nXpoints==1) ? 1e6 : (sx1-sx0)/(nXpoints-1);
	double syinc = (nYpoints==1) ? 1e6 : (sy1-sy0)/(nYpoints-1);
	int counted; 
	for (double sx=sx0; sx<=sx1; sx+=sxinc) //try until we get one that doesn't escape. use that one.
    {
    for (double sy=sy0; sy<=sy1; sy+=syinc)
    {
		if (StringsEqual(MAPSUFFIX,BURGERSUF) && sx==sx0 && sy==sy0) {sx=0.0; sy=0.00001;}
	
		x=sx; y=sy; 
		for (int i=0; i<80; i++)
		{
			MAPEXPRESSION; x=x_; y=y_;
			if (ISTOOBIG(x) || ISTOOBIG(y)) break;
		}
		
		counted = 0; whichID++; //note incr. in here. effects of previous sx,sy are not counted.
		for (int i=0; i<60; i++)
		{
			if (ISTOOBIG(x) || ISTOOBIG(y)) break;
			MAPEXPRESSION; x=x_; y=y_;
			int px = lrint(PHASESIZE * ((x - X0) / (X1 - X0)));
			int py = lrint(/*PHASESIZE -*/ PHASESIZE * ((y - Y0) / (Y1 - Y0)));
			if (py >= 0 && py < PHASESIZE && px>=0 && px<PHASESIZE)
				if (arr[px+py*PHASESIZE]!=whichID)
				{ arr[px+py*PHASESIZE]=whichID; counted++;}
		}
		if (!(ISTOOBIG(x) || ISTOOBIG(y))) goto FoundTotal;

	}
	}
		//if here, they all escaped.
	return SDL_MapRGB(pSurface->format, 0,0,45);
FoundTotal:
	//return counted;
	double val = counted;
	double estimatedmax= 3.4;
	val = sqrt(sqrt(val));
	if (val > estimatedmax) return SDL_MapRGB(pSurface->format, 255,0,0);
	val /= estimatedmax;
	//return HSL2RGB(pSurface, val, 1.0, 0.5);
	int v = 140+(int)(val*100);
	return SDL_MapRGB(pSurface->format, v,v,v);
}



void DrawMenagerie( SDL_Surface* pMSurface, CoordsDiagramStruct*diagram) 
{

double fx,fy; char* pPosition; Uint32 newcol;
int width = pMSurface->w; int height=pMSurface->h; 
double X0=*diagram->px0, X1=*diagram->px1, Y0=*diagram->py0, Y1=*diagram->py1;
double dx = (X1 - X0) / width, dy = (Y1 - Y0) / height;
fx = X0; fy = Y1; //y counts downwards

for (int py=0; py<height; py++)
{
	fx=X0;
	for (int px = 0; px < width; px++)
	{
		newcol = countPhasePlotPixels(pMSurface, fx, fy, 1);

		pPosition = ( char* ) pMSurface->pixels ; //determine position
		pPosition += ( pMSurface->pitch * py ); //offset by y
		pPosition += ( pMSurface->format->BytesPerPixel * px ); //offset by x
		memcpy ( pPosition , &newcol , pMSurface->format->BytesPerPixel ) ;

		fx += dx;
	}
fy -= dy;
}
}

int CalcMenagerieThread(void* pWhichHalf);
//cache the menagerie figure!
#define CACHEH 3200
#define CACHEW 6400
unsigned char * cachedEntire; double cacheX0=0, cacheX1=1, cacheY0=0, cacheY1=1;
double threadOnesProgress=0.0;
BOOL g_BusyThread1=TRUE, g_BusyThread2=TRUE;
BOOL CreateMenagCache( SDL_Surface* pSurface )
{
	char buffer[256], cachefname[256];
	//note: should have loaded defaults before this.
	cachedEntire = (unsigned char*) malloc(sizeof(unsigned char)*CACHEH*CACHEW);
	if (!cachedEntire) {/*assert(0);*/ exit(1);}

	//first, try to load from file.
	snprintf(cachefname, sizeof(cachefname), "%s/CACHE.dat", SAVESFOLDER);
	if (doesFileExist(cachefname))
	{
		FILE * f = fopen(cachefname, "rb");
		fread(&cacheX0, sizeof(double), 1, f); fread(&cacheX1, sizeof(double), 1, f);
		fread(&cacheY0, sizeof(double), 1, f); fread(&cacheY1, sizeof(double), 1, f);
		fread(cachedEntire, sizeof(unsigned char), CACHEH*CACHEW, f);
		fclose(f);
		return TRUE;
	}

	threadOnesProgress = 0.0;
	int iZero=0, iOne=1;
	g_BusyThread1=g_BusyThread2=TRUE;
	//spawn the 2 threads. then return, without blocking.
	SDL_Thread *thread1 = SDL_CreateThread(CalcMenagerieThread, &iZero);
	SDL_Thread *thread2 =  SDL_CreateThread(CalcMenagerieThread, &iOne);
	while (TRUE)
	{
		SDL_Event event;
		if ( SDL_PollEvent ( &event ) )
		{
			if ( event.type == SDL_QUIT ) { return FALSE; break; }
		}
		SDL_Delay(250);
		snprintf(buffer, sizeof(buffer), "%% %d complete", (int)(threadOnesProgress*100));
		SDL_FillRect ( pSurface , NULL , g_white );
		ShowText(buffer, 50, 150, pSurface);
		ShowText("Computing menagerie diagram (will be saved)...", 50, 50, pSurface);
		SDL_UpdateRect ( pSurface , 0 , 0 , 0 , 0 ) ;
		if (!g_BusyThread1 &&!g_BusyThread2)
			break; //we're done.
	}
	//now save the results into a file.
	
	FILE * f= fopen(cachefname,"wb");
	if (!f) return FALSE;
	fwrite(&g_settings->diagramx0, sizeof(double), 1, f); fwrite(&g_settings->diagramx1, sizeof(double), 1, f);
	fwrite(&g_settings->diagramy0, sizeof(double), 1, f); fwrite(&g_settings->diagramy1, sizeof(double), 1, f);
	fwrite(cachedEntire, sizeof(unsigned char), CACHEH*CACHEW, f);
	fclose(f);
	return TRUE;
}


int CalcMenagerieThread(void* pWhichHalf)
{
	int whichHalf = * (int*)pWhichHalf;
	unsigned char * localarrayOfResults = (whichHalf)? cachedEntire : cachedEntire + (CACHEH/2)*CACHEW;
	//note: should have loaded defaults before this.
	double X0=g_settings->diagramx0, X1=g_settings->diagramx1, Y0=g_settings->diagramy0, Y1=g_settings->diagramy1;
	double dx = (X1 - X0) / CACHEW, dy = (Y1 - Y0) / CACHEH;
	double fx = X0, fy = Y1; //y counts down?
	if (!whichHalf) fy -= (g_settings->diagramy1 - g_settings->diagramy0)/2; //only compute half per thread.
	else fy = Y1;

	for (int py=0; py<CACHEH/2; py+=1) //Note the /2!
	{
		fx=X0;
		for (int px = 0; px < CACHEW; px+=1)
		{
			int count = 4;//alternateCountPhasePlotSSE(settings, fx,fy, whichHalf);
			localarrayOfResults[py*CACHEW + px] = (unsigned char) count;
				
			fx += dx;
		}
		fy -= dy;
		if (whichHalf) threadOnesProgress = py/((double)(CACHEH/2));
	}
	if (whichHalf) g_BusyThread2 = FALSE;
	else g_BusyThread1 = FALSE;
	return 0;
}


void DrawMenagerieFromPrecomputed( SDL_Surface* pSmallSurface, CoordsDiagramStruct*diagram) 
{
double chy_x0 = cacheX0, chy_x1=cacheX1, chy_y0=cacheY0, chy_y1=cacheY1;
	double fx,fy;
	int height=diagram->screen_height;
	int width=diagram->screen_width;
	double X0=*diagram->px0, X1=*diagram->px1, Y0=*diagram->py0, Y1=*diagram->py1;
	
    double dx = (X1 - X0) / width, dy = (Y1 - Y0) / height;
    fx = X0; fy = Y1; //y counts downwards
    
	for (int py=0; py<height; py++)
	{
		fx = X0;
		for (int px = 0; px < width; px++)
		{
int hits;
if (fx<chy_x0 || fx>chy_x1 || fy<chy_y0 || fy>chy_y1)
hits=0;
else
{
	int indexx = (int) (CACHEW * (fx-chy_x0)/(chy_x1-chy_x0));
	int indexy = (int) (CACHEH * (fy-chy_y0)/(chy_y1-chy_y0));
	hits = cachedEntire[ indexy*CACHEW + indexx];
}
		double val = sqrt((double)hits)/10.0;

		if (val>1.0) val=1.0; if (val<0.0) val=0.0;
		val = val*2 - 1; //from -1 to 1
		Uint32 r,g,b;
		if (val>=0)
			b=255, r=g= (Uint32) ((1-val)*255.0);
		else
			r=g=b= (Uint32) ((val+1)*255.0);

char* pPosition = ( char* ) pSmallSurface->pixels ; //determine position
pPosition += ( pSmallSurface->pitch * py ); //offset by y
pPosition += ( pSmallSurface->format->BytesPerPixel * px ); //offset by x
Uint32 newcol = SDL_MapRGB ( pSmallSurface->format , r , g , b ) ;
memcpy ( pPosition , &newcol , pSmallSurface->format->BytesPerPixel ) ;

			fx += dx;
		}

		fy -= dy;
	}
}



