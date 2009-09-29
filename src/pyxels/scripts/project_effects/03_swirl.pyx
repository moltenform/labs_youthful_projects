from math import sin, cos, atan2, sqrt, pi
midx = width/2
midy = height/2
factor = .015 
# a large factor such as 3.0 results in moire patterns.

imgOutput = ImageChops.duplicate(imgInput)
imgInputArray = imgInput.load()
imgOutputArray = imgOutput.load()

#algorithm http://www.codeproject.com/KB/GDI-plus/displacementfilters.aspx

loop:
	tx = (x-midx)
	ty = (y-midy)
	theta = atan2(ty,tx)
	
	radius = sqrt((tx*tx) + (ty*ty))
	
	newx =  midx+(radius*cos(theta+ factor*radius)) 
	newy =  midy+(radius*sin(theta+ factor*radius)) 
		
	if (newy>0 and newy<height and newx>0 and newx<width):
		imgOutputArray[x,y] = imgInputArray[newx,newy]

