#Images: http://effbot.org/tkinterbook/photoimage.htm
#http://www.daniweb.com/code/snippet296.html
from recursivemath import *
import math
import wrappers

def getglobals(): return globals()
# See documentation in Help->Engine Notes

class RecShape():
	def __init__(self, app):
		self.app = app
	def rule(self, parray):
		pass
	def axiom(self):
		pass
	def draw(self, d, res=-1):
		if res==-1: res = self.axiom()
		
		# Iterate
		for i in range(d):
			res = self.rule(res)
		# Draw
		for activeSite in res:
			self.app.drawLine(activeSite[0], activeSite[1])
	def animate(self, nIterationsPerFrame, strParameter, fStart, fStop, nFrames,time=400):
		fIncr = (fStop-fStart)/float(nFrames)
		if nFrames < 0: return
		def iter(app,objShape,nIterationsPerFrame,timeInc,  fVal, fIncr, n, nMax):
			if n > nMax:
				return
			setattr(objShape,strParameter, fVal)
			#Draw it
			app.clear()
			objShape.draw(nIterationsPerFrame)
			app.canvas.after(timeInc, wrappers.Callable(iter, app, objShape,nIterationsPerFrame,timeInc, fVal+fIncr, fIncr, n+1, nMax))
			
		iter(app,self,nIterationsPerFrame, time, fStart, fIncr,0, nFrames)
	
class PaintShape(RecShape):
	# This class has drawing instructions within the rule
	def draw(self, d, res=-1):
		if res==-1: res = self.axiom()
		if d<=0: return
		
		return self.draw(d-1, self.rule(res))

class SquareSpiralCool(PaintShape):
	w = 0.5
	def axiom(self):
		return [  polygonpts(4) ]

	def rule(self, parray):
		p = parray[0] #There is only one active site
		w = self.w
		newrect = [ midw(p[0], p[1],w),midw(p[1], p[2],w),midw(p[2], p[3],w),midw(p[3], p[0],w) ]
		self.app.drawLines(newrect)
		return [ newrect ]
#$example$
#$sq = SquareSpiralCool(app)
#$sq.w = 0.65
#$sq.draw(20)

class SquareSpiral(PaintShape):
	def axiom(self):
		return [  polygonpts(4) ]

	def rule(self, parray):
		p = parray[0] #There is only one active site
		newrect = [ mid(p[0], p[1]),mid(p[1], p[2]),mid(p[2], p[3]),mid(p[3], p[0]) ]
		self.app.drawLines(newrect)
		return [ newrect ]
#$example$
#$sq = SquareSpiral(app)
#$sq.draw(20)



class PentaStar(PaintShape):
	w = 0.5
	drawdiag = True
	def axiom(self):
		self.app.drawLines(polygonpts(5))
		return [  polygonpts(5) ]
	def rule(self, parray):
		p = parray[0] #Only one active site
		w = self.w
		starpts = [ midw(p[0], p[1],w),midw(p[1], p[2],w),midw(p[2], p[3],w),midw(p[3], p[4],w), midw(p[4], p[0],w) ]
		if self.drawdiag: pass
		self.app.drawLines([ starpts[0],starpts[2], starpts[4], starpts[1], starpts[3]] ) #draw diagonals
		newpts = []
		def addpt(d1, d2):
			angle = getangle(d2, d1)
			l = distance(d1,d2) / (1.61803399 + .5)
			newpts.append(  extendpt(mid(d1,d2), angle, l/4.)   )
		addpt( starpts[0], starpts[2])
		addpt( starpts[1], starpts[3])
		addpt( starpts[2], starpts[4])
		addpt( starpts[3], starpts[0])
		addpt( starpts[4], starpts[1])
		if self.drawdiag: self.app.drawLines(newpts)
		return [ newpts ]
#$example$
#$ sqq = PentaStar(app)
#$ sqq.drawdiag = False
#$ sqq.w = 0.55 #0.45
#$ sqq.draw(5)

class SerpTriangle(PaintShape):
	def axiom(self):
		tri = polygonpts(3)
		self.app.drawLines(tri)
		return [tri]
		
	def rule(self, parray):
		# There are 3^n active sites
		arrayActive = []
		for p in parray:
			p05 = mid(p[0], p[1])
			p15 = mid(p[1],p[2])
			p25 = mid(p[2],p[0])
			self.app.drawLines( [p05, p15, p25] )
			arrayActive.append( [p[0], p05, p25] )
			arrayActive.append( [p05, p[1], p15] )
			arrayActive.append( [p25, p15, p[2]] )
			
		return arrayActive
