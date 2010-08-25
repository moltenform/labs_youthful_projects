void onKeyUp(SDLKey key, BOOL bControl, BOOL bAlt, BOOL bShift, SDL_Surface*pSurface, int nActive, BOOL *needRedraw, BOOL *needDrawDiagram );

BOOL gParamBreathing = FALSE; //only access through turnOn, turnOff!
double savedC1=0, savedC2=0;
void turnOnBreathing()
{
	if (gParamBreathing) return; //do nothing if already breathing
	savedC1=g_settings->pc1; savedC2=g_settings->pc2;
	gParamBreathing=TRUE;
}
void turnOffBreathing()
{
	if (!gParamBreathing) return; //do nothing if not breathing
	g_settings->pc1=savedC1; g_settings->pc2=savedC2;
	gParamBreathing=FALSE;
}

/*void util_openfile(SDL_Surface* pSurface)
{
	char*ret = Dialog_GetText("Open file named: (also drag/drop onto program)", NULL, pSurface);
	if (!ret) return;
	char buf[256];
	snprintf(buf, sizeof(buf), "%s/%s.cfg", SAVESFOLDER, ret);
	free(ret);
	if (!doesFileExist(buf))
	{ Dialog_Message("No file with that name.", pSurface); return; }
	BOOL didopen = loadFromFile(buf);
	if (!didopen)
	{ Dialog_Message("Could open that file.", pSurface); return; }
}*/
/*void util_savefile(SDL_Surface* pSurface)
{
	char*ret = Dialog_GetText("Save file as:", NULL, pSurface);
	if (!ret) return;
	char buf[256];
	snprintf(buf, sizeof(buf), "%s/%s.cfg", SAVESFOLDER, ret);
	free(ret);
	if (doesFileExist(buf))
	{ Dialog_Message("Already a file with that name.", pSurface); util_savefile(pSurface); return; } //try again
	BOOL didsave = saveToFile(buf, MAPEXPRESSIONTEXT);
	if (!didsave)
	{ Dialog_Message("Could not save to that file.", pSurface); util_savefile(pSurface); return; } //try again
	
}*/
BOOL appendToFilePython(const char * filename)
{
	if (!filename || !filename[0]) return FALSE;
	
	FILE * f = fopen(filename, "a"); //append mode. do NOT overwrite the file.
	if (!f) return FALSE;
	
	if (gParamHasSavedAFrame) //save all the frames...
	{
		FastMapsSettings currentSettings;
		memcpy(&currentSettings,g_settings, sizeof(FastMapsSettings));
		
		for (int i=1; i<=9; i++)
		{
			if (!openFrame(i)) break;
			fprintf(f,"frame0%d",i);
			saveObjectPythonDict(f);
		}
		memcpy(g_settings, &currentSettings, sizeof(FastMapsSettings));
	}
	
	fprintf(f,"\n\n");
	fprintf(f,"latest");
	saveObjectPythonDict(f);
	fclose(f);
	return TRUE;
}

void doSave(BOOL bShift, SDL_Surface *pSurface)
{
	if (!bShift || !gParamFileOriginal || !gParamFileToAppendTo)
	{
		BOOL bRes = appendToFilePython(gParamFileToAppendTo); 
		Dialog_Message(bRes?"Saved.": "Save Failed.",pSurface);
	}
	else //do interesting things. clone the file.
	{
		char* newFname = Dialog_GetText("Save derived figure as:", "", pSurface);
		if (!newFname) return;
		if (strlen(gParamFileOriginal)<4)
		{
			Dialog_Message("Could not find .xyz file extension.",pSurface);
			free(newFname); 
			return;
		}
		//copy chars 1 to strlen-4 of it onto new one
		char newfilename[256] = {0};
		massert( strlen(gParamFileOriginal)<256, "Filename too long");
		//char * s = (char*) memcpy(newfilename, gParamFileOriginal, strlen(gParamFileOriginal)-4);
		char tmp[256]; strcpy(tmp, gParamFileOriginal);
		tmp[strlen(tmp)-4] = '\0';
		//snprintf(s, 255-(strlen(gParamFileOriginal)-4), "%s", newFname);
		//snprintf(newfilename, 255, "%s-%s.ccc", tmp, newFname);
		snprintf(newfilename, 255, "%s-%s.ccc", tmp, newFname);
		Dialog_Message(newfilename,pSurface);
		free(newFname);

		if (doesFileExist(newfilename)) { Dialog_Message("File of that name exists.", pSurface); return; }
		//clone the original file.
		FILE * fout = fopen(newfilename, "w");
		if (!fout) { Dialog_Message("Save failed.", pSurface); return; }
		
		FILE * forig = fopen(gParamFileOriginal, "r");
		int c; while ((c=fgetc(forig))!= EOF) fputc(c, fout);
		fclose(forig); fclose(fout);
		BOOL bRes = appendToFilePython(newfilename);
		massert(bRes, "Save failed.");
		//save this as the new current file.
		free(gParamFileToAppendTo);
		gParamFileToAppendTo = strdup(newfilename);
	}
}

