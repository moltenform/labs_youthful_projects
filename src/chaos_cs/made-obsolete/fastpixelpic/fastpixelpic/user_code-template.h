
#define RGB(r,g,b) (SDL_MapRGB(pSurface->format, (r),(g),(b)))

int g_arr_size=0; int * g_arr=NULL;

//$$INSERT_USER_OUTSIDE

__inline int getValAt(SDL_Surface* pSurface, double fx, double fy, int width)
{
	int height=width;
	double c1= g_settings->pc1,c2= g_settings->pc2,c3= g_settings->pc3;
	double c4= g_settings->pc4,c5= g_settings->pc5,c6= g_settings->pc6;
	double c1b= g_settings->pc1b,c2b= g_settings->pc2b,c3b= g_settings->pc3b;
	double c4b= g_settings->pc4b,c5b= g_settings->pc5b,c6b= g_settings->pc6b;
	int paramDrawing = g_settings->drawing; int paramSettling = g_settings->settling;
	double maxValue = 1; //intended to be overwritten by user code
	double x,y,x_,y_; (void)x; (void)y; (void)x_; (void)y_; //hack: compiler will think it's used.
	double val;
	{
	///// User code to be placed here. in it's own scope (nice because won't cause an error if they have an x or x_).
	//$$INSERT_USER_INSIDE
	}
	
	return standardToColor(pSurface, val / (maxValue+g_settings->maxValueAddition));
}

__inline int getValAtDiagramPlot(SDL_Surface* pSurface, double fx, double fy, int width)
{
	double c1= g_settings->pc1,c2= g_settings->pc2,c3= g_settings->pc3;
	double c4= g_settings->pc4,c5= g_settings->pc5,c6= g_settings->pc6;
	double c1b= g_settings->pc1b,c2b= g_settings->pc2b,c3b= g_settings->pc3b;
	double c4b= g_settings->pc4b,c5b= g_settings->pc5b,c6b= g_settings->pc6b;
	int paramDrawing = g_settings->drawing; int paramSettling = g_settings->settling;
	double maxValue = 1; //intended to be overwritten by user code
	double x,y,x_,y_; (void)x; (void)y; (void)x_; (void)y_; //hack: compiler will think it's used.
	double val;
	{
	///// User code to be placed here. in it's own scope.
	//$$INSERT_USER_DIAG
	}
	
	return standardToColor(pSurface, val / (maxValue+g_settings->maxValueAddition));
}