#$example$
#$sp = SerpTriangle(app)
#$sp.draw(5)


class InnerTriangles(PaintShape):
	def axiom(self):
		tri = [[-1.,0.], [0.,0.],[0.,1.]]
		self.app.drawLines(tri)
		return [tri]
		
	def rule(self, parray):
		p = parray[0]
		corner = midw(p[0], p[2], 0.5)
		rcorner = midw(p[1], p[2], 0.5)
		self.app.drawLines([p[1], corner, rcorner])
		return [[corner, rcorner, p[2]]]
#$example$
#$ t = CurrentShape(app)
#$ tri1 = [[-1.,0.], [0.,0.],[0.,1.]]
#$ tri2 = [[1.,0.], [0.,0.],[0.,-1.]]
#$ tri3 = [[-1,0.], [0.,0.],[0,-1]]
#$ tri4 = [[1,0], [0.,0.],[0,1]]
#$ for tri in [tri1, tri2, tri3, tri4]:
	#$ app.drawLines(tri)
	#$ t.draw(8, [tri])

class RingCircles(PaintShape):
	def axiom(self):
		c =  [[0.,0.], 0.5] #center, radius
		return [c]
	def rule(self, parray):
		nextSites = []
		for c in parray:
			self.app.drawCircle( c[0], c[1] )
			#Shrink the radius, draw on all corners
			radius = c[1]/self.scale
			nextSites.append( [[c[0][0]-c[1],c[0][1]], radius])
			nextSites.append( [[c[0][0]+c[1],c[0][1]], radius])
			nextSites.append( [[c[0][0],c[0][1]-c[1]], radius])
			nextSites.append( [[c[0][0],c[0][1]+c[1]], radius])
		return nextSites
#$example$
#$ t = RingCircles(app)
#$ t.scale = 2.5
#$ t.draw(5)
#$ #t.animate(5,'scale',0.25,2.5,300,40)#Sweet!!!!!

class BigCircles(PaintShape):
	def axiom(self):
		#Note that each active site is just one point.
		#~ self.app.drawCircle([0.,0.],0.25)
		#return [[[0.25,0]],[[0,0.25]],[[-0.25,0.]],[[0,-0.25]]]
		return [ [pt] for pt in polygonpts(t.order, 0.25) ]
	def rule(self, parray):
		nextSites = [] #However, the number of sites will not change.
		for p in parray:
			r = distance([0.,0.], p[0])
			self.app.drawCircle(p[0], r)
			nextSites.append([ extendpt(p[0], getangle([0.,0.],p[0]), r) ]) #Extend away from the origin.
		return nextSites
#$example$
#$ t = BigCircles(app)
#$ t.order = 7
#$ t.draw(3)
class Stars(PaintShape):
	def axiom(self):
		c =  [[0.,0.], 0.5] #center, radius
		return [c]
	def rule(self, parray):
		nextSites = []
		for c in parray:
			# Draw inverted circle
			curve = self.curve
			#Note: looks cool with just these first lines here, too, and recurse at 6, rad=c[1]/2., w = 1/math.sqrt(2.)
			self.app.circularLine(   [c[0][0]-c[1],c[0][1]], [c[0][0],c[0][1]-c[1]], curve)
			self.app.circularLine(   [c[0][0]+c[1],c[0][1]], [c[0][0],c[0][1]+c[1]], curve)
			self.app.circularLine(   [c[0][0],c[0][1]-c[1]], [c[0][0]+c[1],c[0][1]], curve)
			self.app.circularLine(   [c[0][0],c[0][1]+c[1]], [c[0][0]-c[1],c[0][1]], curve)
			radius = c[1]/self.radiusM
			w = 1/math.sqrt(2.) +.2 #pull towards a corner of the square
			nextSites.append( [midw( c[0], [c[0][0]+c[1],c[0][1]+c[1]] ,w) , radius] )
			nextSites.append( [midw( c[0], [c[0][0]+c[1],c[0][1]-c[1]] ,w) , radius] )
			nextSites.append( [midw( c[0], [c[0][0]-c[1],c[0][1]-c[1]] ,w) , radius] )
			nextSites.append( [midw( c[0], [c[0][0]-c[1],c[0][1]+c[1]] ,w) , radius] )
		return nextSites
#$example$	
#$ t = Stars(app)
#$ t.curve = 0.5
#$ t.radiusM = 3.
#$ t.draw(4)
#$ #t.animate(4, 'curve',0.1,2.41,30,40) #Cool

