
// adapted from "FFT of waveIn audio signals" by Fred Ackers
// http://www.codeproject.com/KB/audio-video/waveInFFT.aspx

#include "bcaudio.h"
#include "fft.h"



unsigned int numberOfBitsNeeded( unsigned int p_nSamples )
{
	int i;
	if( p_nSamples < 2 )
		return 0;

	for ( i=0; ; i++ )
		if( p_nSamples & (1 << i) ) return i;
}
bool isPowerOfTwo( unsigned int p_nX )
{
	if( p_nX < 2 ) return FALSE;
	if( p_nX & (p_nX-1) ) return FALSE;
	return TRUE;
}
unsigned int reverseBits(unsigned int p_nIndex, unsigned int p_nBits)
{
	unsigned int i, rev;
	for(i=rev=0; i < p_nBits; i++)
	{
		rev = (rev << 1) | (p_nIndex & 1);
		p_nIndex >>= 1;
	}
	return rev;
}

errormsg fft_double (unsigned int p_nSamples, double* p_lpRealIn,double* p_lpImagIn, double* p_lpRealOut, double* p_lpImagOut, bool p_bInverseTransform)
{
	//~ (*pp_lpRealOut) = malloc(sizeof(double) * p_nSamples);
	//~ (*pp_lpImagOut) = malloc(sizeof(double) * p_nSamples);
	//~ double * p_lpRealOut = *pp_lpRealOut;
	//~ double * p_lpImagOut = *pp_lpImagOut;

	unsigned int NumBits;
	unsigned int i, j, k, n;
	unsigned int BlockSize, BlockEnd;

	double angle_numerator = 2.0 * PI;
	double tr, ti;

	if( !isPowerOfTwo(p_nSamples) )
		return "Error: Length is not a power of 2.";

	if( p_bInverseTransform ) angle_numerator = -angle_numerator;

	NumBits = numberOfBitsNeeded ( p_nSamples );


	for( i=0; i < p_nSamples; i++ )
	{
		j = reverseBits ( i, NumBits );
		p_lpRealOut[j] = p_lpRealIn[i];
		p_lpImagOut[j] = (p_lpImagIn == NULL) ? 0.0 : p_lpImagIn[i];
	}


	BlockEnd = 1;
	for( BlockSize = 2; BlockSize <= p_nSamples; BlockSize <<= 1 )
	{
		double delta_angle = angle_numerator / (double)BlockSize;
		double sm2 = sin ( -2 * delta_angle );
		double sm1 = sin ( -delta_angle );
		double cm2 = cos ( -2 * delta_angle );
		double cm1 = cos ( -delta_angle );
		double w = 2 * cm1;
		double ar[3], ai[3];

		for( i=0; i < p_nSamples; i += BlockSize )
		{

			ar[2] = cm2;
			ar[1] = cm1;

			ai[2] = sm2;
			ai[1] = sm1;

			for ( j=i, n=0; n < BlockEnd; j++, n++ )
			{

				ar[0] = w*ar[1] - ar[2];
				ar[2] = ar[1];
				ar[1] = ar[0];

				ai[0] = w*ai[1] - ai[2];
				ai[2] = ai[1];
				ai[1] = ai[0];

				k = j + BlockEnd;
				tr = ar[0]*p_lpRealOut[k] - ai[0]*p_lpImagOut[k];
				ti = ar[0]*p_lpImagOut[k] + ai[0]*p_lpRealOut[k];

				p_lpRealOut[k] = p_lpRealOut[j] - tr;
				p_lpImagOut[k] = p_lpImagOut[j] - ti;

				p_lpRealOut[j] += tr;
				p_lpImagOut[j] += ti;

			}
		}

		BlockEnd = BlockSize;

	}


	if( p_bInverseTransform )
	{
		double denom = ((double)p_nSamples)/2.0; 
		//ADDED: before, when round-tripping, was too quiet by a factor of 2. 
		//I compensate for that here. I don't know where the true error is.
		
		for ( i=0; i < p_nSamples; i++ )
		{
			p_lpRealOut[i] /= denom;
			p_lpImagOut[i] /= denom;
		}
	}
	return OK;
}


errormsg rawSamplesToFrequency_Preallocated(unsigned int length, double* samples, double* freqReal, double* freqImag)
{
	return fft_double(length,samples, NULL, freqReal, freqImag, FALSE);
}
errormsg rawFrequencyToSamples_Preallocated(double* samples, double* tempBuffer, unsigned int length, double* freqReal, double* freqImag)
{
	return fft_double(length, freqReal, freqImag, samples, tempBuffer, TRUE);
}
errormsg rawSamplesToFrequency(unsigned int length, double* samples, double** outfreqReal, double** outfreqImag)
{
	(*outfreqReal) = malloc(sizeof(double) * length);
	(*outfreqImag) = malloc(sizeof(double) * length);
	return rawSamplesToFrequency_Preallocated(length, samples, *outfreqReal, *outfreqImag);
}
errormsg rawFrequencyToSamples(double** samples, unsigned int length, double* freqReal, double* freqImag)
{
	(*samples) = malloc(sizeof(double) * length);
	double *tempBuffer = malloc(sizeof(double) * length);
	errormsg res = rawFrequencyToSamples_Preallocated(*samples, tempBuffer,length, freqReal, freqImag);
	free(tempBuffer);
	return res;
}

