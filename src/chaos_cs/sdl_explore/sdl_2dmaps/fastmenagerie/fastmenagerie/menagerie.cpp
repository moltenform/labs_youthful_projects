#include "phaseportrait.h"
#include "whichmap.h"
#include "coordsdiagram.h"
#include "float_cast.h"
#include "palette.h"
#include "font.h"
#include "configfiles.h"
#define _USE_MATH_DEFINES
#include <math.h>
/*

when different seedx/seedy are tried, it means that the white areas are slow because must do every seed
in this case, we inefficiently assign work, because one thread has more white area and takes longer.
Better that the threads are evenly balanced(!)
So do something better than just each thread gets vertically half the image. splice them?
(seedsperaxis=10, 1thread=7543, 2threads=5132, not much saving)
(seedsperaxis=1, 1thread=2098, 2threads=1248, better saving)
use strips to improve this. load balancing.

Why lyapunov was slow: 
-settle time only needs one pt, not two.
-log and sqrt were expensive calls. 
-no loop unrolling


*/
#include "bit_approximations.h"

int alternateCountPhasePlotSSE(SDL_Surface* pSurface, double c1, double c2, int whichThread);
int countPhasePlotLyapunovVector(SDL_Surface* pSurface,double c1, double c2);




// This file computes the "menagerie diagram", a plot in parameter space.
// there are two methods: computing lyapunov exponent, and counting pixels drawn in a phase portrait

int gParamMenagerieMode=0;
void toggleMenagerieMode() {gParamMenagerieMode = (gParamMenagerieMode+1)%4; }
__inline unsigned int standardToColors(SDL_Surface* pSurface, double valin, double estimatedMax)
{
	if (!(gParamMenagerieMode&1)) {
		//rainbow colors
		double val = (valin) / (estimatedMax);
		if (val > estimatedMax) return SDL_MapRGB(pSurface->format, 50,0,0);
		val = ((valin) / (estimatedMax)) * 0.8 /*only use 0.0-0.8, because 0.99 looks close to 0.0*/;
		return HSL2RGB(pSurface, val, 1,.5);
	} else {
		//black, white, blue
		double val = (valin);
		if (val > (estimatedMax)) return SDL_MapRGB(pSurface->format, 50,0,0);
		if (val > (estimatedMax)*0.75)
		{
			val = (val-(estimatedMax)*0.75)/(estimatedMax*0.25);
			return SDL_MapRGB(pSurface->format,  255-(Uint8)lrint(val*255),255-(Uint8)lrint(val*255),(Uint8)255);
		}
		else
		{
			val = (val)/(estimatedMax*0.75);
			return SDL_MapRGB(pSurface->format, (Uint8) lrint(val*255),(Uint8)lrint(val*255),(Uint8)lrint(val*255));
		}

	}
}


//keep going until find one that goes the whole ways without escaping.
//todo: it'd be better to try seedx/seedy points that are not close to the previous tried...
int countPhasePlotLyapunov(SDL_Surface* pSurface,double c1, double c2)
{
	//http://sprott.physics.wisc.edu/chaos/lyapexp.htm
	double d0 = 1e-3, d1, total;
	double x, y, x_,y_; double x2, y2; double xtmp, ytmp, sx, sy;
	int N = CountPixelsSettle+CountPixelsDraw; int settle = CountPixelsSettle;
	double sx0= g_settings->seedx0, sx1=g_settings->seedx1, sy0= g_settings->seedy0, sy1=g_settings->seedy1;
	int nXpoints=CountPixelsSeedsPerAxis, nYpoints=CountPixelsSeedsPerAxis;
	double sxinc = (nXpoints==1) ? 1e6 : (sx1-sx0)/(nXpoints-1);
	double syinc = (nYpoints==1) ? 1e6 : (sy1-sy0)/(nYpoints-1);
	//for (double sxi=sx0; sxi<=sx1; sxi+=sxinc)
 //   {
 //   for (double syi=sy0; syi<=sy1; syi+=syinc)
 //   {
	/*if (StringsEqual(MAPSUFFIX,BURGERSUF) && sxi==sx0 && syi==sy0) {sx=0.0; sy=0.00001;}
	else {sx=sxi; sy=syi;}*/
	sx=0.0; sy=0.00001;
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
		if (ISTOOBIG(x) || ISTOOBIG(y))
			break;
	}
	if (!(ISTOOBIG(x) || ISTOOBIG(y))) goto FoundTotal;
	/*}
	}*/
	if (ISTOOBIG(x) || ISTOOBIG(y))
		return SDL_MapRGB(pSurface->format, 255,255,255);
