import Image

import ImageChops; import ImageEnhance; import ImageFilter; import ImageOps
import ImageDraw; import ImageStat 

# Turns input into code that can be run
def parseInput(s):
	s = s.replace('\r\n','\n').replace('\r','\n').replace('\t','    ')
	slines = s.split('\n')
	constructs = ['loop:','map:','loop_hsl:','loop_hsv:']
	for line in slines:
		if line.strip() == constructs and line != line.strip():
			print 'Syntax error: This construct',line.strip(),' cannot occur inside a block.'
			return None
	
	for construct in constructs:
		nLoops = slines.count(construct)
		if nLoops>1:
			print 'At this time, only one construct of type ',construct,'is supported.'
			return None
		elif nLoops==1:
			startLoop, endLoop = pullConstruct(slines, construct)
			
			if construct=='loop:':
				for j in range(startLoop+1, endLoop+1):
					slines[j] = '    '+slines[j]
				loopPrefix, loopSuffix = createLoop(slines[startLoop+1:endLoop+1])
			elif construct=='map:':
				loopPrefix, loopSuffix = createMap(slines[startLoop+1:endLoop+1])
			elif construct=='loop_hsl:' or construct=='loop_hsv:':
				for j in range(startLoop+1, endLoop+1):
					slines[j] = '    '+slines[j]
				loopPrefix, loopSuffix = createLoopHue(construct, slines[startLoop+1:endLoop+1])
				
			slines[startLoop] = loopPrefix
			slines[endLoop] += '\n' + loopSuffix
			return '\n'.join(slines)
		
	return '\n'.join(slines) # no loops found
	
def pullConstruct(slines, construct):
	startLoop = None
	for i in range(len(slines)):
		if slines[i]==construct:
			startLoop = i
			endLoop = None
			i+=1
			while i<len(slines):
				if (not (slines[i].startswith('\t') or slines[i].startswith('  '))) and slines[i].strip()!='':
					endLoop = i-1
					break
				i+=1
			if endLoop==None: endLoop = len(slines)-1
			break
			
	assert startLoop != None and endLoop!=None
	return startLoop, endLoop
	
def getTokens(astrInterested, s):
	occurred = {}
	
	# look at all of the characters in here.
	import StringIO, tokenize
	
	tokens = tokenize.generate_tokens(StringIO.StringIO(s).readline)
	for toknum, tokval, _, _, _  in tokens:
		if toknum==1 and tokval in astrInterested:
			occurred[tokval] = True
	
	return occurred

def createLoop(astr):
	# for optimization, we only want to have code for the variables that are actually used
	# So, we will tokenize the input to see which of these are used:
	interested = ['x','y','r','g','b','R','G','B']
	s = ('\n'.join(astr))
	occurred = getTokens(interested, s)
	
	sSuffix = ''
	sPrefix = '''
for y in xrange(height):
    for x in xrange(width):'''

	startLine = '\n'+' '*8
	if 'r' in occurred: sPrefix += startLine+ 'r = ra[x,y]'
	if 'g' in occurred: sPrefix += startLine+ 'g = ga[x,y]'
	if 'b' in occurred: sPrefix += startLine+ 'b = ba[x,y]'
	if 'R' in occurred: sSuffix += startLine+ 'ra[x,y] = max(min(R,255),0)'
	if 'G' in occurred: sSuffix += startLine+ 'ga[x,y] = max(min(G,255),0)'
	if 'B' in occurred: sSuffix += startLine+ 'ba[x,y] = max(min(B,255),0)'
	
	return sPrefix, sSuffix

