// strangely enough, these can be implemented as in-place effects

// phaser from Audacity by Nasca Octavian Paul

/*
 freq       - Phaser's LFO frequency
 startphase - Phaser's LFO startphase (radians), needed for stereo Phasers
 depth      - Phaser depth (0 - no depth, 255 - max depth)
 stages     - Phaser stages (recomanded from 2 to 16-24, and EVEN NUMBER)
 drywet     - Dry/wet mix, (0 - dry, 128 - dry=wet, 255 - wet)
 fb         - Phaser FeedBack (0 - no feedback, 100 = 100% Feedback, -100 = -100% FeedBack)
*/

#include "bcaudio.h"
#include "effects_audacity.h"

// How many samples are processed before compute the lfo value again
#define fxphaseraudlfoskipsamples 20
#define fxphaseraudlfoshape 4.0
#define fxphaseraudMAX_STAGES 24
void effect_phaseraud_impl(int length, double* data, double freq_scaled, double startphase, double fb, int depth, int stages, int drywet)
{
	if (data==NULL) return;
	// state variables	
	unsigned long skipcount;
	double old[fxphaseraudMAX_STAGES];
	double gain;
	double fbout;
	double lfoskip;
	double phase;
	
	// initialize state variables
	int i, j;
	for (j = 0; j < stages; j++) old[j] = 0;   
	skipcount = 0;
	gain = 0;
	fbout = 0;
	lfoskip = freq_scaled; //not in Hz, already converted.
	phase = startphase;
	
	double m, tmp, in, out;
	for (i = 0; i < length; i++)
	{
		in = data[i];

		m = in + fbout * fb / 100;
		if (((skipcount++) % fxphaseraudlfoskipsamples) == 0) 
		{
			gain = (1 + cos(skipcount * lfoskip + phase)) / 2; //compute sine between 0 and 1
			gain = (exp(gain * fxphaseraudlfoshape) - 1) / (exp(fxphaseraudlfoshape)-1); // change lfo shape
			gain = 1 - gain / 255 * depth;      // attenuate the lfo
		}
		// phasing routine
		for (j = 0; j < stages; j++)
		{
			tmp = old[j];
			old[j] = gain * tmp + m;
			m = tmp - gain * old[j];
		}
		fbout = m;
		out = (m * drywet + in * (255 - drywet)) / 255;
		
		if (out < -1.0) out = -1.0; // Prevent clipping
		else if (out > 1.0) out = 1.0;
		data[i] = out;
	}
	
}

errormsg effect_phaseraud(CAudioData* this, double freq, double fb, int depth, int stages, int drywet)
{
	if (stages > fxphaseraudMAX_STAGES) return "Too many stages"; if (stages % 2 != 0) return "Stages should be even.";
	double startphaseleft = 0;
	double startphaseright = startphaseleft+PI; //note that left and right channels should start pi out of phase
	double freq_scaled = 2.0 * PI * freq / (double)this->sampleRate;
	
	effect_phaseraud_impl(this->length,this->data,freq_scaled, startphaseleft, fb, depth, stages, drywet);
	effect_phaseraud_impl(this->length,this->data_right,freq_scaled, startphaseright, fb, depth, stages, drywet);
	return OK;
}


 
/* Parameters:
   freq - LFO frequency 
   startphase - LFO startphase in RADIANS - usefull for stereo WahWah
   depth - Wah depth
   freqofs - Wah frequency offset
   res - Resonance

   !!!!!!!!!!!!! IMPORTANT!!!!!!!!! :
   depth and freqofs should be from 0(min) to 1(max) !
   res should be greater than 0 !  */
#define fxwahwahaudlfoskipsamples 30
void effect_wahwahaud_impl(int length, double* data, double startphase, double freq_scaled, double depth, double freqofs, double res)
{
	if (data==NULL) return;
	// state variables
	double phase;
	double lfoskip;
	unsigned long skipcount;
	double xn1, xn2, yn1, yn2;
	double b0, b1, b2, a0, a1, a2;

	// initialize variables
	lfoskip = freq_scaled; //already converted from Hz
	skipcount = 0;
	xn1 = 0; xn2 = 0; yn1 = 0; yn2 = 0; b0 = 0;  b1 = 0; b2 = 0; a0 = 0; a1 = 0;  a2 = 0;
	phase = startphase;

	double frequency, omega, sn, cs, alpha;
	double in, out;
	int i;
	for (i = 0; i < length; i++)
	{
		in = data[i];

		if ((skipcount++) % fxwahwahaudlfoskipsamples == 0)
		{
			frequency = (1 + cos(skipcount * lfoskip + phase)) / 2;
			frequency = frequency * depth * (1 - freqofs) + freqofs;
			frequency = exp((frequency - 1) * 6);
			omega = PI * frequency;
			sn = sin(omega);
			cs = cos(omega);
			alpha = sn / (2 * res);
			b0 = (1 - cs) / 2;
			b1 = 1 - cs;
			b2 = (1 - cs) / 2;
			a0 = 1 + alpha;
			a1 = -2 * cs;
			a2 = 1 - alpha;
		}
		out = (b0 * in + b1 * xn1 + b2 * xn2 - a1 * yn1 - a2 * yn2) / a0;
		xn2 = xn1;
		xn1 = in;
		yn2 = yn1;
		yn1 = out;
		
		if (out < -1.0) out = -1.0; // Prevent clipping
		else if (out > 1.0) out = 1.0;
		data[i] = out;
	}
}
  
errormsg effect_wahwahaud(CAudioData* this, double freq, double depth, double freqofs, double res)
{
	double startphaseleft = 0;
	double startphaseright = startphaseleft+PI; //note that left and right channels should start pi out of phase
	double freq_scaled = 2.0 * PI * freq / (double)this->sampleRate;
	
	effect_wahwahaud_impl(this->length,this->data,startphaseleft, freq_scaled, depth, freqofs, res);
	effect_wahwahaud_impl(this->length,this->data_right,startphaseright, freq_scaled, depth, freqofs, res);
	return OK;
}



