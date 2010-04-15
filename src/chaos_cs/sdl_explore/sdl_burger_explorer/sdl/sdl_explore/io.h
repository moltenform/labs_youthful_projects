
BOOL saveData(PhasePortraitSettings * settings, const char * filename, double a,double b);
BOOL loadData(PhasePortraitSettings * settings, const char * filename, double *outA, double *outB);
void onSave(PhasePortraitSettings * settings, double a,double b, SDL_Surface *pSurface);
void onOpen(PhasePortraitSettings * settings, double *a,double *b, BOOL backwards);
void onGetExact(PhasePortraitSettings * settings, double *a,double *b, SDL_Surface *pSurface);
void loadFkeyPreset(int key, BOOL bshift, BOOL balt, PhasePortraitSettings * settings, double *a,double *b);
void onGetMoreOptions(PhasePortraitSettings * settings, SDL_Surface *pSurface);



