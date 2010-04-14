

BOOL ShowText(const char* text, int pos_x, int pos_y, SDL_Surface* pScreen);
BOOL ShowTextAdvanced(const char* text, int type, int pos_x, int pos_y, SDL_Surface* pScreen);

char * Dialog_GetText(const char* prompt, const char*previous, SDL_Surface* pSurface);