void angleToComplex(double * outReal, double * outImag, uint length, double * inMag, double * inAngle)
{
	uint i; 
	for (i=0;i<length; i++) outReal[i] = inMag[i] * cos(inAngle[i]);
	for (i=0;i<length; i++) outImag[i] = inMag[i] * sin(inAngle[i]);
}
void complexToAngle(double * outMag, double * outAngle, uint length, double * inReal, double * inImag)
{
	uint i; 
	for (i=0;i<length; i++) outMag[i] = sqrt(inReal[i]*inReal[i]+inImag[i]*inImag[i]);
	for (i=0;i<length; i++) outAngle[i] = atan2(inImag[i], inReal[i]);
}


errormsg dumpToFrequencyAngles(CAudioData* this, /*const*/ char* filename, uint blocksize) /* blocksize should be power of two*/
{
	if (sizeof(uint)!=4||sizeof(double)!=8) //don't currently worry about endianness
	{
		fputs( "uints should be 32bit, doubles should be 64bit", stderr);
		exit(1);
	}
	
	if( !isPowerOfTwo(blocksize) )
		return "Error: Length is not a power of 2.";
	double *ptrsamples = this->data;
	int nblocks = this->length/blocksize;
	if (nblocks <= 0) 
		return "Error: Blocksize too large.";
	if (this->sampleRate !=SampleRate)
		return "Error: Must have default sample rate: 44100";
	
	FILE * file = fopen(filename, "wb");
	if (!file) return "Error: Could not open file.";
	uint the_uint;
	the_uint = 0xbe7fffff; fwrite(&the_uint, sizeof(uint), 1, file);
	the_uint = 0x0; fwrite(&the_uint, sizeof(uint), 1, file);
	the_uint = blocksize; fwrite(&the_uint, sizeof(uint), 1, file);
	the_uint = nblocks; fwrite(&the_uint, sizeof(uint), 1, file);
	
	double * realBuffer = malloc(sizeof(double)*blocksize);
	double * imagBuffer = malloc(sizeof(double)*blocksize);
	double * magBuffer = malloc(sizeof(double)*blocksize);
	double * angleBuffer = malloc(sizeof(double)*blocksize);

	int i; for (i=0; i<nblocks; i++)
	{
		rawSamplesToFrequency_Preallocated(blocksize, ptrsamples, realBuffer, imagBuffer);
		
		//OPTIMIZATION: we only care about half of the results, since the other half is redundant (because we passed in a real signal)
		complexToAngle(magBuffer, angleBuffer,blocksize/2, realBuffer, imagBuffer);
		fwrite(magBuffer, sizeof(double), blocksize/2, file); 
		fwrite(angleBuffer, sizeof(double), blocksize/2, file);
		
		ptrsamples += blocksize;
	}
	fclose(file);
	
	free(realBuffer); free(imagBuffer); free(magBuffer); free(angleBuffer);
	printf("size of file should be %d bytes.\n", sizeof(uint)*4+sizeof(double)*2*blocksize*nblocks/2);
	printf("nblocks%d blocksize%d\n", nblocks, blocksize );
	return OK;
}

errormsg readFrequenciesToSamples(CAudioData** out, /*const*/ char* filename)
{
	FILE * file = fopen(filename, "rb");
	if (!file) return "Error: Could not open file.";
	printf("hiA");
	
	uint blocksize, nblocks;
	
	uint the_uint;
	fread(&the_uint, sizeof(uint),1,file);
	if (the_uint!=0xbe7fffff) return "Error: Wrong file type? Didn't see magic #.";
	fread(&the_uint, sizeof(uint),1,file);
	fread(&blocksize, sizeof(uint),1,file);
	fread(&nblocks, sizeof(uint),1,file); 
	
	
	
	CAudioData * audio = (*out) = caudiodata_new();
	caudiodata_allocate(audio, nblocks*blocksize , 1, SampleRate);
	
	double * realBuffer = malloc(sizeof(double)*blocksize);
	double * imagBuffer = malloc(sizeof(double)*blocksize);
	double * magBuffer = malloc(sizeof(double)*blocksize);
	double * angleBuffer = malloc(sizeof(double)*blocksize);
	double * tempBuffer = malloc(sizeof(double)*blocksize);

	double *ptrdata = audio->data;
	int i,j; for (i=0; i<nblocks; i++)
	{		
		fread(magBuffer, sizeof(double),blocksize/2,file);
		fread(angleBuffer, sizeof(double),blocksize/2,file);
		angleToComplex(realBuffer, imagBuffer,blocksize/2, magBuffer, angleBuffer);
		
		//OPTIMIZATION: reconstruct redundant half of data. Because we disregard imaginary results, ok to fill with 0, rather than the mirrored results.
		for(j=blocksize/2; j<blocksize;j++)
			realBuffer[j] = imagBuffer[j] = 0.0;
		
		rawFrequencyToSamples_Preallocated(ptrdata, tempBuffer, blocksize, realBuffer, imagBuffer);
		ptrdata += blocksize;
	}
	free(realBuffer); free(imagBuffer); free(magBuffer); free(angleBuffer);free(tempBuffer);
	fclose(file);
	printf("nblocks%d blocksize%d\n", nblocks, blocksize );
	return OK;
}





//optimization: we know neg frequencies will be mirrored. so why bother with them?
//we could override looking up p_lpRealIn[i], to see if i>len(p_lpRealIn), and return mirror image.


double Index_to_frequency(unsigned int p_nBaseFreq, unsigned int p_nSamples, unsigned int p_nIndex)
{
	if(p_nIndex >= p_nSamples)
	{
		return 0.0;
	}
	else if(p_nIndex <= p_nSamples/2)
	{
		return ( (double)p_nIndex / (double)p_nSamples * p_nBaseFreq );
	}
	else
	{
		return ( -(double)(p_nSamples-p_nIndex) / (double)p_nSamples * p_nBaseFreq );
	}
}

