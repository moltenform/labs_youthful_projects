::REM ~ PATH=%PATH%;C:\Program Files\Dev-Cpp\bin
::REM ~ gcc imgstuff.c -o imgstuff.exe -Wall
::#run devenv to build
::#~ http://www.c-sharpcorner.com/UploadFile/tharakram/BuildDotNetSolution11162005052301AM/BuildDotNetSolution.aspx
PATH=%PATH%;C:\Program Files\Microsoft Visual Studio 8\VC\bin
PATH=%PATH%;C:\Program Files\Microsoft Visual Studio 8\VC\lib
PATH=%PATH%;C:\Program Files\Microsoft Visual Studio 8\VC\include
PATH=%PATH%;C:\Program Files\Microsoft Visual Studio 8\Common7\IDE
devenv  fastpixelpic05.sln /Build Release