FoundTotal:
	double val = total / (N-settle);
	if (val < 0) 
		return 0;
	return standardToColors(pSurface, val, .8);
	// 4 possibilities: all escape: white, negative lyapunov: black, ly>cutoff: dark red, otherwise rainbow ly
}




//NOTE: use /fp:fast
int countPhasePlotLyapunovVector(SDL_Surface* pSurface,double c1, double c2)
{
	float d0 = 1e-3, d1, total;
	float x, y, x_,y_; float x2, y2, x2_; float xtmp, ytmp, sx, sy;
	
	sx=0.0; sy=0.00001;
	x=sx; y=sy; 
	
	for (int i=0; i<CountPixelsSettle/4; i++)
	{
		x_ = c1*x - y*y; y= c2*y + x*y; x=x_;
		x_ = c1*x - y*y; y= c2*y + x*y; x=x_;
		x_ = c1*x - y*y; y= c2*y + x*y; x=x_;
		x_ = c1*x - y*y; y= c2*y + x*y; x=x_;

		if (ISTOOBIG(x) || ISTOOBIG(y))
			return SDL_MapRGB(pSurface->format, 255,255,255);
	}
	total = 0.0;
	x=x; y=y;
	x2=x; y2=y+d0;
	for (int i=0; i<CountPixelsDraw/4; i++)
	{
		x_ = c1*x - y*y; y= c2*y + x*y;
		x=x_;
		x2_ = c1*x2 - y2*y2; y2= c2*y2 + x2*y2;
		x2=x2_;

		d1 = ( (x2-x)*(x2-x) + (y2-y)*(y2-y) ); //distance
		float invd1 = invSqrt(d1);
		//oh man, this is totally an inverse sqrt, so we can do the hack! improves time by ~80!
		float d1_d0 = d0*invd1;
		x2=x+ (d1_d0)*(x2-x); //also looks interesting when these are commented out
		y2=y+ (d1_d0)*(y2-y);

		//total+= logBase2OfFloat((1/d1_d0) );//, and add 60 to total later.
		//total+= log((1/d1_d0) );
		//total+= wikipediaQuickLog((1/d1_d0) );	// and add about 5 to total
		total+= flipcodefast_log2((1/d1_d0) );	//which works well!

		x_ = c1*x - y*y; y= c2*y + x*y;
		x=x_;
		x2_ = c1*x2 - y2*y2; y2= c2*y2 + x2*y2;
		x2=x2_;
		d1 = ( (x2-x)*(x2-x) + (y2-y)*(y2-y) );
		 invd1 = invSqrt(d1);
		 d1_d0 = d0*invd1;
		x2=x+ (d1_d0)*(x2-x);
		y2=y+ (d1_d0)*(y2-y);
		total+= flipcodefast_log2((1/d1_d0) );
		x_ = c1*x - y*y; y= c2*y + x*y;
		x=x_;
		x2_ = c1*x2 - y2*y2; y2= c2*y2 + x2*y2;
		x2=x2_;
		d1 = ( (x2-x)*(x2-x) + (y2-y)*(y2-y) );
		 invd1 = invSqrt(d1);
		 d1_d0 = d0*invd1;
		x2=x+ (d1_d0)*(x2-x);
		y2=y+ (d1_d0)*(y2-y);
		total+= flipcodefast_log2((1/d1_d0) );
		x_ = c1*x - y*y; y= c2*y + x*y;
		x=x_;
		x2_ = c1*x2 - y2*y2; y2= c2*y2 + x2*y2;
		x2=x2_;
		d1 = ( (x2-x)*(x2-x) + (y2-y)*(y2-y) );
		 invd1 = invSqrt(d1);
		 d1_d0 = d0*invd1;
		x2=x+ (d1_d0)*(x2-x);
		y2=y+ (d1_d0)*(y2-y);
		total+= flipcodefast_log2((1/d1_d0) );


		if (ISTOOBIG(x) || ISTOOBIG(y)) 
			return SDL_MapRGB(pSurface->format, 255,255,255);
	}

		
FoundTotal:
	//double val = total / (CountPixelsDraw);
	if (total < 0) 
		return 0;
	//convert base 2 to base e
	total *= 1/(log(M_E)/log(2.0));
	return standardToColors(pSurface, total, .8 * CountPixelsDraw);
}



