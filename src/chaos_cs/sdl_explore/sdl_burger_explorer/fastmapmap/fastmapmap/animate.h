#pragma warning (disable:4996)

void deleteAllFrames();
void deleteFrame(int frame);
void saveToFrame(int frame);
BOOL openFrame(int frame);
BOOL previewAnimation(SDL_Surface* pSurface, int nframesPerKeyframe, int width);
BOOL renderAnimation(SDL_Surface* pSurface, int nframesPerKeyframe, int width);

extern int gParamFramesPerKeyframe;