class Tree(PaintShape):
	def axiom(self):
		c = [[0.,-1.], twopi/4., 0.5] #In format point, direction, length
		return [c]
	def rule(self, parray):
		nextSites = []
		for c in parray:
			currentPt = c[0]
			nextPt = extendpt(c[0], c[1], c[2])
			self.app.drawLine(currentPt, nextPt)
			lengthScale = 0.5
			theta1 = c[1] + self.theta1
			theta2 = c[1] - self.theta2
			nextSites.append([ nextPt, theta1, c[2]*self.l1])
			nextSites.append([ nextPt, theta2, c[2]*self.l2])
		return nextSites
#$example$
#$ tr = Tree(app)
#$ tr.theta1 = twopi/8.
#$ tr.theta2 = twopi/6.
#$ tr.l1 = 0.5
#$ tr.l2 = 0.5*1.4
#$ tr.draw(12)
class TreeBalanced(PaintShape):
	def axiom(self):
		c = [[0.,-1.], twopi/4., 0.5] #In format point, direction, length
		return [c]
	def rule(self, parray):
		nextSites = []
		for c in parray:
			currentPt = c[0]
			nextPt = extendpt(c[0], c[1], c[2])
			self.app.drawLine(currentPt, nextPt)
			lengthScale = 0.5
			theta1 = c[1] + self.theta
			theta2 = c[1] - self.theta
			nextSites.append([ nextPt, theta1, c[2]*self.l])
			nextSites.append([ nextPt, theta2, c[2]*self.l])
		return nextSites
#$example$
#$ tr = CurrentShape(app)
#$ tr.theta = twopi/4.1
#$ tr.l = 0.7
#$ tr.draw(12)
#$ #tr.animate(8,'theta',twopi/2,0.1,30,40)

class TopTree(PaintShape):
	def axiom(self):
		c = [[[0.,-.5], twopi/4., .2],[[0.,.5], -twopi/4., .2]] #In format point, direction, length
		return c
	def rule(self, parray):
		nextSites = []
		for c in parray:
			currentPt = c[0]
			nextPt = extendpt(c[0], c[1], c[2])
			self.app.drawLine(currentPt, nextPt)
			lengthScale = self.scale
			turn = self.turn
			theta1 = c[1] + turn
			theta2 = c[1] - turn
			pt1 = extendpt(nextPt, theta1, c[2] * 1.5)
			pt2 = extendpt(nextPt, theta2, c[2] * 1.5)
			self.app.drawLine(nextPt, pt1)
			self.app.drawLine(nextPt, pt2)
			nextSites.append([ pt1, theta1 + turn/2., c[2]*lengthScale])
			nextSites.append([ pt2, theta2 - turn/2., c[2]*lengthScale])
		return nextSites
#$example$
#$ tr = TopTree(app)
#$ tr.turn = twopi/4.1 #twopi/40
#$ tr.scale = 0.7
#$ tr.draw(9)
#$ #tr.animate(3,'turn',0,twopi-.1,300,4) #order 5 is cool too
#$ ##tr.scale=0.9 watch the cpu...
#$ ##tr.animate(9,'turn',twopi/6.,twopi-.1,300,4) 

class Wilted(PaintShape):
	def axiom(self):
		c = [[0.,-1.], twopi/4., 1.] #In format point, direction, length
		return [c]
	def rule(self, parray):
		nextSites = []
		for c in parray:
			turn = twopi/8.
			#Unfinished
		return nextSites
#$example$

class SerpTriangleCurved(PaintShape):
	curve = 0.25
	def axiom(self):
		tri = polygonpts(3)
		self.app.drawCurvedLines(tri, self.curve)
		return [tri]
		
	def rule(self, parray):
		# There are 3^n active sites
		arrayActive = []
		curve = self.curve
		center = [0., 0.]
		for p in parray:
			p05 = mid(p[0], p[1])
			p15 = mid(p[1],p[2])
			p25 = mid(p[2],p[0])
			
			self.app.drawCurvedLines( [p05, p15, p25] , curve)
			
			# New points are towards the center more
			w = self.w
			p05 = midw(p05, center, w)
			p15 = midw(p15, center, w)
			p25 = midw(p25, center, w)

			arrayActive.append( [p[0], p05, p25] )
			arrayActive.append( [p05, p[1], p15] )
			arrayActive.append( [p25, p15, p[2]] )
		return arrayActive
#$example$
#$sp = SerpTriangleCurved(app)
#$sp.curve = 0.7 # 0.25
#$sp.w = 0.3 #0.41
#$sp.draw(4)
#$ #sp.animate(4, 'w',0.01,4.,300,4)
#$ #sp.animate(4, 'w',-6.91,4.,500,4)
#$ #sp.animate(5, 'w',-1.91,0.9,100,40)

