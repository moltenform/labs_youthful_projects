#include "phaseportrait.h"
#include "float_cast.h"
#include "whichmap.h"
#include "palette.h"
#include "coordsdiagram.h" //maybe not the best architecturally

//note that these functions don't use the a and b from g_settings.
//a "seed point" is an initial point. In some maps, choice of (x0,y0) changes behavior.
void DrawPhasePortraitStyle( SDL_Surface* pSurface, double c1, double c2, int width, int xstart ) 
{
	int SETTLE, DRAWING, nXpoints, nYpoints; double sx0,sx1,sy0,sy1;
	if (g_settings->drawingMode == DrawModeStandardPhase) {
		SETTLE = 220, DRAWING = 80;
		double sx0i= g_settings->sx, sx1i=g_settings->sx2, sy0i= g_settings->sy, sy1i=g_settings->sy2;
		//keep higher one.
		sx0=MIN(sx0i,sx1i), sx1=MAX(sx0i,sx1i),sy0=MIN(sy0i,sy1i), sy1=MAX(sy0i,sy1i);
		nXpoints=40; nYpoints=40;
	} else if (g_settings->drawingMode == DrawModeLeavesPhase) {
		SETTLE = 10, DRAWING = 22;
		double sx0i= g_settings->sx, sx1i=g_settings->sx2, sy0i= g_settings->sy, sy1i=g_settings->sy2;
		//keep higher one.
		sx0=MIN(sx0i,sx1i), sx1=MAX(sx0i,sx1i),sy0=MIN(sy0i,sy1i), sy1=MAX(sy0i,sy1i);
		nXpoints= 80; nYpoints= 80;
	}

	int height=width;
	double sxinc = (nXpoints==1 || sx1-sx0==0) ? 1e6 : (sx1-sx0)/(nXpoints-1);
	double syinc = (nYpoints==1 || sy1-sy0==0) ? 1e6 : (sy1-sy0)/(nYpoints-1);

	double x_,y_,x,y;
	double X0=g_settings->x0, X1=g_settings->x1, Y0=g_settings->y0, Y1=g_settings->y1;

	for (double sx=sx0; sx<=sx1; sx+=sxinc)
    {
                for (double sy=sy0; sy<=sy1; sy+=syinc)
                {
                    x = sx; y=sy;

					for (int ii=0; ii<SETTLE/2; ii++)
                    {
						MAPEXPRESSION; x=x_; y=y_;
						MAPEXPRESSION; x=x_; y=y_;
						if (ISTOOBIG(x)||ISTOOBIG(y)) break;
                    }
					for (int ii=0; ii<DRAWING; ii++)
                    {
						MAPEXPRESSION; x=x_; y=y_;
						if (ISTOOBIG(x)||ISTOOBIG(y)) break;

                        int px = lrint(width * ((x - X0) / (X1 - X0)));
                        int py = lrint(height - height * ((y - Y0) / (Y1 - Y0)));
                        if (py >= 0 && py < height && px>=0 && px<width)
						{

  Uint32 col = 0 ; Uint32 colR;  
  char* pPosition = ( char* ) pSurface->pixels ; //determine position
  pPosition += ( pSurface->pitch * py ); //offset by y
  pPosition += ( pSurface->format->BytesPerPixel * (px+xstart) ); //offset by x
  memcpy ( &col , pPosition , pSurface->format->BytesPerPixel ) ; //copy pixel data

  SDL_Color color ;
  SDL_GetRGB ( col , pSurface->format , &color.r , &color.g , &color.b ) ;
  colR = color.r;
						Uint32 newcolor = (colR)-((colR)>>2); //add shade to that pixel

  Uint32 newcol = SDL_MapRGB ( pSurface->format , newcolor , newcolor , newcolor ) ;
  memcpy ( pPosition , &newcol , pSurface->format->BytesPerPixel ) ;
				}
            }
        }
    }
}

