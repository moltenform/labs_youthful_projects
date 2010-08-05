# Histogram of red values
hist = rim.histogram()

# normalize
maxvalue = max(hist[2:-2]) + 1.0001
hist = [int(256*v/maxvalue) for v in hist]

# now draw it graphically!
imgOutput = Image.new('RGB',(256,256),(255,255,255))
draw = ImageDraw.Draw(imgOutput)
for i in range(256):
	draw.line([(i,256),(i,256-hist[i])], fill=(0,0,0))
