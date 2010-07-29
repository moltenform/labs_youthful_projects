
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
void saveObjectPythonDict(FILE * stream)
{
	int i=0;
	fprintf(stream, "=dict(");
	while(GlobalFieldDescriptions[i].fieldType != NULL)
	{
		if (StringsEqual(GlobalFieldDescriptions[i].fieldType, "int"))
		{
			int * theValue = (int*) GlobalFieldDescriptions[i].reference;
			fprintf(stream, "%s=%d, ", GlobalFieldDescriptions[i].fieldName, *theValue);
		}
		else if (StringsEqual(GlobalFieldDescriptions[i].fieldType, "double"))
		{
			double * theValue = (double*) GlobalFieldDescriptions[i].reference;
			if (*theValue==0.00 && GlobalFieldDescriptions[i].fieldName[0]=='p' &&
				GlobalFieldDescriptions[i].fieldName[1]=='c' && strlen(GlobalFieldDescriptions[i].fieldName)<=4)
				; //don't show it. pc6 and so on are assumed to be 0.0 by default.
			else
				fprintf(stream, "%s=%f, ", GlobalFieldDescriptions[i].fieldName, *theValue);
		}
		else  { massert(0, "Unknown data type in definition of Settings object."); } 
		if (StringsEqual(GlobalFieldDescriptions[i].fieldName, "pc6") ||
			StringsEqual(GlobalFieldDescriptions[i].fieldName, "pc6b") ||
			StringsEqual(GlobalFieldDescriptions[i].fieldName, "diagram_c_y1") ||
			StringsEqual(GlobalFieldDescriptions[i].fieldName, "hueShift")) {
			//can peek at stream to see if we've already written a newline?
				fprintf(stream, "\n\t");
		}
		i++;
		
	}
	fprintf(stream, ")\n");
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
	printf("setting hi \n");
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
					printf("setting %s \n", GlobalFieldDescriptions[i].fieldName);
				}
				else  { massert(0, "Unknown data type in definition of Settings object."); }

				break;
			}
			i++;
		}
		if (GlobalFieldDescriptions[i].fieldType == NULL) { hasSeenUnknown=TRUE; massert(0,"unrecognized key"); /*debug*/ }

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
	fclose(f);
	return TRUE;
}