def createLoopHue(space, astr):
	#http://docs.python.org/lib/module-colorsys.html
	sSuffix = ''
	sPrefix = '''
from colorsys import *
for y in xrange(height):
    for x in xrange(width):'''
    
	occurred = getTokens(['h','s','l','v'], '\n'.join(astr))
	# If nothing was read, then we don't need to convert reading rgb into hsv
	startline = '\n'+' '*8
	if space=='loop_hsv:':
		if occurred: sPrefix += startline+'h,s,v=H,S,V=rgb_to_hsv(ra[x,y]/256.0,ga[x,y]/256.0,ba[x,y]/256.0)'
		sSuffix += startline+'r,g,b = hsv_to_rgb(H%1.0,max(min(S,1.0),0.0),max(min(V,1.0),0.0));ra[x,y]=r*256;ba[x,y]=b*256;ga[x,y]=g*256'
	elif space=='loop_hsl:':
		if occurred: sPrefix += startline+'h,l,s=H,L,S=rgb_to_hls(ra[x,y]/256.0,ga[x,y]/256.0,ba[x,y]/256.0)'
		sSuffix += startline+'r,g,b = hls_to_rgb(H%1.0,max(min(L,1.0),0.0),max(min(S,1.0),0.0));ra[x,y]=r*256;ba[x,y]=b*256;ga[x,y]=g*256'
	return sPrefix, sSuffix


def createMap(astr):
	interested = ['x','y','r','g','b','R','G','B']
	s = ('\n'.join(astr))
	occurred = getTokens(interested, s)
	
	if 'x' in occurred or 'y' in occurred:
		raise 'Maps cannot be depending on x,y position. Use loop: instead.'
	for line in astr:
		if line.startswith('R'):
			if getTokens(['g','b'], line): raise 'Maps cannot depend on other colors. Use loop: instead.'
		elif line.startswith('G'):
			if getTokens(['r','b'], line): raise 'Maps cannot depend on other colors. Use loop: instead.'
		elif line.startswith('B'):
			if getTokens(['r','g'], line): raise 'Maps cannot depend on other colors. Use loop: instead.'
	
	sSuffix = ''
	sPrefix = '''
rmap=range(256)
gmap=range(256)
bmap=range(256)
for iii in xrange(256): 
    r,g,b=iii,iii,iii'''

	startline = '\n'+' '*4
	if 'R' in occurred: sSuffix+=startline+'rmap[iii] = max(min(R,255),0)'
	if 'G' in occurred: sSuffix+=startline+'gmap[iii] = max(min(G,255),0)'
	if 'B' in occurred: sSuffix+=startline+'bmap[iii] = max(min(B,255),0)'
	sSuffix+='''
table = rmap
table.extend(gmap)
table.extend(bmap)
imgOutput = imgInput.point(table)'''
	return sPrefix, sSuffix


def runCode(source, imgInput):
	if not source:
		print 'No source given.'
		return None
	rim, gim, bim = imgInput.split()
	ra, ga, ba = rim.load(), gim.load(), bim.load()
	
	# expose information
	dlocals = {}
	dlocals['imgInput'] = imgInput
	dlocals['width'] , dlocals['height'] = imgInput.size[0], imgInput.size[1]	
	dlocals['ra'],dlocals['ga'],dlocals['ba'] = ra, ga, ba
	dlocals['rim'],dlocals['gim'],dlocals['bim'] = rim, gim, bim # I don't expect this to be used often
	
	#expose modules
	dlocals['Image'] = Image; dlocals['ImageChops'] = ImageChops; dlocals['ImageEnhance'] = ImageEnhance; dlocals['ImageFilter'] = ImageFilter;  dlocals['ImageOps'] = ImageOps;
	if 'ImageDraw' in source:
		dlocals['ImageDraw'] = ImageDraw
	if 'ImageStat' in source:
		dlocals['ImageStat'] = ImageStat
	
	#run script
	code = compile(source, '<user-provided code>', 'exec')
	exec code in dlocals
	
	#recreate image
	if 'imgOutput' in dlocals:
		imgOutput = dlocals['imgOutput'] #evidently it was created manually
	else:
		imgOutput = Image.merge("RGB", (rim, gim, bim))
	return imgOutput

def runScript(s, imgInput):
	
	s = parseInput(s)
	if s==None: return None
		
	#~ import sys; print >>sys.stderr, s
	return runCode(s, imgInput)

if __name__=='__main__':
	test = '''
a=4
loop:
	R=r+35
	G=g+25
	B=6
	a=x
a=6
	'''
	
	test2='''
#Increase red by 40
loop:
	R=r+40
print "this is Python code!"
'''

	test3='''imgOutput = Image.new('RGB', (int(x),int(y)))'''

	test4='''
a=4
map:
	R=r+40
a=6'''
	test5='''
a=4
loop_hsv:
	H=h+0.2
a=6'''
	
	
	print parseInput(test5)
	