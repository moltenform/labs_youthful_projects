
//Contains code from codeproject,
//http://www.codeproject.com/KB/clipboard/archerclipboard1.aspx

#ifndef WIN32
int getFromclipboard(char * bufferIn, int bufSizeIn)
{
	return 0;
}
#else
#include "windows.h"
#include "common.h"
#include <stdio.h>
int getFromclipboard(char * bufferIn, int bufSizeIn)
{
	int charsCopied = 0;
	if (OpenClipboard(NULL)) 
	{
		if (::IsClipboardFormatAvailable(CF_TEXT))
		{
			// Retrieve the Clipboard data (specifying that 
			// we want ANSI text (via the CF_TEXT value).
			HANDLE hClipboardData = GetClipboardData(CF_TEXT);

			// Call GlobalLock so that to retrieve a pointer
			// to the data associated with the handle returned
			// from GetClipboardData.
			char *pchData = (char*)GlobalLock(hClipboardData);

			char * nextPtr = strncpy(bufferIn, pchData, MAX(0,bufSizeIn-2));
			//charsCopied = (int)(nextPtr-bufferIn);
			charsCopied = strlen(pchData);

			// Unlock the global memory.
			GlobalUnlock(hClipboardData);

			}
		//else printf("no avail?");
		// Finally, when finished I simply close the Clipboard
		// which has the effect of unlocking it so that other
		// applications can examine or modify its contents.
		CloseClipboard();
	}
	//else printf("no open cboard?");
	return charsCopied;
}

#endif