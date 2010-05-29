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

struct SDLFont
{
  SDL_Surface *font;    // The SDL Surface for the font image
  int width;            // Width of the SDL Surface (same as the height)
  int charWidth;        // Width of one block character in the font (fontWidth/16)
  int *widths;          // Real widths of all the fonts
  unsigned char *data;  // The raw font data
};
SDLFont *initFont(char *fontdir, float r, float g, float b, float a);
void drawString(SDL_Surface *screen, SDLFont *font, int x, int y, const char *str);
void freeFont(SDLFont *font);

/*
  font.cpp
  Cone3D SDL font routines.
  Made by Marius Andra 2002
  http://cone3d.gamedev.net

  You can use the code for anything you like.
  Even in a commercial project.
  But please let me know where it ends up.
  I'm just curious. That's all.
*/

SDLFont *currentFont = NULL;
BOOL ShowText(const char* text, int pos_x, int pos_y, SDL_Surface* pScreen)
{
	if (!text) return FALSE;
	if (!currentFont) 
		currentFont = initFont("data/fontsm", 0.5,0.5,0.5,1);
	drawString(pScreen, currentFont, pos_x, pos_y, text);
	return TRUE;
}

void Free_Fonts()
{
	freeFont(currentFont);
	currentFont = NULL;
}


// this function draws one part of an image to some other part of an other image
// it's only to be used inside the font.cpp file, so it's not available to any
// other source files (no prototype in font.h)
void _fontDrawIMG(SDL_Surface *screen, SDL_Surface *img, int x, int y, int w,
                                                        int h, int x2, int y2)
{
  SDL_Rect dest;
  dest.x = x;
  dest.y = y;
  SDL_Rect src;
  src.x = x2;
  src.y = y2;
  src.w = w;
  src.h = h;
  SDL_BlitSurface(img, &src, screen, &dest);
}

// this function loads in our font file
SDLFont *initFont(char *fontdir, float r, float g, float b, float a)
{
  // some variables
  SDLFont *tempFont;               // a temporary font
  FILE *fp;                        // file pointer - used when reading files
  char tempString[100];            // temporary string
  unsigned char tmp;               // temporary unsigned char
  int width;                       // the width of the font
  SDL_Surface *tempSurface;        // temporary surface

  // find out about the size of a font from the ini file
  sprintf(tempString,"%s/%s",fontdir,"font.ini");
  fp = fopen(tempString, "rb");
  if( fp == NULL )
  {
    return 0;
  }
  fscanf(fp, "%d", &width);
  fclose(fp);

  // let's create our font structure now
  tempFont = new SDLFont;
  tempFont->data = new unsigned char[width*width*4];
  tempFont->width = width;
  tempFont->charWidth = width/16;

  // open the font raw data file and read everything in
  sprintf(tempString,"%s/%s",fontdir,"font.raw");
  fp = fopen(tempString, "rb");
  if( fp != NULL )
  {
    for(int i=0;i<width*width;i++)
    {
      tmp = fgetc(fp);
      tempFont->data[i*4] = (unsigned char)255*(unsigned char)r;
      tempFont->data[i*4+1] = (unsigned char)255*(unsigned char)g;
      tempFont->data[i*4+2] = (unsigned char)255*(unsigned char)b;
      tempFont->data[i*4+3] = (unsigned char)(((float)tmp)*a);
    }
  } else {
    return 0;
  }
  fclose(fp);
  // now let's create a SDL surface for the font
  Uint32 rmask,gmask,bmask,amask;
  #if SDL_BYTEORDER == SDL_BIG_ENDIAN
  rmask = 0xff000000;
  gmask = 0x00ff0000;
  bmask = 0x0000ff00;
  amask = 0x000000ff;
  #else
  rmask = 0x000000ff;
  gmask = 0x0000ff00;
  bmask = 0x00ff0000;
  amask = 0xff000000;
  #endif
  tempSurface = SDL_CreateRGBSurfaceFrom(tempFont->data, width, width,
                              32, width*4, rmask, gmask, bmask, amask);
  tempFont->font = SDL_DisplayFormatAlpha(tempSurface);
  SDL_FreeSurface(tempSurface);

  // let's create a variable to hold all the widths of the font
  tempFont->widths = new int[256];

  // now read in the information about the width of each character
  sprintf(tempString,"%s/%s",fontdir,"font.dat");
  fp = fopen(tempString, "rb");
  if( fp != NULL )
  {
    for(int i=0;i<256;i++)
    {
      tmp = fgetc(fp);
      tempFont->widths[i]=tmp;
    }
  }
  fclose(fp);

  /// return the new font
  return tempFont;
}

