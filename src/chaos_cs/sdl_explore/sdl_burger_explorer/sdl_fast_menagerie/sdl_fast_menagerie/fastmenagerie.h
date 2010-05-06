#include <xmmintrin.h>
#include <emmintrin.h>

void InitialSettings(MenagFastSettings*ps, int width, int height, double *pa, double *pb);
extern BOOL g_BusyThread1, g_BusyThread2;
void constructMenagerieSurface(MenagFastSettings*ps, SDL_Surface* pSmallSurface);
void startMenagCalculation(MenagFastSettings*ps, int direction, SDL_PixelFormat * pixelFormat);

#define henonSetup \
	_m128 mConstOne = _mm_set1_ps(1.0f); \
	__m128 mmConstNegA = _mm_set1_ps(-c1); \
	__m128 mmConstB = _mm_set1_ps(c2);

#define henonExpression \
		mXTmp = _mm_mul_ps(mmX, mmX); \
		mXTmp = _mm_mul_ps(mXTmp, mmConstNegA); \
		mXTmp = _mm_add_ps(mXTmp, mmY); \
		mXTmp = _mm_add_ps(mXTmp, mConstOne); \
		mmY = _mm_mul_ps(mmX, mmConstB); \
		mmX = mXTmp;

#define burgerSetup \
	__m128 mConstCOne = _mm_set1_ps(c1); \
	__m128 mConstCTwo = _mm_set1_ps(c2); \
	__m128 mmTemp;

#define burgerExpression \
		mXTmp = _mm_mul_ps(mConstCOne, mmX); \
		mmTemp = _mm_mul_ps(mmY,mmY); \
		mXTmp = _mm_sub_ps(mXTmp, mmTemp); \
		mmTemp = _mm_add_ps(mmX, mConstCTwo);  \
		mmY = _mm_mul_ps(mmY, mmTemp); \
		mmX = mXTmp;




void BlitMenagerie(SDL_Surface* pSurface,SDL_Surface* pSmallSurface);
