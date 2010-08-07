//move these into main.c, compile, and run to test.

int exfft()
{
	//round-trips from .wav to .dat to .wav
	CAudioData * w1=NULL;  CAudioData  *w2=NULL;
	char inname[] = "..\\..\\media\\songclips\\rb_short.wav";
	errormsg msg = caudiodata_loadwave(&w1, inname);
	if (msg != OK) {puts(msg); return 0;}
	
	msg=dumpToFrequencyAngles(w1,  "testout\\fft_opt.dat", 2048);
	if (msg != OK) {puts(msg); return 0;}
	
	msg=readFrequenciesToSamples(&w2, "testout\\fft_opt.dat");
	if (msg != OK) {puts(msg); return 0;}
	
	msg = caudiodata_savewave(w2, "testout\\fft_opt_roundtrip.wav", 16);
	if (msg != OK) {puts(msg); return 0;}
	
	caudiodata_dispose( w1);
	caudiodata_dispose( w2);
	return 0;
}

caudiodata_void example1()
{
	//create a sample sine wave.
	CAudioData* audio =  caudiodata_new();
	errormsg msg = caudiodata_allocate(audio, 44100*4, 1, 44100); // 4 seconds of audio, mono.
	if (msg!=OK) puts(msg);
	
	double freq = 300;
	int i;
	for (i = 0; i < audio->length; i++)
	{
		audio->data[i] = 0.9 * sin(i * freq * 2.0 * PI / (double)audio->sampleRate);
	}
	
	FILE * f = fopen("out.wav", "wb");
	msg = caudiodata_savewave(audio, f, 16);
	if (msg != OK) puts(msg);
	fclose(f);
	caudiodata_dispose( audio);
}


void example_mix()
{
	CAudioData* w1 =  caudiodata_new();
	CAudioData* w2 =  caudiodata_new();
	
	synth_sin(&w1, 300, 4.0, 0.8); //sine wave, 300Hz
	synth_sin(&w2, 430, 4.0, 0.8); //sine wave, 430Hz
	
	CAudioData* mix=NULL;
	char* msg = effect_mix(&mix, w1, w2, 0.5, 0.5);
	if (msg!=OK) puts(msg);
	
	msg = caudiodata_savewave(mix, "testout\\out_mix.wav", 16);
	if (msg != OK) puts(msg);
	
	caudiodata_dispose(mix);
	caudiodata_dispose(w1);
	caudiodata_dispose(w2);
}

void loadtests()
{
	char tests[6][255]={
"..\\..\\media\\bitrates\\d22k16bit1ch.wav",
"..\\..\\media\\bitrates\\d22k8bit1ch.wav",
"..\\..\\media\\bitrates\\d22k8bit2ch.wav",
"..\\..\\media\\bitrates\\d44k16bit1ch.wav",
"..\\..\\media\\bitrates\\d44k16bit2ch.wav",
"..\\..\\media\\bitrates\\d44k8bit1ch.wav"
	};
	
	int i; for (i=0; i<6; i++)
	{
		CAudioData * audio;
		FILE*fin = fopen(tests[i], "rb");
		errormsg msg = caudiodata_loadwave(&audio, fin);
		if (msg != OK) puts(msg);
		fclose(fin);
		
		printf("\n%s\nLength :%d", tests[i], audio->length);
		inplaceeffect_volume(audio,0.1);
		
		char buf[128];
		sprintf(buf, "out%d.wav", i);
		msg = caudiodata_savewave(audio, buf, 16);
		if (msg != OK) puts(msg);
		
		caudiodata_dispose(audio);
	}
}

void mixwithsine() // or modulate, or append, an easy change
{
	CAudioData * w1; CAudioData* w2;CAudioData* out;
	FILE*fin = fopen("..\\..\\media\\bitrates\\d22k8bit1ch.wav", "rb");
	errormsg msg = caudiodata_loadwave(&w1, fin);
	if (msg != OK) puts(msg);
	fclose(fin);
	
	synth_sin(&w2, 300, caudiodata_getLengthInSecs(w1), 0.8); //sine wave, 300Hz
	
	msg =  effect_mix(&out, w1, w2, 0.5, 0.1); //effect_append(&out, w2, w1);
	if (msg != OK) { puts(msg);  return 0;}
	msg = caudiodata_savewave(out, "testout\\out.wav", 16);
	if (msg != OK) puts(msg);
	
	caudiodata_dispose( w1);
	caudiodata_dispose( w2);
	caudiodata_dispose( out);
}
void appendandclone()
{
	CAudioData * wsine; CAudioData* wsinelouder = NULL;CAudioData* out;

	synth_sin(&wsine, 300, 1.0, 0.3); //sine wave, 300Hz
	
	caudiodata_clone(& wsinelouder, wsine);
	inplaceeffect_volume(wsinelouder, 3);
	
	msg =  effect_append(&out, wsine, wsinelouder);
	if (msg != OK) { puts(msg);  return 0;}
	msg = caudiodata_savewave(out, "testout\\out.wav", 16);
	if (msg != OK) puts(msg);
	
	caudiodata_dispose( wsine);
	caudiodata_dispose( wsinelouder);
	caudiodata_dispose( out);
}