class TriCurved(PaintShape):
	curve = 0.4
	def axiom(self):
		tri = polygonpts(3)
		self.app.drawCurvedLines(tri, self.curve)
		return [tri]
		
	def rule(self, parray):
		p = parray[0] # There is 1 active site
		curve = self.curve
		center = [0., 0.]
		
		p05 = mid(p[0], p[1])
		p15 = mid(p[1],p[2])
		p25 = mid(p[2],p[0])
		
		# New points are towards the center more
		w = 0.01
		p05 = midw(p05, center, w)
		p15 = midw(p15, center, w)
		p25 = midw(p25, center, w)
		
		self.app.drawCurvedLines( [p05, p15, p25] , curve, 1, False)

		return [ [p05, p15, p25]]
#$example$
#$ sp = TriCurved(app)
#$ sp.curve = 0.4
#$ sp.draw(4)
#$ #sp.animate(4,'curve',.01,1.3,300,4)

class KochCurve(RecShape):
	def axiom(self):
		return [[[-1,0],[1,0]]]
	def rule(self, parray):
		# There are 4^n active sites
		# We do not draw lines in the rule, but just return more active sites
		arrayActive = []
		
		for p in parray:
			mid1 = midw(p[0], p[1], 1./3.)
			mid2 = midw(p[0], p[1], 2./3.)
			angle = getangle( p[0], p[1] ) + (-60./360.)*twopi #60 degrees up from where we are now.
			mid15 = extendpt( mid1, angle, distance(p[0], mid1))
			arrayActive.append( [p[0], mid1] )
			arrayActive.append( [mid1, mid15] )
			arrayActive.append( [mid15, mid2] )
			arrayActive.append( [mid2, p[1]] )
			
		return arrayActive
#$example$
#$ kch = KochCurve(app)
#$ kch.draw(5)

class KochTri(KochCurve):
	def axiom(self):
		tri = polygonpts(3)
		return [ [tri[0],tri[1]],[tri[1],tri[2]], [tri[2],tri[0]]]
#$example$
#$ kch = KochTri(app)
#$ kch.draw(5)

class CantorSet(RecShape):
	def axiom(self):
		return [[[-1,0],[1,0]]]
	def rule(self, parray):
		arrayActive = []
		for p in parray:
			mid1 = midw(p[0], p[1], 1./3.)
			mid2 = midw(p[0], p[1], 2./3.)
			arrayActive.append([p[0], mid1])
			arrayActive.append([mid2, p[1]])
		return arrayActive	
#$example$
#$ a = CantorSet(app)
#$ a.draw(4)

class BoxyCurve(RecShape):
	angle = twopi/4.
	def axiom(self):
		return [[[-1,0],[1,0]]]
	def rule(self, parray):
		arrayActive = []
		for p in parray:
			angle = self.angle
			mid1 = midw(p[0], p[1], 1./3.)
			mid2 = midw(p[0], p[1], 2./3.)
			l = distance(p[0], mid1)
			mid1up = extendpt(mid1, getangle(p[0], mid1) + angle, l)
			mid2up = extendpt(mid2, getangle(p[0], mid1) + angle, l)
			self.app.drawLine(mid1, mid1up) #These lines aren't active sites, so they'll have to be drawn
			self.app.drawLine(mid2, mid2up)
			arrayActive.append([p[0], mid1])
			arrayActive.append([mid2, p[1]])
			arrayActive.append([mid1up, mid2up])
		return arrayActive
#$example$
#$ b = BoxyCurve(app)
#$ b.angle = twopi/4.
#$ b.draw(4)


class Boxes(PaintShape):
	angle= twopi/4.
	def axiom(self):
		return [[[-1,0],[1,0]]     ,  [[0,-1],[0,1]] ]
	def rule(self, parray):
		arrayActive = []
		angle = self.angle
		for p in parray:
			mid1 = midw(p[0], p[1], 1./3.)
			mid2 = midw(p[0], p[1], 2./3.)
			l = distance(p[0], mid1)
			mid1up = extendpt(mid1, getangle(p[0], mid1) + angle, l/2.)
			mid2up = extendpt(mid2, getangle(p[0], mid1) + angle, l/2.)
			mid1down = extendpt(mid1, getangle(p[0], mid1) + angle, -l/2.)
			mid2down = extendpt(mid2, getangle(p[0], mid1) + angle, -l/2.)
			
			self.app.drawLines([mid1down, mid1up, mid2up, mid2down])
			
			arrayActive.append([p[0], mid1])
			arrayActive.append([mid2, p[1]])
		return arrayActive	
