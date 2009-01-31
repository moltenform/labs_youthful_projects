from math import *

twopi = pi * 2.

def mid(p1, p2):
	"""Midpoint between two points"""
	return [(p1[0]+p2[0])/2., (p1[1]+p2[1])/2.]
	
def midw(p1, p2, weight):
	"""Weighted midpoint"""
	return [(p1[0]*(1-weight)+p2[0]*weight), (p1[1]*(1-weight)+p2[1]*weight)]

def addpt(p1, p2):
	"""Add two points"""
	return [p1[0] +p2[0],p1[1] +p2[1]] 

def distance(p1, p2):
	"""Distance between two points"""
	return sqrt((p1[1]-p2[1])**2 + (p1[0]-p2[0])**2)
	
def getangle(p1, p2):
	"""Angle between two points, radians"""
	return atan2( p2[1]-p1[1], p2[0]-p1[0] )

def extendpt(p, theta, length):
	"""Given a point, travel "length" in the "theta" direction. Like turtle graphics."""
	return [ p[0]+cos(theta)*length, p[1]+sin(theta)*length]

def makepts(str):
	"""Parse points from a string"""
	astr = str.replace(' ','').split('-')
	def fromstring(strCoords):
		coords = strCoords.split(',')
		return [float(coords[0]), float(coords[1])]
	return [ fromstring(strCoords) for strCoords in astr]

def polygonpts(nSides, radius=1.0):
	"""Create the points of a regular polygon"""
	return [[cos(theta)*radius, sin(theta)*radius] for theta in frange(0, twopi, nSides+1)[:-1] ]


#~ def segmentintersect(p1, p2, p3, p4):
	#~ m1 = (p2[1]-p1[1])/(p2[0]-p1[0])
	#~ m2 = (p4[1]-p3[1])/(p4[0]-p3[0])
	#~ scale = 1/(m1-m2)

def frange(start, stop, n):
    L = [0.0] * n
    nm1 = n - 1
    nm1inv = 1.0 / nm1
    for i in range(n):
        L[i] = nm1inv * (start*(nm1 - i) + stop*i)
    return L




if __name__ == '__main__':
	print getangle([0,0],[1,1])