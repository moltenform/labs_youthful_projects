

errormsg caudiodata_loadwave(CAudioData** waveout, char* filename);
errormsg caudiodata_savewave(CAudioData* this, char* filename, int bitsPerSample /*=8 or 16*/);

errormsg caudiodata_savewavemem(char** out, uint*outLengthInBytes, CAudioData* this, int bitsPerSample /*=8 or 16*/);
//save file into memory.

//errormsg caudiodata_loadwavestream(CAudioData**out, FILE * f);
//errormsg caudiodata_savewavestream(CAudioData* this, Simplestream * f, int bitsPerSample /*=8 or 16*/);



