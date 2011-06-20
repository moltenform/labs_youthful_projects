#pragma once

#include "common.h"
#include <stdio.h>

extern SettingsFieldDescription GlobalFieldDescriptions[];

void initializeObjectToDefaults();
BOOL loadFromFile(const char * filename);
BOOL saveToFile(const char * filename, const char * expressiontext);
void saveObjectPythonDict(FILE * stream, bool b);





