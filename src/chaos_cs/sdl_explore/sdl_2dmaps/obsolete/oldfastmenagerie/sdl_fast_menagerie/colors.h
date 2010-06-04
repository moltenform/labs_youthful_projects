#include "common.h"

void DoubleSeedCoordsToInt(MenagFastSettings*settings, double fx, double fy, int* outX, int* outY);
int HSL2RGB(SDL_Surface* pSurface, double h, double sl, double l);
void plotpoint(SDL_Surface* pSurface, int px, int py, int color); //borrow from phaseportrait.cpp
void drawcolors(SDL_Surface* pSurface, MenagFastSettings * settings, double c1, double c2, int iter)
{
	if (iter<0) return;
	settings->seedx0 = -4; settings->seedx1 = 4;
	settings->seedy0 = -4; settings->seedy1 = 4;
	double PI = 3.14159, x_;
	double xradius=1, yradius=1;//double xradius=0.5, yradius=0.5; w henon cool
	for (double t=0; t<1.0; t+=0.001)
	{
		int color = HSL2RGB(pSurface,t, .5,.5);
		double x = cos(t * 2*PI)*xradius;
		double y = sin(t * 2*PI)*yradius;
		int px, py;

		/*for (int i=0; i<4; i++) { ///8 is pretty.
DoubleSeedCoordsToInt(settings, x,y, &px, &py);
		plotpoint(pSurface, px,py,color);
			x_ = c1*x - y*y; y= c2*y + x*y; 
			x=x_;
		}*/
		if (iter==1) {DoubleSeedCoordsToInt(settings, x,y, &px, &py);plotpoint(pSurface, px,py,color);}
		for (int i=0; i<iter; i++) { 

			x_ = c1*x - y*y; y= c2*y + x*y; 
			x=x_;
		}
		DoubleSeedCoordsToInt(settings, x,y, &px, &py);
		plotpoint(pSurface, px,py,color);
	}
	
}
unsigned int getacolor(double valin, double estimatedMax)
{
	double val = (valin) / (estimatedMax);
	if (val > estimatedMax) val =estimatedMax;
val = (valin) / (estimatedMax*1.2);
	if (val<0) val=0; if (val>1) val=1;
	val = fmod(val-0.2, 1.0); if (val<0) val+=1;
	//val *= 0.7;
	return HSL2RGB(NULL, val, 1,.5);
}
double getDetOfJacobian(double a, double b, double x,double y);
void drawdetjec(SDL_Surface* pSurface, MenagFastSettings * settings, double c1, double c2, int iter)
{
	double fx,fy, x_,x,y;
	double X0=-3, X1=3, Y0=-3, Y1=3;
    double dx = (X1 - X0) / MenagWidth, dy = (Y1 - Y0) / MenagHeight;
    fx = X0; fy = Y1; //y counts downwards
	 char* pPosition;
    for (int py=0; py<MenagHeight; py+=1)
        {
			fx=X0;
	 for (int px = 0; px < MenagWidth; px+=1)
    {
        x=fx; y=fy;
		double v =getDetOfJacobian(c1,c2, fx,fy);
	
		Uint32 newcol = getacolor(v + 8, 16);
	/*double grav = v/6.0;
	if (grav>1.0) grav=1.0; if (grav<0) grav=fmod(grav, 1.0)+1;
	int iv = (int)(grav*255);
Uint32 newcol = iv;*/
	//if (abs(v-1.0) < 0.2) return 244;
			

for (int ncy=0; ncy<1; ncy++){
for (int ncx=0; ncx<1; ncx++){

  pPosition = ( char* ) pSurface->pixels ; //determine position
  pPosition += ( pSurface->pitch * (py+ncy) ); //offset by y
  pPosition += ( pSurface->format->BytesPerPixel * (px+ncx) ); //offset by x
  //Uint32 newcol = SDL_MapRGB ( pSurface->format , r , g , b ) ;
  memcpy ( pPosition , &newcol , pSurface->format->BytesPerPixel ) ;
}}
        fx += dx*1;
        }
        fy -= dy*1;
    }
}

