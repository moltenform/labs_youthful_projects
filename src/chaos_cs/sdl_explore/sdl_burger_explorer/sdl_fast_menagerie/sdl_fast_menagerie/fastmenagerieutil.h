
#ifdef FASTMENAGERIEUTIL_H
#error only include this file once.
#endif
#define FASTMENAGERIEUTIL_H

typedef struct { MenagFastSettings*settings; int whichHalf; } ThreadStructure;
ThreadStructure threadStruct1 = {NULL, 0};
ThreadStructure threadStruct2 = {NULL, 1};
int * arrayOfResults=NULL;
SDL_PixelFormat * g_pixelFormat = NULL;

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
			localarrayOfResults[py*MenagWidth + px] = alternateCountPhasePlotSSE(settings, fx,fy, whichHalf);

			fx += dx;
		}
		fy -= dy;
	}

	if (whichHalf) g_BusyThread2 = FALSE;
	else g_BusyThread1 = FALSE;
	return 0;
}


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

void constructMenagerieSurface(MenagFastSettings*ps, SDL_Surface* pSurface)
{
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