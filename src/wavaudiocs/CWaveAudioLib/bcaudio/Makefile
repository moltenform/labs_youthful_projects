

bcaudio: main.c
	gcc -o main \
	main.c bcaudio.c effects.c fft.c \
	effects_audacity.c inplace_effects.c synth.c synth_construct.c wav_persist.c \
	-lm -Wall

#~ CC=gcc
#~ CFLAGS = -Wall
#~ main: main.o bcaudio.o

#~ bcaudio.o: effects.o effects_audacity.o \
      #~ inplace_effects.o synth.o synth_construct.o wav_persist.o 
      
