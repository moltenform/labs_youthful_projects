from math import *

def mid(p1, p2):
	return [(p1[0]+p2[0])/2., (p1[1]+p2[1])/2.]
def midw(p1, p2, weight):
	return [(p1[0]*(1-weight)+p2[0]*weight), (p1[1]*(1-weight)+p2[1]*weight)]

def addpt(p1, p2):
	return [p1[0] +p2[0],p1[1] +p2[1]] 

def distance(p1, p2):
	return sqrt((p1[1]-p2[1])**2 + (p1[0]-p2[0])**2)
def getangle(p1, p2):
	return atan2( p2[1]-p1[1], p2[0]-p1[0] )

def extendpt(p, theta, length):
	return [ p[0]+cos(theta)*length, p[1]+sin(theta)*length]

def makepts(str):
	astr = str.replace(' ','').split('-')
	def fromstring(strCoords):
		coords = strCoords.split(',')
		return [float(coords[0]), float(coords[1])]
	return [ fromstring(strCoords) for strCoords in astr]

#~ def segmentintersect(p1, p2, p3, p4):
	#~ m1 = (p2[1]-p1[1])/(p2[0]-p1[0])
	#~ m2 = (p4[1]-p3[1])/(p4[0]-p3[0])
	#~ scale = 1/(m1-m2)
	#~ return [ 

def myfrange(start, stop, n):
    L = [0.0] * n
    nm1 = n - 1
    nm1inv = 1.0 / nm1
    for i in range(n):
        L[i] = nm1inv * (start*(nm1 - i) + stop*i)
    return L

def polygonpts(nSides, radius=1.0):
	return [[cos(theta)*radius, sin(theta)*radius] for theta in myfrange(0, twopi, nSides+1)[:-1] ]

twopi = pi * 2

if __name__ == '__main__':
	print getangle([0,0],[1,1])