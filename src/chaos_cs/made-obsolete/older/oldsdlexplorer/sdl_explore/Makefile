
sdlexplorer: main.cpp
	gcc -o sdlexplorer main.cpp common.cpp \
	breathe.cpp io.cpp menagerie.cpp phaseportrait.cpp \
	-lm -lSDL -Wall -fno-exceptions -fno-rtti

#the -fno-exceptions -fno-rtti are needed, otherwise it fails because it
#is looking for the c++ symbols (gxx_personality_v0) having to do with debugging.
