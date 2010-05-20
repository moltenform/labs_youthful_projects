#pragma once

#include "common.h"
#include <stdio.h>

extern SettingsFieldDescription GlobalFieldDescriptions[];

void initializeObject();
BOOL loadFromFile(const char * filename);
BOOL saveToFile(const char * filename, const char * expressiontext);





