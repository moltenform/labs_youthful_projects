
#ifdef FASTMENAGERIEUTIL_H
#error only include this file once.
#endif
#define FASTMENAGERIEUTIL_H

typedef struct { MenagFastSettings*settings; int whichHalf; } ThreadStructure;
ThreadStructure threadStruct1 = {NULL, 0};
ThreadStructure threadStruct2 = {NULL, 1};
int * arrayOfResults=NULL;
SDL_PixelFormat * g_pixelFormat = NULL;


int CalcFastFastMenagerie(void* data);

int * arrThread1 = NULL;
int * arrThread2 = NULL;
#define PHASEW 256
#define PHASEH 256
#define PHASESIZE 256
int CURRENTID1=35,CURRENTID2=35; //note that this should be threadsafe
void startMenagCalculation(MenagFastSettings*ps, int direction, SDL_PixelFormat * pixelFormat)
{
	g_pixelFormat = pixelFormat;
	g_BusyThread1 = g_BusyThread2 = TRUE;
	if (arrayOfResults==NULL) arrayOfResults = (int*) malloc(sizeof(int)*MenagHeight*MenagWidth);
	if (arrThread1==NULL) arrThread1 = (int*)malloc(sizeof(int)*PHASEW * PHASEH);
	if (arrThread2==NULL) arrThread2= (int*)malloc(sizeof(int)*PHASEW * PHASEH);

	//direction is 1: zooming in, -1: zooming out, 0: not a zoom.
	threadStruct1.settings = ps; 
	threadStruct2.settings = ps; 
	SDL_Thread *thread1 = SDL_CreateThread(CalcFastFastMenagerie, &threadStruct1);
	SDL_Thread *thread2 =  SDL_CreateThread(CalcFastFastMenagerie, &threadStruct2);
	//SDL_WaitThread(thread1, NULL);
}
SDL_Surface* g_surface;

void constructMenagerieSurface(MenagFastSettings*ps, SDL_Surface* pSurface)
{
	g_surface = pSurface;
	//SDL_FillRect ( pSmallSurface , NULL , rand() ); 
	if (SDL_MUSTLOCK(pSurface)) SDL_LockSurface ( pSurface ) ;
	int height=MenagHeight; int width=MenagWidth;
	
    for (int py=0; py<height; py++) {
	 for (int px = 0; px < width; px++)
    {
        int newcol = arrayOfResults[py*MenagWidth + px];
   char* pPosition = ( char* ) pSurface->pixels ; //determine position
  pPosition += ( pSurface->pitch * (py) ); //offset by y
  pPosition += ( pSurface->format->BytesPerPixel * (px) ); //offset by x
  memcpy ( pPosition , &newcol , pSurface->format->BytesPerPixel ) ;
        }
    }
	for (int px=MenagWidth; px<MenagColorLegend+width; px++)
    for (int py=0; py<height; py++) {
		int color = standardToColors( (double)py, height);
 char* pPosition = ( char* ) pSurface->pixels ; //determine position
  pPosition += ( pSurface->pitch * (height-py-1) ); //offset by y
  pPosition += ( pSurface->format->BytesPerPixel * (px) ); //offset by x
  memcpy ( pPosition , &color , pSurface->format->BytesPerPixel ) ;
	}
	if (SDL_MUSTLOCK(pSurface)) SDL_UnlockSurface ( pSurface ) ;
}

