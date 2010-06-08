
//could also draw which plots have the most transience. but
//additional figures: which have multiple attractors, average values, variance

//#define UnseenBasinSize 256
//#define UnseenBasinSettling 40 works
#define UnseenBasinSize 128
#define UnseenBasinSettling 20
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

__m128i allZeros = _mm_set1_epi32(0);
__m128i allNumberOne = _mm_set1_epi32(1);//note that we might be able to add one by subtracting allOnes...
__m128i allOnes = _mm_set1_epi32(0xffffffff); //-1; 

__m128 curx = _mm_setr_ps(X0,X0,X0,X0); 
__m128 cury;// = _mm_setr_ps(Y0,Y0+dy,Y0+dy+dy,Y0+dy+dy+dy);
cury.m128_f32[0] = Y0; cury.m128_f32[1] = Y0+dy; cury.m128_f32[2] = Y0+dy*2; cury.m128_f32[3] = Y0+dy*3;

	__m128 mmX = _mm_setr_ps( 0.0,0.0,0.0,0.0);
	__m128 mmY = _mm_setr_ps( 0.0,0.0,0.0,0.0);
	__m128 mXTmp;
	MAPSSEINIT;

	__m128i Counts = allZeros;
	__m128i Howmanyescaped = allZeros;
	__m128i TotalComputed = allZeros;
//state machine.
while (TRUE)
{
	Counts = _mm_add_epi32(Counts, allNumberOne); //inc counts
	//doing all conditionals at once like this created a huge speedup: from 4975 to 2544!
	__m128 istoobigX =  _mm_or_ps(_mm_cmplt_ps( mmX, _mm_set1_ps(-1e2f)), _mm_cmpgt_ps( mmX, _mm_set1_ps(1e2f)));
	__m128 istoobigY =  _mm_or_ps(_mm_cmplt_ps( mmY, _mm_set1_ps(-1e2f)), _mm_cmpgt_ps( mmY, _mm_set1_ps(1e2f)));
	union {__m128 m128; __m128i m128i;} istoobigU; istoobigU.m128 = _mm_or_ps(istoobigX, istoobigY);
	__m128i istoobig = istoobigU.m128i;

	__m128i toomanycounts = _mm_cmpgt_epi32(Counts, _mm_set1_epi32(UnseenBasinSettling));
	union {__m128 m128; __m128i m128i;} needsnewU; needsnewU.m128i = _mm_or_si128(toomanycounts, istoobig);
	__m128 needsnew = needsnewU.m128;

	TotalComputed = _mm_add_epi32(TotalComputed, _mm_and_si128( allNumberOne, needsnewU.m128i));
	Howmanyescaped = _mm_add_epi32(Howmanyescaped, _mm_and_si128( allNumberOne, istoobig));
	Counts = _mm_andnot_si128(needsnewU.m128i, Counts); //if needsnew is true, set counts to 0

	// if needsnew[0] != 0 || needsnew[1] != 0 || needsnew[2] != 0 ... {
	curx = _mm_add_ps(curx, _mm_and_ps( needsnew, _mm_set1_ps(dx)));
	__m128 iscurxlocationtoobig = _mm_cmpge_ps(curx, _mm_set1_ps(UnseenBasinX1));
	cury = _mm_add_ps(cury, _mm_and_ps( iscurxlocationtoobig, _mm_set1_ps(dy * 4))); //skip ahead 4 at once
	__m128 iscurylocationtoobig = _mm_cmpge_ps(cury, _mm_set1_ps(UnseenBasinY1));
	if (iscurylocationtoobig.m128_i32[0] != 0 || iscurylocationtoobig.m128_i32[0] != 0 || iscurylocationtoobig.m128_i32[0] != 0
		|| iscurylocationtoobig.m128_i32[0] != 0)
		goto outsideloop;
	mmX = _mm_or_ps( _mm_and_ps(needsnew,curx), _mm_andnot_ps(needsnew,mmX)); //with no if conditional!
	mmY = _mm_or_ps( _mm_and_ps(needsnew,cury), _mm_andnot_ps(needsnew,mmY)); //with no if conditional!
	// }

	MAPSSE;

}
outsideloop:

int totalC = TotalComputed.m128i_i32[0]+TotalComputed.m128i_i32[1]+TotalComputed.m128i_i32[2]+TotalComputed.m128i_i32[3];
int escaped = Howmanyescaped.m128i_i32[0]+Howmanyescaped.m128i_i32[1]+Howmanyescaped.m128i_i32[2]+Howmanyescaped.m128i_i32[3];
printf("o%d escaped%d\n", totalC, escaped );
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

