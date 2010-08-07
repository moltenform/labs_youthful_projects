
#include <stdio.h>
#include <math.h>
#include <stdlib.h> //used by rand()
#include <time.h> //used by rand(), for srand
#include <string.h> //defines memcpy()

#define SampleRate 44100

typedef int bool;
#ifndef TRUE
#define TRUE (1)
#endif
#ifndef FALSE
#define FALSE (0)
#endif

typedef char* errormsg;
#define OK 0

typedef struct
{
	double *data;
	double *data_right; //this is null if the audio is mono
	int sampleRate;
	int length; //length in samples.
} CAudioData;


// functions from bcaudio.c
CAudioData* caudiodata_new();
void caudiodata_dispose(CAudioData* audio);
errormsg caudiodata_allocate(CAudioData* this, int nSamples, int nChannels, int nSampleRate);
errormsg caudiodata_clone(CAudioData** out, CAudioData* this);
double caudiodata_getLengthInSecs(CAudioData* this);

//pseudo-random number generation
//* returns random floating point value in the range [0,1) {including 0, not including 1}.
#define STARTRAND() (srand ( time(NULL) ))
#define NEXTDOUBLE() ((double)rand() / ((double)(RAND_MAX)+(double)(1)) )

void ftl_exit(char * msg);
void ftl_fail_assert(const char * msg, const char*file, const char* fnname, int lineno );

#define NUMCHANNELS(this) ((this->data_right == NULL)?1:2)

#define assert_ptr(p) {if (!(p)) ftl_fail_assert("unexpected null ptr.",__FILE__,__func__,__LINE__);}
#define assert_null(p) {if (!(p)) ftl_fail_assert("out-parameter should be set to null.",__FILE__,__func__,__LINE__);}
#define assert_gtr0(p) {if (!(p)) ftl_fail_assert("bad parameter (negative number) passed to function.",__FILE__,__func__,__LINE__);}

#include <limits.h>
#define  SHORT_MAX  SHRT_MAX
#define  SHORT_MIN  SHRT_MIN

typedef unsigned int uint;
typedef unsigned short ushort;
typedef char byte;

#define PI 3.1415926535 