int alternateCountPhasePlot(MenagFastSettings*settings,double c1, double c2, int whichThread)
{
	int CURRENTID =  whichThread? ++CURRENTID1 : ++CURRENTID2;
	int*arr = whichThread? arrThread1:arrThread2;
	double x,x_,y;
	x=0.0; y=0.000001; //experimental. it's true.
	int counted=0;
	double X0=settings->menagphasex0, X1=settings->menagphasex1, Y0=settings->menagphasey0, Y1=settings->menagphasey1;
	BOOL hasbeennegative=FALSE;
	BOOL hasbeenpos=FALSE;
	int rightmost=0, leftmost=PHASEW+2;
	int biggesty=0, smallesty=PHASEH+2;

	for (int i=0; i<50/4; i++)
	{
		x_ = c1*x - y*y; y= c2*y + x*y; x=x_;
		x_ = c1*x - y*y; y= c2*y + x*y; x=x_;
		x_ = c1*x - y*y; y= c2*y + x*y; x=x_;
		x_ = c1*x - y*y; y= c2*y + x*y; x=x_;
		if (ISTOOBIG(x) || ISTOOBIG(y))
		{counted=0; goto theend;} //counted;
	}
	//drawing time.
	for (int i=0; i<3000/2; i++) //see how changes if drawing increases?
	{
		x_ = c1*x - y*y; y= c2*y + x*y; x=x_;
		int px = lrint(PHASEW * ((x - X0) / (X1 - X0)));
		int py_times_256 = lrint(PHASEW*PHASEH * ((y - Y0) / (Y1 - Y0)));
		if (py_times_256 >= 0 && py_times_256 < PHASEH*PHASEW && px>=0 && px<PHASEW)
		    if (arr[px + py_times_256 ]!=CURRENTID)
		    { arr[px + py_times_256 ]=CURRENTID; counted++;}

		if (px>rightmost) rightmost=px;
		if (px<leftmost) leftmost=px;
		int py = py_times_256/256;
		if (py<smallesty) smallesty=py;
		if (py>biggesty) biggesty=py;

		x_ = c1*x - y*y; y= c2*y + x*y; x=x_;
		px = lrint(PHASEW * ((x - X0) / (X1 - X0)));
		py_times_256 = lrint(PHASEW*PHASEH * ((y - Y0) / (Y1 - Y0)));
		if (py_times_256 >= 0 && py_times_256 < PHASEH*PHASEW && px>=0 && px<PHASEW)
		    if (arr[px + py_times_256 ]!=CURRENTID)
		    { arr[px + py_times_256 ]=CURRENTID; counted++;}

		if (ISTOOBIG(x) || ISTOOBIG(y))
			{counted=0; goto theend;} //counted;
		//if (y<0) hasbeennegative=TRUE;
		//if (y>0) hasbeenpos=TRUE;
		if (px>rightmost) rightmost=px;
		if (px<leftmost) leftmost=px;
		 py = py_times_256/256;
		if (py<smallesty) smallesty=py;
		if (py>biggesty) biggesty=py;
	}
	/*if (hasbeenpos&&hasbeennegative) return 200;
	if (hasbeennegative) return 500;
	if (hasbeenpos) return 300;*/
theend:
	if (counted==0) return 0x0;
	else
	return standardToColors((double)counted, 3000);
	//return 100;
	//return hasbeennegative?500:200;
	//return counted;
}
/* showing width--it just happened to show some periodic ones, because period was missing that maximum.*/

void BlitMenagerie(SDL_Surface* pSurface,SDL_Surface* pSmallSurface)
{
	SDL_Rect dest;
	dest.x = 0;
	dest.y = 0;
	dest.w = MenagWidth + MenagColorLegend;
	dest.h = MenagHeight; 
	SDL_BlitSurface(pSmallSurface, NULL, pSurface, &dest);
}


