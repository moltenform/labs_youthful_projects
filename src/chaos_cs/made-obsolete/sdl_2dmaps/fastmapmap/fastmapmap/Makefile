
sdlexplorer: main.cpp
	gcc -o fastmapmap main.cpp animate.cpp \
	common.cpp configfiles.cpp coordsdiagram.cpp font.cpp \
	menagerie.cpp palette.cpp phaseportrait.cpp \
	-lm -lSDL -Wall -fno-exceptions -fno-rtti

#the -fno-exceptions -fno-rtti are needed, otherwise it fails because it
#is looking for the c++ symbols (gxx_personality_v0) having to do with debugging.