// this can be made faster with SSE instructions, but that'd require an SSE version of MAPEXPRESSION
#define PHASESIZE 128
int arrT1[PHASESIZE*PHASESIZE] = {0};
int arrT2[PHASESIZE*PHASESIZE] = {0};
int whichIDT1 = 2, whichIDT2 = 2, whichID=2;
int countPhasePlotPixels(SDL_Surface* pSurface, double c1, double c2, int whichThread, FastMapMapSettings * boundsettings)
{
	int * arr = whichThread ? arrT1:arrT2;
	int * whichID = (whichThread)?&whichIDT1:&whichIDT2;

	int counted; double x,y,x_,y_, sx,sy;
	double sx0= g_settings->seedx0, sx1=g_settings->seedx1, sy0= g_settings->seedy0, sy1=g_settings->seedy1;
	int nXpoints=CountPixelsSeedsPerAxis, nYpoints=CountPixelsSeedsPerAxis;
	double sxinc = (nXpoints==1) ? 1e6 : (sx1-sx0)/(nXpoints-1);
	double syinc = (nYpoints==1) ? 1e6 : (sy1-sy0)/(nYpoints-1);
	double X0=boundsettings->x0, X1=boundsettings->x1, Y0=boundsettings->y0, Y1=boundsettings->y1;

	for (double sxi=sx0; sxi<=sx1; sxi+=sxinc)
    {
    for (double syi=sy0; syi<=sy1; syi+=syinc)
    {
	if (StringsEqual(MAPSUFFIX,BURGERSUF) && sxi==sx0 && syi==sy0) {sx=0.0; sy=0.00001;}
	else {sx=sxi; sy=syi;}
		x=sx; y=sy; 
		for (int i=0; i<CountPixelsSettle; i++)
		{
			MAPEXPRESSION; x=x_; y=y_;
			if (ISTOOBIG(x) || ISTOOBIG(y)) break;
		}
		
		counted = 0; *whichID = (*whichID)+1; //note incr. in here. effects of previous sx,sy are not counted.
		for (int i=0; i<CountPixelsDraw; i++)
		{
			if (ISTOOBIG(x) || ISTOOBIG(y)) break;
			MAPEXPRESSION; x=x_; y=y_;
			int px = lrint(PHASESIZE * ((x - X0) / (X1 - X0)));
			int py = lrint(/*PHASESIZE -*/ PHASESIZE * ((y - Y0) / (Y1 - Y0)));
			if (py >= 0 && py < PHASESIZE && px>=0 && px<PHASESIZE)
				if (arr[px+py*PHASESIZE]!=*whichID)
				{ arr[px+py*PHASESIZE]=*whichID; counted++;}
		}
		if (!(ISTOOBIG(x) || ISTOOBIG(y))) goto FoundTotal;

	}
	}
	//if here, they all escaped.
	return 0;
FoundTotal:
	return standardToColors(pSurface, (double)counted, (double)CountPixelsDraw);
		
}

