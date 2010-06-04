
#include <xmmintrin.h>
#include <emmintrin.h>

#define PHASESIZE 256

float *seedxs=NULL;
float *seedys=NULL;

#define SEEDSPERAXIS 6

	float DD = 0.4f;

void constructSeeds()
{
	seedxs = (float *) malloc(sizeof(float)*SEEDSPERAXIS*SEEDSPERAXIS);
	seedys = (float *) malloc(sizeof(float)*SEEDSPERAXIS*SEEDSPERAXIS);
	float BBB = 4;
	float sx0=-BBB, sx1 = BBB, sy0=-BBB, sy1=BBB;  //NOTE: symmetric maps could be optimized here?
	int nSeedsPerAxis = SEEDSPERAXIS; //MUST BE > 1
	float xincr, yincr; xincr = yincr = (sx1-sx0)/(nSeedsPerAxis-1);
	for (int i=0; i<nSeedsPerAxis*nSeedsPerAxis; i++)
	{
		int ind1 = i/nSeedsPerAxis;
		int ind2 = i%nSeedsPerAxis;
		seedxs[i] = sx0 + xincr*ind1;
		seedys[i] = sy0 + xincr*ind2;
	}
}


int _drawPhasePortraitVector(MenagerieSettings* settings, double c1d, double c2d, int arrAns[])
{
	if (seedxs==NULL) constructSeeds();
	float c1= (float) c1d, c2 = (float) c2d;
	CURRENTID++;
	//ASSUMPTION: only one attractor will be noteworthy. If there are 2, only sees 1.
	int totalUniquePoints = 0; 
	//Don't clear array
	//int arrWidth=settings->phaseFigureWidth; int arrHeight=settings->phaseFigureHeight;
	//memset(arrAns, 0, sizeof(int)*settings->phaseFigureWidth*settings->phaseFigureHeight);
	float X0=(float)settings->phasex0, X1=(float)settings->phasex1, Y0=(float)settings->phasey0, Y1=(float)settings->phasey1;
	float x, y, x_;

	int howmanyseedsDone=0; int currentseed=7 /*can be any*/; int inc=7; //must be relatively prime with # of seeds, 16. n/2-1 is good.
	//first step: find one that's in the attractor. Say if it doesn't escape after 5, then bounded...
	while (howmanyseedsDone<SEEDSPERAXIS*SEEDSPERAXIS)
	{
		x=seedxs[currentseed]; y=seedxs[currentseed];
		MAPEXPRESSION; x=x_;
		MAPEXPRESSION; x=x_;
		MAPEXPRESSION; x=x_;
		MAPEXPRESSION; x=x_;
		MAPEXPRESSION; x=x_;

		if (ISTOOBIG(x)||ISTOOBIGF(y)) {
		howmanyseedsDone++; currentseed = (currentseed+inc)%(SEEDSPERAXIS*SEEDSPERAXIS);
		}
		else
			break;
	}
	if (howmanyseedsDone>=16) return 0;

#if PHASESIZE!=256
#error "needs to be 256"
#endif
	// nudge it.
	__m128 mmX;
	__m128 mmY;
	__m128 mXTmp;
	__m128 mConstOne = _mm_set1_ps(1.0f);
	__m128 mmConstNegA = _mm_set1_ps(-c1);
	__m128 mmConstB = _mm_set1_ps(c2);
	mmX = _mm_setr_ps( x, x+DD,x,x-DD); //MAKING IT + or - int he last one changes things!!!
	mmY = _mm_setr_ps( y, y,y+DD,y-DD);
//return currentseed + (int)x + (int)mmX.m128_f32[0] ;//

	//second step: draw lots of orbits for this one.
	for (int ii=0; ii<17 /*(140/8)*/; ii++) // was settings->settling - 10, that's just as arbitrary
	{
		// slightly faster to loop unroll like this...
		mXTmp = _mm_mul_ps(mmX, mmX); mXTmp = _mm_mul_ps(mXTmp, mmConstNegA);
		mXTmp = _mm_add_ps(mXTmp, mmY); mXTmp = _mm_add_ps(mXTmp, mConstOne); 
		mmY = _mm_mul_ps(mmX, mmConstB); mmX = mXTmp;
		mXTmp = _mm_mul_ps(mmX, mmX); mXTmp = _mm_mul_ps(mXTmp, mmConstNegA);
		mXTmp = _mm_add_ps(mXTmp, mmY); mXTmp = _mm_add_ps(mXTmp, mConstOne); 
		mmY = _mm_mul_ps(mmX, mmConstB); mmX = mXTmp;
		mXTmp = _mm_mul_ps(mmX, mmX); mXTmp = _mm_mul_ps(mXTmp, mmConstNegA);
		mXTmp = _mm_add_ps(mXTmp, mmY); mXTmp = _mm_add_ps(mXTmp, mConstOne); 
		mmY = _mm_mul_ps(mmX, mmConstB); mmX = mXTmp;
		mXTmp = _mm_mul_ps(mmX, mmX); mXTmp = _mm_mul_ps(mXTmp, mmConstNegA);
		mXTmp = _mm_add_ps(mXTmp, mmY); mXTmp = _mm_add_ps(mXTmp, mConstOne); 
		mmY = _mm_mul_ps(mmX, mmConstB); mmX = mXTmp;
		mXTmp = _mm_mul_ps(mmX, mmX); mXTmp = _mm_mul_ps(mXTmp, mmConstNegA);
		mXTmp = _mm_add_ps(mXTmp, mmY); mXTmp = _mm_add_ps(mXTmp, mConstOne); 
		mmY = _mm_mul_ps(mmX, mmConstB); mmX = mXTmp;
		mXTmp = _mm_mul_ps(mmX, mmX); mXTmp = _mm_mul_ps(mXTmp, mmConstNegA);
		mXTmp = _mm_add_ps(mXTmp, mmY); mXTmp = _mm_add_ps(mXTmp, mConstOne); 
		mmY = _mm_mul_ps(mmX, mmConstB); mmX = mXTmp;
		mXTmp = _mm_mul_ps(mmX, mmX); mXTmp = _mm_mul_ps(mXTmp, mmConstNegA);
		mXTmp = _mm_add_ps(mXTmp, mmY); mXTmp = _mm_add_ps(mXTmp, mConstOne); 
		mmY = _mm_mul_ps(mmX, mmConstB); mmX = mXTmp;
		mXTmp = _mm_mul_ps(mmX, mmX); mXTmp = _mm_mul_ps(mXTmp, mmConstNegA);
		mXTmp = _mm_add_ps(mXTmp, mmY); mXTmp = _mm_add_ps(mXTmp, mConstOne); 
		mmY = _mm_mul_ps(mmX, mmConstB); mmX = mXTmp;
	}

	//third step: drawing!
	//LET's OPTIMIZE DRAWING!!!!!!!
	float CW = 1.0f/(X1-X0);
	__m128 mMultW = _mm_set1_ps(CW * PHASESIZE * PHASESIZE);  //EXTRA FACTOR
	int ShiftW = (int) (-X0 * PHASESIZE * CW * PHASESIZE); //if -3 to 3, this shifts by +128 (half the arrWidth). 
										//This is sometimes an approximation, but ok due to speed
	float CH = 1.0f/(Y1-Y0);
	__m128 mMultH = _mm_set1_ps(CH * PHASESIZE ); 
	int ShiftH = (int) (-Y0 * PHASESIZE * CH );
	__m128 xPrelim, yPrelim; //use xTmp as xPrelim. apparently the compiler does this for us.

__m128i mmShiftW = _mm_set1_epi32(ShiftW);
__m128i mmShiftH = _mm_set1_epi32(ShiftH);
// tried moving this into memory (someMemX), and then converting to points, to ease register pressure, but didn't help.
	for (int ii=0; ii<8 /*(settings->drawing)*/; ii++) //loop unrolling this body a bit didn't seem to help.
		//TODO: unroll this loop. now that using more vector expressions, code size is smaller, helps.
	{
		mXTmp = _mm_mul_ps(mmX, mmX); mXTmp = _mm_mul_ps(mXTmp, mmConstNegA);
		mXTmp = _mm_add_ps(mXTmp, mmY); mXTmp = _mm_add_ps(mXTmp, mConstOne); 
		mmY = _mm_mul_ps(mmX, mmConstB); mmX = mXTmp;
		xPrelim = _mm_mul_ps(mmX, mMultW);//use that THEN DO INTS, PACK 4 INTS INTO AN MM128!!
		yPrelim = _mm_mul_ps(mmY, mMultH);  //tried not using this, so only using 8 mm128 regs, but was slower
		
		// confirmed that these don't actually introduce a new register. compiler does that much.
		__m128i xPt = _mm_cvttps_epi32(xPrelim); //cast all to int at once. specifies truncate mode. (see if mode matters)
		__m128i yPt = _mm_cvttps_epi32(yPrelim); //is _mm_cvtps_epi32 or _mm_cvttps_epi32 faster? truncate or not? not sure, close.
		xPt = _mm_add_epi32(xPt, mmShiftW);
		yPt = _mm_add_epi32(yPt, mmShiftH);
		__m128i xySum = _mm_add_epi32(xPt, yPt); //confirmed that this was worth doing, even if we don't always use it.
		
		//tried setting this to index, and 0 if conditional false, to perhaps improve pipeline, but hurt slightly. branch prediction might help
		if (yPt.m128i_i32[0] >= 0 && yPt.m128i_i32[0] < PHASESIZE && xPt.m128i_i32[0]>=0 && xPt.m128i_i32[0]<(PHASESIZE* (PHASESIZE-1))) { 
			if (arrAns[xySum.m128i_i32[0]]!=CURRENTID)
		    { arrAns[xySum.m128i_i32[0]]=CURRENTID; totalUniquePoints++;}
		}
		if (yPt.m128i_i32[1] >= 0 && yPt.m128i_i32[1] < PHASESIZE && xPt.m128i_i32[1]>=0 && xPt.m128i_i32[1]<(PHASESIZE* (PHASESIZE-1))) { 
			if (arrAns[xySum.m128i_i32[1]]!=CURRENTID)
		    { arrAns[xySum.m128i_i32[1]]=CURRENTID; totalUniquePoints++;}
		}
		if (yPt.m128i_i32[2] >= 0 && yPt.m128i_i32[2] < PHASESIZE && xPt.m128i_i32[2]>=0 && xPt.m128i_i32[2]<(PHASESIZE* (PHASESIZE-1))) { 
			if (arrAns[xySum.m128i_i32[2]]!=CURRENTID)
		    { arrAns[xySum.m128i_i32[2]]=CURRENTID; totalUniquePoints++;}
		}
		if (yPt.m128i_i32[3] >= 0 && yPt.m128i_i32[3] < PHASESIZE && xPt.m128i_i32[3]>=0 && xPt.m128i_i32[3]<(PHASESIZE* (PHASESIZE-1))) { 
			if (arrAns[xySum.m128i_i32[3]]!=CURRENTID)
		    { arrAns[xySum.m128i_i32[3]]=CURRENTID; totalUniquePoints++;}
		}

		/////////////////////// PT 2////////////
mXTmp = _mm_mul_ps(mmX, mmX); mXTmp = _mm_mul_ps(mXTmp, mmConstNegA);
		mXTmp = _mm_add_ps(mXTmp, mmY); mXTmp = _mm_add_ps(mXTmp, mConstOne); 
		mmY = _mm_mul_ps(mmX, mmConstB); mmX = mXTmp;
		xPrelim = _mm_mul_ps(mmX, mMultW);//use that THEN DO INTS, PACK 4 INTS INTO AN MM128!!
		yPrelim = _mm_mul_ps(mmY, mMultH);  //tried not using this, so only using 8 mm128 regs, but was slower
		
		 xPt = _mm_cvttps_epi32(xPrelim); //cast all to int at once. specifies truncate mode. (see if mode matters)
		 yPt = _mm_cvttps_epi32(yPrelim); //is _mm_cvtps_epi32 or _mm_cvttps_epi32 faster? truncate or not? not sure, close.
		xPt = _mm_add_epi32(xPt, mmShiftW);
		yPt = _mm_add_epi32(yPt, mmShiftH);
		 xySum = _mm_add_epi32(xPt, yPt); //confirmed that this was worth doing, even if we don't always use it.
		
		//tried setting this to index, and 0 if conditional false, to perhaps improve pipeline, but hurt slightly. branch prediction might help
		if (yPt.m128i_i32[0] >= 0 && yPt.m128i_i32[0] < PHASESIZE && xPt.m128i_i32[0]>=0 && xPt.m128i_i32[0]<(PHASESIZE* (PHASESIZE-1))) { 
			if (arrAns[xySum.m128i_i32[0]]!=CURRENTID)
		    { arrAns[xySum.m128i_i32[0]]=CURRENTID; totalUniquePoints++;}
		}
		if (yPt.m128i_i32[1] >= 0 && yPt.m128i_i32[1] < PHASESIZE && xPt.m128i_i32[1]>=0 && xPt.m128i_i32[1]<(PHASESIZE* (PHASESIZE-1))) { 
			if (arrAns[xySum.m128i_i32[1]]!=CURRENTID)
		    { arrAns[xySum.m128i_i32[1]]=CURRENTID; totalUniquePoints++;}
		}
		if (yPt.m128i_i32[2] >= 0 && yPt.m128i_i32[2] < PHASESIZE && xPt.m128i_i32[2]>=0 && xPt.m128i_i32[2]<(PHASESIZE* (PHASESIZE-1))) { 
			if (arrAns[xySum.m128i_i32[2]]!=CURRENTID)
		    { arrAns[xySum.m128i_i32[2]]=CURRENTID; totalUniquePoints++;}
		}
		if (yPt.m128i_i32[3] >= 0 && yPt.m128i_i32[3] < PHASESIZE && xPt.m128i_i32[3]>=0 && xPt.m128i_i32[3]<(PHASESIZE* (PHASESIZE-1))) { 
			if (arrAns[xySum.m128i_i32[3]]!=CURRENTID)
		    { arrAns[xySum.m128i_i32[3]]=CURRENTID; totalUniquePoints++;}
		}
	}
	if (ISTOOBIGF(mmX.m128_f32[0]) || ISTOOBIGF(mmX.m128_f32[1]) ||ISTOOBIGF(mmX.m128_f32[2])|| ISTOOBIGF(mmX.m128_f32[3]) ||
	 ISTOOBIGF(mmY.m128_f32[0]) || ISTOOBIGF(mmY.m128_f32[1]) ||ISTOOBIGF(mmY.m128_f32[2])|| ISTOOBIGF(mmY.m128_f32[3]))
	 printf("no...");
	return totalUniquePoints;
}
