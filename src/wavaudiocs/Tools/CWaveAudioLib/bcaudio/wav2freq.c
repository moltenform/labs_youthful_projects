

#include "bcaudioheaders.h"


const char *usage =
	"Usage:\n"
	"w2f f sound.wav sound.dat 2048 \n"
	"w2f w sound.dat sound.wav \n";


int main(int argc, char ** argv)
{
	errormsg msg;
	if (argc < 4)
	{
		puts(usage);
		return 0;
	}
	if (strcmp(argv[1], "f")==0)
	{
		/*const*/ char* wavname = argv[2];
		/*const*/ char* outname = argv[3];
		int blocksize = atoi(argv[4]);
		if (blocksize<=2) {puts("blocksize must be a positive power of 2 greater than 2.\n"); return 1;}
		
		CAudioData * audio =NULL;
		msg = caudiodata_loadwave(&audio, wavname);
		if (msg != OK) {puts(msg); return 1;}
		msg = dumpToFrequencyAngles(audio, outname, blocksize);
		if (msg != OK) {puts(msg); return 1;}
		
		caudiodata_dispose(audio);
	}
	else if (strcmp(argv[1], "w")==0)
	{
		/*const*/ char* datname = argv[2];
		/*const*/ char* outname = argv[3];
		
		CAudioData * audio =NULL;
		
		msg = readFrequenciesToSamples(&audio, datname);
		if (msg != OK) {puts(msg); return 1;}
		msg = caudiodata_savewave(audio, outname, 16);
		if (msg != OK) {puts(msg); return 0;}
	
		caudiodata_dispose(audio);
	}
	else
	{
		puts("f or w expected.\n");
		puts(usage);
		return 1;
	}
	
	return 0;
}

