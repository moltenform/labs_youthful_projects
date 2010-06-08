
//could also draw which plots have the most transience. but
//additional figures: which have multiple attractors, average values, variance

//#define UnseenBasinSize 256
//#define UnseenBasinSettling 40 works
#define UnseenBasinSize 128
#define UnseenBasinSettling 40
#define UnseenBasinAttractorDrawing 40
#define UnseenBasinX0 -4.0f
#define UnseenBasinX1 1.0f
#define UnseenBasinY0 -2.5f
#define UnseenBasinY1 2.5f


int GetSizeOfAttractionBasinSimple(SDL_Surface* pSurface, double c1, double c2) 
{
double fx,fy, x_,y_,x,y; 
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
float X0=UnseenBasinX0, X1=UnseenBasinX1, Y0=UnseenBasinY0, Y1=UnseenBasinY1;
float dx = (X1 - X0) / UnseenBasinSize, dy = (Y1 - Y0) / UnseenBasinSize;
//float curx = X0, cury = Y0; 
__m128 curx = _mm_setr_ps(X0,X0,X0,X0); 
__m128 cury; cury.m128_f32[0] = Y0; cury.m128_f32[1] = Y0+dy; cury.m128_f32[2] = Y0+dy*2; cury.m128_f32[3] = Y0+dy*3;

__m128 allZeros = _mm_set1_ps(0.0f);
__m128 counts = _mm_set1_ps((float) (UnseenBasinSettling + 10));
__m128 allNumberOne = _mm_set1_ps(1.0f);
__m128 Howmanyescaped = allZeros;
__m128 totalComputed = allZeros;
__m128 iscurylocationtoobig = allZeros;

	__m128 mmX = _mm_setr_ps( 0.0,0.0,0.0,0.0);
	__m128 mmY = _mm_setr_ps( 0.0,0.0,0.0,0.0); //symmetrical, so don't just mult by 2.
	__m128 mXTmp;
	MAPSSEINIT;

//state machine.
while (TRUE)
{
	counts = _mm_add_ps(counts, allNumberOne); //inc counts
	__m128 istoobigX =  _mm_or_ps(_mm_cmplt_ps( mmX, _mm_set1_ps(-1e2f)), _mm_cmpgt_ps( mmX, _mm_set1_ps(1e2f)));
	__m128 istoobigY =  _mm_or_ps(_mm_cmplt_ps( mmY, _mm_set1_ps(-1e2f)), _mm_cmpgt_ps( mmY, _mm_set1_ps(1e2f)));
	__m128 istoobig = _mm_or_ps(istoobigX, istoobigY);

	__m128 toomanyiters = _mm_cmpgt_ps(counts, _mm_set1_ps(UnseenBasinSettling));
	__m128 needsnew = _mm_or_ps(istoobig, toomanyiters);

	if (needsnew.m128_i32[3]!=0||needsnew.m128_i32[2]!=0||needsnew.m128_i32[1]!=0||needsnew.m128_i32[0]!=0)
	{
		Howmanyescaped = _mm_add_ps(Howmanyescaped, _mm_and_ps(istoobig, allNumberOne));
		totalComputed = _mm_add_ps(totalComputed, _mm_and_ps(needsnew, allNumberOne));
		counts = _mm_andnot_ps(needsnew, counts); //if needsnew is true, set counts to 0

		curx = _mm_add_ps(curx, _mm_and_ps(needsnew, _mm_set1_ps(dx)));
		__m128 iscurxlocationtoobig = _mm_cmpge_ps(curx, _mm_set1_ps(UnseenBasinX1));
		cury = _mm_add_ps(cury, _mm_and_ps( iscurxlocationtoobig, _mm_set1_ps(dy * 4))); //skip ahead 4 at once
		iscurylocationtoobig = _mm_cmpge_ps(cury, _mm_set1_ps(UnseenBasinY1));
		curx = _mm_or_ps( _mm_and_ps(iscurxlocationtoobig,_mm_set1_ps(UnseenBasinX0)), _mm_andnot_ps(iscurxlocationtoobig,curx)); 
		if (iscurylocationtoobig.m128_i32[0]!=0 && iscurylocationtoobig.m128_i32[1]!=0 && iscurylocationtoobig.m128_i32[2]!=0 && iscurylocationtoobig.m128_i32[3]!=0)
			goto outsideloop;

		mmX = _mm_or_ps( _mm_and_ps(needsnew,curx), _mm_andnot_ps(needsnew,mmX));
		mmY = _mm_or_ps( _mm_and_ps(needsnew,cury), _mm_andnot_ps(needsnew,mmY));

	}
		//FIXES jaggedness! :
	counts = _mm_andnot_ps(iscurylocationtoobig, counts); //if iscurylocationtoobig is true, set counts to 0
	
	MAPSSE;

}
outsideloop:

int totalC = (int)(totalComputed.m128_f32[0]+totalComputed.m128_f32[1]+totalComputed.m128_f32[2]+totalComputed.m128_f32[3]);
int escaped = (int)(Howmanyescaped.m128_f32[0]+Howmanyescaped.m128_f32[1]+Howmanyescaped.m128_f32[2]+Howmanyescaped.m128_f32[3]);
//printf("o%d\n", totalC);
//int unescaped = (UnseenBasinSize*UnseenBasinSize) - escaped;
int unescaped = (totalC) - escaped;
double ratio = unescaped / ((double)totalC);
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


/*
	if (key==SDLK_z) {
	*needRedraw = getBinaryOpt(wasKeyCombo, key, bShift);
	}
int getBinaryOpt(int a, int b, int q)
{
	return (a & q)|(b & ~q);
}
looking for optimization
*/

