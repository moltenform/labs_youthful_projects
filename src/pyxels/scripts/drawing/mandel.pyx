#couldn't help it...
scaledownx = (width/3.0)
scaledowny = (height/3.0)

loop:
	x0 = ptx = x/scaledownx - 2
	y0 = pty = y/scaledowny - 1.5
	iter = 0
	while ptx*ptx + pty*pty < 4 and iter < 100:
		ptx, pty = ptx*ptx - pty*pty + x0, 2*ptx*pty+y0
		iter+=1
	if iter==100: R=0
	else: R=iter*3
	G=0; B=0