#pragma warning (disable:4996)

#include "phaseportrait.h"
#include "common.h"
#include "font.h"

BOOL saveData(PhasePortraitSettings * settings, const char * filename, double a,double b)
{
	FILE * f = fopen(filename, "w");
	if (!f) return FALSE;
	fprintf(f,";version,1,w,%d,h,%d,x0,%f,x1,%f,y0,%f,y1,%f,a,%f,b,%f,",
		settings->width,settings->height,
		settings->x0, settings->x1, settings->y0, settings->y1,
		a,b);
	fprintf(f,"browsex0,%f,browsex1,%f,browsey0,%f,browsey1,%f," ,
		settings->browsex0, settings->browsex1, settings->browsey0, settings->browsey1);
	fprintf(f,"seeds,%d,settle,%d,drawing,%d" ,
		settings->seedsPerAxis, settings->settling, settings->drawing);
	
//Now add compatibility for cs phaseportrait
	fprintf(f, "\n\n"
		";saved from sdl_explore\n"
		";the following is just so that it can be opened in cs_phase_portrait\n"
		"[main_portrait]\n"
		"X0=%f\n"
		"X1=%f\n"
		"Y0=%f\n"
		"Y1=%f\n"
		, settings->x0, settings->x1, settings->y0, settings->y1
		);
	fprintf(f, "\n"
		"param1=%f\n"
		"param2=%f\n"
		"param3=0\n"
		"param4=0\n"
		"paramSettle=%d\n"
		"paramTotalIters=400\n"
		"paramExpression=%s\n"
		"paramInit=//saved from sdl_explore\n"
		"programVersion=0.0.1"
		, a,b, settings->settling, MAPEXPRESSIONTEXT
		);
	fclose(f);
	return TRUE;
}

BOOL loadData(PhasePortraitSettings * settings, const char * filename, double *outA, double *outB)
{
	FILE * f = fopen(filename, "r");
	if (!f) return FALSE;
	int version;
	fscanf(f,";version,%d,w,%d,h,%d,x0,%lf,x1,%lf,y0,%lf,y1,%lf,a,%lf,b,%lf,",
		&version,
		&settings->width,&settings->height,
		&settings->x0, &settings->x1, &settings->y0, &settings->y1,
		outA, outB);
	fscanf(f,"browsex0,%lf,browsex1,%lf,browsey0,%lf,browsey1,%lf," ,
		&settings->browsex0, &settings->browsex1, &settings->browsey0, &settings->browsey1);
	fscanf(f,"seeds,%d,settle,%d,drawing,%d" ,
		&settings->seedsPerAxis, &settings->settling, &settings->drawing);
	
	fclose(f);
	return TRUE;
}

void onSave(PhasePortraitSettings * settings, double a,double b, SDL_Surface *pSurface)
{
	while (TRUE)
	{
		char*ret = Dialog_GetText("Save file as:", NULL, pSurface);
		if (!ret) return;
		char buf[256];
		snprintf(buf, sizeof(buf), "saves/%s.cfg", ret);
		//does this file exist?
		if (doesFileExist(buf))
		{
			Dialog_Message("Already a file that name.", pSurface);
			continue;
		}
		else
		{
			BOOL didsave = saveData(settings, buf, a,b);
			if (didsave)
			{
				//update index
				FILE*findex=fopen("saves/index","a");
				if (!findex) return;
				fprintf(findex, "%s\n", buf);
				fclose(findex);
				return;
			}
			else
			{
				Dialog_Message("Could not save under that name.", pSurface);
				continue;
			}
		}
	}
}

void onOpen(PhasePortraitSettings * settings, double *a,double *b, BOOL backwards)
{
	static int whichShuffle = 0;
	if (!backwards) whichShuffle++;
	else whichShuffle = (whichShuffle==0)?0:whichShuffle-1;

	char buf[256];
	FILE * findex = fopen("saves/index","r");
	if (!findex) return;
	for(int i=0; i<whichShuffle+1; i++ )
	{
		if (fgets(buf, sizeof(buf), findex) == NULL)
		{
			whichShuffle = 0;
			rewind(findex);
			fgets(buf, sizeof(buf), findex);
			break;
		}
	}
	//remove newline char
	if (strlen(buf)>0) buf[strlen(buf)-1]='\0';
	printf("cur%d fi %s \n", whichShuffle, buf);
	loadData(settings, buf, a,b);
}

void onGetExact(PhasePortraitSettings * settings, double *a,double *b, SDL_Surface *pSurface)
{
	if (!Dialog_GetDouble("Enter a value for a:",pSurface,a))
		return;
	if (!Dialog_GetDouble("Enter a value for b:",pSurface,b))
		return;
}
void onGetMoreOptions(PhasePortraitSettings * settings, SDL_Surface *pSurface)
{
	if (!Dialog_GetInt("Enter a value for seeds per axis:",pSurface,&settings->seedsPerAxis))
		return;
	if (!Dialog_GetInt("Enter a value for settling:",pSurface,&settings->settling))
		return;
	if (!Dialog_GetInt("Enter a value for how many points to draw:",pSurface,&settings->drawing))
		return;
}

void loadPreset(int key, BOOL bshift, BOOL balt, PhasePortraitSettings * settings, double *a,double *b)
{//take from num keys, not fkeys, since those include f4 which should quit.

	// ANSI C standard: forward slashes ok in file names even in windows
	char stemplate[]="presets/%d%s.cfg";

	char path[256] = {0};
	if (key>12||key<0) return;
	const char * comb = (bshift ? "s" : (balt ? "b" : ""));

	snprintf(path, sizeof(path), stemplate, key, comb);
	loadData(settings, path, a, b);
}


