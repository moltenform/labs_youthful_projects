
BOOL showText(const char* text, int pos_x, int pos_y, SDL_Surface* pScreen);
void initFont();
void freeFonts();

char * Dialog_GetText(const char* prompt, const char*previous, SDL_Surface* pSurface);
void Dialog_Message(const char* prompt, SDL_Surface* pSurface);
void Dialog_Messagef(SDL_Surface* pSurface, const char* prompt, ...);
BOOL Dialog_GetDouble(const char* prompt, SDL_Surface* pSurface, double *out);
BOOL Dialog_GetInt(const char* prompt, SDL_Surface* pSurface, int *out);
BOOL Dialog_GetBool(const char* prompt, SDL_Surface* pSurface);

/*
  Cone3D SDL font routines.
  Made by Marius Andra 2002
  http://cone3d.gamedev.net

  You can use the code for anything you like.
  Even in a commercial project.
  But please let me know where it ends up.
  I'm just curious. That's all.
*/


