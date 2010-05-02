
#include "common.h"
#include "phaseportrait.h"
#include "fastmenagerie.h"
#include "float_cast.h"

int alternateCountPhasePlot(MenagFastSettings*settings,double c1, double c2, int whichThread);
BOOL g_BusyThread1 = FALSE;
BOOL g_BusyThread2 = FALSE;
void InitialSettings(MenagFastSettings*ps, int width, int height, double *pa, double *pb)
{
	ps->menagSeedsPerAxis = 40;
	ps->menagSettling = 48;
	ps->menagDrawing = 20; //also, # of iters for the Basins mode.
	ps->browsex0 = 0; ps->browsex1 = 1; ps->browsey0=0; ps->browsey1 = 1;
	ps->x0 = 0; ps->x1 = 1; ps->y0=0; ps->y1 = 1;
	
	if (StringsEqual(STRINGIFY(MAPEXPRESSION), STRINGIFY(BURGER)))
	{
		*pa = -1.1; *pb = 1.72;
		ps->browsex0 = -2; ps->browsex1 = 2; ps->browsey0=-0.5; ps->browsey1 = 3.5;
		ps->x0 = -1.75; ps->x1 = 1.75; ps->y0=-1.75; ps->y1 = 1.75;
		ps->seedx0 = -3; ps->seedx1 = 1; ps->seedy0=0 /*it's symmetrical */; ps->seedy1 = 3;
	}
	else //HENON MAP
	{
		*pa = 1.4; *pb = 0.3;
		ps->browsex0 = -2; ps->browsex1 = 2; ps->browsey0=-1; ps->browsey1 = 3;
		ps->x0 = -1.75; ps->x1 = 1.75; ps->y0=-1.75; ps->y1 = 1.75;
		ps->seedx0 = -3; ps->seedx1 = 3; ps->seedy0=-3 ; ps->seedy1 = 3;
		
	}

}


typedef struct { MenagFastSettings*settings; int whichHalf; } ThreadStructure;
ThreadStructure threadStruct1 = {NULL, 0};
ThreadStructure threadStruct2 = {NULL, 1};
int * arrayOfResults=NULL;



int CalcFastFastMenagerie(void* data)
{
	MenagFastSettings*settings = ((ThreadStructure*)data)->settings;
	int whichHalf = ((ThreadStructure*)data)->whichHalf;
	int * localarrayOfResults = (whichHalf)? arrayOfResults : arrayOfResults + (MenagHeight/2)*MenagWidth;
	double X0=settings->x0, X1=settings->x1;
	double Y0 = (whichHalf)? settings->browsey0 : (settings->browsey0+settings->browsey1)/2;
	double Y1 = (whichHalf)?(settings->browsey0+settings->browsey1)/2 : settings->browsey1;
	
	double dx = (X1 - X0) / MenagWidth, dy = (Y1 - Y0) / MenagHeight;
	double fx = X0, fy = Y1; //y counts downwards
	
	for (int py=0; py<MenagHeight/2; py+=1) //Note the /2!
	{
		fx=X0;
		for (int px = 0; px < MenagWidth; px+=1)
		{
			//int ret = alternateCountPhasePlot(settings, fx,fy, whichHalf);
			//double dd = ret/100.0;
			//if (dd>1.0) dd=1.0; if (dd<0.0) dd=0.0;
			localarrayOfResults[py*MenagWidth + px] = (whichHalf)? 34534 : 94534; //lrint(256*dd);
	//arrayOfResults[py*MenagWidth + px] = (int) ((fx+fy)*4000);

		fx += dx;
		}
		fy -= dy;
	}

	//SDL_Delay(400);

	if (whichHalf)
		g_BusyThread2 = FALSE;
	else
		g_BusyThread1 = FALSE;
	return 0;
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
	if (SDL_MUSTLOCK(pSurface)) SDL_UnlockSurface ( pSurface ) ;
}

int * arrThread1 = NULL;
int * arrThread2 = NULL;
#define PHASEW 256
#define PHASEH 256
void startMenagCalculation(MenagFastSettings*ps, int direction)
{
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


void BlitMenagerie(SDL_Surface* pSurface,SDL_Surface* pSmallSurface)
{
	SDL_Rect src, dest;
	src.x = 0;
	src.y = 0;
	src.w = MenagWidth;
	src.h = MenagHeight;
	dest.x = 0;
	dest.y = 0;
	dest.w = MenagWidth;
	dest.h = MenagHeight; 
	SDL_BlitSurface(pSmallSurface, &src, pSurface, &dest);
}



int CURRENTID1=35,CURRENTID2=35; //note that this should be threadsafe
int alternateCountPhasePlot(MenagFastSettings*settings,double c1, double c2, int whichThread)
{
	int CURRENTID =  whichThread? ++CURRENTID1 : ++CURRENTID2;
	int*arr = whichThread? arrThread1:arrThread2;
	double x,x_,y;
	x=0.0; y=0.0; //experimental. it's true.
	int counted=0;
	double X0=-3, X1=1, Y0=-3, Y1=3; ////////////////////////////////////////////////
	
	for (int i=0; i<100; i++)
	{
		x_ = c1*x - y*y; y= c2*y + x*y; x=x_;
		if (ISTOOBIG(x) || ISTOOBIG(y))
			return counted;
	}
	//drawing time.
	for (int i=0; i<1000; i++) //see how changes if drawing increases?
	{
		x_ = c1*x - y*y; y= c2*y + x*y; x=x_;
		int px = lrint(PHASEW * ((x - X0) / (X1 - X0)));
		int py = lrint(PHASEH * ((y - Y0) / (Y1 - Y0)));
		if (py >= 0 && py < PHASEH && px>=0 && px<PHASEW)
		    if (arr[py + px * PHASEH]!=CURRENTID)
		    { arr[py + px * PHASEH]=CURRENTID; counted++;}
	}

	return counted;
}

