
#include <stdio.h>
#include "util.h"
#include "util_win32.h"

// doesn't take a filter because that would hinder recursion into directories
bool WalkThroughFiles_Impl(const char* szPath, void* obj, PfnWalkfilesCallback callback, int nCurrentdepth, int nMaxdepth)
{
	WIN32_FIND_DATA findFileData;
	char szBuffer[MAX_PATH];
	sprintf_s(szBuffer, _countof(szBuffer), "%s\\*", szPath);

	if (nCurrentdepth > nMaxdepth)
	{
		printf("Error: exceeded max depth!");
		return false;
	}

	HANDLE hFind = FindFirstFile(szBuffer, &findFileData);
    if (hFind == INVALID_HANDLE_VALUE)
        return true;
    do
    {
        if (findFileData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)
        {
            if (!StringAreEqual(".", findFileData.cFileName) && !StringAreEqual("..", findFileData.cFileName))
            {
				sprintf_s(szBuffer, _countof(szBuffer), "%s\\%s", szPath, findFileData.cFileName);
				bool b = WalkThroughFiles_Impl(szBuffer, obj, callback, nCurrentdepth+1, nMaxdepth);
				if (!b) { FindClose(hFind); return b; }
            }
        }
        else
        {
			sprintf_s(szBuffer, _countof(szBuffer), "%s\\%s", szPath, findFileData.cFileName);
			bool b = callback(obj, szBuffer);
			if (!b) { FindClose(hFind); return b; }
        }
    } while (FindNextFile(hFind, &findFileData) != 0);
    FindClose(hFind);
	return true;
}

bool WalkThroughFiles(const char* szDir, void* obj, PfnWalkfilesCallback callback, int nMaxdepth)
{
	return WalkThroughFiles_Impl(szDir, obj, callback, 0, nMaxdepth);
}

