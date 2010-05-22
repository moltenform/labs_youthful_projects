#pragma warning (disable:4996)
#include "common.h"
#include <assert.h>
#include "font.h"

BOOL Dialog_GetBool(const char* prompt, SDL_Surface* pSurface)
{
	char* ret = Dialog_GetText(prompt, "Type y or n.", pSurface);
	if (!ret) 
		return FALSE;
	BOOL wasTrue = StringsEqual(ret,"y") || StringsEqual(ret,"yes");
	free(ret);
	return wasTrue;
}
BOOL Dialog_GetDouble(const char* prompt, SDL_Surface* pSurface, double *out)
{
	char tmpbuf[256];
	snprintf(tmpbuf, sizeof(tmpbuf), "Value was previously:%f", *out);
	char* ret = Dialog_GetText(prompt, tmpbuf, pSurface);
	if (!ret) 
		return FALSE;
	double d;
	int worked = sscanf(ret, "%lf", &d);
	if (worked)
		*out = d;
	free(ret);
	return worked != 0;
}
BOOL Dialog_GetInt(const char* prompt, SDL_Surface* pSurface, int *out)
{
	char tmpbuf[256];
	snprintf(tmpbuf, sizeof(tmpbuf), "Value was previously:%d", *out);
	char* ret = Dialog_GetText(prompt, tmpbuf, pSurface);
	if (!ret) 
		return FALSE;
	int n;
	int worked = sscanf(ret, "%d", &n);
	if (worked)
		*out = n;
	free(ret);
	return worked != 0;
}

//displays message until user presses any key.
void Dialog_Message(const char* prompt, SDL_Surface* pSurface)
{
	SDL_FillRect ( pSurface , NULL , g_white);
	ShowText( prompt, 30, 30, pSurface);
	ShowText( "Press return to continue.", 30, 100, pSurface);
	SDL_Event event;
	while (TRUE)
	{
		if ( SDL_PollEvent ( &event ) )
		{
			if ( event.type == SDL_QUIT ) break; 
			else if (event.type==SDL_KEYUP && event.key.keysym.sym == SDLK_RETURN) break;
			else if (event.type==SDL_KEYUP && event.key.keysym.sym == SDLK_KP_ENTER) break;
			else if (event.type==SDL_KEYUP && event.key.keysym.sym == SDLK_ESCAPE) break;
		}
		SDL_UpdateRect ( pSurface , 0 , 0 , 0 , 0 ) ; 
	}
}

//user responsible for freeing. return null on (cancel or empty string).
char * Dialog_GetText(const char* prompt, const char*previous, SDL_Surface* pSurface)
{
#define BLENGTH 256
	char *buffer = (char*) malloc(sizeof(char)*BLENGTH);
	memset(buffer, 0, BLENGTH/sizeof(char));
	buffer[0] = '_'; buffer[1] = '\0';
	int currentLength = 0; //points to the _
	SDL_Event event;
	BOOL dirty = TRUE;
	BOOL cancelled = FALSE;

	while (TRUE)
	{
		if (currentLength>BLENGTH-4)
			currentLength = BLENGTH-4;
		if (dirty)
		{
			//redraw the screen
			SDL_FillRect ( pSurface , NULL , g_white );
			ShowText( prompt, 30, 30, pSurface);
			if (previous)
				ShowText( previous, 30, 50, pSurface);
			ShowText(">", 30, 100, pSurface);
			ShowText(buffer, 50, 100, pSurface);
			ShowText("Press Enter when finished, or press Escape to cancel.", 30, 400, pSurface);
			dirty = FALSE;
		}

		if ( SDL_PollEvent ( &event ) )
		{
			if ( event.type == SDL_QUIT ) { cancelled=TRUE; break; }
			else if (event.type==SDL_KEYUP)
			{
				  if (event.key.keysym.sym == SDLK_KP_ENTER || event.key.keysym.sym == SDLK_RETURN)
				  { cancelled=FALSE; break; }
				  else if (event.key.keysym.sym == SDLK_ESCAPE)
				  { cancelled=TRUE; break; }
				  else if (event.key.keysym.sym == SDLK_BACKSPACE)
				  {
					if (event.key.keysym.mod & KMOD_CTRL) {
						buffer[0]='_';
						buffer[1]='\0';
						currentLength = 0;
					}
					else if (currentLength>0) {
						buffer[currentLength--]='\0';
						buffer[currentLength]='_';
						}
					dirty=TRUE;
				  }
				  else if (event.key.keysym.sym >= 32 && event.key.keysym.sym <= 126)
				  {
						buffer[currentLength]=event.key.keysym.sym;
						buffer[currentLength+1]='_';
						buffer[currentLength+2]='\0'; //just to be sure.
						currentLength++;
						dirty=TRUE;
				  }
			}

		}
		

		SDL_UpdateRect ( pSurface , 0 , 0 , 0 , 0 ) ;  //apparently needed every frame, even when not redrawing

	}
	if (cancelled || StringsEqual(buffer,"_") || StringsEqual(buffer,"")) 
	{
		free(buffer); 
		return NULL;
	}
	else 
	{
		buffer[currentLength]='\0'; //replace final _ with end of string
		return buffer;
	}
}

