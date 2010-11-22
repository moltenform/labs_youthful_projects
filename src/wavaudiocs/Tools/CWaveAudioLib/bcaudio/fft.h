
errormsg rawSamplesToFrequency(unsigned int length, double* samples, double** freqReal, double** freqImag);
errormsg rawFrequencyToSamples(double** samples, unsigned int length, double* freqReal, double* freqImag);

errormsg dumpToFrequencyAngles(CAudioData* this, /*const*/ char* filename, uint blocksize);
errormsg readFrequenciesToSamples(CAudioData** out, /*const*/ char* filename);

