
#include "common.h"
#include "phaseportrait.h"
#include "fastmenagerie.h"
#include "float_cast.h"

#define USERAINBOW
#if TRUE
#define SETTLING 1450
//#define SETTLING 1900000 
//150
//#define DRAWING 900000 
#define DRAWING 450 
//24000
#define POSTERIZETYPE 0
#else //Show period doubling
#define SETTLING 700
#define DRAWING 500
#define POSTERIZETYPE 1
#endif

int alternateCountPhasePlot(MenagFastSettings*settings,double c1, double c2, int whichThread);
int alternateCountPhasePlotSSE(MenagFastSettings*settings,double c1, double c2, int whichThread);
int alternateCountPhasePlotSSEISPOS(MenagFastSettings*settings,double c1, double c2, int whichThread);
int alternateCountPhasePlotSSEGetDimension(MenagFastSettings*settings,double c1, double c2, int whichThread);
int countPhasePlotLyapunov(MenagFastSettings*settings,double c1, double c2, int whichThread);
int countPhasePlotAverage(MenagFastSettings*settings,double c1, double c2, int whichThread);
int countPhasePlotVariance(MenagFastSettings*settings,double c1, double c2, int whichThread);
int countPhasePlotDualS(MenagFastSettings*settings,double c1, double c2, int whichThread);
int countPhasePlotHowMuchTransience(MenagFastSettings*settings,double c1, double c2, int whichThread);
BOOL g_BusyThread1 = FALSE;
BOOL g_BusyThread2 = FALSE;

void InitialSettings(MenagFastSettings*ps, int width, int height, double *pa, double *pb)
{
	ps->menagSeedsPerAxis = 40;
	ps->menagSettling = 48;
	ps->menagDrawing = 20; //also, # of iters for the Basins mode.
	ps->browsex0 = 0; ps->browsex1 = 1; ps->browsey0=0; ps->browsey1 = 1;
	//ps->x0 = 0; ps->x1 = 1; ps->y0=0; ps->y1 = 1;
	
	if (StringsEqual(STRINGIFY(MAPEXPRESSION), STRINGIFY(BURGER)))
	{
		*pa = -1.1; *pb = 1.72;
		//ps->browsex0 = -2; ps->browsex1 = 2; ps->browsey0=-0.5; ps->browsey1 = 3.5;
		ps->browsex0 = -2.5; ps->browsex1 = 1; ps->browsey0=1.5; ps->browsey1 = 2.0; 
		//ps->browsex0 = -2.6275; ps->browsex1 = -.3678; ps->browsey0=1.5893; ps->browsey1 = 1.9043; 
		//ps->browsex0 = .775233; ps->browsex1 = 5; ps->browsey0=1.5; ps->browsey1 = 1.8; 
		///ps->browsex0 = -.686588-.1; ps->browsex1 = -.686588+.1; ps->browsey0=1.733079-.1; ps->browsey1 = 1.733079+.1; 
		//ah, but my zoom in/out code always makes square zoom sizes...
		//ps->x0 = -1.75; ps->x1 = 1.75; ps->y0=-1.75; ps->y1 = 1.75;
		ps->menagphasex0 = -3/*-5*/; ps->menagphasex0 = 1; ps->menagphasex0=0 /*it's symmetrical */; ps->menagphasex0 = 3;
		ps->seedx0 = -3; ps->seedx1 = 1; ps->seedy0=0 /*it's symmetrical */; ps->seedy1 = 3;
	}
	else //HENON MAP
	{
		*pa = 1.4; *pb = 0.3;
		ps->browsex0 = -2; ps->browsex1 = 2; ps->browsey0=-1; ps->browsey1 = 3;
		//ps->x0 = -1.75; ps->x1 = 1.75; ps->y0=-1.75; ps->y1 = 1.75;
		ps->menagphasex0 = -3; ps->menagphasex0 = 3; ps->menagphasex0=-3; ps->menagphasex0 = 3;
		ps->seedx0 = -3; ps->seedx1 = 3; ps->seedy0=-3 ; ps->seedy1 = 3;
		
	}

}

#include "colortable_1024.h"
int HSL2RGB(SDL_Surface* pSurface, double h, double sl, double l);
extern SDL_Surface* g_surface;
//gradients changing all r,g,b are better, maybe why black to white is better.
inline unsigned int standardToColors(double valin, double estimatedMax)
{
	double val = (valin) / (estimatedMax);
#ifdef USERAINBOW
	if (val > estimatedMax) val =estimatedMax;
val = (valin) / (estimatedMax*1.2);
	if (val<0) val=0; if (val>1) val=1;
	val = fmod(val-0.2, 1.0); if (val<0) val+=1;
	//val *= 0.7;
	return HSL2RGB(NULL, val, 1,.5);
#endif
	//val = sqrt(val) / sqrt(estimatedMax);
	
	int index = lrint(val * (1023));
//index += 250; //////////////////////////// for lyapunov
	if (index<=0) return 0;
	else if (index >= 1024) return 255<<8; //pure green xxrrggxx

#if POSTERIZETYPE==1
	int ival = lrint(valin/2);
	ival = ival-3; if (ival<0) ival=0;
	if (ival==0) return 0x000000; //bbggrr
	else if (ival==1) return 0xFF0000;
	else if (ival==2) return 0xFF7F00;
	else if (ival==3) return 0xFFFF00;
	else if (ival==4) return 0x00FF00;
	else if (ival==5) return 0x0000FF;
	else if (ival==6) return 0x6600FF;
	else if (ival==7) return 0x8B00FF;
	else if (ival==8) return 0xFF00FF;
	else if (ival<30) return 0x7f7f7f;
	else return 0x0FFFFFF;
	//if (val) index *= 40;
	//if (index<25) index *= 40;
	//else index = 1000;
#endif
	//if (index < 870) index = 700;
	//else index=900;
	return color32bit[index];
}

