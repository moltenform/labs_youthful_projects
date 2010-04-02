#pragma once

void oscillate(double curA,double curB,double *outA, double *outB)
{
	static double statePos=0.0, stateFreq=0.0;
	if (statePos>31.415926) statePos=0.0;
	if (stateFreq>31.415926) stateFreq=0.0;
	stateFreq+=0.01;
	statePos+=stateFreq;

	//the frequency itself oscillates
	double oscilFreq = 0.09 + sin(stateFreq)/70;
	*outA = curA+ sin(statePos*.3702342521232353)/550;
	*outB = curB+ cos(statePos)/400; 
}

int breathemain(  )
{
	double curA=0.0, curB=0.0, actualA, actualB;
	bool breathe = false;

	double sliding = 10.0;

curA += (targetA-curA)/sliding;
	curB += (targetB-curB)/sliding;

for ( ; ; )
{
	if ( SDL_PollEvent ( &event ) )
	{
	//an event was found
	if ( event.type == SDL_QUIT ) break ;
	else if (event.type==SDL_KEYDOWN) {
		 if (event.key.keysym.sym == SDLK_UP) targetB += 0.005;
		  else if (event.key.keysym.sym == SDLK_DOWN) targetB -= 0.005;
		  else if (event.key.keysym.sym == SDLK_LEFT) targetA -= 0.005;
		  else if (event.key.keysym.sym == SDLK_RIGHT) targetA += 0.005;
		else break;
	}
	}

	if (breathe)
	{
		oscillate(curA, curB, &actualA, &actualB);
	}
	else
	{
		actualA = curA; 
		actualB = curB;
	}

}

