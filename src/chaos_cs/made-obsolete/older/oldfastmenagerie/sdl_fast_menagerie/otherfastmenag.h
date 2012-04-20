
int alternateCountPhasePlotWidth(MenagFastSettings*settings,double c1, double c2, int whichThread, int*outWidth, int*outHeight)
{
	*outWidth = *outWidth=0;
	int CURRENTID =  whichThread? ++CURRENTID1 : ++CURRENTID2;
	int*arr = whichThread? arrThread1:arrThread2;
	double x,x_,y;
	//x=0.01; y=0.01; //experimental. it's true.
	x=0.0; y=0.001; //experimental. it's true.
	int counted=0;
	double X0=-3, X1=1, Y0=-3, Y1=3; ////////////////////////////////////////////////
	BOOL hasbeennegative=FALSE;
	BOOL hasbeenpos=FALSE;
	int rightmost=0, leftmost=PHASEW+2;
	int biggesty=0, smallesty=PHASEH+2;

	for (int i=0; i<50/4; i++)
	{
		x_ = c1*x - y*y; y= c2*y + x*y; x=x_;
		x_ = c1*x - y*y; y= c2*y + x*y; x=x_;
		x_ = c1*x - y*y; y= c2*y + x*y; x=x_;
		x_ = c1*x - y*y; y= c2*y + x*y; x=x_;
		if (ISTOOBIG(x) || ISTOOBIG(y))
			return 0;//counted;
	}
	//drawing time.
	for (int i=0; i<1000/2; i++) //see how changes if drawing increases?
	{
		x_ = c1*x - y*y; y= c2*y + x*y; x=x_;
		int px = lrint(PHASEW * ((x - X0) / (X1 - X0)));
		int py_times_256 = lrint(PHASEW*PHASEH * ((y - Y0) / (Y1 - Y0)));
		if (py_times_256 >= 0 && py_times_256 < PHASEH*PHASEW && px>=0 && px<PHASEW)
		    if (arr[px + py_times_256 ]!=CURRENTID)
		    { arr[px + py_times_256 ]=CURRENTID; counted++;}

		if (px>rightmost) rightmost=px;
		if (px<leftmost) leftmost=px;
		int py = py_times_256/256;
		if (py<smallesty) smallesty=py;
		if (py>biggesty) biggesty=py;

		x_ = c1*x - y*y; y= c2*y + x*y; x=x_;
		px = lrint(PHASEW * ((x - X0) / (X1 - X0)));
		py_times_256 = lrint(PHASEW*PHASEH * ((y - Y0) / (Y1 - Y0)));
		if (py_times_256 >= 0 && py_times_256 < PHASEH*PHASEW && px>=0 && px<PHASEW)
		    if (arr[px + py_times_256 ]!=CURRENTID)
		    { arr[px + py_times_256 ]=CURRENTID; counted++;}

		if (ISTOOBIG(x) || ISTOOBIG(y))
			return 0;//counted;
		//if (y<0) hasbeennegative=TRUE;
		//if (y>0) hasbeenpos=TRUE;
		if (px>rightmost) rightmost=px;
		if (px<leftmost) leftmost=px;
		 py = py_times_256/256;
		if (py<smallesty) smallesty=py;
		if (py>biggesty) biggesty=py;
	}
	/*if (hasbeenpos&&hasbeennegative) return 200;
	if (hasbeennegative) return 500;
	if (hasbeenpos) return 300;*/
	*outWidth = (rightmost-leftmost);
	*outHeight = (biggesty-smallesty);
	return (rightmost-leftmost);
	//return 100;
	//return hasbeennegative?500:200;
	//return counted;
}

//next: a state machine one.

//the "sampling" one
int alternateCountPhasePlotSampling(MenagFastSettings*settings,double c1, double c2, int whichThread)
{
	int CURRENTID =  whichThread? ++CURRENTID1 : ++CURRENTID2;
	int*arr = whichThread? arrThread1:arrThread2;
	double x,x_,y;
	x=0.01; y=0.01; //experimental. it's true.
	int counted=0;
	double X0=-3, X1=1, Y0=-3, Y1=3; ////////////////////////////////////////////////

	for (int i=0; i<50/4; i++)
	{
		x_ = c1*x - y*y; y= c2*y + x*y; x=x_;
		x_ = c1*x - y*y; y= c2*y + x*y; x=x_;
		x_ = c1*x - y*y; y= c2*y + x*y; x=x_;
		x_ = c1*x - y*y; y= c2*y + x*y; x=x_;
		if (ISTOOBIG(x) || ISTOOBIG(y))
			return 0;//counted;
	}
	//drawing time.
	for (int i=0; i<1000/2; i++) //see how changes if drawing increases?
	{
		x_ = c1*x - y*y; y= c2*y + x*y; x=x_;
		if (x>-.6 && x<-.5)
		{
			int py = lrint(PHASEH* ((y - Y0) / (Y1 - Y0))); 
			if (py>=0 && py<=PHASEH && arr[py]!=CURRENTID)
		    { arr[py]=CURRENTID; counted++;}
		}
		x_ = c1*x - y*y; y= c2*y + x*y; x=x_;
		if (x>-.6 && x<-.5)
		{
			int py = lrint(PHASEH* ((y - Y0) / (Y1 - Y0))); 
			if (py>=0 && py<=PHASEH && arr[py]!=CURRENTID)
		    { arr[py]=CURRENTID; counted++;}
		}

		if (ISTOOBIG(x) || ISTOOBIG(y))
			return counted;
	}
	return counted;
}