
int * g_arr=NULL; int g_arr_size=0;
__inline void getSetup(int width)
{
	double c1= g_settings->pc1,c2= g_settings->pc2,c3= g_settings->pc3;
	int height=width;
	if (!g_arr || width!=g_arr_size) { if(g_arr)free(g_arr); g_arr=(int*)malloc(sizeof(int)*width*height); }
	
	return; //////////////////////////////////////////////////
	///// User code here
	memset(g_arr, 0, sizeof(int)*width*width); 
	int SETTLE = 120, DRAWING = 4000;
	int nXpoints=30, nYpoints=30; //4
	//draw into array
	
	double sx0i= -0.1, sx1i=0.1, sy0i=-0.1, sy1i=0.1;
	//keep higher one.
	double sx0=MIN(sx0i,sx1i), sx1=MAX(sx0i,sx1i),sy0=MIN(sy0i,sy1i), sy1=MAX(sy0i,sy1i);
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

				int px = (int)(width * ((x - X0) / (X1 - X0)));
				int py = (int)(height - height * ((y - Y0) / (Y1 - Y0)));
				if (py >= 0 && py < height && px>=0 && px<width)
				{
					g_arr[py*width+px]++;
				}
			}
	    }
    }

}

//__inline int getValAt(SDL_Surface* pSurface, double fx, double fy, int width)
//{
//	int DRAWING=4000; int height=width;
//	double X0=g_settings->x0, X1=g_settings->x1, Y0=g_settings->y0, Y1=g_settings->y1;
//	int px = (int)(width * ((fx - X0) / (X1 - X0)));
//	int py = (int)(height - height * ((fy - Y0) / (Y1 - Y0)));
//	if (py >= 0 && py < height && px>=0 && px<width)
//	{
//		/*double val = ((double)g_arr[py*width+px]);
//		if (val>DRAWING) val=DRAWING;
//		int v= 255-(int)(255*((val)/((double)DRAWING)));*/
//		double val = (sqrt((double)g_arr[py*width+px])) / (sqrt((double)DRAWING)); //each could have been hit many times
//			//actually, drawing * nseeds * nseeds would be factor
//		
//		if (val>1.0) val=1.0; 
//		int v = 255-(int)(val*255);
//		return SDL_MapRGB(pSurface->format, v,v,v);
//	}
//	return SDL_MapRGB(pSurface->format, 50,0,0); //shouldn't be here
//}


__inline int getValAt(SDL_Surface* pSurface, double fx, double fy, int width)
{
	double c1= g_settings->pc1,c2= g_settings->pc2,c3= g_settings->pc3;
	double c4= g_settings->pc4,c5= g_settings->pc5,c6= g_settings->pc6;
	double c1b= g_settings->pc1b,c2b= g_settings->pc2b,c3b= g_settings->pc3;
	double c4b= g_settings->pc4b,c5b= g_settings->pc5b,c6b= g_settings->pc6;
	int paramIters = g_settings->iters;
	double maxValue = g_settings->maxValue;
	
	///// User code here
	double x=fx, y=fy, x_, y_;
	for (int i=0; i<paramIters; i++)
	{
		x_ = c1*x - y*y;
		y_ = c2*y + x*y;
		x=x_;
		y=y_;
		if (ISTOOBIG(x)||ISTOOBIG(y)) break;
	}
	if (ISTOOBIG(x)||ISTOOBIG(y)) return g_white; //maxValue+10;
	return standardToColor(pSurface, sqrt( (x-fx)*(x-fx)+(y-fy)*(y-fy)) / maxValue);
}

//__inline int getValAt(double fx, double fy)
//{
//	double c1= g_settings->pc1,c2= g_settings->pc2,c3= g_settings->pc3;
//	double c4= g_settings->pc4,c5= g_settings->pc5,c6= g_settings->pc6;
//	double c1b= g_settings->pc1b,c2b= g_settings->pc2b,c3b= g_settings->pc3;
//	double c4b= g_settings->pc4b,c5b= g_settings->pc5b,c6b= g_settings->pc6;
//	int paramIters = g_settings->iters;
//	double maxValue = g_settings->maxValue;
//	
//	///// User code here
//	int REP=2;  //but note imbalance
//	int N = 70;//paramIters;//////////////////
//	double p0 = 0.5;
//	c1=0.018+g_settings->pc1;//////////////////
//
//	double total = 0, p=p0;
//	for (int i = 0; i < N; i++)
//	{
//		double r = (i%(REP*2)<REP+1) ? fy : fx;
//		//note imbalance: 1 more fy than fx
//
//		p = (100*c1)/sin(r*p);
//		//double d0=(Math.Log(Math.Sin(r*p/2))- Math.Log(Math.Cos(r*p/2)))/r;
//		//double d1=-Math.Cos(r+p)/(Math.Sin(r+p)*Math.Sin(r+p));
//		double sinmapDeriv = -cos(r+p)/(sin(r+p)*sin(r+p));
//		if (i>20)
//			total += log(fabs(sinmapDeriv));
//		if (total<-1000) break;
//	}
//	total = -total/N;
//	if (total<-1000) total=0.0;
//	if (total<0) total= /*-*/ sqrt(-total)/4;
//	else total=sqrt(total)/2;
//
//	return standardToColor(pSurface, total / maxValue);
//}