int alternateCountPhasePlotSSEISPOS(MenagFastSettings*settings,double c1, double c2, int whichThread)
{
	int CURRENTID =  whichThread? ++CURRENTID1 : ++CURRENTID2;
	int*arr = whichThread? arrThread1:arrThread2;
	int counted=0;
	///double X0=settings->menagphasex0, X1=settings->menagphasex1, Y0=settings->menagphasey0, Y1=settings->menagphasey1;
	double X0=-3, X1=1, Y0=-3, Y1=3;

	BOOL bHasBeenNeg=FALSE;
	__m128 mmX = _mm_setr_ps( 0.0f, 0.0f, 0.0f, 0.0f);
	__m128 mmY = _mm_setr_ps( 0.000001f, 0.000002f,-0.0000011f,-0.0000019f); //symmetrical, so don't just mult by 2.
	__m128 mXTmp;
	burgerSetup;

	for (int i=0; i<100/4; i++)
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
	for (int i=0; i<500/8; i++) //see how changes if drawing increases?
	{
		burgerExpression;
		if (mmY.m128_f32[0] <0 )bHasBeenNeg=TRUE;
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
	if (counted==0) return 0x0;
	else if (bHasBeenNeg) return  standardToColors((double)400, 1000);
	else  return  standardToColors((double)800, 1000);
}



///////////////////
#define BIGGERPHASESIZE PHASESIZE
#define SMALLERPHASESIZE PHASESIZE/4

int alternateCountPhasePlotSSEGetDimensionSMALLER(MenagFastSettings*settings,double c1, double c2, int whichThread)
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
	__m128 mMultW = _mm_set1_ps(CW * SMALLERPHASESIZE * SMALLERPHASESIZE);  //EXTRA FACTOR
	int ShiftW = (int) (-X0 * SMALLERPHASESIZE * CW * SMALLERPHASESIZE); //if -3 to 3, this shifts by +128 (half the arrWidth). 
									//This is sometimes an approximation, but ok due to speed
	float CH = 1.0f/(Y1-Y0);
	__m128 mMultH = _mm_set1_ps(CH * SMALLERPHASESIZE ); 
	int ShiftH = (int) (-Y0 * SMALLERPHASESIZE * CH );
	__m128 xPrelimTimes256, yPrelim;
	__m128i mmShiftW = _mm_set1_epi32(ShiftW);
	__m128i mmShiftH = _mm_set1_epi32(ShiftH);
	for (int i=0; i<DRAWING/4; i++) //see how changes if drawing increases?
	{
		burgerExpression;
		xPrelimTimes256 = _mm_mul_ps(mmX, mMultW);
		yPrelim = _mm_mul_ps(mmY, mMultH);
		
		__m128i xPt256 = _mm_cvttps_epi32(xPrelimTimes256); //cast all to int at once. truncate mode.
		__m128i yPt = _mm_cvttps_epi32(yPrelim); 
		xPt256 = _mm_add_epi32(xPt256, mmShiftW);
		yPt = _mm_add_epi32(yPt, mmShiftH);
		__m128i xySum = _mm_add_epi32(xPt256, yPt); //this is worth doing, even though we don't always use it.
		
		if (yPt.m128i_i32[0] >= 0 && yPt.m128i_i32[0] < SMALLERPHASESIZE && xPt256.m128i_i32[0]>=0 && xPt256.m128i_i32[0]<(SMALLERPHASESIZE* (SMALLERPHASESIZE-1))) { 
			if (arr[xySum.m128i_i32[0]]!=CURRENTID)
		    { arr[xySum.m128i_i32[0]]=CURRENTID; counted++;}
		}
		if (yPt.m128i_i32[1] >= 0 && yPt.m128i_i32[1] < SMALLERPHASESIZE && xPt256.m128i_i32[1]>=0 && xPt256.m128i_i32[1]<(SMALLERPHASESIZE* (SMALLERPHASESIZE-1))) { 
			if (arr[xySum.m128i_i32[1]]!=CURRENTID)
		    { arr[xySum.m128i_i32[1]]=CURRENTID; counted++;}
		}
		if (yPt.m128i_i32[2] >= 0 && yPt.m128i_i32[2] < SMALLERPHASESIZE && xPt256.m128i_i32[2]>=0 && xPt256.m128i_i32[2]<(SMALLERPHASESIZE* (SMALLERPHASESIZE-1))) { 
			if (arr[xySum.m128i_i32[2]]!=CURRENTID)
		    { arr[xySum.m128i_i32[2]]=CURRENTID; counted++;}
		}
		if (yPt.m128i_i32[3] >= 0 && yPt.m128i_i32[3] < SMALLERPHASESIZE && xPt256.m128i_i32[3]>=0 && xPt256.m128i_i32[3]<(SMALLERPHASESIZE* (SMALLERPHASESIZE-1))) { 
			if (arr[xySum.m128i_i32[3]]!=CURRENTID)
		    { arr[xySum.m128i_i32[3]]=CURRENTID; counted++;}
		}
	}
	
theend:
	if (counted==0) return 0x0;
	else return counted;//return standardToColors((double)counted, DRAWING);
}
int alternateCountPhasePlotSSEGetDimensionBIGGER(MenagFastSettings*settings,double c1, double c2, int whichThread)
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
	__m128 mMultW = _mm_set1_ps(CW * BIGGERPHASESIZE * BIGGERPHASESIZE);  //EXTRA FACTOR
	int ShiftW = (int) (-X0 * BIGGERPHASESIZE * CW * BIGGERPHASESIZE); //if -3 to 3, this shifts by +128 (half the arrWidth). 
									//This is sometimes an approximation, but ok due to speed
	float CH = 1.0f/(Y1-Y0);
	__m128 mMultH = _mm_set1_ps(CH * BIGGERPHASESIZE ); 
	int ShiftH = (int) (-Y0 * BIGGERPHASESIZE * CH );
	__m128 xPrelimTimes256, yPrelim;
	__m128i mmShiftW = _mm_set1_epi32(ShiftW);
	__m128i mmShiftH = _mm_set1_epi32(ShiftH);
	for (int i=0; i<DRAWING/4; i++) //see how changes if drawing increases?
	{
		burgerExpression;
		xPrelimTimes256 = _mm_mul_ps(mmX, mMultW);
		yPrelim = _mm_mul_ps(mmY, mMultH);
		
		__m128i xPt256 = _mm_cvttps_epi32(xPrelimTimes256); //cast all to int at once. truncate mode.
		__m128i yPt = _mm_cvttps_epi32(yPrelim); 
		xPt256 = _mm_add_epi32(xPt256, mmShiftW);
		yPt = _mm_add_epi32(yPt, mmShiftH);
		__m128i xySum = _mm_add_epi32(xPt256, yPt); //this is worth doing, even though we don't always use it.
		
		if (yPt.m128i_i32[0] >= 0 && yPt.m128i_i32[0] < BIGGERPHASESIZE && xPt256.m128i_i32[0]>=0 && xPt256.m128i_i32[0]<(BIGGERPHASESIZE* (BIGGERPHASESIZE-1))) { 
			if (arr[xySum.m128i_i32[0]]!=CURRENTID)
		    { arr[xySum.m128i_i32[0]]=CURRENTID; counted++;}
		}
		if (yPt.m128i_i32[1] >= 0 && yPt.m128i_i32[1] < BIGGERPHASESIZE && xPt256.m128i_i32[1]>=0 && xPt256.m128i_i32[1]<(BIGGERPHASESIZE* (BIGGERPHASESIZE-1))) { 
			if (arr[xySum.m128i_i32[1]]!=CURRENTID)
		    { arr[xySum.m128i_i32[1]]=CURRENTID; counted++;}
		}
		if (yPt.m128i_i32[2] >= 0 && yPt.m128i_i32[2] < BIGGERPHASESIZE && xPt256.m128i_i32[2]>=0 && xPt256.m128i_i32[2]<(BIGGERPHASESIZE* (BIGGERPHASESIZE-1))) { 
			if (arr[xySum.m128i_i32[2]]!=CURRENTID)
		    { arr[xySum.m128i_i32[2]]=CURRENTID; counted++;}
		}
		if (yPt.m128i_i32[3] >= 0 && yPt.m128i_i32[3] < BIGGERPHASESIZE && xPt256.m128i_i32[3]>=0 && xPt256.m128i_i32[3]<(BIGGERPHASESIZE* (BIGGERPHASESIZE-1))) { 
			if (arr[xySum.m128i_i32[3]]!=CURRENTID)
		    { arr[xySum.m128i_i32[3]]=CURRENTID; counted++;}
		}
	}
	