int docolors(SDL_Surface* pSurface, MenagFastSettings * settings, double *curA, double *curB)
{
	SDL_Event event;
BOOL isDirty=TRUE;
int iter=1;
while (TRUE)
{
	if ( SDL_PollEvent ( &event ) )
	{
	//an event was found
	if ( event.type == SDL_QUIT ) return 0;
	else if (event.type==SDL_MOUSEBUTTONDOWN) return 0;
	else if (event.type==SDL_KEYDOWN){
		isDirty=TRUE;
		switch(event.key.keysym.sym)
		{
			case SDLK_UP: *curB += (event.key.keysym.mod & KMOD_SHIFT) ? 0.005 : 0.05; break;
			case SDLK_DOWN: *curB -= (event.key.keysym.mod & KMOD_SHIFT) ? 0.005 : 0.05; break;
			case SDLK_LEFT: *curA -= (event.key.keysym.mod & KMOD_SHIFT) ? 0.005 : 0.05; break;
			case SDLK_RIGHT: *curA += (event.key.keysym.mod & KMOD_SHIFT) ? 0.005 : 0.05; break;
			case SDLK_i: iter+= (event.key.keysym.mod & KMOD_SHIFT) ? -1 : 1; break;
			default:isDirty=FALSE; break;
		}
	  }
	}

		if (LockFramesPerSecond() && isDirty) 
		{
			SDL_FillRect ( pSurface , NULL , g_white );  //clear surface quickly
			if (SDL_MUSTLOCK(pSurface)) SDL_LockSurface ( pSurface ) ;
			//drawdetjec(pSurface, settings, *curA,*curB, iter);
			drawcolors(pSurface, settings, *curA,*curB, iter);
			if (SDL_MUSTLOCK(pSurface)) SDL_UnlockSurface ( pSurface ) ;
			SDL_UpdateRect ( pSurface , 0 , 0 , 0 , 0 ) ;  //apparently needed every frame, even when not redrawing
			isDirty=FALSE;
		}

	}

return 0;
}



 // Given H,S,L in range of 0-1
      // Returns a Color (RGB struct) in range of 0-255
      int HSL2RGB(SDL_Surface* pSurface, double h, double sl, double l)
      {
            double v;
            double r,g,b;
 
            r = l;   // default to gray
            g = l;
            b = l;
            v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);
            if (v > 0)
            {
                  double m;
                  double sv;
                  int sextant;
                  double fract, vsf, mid1, mid2;
 
                  m = l + l - v;
                  sv = (v - m ) / v;
                  h *= 6.0;
                  sextant = (int)h;
                  fract = h - sextant;
                  vsf = v * sv * fract;
                  mid1 = m + vsf;
                  mid2 = v - vsf;
                  switch (sextant)
                  {
                        case 0:
                              r = v;
                              g = mid1;
                              b = m;
                              break;
                        case 1:
                              r = mid2;
                              g = v;
                              b = m;
                              break;
                        case 2:
                              r = m;
                              g = v;
                              b = mid1;
                              break;
                        case 3:
                              r = m;
                              g = mid2;
                              b = v;
                              break;
                        case 4:
                              r = mid1;
                              g = m;
                              b = v;
                              break;
                        case 5:
                              r = v;
                              g = m;
                              b = mid2;
                              break;
                  }
            }
            int ir,ig,ib;
            ir = (int)(r * 255.0f);
            ig = (int)(g * 255.0f);
            ib = (int)(b * 255.0f);
			if (!pSurface)
				return (ir<<16) + (ig<<8) + ib;
			else
            return SDL_MapRGB ( pSurface->format , ir,ig,ib ) ;
			//(ir<<16) + (ig<<8) + ib;
      }