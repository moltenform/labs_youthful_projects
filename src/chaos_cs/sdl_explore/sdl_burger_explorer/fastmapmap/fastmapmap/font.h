

BOOL ShowText(const char* text, int pos_x, int pos_y, SDL_Surface* pScreen);
BOOL ShowTextAdvanced(const char* text, int type, int pos_x, int pos_y, SDL_Surface* pScreen);

char * Dialog_GetText(const char* prompt, const char*previous, SDL_Surface* pSurface);
void Dialog_Message(const char* prompt, SDL_Surface* pSurface);
BOOL Dialog_GetDouble(const char* prompt, SDL_Surface* pSurface, double *out);
BOOL Dialog_GetInt(const char* prompt, SDL_Surface* pSurface, int *out);
BOOL Dialog_GetBool(const char* prompt, SDL_Surface* pSurface);
