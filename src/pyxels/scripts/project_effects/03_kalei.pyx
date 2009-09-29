from math import sin, cos, atan2, sqrt, pi
midx = width/2
midy = height/2
kalei = 3
kshift = 0.2

imgOutput = ImageChops.duplicate(imgInput)
imgInputArray = imgInput.load()
imgOutputArray = imgOutput.load()

loop:
	tx = (x-midx)
	ty = (y-midy)
	theta = (atan2(ty,tx) * kalei + kshift)
	
	radius = sqrt((tx*tx) + (ty*ty))
	newradius = (radius*radius)/max(width/2, height/2)
	
	newx =  midx+(newradius*cos(theta)) 
	newy =  midy+(newradius*sin(theta)) 
		
	if (newy>0 and newy<height and newx>0 and newx<width):
		imgOutputArray[x,y] = imgInputArray[newx,newy]