#$example$
#$ b = Boxes(app)
#$ b.angle = twopi/4. #twopi/6.
#$ b.draw(4)
#$ #b.animate(4,'angle', twopi/4., -twopi/6.,300,4)

class TheCircleSpiral(PaintShape):
	def axiom(self):
		return [[[-1,0],[1,0]]]
	def rule(self, parray):
		p = parray[0]
		self.app.circularLine(p[1], addpt( p[0], [0,.00001]), 0.8)
		return [[ p[1], mid(p[0], p[1]) ]] #Note that reversing the order, flips the circle
#$example$
#$ sp = TheCircleSpiral(app)
#$ sp.draw(8, [[[-1,0],[1,0]]])
#$ sp.draw(8, [[[1,0],[-1,0]]])
#$ sp.draw(8, [[[0,-1],[0,1]]])
#$ sp.draw(8, [[[0,1],[0,-1]]])

class TheNestedCircles(PaintShape):
	def axiom(self):
		return [[[-1.333333,0],[0.666666,0]]]
	def rule(self, parray):
		p = parray[0]
		self.app.circularLine(p[1], p[0], 1., True) #Changing the parameter here won't change curvature.
		return [[ p[1], mid(p[0], p[1]) ]]
#$example$
#$ sp = TheNestedCircles(app)
#$ sp.draw(18)
#$ # and now zoom in...

class SSquare(PaintShape):
	def axiom(self):
		return [[[-1,-1],2]]
	def rule(self, parray):
		nextsites = []
		for p in parray:
			l = p[1]/3.
			start = p[0]
			for x in (0,1,2):
				for y in (0,1,2):
					corner = addpt(start, [l*x, l*y])
					if x==1 and y==1:
						app.drawLines([corner, addpt(corner,[l,0]),addpt(corner,[l,l]),addpt(corner,[0,l])])
					else:
						nextsites.append([corner,l])
		return nextsites
#$example$
#$ sp = SSquare(app)
#$ sp.draw(3)
class SSquareSharp(PaintShape):
	def axiom(self):
		return [[[-1,-1],2]]
	def rule(self, parray):
		nextsites = []
		for p in parray:
			l = p[1]/3.
			start = p[0]
			app.drawLine(addpt(start, [l*1, l*0]),addpt(start, [l*1, l*3]))
			app.drawLine(addpt(start, [l*2, l*0]),addpt(start, [l*2, l*3]))
			app.drawLine(addpt(start, [l*0, l*1]),addpt(start, [l*3, l*1]))
			app.drawLine(addpt(start, [l*0, l*2]),addpt(start, [l*3, l*2]))
			for x in (0,2):
				for y in (0,2):
					corner = addpt(start, [l*x, l*y])
					nextsites.append([corner,l])
		return nextsites
#$example$
#$ sp = SSquareSharp(app)
#$ sp.draw(5)
class Ruler(PaintShape):
	def axiom(self):
		line = [[-1,0],[1,0]]
		app.drawLine(line[0], line[1])
		return [line]
	def rule(self, parray):
		nextsites=[]
		for p in parray:
			angle = getangle(p[0], p[1])+twopi/4.
			l = distance(p[0], p[1])/2.
			midpt = midw(p[0], p[1],0.5)
			app.drawLine(midpt, extendpt(midpt,angle,l))
			if True: #Flip
				nextsites.append([p[0], midpt])
				nextsites.append([p[1], midpt])
			else:
				nextsites.append([p[0], midpt])
				nextsites.append([midpt, p[1]])
		return nextsites
#$example$
#$ sp = Ruler(app)
#$ sp.draw(8)
class Parens(PaintShape):
	curve = 0.2
	def axiom(self):
		line = [[-1,0],[1,0]]
		app.drawLine(line[0], line[1])
		return [line]
	def rule(self, parray):
		nextsites=[]
		for p in parray:
			mid1 = midw(p[0], p[1], 2/5.)
			mid2 = midw(p[0], p[1], 3/5.)
			l = (distance(p[0], p[1]) * 3/5.) / 2.
			angle = getangle(p[0], p[1])
			app.circularLine( extendpt(mid1, angle+twopi/4.,l), extendpt(mid1, angle-twopi/4.,l),self.curve)
			app.circularLine( extendpt(mid2, angle-twopi/4.,l), extendpt(mid2, angle+twopi/4.,l),self.curve)
			nextsites.append([p[0], mid1])
			nextsites.append([mid2, p[1]])
		return nextsites
#$example$
#$ sp = Parens(app)
#$ sp.curve=0.2 #1.4
#$ sp.draw(5)