void util_getNumberFrames(SDL_Surface *pSurface)
{
	int nframes = gParamFramesPerKeyframe;
	if (!Dialog_GetInt("Number of frames per key frame:",pSurface,&nframes))
		return;
	if (nframes>0) gParamFramesPerKeyframe = nframes;
}


void util_showVals(SDL_Surface* pSurface)
{
	char buf[256];
	snprintf(buf, sizeof(buf),"c1:%f c2:%f\tc1b:%f c2b:%f", g_settings->pc1, g_settings->pc2, g_settings->pc1b, g_settings->pc2b);
	snprintf(buf, sizeof(buf),"c3:%f c4:%f\tc3b:%f c4b:%f", g_settings->pc3, g_settings->pc4, g_settings->pc3b, g_settings->pc4b);
	snprintf(buf, sizeof(buf),"c5:%f c6:%f\tc5b:%f c6b:%f", g_settings->pc5, g_settings->pc6, g_settings->pc5b, g_settings->pc6b);
	Dialog_Message(buf, pSurface);
}

void util_incr(int direction /* 1 or -1*/, BOOL bShift)
{
	if (!bShift) {
		g_settings->drawing = MAX(0, g_settings->drawing + direction*1);
	} else {
		g_settings->settling = MAX(0, g_settings->settling + direction*1);
	}
}
void util_changeMaxValue(BOOL bShift)
{
	g_settings->maxValueAddition = MAX(0, g_settings->maxValueAddition + 0.5*(bShift?-1:1));
}
void util_shifthue(BOOL bShift)
{
	if (!bShift) {
		g_settings->hueShift += 0.02;
	} else {
		g_settings->hueShift -= 0.02;
	}
	if (g_settings->hueShift>1.0) g_settings->hueShift-=1.0;
	if (g_settings->hueShift<0.0) g_settings->hueShift+=1.0;
}
void util_changeWrapping()
{
	g_settings->colorWrapping = (g_settings->colorWrapping+1)%3;
}

//nudge figure
//~ void util_nudgeParam
//~ void util_nudgeBounds

void util_onGetExact(SDL_Surface *pSurface, int nActive)
{
	if (nActive==3) {
		if (!Dialog_GetDouble("Enter a value for c5:",pSurface,&g_settings->pc5)) return;
		if (!Dialog_GetDouble("Enter a value for c6:",pSurface,&g_settings->pc6)) return;
	} else if (nActive==2) {
		if (!Dialog_GetDouble("Enter a value for c3:",pSurface,&g_settings->pc3)) return;
		if (!Dialog_GetDouble("Enter a value for c4:",pSurface,&g_settings->pc4)) return;
	} else {
		if (!Dialog_GetDouble("Enter a value for c1:",pSurface,&g_settings->pc1)) return;
		if (!Dialog_GetDouble("Enter a value for c2:",pSurface,&g_settings->pc2)) return;
	}
}
void util_onGetMoreOptions(SDL_Surface *pSurface)
{
	if (!Dialog_GetInt("Enter a value for iters:",pSurface,&g_settings->drawing))
		return;
	if (!Dialog_GetDouble("Enter a value for maxvalue:",pSurface,&g_settings->maxValueAddition))
		return;
}


