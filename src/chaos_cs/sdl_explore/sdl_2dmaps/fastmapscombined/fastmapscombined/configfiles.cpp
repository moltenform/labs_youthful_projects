
#include "configfiles.h"
#include "common.h"

// a simple yet flexible (and human readable) config format. key and value pairs.
// key=value;key=value;key=value;
// a GlobalFieldDescriptions object should be defined, with the settings to be used.



//sets all fields to defaults.
void initializeObjectToDefaults()
{
	int i=0;
	while(GlobalFieldDescriptions[i].fieldType != NULL)
	{
		if (StringsEqual(GlobalFieldDescriptions[i].fieldType, "int"))
		{
			int * theValue = (int*) GlobalFieldDescriptions[i].reference;
			*theValue = GlobalFieldDescriptions[i].defaultValue;
		}
		else if (StringsEqual(GlobalFieldDescriptions[i].fieldType, "double"))
		{
			double * theValue = (double*) GlobalFieldDescriptions[i].reference;
			*theValue = (double) GlobalFieldDescriptions[i].defaultValue;
		}
		else  { massert(0, "Unknown data type in definition of Settings object."); } 
		i++;
	}
}

void saveObject(FILE * stream)
{
	int i=0;
	while(GlobalFieldDescriptions[i].fieldType != NULL)
	{
		if (StringsEqual(GlobalFieldDescriptions[i].fieldType, "int"))
		{
			int * theValue = (int*) GlobalFieldDescriptions[i].reference;
			fprintf(stream, "%s=%d;", GlobalFieldDescriptions[i].fieldName, *theValue);
		}
		else if (StringsEqual(GlobalFieldDescriptions[i].fieldType, "double"))
		{
			double * theValue = (double*) GlobalFieldDescriptions[i].reference;
			fprintf(stream, "%s=%f;", GlobalFieldDescriptions[i].fieldName, *theValue);
		}
		else  { massert(0, "Unknown data type in definition of Settings object."); } 
		i++;
	}
	fprintf(stream, "END=0;");
}

BOOL readUntilEquals(char * buffer, int bufsize, FILE* stream)
{
	char c; int i=0;
	while (true)
	{
		if (i>=bufsize) {massert(0, "Invalid file, keyname too long."); return FALSE;}
		if (feof(stream)) {massert(0, "Invalid file, eof reached before end of keyname."); return FALSE;}
		fscanf(stream, "%c", &c);
		if (c=='=') { buffer[i]='\0'; return TRUE;}
		else {buffer[i] = c;}
		i++;
	}
}

BOOL loadObject(FILE * stream)
{
	char keyname [1024]={0};
	double fvalue; //initially reads everything as doubles.
	BOOL hasSeenUnknown = FALSE;
	while (true)
	{
		if (feof(stream)) {massert(0, "Invalid file, did not see END key."); return FALSE;}
		readUntilEquals(keyname, sizeof(keyname), stream);
		int ret = fscanf(stream, "%lf;", &fvalue);
		if (ret < 1) {massert(0, "Invalid file, expected value."); return FALSE; }
		if (StringsEqual(keyname, "END")) return TRUE; //hasSeenUnknown;
		int i=0;
		while(GlobalFieldDescriptions[i].fieldType != NULL)
		{
			if (StringsEqual(keyname, GlobalFieldDescriptions[i].fieldName))
			{
				if (StringsEqual(GlobalFieldDescriptions[i].fieldType, "int"))
				{
					int * theValue = (int*) GlobalFieldDescriptions[i].reference;
					*theValue = (int) fvalue;
				}
				else if (StringsEqual(GlobalFieldDescriptions[i].fieldType, "double"))
				{
					double * theValue = (double*) GlobalFieldDescriptions[i].reference;
					*theValue = fvalue;
				}
				else  { massert(0, "Unknown data type in definition of Settings object."); }

				break;
			}
			i++;
		}
		if (GlobalFieldDescriptions[i].fieldType == NULL) { hasSeenUnknown=TRUE; /*massert(0,"unrecognized key");*/ /*debug*/ }

	}
}
BOOL loadFromFile(const char * filename)
{
	FILE * f = fopen(filename, "r");
	if (!f) return FALSE;
	int versionNumber;
	int ret = fscanf(f,";fmmversion=%d;",&versionNumber);
	if (ret < 1) return FALSE; //not a valid saved file. Note: loading animations rely on this returning false.
	if (versionNumber!=1) { massert(0, "File is from incompatible version."); exit(1); }
	initializeObjectToDefaults(); //sets the defaults, so if this file misses anything, we don't retain.
	loadObject(f);
	fclose(f);
	return TRUE;
}

BOOL saveToFile(const char * filename, const char * expressiontext)
{
	FILE * f = fopen(filename, "w");
	if (!f) return FALSE;
	fprintf(f,";fmmversion=1;");
	saveObject(f);
	//Now add compatibility for cs phaseportrait
	fprintf(f, "\n\n"
		";saved from fastmapscombined\n"
		";formula is %s \n", expressiontext);
	/*	";the following is just so that it can be opened in cs_phase_portrait\n"
		"[main_portrait]\n"
		"X0=%f\n"
		"X1=%f\n"
		"Y0=%f\n"
		"Y1=%f\n"
		, g_settings->x0, g_settings->x1, g_settings->y0, g_settings->y1
		);
	fprintf(f, "\n"
		"param1=%f\n"
		"param2=%f\n"
		"param3=0\n"
		"param4=0\n"
		"paramSettle=%d\n"
		"paramTotalIters=400\n"
		"paramExpression=%s\n"
		"paramInit=//saved from fastmapscombined, by Ben Fisher\n"
		"programVersion=0.0.1"
		, g_settings->a,g_settings->b, g_settings->settlingTime, expressiontext
		);*/
	fclose(f);
	return TRUE;
}


