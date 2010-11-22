
#include "bcaudio.h"
#include "synth.h"



typedef double(*OscillatorFn)(double);

int synth_changeSquareState;
double synth_rednoiseState;
double synth_rednoise_factor;

errormsg synth_periodicsynth(CAudioData**out,OscillatorFn fn,bool purelyPeriodic, double freq, double lengthSeconds, double amp)
{
	CAudioData* audio;
	audio = *out = caudiodata_new(); //use audio as an alias for the output, *out.

	if (lengthSeconds<0) return "Invalid length"; if (freq<=0) return "Invalid frequency";
	int length = (int)(lengthSeconds * SampleRate);
	errormsg msg = caudiodata_allocate(audio, length, 1, SampleRate);
	if (msg!=OK) return msg;
		
	double timeScale = 2.0 * PI * freq / (double)SampleRate;
	int waveformPeriod = (int)(SampleRate / freq);
	
	if (purelyPeriodic)
	{
		// could be as simple as int i; for(i=0; i<length; i++) out->data[i]= sin(timeScale*i)*amp; 
		// Because this is periodic, though, we can optimize. (Comes at the cost of a very-slightly inaccurate frequency due to rounding waveform period to integer).
		int i; for (i = 0; i < waveformPeriod; i++)
		{
			audio->data[i] = fn(i * timeScale) * amp;
		}
		for (i = waveformPeriod; i < length; i++)
		{
			audio->data[i] = audio->data[i % waveformPeriod];
		}
	}
	else
	{
		int i; 
		for (i = 0; i < length; i++)
			audio->data[i] = fn(i * timeScale) * amp;
	}
	
	return OK;
}
double synth_frequencyFromMidiNote(int n) {	return 8.1758 * pow(2.0, (n / 12.0)); }

////////////////////////////////////////////////////////////
errormsg synth_sin(CAudioData**out, double freq, double length, double amp)
{
	return synth_periodicsynth(out, (OscillatorFn)sin, 1,freq, length, amp);
}

double synth_square_impl(double v)
{
	return (fmod(v , (2*PI)) > PI ? 1.0 : -1.0);
}
errormsg synth_square(CAudioData**out, double freq, double length, double amp)
{
	return synth_periodicsynth(out, (OscillatorFn)synth_square_impl, 1,freq, length, amp);
}

double synth_sawtooth_impl(double x)
{
	x = fmod(x , (PI * 2));
	return ((x / (PI * 2)) * 2) - 1;
}
errormsg synth_sawtooth(CAudioData**out, double freq, double length, double amp)
{
	return synth_periodicsynth(out, (OscillatorFn)synth_sawtooth_impl, 1,freq, length, amp);
}

double synth_triangle_impl(double x)
{
	x = fmod(x , (PI * 2));
	x = x / (PI*2); // now x goes from 0 to 1
	if (x > 0.5)
		return -1 + (x - 0.5) * 4;
	else
		return 1 - (x) * 4;
}
errormsg synth_triangle(CAudioData**out, double freq, double length, double amp)
{
	return synth_periodicsynth(out, (OscillatorFn)synth_triangle_impl, 1,freq, length, amp);
}

double synth_circle_impl(double x)
{
	x = fmod(x , (PI * 2));
	double qtrperiod = 2*PI*0.25, halfperiod = 2*PI*0.5;
	if (x < halfperiod)
		return sqrt(1 - pow(((x) / qtrperiod - 1), 2.0));
	else
		return -1 * sqrt(1 - pow((((x) - halfperiod) / qtrperiod - 1), 2.0));
}
errormsg synth_circle(CAudioData**out, double freq, double length, double amp)
{
	return synth_periodicsynth(out, (OscillatorFn)synth_circle_impl, 1,freq, length, amp);
}

// note: has a state variable, synth_changeSquareState. Not thread safe.
double synth_square_change_impl(double x)
{
	synth_changeSquareState ++;
	double fx_freq = 0.25;
	x = fmod(x , (PI * 2));
	
	double cutoff = sin(synth_changeSquareState*(fx_freq*2.0*PI / (SampleRate)))*0.48 + 0.5;
	return (x > cutoff * 2 * PI) ? 1 : -1;
	
	/* //morph between triangle and sawtooth wave
	x = x / (PI*2); // now x goes from 0 to 1
	if (x > cutoff)
		return -1 + (x - 0.5) * 4;
	else
		return 1 - (x) * 4;
	*/
}
errormsg synth_square_change(CAudioData**out, double freq, double length, double amp)
{
	synth_changeSquareState = 0;
	// note: pass 0 because it isn't purely periodic.
	return synth_periodicsynth(out, (OscillatorFn)synth_square_change_impl, 0, freq, length, amp);
}


