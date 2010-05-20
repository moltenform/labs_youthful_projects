

void util_openfile(SDL_Surface* pSurface)
{
	char*ret = Dialog_GetText("Open file named: (also drag/drop onto program)", NULL, pSurface);
	if (!ret) return;
	char buf[256];
	snprintf(buf, sizeof(buf), "%s%s.cfg", SAVESFOLDER, ret);
	free(ret);
	if (!doesFileExist(buf))
	{ Dialog_Message("No file with that name.", pSurface); return; }
	BOOL didopen = loadFromFile(buf);
	if (!didopen)
	{ Dialog_Message("Could open that file.", pSurface); return; }
}
void util_savefile(SDL_Surface* pSurface)
{
	char*ret = Dialog_GetText("Save file as:", NULL, pSurface);
	if (!ret) return;
	char buf[256];
	snprintf(buf, sizeof(buf), "%s%s.cfg", SAVESFOLDER, ret);
	free(ret);
	if (doesFileExist(buf))
	{ Dialog_Message("Already a file with that name.", pSurface); util_savefile(pSurface); return; } //try again
	BOOL didsave = saveToFile(buf, MAPEXPRESSIONTEXT);
	if (!didsave)
	{ Dialog_Message("Could not save to that file.", pSurface); util_savefile(pSurface); return; } //try again
}

void util_showVals(SDL_Surface* pSurface)
{
	char buf[256];
	snprintf(buf, sizeof(buf),"a:%f b:%f", g_settings->a, g_settings->b);
	Dialog_Message(buf, pSurface);
}

void util_onGetExact(SDL_Surface *pSurface)
{
	if (!Dialog_GetDouble("Enter a value for a:",pSurface,&g_settings->a))
		return;
	if (!Dialog_GetDouble("Enter a value for b:",pSurface,&g_settings->b))
		return;
}
void util_onGetMoreOptions(SDL_Surface *pSurface)
{
	if (!Dialog_GetInt("Enter a value for seeds per axis:",pSurface,&g_settings->seedsPerAxis))
		return;
	if (!Dialog_GetInt("Enter a value for settling:",pSurface,&g_settings->settlingTime))
		return;
	if (!Dialog_GetInt("Enter a value for how many points to draw:",pSurface,&g_settings->drawingTime))
		return;
}
void util_onGetSeed(SDL_Surface *pSurface)
{
	if (!Dialog_GetDouble("Enter a value for seedx0:",pSurface,&g_settings->seedx0)) return;
	if (!Dialog_GetDouble("Enter a value for seedx1:",pSurface,&g_settings->seedx1)) return;
	if (!Dialog_GetDouble("Enter a value for seedy0:",pSurface,&g_settings->seedy0)) return;
	if (!Dialog_GetDouble("Enter a value for seedy1:",pSurface,&g_settings->seedy1)) return;
}



