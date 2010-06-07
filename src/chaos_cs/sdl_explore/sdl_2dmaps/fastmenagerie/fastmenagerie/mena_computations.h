

//#define UnseenBasinSize 256
//#define UnseenBasinSettling 40 works
#define UnseenBasinSize 128
#define UnseenBasinSettling 20
#define UnseenBasinAttractorDrawing 40
#define UnseenBasinX0 -4
#define UnseenBasinX1 1
#define UnseenBasinY0 -2.5
#define UnseenBasinY1 2.5


int GetSizeOfAttractionBasinSimple(SDL_Surface* pSurface, double c1, double c2) 
{
double fx,fy, x_,y_,x,y; char* pPosition; Uint32 r,g,b, newcol; double val;
double X0=UnseenBasinX0, X1=UnseenBasinX1, Y0=UnseenBasinY0, Y1=UnseenBasinY1;
double dx = (X1 - X0) / UnseenBasinSize, dy = (Y1 - Y0) / UnseenBasinSize;
fx = X0; fy = Y1; //y counts downwards
int escaped = 0, unescaped = 0;
for (int py=0; py<UnseenBasinSize; py+=1)
{
	fx=X0;
	for (int px = 0; px < UnseenBasinSize; px+=1)
	{
		x=fx; y=fy;
		for (int i=0; i<UnseenBasinSettling; i++)
		{
			MAPEXPRESSION;
			x=x_; y=y_;
			if (ISTOOBIG(x)||ISTOOBIG(y)) break;
		}
		if (!(ISTOOBIG(x)||ISTOOBIG(y))) unescaped++;
				
		fx += dx;
	}
fy -= dy;
}
double ratio = unescaped / ((double)UnseenBasinSize*UnseenBasinSize);
return standardToColors(pSurface, ratio, 0.75);
}


int GetSizeOfAttractionBasinSSE(SDL_Surface* pSurface, double c1, double c2) 
{
double X0=UnseenBasinX0, X1=UnseenBasinX1, Y0=UnseenBasinY0, Y1=UnseenBasinY1;
double dx = (X1 - X0) / UnseenBasinSize, dy = (Y1 - Y0) / UnseenBasinSize;
double curx = X0, cury = Y0; 
int escaped = 0, totalComputed=0; //runs perfectly, 16384 iterations
int count0, count1,count2,count3;
count0=count1=count2=count3=UnseenBasinSettling + 10; //make them all expired.

	__m128 mmX = _mm_setr_ps( 0.0,0.0,0.0,0.0);
	__m128 mmY = _mm_setr_ps( 0.0,0.0,0.0,0.0); //symmetrical, so don't just mult by 2.
	__m128 mXTmp;
	MAPSSEINIT;

//state machine.
while (TRUE)
{
	count0++; count1++; count2++; count3++;
	BOOL isTooBig3 = ISTOOBIGF(mmX.m128_f32[3]) || ISTOOBIGF(mmY.m128_f32[3]); //could do all istoobigs?
	if (isTooBig3 || count3 > UnseenBasinSettling)
	{
		if (isTooBig3) escaped++;
		totalComputed++;
		//assign new number.
		curx += dx;
		if (curx>=UnseenBasinX1)
		{
			curx=UnseenBasinX0;
			cury += dy;
			if (cury>=UnseenBasinY1)
				goto outsideloop;
		}
		mmX.m128_f32[3] = curx;
		mmY.m128_f32[3] = cury;
		count3 = 0;
	}
	BOOL isTooBig2 = ISTOOBIGF(mmX.m128_f32[2]) || ISTOOBIGF(mmY.m128_f32[2]); //could do all istoobigs?
	if (isTooBig2 || count2 > UnseenBasinSettling)
	{
		if (isTooBig2) escaped++;
		totalComputed++;
		//assign new number.
		curx += dx;
		if (curx>=UnseenBasinX1)
		{
			curx=UnseenBasinX0;
			cury += dy;
			if (cury>=UnseenBasinY1)
				goto outsideloop;
		}
		mmX.m128_f32[2] = curx;
		mmY.m128_f32[2] = cury;
		count2 = 0;
	}
	BOOL isTooBig1 = ISTOOBIGF(mmX.m128_f32[1]) || ISTOOBIGF(mmY.m128_f32[1]); //could do all istoobigs?
	if (isTooBig1 || count1 > UnseenBasinSettling)
	{
		if (isTooBig1) escaped++;
		totalComputed++;
		//assign new number.
		curx += dx;
		if (curx>=UnseenBasinX1)
		{
			curx=UnseenBasinX0;
			cury += dy;
			if (cury>=UnseenBasinY1)
				goto outsideloop;
		}
		mmX.m128_f32[1] = curx;
		mmY.m128_f32[1] = cury;
		count1 = 0;
	}
	BOOL isTooBig0 = ISTOOBIGF(mmX.m128_f32[0]) || ISTOOBIGF(mmY.m128_f32[0]); //could do all istoobigs?
	if (isTooBig0 || count0 > UnseenBasinSettling)
	{
		if (isTooBig0) escaped++;
		totalComputed++;
		//assign new number.
		curx += dx;
		if (curx>=UnseenBasinX1)
		{
			curx=UnseenBasinX0;
			cury += dy;
			if (cury>=UnseenBasinY1)
				goto outsideloop;
		}
		mmX.m128_f32[0] = curx;
		mmY.m128_f32[0] = cury;
		count0 = 0;
	}

	MAPSSE;

}
outsideloop:

//printf("o%d\n", totalComputed);
int unescaped = (UnseenBasinSize*UnseenBasinSize) - escaped;
double ratio = unescaped / ((double)UnseenBasinSize*UnseenBasinSize);
return standardToColors(pSurface, ratio, 0.75);
}



int GetNumberOfAttractors(SDL_Surface* pSurface, double c1, double c2) 
{
double fx,fy, x_,y_,x,y; char* pPosition; Uint32 r,g,b, newcol; double val;
double X0=UnseenBasinX0, X1=UnseenBasinX1, Y0=UnseenBasinY0, Y1=UnseenBasinY1;
double dx = (X1 - X0) / UnseenBasinSize, dy = (Y1 - Y0) / UnseenBasinSize;
fx = X0; fy = Y1; //y counts downwards
int escaped = 0, unescaped = 0;
//40 iters * about 0.5, = about 20. 4 different counts as different?
//why don't we just take the mean for now.
double totalDistance = 0.0, totalDistancePart=0.0;
for (int py=0; py<UnseenBasinSize; py+=1)
{
	fx=X0;
	for (int px = 0; px < UnseenBasinSize; px+=1)
	{
		x=fx; y=fy;
		totalDistancePart=0.0;
		//totalDistance = 0.0;
		for (int i=0; i<UnseenBasinAttractorDrawing; i++)
		{
			MAPEXPRESSION;
			x=x_; y=y_;
			if (ISTOOBIG(x)||ISTOOBIG(y)) break;
			totalDistancePart += x*x+y*y;
		}
		if (!(ISTOOBIG(x)||ISTOOBIG(y))) { totalDistance += totalDistancePart/UnseenBasinAttractorDrawing; 
		unescaped++;}
		
		fx += dx;
	}
fy -= dy;
}

return standardToColors(pSurface, totalDistance, UnseenBasinSize*UnseenBasinSize*1.2);
}