#define pickrandom {if (rand()&(0x1<<1)) c1=a_one,c2=b_one; else c1=a_two,c2=b_two;}
//alternating is like the 2nd return map with different params. so the result is clearly another 2d discrete map that could be found.
//alternate between 3? any more interesting?
void DrawPhasePortraitAlternate( SDL_Surface* pSurface, double a_one, double b_one, int width, int xstart  ) 
{
	srand(1334);
	double a_two = g_settings->a2, b_two = g_settings->b2 ,c1,c2;
	int SETTLE = 220, DRAWING = 80;
	double sx0i= g_settings->sx, sx1i=g_settings->sx2, sy0i= g_settings->sy, sy1i=g_settings->sy2;
		//keep higher one.
	double sx0=MIN(sx0i,sx1i), sx1=MAX(sx0i,sx1i),sy0=MIN(sy0i,sy1i), sy1=MAX(sy0i,sy1i);
	int nXpoints=40; int nYpoints=40;
	int height=width;
	double sxinc = (nXpoints==1) ? 1e6 : (sx1-sx0)/(nXpoints-1);
	double syinc = (nYpoints==1) ? 1e6 : (sy1-sy0)/(nYpoints-1);

	double x_,y_,x,y; Uint32 col = 0 ; Uint32 colR;    SDL_Color color ;
	double X0=g_settings->x0, X1=g_settings->x1, Y0=g_settings->y0, Y1=g_settings->y1;

	for (double sx=sx0; sx<=sx1; sx+=sxinc)
    {
        for (double sy=sy0; sy<=sy1; sy+=syinc)
        {
            x = sx; y=sy;
			
			if (g_settings->drawingMode==DrawModeAlternatePhase) {
				for (int ii=0; ii<SETTLE/4; ii++)
				{
					c1=a_one;c2=b_one; MAPEXPRESSION; x=x_; y=y_;  c1=a_two;c2=b_two; MAPEXPRESSION; x=x_; y=y_;
					c1=a_one;c2=b_one; MAPEXPRESSION; x=x_; y=y_;  c1=a_two;c2=b_two; MAPEXPRESSION; x=x_; y=y_;
					if (ISTOOBIG(x)||ISTOOBIG(y)) break;
				}
			} else if (g_settings->drawingMode==DrawModeRandomPhase) {
				for (int ii=0; ii<SETTLE/4; ii++)
				{
					pickrandom; MAPEXPRESSION; x=x_; y=y_;  pickrandom; MAPEXPRESSION; x=x_; y=y_;
					pickrandom; MAPEXPRESSION; x=x_; y=y_;  pickrandom; MAPEXPRESSION; x=x_; y=y_;
					if (ISTOOBIG(x)||ISTOOBIG(y)) break;
				}
			}
			for (int ii=0; ii<DRAWING; ii++)
            {
				c1=a_one;c2=b_one;
				if (g_settings->drawingMode==DrawModeRandomPhase) pickrandom;
				MAPEXPRESSION; x=x_; y=y_;
				if (ISTOOBIG(x)||ISTOOBIG(y)) break;

                int px = lrint(width * ((x - X0) / (X1 - X0)));
                int py = lrint(height - height * ((y - Y0) / (Y1 - Y0)));
                if (py >= 0 && py < height && px>=0 && px<width)
				{
  char* pPosition = ( char* ) pSurface->pixels ; //determine position
  pPosition += ( pSurface->pitch * py ); //offset by y
  pPosition += ( pSurface->format->BytesPerPixel * (px+xstart) ); //offset by x
  memcpy ( &col , pPosition , pSurface->format->BytesPerPixel ) ; //copy pixel data
  SDL_GetRGB ( col , pSurface->format , &color.r , &color.g , &color.b ) ;
  colR = color.r;
						Uint32 newcolor = (colR)-((colR)>>2); //add shade to that pixel
  Uint32 newcol = SDL_MapRGB ( pSurface->format , newcolor , newcolor , newcolor ) ;
  memcpy ( pPosition , &newcol , pSurface->format->BytesPerPixel ) ;
						}
				
				c1=a_two;c2=b_two;
				if (g_settings->drawingMode==DrawModeRandomPhase) pickrandom;
				MAPEXPRESSION; x=x_; y=y_;
				if (ISTOOBIG(x)||ISTOOBIG(y)) break;

                px = lrint(width * ((x - X0) / (X1 - X0)));
                py = lrint(height - height * ((y - Y0) / (Y1 - Y0)));
                if (py >= 0 && py < height && px>=0 && px<width)
				{
  char* pPosition = ( char* ) pSurface->pixels ; //determine position
  pPosition += ( pSurface->pitch * py ); //offset by y
  pPosition += ( pSurface->format->BytesPerPixel * (px+xstart) ); //offset by x
  memcpy ( &col , pPosition , pSurface->format->BytesPerPixel ) ; //copy pixel data
  SDL_GetRGB ( col , pSurface->format , &color.r , &color.g , &color.b ) ;
  colR = color.r;
						Uint32 newcolor = (colR)-((colR)>>2); //add shade to that pixel
  Uint32 newcol = SDL_MapRGB ( pSurface->format , newcolor , newcolor , newcolor ) ;
  memcpy ( pPosition , &newcol , pSurface->format->BytesPerPixel ) ;
				}
			}
        }
    }
}

