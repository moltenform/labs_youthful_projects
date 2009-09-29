# Use an affine transformation matrix
# This example is a rotation/compression
from math import sin,cos,pi
angle = pi/6
stretch = 0.5

matrix=[
cos(angle)+stretch, -sin(angle),80,
sin(angle), cos(angle), 0]
size=(width,height)

imgOutput = imgInput.transform(size,Image.AFFINE,matrix)

# you can also warp a quadrilateral to fit a new rectangle.
quad = (15,55, #upper left
25,height, #lower left
width,height, #lower right
width-10, 0) #upper right
#imgOutput = imgInput.transform(size,Image.QUAD,quad)


#http://www.pythonware.com/library/pil/handbook/image.htm


