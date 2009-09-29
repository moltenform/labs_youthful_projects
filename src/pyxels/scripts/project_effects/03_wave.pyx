from math import sin, cos, atan2, sqrt, pi
midx = width/2
midy = height/2
nwave = 12

imgOutput = ImageChops.duplicate(imgInput)
imgInputArray = imgInput.load()
imgOutputArray = imgOutput.load()

twopiconst = 2.0 * pi / 128.0

loop:
	newx = x + nwave*sin( y * twopiconst)
	newy = y + nwave*cos( x * twopiconst)
	
	if (newy>0 and newy<height and newx>0 and newx<width):
		imgOutputArray[x,y] = imgInputArray[newx,newy]