void DrawPhasePortraitIntoArr( double c1, double c2, int width, int*arr ) 
{
	int SETTLE, DRAWING, nXpoints, nYpoints; double sx0,sx1,sy0,sy1;
	if (g_settings->drawingMode==DrawModeSmearLine) SETTLE = 80, DRAWING = 100; //SETTLE = 100, DRAWING = 200;
	else if (g_settings->drawingMode==DrawModeSmearRectangle) SETTLE = 80, DRAWING = 25000;
		double sx0i= g_settings->sx, sx1i=g_settings->sx2, sy0i= g_settings->sy, sy1i=g_settings->sy2;
		//keep higher one.
		sx0=MIN(sx0i,sx1i), sx1=MAX(sx0i,sx1i),sy0=MIN(sy0i,sy1i), sy1=MAX(sy0i,sy1i);
		nXpoints=4; nYpoints=4;
//we could use a bit mask to force it only to contribute 1 (say reserve top bit to indicate that)
	int height=width;
	double sxinc = (nXpoints==1 || sx1-sx0==0) ? 1e6 : (sx1-sx0)/(nXpoints-1);
	double syinc = (nYpoints==1 || sy1-sy0==0) ? 1e6 : (sy1-sy0)/(nYpoints-1);

	double x_,y_,x,y;
	double X0=g_settings->x0, X1=g_settings->x1, Y0=g_settings->y0, Y1=g_settings->y1;

	// if basin of attraction is smaller, will be fainter, but that's ok.
	for (double sx=sx0; sx<=sx1; sx+=sxinc)
    {
                for (double sy=sy0; sy<=sy1; sy+=syinc)
                {
                    x = sx; y=sy;

					for (int ii=0; ii<SETTLE/4; ii++)
                    {
						MAPEXPRESSION; x=x_; y=y_; MAPEXPRESSION; x=x_; y=y_;
						MAPEXPRESSION; x=x_; y=y_; MAPEXPRESSION; x=x_; y=y_;
						if (ISTOOBIG(x)||ISTOOBIG(y)) break;
                    }
					for (int ii=0; ii<DRAWING; ii++)
                    {
						MAPEXPRESSION; x=x_; y=y_;
						if (ISTOOBIG(x)||ISTOOBIG(y)) break;

                        int px = lrint(width * ((x - X0) / (X1 - X0)));
                        int py = lrint(height - height * ((y - Y0) / (Y1 - Y0)));
                        if (py >= 0 && py < height && px>=0 && px<width)
						{
							arr[py*width+px]++;
						}
					}
        }
    }
}
//what would it look like if all params ever were smeared together?
int* SmearArray = NULL;
void DrawSmear( SDL_Surface* pSurface, double a_one, double b_one, int width, int xstart) 
{
	int height=width;
	if (!SmearArray) { SmearArray=(int*)malloc(sizeof(int)*width*height); }
	memset(SmearArray, 0, sizeof(int)*width*height); 
	int SMEARSTEPS = 25; //100;
	//get data
	double a_two= g_settings->a2,b_two= g_settings->b2;
	double a0=MIN(a_one,a_two), a1=MAX(a_one,a_two),b0=MIN(b_one,b_two), b1=MAX(b_one,b_two);
	for (double a=a0; a<a1; a+=(a1-a0)/SMEARSTEPS)
		for (double b=b0; b<b1; b+=(b1-b0)/SMEARSTEPS)
			DrawPhasePortraitIntoArr(a,b,width,SmearArray);
int newcol; 
for (int py=0; py<height; py++)
{
	for (int px = 0; px < width; px++)
	{
		double val = sqrt((double)SmearArray[py*width+px]) / sqrt((double)SMEARSTEPS*15); //each could have been hit many times
		if (val>1.0) newcol= SDL_MapRGB(pSurface->format, 50,0,0);
		else {val += 0.5; if (val>1) val-=1;
		newcol = HSL2RGB(pSurface, val, 0.5, 0.5);
		}

		char * pPosition = ( char* ) pSurface->pixels ; //determine position
		pPosition += ( pSurface->pitch * py ); //offset by y
		pPosition += ( pSurface->format->BytesPerPixel * (px+xstart) ); //offset by x
		memcpy ( pPosition , &newcol , pSurface->format->BytesPerPixel ) ;
	}
}
}
//simply one phaseportrait but draw a long time.
void DrawLongTime( SDL_Surface* pSurface, double a_one, double b_one, int width, int xstart) 
{
	int height=width;
	if (!SmearArray) { SmearArray=(int*)malloc(sizeof(int)*width*height); }
	memset(SmearArray, 0, sizeof(int)*width*height); 
	DrawPhasePortraitIntoArr(a_one,b_one,width,SmearArray);
int newcol; 
for (int py=0; py<height; py++)
{
	for (int px = 0; px < width; px++)
	{
		double val = ((double)SmearArray[py*width+px]) / (150.0); //each could have been hit many times
		if (val>1.0) newcol= SDL_MapRGB(pSurface->format, 50,0,0);
		else {val += 0.5; if (val>1) val-=1;
		newcol = HSL2RGB(pSurface, val, 0.5, 0.5);
		}

		char * pPosition = ( char* ) pSurface->pixels ; //determine position
		pPosition += ( pSurface->pitch * py ); //offset by y
		pPosition += ( pSurface->format->BytesPerPixel * (px+xstart) ); //offset by x
		memcpy ( pPosition , &newcol , pSurface->format->BytesPerPixel ) ;
	}
}
}

