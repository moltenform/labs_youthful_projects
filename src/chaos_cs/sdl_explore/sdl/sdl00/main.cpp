//http://www.linuxjournal.com/article/4401?page=0,1
//http://www.gamedev.net/reference/programming/features/sdl2/page5.asp
//http://gpwiki.org/index.php/SDL:Tutorials:Keyboard_Input_using_an_Event_Loop
//http://www.gameprogrammer.com/fastevents/fastevents1.html
/* have physics? we're elastically pulled to a point, use arrow keys to set point, has natural oscillation
*/

#include "SDL.h"
#include <stdio.h>
#include <stdlib.h>

#include <memory.h>
#include <math.h>

double X0,X1,Y0,Y1;
Uint32 White;

enum {
  SCREENWIDTH = 640,
  SCREENHEIGHT = 480,
  SCREENBPP = 0,
  SCREENFLAGS = SDL_ANYFORMAT
} ; 
void DoCoolStuff ( SDL_Surface* pSurface, double sxinc, double syinc, double c1, double c2 ) ;

void SetPixel ( SDL_Surface* pSurface , int x , int y , SDL_Color color ) ;
SDL_Color GetPixel ( SDL_Surface* pSurface , int x , int y ) ;

inline bool LockFramesPerSecond() //run no faster than 60fps
{
	int framerate=60;
static float lastTime = 0.0f;
float currentTime = SDL_GetTicks() * 0.001f;
if((currentTime - lastTime) > (1.0f / framerate))
{
lastTime = currentTime;
return true;
}
return false;
}

int main( int argc, char* argv[] )
{
	X0=-1.75; X1=1.75;Y0=-1.75;Y1=1.75;
	double sx0= -2, sx1=2, sy0= -2, sy1=2;
//int nXpoints = 60, nYpoints = 60;
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


int i=0;
  White = SDL_MapRGB ( pSurface->format , 255,255,255 ) ;
SDL_FillRect ( pSurface , NULL , White );

bool bNeedToLock =  ( SDL_MUSTLOCK ( pSurface ) );
double curA = -1.1, curB = 1.72;
double targetA = curA, targetB = curB;

  //message pump
  for ( ; ; )
  {
    //look for an event
    if ( SDL_PollEvent ( &event ) )
    {
      //an event was found
      if ( event.type == SDL_QUIT ) break ;
	  else if (event.type==SDL_KEYUP)
	  {
		  if (event.key.keysym.sym == SDLK_UP) targetB += 0.1;
		  else if (event.key.keysym.sym == SDLK_DOWN) targetB -= 0.1;
		  else if (event.key.keysym.sym == SDLK_LEFT) targetA -= 0.1;
		  else if (event.key.keysym.sym == SDLK_RIGHT) targetA += 0.1;
		  else if (event.key.keysym.sym == SDLK_ESCAPE) {targetA=-1.1, targetB = 1.72;}
	  }
    }
	if (curA > targetA) curA -= 0.005;
	else curA += 0.005;
	if (curB > targetB) curB -= 0.005;
	else curB += 0.005;
 
    //lock the surface
    if (bNeedToLock) SDL_LockSurface ( pSurface ) ;

if (LockFramesPerSecond()) 
{
	DoCoolStuff(pSurface, sxinc, syinc, curA,curB);
	i=0;
} 


    //unlock surface
    if (bNeedToLock) SDL_UnlockSurface ( pSurface ) ;

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


void DoCoolStuff ( SDL_Surface* pSurface, double sxinc, double syinc, double c1, double c2 ) 
{

	double x_,x,y;
	//double c1=-1.1, c2=1.72;
	//c2 -= 0.001;
static double State=0;
State+=0.09; if (State>100) State=0;
//c1 = c1 + sin(State)/64000;
//c2 = c1 + cos(State)/64000;

int paramSettle = 48;
int nItersPerPoint=20; //10

	SDL_FillRect ( pSurface , NULL , White );  //clear surface quickly

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
						//int newcolor = (color.r)-((color.r)>>3);
						int newcolor = (color.r)-((color.r)>>2);
						//int newcolor = ((color.r)>>2)+((color.r)>>3); //5/8
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
