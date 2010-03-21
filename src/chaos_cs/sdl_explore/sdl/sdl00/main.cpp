//http://www.linuxjournal.com/article/4401?page=0,1
//http://www.gamedev.net/reference/programming/features/sdl2/page5.asp

#include "SDL.h"
#include <stdio.h>
#include <stdlib.h>

void DrawImage(SDL_Surface *srcimg, int sx, int sy, int sw, int sh, SDL_Surface *dstimg, int dx, int dy, int alpha = 255) {
  if ((!srcimg) || (alpha == 0)) return; //If theres no image, or its 100% transparent.
  SDL_Rect src, dst;
  
  src.x = sx;  src.y = sy;  src.w = sw;  src.h = sh;
  dst.x = dx;  dst.y = dy;  dst.w = src.w;  dst.h = src.h;
  //if (alpha != 255) SDL_SetAlpha(srcimg, SDL_SRCALPHA, alpha);
  // - This is incorrect, if alpha is 10, then set to 255, the image alpha will still be 10.
  SDL_SetAlpha(srcimg, SDL_SRCALPHA, alpha);
  SDL_BlitSurface(srcimg, &src, dstimg, &dst);
}

#include "SDL.h"
#include <stdlib.h>
#include <memory.h>

enum {
  SCREENWIDTH = 640,
  SCREENHEIGHT = 480,
  SCREENBPP = 0,
  SCREENFLAGS = SDL_ANYFORMAT
} ;

void SetPixel ( SDL_Surface* pSurface , int x , int y , SDL_Color color ) ;
SDL_Color GetPixel ( SDL_Surface* pSurface , int x , int y ) ;
int main( int argc, char* argv[] )
{
  //initialize systems
  SDL_Init ( SDL_INIT_VIDEO ) ;

  //set our at exit function
  atexit ( SDL_Quit ) ;

  //create a window
  SDL_Surface* pSurface = SDL_SetVideoMode ( SCREENWIDTH , SCREENHEIGHT ,
                                             SCREENBPP , SCREENFLAGS ) ;

  //declare event variable
  SDL_Event event ;

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
    SetPixel ( pSurface , rand ( ) % SCREENWIDTH , rand ( ) % SCREENHEIGHT , color ) ;

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