#include "fastmenagerieutil.h"
int CalcFastFastMenagerie(void* data)
{
	MenagFastSettings*settings = ((ThreadStructure*)data)->settings;
	int whichHalf = ((ThreadStructure*)data)->whichHalf;
	int * localarrayOfResults = (whichHalf)? arrayOfResults : arrayOfResults + (MenagHeight/2)*MenagWidth;
	double X0=settings->browsex0, X1=settings->browsex1;
	double Y0=settings->browsey0, Y1=settings->browsey1;
	double dx = (X1 - X0) / MenagWidth, dy = (Y1 - Y0) / MenagHeight;
	double fx = X0, fy = Y1; //y counts down?
	if (!whichHalf) fy -= (settings->browsey1 - settings->browsey0)/2;
	else fy = Y1;

	for (int py=0; py<MenagHeight/2; py+=1) //Note the /2!
	{
		fx=X0;
		for (int px = 0; px < MenagWidth; px+=1)
		{
			//double ffx = sqrt(sqrt(sqrt(fabs(fx))));
			//double ffy = fy;//(fy + fx*.0725);//
			double ffx = fx;//(fx<0.3)?fx : (-1/(fx*fx) + 1);//(fx>-0.3)?fx : (1/(fx*fx) - 1.6);
			double ffy = fy; //(fy) * (1.1- fx*fx/90);
			localarrayOfResults[py*MenagWidth + px] = 
				alternateCountPhasePlotSSE(settings, ffx,ffy, whichHalf);
			fx += dx;
		}
		fy -= dy;
	}
	if (whichHalf) g_BusyThread2 = FALSE;
	else g_BusyThread1 = FALSE;
	return 0;
}
int THECOUNT = 0;