theend:
	if (counted==0) return 0x0;
	else return counted; //return standardToColors((double)counted, DRAWING);
}
int alternateCountPhasePlotSSEGetCountForTransience(MenagFastSettings*settings,double c1, double c2, int whichThread, int TheSettling)
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

	for (int i=0; i<TheSettling/4; i++)
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
	__m128 mMultW = _mm_set1_ps(CW * BIGGERPHASESIZE * BIGGERPHASESIZE);  //EXTRA FACTOR
	int ShiftW = (int) (-X0 * BIGGERPHASESIZE * CW * BIGGERPHASESIZE); //if -3 to 3, this shifts by +128 (half the arrWidth). 
									//This is sometimes an approximation, but ok due to speed
	float CH = 1.0f/(Y1-Y0);
	__m128 mMultH = _mm_set1_ps(CH * BIGGERPHASESIZE ); 
	int ShiftH = (int) (-Y0 * BIGGERPHASESIZE * CH );
	__m128 xPrelimTimes256, yPrelim;
	__m128i mmShiftW = _mm_set1_epi32(ShiftW);
	__m128i mmShiftH = _mm_set1_epi32(ShiftH);
	for (int i=0; i<DRAWING/4; i++) //see how changes if drawing increases?
	{
		burgerExpression;
		xPrelimTimes256 = _mm_mul_ps(mmX, mMultW);
		yPrelim = _mm_mul_ps(mmY, mMultH);
		
		__m128i xPt256 = _mm_cvttps_epi32(xPrelimTimes256); //cast all to int at once. truncate mode.
		__m128i yPt = _mm_cvttps_epi32(yPrelim); 
		xPt256 = _mm_add_epi32(xPt256, mmShiftW);
		yPt = _mm_add_epi32(yPt, mmShiftH);
		__m128i xySum = _mm_add_epi32(xPt256, yPt); //this is worth doing, even though we don't always use it.
		
		if (yPt.m128i_i32[0] >= 0 && yPt.m128i_i32[0] < BIGGERPHASESIZE && xPt256.m128i_i32[0]>=0 && xPt256.m128i_i32[0]<(BIGGERPHASESIZE* (BIGGERPHASESIZE-1))) { 
			if (arr[xySum.m128i_i32[0]]!=CURRENTID)
		    { arr[xySum.m128i_i32[0]]=CURRENTID; counted++;}
		}
		if (yPt.m128i_i32[1] >= 0 && yPt.m128i_i32[1] < BIGGERPHASESIZE && xPt256.m128i_i32[1]>=0 && xPt256.m128i_i32[1]<(BIGGERPHASESIZE* (BIGGERPHASESIZE-1))) { 
			if (arr[xySum.m128i_i32[1]]!=CURRENTID)
		    { arr[xySum.m128i_i32[1]]=CURRENTID; counted++;}
		}
		if (yPt.m128i_i32[2] >= 0 && yPt.m128i_i32[2] < BIGGERPHASESIZE && xPt256.m128i_i32[2]>=0 && xPt256.m128i_i32[2]<(BIGGERPHASESIZE* (BIGGERPHASESIZE-1))) { 
			if (arr[xySum.m128i_i32[2]]!=CURRENTID)
		    { arr[xySum.m128i_i32[2]]=CURRENTID; counted++;}
		}
		if (yPt.m128i_i32[3] >= 0 && yPt.m128i_i32[3] < BIGGERPHASESIZE && xPt256.m128i_i32[3]>=0 && xPt256.m128i_i32[3]<(BIGGERPHASESIZE* (BIGGERPHASESIZE-1))) { 
			if (arr[xySum.m128i_i32[3]]!=CURRENTID)
		    { arr[xySum.m128i_i32[3]]=CURRENTID; counted++;}
		}
	}
	
theend:
	if (counted==0) return 0x0;
	else return counted; //return standardToColors((double)counted, DRAWING);
}

int alternateCountPhasePlotSSEGetDimension(MenagFastSettings*settings,double c1, double c2, int whichThread)
{
	int smaller=alternateCountPhasePlotSSEGetDimensionSMALLER(settings,c1,c2,whichThread);
	int bigger=alternateCountPhasePlotSSEGetDimensionBIGGER(settings,c1,c2,whichThread);
	if (smaller==0) return 0; //diverges
	double dimEstimate = (double)bigger/(double)smaller    ;
	//we've increased height/width by 4. so 2d inc by 16, 1d inc by 4, 0d inc by 1
	return standardToColors(dimEstimate, 4.1);
}