double synth_whitenoise_impl(double x)
{
	double r = NEXTDOUBLE(); 
	return r*2.0 - 1.0;
}
errormsg synth_whitenoise(CAudioData**out, double length, double amp)
{
	STARTRAND();
	// note: pass 0 because it isn't purely periodic.
	return synth_periodicsynth(out, (OscillatorFn)synth_whitenoise_impl, 0, 1.000, length, amp);
}

// note: has State in synth_rednoiseState, synth_rednoise_factor
double synth_rednoise_impl(double x)
{
	synth_rednoiseState+= (NEXTDOUBLE() * 2 - 1.0) * synth_rednoise_factor;
	if (synth_rednoiseState>1.0) synth_rednoiseState = 1.0;
	else if (synth_rednoiseState < -1.0) synth_rednoiseState = -1.0;
	
	return synth_rednoiseState;
}
errormsg synth_rednoise(CAudioData**out, double length, double amp)
{
	STARTRAND();
	synth_rednoiseState = 0;
	synth_rednoise_factor = 0.1; //hard-coded constant, others may sound ok too
	// note: pass 0 because it isn't purely periodic.
	return synth_periodicsynth(out, (OscillatorFn)synth_rednoise_impl, 0,1.000, length, amp);
}

// http://home.earthlink.net/~ltrammell/tech/pinkalg.htm
double synth_pinknoise_av[] = {4.6306e-003,  5.9961e-003,  8.3586e-003};
double synth_pinknoise_pv[] = { 3.1878e-001, 7.7686e-001, 9.7785e-001 };
double synth_pinknoise_randreg[] = { 0,0,0 };
double synth_pinknoise_impl(double x)
{
	double rv = NEXTDOUBLE();
	// Update each generator state per probability schedule
	int i; for (i = 0; i < 3; i++)
	{
		if (rv > synth_pinknoise_pv[i])
			synth_pinknoise_randreg[i] = synth_pinknoise_av[i] * 2 * ((NEXTDOUBLE()) - 0.5);
	}
	// Signal is the sum of the generators
	return (synth_pinknoise_randreg[0] + synth_pinknoise_randreg[1] + synth_pinknoise_randreg[2]) * 30.0;
	
}
errormsg synth_pinknoise(CAudioData**out, double length, double amp)
{
	STARTRAND();
	// Initialize the randomized sources state
	int i; for (i=0;i<3;i++)
		synth_pinknoise_randreg[i] = synth_pinknoise_av[i] * 2 * ((NEXTDOUBLE()) - 0.5);
	
	return synth_periodicsynth(out, (OscillatorFn)synth_pinknoise_impl, 0,1.000, length, amp);
}

errormsg synth_redglitch(CAudioData**out,double freq, double lengthSeconds, double amp, double chunkLength, double rednoisefactor)
{
	STARTRAND();
	
	CAudioData* audio;
	audio = *out = caudiodata_new(); //use audio as an alias for the output, *out.
	if (lengthSeconds<0) return "Invalid length"; if (freq<=0) return "Invalid frequency";
	int length = (int)(lengthSeconds * SampleRate);
	errormsg msg = caudiodata_allocate(audio, length, 1, SampleRate);
	if (msg!=OK) return msg;	
	
	CAudioData* tmp;
	int nChunks = lengthSeconds/chunkLength;
	int i; for (i=0; i<nChunks; i++)
	{
		synth_rednoiseState = 0;
		synth_rednoise_factor = rednoisefactor;
		synth_periodicsynth(&tmp, (OscillatorFn)synth_rednoise_impl, 1,freq, chunkLength, amp);
		
		int offset = (i * chunkLength * (audio->sampleRate));
		memcpy(audio->data + offset, tmp->data, tmp->length*sizeof(double));
		caudiodata_dispose(tmp);
	}
	
	return OK;
}