//sse. do one with just 4. let's try this.
//we use the fact that (0,0.000001) and (0,-0.000001) should be in the basin.
//we still have to check for it getting too big, though.
int alternateCountPhasePlotSSE(MenagFastSettings*settings,double c1, double c2, int whichThread)
{
	int CURRENTID =  whichThread? ++CURRENTID1 : ++CURRENTID2;
	int*arr = whichThread? arrThread1:arrThread2;
	int counted=0;
	///double X0=settings->menagphasex0, X1=settings->menagphasex1, Y0=settings->menagphasey0, Y1=settings->menagphasey1;
	double X0=-3, X1=1, Y0=-3, Y1=3;

	__m128 mmX = _mm_setr_ps( 0.0f, 0.0f, 0.0f, 0.0f);
	__m128 mmY = _mm_setr_ps( 0.000001f, 0.000002f,-0.0000011f,-0.0000019f); //symmetrical, so don't just mult by 2.
	__m128 mXTmp;
	burgerSetup;

	for (int i=0; i<SETTLING/4; i++)
	{
		burgerExpression; 
		burgerExpression;
		burgerExpression;
		burgerExpression;
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
	for (int i=0; i<DRAWING/8; i++) //see how changes if drawing increases?
	{
		burgerExpression;
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
		burgerExpression;
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
	else if (counted<50) { THECOUNT++; return 0x0;}
	else return standardToColors(((double)counted), ((double)DRAWING));
}

int countPhasePlotLyapunov(MenagFastSettings*settings,double c1, double c2, int whichThread)
{
	//http://sprott.physics.wisc.edu/chaos/lyapexp.htm
	double d0 = 1e-3, d1;
	//int N = 400; int settle = 300; //used to be 20. 1000/600 causes moire patterns
	int N = 900; int settle = 800; //used to be 20. 1000/600 causes moire patterns
	double total = 0.0;
	double sx = 0.0;double sy=0.00001;
	double x=sx, y=sy, x_;
	double x2=sx, y2=sy+d0, x2_;
	for (int i=0; i<N; i++)
	{
		x_ = c1*x - y*y; y= c2*y + x*y;
		x=x_;
		x2_ = c1*x2 - y2*y2; y2= c2*y2 + x2*y2;
		x2=x2_;

		d1 = sqrt( (x2-x)*(x2-x) + (y2-y)*(y2-y) ); //distance
		x2=x+ (d0/d1)*(x2-x); //also looks interesting when these are commented out
		y2=y+ (d0/d1)*(y2-y);
		if (i>settle) total+= log(d1/d0 );
		if (ISTOOBIG(x) || ISTOOBIG(y)) return 0xffffff;//255<<8; //pure green xxrrggxx
	}
	double val = total / (N-settle);
	if (val < 0) { THECOUNT++; return 0x0;//50<<8;
		//val = sqrt(sqrt(-val));
		//return standardToColors((double)val, .9);

	} 
	val = sqrt(sqrt(val));
	return standardToColors((double)val, .9);
}

int countPhasePlotAverage(MenagFastSettings*settings,double c1, double c2, int whichThread)
{
	double d0 = 1e-3, d1;
	int N = 900; int settle = 800; //used to be 20. 1000/600 causes moire patterns
	double total = 0.0;
	double sx = 0.0;double sy=0.00001;
	double x=sx, y=sy, x_;
	for (int i=0; i<N; i++)
	{
		x_ = c1*x - y*y; y= c2*y + x*y;
		x=x_;

		d1 = sqrt( x*x + y*y ); //distance
		if (i>settle) total+= d1;
		if (ISTOOBIG(x) || ISTOOBIG(y)) return 0xffffff;//255<<8; //pure green xxrrggxx
	}
	double val = total / (N-settle);
	
	return standardToColors((double)val, 3);
}

int countPhasePlotVariance(MenagFastSettings*settings,double c1, double c2, int whichThread)
{
	double d0 = 1e-3, d1;
	int N = 900; int settle = 800; //used to be 20. 1000/600 causes moire patterns
	double total = 0.0;
	double x=0.0, y=0.00001, x_;
	//find avg x value
	for (int i=0; i<N; i++)
	{
		x_ = c1*x - y*y; y= c2*y + x*y;
		x=x_;

		d1 = x; //
		if (i>settle) total+= d1;
		if (ISTOOBIG(x) || ISTOOBIG(y)) return 0xffffff;//255<<8; //pure green xxrrggxx
	}
	double avgx = total / (N-settle);
	//find variance
	total=0;
	for (int i=0; i<N; i++)
	{
		x_ = c1*x - y*y; y= c2*y + x*y;
		x=x_;

		d1 = (x-avgx)*(x-avgx); //
		if (i>settle) total+= d1;
		if (ISTOOBIG(x) || ISTOOBIG(y)) return 0xffffff;//255<<8; //pure green xxrrggxx
	}
	double variance = total / (N-settle);

	//normalize!!
	double val = variance / ( avgx*avgx);
	return standardToColors((double)val, 6); //2
}

int countPhasePlotDualS(MenagFastSettings*settings,double c1, double c2, int whichThread)
{
	double d0 = 1e-3, d1;
	int N = 900; int settle = 800; //used to be 20. 1000/600 causes moire patterns
	double total = 0.0;
	double sx = 0.0;double sy=0.00001;
	double x=sx, y=sy, x_;
	for (int i=0; i<N; i++)
	{
		x_ = c1*x - y*y; y= c2*y + x*y;
		x=x_;

		d1 = sqrt( x*x + y*y ); //distance
		if (i>settle) total+= d1;
		if (ISTOOBIG(x) || ISTOOBIG(y)) return 0xffffff;//255<<8; //pure green xxrrggxx
	}
	double valAtZero = total / (N-settle);
	total = 0.0;
	 //sx = -.79296875; sy=0.546875;
	 sx = -.79296875; sy=0.546875;
	 x=sx, y=sy, x_;
	for (int i=0; i<N; i++)
	{
		x_ = c1*x - y*y; y= c2*y + x*y;
		x=x_;

		d1 = sqrt( x*x + y*y ); //distance
		if (i>settle) total+= d1;
		if (ISTOOBIG(x) || ISTOOBIG(y)) return 0xffffff;//255<<8; //pure green xxrrggxx
	}
	double valAtOther = total / (N-settle);
	double v = abs(valAtZero - valAtOther);
	return (v>0.15)?0xff:0xffffff; //standardToColors(v, 3);
}

 double getDetOfJacobian(double a, double b, double x,double y)
{
	double j00=a, j01=-2*y, j10=y, j11=x+b;
	return j00*j11 - j10*j01; //a*(x+b) - -2*y*y, ax+ab+2yy
}
int countPhasePlotHowMuchTransience(MenagFastSettings*settings,double c1, double c2, int whichThread)
{
	int sm = alternateCountPhasePlotSSEGetCountForTransience(settings,c1,c2,whichThread, 1000);
	int bg = alternateCountPhasePlotSSEGetCountForTransience(settings,c1,c2,whichThread, 25);
	double val = (double)sm-bg;
	if (val<=0) return 0;
	else return standardToColors(val, (40.0));
	//int bg = alternateCountPhasePlotSSEGetCountForTransience(settings,c1,c2,whichThread, 25);
	//return standardToColors((double)bg, (DRAWING));
}