//estimate "basins of attraction". Pixel is x0,y0, colored by final value after n iterations.
void DrawBasinsStandard( SDL_Surface* pSurface, double c1, double c2, int width, int xstart) 
{
double fx,fy, x_,y_,x,y; char* pPosition; Uint32 r,g,b, newcol; double val;
int height=width;
double X0=g_settings->x0, X1=g_settings->x1, Y0=g_settings->y0, Y1=g_settings->y1;
double dx = (X1 - X0) / width, dy = (Y1 - Y0) / height;
fx = X0; fy = Y1; //y counts downwards
for (int py=0; py<height; py+=1)
{
	fx=X0;
	for (int px = 0; px < width; px+=1)
	{
		x=fx; y=fy;
		for (int i=0; i<g_settings->basinsTime; i++)
		{
			MAPEXPRESSION;
			x=x_; y=y_;
			if (ISTOOBIG(x)||ISTOOBIG(y)) break;
		}
		
		val = sqrt( (x-fx)*(x-fx)+(y-fy)*(y-fy));
		
		if (ISTOOBIG(x)||ISTOOBIG(y)) { 
			// dark blue to distinguish from black.
			newcol = SDL_MapRGB( pSurface->format , 0 , 0, 35 ) ; 
		}
		else
		{
			if (!(g_settings->drawingOptions & maskOptionsBasinColor)) {
				val = val / g_settings->basinsMaxColor;
				if (val>=1.0)
					newcol = SDL_MapRGB( pSurface->format , 220 , 220, 255 );
				else {
					int v = (int)(val*255);
					newcol = SDL_MapRGB( pSurface->format , v,v,v );
				}
			}else {
				//val += 0.5; if (val>1) val-=1;
				//newcol = HSL2RGB(pSurface, val, 0.5, 0.5);
				val = sqrt(val) / sqrt(g_settings->basinsMaxColor);
				if (val>=1.0) val=1.0; if (val<0.0) val=0.0;
				val=val*2-1;
				if (val<=0)
					b=255, r=g= (Uint32) ((1+val)*255.0);
				else
					r=g=b= (Uint32) ((1-val)*255.0);
				newcol = SDL_MapRGB( pSurface->format , r,g,b );
			}
		}
		
		pPosition = ( char* ) pSurface->pixels ; //determine position
		pPosition += ( pSurface->pitch * py ); //offset by y
		pPosition += ( pSurface->format->BytesPerPixel * (px+xstart) ); //offset by x
		memcpy ( pPosition , &newcol , pSurface->format->BytesPerPixel ) ;

		fx += dx;
	}
fy -= dy;
}
}

