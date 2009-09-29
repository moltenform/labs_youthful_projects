# how many pixels are close to pure red?
targetColor = (255,0,0)
c = 0
threshold = 4
loop:
	if abs(r-targetColor[0])<threshold and abs(g-targetColor[1])<threshold and abs(b-targetColor[2])<threshold:
		c+=1
	
print 'Answer:',c