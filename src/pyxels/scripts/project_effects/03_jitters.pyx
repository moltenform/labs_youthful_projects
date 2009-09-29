from random import randint
v = 2
nv = -v
loop:
	mvx = min(max(0, x + randint(nv,v)), width-1)
	mvy = min(max(0, y + randint(nv,v)), height-1)
	ra[mvx,mvy] = ra[x,y]
	ga[mvx,mvy] = ga[x,y]
	ba[mvx,mvy] = ba[x,y]