void DrawBasinsBasic( SDL_Surface* pSurface, double c1, double c2, CoordsDiagramStruct * diagram) 
{
int width = diagram->screen_width, height=diagram->screen_height, xstart=diagram->screen_x, ystart=diagram->screen_y;
double fx,fy, x_,y_,x,y; char* pPosition; Uint32 newcol;
double X0=*diagram->px0, X1=*diagram->px1, Y0=*diagram->py0, Y1=*diagram->py1;
double dx = (X1 - X0) / width, dy = (Y1 - Y0) / height;
fx = X0; fy = Y1; //y counts downwards
for (int py=0; py<height; py++)
{
	fx=X0;
	for (int px = 0; px < width; px++)
	{
		x=fx; y=fy;
		for (int i=0; i<120/4; i++)
		{
			MAPEXPRESSION; x=x_; y=y_;
			MAPEXPRESSION; x=x_; y=y_;
			MAPEXPRESSION; x=x_; y=y_;
			MAPEXPRESSION; x=x_; y=y_;
			if (ISTOOBIG(x)||ISTOOBIG(y)) break;
		}
		
		if (ISTOOBIG(x)||ISTOOBIG(y))
			newcol = g_white;
		else
			newcol = 0;
		
		pPosition = ( char* ) pSurface->pixels ; //determine position
		pPosition += ( pSurface->pitch * (py+ystart) ); //offset by y
		pPosition += ( pSurface->format->BytesPerPixel * (px+xstart) ); //offset by x
		memcpy ( pPosition , &newcol , pSurface->format->BytesPerPixel ) ;

		fx += dx;
	}
fy -= dy;
}
}




void DrawFigure( SDL_Surface* pSurface, double c1, double c2, int width, int px ) 
{
	switch (g_settings->drawingMode)
	{
		case DrawModeStandardPhase: 
		case DrawModeLeavesPhase: 
			DrawPhasePortraitStyle(pSurface, c1, c2, width, px); break;
		case DrawModeAlternatePhase:  
			case DrawModeRandomPhase:  
			DrawPhasePortraitAlternate(pSurface, c1, c2, width, px); break;
			
		case DrawModeStandardBasins:  DrawBasinsStandard(pSurface, c1, c2, width, px); break;
		case DrawModeSmearLine:  DrawSmear(pSurface, c1, c2, width, px); break;
		case DrawModeSmearRectangle:  DrawLongTime(pSurface, c1, c2, width, px); break;

		default: {massert(0, "Unknown drawing mode."); }
	}
}

void renderLargeFigure( SDL_Surface* pSurface, int width, const char*filename ) 
{
	char filenameext[256];
	snprintf(filenameext, sizeof(filenameext), "%s.bmp", filename);
//create a new surface.
	SDL_Surface* pRenderSurface = SDL_CreateRGBSurface( SDL_SWSURFACE, width, width, pSurface->format->BitsPerPixel, pSurface->format->Rmask, pSurface->format->Gmask, pSurface->format->Bmask, 0 );
	SDL_FillRect ( pRenderSurface , NULL , g_white );
	//increase the amount of drawing.
	int oldDrawingTime = g_settings->drawingTime;
	g_settings->drawingTime *= 32;
	DrawFigure(pRenderSurface, g_settings->a, g_settings->b, width, 0);
	g_settings->drawingTime = oldDrawingTime;
	SDL_SaveBMP(pRenderSurface, filenameext);
	SDL_FreeSurface(pRenderSurface);
}



