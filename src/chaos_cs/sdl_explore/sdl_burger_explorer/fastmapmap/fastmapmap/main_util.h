void oscillateBreathing(double curA,double curB,double *outA, double *outB);
void onKeyUp(SDLKey key, BOOL bControl, BOOL bAlt, BOOL bShift, SDL_Surface*pSurface, BOOL *needRedraw, BOOL *needDrawDiagram );

void util_openfile(SDL_Surface* pSurface)
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
}
void util_savefile(SDL_Surface* pSurface)
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
}

void util_switchModes(BOOL bShift)
{
	if (bShift) { //get back to before where started by calling many times. there are 7 modes, so go 6.
		for (int i=0; i<6; i++) util_switchModes(FALSE);
		return;}
	switch(g_settings->drawingMode)
	{
		case DrawModeBasinsDistance: g_settings->drawingMode = DrawModeBasinsX; break;
		case DrawModeBasinsX: g_settings->drawingMode = DrawModeBasinsQuadrant; break;
		case DrawModeBasinsQuadrant: g_settings->drawingMode = DrawModeBasinsDifference; break;
		case DrawModeBasinsDifference: g_settings->drawingMode = DrawModePhase; break;
		case DrawModePhase: g_settings->drawingMode = DrawModeColorLine; break;
		case DrawModeColorLine: g_settings->drawingMode = DrawModeColorDisk; break;
		case DrawModeColorDisk: g_settings->drawingMode = DrawModeBasinsDistance; break;
		default: g_settings->drawingMode = DrawModePhase; break; //shouldn't be here
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
	snprintf(buf, sizeof(buf),"a:%f b:%f", g_settings->a, g_settings->b);
	Dialog_Message(buf, pSurface);
}


void util_incr(int direction /* 1 or -1*/, BOOL bShift)
{
	if (!bShift) {
		if (g_settings->drawingMode == DrawModePhase)
			g_settings->settlingTime = MAX(0, g_settings->settlingTime+ direction*15);
		else if (g_settings->drawingMode == DrawModeColorDisk || g_settings->drawingMode == DrawModeColorLine)
			g_settings->colorsStep = MAX(0, g_settings->colorsStep+ direction*1);
		else
			g_settings->basinsTime = MAX(0, g_settings->basinsTime+ direction*1);
		
	} else {
		if (g_settings->drawingMode == DrawModePhase)
			g_settings->seedsPerAxis = MAX(0, g_settings->seedsPerAxis+ direction*2);
		else if (g_settings->drawingMode == DrawModeColorDisk || g_settings->drawingMode == DrawModeColorLine)
			bShowOnlyOneColorLine = !bShowOnlyOneColorLine;
		else
			g_settings->basinsMaxColor = MAX(0, g_settings->basinsMaxColor+ direction*0.7);
	}
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

enum {
	BtnDiagramX = 450,
	BtnDiagramY = 10,
	BtnInfoX = 450+16+10,
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
	//draw a cross
	plotlineHoriz(pSurface, BtnDiagramX, BtnDiagramX+BtnWidth, BtnDiagramY+BtnHeight/2, 0);
	plotlineVert(pSurface, BtnDiagramX+BtnWidth/2, BtnDiagramY,BtnDiagramY+BtnHeight, 0);
	//draw 3 dots
	plotlineRectangle(pSurface, BtnInfoX+2,BtnInfoX+5,BtnInfoY+10,BtnInfoY+13, 0);
	plotlineRectangle(pSurface, BtnInfoX+7,BtnInfoX+10,BtnInfoY+10,BtnInfoY+13, 0);
	plotlineRectangle(pSurface, BtnInfoX+12,BtnInfoX+15,BtnInfoY+10,BtnInfoY+13, 0);
}
void showInfo(SDL_Surface *pSurface)
{
	SDL_FillRect ( pSurface , NULL , g_white );
	ShowText(
		"Features\n"
		"_______\n"
		"\n"
		"Ctrl+N		reset\n"
		"Ctrl+S		test here\n"
		"Ctrl+O		test hh\n"
		"\n"
		"Alt+F\n"
		"Alt+B\n"
		"Alt+G\n"
		"\n"
		"Ctrl+'\n"
		"Ctrl+;\n"
		"PgUp\n"
		"PgDn\n"
		"Space\n"
		"Esc\n"
		"\n"
		"\n"
		"Arrow keys\n"
		"Ctrl-click\n"
		"Shift-click\n"
		"Right-click\n"
		, 30, 30, pSurface);

	SDL_UpdateRect ( pSurface , 0 , 0 , 0 , 0 ) ;
	SDL_Event event;
	while (TRUE) {
		if ( SDL_PollEvent ( &event ) ) {
			if ( event.type == SDL_QUIT ) return;
			else if (event.type==SDL_MOUSEBUTTONDOWN) return;
			else if (event.type==SDL_KEYUP) return;
		}
	}
}



