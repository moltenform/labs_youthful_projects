#pragma warning (disable:4996)

void deleteAllFrames();
void deleteFrame(int frame);
void saveToFrame(int frame);
BOOL openFrame(int frame);
int dotestanimation(SDL_Surface* pSurface, int nframesPerKeyframe, int width);
int dowriteanimation(SDL_Surface* pSurface, int nframesPerKeyframe, int width);

extern int nFramesPerKeyframe;

