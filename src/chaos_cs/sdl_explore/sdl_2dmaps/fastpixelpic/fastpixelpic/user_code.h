

__inline int getValAt(SDL_Surface* pSurface, double fx, double fy, int width)
{
	double c1= g_settings->pc1,c2= g_settings->pc2,c3= g_settings->pc3;
	double c4= g_settings->pc4,c5= g_settings->pc5,c6= g_settings->pc6;
	double c1b= g_settings->pc1b,c2b= g_settings->pc2b,c3b= g_settings->pc3;
	double c4b= g_settings->pc4b,c5b= g_settings->pc5b,c6b= g_settings->pc6;
	int paramIters = g_settings->drawing;
	double maxValue = g_settings->maxValue;
	
	///// User code here
	double val = fx*fy + fabs(fx);
	
	return standardToColor(pSurface, val / maxValue);
}



