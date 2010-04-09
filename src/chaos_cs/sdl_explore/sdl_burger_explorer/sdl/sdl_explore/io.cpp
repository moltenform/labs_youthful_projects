#pragma warning (disable:4996)

#include "phaseportrait.h"


char * HELPERINPUTFILE = "helper_in.txt";
char * HELPEROUTPUTFILE = "helper_out.txt";

void saveData(PhasePortraitSettings * settings, char * filename, double a,double b)
{
	FILE * f = fopen(filename, "w");
	if (!f) return;
	fprintf(f,";version,1,w,%d,h,%d,x0,%f,x1,%f,y0,%f,y1,%f,a,%f,b,%f,",
		settings->width,settings->height,
		settings->x0, settings->x1, settings->y0, settings->y1,
		a,b);
	fprintf(f,"browsex0,%f,browsex1,%f,browsey0,%f,browsey1,%f," ,
		settings->browsex0, settings->browsex1, settings->browsey0, settings->browsey1);
	fprintf(f,"seeds,%d,settle,%d,drawing,%d" ,
		settings->seedsPerAxis, settings->settling, settings->drawing);
	
	fclose(f);
}

void loadData(PhasePortraitSettings * settings, char * filename, double *outA, double *outB)
{
	FILE * f = fopen(filename, "r");
	if (!f) return;
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
}

void onSave(PhasePortraitSettings * settings, double a,double b)
{
	saveData(settings, HELPERINPUTFILE, a,b);
	system("cshelper s");
}
bool didUserCancel()
{
	int tmp; bool bSuccess=false;
	//if the file begins with an integer, user hit cancel
	FILE * fop = fopen(HELPEROUTPUTFILE, "r");
	
	if (fscanf(fop, "%d", &tmp)>0) bSuccess=false;
	else bSuccess=true;
	fclose(fop);
	return !bSuccess;
}
void onOpen(PhasePortraitSettings * settings, double *a,double *b)
{
	system("cshelper o");
	
	if (!didUserCancel())
		loadData(settings, HELPEROUTPUTFILE, a, b);
}

void onGetExact(PhasePortraitSettings * settings, double *a,double *b)
{
	system("cshelper :");
	if (!didUserCancel())
	{
		FILE * f = fopen(HELPEROUTPUTFILE, "r");
		if (!f) return;
		fscanf(f, "out:%lf,%lf", a,b);
		fclose(f);
	}
}
void onGetMoreOptions(PhasePortraitSettings * settings, double *a,double *b)
{
	system("cshelper ;");
	if (!didUserCancel())
	{
		FILE * f = fopen(HELPEROUTPUTFILE, "r");
		if (!f) return;
		fscanf(f, "out:%d,%d,%d", &settings->seedsPerAxis,&settings->settling, &settings->drawing);
		fclose(f);
	}
}


