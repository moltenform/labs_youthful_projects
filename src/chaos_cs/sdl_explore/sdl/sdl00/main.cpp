//http://www.linuxjournal.com/article/4401?page=0,1
//http://www.gamedev.net/reference/programming/features/sdl2/page5.asp

#include "SDL.h"
#include <stdio.h>
#include <stdlib.h>

#include <memory.h>

double X0,X1,Y0,Y1;

enum {
  SCREENWIDTH = 640,
  SCREENHEIGHT = 480,
  SCREENBPP = 0,
  SCREENFLAGS = SDL_ANYFORMAT
} ;
void DoCoolStuff ( SDL_Surface* pSurface, double sxinc, double syinc ) ;

void SetPixel ( SDL_Surface* pSurface , int x , int y , SDL_Color color ) ;
SDL_Color GetPixel ( SDL_Surface* pSurface , int x , int y ) ;
int main( int argc, char* argv[] )
{
	X0=-1.75; X1=1.75;Y0=-1.75;Y1=1.75;
	double sx0= -2, sx1=2, sy0= -2, sy1=2;
int nXpoints = 40, nYpoints = 40;
double sxinc = (nXpoints==1) ? 0xffffffff : (sx1-sx0)/(nXpoints-1);
double syinc = (nYpoints==1) ? 0xffffffff : (sy1-sy0)/(nYpoints-1);

  //initialize systems
  SDL_Init ( SDL_INIT_VIDEO ) ;

  //set our at exit function
  atexit ( SDL_Quit ) ;

  //create a window
  SDL_Surface* pSurface = SDL_SetVideoMode ( SCREENWIDTH , SCREENHEIGHT ,
                                             SCREENBPP , SCREENFLAGS ) ;

  //declare event variable
  SDL_Event event ;

  //SDL_Surface* bWhite;
  //SDL_FillRect ( bWhite , &rect ,
   //                SDL_MapRGB ( pSurface->format , 255 , 255, 255 ) ) ;

int i=0;
  Uint32 White = SDL_MapRGB ( pSurface->format , 255,255,255 ) ;
SDL_FillRect ( pSurface , NULL , White );

  //message pump
  for ( ; ; )
  {
    //look for an event
    if ( SDL_PollEvent ( &event ) )
    {
      //an event was found
      if ( event.type == SDL_QUIT ) break ;
    }

    //pick a random color
    SDL_Color color ;
    color.r = rand ( ) % 256 ;
    color.g = rand ( ) % 256 ;
    color.b = rand ( ) % 256 ;

    //lock the surface
    SDL_LockSurface ( pSurface ) ;

    //plot pixel at random location
	//for (int j=0; j<900000; j++)
	//for (int j=0; j<9; j++)
    //SetPixel ( pSurface , rand ( ) % SCREENWIDTH , rand ( ) % SCREENHEIGHT , color ) ;

	//SDL_FillRect ( pSurface , NULL , White );

i++;
if (i>1) {

	SDL_FillRect ( pSurface , NULL , White );  //clear surface quickly
	DoCoolStuff(pSurface, sxinc, syinc);
	i=0;
} 


    //unlock surface
    SDL_UnlockSurface ( pSurface ) ;

    //update surface
    SDL_UpdateRect ( pSurface , 0 , 0 , 0 , 0 ) ;

	
  }//end of message pump

  //done
  return ( 0 ) ;
}
void SetPixel ( SDL_Surface* pSurface , int x , int y , SDL_Color color ) 
{
  //convert color
  Uint32 col = SDL_MapRGB ( pSurface->format , color.r , color.g , color.b ) ;

  //determine position
  char* pPosition = ( char* ) pSurface->pixels ;

  //offset by y
  pPosition += ( pSurface->pitch * y ) ;

  //offset by x
  pPosition += ( pSurface->format->BytesPerPixel * x ) ;

  //copy pixel data
  memcpy ( pPosition , &col , pSurface->format->BytesPerPixel ) ;
}
SDL_Color GetPixel ( SDL_Surface* pSurface , int x , int y ) 
{
  SDL_Color color ;
  Uint32 col = 0 ;

  //determine position
  char* pPosition = ( char* ) pSurface->pixels ;

  //offset by y
  pPosition += ( pSurface->pitch * y ) ;

  //offset by x
  pPosition += ( pSurface->format->BytesPerPixel * x ) ;

  //copy pixel data
  memcpy ( &col , pPosition , pSurface->format->BytesPerPixel ) ;

  //convert color
  SDL_GetRGB ( col , pSurface->format , &color.r , &color.g , &color.b ) ;
  return ( color ) ;
}


void DoCoolStuff ( SDL_Surface* pSurface, double sxinc, double syinc ) 
{
	double x_,x,y;
	static double c1=-1.1, c2=1.72;
	c2 -= 0.001;
int paramSettle = 48;
int nItersPerPoint=20; //10


	double sx0= -2, sx1=2, sy0= -2, sy1=2;


	for (double sx=sx0; sx<=sx1; sx+=sxinc)
            {
                for (double sy=sy0; sy<=sy1; sy+=syinc)
                {
                    x = sx; y=sy;

                    for (int ii=0; ii<paramSettle; ii++)
                    {
                        x_ = c1*x - y*y;
						y = c2*y + x*y;
                        x=x_; 
                    }
                    for (int ii=0; ii<nItersPerPoint; ii++)
                    {
                        x_ = c1*x - y*y;
						y = c2*y + x*y;
                        x=x_; 

                        int px = (int)(SCREENWIDTH * ((x - X0) / (X1 - X0)));
                        int py = (int)(SCREENHEIGHT - SCREENHEIGHT * ((y - Y0) / (Y1 - Y0)));
                        if (py >= 0 && py < SCREENHEIGHT && px>=0 && px<SCREENWIDTH)
						{
							//get pixel color, mult by 0.875 (x-x>>3)
  SDL_Color color ;
  Uint32 col = 0 ;
  //determine position
  char* pPosition = ( char* ) pSurface->pixels ;
  //offset by y
  pPosition += ( pSurface->pitch * py ) ;
  //offset by x
  pPosition += ( pSurface->format->BytesPerPixel * px ) ;
  //copy pixel data
  memcpy ( &col , pPosition , pSurface->format->BytesPerPixel ) ;
  //convert color
  SDL_GetRGB ( col , pSurface->format , &color.r , &color.g , &color.b ) ;
							
							// a quick mult, stops at 7, but whatever
						int newcolor = (color.r)-((color.r)>>3);
//convert color
  Uint32 newcol = SDL_MapRGB ( pSurface->format , newcolor , newcolor , newcolor ) ;
  //determine position
  //char* pPosition = ( char* ) pSurface->pixels ;
  //offset by y
  //pPosition += ( pSurface->pitch * py ) ;
  //offset by x
  //pPosition += ( pSurface->format->BytesPerPixel * px ) ;
  //copy pixel data
  memcpy ( pPosition , &newcol , pSurface->format->BytesPerPixel ) ;

						}
                    }
                }
            }

}