__inline float apxlog(float v) //wikipediaQuickLog
{
	int x = *(const int *) &v;
	float f = (float) x;
	return f/(1<<23) - 127.0f;
}

//__inline int getValAt(SDL_Surface* pSurface, double dfx, double dfy,int width)
//{
//	float fx=(float)dfx, fy=(float)dfy;
//	float c1= g_settings->pc1,c2= g_settings->pc2,c3= g_settings->pc3;
//	float c4= g_settings->pc4,c5= g_settings->pc5,c6= g_settings->pc6;
//	float c1b= g_settings->pc1b,c2b= g_settings->pc2b,c3b= g_settings->pc3;
//	float c4b= g_settings->pc4b,c5b= g_settings->pc5b,c6b= g_settings->pc6;
//	int paramIters = g_settings->iters;
//	float maxValue = g_settings->maxValue;
//	
//	///// User code here
//	int N = 70;//paramIters;//////////////////
//	float p0 = 0.5;
//	c1=0.018+g_settings->pc1;//////////////////
//
//	float total = 0, p=p0; float r;
//	for (int i = 0; i < 20; i++)
//	{
//		p = (100*c1)/sin(r*p);
//	}
//	for (int i = 0; i < N/4; i++)
//	{
//		r = fy;
//		//note imbalance: 1 more fy than fx
//
//		p = (100*c1)/sin(r*p);
//		float sinmapDeriv = -c2*cos(r+p)/(sin(r+p)*sin(r+p));
//		total += apxlog(fabs(sinmapDeriv));
//		p = (100*c1)/sin(r*p);
//		sinmapDeriv = -c2*cos(r+p)/(sin(r+p)*sin(r+p));
//		total += apxlog(fabs(sinmapDeriv));
//		
//
//		r = fx;
//		p = (100*c1)/sin(r*p);
//		sinmapDeriv = -c2*cos(r+p)/(sin(r+p)*sin(r+p));
//		total += apxlog(fabs(sinmapDeriv));
//		p = (100*c1)/sin(r*p);
//		sinmapDeriv = -c2*cos(r+p)/(sin(r+p)*sin(r+p));
//		total += apxlog(fabs(sinmapDeriv));
//		
//		if (total<-1000) break;
//	}
//	total = -total/N;
//	if (total<-1000) total=0.0;
//	if (total<0) total= /*-*/ sqrt(-total)/4;
//	else total=sqrt(total)/2;
//
//	return standardToColor(pSurface, total / maxValue);
//}


//__inline int getValAt(SDL_Surface* pSurface, double fx, double fy)
//{
//	float c1= g_settings->pc1,c2= g_settings->pc2,c3= g_settings->pc3,c4= g_settings->pc4;
//	double init0 = 35+fx ;
//            double init1 = -10+fy;
//            double init2 = -7 + c4;
//            double cur0 = init0;
//            double cur1 = init1;
//            double cur2 = init2;
//		double cur0_, cur1_, cur2_;
//		double dt = 0.01;//0.001;
//	    //solve some things
//		double B = 8.0/3 + c1, OO=10+c2, P=28+c3;
//		for (double t=0; t<1; t+= dt)
//		{
//			cur0_ = -B*cur0 + cur1*cur2;
//			cur1_ = -OO*cur1 + OO*cur2;
//			cur2_ = -cur1*cur0 + P*cur1 - cur2;
//			cur0 += dt*cur0_;
//			cur1 += dt*cur1_;
//			cur2 += dt*cur2_;
//		}
//		double distance = sqrt( 
//			(cur0-init0)*(cur0-init0) +
//			(cur1-init1)*(cur1-init1) +
//			(cur2-init2)*(cur2-init2) );
//		
//	return standardToColor(pSurface, distance / 100);
//}
//
