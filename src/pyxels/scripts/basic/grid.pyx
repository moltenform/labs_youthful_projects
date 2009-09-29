gridx = 16
gridy = 16
loop:
	if x%gridx==0 or y%gridy==0:
		R=G=B = 0
	else:
		R=G=B = 255