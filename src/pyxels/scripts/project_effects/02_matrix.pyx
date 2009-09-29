scale = None
offset = 0

def main():
	global imgOutput
	edge_detect() #choose a function to call here, which will set m and possibly scale/offset
	imgOutput = imgInput.filter(ImageFilter.Kernel((3,3),m,scale=scale, offset=offset))


def gaussian_blur():
	global m
	m = (
		1,2,1,
		2,4,2,
		1,2,1,
	)

def sharpen():
	global m
	m = (
		0,-2,0,
		-2,11,-2,
		0,-2,0
	)

def edge_detect():
	global m, scale, offset
	m = (
		1,1,1,
		0,0,0,
		-1,-1,-1
	)
	scale = 1; offset = 127

def emboss():
	global m, scale, offset
	m = (
		-1,0,-1,
		0,4,0,
		-1,0,-1
	)
	scale = 1
	offset = 127


def embossHV():
	global m, scale, offset
	m = (
		0,-1,0,
		-1,4,-1,
		0,-1,0
	)
	scale = 1; offset = 127



main()