// Each thread creates half of the diagram. The PassToThread struct simply passes arguments to function.
#define LegendWidth 4
typedef struct { int whichHalf; SDL_Surface* pMSurface; CoordsDiagramStruct*diagram; FastMapMapSettings * bounds; } PassToThread;
int DrawMenagerieThread( void* pStruct) 
{
	PassToThread*p = (PassToThread *)pStruct; int whichHalf = p->whichHalf; SDL_Surface*pMSurface=p->pMSurface;CoordsDiagramStruct*diagram=p->diagram; 
	FastMapMapSettings * boundsettings=p->bounds;

	double fx,fy; char*pPosition; Uint32 newcol;
	int width = pMSurface->w - LegendWidth; int height=pMSurface->h; 
	double X0=*diagram->px0, X1=*diagram->px1, Y0=*diagram->py0, Y1=*diagram->py1;
	double dx = (X1 - X0) / width, dy = (Y1 - Y0) / height;
	fx = X0; fy = Y1; //y counts downwards
	//only compute half per thread.
	if (whichHalf) fy = Y1; 
	else fy = Y1 - dy;

	for (int py=(whichHalf?0:1); py<(height); py+=2)
	{
		fx=X0;
		for (int px = 0; px < width; px++)
		{
			/*if (gParamMenagerieMode==0||gParamMenagerieMode==1) 
				newcol = countPhasePlotLyapunov(pMSurface, fx, fy);
			else 
				newcol = countPhasePlotPixels(pMSurface,fx,fy,whichHalf,boundsettings);*/
//newcol = alternateCountPhasePlotSSE(pMSurface,fx,fy,whichHalf);
newcol = countPhasePlotLyapunovVector(pMSurface,fx,fy);

			pPosition = ( char* ) pMSurface->pixels ; //determine position
			pPosition += ( pMSurface->pitch * py ); //offset by y
			pPosition += ( pMSurface->format->BytesPerPixel * px ); //offset by x
			memcpy ( pPosition , &newcol , pMSurface->format->BytesPerPixel ) ;

			fx += dx;
		}
	fy -= 2*dy;
	}
	return 0;
}
PassToThread pThread1; PassToThread pThread2;
#include "timecounter.h"
void DrawMenagerieMultithreaded( SDL_Surface* pMSurface, CoordsDiagramStruct*diagram) 
{
	//draw a color legend. we don't need to cache this, it's very fast
	for (int px=pMSurface->w - LegendWidth; px<pMSurface->w; px++)
	for (int py=0; py<pMSurface->h; py++) {
		int color = standardToColors(pMSurface, (double)py, (double) pMSurface->h);
		char* pPosition = ( char* ) pMSurface->pixels ; //determine position
		pPosition += ( pMSurface->pitch * (pMSurface->h - py-1) ); //offset by y
		pPosition += ( pMSurface->format->BytesPerPixel * (px) ); //offset by x
		memcpy ( pPosition , &color , pMSurface->format->BytesPerPixel ) ;
	}
	//put into boundsettings a copy of default bounds, which we'll use in countpixels.
	FastMapMapSettings currentSettings, boundsettings;
	memcpy(&currentSettings, g_settings, sizeof(FastMapMapSettings));
	loadFromFile(MAPDEFAULTFILE); //load defaults
	memcpy(&boundsettings, g_settings, sizeof(FastMapMapSettings));
	memcpy(g_settings, &currentSettings, sizeof(FastMapMapSettings));

	//pass arguments via structure. note: pThread1 apparently should be global.
	pThread1.whichHalf=0; pThread1.bounds=&boundsettings; pThread1.diagram=diagram; pThread1.pMSurface=pMSurface; 
	pThread2.whichHalf=1; pThread2.bounds=&boundsettings; pThread2.diagram=diagram; pThread2.pMSurface=pMSurface; 
	
startTimer();
	SDL_Thread *thread2 =  SDL_CreateThread(DrawMenagerieThread, &pThread2);
	DrawMenagerieThread(&pThread1);
	SDL_WaitThread(thread2, NULL);
printf("timeb:%d", (int)stopTimer());

/*startTimer();
	DrawMenagerieThread(&pThread1);
printf("time1:%d", (int)stopTimer());
startTimer();
	DrawMenagerieThread(&pThread2);
printf("time2:%d", (int)stopTimer());*/

}

