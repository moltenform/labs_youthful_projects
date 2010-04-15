void deleteFrame(int frame);
void deleteFrames();
void saveToFrame(int frame, PhasePortraitSettings * settings, double a, double b);
BOOL openFrame(int frame, PhasePortraitSettings * settings, double *outA, double *outB);
void getFrameInterp(double fwhere, PhasePortraitSettings * settings1, double a1, double b1, PhasePortraitSettings * settings2, double a2, double b2, PhasePortraitSettings * settingsOut, double *aOut, double *bOut);
int dotestanimation(SDL_Surface* pSurface);