void testfadein()
{
	CAudioData* audio;
	
	synth_sin(&audio, 300, 4.0, 0.3); //sine wave, 300Hz
	
	inplaceeffect_fade(audio, 0, 2.5); //fade out
	//~ inplaceeffect_fade(audio, 1, 2.5); // fade in
	
	msg = caudiodata_savewave(audio, "testout\\out.wav", 16);
	if (msg != OK) puts(msg);
	caudiodata_dispose( audio);
}
void testReverse()
{
	CAudioData * w1; 
	FILE*fin = fopen("..\\..\\media\\bitrates\\d22k8bit1ch.wav", "rb");
	errormsg msg = caudiodata_loadwave(&w1, fin);
	if (msg != OK) puts(msg);
	fclose(fin);
	
	inplaceeffect_reverse(w1);
	
	msg = caudiodata_savewave(w1, "testout\\outrev.wav", 16);
	if (msg != OK) puts(msg);
	caudiodata_dispose( w1);
}

void testTremelo()
{
	CAudioData* audio;
	synth_sin(&audio, 300, 4.0, 0.3); //sine wave, 300Hz
	
	inplaceeffect_tremelo(audio, 4, 0.2);

	msg = caudiodata_savewave(audio, "testout\\out.wav", 16);
	if (msg != OK) puts(msg);
	caudiodata_dispose( audio);
}

void testPitchScaleOrVibrato()
{
	CAudioData * w1;CAudioData* out;
	FILE*fin = fopen("longinput.wav", "rb");
	errormsg msg = caudiodata_loadwave(&w1, fin);
	if (msg != OK) puts(msg);
	fclose(fin);

	//~ msg =  effect_scale_pitch_duration(&out, w1, 0.7);
	msg =  effect_vibrato(&out, w1, 0.2, 0.1);
	if (msg != OK) { puts(msg);  return 0;}

	msg = caudiodata_savewave(out, "testout\\out.wav", 16);
	if (msg != OK) puts(msg);

	
	caudiodata_dispose( w1);
	caudiodata_dispose( out);
}

void testSynth()
{
	CAudioData* audio;
	synth_sin(&audio, 300, 10.0, 0.8);
	

	errormsg msg = caudiodata_savewave(audio, "testout\\out.wav", 16);
	if (msg != OK) puts(msg);

	caudiodata_dispose( audio);
}
void testRedGlitchNess()
{
	/*
	CAudioData* w1; CAudioData* w2; CAudioData* combo;
	synth_redglitch(&w1, 100, 20.0, 0.8, 0.09, 0.261); //- cool!
	synth_redglitch(&w2, 99, 20.0, 0.8, 0.09, 0.33);
	
	//~ synth_redglitch(&w1, 100.0, 20.0, 0.8, 0.02, 0.061); //- also cool!
	//~ synth_redglitch(&w2, 100.6, 20.0, 0.8, 0.02, 0.061);
	
	//~ synth_redglitch(&w1, 80.0, 20.0, 0.8, 0.09, 0.661); //- also cool!
	//~ synth_redglitch(&w2, 120.6, 20.0, 0.8, 0.09, 0.661);
	
	//Left channel is w1, right channel is w2
	combo = caudiodata_new();
	combo->length = w1->length;
	combo->sampleRate = w1->sampleRate;
	combo->data = w1->data;
	combo->data_right = w2->data;
	
	FILE * f = fopen("outcombo.wav", "wb");
	errormsg msg = caudiodata_savewave(combo, f, 16);
	if (msg != OK) puts(msg);
	fclose(f);
	
	caudiodata_dispose( combo);
	caudiodata_dispose( w1);
	caudiodata_dispose( w2);
	*/
}
void testWriteToMemory()
{
	CAudioData* w1; CAudioData* w2; CAudioData* combo;
	synth_sin(&w1, 300, 2.0, 0.8);
	synth_sin(&w2, 303, 2.0, 0.8);
	
	//Left channel is w1, right channel is w2
	combo = caudiodata_new();
	combo->length = w1->length;
	combo->sampleRate = w1->sampleRate;
	combo->data = w1->data;
	combo->data_right = w2->data;
	
	//~ errormsg msg = caudiodata_savewave(combo, "testout\\outsines.wav", 16);
	char *inmemory; uint inmemorylength;
	errormsg msg = caudiodata_savewavemem(&inmemory, &inmemorylength, combo, 16);
	FILE*f=fopen("testout\\outsines.wav","wb");
	fwrite(inmemory, 1, inmemorylength, f);
	fclose(f);
	
	if (msg != OK) puts(msg);
	caudiodata_dispose( combo);
	//~ caudiodata_dispose( w1);
	//~ caudiodata_dispose( w2);
	
	free(inmemory);
}

void testAudacityPhaser()
{
	CAudioData * w1;
	char inname[] = "..\\..\\media\\songclips\\rb_long.wav";
	errormsg msg = caudiodata_loadwave(&w1, inname);
	if (msg != OK) {puts(msg); return 0;}
	
	//Phaser:
	//double freq, double fb, int depth, int stages, int drywet
	//msg =  effect_phaseraud(w1, .7, -60, 130, 4, 255);
	
	//Wahwah: (use simon.wav)
	//double freq, double depth, double freqofs, double res)
	//msg =  effect_wahwahaud(w1, 1.5, 0.7, 0.1, 2.5);
	
	if (msg != OK) { puts(msg);  return 0;}
	
	char outname[] = "testout\\out2.wav"; 
	msg = caudiodata_savewave(w1, outname, 16);
	if (msg != OK) {puts(msg); return 0;}
	
	caudiodata_dispose( w1);
}

