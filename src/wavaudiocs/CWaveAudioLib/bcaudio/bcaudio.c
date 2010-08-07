

#include "bcaudio.h"


CAudioData* caudiodata_new()
{
	// one must assign to ALL members.
	CAudioData* ret = (CAudioData*) malloc(sizeof(CAudioData));
	ret->data = ret->data_right = NULL;
	ret->sampleRate = 44100;
	ret->length = 0;
	return ret;
}

void caudiodata_dispose(CAudioData* audio)
{
	assert_ptr(audio);
	free(audio->data); //remember that freeing NULL is completely ok
	free(audio->data_right);
	audio->data = audio->data_right = NULL;
	free(audio);
	audio = NULL;
}

// only allocates, does not set to zero.
errormsg caudiodata_allocate(CAudioData* this, int nSamples, int nChannels, int nSampleRate)
{
	assert_ptr(this); assert_gtr0(nSamples); assert_gtr0(nSampleRate);
	if (!(nChannels==1||nChannels==2)) ftl_exit("Number of channels not supported.");
	
	free(this->data); free(this->data_right); //remember that freeing NULL is completely ok
	this->data = (double*) malloc(nSamples * sizeof(double));
	assert_ptr(this->data);
	if (nChannels == 2)
	{
		this->data_right = (double*) malloc(nSamples * sizeof(double));
		assert_ptr(this->data_right);
	}
	this->length = nSamples;
	this->sampleRate = nSampleRate;
	
	return OK; //always return this. ftl_exit if fails.
}

// Caller responsible for calling caudiodata_dispose on the result!
errormsg caudiodata_clone(CAudioData** out, CAudioData* this)
{
	assert_ptr(this);
	
	CAudioData* audio;
	audio = *out = caudiodata_new();
	
	errormsg msg = caudiodata_allocate(audio, this->length, NUMCHANNELS(this), this->sampleRate);
	if (msg!=OK) return msg;
	
	memcpy(audio->data, this->data, this->length*sizeof(double));
	if (this->data_right!=NULL)
		memcpy(audio->data_right, this->data_right, this->length*sizeof(double));
	
	return OK;
}

double caudiodata_getLengthInSecs(CAudioData* this)
{
	assert_ptr(this);
	if (this->data==NULL) return 0;
	return this->length / (double) this->sampleRate;
}

void ftl_exit(char * msg)
{
	fputs(msg, stderr); 
	exit(1);
}

void ftl_fail_assert(const char * msg, const char*file, const char* fnname, int lineno)
{
	fprintf(stderr, "assertion failed. file %s line %d function %s \n", file,lineno,fnname );
	fputs(msg, stderr);
	exit(1);
}


//todo:
//next: add debug system to catch mem leaks. limit malloc calls to one place
//validate all parameters, before allocating.
//possible memory leak in the synths?
//audio = *out = caudiodata_new(); //use audio as an alias for the output, *out.
//if (lengthSeconds<0) return "Invalid length"; if (freq<=0) return "Invalid frequency";
//user might not know to dispose after error
//	dispose always, even on error

//note that caudiodata_allocate always returns OK?

// Nasca Octavian Paul- same guy who made ZynAddSubEffects
//todo: normalize inputs to wah wah, phaser. Perhaps all parameters doubles from 0 to 1

//it is a good habit to set all output ptrs to null. Foo*x=NULL; CreateFoo(&x); so if error occurs, can check value of x.

