
#include "common.h"
#include "animate.h"
#include "io.h"

void deleteFrame(int frame)
{
	saveToFrame(frame, NULL, 0,0);
}
void deleteFrames()
{
for (int i=1; i<=9; i++)
	deleteFrame(i);
}

//if settings is null, clears the frame
void saveToFrame(int frame, PhasePortraitSettings * settings, double a, double b)
{
char buf[256];
snprintf(buf, sizeof(buf), "saves/frame0%d", frame);
if (settings ==NULL)
{
	FILE * f= fopen(buf, "w");
	if (!f) {exit(1);}
	fputc('$', f); 
	fclose(f);
}
else
{
	saveData(settings, buf, a,b);
}
}

void openFrame(int frame, PhasePortraitSettings * settings, const char * filename, double *outA, double *outB)
{
	char buf[256];
	snprintf(buf, sizeof(buf), "saves/frame0%d", frame);
	loadData(settings, buf, outA, outB);
}
