from math import sin, cos, atan2, sqrt, pi
midx = width/2
midy = height/2
factor = 10

imgOutput = ImageChops.duplicate(imgInput)
imgInputArray = imgInput.load()
imgOutputArray = imgOutput.load()

loop:
	tx = (x-midx)
	ty = (y-midy)
	theta = atan2(ty,tx)
	
	radius = sqrt((tx*tx) + (ty*ty))
	newradius = (sqrt(radius)) * factor
	
	newx =  midx+(newradius*cos(theta)) 
	newy =  midy+(newradius*sin(theta)) 
		
	if (newy>0 and newy<height and newx>0 and newx<width):
		imgOutputArray[x,y] = imgInputArray[newx,newy]