void drawStringf(SDL_Surface *screen, SDLFont *font, int x, int y, char *str, ...)
{
	char string[1024];                  // Temporary string

  va_list ap;                         // Pointer To List Of Arguments
  va_start(ap, str);                  // Parses The String For Variables
  vsprintf(string, str, ap);          // And Converts Symbols To Actual Numbers
  va_end(ap);                         // Results Are Stored In Text
	drawString(screen,font, x,y, string);
}

// here we draw the string
void drawString(SDL_Surface *screen, SDLFont *font, int x, int y, const char *string)
{
 
  int len=(int) strlen(string);             // Get the number of chars in the string
  int xx=0;            // This will hold the place where to draw the next char
  for(int i=0;i<len;i++)              // Loop through all the chars in the string
  {
	if (string[i]=='\n') //a newline (my addition)
	{
		y+=font->charWidth + 2;
		xx=0;
		continue;
	}
	else if (string[i]=='\t') //a tab (my addition)
	{
		xx= 100 * (int)(xx/100.0 + 1); //round up to the nearest 100px
		continue;
	}
    // This may look scary, but it's really not.
    // We only draw one character with this code.
    // At the next run of the loop we draw the next character.

    // Remember, the _fontDrawIMG function looks like this:
	// void fontDrawIMG(SDL_Surface *screen, SDL_Surface *img, int x, int y,
    //                                         int w, int h, int x2, int y2)

    // We draw onto the screen SDL_Surface a part of the font SDL_Surface.
    _fontDrawIMG(
      screen,
      font->font,
    // We draw the char at pos [x+xx,y].
    // x+xx: this function's parameter x + the width of all the characters before
    // this one, so we wouldn't overlap any of the previous characters in the string
    // y: this function's y parameter
      xx+x,
      y,
    // For the width of the to-be-drawn character we take it's real width + 2
      font->widths[string[i]]+2,
    // And for the height we take the height of the character (height of the font/16)
      font->charWidth,
    // Now comes the tricky part
    // The font image DOES consist of 16x16 grid of characters. From left to
    // right in the image, the ascii values of the characters increase:
    // The character at block 0x0 in the font image is the character with the
    // ascii code 0, the character at the pos 1x0 has the ascii code 1, the char
    // at the pos 15x0 has the ascii code 15. And that's the end of the first row
    // Now in the second row on the image, the first character (0x1) has the ascii
    // value 16, the fourth character on the second row of the image (3x1) has the ascii
    // value 19. To calculate the ascii value of the character 3x1, we use the
    // really simple equation: row*[number of thing in one row (=16)]+column pos.
    // So the position 3x1 has the ascii value 1*16+3 = 19. The character in the
    // image at the position 8x12 has the ascii value 12*16+8=200, and so on.
    // But this isn't much of use to us since we KNOW the ascii value of a character,
    // but we NEED to find out it's position on the image.
    // First we'll get the column on the image. For that we'll divide the ascii value
    // with 16 (the number of columns) and get it's remainder (we'll use the modulus
    // operator). We'll do this equation to get the column: [ascii value]%16.
    // Now to get to the position of the column in pixels, we multiply the ascii
    // value by the width of one column ([font image width]/16)
    // And so the equation to get the first pixel of the block becomes:
    // [font image width]%16*[1/16th of the font image width]
    // Now, since all the letters are centered in each cell (1/16th of the image),
    // we need to get the center of the cell. This is done by adding half the width
    // of the cell to the number we got before. And the only thing left to do is to
    // subtract half of the character's real width and we get the x position from where
    // to draw our character on the font map :)
      (string[i]%16*font->charWidth)+((font->charWidth/2)-(font->widths[string[i]])/2),
    // To get the row of the character in the image, we divide the ascii value of
    // the character by 16 and get rid of all the numbers after the point (.)
    // (if we get the number 7.125257.., we remove the .125157... and end up with 7)
    // We then multiply the result with the height of one cell and voila - we get
    // the y position!
      (((int)string[i]/16)*font->charWidth)
    );

    // Now we increase the xx printed string width counter by the width of the
    // drawn character
    xx+=font->widths[string[i]];
  }
}

// This function returns the width of a string
int stringWidth(SDLFont *font,char *str,...)
{
  char string[1024];                  // Temporary string

  va_list ap;                         // Pointer To List Of Arguments
  va_start(ap, str);                  // Parses The String For Variables
  vsprintf(string, str, ap);          // And Converts Symbols To Actual Numbers
  va_end(ap);                         // Results Are Stored In Text

  // Now we just count up the width of all the characters
  int xx=0;
  int len=(int) strlen(string);
  for(int i=0;i<len;i++)
  {
    // Add their widths together
    xx+=font->widths[string[i]];
  }

  // and then return the sum
  return xx;
}

// Clear up
void freeFont(SDLFont *font)
{
  delete [] font->widths;
  delete [] font->data;
  SDL_FreeSurface(font->font);
  delete font;
}