enum {
	BtnDiagramX = 700,
	BtnDiagramY = 10,
	BtnInfoX = 700+16+10,
	BtnInfoY = 10,
	BtnWidth=16, BtnHeight=16
};
void showInfo(SDL_Surface *pSurface);
BOOL didClickOnButton(SDL_Surface *pSurface, int mousex, int mousey)
{
	if (mousex>=BtnDiagramX && mousex<BtnDiagramX+BtnWidth && mousey>=BtnDiagramY && mousey<BtnDiagramY+BtnHeight)
	{return 1;}
	if (mousex>=BtnInfoX && mousex<BtnInfoX+BtnWidth && mousey>=BtnInfoY && mousey<BtnInfoY+BtnHeight)
	{showInfo(pSurface); return 2;}
	return -1;
}
void drawButtons(SDL_Surface *pSurface)
{
	plotlineRectangle(pSurface, BtnDiagramX,BtnDiagramX+BtnWidth,BtnDiagramY,BtnDiagramY+BtnHeight, 0);
	plotlineRectangle(pSurface, BtnInfoX,BtnInfoX+BtnWidth,BtnInfoY,BtnInfoY+BtnHeight, 0);
	//draw a smaller rect
	plotlineRectangle(pSurface, BtnDiagramX+2,BtnDiagramX+BtnWidth-2,BtnDiagramY+2,BtnDiagramY+BtnHeight-2, 0);
	//draw 3 dots
	plotlineRectangle(pSurface, BtnInfoX+2,BtnInfoX+5,BtnInfoY+10,BtnInfoY+13, 0);
	plotlineRectangle(pSurface, BtnInfoX+7,BtnInfoX+10,BtnInfoY+10,BtnInfoY+13, 0);
	plotlineRectangle(pSurface, BtnInfoX+12,BtnInfoX+15,BtnInfoY+10,BtnInfoY+13, 0);
}
void drawActive(SDL_Surface *pSurface, int nActive)
{
	if (nActive==-1) return;
	if (nActive==1 || nActive==2 || nActive==3) {
	SDL_Rect r; r.h = 25, r.w=2; 
	r.x = diagramsLayout[nActive].screen_x+diagramsLayout[nActive].screen_width;
	r.y = diagramsLayout[nActive].screen_y;
	SDL_FillRect(pSurface, &r, 0); //black mark to next to it
	}
}
double getAmountToMove(double width, BOOL bShift, BOOL bControl, BOOL bAlt)
{
	if (!bShift && !bControl && !bAlt)
		return 0.005;
	if (!bShift && bControl && !bAlt) //fine control
		return width/1024.0;
	if (bShift && !bControl && !bAlt) //course control
		return width/16.0;
	return 0.0;
}


void showInfoN(SDL_Surface *pSurface, int n)
{
	SDL_FillRect ( pSurface , NULL , g_white );
	if (n==1)
		showText( //replace '\r\n' with '\\n"\r\n"'
"Use the arrow keys to move around.\n"
"\n"
"Control-click		zoom in\n"
"Shift-click		zoom out\n"
"Shift-Right-click 		undo zoom\n"
"Alt-drag		zoom to region\n"
"Alt-shift-drag		zoom to any rectangle\n"
"\n"
"Press Tab to switch between these drawing modes:\n"
"1	Basins of Attraction\n"
"	(Alt-1 to add color, Shift-1 to color based on last x coordinate)\n"
"	\n"
"2	Quadrants\n"
"	(Alt-2 for contrast, Shift-2 to color based on difference between coords)\n"
"	\n"
"3	Phase Portrait\n"
"\n"
"4	Color lines\n"
"	(Shift-4 for shaded disk, Shift + and Shift - change disk size, Alt-4 for one) \n"
"5	Escape time\n"
"	(Shift-5 for basins view, Alt-5 to fill basins, Alt-Shift-5 more drawing ) \n"
"\n"
"Alt-b		starts 'breathing' mode\n"
"b, shift-b		adjust amplitude of breathing"
"\n\n"
"Press Enter to see more..."
		, 30, 30, pSurface);
	else
		showText(
		"Ctrl-S	Save\n"
"Ctrl-O	Open\n"
"Ctrl-N	Reset\n"
"Ctrl-R	Render image (higher quality)\n"
"Ctrl-B	Render animation of 'breathing' mode (sequence of .bmp files)\n"
"Ctrl-' Ctrl-; Ctrl-: 	Set other parameters\n"
"\n"
"Middle-click to reset view.\n"
"Shift-arrow keys for finer movement.\n"
"\n"
"Ctrl-F1		save into first keyframe (Ctrl-F2 for second keyframe and so on...)\n"
"F1		open first keyframe\n"
"Ctrl-Shift-F1		delete first keyframe\n"
"Shift-3		set number of interpolated frames per keyframe\n"
"Return/Enter		preview animation\n"
"Ctrl-Enter		render animation, creates sequence of .bmp files\n"
"\n"
"= or -		increase or decrease iterations\n"
"Shift = or -		adjust coloring, shading\n"
"Alt-D, Alt-C		change diagram coloring\n"
		, 30, 30, pSurface);

	SDL_UpdateRect ( pSurface , 0 , 0 , 0 , 0 ) ;
	SDL_Event event;
	while (TRUE) {
		if ( SDL_PollEvent ( &event ) ) {
			if ( event.type == SDL_QUIT ) return;
			else if (event.type==SDL_MOUSEBUTTONDOWN) break;
			else if (event.type==SDL_KEYUP) break;
		}
	}
	if (n==1) return showInfoN(pSurface, 2);
	else return;
}

void showInfo(SDL_Surface *pSurface) { showInfoN(pSurface, 1); }

