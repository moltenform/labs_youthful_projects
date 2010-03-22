//http://www.linuxjournal.com/article/4401?page=0,1
//http://www.gamedev.net/reference/programming/features/sdl2/page5.asp
//http://gpwiki.org/index.php/SDL:Tutorials:Keyboard_Input_using_an_Event_Loop
//http://www.gameprogrammer.com/fastevents/fastevents1.html
/* have physics? we're elastically pulled to a point, use arrow keys to set point, has natural oscillation
//http://www.daniweb.com/code/post968823.html#
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
  SCREENFLAGS = SDL_ANYFORMAT// | SDL_FULLSCREEN
} ; 
void DoCoolStuff ( SDL_Surface* pSurface, double sxinc, double syinc, double c1, double c2 ) ;

void jump(double *outA, double *outB);
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
void save(double a,double b)
{
	FILE*fp = fopen("..\\saved.txt", "a"); //append mode
	fprintf(fp, "%f,%f\n", a,b);
	fclose(fp);
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
double oscilState=0.0, oscilFreq=0.1, oscilFreqState=0.0;

SDL_EnableKeyRepeat(30 /*SDL_DEFAULT_REPEAT_DELAY*/, SDL_DEFAULT_REPEAT_INTERVAL);
//is there a better way of doing this? One could also have a SDL_KEYDOWN and paired KEYUP, and do stuff in between 

  //message pump
  for ( ; ; )
  {
    //look for an event
    if ( SDL_PollEvent ( &event ) )
    {
      //an event was found
      if ( event.type == SDL_QUIT ) break ;
	  else if (event.type==SDL_KEYDOWN)
	  {
		  if (event.key.keysym.sym == SDLK_UP) targetB += 0.005;
		  else if (event.key.keysym.sym == SDLK_DOWN) targetB -= 0.005;
		  else if (event.key.keysym.sym == SDLK_LEFT) targetA -= 0.005;
		  else if (event.key.keysym.sym == SDLK_RIGHT) targetA += 0.005;
		  else if (event.key.keysym.sym == SDLK_ESCAPE) {targetA=-1.1, targetB = 1.72;}
		  else if (event.key.keysym.sym == SDLK_s) {save(curA,curB);}
		  else if (event.key.keysym.sym == SDLK_j) {jump(&targetA, &targetB);}
		  else if (event.key.keysym.sym == SDLK_F4) {break;}
	  }
    }
//the frequency itself oscillates
if (oscilFreqState>31.415926) oscilFreqState=0.0;
oscilFreqState+=0.01;
oscilFreq = 0.09 + sin(oscilFreqState)/70;
//oscilFreq = 0.1;

if (oscilState>31.415926) oscilState=0.0;
oscilState+=oscilFreq;

	curA += (targetA-curA)/10;
	curB += (targetB-curB)/10;

double actualA = curA+ sin(oscilState*.3702342521232353)/550;
double actualB = curB+ cos(oscilState)/400;

 
    //lock the surface
    if (bNeedToLock) SDL_LockSurface ( pSurface ) ;

if (LockFramesPerSecond()) 
{
	DoCoolStuff(pSurface, sxinc, syinc, actualA,actualB);
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

typedef struct node node ;
struct node {  double vala; double valb;  node* next ; };
node * NSaved = NULL;
node* choose_random_node( node* first )
{
  int num_nodes = 0 ; // nodes seen so far
  node* selected = NULL ; // selected node
  node* pn = NULL ;
  for( pn = first ; pn != NULL ; pn = pn->next )
    if(  ( rand() % ++num_nodes  ) == 0 ) selected = pn ;
  return selected ;
}
node* createNode(double a, double b, node *next)
{
node* newnode = (node*) malloc(sizeof(node));
newnode->vala=a; newnode->valb=b;
newnode->next = NULL;
return newnode;
}
void jump(double *outA, double *outB)
{
		//FILE*fdbg = fopen("..\\debug.txt", "a");
	double a,b;
	if (NSaved==NULL)
	{
		//fprintf(fdbg, "mknew\n"); /////////
		NSaved = createNode(-1.1, 1.72, NULL);
		node*cur = NSaved;
		FILE*fp = fopen("..\\saved.txt", "r");
		while (fscanf(fp, "%lf,%lf\n", &a,&b)==2)
		{
			//fprintf(fdbg, "val%lf,%lf\n",a,b); /////////
			cur->next = createNode(a,b, NULL);
			cur = cur->next;
		}
		fclose(fp);
	}
	node * picked = choose_random_node(NSaved);
	*outA = picked->vala;
	*outB = picked->valb;
			//fprintf(fdbg, "PICKED%f,%f\n",*outA, *outB); /////////
}

