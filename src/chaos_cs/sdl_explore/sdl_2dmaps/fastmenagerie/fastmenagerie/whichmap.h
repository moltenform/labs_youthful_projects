#include <xmmintrin.h>
#include <emmintrin.h>

#define HENON_SSE_INIT \
	_m128 mConstOne = _mm_set1_ps(1.0f); \
	__m128 mmConstNegA = _mm_set1_ps(-c1); \
	__m128 mmConstB = _mm_set1_ps(c2);
#define HENON_SSE \
		mXTmp = _mm_mul_ps(mmX, mmX); \
		mXTmp = _mm_mul_ps(mXTmp, mmConstNegA); \
		mXTmp = _mm_add_ps(mXTmp, mmY); \
		mXTmp = _mm_add_ps(mXTmp, mConstOne); \
		mmY = _mm_mul_ps(mmX, mmConstB); \
		mmX = mXTmp;
#define BURGER_SSE_INIT \
	__m128 mConstCOne = _mm_set1_ps(c1); \
	__m128 mConstCTwo = _mm_set1_ps(c2); \
	__m128 mmTemp;
#define BURGER_SSE \
		mXTmp = _mm_mul_ps(mConstCOne, mmX); \
		mmTemp = _mm_mul_ps(mmY,mmY); \
		mXTmp = _mm_sub_ps(mXTmp, mmTemp); \
		mmTemp = _mm_add_ps(mmX, mConstCTwo);  \
		mmY = _mm_mul_ps(mmY, mmTemp); \
		mmX = mXTmp;

#define HENON x_ = 1 - c1*x*x + y; y_ = c2*x; 
#define HENONSUF "_henon"
#define BURGER x_ = c1*x - y*y; y_ = c2*y + x*y;
#define BURGERSUF "_b"

#define MAPEXPRESSION BURGER
#define MAPSUFFIX BURGERSUF
#define MAPSSEINIT BURGER_SSE_INIT
#define MAPSSE BURGER_SSE

#define SAVESFOLDER ("saves" MAPSUFFIX  )
#define MAPDEFAULTFILE ("saves" MAPSUFFIX "/default.cfg" )

#define STRINGIFY2( x) #x
#define STRINGIFY(x) STRINGIFY2(x)
#define MAPEXPRESSIONTEXT STRINGIFY(MAPEXPRESSION)

//#define CountPixelsSettle 160
//#define CountPixelsDraw 80
#define CountPixelsSettle 160
#define CountPixelsDraw 80
#define CountPixelsSeedsPerAxis 10