//from http://ubuntu-gamedev.pbworks.com/Using%20Bitmap%20Fonts%20with%20SDL
//ported to C by Ben Fisher, 2010
//added line with case '.': // .

SDL_Surface* g_pFontList = NULL;
BOOL ShowText(const char* text, int pos_x, int pos_y, SDL_Surface* pScreen)
{
	// this one doesn't choose font.
	return ShowTextAdvanced(text, 1, pos_x, pos_y, pScreen);
}
BOOL ShowTextAdvanced(const char* text, int type, int pos_x, int pos_y, SDL_Surface* pScreen)
{
	if (!text || strlen(text)==0) return FALSE;
	/* TODO: We need to calculate the fonts height into the pos_y thing. */
	// Also, id like to see this stuff gathered from an ini file.
	// That way we can alter fonts without the need for recompilcation

	if (!g_pFontList) {
		//check if the file is there, and exit if not.
		if (!doesFileExist("data/font_source2.bmp"))
			{assert(0); exit(1);}

		SDL_Surface* temp = SDL_LoadBMP("data/font_source2.bmp"); 
		if (!temp) { assert(0); exit(1);}
		g_pFontList = SDL_DisplayFormat(temp);
		SDL_FreeSurface(temp);

		//SDL_DisplayFormatAlpha(IMG_Load("data/font_source2.png"));
	}
	if(!pScreen) return FALSE;

	SDL_Rect rect; //
	rect.x = pos_x;
	rect.y = pos_y;
	SDL_Rect tmp_rect;
	tmp_rect.y = 0*type;

	for(unsigned int i=0; i < strlen(text); i++) {
		
		tmp_rect.y = 0; // set right y axe

		switch(text[i]) {
			case 0x20:
				rect.x += 10;
				break;
			case '\n':
				rect.x = pos_x;
				rect.y += 20;
				break;
			case 0x21: // !
				tmp_rect.x = 4;
				tmp_rect.w = 6;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x2D: // -
				tmp_rect.x = 184;
				tmp_rect.w = 8;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case '\'': 
				tmp_rect.x = 199-15*7;
				tmp_rect.w = 8;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case '*': 
				tmp_rect.x = 199-15*4;
				tmp_rect.w = 8;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case '+':
				tmp_rect.x = 199-15*3;
				tmp_rect.w = 8;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case ',':
				tmp_rect.x = 199-15-15;
				tmp_rect.w = 8;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case '.': 
				tmp_rect.x = 199;
				tmp_rect.w = 8;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x30: // 0
				tmp_rect.x = 226;
				tmp_rect.w = 11;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;			
			case 0x31: // 1
				tmp_rect.x = 244;
				tmp_rect.w = 9;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;	
			case 0x32: // 2
				tmp_rect.x = 256;
				tmp_rect.w = 12;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;	
			case 0x33: // 3
				tmp_rect.x = 272;
				tmp_rect.w = 11;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;	
			case 0x34: // 4
				tmp_rect.x = 286;
				tmp_rect.w = 12;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;	
			case 0x35: // 5
				tmp_rect.x = 302;
				tmp_rect.w = 11;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x36: // 6
				tmp_rect.x = 317;
				tmp_rect.w = 10;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x37: // 7
				tmp_rect.x = 332;
				tmp_rect.w = 10;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x38: // 8
				tmp_rect.x = 347;
				tmp_rect.w = 11;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x39: // 9
				tmp_rect.x = 362;
				tmp_rect.w = 11;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x3A: // :
				tmp_rect.x = 379;
				tmp_rect.w = 6;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x3B: // ;
				tmp_rect.x = 394;
				tmp_rect.w = 5;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x3C: // <
				tmp_rect.x = 407;
				tmp_rect.w = 8;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x3D: // =
				tmp_rect.x = 424;
				tmp_rect.w = 8;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x3E: // >
				tmp_rect.x = 440;
				tmp_rect.w = 8;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x3F: // ?
				tmp_rect.x = 454;
				tmp_rect.w = 10;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x40: // ?
				tmp_rect.x = 465;
				tmp_rect.w = 16;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x41: // A
				tmp_rect.x = 482;
				tmp_rect.w = 13;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x42: // B
				tmp_rect.x = 498;
				tmp_rect.w = 10;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x43: // C
				tmp_rect.x = 511;
				tmp_rect.w = 13;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x44: // D
				tmp_rect.x = 527;
				tmp_rect.w = 12;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x45: // E
				tmp_rect.x = 542;
				tmp_rect.w = 12;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x46: // F
				tmp_rect.x = 558;
				tmp_rect.w = 9;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x47: // G
				tmp_rect.x = 571;
				tmp_rect.w = 14;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x48: // H
				tmp_rect.x = 586;
				tmp_rect.w = 13;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x49: // I
				tmp_rect.x = 602;
				tmp_rect.w = 10;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x4A: // J
				tmp_rect.x = 616;
				tmp_rect.w = 12;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x4B: // K
				tmp_rect.x = 631;
				tmp_rect.w = 12;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x4C: // L
				tmp_rect.x = 647;
				tmp_rect.w = 10;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x4D: // M
				tmp_rect.x = 659;
				tmp_rect.w = 16;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x4E: // N
				tmp_rect.x = 406;
				tmp_rect.w = 14;
				tmp_rect.h = 19;
				tmp_rect.y = 19; // <-- this is right. I made a mistake creating the image, that is why its somewhere else.
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x4F: // O
				tmp_rect.x = 675;
				tmp_rect.w = 15;
				tmp_rect.h = 19;
				tmp_rect.y = 0; // <-- Here we fix that mistake :)
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x50: // P
				tmp_rect.x = 692;
				tmp_rect.w = 10;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x51: // Q
				tmp_rect.x = 705;
				tmp_rect.w = 14;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x52: // R
				tmp_rect.x = 722;
				tmp_rect.w = 10;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x53: // S
				tmp_rect.x = 736;
				tmp_rect.w = 13;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x54: // T
				tmp_rect.x = 751;
				tmp_rect.w = 12;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x55: // U
				tmp_rect.x = 766;
				tmp_rect.w = 13;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x56: // V
				tmp_rect.x = 782;
				tmp_rect.w = 12;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x57: // W
				tmp_rect.x = 795;
				tmp_rect.w = 15;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x58: // X
				tmp_rect.x = 811;
				tmp_rect.w = 13;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x59: // Y
				tmp_rect.x = 827;
				tmp_rect.w = 11;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x5A: // Z
				tmp_rect.x = 841;
				tmp_rect.w = 12;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x5B: // [
				tmp_rect.x = 858;
				tmp_rect.w = 8;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			
			case 0x5C: // /
				tmp_rect.x = 873;
				tmp_rect.w = 9;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x5D: // ]
				tmp_rect.x = 888;
				tmp_rect.w = 9;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x5E: // ]
				tmp_rect.x = 903;
				tmp_rect.w = 10;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x5F: // _
				tmp_rect.x = 915;
				tmp_rect.w = 15;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x60: // `
				tmp_rect.x = 936;
				tmp_rect.w = 7;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x61: // a
				tmp_rect.x = 946;
				tmp_rect.w = 11;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x62: // b
				tmp_rect.x = 962;
				tmp_rect.w = 11;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x63: // c
				tmp_rect.x = 976;
				tmp_rect.w = 10;
				tmp_rect.h = 19;
				tmp_rect.y = 0;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x64: // d
				tmp_rect.x = 1;
				tmp_rect.w = 11;
				tmp_rect.h = 19;
				tmp_rect.y = 19;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x65: // e
				tmp_rect.x = 16;
				tmp_rect.w = 11;
				tmp_rect.h = 19;
				tmp_rect.y = 19;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x66: // f
				tmp_rect.x = 34;
				tmp_rect.w = 9;
				tmp_rect.h = 19;
				tmp_rect.y = 19;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x67: // g
				tmp_rect.x = 48;
				tmp_rect.w = 10;
				tmp_rect.h = 19;
				tmp_rect.y = 19;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x68: // h
				tmp_rect.x = 62;
				tmp_rect.w = 10;
				tmp_rect.h = 19;
				tmp_rect.y = 19;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x69: // i
				tmp_rect.x = 80;
				tmp_rect.w = 6;
				tmp_rect.h = 19;
				tmp_rect.y = 19;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x6A: // j
				tmp_rect.x = 91;
				tmp_rect.w = 10;
				tmp_rect.h = 19;
				tmp_rect.y = 19;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x6B: // k
				tmp_rect.x = 108;
				tmp_rect.w = 10;
				tmp_rect.h = 19;
				tmp_rect.y = 19;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x6C: // l
				tmp_rect.x = 123;
				tmp_rect.w = 6;
				tmp_rect.h = 19;
				tmp_rect.y = 19;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x6D: // m
				tmp_rect.x = 136;
				tmp_rect.w = 14;
				tmp_rect.h = 19;
				tmp_rect.y = 19;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x6E: // n
				tmp_rect.x = 152;
				tmp_rect.w = 10;
				tmp_rect.h = 19;
				tmp_rect.y = 19;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x6F: // o
				tmp_rect.x = 167;
				tmp_rect.w = 10;
				tmp_rect.h = 19;
				tmp_rect.y = 19;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x70: // p
				tmp_rect.x = 182;
				tmp_rect.w = 11;
				tmp_rect.h = 19;
				tmp_rect.y = 19;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x71: // q
				tmp_rect.x = 197;
				tmp_rect.w = 10;
				tmp_rect.h = 19;
				tmp_rect.y = 19;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x72: // r
				tmp_rect.x = 212;
				tmp_rect.w = 11;
				tmp_rect.h = 19;
				tmp_rect.y = 19;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x73: // s
				tmp_rect.x = 229;
				tmp_rect.w = 9;
				tmp_rect.h = 19;
				tmp_rect.y = 19;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x74: // t
				tmp_rect.x = 242;
				tmp_rect.w = 10;
				tmp_rect.h = 19;
				tmp_rect.y = 19;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x75: // u
				tmp_rect.x = 258;
				tmp_rect.w = 10;
				tmp_rect.h = 19;
				tmp_rect.y = 19;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x76: // v
				tmp_rect.x = 272;
				tmp_rect.w = 10;
				tmp_rect.h = 19;
				tmp_rect.y = 19;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x77: // w
				tmp_rect.x = 285;
				tmp_rect.w = 14;
				tmp_rect.h = 19;
				tmp_rect.y = 19;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x78: // x
				tmp_rect.x = 301;
				tmp_rect.w = 12;
				tmp_rect.h = 19;
				tmp_rect.y = 19;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x79: // y
				tmp_rect.x = 318;
				tmp_rect.w = 12;
				tmp_rect.h = 19;
				tmp_rect.y = 19;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;
			case 0x7A: // z
				tmp_rect.x = 332;
				tmp_rect.w = 11;
				tmp_rect.h = 19;
				tmp_rect.y = 19;
				SDL_BlitSurface( g_pFontList, &tmp_rect, pScreen, &rect);
				rect.x += tmp_rect.w;
				break;

		}

	}

	return TRUE;
}