//sse. do one with just 4. let's try this.
//we use the fact that (0,0.000001) and (0,-0.000001) should be in the basin.
//we still have to check for it getting too big, though.
int CURRENTID1=2,CURRENTID2=2;
int alternateCountPhasePlotSSE(SDL_Surface* pSurface, double c1, double c2, int whichThread)
{
	int CURRENTID =  whichThread? ++CURRENTID1 : ++CURRENTID2;
	int*arr = whichThread? arrT1:arrT2;
	int counted=0;
	///double X0=settings->menagphasex0, X1=settings->menagphasex1, Y0=settings->menagphasey0, Y1=settings->menagphasey1;
	double X0=-3, X1=1, Y0=-3, Y1=3;

	__m128 mmX = _mm_setr_ps( 0.0f, 0.0f, 0.0f, 0.0f);
	__m128 mmY = _mm_setr_ps( 0.000001f, 0.000002f,-0.0000011f,-0.0000019f); //symmetrical, so don't just mult by 2.
	__m128 mXTmp;
	MAPSSEINIT;

	for (int i=0; i<CountPixelsSettle/4; i++)
	{
		MAPSSE; 
		MAPSSE;
		MAPSSE;
		MAPSSE;
		if (ISTOOBIGF(mmX.m128_f32[0]) || ISTOOBIGF(mmX.m128_f32[1]) || ISTOOBIGF(mmX.m128_f32[2]) || ISTOOBIGF(mmX.m128_f32[3]) ||
			ISTOOBIGF(mmY.m128_f32[0]) || ISTOOBIGF(mmY.m128_f32[1]) || ISTOOBIGF(mmY.m128_f32[2]) || ISTOOBIGF(mmY.m128_f32[3]))
		{counted=0; goto theend;} //note: shouldn't do this??? only one of the four dropped...
	}
	//drawing time.
	float CW = 1.0f/(X1-X0);
	__m128 mMultW = _mm_set1_ps(CW * PHASESIZE * PHASESIZE);  //EXTRA FACTOR
	int ShiftW = (int) (-X0 * PHASESIZE * CW * PHASESIZE); //if -3 to 3, this shifts by +128 (half the arrWidth). 
									//This is sometimes an approximation, but ok due to speed
	float CH = 1.0f/(Y1-Y0);
	__m128 mMultH = _mm_set1_ps(CH * PHASESIZE ); 
	int ShiftH = (int) (-Y0 * PHASESIZE * CH );
	__m128 xPrelimTimes256, yPrelim;
	__m128i mmShiftW = _mm_set1_epi32(ShiftW);
	__m128i mmShiftH = _mm_set1_epi32(ShiftH);
	for (int i=0; i<CountPixelsDraw/2 /* /8 */; i++) //see how changes if drawing increases?
	{
		MAPSSE;
		xPrelimTimes256 = _mm_mul_ps(mmX, mMultW);
		yPrelim = _mm_mul_ps(mmY, mMultH);
		
		__m128i xPt256 = _mm_cvttps_epi32(xPrelimTimes256); //cast all to int at once. truncate mode.
		__m128i yPt = _mm_cvttps_epi32(yPrelim); 
		xPt256 = _mm_add_epi32(xPt256, mmShiftW);
		yPt = _mm_add_epi32(yPt, mmShiftH);
		__m128i xySum = _mm_add_epi32(xPt256, yPt); //this is worth doing, even though we don't always use it.
		
		//if (xySum.m128i_i32[0] >= 0 && xySum.m128i_i32[0] < PHASESIZE*PHASESIZE) { 
		if (yPt.m128i_i32[0] >= 0 && yPt.m128i_i32[0] < PHASESIZE && xPt256.m128i_i32[0]>=0 && xPt256.m128i_i32[0]<(PHASESIZE* (PHASESIZE-1))) { 
			if (arr[xySum.m128i_i32[0]]!=CURRENTID)
		    { arr[xySum.m128i_i32[0]]=CURRENTID; counted++;}
		}
		//if (xySum.m128i_i32[1] >= 0 && xySum.m128i_i32[1] < PHASESIZE*PHASESIZE) { 
		if (yPt.m128i_i32[1] >= 0 && yPt.m128i_i32[1] < PHASESIZE && xPt256.m128i_i32[1]>=0 && xPt256.m128i_i32[1]<(PHASESIZE* (PHASESIZE-1))) { 
			if (arr[xySum.m128i_i32[1]]!=CURRENTID)
		    { arr[xySum.m128i_i32[1]]=CURRENTID; counted++;}
		}
		//if (xySum.m128i_i32[2] >= 0 && xySum.m128i_i32[2] < PHASESIZE*PHASESIZE) { 
		if (yPt.m128i_i32[2] >= 0 && yPt.m128i_i32[2] < PHASESIZE && xPt256.m128i_i32[2]>=0 && xPt256.m128i_i32[2]<(PHASESIZE* (PHASESIZE-1))) { 
			if (arr[xySum.m128i_i32[2]]!=CURRENTID)
		    { arr[xySum.m128i_i32[2]]=CURRENTID; counted++;}
		}
		//if (xySum.m128i_i32[3] >= 0 && xySum.m128i_i32[3] < PHASESIZE*PHASESIZE) { 
		if (yPt.m128i_i32[3] >= 0 && yPt.m128i_i32[3] < PHASESIZE && xPt256.m128i_i32[3]>=0 && xPt256.m128i_i32[3]<(PHASESIZE* (PHASESIZE-1))) { 
			if (arr[xySum.m128i_i32[3]]!=CURRENTID)
		    { arr[xySum.m128i_i32[3]]=CURRENTID; counted++;}
		}

		//Loop unrolled///////////////////////////////////////
		MAPSSE;
		xPrelimTimes256 = _mm_mul_ps(mmX, mMultW);
		yPrelim = _mm_mul_ps(mmY, mMultH);
		
		 xPt256 = _mm_cvttps_epi32(xPrelimTimes256); //cast all to int at once. truncate mode.
		 yPt = _mm_cvttps_epi32(yPrelim); 
		xPt256 = _mm_add_epi32(xPt256, mmShiftW);
		yPt = _mm_add_epi32(yPt, mmShiftH);
		 xySum = _mm_add_epi32(xPt256, yPt); //this is worth doing, even though we don't always use it.
		
		//if (xySum.m128i_i32[0] >= 0 && xySum.m128i_i32[0] < PHASESIZE*PHASESIZE) { 
		if (yPt.m128i_i32[0] >= 0 && yPt.m128i_i32[0] < PHASESIZE && xPt256.m128i_i32[0]>=0 && xPt256.m128i_i32[0]<(PHASESIZE* (PHASESIZE-1))) { 
			if (arr[xySum.m128i_i32[0]]!=CURRENTID)
		    { arr[xySum.m128i_i32[0]]=CURRENTID; counted++;}
		}
		//if (xySum.m128i_i32[1] >= 0 && xySum.m128i_i32[1] < PHASESIZE*PHASESIZE) { 
		if (yPt.m128i_i32[1] >= 0 && yPt.m128i_i32[1] < PHASESIZE && xPt256.m128i_i32[1]>=0 && xPt256.m128i_i32[1]<(PHASESIZE* (PHASESIZE-1))) { 
			if (arr[xySum.m128i_i32[1]]!=CURRENTID)
		    { arr[xySum.m128i_i32[1]]=CURRENTID; counted++;}
		}
		//if (xySum.m128i_i32[2] >= 0 && xySum.m128i_i32[2] < PHASESIZE*PHASESIZE) { 
		if (yPt.m128i_i32[2] >= 0 && yPt.m128i_i32[2] < PHASESIZE && xPt256.m128i_i32[2]>=0 && xPt256.m128i_i32[2]<(PHASESIZE* (PHASESIZE-1))) { 
			if (arr[xySum.m128i_i32[2]]!=CURRENTID)
		    { arr[xySum.m128i_i32[2]]=CURRENTID; counted++;}
		}
		//if (xySum.m128i_i32[3] >= 0 && xySum.m128i_i32[3] < PHASESIZE*PHASESIZE) { 
		if (yPt.m128i_i32[3] >= 0 && yPt.m128i_i32[3] < PHASESIZE && xPt256.m128i_i32[3]>=0 && xPt256.m128i_i32[3]<(PHASESIZE* (PHASESIZE-1))) { 
			if (arr[xySum.m128i_i32[3]]!=CURRENTID)
		    { arr[xySum.m128i_i32[3]]=CURRENTID; counted++;}
		}

		if (ISTOOBIGF(mmX.m128_f32[0]) || ISTOOBIGF(mmX.m128_f32[1]) || ISTOOBIGF(mmX.m128_f32[2]) || ISTOOBIGF(mmX.m128_f32[3]) ||
			ISTOOBIGF(mmY.m128_f32[0]) || ISTOOBIGF(mmY.m128_f32[1]) || ISTOOBIGF(mmY.m128_f32[2]) || ISTOOBIGF(mmY.m128_f32[3]))
		{counted=0; goto theend;} //note: shouldn't do this??? only one of the four dropped...
	}
	
theend:
	//alter due to new coloring.
	/*if (counted==0) return 0x0;
	else return standardToColors((double)counted, DRAWING);*/
	if (counted==0) return 0xffffff;
	else if (counted<50) { return 0x0;}
	else return standardToColors(pSurface, ((double)counted), ((double)CountPixelsDraw));
}

void blitDiagram(SDL_Surface* pSurface,SDL_Surface* pSmallSurface, int px, int py)
{
	SDL_Rect dest;
	dest.x = px;
	dest.y = py;
	dest.w = pSmallSurface->w;
	dest.h = pSmallSurface->h; 
	SDL_BlitSurface(pSmallSurface, NULL, pSurface, &dest);
}