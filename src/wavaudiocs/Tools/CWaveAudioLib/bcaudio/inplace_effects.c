
#include "bcaudio.h"
#include "inplace_effects.h"

void inplaceeffect_volume_impl(double * data, int length, double factor)
{
	if (data==NULL) return;
	int i; for (i=0; i<length;i++)
		data[i] = data[i]*factor;
}
void inplaceeffect_volume(CAudioData* this, double factor)
{
	inplaceeffect_volume_impl(this->data,this->length, factor);
	inplaceeffect_volume_impl(this->data_right,this->length, factor);
}

void inplaceeffect_reverse_impl(double * data, int length)
{
	if (data==NULL) return;
	int i; for (i=0; i<length/2;i++)
	{
		double first=data[i]; double second=data[length-i];
		data[length-i] = first; data[i] = second;
	}
}
void inplaceeffect_reverse(CAudioData* this)
{
	inplaceeffect_reverse_impl(this->data,this->length);
	inplaceeffect_reverse_impl(this->data_right,this->length);
}

//Linear fade in and fade out
void inplaceeffect_fade_impl(double * data, int length,  int inOrOut, double fSeconds, double lengthInSeconds)
{
	if (data==NULL) return;
	if (fSeconds > lengthInSeconds) fSeconds = lengthInSeconds;
	if (fSeconds < 0) fSeconds = 0;
	int nFadeSamples = (int) length*(fSeconds/lengthInSeconds);
	double fScale = 1 / (double)nFadeSamples;
	
	if (inOrOut==1) //fade in
	{
		int i; for (i=0; i<nFadeSamples;i++)
			data[i] *= fScale*i;
	}
	else //fade out
	{
		int i; for (i=length-nFadeSamples; i<length;i++)
			data[i] *= fScale* (length-i);
	}
}
void inplaceeffect_fade(CAudioData* this, int inOrOut, double fSeconds)
{
	double myLengthInSeconds = this->length / ((double) this->sampleRate);
	inplaceeffect_fade_impl(this->data,this->length, inOrOut, fSeconds, myLengthInSeconds);
	inplaceeffect_fade_impl(this->data_right,this->length, inOrOut, fSeconds, myLengthInSeconds);
}

//Linear fade in and fade out
void inplaceeffect_tremelo_impl(double * data, int length, double tremeloFreqScale, double amp)
{
	if (data==NULL) return;
	int i; for (i = 0; i < length; i++)
	{
		double val = data[i] * (1 + amp * sin(tremeloFreqScale * i));
		if (val > 1.0) val = 1.0;
		else if (val < -1.0) val = -1.0;
		data[i] = val;
	}
}
// Common values: 1.0, 0.2. tremfreq is in Hz, amp is the strength of the effect.
void inplaceeffect_tremelo(CAudioData* this, double tremfreq, double amp)
{
	double tremeloFreqScale = 2.0 * PI * tremfreq / (double)this->sampleRate;
	inplaceeffect_tremelo_impl(this->data,this->length, tremeloFreqScale, amp);
	inplaceeffect_tremelo_impl(this->data_right,this->length, tremeloFreqScale, amp);
}


