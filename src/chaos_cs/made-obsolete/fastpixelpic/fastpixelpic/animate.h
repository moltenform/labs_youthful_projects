
void deleteAllFrames();
void deleteFrame(int frame);
void saveToFrame(int frame);
BOOL openFrame(int frame);
BOOL previewAnimation(SDL_Surface* pSurface, int nframesPerKeyframe, int width);
BOOL renderAnimation(SDL_Surface* pSurface, int nframesPerKeyframe, int width);
void oscillateBreathing(double curA,double curB,double *outA, double *outB);
BOOL renderBreathing(SDL_Surface* pSurface, int width);

extern int gParamFramesPerKeyframe;
extern BOOL gParamHasSavedAFrame;

